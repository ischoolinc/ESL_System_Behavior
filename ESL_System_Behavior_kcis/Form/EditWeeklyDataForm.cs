using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.Data;
using K12.Data;

namespace ESL_System_Behavior.Form
{
    public partial class EditWeeklyDataForm : BaseForm
    {

        List<WeeklyReportLogRecord> WeeklyReportLogRecordList;
        List<WeeklyDataInfo> WeeklyDataInfoList;
        List<BehaviorInfo> BehaviorInfoList;
        string SelectedWeeklyReportUID = "";
        string SelectedWeeklyDataUID = "";

        public EditWeeklyDataForm()
        {
            InitializeComponent();
            WeeklyReportLogRecordList = new List<WeeklyReportLogRecord>();
            WeeklyDataInfoList = new List<WeeklyDataInfo>();
            BehaviorInfoList = new List<BehaviorInfo>();
        }

        public void SetWeeklyReportLogRecord(List<WeeklyReportLogRecord> data)
        {
            WeeklyReportLogRecordList = data;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void EditWeeklyDataForm_Load(object sender, EventArgs e)
        {
            LoadWeeklyReport();
        }

        /// <summary>
        /// 載入讀取資料
        /// </summary>
        private void LoadWeeklyReport()
        {
            dgWeeklyReport.Rows.Clear();
            txtGeneralComment.Text = "";
            foreach (WeeklyReportLogRecord rec in WeeklyReportLogRecordList)
            {
                int rowIdx = dgWeeklyReport.Rows.Add();

                dgWeeklyReport.Rows[rowIdx].Tag = rec;
                dgWeeklyReport.Rows[rowIdx].Cells[colCourseName.Index].Value = rec.CourseName;
                dgWeeklyReport.Rows[rowIdx].Cells[colTeacherName.Index].Value = rec.TeacherName;
                dgWeeklyReport.Rows[rowIdx].Cells[colBBeginDate.Index].Value = rec.BeginDate;
                dgWeeklyReport.Rows[rowIdx].Cells[colBEndDate.Index].Value = rec.EndDate;
            }

            if (WeeklyReportLogRecordList.Count > 0)
            {
                dgWeeklyReport.Rows[0].Selected = true;
                txtGeneralComment.Text = WeeklyReportLogRecordList[0].GeneralComment;
                SelectedWeeklyReportUID = WeeklyReportLogRecordList[0].UID;
                LoadLoadWeeklyDataByReportID(WeeklyReportLogRecordList[0]);
            }
        }

        /// <summary>
        /// 透過 ReportID 取得相關 Weekly Data
        /// </summary>
        /// <param name="rptID"></param>
        private void LoadLoadWeeklyDataByReportID(WeeklyReportLogRecord rec)
        {
            if (!string.IsNullOrWhiteSpace(rec.UID))
            {
                // 取得這份 Weekly Report 關聯的學生與 Personal Comment
                string SQL = "SELECT " +
                    "$esl.weekly_data.uid" +
                    ",student.id AS student_id" +
                    ",class.class_name" +
                    ",student.seat_no" +
                    ",student.name AS student_name" +
                    ",personal_comment " +
                    " FROM $esl.weekly_data " +
                    "INNER JOIN student " +
                    " ON student.id = $esl.weekly_data.ref_student_id " +
                    " LEFT JOIN class ON student.ref_class_id = class.id  " +
                    " WHERE $esl.weekly_data.ref_weekly_report_uid =" + rec.UID +
                    " ORDER BY class.display_order,class_name,seat_no,student.name;";

                WeeklyDataInfoList.Clear();

                QueryHelper qh = new QueryHelper();
                DataTable dt = qh.Select(SQL);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        WeeklyDataInfo wdi = new WeeklyDataInfo();
                        wdi.UID = "" + dr["uid"];
                        wdi.ClassName = "" + dr["class_name"];
                        wdi.CourseID = rec.CourseID;
                        wdi.TeacherID = rec.TeacherID;
                        wdi.StudentID = "" + dr["student_id"];
                        wdi.PersonalComment = "" + dr["personal_comment"];
                        wdi.SeatNo = "" + dr["seat_no"];
                        wdi.StudentName = "" + dr["student_name"];
                        WeeklyDataInfoList.Add(wdi);
                    }
                }


                // Load To DataGrid
                dgStudentData.Rows.Clear();
                txtTeacherComment.Text = "";
                foreach (WeeklyDataInfo data in WeeklyDataInfoList)
                {
                    int rowIdx = dgStudentData.Rows.Add();
                    dgStudentData.Rows[rowIdx].Tag = data;
                    dgStudentData.Rows[rowIdx].Cells[colClassName.Index].Value = data.ClassName;
                    dgStudentData.Rows[rowIdx].Cells[colSeatNo.Index].Value = data.SeatNo;
                    dgStudentData.Rows[rowIdx].Cells[colName.Index].Value = data.StudentName;
                }

                if (WeeklyDataInfoList.Count > 0)
                {
                    dgStudentData.Rows[0].Selected = true;
                    txtTeacherComment.Text = WeeklyDataInfoList[0].PersonalComment;
                    SelectedWeeklyDataUID = WeeklyDataInfoList[0].UID;
                    LoadBehaviorData(WeeklyDataInfoList[0].StudentID, WeeklyDataInfoList[0].CourseID, WeeklyDataInfoList[0].TeacherID);
                }
            }
        }

        private void LoadBehaviorData(string StudentID, string CourseID, string TeacherID)
        {
            string sql = "SELECT " +
                "uid" +
                ",create_date" +
                ",comment" +
                ",is_good_behavior" +
                ",detention " +
                " FROM $esl.behavior_data" +
                " WHERE ref_course_id = " + CourseID + " AND ref_student_id = " + StudentID + " AND ref_teacher_id = " + TeacherID + " ORDER BY create_date";

            BehaviorInfoList.Clear();
            dgBehavior.Rows.Clear();

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(sql);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    BehaviorInfo bi = new BehaviorInfo();
                    bi.Comment = "" + dr["comment"];
                    bi.CourseID = CourseID;
                    bi.UID = "" + dr["uid"];
                    bi.isGood = false;
                    bi.isDetention = false;
                    bi.CreateDate = "" + dr["create_date"] != "" ? DateTime.Parse("" + dr["create_date"]).ToString("yyyy/MM/dd") : "";
                    if ("" + dr["is_good_behavior"] == "true")
                    {
                        bi.isGood = true;
                    }
                    if ("" + dr["detention"] == "true")
                    {
                        bi.isDetention = true;
                    }
                    bi.TeacherID = TeacherID;
                    bi.StudentID = StudentID;
                    BehaviorInfoList.Add(bi);
                }
            }

            foreach (BehaviorInfo bi in BehaviorInfoList)
            {
                int rowIdx = dgBehavior.Rows.Add();
                dgBehavior.Rows[rowIdx].Tag = bi;
                dgBehavior.Rows[rowIdx].Cells[colBCreateDate.Index].Value = bi.CreateDate;
                dgBehavior.Rows[rowIdx].Cells[colBComment.Index].Value = bi.Comment;
                dgBehavior.Rows[rowIdx].Cells[colGood.Index].Value = false;
                dgBehavior.Rows[rowIdx].Cells[colDetention.Index].Value = false;

                if (bi.isGood)
                    dgBehavior.Rows[rowIdx].Cells[colGood.Index].Value = true; ;

                if (bi.isDetention)
                    dgBehavior.Rows[rowIdx].Cells[colDetention.Index].Value = true;

            }
        }


        private void dgWeeklyReport_SelectionChanged(object sender, EventArgs e)
        {
            txtGeneralComment.Text = "";
            SelectedWeeklyReportUID = "";
            if (dgWeeklyReport.SelectedRows.Count > 0)
            {
                WeeklyReportLogRecord rec = dgWeeklyReport.SelectedRows[0].Tag as WeeklyReportLogRecord;
                if (rec != null)
                {
                    txtGeneralComment.Text = rec.GeneralComment;
                    SelectedWeeklyReportUID = rec.UID;
                    LoadLoadWeeklyDataByReportID(rec);
                }
            }
        }

        private void dgStudentData_SelectionChanged(object sender, EventArgs e)
        {
            txtTeacherComment.Text = "";
            SelectedWeeklyDataUID = "";
            if (dgStudentData.SelectedRows.Count > 0)
            {
                WeeklyDataInfo wdi = dgStudentData.SelectedRows[0].Tag as WeeklyDataInfo;
                if (wdi != null)
                {
                    txtTeacherComment.Text = wdi.PersonalComment;
                    SelectedWeeklyDataUID = wdi.UID;
                    LoadBehaviorData(wdi.StudentID, wdi.CourseID, wdi.TeacherID);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            bool chkPass = true;
            List<string> errorMsgList = new List<string>();
            // 檢查資料
            foreach (DataGridViewRow drv in dgBehavior.Rows)
            {
                // 檢查 good,detention 不能同時 true
                bool cG = false, cD = false;
                bool.TryParse(drv.Cells[colGood.Index].Value + "", out cG);
                bool.TryParse(drv.Cells[colDetention.Index].Value + "", out cD);
                if (cG == true && cD == true)
                {
                    chkPass = false;
                    errorMsgList.Add("Good 與 Detention 不能同時勾選。");
                    break;
                }
            }

            if (chkPass)
            {
                try
                {
                    List<string> updateCmdList = new List<string>();
                    // 更新 General Comment
                    string gComment = txtGeneralComment.Text.Replace("'", "''");
                    string updateGComment = "UPDATE $esl.weekly_report SET general_comment = '" + gComment + "' WHERE uid = " + SelectedWeeklyReportUID + ";";
                    if (!string.IsNullOrWhiteSpace(SelectedWeeklyReportUID))
                    {
                        updateCmdList.Add(updateGComment);
                        foreach (WeeklyReportLogRecord rec in WeeklyReportLogRecordList)
                        {
                            if (rec.UID == SelectedWeeklyReportUID)
                            {
                                rec.GeneralComment = txtGeneralComment.Text;
                            }
                        }
                    }


                    // 更新 PersionComment 
                    string pComment = txtTeacherComment.Text.Replace("'", "''");
                    string updatePComment = "UPDATE $esl.weekly_data SET personal_comment = '" + pComment + "' WHERE uid = " + SelectedWeeklyDataUID + ";";

                    if (!string.IsNullOrWhiteSpace(SelectedWeeklyDataUID))
                    {
                        updateCmdList.Add(updatePComment);

                        foreach (WeeklyDataInfo wdi in WeeklyDataInfoList)
                        {
                            if (wdi.UID == SelectedWeeklyDataUID)
                            {
                                wdi.PersonalComment = txtTeacherComment.Text;
                            }
                        }
                    }

                    // 更新 Behavior
                    BehaviorInfoList.Clear();
                    foreach (DataGridViewRow drv in dgBehavior.Rows)
                    {
                        BehaviorInfo bi = drv.Tag as BehaviorInfo;
                        if (bi != null)
                        {
                            bi.Comment = drv.Cells[colBComment.Index].Value + "";
                            bool bG = false, bD = false;
                            bool.TryParse(drv.Cells[colGood.Index].Value + "", out bG);
                            bool.TryParse(drv.Cells[colDetention.Index].Value + "", out bD);
                            bi.isGood = bG;
                            bi.isDetention = bD;
                            
                            BehaviorInfoList.Add(bi);
                        }
                    }


                    foreach (BehaviorInfo bi in BehaviorInfoList)
                    {
                        string comm = bi.Comment.Replace("'", "''");
                        string good = "false";
                        string det = "false";
                        if (bi.isGood)
                            good = "true";

                        if (bi.isDetention)
                            det = "true";

                        string strSQL = "UPDATE $esl.behavior_data SET comment='" + comm + "',is_good_behavior=" + good + ",detention=" + det + " WHERE UID = " + bi.UID + "";

                        if (!string.IsNullOrWhiteSpace(bi.UID))
                        {
                            updateCmdList.Add(strSQL);
                        }

                    }

                    // 更新資料
                    UpdateHelper uh = new UpdateHelper();
                    uh.Execute(updateCmdList);

                    MessageBox.Show("儲存完成");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("儲存失敗," + ex.Message);
                    btnSave.Enabled = true;
                }
            }
            else
            {
                if (errorMsgList.Count > 0)
                {
                    MessageBox.Show(string.Join(",", errorMsgList.ToArray()));
                }
            }




            btnSave.Enabled = true;
        }
    }
}
