DECLARE @RunId int = 1
SELECT * 
FROM Havm2TTComparisonRun r
JOIN Havm2TTComparisonRunTable rt ON rt.Havm2TTComparisonRunId = r.Id
WHERE r.Id = @RunId

SELECT *
FROM Havm2TTComparison_T_Temp_Trips_MissingFromNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY [DayOfWeek], FirstTime, TripID

SELECT *
FROM Havm2TTComparison_T_Temp_Trips_ExtraInNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY [DayOfWeek], FirstTime, TripID

SELECT
 '>>' [ ], old.TripID			, new.TripID		, CASE WHEN old.TripID = new.TripID THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.RunNo			, new.RunNo			, CASE WHEN old.RunNo = new.RunNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.RouteNo			, new.RouteNo		, CASE WHEN old.RouteNo = new.RouteNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.FirstTP			, new.FirstTP		, CASE WHEN old.FirstTP = new.FirstTP THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.FirstTime		, new.FirstTime		, CASE WHEN old.FirstTime = new.FirstTime THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.EndTP			, new.EndTP			, CASE WHEN old.EndTP = new.EndTP THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.EndTime			, new.EndTime		, CASE WHEN old.EndTime = new.EndTime THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.AtLayoverTime	, new.AtLayoverTime	, CASE WHEN old.AtLayoverTime = new.AtLayoverTime THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.NextRouteNo		, new.NextRouteNo	, CASE WHEN old.NextRouteNo = new.NextRouteNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.UpDirection		, new.UpDirection	, CASE WHEN old.UpDirection = new.UpDirection THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.LowFloor			, new.LowFloor		, CASE WHEN old.LowFloor = new.LowFloor THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.TripDistance		, new.TripDistance	, CASE WHEN old.TripDistance = new.TripDistance THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.PublicTrip		, new.PublicTrip	, CASE WHEN old.PublicTrip = new.PublicTrip THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.[DayOfWeek]		, new.[DayOfWeek]	, CASE WHEN old.[DayOfWeek] = new.[DayOfWeek] THEN 1 ELSE 0 END [Equal]
FROM Havm2TTComparison_T_Temp_Trips_Differing old
JOIN Havm2TTComparison_T_Temp_Trips_Differing new ON new.PairIdentifier = old.PairIdentifier AND new.IsExisting = 0
WHERE old.Havm2TTComparisonRunId = @RunId
AND old.IsExisting = 1
ORDER BY old.[DayOfWeek], old.RunNo, old.FirstTime

SELECT *
FROM Havm2TTComparison_T_Temp_Schedules_MissingFromNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY [DayOfWeek], TripID, Time

SELECT *
FROM Havm2TTComparison_T_Temp_Schedules_ExtraInNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY [DayOfWeek], TripID, Time

SELECT 
 '>>' [ ], old.TripID			, new.TripID		, CASE WHEN old.TripID = new.TripID THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.RunNo			, new.RunNo			, CASE WHEN old.RunNo = new.RunNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.StopID			, new.StopID		, CASE WHEN old.StopID = new.StopID THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.RouteNo			, new.RouteNo		, CASE WHEN old.RouteNo = new.RouteNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.Time				, new.Time			, CASE WHEN old.Time = new.Time THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.DayOfWeek		, new.DayOfWeek		, CASE WHEN old.DayOfWeek = new.DayOfWeek THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.LowFloor			, new.LowFloor		, CASE WHEN old.LowFloor = new.LowFloor THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.PublicTrip		, new.PublicTrip	, CASE WHEN old.PublicTrip = new.PublicTrip THEN 1 ELSE 0 END [Equal]
FROM Havm2TTComparison_T_Temp_Schedules_Differing old
JOIN Havm2TTComparison_T_Temp_Schedules_Differing new ON new.PairIdentifier = old.PairIdentifier AND new.IsExisting = 0
WHERE old.Havm2TTComparisonRunId = @RunId
AND old.IsExisting = 1
ORDER BY old.[DayOfWeek], old.TripID, old.Time

SELECT *
FROM Havm2TTComparison_T_Temp_SchedulesMaster_MissingFromNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY StartDate, RunNo, TripNo

SELECT *
FROM Havm2TTComparison_T_Temp_SchedulesMaster_ExtraInNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY StartDate, RunNo, TripNo

SELECT 
 '>>' [ ], old.TramClass		, new.TramClass		, CASE WHEN old.TramClass = new.TramClass THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.HeadboardNo		, new.HeadboardNo	, CASE WHEN old.HeadboardNo = new.HeadboardNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.RouteNo			, new.RouteNo		, CASE WHEN old.RouteNo = new.RouteNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.RunNo			, new.RunNo			, CASE WHEN old.RunNo = new.RunNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.StartDate		, new.StartDate		, CASE WHEN old.StartDate = new.StartDate THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.TripNo			, new.TripNo		, CASE WHEN old.TripNo = new.TripNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.PublicTrip		, new.PublicTrip	, CASE WHEN old.PublicTrip = new.PublicTrip THEN 1 ELSE 0 END [Equal]
FROM Havm2TTComparison_T_Temp_SchedulesMaster_Differing old
JOIN Havm2TTComparison_T_Temp_SchedulesMaster_Differing new ON new.PairIdentifier = old.PairIdentifier AND new.IsExisting = 0
WHERE old.Havm2TTComparisonRunId = @RunId
AND old.IsExisting = 1
ORDER BY old.StartDate, old.RunNo, old.TripNo

SELECT *
FROM Havm2TTComparison_T_Temp_SchedulesDetails_MissingFromNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY RunNo, TripID,ArrivalTime

SELECT *
FROM Havm2TTComparison_T_Temp_SchedulesDetails_ExtraInNew
WHERE Havm2TTComparisonRunId = @RunId
ORDER BY RunNo, TripID,ArrivalTime

SELECT 
 '>>' [ ], old.ArrivalTime		, new.ArrivalTime	, CASE WHEN old.ArrivalTime = new.ArrivalTime THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.StopID			, new.StopID		, CASE WHEN old.StopID = new.StopID THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.TripID			, new.TripID		, CASE WHEN old.TripID = new.TripID THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.RunNo			, new.RunNo			, CASE WHEN old.RunNo = new.RunNo THEN 1 ELSE 0 END [Equal]
,'>>' [ ], old.OPRTimePoint		, new.OPRTimePoint	, CASE WHEN old.OPRTimePoint = new.OPRTimePoint THEN 1 ELSE 0 END [Equal]
FROM Havm2TTComparison_T_Temp_SchedulesDetails_Differing old
JOIN Havm2TTComparison_T_Temp_SchedulesDetails_Differing new ON new.PairIdentifier = old.PairIdentifier AND new.IsExisting = 0
WHERE old.Havm2TTComparisonRunId = @RunId
AND old.IsExisting = 1
ORDER BY old.RunNo, old.TripID, old.ArrivalTime