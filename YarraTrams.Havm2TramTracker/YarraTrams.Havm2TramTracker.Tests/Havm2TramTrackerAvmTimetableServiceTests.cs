using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YarraTrams.Havm2TramTracker.Models;
using System.Linq;
using System.Linq.Expressions;
using YarraTrams.Havm2TramTracker.Processor.Services;
using YarraTrams.Havm2TramTracker.Logger;
using System.Diagnostics;
using System.Threading;

namespace YarraTrams.Havm2TramTracker.Tests
{
    [TestClass]
    public class Havm2TramTrackerAvmTimetableServiceTests
    {
        [TestMethod]
        public void TestExtractTomorrowsAvmTimetableRevisionFromFileContentWithStandardContent()
        {
            // arrange
            string fileContent = @"avmxxx.sch,1604493569,3.14,'Wednesday',wt316320.316,1603746135,5.22,11797
avmsch20.316,1604493569,3.14,'Wednesday',wt316320.316,1603746135,5.22,11797
avmsch20.317,1604493573,3.14,'Thursday',wt317420.317,1603759392,5.22,11798
avmsch20.318,1604922856,3.14,'Friday',wt318520.318,1604437967,5.22,11810,11809
avmsch20.319,1604922861,3.14,'Saturday',wt319620.319,1604018874,5.22,11809,11808
avmsch20.320,1604922866,3.14,'Sunday',wt320720.320,1604018809,5.22,11808
avmsch20.321,1604922870,3.14,'Monday',wt321120.321,1604449242,5.22,11811
avmsch20.322,1604922875,3.14,'Tuesday',wt322220.322,1604453725,5.22,11813";

            int expected = 1603759392;

            // act
            AvmTimetableService service = new AvmTimetableService();
            int result = service.ExtractTomorrowsAvmTimetableRevisionFromFileContent(fileContent);

            // assert
            Assert.IsTrue(result == expected, $"Expecting a timestamp of {expected} but got {result}.");
        }

        [TestMethod]
        public void TestExtractTomorrowsAvmTimetableRevisionFromFileContentWithEmptyContent()
        {
            // arrange
            string fileContent = "";

            // act
            AvmTimetableService service = new AvmTimetableService();

            // assert
            Assert.ThrowsException<FormatException>(() =>
            {
                int result = service.ExtractTomorrowsAvmTimetableRevisionFromFileContent(fileContent);
            }, "Expecting an exception of type FormatException when extracting a timestamp from an empty file.");
        }

        [TestMethod]
        public void TestExtractTomorrowsAvmTimetableRevisionFromFileContentWithTruncatedContent()
        {
            // arrange
            // The TimetableBuildStatus (TBS) application occasionally encounters files trucated at 500 chars.
            // We are unsure whether this is due to a TBS bug or a Peracon bug, if it is the latter then we
            // have to deal with it here.
            string fileContent = @"avmxxx.sch,1595422528,3.14,'Friday',wt213520.213,1594018487,5.22,11312,11292
avmsch20.213,1595422528,3.14,'Friday',wt213520.213,1594018487,5.22,11312,11292
avmsch20.214,1596193800,3.14,'Saturday',wt214620.214,1594018304,5.22,11292,11293
avmsch20.215,1596193805,3.14,'Sunday',wt215720.215,1594018237,5.22,11293
avmsch20.216,1596193810,3.14,'Monday',wt216120.216,1594075429,5.22,11318
avmsch20.217,1596193814,3.14,'Tuesday',wt217220.217,1594077279,5.22,11319
avmsch20.218,1596193821,3.14,'Wednesday',wt218320.218,1";

            //We still read the timestamp from the third line when the file is truncated.
            int expected = 1594018304;

            // We give this test 5 seconds of clear air to ensure nothing else is writing to the event log.
            int delayInMs = 5000;
            Thread.Sleep(delayInMs);

            // act
            AvmTimetableService service = new AvmTimetableService();
            int result = service.ExtractTomorrowsAvmTimetableRevisionFromFileContent(fileContent);

            // assert
            Assert.IsTrue(result == expected, $"Expecting a timestamp of {expected} but got {result}.");

            EventLog eventLog = new EventLog();
            eventLog.Log = LogWriter.Instance.EventLogName;

            IEnumerable<EventLogEntry> logEntries = eventLog.Entries.Cast<EventLogEntry>();
            int totalTruncatdFileEntries = logEntries.Count(l => l.InstanceId == EventLogCodes.TRUNCATED_FILE_ON_AVM_ENDPOINT && (DateTime.Now - l.TimeWritten).TotalMilliseconds < delayInMs);
            Assert.AreEqual(1, totalTruncatdFileEntries, $"Expected a single TRUNCATED_FILE_ON_AVM_ENDPOINT event log entry but found {totalTruncatdFileEntries} entries.");
        }
    }
}
