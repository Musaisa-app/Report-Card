# Abu-Ashraf Architecture Documentation

## System Overview

Abu-Ashraf is built using Clean Architecture with clear separation of concerns.

## Architecture Layers

### 1. **Domain Layer** (`AbuAshraf.Domain`)
- Pure business logic and entities
- No dependencies on other layers
- Contains:
  - Entities (User, Wallet, Transaction, VTU, Agent, etc.)
  - Value Objects
  - Business rules

### 2. **Application Layer** (`AbuAshraf.Application`)
- Use cases and business workflows
- Service interfaces
- DTOs (Data Transfer Objects)
- MediatR handlers
- Validators
- Mappers
- Does NOT depend on Infrastructure

### 3. **Infrastructure Layer** (`AbuAshraf.Infrastructure`)
- External service implementations:
  - Payment gateways (Paystack, Flutterwave)
  - VTU providers (VTU.ng, Clubkonnect, Reloadly)
  - Email/SMS services
  - Notification services
- Does NOT depend on Persistence or API

### 4. **Persistence Layer** (`AbuAshraf.Persistence`)
- Database context and configurations
- Repository implementations
- EF Core DbContext
- Database migrations

### 5. **API Layer** (`AbuAshraf.API`)
- ASP.NET Core Web API
- Controllers
- Middleware
- Dependency injection setup

## Key Design Patterns

### Repository Pattern
```
IRepository<T> → GenericRepository<T> → DbContext
```

### Service Layer
```
IAuthenticationService → AuthenticationService → Repository + Infrastructure
```

### CQRS (Optional)
Use MediatR for:
- Commands: State-changing operations
- Queries: Read operations
- Handlers: Business logic

## Data Flow

### User Registration Flow
```
API Controller
    ↓
RegisterUserCommand (MediatR)
    ↓
RegisterUserCommandHandler
    ↓
IAuthenticationService
    ↓
IUserRepository, IWalletRepository
    ↓
PostgreSQL Database
    ↓
Response Email Notification
```

### Purchase Airtime Flow
```
API Controller
    ↓
PurchaseAirtimeCommand
    ↓
PurchaseAirtimeHandler
    ↓
IVtuService (selects provider)
    ↓
VTU Provider API (VTU.ng, Clubkonnect, etc.)
    ↓
IWalletService (deducts amount)
    ↓
IProfitEngine (calculates profit)
    ↓
Database Update
    ↓
Notification Service
```

## Database Schema

See `database/schema.sql` for complete schema.

Key tables:
- `users` - User accounts
- `wallets` - User balances
- `wallet_transactions` - All wallet movements
- `vtu_transactions` - VTU service purchases
- `payment_transactions` - Payment gateway records
- `agents` - Agent records with commission
- `referrals` - Referral tracking
- `data_plans` - Available data bundles
- `daily_profit_summary` - Business analytics

## Security Architecture

### Authentication
- JWT tokens
- Refresh token rotation
- BCrypt password hashing
- Account lockout after failed attempts

### Authorization
- Role-based access control (RBAC)
- Policy-based authorization
- Resource ownership checks

### Data Protection
- Sensitive data encrypted at rest
- TLS/HTTPS in transit
- API key management via Environment variables
- Audit logging for all transactions

## Scalability Considerations

1. **Horizontal Scaling**
   - Stateless API design
   - Database connection pooling
   - Distributed cache (Redis)

2. **Database Optimization**
   - Indexes on frequently queried columns
   - Query optimization
   - Read replicas for reporting

3. **Caching Strategy**
   - Cache data plans (updated daily)
   - Cache user roles
   - Cache exchange rates

4. **Async Processing**
   - Background jobs for notifications
   - Queue payment verifications
   - Scheduled profit calculations

## Deployment

See `DEPLOYMENT.md` for production deployment instructions.

- **API**: Azure App Service / AWS EC2 / DigitalOcean
- **Database**: Azure SQL / AWS RDS / DigitalOcean Managed PostgreSQL
- **Frontend**: Vercel / Netlify
- **CDN**: CloudFlare

## Monitoring & Observability

- Application Insights / CloudWatch
- Structured logging (Serilog)
- Error tracking (Sentry)
- Performance monitoring
- Custom dashboards
