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

namespace IriskingAttend.CustomUI
{
    public partial class DropDownCheckList : UserControl
    {
        public DropDownCheckList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 用键值对初始化待选择的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valuePairs"></param>
        public void InitContent<T>(Dictionary<string,T> valuePairs)
        {
            CheckBox chkAll = new CheckBox();
            chkAll.Tag = default(T);
            chkAll.Content = "全部";
            this.comboBox.Items.Add(chkAll);
            chkAll.Click += new RoutedEventHandler(chkAll_Click);

            foreach (var item in valuePairs)
            {
                CheckBox chk = new CheckBox();
                chk.Content = item.Key;
                chk.Tag = item.Value;
                chk.Click += new RoutedEventHandler(chk_Click);
                this.comboBox.Items.Add(chk);
            }
        }

        void chkAll_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = ((CheckBox)sender).IsChecked == true;
            foreach (CheckBox chk in this.comboBox.Items)
            {
                chk.IsChecked = isChecked;
            }
            chk_Click(null, null);
        }

        void chk_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            bool hasValue = false;
            foreach (CheckBox chk in this.comboBox.Items)
            {
                if (chk.IsChecked == true && chk.Content.ToString() != "全部")
                {
                    sb.Append(chk.Content + ",");
                    hasValue = true;
                }
            }
            if (hasValue)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            this.txt.Text = sb.ToString();
            
        }

        /// <summary>
        /// 获取被选择中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetSelectedValues<T>()
        {
            List<T> res = new List<T>();
            foreach (CheckBox chk in this.comboBox.Items)
            {
                if (chk.IsChecked == true && chk.Content.ToString() != "全部")
                {
                    res.Add((T)chk.Tag);
                }
            }
            
            return res;
        }

        /// <summary>
        /// 获取被选中的类容，用spliter隔开
        /// </summary>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public string GetSelectedContent(string spliter)
        {
            StringBuilder res = new StringBuilder();
            foreach (CheckBox chk in this.comboBox.Items)
            {
                if (chk.IsChecked == true && chk.Content.ToString() != "全部")
                {
                    res.Append(chk.Content + spliter);
                }
            }
            if (res.Length > 0)
            {
                res.Remove(res.Length - 1, 1);
            }
            else
            {
                res.Append("全部");
            }
            return res.ToString();
        }

    }
}
