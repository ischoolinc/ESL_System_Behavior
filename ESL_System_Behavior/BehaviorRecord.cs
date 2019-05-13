using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESL_System_Behavior
{
    public class BehaviorRecord
    {
        public string StudentID { get; set; }
        public string UID { get; set; }
        public string Comment { get; set; }
        public string CreateDate { get; set; }
        public string CourseID { get; set; } //課程ID
        public string Course { get; set; }
        public string Teacher { get; set; }
        public Boolean IsGood { get; set; }
        public Boolean IsDentetion { get; set; }



        //淺層複製
        public BehaviorRecord ShallowCopy()
        {
            return (BehaviorRecord)this.MemberwiseClone();
        }
    }
}
