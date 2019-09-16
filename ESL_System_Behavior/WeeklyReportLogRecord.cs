using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESL_System_Behavior
{
    public class WeeklyReportLogRecord
    {


        /// <summary>
        /// EndDate
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// CourseID
        /// </summary>
        public string CourseID { get; set; } //課程ID

        /// <summary>
        /// CourseName
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// TeacherID
        /// </summary>
        public string TeacherID { get; set; } //教師ID

        /// <summary>
        /// TeacherName
        /// </summary>
        public string TeacherName { get; set; }

        /// <summary>
        /// WeeklyReportCount
        /// </summary>
        public int WeeklyReportCount { get; set; } // 本周建立筆數

    }
}
