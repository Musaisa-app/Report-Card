# Phase 2: Authentication System - Implementation Guide

## Overview

Phase 2 implements a production-grade authentication system with:
- User registration with validation
- Email and phone login
- JWT token generation and refresh
- Email verification
- Password reset flow
- Account lockout protection
- Comprehensive security measures

## Architecture

### Entities

1. **User** - Main user entity with authentication fields
2. **RefreshToken** - Secure token rotation
3. **EmailVerificationToken** - Email verification flow
4. **PasswordResetToken** - Password reset security

### Services

1. **ITokenService** - JWT token generation and validation
2. **IAuthenticationService** - Business logic for auth operations
3. **IEmailService** - Email notifications

### API Endpoints

```
POST   /api/auth/register                 - Register new user
POST   /api/auth/login                    - Login user
POST   /api/auth/refresh-token            - Refresh access token
POST   /api/auth/verify-email             - Verify email address
POST   /api/auth/resend-verification-email - Resend verification email
POST   /api/auth/request-password-reset   - Request password reset
POST   /api/auth/reset-password           - Reset password
POST   /api/auth/change-password          - Change password (authenticated)
POST   /api/auth/logout                   - Logout (revoke refresh token)
GET    /api/auth/health                   - Health check
```

## Security Features

### Password Security
- BCrypt hashing (ASP.NET Core Identity)
- Minimum 8 characters
- Requires uppercase, lowercase, number, special character
- Never stored in plain text

### Token Security
- JWT tokens with 1-hour expiration
- Refresh tokens with 7-day rotation
- Token revocation on logout
- IP address and user agent tracking

### Account Protection
- Account lockout after 5 failed attempts
- 30-minute lockout duration
- Email notifications on suspicious activity
- Login tracking

### Email Verification
- Email verification required for account
- 24-hour token expiration
- Resend capability
- Verification link via email

### Password Reset
- Secure token generation
- 1-hour token expiration
- Email verification required
- Password changed notification

## Database Schema

### users table
```sql
user_id (UUID, PK)
full_name (VARCHAR)
username (VARCHAR, UNIQUE)
email (VARCHAR, UNIQUE)
phone_number (VARCHAR, UNIQUE)
password_hash (VARCHAR)
role_id (INT, FK)
referral_code (VARCHAR, UNIQUE)
is_email_verified (BOOLEAN)
is_phone_verified (BOOLEAN)
status (VARCHAR)
account_locked (BOOLEAN)
failed_login_attempts (INT)
last_login (TIMESTAMP)
created_date (TIMESTAMP)
updated_date (TIMESTAMP)
```

### refresh_tokens table
```sql
refresh_token_id (UUID, PK)
user_id (UUID, FK)
token (VARCHAR, UNIQUE)
expiry_date (TIMESTAMP)
is_revoked (BOOLEAN)
created_date (TIMESTAMP)
revoked_date (TIMESTAMP)
ip_address (VARCHAR)
user_agent (TEXT)
```

### email_verification_tokens table
```sql
token_id (UUID, PK)
user_id (UUID, FK)
token (VARCHAR, UNIQUE)
expiry_date (TIMESTAMP)
is_used (BOOLEAN)
created_date (TIMESTAMP)
used_date (TIMESTAMP)
```

### password_reset_tokens table
```sql
token_id (UUID, PK)
user_id (UUID, FK)
token (VARCHAR, UNIQUE)
expiry_date (TIMESTAMP)
is_used (BOOLEAN)
created_date (TIMESTAMP)
used_date (TIMESTAMP)
```

## Implementation Details

### User Registration Flow

1. Validate input (email format, password strength, etc.)
2. Check if user already exists
3. Generate unique referral code
4. Hash password using BCrypt
5. Create user record
6. Create wallet for user
7. Handle referral if provided
8. Generate email verification token
9. Send verification email
10. Return success response

### Login Flow

1. Find user by email or phone
2. Check if account is locked
3. Verify password
4. On failure: increment failed attempts, lock if needed
5. On success: reset failed attempts
6. Generate JWT access token
7. Generate and store refresh token
8. Send login notification email
9. Return tokens

### Token Refresh Flow

1. Validate refresh token exists and not revoked
2. Check expiration date
3. Generate new access token
4. Generate new refresh token
5. Revoke old refresh token
6. Return new tokens

### Password Reset Flow

1. Receive email
2. Find user by email
3. Generate reset token
4. Set expiration to 1 hour
5. Send reset link via email
6. User clicks link and enters new password
7. Validate token and expiration
8. Hash new password
9. Update user record
10. Mark token as used
11. Send confirmation email

## Frontend Integration

### Store (Zustand)
- Manages auth state
- User data
- Loading and error states
- Login/register/logout actions

### API Service
- Axios client with JWT interceptors
- Auto token refresh
- Auto logout on 401

### Components
- Login form
- Register form
- Email verification
- Password reset
- Protected routes

## Testing Checklist

- [ ] User can register
- [ ] Email verification works
- [ ] User can login with email
- [ ] User can login with phone
- [ ] Invalid password fails
- [ ] Account lockout after 5 attempts
- [ ] Password reset flow works
- [ ] Token refresh works
- [ ] Logout revokes token
- [ ] Protected endpoints require auth
- [ ] Referral code tracking works
- [ ] Wallet created automatically

## Configuration

### appsettings.json
```json
{
  "JwtSettings": {
    "Secret": "your-32-char-secret-key-here",
    "ExpiryInMinutes": 60,
    "RefreshTokenExpiryInDays": 7
  }
}
```

### Environment Variables
```
JWT_SECRET=your-secret-key
JWT_EXPIRY_MINUTES=60
REFRESH_TOKEN_EXPIRY_DAYS=7
```

## Next Steps

- Phase 3: Wallet System
- Phase 4: Payment Integration
- Phase 5: VTU API Integration
