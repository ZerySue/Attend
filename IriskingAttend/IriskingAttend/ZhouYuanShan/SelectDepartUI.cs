/*************************************************************************
** 文件名:   SelectDepartUI.cs
** 主要类:   SelectDepartUI
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   选择部门UI
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
*************************************************************************/
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Irisking.Web.DataModel;
using System.Linq;

namespace IriskingAttend.ZhouYuanShan
{
    public class SelectDepartUI:SelectObjUI
    {
      
        /// <summary>
        /// 数据源
        /// </summary>
        private List<UserDepartInfo> _source;

        /// <summary>
        /// 载入内容
        /// </summary>
        /// <typeparam name="UserDepartInfo"></typeparam>
        /// <param name="items"></param>
        public void LoadContent(List<UserDepartInfo> items)
        {
            //防止数据源有add，remove操作
            _source = new List<UserDepartInfo>();
            foreach (var item in items)
            {
                _source.Add(item);
            }

            this.candidateList.ItemsSource = _source;
            candidateList.DisplayMemberPath = "depart_name";
            selectedList.DisplayMemberPath = "depart_name";
            

            this.stackPanelDepart.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 设置已经被选择的部门
        /// 在载入部门之后调用
        /// </summary>
        /// <param name="personIds"></param>
        public void SetSelectedDeparts(string[] departNames)
        {
            if (departNames == null || departNames.Length == 0)
            {
                return;
            }
            
            this.Dispatcher.BeginInvoke(() =>
            {
                foreach (var item in candidateList.Items)
                {
                    //从原字符串里去掉前置的'-'字符
                    string oriName = PublicMethods.RemoveAtStart(((UserDepartInfo)item).depart_name, '-');
                    if (departNames.Contains(oriName))
                    {
                        selectedList.Items.Add(item);
                    }
                }
                if (SelectedEvent != null)
                {
                    SelectedEvent((int)this.Tag);
                }
            });
        }

        /// <summary>
        /// 设置已经被选择的部门
        /// 在载入部门之后调用
        /// </summary>
        /// <param name="personIds"></param>
        public void SetSelectedDeparts(int[] departIds)
        {
            if (departIds == null || departIds.Length == 0)
            {
                return;
            }

            this.Dispatcher.BeginInvoke(() =>
            {
                foreach (var item in candidateList.Items)
                {
                    if (departIds.Contains(((UserDepartInfo)item).depart_id))
                    {
                        selectedList.Items.Add(item);
                    }
                }
                if (SelectedEvent != null)
                {
                    SelectedEvent((int)this.Tag);
                }
            });
        }

        //过滤开始通知
        protected override void OnFilterStart()
        {
            if (_source == null || _source.Count == 0)
            {
                return;
            }
            candidateList.ItemsSource = Filter(this._source);
        }

        private IEnumerable<UserDepartInfo> Filter(ICollection<UserDepartInfo> source)
        {
            List<UserDepartInfo> res = new List<UserDepartInfo>();
            //模糊匹配条件过滤
            //名称模糊过滤
            foreach (var item in source)
            {
                bool isLikeSeach = this.txtLike.Text.Trim() == "" ? true : item.depart_name.Contains(this.txtLike.Text.Trim());
                bool isAdd = isLikeSeach;
                if (isAdd)
                {
                    res.Add(item);  
                }
            }

            return res;
        }

        /// <summary>
        /// 克隆UI
        /// </summary>
        /// <returns></returns>
        public SelectDepartUI Clone()
        {
            SelectDepartUI res = new SelectDepartUI();
            res.LoadDeparts(this._departs);
            res.LoadContent(this._source);
            
            //过滤条件中选择部门
            res.SelectDepart(this._currentDepartIds);
            
            //已选项赋值
            List<int> selectedDepartIDs = new List<int>();
            foreach (var item in this.selectedList.Items)
            {
                selectedDepartIDs.Add(((UserDepartInfo)item).depart_id);
            }
            res.SetSelectedDeparts(selectedDepartIDs.ToArray());

            return res;
        }

    }
}
