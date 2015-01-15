/*************************************************************************
** 文件名:   VmSelectInfoFilter.cs
** 主要类:   VmSelectInfoFilter
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2014-6-19
** 修改人:   
** 日  期:
** 描  述:   VmSelectInfoFilter，报表查询条件
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
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;
using IriskingAttend.Web;
using System.ServiceModel.DomainServices.Client;
using EDatabaseError;
using IriskingAttend.View;

namespace IriskingAttend
{
    #region 数据绑定类

    /// <summary>
    /// 数据绑定的实体类
    /// </summary>
    public class SelectInfoFilter : BaseViewModel
    {
        private bool _checked;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                OnPropertyChanged<bool>(() => this.Checked);
            }
        }

        private string _infoName;
        /// <summary>
        /// 信息名字
        /// </summary>
        public string InfoName
        {
            get
            {
                return _infoName;
            }
            set
            {
                _infoName = value;
                OnPropertyChanged<string>(() => this.InfoName);
            }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        public SelectInfoFilter()
        {
            Checked = false;
        }

    }
    #endregion
    public class VmSelectInfoFilter:BaseViewModel
    {
        #region 变量        

        /// <summary>
        /// 全选按钮的绑定类
        /// </summary>
        public MarkObject MarkObj
        {
            get;
            set;
        }

        #endregion

        #region 与界面绑定属性

        /// <summary>
        /// 过滤信息
        /// </summary>
        public BaseViewModelCollection<SelectInfoFilter> InfoFilterModel
        {
            get;
            set;
        }
        
        #endregion        

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public VmSelectInfoFilter()
        {
            MarkObj = new MarkObject();
            InfoFilterModel = new BaseViewModelCollection<SelectInfoFilter>();           
        }

        #endregion        

        #region 过滤信息的选中与全选操作

        /// <summary>
        /// 过滤信息全选操作
        /// </summary>
        /// <param name="isChecked"></param>
        public void SelectAllInfoFilter(bool? isChecked)
        {
            if (!isChecked.HasValue)
            {
                return;
            }
            if (isChecked.Value)
            {
                foreach (var item in InfoFilterModel)
                {
                    item.Checked = true;
                }
            }
            else
            {
                foreach (var item in InfoFilterModel)
                {
                    item.Checked = false;
                }
            }
        }

        /// <summary>
        /// 按需选择datagrid的Items
        /// </summary>
        /// <param name="sender">datagrid对象</param>
        public void SelectItemInfoFilter(SelectInfoFilter info)
        {
            info.Checked = !info.Checked;
            this.MarkObj.Selected = CheckInfoIsAllSelected();
        }

        /// <summary>
        /// 检查Item是否全部被选中
        /// </summary>
        private bool CheckInfoIsAllSelected()
        {
            if (InfoFilterModel.Count == 0)
            {
                return false;
            }
            foreach (var item in InfoFilterModel)
            {
                if (!item.Checked)
                {
                    return false;
                }
            }
            return true;

        }
        #endregion
    }
}
