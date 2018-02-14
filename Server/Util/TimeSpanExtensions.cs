using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Server.MasterData.DTO.Data.CrossService;

namespace Server.Util
{
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Multiplies a timespan by an integer value
        /// </summary>
        public static long DividedBy(this TimeSpan dividend, TimeSpan divisor)
        {
            return dividend.Ticks / divisor.Ticks;
        }

        public static TimeSpan ToTimeSpan(this RequiredAttentiveness attentiveness)
        {
            switch (attentiveness)
            {
                case RequiredAttentiveness.Constantly:
                    return TimeSpan.FromSeconds(1);
                case RequiredAttentiveness.Minutely:
                    return TimeSpan.FromMinutes(1);
                case RequiredAttentiveness.Hourly:
                    return TimeSpan.FromHours(1);
                case RequiredAttentiveness.Daily:
                    return TimeSpan.FromDays(1);
                case RequiredAttentiveness.Weekly:
                    return TimeSpan.FromDays(7);
                default:
                    return TimeSpan.FromHours(1);
            }
        }

    }
}