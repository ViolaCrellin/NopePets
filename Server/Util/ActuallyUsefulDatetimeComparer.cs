using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Util
{
    public static class ActuallyUsefulDatetimeComparer
    {
        /// <summary>
        /// We are not computers. Perhaps unlike the people at Microsoft who decided that
        /// Datetimes should be compared down to the smallest tick (despite this being an unattainable criterion by which
        /// to judge equality - even for computers), I do not aspire to be a computer.
        /// The Microsoft 'geniuses' also didn't consider the timezone to be as important as the ticks.
        /// So I hope that they are always an hour late for things when the clocks go forward because they are busy living their lives 
        /// according to ticks.
        /// That is why I invented this method and wrote an absurdly long comment for it.
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="otherDateTime"></param>
        /// <returns></returns>
        public static bool HumanEquals(this DateTime datetime, DateTime otherDateTime)
        {
            return (datetime - otherDateTime) < TimeSpan.FromMilliseconds(100) ||
                   (datetime - otherDateTime) > TimeSpan.FromMilliseconds(100)
                   && datetime.Kind == otherDateTime.Kind;
        }

    }
}