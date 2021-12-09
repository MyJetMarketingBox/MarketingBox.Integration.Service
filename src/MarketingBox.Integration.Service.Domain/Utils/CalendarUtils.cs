﻿using System;

namespace MarketingBox.Integration.Service.Domain.Utils
{
    public static class CalendarUtils
    {
        public static DateTime StartOfMonth(DateTime current)
        {
            DateTime first = new DateTime(current.Year, current.Month, 1);
            return first;
        }

        public static DateTime EndOfMonth(DateTime current)
        {
            DateTime last = new DateTime(current.Year, current.Month,
                                    DateTime.DaysInMonth(current.Year, current.Month));
            return last;
        }
    }
}
