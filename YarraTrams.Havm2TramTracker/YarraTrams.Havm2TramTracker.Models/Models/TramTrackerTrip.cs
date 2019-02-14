﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Models
{
    class TramTrackerTrip : TramTrackerBase
    {
        public int TripID { get; set; }
        public string RunNo{ get; set; }
        public short RouteNo{ get; set; }
        public string FirstTP{ get; set; }
        public int FirstTime{ get; set; }
        public string EndTP{ get; set; }
        public int EndTime{ get; set; }
        public short AtLayoverTime{ get; set; }
        public short NextRouteNo{ get; set; }
        public bool UpDirection{ get; set; }
        public bool LowFloor{ get; set; }
        public decimal TripDistance{ get; set; }
        public bool PublicTrip{ get; set; }
        public byte DayOfWeek{ get; set; }

        /// <summary>
        /// Populate data from HavmTrip object
        /// </summary>
        public void FromHavmTrip(HavmTrip havmTrip)
        {
            this.TripID = havmTrip.HastusTripId;
            this.RunNo = this.GetRunNumberShortForm(havmTrip);
            this.RouteNo = this.GetRouteNumberUsingHeadboard(havmTrip);
            this.FirstTP = havmTrip.StartTimepoint;
            this.FirstTime = (int)havmTrip.StartTime.TotalSeconds;
            this.EndTP = havmTrip.EndTimepoint;
            this.EndTime = (int)havmTrip.EndTime.TotalSeconds - 1;//Todo: Investigate this. Why is TT making all trips end 1 second early?
            this.AtLayoverTime = this.GetAtLayovertime(havmTrip);
            this.NextRouteNo = this.GetNextRouteNo(havmTrip);
            this.UpDirection = this.GetUpDirection(havmTrip);
            this.LowFloor = this.GetLowFloor(havmTrip);
            this.TripDistance = this.GetTripDistance(havmTrip);
            this.PublicTrip = havmTrip.IsPublic; //Todo: Confirm whether we bother filtering non public trips or we trust HAVM2.
            this.DayOfWeek = this.GetDayOfWeek(havmTrip);
        }

        /// <summary>
        /// Layover time (the time a vehicle spends at its final timpoint prior to embarking on its next trip) is measured in seconds by HAVM2.
        /// In TramTRACKER the layover time is measured in minutes.
        /// This routine converts the seconds to minutes and does some validation as it goes.
        /// This routine also rounds to the nearest minute, if required, however at the moment all HAVM2 values are recorded as a round minute (always a multiple of 60).
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private short GetAtLayovertime(HavmTrip trip)
        {
            //We merely convert the seconds to minutes
            decimal AtLayovertimeDec = ((decimal)trip.HeadwayNextSeconds / 60);

            short AtLayovertimeShort;

            if (AtLayovertimeDec >= short.MaxValue)
            {
#if !DEBUG
                //Todo: Log warning, but not when unit testing
#endif
                AtLayovertimeShort = short.MaxValue;
            }
            else if (AtLayovertimeDec <= 0)
            {
#if !DEBUG
                //Todo: Log warning, but not when unit testing
#endif
                AtLayovertimeShort = 0;
            }
            else
            {
                AtLayovertimeShort = (short)Math.Round(AtLayovertimeDec, MidpointRounding.AwayFromZero);
            }

            return AtLayovertimeShort;
        }

        /// <summary>
        /// Route numbers arrive from HAVM2 as text but get saved to TramTRACKER as a number.
        /// This routine does the conversion and throws a friendly error if the conversion fails.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private short GetNextRouteNo(HavmTrip trip)
        {
            if (short.TryParse(trip.NextRoute, out short nextRoute))
            {
                return nextRoute;
            }
            else
            {
                throw new FormatException($"Unexpected format for next route number on trip with HASTUS Id {trip.HastusTripId}. Expecting a number but got \"{(trip.NextRoute ?? "")}\".");
            }
        }

        /// <summary>
        /// The designation for up/down direction comes from HAVM2 as a string (either "UP" or "DOWN").
        /// TramTRACKER expects the up/down direction to be defined as true/false (up = true, down = false).
        /// THis routine converts the string designation to a boolean.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private bool GetUpDirection(HavmTrip trip)
        {
            if (!(trip.Direction == null))
            {
                if (trip.Direction.Trim().ToUpper() == "UP")
                {
                    return true;
                }
                else if (trip.Direction.Trim().ToUpper() == "DOWN")
                {
                    return false;
                }
                else
                {
                    throw new FormatException($"Unexpected trip direction on trip with HASTUS Id {trip.HastusTripId}. Expecting \"UP\" or \"DOWN\" but got \"{trip.Direction}\".");
                }
            }
            else
            {
                throw new FormatException($"Unexpected trip direction on trip with HASTUS Id {trip.HastusTripId}. Expecting \"UP\" or \"DOWN\" but got null.");
            }
        }

        /// <summary>
        /// Trip distance in HAVM is defined in metres.
        /// This routine converts the metres in to kilometres.
        /// </summary>
        /// <param name="trip"></param>
        /// <returns></returns>
        private decimal GetTripDistance(HavmTrip trip)
        {
            return (decimal)trip.DistanceMetres / (decimal)1000m;
        }
    }
}
