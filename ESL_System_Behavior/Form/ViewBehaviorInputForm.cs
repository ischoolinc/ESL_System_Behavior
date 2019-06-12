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
        // 用來記錄 欄位原本欄位 之資料 以便畫面上現示差異
        private Dictionary<string, BehaviorRecord> _DicOriBehaviorRecord = new Dictionary<string, BehaviorRecord>();

        //updateTarget 存放更新筆數 以便控式能否點選儲存按鈕
        private List<string> _ListUpdateTarget = new List<string>();

        public ViewBehaviorInputForm()
        {
            InitializeComponent();
        }

        private void BehaviorCommentSettingForm_Load(object sender, EventArgs e)
        {

            //恩正討論先將學年度學期拿掉
            //comboBoxEx1.Items.Add("" + (int.Parse(K12.Data.School.DefaultSchoolYear) - 1));
            //comboBoxEx1.Items.Add(K12.Data.School.DefaultSchoolYear);
            //comboBoxEx1.Items.Add("" + (int.Parse(K12.Data.School.DefaultSchoolYear) + 1));
            //comboBoxEx1.Text = K12.Data.School.DefaultSchoolYear;

            //comboBoxEx2.Items.Add("1");
            //comboBoxEx2.Items.Add("2");
            //comboBoxEx2.Text = K12.Data.School.DefaultSemester;

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
            LoadingView();

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
                if (_DicOriBehaviorRecord.ContainsKey("" + row.Tag) && !row.IsNewRow)
                {
                    //如果有變更
                    if (_DicOriBehaviorRecord["" + row.Tag].Comment != "" + row.Cells[8].Value || _DicOriBehaviorRecord["" + row.Tag].IsGood.ToString() != "" + row.Cells[9].Value || _DicOriBehaviorRecord["" + row.Tag].IsDentetion.ToString() != "" + row.Cells[10].Value)
                    {
                        if ((Boolean)row.Cells[9].Value == true && (Boolean)row.Cells[10].Value == true)
                        {
                            MsgBox.Show("Good 和 Detention 不可同時勾選");
                            this.buttonX4.Enabled = true;
                            return;
                        }
                        updateLogList.Add(@"已將時間:「" + row.Cells[7].Value + "」" +
                        ",課程:「" + row.Cells[5].Value + "」" +
                        ",教師:「" + row.Cells[6].Value + "」" +
                        "給學生:「" + row.Cells[3].Value + "」的 \n"+"Comment: 由「" +
                        _DicOriBehaviorRecord["" + row.Tag].Comment + "」修改為:「" + row.Cells[8].Value + "」 \n" +
                        "Good: 由「" + _DicOriBehaviorRecord["" + row.Tag].IsGood + "」修改為:「" + row.Cells[9].Value + "」 \n" +
                        "Detention: 由「" + _DicOriBehaviorRecord["" + row.Tag].IsDentetion + "」修改為:「" + row.Cells[10].Value + "」 \n"+
                        "------------------------------------------------------------------------------- \n");

                        string updatedata = string.Format(@"
        SELECT
            {0} :: BIGINT AS uid
            ,'{1}':: TEXT AS comment
            , {2} ::BOOLEAN AS is_good_behavior
            , {3} ::BOOLEAN AS detention 
             ,'UPDATE' :: TEXT AS action
                  ", row.Tag, row.Cells[8].Value, row.Cells[9].Value, row.Cells[10].Value);

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
        , is_good_behavior = raw_data.is_good_behavior
        , detention = raw_data.detention
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
, '檢視教師Behavior輸入。修改:{2} ' 
)", _actor, _client_info, nowComment);


            UpdateHelper uh = new UpdateHelper();

            //執行sql
            uh.Execute(sql);

            MsgBox.Show("儲存設定成功。");
            LoadingView();
            this.buttonX4.Enabled = false;

        }


        private void dataGridViewX1_Enter(object sender, EventArgs e)
        {
            this.ImeMode = ImeMode.Close;
            this.ImeModeBase = ImeMode.Off;
        }

        private void dataGridViewX1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                CheckIsRowUpated();
            }
        }



        //針對 comment 欄位修改
        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CheckIsRowUpated();
        }

        /// <summary>
        /// load畫面
        /// </summary>
        private void LoadingView()
        {
            _DicOriBehaviorRecord.Clear();
            dataGridViewX1.Rows.Clear();
            _ListUpdateTarget.Clear();

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
    , $esl.behavior_data.is_good_behavior
    , $esl.behavior_data.detention
FROM  
    $esl.behavior_data
	LEFT JOIN course ON $esl.behavior_data.ref_course_id = course.id
	LEFT JOIN teacher ON $esl.behavior_data.ref_teacher_id = teacher.id
	LEFT JOIN student ON $esl.behavior_data.ref_student_id = student.id
	LEFT JOIN class ON student.ref_class_id = class.id
WHERE 
 
    create_date::DATE >= '{0}'::DATE
    AND create_date::DATE <= '{1}'::DATE
ORDER BY 
    class.grade_year
    , class.display_order
    , class.class_name
    , student.seat_no
    , student.id
    , course_name
    , class_name
    , create_date
"

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

                    //將資料庫撈回來資料 塞到_DicOriBehaviorRecord<behaviorRecord> 以便核對是否修改
                    row.Tag = dr["uid"];
                    BehaviorRecord behaviorRecord = new BehaviorRecord();
                    behaviorRecord.UID = "" + dr["uid"];
                    behaviorRecord.Comment = "" + dr["comment"];
                    behaviorRecord.IsGood = "" + dr["is_good_behavior"].ToString() == "true";
                    behaviorRecord.IsDentetion = dr["detention"].ToString() == "true";


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
                    row.Cells[i++].Value = dr["is_good_behavior"].ToString() == "true";
                    row.Cells[i++].Value = dr["detention"].ToString() == "true";
                    row.Cells[i - 1].ToolTipText = "" + dr["comment"];

                    dataGridViewX1.Rows.Add(row);


                    // 建立原本資訊的字典，作為對照用
                    //  oriCommentDict.Add("" + dr["uid"], "" + dr["comment"]);
                    _DicOriBehaviorRecord.Add("" + dr["uid"], behaviorRecord);
                }
            }
        }


        /// <summary>
        /// 確認當列資料是否修改 有修改就 改變cell背景顏色  並將 列欄位加入List
        /// </summary>
        private void CheckIsRowUpated()
        {
            DataGridViewRow row = this.dataGridViewX1.CurrentRow;//去得當前的Row
            string rowID = dataGridViewX1.CurrentRow.Tag.ToString();//取的當前的row id 

            if (dataGridViewX1.CurrentCell.Value == null) //如果為空值 跳出提醒
            {
                dataGridViewX1.CurrentCell.ErrorText = "不可為空白";
               
                return;
            }
            else
            {
                if (String.IsNullOrWhiteSpace("" + dataGridViewX1.CurrentCell.Value))
                {
                    dataGridViewX1.CurrentCell.ErrorText = "不可為空白";
                  
                    return;
                }

                dataGridViewX1.CurrentCell.ErrorText = null;
           
            }
            //如果修改為欄為 8( comment)
            if (dataGridViewX1.CurrentCell.ColumnIndex == 8)
            {
                if (dataGridViewX1.CurrentCell.Value.ToString() != this._DicOriBehaviorRecord["" + row.Tag].Comment)
                {
                    dataGridViewX1.CurrentCell.Style.BackColor = Color.LightPink;
                }
                else
                {
                    dataGridViewX1.CurrentCell.Style.BackColor = Color.White;
                }

            } //如果修改欄為 9 ( Good )
            else if (dataGridViewX1.CurrentCell.ColumnIndex == 9)
            {
                if ((Boolean)dataGridViewX1.CurrentCell.Value != this._DicOriBehaviorRecord["" + row.Tag].IsGood)
                {
                    dataGridViewX1.CurrentCell.Style.BackColor = Color.LightPink;
                }
                else
                {
                    dataGridViewX1.CurrentCell.Style.BackColor = Color.White;
                }

            } //如果修改欄為 10 (Detention)
            else if (dataGridViewX1.CurrentCell.ColumnIndex == 10)
            {
                if ((Boolean)dataGridViewX1.CurrentCell.Value != this._DicOriBehaviorRecord["" + row.Tag].IsDentetion)
                {
                    dataGridViewX1.CurrentCell.Style.BackColor = Color.LightPink;
                }
                else
                {
                    dataGridViewX1.CurrentCell.Style.BackColor = Color.White;
                }

            }

            //加入有修改列數
            if ("" + row.Cells[8].Value != _DicOriBehaviorRecord["" + row.Tag].Comment || (Boolean)row.Cells[9].Value != _DicOriBehaviorRecord["" + row.Tag].IsGood || (Boolean)row.Cells[10].Value != _DicOriBehaviorRecord["" + row.Tag].IsDentetion)
            {
                if (!_ListUpdateTarget.Contains(rowID))
                     _ListUpdateTarget.Add(rowID);
            }
            else
            {
                if (_ListUpdateTarget.Contains(rowID))
                    _ListUpdateTarget.Remove(rowID);
            }
            if (this._ListUpdateTarget.Count() > 0)
                this.buttonX4.Enabled = true;
            else
            {

                this.buttonX4.Enabled = false;
            }

        }
    }
}


