using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Models;
using YarraTrams.Havm2TramTracker.Processor;

namespace YarraTrams.Havm2TramTracker.Tests
{
    [TestClass]
    public class Havm2TramTrackerTransformationTests
    {
        #region RunNo
        [TestMethod]
        public void TestRunNoTransformationWithNormalDepot()
        {
            // arrange
            const string block = "db21-  1";
            const string expectedResult = "D-1";
            var trip = new Models.HavmTrip
            {
                Block = block //Ignoring all other properties
            };

            // act
            var RunNo = Models.Transformations.GetRunNumber(trip);

            // assert
            Assert.IsTrue(RunNo == expectedResult, "Expecting value \"{0}\" from input of \"{1}\" but got \"{2}\" instead.", expectedResult, block, RunNo);
        }

        [TestMethod]
        public void TestRunNoTransformationWithCamberwellDepot()
        {
            // arrange
            const string block = "cw07- 27";
            const string expectedResult = "V-27";
            var trip = new Models.HavmTrip
            {
                Block = block //Ignoring all other properties
            };

            // act
            var RunNo = Models.Transformations.GetRunNumber(trip);

            // assert
            Assert.IsTrue(RunNo == expectedResult, "Expecting value \"{0}\" from input of \"{1}\" but got \"{2}\" instead.", expectedResult, block, RunNo);
        }

        #endregion

        #region AtLayover
        [TestMethod]
        public void TestAtLayoverTransformationWithNegative()
        {
            // arrange
            const int headway = -1; //Seconds
            const short expectedResult = 0; //Minute
            var trip = new Models.HavmTrip
            {
                HeadwayNextSeconds = headway //Ignoring all other properties
            };

            // act
            var atLayover = Models.Transformations.GetAtLayovertime(trip);

            // assert
            Assert.IsTrue(atLayover == expectedResult, "Expecting value {0} from input of \"{1}\" but got {2} instead.", expectedResult, headway, atLayover);
        }

        [TestMethod]
        public void TestAtLayoverTransformationWithRoundFigure()
        {
            // arrange
            //Todo: Confirm rounding rule with YT
            const int headway = 60; //Seconds
            const short expectedResult = 1; //Minute
            var trip = new Models.HavmTrip
            {
                HeadwayNextSeconds = headway //Ignoring all other properties
            };

            // act
            var atLayover = Models.Transformations.GetAtLayovertime(trip);

            // assert
            Assert.IsTrue(atLayover == expectedResult, "Expecting value {0} from input of \"{1}\" but got {2} instead.", expectedResult, headway, atLayover);
        }

        [TestMethod]
        public void TestAtLayoverTransformationWithFigureToRoundDown()
        {
            // arrange
            const int headway = 29; //Seconds
            const short expectedResult = 0; //Minute
            var trip = new Models.HavmTrip
            {
                HeadwayNextSeconds = headway //Ignoring all other properties
            };

            // act
            var atLayover = Models.Transformations.GetAtLayovertime(trip);

            // assert
            Assert.IsTrue(atLayover == expectedResult, "Expecting value {0} from input of \"{1}\" but got {2} instead.", expectedResult, headway, atLayover);
        }

        [TestMethod]
        public void TestAtLayoverTransformationWithFigureToRoundUp()
        {
            // arrange
            const int headway = 90; //Seconds
            const short expectedResult = 2; //Minute
            var trip = new Models.HavmTrip
            {
                HeadwayNextSeconds = headway //Ignoring all other properties
            };

            // act
            var atLayover = Models.Transformations.GetAtLayovertime(trip);

            // assert
            Assert.IsTrue(atLayover == expectedResult, "Expecting value {0} from input of \"{1}\" but got {2} instead.", expectedResult, headway, atLayover);
        }

        [TestMethod]
        public void TestAtLayoverTransformationWithFigureOutside2BitRange()
        {
            // arrange
            const int headway = 32768 * 60; //Seconds
            const short expectedResult = 32767; //Minute
            var trip = new Models.HavmTrip
            {
                HeadwayNextSeconds = headway //Ignoring all other properties
            };

            // act
            var atLayover = Models.Transformations.GetAtLayovertime(trip);

            // assert
            Assert.IsTrue(atLayover == expectedResult, "Expecting value {0} from input of \"{1}\" but got {2} instead.", expectedResult, headway, atLayover);
        }
        #endregion

        #region UpDirection
        [TestMethod]
        public void TestUpDirectionTransformationWithValidUp()
        {
            // arrange
            const string direction = "UP";
            var trip = new Models.HavmTrip
            {
                Direction = direction //Ignoring all other properties
            };

            // act
            var UpDirection = Models.Transformations.GetUpDirection(trip);

            // assert
            Assert.IsTrue(UpDirection, "Expecting value {0} from input of \"{1}\" but got {2} instead.", true, direction, UpDirection);

        }

        [TestMethod]
        public void TestUpDirectionTransformationWithValidDown()
        {
            // arrange
            const string direction = " down ";
            var trip = new Models.HavmTrip
            {
                Direction = direction //Ignoring all other properties
            };

            // act
            var UpDirection = Models.Transformations.GetUpDirection(trip);

            // assert
            Assert.IsFalse(UpDirection, "Expecting value {0} from input of \"{1}\" but got {2} instead.", true, direction, UpDirection);

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
            bool success;
            try
            {
                var UpDirection = Models.Transformations.GetUpDirection(trip);
                success = false;
            }
            catch (FormatException ex)
            {
                var message = ex.Message;
                success = true; //we are expecting an exception when we pass invalid input
            }

            // assert
            Assert.IsTrue(success, "Expecting a FormatException when passing invalid input. Didn't encounter such an exception.");
        }
        #endregion

        #region LowFloor
        [TestMethod]
        public void TestLowFloorTransformationWithLowFloorGroup()
        {
            // arrange
            const string vehicleGroup = "c";
            var trip = new Models.HavmTrip
            {
                VehicleType = vehicleGroup //Ignoring all other properties
            };

            // act
            var LowFloor = Models.Transformations.GetLowFloor(trip);

            // assert
            Assert.IsTrue(LowFloor, "Expecting value {0} from input of \"{1}\" but got {2} instead.", true, vehicleGroup, LowFloor);

        }

        [TestMethod]
        public void TestLowFloorTransformationWithNonLowFloorGroup()
        {
            // arrange
            const string vehicleGroup = "w";
            var trip = new Models.HavmTrip
            {
                VehicleType = vehicleGroup //Ignoring all other properties
            };

            // act
            var LowFloor = Models.Transformations.GetLowFloor(trip);

            // assert
            Assert.IsFalse(LowFloor, "Expecting value {0} from input of \"{1}\" but got {2} instead.", true, vehicleGroup, LowFloor);

        }

        [TestMethod]
        public void TestLowFloorTransformationWithUnknownGroup()
        {
            // arrange
            const string vehicleGroup = "NotKnown";
            var trip = new Models.HavmTrip
            {
                VehicleType = vehicleGroup //Ignoring all other properties
            };

            // act
            var LowFloor = Models.Transformations.GetLowFloor(trip);

            // assert
            Assert.IsFalse(LowFloor, "Expecting value {0} from input of \"{1}\" but got {2} instead.", true, vehicleGroup, LowFloor);

        }
        #endregion

        #region TripDistance
        [TestMethod]
        public void TestTripDistanceTransformation()
        {
            // arrange
            int distanceMetres = 3123;
            decimal expectedResult = 3.123m;
            var trip = new Models.HavmTrip
            {
                DistanceMetres = distanceMetres //Ignoring all other properties
            };

            // act
            var TripDistance = Models.Transformations.GetTripDistance(trip);

            // assert
            Assert.IsTrue(Decimal.Equals(TripDistance, expectedResult), "Expecting value {0} from input of {1} but got {2} instead.", expectedResult, distanceMetres, TripDistance);
        }

        #endregion

        #region DayOfWeek
        [TestMethod]
        public void TestDayOfWeekTransformationWithValidDay()
        {
            // arrange
            DateTime operationalDay = new DateTime(2019, 1, 1);
            const byte expectedResult = 2;
            var trip = new Models.HavmTrip
            {
                OperationalDay = operationalDay //Ignoring all other properties
            };

            // act
            var DayOfWeek = Models.Transformations.GetDayOfWeek(trip);

            // assert
            Assert.IsTrue(DayOfWeek == expectedResult, "Expecting value {0} from input of {1:dddd, MMMM d, yyyy} but got {2} instead.", expectedResult, operationalDay, DayOfWeek);
        }

        #endregion

        #region StopId

        [TestMethod]
        public void TestStopIdTransformationWithValidStop()
        {
            // arrange
            const int hastusStopId = 1010;
            const string expectedResult = "U035Mill";

            Models.HastusStopMapper.stops = new Dictionary<int, string>();
            Models.HastusStopMapper.stops.Add(hastusStopId, expectedResult);

            var stop = new HavmTripStop
            {
                HastusStopId = hastusStopId.ToString()
            };

            // act
            string stopId = Models.Transformations.GetStopId(stop);

            // assert
            Assert.IsTrue(stopId == expectedResult, "Expecting value {0} from input of {1} but got {2} instead.", expectedResult, hastusStopId, stopId);
        }

        [TestMethod]
        public void TestStopIdTransformationWithInvalidStop()
        {
            // arrange
            Models.HastusStopMapper.stops = new Dictionary<int, string>();
            Models.HastusStopMapper.stops.Add(1234, "A stop we won't find");

            const int hastusStopId = 999999;

            var stop = new HavmTripStop
            {
                HastusStopId = hastusStopId.ToString()
            };

            // act
            // See inside the assert.

            // assert
            Assert.ThrowsException<Exception>(() =>
            {
                string stopId = Models.Transformations.GetStopId(stop);
            },"Expecting an exception when searching for a stop when the HastusStopMapper is empty.");
        }

        #endregion
    }
}
