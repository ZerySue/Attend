/*************************************************************************
** 文件名:   MultiDelegateObjects.cs
×× 主要类:   MultiDelegateObjects
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:
** 描  述:   MultiDelegateObjects类，用来支持多参传值
**
** 版  本:   1.0.0
** 备  注:  命名及代码编写遵守C#编码规范
**
 * ***********************************************************************/

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System;

namespace IriskingAttend.BehaviorSelf
{
    public class MultiDelegateObjects : DependencyObjectCollection<DelegateObject>
    {
        /// <summary>
        /// 构造函数，初始化_keysSnapshot，并注册事件
        /// </summary>
        public MultiDelegateObjects()
        {
            _keysSnapshot = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            ((INotifyCollectionChanged)this).CollectionChanged += _dependencyParametersCollectionChanged;
        }

        private const string _keyAlreadyExist = "key'{0}'已存在";
        private const string _keyNotExist = "key:'{0}'不存在！";
        private HashSet<string> _keysSnapshot;

        /// <summary>
        /// 通过键值获取属性值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public object this[string key]
        {
            get { return this.First(p => string.Equals(p.Key, key, StringComparison.InvariantCultureIgnoreCase)).Value; }
            set { this.First(p => string.Equals(p.Key, key, StringComparison.InvariantCultureIgnoreCase)).Value = value; }
        }

        /// <summary>
        /// 键值集合
        /// </summary>
        public ICollection<string> Keys
        {
            get { return this.Select(p => p.Key).ToList(); }
        }

        /// <summary>
        /// 键值对于值的集合
        /// </summary>
        public ICollection<object> Values
        {
            get { return this.Select(p => p.Value).ToList(); }
        }

        /// <summary>
        /// 添加键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(string key, object value)
        {
            ArgumentExceptionGuard.ArgumentNotNull(key, "key");
            base.Add(new DelegateObject(key, value));
        }

        /// <summary>
        /// 判断是否存在键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否存在</returns>
        public bool ContainsKey(string key)
        {
            return _keysSnapshot.Contains(key);
        }

        /// <summary>
        /// 测试键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetValue(string key)
        {
            object o;
            return TryGetValue(key, out o);
        }

        /// <summary>
        /// 测试键值对
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否存在</returns>
        public bool TryGetValue(string key, out object value)
        {
            ArgumentExceptionGuard.ArgumentNotNull(key, "key");
            if (_keysSnapshot.Contains(key))
            {
                value = this[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        /// <summary>
        /// 通过键获取值
        /// </summary>
        /// <typeparam name="T">值</typeparam>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public T GetValue<T>(string key)
        {
            try
            {
                object o;
                if (TryGetValue(key, out o))
                {
                    return (T)o;
                }
                else
                {
                    throw new ArgumentException(string.Format(_keyNotExist, key));
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// 刷新键值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="o">值</param>
        public void Refresh(string key, DelegateObject o)
        {
            var dp = this.Where(d => d.Key == key).Single();
            var i = IndexOf(dp);
            Remove(dp);
            dp = null;
            dp = o;
            this.Insert(i, dp);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            _dependencyParametersCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        /// <summary>
        /// 添加委托
        /// </summary>
        /// <param name="parameter"></param>
        protected virtual void OnItemAdded(DelegateObject parameter) { }
        /// <summary>
        /// 移除委托
        /// </summary>
        /// <param name="parameter"></param>
        protected virtual void OnItemRemoved(DelegateObject parameter) { }

        #region 事件
        /// <summary>
        /// 集合改变触发事件
        /// </summary>
        private NotifyCollectionChangedEventHandler _dependencyParametersCollectionChanged
        {
            get
            {
                return (o, e) =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (DelegateObject _parameter in e.NewItems)
                            {
                                AddItemKey(_parameter.Key);
                                OnItemAdded(_parameter);
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            foreach (DelegateObject _parameter in e.OldItems)
                            {
                                RemoveItemKey(_parameter.Key);
                                OnItemRemoved(_parameter);
                            }
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            foreach (DelegateObject _parameter in e.OldItems)
                            {
                                RemoveItemKey(_parameter.Key);
                                OnItemRemoved(_parameter);
                            }
                            foreach (DelegateObject _parameter in e.NewItems)
                            {
                                AddItemKey(_parameter.Key);
                                OnItemAdded(_parameter);
                                continue;
                            }
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            _keysSnapshot.Clear();
                            _keysSnapshot = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
                            foreach (var _parameter in this)
                            {
                                AddItemKey(_parameter.Key);
                                OnItemAdded(_parameter);
                            }
                            break;

                        default: return;
                    }
                };
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 添加键
        /// </summary>
        /// <param name="key">键</param>
        private void AddItemKey(string key)
        {
            ArgumentExceptionGuard.ArgumentNotNullOrWhiteSpace(key, "parameter");
            ArgumentExceptionGuard.ArgumentValue(!_keysSnapshot.Add(key), "parameter", _keyAlreadyExist, key);
        }

        /// <summary>
        /// 清除键
        /// </summary>
        /// <param name="key">键</param>
        private void RemoveItemKey(string key)
        {
            ArgumentExceptionGuard.ArgumentNotNullOrWhiteSpace(key, "parameter");
            _keysSnapshot.Remove(key);
        }

        #endregion
    }
}