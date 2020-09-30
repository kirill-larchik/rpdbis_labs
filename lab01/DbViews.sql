USE TvChannel
GO

CREATE VIEW [dbo].[View_Shows]
AS
SELECT	dbo.Shows.Name, dbo.Shows.ReleaseDate, dbo.Genres.GenreName,
			dbo.Shows.Description, dbo.Shows.Duration, dbo.Shows.Mark, dbo.Shows.MarkMonth,
			dbo.Shows.MarkYear
FROM	dbo.Shows JOIN dbo.Genres on dbo.Shows.GenreId = dbo.Genres.GenreId
GO

CREATE VIEW [dbo].[View_Timetables]
AS
SELECT dbo.Shows.Name, dbo.Genres.GenreName, dbo.Timetables.DayOfWeek, dbo.Timetables.Month, dbo.Timetables.Year,
			dbo.Timetables.StartTime, dbo.Timetables.EndTime
FROM	dbo.Shows JOIN dbo.Genres on dbo.Shows.GenreId = dbo.Genres.GenreId
		JOIN dbo.Timetables on dbo.Shows.ShowId = dbo.Timetables.ShowId
GO

CREATE VIEW [dbo].[View_Genres]
AS
SELECT	dbo.Genres.GenreName, dbo.Genres.GenreDescription 
FROM	dbo.Genres