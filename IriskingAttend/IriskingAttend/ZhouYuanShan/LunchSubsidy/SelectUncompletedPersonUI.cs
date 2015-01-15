/*************************************************************************
** 文件名:   SelectUncompletedPersonUI.cs
** 主要类:   SelectUncompletedPersonUI
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-3-4
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
using IriskingAttend.Web.ZhouYuanShan;
using IriskingAttend.Dialog;

namespace IriskingAttend.ZhouYuanShan
{
    public class SelectUncompletedPersonUI : SelectObjUI
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private List<UserPersonInfo> _source = new List<UserPersonInfo>();
        private List<ReportRecordInfoOnDepart_ZhouYuanShan> _compareRecordInfo;

        public SelectUncompletedPersonUI(List<ReportRecordInfoOnDepart_ZhouYuanShan> compareRecordInfo)
        {
            bExecuteAddClick = false;
            this._compareRecordInfo = compareRecordInfo;
            this.addButton.Click += new RoutedEventHandler(addButton_Click);
            this.addAllBtn.Click += new RoutedEventHandler(addAllBtn_Click);
        }

        private void AddPerson( IList Items )
        {
            bool isDone = false;
            List<UserPersonInfo> existPerson = new List<UserPersonInfo>();

            foreach (var item in Items)
            {
                bool bAdd = true;
                if (_compareRecordInfo != null)
                {
                    for (int i = 0; i < _compareRecordInfo.Count; i++)
                    {
                        if (_compareRecordInfo[i].diff_person_ids != null && _compareRecordInfo[i].diff_person_ids.Count() > 0)
                        {
                            for (int j = 0; j < _compareRecordInfo[i].diff_person_ids.Count(); j++)
                            {
                                if (((UserPersonInfo)item).person_id == _compareRecordInfo[i].diff_person_ids[j])
                                {
                                    bAdd = false;

                                    if (!selectedList.Items.Contains(item))
                                    {
                                        existPerson.Add((UserPersonInfo)item);
                                    }
                                }
                            }
                        }
                        if (_compareRecordInfo[i].attend_person_ids != null && _compareRecordInfo[i].attend_person_ids.Count() > 0)
                        {
                            for (int j = 0; j < _compareRecordInfo[i].attend_person_ids.Count(); j++)
                            {
                                if (((UserPersonInfo)item).person_id == _compareRecordInfo[i].attend_person_ids[j])
                                {
                                    bAdd = false;

                                    if (!selectedList.Items.Contains(item))
                                    {
                                        existPerson.Add((UserPersonInfo)item);
                                    }
                                }
                            }
                        }
                    }
                }
                if (bAdd)
                {
                    isDone = true;
                    if (!selectedList.Items.Contains(item))
                    {
                        selectedList.Items.Add(item);
                    }
                }
            }

            if (existPerson.Count > 0)
            {
                string note = "";
                if (existPerson.Count == 1)
                {
                    note = string.Format("此日期及此班次内已存在人员({0})，是否仍然添加？", existPerson[0].person_name);
                }
                else
                {
                    note = string.Format("此日期及此班次内已存在人员({0}、{1})等共{2}名，是否仍然添加？", existPerson[0].person_name, existPerson[1].person_name,existPerson.Count());
                }

                MsgBoxWindow.MsgBox(note, MsgBoxWindow.MsgIcon.Question, MsgBoxWindow.MsgBtns.OKCancel, (iResult) =>
                                            {
                                                if (iResult == MsgBoxWindow.MsgResult.OK)
                                                {
                                                    isDone = true;
                                                    foreach (var item in existPerson)
                                                    {
                                                        if (!selectedList.Items.Contains(item))
                                                        {
                                                            selectedList.Items.Add(item);
                                                        }
                                                    }
                                                }
                                            });
            }
            if (isDone)
            {
                if (SelectedEvent != null)
                {
                    SelectedEvent((int)this.Tag);
                }
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddPerson(candidateList.SelectedItems);            
        }

        private void addAllBtn_Click(object sender, RoutedEventArgs e)
        {
            selectedList.Items.Clear();

            AddPerson(candidateList.Items);               
        }

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
                bool isLikeSeach = this.txtLike.Text.Trim() == "" ? true : item.person_name.Contains(this.txtLike.Text.Trim());
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

    }
}
