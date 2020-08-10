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
    public partial class ViewWeeklyReportForm : BaseForm
    {
        
        public ViewWeeklyReportForm()
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

            string query = string.Format(@"SELECT $esl.weekly_report.uid,course.course_name,teacher.teacher_name,$esl.weekly_report.end_date,$esl.weekly_report.is_send FROM $esl.weekly_report 
LEFT JOIN course ON $esl.weekly_report.ref_course_id = course.id
LEFT JOIN teacher ON $esl.weekly_report.ref_teacher_id = teacher.id
WHERE course.school_year = '{0}' AND semester = '{1}' AND end_date >= TIMESTAMP '{2}' AND end_date<= TIMESTAMP '{3}'",comboBoxEx1.Text,comboBoxEx2.Text,dateTimeInput1.Value.Date.ToShortDateString(), dateTimeInput2.Value.Date.ToShortDateString());

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);

                    row.Tag = dr["uid"];
                    row.Cells[0].Value = dr["course_name"];
                    row.Cells[1].Value = dr["teacher_name"];
                    row.Cells[2].Value = ""+ dr["is_send"] == "true" ? "是" : "否";
                    
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


