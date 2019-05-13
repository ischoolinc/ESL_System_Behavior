﻿using FISCA.Data;
using FISCA.LogAgent;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESL_System_Behavior.Form
{
    public partial class BehaviorForm : BaseForm
    {
        private StudentRecord _Student;
        private string _SchoolYear;
        private string _SchoolSemester;
        private BackgroundWorker BGW = new BackgroundWorker();
        private BehaviorRecord _BehaviorRecord;
        private QueryHelper _Qh;
        private Dictionary<string, string> _DicCourseInfo;
        private String _Action = "";
        private BehaviorRecord _BefaoreUpdate; // LOG用 
        private bool _Editable; 


        public BehaviorForm()
        {
            InitializeComponent();

        }
        //新增
        public BehaviorForm(string studentID ,bool editable)
        {
            InitializeComponent();
            this._Editable = editable;
            //權限設定
            SetPerm();
            this._Action = "新增";
            this._BehaviorRecord = new BehaviorRecord();
            this._BehaviorRecord.StudentID = studentID;

        }

        //修改
        public BehaviorForm(BehaviorRecord behavior, bool editable)
        {
            InitializeComponent();
            this._Editable = editable;
            //權限設定
            SetPerm();
            this._Action = "修改";
            this._BehaviorRecord = behavior;
            this._BefaoreUpdate = behavior.ShallowCopy();
            this.comboBoxCourse.Enabled = false;

        }


        private void BehaviorForm_Load(object sender, System.EventArgs e)
        {

            _Student = Student.SelectByID(this._BehaviorRecord.StudentID);
            //載入BGW
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            _Qh = new QueryHelper();

            _DicCourseInfo = new Dictionary<string, string>(); //課程資訊使用

            textBoxTeacher.Enabled = false;  //設定老師不能變更
            this.dateTimeCreateDate.Value = DateTime.Now;
            this.dateTimeCreateDate.Enabled = false;

            //載入學年度學期 為了select 課程資訊
            _SchoolYear = School.DefaultSchoolYear;
            _SchoolSemester = School.DefaultSemester;

            //開始跑BGW
            BGW.RunWorkerAsync();
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            String queryCourse = @"
SELECT
	sc_attend.ref_course_id 
	 , course.course_name
	 ,  ref_student_id
FROM
	sc_attend 
LEFT JOIN
  course
  ON course.id = sc_attend.ref_course_id
WHERE 
 	ref_student_id = {0}
	AND course.school_year = {1}
	AND course.semester = {2}
";

            queryCourse = String.Format(queryCourse, _BehaviorRecord.StudentID, this._SchoolYear, this._SchoolSemester);

            //取得課程
            DataTable courses = _Qh.Select(queryCourse);
            foreach (DataRow courseInfo in courses.Rows)
            {
                _DicCourseInfo.Add(courseInfo.Field<string>("course_name"), courseInfo.Field<string>("ref_course_id"));
            }
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this._Action == "新增")
            {
                dateTimeCreateDate.Value = DateTime.Now;
                //載入課程
                comboBoxCourse.Items.Add("");
                foreach (string courseId in _DicCourseInfo.Keys)
                {
                    if (!String.IsNullOrEmpty(courseId))
                    {
                        comboBoxCourse.Items.Add(courseId);
                    }
                }
                //加入原本課程
                if (!comboBoxCourse.Items.Contains(this._BehaviorRecord.Course != null ? this._BehaviorRecord.Course : ""))
                {
                    comboBoxCourse.Items.Add(this._BehaviorRecord.Course);
                }
                comboBoxCourse.SelectedItem = this._BehaviorRecord.Course;//顯示遠本課程
            }

            else if (this._Action == "修改")
            {
                dateTimeCreateDate.Value = DateTime.Parse(_BehaviorRecord.Date);//填入日期
                textBoxTeacher.Text = _BehaviorRecord.Teacher; //填入老師
                textBoxComment.Text = _BehaviorRecord.Comment; //填入事由
                //載入課程combox
                comboBoxCourse.Items.Add("");
                foreach (string courseId in _DicCourseInfo.Keys)
                {
                    if (!String.IsNullOrEmpty(courseId))
                    {
                        comboBoxCourse.Items.Add(courseId);
                    }
                }
                //加入原本課程(因修改所以填入原本課程)
                if (!comboBoxCourse.Items.Contains(this._BehaviorRecord.Course != null ? this._BehaviorRecord.Course : ""))
                {
                    comboBoxCourse.Items.Add(this._BehaviorRecord.Course);
                }
                comboBoxCourse.SelectedItem = this._BehaviorRecord.Course;//顯示遠本課程
            }

            //填入 good or detention 
            this.chexkBoxDetention.Checked = _BehaviorRecord.IsDentetion;
            this.CheckboxＧoodBehavior.Checked = _BehaviorRecord.IsGood;
        }

        //樣板
       

        //按下儲存
        private void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            Boolean isPass = ValidateValue();

            //如果驗證沒過就出去
            if (!isPass)
            {
                btnSave.Enabled = true;
                return;
            }
            //把資料存到behavior record 物件
            FillDateToEdit();

            if (this._Action == "新增")
            {
                #region 新增
                string insertSql = @"
INSERT  INTO  
	$esl.behavior_data
		(
          last_update
		, ref_student_id
		, ref_course_id
		, create_date 
		, comment
		, is_good_behavior
		, detention
		)
     VALUES
		( 
		  now()
		, {0} 
		, {1}
		, now()
		, '{2}'
		, {3}
		, {4}  
		)  
RETURNING *

";
                insertSql = String.Format(insertSql
                                         , this._BehaviorRecord.StudentID
                                         , this._BehaviorRecord.CourseID != null ? this._BehaviorRecord.CourseID : "null"
                                         , this._BehaviorRecord.Comment
                                         , this._BehaviorRecord.IsGood
                                         , this._BehaviorRecord.IsDentetion)
                                         ;
                try
                {
                    DataTable retuen = _Qh.Select(insertSql);

                    //(新增)LOG 
                    StringBuilder sb = new StringBuilder();
                    sb.Append("班級「" + (string.IsNullOrEmpty(_Student.RefClassID) ? "" : _Student.Class.Name) + "」");
                    sb.Append("座號「" + (_Student.SeatNo.HasValue ? _Student.SeatNo.Value.ToString() : "") + "」");
                    sb.AppendLine("學生「" + DateTime.Now.Date+ "」");
                    sb.AppendLine("新增一筆生活行為記錄。");
                    sb.AppendLine("登錄日期「" + _BehaviorRecord.Date + "」");
                    sb.AppendLine("課　　程「" + (this._BehaviorRecord.Course != null && this._BehaviorRecord.Course != "" ? this._BehaviorRecord.Course : "") + "」");
                    sb.AppendLine("事　　由「" + this._BehaviorRecord.Comment + "」");
                    sb.AppendLine("Good Behavior「" + (this._BehaviorRecord.IsGood ? "是" : "否") + "」");
                    sb.AppendLine("Detention「" + (this._BehaviorRecord.IsDentetion ? "是" : "否") + "」");
                    ApplicationLog.Log("生活行為紀錄", "新增生活行為紀錄", "student", _Student.ID, sb.ToString());

                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增[行為表現紀錄]發生錯誤: \n" + ex.Message);
                    return;
                }

                MsgBox.Show("新增[行為表現紀錄]成功!");
                this.DialogResult = DialogResult.OK;
                #endregion
            }
            else if (this._Action == "修改")
            {

                #region 修改
                string updateSql = @"
UPDATE 
		$esl.behavior_data
SET 
		comment = '{2}'
		, ref_course_id = {3}
		, last_update  = now()
  	    , is_good_behavior = {4}
		, detention = {5}
        
WHERE  
       uid = {0}
	   AND  ref_student_id = {1}
RETURNING *
";
                try
                {
                    updateSql = String.Format(
                                                updateSql,
                                                this._BehaviorRecord.UID
                                                , this._BehaviorRecord.StudentID
                                                , this._BehaviorRecord.Comment
                                                , this._BehaviorRecord.Course != null && this._BehaviorRecord.Course != "" ? _DicCourseInfo[this._BehaviorRecord.Course] : "null"
                                                , this._BehaviorRecord.IsGood
                                                , this._BehaviorRecord.IsDentetion
                                                );
                    DataTable dt = _Qh.Select(updateSql);
                    string createDate = dt.Rows[0].Field<string>("create_date");

                    //(修改)LOG 
                    StringBuilder sb = new StringBuilder();
                    sb.Append("班級「" + (string.IsNullOrEmpty(_Student.RefClassID) ? "" : _Student.Class.Name) + "」");
                    sb.Append("座號「" + (_Student.SeatNo.HasValue ? _Student.SeatNo.Value.ToString() : "") + "」");
                    sb.AppendLine("學生「" + _Student.Name + "」");
                    sb.AppendLine("修改一筆生活行為紀錄。");
                    sb.AppendLine("登錄日期「" + _BehaviorRecord.Date+ "」");
                    sb.AppendLine("事　　由「" + this._BefaoreUpdate.Comment + "」");
                    sb.AppendLine("修改為「" + this._BehaviorRecord.Comment + "」");
                    sb.AppendLine("Good Behavior「" + (this._BefaoreUpdate.IsGood ? "是" : "否") + "」修改為「" + (this._BehaviorRecord.IsGood ? "是" : "否") + "」");
                    sb.AppendLine("Detention「" + (this._BefaoreUpdate.IsDentetion ? "是" : "否") + "」修改為「" + (this._BehaviorRecord.IsDentetion ? "是" : "否") + "」");
                    ApplicationLog.Log("生活行為紀錄", "修改生活行為紀錄", "student", _Student.ID, sb.ToString());
                }
                catch (Exception ex)
                {
                    MsgBox.Show("修改[行為表現紀錄]發生錯誤: \n" + ex.Message);
                    return;
                }

                MsgBox.Show("修改[行為表現紀錄]成功!");
                this.DialogResult = DialogResult.OK;
            }

            #endregion

            this.Close();
        }

        //將畫面資料填入物件
        private void FillDateToEdit()
        {
            this._BehaviorRecord.Comment = textBoxComment.Text; //回填事由
            this._BehaviorRecord.Course = comboBoxCourse.Text; //回填課程
            this._BehaviorRecord.CourseID = _DicCourseInfo.ContainsKey(comboBoxCourse.Text) ? _DicCourseInfo[comboBoxCourse.Text] : null;
            this._BehaviorRecord.IsGood = CheckboxＧoodBehavior.Checked;
            this._BehaviorRecord.IsDentetion = chexkBoxDetention.Checked;
            this._BehaviorRecord.Date = dateTimeCreateDate.Value.ToString();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private Boolean ValidateValue()
        {
            if (textBoxComment.Text.Trim() == "")
            {
                DialogResult dr = MsgBox.Show("事由未輸入，請填寫!");
                return false;
            }
            else
            {
                return true;
            }
        }
        //資料項目是否可以編輯
        private void SetPerm()
        {
            textBoxComment.Enabled = _Editable;
            chexkBoxDetention.Enabled = _Editable;  
            CheckboxＧoodBehavior.Enabled = _Editable;
            comboBoxCourse.Enabled = _Editable;
            btnSave.Enabled = _Editable;
        }
    }
}
