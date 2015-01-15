/*************************************************************************
** 文件名:   Page_peopleMng.cs
×× 主要类:   Page_peopleMng
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-6-14
** 修改人:   gqy
** 日  期:   2014-03-25 修改描述：删除部门树右键点击的操作，添加所有按钮的权限 
** 描  述:   Page_peopleMng类，人员信息查询页面
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
using IriskingAttend.ViewModel.PeopleViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using IriskingAttend.ExportExcel;
using System.Windows.Controls.Primitives;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Collections;
using CustomCursor;
using IriskingAttend.CustomCursor;
using IriskingAttend.Dialog;
using System.Windows.Data;
using IriskingAttend.ApplicationType;

namespace IriskingAttend.View.PeopleView
{
    public partial class PageDepartAndPeopleMng : Page
    {
        #region 字段声明

        private DragState _dragState = new DragState();  //拖拽状态
       
        //缓存拖拽人员
        private List<UserPersonInfo> _readyToDragPersons = new List<UserPersonInfo>();
        //缓存拖拽部门
        private TreeNode<UserDepartInfo> _readyToDragDeparts = null;
        
        //要改变鼠标样式的空间集合
        private List<FrameworkElement> _mouseChangeableUI = new List<FrameworkElement>();
       
        //datagrid row的鼠标左键点下委托
        private MouseButtonEventHandler _mouseLeftButtonDownEventHandler;

        //自定义鼠标图案
        private string _personCursorUri = "/IriskingAttend;component/Images/DragPersons.png";
        private string _departCursorUri = "/IriskingAttend;component/Images/DragDeparts.png";
        private MovableMouse _personCursor = null;
        private MovableMouse _departCursor = null;


        private double _vScrollOffset = -1;     //拖动栏的竖直偏移

        //container for mouseAnimation
        Canvas LayOutCanvas = null;
        #endregion

        #region 构造函数以及初始化
        
        public PageDepartAndPeopleMng()
        {
            InitializeComponent();

            if (AppTypePublic.GetCustomAppType().GetType().ToString().CompareTo("IriskingAttend.ApplicationType.ShenShuoRailwayApp") == 0)
            {
                btnBatchAddAttendLeave.Visibility = Visibility.Visible;
            }

            //add canvas as container for mouseAnimation
            // <Canvas x:Name="LayOutCanvas" Grid.ColumnSpan="10" Grid.RowSpan="10" />
            LayOutCanvas = new Canvas();
            LayOutCanvas.Name = "LayOutCanvas";
            Grid.SetRowSpan(LayOutCanvas, 10);
            Grid.SetColumnSpan(LayOutCanvas, 10);
            this.LayoutRoot.Children.Add(LayOutCanvas);
            
            Init();

            //added begin by gqy at 2014-03-25 权限            
            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonQuery) && !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonQuery])
            {
                this.btnQuery.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonAddDepart) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonAddDepart])
            {
                this.btnAddDepart.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonDeleteDepart) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonDeleteDepart])
            {
                this.btnDelDepart.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonModifyDepart) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonModifyDepart])
            {
                this.btnModifyDepart.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonBatchAddRecord) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonBatchAddRecord])
            {
                this.btnBatchAddRecord.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonBatchModifyPerson) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonBatchModifyPerson])
            {
                this.btnBatchModifyPerson.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonBatchStopIris) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonBatchStopIris])
            {
                this.btnBatchStopIris.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonBatchDeletePerson) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonBatchDeletePerson])
            {
                this.btnBatchDeletePerson.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonAddPerson) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonAddPerson])
            {
                this.btnAddPerson.Visibility = Visibility.Collapsed;
            }

            if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonExportExcel) &&!VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonExportExcel])
            {
                this.btnExportExl.Visibility = Visibility.Collapsed;
            }
            
            //added end by gqy at 2014-03-25
          
        }
        
        /// <summary>
        /// 内容初始化
        /// </summary>
        private void Init()
        {
            _personCursor = new MovableMouse(_personCursorUri);
            _departCursor = new MovableMouse(_departCursorUri);

            //注册全界面的鼠标左键抬起事件
            this.LayoutRoot.AddHandler(Grid.MouseLeftButtonUpEvent, new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp), true);

            //人员列表区域初始化鼠标左键点击委托
            _mouseLeftButtonDownEventHandler = new MouseButtonEventHandler(dataGridPersonRow_MouseLeftButtonDownEx);

            //首先要区分矿山和非矿模式的界面显示
            SetOnMineValue(VmLogin.GetIsMineApplication());

            //添加需要改变鼠标样式的控件
            _mouseChangeableUI.Add(this.LayoutRoot);
            _mouseChangeableUI.Add(this.gridSplitter);
            _mouseChangeableUI.Add(this.dataGridPerson);
            _mouseChangeableUI.Add(this.departTree);

            //初始化vm
            var vmDepartAndPeopleMng = new VmDepartAndPeopleMng();
            vmDepartAndPeopleMng.MarkObj = this.Resources["MarkObject"] as MarkObject;

            //vm加载完毕事件
            vmDepartAndPeopleMng.LoadCompletedEvent += (sender, ergs) =>
            {
                this.DataContext = sender;

                //解决初始化时 部门树的 scrollbar遮挡问题
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.LayoutRoot.ColumnDefinitions[0].Width =
                        new System.Windows.GridLength(this.LayoutRoot.ColumnDefinitions[0].Width.Value + 1);

                    //手动绑定树节点
                    PublicMethods.BindingAllTree(departTree);
                });
            };

            //scrollViewer改变事件
            vmDepartAndPeopleMng.ScrollBarVerticalOffsetLayout = () =>
            {
                if (_vScrollOffset > 0f)
                {
                    departTree.GetScrollHost().ScrollToVerticalOffset(_vScrollOffset);
                    _vScrollOffset = -1;
                }
            };


            //屏幕默认的右键
            this.MouseRightButtonDown += new MouseButtonEventHandler((sender, e) =>
            {
                e.Handled = true;
            });

            //监听鼠标是否在部门树控件中
            _moniterMouseOnDepartTimer.Interval = TimeSpan.FromSeconds(0.1);
            _moniterMouseOnDepartTimer.Tick += new EventHandler(_moniterMouseOnDepartTimer_Tick);

            //设置权限  added by gqy at 2014-03-25
            dataGridPerson.LoadingRow += (a, e) =>
            {
                var cell = dataGridPerson.Columns[11].GetCellContent(e.Row) as StackPanel;

                HyperlinkButton hlbtnModify = cell.FindName("btnModifyPersonInfo") as HyperlinkButton;
                if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonModifyPerson) && !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonModifyPerson])
                {
                    hlbtnModify.Visibility = Visibility.Collapsed;
                }
                hlbtnModify = cell.FindName("btnDeletePersonInfo") as HyperlinkButton;
                if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonDeletePerson) && !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonDeletePerson])
                {
                    hlbtnModify.Visibility = Visibility.Collapsed;
                }
                hlbtnModify = cell.FindName("btnRecordPersonInfo") as HyperlinkButton;
                if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonIdentifyRecord) && !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonIdentifyRecord])
                {
                    hlbtnModify.Visibility = Visibility.Collapsed;
                }
                hlbtnModify = cell.FindName("btnStopIrisPersonInfo") as HyperlinkButton;
                if (VmLogin.DictPrivilege.ContainsKey(AbstractApp.PrivilegeENUM.PersonStopIris) && !VmLogin.DictPrivilege[AbstractApp.PrivilegeENUM.PersonStopIris])
                {
                    hlbtnModify.Visibility = Visibility.Collapsed;
                }

                //添加行号
                int index = e.Row.GetIndex();
                var cellRow = dataGridPerson.Columns[1].GetCellContent(e.Row) as TextBlock;
                cellRow.Text = (index + 1).ToString();
                ((UserPersonInfo)e.Row.DataContext).index = index;
                
            };
          
        }

        /// <summary>
        /// 离开该页面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //如果正在批量添加识别记录则停止该操作
            ((VmDepartAndPeopleMng)this.DataContext).CancelBatchAddRecord();
            base.OnNavigatedFrom(e);
        }

        #endregion

        #region 选择人员列表事件
        
        //全选人员操作
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                ((VmDepartAndPeopleMng)this.DataContext).SelectAllPerson(((CheckBox)sender).IsChecked);
            }
        }

        //注册每行的鼠标事件
        private void dataGridPerson_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //注册鼠标左键抬起事件
            e.Row.MouseLeftButtonUp -= new MouseButtonEventHandler(Row_MouseLeftButtonUp);
            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(Row_MouseLeftButtonUp);

            //注册鼠标左键按下事件，enhence模式，路由事件为true也要触发该事件。
            e.Row.RemoveHandler(DataGrid.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler);
            e.Row.AddHandler(DataGrid.MouseLeftButtonDownEvent, _mouseLeftButtonDownEventHandler, true);
          
        }

        //点击行 选择item操作
        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.DataContext != null)
            {
                if (dataGridPerson.CurrentColumn != null && dataGridPerson.CurrentColumn.DisplayIndex == 0)
                {
                    if (dataGridPerson.SelectedItem != null)
                    {
                        var obj = dataGridPerson.SelectedItem as UserPersonInfo;
                        if (obj != null)
                        {
                            ((VmDepartAndPeopleMng)this.DataContext).SelectItems(obj);
                        }
                    }
                }
            }
        }

        #endregion

        #region  按钮的响应


        //修改按钮 by cty
        private void btnModifyPersonInfo_Click(object sender, RoutedEventArgs e)
        {
            UserPersonInfo personInfo = (((HyperlinkButton)sender).DataContext as UserPersonInfo);
            ((VmDepartAndPeopleMng)this.DataContext).ModifyPerson(personInfo);
        }

        //删除按钮 by cty
        private void btnDeletePersonInfo_Click(object sender, RoutedEventArgs e)
        {
            UserPersonInfo personInfo = (((HyperlinkButton)sender).DataContext as UserPersonInfo);
            ((VmDepartAndPeopleMng)this.DataContext).DeletePerson(personInfo);
        }

        //查看识别记录按钮 by cty
        private void btnRecordPersonInfo_Click(object sender, RoutedEventArgs e)
        {
            UserPersonInfo pInfo = (((HyperlinkButton)sender).DataContext as UserPersonInfo);
            ((VmDepartAndPeopleMng)this.DataContext).Record(pInfo);
        }

        //停用虹膜按钮 by cty
        private void btnStopIrisPersonInfo_Click(object sender, RoutedEventArgs e)
        {
            UserPersonInfo personInfo = (((HyperlinkButton)sender).DataContext as UserPersonInfo);
            ((VmDepartAndPeopleMng)this.DataContext).StopIris(personInfo);
        }

        //导出excel
        private void btnExportExl_Click(object sender, RoutedEventArgs e)
        {
            if (VmLogin.GetIsMineApplication())
            {
                string space = "                                                  ";
                ExpExcel.ExportExcelFromDataGrid(this.dataGridPerson, 2, 11, (space + "部门人员管理" + space), "部门人员管理", 9, 3000);
            }
            else
            {
                string space = "                                             ";
                ExpExcel.ExportExcelFromDataGrid(this.dataGridPerson, 2, 10, (space + "部门人员管理" + space), "部门人员管理", 9, 3000);
            }
        }

        #endregion

        #region 排序 


        //显示排序箭头
        private void dataGridPerson_LayoutUpdated(object sender, EventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //显示排序箭头
        private void dataGridPerson_MouseMove(object sender, MouseEventArgs e)
        {
            MyDataGridSortInChinese.SetColumnSortState();
        }

        //排序
        private void dataGridPerson_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MyDataGridSortInChinese.OrderData(sender, e, ((VmDepartAndPeopleMng)this.DataContext).UserPersonInfos);
        }
        #endregion

        #region 部门右键菜单

        ////屏蔽默认的右键菜单
        //private void Grid_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    e.Handled = true;
        //}

        ////部门树右键点击事件
        //private void departItemStackPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    TreeNode<UserDepartInfo> item = ((StackPanel)sender).DataContext as TreeNode<UserDepartInfo>;
        //    item.IsChecked = true;
        //}

        ////加载部门树右键菜单
        //private void departItemStackPanel_Loaded(object sender, RoutedEventArgs e)
        //{
        //    _mouseChangeableUI.Add((StackPanel)sender);

        //    #region  网页上自动下载的是silverlight4 不是5 暂时不支持右键菜单的功能
        //    ContextMenu contextMenu = new ContextMenu();//新建右键菜单
        //    contextMenu.Tag = sender;
        //    ContextMenuService.SetContextMenu((StackPanel)sender, contextMenu);//为控件绑定右键菜单
        //    InitContextMenu(contextMenu);
        //    #endregion
        //}

        ////初始化右键菜单
        //private void InitContextMenu(ContextMenu contextMenu)
        //{
        //    MenuItem menuItemModify = new MenuItem();//新建右键菜单项
        //    menuItemModify.Header = "修改";
        //    menuItemModify.Icon = new Image()
        //    {
        //        Source = new BitmapImage(new Uri("/IriskingAttend;component/Images/tools.png", UriKind.Relative)),
        //        Width = 20,
        //        Height = 20
        //    };
        //    menuItemModify.Click += new RoutedEventHandler(menuItemModify_Click);

        //    MenuItem menuItemDelete = new MenuItem();//新建右键菜单项
        //    menuItemDelete.Header = "删除";
        //    menuItemDelete.Click += new RoutedEventHandler(menuItemDelete_Click);
        //    menuItemDelete.Icon = new Image()
        //    {
        //        Source = new BitmapImage(new Uri("/IriskingAttend;component/Images/Del.png", UriKind.Relative)),
        //        Width = 20,
        //        Height = 20
        //    };

        //    MenuItem menuItemAdd = new MenuItem();//新建右键菜单项
        //    menuItemAdd.Header = "添加";
        //    menuItemAdd.Click += new RoutedEventHandler(menuItemAdd_Click);
        //    menuItemAdd.Icon = new Image()
        //    {
        //        Source = new BitmapImage(new Uri("/IriskingAttend;component/Images/Add.png", UriKind.Relative)),
        //        Width = 20,
        //        Height = 20
        //    };


        //    ((ContextMenu)contextMenu).Items.Clear();
        //    TreeNode<UserDepartInfo> treeNode = ((StackPanel)(contextMenu).Tag).DataContext as TreeNode<UserDepartInfo>;
        //    if (treeNode.NodeName.Equals("全部"))
        //    {
        //        ((ContextMenu)contextMenu).Items.Add(menuItemAdd);
        //    }
        //    else
        //    {
        //        ((ContextMenu)contextMenu).Items.Add(menuItemAdd);
        //        ((ContextMenu)contextMenu).Items.Add(menuItemDelete);
        //        ((ContextMenu)contextMenu).Items.Add(menuItemModify);
        //    }

        //    this.contextMenu.Visibility = System.Windows.Visibility.Collapsed;
        //}

        //添加部门
        private void menuItemAdd_Click(object sender, RoutedEventArgs e)
        {
            TreeNode<UserDepartInfo> departNode = this.departTree.SelectedItem as TreeNode<UserDepartInfo>;
            if (departNode != null)
            {
                ((VmDepartAndPeopleMng)this.DataContext).AddDepart(departNode.NodeValue);
                _vScrollOffset = departTree.GetScrollHost().VerticalOffset;
            }
            else
            {
                MsgBoxWindow.MsgBox("请选中某部门再执行添加操作！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
        }

        //删除部门
        private void menuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            TreeNode<UserDepartInfo> departNode = this.departTree.SelectedItem as TreeNode<UserDepartInfo>;
            if (departNode != null && departNode.NodeValue != null && departNode.NodeValue.depart_id > 0)
            {
                ((VmDepartAndPeopleMng)this.DataContext).DeleteDepart(departNode.NodeValue);
                _vScrollOffset = departTree.GetScrollHost().VerticalOffset;

            }
            else if (departNode == null)
            {
                MsgBoxWindow.MsgBox("请选中某部门再执行删除操作！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
            else
            {
                MsgBoxWindow.MsgBox("根节点部门不能被删除！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
        }

        //修改部门
        private void menuItemModify_Click(object sender, RoutedEventArgs e)
        {
            TreeNode<UserDepartInfo> departNode = this.departTree.SelectedItem as TreeNode<UserDepartInfo>;
            if (departNode != null && departNode.NodeValue != null && departNode.NodeValue.depart_id > 0)
            {
                ((VmDepartAndPeopleMng)this.DataContext).ModifyDepart(departNode.NodeValue);
                _vScrollOffset = departTree.GetScrollHost().VerticalOffset;

            }
            else if (departNode == null)
            {
                MsgBoxWindow.MsgBox("请选中某部门再执行修改操作！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
            else
            {
                MsgBoxWindow.MsgBox("根节点部门不能被修改！", MsgBoxWindow.MsgIcon.Information, MsgBoxWindow.MsgBtns.OK);
            }
        }

        #endregion
       
        #region 拖动结束相关

        //在drag的状态下，鼠标移出LayoutRoot，当做drag失败或者按键抬起
        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            EndDrag( _dragState.DragCurPos);
        }

        //dataGridPerson失去焦点时，drag结束
        private void dataGridPerson_LostFocus(object sender, RoutedEventArgs e)
        {
            EndDrag( null);
        }

        //在LayoutRoot控件中，鼠标左键抬起时，drag结束
        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
          
            EndDrag( e.GetPosition(this));
        }

        //当前部门改变 或者 拖拽成功完成
        private void departItemStackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //如果既不是拖拽人员，也不是拖拽部门，那么是当前选择部门改变，刷新人员列表
            if (_dragState.DragPossition != DragPossition.Draging &&
                _dragState.DragPossition != DragPossition.AfterDrag)
            {
                UpdateCurDataGridPerson();
            }

            if (_dragState.DragPossition == DragPossition.Draging) //如果拖拽存在
            {
                if (_dragState.DragObject == DragObject.Person) //如果是拖拽人员
                {
                    //如果拖拽目标部门存在,则将人员调离到目标部门中去
                    TreeNode<UserDepartInfo> departInfo = ((StackPanel)sender).DataContext as TreeNode<UserDepartInfo>;
                    if (departInfo != null && departInfo.NodeValue != null && departInfo.NodeValue.depart_id > 0)
                    {
                        ((VmDepartAndPeopleMng)this.DataContext).BatchTransferPersonsRia(_readyToDragPersons, departInfo.NodeValue);
                        _readyToDragPersons.Clear(); //清空拖拽人员
                    }
                }

                if (_dragState.DragObject == DragObject.Depart) //如果是拖拽部门
                {
                    //如果拖拽目标部门存在,则将部门移动到目标部门中去
                    TreeNode<UserDepartInfo> departInfo = ((StackPanel)sender).DataContext as TreeNode<UserDepartInfo>;
                    if (departInfo != null && departInfo.NodeValue != null)
                    {
                        ((VmDepartAndPeopleMng)this.DataContext).TransferDepartRia(_readyToDragDeparts, departInfo.NodeValue);
                        _readyToDragDeparts = null; //清空拖拽部门
                    }
                }
                //结束拖拽
                EndDrag( e.GetPosition(this), true);
            }
        }

        /// <summary>
        /// 结束拖拽，播放拖拽结束动画
        /// </summary>
        /// <param name="dragState">状态</param>
        /// <param name="curMousePos">当前位置</param>
        /// <param name="isDragSucceed">是否拖拽成功</param>
        private void EndDrag(Point? curMousePos,bool isDragSucceed = false)
        {
            //关闭监听
            _moniterMouseOnDepartTimer.Stop();

            //改变鼠标样式
            foreach (var item in _mouseChangeableUI)
            {
                CursorEx.SetCustomCursor(item, null);
            }
            if (_dragState.DragPossition == DragPossition.ReadyToDrag)
            {
                _dragState.DragPossition = DragPossition.NoDrag;
            }

            if (_dragState.DragPossition == DragPossition.Draging)
            {
                string movableMouseUri = _dragState.DragObject == DragObject.Person ? _personCursorUri : _departCursorUri;
                //初始化拖拽结束的动画播放控件
                var mouse = new MovableMouse(movableMouseUri,
                    80, (mouseBack) =>
                    {
                        //拖拽结束动画 回调函数内容
                        this.LayOutCanvas.Children.Remove(mouseBack);
                        mouseBack = null;
                        GC.Collect();
                        //设置鼠标样式
                        foreach (var item in _mouseChangeableUI)
                        {
                            item.Cursor = Cursors.Arrow;
                        }

                        //拖拽动画完成
                        //拖拽人员结束
                        if (_dragState.DragObject == DragObject.Person)
                        {
                            ((VmDepartAndPeopleMng)this.DataContext).RevertDragPersons(_readyToDragPersons);
                            this.dataGridPerson.SelectedIndex--;
                        }
                        //拖拽部门结束
                        if (_dragState.DragObject == DragObject.Depart)
                        {
                            ((VmDepartAndPeopleMng)this.DataContext).RevertDragDeparts(_readyToDragDeparts);
                           
                        }
                        _dragState.DragObject = DragObject.None;
                        _dragState.DragPossition = DragPossition.NoDrag;
                    });
                mouse.VerticalAlignment = VerticalAlignment.Top;
                mouse.HorizontalAlignment = HorizontalAlignment.Left;
                
                LayOutCanvas.Children.Add(mouse);

               
                //拖拽成功和失败分别播放不同的动画
                if (isDragSucceed)
                {
                    ((MovableMouse)mouse).StartMove(curMousePos.Value, curMousePos.Value, 0.2);
                }
                else
                {
                    ((MovableMouse)mouse).StartMove(curMousePos.Value, _dragState.DragStartPos, 0.5);
                }

                //改变鼠标样式
                foreach (var item in _mouseChangeableUI)
                {
                    item.Cursor = Cursors.None;
                }
                _vScrollOffset = departTree.GetScrollHost().VerticalOffset;
                _dragState.DragPossition = DragPossition.AfterDrag;
              
            }
        }

        #endregion

        #region 拖拽准备开始相关
      
        //在dataGridPerson控件中，鼠标左键按下时，准备开始拖拽人员
        private void dataGridPersonRow_MouseLeftButtonDownEx(object sender, MouseButtonEventArgs e)
        {
            
            //是否没有开始拖拽
            if (_dragState.DragPossition != DragPossition.NoDrag)
            {
                return;
            }
            _dragState.DragObject = DragObject.Person;
            _dragState.DragPossition = DragPossition.ReadyToDrag;
            _dragState.DragStartPos = e.GetPosition(this);
            
            //drag开始前，将选中的item缓存起来
            _readyToDragPersons.Clear();
            foreach (var item in this.dataGridPerson.SelectedItems)
            {
                _readyToDragPersons.Add((UserPersonInfo)item);
            }
        }

        //点下当前部门,准备开始拖拽部门
        private void departItemStackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //是否没有开始拖拽
            if (_dragState.DragPossition != DragPossition.NoDrag)
            {
                return;
            }

            _dragState.DragPossition = DragPossition.ReadyToDrag;
            _dragState.DragObject = DragObject.Depart;
            _dragState.DragStartPos = e.GetPosition(this);
            

            //drag开始前，将选中的item缓存起来
            _readyToDragDeparts = ((StackPanel)sender).DataContext as TreeNode<UserDepartInfo>;

        }
 
        #endregion
        
        #region 拖拽开始相关
        private const double _dragScrollAreaHeight = 30;
        private const double _scrollDeltaValue = 10;
        /// <summary>
        /// 是否scroll能自动滚动，需满足两个条件：
        /// 1 部门树的高度大于自动滚动高度的2倍
        /// 2 处于拖拽状态 
        /// </summary>
        private bool isDragAutoScrollEnable
        {
            get
            {
                return departTree.ActualHeight > (2 * _dragScrollAreaHeight) &&
                    _dragState.DragPossition == DragPossition.Draging;
            }
           
        }
        //在LayoutRoot控件中，鼠标按下并且移动一定距离，drag开始
        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            //在拖拽状态下
            if (_dragState.DragPossition == DragPossition.Draging)
            {
                _dragState.DragCurPos = e.GetPosition(this);
                _lastMouseEventArgs = e;
            }

            //开始拖拽
            if (_dragState.DragPossition == DragPossition.ReadyToDrag)
            {
                 Point curPos = e.GetPosition(this);
                 if ((curPos.X - _dragState.DragStartPos.X) * (curPos.X - _dragState.DragStartPos.X) +
                     (curPos.Y - _dragState.DragStartPos.Y) * (curPos.Y - _dragState.DragStartPos.Y) > 10)
                 {

                   
                     //drag开始后，将选中的item从原有的列表中移除
                     if (_dragState.DragObject == DragObject.Depart)
                     {
                         ((VmDepartAndPeopleMng)this.DataContext).RemoveDragDeparts(_readyToDragDeparts);
                         this.departTree.SetSelectedContainer(null);
                         //改变鼠标图案
                         foreach (var item in _mouseChangeableUI)
                         {
                             CursorEx.SetCustomCursor(item, _departCursor);
                         }
                         _vScrollOffset = departTree.GetScrollHost().VerticalOffset;

                     }
                     if (_dragState.DragObject == DragObject.Person)
                     {
                         ((VmDepartAndPeopleMng)this.DataContext).RemoveDragPersons(_readyToDragPersons);
                         this.dataGridPerson.SelectedIndex = -1;
                         //改变鼠标图案
                         foreach (var item in _mouseChangeableUI)
                         {
                             CursorEx.SetCustomCursor(item, _personCursor);
                         }
                     }

                     _dragState.DragPossition = DragPossition.Draging;
                     _moniterMouseOnDepartTimer.Start();
                     
                 }
            }
        }

        #endregion

        #region  拖拽时鼠标处于边缘处，滚动条向上或者向下滚动
        DispatcherTimer _moniterMouseOnDepartTimer = new DispatcherTimer();
        MouseEventArgs _lastMouseEventArgs = null;
        private void _moniterMouseOnDepartTimer_Tick(object sender, EventArgs e)
        {
            if (!isDragAutoScrollEnable)
            {
                return;
            }

            Point mousePos = _lastMouseEventArgs.GetPosition(departTree);
            //鼠标不在部门树里，返回
            if (mousePos.X > departTree.ActualWidth || mousePos.X<0 ||
                mousePos.Y > departTree.ActualHeight || mousePos.Y < 0)
            {
                return;
            }

            double mouseY = mousePos.Y;
            //向上滚动
            if (mouseY < _dragScrollAreaHeight)
            {
                double targetVerticalOffset = departTree.GetScrollHost().VerticalOffset - _scrollDeltaValue;
                targetVerticalOffset = targetVerticalOffset > 0 ? targetVerticalOffset : 0;
                departTree.GetScrollHost().ScrollToVerticalOffset(targetVerticalOffset);
            }
            //向下滚动
            else if (mouseY > departTree.ActualHeight - _dragScrollAreaHeight)
            {
                double targetVerticalOffset = departTree.GetScrollHost().VerticalOffset + _scrollDeltaValue;
                targetVerticalOffset = targetVerticalOffset < departTree.GetScrollHost().ScrollableHeight
                    ? targetVerticalOffset : departTree.GetScrollHost().ScrollableHeight;
                departTree.GetScrollHost().ScrollToVerticalOffset(targetVerticalOffset);
            }
        }



         
     
        #endregion

        #region 功能函数
        
        /// <summary>
        /// 区别矿和非矿模式的界面显示
        /// </summary>
        /// <param name="isOnMine">矿模式标志</param>
        private void SetOnMineValue(bool isOnMine)
        {
            foreach (var col in dataGridPerson.Columns)
            {
                //如果有绑定数据
                if (col.ClipboardContentBinding != null &&
                    col.ClipboardContentBinding.Path != null &&
                    !col.ClipboardContentBinding.Path.Path.Equals(""))
                {
                    if (col.ClipboardContentBinding.Path.Path.Equals("class_type_name_on_ground"))
                    {
                        if (isOnMine)
                        {
                            col.Header = "地面班制";
                        }
                        else
                        {
                            col.Header = "所在班制";
                        }
                    }
                    if (col.ClipboardContentBinding.Path.Path.Equals("class_type_name"))
                    {
                        if (isOnMine)
                        {
                            col.Header = "井下班制";
                            col.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            col.Visibility = Visibility.Collapsed;
                        }
                    }
                    
                }
            }
        }

        /// <summary>
        /// 刷新显示当前部门的人员列表
        /// </summary>
        private void UpdateCurDataGridPerson()
        {
            if (departTree.SelectedValue != null)
            {
                try
                {
                    ((VmDepartAndPeopleMng)this.DataContext).ChangeCurDepart(((TreeNode<UserDepartInfo>)this.departTree.SelectedValue).NodeValue.depart_id);
                }
                catch (System.Exception ex)
                {
                    ErrorWindow err = new ErrorWindow(ex);
                    err.Show();
                }
            }
        }


       

        #endregion

    }
}
