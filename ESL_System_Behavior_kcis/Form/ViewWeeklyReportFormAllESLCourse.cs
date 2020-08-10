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
using DevComponents.DotNetBar;
using System.Xml.Linq;
using K12.Data;
using System.Xml;
using FISCA.Authentication;
using FISCA.LogAgent;
using K12.Data;

namespace ESL_System_Behavior.Form
{
    public partial class ViewWeeklyReportFormAllESLCourse : BaseForm
    {
        // 查詢資料
        private Dictionary<string, WeeklyReportLogRecord> WeeklyReportLogDict = new Dictionary<string, WeeklyReportLogRecord>();


        // 修改與刪除資料用
        Dictionary<string, List<WeeklyReportLogRecord>> WeeklyReportDataDict = new Dictionary<string, List<WeeklyReportLogRecord>>();

        public ViewWeeklyReportFormAllESLCourse()
        {
            InitializeComponent();
        }

        private void BehaviorCommentSettingForm_Load(object sender, EventArgs e)
        {

            comboBoxEx1.Items.Add("" + (int.Parse(K12.Data.School.DefaultSchoolYear) - 1));
            comboBoxEx1.Items.Add(K12.Data.School.DefaultSchoolYear);
            comboBoxEx1.Items.Add("" + (int.Parse(K12.Data.School.DefaultSchoolYear) + 1));
            comboBoxEx1.Text = K12.Data.School.DefaultSchoolYear;

            comboBoxEx2.Items.Add("1");
            comboBoxEx2.Items.Add("2");
            comboBoxEx2.Text = K12.Data.School.DefaultSemester;

        }


        // 載入搜尋資料
        private void LoadSearchData()
        {
            if (dateTimeInput1.Text == "" || dateTimeInput2.Text == "")
            {
                MessageBox.Show("請輸入完整日期區間!");
                return;
            }

            if (dateTimeInput1.Value > dateTimeInput2.Value)
            {
                MessageBox.Show("結束日期必須大於開始日期!");
                return;
            }


            buttonX1.Enabled = false;
            btnDel.Enabled = false;
            btnEdit.Enabled = false;
            dataGridViewX1.Rows.Clear();
            WeeklyReportLogDict.Clear();
            WeeklyReportDataDict.Clear();

            // 選出套用ESL樣板的課程 檢查時間區段裡 老師是否有建立weekly_report

            string query = string.Format(@"SELECT 
                                            course.id AS course_id
                                            ,course.course_name
                                            ,exam_template.description 
                                            ,teacher.id AS teacher_id
	                                        ,teacher.teacher_name
	                                        ,tc_instruct.sequence
                                            ,exam_template.id AS templateID
                                            ,exam_template.name AS templateName
                                            ,$esl.weekly_report.begin_date
	                                        ,$esl.weekly_report.end_date
                                            ,$esl.weekly_report.uid
                                            ,$esl.weekly_report.general_comment
                                        FROM course 
                                        LEFT JOIN  exam_template ON course.ref_exam_template_id =exam_template.id  
                                        LEFT JOIN  tc_instruct ON tc_instruct.ref_course_id =course.id  
                                        LEFT JOIN  teacher ON   teacher.id = tc_instruct.ref_teacher_id 
                                        LEFT JOIN   $esl.weekly_report ON $esl.weekly_report.ref_course_id =course.id  AND $esl.weekly_report.ref_teacher_id = teacher.id
                                        WHERE   exam_template.description IS NOT NULL 
                                        AND course.school_year = '{0}' 
                                        AND semester = '{1}' 
                                        AND (end_date >= TIMESTAMP '{2}' AND end_date<= TIMESTAMP '{3}' OR end_date IS NULL)
                                        ORDER BY course_name,sequence", comboBoxEx1.Text, comboBoxEx2.Text, dateTimeInput1.Value.Date.ToShortDateString(), dateTimeInput2.Value.Date.ToShortDateString());

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string rKey = "" + dr["course_id"] + "_" + dr["teacher_id"];

                    WeeklyReportLogRecord lr = new WeeklyReportLogRecord();
                    lr.RowKey = rKey;
                    lr.UID = "" + dr["uid"];
                    lr.CourseID = "" + dr["course_id"];
                    lr.CourseName = "" + dr["course_name"];

                    lr.BeginDate = "" + dr["begin_date"] != "" ? DateTime.Parse("" + dr["begin_date"]).ToString("yyyy/MM/dd") : "";
                    lr.EndDate = "" + dr["end_date"] != "" ? DateTime.Parse("" + dr["end_date"]).ToString("yyyy/MM/dd") : "";

                    lr.TeacherID = "" + dr["teacher_id"];
                    lr.TeacherName = "" + dr["teacher_name"];
                    lr.GeneralComment = "" + dr["general_comment"];

                    // 沒有時間 代表 本周沒有建立WeeklyReport
                    if ("" + dr["end_date"] != "")
                    {
                        lr.WeeklyReportCount = 1;
                    }
                    else
                    {
                        lr.WeeklyReportCount = 0;
                    }

                    // 查詢畫面使用
                    if (!WeeklyReportLogDict.ContainsKey(rKey))
                    {
                        WeeklyReportLogDict.Add(rKey, lr);
                    }
                    else
                    {
                        WeeklyReportLogDict[rKey].WeeklyReportCount++;
                    }

                    // 修改刪除使用
                    if (!WeeklyReportDataDict.ContainsKey(rKey))
                    {
                        WeeklyReportDataDict.Add(rKey, new List<WeeklyReportLogRecord>());
                    }

                    WeeklyReportDataDict[rKey].Add(lr);
                }

                foreach (string key in WeeklyReportLogDict.Keys)
                {

                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);
                    row.Tag = key;
                    row.Cells[0].Value = WeeklyReportLogDict[key].CourseName;
                    row.Cells[1].Value = WeeklyReportLogDict[key].TeacherName;
                    row.Cells[2].Value = WeeklyReportLogDict[key].BeginDate;
                    row.Cells[3].Value = WeeklyReportLogDict[key].EndDate;
                    row.Cells[4].Value = WeeklyReportLogDict[key].WeeklyReportCount == 0 ? "本週尚未建立" : "本週已建立" + WeeklyReportLogDict[key].WeeklyReportCount + "筆";

                    // 如果沒有建立 標紅色
                    if (WeeklyReportLogDict[key].WeeklyReportCount == 0)
                    {
                        DataGridViewCellStyle s = new DataGridViewCellStyle();

                        s.ForeColor = Color.Red;

                        row.Cells[4].Style = s;
                    }

                    dataGridViewX1.Rows.Add(row);
                }
            }

            buttonX1.Enabled = true;
            btnDel.Enabled = true;
            btnEdit.Enabled = true;
        }


        // 儲存
        private void buttonX1_Click(object sender, EventArgs e)
        {
            LoadSearchData();
        }

        // 取消
        private void buttonX2_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            // 刪除
            btnDel.Enabled = false;

            if (dataGridViewX1.Rows.Count > 0)
            {
                if (dataGridViewX1.SelectedRows.Count == 1)
                {
                    List<string> delUID = new List<string>();
                    if (dataGridViewX1.SelectedRows[0].Tag != null)
                    {
                        string rKey = dataGridViewX1.SelectedRows[0].Tag.ToString();
                        if (WeeklyReportDataDict.ContainsKey(rKey))
                        {
                            foreach (WeeklyReportLogRecord rec in WeeklyReportDataDict[rKey])
                            {
                                delUID.Add(rec.UID);
                            }
                        }

                        if (delUID.Count > 0)
                        {
                            if (MsgBox.Show("按「是」將刪除" + delUID.Count + " 筆資料，請問確認刪除?", "刪除資料", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                            {
                                try
                                {
                                    // del SQL，先刪除子資料在刪除主資料
                                    //DELETE FROM $esl.weekly_data WHERE ref_weekly_report_uid IN(111);
                                    //DELETE FROM $esl.weekly_report WHERE UID IN(111);
                                    string uid = string.Join(",", delUID.ToArray());
                                    List<string> sqlList = new List<string>();
                                    sqlList.Add("DELETE FROM $esl.weekly_data WHERE ref_weekly_report_uid IN(" + uid + ");");
                                    sqlList.Add("DELETE FROM $esl.weekly_report WHERE UID IN(" + uid + ");");

                                    // update
                                    UpdateHelper uh = new UpdateHelper();
                                    uh.Execute(sqlList);

                                    // 寫 Log
                                    MsgBox.Show("刪除完成");
                                    LoadSearchData();

                                }
                                catch (Exception ex)
                                {
                                    MsgBox.Show("刪除資料發生錯誤", ex.Message);
                                }
                            }
                        }
                    }             
                }
            }

            btnDel.Enabled = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            btnEdit.Enabled = false;
            if (dataGridViewX1.Rows.Count > 0)
            {
                if (dataGridViewX1.SelectedRows.Count == 1)
                {
                    if (dataGridViewX1.SelectedRows[0].Tag != null)
                    {
                        string rKey = dataGridViewX1.SelectedRows[0].Tag.ToString();
                        if (WeeklyReportDataDict.ContainsKey(rKey))
                        {
                            if (WeeklyReportDataDict[rKey].Count > 0)
                            {
                                EditWeeklyDataForm wedf = new EditWeeklyDataForm();
                                wedf.SetWeeklyReportLogRecord(WeeklyReportDataDict[rKey]);
                                wedf.ShowDialog();
                            }
                        }
                    }
                  
                }
            }
            btnEdit.Enabled = true;
        }
    }
}


