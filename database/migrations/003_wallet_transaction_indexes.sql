-- Migration 003: Wallet Transaction Indexes
-- This migration adds indexes for optimized wallet transaction queries

CREATE INDEX idx_wallet_transactions_user_date ON wallet_transactions(user_id, created_date DESC);
CREATE INDEX idx_wallet_transactions_status ON wallet_transactions(status);
CREATE INDEX idx_wallet_transactions_type_date ON wallet_transactions(transaction_type, created_date DESC);

-- View for wallet transaction summary
CREATE VIEW wallet_transaction_summary AS
SELECT
    user_id,
    DATE(created_date) as transaction_date,
    COUNT(*) as total_transactions,
    SUM(CASE WHEN status = 'Success' THEN 1 ELSE 0 END) as successful_transactions,
    SUM(CASE WHEN status = 'Failed' THEN 1 ELSE 0 END) as failed_transactions,
    SUM(CASE WHEN transaction_type IN ('Airtime', 'Data', 'Cable', 'Electricity') THEN amount ELSE 0 END) as total_purchases,
    SUM(CASE WHEN transaction_type = 'Commission' THEN amount ELSE 0 END) as total_commissions,
    SUM(amount) as total_amount
FROM wallet_transactions
WHERE status = 'Success'
GROUP BY user_id, DATE(created_date);
