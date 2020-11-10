using System;
using System.Globalization;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    static class DateHelper
    {

        /// <summary>
        /// Gets trigger time in milliseconds, taking into account daylight savings start & ends
        /// </summary>
        /// <param name="currentDateTime">The current date + time of day</param>
        /// <param name="triggerTime">The time of the next trigger event, not adjusted for Daylight savings</param>
        /// <returns>The trigger measure from now in milliseconds</returns>
        static public int GetTriggerTime(DateTime currentDateTime, TimeSpan triggerTime)
        {
            int triggerInMilliseconds = ConvertDueTimeToMilliseconds(currentDateTime.TimeOfDay, triggerTime);
            TimeZone localTimezone = TimeZone.CurrentTimeZone;
            TimeSpan currentOffset = localTimezone.GetUtcOffset(currentDateTime);
            DateTime triggerDateTime = currentDateTime.AddMilliseconds(triggerInMilliseconds);
            TimeSpan tomorrowsOffet = localTimezone.GetUtcOffset(currentDateTime.AddDays(1));

            // if the trigger is happening tomorrow
            if (currentDateTime.Date != triggerDateTime.Date)
            {
                // if tomorrows UTC offset is different from todays...
                if (currentOffset != tomorrowsOffet)
                {
                    // get difference in milliseconds (e.g. -1 hour in ms for DST start, +1 hour in ms for DST end)
                    int offsetDifferenceMilliseconds = (int)(currentOffset - tomorrowsOffet).TotalMilliseconds;

                    // add difference to the total to correctly realign the trigger time
                    triggerInMilliseconds += offsetDifferenceMilliseconds;
                }
            }
            else
            {
                // if the trigger is happening today
                DaylightTime daylightSavingsInfo = TimeZone.CurrentTimeZone.GetDaylightChanges(currentDateTime.Year);

                // if today is a DST change over day
                if (currentDateTime.Date == daylightSavingsInfo.Start.Date || currentDateTime.Date == daylightSavingsInfo.End.Date)
                {
                    // and current time is > 12 and < 2am
                    if (currentDateTime.Hour >= 0 && currentDateTime.Hour <= 1)
                    {
                        TimeSpan yesterdayOffset = localTimezone.GetUtcOffset(currentDateTime.AddDays(-1));

                        // get difference in milliseconds from yesterday to tommorow
                        int offsetDifferenceMilliseconds = (int)(yesterdayOffset - tomorrowsOffet).TotalMilliseconds;

                        // add difference to the total to correctly realign the trigger time
                        // making sure the trigger later today adds or removes the hour correctly
                        triggerInMilliseconds += offsetDifferenceMilliseconds;
                    }
                    // else if its in the DST "window", we don't support this currently (between 2 and 3)
                    else if (currentDateTime.Hour >= 2 && currentDateTime.Hour < 3)
                    {
                        // this is within daylight savings switch over time which is not supported (where there could be weirdness)
                        // log an event, and do no adjustment
                        LogWriter.Instance.Log(EventLogCodes.DST_TRIGGER_IN_ADJUSTMENT_TIME_NOT_SUPPORTED,
                            String.Format("We do not support adjustments for a Havm2TramTracker timer when inside the DST changeover period, triggering in {0}", TimeSpan.FromMilliseconds(triggerInMilliseconds)));
                    }
                }
            }

            return triggerInMilliseconds;
        }

        /// <summary>
        /// Returns an int representing the number of milliseconds between the passed-in currentTime and the dueTime.
        /// Assumes both times are less than 24hrs.
        /// </summary>
        static public int ConvertDueTimeToMilliseconds(TimeSpan currentTime, TimeSpan dueTime)
        {
            int dueTimeSeconds;

            if (currentTime < dueTime)
            {
                //If trigger time hasn't yet happened today then find the number of seconds between now and then
                dueTimeSeconds = (int)dueTime.Subtract(currentTime).TotalSeconds;
            }
            else
            {
                //If trigger time has already happened today then take 24 hours and minus the time elapsed since 3am
                dueTimeSeconds = (60 * 60 * 24) - (int)currentTime.Subtract(dueTime).TotalSeconds;
            }

            return dueTimeSeconds * 1000;
        }
    }
}
