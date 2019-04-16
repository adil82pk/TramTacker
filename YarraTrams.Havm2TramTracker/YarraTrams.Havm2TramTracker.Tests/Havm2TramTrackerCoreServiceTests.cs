using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Processor;
using System.ServiceProcess;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace YarraTrams.Havm2TramTracker.Tests
{
    [TestClass]
    public class Havm2TramTrackerCoreServiceTests
    {
        [TestMethod]
        public void TestAllStringsAreLowerCaseWhenTheyAreAllLowerCase()
        {
            // arrange
            String[] arr = new String[] { "a", "c1", "taxi" };
            StringCollection col = new StringCollection();
            col.AddRange(arr);

            var service = new Havm2TramTrackerService();

            // act
            bool result = service.AllStringsAreLowerCase(col);

            // assert
            Assert.IsTrue(result, "Passing the values \"{0}\" should be valid.", String.Join(",", arr));
        }

        [TestMethod]
        public void TestAllStringsAreLowerCaseWhenPassingABlank()
        {
            // arrange
            String[] arr = new String[] { "", "c1", "taxi" };
            StringCollection col = new StringCollection();
            col.AddRange(arr);

            var service = new Havm2TramTrackerService();

            // act
            bool result = service.AllStringsAreLowerCase(col);

            // assert
            Assert.IsFalse(result, "Passing the values \"{0}\" should be invalid.", String.Join(",", arr));
        }

        [TestMethod]
        public void TestAllStringsAreLowerCaseWhenPassingAnUpper()
        {
            // arrange
            String[] arr = new String[] { "A", "c1", "taxi" };
            StringCollection col = new StringCollection();
            col.AddRange(arr);

            var service = new Havm2TramTrackerService();

            // act
            bool result = service.AllStringsAreLowerCase(col);

            // assert
            Assert.IsFalse(result, "Passing the values \"{0}\" should be invalid.", String.Join(",", arr));
        }

        [TestMethod]
        public void TestTriggerTimesAreValidWhenTriggerTimesAreValid()
        {
            // arrange
            TimeSpan refreshTempDueTime = new TimeSpan(23, 0, 0);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);

            var service = new Havm2TramTrackerService();

            // act
            bool result = service.TriggerTimesAreValid(refreshTempDueTime, copyToLiveDueTime);

            // assert
            Assert.IsTrue(result, "Passing a refreshTempDueTime of {0} and a copyToLiveDueTime of {1} should be valid.", refreshTempDueTime, copyToLiveDueTime);
        }

        [TestMethod]
        public void TestTriggerTimesAreValidWhenTimeGreaterThan24Hours()
        {
            // arrange
            TimeSpan refreshTempDueTime = new TimeSpan(24, 00, 00);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);

            var service = new Havm2TramTrackerService();

            // act
            bool result = service.TriggerTimesAreValid(refreshTempDueTime, copyToLiveDueTime);

            // assert
            Assert.IsFalse(result, "Passing a refreshTempDueTime of {0} should be invalid because it exceeds the time in a single day.", refreshTempDueTime);
        }

        [TestMethod]
        public void TestTriggerTimesAreValidWhenRefreshTempPredatesCopyToLive()
        {
            // arrange
            TimeSpan refreshTempDueTime = new TimeSpan(0, 0, 0);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);

            var service = new Havm2TramTrackerService();

            // act
            bool result = service.TriggerTimesAreValid(refreshTempDueTime, copyToLiveDueTime);

            // assert
            Assert.IsFalse(result, "Passing a refreshTempDueTime of {0} and a copyToLiveDueTime of {1} should be invalid because CopyToLive needs today's data and RefreshTemp will delete today's data.", refreshTempDueTime, copyToLiveDueTime);
        }

        [TestMethod]
        public void TestTriggerTimesAreValidWhenTriggersAreTooCloseTogether()
        {
            // arrange
            TimeSpan refreshTempDueTime = new TimeSpan(3, 29, 59);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);

            var service = new Havm2TramTrackerService();

            // act
            bool result = service.TriggerTimesAreValid(refreshTempDueTime, copyToLiveDueTime);

            // assert
            Assert.IsFalse(result, "Passing a refreshTempDueTime of {0} and a copyToLiveDueTime of {1} should be invalid because the two triggers are too close together.", refreshTempDueTime, copyToLiveDueTime);
        }

        [TestMethod]
        public void TestDetermineNextTriggerWhenPassedAllTodaysTriggerTimes()
        {
            // arrange
            TimeSpan currentTime = new TimeSpan(23, 59, 59);
            TimeSpan refreshTempDueTime = new TimeSpan(23, 0, 0);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);
            TimeSpan dueTime;
            Havm2TramTrackerService.Processes process;

            TimeSpan expectedDueTime = copyToLiveDueTime;
            Havm2TramTrackerService.Processes expectedProcess = Havm2TramTrackerService.Processes.CopyToLive;

            var service = new Havm2TramTrackerService();

            // act
            service.DetermineNextTrigger(currentTime, refreshTempDueTime, copyToLiveDueTime, out dueTime, out process);

            // assert
            Assert.AreEqual(expectedDueTime, dueTime, "Returned trigger time ({0}) doesn't match the expected trigger time ({1}).", dueTime, expectedDueTime);
            Assert.AreEqual(expectedProcess, process, "Returned trigger process ({0}) doesn't match the expected trigger process ({1}).", process, expectedProcess);
        }

        [TestMethod]
        public void TestDetermineNextTriggerWhenEarlierThanAllTodaysTriggerTimes()
        {
            // arrange
            TimeSpan currentTime = new TimeSpan(0, 0, 0);
            TimeSpan refreshTempDueTime = new TimeSpan(23, 00, 00);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);
            TimeSpan dueTime;
            Havm2TramTrackerService.Processes process;

            TimeSpan expectedDueTime = copyToLiveDueTime;
            Havm2TramTrackerService.Processes expectedProcess = Havm2TramTrackerService.Processes.CopyToLive;

            var service = new Havm2TramTrackerService();

            // act
            service.DetermineNextTrigger(currentTime, refreshTempDueTime, copyToLiveDueTime, out dueTime, out process);

            // assert
            Assert.AreEqual(expectedDueTime, dueTime, "Returned trigger time ({0}) doesn't match the expected trigger time ({1}).", dueTime, expectedDueTime);
            Assert.AreEqual(expectedProcess, process, "Returned trigger process ({0}) doesn't match the expected trigger process ({1}).", process, expectedProcess);
        }

        [TestMethod]
        public void TestDetermineNextTriggerWhenBetweenTriggers()
        {
            // arrange
            TimeSpan currentTime = new TimeSpan(12, 0, 0);
            TimeSpan refreshTempDueTime = new TimeSpan(23, 00, 00);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);
            TimeSpan dueTime;
            Havm2TramTrackerService.Processes process;

            TimeSpan expectedDueTime = refreshTempDueTime;
            Havm2TramTrackerService.Processes expectedProcess = Havm2TramTrackerService.Processes.RefreshTemp;

            var service = new Havm2TramTrackerService();

            // act
            service.DetermineNextTrigger(currentTime, refreshTempDueTime, copyToLiveDueTime, out dueTime, out process);

            // assert
            Assert.AreEqual(expectedDueTime, dueTime, "Returned trigger time ({0}) doesn't match the expected trigger time ({1}).", dueTime, expectedDueTime);
            Assert.AreEqual(expectedProcess, process, "Returned trigger process ({0}) doesn't match the expected trigger process ({1}).", process, expectedProcess);
        }

        [TestMethod]
        public void TestConvertDueTimeToMillisecondsWhenDueTimeIsToday()
        {
            // arrange
            TimeSpan currentTime = new TimeSpan(0, 0, 0);
            TimeSpan dueTime = new TimeSpan(23, 59, 59);
            int expectedMilliseconds = 86399000;

            var service = new Havm2TramTrackerService();

            // act
            int milliseconds = service.ConvertDueTimeToMilliseconds(currentTime, dueTime);

            // assert
            Assert.AreEqual(expectedMilliseconds, milliseconds, "Returned milliseconds value ({0}) doesn't match the expected value ({1}).", milliseconds, expectedMilliseconds);
        }

        [TestMethod]
        public void TestConvertDueTimeToMillisecondsWhenDueTimeIsTomorrow()
        {
            // arrange
            TimeSpan currentTime = new TimeSpan(23, 59, 59);
            TimeSpan dueTime = new TimeSpan(0, 0, 0);
            int expectedMilliseconds = 1000;

            var service = new Havm2TramTrackerService();

            // act
            int milliseconds = service.ConvertDueTimeToMilliseconds(currentTime, dueTime);

            // assert
            Assert.AreEqual(expectedMilliseconds, milliseconds, "Returned milliseconds value ({0}) doesn't match the expected value ({1}).", milliseconds, expectedMilliseconds);
        }
    }
}
