/*************************************************************************
** 文件名:   BaseViewModelCollection.cs
×× 主要类:   BaseViewModelCollection
**  
** Copyright (c) 中科虹霸有限公司公司
** 创建人:   fjf
** 日  期:   2012-11-08
** 修改人:   
** 日  期:
** 描  述:   考勤视图模板类
** 功  能:   主要用于对考勤视图进行控制
** 版  本:   1.0.0
** 备  注:   对视图模板有两种处理方式：
 *           1、继承BaseViewModel类
 *           2、继承ObservableCollection<T>
 *           3、二者区别：主要是对单一和泛型的分别处理，前者更加灵活
 *           命名及代码编写遵守C#编码规范
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IriskingAttend.ViewModel
{
    /// <summary>
    /// 多数据源情况下采用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseViewModelCollection<T> : ObservableCollection<T>
    {
        //隐藏父类的事件
        public new event PropertyChangedEventHandler PropertyChanged;
        private delegate bool CompareAction(T object1,T object2);

        public BaseViewModelCollection()
            : base()
        {
        }
        /// <summary>
        /// 增加函数
        /// </summary>
        /// <param name="item"></param>
        public new void Add(T item)
        {
            ((INotifyPropertyChanged)item).PropertyChanged += new PropertyChangedEventHandler(ViewModelCollectionPropertyChanged);
            base.Add(item);
        }

        /// <summary>
        ///  将元素插入 BaseViewModelCollection 的指定索引处
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 item</param>
        /// <param name="item">要插入的对象。对于引用类型，该值可以为 null</param>
        public new void Insert(int index, T item)
        {
            ((INotifyPropertyChanged)item).PropertyChanged += new PropertyChangedEventHandler(ViewModelCollectionPropertyChanged);
            base.Insert(index, item);
        }

        /// <summary>
        /// 按指定列元素的toString()值
        /// 对整个集合排序 by wz
        /// 耗时太长  暂时不用
        /// </summary>
        /// <param name="columnName">要排序的列名字</param>
        public void Sort(string columnName, bool isDescending)
        {
            int element = isDescending ? 1 : -1;
            for (int i = 0; i < this.Count-1; i++)
            {
                for (int j = i+1; j < this.Count; j++)
                {
                    string Istr = this[i].GetType().GetProperties().Where(p => p.Name == columnName).Single().GetValue(this[i], null).ToString();
                    string Jstr = this[j].GetType().GetProperties().Where(p => p.Name == columnName).Single().GetValue(this[j], null).ToString();
                    if ( (Istr.CompareTo(Jstr) *element) > 0)
                    {
                        T tmp = this[i];
                        this[i] = this[j];
                        this[j] = tmp;
                    }
                }
            }
        }
        
        ///// <summary>
        ///// 增加函数
        ///// </summary>
        ///// <param name="item"></param>
        //public Collection<T> Get()
        //{
        //    //((INotifyPropertyChanged)item).PropertyChanged += new PropertyChangedEventHandler(ViewModelCollectionPropertyChanged);
        //    return base.;
        //}
        ///<summary>
        ///登陆函数
        /// </summary>
        ///
        //public new void CheckLogin(T item)
        //{
        //   ((INotifyPropertyChanged)item).PropertyChanged += new PropertyChangedEventHandler(ViewModelCollectionPropertyChanged);
        //}

        /// <summary>
        /// 控制属性的动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ViewModelCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(e.PropertyName));
            }
        }
    }

}
