using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESL_System_Behavior
{
    public class BehaviorRecord
    {
        /// <summary>
        /// 學生ID
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// UID
        /// </summary>
        public string UID { get; set; }
        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// CreateDate
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// CourseID
        /// </summary>
        public string CourseID { get; set; } //課程ID
        /// <summary>
        /// Course
        /// </summary>
        public string Course { get; set; }
        /// <summary>
        /// ref_teacher_id
        /// </summary>
        public string Teacher { get; set; }
        /// <summary>
        ///  is_good_behavior
        /// </summary>
        public Boolean IsGood { get; set; }
        /// <summary>
        /// dentetion
        /// </summary>
        public Boolean IsDentetion { get; set; }



        //淺層複製
        public BehaviorRecord ShallowCopy()
        {
            return (BehaviorRecord)this.MemberwiseClone();
        }
    }
}
