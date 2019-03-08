IF EXISTS (SELECT 1 from sys.objects WHERE name = 'T_Preferences' )
    PRINT 'T_Preferences already exists'
ELSE
BEGIN
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
END
GO

