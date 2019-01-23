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
            #pragma warning disable CS0168 // Variable is declared but never used
            catch (FormatException ex)
            #pragma warning restore CS0168 // Variable is declared but never used
            {
                success = true; //we are expecting an exception when we pass invalid input
            }

            // assert
            Assert.IsTrue(success, "Expecting a FormatException when passing invalid input. Didn't encounter such an exception.");
        }
        #endregion
    }
}
