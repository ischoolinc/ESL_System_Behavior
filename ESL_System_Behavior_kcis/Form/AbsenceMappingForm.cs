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
using FISCA.DSAUtil;
using Framework.Feature;


namespace ESL_System_Behavior.Form
{
    public partial class AbsenceMappingForm : BaseForm
    {
        // 用來記錄 原本的 評語Dict ，<uid,comment>
        private Dictionary<string, string> oriCommentDict = new Dictionary<string, string>();

        // 用來記錄 現在的評語的清單
        private List<string> nowCommentList = new List<string>();

        public AbsenceMappingForm()
        {
            InitializeComponent();
        }

        private void BehaviorCommentSettingForm_Load(object sender, EventArgs e)
        {
            // 儲存使用者目前的設定 <中文節次名稱,英文節次名稱>
            Dictionary<string, AbsenceEng> userSettingDict = new Dictionary<string, AbsenceEng>();

            // ESL 的目前設定 作為和學務作業缺曠類別的對照
            List<string> eslSetList = new List<string>();

            string query = "SELECT * from $esl.attendance_absence";

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);

                    row.Tag = dr["uid"];
                    row.Cells[0].Value = dr["name"];
                    row.Cells[1].Value = dr["english_name"];
                    row.Cells[2].Value = dr["english_abbreviation"];


                    // 建立原本資訊的字典，作為對照用
                    
                    //修改增加防止基本錯誤
                    //2024/8/26 - by Dylan
                    if (!oriCommentDict.ContainsKey("" + dr["uid"]))
                        oriCommentDict.Add("" + dr["uid"], "" + dr["name"] + "_" + dr["english_name"] + "_" + dr["english_abbreviation"]);

                    dataGridViewX1.Rows.Add(row);

                    eslSetList.Add("" + dr["name"]);

                    //修改增加防止基本錯誤
                    //2024/8/26 - by Dylan
                    if (!userSettingDict.ContainsKey("" + dr["name"]))
                        userSettingDict.Add("" + dr["name"], new AbsenceEng() { EnglishName = "" + dr["english_name"], EnglishAbbreviation = "" + dr["english_abbreviation"] });

                }
            }
            else
            {

                //儲存 學務作業目前設定的缺曠類別，作為提醒使用者設定需與學務作業一致
                List<string> _behaviorSetList = new List<string>();

                //取得Xml結構
                DSResponse _dsrsp = Config.GetAbsenceList();
                DSXmlHelper _helper = _dsrsp.GetContent();
                foreach (XmlElement element in _helper.GetElements("Absence"))
                {

                    string name = element.GetAttribute("Name");

                    _behaviorSetList.Add(name);
                }

                foreach (string abs in _behaviorSetList)
                {
                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);

                    row.Cells[0].Value = abs;

                    dataGridViewX1.Rows.Add(row);
                }


            }

            //儲存 學務作業目前設定的缺曠類別，作為提醒使用者設定需與學務作業一致
            List<string> behaviorSetList = new List<string>();

            //取得Xml結構
            DSResponse dsrsp = Config.GetAbsenceList();
            DSXmlHelper helper = dsrsp.GetContent();
            foreach (XmlElement element in helper.GetElements("Absence"))
            {

                string name = element.GetAttribute("Name");

                behaviorSetList.Add(name);
            }

            List<string> behaviorSetList_OriOlrder = new List<string>(); // 記住原順序的List

            behaviorSetList_OriOlrder.AddRange(behaviorSetList);

            eslSetList.Sort();
            behaviorSetList.Sort();



            if (!eslSetList.SequenceEqual(behaviorSetList))
            {

                dataGridViewX1.Rows.Clear();

                foreach (string abs in behaviorSetList_OriOlrder)
                {
                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);

                    row.Cells[0].Value = abs;
                    row.Cells[1].Value = userSettingDict.ContainsKey(abs) ? userSettingDict[abs].EnglishName : "";
                    row.Cells[2].Value = userSettingDict.ContainsKey(abs) ? userSettingDict[abs].EnglishAbbreviation : "";

                    dataGridViewX1.Rows.Add(row);
                }
            }



        }


        // 儲存
        private void buttonX1_Click(object sender, EventArgs e)
        {
            List<string> checkRepeatList = new List<string>();

            //檢視 目前介面 dataGridViewX1上有的項目 
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (checkRepeatList.Contains("" + row.Cells[0].Value))
                {
                    MsgBox.Show("表單中有相同缺曠名稱，請重新設定。");

                    return;
                }

                checkRepeatList.Add("" + row.Cells[0].Value);
            }

            string sql = "";

            List<string> rawdataList = new List<string>(); // 2018/6/28 穎驊聽取恩證的建議後，決定建立一個raw_data，統一存放，這樣SQL 才好維護

            //檢視 目前介面 dataGridViewX1上有的項目 
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                //沒有Tag 的東西 就是本次使用者自己加的，為insert 內容。
                if ("" + row.Tag == "" && !row.IsNewRow)
                {
                    string insertdata = string.Format(@"
    SELECT
        NULL :: BIGINT AS uid                       
        ,'{0}':: TEXT AS name
        ,'{1}':: TEXT AS english_name
        ,'{2}':: TEXT AS english_abbreviation        
        ,'INSERT' :: TEXT AS action
                  ", row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value);
                    rawdataList.Add(insertdata);
                }

                // 存放於原本字典有的東西，但是後來內容改變， 為update 內容。
                if (oriCommentDict.ContainsKey("" + row.Tag) && !row.IsNewRow)
                {
                    if (oriCommentDict["" + row.Tag] != "" + row.Cells[0].Value + "_" + row.Cells[1].Value + "_" + row.Cells[2].Value)
                    {
                        string updatedata = string.Format(@"
    SELECT
        {0} :: BIGINT AS uid
        ,'{1}':: TEXT AS name
        ,'{2}':: TEXT AS english_name
        ,'{3}':: TEXT AS english_abbreviation        
        ,'UPDATE' :: TEXT AS action
                  ", row.Tag, row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value);

                        rawdataList.Add(updatedata);
                    }
                }

                if (!nowCommentList.Contains("(" + row.Cells[0].Value + "_" + row.Cells[1].Value + "_" + row.Cells[2].Value + ")") && !row.IsNewRow)
                {
                    nowCommentList.Add("(" + row.Cells[0].Value + "_" + row.Cells[1].Value + "_" + row.Cells[2].Value + ")");
                }

            }

            // 存放於原本字典有的東西，但在新的介面找不到， 為delete 內容。
            foreach (string key in oriCommentDict.Keys)
            {
                bool hasKey = false;

                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    if ("" + row.Tag == key)
                    {
                        hasKey = true;
                    }
                }
                if (!hasKey)
                {
                    string deletedata = string.Format(@"
    SELECT
        {0} :: BIGINT AS uid
        ,'':: TEXT AS name        
        ,'':: TEXT AS english_name
        ,'':: TEXT AS english_abbreviation        
        ,'DELETE' :: TEXT AS action
                  ", key);
                    rawdataList.Add(deletedata);

                }
            }

            // 甚麼都沒更正，也就甚麼都不做。
            if (rawdataList.Count == 0)
            {
                return;
            }

            sql = "WITH ";


            string rawData = string.Join(" UNION ALL", rawdataList);

            sql += string.Format(@"raw_data AS( {0} )", rawData);

            sql += @"
,insert_data AS
(   -- 新增 
    INSERT INTO $esl.attendance_absence(
        name        
        ,english_name
        ,english_abbreviation        
    )
    SELECT         
        raw_data.name::TEXT AS name           
        ,raw_data.english_name::TEXT AS english_name   
        ,raw_data.english_abbreviation::TEXT AS english_abbreviation           
    FROM
        raw_data
    WHERE raw_data.action ='INSERT'                    
    RETURNING  $esl.attendance_absence.* 
)
,update_data AS(
    -- 更新
    Update $esl.attendance_absence
    SET
        name = raw_data.name        
        ,english_name = raw_data.english_name
        ,english_abbreviation = raw_data.english_abbreviation        
    FROM 
        raw_data    
    WHERE $esl.attendance_absence.uid = raw_data.uid  
        AND raw_data.action ='UPDATE'
    RETURNING  $esl.attendance_absence.* 
)
,delete_data AS(
    -- 刪除
    DELETE 
    FROM $esl.attendance_absence    
    WHERE $esl.attendance_absence.uid IN (
        SELECT raw_data.uid
        FROM raw_data        
        WHERE raw_data.action ='DELETE'
     )                
)

  ";

            // test_data1、test_data2 為避免 delete 與 insert|| update 配對 造成逾時

            string _actor = DSAServices.UserAccount; ;
            string _client_info = ClientInfo.GetCurrentClientInfo().OutputResult().OuterXml;

            string nowComment = string.Join("、", nowCommentList);

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
, '管理設定缺曠對照' 
	, 'course'
	, '' 
	, now() 
, '{1}' 
	, '設定'
, '管理設定缺曠對照更正。目前內容:{2}' 
)", _actor, _client_info, nowComment);


            UpdateHelper uh = new UpdateHelper();

            //執行sql
            uh.Execute(sql);

            MsgBox.Show("儲存設定成功。");

            // 儲存成功 關閉視窗
            this.Close();

        }

        // 取消
        private void buttonX2_Click(object sender, EventArgs e)
        {

            this.Close();
        }




    }

    public class AbsenceEng
    {
        public string EnglishName;
        public string EnglishAbbreviation;

    }
}


