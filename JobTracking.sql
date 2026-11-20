-- ============================================================================
-- Database Creation & Setup: JobTracking
-- Description: Tracks jobs applied for, including job types, locations, and notes.
-- Compatible with: Microsoft SQL Server (T-SQL)
-- ============================================================================

-- 1. Create the Database if it does not already exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'JobTracking')
BEGIN
    CREATE DATABASE JobTracking;
END;
GO

USE JobTracking;
GO

-- 2. Drop Tables if they exist (in reverse dependency order)
IF OBJECT_ID('dbo.JobNotes', 'U') IS NOT NULL DROP TABLE dbo.JobNotes;
IF OBJECT_ID('dbo.Job', 'U') IS NOT NULL DROP TABLE dbo.Job;
IF OBJECT_ID('dbo.JobLocation', 'U') IS NOT NULL DROP TABLE dbo.JobLocation;
IF OBJECT_ID('dbo.JobType', 'U') IS NOT NULL DROP TABLE dbo.JobType;
GO

-- ============================================================================
-- Table: JobType
-- Description: Lookup table for employment arrangement (Full Time, Part Time, etc.)
-- ============================================================================
CREATE TABLE dbo.JobType (
    JobTypeId INT IDENTITY(1,1) NOT NULL,
    TypeName NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_JobType PRIMARY KEY CLUSTERED (JobTypeId),
    CONSTRAINT UQ_JobType_TypeName UNIQUE (TypeName)
);
GO

-- ============================================================================
-- Table: JobLocation
-- Description: Lookup table for workplace arrangement (On-Site, Remote, Hybrid)
-- ============================================================================
CREATE TABLE dbo.JobLocation (
    JobLocationId INT IDENTITY(1,1) NOT NULL,
    LocationType NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_JobLocation PRIMARY KEY CLUSTERED (JobLocationId),
    CONSTRAINT UQ_JobLocation_LocationType UNIQUE (LocationType)
);
GO

-- ============================================================================
-- Table: Job
-- Description: Primary table representing individual job applications
-- ============================================================================
CREATE TABLE dbo.Job (
    JobId INT IDENTITY(1,1) NOT NULL,
    Company NVARCHAR(150) NOT NULL,
    JobTitle NVARCHAR(150) NOT NULL,
    JobTypeId INT NOT NULL,
    JobLocationId INT NOT NULL,
    PhysicalLocation NVARCHAR(150) NULL, -- e.g., 'Dallas, TX' or office address
    ApplicationDate DATE NOT NULL CONSTRAINT DF_Job_ApplicationDate DEFAULT (CAST(GETDATE() AS DATE)),
    Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Job_Status DEFAULT ('Applied'), -- Applied, Interviewing, Offered, Rejected, Withdrawn
    SalaryRange NVARCHAR(100) NULL,
    JobUrl NVARCHAR(2048) NULL,
    ContactPerson NVARCHAR(100) NULL,
    ContactEmail NVARCHAR(256) NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Job_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_Job_UpdatedAt DEFAULT (SYSUTCDATETIME()),
    
    CONSTRAINT PK_Job PRIMARY KEY CLUSTERED (JobId),
    CONSTRAINT FK_Job_JobType FOREIGN KEY (JobTypeId) REFERENCES dbo.JobType (JobTypeId),
    CONSTRAINT FK_Job_JobLocation FOREIGN KEY (JobLocationId) REFERENCES dbo.JobLocation (JobLocationId)
);
GO

-- Indexes for performance on frequently queried columns
CREATE NONCLUSTERED INDEX IX_Job_JobTypeId ON dbo.Job (JobTypeId);
CREATE NONCLUSTERED INDEX IX_Job_JobLocationId ON dbo.Job (JobLocationId);
CREATE NONCLUSTERED INDEX IX_Job_Status ON dbo.Job (Status);
CREATE NONCLUSTERED INDEX IX_Job_Company ON dbo.Job (Company);
GO

-- ============================================================================
-- Table: JobNotes
-- Description: Logs progress notes, interview feedback, or followup details
-- ============================================================================
CREATE TABLE dbo.JobNotes (
    JobNoteId INT IDENTITY(1,1) NOT NULL,
    JobId INT NOT NULL,
    NoteText NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2(7) NOT NULL CONSTRAINT DF_JobNotes_CreatedAt DEFAULT (SYSUTCDATETIME()),
    
    CONSTRAINT PK_JobNotes PRIMARY KEY CLUSTERED (JobNoteId),
    CONSTRAINT FK_JobNotes_Job FOREIGN KEY (JobId) REFERENCES dbo.Job (JobId) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX IX_JobNotes_JobId ON dbo.JobNotes (JobId);
GO

-- ============================================================================
-- Seed Lookup Data
-- ============================================================================

-- Seed JobType values
INSERT INTO dbo.JobType (TypeName)
VALUES 
    (N'Full Time'),
    (N'Part Time'),
    (N'Contract'),
    (N'Temp');
GO

-- Seed JobLocation values
INSERT INTO dbo.JobLocation (LocationType)
VALUES 
    (N'On-Site'),
    (N'Remote'),
    (N'Hybrid');
GO
