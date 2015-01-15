/*************************************************************************
** 文件名:   BackupConfig.cs
** 主要类:   BackupConfig
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   gqy
** 日  期:   2013-6-14
** 修改人:   cty
** 日  期:   2013-8-4
*修改内容：  增加备份虹膜库功能，修改控件命名
** 描  述:   BackupConfig类,手动备份界面
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
using IriskingAttend.ViewModel.SystemViewModel;
using System.IO;

namespace IriskingAttend.View.SystemView
{
    public partial class BackupConfig : Page
    {
        #region 构造函数

        /// <summary>
        /// 构造函数，初始化
        /// </summary>
        public BackupConfig()
        {            
            InitializeComponent();

            //此label涉及到换行，所以在代码中实现，而不是在界面中直接加载
            this.labIrisApp.Content = "注意:此地址为服务端备份文件地址,若" + Environment.NewLine + "输入错误,可能会发生不可预估的错误";
            this.labIrisData.Content = "注意:此地址为服务端备份文件地址,若" + Environment.NewLine + "输入错误,可能会发生不可预估的错误";
            
            //vm初始化
            VmDBBackup vmDBBackup = new VmDBBackup();
            
            //数据绑定
            this.DataContext = vmDBBackup;
        }

        #endregion

    }
}
