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

namespace ESL_System_Behavior.Form
{
    public partial class BehaviorCommentSettingForm : BaseForm
    {
        // 用來記錄 原本的 評語Dict ，<uid,comment>
        private Dictionary<string, string> oriCommentDict = new Dictionary<string, string>();

        // 用來記錄 現在的評語的清單
        private List<string> nowCommentList = new List<string>();


        public BehaviorCommentSettingForm()
        {
            InitializeComponent();
        }

        private void BehaviorCommentSettingForm_Load(object sender, EventArgs e)
        {
            // 2018/06/27 穎驊備註 在table $esl.behavior_comment_template，  ref_teacher_id 為空、is_default =true 代表其為後端系統預設的Comment
            string query = "select * from $esl.behavior_comment_template where ref_teacher_id is null and is_default ='true'";

            QueryHelper qh = new QueryHelper();
            DataTable dt = qh.Select(query);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataGridViewRow row = new DataGridViewRow();

                    row.CreateCells(dataGridViewX1);

                    row.Tag = dr["uid"];
                    row.Cells[0].Value = dr["comment"];

                    oriCommentDict.Add("" + dr["uid"], "" + dr["comment"]);

                    dataGridViewX1.Rows.Add(row);
                }
            }
        }


        // 儲存
        private void buttonX1_Click(object sender, EventArgs e)
        {

            string sql = "";

            // 兜資料
            List<string> insertdataList = new List<string>(); // 新增的資料
            List<string> updatedataList = new List<string>(); // 更新的資料
            List<string> deletedataList = new List<string>(); // 刪除的資料

            //檢視 目前介面 dataGridViewX1上有的項目 
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                //沒有Tag 的東西 就是本次使用者自己加的，為insert 內容。
                if ("" + row.Tag == "" && !row.IsNewRow)
                {
                    string insertdata = string.Format(@"
                    SELECT
                    true::BOOLEAN AS is_default                    
                    ,'{0}':: TEXT AS comment   
                  ", row.Cells[0].Value);
                    insertdataList.Add(insertdata);
                }

                // 存放於原本字典有的東西，但是後來內容改變， 為update 內容。
                if (oriCommentDict.ContainsKey("" + row.Tag) && !row.IsNewRow)
                {
                    if (oriCommentDict["" + row.Tag] != "" + row.Cells[0].Value)
                    {
                        string updatedata = string.Format(@"
                    SELECT
                    {0} :: BIGINT AS uid
                    ,true::BOOLEAN AS is_default                    
                    ,'{1}':: TEXT AS comment   
                  ", row.Tag, row.Cells[0].Value);

                        updatedataList.Add(updatedata);
                    }
                }

                if (!nowCommentList.Contains(""+row.Cells[0].Value) && !row.IsNewRow)
                {
                    nowCommentList.Add(""+row.Cells[0].Value);
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
                  ", key);

                    deletedataList.Add(deletedata);

                }
            }

            sql = "WITH ";




            if (insertdataList.Count > 0)
            {


                string insertData = string.Join(" UNION ALL", insertdataList);

                sql += string.Format(@"insert_data_row AS( 
            			 {0}),insert_data AS
(
            -- 新增 
            INSERT INTO $esl.behavior_comment_template(
            	is_default
            	, comment	
            )
            SELECT 
            	insert_data_row.is_default::BOOLEAN AS is_default
                ,insert_data_row.comment::TEXT AS comment	
            FROM
            	insert_data_row            
            RETURNING  $esl.behavior_comment_template.* 
)
            ", insertData);



            }

            if (updatedataList.Count > 0)
            {
                if (insertdataList.Count > 0) //前項有東西，加逗號
                {
                    sql += ",";
                }

                string updateData = string.Join(" UNION ALL", updatedataList);

                sql += string.Format(@"update_data_row AS( 
                			 {0}),update_data AS(
                -- 更新
                Update $esl.behavior_comment_template
                SET
                comment = update_data_row.comment
                FROM update_data_row
                WHERE $esl.behavior_comment_template.uid = update_data_row.uid	
                RETURNING  $esl.behavior_comment_template.* 
)
                                ", updateData);


            }

            if (deletedataList.Count > 0)
            {
                if (insertdataList.Count > 0 | updatedataList.Count > 0) //前項有東西，加逗號
                {
                    sql += ",";
                }

                string deleteData = string.Join(" UNION ALL", deletedataList);


                sql += string.Format(@"delete_data_row AS( 
                			 {0}), delete_data AS(
                -- 刪除
                DELETE 
                FROM $esl.behavior_comment_template
                WHERE $esl.behavior_comment_template.uid IN (
                		SELECT delete_data_row.uid
                		FROM delete_data_row
                		) 
)
                                ", deleteData);


                if (insertdataList.Count > 0 && updatedataList.Count ==0) //若 insert 與 delete 單獨存在會逾時爆掉，需要加這個，明確規範順序
                {
                    sql += @",test_data1 AS(
                    SELECT * FROM delete_data_row
                    LEFT JOIN insert_data ON delete_data_row.uid = NULL
                  ) ";
                }

                if (insertdataList.Count ==0 && updatedataList.Count > 0) //若 update 與 delete 單獨存在會逾時爆掉，需要加這個，明確規範順序
                {
                    sql += @",test_data2 AS(
                    SELECT * FROM delete_data_row
                    LEFT JOIN update_data ON delete_data_row.uid = NULL
                  ) ";
                }


            }

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
, '管理課堂表現內容清單' 
	, 'course'
	, '' 
	, now() 
, '{1}' 
	, '設定'
, '管理課堂表現內容清單更正。目前內容:{2}' 
)", _actor, _client_info, nowComment);


            UpdateHelper uh = new UpdateHelper();

            //執行sql
            uh.Execute(sql);

            MsgBox.Show("儲存設定成功。");

        }

        // 取消
        private void buttonX2_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        
    }
}


