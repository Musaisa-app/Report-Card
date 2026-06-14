-- Abu-Ashraf Database Schema
-- PostgreSQL Production Schema

-- Drop existing tables (development only)
DROP TABLE IF EXISTS wallet_transactions CASCADE;
DROP TABLE IF EXISTS wallets CASCADE;
DROP TABLE IF EXISTS user_roles CASCADE;
DROP TABLE IF EXISTS roles CASCADE;
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS payment_transactions CASCADE;
DROP TABLE IF EXISTS vtu_transactions CASCADE;
DROP TABLE IF EXISTS data_plans CASCADE;
DROP TABLE IF EXISTS agents CASCADE;
DROP TABLE IF EXISTS referrals CASCADE;
DROP TABLE IF EXISTS notifications CASCADE;
DROP TABLE IF EXISTS audit_logs CASCADE;

-- Roles Table
CREATE TABLE roles (
    role_id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) NOT NULL UNIQUE,
    description TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO roles (role_name, description) VALUES
('SuperAdmin', 'Full system access'),
('Admin', 'Administrative access'),
('Agent', 'Agent with commission structure'),
('Customer', 'Regular customer');

-- Users Table
CREATE TABLE users (
    user_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name VARCHAR(255) NOT NULL,
    username VARCHAR(100) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    phone_number VARCHAR(20) UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    role_id INTEGER NOT NULL REFERENCES roles(role_id),
    referral_code VARCHAR(20) UNIQUE,
    is_email_verified BOOLEAN DEFAULT FALSE,
    is_phone_verified BOOLEAN DEFAULT FALSE,
    status VARCHAR(20) DEFAULT 'Active', -- Active, Suspended, Inactive
    account_locked BOOLEAN DEFAULT FALSE,
    failed_login_attempts INTEGER DEFAULT 0,
    last_login TIMESTAMP,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_phone ON users(phone_number);
CREATE INDEX idx_users_referral ON users(referral_code);

-- Wallets Table
CREATE TABLE wallets (
    wallet_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL UNIQUE REFERENCES users(user_id) ON DELETE CASCADE,
    balance DECIMAL(15, 2) DEFAULT 0.00,
    hold_amount DECIMAL(15, 2) DEFAULT 0.00, -- Amount in pending transactions
    available_balance DECIMAL(15, 2) DEFAULT 0.00, -- balance - hold_amount
    daily_limit DECIMAL(15, 2) DEFAULT 1000000.00,
    monthly_limit DECIMAL(15, 2) DEFAULT 50000000.00,
    daily_spent DECIMAL(15, 2) DEFAULT 0.00,
    monthly_spent DECIMAL(15, 2) DEFAULT 0.00,
    currency VARCHAR(3) DEFAULT 'NGN',
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_wallets_user ON wallets(user_id);

-- Wallet Transactions Table
CREATE TABLE wallet_transactions (
    transaction_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    wallet_id UUID NOT NULL REFERENCES wallets(wallet_id) ON DELETE CASCADE,
    reference_number VARCHAR(100) UNIQUE NOT NULL,
    transaction_type VARCHAR(50) NOT NULL, -- Deposit, Airtime, Data, Cable, Electricity, Refund, Commission, Withdrawal
    amount DECIMAL(15, 2) NOT NULL,
    previous_balance DECIMAL(15, 2),
    new_balance DECIMAL(15, 2),
    status VARCHAR(20) DEFAULT 'Pending', -- Pending, Success, Failed, Reversed
    description TEXT,
    metadata JSONB, -- Store additional data like network, phone number, etc.
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_transactions_user ON wallet_transactions(user_id);
CREATE INDEX idx_transactions_reference ON wallet_transactions(reference_number);
CREATE INDEX idx_transactions_type ON wallet_transactions(transaction_type);
CREATE INDEX idx_transactions_date ON wallet_transactions(created_date);

-- Payment Transactions Table (Gateway Integration)
CREATE TABLE payment_transactions (
    payment_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    wallet_transaction_id UUID REFERENCES wallet_transactions(transaction_id),
    user_id UUID NOT NULL REFERENCES users(user_id),
    gateway VARCHAR(50) NOT NULL, -- Paystack, Flutterwave, Monnify
    gateway_reference VARCHAR(255) UNIQUE NOT NULL,
    amount DECIMAL(15, 2) NOT NULL,
    status VARCHAR(20) DEFAULT 'Pending', -- Pending, Success, Failed
    gateway_response JSONB,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_payments_gateway_ref ON payment_transactions(gateway_reference);
CREATE INDEX idx_payments_user ON payment_transactions(user_id);

-- VTU Transactions Table
CREATE TABLE vtu_transactions (
    vtu_transaction_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    wallet_transaction_id UUID REFERENCES wallet_transactions(transaction_id),
    user_id UUID NOT NULL REFERENCES users(user_id),
    service_type VARCHAR(50) NOT NULL, -- Airtime, Data, Cable, Electricity
    network VARCHAR(50), -- MTN, Airtel, Glo, 9mobile
    phone_number VARCHAR(20),
    amount DECIMAL(15, 2) NOT NULL,
    vtu_provider VARCHAR(100), -- VTU.ng, Clubkonnect, Reloadly
    provider_reference VARCHAR(255),
    status VARCHAR(20) DEFAULT 'Pending',
    api_cost DECIMAL(15, 2),
    selling_price DECIMAL(15, 2),
    profit DECIMAL(15, 2),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_vtu_user ON vtu_transactions(user_id);
CREATE INDEX idx_vtu_type ON vtu_transactions(service_type);
CREATE INDEX idx_vtu_network ON vtu_transactions(network);

-- Data Plans Table
CREATE TABLE data_plans (
    data_plan_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    network VARCHAR(50) NOT NULL, -- MTN, Airtel, Glo, 9mobile
    plan_name VARCHAR(255) NOT NULL,
    data_size VARCHAR(50), -- 1GB, 2GB, etc.
    api_cost DECIMAL(15, 2) NOT NULL,
    admin_selling_price DECIMAL(15, 2) NOT NULL,
    agent_cost DECIMAL(15, 2),
    customer_price DECIMAL(15, 2),
    validity_days INTEGER,
    is_active BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_data_plans_network ON data_plans(network);

-- Agents Table
CREATE TABLE agents (
    agent_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL UNIQUE REFERENCES users(user_id) ON DELETE CASCADE,
    commission_rate DECIMAL(5, 2) DEFAULT 2.5, -- Percentage commission
    total_sales DECIMAL(15, 2) DEFAULT 0.00,
    total_commission DECIMAL(15, 2) DEFAULT 0.00,
    agent_tier VARCHAR(50) DEFAULT 'Bronze', -- Bronze, Silver, Gold, Platinum
    is_active BOOLEAN DEFAULT TRUE,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_agents_user ON agents(user_id);

-- Referrals Table
CREATE TABLE referrals (
    referral_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    referrer_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    referee_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    commission_rate DECIMAL(5, 2) DEFAULT 1.0,
    total_referral_earnings DECIMAL(15, 2) DEFAULT 0.00,
    referral_count INTEGER DEFAULT 0,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(referrer_id, referee_id)
);

CREATE INDEX idx_referrals_referrer ON referrals(referrer_id);

-- Notifications Table
CREATE TABLE notifications (
    notification_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    notification_type VARCHAR(50) NOT NULL, -- TransactionSuccess, TransactionFailed, WalletFunded, CommissionReceived
    title VARCHAR(255) NOT NULL,
    message TEXT NOT NULL,
    is_read BOOLEAN DEFAULT FALSE,
    notification_channel VARCHAR(50), -- Email, SMS, Push, InApp
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_notifications_user ON notifications(user_id);
CREATE INDEX idx_notifications_read ON notifications(is_read);

-- Audit Logs Table
CREATE TABLE audit_logs (
    audit_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID REFERENCES users(user_id),
    action VARCHAR(255) NOT NULL,
    entity_type VARCHAR(100),
    entity_id VARCHAR(255),
    old_values JSONB,
    new_values JSONB,
    ip_address VARCHAR(45),
    user_agent TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_audit_user ON audit_logs(user_id);
CREATE INDEX idx_audit_date ON audit_logs(created_date);

-- Daily Profit Summary Table
CREATE TABLE daily_profit_summary (
    summary_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    summary_date DATE NOT NULL UNIQUE,
    total_transactions INTEGER DEFAULT 0,
    total_revenue DECIMAL(15, 2) DEFAULT 0.00,
    total_api_cost DECIMAL(15, 2) DEFAULT 0.00,
    total_profit DECIMAL(15, 2) DEFAULT 0.00,
    total_commission DECIMAL(15, 2) DEFAULT 0.00,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create a view for user dashboard summary
CREATE VIEW user_dashboard_summary AS
SELECT 
    u.user_id,
    u.full_name,
    u.email,
    w.balance,
    w.available_balance,
    COUNT(DISTINCT wt.transaction_id) as total_purchases,
    COALESCE(SUM(CASE WHEN wt.transaction_type IN ('Airtime', 'Data', 'Cable', 'Electricity') THEN wt.amount ELSE 0 END), 0) as total_spending,
    COALESCE(SUM(CASE WHEN wt.transaction_type = 'Commission' THEN wt.amount ELSE 0 END), 0) as referral_earnings
FROM users u
LEFT JOIN wallets w ON u.user_id = w.user_id
LEFT JOIN wallet_transactions wt ON u.user_id = wt.user_id
GROUP BY u.user_id, u.full_name, u.email, w.balance, w.available_balance;
