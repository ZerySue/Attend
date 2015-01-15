/*************************************************************************
** 文件名:   TreeNode.cs
×× 主要类:   TreeNode
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   wz
** 日  期:   2013-7-23
** 修改人:   gqy
** 日  期:   2013-8-09
** 描  述:   树节点类
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using IriskingAttend.ViewModel;
using System.Windows.Media.Imaging;
using Irisking.Web.DataModel;
using IriskingAttend.Common;
using System.Collections.Generic;

namespace IriskingAttend
{
    /// <summary>
    /// 树节点类，与treeview控件结合使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TreeNode<T> : BaseViewModel
    {
        #region 字段
        
        /// <summary>
        /// 获取节点名字的委托
        /// </summary>
        public Func<string> GetNodeNameDelegate; 

        //节点图标uri资源
        private static Uri[] _imgSources = new Uri[]{
                new Uri("/IriskingAttend;component/Images/generalDepart.png", UriKind.Relative) ,     // 总部门
                new Uri("/IriskingAttend;component/Images/directly.png", UriKind.Relative) ,     // 直属
                new Uri("/IriskingAttend;component/Images/directly_disable.png", UriKind.Relative) ,     // 直属 disable
                new Uri("/IriskingAttend;component/Images/indirectly.png", UriKind.Relative),      // 非直属
                 new Uri("/IriskingAttend;component/Images/indirectly_disable.png", UriKind.Relative),      // 非直属 disable
            };

        /// <summary>
        /// 层级关系,根节点第0层，子节点依次+1
        /// </summary>
        public int Level = -1;

      

        #endregion

        #region 构造函数

        public TreeNode()
        {
            this.Children = new BaseViewModelCollection<TreeNode<T>>();
        }

        #endregion

        #region 绑定属性

        /// <summary>
        /// 节点存储的值
        /// </summary>
        public T NodeValue 
        { 
            get; 
            set; 
        }
        
        /// <summary>
        /// 节点显示的名字
        /// </summary>
        public String NodeName    
        {
            get
            {
                if (GetNodeNameDelegate != null)
                {
                    return GetNodeNameDelegate();
                }
                return "";
            }
        }

        private bool _isOpen = false;
        /// <summary>
        /// 该节点是否展开
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }
            set
            {
                _isOpen = value;
                this.OnPropertyChanged<bool>(() => this.IsOpen);
            }
        }

        private bool _isChecked = false;
        /// <summary>
        /// 该节点是否被选中， add by gqy
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                this.OnPropertyChanged<bool>(() => this.IsChecked);
            }
        }

        private int _index;
        /// <summary>
        /// 节点序号
        /// </summary>
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                _index = value;
                this.OnPropertyChanged<int>(() => this.Index);
            }
        }

        private bool _isEnable = true;
        
        /// <summary>
        /// 该节点是否能被操作
        /// 遵循如下规则：
        /// 如果父节点有权限，则其下的子节点都有权限
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return _isEnable;
            }
            set
            {
                _isEnable = value;
                this.OnPropertyChanged<bool>(() => this.IsEnable);
            }
        }

        private Visibility _visibility = Visibility.Visible;
        
        /// <summary>
        /// 该节点的visible属性
        /// 遵循如下规则：
        /// 1. 如果该节点是叶子节点，则它的可见性由IsEnable属性决定
        /// 2. 否则，该节点的可见性 由 它的子节点是否包含可见节点来决定
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                this.OnPropertyChanged<Visibility>(() => this.Visibility);
            }
        }

     
        /// <summary>
        /// 该节点的前景色
        /// </summary>
        public Brush Foreground
        {
            get
            {
                if (IsEnable)
                {
                    return new SolidColorBrush(mathFun.ReturnColorFromString("0xff000000"));
                }
                else
                {
                    return new SolidColorBrush(mathFun.ReturnColorFromString("0x71000000"));
                }
            }
       
        }


        /// <summary>
        /// 节点显示的图标
        /// </summary>
        public ImageSource NodeImage
        {
            get
            {
                if (IsEnable)
                {
                    if (Level == 0)
                    {
                         return new BitmapImage(_imgSources[0]);
                    }
                    else if (Level == 1)
                    {
                        return new BitmapImage(_imgSources[1]);
                    }
                    else
                    {
                        return new BitmapImage(_imgSources[3]);
                    }
                }
                else
                {
                    if (Level == 0)
                    {
                        return new BitmapImage(_imgSources[0]);
                    }
                    else if (Level == 1)
                    {
                        return new BitmapImage(_imgSources[2]);
                    }
                    else
                    {
                        return new BitmapImage(_imgSources[4]);
                    }
                }
            }
        }

        /// <summary>
        /// 节点拥有的子节点
        /// </summary>
        public BaseViewModelCollection<TreeNode<T>> Children { get; set; }

        #endregion


    }
}
