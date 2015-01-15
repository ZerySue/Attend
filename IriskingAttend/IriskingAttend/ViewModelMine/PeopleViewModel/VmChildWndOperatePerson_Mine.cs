/*************************************************************************
** 文件名:   VmChildWndOperatePerson.cs
×× 主要类:   VmChildWndOperatePerson
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   wz 代码优化
** 日  期:   2013-7-23
** 描  述:   VmChildWndOperatePerson类,人员信息管理（增删改查） 用于矿山模式
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Client;
using Irisking.Web.DataModel;
using EDatabaseError;
using IriskingAttend.Web;
using IriskingAttend.Common;
using System.IO.IsolatedStorage;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using IriskingAttend.View.PeopleView;
using System.Windows.Media.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using IriskingAttend.Dialog;
using IriskingAttend.BehaviorSelf;
using IriskingAttend.ViewModel;
using IriskingAttend.ViewModel.PeopleViewModel;
using IriskingAttend.ViewModel.SystemViewModel;
using System.Text;

namespace IriskingAttend.ViewModelMine.PeopleViewModel
{
    public class VmChildWndOperatePerson_Mine : BaseViewModel
    {

        #region 字段声明
        
        /// <summary>
        /// 与服务声明
        /// </summary>
        private DomainServiceIriskingAttend serviceDomDbAccess = new DomainServiceIriskingAttend();

        /// <summary>
        /// viewModel加载完毕事件
        /// </summary>
        public event EventHandler LoadCompletedEvent;

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        public event Action<bool> CloseEvent;

        //子窗口的操作模式
        private ChildWndOptionMode operatePersonMode;

        private ImageSource noneImg;    //无照片人员的显示的照片图像
        private ImageSource selectImg;  //可编辑模式下无照片人员显示的照片图像

        private string imgType;   //照片图像的类型
        private byte[] imgData;   //存放照片图像

        //当前人员的ID
        private int personID = -1;

        //当前人员信息
        private UserPersonInfo curPersonInfo;

        //班次ID列表
        private List<int> classTypeIDs;
        //部门ID列表
        private List<int> departIDs;

        //添加人员操作时的目标部门
        private int _targetDepartId = 0;

        //是否执行了继续添加操作
        private bool _isContinueAdd = false;

        #endregion

        #region 与页面绑定的命令
        
        /// <summary>
        /// 选择照片命令
        /// </summary>
        public DelegateCommand ChooseImgCmd { get; set; }

        /// <summary>
        /// 取消选择的照片命令
        /// </summary>
        public DelegateCommand CancelChoosedImgCmd { get; set; }

        /// <summary>
        /// 取消按钮命令
        /// </summary>
        public DelegateCommand CancelBtnCmd { get; set; }

        /// <summary>
        /// ok按钮命令
        /// </summary>
        public DelegateCommand OkBtnCmd { get; set; }

        /// <summary>
        /// bug2730 继续添加按钮命令 by蔡天雨
        /// </summary>
        public DelegateCommand OkContinueAddCmd { get; set; }

        #endregion

        #region   与页面绑定的属性

        private Visibility oKContinueVisibility;

        /// <summary>
        /// 继续添加按钮可见性
        /// </summary>
        public Visibility OKContinueVisibility
        {
            get { return oKContinueVisibility; }
            set
            {
                oKContinueVisibility = value;
                this.NotifyPropertyChanged("OKContinueVisibility");
            }
        }

        private Visibility oKButtonVisibility;

        /// <summary>
        /// ok按钮可见性
        /// </summary>
        public Visibility OKButtonVisibility
        {
            get { return oKButtonVisibility; }
            set
            {
                oKButtonVisibility = value;
                this.NotifyPropertyChanged("OKButtonVisibility");
            }
        }
        

        private string title;

        /// <summary>
        /// 窗口标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                this.NotifyPropertyChanged("Title");
            }
        }

        
        private List<string> departNames;
        /// <summary>
        /// 部门名称表
        /// </summary>
        public List<string> DepartNames
        {
            get { return departNames; }
            set
            {
                departNames = value;
                this.NotifyPropertyChanged("DepartNames");
            }
        }

        
        private int departNames_SelectedIndex;

        /// <summary>
        /// 部门名称表当前选择的Index
        /// </summary>
        public int DepartNames_SelectedIndex
        {
            get { return departNames_SelectedIndex; }
            set
            {
                departNames_SelectedIndex = value;
                this.NotifyPropertyChanged("DepartNames_SelectedIndex");
            }
        }

        
        private bool isEditable;

        /// <summary>
        /// 页面内容是否可以编辑
        /// </summary>
        public bool IsEditable
        {
            get { return isEditable; }
            set
            {
                this.IsReadOnly = !value;
                isEditable = value;
                this.NotifyPropertyChanged("IsEditable");
            }
        }

        private bool isReadOnly;
        /// <summary>
        /// 页面内容是否是只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                this.NotifyPropertyChanged("IsReadOnly");
            }
        }
        
        private string enrollInfo;
        /// <summary>
        /// 注册状态
        /// </summary>
        public string EnrollInfo
        {
            get { return enrollInfo; }
            set
            {
                enrollInfo = value;
                this.NotifyPropertyChanged("EnrollInfo");
            }
        }

        private string personName;
        /// <summary>
        /// 姓名
        /// </summary>
        public string PersonName
        {
            get { return personName; }
            set
            {
                personName = value;
                this.NotifyPropertyChanged("PersonName");
            }
        }

        private string workSn;
        /// <summary>
        /// 工号
        /// </summary>
        public string WorkSn
        {
            get { return workSn; }
            set
            {
                workSn = value;
                this.NotifyPropertyChanged("WorkSn");
            }
        }

        private List<string> classTypeNamesAll;
        /// <summary>
        /// 班制集合
        /// </summary>
        public List<string> ClassTypeNamesAll
        {
            get { return classTypeNamesAll; }
            set
            {
                classTypeNamesAll = value;
                this.NotifyPropertyChanged("ClassTypeNamesAll");
            }
        }

        private int classTypeNames_SelectedIndex_OnGround;
        
        /// <summary>
        /// 当前地面班制 索引
        /// </summary>
        public int ClassTypeNames_SelectedIndex_OnGround
        {
            get { return classTypeNames_SelectedIndex_OnGround; }
            set
            {
                classTypeNames_SelectedIndex_OnGround = value;
                this.NotifyPropertyChanged("ClassTypeNames_SelectedIndex_OnGround");
            }
        }

        private int classTypeNames_SelectedIndex;
        /// <summary>
        /// 当前井下班制 索引
        /// </summary>
        public int ClassTypeNames_SelectedIndex
        {
            get { return classTypeNames_SelectedIndex; }
            set
            {
                classTypeNames_SelectedIndex = value;
                this.NotifyPropertyChanged("ClassTypeNames_SelectedIndex");
            }
        }

        private string phone;
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone
        {
            get { return phone; }
            set
            {
                phone = value;
                this.NotifyPropertyChanged("Phone");
            }
        }

        private string zipCode;
        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode
        {
            get { return zipCode; }
            set
            {
                zipCode = value;
                this.NotifyPropertyChanged("ZipCode");
            }
        }

        
        private string iDCardNumber;

        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCardNumber
        {
            get { return iDCardNumber; }
            set
            {
                iDCardNumber = value;
                this.NotifyPropertyChanged("IDCardNumber");
            }
        }

        
        private string address;

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                this.NotifyPropertyChanged("Address");
            }
        }

        
        private string email;
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
                this.NotifyPropertyChanged("Email");
            }
        }

        
        private string tagInfo;

        /// <summary>
        /// 备注
        /// </summary>
        public string TagInfo
        {
            get { return tagInfo; }
            set
            {
                tagInfo = value;
                this.NotifyPropertyChanged("TagInfo");
            }
        }

        
        private ImageSource personImg;

        /// <summary>
        /// 照片
        /// </summary>
        public ImageSource PersonImg
        {
            get { return personImg; }
            set
            {
                personImg = value;
                this.NotifyPropertyChanged("PersonImg");
            }
        }
        
        private object birthDate;
        /// <summary>
        /// 出生日期
        /// </summary>
        public object BirthDate
        {
            get { return birthDate; }
            set
            {
                birthDate = value;
                this.NotifyPropertyChanged("BirthDate");
            }
        }

        private object workDate;
        /// <summary>
        /// 参加工作时间
        /// </summary>
        public object WorkDate
        {
            get { return workDate; }
            set
            {
                workDate = value;
                this.NotifyPropertyChanged("WorkDate");
            }
        }

        private int sex;
        /// <summary>
        /// 性别 0是男 1是女
        /// </summary>
        public int Sex
        {
            get { return sex; }
            set
            {
                if (value > 0) value = 1;
                sex = value;
                this.NotifyPropertyChanged("Sex");
            }
        }

        private int bloodType;
        /// <summary>
        /// 血型 
        /// </summary>
        public int BloodType
        {
            get { return bloodType; }
            set
            {
                bloodType = value;
                this.NotifyPropertyChanged("BloodType");
            }
        }

        private string okBtnContent;
        /// <summary>
        /// ok按钮的内容
        /// </summary>
        public string OkBtnContent
        {
            get { return okBtnContent; }
            set
            {
                okBtnContent = value;
                this.NotifyPropertyChanged("OkBtnContent");
            }
        }

        private string cancelBtnContent;
        /// <summary>
        /// 取消按钮的内容 by cty
        /// </summary>
        public string CancelBtnContent
        {
            get { return cancelBtnContent; }
            set
            {
                cancelBtnContent = value;
                this.NotifyPropertyChanged("CancelBtnContent");
            }
        }

        #endregion

        #region 按需添加项
       
        #region 五虎山需求 与界面绑定属性

        private List<PrincipalInfo> _principalInfos;

        /// <summary>
        /// 职务信息列表
        /// </summary>
        public List<PrincipalInfo> PrincipalInfos
        {
            get { return _principalInfos; }
            set
            {
                _principalInfos = value;
                this.OnPropertyChanged(() => this.PrincipalInfos);
            }
        }

        private PrincipalInfo _selectedPrincipalInfo;

        /// <summary>
        /// 当前选择的职务
        /// </summary>
        public PrincipalInfo SelectedPrincipalInfo
        {
            get { return _selectedPrincipalInfo; }
            set
            {
                _selectedPrincipalInfo = value;
                this.OnPropertyChanged(() => this.SelectedPrincipalInfo);
            }
        }

        private List<WorkTypeInfo> _workTypeInfos;

        /// <summary>
        /// 工种信息列表
        /// </summary>
        public List<WorkTypeInfo> WorkTypeInfos
        {
            get { return _workTypeInfos; }
            set
            {
                _workTypeInfos = value;
                this.OnPropertyChanged(() => this.WorkTypeInfos);
            }
        }

        private WorkTypeInfo _selectedWorkTypeInfo;

        /// <summary>
        /// 当前选择的工种
        /// </summary>
        public WorkTypeInfo SelectedWorkTypeInfo
        {
            get { return _selectedWorkTypeInfo; }
            set
            {
                _selectedWorkTypeInfo = value;
                this.OnPropertyChanged(() => this.SelectedWorkTypeInfo);
            }
        }


        #endregion

        #endregion

        #region 构造函数

        public VmChildWndOperatePerson_Mine(ChildWndOptionMode _OperatePersonMode, UserPersonInfo _personInfo, int targetDepartId=0)
        {
            EnrollInfo = "未知";
            operatePersonMode = _OperatePersonMode;
            _targetDepartId = targetDepartId;

            noneImg = new BitmapImage(new Uri(@"/IriskingAttend;component/Images/NoneImg.png", UriKind.Relative));
            selectImg = new BitmapImage(new Uri(@"/IriskingAttend;component/Images/SelectImg.png", UriKind.Relative));

            curPersonInfo = _personInfo;
            personID = _personInfo == null ? -1:_personInfo.person_id;

            ChooseImgCmd = new DelegateCommand(new Action(ChooseImg));
            CancelChoosedImgCmd = new DelegateCommand(new Action(CancelChoosedImg));
            OkBtnCmd = new DelegateCommand(new Action(OkBtnClicked));
            OkContinueAddCmd = new DelegateCommand(new Action(OkBtnContinueAddClicked));//bug2730 by蔡天雨
            CancelBtnCmd = new DelegateCommand(new Action(CancelBtnClicked));
            GetContent(_OperatePersonMode);


        }
        #endregion

        #region 界面绑定的事件响应
     
        /// <summary>
        /// 选择本地照片
        /// </summary>
        void ChooseImg()
        {
            

            //选择图片
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "png图像文件|*.png"; by wangzhuo
            ofd.Filter = "图片文件(*.jpg,*.png,*.bmp)|*.png;*.jpg;*.bmp"; //修改 不只是可以添加png格式的图片 by cty
            ofd.Multiselect = false;
            bool? res = ofd.ShowDialog();
            if (res.HasValue && res.Value)
            {
                try
                {
                    Stream stream = ofd.File.OpenRead();
                    BitmapImage img = new BitmapImage();
                    imgType = ofd.File.Name;
                    img.SetSource(stream);
                    imgData = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(imgData, 0, imgData.Length);

                    stream.Close();
                    this.PersonImg = null;
                    GC.Collect();
                    this.PersonImg = img;
                }
                catch (Exception e)
                {
                    ErrorWindow err = new ErrorWindow(e);
                    err.Show();
                }
             
            }
        }

        /// <summary>
        /// 取消选择照片
        /// </summary>
        void CancelChoosedImg()
        {
            this.PersonImg = selectImg;
            this.imgData = null;
            this.imgType = null;
        }

        //继续添加按钮命令 by蔡天雨
        void OkBtnContinueAddClicked()
        {
            switch (operatePersonMode)
            {
                case ChildWndOptionMode.Modify:
                    ModifyRia();
                    break;
                case ChildWndOptionMode.Delete:
                    DeleteRia(personID);
                    break;
                case ChildWndOptionMode.Check:
                    if (CloseEvent != null)
                    {
                        CloseEvent(false);
                    }
                    break;
                case ChildWndOptionMode.Add:

                    //bug2673 校验输入的人员姓名中是否包含非法字符 by蔡天雨 
                    CheckInputInfo(() =>
                        {
                            AddPersonRia(false);//调用添加人员信息的函数
                        });

                    break;
                case ChildWndOptionMode.Record:
                    break;
                case ChildWndOptionMode.StopIris:
                    break;
            }
        }

        void OkBtnClicked()
        {
            switch (operatePersonMode)
            {
                case ChildWndOptionMode.Modify:
                    //bug2673 校验输入的人员姓名中是否包含非法字符 by蔡天雨 
                    CheckInputInfo(() =>
                    {
                        ModifyRia();
                    });
                    break;
                case ChildWndOptionMode.Delete:
                    DeleteRia(personID);
                    break;
                case ChildWndOptionMode.Check:
                    if (CloseEvent != null)
                    {
                        CloseEvent(false);
                    }
                    break;
                case ChildWndOptionMode.Add:
 
                    //bug2673 校验输入的人员姓名中是否包含非法字符 by蔡天雨 
                    CheckInputInfo(() =>
                        {
                            AddPersonRia(true);
                        });

                    break;
                case ChildWndOptionMode.Record:
                    break;
                case ChildWndOptionMode.StopIris:
                    break;
            }
        }

        void CancelBtnClicked()
        {
            if (this.CloseEvent != null)
            {
                CloseEvent(_isContinueAdd);
            }
        }

        #endregion

        #region 私有功能函数

        /// <summary>
        /// 根据窗口操作模式决定显示哪种内容
        /// </summary>
        /// <param name="OperatePersonMode"></param>
        private void GetContent(ChildWndOptionMode OperatePersonMode)
        {
            //异步操作 ，先获取部门信息，再获取班制，再获取职务，在获取工种
            //最后获取人员信息
            WaitingDialog.ShowWaiting();
            GetDepartNames(() =>
                {
                    GetClassTypeNames(() =>
                        {
                            GetPricipalsRia(() =>
                                {
                                    GetWorkTypesRia(() =>
                                        {
                                            if (operatePersonMode != ChildWndOptionMode.Add)
                                            {
                                                GePersonDetailInfoRia(() =>
                                                {
                                                    //延迟绑定
                                                    if (LoadCompletedEvent != null)
                                                    {
                                                        LoadCompletedEvent(this, null);
                                                    }
                                                    WaitingDialog.HideWaiting();
                                                });
                                            }
                                            else
                                            {
                                                //延迟绑定
                                                if (LoadCompletedEvent != null)
                                                {
                                                    LoadCompletedEvent(this, null);
                                                }
                                                WaitingDialog.HideWaiting();
                                            }
                                        });
                                });
                        });
                });
            OKContinueVisibility = Visibility.Collapsed;
            switch (OperatePersonMode)
            {
                case ChildWndOptionMode.Modify:

                    Title = "人员详细信息";
                    this.IsEditable = true;
                    this.OkBtnContent = "修改";
                    this.CancelBtnContent = "取消";
                    OKButtonVisibility = Visibility.Visible;
                    break;
                case ChildWndOptionMode.Delete:

                    Title = "删除人员信息";
                    this.IsEditable = false;
                    this.OkBtnContent = "删除";
                    this.CancelBtnContent = "取消";
                    break;
                case ChildWndOptionMode.Check:

                    this.IsEditable = false;
                    Title = "查看人员信息";
                    this.OkBtnContent = "确定";
                    this.CancelBtnContent = "关闭";
                    OKButtonVisibility = Visibility.Collapsed;
                    break;
                case ChildWndOptionMode.Add:
                    Title = "添加新员工";
                    this.OkBtnContent = "添加";
                    this.CancelBtnContent = "取消";
                    this.IsEditable = true;
                    this.PersonImg = selectImg;
                    OKButtonVisibility = Visibility.Visible;
                    OKContinueVisibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// 刷新内容
        /// </summary>
        private void RefreshContent()
        {
            this.PersonName = "";
            this.WorkSn = "";
            this.Phone = "";
            this.ZipCode = "";
            this.IDCardNumber = "";
            this.Address = "";
            this.Email = "";
            this.TagInfo = "";
            this.BirthDate = null;
            this.WorkDate = null;
            this.PersonImg = selectImg;
            this.imgData = null;
            this.imgType = null;
           
        }


        /// <summary>
        /// 校验输入的信息中是否包含非法字符 by蔡天雨
        /// </summary>
        /// <returns></returns>
        private void CheckInputInfo(Action callBack)
        {

            if (PersonName == null || PersonName.Equals(""))
            {

                MsgBoxWindow.MsgBox(
                     "人员姓名不能为空",
                     Dialog.MsgBoxWindow.MsgIcon.Information,
                     Dialog.MsgBoxWindow.MsgBtns.OK);
                return;
            }

            Regex regExp_PersonName = new Regex("[~!@#$%^&*()=+[\\]{}''\";:/?.,><`|！·￥…—（）\\-、；：。，》《]");//人员姓名 by蔡天雨
            Regex regExp_Phone = new Regex(@"(^\d+-+\d$|^\d)");//(@"(^1[3-8]\d{9}$|^\d{3}-\d{8}$|^\d{4}-\d{7}$)");       //联系电话 by蔡天雨
            Regex regExp_zipCode = new Regex(@"^[0-9]*$");                                                                //邮编 by蔡天雨
            Regex regExp_Address = new Regex("[~!@$%^&=+！￥]");                                                      //联系地址 by蔡天雨
            Regex regExp_Email = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)" + @"|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");//电子邮箱 by蔡天雨
            if (regExp_PersonName.IsMatch(PersonName))                            //bug2670 校验输入的人员姓名中是否包含非法字符 by蔡天雨
            {
                MsgBoxWindow.MsgBox(
                       "您输入的姓名中包含非法字符！",
                       Dialog.MsgBoxWindow.MsgIcon.Information,
                       Dialog.MsgBoxWindow.MsgBtns.OK);
            }
            else if (!(Phone == null || Phone == "") && !regExp_Phone.IsMatch(Phone))
            {
                MsgBoxWindow.MsgBox(
                       "联系电话！\r请确定您输入的是数字！",
                       Dialog.MsgBoxWindow.MsgIcon.Information,
                       Dialog.MsgBoxWindow.MsgBtns.OK);
            }
            else if (!(ZipCode == null || ZipCode == "") && !regExp_zipCode.IsMatch(ZipCode) || (CheckLength(ZipCode) > 10))
            {
                MsgBoxWindow.MsgBox(
                          "邮政编码！\r请确定您输入的是小于10位的数字！",
                          Dialog.MsgBoxWindow.MsgIcon.Information,
                          Dialog.MsgBoxWindow.MsgBtns.OK);
            }
            else if (!(Address == null || Address == "") && regExp_Address.IsMatch(Address))
            {
                MsgBoxWindow.MsgBox(
                             "联系地址！\r您输入的地址中包含非法字符！",
                             Dialog.MsgBoxWindow.MsgIcon.Information,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
            }
            else if (!(Email == null || Email == "") && !regExp_Email.IsMatch(Email))
            {
                MsgBoxWindow.MsgBox(
                             "电子邮箱！\r您输入的电子邮箱格式不正确！",
                             Dialog.MsgBoxWindow.MsgIcon.Information,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
            }
            else
            {
                CheckWorkSn( callBack);
            }

        }

        /// <summary>
        /// 取得字符串的长度并返回 
        /// 如果字符串为空 则返回0
        /// by蔡天雨
        /// </summary>
        /// <param name="s">输入的字符串</param>
        /// <returns></returns>
        private int CheckLength(string s)
        {
            int length_ZipCode = 0;
            if (s != null)
            {
                length_ZipCode = s.Length;

            }

            return length_ZipCode;
        }

        /// <summary>
        /// 工号校验，不能添加相同工号的人员 by 蔡天雨
        /// </summary>
        /// <param name="isCloseWnd"></param>
        private void CheckWorkSn(Action callBack)
        {
            WaitingDialog.ShowWaiting();
            int a = VmDepartAndPeopleMng.AllDepartId; string b = null; string c = null; string d = "全部"; string e = "全部"; string f = "包含";
            EntityQuery<UserPersonInfo> list = serviceDomDbAccess.GetPersonsInfoTableQuery(a, b, c, d, e, f);
            ///回调异常类
            Action<LoadOperation<UserPersonInfo>> actionCallBack = new Action<LoadOperation<UserPersonInfo>>(ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack);
            ///异步事件
            LoadOperation<UserPersonInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
            lo.Completed += delegate
            {
                List<UserPersonInfo> UserPersonInfo_QueryRes = new List<UserPersonInfo>(); 
                //异步获取数据
                foreach (UserPersonInfo item in lo.Entities)
                {
                    UserPersonInfo_QueryRes.Add(item);
                }
                
                //查询到的数据库中工号与当前工号相同的数量
                int theSameCount = UserPersonInfo_QueryRes.Where( (per) => 
                    {
                        //在修改状态下，如果工号没做改变，则不算重复
                        if (operatePersonMode == ChildWndOptionMode.Modify &&
                            WorkSn == curPersonInfo.work_sn)
                        {
                            return false;
                        }
                        //如果工号为空，则不算重复
                        if (WorkSn == "")
                        {
                            return false;
                        }
                        return per.work_sn == WorkSn;
                    }).Count();
                
           
                if (theSameCount == 0)
                {
                    if (CheckIDCard(IDCardNumber))// 验证身份证号码
                    {
                        callBack();
                    }
                    else
                    {
                        MsgBoxWindow.MsgBox(
                             "身份证号！\r请确定您输入的是15-18位数字！",
                             Dialog.MsgBoxWindow.MsgIcon.Information,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
                        WaitingDialog.HideWaiting();
                    }
                }
                else
                {
                    MsgBoxWindow.MsgBox(
                             "您输入的【人员工号】已被占用，请重新输入！",
                             Dialog.MsgBoxWindow.MsgIcon.Warning,
                             Dialog.MsgBoxWindow.MsgBtns.OK);
                    WaitingDialog.HideWaiting();
                }
               
            };
        }

        /// <summary>
        /// 从部门列表中获取指定部门的index
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetDepartIndex(string name)
        {
            if (name == null)
            {
                return -1;
            }
            name = name.Trim();
            for (int i = 0; i < DepartNames.Count; i++)
            {
                if (DepartNames[i].Equals(name))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从性别列表中获取指定性别的index
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetSexIndex(string name)
        {
            if (name == null)
            {
                return -1;
            }
            name = name.Trim();
            if (name.Equals("男"))
            {
                return 0;
            }
            if (name.Equals("女"))
            {
                return 1;
            }
            return -1;
        }

        /// <summary>
        /// 从班制列表中获取指定班制的index
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetClassTypeIndex(string name)
        {
            if (name == null)
            {
                return -1;
            }
            name = name.Trim();
            for (int i = 0; i < ClassTypeNamesAll.Count; i++)
            {
                if (ClassTypeNamesAll[i].Equals(name))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 将byte[]数据填入图像中
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ImageSource GetImg(byte[] data)
        {

            if (data == null || data.Length == 0)
            {
                if (operatePersonMode == ChildWndOptionMode.Modify)
                {
                    return selectImg;
                }
                else
                {
                    return noneImg;
                }
            }
            else
            {
                BitmapImage imageSource = null;
                try
                {
                    using (MemoryStream stream = new MemoryStream(data))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        BitmapImage b = new BitmapImage();
                        b.SetSource(stream);
                        imageSource = b;
                    }
                }
                catch (System.Exception ex)
                {
                    ErrorWindow err = new ErrorWindow(ex);
                    err.Show();
                }
                return imageSource;

            }
        }

        #region 验证身份证号码是否合法 by蔡天雨
        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="Id">身份证字符串</param>
        /// <returns></returns>
        private bool CheckIDCard(string Id)
        {
            if (Id == null || Id == "")
            {
                return true;
            }
            else if (Id.Length == 18)
            {
                bool check = CheckIDCard18(Id);
                return check;
            }
            else if (Id.Length == 15)
            {
                bool check = CheckIDCard15(Id);
                return check;
            }
            else
            {
                return false;
            }
        }

        private bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }

            return true;//符合GB11643-1999标准
        }

        private bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }

            return true;//符合15位身份证标准
        }
        #endregion//////////////////////////////////////////////////////////////////////////by 蔡天雨

        /// <summary>
        /// 获取修改操作描述
        /// </summary>
        /// <returns></returns>
        private string GetModifyDescription(out bool isModify)
        {
            isModify = false;
            StringBuilder description = new StringBuilder(string.Format("姓名：{0}，工号：{1}；\r\n", _lastPersonInfo.person_name,_lastPersonInfo.work_sn));
            if (_lastPersonInfo.person_name != PersonName)
            {
                isModify = true;
                description.Append(string.Format("姓名：{0}->{1}，", _lastPersonInfo.person_name, PersonName));
            }
            if (DepartNames_SelectedIndex > -1 && _lastPersonInfo.depart_id != departIDs[DepartNames_SelectedIndex])
            {
                isModify = true;
                description.Append(string.Format("所属部门：{0}->{1}，", _lastPersonInfo.depart_name, DepartNames[DepartNames_SelectedIndex]));
            }
            if (_lastPersonInfo.work_sn != WorkSn)
            {
                isModify = true;
                description.Append(string.Format("工号：{0}->{1}，", _lastPersonInfo.work_sn, WorkSn));
            }
            //井下班制
            if (ClassTypeNames_SelectedIndex>-1 && _lastPersonInfo.class_type_id != classTypeIDs[ClassTypeNames_SelectedIndex])
            {
                isModify = true;
                description.Append(string.Format("井下班制：{0}->{1}，", _lastPersonInfo.class_type_name, ClassTypeNamesAll[ClassTypeNames_SelectedIndex]));
            }
            //地面班制
            if (ClassTypeNames_SelectedIndex_OnGround  > -1 &&
                _lastPersonInfo.class_type_id_on_ground != classTypeIDs[ClassTypeNames_SelectedIndex_OnGround])
            {
                isModify = true;
                string objectName = VmLogin.GetIsMineApplication() ? "地面班制" : "所在班制";
                description.Append(string.Format(objectName + "：{0}->{1}，", _lastPersonInfo.class_type_name_on_ground, ClassTypeNamesAll[ClassTypeNames_SelectedIndex_OnGround]));
            }
            //职务
            if (_lastPersonInfo.principal_id != SelectedPrincipalInfo.principal_id)
            {
                isModify = true;
                description.Append(string.Format("职务：{0}->{1}，", _lastPersonInfo.principal_name, SelectedPrincipalInfo.principal_name));
            }
            //工种
            if (_lastPersonInfo.work_type_id != SelectedWorkTypeInfo.work_type_id)
            {
                isModify = true;
                description.Append(string.Format("工种：{0}->{1}，", _lastPersonInfo.work_type_name, SelectedWorkTypeInfo.work_type_name));
            }
            if (_lastPersonInfo.phone != Phone)
            {
                isModify = true;
                description.Append(string.Format("电话：{0}->{1}，", _lastPersonInfo.phone, Phone));
            }
            if (_lastPersonInfo.zipcode != ZipCode)
            {
                isModify = true;
                description.Append(string.Format("邮政编码：{0}->{1}，", _lastPersonInfo.zipcode, ZipCode));
            }
            if (_lastPersonInfo.id_card != IDCardNumber)
            {
                isModify = true;
                description.Append(string.Format("身份证号码：{0}->{1}，", _lastPersonInfo.id_card, IDCardNumber));
            }
            
            //出生日期
            object lastBirthDate = _lastPersonInfo.birthdate;
            if (_lastPersonInfo.birthdate.Equals(DateTime.MinValue))
            {
                lastBirthDate = null;
            }

            if (!DateTime.Equals(lastBirthDate, BirthDate))
            {
                isModify = true;
                description.Append(string.Format("出生日期：{0}->{1}，", lastBirthDate, BirthDate));
            }
            //参加工作日期
            //出生日期
            object lastWorkday = _lastPersonInfo.workday;
            if (_lastPersonInfo.workday.Equals(DateTime.MinValue))
            {
                lastWorkday = null;
            }
            if (!DateTime.Equals(lastWorkday, WorkDate))
            {
                isModify = true;
                description.Append(string.Format("参加工作日期：{0}->{1}，", lastWorkday, WorkDate));
            }
            //性别
            string sex = Sex == 0 ? "男" : "女";
            if (_lastPersonInfo.sex != sex)
            {
                isModify = true;
                description.Append(string.Format("性别：{0}->{1}，", _lastPersonInfo.sex, sex));
            }
            //血型
            IrisConverter converter = new BloodConverter();
            string blood_type = converter.IndexToString(BloodType);
       
            if (_lastPersonInfo.blood_type != blood_type)
            {
                isModify = true;
                description.Append(string.Format("血型：{0}->{1}，", _lastPersonInfo.blood_type, blood_type));
            }
            if (_lastPersonInfo.email != Email)
            {
                isModify = true;
                description.Append(string.Format("电子邮箱：{0}->{1}，", _lastPersonInfo.email, Email));
            }
            if (_lastPersonInfo.address != Address)
            {
                isModify = true;
                description.Append(string.Format("地址：{0}->{1}，", _lastPersonInfo.address, Address));
            }
            //照片
            if (_lastPersonInfo.image != imgData)
            {
                isModify = true;
                description.Append("修改了照片，");
            }
            if (_lastPersonInfo.memo != this.TagInfo)
            {
                isModify = true;
                description.Append(string.Format("备注：{0}->{1}，", _lastPersonInfo.memo, TagInfo));
            }
            description.Remove(description.Length - 1, 1);
            description.Append("；\r\n");

            //
            return description.ToString();
        }

        #endregion

        #region ria连接后台操作
       
        /// <summary>
        /// 获取部门名称列表
        /// riaOperateCallBack为异步操作完成后的回调函数
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetDepartNames(Action riaOperateCallBack)
        {
            try
            {

                EntityQuery<UserDepartInfo> list = serviceDomDbAccess.GetDepartsInfoQuery();
                ///回调异常类
                Action<LoadOperation<UserDepartInfo>> actionCallBack = ErrorHandle<UserDepartInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserDepartInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    DepartNames = new List<string>();
                    departIDs = new List<int>();
                    bool hasValue = false;
                  
                    //异步获取数据
                    foreach (UserDepartInfo ar in lo.Entities)
                    {
                        //权限过滤
                        if (VmDepartMng.OperateDepartIdCollection.Contains(ar.depart_id))
                        {
                            DepartNames.Add(ar.depart_name);
                            departIDs.Add(ar.depart_id);
                            hasValue = true;
                        }
                    }

                    this.DepartNames_SelectedIndex = 0;
                    if (hasValue)
                    {
                        this.DepartNames_SelectedIndex = departIDs.IndexOf(_targetDepartId);
                        this.DepartNames_SelectedIndex = this.DepartNames_SelectedIndex < 0 ? 0 : this.DepartNames_SelectedIndex;
                    }
                    if (riaOperateCallBack != null)
                    {
                        riaOperateCallBack();
                    }
                    
                    
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        
        
        /// <summary>
        /// 获取班次类型列表
        /// riaOperateCallBack为异步操作完成后的回调函数
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetClassTypeNames(Action riaOperateCallBack)
        {
            try
            {

                EntityQuery<UserClassTypeInfo> list = serviceDomDbAccess.GetClassTypeInfosQuery();
                ///回调异常类
                Action<LoadOperation<UserClassTypeInfo>> actionCallBack = ErrorHandle<UserClassTypeInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<UserClassTypeInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    ClassTypeNamesAll = new List<string>();
                   
                    classTypeIDs = new List<int>();
                   
                    bool hasValue = false;
                    //异步获取数据
                    foreach (UserClassTypeInfo ar in lo.Entities)
                    {
                        ClassTypeNamesAll.Add(ar.class_type_name);
                        classTypeIDs.Add(ar.class_type_id);
                        hasValue = true;
                    }
                    if (hasValue)
                    {
                        if (VmLogin.GetIsMineApplication())
                        {
                            this.ClassTypeNames_SelectedIndex_OnGround = 0;
                            this.ClassTypeNames_SelectedIndex = 0;
                        }
                        else
                        {
                            this.ClassTypeNames_SelectedIndex_OnGround = 0;
                            this.ClassTypeNames_SelectedIndex = -1;
                        }
                    }

                    if (riaOperateCallBack != null)
                    {
                        riaOperateCallBack();
                    }
                    
                  
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 获取职务列表
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetPricipalsRia(Action riaOperateCallBack)
        {
            try
            {

                EntityQuery<PrincipalInfo> list = serviceDomDbAccess.GetPrincipalsQuery();
                ///回调异常类
                Action<LoadOperation<PrincipalInfo>> actionCallBack = ErrorHandle<PrincipalInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<PrincipalInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
               
                lo.Completed += delegate
                {
                    this.PrincipalInfos = new List<PrincipalInfo>();
                    PrincipalInfo nonePrincipalInfo = new PrincipalInfo()
                    {
                        principal_name = "无",
                        principal_id = 0,
                    };
                    PrincipalInfos.Add(nonePrincipalInfo);

                    //异步获取数据
                    foreach (PrincipalInfo ar in lo.Entities)
                    {
                        PrincipalInfos.Add(ar);
                    }

                    this.SelectedPrincipalInfo = PrincipalInfos[0];

                    if (riaOperateCallBack != null)
                    {
                        riaOperateCallBack();
                    }


                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 获取工种列表
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GetWorkTypesRia(Action riaOperateCallBack)
        {
            try
            {

                EntityQuery<WorkTypeInfo> list = serviceDomDbAccess.GetWorkTypesQuery();
                ///回调异常类
                Action<LoadOperation<WorkTypeInfo>> actionCallBack = ErrorHandle<WorkTypeInfo>.OnLoadErrorCallBack;
                ///异步事件
                LoadOperation<WorkTypeInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);

                lo.Completed += delegate
                {
                    this.WorkTypeInfos = new List<WorkTypeInfo>();
                    WorkTypeInfo noneWorkTypeInfo = new WorkTypeInfo()
                    {
                        work_type_name = "无",
                        work_type_id = 0,
                    };
                    WorkTypeInfos.Add(noneWorkTypeInfo);

                    //异步获取数据
                    foreach (WorkTypeInfo ar in lo.Entities)
                    {
                        WorkTypeInfos.Add(ar);
                    }

                    this.SelectedWorkTypeInfo = WorkTypeInfos[0];


                    if (riaOperateCallBack != null)
                    {
                        riaOperateCallBack();
                    }


                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        UserPersonInfo _lastPersonInfo = null;
        /// <summary>
        /// 获取人员详细信息
        /// </summary>
        /// <param name="riaOperateCallBack">异步操作完成后的回调函数</param>
        private void GePersonDetailInfoRia(Action riaOperateCallBack)
        {
            try
            {
               
                EntityQuery<UserPersonInfo> list = serviceDomDbAccess.GetPersonDetailInfoQuery(personID);
                ///回调异常类
                Action<LoadOperation<UserPersonInfo>> actionCallBack = new Action<LoadOperation<UserPersonInfo>>(ErrorHandle<UserPersonInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<UserPersonInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {

                    foreach (UserPersonInfo item in lo.Entities)
                    {
                        _lastPersonInfo = item;
                        this.DepartNames_SelectedIndex = GetDepartIndex(item.depart_name);
                        this.PersonName = item.person_name;
                        this.EnrollInfo = item.iris_register;
                        this.WorkSn = item.work_sn;

                        this.Sex = GetSexIndex(item.sex);
                        BloodConverter converter = new BloodConverter();
                        this.BloodType = converter.StingToIndex(item.blood_type);
                        if (item.birthdate != DateTime.MinValue)
                        {
                            this.BirthDate = item.birthdate;
                        }
                        if (item.workday != DateTime.MinValue)
                        {
                            this.WorkDate = item.workday;
                        }

                        this.ClassTypeNames_SelectedIndex_OnGround = GetClassTypeIndex(item.class_type_name_on_ground);

                        this.ClassTypeNames_SelectedIndex = GetClassTypeIndex(item.class_type_name);
                        this.Phone = item.phone;
                        this.TagInfo = item.memo;
                        this.ZipCode = item.zipcode;
                        this.IDCardNumber = item.id_card;
                        this.Address = item.address;
                        this.Email = item.email;
                        this.PersonImg = GetImg(item.image);
                        this.imgData = item.image;
                        this.imgType = item.img_type;

                        try
                        {
                            this.SelectedPrincipalInfo = this.PrincipalInfos.Single((p) => p.principal_id == item.principal_id);
                        }
                        catch (Exception)
                        {
                            this.SelectedPrincipalInfo = PrincipalInfos[0];
                        }

                        try
                        {
                            this.SelectedWorkTypeInfo = this.WorkTypeInfos.Single((w) => w.work_type_id == item.work_type_id);
                        }
                        catch (Exception)
                        {
                            this.SelectedWorkTypeInfo = WorkTypeInfos[0];
                        }
                      

                        break;
                    }

                    if (riaOperateCallBack != null)
                    {
                        riaOperateCallBack();
                    }
       
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        

        //删除人员
        private void DeleteRia(int personId)
        {
            try
            {
                WaitingDialog.ShowWaiting();
                Action<InvokeOperation<OptionInfo>> callBack = CallBackHandleControl<OptionInfo>.OnInvokeErrorCallBack;
                CallBackHandleControl<OptionInfo>.m_sendValue = (optionInfo) =>
                {
                    if (!optionInfo.isSuccess)
                    {
                        MsgBoxWindow.MsgBox(
                            optionInfo.option_info,
                            MsgBoxWindow.MsgIcon.Error,
                            MsgBoxWindow.MsgBtns.OK);
                    }
                    else
                    {
                        if (!optionInfo.isNotifySuccess)
                        {
                            MsgBoxWindow.MsgBox(
                                 optionInfo.option_info,
                                 MsgBoxWindow.MsgIcon.Warning,
                                 MsgBoxWindow.MsgBtns.OK);
                        }
                        else
                        {
                            MsgBoxWindow.MsgBox(
                                "操作成功，共有1个人被删除",
                                MsgBoxWindow.MsgIcon.Succeed,
                                MsgBoxWindow.MsgBtns.OK);

                        }
                        if (CloseEvent != null)
                        {
                            CloseEvent(true);
                        }
                     
                    }
                    WaitingDialog.HideWaiting();
                };
                serviceDomDbAccess.BatchDeletePerson(new int[] { personID }, callBack, null);
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        //修改人员信息
        private void ModifyRia()
        {
           

            string worksn = PublicMethods.ToString(WorkSn);
            string name = PublicMethods.ToString(PersonName);
            string sex = PublicMethods.ToString(Sex);

            BloodConverter converTor = new BloodConverter();

            string blood_type = converTor.IndexToString(BloodType);
            blood_type = "'" + blood_type + "'";

            string birthdate = PublicMethods.ToString(BirthDate);

            string workday = PublicMethods.ToString(WorkDate);

            string id_card = PublicMethods.ToString(IDCardNumber);
            string phone = PublicMethods.ToString(Phone);
            string address = PublicMethods.ToString(Address);
            string zipcode = PublicMethods.ToString(ZipCode);
            string email = PublicMethods.ToString(Email);
            string memo = PublicMethods.ToString(TagInfo);
            string depart_id = "null";
            if (departIDs.Count > DepartNames_SelectedIndex && DepartNames_SelectedIndex >= 0)
            {
                depart_id = PublicMethods.ToString(departIDs[DepartNames_SelectedIndex]);
            }
            string class_type_id_on_ground = "null";
            if (ClassTypeNamesAll.Count > ClassTypeNames_SelectedIndex_OnGround && ClassTypeNames_SelectedIndex_OnGround >= 0)
            {
                class_type_id_on_ground = PublicMethods.ToString(classTypeIDs[ClassTypeNames_SelectedIndex_OnGround]);
            }
            string class_type_id = "null";
            if (ClassTypeNamesAll.Count > ClassTypeNames_SelectedIndex && ClassTypeNames_SelectedIndex >= 0)
            {
                class_type_id = PublicMethods.ToString(classTypeIDs[ClassTypeNames_SelectedIndex]);
            }

            string principalId = this.SelectedPrincipalInfo.principal_id > 0 ?
             this.SelectedPrincipalInfo.principal_id.ToString() : "null";

            string workTypeId = this.SelectedWorkTypeInfo.work_type_id > 0 ?
               this.SelectedWorkTypeInfo.work_type_id.ToString() : "null";

            bool isModify = false;
            string description = GetModifyDescription(out isModify);
            if (!isModify) //没做修改操作则直接返回
            {
                if (CloseEvent != null)
                {
                    CloseEvent(true);
                }
                return;
            }

            try
            {
                WaitingDialog.ShowWaiting();
                 EntityQuery<OptionInfo> list = serviceDomDbAccess.UpdatePersonInfoOnMineQuery(personID, depart_id,
                    class_type_id_on_ground, class_type_id ,worksn, name, sex, blood_type, birthdate, workday, id_card, phone,
                    address, zipcode, email, memo, this.imgData, imgType,
                    principalId, workTypeId);
                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                        {
                            if (CloseEvent != null)
                            {
                                CloseEvent(true);
                            }
                        };

                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                                item.option_info,
                                MsgBoxWindow.MsgIcon.Error,
                                MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0, "修改人员", description + item.option_info, null);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                    item.option_info,
                                    MsgBoxWindow.MsgIcon.Warning,
                                    MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "修改人员", description + item.option_info, completeCallBack);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                   item.option_info,
                                   MsgBoxWindow.MsgIcon.Succeed,
                                   MsgBoxWindow.MsgBtns.OK);
                                VmOperatorLog.InsertOperatorLog(1, "修改人员", description , completeCallBack);
                            }
                          
                        }

                        break;
                    }
                  
                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

        /// <summary>
        /// 添加新员工
        /// </summary>
        /// <param name="isCloseWnd">添加完成后是否关闭窗口</param>
        private void AddPersonRia(bool isCloseWnd)
        {
          


            string description = string.Format("姓名：{0}，工号：{1}\r\n", PersonName, WorkSn);

            string worksn = PublicMethods.ToString(WorkSn);
            string name = PublicMethods.ToString(PersonName);
            string sex = PublicMethods.ToString(Sex);
            IrisConverter converter = new BloodConverter();
            string blood_type = converter.IndexToString(BloodType);
           
            blood_type = "'" + blood_type + "'";

            string birthdate = PublicMethods.ToString(BirthDate);
            string workday = PublicMethods.ToString(WorkDate);
            string id_card = PublicMethods.ToString(IDCardNumber);
            string phone = PublicMethods.ToString(Phone);
            string address = PublicMethods.ToString(Address);
            string zipcode = PublicMethods.ToString(ZipCode);
            string email = PublicMethods.ToString(Email);
            string memo = PublicMethods.ToString(TagInfo);
            string depart_id = "null";
            if (departIDs.Count > DepartNames_SelectedIndex && DepartNames_SelectedIndex >= 0)
            {
                depart_id = PublicMethods.ToString(departIDs[DepartNames_SelectedIndex]);
            }
           
            string class_type_id_on_ground = "null";
            if (ClassTypeNamesAll.Count > ClassTypeNames_SelectedIndex_OnGround && ClassTypeNames_SelectedIndex_OnGround >= 0)
            {
                class_type_id_on_ground = PublicMethods.ToString(classTypeIDs[ClassTypeNames_SelectedIndex_OnGround]);
            }
            string class_type_id = "null";
            if (ClassTypeNamesAll.Count > ClassTypeNames_SelectedIndex && ClassTypeNames_SelectedIndex >= 0)
            {
                class_type_id = PublicMethods.ToString(classTypeIDs[ClassTypeNames_SelectedIndex]);
            }

            string principalId = this.SelectedPrincipalInfo.principal_id > 0 ?
                this.SelectedPrincipalInfo.principal_id.ToString() : "null";

            string workTypeId = this.SelectedWorkTypeInfo.work_type_id > 0 ?
               this.SelectedWorkTypeInfo.work_type_id.ToString() : "null";
            

            try
            {
                _isContinueAdd = !isCloseWnd;
                WaitingDialog.ShowWaiting();
                EntityQuery<OptionInfo> list = serviceDomDbAccess.AddPersonOnMineQuery(depart_id,
                    class_type_id_on_ground, class_type_id, worksn, name, sex, blood_type, birthdate,
                    workday, id_card, phone,
                    address, zipcode, email, memo, this.imgData, imgType,
                    principalId, workTypeId);
                ///回调异常类
                Action<LoadOperation<OptionInfo>> actionCallBack = new Action<LoadOperation<OptionInfo>>(ErrorHandle<OptionInfo>.OnLoadErrorCallBack);
                ///异步事件
                LoadOperation<OptionInfo> lo = this.serviceDomDbAccess.Load(list, actionCallBack, null);
                lo.Completed += delegate
                {
                    VmOperatorLog.CompleteCallBack completeCallBack = () =>
                        {
                            if (isCloseWnd && CloseEvent != null)
                            {
                                CloseEvent(true);
                            }
                        };

                    foreach (OptionInfo item in lo.Entities)
                    {
                        if (!item.isSuccess)
                        {
                            MsgBoxWindow.MsgBox(
                                item.option_info,
                                MsgBoxWindow.MsgIcon.Error,
                                MsgBoxWindow.MsgBtns.OK);
                            VmOperatorLog.InsertOperatorLog(0, "添加人员", description + item.option_info, completeCallBack);
                        }
                        else
                        {
                            if (!item.isNotifySuccess)
                            {
                                MsgBoxWindow.MsgBox(
                                    item.option_info,
                                    MsgBoxWindow.MsgIcon.Warning,
                                    MsgBoxWindow.MsgBtns.OK);
                            }
                            else
                            {
                                MsgBoxWindow.MsgBox(
                                   item.option_info,
                                   MsgBoxWindow.MsgIcon.Succeed,
                                   MsgBoxWindow.MsgBtns.OK);
                            }
                            VmOperatorLog.InsertOperatorLog(1, "添加人员", description , completeCallBack);
                        }
                        RefreshContent();
                        break;
             
                    }
                   
                  
                    WaitingDialog.HideWaiting();
                };
            }
            catch (Exception e)
            {
                WaitingDialog.HideWaiting();
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }

      
        #endregion

    }

 

}
