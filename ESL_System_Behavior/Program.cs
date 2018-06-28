using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FISCA;
using FISCA.Presentation;
using K12.Presentation;
using FISCA.Permission;


namespace ESL_System_Behavior
{
    public class Program
    {
        //2018/4/11 穎驊因應康橋英文系統、弘文ESL 專案 ，開始建構教務作業ESL 評分樣版設定
        [FISCA.MainMethod()]
        public static void Main()
        {
            

            Catalog ribbon4 = RoleAclSource.Instance["教務作業"]["功能按鈕"];
            ribbon4.Add(new RibbonFeature("ESL日常生活表現", "管理課堂表現內容清單"));

            //MotherForm.RibbonBarItems["課程", "ESL課程"]["管理課堂表現內容清單"].Enable = UserAcl.Current["管理課堂表現內容清單"].Executable;
            MotherForm.RibbonBarItems["課程", "ESL課程"]["管理課堂表現內容清單"].Enable = true;

            MotherForm.RibbonBarItems["課程", "ESL課程"]["管理課堂表現內容清單"].Click += delegate
            {
                Form.BehaviorCommentSettingForm bcsf = new Form.BehaviorCommentSettingForm();

                bcsf.Show();
            };
        }
    }
}
