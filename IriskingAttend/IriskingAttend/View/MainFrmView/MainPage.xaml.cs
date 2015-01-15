/*************************************************************************
** 文件名:   MainPage.cs
×× 主要类:   MainPage
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   
** 日  期:
** 描  述:   MainPage类,主界面
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
using IriskingAttend.Dialog;
using System.Windows.Media.Imaging;
using IriskingAttend.ApplicationType;
using IriskingAttend.ZhouYuanShan;

namespace IriskingAttend
{
    public partial class MainPage : Page
    {
        #region 变量    

        //是否为第一次启动
        private bool _firstLoad = true;

        //选择的列表框选项与所显示的界面uri之间的对应关系
        private Dictionary<ListBoxItem, string> _dictListItemAndUri = new Dictionary<ListBoxItem, string>();

        #endregion
        
        #region 构造函数

        /// <summary>
        /// 构造函数，初始化
        /// </summary>
        public MainPage()
        {
            if (VmLogin.GetUserName() == "")
            {
                //重新回到登录界面
                App currentApp = (App)Application.Current;
                Uri url = new Uri("/Login", UriKind.Relative);
                currentApp.Navigation(url);
                return;
            }

            //初始化控件
            InitializeComponent();
          
            //string imageUrl = "/IriskingAttend;component/images/Attend.png";
            //System.IO.Stream streamInfo = Application.GetResourceStream(new Uri(imageUrl, UriKind.Relative)).Stream;
            //BitmapImage image = new BitmapImage();
            //image.SetSource(streamInfo);

            //Image imageTest = new Image();
            //imageTest.Source = image;

            //Label lblTest = new Label();
            //lblTest.Content = "当前测试";
            //lblTest.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            //lblTest.FontSize = 12;

            //StackPanel stackTest = new StackPanel();
            //stackTest.Orientation = Orientation.Horizontal;
            //stackTest.Children.Add(imageTest);
            //stackTest.Children.Add(lblTest);

            //HyperlinkButton hbtnTest = new HyperlinkButton();
            //hbtnTest.Name = "hbtnTest";
            //hbtnTest.Foreground = new SolidColorBrush(Colors.Black);
            //hbtnTest.Click += new RoutedEventHandler(hbtn_Click);
            //hbtnTest.Content = stackTest;

            //ListBoxItem itemTest = new ListBoxItem();
            //itemTest.Name = "listBoxItemTest";
            //itemTest.Width = 150;
            //itemTest.Height = 28;
            //itemTest.Content = hbtnTest;

            //ListBox testLst = new ListBox();
            //testLst.Name = "listBoxTest";
            //testLst.ItemContainerStyle = (Style)Application.Current.Resources["ListBoxItemStyle1"];
            //testLst.Loaded += new RoutedEventHandler(listBoxLoaded);
            //testLst.SelectionChanged += new SelectionChangedEventHandler(listBoxSelectionChanged);

            //testLst.Items.Add(itemTest);

            //Silverlight.OutlookBar.OutlookBarButton btn = new Silverlight.OutlookBar.OutlookBarButton();
            //btn.Name = "outlookBtnTest";
            //btn.Header = "测试";
            //btn.HeaderImage = image;//"/IriskingAttend;component/images/Attend.png";
            //btn.Content = testLst;

            //this.outBar.Items.Add(btn);

            //标题抬头
            this.Title = Version.GetFirstVersion();

            txtAppName.Text = Version.GetFirstVersion();

            //初始化初次启动值
            _firstLoad = true;

            //初始化当前用户名显示label
            this.labCurOperater.Content = VmLogin.GetUserName();

            ///初始化字典：选择的列表框选项与所显示的界面uri之间的对应关系
            ///矿山与非矿山应用程序指向的链接不同
            if (VmLogin.GetIsMineApplication())
            {
                InitDictListItemAndUri_Mine();
            }
            else
            {
                InitDictListItemAndUri();
            }
            InitOutLookBarList();
            InitPrivilege();

            AppTypePublic.GetCustomAppType().SetAppLogo();
            imageLogo.Source = AppTypePublic.GetCustomAppType().GetLogoImage();
            txtLogo.Text = AppTypePublic.GetCustomAppType().GetLogoText();
            outlookBtnReport.Visibility = Visibility.Collapsed;//szr
        }

        #endregion

        #region 辅助函数：初始化、页面跳转

        /// <summary>
        /// 初始化字典：选择的列表框选项与所显示的界面uri之间的对应关系
        /// </summary>
        private void InitDictListItemAndUri()
        {
            _dictListItemAndUri.Clear();

            //系统管理模块
            _dictListItemAndUri.Add(listBoxItemSystemOperator, "/OperatorManage");

         
            _dictListItemAndUri.Add(listBoxItemSystemParameter, "/ParameterConfig");
            _dictListItemAndUri.Add(listBoxItemSystemDBBackup, "/BackupConfig");
            _dictListItemAndUri.Add(listBoxItemSystemOperatorLog, "/OperatorLog"); 

            //人员管理模块
            //DictListItemAndUri.Add(listBoxItemPersonInfo, "/Page_peopleMng");
            _dictListItemAndUri.Add(listBoxItemPersonDepart, "/Page_departMng");           
            _dictListItemAndUri.Add(listBoxItemPersonDepartAndPeople, "/PageDepartAndPeopleMng");
            _dictListItemAndUri.Add(listBoxItemPersonClassTypeAndOrder, "/PageClassTypeAndClassOrderMng");
            _dictListItemAndUri.Add(listBoxItemPersonPrincipal, "/PagePrincipal");
            _dictListItemAndUri.Add(listBoxItemPersonWorkType, "/PageWorkType");
            _dictListItemAndUri.Add(listBoxItemPersonPrincipalType, "/PagePrincipalType");

            //管理班中餐
            _dictListItemAndUri.Add(listBoxItemLunchQuery, "/PageQueryLunchRecord");
            _dictListItemAndUri.Add(listBoxItemLunchManage, "/PageUnCompletedLunch");
            

            //考勤管理模块
            if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ZhongKeHongBaApp") != 0)
            {
                _dictListItemAndUri.Add(listBoxItemAttendQuery, "/AttendRecord");
            }
            else
            {
                _dictListItemAndUri.Add(listBoxItemAttendQuery, "/ZKHBAttendRecord");
            }
            _dictListItemAndUri.Add(listBoxItemAttendOnduty, "/InWellPerson");
            _dictListItemAndUri.Add(listBoxItemAttendIdentifyRecord, "/RecogManage");
            _dictListItemAndUri.Add(listBoxItemAttendAbsense, "/AttendLeave");
            _dictListItemAndUri.Add(listBoxItemAttendLeaveManage, "/AttendLeaveManage");
            _dictListItemAndUri.Add(listBoxItemAttendRestructure, "/RebuildAttend");
            _dictListItemAndUri.Add(listBoxItemAttendFestivalMng, "/FestivalMng");
            _dictListItemAndUri.Add(listBoxItemAbnormalAttendInfo, "/AbnormalAttendInfo");
            _dictListItemAndUri.Add(listBoxItemIrisAttendQuery, "/IrisAttendQuery");
            _dictListItemAndUri.Add(listBoxItemLeaderScheduling, "/LeaderScheduling");

            //报表打印模块
            _dictListItemAndUri.Add(listBoxItemReportOriginRecord, "/Xls_OriginRecSumReport");
            _dictListItemAndUri.Add(listBoxItemReportCustom, "/IkReport");

            //西北电缆厂
            _dictListItemAndUri.Add(listBoxItemReportXiBeiDianLanAttend, "/XiBeiDianLanAttendCollect");

            //五虎山
            _dictListItemAndUri.Add(listBoxItemReportDepartAttendCollect, "/DepartAttendCollect");

            //神朔铁路
            _dictListItemAndUri.Add(listBoxItemReportPersonAttendCollect, "/PersonAttendCollect");
            _dictListItemAndUri.Add(listBoxItemReportShenShuoAttendCollect, "/ShenShuoAttendCollect");

            //新巨龙矿
            _dictListItemAndUri.Add(listBoxItemXinJuLongDepartInWellCollect, "/XinJuLongDepartInWellCollect");
            _dictListItemAndUri.Add(listBoxItemXinJuLongPersonInWellCollect, "/XinJuLongPersonInWellCollect");
            _dictListItemAndUri.Add(listBoxItemXinJuLongInCompleteQuery, "/XinJuLongInCompleteQuery");
            _dictListItemAndUri.Add(listBoxItemXinJuLongReportPersonMonth, "/XinJuLongReportPersonMonth");

            //中科虹霸
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonFullAttend, "/ZhongKeHongBaPersonFullAttend");
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaLeakageAttendanceQuery, "/ZhongKeHongBaLeakageAttendanceQuery");     //中科虹霸"楼考勤报表"
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonMealSupplement, "/ZhongKeHongBaPersonMealSupplement");        //中科虹霸餐补报表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaOriginRecSumList, "/ZhongKeHongBaOriginRecSumList");            //原始记录汇总表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonLeaveList ,"/ZhongKeHongBaPersonLeaveList");            //人员请假列表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonLatearrivalList, "/ZhongKeHongBaPersonLatearrivalList");      //迟到早退人员列表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonTimeProblemList, "/ZhongKeHongBaPersonTimeProblemList");     //工时不足8小时超过3次的人员列表
        
            //国电
            _dictListItemAndUri.Add(listBoxItemReportGuoDian, "/GuoDianReport");

            //虎峰
            _dictListItemAndUri.Add(listBoxItemReportHuFengDayAttend, "/HuFengDayAttend");
            _dictListItemAndUri.Add(listBoxItemReportHuFengMonthAttend, "/HuFengMonthAttend");

            //西沟一矿
            _dictListItemAndUri.Add(listBoxItemReportXiGouDayAttend, "/XiGouDayAttend");
            _dictListItemAndUri.Add(listBoxItemReportXiGouMonthAttend, "/XiGouMonthAttend");
            _dictListItemAndUri.Add(listBoxItemReportXiGouInWellPerson, "/XiGouInWellPersonReport");
            _dictListItemAndUri.Add(listBoxItemReportXiGouInWellPersonDetail, "/XiGouInWellPersonDetail");            

            //林州铸锻
            _dictListItemAndUri.Add(listBoxItemReportZhuDuanMonthAttend, "/ZhuDuanMonthAttend");   

            //硫磺沟
            _dictListItemAndUri.Add(listBoxItemReportLiuHuangGouMonthAttend, "/ReportLiuhuangGouMonthAttend");
            _dictListItemAndUri.Add(listBoxItemReportLiuHuangGouMonthAttendUnderRule, "/ReportLiuHuangGouMonthAttendUnderRule");
            _dictListItemAndUri.Add(listBoxItemReportLiuHuangGouYearAttend, "/ReportLiuhuangGouYearAttend");   
            
        }

        /// <summary>
        /// 初始化字典：选择的列表框选项与所显示的界面uri之间的对应关系
        /// </summary>
        private void InitDictListItemAndUri_Mine()
        {
            _dictListItemAndUri.Clear();
            
            //系统管理模块
            _dictListItemAndUri.Add(listBoxItemSystemOperator, "/OperatorManage");

           // DeviceManageYangMei suzengrui 判断是否为阳煤项目 
            if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.YangMeiApp") != 0)
            {
                _dictListItemAndUri.Add(listBoxItemSystemDevice, "/DeviceManage");
            }
            else
            {
                _dictListItemAndUri.Add(listBoxItemSystemDevice, "/DeviceManageYangMei");
            }
            _dictListItemAndUri.Add(listBoxItemSystemParameter, "/ParameterConfig");
            _dictListItemAndUri.Add(listBoxItemSystemDBBackup, "/BackupConfig");
            _dictListItemAndUri.Add(listBoxItemSystemOperatorLog, "/OperatorLog");            

            //人员管理模块        
            _dictListItemAndUri.Add(listBoxItemPersonDepart, "/Page_departMng");
            if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.WuHuShanApp") != 0)
            {
                _dictListItemAndUri.Add(listBoxItemPersonDepartAndPeople, "/PageDepartAndPeopleMng");
            }
            else
            {
                _dictListItemAndUri.Add(listBoxItemPersonDepartAndPeople, "/WuHuShanPageDepartAndPeopleMng");
            }
            _dictListItemAndUri.Add(listBoxItemPersonClassTypeAndOrder, "/PageClassTypeAndClassOrderMng");
            _dictListItemAndUri.Add(listBoxItemPersonPrincipal, "/PagePrincipal");
            _dictListItemAndUri.Add(listBoxItemPersonWorkType, "/PageWorkType");
            _dictListItemAndUri.Add(listBoxItemPersonPrincipalType, "/PagePrincipalType");

            //考勤管理模块
            _dictListItemAndUri.Add(listBoxItemAttendQuery, "/AttendRecordMine");
            _dictListItemAndUri.Add(listBoxItemAttendOnduty, "/InWellPersonMine");
            _dictListItemAndUri.Add(listBoxItemAttendIdentifyRecord, "/RecogManageMine");
            _dictListItemAndUri.Add(listBoxItemAttendAbsense, "/AttendLeave");
            _dictListItemAndUri.Add(listBoxItemAttendLeaveManage, "/AttendLeaveManage");
            _dictListItemAndUri.Add(listBoxItemAttendRestructure, "/RebuildAttend");
            _dictListItemAndUri.Add(listBoxItemAttendFestivalMng, "/FestivalMng");
            _dictListItemAndUri.Add(listBoxItemAbnormalAttendInfo, "/AbnormalAttendInfo");
            _dictListItemAndUri.Add(listBoxItemIrisAttendQuery, "/IrisAttendQuery");
            _dictListItemAndUri.Add(listBoxItemLeaderScheduling, "/LeaderScheduling");

            //报表打印模块
            _dictListItemAndUri.Add(listBoxItemReportOriginRecord, "/Xls_OriginRecSumReport");
            _dictListItemAndUri.Add(listBoxItemReportCustom, "/IkReport");

            //西北电缆厂
            _dictListItemAndUri.Add(listBoxItemReportXiBeiDianLanAttend, "/XiBeiDianLanAttendCollect");

            _dictListItemAndUri.Add(listBoxItemReportDepartAttendCollect, "/DepartAttendCollect");
            _dictListItemAndUri.Add(listBoxItemReportPersonAttendCollect, "/PersonAttendCollect");
            _dictListItemAndUri.Add(listBoxItemReportShenShuoAttendCollect, "/ShenShuoAttendCollect");
            _dictListItemAndUri.Add(listBoxItemReportZhouYuanShan, "/Home"); 

            //管理班中餐
            _dictListItemAndUri.Add(listBoxItemLunchQuery, "/PageQueryLunchRecord");
            _dictListItemAndUri.Add(listBoxItemLunchManage, "/PageUnCompletedLunch");

            //新巨龙矿
            _dictListItemAndUri.Add(listBoxItemXinJuLongDepartInWellCollect, "/XinJuLongDepartInWellCollect");
            _dictListItemAndUri.Add(listBoxItemXinJuLongPersonInWellCollect, "/XinJuLongPersonInWellCollect");
            _dictListItemAndUri.Add(listBoxItemXinJuLongInCompleteQuery, "/XinJuLongInCompleteQuery");
            _dictListItemAndUri.Add(listBoxItemXinJuLongReportPersonMonth, "/XinJuLongReportPersonMonth");

            //中科虹霸
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonFullAttend, "/ZhongKeHongBaPersonFullAttend");
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaLeakageAttendanceQuery, "/ZhongKeHongBaLeakageAttendanceQuery");     //中科虹霸"楼考勤报表"
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonMealSupplement, "/ZhongKeHongBaPersonMealSupplement");        //中科虹霸餐补报表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaOriginRecSumList, "/ZhongKeHongBaOriginRecSumList");            //原始记录汇总表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonLeaveList, "/ZhongKeHongBaPersonLeaveList");            //人员请假列表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonLatearrivalList, "/ZhongKeHongBaPersonLatearrivalList");      //迟到早退人员列表
            _dictListItemAndUri.Add(listBoxItemZhongKeHongBaPersonTimeProblemList, "/ZhongKeHongBaPersonTimeProblemList");     //工时不足8小时超过3次的人员列表
        
            //国电
            _dictListItemAndUri.Add(listBoxItemReportGuoDian, "/GuoDianReport");

            //虎峰
            _dictListItemAndUri.Add(listBoxItemReportHuFengDayAttend, "/HuFengDayAttend");
            _dictListItemAndUri.Add(listBoxItemReportHuFengMonthAttend, "/HuFengMonthAttend");

            //西沟一矿
            _dictListItemAndUri.Add(listBoxItemReportXiGouDayAttend, "/XiGouDayAttend");
            _dictListItemAndUri.Add(listBoxItemReportXiGouMonthAttend, "/XiGouMonthAttend");
            _dictListItemAndUri.Add(listBoxItemReportXiGouInWellPerson, "/XiGouInWellPersonReport");
            _dictListItemAndUri.Add(listBoxItemReportXiGouInWellPersonDetail, "/XiGouInWellPersonDetail");   

            //林州铸锻
            _dictListItemAndUri.Add(listBoxItemReportZhuDuanMonthAttend, "/ZhuDuanMonthAttend");

            //硫磺沟
            _dictListItemAndUri.Add(listBoxItemReportLiuHuangGouMonthAttend, "/ReportLiuhuangGouMonthAttend");
            _dictListItemAndUri.Add(listBoxItemReportLiuHuangGouMonthAttendUnderRule, "/ReportLiuHuangGouMonthAttendUnderRule");
            _dictListItemAndUri.Add(listBoxItemReportLiuHuangGouYearAttend, "/ReportLiuhuangGouYearAttend");   
        }

        /// <summary>
        /// 页面跳转
        /// </summary>
        /// <param name="url">跳转到页面的Uri</param>
        private void ConvertToFunctionPage(Uri url)
        {
            this.ContentFrame.Source = url;
        }

        //旋转界面调用
        /*private void ConvertToFunctionPage(Uri url)
        {
            FrmMain frm = (FrmMain)this.ContentFrame.Content;
            frm.ConvertToFunctionPage(url);   
        }*/

        #endregion

        #region 事件触发函数

        /// <summary>
        /// 左面列表大模块load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxLoaded(object sender, RoutedEventArgs e)
        {
            ///MainFrame页面刚刚加载时，左面列表的默认加载列表会加载两次，
            ///所以忽略第一次，否则两次同时访问数据库会发生异常
            if (_firstLoad)
            {
                _firstLoad = false;
                return;
            }

            //第二次加载时，正常进行列表item加载。
            ListBoxItemSelectionChanged((ListBox)sender);

            //this.Dispatcher.BeginInvoke(new EventHandler((listBox, ee) => listBoxLoaded(listBox, (RoutedEventArgs)ee)), new object[] { sender, e });               
        }

        /// <summary>
        /// 左面列表小模块item发生变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItemSelectionChanged((ListBox)sender);
        }

        /// <summary>
        /// 左面列表item中的超链接按钮点击刷新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtn_Click(object sender, RoutedEventArgs e)
        {
            //获得超链接按钮的名称，根据超链接按钮的名称获得对应的列表item名称
            string controlName = ((HyperlinkButton)sender).Name;
            controlName = controlName.Replace("hbtn", "listBoxItem");

            //根据列表item名称获得列表item对象
            object o = this.GetType().GetField(controlName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase).GetValue(this);

            ///与点击的超链接按钮匹配的列表item处于未被选中状态时，将其置为选中状态，
            ///这样就会触发列表item选中状态发生变化的事件。
            if (!((ListBoxItem)o).IsSelected)
            {
                ((ListBoxItem)o).IsSelected = true;
                return;
            }

            #region 周源山矿定制
            if (hbtnReportZhouYuanShan == sender)
            {
                LoadQueryPage();
                return;
            }
            #endregion

            //与点击的超链接按钮匹配的列表item处于选中状态时，右面Frame页面进行刷新
            this.ContentFrame.Refresh();
        }       

        /// <summary>
        /// 退出登录按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MsgBoxWindow.MsgBox("确定要退出登录么？", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OKCancel, (result) =>
                {
                    if (result == MsgBoxWindow.MsgResult.OK)
                    {
                        //销毁单例对象
                        ChildWndReportQuery.DestroyInstance();
                        //重新回到登录界面
                        App currentApp = (App)Application.Current;
                        Uri url = new Uri("/Login", UriKind.Relative);
                        currentApp.Navigation(url);  
                    }
                });                      
        }

        /// <summary>
        /// 修改当前用户密码按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnModifyPassword_Click(object sender, RoutedEventArgs e)
        {
            //加载修改当前用户密码对话框
            DlgModifyCurPwd dlgModifyCurPwd = new DlgModifyCurPwd();
            dlgModifyCurPwd.Show();
        }

        #endregion  

        #region 关于

        /// <summary>
        /// cty
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hbtnAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutSystem about = new AboutSystem();
            about.Show();
        }

        #endregion
    }
}
