/*************************************************************************
** 文件名:   BaseViewModel.cs
×× 主要类:   BaseViewModel
**  
** Copyright (c) 中科虹霸有限公司公司
** 创建人:   fjf
** 日  期:   2012-11-08
** 修改人:   
** 日  期:
** 描  述:   视图模板的基类
** 功  能:   主要用于对视图模板的动态编码和扩展
** 版  本:   1.0.0
** 备  注:   命名及代码编写遵守C#编码规范
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
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.Practices.Prism.Commands;

namespace IriskingAttend.ViewModel
{
    /// <summary>
    /// 视图模板基类
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged, IDisposable
    {



        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        #region 基本的成员 INotifyPropertyChanged Members
        protected void OnPropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            OnPropertyChanged(propertyName);
        }
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
        #region IDisposable Members
        public void Dispose()
        {
            this.OnDispose();
        }
        protected virtual void OnDispose()
        {
        }
        #endregion
        #endregion
    }
    /// <summary>
    /// 动态扩展
    /// 用于非硬编码实现属性改变
    /// </summary>
    public static class PropertyChangedBaseEx
    {
        public static void NotifyPropertyChanged<T, TProperty>(this T propertyChangedBase, Expression<Func<T, TProperty>> expression) where T : BaseViewModel
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;
                propertyChangedBase.NotifyPropertyChanged(propertyName);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
