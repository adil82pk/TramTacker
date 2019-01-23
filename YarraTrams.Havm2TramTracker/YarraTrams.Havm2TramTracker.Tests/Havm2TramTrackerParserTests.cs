using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Models;
using YarraTrams.Havm2TramTracker.Processor;

namespace YarraTrams.Havm2TramTracker.Tests
{
    [TestClass]
    public class Havm2TramTrackerParserTests
    {
        [TestMethod]
        public void TestTripsToT_Temp_TripsDataTable()
        {
            // arrange
            var trips = new List<Models.HavmTrip>();

            trips.Add(new Models.HavmTrip
            {
                HastusTripId = 1,
                Block = "Block 1",
                DisplayCode = "86",
                StartTimepoint = "ncob",
                StartTime = new TimeSpan(0, 1, 0, 0, 0),
                EndTimepoint = "mpnd",
                EndTime = new TimeSpan(0, 2, 0, 0, 0),
                HeadwayNextSeconds = 120,
                NextDisplayCode = "86",
                Direction = "DOWN",
                VehicleType = "Z",
                DistanceMetres = 10000,
                IsPublic = true,
                OperationalDay = new DateTime(2019, 2, 1),
                Stops = new List<Models.HavmTripStop>()
            });

            // act
            var tripsDT = Processor.Processor.CopyTripsToT_Temp_TripsDataTable(trips);

            // assert
            Assert.IsTrue(trips.Count == tripsDT.Rows.Count, "Number of records in DataTable ({1:d}) doesn't match number of records in Trip class list ({0:d}).", trips.Count, tripsDT.Rows.Count);
            //Todo: check more stuff. Everything!
            Assert.IsTrue(trips[0].HastusTripId == tripsDT[0].TripID, "TripId field in DataTable ({1}) doesn't match HastusTripId from Trip class ({0}).", trips[0].HastusTripId, tripsDT[0].TripID);
        }

        [TestMethod]
        public void TestJsonToTrips()
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
                                    ""startTime"": ""00:00:00"",
                                    ""endTimepoint"": ""scj1"",
                                    ""endTime"": ""00:06:00"",
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
            Assert.IsTrue(trips.Count == 1, "Number of records in Trip class list ({1:d}) doesn't match number of records in the JSON ({0:d}).",1, trips.Count);
            //Todo: check more stuff
            Assert.IsTrue(trips[0].Stops[trips[0].Stops.Count-1].HastusStopId == "1124", "We expected the HastusStopId on the final stop of the first trip to be '1124' but it appears to be '{0}' instead.", trips[0].Stops[trips[0].Stops.Count - 1].HastusStopId);
        }
        //Todo Test for malformed AND extra fields (to be ignored) and missing fields.
        [TestMethod]
        public void TestAtLayoverTransformation()
        {//Todo: split that in to 5
            var headwayMapping = new Dictionary<int, short>
            {
                { -1, 0 },          //Negatives go to zero
                { 60, 1 },          // Normal, no rounding
                { 90, 2 },          // Round up
                { 29, 0 },          // Round down
                {32768*60,32767}    //Anything exceeding a short gets set to the max short
            };

            foreach (var map in headwayMapping)
            {
                // arrange
                var trip = new Models.HavmTrip
                {
                    HeadwayNextSeconds = map.Key //Ignoring all other properties
                };

                // act
                var AtLayover = Models.Transformations.GetAtLayovertime(trip);

                // assert
                Assert.IsTrue(AtLayover == map.Value, "Expecting value {0} from input of {1} but got {2} instead.", map.Value, map.Key, AtLayover);
            }
        }

        [TestMethod]
        public void TestUpDirectionTransformationWithValidInputs()
        {
            var directionMapping = new Dictionary<string, Nullable<bool>>
            {
                { "UP", true },        //UP is true
                { " down ", false },   // DOWN is false, even with padding and lower-case
            };

            foreach (var map in directionMapping)
            {
                // arrange
                var trip = new Models.HavmTrip
                {
                    Direction = map.Key //Ignoring all other properties
                };

                // act
                var UpDirection = Models.Transformations.GetUpDirection(trip);
                
                // assert
                Assert.IsTrue(UpDirection == map.Value, "Expecting value {0} from input of {1} but got {2} instead.", map.Value, map.Key, UpDirection);
            }
        }

        [TestMethod]
        public void TestUpDirectionTransformationWithBadInput()
        {
            // arrange
            var trip = new Models.HavmTrip
            {
                Direction = "all over the place" //Ignoring all other properties
            };

            // act
            bool UpDirection;
            bool success;
            try
            {
                UpDirection = Models.Transformations.GetUpDirection(trip);
                success = false;
            }
            #pragma warning disable CS0168 // Variable is declared but never used
            catch (FormatException ex)
            #pragma warning restore CS0168 // Variable is declared but never used
            {
                success = true; //we are expecting an exception when we pass invalid input
            }

            // assert
            Assert.IsTrue(success, "Expecting a FormatException when passing invalid input. Didn't encounter such an exception.");
        }
    }
}
