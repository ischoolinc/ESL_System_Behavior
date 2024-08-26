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
        string SelectdBeginDate = "";
        string SelectdEndDate = "";
        string oldGeneralComment = "";
        string oldTeacherComment = "";


        public EditWeeklyDataForm()
        {
            InitializeComponent();
            WeeklyReportLogRecordList = new List<WeeklyReportLogRecord>();
            WeeklyDataInfoList = new List<WeeklyDataInfo>();
            BehaviorInfoList = new List<BehaviorInfo>();

            this.Text = "修改 " + NameCheck.ReportName;
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
                oldGeneralComment = txtGeneralComment.Text;
                SelectdBeginDate = WeeklyReportLogRecordList[0].BeginDate;
                SelectdEndDate = WeeklyReportLogRecordList[0].EndDate;
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
                    ",student.student_number AS student_number" +
                    ",student.english_name AS english_name" +
                    " FROM $esl.weekly_data " +
                    "INNER JOIN student " +
                    " ON student.id = $esl.weekly_data.ref_student_id " +
                    " LEFT JOIN class ON student.ref_class_id = class.id  " +
                    " WHERE $esl.weekly_data.ref_weekly_report_uid =" + rec.UID +
                    " ORDER BY student_number,class.display_order,class_name,seat_no,student.name;";

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
                        wdi.SeatNo = 0;
                        int seatNo;
                        if(int.TryParse("" + dr["seat_no"],out seatNo))
                        {
                            wdi.SeatNo = seatNo;
                        }
                       
                        wdi.StudentName = "" + dr["student_name"];
                        wdi.StudentNumber = "" + dr["student_number"];
                        wdi.EnglishName = "" + dr["english_name"];

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
                    dgStudentData.Rows[rowIdx].Cells[colStudentNumber.Index].Value = data.StudentNumber;
                    dgStudentData.Rows[rowIdx].Cells[colEnglishName.Index].Value = data.EnglishName;

                }

                if (WeeklyDataInfoList.Count > 0)
                {
                    dgStudentData.Rows[0].Selected = true;
                    txtTeacherComment.Text = WeeklyDataInfoList[0].PersonalComment;
                    SelectedWeeklyDataUID = WeeklyDataInfoList[0].UID;
                    oldTeacherComment = txtTeacherComment.Text;
                    LoadBehaviorData(WeeklyDataInfoList[0].StudentID, WeeklyDataInfoList[0].CourseID, WeeklyDataInfoList[0].TeacherID);
                }
            }
        }

        private void LoadBehaviorData(string StudentID, string CourseID, string TeacherID)
        {
            try
            {
                if (string.IsNullOrEmpty(SelectdBeginDate))
                {
                    SelectdBeginDate = DateTime.Now.ToString("yyyy-MM-dd");
                }
                if (string.IsNullOrEmpty(SelectdEndDate))
                {
                    SelectdEndDate = DateTime.Now.ToString("yyyy-MM-dd");
                }

                SelectdBeginDate = DateTime.Parse(SelectdBeginDate).ToString("yyyy-MM-dd");
                SelectdEndDate = DateTime.Parse(SelectdEndDate).ToString("yyyy-MM-dd");

                string sql = "SELECT " +
                    "uid" +
                    ",create_date" +
                    ",comment" +
                    ",is_good_behavior" +
                    ",detention " +
                    " FROM $esl.behavior_data" +
                    " WHERE ref_course_id = " + CourseID + "" +
                    " AND ref_student_id = " + StudentID + "" +
                    " AND ref_teacher_id = " + TeacherID + "" +
                    " AND create_date >= '" + SelectdBeginDate + "'" +
                    " AND create_date <= '" + SelectdEndDate + "'" +
                    " ORDER BY create_date";

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
            catch (Exception ex)
            {
                MessageBox.Show("載入 Behavior 資料失敗," + ex.Message);
            }

        }


        private void dgWeeklyReport_SelectionChanged(object sender, EventArgs e)
        {
            txtGeneralComment.Text = "";
            SelectedWeeklyReportUID = "";
            SelectdBeginDate = SelectdEndDate = "";
            if (dgWeeklyReport.SelectedRows.Count > 0)
            {
                WeeklyReportLogRecord rec = dgWeeklyReport.SelectedRows[0].Tag as WeeklyReportLogRecord;
                if (rec != null)
                {
                    txtGeneralComment.Text = rec.GeneralComment;
                    SelectedWeeklyReportUID = rec.UID;
                    oldGeneralComment = rec.GeneralComment;
                    SelectdBeginDate = rec.BeginDate;
                    SelectdEndDate = rec.EndDate;
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
                    oldTeacherComment = txtTeacherComment.Text;
                    LoadBehaviorData(wdi.StudentID, wdi.CourseID, wdi.TeacherID);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            bool chkPass = true;
            List<string> errorMsgList = new List<string>();
            List<string> logStrList = new List<string>();
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
                        if (oldGeneralComment != txtGeneralComment.Text)
                        {
                            logStrList.Add("Genner Comment 由「" + oldGeneralComment + "」改成「" + txtGeneralComment.Text + "」。");
                        }

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

                        logStrList.Add("Teacher Comment 由「" + oldTeacherComment + "」改成「" + txtTeacherComment.Text + "」。");

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
                    List<string> tmpList = new List<string>();
                    foreach (DataGridViewRow drv in dgBehavior.Rows)
                    {
                        tmpList.Clear();
                        BehaviorInfo bi = drv.Tag as BehaviorInfo;
                        if (bi != null)
                        {
                            // 處理 log
                            if (bi.Comment != drv.Cells[colBComment.Index].Value + "")
                            {
                                tmpList.Add("Behavior Comment 由「" + bi.Comment + "」改成「" + drv.Cells[colBComment.Index].Value + "" + "」。");
                            }


                            bi.Comment = drv.Cells[colBComment.Index].Value + "";

                            bool bG = false, bD = false;
                            bool.TryParse(drv.Cells[colGood.Index].Value + "", out bG);
                            bool.TryParse(drv.Cells[colDetention.Index].Value + "", out bD);
                            if (bi.isGood != bG)
                            {
                                tmpList.Add("Behavior Good 由「" + bi.isGood + "」改成「" + bG + "」。");
                            }

                            bi.isGood = bG;

                            if (bi.isDetention != bD)
                            {
                                tmpList.Add("Behavior Detention 由「" + bi.isDetention + "」改成「" + bD + "」。");
                            }

                            bi.isDetention = bD;

                            if (tmpList.Count > 0)
                            {
                                string str = "Behavior 日期:" + bi.CreateDate ;
                                logStrList.Add(str);
                                foreach (string s in tmpList)
                                    logStrList.Add(s);
                            }

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

                    if (logStrList.Count > 0)
                    {
                        //  取得目前所選學生
                        if (dgStudentData.SelectedRows.Count > 0)
                        {
                            WeeklyDataInfo wdi = dgStudentData.SelectedRows[0].Tag as WeeklyDataInfo;
                            if (wdi != null)
                            {
                                string sinfo = "班級:" + wdi.ClassName + " ,座號：" + wdi.SeatNo + "  ,姓名：" + wdi.StudentName + " ,\n";
                                FISCA.LogAgent.ApplicationLog.Log("ESL 檢視週報表", "修改資料", "student", wdi.StudentID, sinfo + string.Join("\n", logStrList.ToArray()));
                            }

                        }


                    }

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

        private void dgBehavior_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        private void dgBehavior_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.ColumnIndex == colGood.Index)
                {
                    bool b;
                    bool.TryParse(dgBehavior.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + "", out b);

                    if (b == false)
                    {
                        dgBehavior.Rows[e.RowIndex].Cells[colGood.Index].Value = true;
                        dgBehavior.Rows[e.RowIndex].Cells[colDetention.Index].Value = false;
                    }
                }

                if (e.ColumnIndex == colDetention.Index)
                {
                    bool b;
                    bool.TryParse(dgBehavior.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + "", out b);

                    if (b == false)
                    {
                        dgBehavior.Rows[e.RowIndex].Cells[colGood.Index].Value = false;
                        dgBehavior.Rows[e.RowIndex].Cells[colDetention.Index].Value = true;
                    }
                }
            }
        }
    }
}
