﻿/*************************************************************************
** 文件名:   ShowDetailsWuHuShan.cs
×× 主要类:   ShowDetailsWuHuShan
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   cty
** 日  期:   2013-8-30
** 修改人:   
** 日  期:   
** 描  述:   ShowDetailsWuHuShan类，五虎山独立查询界面弹出的显示详细信息的子窗口
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
using IriskingAttend.ViewModel;

namespace IriskingAttend.NewWuHuShan
{
    public partial class ShowDetailsWuHuShan : ChildWindow
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="model"></param>
        public ShowDetailsWuHuShan(BaseViewModelCollection<XlsAttendWuHuShan> model)
        {
            

            InitializeComponent();
            try
            {
                dgShowDetails.ItemsSource = model;

                //添加序号
                this.dgShowDetails.LoadingRow += (a, e) =>
                {
                    int index = e.Row.GetIndex();
                    var cell = dgShowDetails.Columns[0].GetCellContent(e.Row) as TextBlock;

                    cell.Text = (index + 1).ToString();
                };
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
    }
}
