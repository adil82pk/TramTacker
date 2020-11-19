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
    public class Havm2TramTrackerHavm2TimetableServiceTests
    {
        [TestMethod]
        public void TestTransformHavm2JsonToTimetableList()
        {
            // arrange
            #region JSON string
            string json = @"[
    {
        ""date"": ""2020-04-03"",
        ""exportTimestamp"": 1588854938,
        ""isPtvApproved"": true,
        ""ptvApprovedDate"": ""2020-05-08T21:09:36.47"",
        ""id"": 1061,
        ""createdDate"": ""2020-05-07T21:09:36.47"",
        ""updatedDate"": ""2020-05-07T22:38:05.637""
    },
    {
        ""date"": ""2020-04-04"",
        ""exportTimestamp"": 1588855458,
        ""isPtvApproved"": false,
        ""ptvApprovedDate"": null,
        ""id"": 1063,
        ""createdDate"": ""2020-05-07T21:21:51.29"",
        ""updatedDate"": ""2020-05-07T22:45:41.567""
    },
    {
        ""date"": ""2020-04-05"",
        ""exportTimestamp"": 1588855388,
        ""isPtvApproved"": false,
        ""ptvApprovedDate"": null,
        ""id"": 1062,
        ""createdDate"": ""2020-05-07T21:21:51.29"",
        ""updatedDate"": ""2020-05-07T22:45:41.513""
    },
    {
        ""date"": ""2020-04-06"",
        ""exportTimestamp"": 1588853162,
        ""isPtvApproved"": false,
        ""ptvApprovedDate"": null,
        ""id"": 1065,
        ""createdDate"": ""2020-05-07T21:21:51.29"",
        ""updatedDate"": null
    }
]";
            #endregion

            int expectedTimetableCount = 4;
            DateTime expectedTimestamp = new DateTime(2020, 4, 3);


            // act
            Havm2TimetableService service = new Havm2TimetableService();
            List<HavmTimetable> timetables = service.CopyJsonToTimetables(json);

            // assert
            Assert.IsTrue(timetables.Count == expectedTimetableCount, String.Format("Expecting {0} timetables but got {1}.", expectedTimetableCount, timetables.Count));
            Assert.IsTrue(timetables.First().Date == expectedTimestamp, String.Format("Expecting date on first timetable to be {0:yyyy-MM-dd} timetables but got {1:yyyy-MM-dd}.", expectedTimestamp, timetables.First().Date));
        }

        [TestMethod]
        public void TestExtractTomorrowsHavm2TimetableRevisionFromTimetableList()
        {
            // arrange
            int expectedTimestamp = 1588854938;
            List<HavmTimetable> timetables = new List<HavmTimetable>
            {
                new HavmTimetable() { Date = new DateTime(2020, 4, 3), ExportTimestamp = expectedTimestamp}
            };

            // act
            Havm2TimetableService service = new Havm2TimetableService();
            int timestamp = service.GetTimestampFromTimetables(timetables);

            // assert
            Assert.IsTrue(timestamp == expectedTimestamp, String.Format("Expecting timestamp {0} timetables but got {1}.", expectedTimestamp, timestamp));
        }

        [TestMethod]
        public void TestThrowsExceptionWhenThereAreNoTimetables()
        {
            // arrange
            List<HavmTimetable> timetables = new List<HavmTimetable>();

            // act/assert
            Havm2TimetableService service = new Havm2TimetableService();
            Assert.ThrowsException<ArgumentException>(() =>
            {
                service.GetTimestampFromTimetables(timetables);
            }, "Expecting an exception of type ArgumentException when extracting a timestamp from an empty list of timetables.");
        }
    }
}
