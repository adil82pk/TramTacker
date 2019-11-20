using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Processor;
using System.ServiceProcess;
using System.Linq.Expressions;
using System.Collections.Specialized;
using YarraTrams.Havm2TramTracker.Processor.Helpers;

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
        public void TestTriggerTimesAreValidWhenDaylightSavingsStartDayTomorrow()
        {
            // arrange
            // daylight savings start time is 06/10/2019, lets emulate the night before at 11:00
            DateTime currentTime = new DateTime(2019, 10, 5, 23, 0, 0);

            // calculating time difference to 2:59am the next day, but that actually occurs in only
            // 2 hours and 59 minutes time hours time (not 3 hours and 59 minutes), as there is no 2am on 
            // DST end day (goes from 1:59:59 to 3am).
            TimeSpan copyToLiveDueTime = new TimeSpan(2, 59, 00);

            var service = new Havm2TramTrackerService();

            // act
            int triggerMilliseconds = service.GetTriggerTime(currentTime, copyToLiveDueTime);
            int expectedMilliseconds = (int)(new TimeSpan(2, 59, 00).TotalMilliseconds);

            // assert
            Assert.IsTrue(triggerMilliseconds == expectedMilliseconds, "Daylight savings start trigger should be adjusted correctly");
        }

        [TestMethod]
        public void TestTriggerTimesAreValidWhenDaylightSavingsStartDayToday()
        {
            // arrange
            // daylight savings start time is 2am on the 06/10/2019, lets emulate a trigger before this but on the same day, 12:05am
            DateTime currentTime = new DateTime(2019, 10, 6, 00, 5, 0);

            // and trigger time is at 2:59, but need to take into account the time will jump from 1:59:59 to 3:00:00
            TimeSpan copyToLiveDueTime = new TimeSpan(2, 59, 00);

            var service = new Havm2TramTrackerService();

            // act
            int triggerMilliseconds = service.GetTriggerTime(currentTime, copyToLiveDueTime);
            //  we expect it to only be 1:54 difference between 12:05 and 2:59
            int expectedMilliseconds = (int)(new TimeSpan(1, 54, 00).TotalMilliseconds);

            // assert
            Assert.IsTrue(triggerMilliseconds == expectedMilliseconds, "Daylight savings start trigger should be adjusted correctly on the same day");
        }

        [TestMethod]
        public void TestTriggerTimesAreValidWhenDaylightSavingsEndDayTomorrow()
        {
            // arrange
            // daylight savings end time is 05/04/2020, lets emulate the night before at 11:00
            DateTime currentTime = new DateTime(2020, 04, 04, 23, 0, 0);

            // calculating time difference to 2:59am the next day, but that actually occurs twice on DST end days,
            // and we want to target the second one n 4 hours and 59 minutes
            TimeSpan copyToLiveDueTime = new TimeSpan(2, 59, 00);

            var service = new Havm2TramTrackerService();

            // act
            int triggerMilliseconds = service.GetTriggerTime(currentTime, copyToLiveDueTime);
            int expectedMilliseconds = (int)(new TimeSpan(4, 59, 00).TotalMilliseconds);

            // assert
            Assert.IsTrue(triggerMilliseconds == expectedMilliseconds, "Daylight savings end trigger should be adjusted correctly");
        }

        [TestMethod]
        public void TestTriggerTimesAreValidWhenDaylightSavingsEndDayToday()
        {
            // arrange
            // daylight savings end time is 3am on the 05/04/2020, lets emulate a trigger before this but on the same day, 12:05am
            DateTime currentTime = new DateTime(2020, 04, 05, 00, 5, 0);

            // and trigger time is at 2:59, but need to take into account there are 2 of these (we want the second one)
            TimeSpan copyToLiveDueTime = new TimeSpan(2, 59, 00);

            var service = new Havm2TramTrackerService();

            // act
            int triggerMilliseconds = service.GetTriggerTime(currentTime, copyToLiveDueTime);
            //  we expect it to actually be 3:54 difference between 12:05 and the second 2:59
            int expectedMilliseconds = (int)(new TimeSpan(3, 54, 00).TotalMilliseconds);

            // assert
            Assert.IsTrue(triggerMilliseconds == expectedMilliseconds, "Daylight savings end trigger should be adjusted correctly on the same day");
        }

        [TestMethod]
        public void TestNotSupportingAdjustmentsIfDSTEndAndTriggerIsInTheChangePeriod()
        {
            // arrange
            // this tests that the system does not (currently) handle a trigger timer if it is a daylight savings end day
            // and current time is after 1:59:59am and before 3:59:59 am
            // daylight savings end time is 3am on the 05/04/2020, lets emulate the current time as 2:05am
            DateTime currentTime = new DateTime(2020, 04, 05, 02, 5, 0);

            // and trigger time is at 2:59
            TimeSpan copyToLiveDueTime = new TimeSpan(2, 59, 00);

            var service = new Havm2TramTrackerService();

            // act
            int triggerMilliseconds = service.GetTriggerTime(currentTime, copyToLiveDueTime);
            //  we expect it to not take DST time in account, as this is unsupported currently 
            int expectedMilliseconds = (int)(new TimeSpan(0, 54, 00).TotalMilliseconds);

            // assert
            Assert.IsTrue(triggerMilliseconds == expectedMilliseconds, "Changes in Adjustment period of DST end day are not implemented");
        }

        [TestMethod]
        public void TestNotSupportingAdjustmentsIfDSTStartAndTriggerIsInTheChangePeriod()
        {
            // arrange
            // this tests that the system does not (currently) handle a trigger timer if it is a daylight savings start day
            // and current time is after 1:59:59am and before 3:59:59 am
            // daylight savings start time is 2am on the 05/04/2020, lets emulate the current time as 2:05am
            DateTime currentTime = new DateTime(2019, 10, 6, 2, 5, 0);

            // and trigger time is at 2:59
            TimeSpan copyToLiveDueTime = new TimeSpan(2, 59, 00);

            var service = new Havm2TramTrackerService();

            // act
            int triggerMilliseconds = service.GetTriggerTime(currentTime, copyToLiveDueTime);
            //  we expect it to not take DST time in account, as this is unsupported currently 
            int expectedMilliseconds = (int)(new TimeSpan(0, 54, 00).TotalMilliseconds);

            // assert
            Assert.IsTrue(triggerMilliseconds == expectedMilliseconds, "Changes in Adjustment period of DST end day are not implemented");
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
            DateTime currentTime = new DateTime(2019, 8, 1, 23, 59, 59);
            TimeSpan refreshTempDueTime = new TimeSpan(23, 0, 0);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);
            TimeSpan dueTime;
            Enums.Processes process;

            TimeSpan expectedDueTime = copyToLiveDueTime;
            Enums.Processes expectedProcess = Enums.Processes.CopyToLive;

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
            DateTime currentTime = new DateTime(2019, 8, 1, 0, 0, 0);
            TimeSpan refreshTempDueTime = new TimeSpan(23, 00, 00);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);
            TimeSpan dueTime;
            Enums.Processes process;

            TimeSpan expectedDueTime = copyToLiveDueTime;
            Enums.Processes expectedProcess = Enums.Processes.CopyToLive;

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
            DateTime currentTime = new DateTime(2019, 8, 1, 23, 0, 0);
            TimeSpan refreshTempDueTime = new TimeSpan(23, 00, 00);
            TimeSpan copyToLiveDueTime = new TimeSpan(3, 0, 0);
            TimeSpan dueTime;
            Enums.Processes process;

            TimeSpan expectedDueTime = refreshTempDueTime;
            Enums.Processes expectedProcess = Enums.Processes.RefreshTemp;

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
            DateTime currentTime = new DateTime(2019, 8, 1, 0, 0, 0);
            TimeSpan dueTime = new TimeSpan(23, 59, 59);
            int expectedMilliseconds = 86399000;

            var service = new Havm2TramTrackerService();

            // act
            int milliseconds = service.ConvertDueTimeToMilliseconds(currentTime.TimeOfDay, dueTime);

            // assert
            Assert.AreEqual(expectedMilliseconds, milliseconds, "Returned milliseconds value ({0}) doesn't match the expected value ({1}).", milliseconds, expectedMilliseconds);
        }

        [TestMethod]
        public void TestConvertDueTimeToMillisecondsWhenDueTimeIsTomorrow()
        {
            // arrange
            DateTime currentTime = new DateTime(2019, 8, 1, 23, 59, 59);
            TimeSpan dueTime = new TimeSpan(0, 0, 0);
            int expectedMilliseconds = 1000;

            var service = new Havm2TramTrackerService();

            // act
            int milliseconds = service.ConvertDueTimeToMilliseconds(currentTime.TimeOfDay, dueTime);

            // assert
            Assert.AreEqual(expectedMilliseconds, milliseconds, "Returned milliseconds value ({0}) doesn't match the expected value ({1}).", milliseconds, expectedMilliseconds);
        }
    }
}
