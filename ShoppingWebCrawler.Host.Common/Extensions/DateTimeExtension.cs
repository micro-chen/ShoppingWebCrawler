using System;

namespace System
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 将时间转换为 yyyy-MM-dd HH:mm:ss 
        /// 格式的字符串
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ToLongOftenString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public static string ToLongString(this DateTime time)
        {
            return time.ToString("yyyyMMddHHmmss");
        }

        public static string ToDayHourString(this DateTime Time)
        {
            return Time.ToString("yyyy-MM-dd HH:mm");
        }
        public static string Getweek(this DateTime time)
        {
            string[] weekdays = { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
            string week = weekdays[Convert.ToInt32(time.DayOfWeek)];
            return week;
        }
        public static string ToOftenString(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }

        public static string ToTimeOftenString(this DateTime time)
        {
            return time.ToString("HH:mm:ss");
        }

        public static string ToMonthAndDay(this DateTime time)
        {
            return time.ToString("yyyy-MM");
        }

        

        public static string ToReportTime(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm");
        }
        public static string ToTimeOftenString2(this DateTime time)
        {
            return time.ToString("HH:mm");
        }

        public static bool IsWeekendOrHoliday(this DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday ||
                date.ToString("MM-dd") == "01-01" ||
                date.ToString("MM-dd") == "04-04" ||
                date.ToString("MM-dd") == "05-01" ||
                date.ToString("MM-dd") == "10-01") return true;
            return false;
        }

        public static bool IsRightTime(this DateTime date)
        {
            return CompareTime(date, new DateTime(2000, 1, 1));
        }

        public static bool CompareTime(this DateTime date, DateTime d)
        {
            return date > d;
        }
        /// <summary>
        /// 获取时间的时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long GetTimeStamp(this DateTime date)
        {
            return Convert.ToInt64((date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }
        /// <summary>
        /// 验证时间戳是否过期（默认24小时）
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static bool IsOutTime(this DateTime date, long timestamp, int outtime = 86400)
        {

            return (date.GetTimeStamp() - timestamp) >= outtime;
        }
        /// <summary>
        /// 日期间隔计算
        /// </summary>
        /// <param name="date"></param>
        /// <param name="DateTimeNew"></param>
        /// <returns></returns>
        public static string DateDiff(this DateTime date, DateTime DateTimeNew)
        {
            string dateDiff = "";
            TimeSpan ts1 = new TimeSpan(date.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTimeNew.Ticks);
            //if (date.ToString("yyyyMMdd").CompareTo(DateTimeNew.ToString("yyyyMMdd")) == 0) 
            //{
            //    ts2 = new TimeSpan(DateTime.Now.Ticks); 
            //}                     
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            int day = ts.Days;
            int hou = ts.Hours;
            int minu = ts.Minutes;
            int sec = ts.Seconds;
            if (day > 0)
            {
                if (day > 30)
                {
                    if (day > 364)
                    {
                        int m = day % 365 / 30;
                        dateDiff += day / 365 + "年";
                        if (m > 0)
                        {
                            dateDiff += m + "个月";
                        }
                        else
                        {
                            //dateDiff += "1个月";
                        }

                    }
                    else
                    {
                        dateDiff += day / 30 + "个月";
                    }
                }
                else
                {
                    //dateDiff += day.ToString() + "天";
                    dateDiff += "1个月";
                }
            }
            else
            {
                dateDiff += "1个月";
            }
            return dateDiff;
        }
        public static string DateDiff(this DateTime date, DateTime DateTimeNew, bool isnow)
        {
            string dateDiff = "";
            TimeSpan ts1 = new TimeSpan(date.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTimeNew.Ticks);
            if (isnow)
            {
                ts2 = new TimeSpan(DateTime.Now.Ticks);
            }
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            int day = ts.Days;
            int hou = ts.Hours;
            int minu = ts.Minutes;
            int sec = ts.Seconds;
            if (day > 0)
            {
                if (day > 30)
                {
                    if (day > 364)
                    {
                        int m = day % 365 / 30;
                        dateDiff += day / 365 + "年";
                        if (m > 0)
                        {
                            dateDiff += m + "个月";
                        }
                        else
                        {
                            //dateDiff += "1个月";
                        }

                    }
                    else
                    {
                        dateDiff += day / 30 + "个月";
                    }
                }
                else
                {
                    //dateDiff += day.ToString() + "天";
                    dateDiff += "1个月";
                }
            }
            else
            {
                dateDiff += "1个月";
            }
            return dateDiff;
        }
        /// <summary>
        /// 转化成时间戳-精确到毫秒
        /// </summary>
        /// <returns></returns>
        public static long ConvertToUnixTimestamp(this DateTime dt)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));

            DateTime nowTime = DateTime.Now;

            long unixTime = (long)(nowTime - startTime).TotalMilliseconds;
            return unixTime;
        }

        /// <summary>
        /// 将精确到毫秒级别的事件戳转化成时间
        /// </summary>
        /// <param name="stamp"></param>
        /// <returns></returns>
        public static DateTime ConvertUnixTimeTokenToDateTime(this long stamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            System.DateTime time = System.DateTime.MinValue;

            time = startTime.AddMilliseconds(stamp);
            return time;
        }

        /// <summary>
        /// 把秒转换成分钟
        /// </summary>
        /// <returns></returns>
        public static DateTime ConvertSecondToDateTime(this long Second)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            System.DateTime time = System.DateTime.MinValue;

            time = startTime.AddSeconds(Second);
            return time;
        }

        /// <summary>
        /// 把秒转换成分钟
        /// </summary>
        /// <returns></returns>
        public static int SecondToMinute(int Second)
        {
            decimal mm = (decimal)((decimal)Second / (decimal)60);
            return Convert.ToInt32(Math.Ceiling(mm));
        }

        #region 返回某年某月最后一天
        /// <summary>
        /// 返回某年某月最后一天
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>日</returns>
        public static int GetMonthLastDate(int year, int month)
        {
            DateTime lastDay = new DateTime(year, month, new System.Globalization.GregorianCalendar().GetDaysInMonth(year, month));
            int Day = lastDay.Day;
            return Day;
        }
        #endregion

    }
}
