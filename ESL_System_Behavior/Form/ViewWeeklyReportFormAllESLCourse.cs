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
        private Dictionary<string, WeeklyReportLogRecord> WeeklyReportLogDict = new Dictionary<string, WeeklyReportLogRecord>();


        public ViewWeeklyReportFormAllESLCourse()
        {
            InitializeComponent();
        }

        private void BehaviorCommentSettingForm_Load(object sender, EventArgs e)
        {

            comboBoxEx1.Items.Add(""+( int.Parse(K12.Data.School.DefaultSchoolYear) - 1));
            comboBoxEx1.Items.Add(K12.Data.School.DefaultSchoolYear);
            comboBoxEx1.Items.Add("" + (int.Parse(K12.Data.School.DefaultSchoolYear) +1));
            comboBoxEx1.Text = K12.Data.School.DefaultSchoolYear;

            comboBoxEx2.Items.Add("1");
            comboBoxEx2.Items.Add("2");
            comboBoxEx2.Text = K12.Data.School.DefaultSemester;

        }


        // 儲存
        private void buttonX1_Click(object sender, EventArgs e)
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


            dataGridViewX1.Rows.Clear();

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
	                                        ,$esl.weekly_report.end_date
                                        FROM course 
                                        LEFT JOIN  exam_template ON course.ref_exam_template_id =exam_template.id  
                                        LEFT JOIN  tc_instruct ON tc_instruct.ref_course_id =course.id  
                                        LEFT JOIN  teacher ON   teacher.id = tc_instruct.ref_teacher_id 
                                        LEFT JOIN   $esl.weekly_report ON $esl.weekly_report.ref_course_id =course.id  AND $esl.weekly_report.ref_teacher_id = teacher.id
                                        WHERE   exam_template.description IS NOT NULL 
                                        AND course.school_year = '{0}' 
                                        AND semester = '{1}' 
                                        AND (end_date >= TIMESTAMP '{2}' AND end_date<= TIMESTAMP '{3}' OR end_date IS NULL)
                                        ORDER BY course_name,sequence", comboBoxEx1.Text,comboBoxEx2.Text,dateTimeInput1.Value.Date.ToShortDateString(), dateTimeInput2.Value.Date.ToShortDateString());

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (!WeeklyReportLogDict.ContainsKey("" + dr["course_id"] + "_" + dr["teacher_id"]))
                    {
                        WeeklyReportLogRecord lr = new WeeklyReportLogRecord();

                        lr.CourseID = "" + dr["course_id"];
                        lr.CourseName = "" + dr["course_name"];
                        lr.EndDate = "" + dr["end_date"];
                        lr.TeacherID = "" + dr["teacher_id"];
                        lr.TeacherName = "" + dr["teacher_name"];

                        // 沒有時間 代表 本周沒有建立WeeklyReport
                        if ("" + dr["end_date"] != "")
                        {
                            lr.WeeklyReportCount = 1;
                        }
                        else
                        {
                            lr.WeeklyReportCount = 0;
                        }


                        WeeklyReportLogDict.Add("" + dr["course_id"] + "_" + dr["teacher_id"], lr);
                    }
                    else
                    {
                        WeeklyReportLogDict["" + dr["course_id"] + "_" + dr["teacher_id"]].WeeklyReportCount++;
                    }                                       
                }

                foreach (string key in WeeklyReportLogDict.Keys)
                {

                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);

                    row.Cells[0].Value = WeeklyReportLogDict[key].CourseName;
                    row.Cells[1].Value = WeeklyReportLogDict[key].TeacherName;
                    row.Cells[2].Value = WeeklyReportLogDict[key].WeeklyReportCount == 0 ? "本周尚未建立" : "本周已建立" + WeeklyReportLogDict[key].WeeklyReportCount + "筆";

                    // 如果沒有建立 標紅色
                    if (WeeklyReportLogDict[key].WeeklyReportCount == 0)
                    {
                        DataGridViewCellStyle s = new DataGridViewCellStyle();

                        s.ForeColor = Color.Red;

                        row.Cells[2].Style = s;
                    }

                    dataGridViewX1.Rows.Add(row);
                }
            }


        }

        // 取消
        private void buttonX2_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        
    }
}


