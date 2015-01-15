/*************************************************************************
** 文件名:   MainPagePrivilege.cs
×× 主要类:   MainPage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-8-14
** 修改人:   
** 日  期:
** 描  述:   MainPage类,主界面的权限控制部分
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using IriskingAttend.ViewModel.SystemViewModel;
using Silverlight.OutlookBar;
using IriskingAttend.Dialog;
using IriskingAttend.ApplicationType;
using System.Diagnostics;

namespace IriskingAttend
{
    public partial class MainPage : Page
    {
        public struct OutLookBarItemList
        {
            public OutlookBarButton barButton;
            public ListBox listBox;
            public List<ListBoxItem> itemList;
        }

        private static List<OutLookBarItemList> barButtonList;
        private void InitOutLookBarList()
        {
            barButtonList = new List<OutLookBarItemList>();

            OutLookBarItemList attendList = new OutLookBarItemList();
            attendList.barButton = outlookBtnAttend;
            attendList.listBox = listBoxAttend;
            attendList.itemList = new List<ListBoxItem>();
            attendList.itemList.Add(listBoxItemAttendOnduty);
            attendList.itemList.Add(listBoxItemAttendQuery);
            attendList.itemList.Add(listBoxItemAttendIdentifyRecord);
            attendList.itemList.Add(listBoxItemAttendAbsense);
            attendList.itemList.Add(listBoxItemAttendRestructure);
            attendList.itemList.Add(listBoxItemAttendLeaveManage);
            attendList.itemList.Add(listBoxItemAttendFestivalMng);
            attendList.itemList.Add(listBoxItemAbnormalAttendInfo);
            attendList.itemList.Add(listBoxItemIrisAttendQuery);
            attendList.itemList.Add(listBoxItemLeaderScheduling);

            OutLookBarItemList lunchList = new OutLookBarItemList();
            lunchList.barButton = outlookBtnLunch;
            lunchList.listBox = listBoxLunch;
            lunchList.itemList = new List<ListBoxItem>();
            lunchList.itemList.Add(listBoxItemLunchManage);
            lunchList.itemList.Add(listBoxItemLunchQuery);

            OutLookBarItemList personList = new OutLookBarItemList();
            personList.barButton = outlookBtnPerson;
            personList.listBox = listBoxPerson;
            personList.itemList = new List<ListBoxItem>();            
            personList.itemList.Add(listBoxItemPersonDepartAndPeople);
            personList.itemList.Add(listBoxItemPersonClassTypeAndOrder);
            personList.itemList.Add(listBoxItemPersonDepart);
            personList.itemList.Add(listBoxItemPersonPrincipal);            
            personList.itemList.Add(listBoxItemPersonWorkType);
            personList.itemList.Add(listBoxItemPersonPrincipalType);

            OutLookBarItemList systemList = new OutLookBarItemList();
            systemList.barButton = outlookBtnSystem;
            systemList.listBox = listBoxSystem;
            systemList.itemList = new List<ListBoxItem>();            
            systemList.itemList.Add(listBoxItemSystemParameter);
            //systemList.itemList.Add(listBoxItemSystemOperator);
            systemList.itemList.Add(listBoxItemSystemDevice);
            systemList.itemList.Add(listBoxItemSystemDBBackup);            
            systemList.itemList.Add(listBoxItemSystemOperatorLog);

            OutLookBarItemList reportList = new OutLookBarItemList();
            reportList.barButton = outlookBtnReport;
            reportList.listBox = listBoxReport;
            reportList.itemList = new List<ListBoxItem>();
            reportList.itemList.Add(listBoxItemReportOriginRecord);
            reportList.itemList.Add(listBoxItemReportCustom);
            reportList.itemList.Add(listBoxItemReportDepartAttendCollect);
            reportList.itemList.Add(listBoxItemReportShenShuoAttendCollect);
            reportList.itemList.Add(listBoxItemReportPersonAttendCollect);            
            reportList.itemList.Add(listBoxItemReportZhouYuanShan );
            reportList.itemList.Add(listBoxItemReportXiBeiDianLanAttend);
            reportList.itemList.Add(listBoxItemReportGuoDian);
            reportList.itemList.Add(listBoxItemReportHuFengDayAttend);
            reportList.itemList.Add(listBoxItemReportHuFengMonthAttend);
            reportList.itemList.Add(listBoxItemReportXiGouDayAttend);
            reportList.itemList.Add(listBoxItemReportXiGouMonthAttend);
            reportList.itemList.Add(listBoxItemReportXiGouInWellPerson);
            reportList.itemList.Add(listBoxItemReportXiGouInWellPersonDetail);
            reportList.itemList.Add(listBoxItemReportZhuDuanMonthAttend);
            reportList.itemList.Add(listBoxItemReportLiuHuangGouMonthAttend);
            reportList.itemList.Add(listBoxItemReportLiuHuangGouMonthAttendUnderRule);
            reportList.itemList.Add(listBoxItemReportLiuHuangGouYearAttend);            

            OutLookBarItemList xinJuLongList = new OutLookBarItemList();
            xinJuLongList.barButton = outlookBtnXinJuLong;
            xinJuLongList.listBox = listBoxXinJuLong;
            xinJuLongList.itemList = new List<ListBoxItem>();
            xinJuLongList.itemList.Add(listBoxItemXinJuLongDepartInWellCollect);
            xinJuLongList.itemList.Add(listBoxItemXinJuLongPersonInWellCollect);
            xinJuLongList.itemList.Add(listBoxItemXinJuLongInCompleteQuery);
            xinJuLongList.itemList.Add(listBoxItemXinJuLongReportPersonMonth);


            OutLookBarItemList zhongKeHongBaList = new OutLookBarItemList();
            zhongKeHongBaList.barButton = outlookBtnZhongKeHongBa;
            zhongKeHongBaList.listBox = listBoxZhongKeHongBa;
            zhongKeHongBaList.itemList = new List<ListBoxItem>();
            zhongKeHongBaList.itemList.Add(listBoxItemZhongKeHongBaPersonFullAttend);
            zhongKeHongBaList.itemList.Add(listBoxItemZhongKeHongBaLeakageAttendanceQuery);
            zhongKeHongBaList.itemList.Add(listBoxItemZhongKeHongBaPersonMealSupplement);
            zhongKeHongBaList.itemList.Add(listBoxItemZhongKeHongBaOriginRecSumList);
            zhongKeHongBaList.itemList.Add(listBoxItemZhongKeHongBaPersonLeaveList);
            zhongKeHongBaList.itemList.Add(listBoxItemZhongKeHongBaPersonLatearrivalList);
            zhongKeHongBaList.itemList.Add(listBoxItemZhongKeHongBaPersonTimeProblemList);

            barButtonList.Add(attendList);            
            barButtonList.Add(lunchList);
            barButtonList.Add(personList);
            barButtonList.Add(systemList);
            barButtonList.Add(reportList);
            barButtonList.Add(xinJuLongList);
            barButtonList.Add(zhongKeHongBaList);
        }
        
        #region 权限私有函数        

        private AbstractApp.PrivilegeENUM GetEnumValue(string enumName)
        {
            AbstractApp.PrivilegeENUM enumValue;
            try
            {
                //根据枚举名称获得枚举对象值              
                enumValue = (AbstractApp.PrivilegeENUM)Enum.Parse(typeof(AbstractApp.PrivilegeENUM), enumName, false);
                return enumValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 初始化权限
        /// </summary>
        private void InitPrivilege()
        {
            Dictionary<bool, Visibility> dictVisualAndBool = new Dictionary<bool, Visibility>();
            dictVisualAndBool.Add( true, Visibility.Visible);
            dictVisualAndBool.Add(false, Visibility.Collapsed);

            string typeName = "";
            AbstractApp.PrivilegeENUM enumValue = 0;
            foreach (OutLookBarItemList barItem in barButtonList)
            {
                //获得超链接按钮的名称，根据超链接按钮的名称获得对应的列表item名称
                typeName = barItem.barButton.Name.Replace("outlookBtn", "");
               
                //根据枚举名称获得枚举对象值              
                enumValue = GetEnumValue(typeName);
                if (VmLogin.DictPrivilege.ContainsKey(enumValue))
                {
                    barItem.barButton.Visibility = dictVisualAndBool[VmLogin.DictPrivilege[enumValue]];
                }
                else
                {
                    barItem.barButton.Visibility = Visibility.Collapsed;
                    continue;
                }
                foreach (ListBoxItem  boxItem in barItem.itemList)
                {
                    //获得超链接按钮的名称，根据超链接按钮的名称获得对应的列表item名称
                    typeName = boxItem.Name.Replace("listBoxItem", "");                    
                   
                    //根据枚举名称获得枚举对象值              
                    enumValue = GetEnumValue(typeName);
                    if (VmLogin.DictPrivilege.ContainsKey(enumValue))
                    {
                        boxItem.Visibility = dictVisualAndBool[VmLogin.DictPrivilege[enumValue]];
                    }
                    else
                    {
                        boxItem.Visibility = Visibility.Collapsed;
                    }
                }
            }

            //操作员管理小模块
            if (!VmLogin.GetIsSuperUser())
            {
                this.listBoxItemSystemOperator.Visibility = dictVisualAndBool[false];
            }
            else
            {
                this.listBoxItemSystemOperator.Visibility = dictVisualAndBool[true];
            }

            InitOutlookBarButtonSelected();
        }

        /// <summary>
        /// 初始化左面outlookbar的按钮显示
        /// </summary>
        private void InitOutlookBarButtonSelected()
        {
            int index = 0;
            while (index < barButtonList.Count && barButtonList[index].barButton.Visibility != Visibility.Visible)
            {
                index++;
            }

            if (index >= barButtonList.Count)            
            {
                MsgBoxWindow.MsgBox("此用户没有任何权限，请重新登录！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
                //重新回到登录界面
                App currentApp = (App)Application.Current;
                Uri url = new Uri("/Login", UriKind.Relative);
                currentApp.Navigation(url);
                return;
            }

            barButtonList[index].barButton.IsSelected = true;                   
        }

        /// <summary>
        /// 当左面列表中的item发生变化时所进行的操作
        /// </summary>
        /// <param name="sender">所包含的item发生变化的列表名字</param>
        private void ListBoxItemSelectionChanged(ListBox sender)
        {
            Uri url = null;

            //Sender列表被选中的item不能为空，并且dictionary当中包含此被选中的item
            if ((sender.SelectedItem != null) && _dictListItemAndUri.ContainsKey((ListBoxItem)sender.SelectedItem))
            {
                //左面Frame跳转到指定的链接页面
                url = new Uri(_dictListItemAndUri[(ListBoxItem)sender.SelectedItem], UriKind.Relative);
                ConvertToFunctionPage(url);

                #region 周源山矿定制
                if (listBoxItemReportZhouYuanShan == sender.SelectedItem)
                {
                    LoadQueryPage();
                }
                #endregion
                return;
            }

            //先寻找选中的是哪个listbox
            int listBoxIndex = 0;
            while (listBoxIndex < barButtonList.Count && barButtonList[listBoxIndex].listBox != sender)
            {
                listBoxIndex++;
            }
            if (listBoxIndex >= barButtonList.Count)
            {
                return;
            }

            //再寻找listBoxItem中默认显示顺序中可见的item
            int itemIndex = 0;
            while (itemIndex < barButtonList[listBoxIndex].itemList.Count && barButtonList[listBoxIndex].itemList[itemIndex].Visibility != Visibility.Visible)
            {
                itemIndex++;
            }
            if (itemIndex >= barButtonList[listBoxIndex].itemList.Count)
            {
                return;
            } 
          
            //确定默认选中的item
            sender.SelectedItem = barButtonList[listBoxIndex].itemList[itemIndex];            
        }

        #endregion
    }
       
}
