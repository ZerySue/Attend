/*************************************************************************
** 文件名:   SelectWorkTypeUI.cs
** 主要类:   SelectWorkTypeUI
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-12-30
** 修改人:   
** 日  期:
** 描  述:   选择工种UI
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
using IriskingAttend.Web.ZhouYuanShan;

namespace IriskingAttend.ZhouYuanShan
{
    public class SelectWorkTypeUI:SelectObjUI
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private List<WorkTypeInfo_ZhouYuanShan> _source;

        private IEnumerable<WorkTypeInfo_ZhouYuanShan> Filter(ICollection<WorkTypeInfo_ZhouYuanShan> source)
        {
            List<WorkTypeInfo_ZhouYuanShan> res = new List<WorkTypeInfo_ZhouYuanShan>();
            foreach (var item in source)
            {
                //名称模糊过滤
                bool isLikeSeach = this.txtLike.Text.Trim() == "" ? true : item.work_type_name.Contains(this.txtLike.Text.Trim());
                bool isAdd = this._currentDepartIds.Contains(item.depart_id) && //部门过滤
                    isLikeSeach;     
                if (isAdd)
                {
                    //防止 多个部门 过滤时  存在相同的工种，将相同的工种过滤掉
                    if (res.FirstOrDefault((info) => info.work_type_id == item.work_type_id) == null)
                    {
                        res.Add(item);
                    }
                }
            }

            return res;
        }
        
        /// <summary>
        /// 载入内容
        /// </summary>
        /// <typeparam name="WorkTypeInfo"></typeparam>
        /// <param name="items"></param>
        public void LoadContent(List<WorkTypeInfo_ZhouYuanShan> items)
        {
            //防止数据源有add，remove操作
            _source = new List<WorkTypeInfo_ZhouYuanShan>();
            foreach (var item in items)
            {
                _source.Add(item);
            }

            candidateList.ItemsSource = Filter(this._source);
            candidateList.DisplayMemberPath = "work_type_name";
            selectedList.DisplayMemberPath = "work_type_name";
            

            this.stackPanelDepart.Visibility = Visibility.Visible;
        }

        protected override void OnFilterStart()
        {
            if (_source == null || _source.Count == 0)
            {
                return;
            }
            candidateList.ItemsSource = Filter(this._source);
        }


        /// <summary>
        /// 设置已经被选择的部门
        /// 在载入部门之后调用
        /// </summary>
        /// <param name="personIds"></param>
        public void SetSelectedWorkTypes(int[] workTypeIds)
        {
            if (workTypeIds == null || workTypeIds.Length == 0)
            {
                return;
            }

            this.Dispatcher.BeginInvoke(() =>
            {
                foreach (var item in candidateList.Items)
                {
                    if (workTypeIds.Contains(((WorkTypeInfo_ZhouYuanShan)item).work_type_id))
                    {
                        selectedList.Items.Add(item);
                    }
                    if (SelectedEvent != null)
                    {
                        SelectedEvent((int)this.Tag);
                    }
                }
            });
        }


        /// <summary>
        /// 克隆UI
        /// </summary>
        /// <returns></returns>
        public SelectWorkTypeUI Clone()
        {
            SelectWorkTypeUI res = new SelectWorkTypeUI();
            res.LoadDeparts(this._departs);
            res.LoadContent(this._source);

            //过滤条件中选择部门
            res.SelectDepart(this._currentDepartIds);

            //已选项赋值
            List<int> selectedWorkTypeIDs = new List<int>();
            foreach (var item in this.selectedList.Items)
            {
                selectedWorkTypeIDs.Add(((WorkTypeInfo_ZhouYuanShan)item).work_type_id);
            }
            res.SetSelectedWorkTypes(selectedWorkTypeIDs.ToArray());

            return res;
        }

    }
}
