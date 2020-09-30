USE TvChannel

DECLARE @Letters CHAR(52) = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz',
		@i int,
		@Position int,
		@RowCount int,
		@RowIndex int,
		@MinLetters int,
		@MaxLetters int,
		@LettersLimit int,

		-- Таблица "Genres".
		@GenreName varchar(16),
		@GenreDescription varchar(32),
		
		-- Таблица "Shows".
		@ShowName varchar(32),
		@ShowReleaseDate date,
		@ShowDuration time,
		@ShowMark int,
		@ShowMarkMonth int,
		@ShowMarkYear int,
		@ShowGenreId int,
		@ShowDescription varchar(32),

		-- Таблица "Timetables".
		@DayOfWeek int,
		@Month int,
		@Year int,
		@ShowId int,
		@StartTime time,
		@EndTime time

SET NOCOUNT ON

-- Таблица "Genres".
SET @RowCount = 500
SET @RowIndex = 1
SET @MinLetters = 8

WHILE @RowIndex <= @RowCount
BEGIN
	-- Название жанра.
	SET @MaxLetters = 16

	SET @LettersLimit = @MinLetters + RAND()*(@MaxLetters - @MinLetters)
	SET @i = 1
	SET @GenreName = ''
	WHILE @i <= @LettersLimit
	BEGIN
		SET @Position = RAND()*52
		SET @GenreName = @GenreName + SUBSTRING(@Letters, @Position, 1)
		SET @i += 1
	END

	-- Описания жанра.
	SET @MaxLetters = 32

	SET @LettersLimit = @MinLetters + RAND()*(@MaxLetters - @MinLetters)
	SET @i = 1
	SET @GenreDescription = ''
	WHILE @i <= @LettersLimit
	BEGIN
		SET @Position = RAND()*52
		SET @GenreDescription = @GenreDescription + SUBSTRING(@Letters, @Position, 1)
		SET @i += 1
	END

	INSERT INTO Genres (GenreName, GenreDescription) VALUES (@GenreName, @GenreDescription)

	SET @RowIndex += 1
END

-- Таблица "Shows".
SET @RowCount = 20000;
SET @RowIndex = 1;

WHILE @RowIndex <= @RowCount
BEGIN
	-- Название телепередачи.
	SET @MinLetters = 8;
	SET @MaxLetters = 32;
	SET @LettersLimit = @MinLetters + RAND()*(@MaxLetters - @MinLetters)
	SET @ShowName = ''
	SET @i = 1
	WHILE @i <= @LettersLimit
	BEGIN
		SET @Position = RAND()*52
		SET @ShowName = @ShowName + SUBSTRING(@Letters, @Position, 1)
		SET @i += 1
	END

	-- Дата выхода.
	SET @ShowReleaseDate = dateadd(DAY, -RAND()*15000, GETDATE())
	
	-- Продолжительность
	DECLARE @TempDuration int = RAND()*(10801 - 3600) + 3600
	SET @ShowDuration = CONVERT(VARCHAR(8), (Select DATEADD(SECOND,DATEDIFF(DAY,0, @TempDuration),'00:00:00')), 108)

	-- Оценка
	SET @ShowMark = RAND()*(11-1)+1
	SET @ShowMarkMonth = RAND()*(13-1)+1
	SET @ShowMarkYear = RAND()*(2021-2010)+2010

	-- Внешний ключ к т. "Genres".
	SET @ShowGenreId = RAND()*(501-1)+1

	-- Описания телепередачи.
	SET @MaxLetters = 32

	SET @LettersLimit = @MinLetters + RAND()*(@MaxLetters - @MinLetters)
	SET @i = 1
	SET @ShowDescription = ''
	WHILE @i <= @LettersLimit
	BEGIN
		SET @Position = RAND()*52
		SET @ShowDescription = @ShowDescription + SUBSTRING(@Letters, @Position, 1)
		SET @i += 1
	END

	INSERT INTO Shows (Name, ReleaseDate, Duration, Mark, MarkMonth, MarkYear, GenreId, Description) 
		VALUES (@ShowName, @ShowReleaseDate, @ShowDuration, @ShowMark, @ShowMarkMonth, @ShowMarkYear, @ShowGenreId, @ShowDescription)

	SET @RowIndex += 1
END

-- Таблица "Timetables".
SET @RowCount = 20000
SET @RowIndex = 1

WHILE @RowIndex <= @RowCount
BEGIN
	SET @DayOfWeek = RAND()*(8-1)+1
	SET @Month = RAND()*(13-1)+1
	SET @Year = RAND()*(2021-2010)+2010
	SET @ShowId = RAND()*(20001 - 1) + 1

	DECLARE @TempStartTime int = RAND()*(75600 - 28800) + 28800
	SET @StartTime = CONVERT(VARCHAR(8), (Select DATEADD(SECOND,DATEDIFF(DAY,0, @TempStartTime),'00:00:00')), 108)
	SET @EndTime = CONVERT(VARCHAR(8), (SELECT DATEADD(SECOND, DATEDIFF(SECOND,0,(SELECT Shows.Duration FROM Shows WHERE @ShowId = Shows.ShowId)), @StartTime)), 108)

	INSERT INTO Timetables (DayOfWeek, Month, Year, ShowId, StartTime, EndTime) VALUES
		(@DayOfWeek, @Month, @Year, @ShowId, @StartTime, @EndTime)

	SET @RowIndex += 1
END