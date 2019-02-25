CREATE TABLE Havm2TTComparisonRun (
Id int IDENTITY(1,1) NOT NULL
,RunTime datetime NOT NULL
,CONSTRAINT [PK_Havm2TTComparisonRun] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO

CREATE TABLE Havm2TTComparisonRunTable (
Id int IDENTITY(1,1) NOT NULL
,Havm2TTComparisonRunId int NOT NULL
,TableName varchar(max) NOT NULL
,TotalRecordsExisting int NOT NULL
,TotalRecordsNew int NOT NULL
,RecordsIdentical int NOT NULL
,RecordsMissingFromNew int NOT NULL
,RecordsExtraInNew int NOT NULL
,RecordsDiffering int NOT NULL
,CONSTRAINT [PK_Havm2TTComparisonRunTable] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
ALTER TABLE Havm2TTComparisonRunTable WITH CHECK ADD CONSTRAINT [FK_Havm2TTComparisonRunTable_Havm2TTComparisonRun] FOREIGN KEY(Havm2TTComparisonRunId) REFERENCES Havm2TTComparisonRun ([Id])
GO

CREATE TABLE Havm2TTComparison_T_Temp_Trips_MissingFromNew (
Id int IDENTITY(1,1) NOT NULL
,Havm2TTComparisonRunId int NOT NULL
,[TripID] [int] NULL
,[RunNo] [char](5) NOT NULL
,[RouteNo] [smallint] NOT NULL
,[FirstTP] [char](4) NOT NULL
,[FirstTime] [int] NOT NULL
,[EndTP] [char](4) NOT NULL
,[EndTime] [int] NOT NULL
,[AtLayoverTime] [smallint] NOT NULL
,[NextRouteNo] [smallint] NULL
,[UpDirection] [bit] NOT NULL
,[LowFloor] [bit] NOT NULL
,[TripDistance] [decimal](6, 3) NULL
,[PublicTrip] [bit] NOT NULL
,[DayOfWeek] [tinyint] NOT NULL
,CONSTRAINT [PK_Havm2TTComparison_T_Temp_Trips_MissingFromNew] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
ALTER TABLE Havm2TTComparison_T_Temp_Trips_MissingFromNew WITH CHECK ADD CONSTRAINT [FK_Havm2TTComparison_T_Temp_Trips_MissingFromNew_Havm2TTComparisonRun] FOREIGN KEY(Havm2TTComparisonRunId) REFERENCES Havm2TTComparisonRun ([Id])
GO

CREATE TABLE Havm2TTComparison_T_Temp_Schedules_MissingFromNew (
Id int IDENTITY(1,1) NOT NULL
,Havm2TTComparisonRunId int NOT NULL
,[TripID] [int] NOT NULL
,[RunNo] [char](5) NOT NULL
,[StopID] [char](8) NOT NULL
,[RouteNo] [smallint] NOT NULL
,[OPRTimePoint] [bit] NOT NULL
,[Time] [int] NOT NULL
,[DayOfWeek] [tinyint] NOT NULL
,[LowFloor] [bit] NOT NULL
,[PublicTrip] [bit] NOT NULL
,CONSTRAINT [PK_Havm2TTComparison_T_Temp_Schedules_MissingFromNew] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
ALTER TABLE Havm2TTComparison_T_Temp_Schedules_MissingFromNew WITH CHECK ADD CONSTRAINT [FK_Havm2TTComparison_T_Temp_Schedules_MissingFromNew_Havm2TTComparisonRun] FOREIGN KEY(Havm2TTComparisonRunId) REFERENCES Havm2TTComparisonRun ([Id])
GO

CREATE TABLE Havm2TTComparison_T_Temp_TripsExtraInNew (
Id int IDENTITY(1,1) NOT NULL
,Havm2TTComparisonRunId int NOT NULL
,[TripID] [int] NULL
,[RunNo] [char](5) NOT NULL
,[RouteNo] [smallint] NOT NULL
,[FirstTP] [char](4) NOT NULL
,[FirstTime] [int] NOT NULL
,[EndTP] [char](4) NOT NULL
,[EndTime] [int] NOT NULL
,[AtLayoverTime] [smallint] NOT NULL
,[NextRouteNo] [smallint] NULL
,[UpDirection] [bit] NOT NULL
,[LowFloor] [bit] NOT NULL
,[TripDistance] [decimal](6, 3) NULL
,[PublicTrip] [bit] NOT NULL
,[DayOfWeek] [tinyint] NOT NULL
,CONSTRAINT [PK_Havm2TTComparison_T_Temp_TripsExtraInNew] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
ALTER TABLE Havm2TTComparison_T_Temp_TripsExtraInNew WITH CHECK ADD CONSTRAINT [FK_Havm2TTComparison_T_Temp_TripsExtraInNew_Havm2TTComparisonRun] FOREIGN KEY(Havm2TTComparisonRunId) REFERENCES Havm2TTComparisonRun ([Id])
GO

CREATE TABLE Havm2TTComparison_T_Temp_SchedulesExtraInNew (
Id int IDENTITY(1,1) NOT NULL
,Havm2TTComparisonRunId int NOT NULL
,[TripID] [int] NOT NULL
,[RunNo] [char](5) NOT NULL
,[StopID] [char](8) NOT NULL
,[RouteNo] [smallint] NOT NULL
,[OPRTimePoint] [bit] NOT NULL
,[Time] [int] NOT NULL
,[DayOfWeek] [tinyint] NOT NULL
,[LowFloor] [bit] NOT NULL
,[PublicTrip] [bit] NOT NULL
,CONSTRAINT [PK_Havm2TTComparison_T_Temp_SchedulesExtraInNew] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
ALTER TABLE Havm2TTComparison_T_Temp_SchedulesExtraInNew WITH CHECK ADD CONSTRAINT [FK_Havm2TTComparison_T_Temp_SchedulesExtraInNew_Havm2TTComparisonRun] FOREIGN KEY(Havm2TTComparisonRunId) REFERENCES Havm2TTComparisonRun ([Id])
GO

CREATE TABLE Havm2TTComparison_T_Temp_TripsDiffering (
Id int IDENTITY(1,1) NOT NULL
,Havm2TTComparisonRunId int NOT NULL
,PairIdentifier uniqueidentifier NOT NULL
,[TripID] [int] NULL
,[RunNo] [char](5) NOT NULL
,[RouteNo] [smallint] NOT NULL
,[FirstTP] [char](4) NOT NULL
,[FirstTime] [int] NOT NULL
,[EndTP] [char](4) NOT NULL
,[EndTime] [int] NOT NULL
,[AtLayoverTime] [smallint] NOT NULL
,[NextRouteNo] [smallint] NULL
,[UpDirection] [bit] NOT NULL
,[LowFloor] [bit] NOT NULL
,[TripDistance] [decimal](6, 3) NULL
,[PublicTrip] [bit] NOT NULL
,[DayOfWeek] [tinyint] NOT NULL
,CONSTRAINT [PK_Havm2TTComparison_T_Temp_TripsDiffering] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
ALTER TABLE Havm2TTComparison_T_Temp_TripsDiffering WITH CHECK ADD CONSTRAINT [FK_Havm2TTComparison_T_Temp_TripsDiffering_Havm2TTComparisonRun] FOREIGN KEY(Havm2TTComparisonRunId) REFERENCES Havm2TTComparisonRun ([Id])
GO

CREATE TABLE Havm2TTComparison_T_Temp_SchedulesDiffering (
Id int IDENTITY(1,1) NOT NULL
,Havm2TTComparisonRunId int NOT NULL
,PairIdentifier uniqueidentifier NOT NULL
,[TripID] [int] NOT NULL
,[RunNo] [char](5) NOT NULL
,[StopID] [char](8) NOT NULL
,[RouteNo] [smallint] NOT NULL
,[OPRTimePoint] [bit] NOT NULL
,[Time] [int] NOT NULL
,[DayOfWeek] [tinyint] NOT NULL
,[LowFloor] [bit] NOT NULL
,[PublicTrip] [bit] NOT NULL
,CONSTRAINT [PK_Havm2TTComparison_T_Temp_SchedulesDiffering] PRIMARY KEY CLUSTERED ([Id] ASC)
)
GO
ALTER TABLE Havm2TTComparison_T_Temp_SchedulesDiffering WITH CHECK ADD CONSTRAINT [FK_Havm2TTComparison_T_Temp_SchedulesDiffering_Havm2TTComparisonRun] FOREIGN KEY(Havm2TTComparisonRunId) REFERENCES Havm2TTComparisonRun ([Id])
GO
