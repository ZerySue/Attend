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
using System.Collections.Generic;


namespace IriskingAttend.Data
{
    public class KObject : INotifyPropertyChanged//, IComparable<KObject>
    {
        public int Id { get; set; }

        internal string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                    throw new Exception("名称不能为空");

                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public KObject(int Id = 0, string Name = "")
        {
            this.Id = Id;
            this._name = Name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propertyName)
        {
            _changed = true;
            if (null != PropertyChanged)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _changed = false;
        // 调用完后_changed自动清空
        public virtual bool DataChanged
        {
            get
            {
                bool ret = _changed;
                _changed = false;
                return ret;
            }
        }


        //public int CompareTo(KObject other)
        //{
        //    //throw new NotImplementedException();
        //    if (null != other && null != this)
        //        return Id - other.Id;
        //    else
        //        return 1;
        //}

        public static bool operator ==(KObject a, KObject b)
        {
            return Equals(a, b);
        }
        public static bool operator !=(KObject a, KObject b)
        {
            return !Equals(a, b);
        }
        public override bool Equals(object obj)
        {
            KObject other = obj as KObject;
            if (obj == null && this == null)
                return true;
            else if (other == null || this == null)
                return false;// base.Equals(obj);
            else
                return Id == other.Id;
        }

        public override int GetHashCode()
        {
            if (null != this)
                return Id;
            return base.GetHashCode();
        }
    }

    public class KCompany : KObject
    {
        /// <summary>
        /// 是否矿级单位
        /// </summary>
        public bool IsMine { get; set; }
    }
    public class KDepart : KObject
    {
        private KCompany _company;
        public KCompany Company
        {
            get { return _company; }
            set
            {
                if (value != _company)
                {
                    _company = value;
                    NotifyPropertyChanged("Mine");
                }
            }
        }

        /// <summary>
        /// 父部门
        /// </summary>
        private KDepart _parent;
        public KDepart Parent
        {
            get { return _parent; }
            set
            {
                if (value != _parent)
                {
                    _parent = value;
                    NotifyPropertyChanged("Parent");
                }
            }
        }

        private string _sn = "";
        public string Sn
        {
            get { return _sn; }
            set
            {
                if (_sn != value)
                {
                    _sn = value;
                    NotifyPropertyChanged("Sn");
                }
            }
        }

        private string _memo = "";
        public string Memo
        {
            get { return _memo; }
            set
            {
                if (value != _memo)
                {
                    _memo = value;
                    NotifyPropertyChanged("Memo");
                }
            }
        }

        private bool _isMine = true;
        public bool IsMine
        {
            get { return _isMine; }
            set
            {
                if (value != _isMine)
                {
                    _isMine = value;
                    NotifyPropertyChanged("IsMine");
                }
            }
        }

        private string _telephone = "";
        public string Telephone
        {
            get { return _telephone; }
            set
            {
                if (value != _telephone)
                {
                    _telephone = value;
                    NotifyPropertyChanged("Telephone");
                }
            }
        }

        internal List<KDepart> _children = new List<KDepart>();
        public List<KDepart> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                foreach (KDepart d in _children)
                    d.Parent = this;
            }
        }

        public KDepart()
        {
            //Parent = new KObject();
            //Sn = "";
            //IsMine = false;
            //Memo = "";
            //Telephone = "";
        }

        public override bool DataChanged
        {
            get
            {
                bool _changed = base.DataChanged;
                if (Parent != null)
                    _changed |= Parent.DataChanged;
                return _changed;
            }
        }

        /// <summary>
        /// 判断其上有几级Parent
        /// </summary>
        public int level
        {
            get
            {
                int _l = 0;
                for (KDepart p = this; p.Parent != null; p = p.Parent)
                    _l++;

                return _l;
            }
        }
        public string Indent
        {
            get { return string.Format("{0},0,0,0", level * 16); }
        }

        public Visibility IconVisibility
        {
            get
            {
                return Children == null || Children.Count <= 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}
