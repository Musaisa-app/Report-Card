# Phase 3: Wallet System - Implementation Guide

## Overview

Phase 3 implements a complete wallet system with:
- Automatic wallet creation on user registration
- Balance management and tracking
- Transaction history with pagination
- Daily and monthly spending limits
- Hold amount for pending transactions
- Wallet dashboard with analytics
- Transaction verification

## Architecture

### Services

1. **IWalletService** - Main wallet operations
2. **IWalletRepository** - Wallet data access
3. **IWalletTransactionRepository** - Transaction data access

### API Endpoints

```
GET    /api/wallet/balance                 - Get wallet balance
GET    /api/wallet/available-balance       - Get available balance
GET    /api/wallet/dashboard               - Get wallet dashboard
GET    /api/wallet/transactions            - Get transaction history (paginated)
GET    /api/wallet/transactions/{reference}- Get transaction by reference
POST   /api/wallet/fund                    - Fund wallet
POST   /api/wallet/verify-balance         - Verify balance sufficiency
```

## Key Features

### Balance Management
- **Balance**: Total wallet balance
- **Hold Amount**: Amount reserved for pending transactions
- **Available Balance**: Balance - Hold Amount
- **Currency**: Defaults to NGN (Nigerian Naira)

### Spending Limits
- **Daily Limit**: Default ₦1,000,000 per day
- **Monthly Limit**: Default ₦50,000,000 per month
- **Daily Spent**: Tracked daily
- **Monthly Spent**: Reset monthly

### Transaction Types
- Deposit
- Airtime
- Data
- Cable
- Electricity
- Refund
- Commission
- Withdrawal

### Transaction Statuses
- Pending
- Success
- Failed
- Reversed

## Service Methods

### GetWalletAsync(userId)
Retrieve wallet information for a user.

```csharp
var wallet = await walletService.GetWalletAsync(userId);
// Returns: WalletDto with balance, available balance, limits
```

### FundWalletAsync(userId, amount, transactionType)
Add funds to wallet and create transaction record.

```csharp
var success = await walletService.FundWalletAsync(userId, 50000, "Deposit_Paystack");
```

### DebitWalletAsync(userId, amount, transactionType, description)
Deduct funds from wallet with validation.

```csharp
var success = await walletService.DebitWalletAsync(
    userId, 
    1000, 
    "Airtime", 
    "MTN Airtime - 1000 to 08012345678"
);
```

### HoldAmountAsync(userId, amount)
Reserve amount for pending transaction.

```csharp
var success = await walletService.HoldAmountAsync(userId, 5000);
```

### ReleaseHoldAsync(userId, amount)
Release held amount (when transaction fails).

```csharp
var success = await walletService.ReleaseHoldAsync(userId, 5000);
```

### GetTransactionHistoryAsync(userId, pageNumber, pageSize)
Retrieve paginated transaction history.

```csharp
var history = await walletService.GetTransactionHistoryAsync(userId, 1, 10);
// Returns: 10 most recent transactions with total count
```

### GetDashboardSummaryAsync(userId)
Get wallet analytics for dashboard.

```csharp
var dashboard = await walletService.GetDashboardSummaryAsync(userId);
// Returns:
// - Balance
// - Available Balance
// - Total Purchases
// - Total Spending
// - Referral Earnings
```

### IsWithinDailyLimitAsync(userId, amount)
Check if transaction is within daily limit.

```csharp
var isAllowed = await walletService.IsWithinDailyLimitAsync(userId, 100000);
```

### IsWithinMonthlyLimitAsync(userId, amount)
Check if transaction is within monthly limit.

```csharp
var isAllowed = await walletService.IsWithinMonthlyLimitAsync(userId, 1000000);
```

### HasSufficientBalanceAsync(userId, amount)
Verify available balance.

```csharp
var hasSufficient = await walletService.HasSufficientBalanceAsync(userId, 5000);
```

## Database Schema

### wallets table
```sql
wallet_id (UUID, PK)
user_id (UUID, FK, UNIQUE)
balance (DECIMAL)
hold_amount (DECIMAL)
available_balance (DECIMAL)
daily_limit (DECIMAL)
monthly_limit (DECIMAL)
daily_spent (DECIMAL)
monthly_spent (DECIMAL)
currency (VARCHAR)
created_date (TIMESTAMP)
updated_date (TIMESTAMP)
```

### wallet_transactions table
```sql
transaction_id (UUID, PK)
user_id (UUID, FK)
wallet_id (UUID, FK)
reference_number (VARCHAR, UNIQUE)
transaction_type (VARCHAR)
amount (DECIMAL)
previous_balance (DECIMAL)
new_balance (DECIMAL)
status (VARCHAR)
description (TEXT)
metadata (JSONB)
created_date (TIMESTAMP)
updated_date (TIMESTAMP)
```

## Transaction Flow

### Funding Wallet
1. User initiates wallet funding
2. Redirect to payment gateway (Paystack/Flutterwave)
3. Payment gateway processes payment
4. Webhook received from gateway
5. Wallet funded with transaction record
6. User notified

### Making Purchase
1. User initiates purchase (e.g., airtime)
2. Check wallet balance sufficiency
3. Check daily limit
4. Check monthly limit
5. Hold amount in wallet
6. Process VTU API request
7. If success: Debit wallet, release hold, create transaction
8. If failed: Release hold, notify user

## Frontend Integration

### Wallet Service
```typescript
// Get wallet balance
const wallet = await walletService.getWallet();

// Get available balance
const available = await walletService.getAvailableBalance();

// Get dashboard
const dashboard = await walletService.getDashboard();

// Get transaction history
const history = await walletService.getTransactionHistory(1, 10);

// Fund wallet
const result = await walletService.fundWallet(10000, 'Paystack');

// Verify balance before transaction
const verified = await walletService.verifyBalance(5000);
```

### Wallet Store (Zustand)
```typescript
const { wallet, dashboard, isLoading, error } = useWalletStore();

// Refresh wallet data
await useWalletStore.getState().refreshWallet();

// Refresh dashboard
await useWalletStore.getState().refreshDashboard();

// Fund wallet
await useWalletStore.getState().fundWallet(10000, 'Paystack');

// Verify balance
const canProceed = await useWalletStore.getState().verifyBalance(5000);
```

## Testing Checklist

- [ ] Wallet created automatically on registration
- [ ] Balance displays correctly
- [ ] Available balance calculated correctly
- [ ] Fund wallet increases balance
- [ ] Debit wallet decreases balance
- [ ] Daily limit enforced
- [ ] Monthly limit enforced
- [ ] Hold amount works
- [ ] Transaction history paginated
- [ ] Dashboard shows correct totals
- [ ] Transaction reference generated
- [ ] Transaction status updates

## Error Handling

```csharp
// Insufficient balance
if (!await walletService.HasSufficientBalanceAsync(userId, amount))
    return BadRequest("Insufficient balance");

// Daily limit exceeded
if (!await walletService.IsWithinDailyLimitAsync(userId, amount))
    return BadRequest("Daily limit exceeded");

// Monthly limit exceeded
if (!await walletService.IsWithinMonthlyLimitAsync(userId, amount))
    return BadRequest("Monthly limit exceeded");
```

## Next Steps

- Phase 4: Payment Integration (Paystack, Flutterwave)
- Phase 5: VTU API Integration
- Phase 6: Profit Engine
