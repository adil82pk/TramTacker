using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Processor;
using System.ServiceProcess;
using System.Linq.Expressions;

namespace YarraTrams.Havm2TramTracker.Tests
{
    [TestClass]
    public class Havm2TramTrackerCoreServiceTests
    {
        [TestMethod]
        public void TestDetermineNextTriggerWhenPassedAllTodaysTriggerTimes()
        {
            // arrange
            TimeSpan currentTime = new TimeSpan(23, 59, 59);
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
