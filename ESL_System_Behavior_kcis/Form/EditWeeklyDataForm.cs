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

namespace ESL_System_Behavior.Form
{
    public partial class EditWeeklyDataForm : BaseForm
    {

        List<WeeklyReportLogRecord> WeeklyReportLogRecordList;

        public EditWeeklyDataForm()
        {
            InitializeComponent();
            WeeklyReportLogRecordList = new List<WeeklyReportLogRecord>();
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

        private void LoadWeeklyReport()
        {
            dgWeeklyReport.Rows.Clear();
            txtGeneralComment.Text = "";
            foreach(WeeklyReportLogRecord rec in WeeklyReportLogRecordList)
            {
                int rowIdx = dgWeeklyReport.Rows.Add();

                dgWeeklyReport.Rows[rowIdx].Tag = rec;
                dgWeeklyReport.Rows[rowIdx].Cells[colCourseName.Index].Value = rec.CourseName;
                dgWeeklyReport.Rows[rowIdx].Cells[colTeacherName.Index].Value = rec.TeacherName;
            }

            if (WeeklyReportLogRecordList.Count > 0)
            {
                dgWeeklyReport.Rows[0].Selected = true;
                txtGeneralComment.Text = WeeklyReportLogRecordList[0].GeneralComment;
            }

        }

    }
}
