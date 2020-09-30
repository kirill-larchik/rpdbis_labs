use TvChannel

DELETE FROM Timetables
DELETE FROM Shows
DELETE FROM Genres

DBCC CHECKIDENT('[Genres]', RESEED, 0);
DBCC CHECKIDENT('[Shows]', RESEED, 0);
DBCC CHECKIDENT('[Timetables]', RESEED, 0);
GO