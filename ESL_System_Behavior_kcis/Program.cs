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

            #region 設定Behavior General Comment清單
            {
                // MotherForm.RibbonBarItems["學務作業", "批次作業/查詢"]["設定"].Image = Properties.Resources.blackboard_config_64;

                Catalog ribbon = RoleAclSource.Instance["學務作業"]["基本設定"];
                ribbon.Add(new RibbonFeature("AE1E3C3A - 5F63 - 4776 - 8ECB - 5CB76EDCE74F", "設定Behavior General Comment清單"));

                MotherForm.RibbonBarItems["學務作業", "基本設定"]["設定"]["設定Behavior General Comment清單"].Enable = UserAcl.Current["AE1E3C3A - 5F63 - 4776 - 8ECB - 5CB76EDCE74F"].Executable;

                MotherForm.RibbonBarItems["學務作業", "基本設定"]["設定"]["設定Behavior General Comment清單"].Click += delegate
                {
                    Form.BehaviorCommentSettingForm bcsf = new Form.BehaviorCommentSettingForm();

                    bcsf.ShowDialog();
                };
            }
            #endregion
            {
                Catalog ribbon = RoleAclSource.Instance["課程"]["ESL課程"];
                ribbon.Add(new RibbonFeature("84615CE1 - E1B7 - 43A3 - AF91 - F33B95F0C960", "設定Attendance假別對照"));

                MotherForm.RibbonBarItems["課程", "ESL課程"]["設定"]["設定Attendance假別對照"].Enable = UserAcl.Current["84615CE1 - E1B7 - 43A3 - AF91 - F33B95F0C960"].Executable;

                MotherForm.RibbonBarItems["課程", "ESL課程"]["設定"]["設定Attendance假別對照"].Click += delegate
                {
                    Form.AbsenceMappingForm amf = new Form.AbsenceMappingForm();

                    amf.ShowDialog();
                };
            }
            {
                Catalog ribbon = RoleAclSource.Instance["課程"]["ESL課程"];
                ribbon.Add(new RibbonFeature("A052015A - B865 - 49E6 - 861D - 7B3B25B9BB63", "設定Attendance節次對照"));

                MotherForm.RibbonBarItems["課程", "ESL課程"]["設定"]["設定Attendance節次對照"].Enable = UserAcl.Current["A052015A - B865 - 49E6 - 861D - 7B3B25B9BB63"].Executable;


                MotherForm.RibbonBarItems["課程", "ESL課程"]["設定"]["設定Attendance節次對照"].Click += delegate
                {
                    Form.AbsencePeriodMappingForm apmf = new Form.AbsencePeriodMappingForm();

                    apmf.ShowDialog();
                };
            }
            {
                Catalog ribbon = RoleAclSource.Instance["課程"]["ESL課程"];
                ribbon.Add(new RibbonFeature("3ECAD583 - F8D8 - 4DFA - 8299 - 312F603A6F0F", "檢視 "+ NameCheck.ReportName));

                MotherForm.RibbonBarItems["課程", "ESL課程"]["檢視 " + NameCheck.ReportName].Enable = UserAcl.Current["3ECAD583 - F8D8 - 4DFA - 8299 - 312F603A6F0F"].Executable;
                MotherForm.RibbonBarItems["課程", "ESL課程"]["檢視 " + NameCheck.ReportName].Size = RibbonBarButton.MenuButtonSize.Medium;
                MotherForm.RibbonBarItems["課程", "ESL課程"]["檢視 " + NameCheck.ReportName].Image = Properties.Resources.admissions_zoom_64;

                MotherForm.RibbonBarItems["課程", "ESL課程"]["檢視 " + NameCheck.ReportName].Click += delegate
                {
                    // 2019/09/12 穎驊修改， 因新竹康橋 WeeklyReport改用 電子報表發送， 
                    //接下來的情境 將不再看WeeklyReport改用 is_send 發送與否， 而是透過後端 檢查是否教師有建立
                    //Form.ViewWeeklyReportForm vwrf = new Form.ViewWeeklyReportForm();
                    //vwrf.ShowDialog();

                    Form.ViewWeeklyReportFormAllESLCourse vwrf = new Form.ViewWeeklyReportFormAllESLCourse();
                    vwrf.ShowDialog();

                };
            }
            {
                Catalog ribbon = RoleAclSource.Instance["學務作業"]["批次作業/查詢"];
                ribbon.Add(new RibbonFeature("BFE3D505-71EE-4CCA-B903-30EAA5F5AF9A", "檢視教師Behavior紀錄"));

                MotherForm.RibbonBarItems["學務作業", "批次作業/查詢"]["檢視Behavior紀錄"].Enable = UserAcl.Current["BFE3D505-71EE-4CCA-B903-30EAA5F5AF9A"].Executable;
                MotherForm.RibbonBarItems["學務作業", "批次作業/查詢"]["檢視Behavior紀錄"].Size = RibbonBarButton.MenuButtonSize.Medium;
                //MotherForm.RibbonBarItems["課程", "ESL課程"]["檢視Behavior紀錄"].Image = Properties.Resources.admissions_zoom_64;
                MotherForm.RibbonBarItems["學務作業", "批次作業/查詢"]["檢視Behavior紀錄"].Click += delegate
                {
                    Form.ViewBehaviorInputForm vbif = new Form.ViewBehaviorInputForm();

                    vbif.ShowDialog();
                };

            }


            #region 生活行為紀錄 毛毛蟲
            {
                string key = "6B4FFF43-611A-4344-8711-4BA0F85DB73B";
                RoleAclSource.Instance["學生"]["資料項目"].Add(new DetailItemFeature(key, "生活行為紀錄"));
                if (FISCA.Permission.UserAcl.Current[key].Editable || FISCA.Permission.UserAcl.Current[key].Viewable)
                {
                    K12.Presentation.NLDPanels.Student.AddDetailBulider(new FISCA.Presentation.DetailBulider<BehaviorItem>());
                }
            }
            #endregion
        }
    }
}
