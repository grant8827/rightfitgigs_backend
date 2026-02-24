#!/bin/bash

# SQLite to PostgreSQL Migration Script
# This script exports data from SQLite and imports it into PostgreSQL

SQLITE_DB="/Users/gregorygrant/Desktop/Websites/flutter/rightfitgigs/backend/rightfitgigs.db"
PG_HOST="hopper.proxy.rlwy.net"
PG_PORT="33137"
PG_USER="postgres"
PG_PASSWORD="bKnpjKnCKoqpuWyTjTziZxfNtceZRXIs"
PG_DATABASE="railway"

export PGPASSWORD="$PG_PASSWORD"

echo "Starting SQLite to PostgreSQL migration..."
echo ""

# Step 1: Clear all existing data (in reverse order of dependencies)
echo "Step 1: Clearing existing data from PostgreSQL..."
psql -h "$PG_HOST" -p "$PG_PORT" -U "$PG_USER" -d "$PG_DATABASE" << EOF
DELETE FROM "Messages";
DELETE FROM "Notifications";
DELETE FROM "Applications";
DELETE FROM "AppMetrics";
DELETE FROM "Advertisements";
DELETE FROM "Jobs";
DELETE FROM "Users";
DELETE FROM "Companies";
EOF
echo "  ✓ All tables cleared"

# Step 2: Export and import all data
echo ""
echo "Step 2: Migrating all data from SQLite to PostgreSQL..."

# Function to migrate a table
migrate_table() {
    local table_name=$1
    echo ""
    echo "  Migrating $table_name..."
    
    # Export to CSV
    sqlite3 -header -csv "$SQLITE_DB" "SELECT * FROM $table_name;" > "/tmp/${table_name}.csv"
    
    # Check if CSV has data
    local line_count=$(wc -l < "/tmp/${table_name}.csv")
    if [ "$line_count" -le 1 ]; then
        echo "    No data in $table_name"
        rm "/tmp/${table_name}.csv"
        return
    fi
    
    # Import to PostgreSQL
    psql -h "$PG_HOST" -p "$PG_PORT" -U "$PG_USER" -d "$PG_DATABASE" -c "\COPY \"$table_name\" FROM '/tmp/${table_name}.csv' WITH CSV HEADER;" 2>&1 | grep -v "^COPY"
    
    local imported=$(tail -1 "/tmp/${table_name}.csv" | wc -l)
    echo "    ✓ Imported $((line_count - 1)) rows"
    
    # Clean up
    rm "/tmp/${table_name}.csv"
}

# Migrate in order of dependencies
migrate_table "Companies"
migrate_table "Users"
migrate_table "Jobs"
migrate_table "Applications"
migrate_table "Messages"
migrate_table "Notifications"
migrate_table "Advertisements"
migrate_table "AppMetrics"

echo ""
echo "Migration complete!"
echo ""
echo "Verifying final data counts..."
psql -h "$PG_HOST" -p "$PG_PORT" -U "$PG_USER" -d "$PG_DATABASE" -t -c "
SELECT 'Users: ' || COUNT(*)::text FROM \"Users\" 
UNION ALL SELECT 'Companies: ' || COUNT(*)::text FROM \"Companies\" 
UNION ALL SELECT 'Jobs: ' || COUNT(*)::text FROM \"Jobs\" 
UNION ALL SELECT 'Applications: ' || COUNT(*)::text FROM \"Applications\" 
UNION ALL SELECT 'Messages: ' || COUNT(*)::text FROM \"Messages\"
UNION ALL SELECT 'Notifications: ' || COUNT(*)::text FROM \"Notifications\"
UNION ALL SELECT 'Advertisements: ' || COUNT(*)::text FROM \"Advertisements\"
UNION ALL SELECT 'AppMetrics: ' || COUNT(*)::text FROM \"AppMetrics\";
"

unset PGPASSWORD
