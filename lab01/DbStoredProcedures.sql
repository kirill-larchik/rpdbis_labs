use TvChannel
go

create procedure sp_InsertShow
	@Name varchar(32),
	@ReleaseDate datetime,
	@Duration time,
	@Mark int,
	@MarkMonth int,
	@MarkYear int,
	@GenreName varchar(16),
	@Description varchar(32)
as 
	declare @TempDate datetime = dateadd(month, @MarkMonth, @ReleaseDate)
	set @TempDate = dateadd(year, @MarkYear, @ReleaseDate)
	if @ReleaseDate > @TempDate
	begin
		set @Mark = null
		set @MarkMonth = null
		set @MarkYear = null
	end

	declare @GenreId int = (select dbo.Genres.GenreId from dbo.Genres where dbo.Genres.GenreName = @GenreName)

	insert into dbo.Shows (Name, ReleaseDate, Duration, Mark, MarkMonth, MarkYear, GenreId, Description)
	select @Name, @ReleaseDate, @Duration, @Mark, @MarkMonth, @MarkYear, @GenreId, @Description
go

create procedure sp_UpdateShow
	@Name varchar(32),
	@ReleaseDate datetime,
	@Duration time,
	@Mark int,
	@MarkMonth int,
	@MarkYear int,
	@GenreName varchar(16),
	@Description varchar(32),
	@ShowId int
as
	declare @TempDate datetime = dateadd(month, @MarkMonth, @ReleaseDate)
	set @TempDate = dateadd(year, @MarkYear, @ReleaseDate)
	if @ReleaseDate > @TempDate
	begin
		set @Mark = null
		set @MarkMonth = null
		set @MarkYear = null
	end

	declare @GenreId int = (select dbo.Genres.GenreId from dbo.Genres where dbo.Genres.GenreName = @GenreName)

	update dbo.Shows
	set dbo.Shows.Name = @Name,
		dbo.Shows.ReleaseDate = @ReleaseDate,
		dbo.Shows.Duration = @Duration,
		dbo.Shows.Mark = @Mark,
		dbo.Shows.MarkMonth = @MarkMonth,
		dbo.Shows.MarkYear = @MarkYear,
		dbo.Shows.GenreId = @GenreId,
		dbo.Shows.Description = @Description
	where dbo.Shows.ShowId = @ShowId
go

create procedure sp_InsertTimetables
	@DayOfWeek int,
	@Month int,
	@Year int,
	@ShowName varchar(32),
	@StartTime time,
	@StaffId int null
as
	declare @ShowId int = (select dbo.Shows.ShowId from dbo.Shows where dbo.Shows.Name = @ShowName)
	declare @EndTime datetime =  CONVERT(VARCHAR(8), (SELECT DATEADD(SECOND, DATEDIFF(SECOND,0,(SELECT Shows.Duration FROM Shows WHERE @ShowId = Shows.ShowId)), @StartTime)), 108)

	insert into dbo.Timetables (DayOfWeek, Month, Year, ShowId, StartTime, EndTime)
	select @DayOfWeek, @Month, @Year, @ShowId, @StartTime, @EndTime
go