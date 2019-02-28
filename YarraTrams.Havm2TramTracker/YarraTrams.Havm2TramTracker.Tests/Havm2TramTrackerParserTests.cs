using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Models;
using System.Linq;

namespace YarraTrams.Havm2TramTracker.Tests
{
    [TestClass]
    public class Havm2TramTrackerParserTests
    {
        [TestMethod]
        public void TestHavmTripsToTramTrackerTripsWithGoodData()
        {
            // arrange
            var trips = new List<Models.HavmTrip>();

            trips.Add(new Models.HavmTrip
            {
                HastusTripId = 1,
                Block = "Block 1",
                Headboard = "86",
                StartTimepoint = "ncob",
                StartTimeSam = 3600,
                EndTimepoint = "mpnd",
                EndTimeSam = 7200,
                HeadwayNextSeconds = 120,
                NextRoute = "86",
                Direction = "DOWN",
                VehicleType = "Z",
                DistanceMetres = 10000,
                IsPublic = true,
                OperationalDay = new DateTime(2019, 2, 1),
                Stops = new List<Models.HavmTripStop>()
            });

            var tripsService = new Processor.Services.TramTrackerTripsService();
            List<Models.TramTrackerTrips> tramTrackerTrips;

            // act
            tramTrackerTrips = tripsService.FromHavmTrips(trips, false);

            // assert
            Assert.IsTrue(trips.Count == tramTrackerTrips.Count, "Number of records in tramTrackerTrips ({1:d}) doesn't match number of records in Trip class list ({0:d}).", trips.Count, tramTrackerTrips.Count);
            //Todo: check more stuff. Everything! Only once we've settled on the data contract.
            Assert.IsTrue(trips[0].HastusTripId == tramTrackerTrips[0].TripID, "TripId field in tramTrackerTrips ({1}) doesn't match HastusTripId from Trip class ({0}).", trips[0].HastusTripId, tramTrackerTrips[0].TripID);
        }

        [TestMethod]
        public void TestHavmTripsToTramTrackerTripsWithInvalidTripDirection()
        {
            // arrange
            var trips = new List<Models.HavmTrip>();

            trips.Add(new Models.HavmTrip
            {
                HastusTripId = 1,
                Block = "Block 1",
                Headboard = "86",
                StartTimepoint = "ncob",
                StartTimeSam = 3600,
                EndTimepoint = "mpnd",
                EndTimeSam = 7200,
                HeadwayNextSeconds = 120,
                NextRoute = "86",
                Direction = "ZIGZAG",
                VehicleType = "Z",
                DistanceMetres = 10000,
                IsPublic = true,
                OperationalDay = new DateTime(2019, 2, 1),
                Stops = new List<Models.HavmTripStop>()
            });

            var tripsService = new Processor.Services.TramTrackerTripsService();
            List<Models.TramTrackerTrips> tramTrackerTrips;

            // act
            tramTrackerTrips = tripsService.FromHavmTrips(trips, false);

            // assert
            Assert.IsTrue(tramTrackerTrips.Count == 0, "Expecting zero records in the Trip class list but found {0:d} records.", tramTrackerTrips.Count);
        }

        [TestMethod]
        public void TestTripsToSchedulesWithGoodData()
        {
            // arrange
            var trips = new List<Models.HavmTrip>();

            trips.Add(new Models.HavmTrip
            {
                HastusTripId = 1,
                Block = "Block 1",
                Headboard = "86",
                Route = "86",
                StartTimepoint = "ncob",
                StartTimeSam = 3600,
                EndTimepoint = "mpnd",
                EndTimeSam = 7200,
                HeadwayNextSeconds = 120,
                NextRoute = "86",
                Direction = "DOWN",
                VehicleType = "Z",
                DistanceMetres = 10000,
                IsPublic = true,
                OperationalDay = new DateTime(2019, 2, 1),
                Stops = new List<Models.HavmTripStop> {
                    new HavmTripStop
                        {
                            PassingTime = new TimeSpan(0, 1, 0, 0, 0),
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTime = new TimeSpan(0, 2, 0, 0, 0),
                            HastusStopId = "3398",
                            IsMonitoredOPRReliability = true
                        }
                    }
            });

            var schedulesService = new Processor.Services.TramTrackerSchedulesService();
            List<Models.TramTrackerSchedules> schedules;

            // act
            schedules = schedulesService.FromHavmTrips(trips, false);

            // assert
            Assert.IsTrue(trips.Select(x => x.Stops.Count()).Sum() == schedules.Count, "Number of records in DataTable ({1:d}) doesn't match number of total stops in Trip class list ({0:d}).", trips.Select(x => x.Stops.Count()).Sum(), schedules.Count);
            //Todo: check more stuff. Everything! Only once we've settled on the data contract.
            Assert.IsTrue(trips[0].HastusTripId == schedules[0].TripID, "TripId field in DataTable ({1}) doesn't match HastusTripId from Trip class ({0}).", trips[0].HastusTripId, schedules[0].TripID);
        }

        [TestMethod]
        public void TestTripsToSchedulesMasterWithGoodData()
        {
            // arrange
            var trips = new List<Models.HavmTrip>();

            trips.Add(new Models.HavmTrip
            {
                HastusTripId = 1,
                Block = "Block 1",
                Headboard = "86",
                Route = "86",
                StartTimepoint = "ncob",
                StartTimeSam = 3600,
                EndTimepoint = "mpnd",
                EndTimeSam = 7200,
                HeadwayNextSeconds = 120,
                NextRoute = "86",
                Direction = "DOWN",
                VehicleType = "Z",
                DistanceMetres = 10000,
                IsPublic = true,
                OperationalDay = new DateTime(2019, 2, 1),
                Stops = new List<Models.HavmTripStop> {
                    new HavmTripStop
                        {
                            PassingTime = new TimeSpan(0, 1, 0, 0, 0),
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTime = new TimeSpan(0, 2, 0, 0, 0),
                            HastusStopId = "3398",
                            IsMonitoredOPRReliability = true
                        }
                    }
            });

            var schedulesMasterService = new Processor.Services.TramTrackerSchedulesMasterService();
            List<Models.TramTrackerSchedulesMaster> schedulesMasters;

            // act
            schedulesMasters = schedulesMasterService.FromHavmTrips(trips,false);

            // assert
            Assert.IsTrue(trips.Count == schedulesMasters.Count, "Number of records in {0} ({2:d}) doesn't match number of total trips in Trip class list ({1:d}).", schedulesMasters.GetType().Name, trips.Count, schedulesMasters.Count);
            //Todo: check more stuff. Everything! Only once we've settled on the data contract.
            Assert.IsTrue(trips[0].Headboard == schedulesMasters[0].HeadboardNo, "HeadboardNo field in DataTable ({1}) doesn't match Headboard from Trip class ({0}).", trips[0].HastusTripId, schedulesMasters[0].HeadboardNo);
        }

        [TestMethod]
        public void TestTripsToSchedulesDetailsWithGoodData()
        {
            // arrange
            var trips = new List<Models.HavmTrip>();

            trips.Add(new Models.HavmTrip
            {
                HastusTripId = 1,
                Block = "Block 1",
                Headboard = "86",
                Route = "86",
                StartTimepoint = "ncob",
                StartTimeSam = 3600,
                EndTimepoint = "mpnd",
                EndTimeSam = 7200,
                HeadwayNextSeconds = 120,
                NextRoute = "86",
                Direction = "DOWN",
                VehicleType = "Z",
                DistanceMetres = 10000,
                IsPublic = true,
                OperationalDay = new DateTime(2019, 2, 1),
                Stops = new List<Models.HavmTripStop> {
                    new HavmTripStop
                        {
                            PassingTime = new TimeSpan(0, 1, 0, 0, 0),
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTime = new TimeSpan(0, 2, 0, 0, 0),
                            HastusStopId = "3398",
                            IsMonitoredOPRReliability = true
                        }
                    }
            });

            var schedulesDetailsService = new Processor.Services.TramTrackerSchedulesDetailsService();
            List<Models.TramTrackerSchedulesDetails> schedulesDetailss;

            // act
            schedulesDetailss = schedulesDetailsService.FromHavmTrips(trips, false);

            // assert
            Assert.IsTrue(trips.Select(x => x.Stops.Count()).Sum() == schedulesDetailss.Count, "Number of records in {0} ({2:d}) doesn't match number of total stops in Trip class list ({1:d}).", schedulesDetailss.GetType().Name, trips.Select(x => x.Stops.Count()).Sum(), schedulesDetailss.Count);
            //Todo: check more stuff. Everything! Only once we've settled on the data contract.
        }

        [TestMethod]
        public void TestJsonToTripsWithStandardJson()
        {
            // arrange
            #region "JSON string containing a single trip"
            var jsonString = @"
                            [
                                {
                                    ""hastusTripId"": 62770458,
                                    ""block"": ""gh02- 27 This is wrong"",
                                    ""displayCode"": ""61"",
                                    ""startTimepoint"": ""ebtn"",
                                    ""startTimeSam"": 86400,
                                    ""endTimepoint"": ""scj1"",
                                    ""endTimeSam"": 86760,
                                    ""headwayNextSeconds"": 0,
                                    ""nextDisplayCode"": ""61"",
                                    ""direction"": ""DOWN"",
                                    ""vehicleType"": ""b"",
                                    ""distanceMetres"": 3074,
                                    ""isPublic"": true,
                                    ""operationalDay"": ""2018-04-23T00:00:00"",
                                    ""stops"": [
                                        {
                                            ""passingTime"": ""00:00:00"",
                                            ""hastusStopId"": ""1135"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:01:00"",
                                            ""hastusStopId"": ""1134"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:01:00"",
                                            ""hastusStopId"": ""1133"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:02:00"",
                                            ""hastusStopId"": ""1132"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:02:00"",
                                            ""hastusStopId"": ""1131"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:03:00"",
                                            ""hastusStopId"": ""1130"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:03:00"",
                                            ""hastusStopId"": ""1129"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:04:00"",
                                            ""hastusStopId"": ""1128"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:04:00"",
                                            ""hastusStopId"": ""1127"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:05:00"",
                                            ""hastusStopId"": ""1126"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:05:00"",
                                            ""hastusStopId"": ""1125"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:06:00"",
                                            ""hastusStopId"": ""1124"",
                                            ""isMonitoredOPRReliability"": false
                                        }
                                    ]
                                }
                            ]";
            #endregion
            // act
            var trips = Processor.Processor.CopyJsonToTrips(jsonString);

            // assert
            Assert.IsTrue(trips.Count == 1, "Number of records in Trip class list ({1:d}) doesn't match number of records in the JSON ({0:d}).", 1, trips.Count);
            //Todo: check more stuff
            Assert.IsTrue(trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId == "1124", "We expected the HastusStopId on the final stop of the first trip to be '1124' but it appears to be '{0}' instead.", trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId);
        }

        [TestMethod]
        public void TestJsonToTripsWithExtraFieldInJson()
        {
            // arrange
            #region "JSON string containing a single trip and more fields than we expect"
            var jsonString = @"
                            [
                                {
                                    ""ANewFieldThatWeDoNotKnowAbout"": ""A new value that we are not expecting"",
                                    ""hastusTripId"": 62770458,
                                    ""block"": ""gh02- 27 This is wrong"",
                                    ""displayCode"": ""61"",
                                    ""startTimepoint"": ""ebtn"",
                                    ""startTimeSam"": 86400,
                                    ""endTimepoint"": ""scj1"",
                                    ""endTimeSam"": 86760,
                                    ""headwayNextSeconds"": 0,
                                    ""nextDisplayCode"": ""61"",
                                    ""direction"": ""DOWN"",
                                    ""vehicleType"": ""b"",
                                    ""distanceMetres"": 3074,
                                    ""isPublic"": true,
                                    ""operationalDay"": ""2018-04-23T00:00:00"",
                                    ""stops"": [
                                        {
                                            ""passingTime"": ""00:00:00"",
                                            ""hastusStopId"": ""1135"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:01:00"",
                                            ""hastusStopId"": ""1134"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:01:00"",
                                            ""hastusStopId"": ""1133"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:02:00"",
                                            ""hastusStopId"": ""1132"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:02:00"",
                                            ""hastusStopId"": ""1131"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:03:00"",
                                            ""hastusStopId"": ""1130"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:03:00"",
                                            ""hastusStopId"": ""1129"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:04:00"",
                                            ""hastusStopId"": ""1128"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:04:00"",
                                            ""hastusStopId"": ""1127"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:05:00"",
                                            ""hastusStopId"": ""1126"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:05:00"",
                                            ""hastusStopId"": ""1125"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:06:00"",
                                            ""hastusStopId"": ""1124"",
                                            ""isMonitoredOPRReliability"": false
                                        }
                                    ]
                                }
                            ]";
            #endregion
            // act
            var trips = Processor.Processor.CopyJsonToTrips(jsonString);

            // assert
            Assert.IsTrue(trips.Count == 1, "Number of records in Trip class list ({1:d}) doesn't match number of records in the JSON ({0:d}).", 1, trips.Count);
            //Todo: check more stuff
            Assert.IsTrue(trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId == "1124", "We expected the HastusStopId on the final stop of the first trip to be '1124' but it appears to be '{0}' instead.", trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId);
        }

        [TestMethod]
        public void TestJsonToTripsWithMissingFieldInJson()
        {
            // arrange
            #region "JSON string containing a single trip but missing an important field"
            var jsonString = @"
                            [
                                {
                                    //Usually the Hastus Trip Id field is here
                                    ""block"": ""gh02- 27 This is wrong"",
                                    ""displayCode"": ""61"",
                                    ""startTimepoint"": ""ebtn"",
                                    ""startTimeSam"": 86400,
                                    ""endTimepoint"": ""scj1"",
                                    ""endTimeSam"": 86760,
                                    ""headwayNextSeconds"": 0,
                                    ""nextDisplayCode"": ""61"",
                                    ""direction"": ""DOWN"",
                                    ""vehicleType"": ""b"",
                                    ""distanceMetres"": 3074,
                                    ""isPublic"": true,
                                    ""operationalDay"": ""2018-04-23T00:00:00"",
                                    ""stops"": [
                                        {
                                            ""passingTime"": ""00:00:00"",
                                            ""hastusStopId"": ""1135"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:01:00"",
                                            ""hastusStopId"": ""1134"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:01:00"",
                                            ""hastusStopId"": ""1133"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:02:00"",
                                            ""hastusStopId"": ""1132"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:02:00"",
                                            ""hastusStopId"": ""1131"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:03:00"",
                                            ""hastusStopId"": ""1130"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:03:00"",
                                            ""hastusStopId"": ""1129"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:04:00"",
                                            ""hastusStopId"": ""1128"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:04:00"",
                                            ""hastusStopId"": ""1127"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:05:00"",
                                            ""hastusStopId"": ""1126"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:05:00"",
                                            ""hastusStopId"": ""1125"",
                                            ""isMonitoredOPRReliability"": false
                                        },
                                        {
                                            ""passingTime"": ""00:06:00"",
                                            ""hastusStopId"": ""1124"",
                                            ""isMonitoredOPRReliability"": false
                                        }
                                    ]
                                }
                            ]";
            #endregion
            // act
            var trips = Processor.Processor.CopyJsonToTrips(jsonString);

            // assert
            //Todo: check that derialisation fails in some way
            Assert.IsTrue(trips.Count == 1, "Number of records in Trip class list ({1:d}) doesn't match number of records in the JSON ({0:d}).", 1, trips.Count);
            Assert.IsTrue(trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId == "1124", "We expected the HastusStopId on the final stop of the first trip to be '1124' but it appears to be '{0}' instead.", trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId);
        }
    }
}
