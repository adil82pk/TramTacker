USE [tramtracker]
GO

/****** Object:  Table [dbo].[DailyRunoutTimes]    Script Date: 19/08/2019 4:47:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[DailyRunoutTimes](
	[RunNo] [char](5) NOT NULL,
	[RunoutTime] [int] NOT NULL,
	[IsOut] [int] NOT NULL,
 CONSTRAINT [PK_DailyRunoutTimes] PRIMARY KEY CLUSTERED 
(
	[RunNo] ASC,
	[RunoutTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[PredictionCacheRefreshLog]    Script Date: 19/08/2019 4:48:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PredictionCacheRefreshLog](
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[Predictions]    Script Date: 19/08/2019 4:49:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Predictions](
	[RunNo] [char](5) NULL,
	[StopID] [char](8) NULL,
	[StopNo] [smallint] NULL,
	[InternalRouteNo] [smallint] NULL,
	[RouteNo] [smallint] NULL,
	[HeadboardRouteNo] [varchar](20) NULL,
	[Destination] [varchar](100) NULL,
	[StopDistance] [float] NULL,
	[TramDistance] [float] NULL,
	[Deviation] [int] NULL,
	[AVMTime] [datetime] NULL,
	[VehicleNo] [int] NULL,
	[LowFloor] [bit] NULL,
	[Down] [bit] NULL,
	[Schedule] [datetime] NULL,
	[Adjustment] [int] NULL,
	[DisplayPrediction] [bit] NULL,
	[SpecialEventMessage] [varchar](255) NULL,
	[TTDMSMessage] [varchar](255) NULL,
	[DisplayFOCMessage] [bit] NULL,
	[DisplayAirCondition] [bit] NULL
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[PredictionsCache]    Script Date: 19/08/2019 4:49:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PredictionsCache](
	[Prediction] [int] NOT NULL,
	[RunNo] [char](5) NOT NULL,
	[StopID] [char](8) NULL,
	[StopNo] [smallint] NOT NULL,
	[InternalRouteNo] [smallint] NOT NULL,
	[RouteNo] [smallint] NOT NULL,
	[HeadboardRouteNo] [varchar](20) NULL,
	[Destination] [varchar](100) NULL,
	[StopDistance] [float] NULL,
	[TramDistance] [float] NULL,
	[Deviation] [int] NULL,
	[AVMTime] [datetime] NOT NULL,
	[VehicleNo] [int] NOT NULL,
	[LowFloor] [bit] NULL,
	[Down] [bit] NULL,
	[Schedule] [datetime] NOT NULL,
	[Adjustment] [int] NULL,
	[DisplayPrediction] [bit] NULL,
	[SpecialEventMessage] [varchar](255) NULL,
	[TTDMSMessage] [varchar](255) NULL,
	[DisplayFOCMessage] [bit] NULL,
	[DisplayAirCondition] [bit] NULL
) ON [PRIMARY]
GO
USE [tramtracker]
GO

/****** Object:  Table [dbo].[StopMessages]    Script Date: 19/08/2019 4:49:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StopMessages](
	[RouteNo] [smallint] NULL,
	[StopNo] [smallint] NULL,
	[DisplayPrediction] [bit] NULL,
	[SpecialEventMessage] [varchar](255) NULL,
	[TTDMSMessage] [varchar](255) NULL,
	[DisplayFOCMessage] [bit] NULL
) ON [PRIMARY]
GO
USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_DestinationLookUps]    Script Date: 19/08/2019 4:50:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_DestinationLookUps](
	[DestinationCode] [char](4) NOT NULL,
	[Destination] [varchar](100) NOT NULL,
 CONSTRAINT [PK_T_DestinationLookUps] PRIMARY KEY CLUSTERED 
(
	[DestinationCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Preferences]    Script Date: 19/08/2019 4:50:59 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Preferences](
	[IgnoreCache] [bit] NOT NULL,
	[CacheTimestamp] [datetime] NOT NULL,
	[CacheReady] [bit] NOT NULL,
	[PredictionAvailable] [bit] NOT NULL,
	[DataAvailable] [bit] NOT NULL,
	[ScheduleLoaded] [bit] NOT NULL,
	[TripsLoaded] [bit] NOT NULL,
	[DisplayAC] [bit] NOT NULL,
	[MinTempForAC] [tinyint] NOT NULL,
	[CurrentTemp] [tinyint] NULL,
 CONSTRAINT [PK_T_Preferences] PRIMARY KEY CLUSTERED 
(
	[CacheTimestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_RouteLookUps]    Script Date: 19/08/2019 4:51:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_RouteLookUps](
	[InternalRouteNo] [smallint] NOT NULL,
	[HeadboardRouteNo] [smallint] NOT NULL,
	[RouteNo] [smallint] NOT NULL,
	[AlphaNumericRouteNo] [varchar](5) NULL,
	[MainRouteNo] [varchar](5) NOT NULL,
	[VariantDestination] [varchar](50) NULL,
	[IsMainRoute]  AS (case when [RouteNo]=[HeadboardRouteNo] AND [RouteNo]=[InternalRouteNo] then (1) else (0) end),
	[LastModified] [datetime] NOT NULL,
 CONSTRAINT [PK_T_RouteLookUps] PRIMARY KEY CLUSTERED 
(
	[InternalRouteNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Routes_Stops]    Script Date: 19/08/2019 4:51:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Routes_Stops](
	[RouteNo] [smallint] NOT NULL,
	[StopID] [char](8) NOT NULL,
	[StopSequence] [tinyint] NOT NULL,
	[Distance] [decimal](6, 3) NOT NULL,
	[UpStop] [bit] NOT NULL,
	[FirstStop] [bit] NOT NULL,
	[LastStop] [bit] NOT NULL,
	[TimePoint] [varchar](4) NOT NULL,
	[TurnID] [tinyint] NULL,
	[TurnMessage] [varchar](150) NULL,
 CONSTRAINT [PK_T_Routes_Stops] PRIMARY KEY CLUSTERED 
(
	[RouteNo] ASC,
	[StopID] ASC,
	[UpStop] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Schedules]    Script Date: 19/08/2019 4:51:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Schedules](
	[ScheduleID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TripID] [int] NOT NULL,
	[RunNo] [char](5) NOT NULL,
	[StopID] [char](8) NOT NULL,
	[RouteNo] [smallint] NOT NULL,
	[OPRTimePoint] [bit] NOT NULL,
	[Time] [int] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
	[LowFloor] [bit] NOT NULL,
	[PublicTrip] [bit] NOT NULL,
 CONSTRAINT [PK_T_Schedules_1] PRIMARY KEY CLUSTERED 
(
	[TripID] ASC,
	[RunNo] ASC,
	[StopID] ASC,
	[RouteNo] ASC,
	[Time] ASC,
	[DayOfWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[T_Schedules] ADD  CONSTRAINT [DF_T_Schedules_StopNo]  DEFAULT ((0)) FOR [StopID]
GO

ALTER TABLE [dbo].[T_Schedules] ADD  CONSTRAINT [DF_T_Schedules_RouteNo]  DEFAULT ((0)) FOR [RouteNo]
GO

ALTER TABLE [dbo].[T_Schedules] ADD  CONSTRAINT [DF_T_Schedules_OPRTimePoint]  DEFAULT ((0)) FOR [OPRTimePoint]
GO

ALTER TABLE [dbo].[T_Schedules] ADD  CONSTRAINT [DF_T_Schedules_Time]  DEFAULT ((0)) FOR [Time]
GO

ALTER TABLE [dbo].[T_Schedules] ADD  CONSTRAINT [DF_T_Schedules_DayOfWeek]  DEFAULT ((0)) FOR [DayOfWeek]
GO

ALTER TABLE [dbo].[T_Schedules] ADD  CONSTRAINT [DF_T_Schedules_LowFloor]  DEFAULT ((0)) FOR [LowFloor]
GO

ALTER TABLE [dbo].[T_Schedules] ADD  CONSTRAINT [DF_T_Schedules_PublicTrips]  DEFAULT ((0)) FOR [PublicTrip]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'New 4 digit stop number' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Schedules', @level2type=N'COLUMN',@level2name=N'StopID'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Headboard Route No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Schedules', @level2type=N'COLUMN',@level2name=N'RouteNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Scheduled arrival time of the tram at the stop in seconds after midnight' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Schedules', @level2type=N'COLUMN',@level2name=N'Time'
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Stops]    Script Date: 19/08/2019 4:52:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Stops](
	[StopID] [char](8) NOT NULL,
	[Description] [varchar](255) NOT NULL,
	[PIDUnitID] [smallint] NOT NULL,
	[StopNo] [smallint] NOT NULL,
	[FlagStopNo] [varchar](10) NOT NULL,
	[PreCalculated] [bit] NOT NULL,
	[Monitored] [bit] NOT NULL,
	[Logged] [bit] NOT NULL,
	[StopName] [varchar](200) NOT NULL,
	[PISName] [varchar](200) NOT NULL,
	[DirectionID] [smallint] NOT NULL,
	[SuburbID] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Removed] [bit] NOT NULL,
	[IsCityStop] [bit] NOT NULL,
	[HasConnectingBuses] [bit] NOT NULL,
	[HasConnectingTrains] [bit] NOT NULL,
	[HasConnectingTrams] [bit] NOT NULL,
	[IsPlatformStop] [bit] NOT NULL,
	[StopLength] [smallint] NOT NULL,
	[LastModified] [datetime] NOT NULL,
 CONSTRAINT [PK_T_Stops] PRIMARY KEY CLUSTERED 
(
	[StopID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_SubVehicleInformation]    Script Date: 19/08/2019 4:52:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_SubVehicleInformation](
	[VehicleID] [smallint] NOT NULL,
	[Color] [char](1) NULL,
	[VehicleType] [char](1) NULL,
	[VehicleNo] [smallint] NOT NULL,
	[Class] [varchar](3) NULL,
	[Run] [char](5) NULL,
	[RouteNo] [smallint] NOT NULL,
	[HeadBoardRouteNo] [smallint] NOT NULL,
	[Deviation] [int] NOT NULL,
	[Distance] [decimal](6, 3) NOT NULL,
	[Down] [bit] NOT NULL,
	[End1Active] [bit] NOT NULL,
	[End2Active] [bit] NOT NULL,
	[SilentAlarm] [bit] NOT NULL,
	[OffRoute] [bit] NOT NULL,
	[Ghost] [bit] NOT NULL,
	[NotToSchedule] [bit] NOT NULL,
	[Unscheduled] [bit] NOT NULL,
	[InService] [bit] NOT NULL,
	[AtLayover] [bit] NOT NULL,
	[DoNotDraw] [bit] NOT NULL,
	[Destination] [char](4) NULL,
	[AvmTimestamp] [datetime] NOT NULL,
	[FileTimestamp] [datetime] NOT NULL,
	[SequenceID] [int] NOT NULL,
	[StopID] [varchar](8) NULL,
 CONSTRAINT [PK_T_SubVehicleInformation] PRIMARY KEY CLUSTERED 
(
	[VehicleID] ASC,
	[FileTimestamp] DESC,
	[SequenceID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Temp_Schedules]    Script Date: 19/08/2019 4:52:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Temp_Schedules](
	[ScheduleID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TripID] [int] NOT NULL,
	[RunNo] [char](5) NOT NULL,
	[StopID] [char](8) NOT NULL,
	[RouteNo] [smallint] NOT NULL,
	[OPRTimePoint] [bit] NOT NULL,
	[Time] [int] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
	[LowFloor] [bit] NOT NULL,
	[PublicTrip] [bit] NOT NULL,
 CONSTRAINT [PK_T_Temp_Schedules_1] PRIMARY KEY CLUSTERED 
(
	[TripID] ASC,
	[RunNo] ASC,
	[StopID] ASC,
	[RouteNo] ASC,
	[Time] ASC,
	[DayOfWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[T_Temp_Schedules] ADD  CONSTRAINT [DF_T_Temp_Schedules_OPRTimePoint]  DEFAULT ((0)) FOR [OPRTimePoint]
GO

ALTER TABLE [dbo].[T_Temp_Schedules] ADD  CONSTRAINT [DF_T_Temp_Schedules_DayOfWeek]  DEFAULT ((0)) FOR [DayOfWeek]
GO

ALTER TABLE [dbo].[T_Temp_Schedules] ADD  CONSTRAINT [DF_T_Temp_Schedules_LowFloor]  DEFAULT ((0)) FOR [LowFloor]
GO

ALTER TABLE [dbo].[T_Temp_Schedules] ADD  CONSTRAINT [DF_T_Temp_Schedules_PublicTrip]  DEFAULT ((0)) FOR [PublicTrip]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Temp_SchedulesDetails]    Script Date: 19/08/2019 4:53:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Temp_SchedulesDetails](
	[ArrivalTime] [varchar](50) NULL,
	[StopID] [varchar](50) NULL,
	[TripID] [varchar](50) NULL,
	[RunNo] [varchar](50) NULL,
	[OPRTimePoint] [varchar](50) NULL
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Temp_SchedulesMaster]    Script Date: 19/08/2019 4:53:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Temp_SchedulesMaster](
	[TramClass] [varchar](50) NULL,
	[HeadboardNo] [varchar](50) NULL,
	[RouteNo] [varchar](50) NULL,
	[RunNo] [varchar](50) NULL,
	[StartDate] [varchar](50) NULL,
	[TripNo] [varchar](15) NULL,
	[PublicTrip] [varchar](50) NULL
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Temp_Trips]    Script Date: 19/08/2019 4:53:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Temp_Trips](
	[TripID] [int] NULL,
	[RunNo] [char](5) NOT NULL,
	[RouteNo] [smallint] NOT NULL,
	[FirstTP] [char](4) NOT NULL,
	[FirstTime] [int] NOT NULL,
	[EndTP] [char](4) NOT NULL,
	[EndTime] [int] NOT NULL,
	[AtLayoverTime] [smallint] NOT NULL,
	[NextRouteNo] [smallint] NULL,
	[UpDirection] [bit] NOT NULL,
	[LowFloor] [bit] NOT NULL,
	[TripDistance] [decimal](6, 3) NULL,
	[PublicTrip] [bit] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
 CONSTRAINT [PK_T_Temp_Trips] PRIMARY KEY CLUSTERED 
(
	[RunNo] ASC,
	[RouteNo] ASC,
	[FirstTime] ASC,
	[DayOfWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[T_Temp_Trips] ADD  CONSTRAINT [DF_T_Temp_Trips_PublicTrip]  DEFAULT ((0)) FOR [PublicTrip]
GO

ALTER TABLE [dbo].[T_Temp_Trips] ADD  CONSTRAINT [DF_T_Temp_Trips_DayOfWeek]  DEFAULT ((0)) FOR [DayOfWeek]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hastus Block No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'RunNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Headboard Route No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'RouteNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time Point where the trip starts' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'FirstTP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trip start time in seconds after midnight' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'FirstTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time Point where the trip ends' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'EndTP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trip end time in seconds after midnight' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'EndTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time at layover in minutes' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'AtLayoverTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Vehicle''s next trip''s Headboard Route No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips', @level2type=N'COLUMN',@level2name=N'NextRouteNo'
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Trams]    Script Date: 19/08/2019 4:53:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Trams](
	[TramNo] [smallint] NOT NULL,
	[Class] [varchar](3) NOT NULL,
	[LowFloor] [bit] NOT NULL,
	[AirConditioned] [bit] NOT NULL,
 CONSTRAINT [PK_T_Trams] PRIMARY KEY CLUSTERED 
(
	[TramNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_Trips]    Script Date: 19/08/2019 4:54:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_Trips](
	[TripID] [int] NULL,
	[RunNo] [char](5) NOT NULL,
	[RouteNo] [smallint] NOT NULL,
	[FirstTP] [char](4) NOT NULL,
	[FirstTime] [int] NOT NULL,
	[EndTP] [char](4) NOT NULL,
	[EndTime] [int] NOT NULL,
	[AtLayoverTime] [smallint] NOT NULL,
	[NextRouteNo] [smallint] NULL,
	[UpDirection] [bit] NOT NULL,
	[LowFloor] [bit] NOT NULL,
	[TripDistance] [decimal](6, 3) NULL,
	[PublicTrip] [bit] NOT NULL,
	[DayOfWeek] [tinyint] NOT NULL,
 CONSTRAINT [PK_T_Trips] PRIMARY KEY CLUSTERED 
(
	[RunNo] ASC,
	[RouteNo] ASC,
	[FirstTime] ASC,
	[DayOfWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[T_Trips] ADD  CONSTRAINT [DF_T_Trips_PublicTrip]  DEFAULT ((0)) FOR [PublicTrip]
GO

ALTER TABLE [dbo].[T_Trips] ADD  CONSTRAINT [DF_T_Trips_DayOfWeek]  DEFAULT ((0)) FOR [DayOfWeek]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hastus Block No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'RunNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Headboard Route No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'RouteNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time Point where the trip starts' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'FirstTP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trip start time in seconds after midnight' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'FirstTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time Point where the trip ends' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'EndTP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trip end time in seconds after midnight' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'EndTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time at layover in minutes' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'AtLayoverTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Vehicle''s next trip''s Headboard Route No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Trips', @level2type=N'COLUMN',@level2name=N'NextRouteNo'
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_VehicleInformation]    Script Date: 19/08/2019 4:54:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_VehicleInformation](
	[VehicleID] [smallint] NOT NULL,
	[Color] [char](1) NULL,
	[VehicleType] [char](1) NULL,
	[VehicleNo] [smallint] NOT NULL,
	[Class] [varchar](3) NULL,
	[Run] [char](5) NULL,
	[RouteNo] [smallint] NOT NULL,
	[HeadBoardRouteNo] [smallint] NOT NULL,
	[Deviation] [int] NOT NULL,
	[Distance] [decimal](6, 3) NOT NULL,
	[Down] [bit] NOT NULL,
	[End1Active] [bit] NOT NULL,
	[End2Active] [bit] NOT NULL,
	[SilentAlarm] [bit] NOT NULL,
	[OffRoute] [bit] NOT NULL,
	[Ghost] [bit] NOT NULL,
	[NotToSchedule] [bit] NOT NULL,
	[Unscheduled] [bit] NOT NULL,
	[InService] [bit] NOT NULL,
	[AtLayover] [bit] NOT NULL,
	[DoNotDraw] [bit] NOT NULL,
	[Destination] [char](4) NULL,
	[AvmTimestamp] [datetime] NOT NULL,
	[FileTimestamp] [datetime] NOT NULL,
	[SequenceID] [int] NOT NULL,
	[StopID] [varchar](8) NULL,
 CONSTRAINT [PK_T_VehicleInformation] PRIMARY KEY CLUSTERED 
(
	[VehicleID] ASC,
	[FileTimestamp] DESC,
	[SequenceID] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

USE [tramtracker]
GO

/****** Object:  Table [dbo].[T_VehicleRunAssignments]    Script Date: 19/08/2019 4:54:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[T_VehicleRunAssignments](
	[VehicleID] [smallint] NOT NULL,
	[Color] [char](1) NULL,
	[VehicleType] [char](1) NULL,
	[VehicleNo] [smallint] NOT NULL,
	[Class] [varchar](3) NULL,
	[RunNo] [char](5) NULL,
	[RouteNo] [smallint] NOT NULL,
	[HeadBoardRouteNo] [smallint] NOT NULL,
	[Deviation] [int] NOT NULL,
	[Distance] [decimal](6, 3) NOT NULL,
	[Down] [bit] NOT NULL,
	[End1Active] [bit] NOT NULL,
	[End2Active] [bit] NOT NULL,
	[SilentAlarm] [bit] NOT NULL,
	[OffRoute] [bit] NOT NULL,
	[Ghost] [bit] NOT NULL,
	[NotToSchedule] [bit] NOT NULL,
	[Unscheduled] [bit] NOT NULL,
	[InService] [bit] NOT NULL,
	[AtLayover] [bit] NOT NULL,
	[DoNotDraw] [bit] NOT NULL,
	[Destination] [char](4) NULL,
	[AvmTimestamp] [datetime] NOT NULL,
	[FileTimestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_T_VehicleRunAssignments] PRIMARY KEY CLUSTERED 
(
	[VehicleID] ASC,
	[FileTimestamp] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO





