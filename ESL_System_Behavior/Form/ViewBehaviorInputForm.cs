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
using Aspose.Cells;

namespace ESL_System_Behavior.Form
{
    public partial class ViewBehaviorInputForm : BaseForm
    {

        // 用來記錄 原本的 評語Dict ，<uid,comment>
        private Dictionary<string, string> oriCommentDict = new Dictionary<string, string>();

        public ViewBehaviorInputForm()
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

            this.dateTimeInput1.Value = DateTime.Now.Date;
            this.dateTimeInput2.Value = DateTime.Now.Date;
        }


        // 查詢
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

            oriCommentDict.Clear();
            dataGridViewX1.Rows.Clear();

            string query = string.Format(@"
SELECT  
    $esl.behavior_data.uid
    , course.course_name
    , teacher.teacher_name
    , class.class_name
    , student.seat_no
    , student.student_number
    , student.name AS student_name
    , student.english_name
    , $esl.behavior_data.comment
    , $esl.behavior_data.create_date 
FROM  
    $esl.behavior_data
	LEFT JOIN course ON $esl.behavior_data.ref_course_id = course.id
	LEFT JOIN teacher ON $esl.behavior_data.ref_teacher_id = teacher.id
	LEFT JOIN student ON $esl.behavior_data.ref_student_id = student.id
	LEFT JOIN class ON student.ref_class_id = class.id
WHERE 
    course.school_year = '{0}' 
    AND semester = '{1}' 
    AND create_date::DATE >= '{2}'::DATE
    AND create_date::DATE <= '{3}'::DATE
ORDER BY 
    class.grade_year
    , class.display_order
    , class.class_name
    , student.seat_no
    , student.id
    , course_name
    , class_name
"
        , comboBoxEx1.Text
        , comboBoxEx2.Text
        , dateTimeInput1.Value.Date.ToShortDateString()
        , dateTimeInput2.Value.Date.ToShortDateString()
    );

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);

                    row.Tag = dr["uid"];
                    int i = 0;
                    row.Cells[i++].Value = "" + dr["class_name"];
                    row.Cells[i++].Value = "" + dr["seat_no"];
                    row.Cells[i++].Value = "" + dr["student_number"];
                    row.Cells[i++].Value = "" + dr["student_name"];
                    row.Cells[i++].Value = "" + dr["english_name"];
                    row.Cells[i++].Value = "" + dr["course_name"];
                    row.Cells[i++].Value = "" + dr["teacher_name"];
                    row.Cells[i++].Value = DateTime.Parse("" + dr["create_date"]).ToShortDateString(); ;
                    row.Cells[i++].Value = "" + dr["comment"];
                    row.Cells[i - 1].ToolTipText = "" + dr["comment"];

                    dataGridViewX1.Rows.Add(row);


                    // 建立原本資訊的字典，作為對照用
                    oriCommentDict.Add("" + dr["uid"], "" + dr["comment"]);

                }
            }


        }
        // 匯出畫面 dgv
        private void buttonX3_Click(object sender, EventArgs e)
        {
            Workbook book = new Workbook();
            book.Worksheets.Clear();
            Worksheet ws = book.Worksheets[book.Worksheets.Add()];
            ws.Name = "Behavior紀錄";

            int index = 0;
            Dictionary<string, int> map = new Dictionary<string, int>();

            #region 建立標題
            for (int i = 0; i < dataGridViewX1.Columns.Count; i++)
            {
                DataGridViewColumn col = dataGridViewX1.Columns[i];
                ws.Cells[index, i].PutValue(col.HeaderText);
                map.Add(col.HeaderText, i);
            }
            index++;
            #endregion

            #region 填入內容
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    int column = map[cell.OwningColumn.HeaderText];
                    ws.Cells[index, column].PutValue("" + cell.Value);
                }
                index++;
            }
            #endregion
            ws.AutoFitColumns();

            SaveFileDialog sd = new SaveFileDialog();
            sd.FileName = "Behavior紀錄";
            sd.Filter = "Excel檔案(*.xlsx)|*.xlsx";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                DialogResult result = new DialogResult();

                try
                {
                    book.Save(sd.FileName, SaveFormat.Xlsx);
                    result = MsgBox.Show("檔案儲存完成，是否開啟檔案?", "是否開啟", MessageBoxButtons.YesNo);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("儲存失敗。" + ex.Message);
                }

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(sd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 儲存目前介面的更動
        private void buttonX4_Click(object sender, EventArgs e)
        {
            string sql = "";

            List<string> updateLogList = new List<string>();

            List<string> rawdataList = new List<string>();

            //檢視 目前介面 dataGridViewX1上有的項目 
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                // 存放於原本字典有的東西，但是後來內容改變， 為update 內容。
                if (oriCommentDict.ContainsKey("" + row.Tag) && !row.IsNewRow)
                {
                    if (oriCommentDict["" + row.Tag] != "" + row.Cells[8].Value)
                    {
                        updateLogList.Add(@"已將時間:「" + row.Cells[7].Value + "」" +
                            ",課程:「" + row.Cells[5].Value + "」" +
                            ",教師:「" + row.Cells[6].Value + "」" +
                            "給學生:「" + row.Cells[3].Value + "」的Comment由「" +
                            oriCommentDict["" + row.Tag] + "」修改為:「" + row.Cells[8].Value + "」");

                        string updatedata = string.Format(@"
        SELECT
            {0} :: BIGINT AS uid
            ,'{1}':: TEXT AS comment        
             ,'UPDATE' :: TEXT AS action
                  ", row.Tag, row.Cells[8].Value);

                        rawdataList.Add(updatedata);
                    }
                }
            }

            sql = "WITH ";

            string rawData = string.Join(" UNION ALL", rawdataList);

            sql += string.Format(@"raw_data AS( {0} )", rawData);

            sql += @"
,update_data AS(
    -- 更新
    Update $esl.behavior_data
    SET
        comment = raw_data.comment       
    FROM 
        raw_data    
    WHERE $esl.behavior_data.uid = raw_data.uid  
        AND raw_data.action ='UPDATE'
    RETURNING  $esl.behavior_data.* 
)


  ";
            // test_data1、test_data2 為避免 delete 與 insert|| update 配對 造成逾時

            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            string nowComment = string.Join("、", updateLogList);

            sql += string.Format(@" INSERT INTO log(
	actor
	, action_type
	, action
	, target_category
	, target_id
	, server_time
	, client_info
	, action_by
	, description
)
VALUES ('{0}'::TEXT 
, 'Record' 
, 'ESL' 
	, 'course'
	, '' 
	, now() 
, '{1}' 
	, '設定'
, '檢視教師Behavior輸入。修正:{2}' 
)", _actor, _client_info, nowComment);


            UpdateHelper uh = new UpdateHelper();

            //執行sql
            uh.Execute(sql);

            MsgBox.Show("儲存設定成功。");

        }

        private void dataGridViewX1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dataGridViewX1.EndEdit();
            dataGridViewX1.BeginEdit(false);
            bool hasChange = false;
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if((""+row.Cells[8].Value) != oriCommentDict["" + row.Tag])
                {
                    hasChange = true;
                    row.Cells[8].Style.BackColor = Color.LightPink;
                }
                else
                {
                    row.Cells[8].Style.BackColor = Color.White;
                }
            }
            this.buttonX4.Enabled = hasChange;
        }

        private void dataGridViewX1_Enter(object sender, EventArgs e)
        {
            this.ImeMode = ImeMode.Close;
            this.ImeModeBase = ImeMode.Off;
        }
    }
}


