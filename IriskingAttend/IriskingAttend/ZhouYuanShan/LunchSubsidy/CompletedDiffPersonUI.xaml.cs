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
using System.Text;

namespace IriskingAttend.ZhouYuanShan.LunchSubsidy
{
    public partial class CompletedDiffPersonUI : UserControl
    {
        public CompletedDiffPersonUI()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 初始化差异人员列表
        /// </summary>
        /// <param name="names"></param>
        public void Init(string[] names)
        {
            if (names == null)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            foreach (var item in names)
            {
                sb.Append(item + "、");
            }
            if (names.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            this.textDiffPersons.Text = sb.ToString();
        }
    }
}
