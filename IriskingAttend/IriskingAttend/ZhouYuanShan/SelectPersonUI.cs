/*************************************************************************
** 文件名:   SelectPersonUI.cs
** 主要类:   SelectPersonUI
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   选择人员UI
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
using System.Collections;
using System.Linq;
using IriskingAttend.Web.WuHuShan;

namespace IriskingAttend.ZhouYuanShan
{
    public class SelectPersonUI : SelectObjUI
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private List<UserPersonInfo> _source = new List<UserPersonInfo>();

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IEnumerable<UserPersonInfo> Filter(ICollection<UserPersonInfo> source)
        {
            List<UserPersonInfo> res = new List<UserPersonInfo>();
           
            foreach (var item in source)
            {
                //名称模糊过滤
                bool isLikeSeach = this.txtLike.Text.Trim() == "" ? true : item.person_name.Contains(this.txtLike.Text.Trim())
                    || this.txtLike.Text.Trim() == "" ? true : item.work_sn.CompareTo(this.txtLike.Text.Trim()) == 0;
                bool isAdd = this._currentDepartIds.Contains(item.depart_id) && //部门过滤
                    isLikeSeach;
                if (isAdd)
                {
                    res.Add(item);
                }
            }
            return res;
        }
        
        /// <summary>
        /// 载入内容
        /// </summary>
        /// <typeparam name="WorkTypeInfo"></typeparam>
        /// <param name="items"></param>
        public void LoadContent(List<UserPersonInfo> items)
        {
            //防止数据源有add，remove操作,影响到上层
            _source = new List<UserPersonInfo>();
            foreach (var item in items)
            {
                _source.Add(item);
            }
           
            candidateList.ItemsSource = Filter(this._source);
            candidateList.DisplayMemberPath = "person_name";
            selectedList.DisplayMemberPath = "person_name";
            

            this.stackPanelDepart.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 设置不被选择的人员
        /// 在载入人员之后调用
        /// </summary>
        /// <param name="personIds"></param>
        public void SetUnSelectedPersons(int[] personIds)
        {
            if (personIds == null) return;
            for (int i = _source.Count-1; i >=0 ; i--)
            {
                if (personIds.Contains(_source[i].person_id))
                {
                    _source.Remove(_source[i]);
                }
            }
            candidateList.ItemsSource = Filter(this._source);
        }

        /// <summary>
        /// 设置已经被选择的人员
        /// 在载入人员之后调用
        /// </summary>
        /// <param name="personIds"></param>
        public void SetSelectedPersons(string[] personNames)
        {
            if (personNames == null || personNames.Length == 0)
            {
                return;
            }
            this.Dispatcher.BeginInvoke(()=>
                {
                    foreach (var item in _source)
                    {
                        if (personNames.Contains(item.person_name))
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
        /// 设置已经被选择的人员
        /// 在载入人员之后调用
        /// </summary>
        /// <param name="personIds"></param>
        public void SetSelectedPersons(int[] personIds)
        {
            if (personIds == null || personIds.Length == 0)
            {
                return;
            }
            this.Dispatcher.BeginInvoke(() =>
            {
                foreach (var item in _source)
                {
                    if (personIds.Contains(item.person_id))
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
        /// 当前部门改变事件
        /// </summary>
        protected override void OnFilterStart()
        {
            if (_source == null || _source.Count == 0)
            {
                return;
            }
            candidateList.ItemsSource = Filter(this._source);
        }

        /// <summary>
        /// 克隆UI
        /// </summary>
        /// <returns></returns>
        public SelectPersonUI Clone()
        {
            SelectPersonUI res = new SelectPersonUI();
            res.LoadDeparts(this._departs);
            res.LoadContent(this._source);

            //过滤条件中选择部门
            res.SelectDepart(this._currentDepartIds);

            //已选项赋值
            List<int> selectedPersonIDs = new List<int>();
            foreach (var item in this.selectedList.Items)
            {
                selectedPersonIDs.Add(((UserPersonInfo)item).person_id);
            }
            res.SetSelectedPersons(selectedPersonIDs.ToArray());

            return res;
        }


    }
}
