using System;

namespace NeteaseCloudMusic.Wpf.Helper
{
    public class DateTimeHelper
    {
        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns>UTC</returns> 
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// 转换时间戳为C#时间
        /// </summary>
        /// <param name="timeStamp">时间戳 单位：毫秒</param>
        /// <returns>C#时间</returns>
        public static DateTime ConvertTimeStampToDateTime(long timeStamp)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(timeStamp);
            return dt;
        }
    }
}
