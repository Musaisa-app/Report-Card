# Abu-Ashraf Installation Guide

## Prerequisites

- .NET 8 SDK
- Node.js 18+
- PostgreSQL 14+
- Flutter 3.x (for mobile)
- Git

## Backend Setup

### 1. Database Setup

```bash
# Create PostgreSQL database
psql -U postgres
CREATE DATABASE abu_ashraf;
```

### 2. Install .NET Dependencies

```bash
cd backend
dotnet restore
```

### 3. Configure Environment

Create `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=abu_ashraf;User Id=postgres;Password=your_password;"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-min-32-characters",
    "ExpiryInMinutes": 60
  },
  "PaymentGateways": {
    "Paystack": {
      "SecretKey": "your_paystack_secret_key"
    },
    "Flutterwave": {
      "SecretKey": "your_flutterwave_secret_key"
    }
  }
}
```

### 4. Run Migrations

```bash
dotnet ef database update
```

### 5. Start Backend

```bash
dotnet run
```

API runs on: `https://localhost:5001`

## Frontend Setup

### 1. Install Dependencies

```bash
cd frontend
npm install
```

### 2. Configure Environment

Create `.env.local`:

```
REACT_APP_API_URL=http://localhost:5000
REACT_APP_PAYSTACK_KEY=your_paystack_public_key
```

### 3. Start Development Server

```bash
npm start
```

App runs on: `http://localhost:3000`

## Mobile Setup

### 1. Install Dependencies

```bash
cd mobile
flutter pub get
```

### 2. Configure API URL

Edit `lib/config/api_config.dart`

### 3. Run App

```bash
flutter run
```

## Verification

- [ ] Backend API responds at `/api/health`
- [ ] Database contains all tables
- [ ] Frontend loads without errors
- [ ] Can register a test user
- [ ] Login functionality works
