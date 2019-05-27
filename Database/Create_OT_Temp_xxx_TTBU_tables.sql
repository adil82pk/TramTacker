

CREATE TABLE [dbo].[T_Temp_Trips_TTBU](
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
 CONSTRAINT [PK_T_Temp_Trips_TTBU] PRIMARY KEY CLUSTERED 
(
	[RunNo] ASC,
	[RouteNo] ASC,
	[FirstTime] ASC,
	[DayOfWeek] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[T_Temp_Trips_TTBU] ADD  CONSTRAINT [DF_T_Temp_Trips_TTBU_PublicTrip]  DEFAULT ((0)) FOR [PublicTrip]
GO

ALTER TABLE [dbo].[T_Temp_Trips_TTBU] ADD  CONSTRAINT [DF_T_Temp_Trips_TTBU_DayOfWeek]  DEFAULT ((0)) FOR [DayOfWeek]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hastus Block No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'RunNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Headboard Route No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'RouteNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time Point where the trip starts' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'FirstTP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trip start time in seconds after midnight' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'FirstTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time Point where the trip ends' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'EndTP'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Trip end time in seconds after midnight' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'EndTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Time at layover in minutes' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'AtLayoverTime'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Vehicle''s next trip''s Headboard Route No' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Temp_Trips_TTBU', @level2type=N'COLUMN',@level2name=N'NextRouteNo'
GO


CREATE TABLE [dbo].[T_Temp_Schedules_TTBU](
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
 CONSTRAINT [PK_T_Temp_Schedules_TTBU_1] PRIMARY KEY CLUSTERED 
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

ALTER TABLE [dbo].[T_Temp_Schedules_TTBU] ADD  CONSTRAINT [DF_T_Temp_Schedules_TTBU_OPRTimePoint]  DEFAULT ((0)) FOR [OPRTimePoint]
GO

ALTER TABLE [dbo].[T_Temp_Schedules_TTBU] ADD  CONSTRAINT [DF_T_Temp_Schedules_TTBU_DayOfWeek]  DEFAULT ((0)) FOR [DayOfWeek]
GO

ALTER TABLE [dbo].[T_Temp_Schedules_TTBU] ADD  CONSTRAINT [DF_T_Temp_Schedules_TTBU_LowFloor]  DEFAULT ((0)) FOR [LowFloor]
GO

ALTER TABLE [dbo].[T_Temp_Schedules_TTBU] ADD  CONSTRAINT [DF_T_Temp_Schedules_TTBU_PublicTrip]  DEFAULT ((0)) FOR [PublicTrip]
GO

CREATE TABLE [dbo].[T_Temp_SchedulesMaster_TTBU](
	[TramClass] [varchar](50) NULL,
	[HeadboardNo] [varchar](50) NULL,
	[RouteNo] [varchar](50) NULL,
	[RunNo] [varchar](50) NULL,
	[StartDate] [varchar](50) NULL,
	[TripNo] [varchar](15) NULL,
	[PublicTrip] [varchar](50) NULL
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[T_Temp_SchedulesDetails_TTBU](
	[ArrivalTime] [varchar](50) NULL,
	[StopID] [varchar](50) NULL,
	[TripID] [varchar](50) NULL,
	[RunNo] [varchar](50) NULL,
	[OPRTimePoint] [varchar](50) NULL
) ON [PRIMARY]
GO
