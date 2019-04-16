using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Models;
using System.Linq;
using System.Linq.Expressions;

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
                            PassingTimeSam = 3600,
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTimeSam = 7200,
                            HastusStopId = "3398",
                            IsMonitoredOPRReliability = true
                        }
                    }
            });

            // Stop mapping
            string expectedStop1Value = "A Mapped Stop";
            string expectedStop2Value = "3398    ";
            Dictionary<int, string> stopMapping = new Dictionary<int, string>();
            stopMapping.Add(1626, expectedStop1Value);

            var schedulesService = new Processor.Services.TramTrackerSchedulesService();

            // act
            List<Models.TramTrackerSchedules> schedules = schedulesService.FromHavmTrips(trips, stopMapping, false);

            // assert
            Assert.IsTrue(trips.Select(x => x.Stops.Count()).Sum() == schedules.Count, "Number of records in schedules ({1:d}) doesn't match number of total stops in Trip class list ({0:d}).", trips.Select(x => x.Stops.Count()).Sum(), schedules.Count);
            //Todo: check more stuff. Everything! Only once we've settled on the data contract.
            Assert.IsTrue(trips[0].HastusTripId == schedules[0].TripID, "TripId field in schedules ({1}) doesn't match HastusTripId from Trip class ({0}).", trips[0].HastusTripId, schedules[0].TripID);
            Assert.IsTrue(schedules[0].StopID == expectedStop1Value, "StopID field in first schedule \"{1}\" doesn't match the expected StopID \"{0}\".", expectedStop1Value, schedules[0].StopID);
            Assert.IsTrue(schedules[1].StopID == expectedStop2Value, "StopID field in second schedule \"{1}\" doesn't match the expected StopID \"{0}\".", expectedStop2Value, schedules[1].StopID);

        }

        [TestMethod]
        public void TestTripsToSchedulesWithNonPublicTrip()
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
                IsPublic = false,
                OperationalDay = new DateTime(2019, 2, 1),
                Stops = new List<Models.HavmTripStop> {
                    new HavmTripStop
                        {
                            PassingTimeSam = 3600,
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTimeSam = 7200,
                            HastusStopId = "3398",
                            IsMonitoredOPRReliability = true
                        }
                    }
            });

            // Stop mapping (empty for this test).
            Dictionary<int, string> stopMapping = new Dictionary<int, string>();

            var schedulesService = new Processor.Services.TramTrackerSchedulesService();

            // act
            List<Models.TramTrackerSchedules> schedules = schedulesService.FromHavmTrips(trips, stopMapping, false);

            // assert
            Assert.IsTrue(schedules.Count == 0, "Number of records in schedules ({0:d}) should be zero for a non public trip.", schedules.Count);
        }

        [TestMethod]
        public void TestTripsToSchedulesWithUnknownStop()
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
                Stops = new List<Models.HavmTripStop> {
                    new HavmTripStop
                        {
                            PassingTimeSam = 3600,
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTimeSam = 5400,
                            HastusStopId = "NotValid",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTimeSam = 7200,
                            HastusStopId = "3398",
                            IsMonitoredOPRReliability = true
                        }
                    }
            });

            // Stop mapping (empty for this test).
            Dictionary<int, string> stopMapping = new Dictionary<int, string>();

            var schedulesService = new Processor.Services.TramTrackerSchedulesService();

            // act
            List<Models.TramTrackerSchedules> schedules = schedulesService.FromHavmTrips(trips, stopMapping, false);

            // assert
            Assert.IsTrue(schedules.Count == 2, "Number of records in schedules ({0:d}) should be two for this trip because one of the three provided stops is invalid.", schedules.Count);
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
                            PassingTimeSam = 3600,
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTimeSam = 7200,
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
            Assert.IsTrue(trips[0].Headboard == schedulesMasters[0].HeadboardNo, "HeadboardNo field in schedulesMasters ({1}) doesn't match Headboard from Trip class ({0}).", trips[0].HastusTripId, schedulesMasters[0].HeadboardNo);
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
                            PassingTimeSam = 3600,
                            HastusStopId = "1626",
                            IsMonitoredOPRReliability = true
                        },
                    new HavmTripStop
                        {
                            PassingTimeSam = 7200,
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
            int hastusTripID = 76245555;
            string block = "kw05-  1";
            string headboard = "48";
            string route = "48";
            string startTimepoint = "nbal";
            int startTimeSam = 25920;
            string endTimepoint = "viha";
            int endTimeSam = 28320;
            int headwayNextSeconds = 1260;
            string nextRoute = "48";
            string direction = "UP";
            string vehicleType = "c1";
            int distanceMetres = 14122;
            bool isPublic = true;
            DateTime operationalDay = new DateTime(2019, 3, 10);
            int stopCount = 12;
            int firstStopPassingTimeSam = 25920;
            bool secondIsMonitoredOPRReliability = true;
            string lastStopId = "8018";

            #region "JSON string containing a single trip"
            string jsonString = @"
                                    [
                                        {
                                        ""hastusTripId"":76245555,
                                        ""block"":""kw05-  1"",
                                        ""headboard"":""48"",
                                        ""route"":""48"",
                                        ""startTimepoint"":""nbal"",
                                        ""startTimeSam"":25920,
                                        ""endTimepoint"":""viha"",
                                        ""endTimeSam"":28320,
                                        ""headwayNextSeconds"":1260,
                                        ""nextRoute"":""48"",
                                        ""direction"":""UP"",
                                        ""vehicleType"":""c1"",
                                        ""distanceMetres"":14122,
                                        ""isPublic"":true,
                                        ""operationalDay"":""2019-03-10T00:00:00"",
                                        ""stops"":[
                                            {
                                                ""passingTimeSam"":25920,
                                                ""hastusStopId"":""1896"",
                                                // This is where the isMonitoredOPRReliability fields goes but it's optional.
                                            },
                                            {
                                                ""passingTimeSam"":25980,
                                                ""hastusStopId"":""1895"",
                                                ""isMonitoredOPRReliability"":true
                                            },
                                            {
                                                ""passingTimeSam"":26940,
                                                ""hastusStopId"":""1923"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27000,
                                                ""hastusStopId"":""1922"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27480,
                                                ""hastusStopId"":""3710"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27540,
                                                ""hastusStopId"":""3709"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27540,
                                                ""hastusStopId"":""fls1"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27660,
                                                ""hastusStopId"":""3509"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27780,
                                                ""hastusStopId"":""3508"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28200,
                                                ""hastusStopId"":""3498"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28260,
                                                ""hastusStopId"":""3497"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28320,
                                                ""hastusStopId"":""8018"",
                                                ""isMonitoredOPRReliability"":false
                                            }
                                            ]
                                        }
                                    ]";
            #endregion
            
            // act
            var trips = Processor.Processor.CopyJsonToTrips(jsonString);

            // assert
            Assert.IsTrue(trips.Count == 1, "Number of records in Trip class list ({1:d}) doesn't match number of records in the JSON ({0:d}).", 1, trips.Count);
            Assert.IsTrue(trips[0].HastusTripId == hastusTripID, "Expecting HastusTripId value of '{0}' but got '{1}'.", hastusTripID, trips[0].HastusTripId);
            Assert.IsTrue(trips[0].Block == block, "Expecting Block value of '{0}' but got '{1}'.", block, trips[0].Block);
            Assert.IsTrue(trips[0].Headboard == headboard, "Expecting Headboard value of '{0}' but got '{1}'.", headboard, trips[0].Headboard);
            Assert.IsTrue(trips[0].Route == route, "Expecting Route value of '{0}' but got '{1}'.", route, trips[0].Route);
            Assert.IsTrue(trips[0].StartTimepoint == startTimepoint, "Expecting StartTimepoint value of '{0}' but got '{1}'.", startTimepoint, trips[0].StartTimepoint);
            Assert.IsTrue(trips[0].StartTimeSam == startTimeSam, "Expecting StartTimeSam value of '{0}' but got '{1}'.", startTimeSam, trips[0].StartTimeSam);
            Assert.IsTrue(trips[0].EndTimepoint == endTimepoint, "Expecting EndTimepoint value of '{0}' but got '{1}'.", endTimepoint, trips[0].EndTimepoint);
            Assert.IsTrue(trips[0].EndTimeSam == endTimeSam, "Expecting EndTimeSam value of '{0}' but got '{1}'.", endTimeSam, trips[0].EndTimeSam);
            Assert.IsTrue(trips[0].HeadwayNextSeconds == headwayNextSeconds, "Expecting HeadwayNextSeconds value of '{0}' but got '{1}'.", headwayNextSeconds, trips[0].HeadwayNextSeconds);
            Assert.IsTrue(trips[0].NextRoute == nextRoute, "Expecting NextRoute value of '{0}' but got '{1}'.", nextRoute, trips[0].NextRoute);
            Assert.IsTrue(trips[0].Direction == direction, "Expecting Direction value of '{0}' but got '{1}'.", direction, trips[0].Direction);
            Assert.IsTrue(trips[0].VehicleType == vehicleType, "Expecting VehicleType value of '{0}' but got '{1}'.", vehicleType, trips[0].VehicleType);
            Assert.IsTrue(trips[0].DistanceMetres == distanceMetres, "Expecting DistanceMetres value of '{0}' but got '{1}'.", distanceMetres, trips[0].DistanceMetres);
            Assert.IsTrue(trips[0].IsPublic == isPublic, "Expecting IsPublic value of '{0}' but got '{1}'.", isPublic, trips[0].IsPublic);
            Assert.IsTrue(trips[0].OperationalDay == operationalDay, "Expecting OperationalDay value of '{0}' but got '{1}'.", operationalDay, trips[0].OperationalDay);
            Assert.IsTrue(trips[0].Stops.Count == stopCount, "Expecting {0} stops but found {0}.", stopCount, trips[0].Stops.Count);
            Assert.IsTrue(trips[0].Stops[0].PassingTimeSam == firstStopPassingTimeSam, "We expected the PassingTime on the first stop of the first trip to be '{0}' but it appears to be '{1}' instead.", firstStopPassingTimeSam, trips[0].Stops[0].PassingTimeSam);
            Assert.IsTrue(trips[0].Stops[1].IsMonitoredOPRReliability == secondIsMonitoredOPRReliability, "We expected the isMonitoredOPRReliability on the second stop of the first trip to be '{0}' but it appears to be '{1}' instead.", secondIsMonitoredOPRReliability, trips[0].Stops[1].IsMonitoredOPRReliability);
            Assert.IsTrue(trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId == lastStopId, "We expected the HastusStopId on the final stop of the first trip to be '{0}' but it appears to be '{1}' instead.", lastStopId, trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId);
        }

        [TestMethod]
        public void TestJsonToTripsWithExtraFieldInJson()
        {
            // arrange
            int hastusTripID = 76245555;
            string block = "kw05-  1";
            string headboard = "48";
            string route = "48";
            string startTimepoint = "nbal";
            int startTimeSam = 25920;
            string endTimepoint = "viha";
            int endTimeSam = 28320;
            int headwayNextSeconds = 1260;
            string nextRoute = "48";
            string direction = "UP";
            string vehicleType = "c1";
            int distanceMetres = 14122;
            bool isPublic = true;
            DateTime operationalDay = new DateTime(2019,3,10);
            int stopCount = 12;
            int firstStopPassingTimeSam = 25920;
            bool secondIsMonitoredOPRReliability = true;
            string lastStopId = "8018";
            #region "JSON string containing a single trip and more fields than we expect"
            string jsonString = @"
                                    [
                                        {
                                        ""ANewFieldThatWeDoNotKnowAbout"": ""A new value that we are not expecting"",
                                        ""hastusTripId"":76245555,
                                        ""block"":""kw05-  1"",
                                        ""headboard"":""48"",
                                        ""route"":""48"",
                                        ""startTimepoint"":""nbal"",
                                        ""startTimeSam"":25920,
                                        ""endTimepoint"":""viha"",
                                        ""endTimeSam"":28320,
                                        ""headwayNextSeconds"":1260,
                                        ""nextRoute"":""48"",
                                        ""direction"":""UP"",
                                        ""vehicleType"":""c1"",
                                        ""distanceMetres"":14122,
                                        ""isPublic"":true,
                                        ""operationalDay"":""2019-03-10T00:00:00"",
                                        ""stops"":[
                                            {
                                                ""passingTimeSam"":25920,
                                                ""hastusStopId"":""1896"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":25980,
                                                ""hastusStopId"":""1895"",
                                                ""isMonitoredOPRReliability"":true
                                            },
                                            {
                                                ""passingTimeSam"":26940,
                                                ""hastusStopId"":""1923"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27000,
                                                ""hastusStopId"":""1922"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27480,
                                                ""hastusStopId"":""3710"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27540,
                                                ""hastusStopId"":""3709"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27540,
                                                ""hastusStopId"":""fls1"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27660,
                                                ""hastusStopId"":""3509"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27780,
                                                ""hastusStopId"":""3508"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28200,
                                                ""hastusStopId"":""3498"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28260,
                                                ""hastusStopId"":""3497"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28320,
                                                ""hastusStopId"":""8018"",
                                                ""isMonitoredOPRReliability"":false
                                            }
                                            ]
                                        }
                                    ]";
            #endregion
            // act
            var trips = Processor.Processor.CopyJsonToTrips(jsonString);
            
            // assert
            Assert.IsTrue(trips.Count == 1, "Number of records in Trip class list ({1:d}) doesn't match number of records in the JSON ({0:d}).", 1, trips.Count);
            Assert.IsTrue(trips[0].HastusTripId == hastusTripID, "Expecting HastusTripId value of '{0}' but got '{1}'.", hastusTripID, trips[0].HastusTripId);
            Assert.IsTrue(trips[0].Block == block, "Expecting Block value of '{0}' but got '{1}'.", block, trips[0].Block);
            Assert.IsTrue(trips[0].Headboard == headboard, "Expecting Headboard value of '{0}' but got '{1}'.", headboard, trips[0].Headboard);
            Assert.IsTrue(trips[0].Route == route, "Expecting Route value of '{0}' but got '{1}'.", route, trips[0].Route);
            Assert.IsTrue(trips[0].StartTimepoint == startTimepoint, "Expecting StartTimepoint value of '{0}' but got '{1}'.", startTimepoint, trips[0].StartTimepoint);
            Assert.IsTrue(trips[0].StartTimeSam == startTimeSam, "Expecting StartTimeSam value of '{0}' but got '{1}'.", startTimeSam, trips[0].StartTimeSam);
            Assert.IsTrue(trips[0].EndTimepoint == endTimepoint, "Expecting EndTimepoint value of '{0}' but got '{1}'.", endTimepoint, trips[0].EndTimepoint);
            Assert.IsTrue(trips[0].EndTimeSam == endTimeSam, "Expecting EndTimeSam value of '{0}' but got '{1}'.", endTimeSam, trips[0].EndTimeSam);
            Assert.IsTrue(trips[0].HeadwayNextSeconds == headwayNextSeconds, "Expecting HeadwayNextSeconds value of '{0}' but got '{1}'.", headwayNextSeconds, trips[0].HeadwayNextSeconds);
            Assert.IsTrue(trips[0].NextRoute == nextRoute, "Expecting NextRoute value of '{0}' but got '{1}'.", nextRoute, trips[0].NextRoute);
            Assert.IsTrue(trips[0].Direction == direction, "Expecting Direction value of '{0}' but got '{1}'.", direction, trips[0].Direction);
            Assert.IsTrue(trips[0].VehicleType == vehicleType, "Expecting VehicleType value of '{0}' but got '{1}'.", vehicleType, trips[0].VehicleType);
            Assert.IsTrue(trips[0].DistanceMetres == distanceMetres, "Expecting DistanceMetres value of '{0}' but got '{1}'.", distanceMetres, trips[0].DistanceMetres);
            Assert.IsTrue(trips[0].IsPublic == isPublic, "Expecting IsPublic value of '{0}' but got '{1}'.", isPublic, trips[0].IsPublic);
            Assert.IsTrue(trips[0].OperationalDay == operationalDay, "Expecting OperationalDay value of '{0}' but got '{1}'.", operationalDay, trips[0].OperationalDay);
            Assert.IsTrue(trips[0].Stops.Count == stopCount,"Expecting {0} stops but found {1}.", stopCount, trips[0].Stops.Count);
            Assert.IsTrue(trips[0].Stops[0].PassingTimeSam == firstStopPassingTimeSam, "We expected the PassingTime on the first stop of the first trip to be '{0}' but it appears to be '{1}' instead.", firstStopPassingTimeSam, trips[0].Stops[0].PassingTimeSam);
            Assert.IsTrue(trips[0].Stops[1].IsMonitoredOPRReliability == secondIsMonitoredOPRReliability, "We expected the isMonitoredOPRReliability on the second stop of the first trip to be '{0}' but it appears to be '{1}' instead.", secondIsMonitoredOPRReliability, trips[0].Stops[1].IsMonitoredOPRReliability);
            Assert.IsTrue(trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId == lastStopId, "We expected the HastusStopId on the final stop of the first trip to be '{0}' but it appears to be '{1}' instead.", lastStopId, trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId);
        }

        [TestMethod]
        public void TestJsonToTripsWithMissingFieldInJson()
        {
            // arrange
            string exceptionMessage = "";
            string exceptionText = "HastusTripId";
            #region "JSON string containing a single trip but missing an important field"
            string jsonString = @"
                                    [
                                        {
                                        // Usually the hastusTripId field is here.
                                        ""block"":""kw05-  1"",
                                        ""headboard"":""48"",
                                        ""route"":""48"",
                                        ""startTimepoint"":""nbal"",
                                        ""startTimeSam"":25920,
                                        ""endTimepoint"":""viha"",
                                        ""endTimeSam"":28320,
                                        ""headwayNextSeconds"":1260,
                                        ""nextRoute"":""48"",
                                        ""direction"":""UP"",
                                        ""vehicleType"":""c1"",
                                        ""distanceMetres"":14122,
                                        ""isPublic"":true,
                                        ""operationalDay"":""2019-03-10T00:00:00"",
                                        ""stops"":[
                                            {
                                                ""passingTimeSam"":25920,
                                                ""hastusStopId"":""1896"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":25980,
                                                ""hastusStopId"":""1895"",
                                                ""isMonitoredOPRReliability"":true
                                            },
                                            {
                                                ""passingTimeSam"":26940,
                                                ""hastusStopId"":""1923"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27000,
                                                ""hastusStopId"":""1922"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27480,
                                                ""hastusStopId"":""3710"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27540,
                                                ""hastusStopId"":""3709"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27540,
                                                ""hastusStopId"":""fls1"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27660,
                                                ""hastusStopId"":""3509"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":27780,
                                                ""hastusStopId"":""3508"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28200,
                                                ""hastusStopId"":""3498"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28260,
                                                ""hastusStopId"":""3497"",
                                                ""isMonitoredOPRReliability"":false
                                            },
                                            {
                                                ""passingTimeSam"":28320,
                                                ""hastusStopId"":""8018"",
                                                ""isMonitoredOPRReliability"":false
                                            }
                                            ]
                                        }
                                    ]";
            #endregion
            // act
            try
            {
                var trips = Processor.Processor.CopyJsonToTrips(jsonString);
            }
            catch (Exception ex)
            {
                exceptionMessage = ex.Message;
            }

            // assert
            Assert.IsTrue(exceptionMessage.Contains(exceptionText), "Expecting an exception mentioning the word '{0}' when mapping json that is missing the required {0} field.", exceptionText);
        }
    }
}
