USE master
CREATE DATABASE TvChannel;

GO
ALTER DATABASE TvChannel SET RECOVERY SIMPLE
GO

USE TvChannel

CREATE TABLE Genres 
(
	GenreId INT PRIMARY KEY IDENTITY(1,1),
	GenreName VARCHAR(16),
	GenreDescription VARCHAR(32)
)

CREATE TABLE Shows 
(
	ShowId INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(32),
	ReleaseDate DATE,
	Duration TIME,
	Mark INT,
	MarkMonth INT,
	MarkYear INT,
	GenreId INT,
	Description VARCHAR(32)
)

CREATE TABLE Appeals
(	
	AppealId INT PRIMARY KEY IDENTITY(1,1),
	FullName VARCHAR(64),
	Organization VARCHAR(64),
	ShowId INT,
	GoalRequest VARCHAR(128)
)

CREATE TABLE Timetables
(
	TimetableId INT PRIMARY KEY IDENTITY(1,1),
	DayOfWeek INT,
	Month INT,
	Year INT,
	ShowId INT,
	StartTime TIME,
	EndTime TIME,
	StaffId INT
)

CREATE TABLE Staff
(
	StaffId INT PRIMARY KEY IDENTITY(1,1),
	FullName VARCHAR(64),
	PositionId INT
)

CREATE TABLE Positions
(
	PositionId INT PRIMARY KEY IDENTITY(1,1),
	Name VARCHAR(16)
)

ALTER TABLE dbo.Shows WITH CHECK ADD CONSTRAINT FK_Shows_Genres FOREIGN KEY(GenreId)
REFERENCES dbo.Genres (GenreId) ON DELETE CASCADE
GO
ALTER TABLE dbo.Appeals WITH CHECK ADD CONSTRAINT FK_Appeals_Shows FOREIGN KEY(ShowId)
REFERENCES dbo.Shows (ShowId)
GO
ALTER TABLE dbo.Timetables WITH CHECK ADD CONSTRAINT FK_Timetables_Shows FOREIGN KEY(ShowId)
REFERENCES dbo.Shows (ShowId) ON DELETE CASCADE
GO
ALTER TABLE dbo.Timetables WITH CHECK ADD CONSTRAINT FK_Timetables_Staff FOREIGN KEY(StaffId)
REFERENCES dbo.Staff (StaffId)
GO
ALTER TABLE dbo.Staff WITH CHECK ADD CONSTRAINT FK_Staff_Positions FOREIGN KEY(PositionId)
REFERENCES dbo.Positions (PositionId)
GO
ALTER TABLE dbo.Timetables WITH CHECK ADD CONSTRAINT Check_EndTime CHECK(EndTime > StartTime)
GO
