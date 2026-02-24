-- Fix DateTime columns in PostgreSQL by converting TEXT to timestamp with time zone

-- Users table
ALTER TABLE "Users" ALTER COLUMN "CreatedDate" TYPE timestamp with time zone USING "CreatedDate"::timestamp with time zone;
ALTER TABLE "Users" ALTER COLUMN "UpdatedDate" TYPE timestamp with time zone USING "UpdatedDate"::timestamp with time zone;

-- Companies table
ALTER TABLE "Companies" ALTER COLUMN "CreatedDate" TYPE timestamp with time zone USING "CreatedDate"::timestamp with time zone;
ALTER TABLE "Companies" ALTER COLUMN "UpdatedDate" TYPE timestamp with time zone USING "UpdatedDate"::timestamp with time zone;

-- Jobs table
ALTER TABLE "Jobs" ALTER COLUMN "PostedDate" TYPE timestamp with time zone USING "PostedDate"::timestamp with time zone;
ALTER TABLE "Jobs" ALTER COLUMN "UpdatedDate" TYPE timestamp with time zone USING "UpdatedDate"::timestamp with time zone;

-- Messages table
ALTER TABLE "Messages" ALTER COLUMN "SentDate" TYPE timestamp with time zone USING "SentDate"::timestamp with time zone;
ALTER TABLE "Messages" ALTER COLUMN "ReadDate" TYPE timestamp with time zone USING "ReadDate"::timestamp with time zone;

-- Applications table (if exists)
ALTER TABLE "Applications" ALTER COLUMN "AppliedDate" TYPE timestamp with time zone USING "AppliedDate"::timestamp with time zone;
ALTER TABLE "Applications" ALTER COLUMN "UpdatedDate" TYPE timestamp with time zone USING "UpdatedDate"::timestamp with time zone;

-- Notifications table (if exists)
ALTER TABLE "Notifications" ALTER COLUMN "CreatedDate" TYPE timestamp with time zone USING "CreatedDate"::timestamp with time zone;
ALTER TABLE "Notifications" ALTER COLUMN "ReadDate" TYPE timestamp with time zone USING "ReadDate"::timestamp with time zone;

-- Advertisements table (if exists)
ALTER TABLE "Advertisements" ALTER COLUMN "CreatedDate" TYPE timestamp with time zone USING "CreatedDate"::timestamp with time zone;
ALTER TABLE "Advertisements" ALTER COLUMN "UpdatedDate" TYPE timestamp with time zone USING "UpdatedDate"::timestamp with time zone;
ALTER TABLE "Advertisements" ALTER COLUMN "StartDate" TYPE timestamp with time zone USING "StartDate"::timestamp with time zone;
ALTER TABLE "Advertisements" ALTER COLUMN "EndDate" TYPE timestamp with time zone USING "EndDate"::timestamp with time zone;

-- AppMetrics table (if exists)
ALTER TABLE "AppMetrics" ALTER COLUMN "CreatedDate" TYPE timestamp with time zone USING "CreatedDate"::timestamp with time zone;
