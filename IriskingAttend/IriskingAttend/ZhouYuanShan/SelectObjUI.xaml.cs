/*************************************************************************
** 文件名:   SelectObjUI.cs
** 主要类:   SelectObjUI
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   选择对象UI
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
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
using System.Collections;
using Irisking.Web.DataModel;
using IriskingAttend.ViewModel;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.Web.WuHuShan;

namespace IriskingAttend.ZhouYuanShan
{
    public partial class SelectObjUI : UserControl
    {

        //当前部门列表id
        protected List<int> _currentDepartIds = new List<int>();
        protected List<UserDepartInfo> _departs;
        public bool bExecuteAddClick = true;

        /// <summary>
        /// 选择或者取消选择操作发生事件
        /// </summary>
        public  Action<int> SelectedEvent;

        public SelectObjUI()
        {
            InitializeComponent();

            this.addAllBtn.Click+=new RoutedEventHandler(addAllBtn_Click);
            this.removeBtn.Click+=new RoutedEventHandler(removeBtn_Click);
            this.addButton.Click+=new RoutedEventHandler(addButton_Click);
            this.removeAllBtn.Click+=new RoutedEventHandler(removeAllBtn_Click);
            this.btnQuery.Click+=new RoutedEventHandler(btnQuery_Click);
            this.candidateList.MouseLeftButtonUp += new MouseButtonEventHandler(candidateList_MouseLeftButtonUp);
        }

      

        #region 部门相关

        /// <summary>
        /// 待选条件过滤开始函数
        /// </summary>
        protected virtual void OnFilterStart()
        {

        }
       
        /// <summary>
        /// 载入部门
        /// </summary>
        /// <param name="departs"></param>
        public void LoadDeparts(List<UserDepartInfo> departs)
        {
            _departs = departs;
            _currentDepartIds = new List<int>();
            
            ComboBoxItem allItem = new ComboBoxItem();
            TextBlock text = new TextBlock();
            text.Text = "全部显示";
            allItem.Content = text;
            //this.LayoutRoot.AddHandler(Grid.MouseLeftButtonUpEvent, new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonUp), true);
            allItem.AddHandler(ComboBoxItem.MouseLeftButtonDownEvent, new MouseButtonEventHandler(allItem_MouseRightButtonDown), true);
            //text.MouseRightButtonDown += new MouseButtonEventHandler(allItem_MouseRightButtonDown);
            this.cmbDepart.Items.Add(allItem);

            foreach (var item in departs)
            {
                CheckBox chkBox = new CheckBox();
                chkBox.Content = item.depart_name;
                chkBox.Tag = item.depart_id;
                chkBox.Click += new RoutedEventHandler(chkBox_Click);
                this.cmbDepart.Items.Add(chkBox);
                _currentDepartIds.Add(item.depart_id);
            }
            //通知子类当前部门改变
            this.OnFilterStart();
        }
        

        //显示全部部门对应的内容
        void allItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _currentDepartIds = new List<int>();
            //更新checkbox属性
            foreach (var item in this.cmbDepart.Items)
            {
                if (item is CheckBox)
                {
                    ((CheckBox)item).IsChecked = false;
                    _currentDepartIds.Add((int)((CheckBox)item).Tag);
                }
            }
           


            //通知子类当前部门改变
            this.OnFilterStart();
        }

        //显示当前选择部门对应的内容
        void chkBox_Click(object sender, RoutedEventArgs e)
        {
            _currentDepartIds.Clear();
            foreach (var item in this.cmbDepart.Items)
            {
                if (item is CheckBox)
                {
                    bool isSelected = ((CheckBox)item).IsChecked.Value ;
                    if (isSelected)
                    {
                        _currentDepartIds.Add((int)((CheckBox)item).Tag);
                    }
                   
                }
            }
            
            //通知子类当前部门改变
            this.OnFilterStart();
        }

        /// <summary>
        /// 扩展功能，指定选择某部门
        /// 需要在载入部门之后执行
        /// </summary>
        public void SelectDepart(int departId)
        {
            List<int> departIds = new List<int>();
            departIds.Add(departId);
            SelectDepart(departIds);
        }

        /// <summary>
        /// 扩展功能，指定选择某部门
        /// 需要在载入部门之后执行
        /// </summary>
        public void SelectDepart(List<int> departIds)
        {
            _currentDepartIds.Clear();
            foreach (var item in this.cmbDepart.Items)
            {
                if (item is CheckBox)
                {
                    if (departIds.Contains((int)((CheckBox)item).Tag))
                    {
                        ((CheckBox)item).IsChecked = true;
                        _currentDepartIds.Add((int)((CheckBox)item).Tag);
                    }
                }
            }
            //通知子类当前部门改变
            this.OnFilterStart();
        }
        
        #endregion

        #region 选择相关

        private List<UserDepartInfo> GetChildDepart(List<UserDepartInfo> items, int departId)
        {
            return items.Where(a => ((UserDepartInfo)a).parent_depart_id == departId).ToList();
        }

        private List<UserDepartInfo> GetSelectChildDepart(List<UserDepartInfo> items, int departId)
        {
            List<UserDepartInfo> tempDepart = GetChildDepart(items, departId);

            if (tempDepart == null || tempDepart.Count <= 0)
            {
                return tempDepart;
            }

            List<UserDepartInfo> totalchildDepart = new List<UserDepartInfo>();

            foreach (UserDepartInfo item in tempDepart)
            {
                List<UserDepartInfo> childDepart = GetSelectChildDepart(items, item.depart_id);

                if (childDepart != null && childDepart.Count > 0)
                {
                    foreach (UserDepartInfo departInfo in childDepart)
                    {
                        totalchildDepart.Add(departInfo);
                    }
                }
            }

            foreach (UserDepartInfo departInfo in totalchildDepart)
            {
                tempDepart.Add(departInfo);
            }

            return tempDepart;
        }
       
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (!bExecuteAddClick)
            {
                return;
            }
            List<UserDepartInfo> totalDepart = new List<UserDepartInfo>();

            foreach (var item in candidateList.Items)
            {
                if (item.GetType().Equals(typeof(UserDepartInfo)))
                {
                    totalDepart.Add((UserDepartInfo)item);
                }
            }

            bool isDone = false;
            foreach (var item in candidateList.SelectedItems)
            {
                isDone = true;
                if (!selectedList.Items.Contains(item))
                {
                    selectedList.Items.Add(item);

                    if (item.GetType().Equals(typeof(UserDepartInfo)))
                    {
                        List<UserDepartInfo> tempInDepart = GetSelectChildDepart(totalDepart, ((UserDepartInfo)item).depart_id);
                        List<UserDepartInfo> tempOutDepart = new List<UserDepartInfo>();
                        PublicMethods.OrderDepartByTree(tempInDepart, tempOutDepart, ((UserDepartInfo)item).depart_id, "");

                        if (tempOutDepart != null && tempOutDepart.Count > 0)
                        {
                            foreach (var departInfo in tempOutDepart)
                            {
                                if (!selectedList.Items.Contains(departInfo))
                                {
                                    selectedList.Items.Add(departInfo);
                                }
                            }
                        }
                    }
                }
            }
            if (isDone)
            {
                if (SelectedEvent != null)
                {
                    SelectedEvent((int)this.Tag);
                }
            }
        }

        private void removeBtn_Click(object sender, RoutedEventArgs e)
        {
            List<UserDepartInfo> totalDepart = new List<UserDepartInfo>();
            foreach (var item in selectedList.Items)
            {
                if (item.GetType().Equals(typeof(UserDepartInfo)))
                {
                    totalDepart.Add((UserDepartInfo)item);
                }
            }
            bool isDone = false;
            List<object> toBeRemoved = new List<object>();
            foreach (var item in selectedList.SelectedItems)
            {
                toBeRemoved.Add(item);                
            }
            foreach (var item in toBeRemoved)
            {
                isDone = true;
                selectedList.Items.Remove(item);                

                if (item.GetType().Equals(typeof(UserDepartInfo)))
                {
                    List<UserDepartInfo> tempDepart = GetSelectChildDepart(totalDepart, ((UserDepartInfo)item).depart_id);

                    if (tempDepart != null && tempDepart.Count > 0)
                    {
                        foreach (var departInfo in tempDepart)
                        {
                            if (selectedList.Items.Contains(departInfo))
                            {
                                selectedList.Items.Remove(departInfo);
                            }
                        }
                    }
                }
               
            }
            if (isDone)
            {
                if (SelectedEvent != null)
                {
                    SelectedEvent((int)this.Tag);
                }
            }
        }

        private void addAllBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!bExecuteAddClick)
            {
                return;
            }
            selectedList.Items.Clear();
            foreach (var item in candidateList.Items)
            {
                selectedList.Items.Add(item);
            }

            if (SelectedEvent != null)
            {
                SelectedEvent((int)this.Tag);
            }
            
        }

        private void removeAllBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedList.Items.Clear();

            if (SelectedEvent != null)
            {
                SelectedEvent((int)this.Tag);
            }
        }

        #endregion

        /// <summary>
        /// 清空选择对象
        /// </summary>
        public void ClearAllSelectedValue()
        {
            selectedList.Items.Clear();
        }

        /// <summary>
        /// 启用模糊匹配 过滤待选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            OnFilterStart();
        }

        private void candidateList_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!PublicMethods.IsUserControlDoubleClicked(this.candidateList))
            {
                return;
            }
            addButton_Click( null, null );
        }

     
    }
}
