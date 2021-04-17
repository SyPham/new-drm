using System;
using System.Collections.Generic;
using System.Text;

namespace TodolistScheduleService.Dto
{
   public class SendMailParams
    {
        public SendMailParams()
        {
        }
        /// <summary>
        /// Đặt Identity Key cho 1 công việc
        /// </summary>
        /// <param name="report">Loại báo cáo</param>
        /// <param name="frequency">Báo cáo theo thời gian VD: Hàng ngày, hàng tuần, hàng tháng</param>
        public void IdentityParams(string report, string frequency)
        {
            Report = report;
            Frequency = frequency;
        }
        /// <summary>
        /// Thông tin request đến Web_API
        /// </summary>
        /// <param name="uRL">Đường dẫn của Web_API</param>
        /// <param name="type"></param>
        /// <param name="pathName">Tên đường dẫn (Tên controller và method)</param>
        /// <param name="emails">Danh sách người nhận</param>
        public void APIInfo(string uRL, string type, string pathName, List<string> emails)
        {
            URL = uRL;
            Type = type;
            PathName = pathName;
            Emails = emails;
        }
        public string GetIdentityParams()
        {
            return $"{Report}-{Frequency}";
        }
        public string URL { get; set; }
        public string Type { get; set; }
        public string PathName { get; set; }
        public string Report { get; set; }
        public string Frequency { get; set; }
        public List<string> Emails { get; set; }
    }
}
