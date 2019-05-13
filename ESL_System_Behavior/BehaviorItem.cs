using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FISCA.Presentation;
using FISCA.Data;
using ESL_System_Behavior.Form;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.LogAgent;
using static System.Windows.Forms.ListViewItem;

namespace ESL_System_Behavior
{
    public partial class BehaviorItem : DetailContent
    {
        private QueryHelper qp = new QueryHelper();
        private BackgroundWorker _worker = new BackgroundWorker();
        private List<BehaviorRecord> _listBehaviorReocrd = new List<BehaviorRecord>();//抓每筆學生資料
        private string _RunningID;

        private string _PermissionCode = "6B4FFF43-611A-4344-8711-4BA0F85DB73B";
        private bool _Editable = false;


        public BehaviorItem()
        {
            InitializeComponent();
            Group = "生活行為紀錄";

            _worker.DoWork += new DoWorkEventHandler(BkW_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkW_RunWorkerCompleted);
            //當DetailContent的PrimaryKey 變動
            this.PrimaryKeyChanged += delegate { LoadingData(); };

            this._Editable = FISCA.Permission.UserAcl.Current[this._PermissionCode].Editable;

            btnInsert.Enabled = this._Editable;
            btnUpdate.Enabled = this._Editable;
            btnDelete.Enabled = this._Editable;
        }

        void BkW_DoWork(object sender, DoWorkEventArgs e)
        {

            if (String.IsNullOrEmpty(this.PrimaryKey))
            {
                return;
            }
            else
            {
                GetStudentBehavior();
            }
        }

        void BkW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_RunningID != PrimaryKey)
            {
                LoadingData();
            }

            else
            {
                FillListView();
                Loading = false;
            }
        }

        //填入ListView
        private void FillListView()
        {
            //先清空
            listView.Items.Clear();

            if (_listBehaviorReocrd.Count() > 0)
            {
                foreach (BehaviorRecord record in _listBehaviorReocrd)
                {
                    string[] row = { record.Date, record.Comment, record.Course };
                    ListViewItem itm = new ListViewItem(row);

                    if (record.IsDentetion == true)
                    {
                        itm.ForeColor = Color.Red;
                        foreach (ListViewSubItem itemin in itm.SubItems)
                        {
                            itemin.ForeColor = Color.Red;
                        }

                    }
                    else if (record.IsGood == true)
                    {
                        itm.ForeColor = Color.Green;
                        foreach (ListViewSubItem itemin in itm.SubItems)
                        {
                            itemin.ForeColor = Color.Green;
                        }
                    }

                    listView.Items.Add(itm);
                    itm.Tag = record;
                }
            }
        }

        //抓資料
        private void LoadingData()
        {
            if (!_worker.IsBusy)
            {
                _listBehaviorReocrd.Clear(); ;
                _RunningID = PrimaryKey;
                _worker.RunWorkerAsync(_RunningID);
                Loading = true;
            }
        }

        //載入資料
        private void GetStudentBehavior()
        {
            _listBehaviorReocrd.Clear();
            string queryByStudent = @"
SELECT 
	$esl.behavior_data.ref_student_id
	, $esl.behavior_data.uid
	, to_char($esl.behavior_data.create_date, 'yyyy/MM/dd') AS create_date
	, $esl.behavior_data.comment
	, $esl.behavior_data.is_good_behavior
	, $esl.behavior_data.detention
	, teacher.teacher_name
	, course.course_name
	, course.id  AS course_id
FROM  
	$esl.behavior_data
LEFT JOIN
	course
		ON course.id =	$esl.behavior_data.ref_course_id
LEFT JOIN
	teacher
		ON teacher.id =  $esl.behavior_data.ref_teacher_id
WHERE ref_student_id = {0}
ORDER BY create_date DESC

";

            queryByStudent = String.Format(queryByStudent, _RunningID);

            DataTable behaviorRecord = qp.Select(queryByStudent);
            foreach (DataRow row in behaviorRecord.Rows)
            {
                BehaviorRecord record = new BehaviorRecord();
                record.StudentID = row.Field<string>("ref_student_id");
                record.UID = row.Field<string>("uid");
                record.Date = row.Field<string>("create_date");
                record.Comment = row.Field<string>("comment");
                record.Teacher = row.Field<string>("teacher_name");
                record.Course = row.Field<string>("course_id");
                record.Course = row.Field<string>("course_name");
                record.IsGood = row.Field<string>("is_good_behavior") == "true" ? true : false;
                record.IsDentetion = row.Field<string>("detention") == "true" ? true : false;
                _listBehaviorReocrd.Add(record);
            }
        }


        //新增
        private void btnInsert_Click(object sender, EventArgs e)
        {

            BehaviorForm editor = new BehaviorForm(_RunningID, _Editable);

            if (editor.ShowDialog() == DialogResult.OK)
            {
                LoadingData();
            }
        }

        //修改
        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                BehaviorForm editor = new BehaviorForm(listView.SelectedItems[0].Tag as BehaviorRecord, _Editable);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadingData();
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {


            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("請先選擇一筆您要修改的資料");
                return;
            }
            else if (listView.SelectedItems.Count > 1)
            {
                MsgBox.Show("選擇資料筆數過多，一次只能修改一筆資料");
                return;
            }

            if (listView.SelectedItems.Count == 1)
            {
                BehaviorForm editor = new BehaviorForm(listView.SelectedItems[0].Tag as BehaviorRecord, _Editable);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    LoadingData();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            if (listView.SelectedItems.Count == 0)
            {
                MsgBox.Show("必須選擇一筆以上資料!!");
                return;
            }

            List<BehaviorRecord> behaviorList = new List<BehaviorRecord>();
            List<string> listSelectedStuID = new List<string>();
            foreach (ListViewItem item in listView.SelectedItems)
            {
                BehaviorRecord editor = item.Tag as BehaviorRecord;
                behaviorList.Add(editor);
                listSelectedStuID.Add(editor.UID);
            }

            //如果
            if (MsgBox.Show($"確認刪除所選擇-[行為表現記錄]?", "確認", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                string deleteSql = $"DELETE FROM  $esl.behavior_data  WHERE uid  IN ( {String.Join(",", listSelectedStuID)} ) RETURNING *";
                qp.Select(deleteSql);

            }
            catch (Exception ex)
            {
                MsgBox.Show("刪除「服務學習記錄」資料失敗" + ex.Message);
                return;
            }
            LoadingData();


            //LOG (刪除)
            StringBuilder sb = new StringBuilder();
            StudentRecord sr = K12.Data.Student.SelectByID(this.PrimaryKey);
            sb.Append("班級「" + (string.IsNullOrEmpty(sr.RefClassID) ? "" : sr.Class.Name) + "」");
            sb.Append("座號「" + (sr.SeatNo.HasValue ? sr.SeatNo.Value.ToString() : "") + "」");
            sb.AppendLine("學生「" + sr.Name + "」");
            foreach (BehaviorRecord behavior in behaviorList)
            {
                sb.AppendLine("日期「" + behavior.Date + "」，生活行為紀錄已被刪除");
            }
            ApplicationLog.Log("生活行為紀錄", "刪除生活行為紀錄", "student", this.PrimaryKey, sb.ToString());

        }
    }
}
