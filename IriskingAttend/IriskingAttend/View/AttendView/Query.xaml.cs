/*************************************************************************
** 文件名:   Query.cs
×× 主要类:   Query
**  
** Copyright (c) 中科虹霸有限公司
** 创建人:   lzc
** 日  期:   2013-4-8
** 修改人:   
** 日  期:
** 描  述:   Query类，查询界面
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
using System.Windows.Navigation;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.IO.IsolatedStorage;
using IriskingAttend.Web;
using IriskingAttend.ViewModel;
using Irisking.Web.DataModel;

namespace IriskingAttend.View
{
    public partial class Query : Page
    {

        private VmDepartment m_Depart = new VmDepartment();
        private VmLeaveType m_LeaveType = new VmLeaveType();
        private string m_Url = "/AttendRecord";
        /// <summary>
        /// 构造函数
        /// </summary>
        public Query()
        {
            InitializeComponent();
            SetInit();
           
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Query(string urlStr)
        {
            InitializeComponent();
            m_Url = urlStr;
            SetInit();
        }

        private void SetInit()
        {
            this.btnQuery.IsEnabled = false;
            m_Depart.GetDepartment();
            m_Depart.DepartmentLoadCompleted += delegate
            {
                this.listBoxDepartment.ItemsSource = m_Depart.DepartmentModel.Where(a=>a.depart_id>0);
                this.listBoxDepartment.DisplayMemberPath = "depart_name";
                m_LeaveType.GetLeaveType(0);
            };
            //this.listBoxDepartment.ItemsSource = m_Depart.DepartmentModel;
            //this.listBoxDepartment.DisplayMemberPath = "depart_name";

            this.listBoxAttendType.ItemsSource = m_LeaveType.LeaveTypeModel;
            this.listBoxAttendType.DisplayMemberPath = "leave_type_name";
        }
        /// <summary>
        /// 点击<查询>按钮后，跳转的页面
        /// </summary>
        private string _nextPage = string.Empty;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 样例：/Query?Action=/SafeManager/InWellPersonQuery&NoDateTime&NoMine
            //string strTip = string.Empty;
            foreach (var item in this.NavigationContext.QueryString)
            {
                //strTip = string.Format("{0} = {1}\n", item.Key, item.Value);
                if (item.Key == "Action")
                {
                    _nextPage = item.Value;
                }
                else if (item.Key == "Title")
                {
                    lblTitle.Content = "  " + item.Value;
                    if (item.Value == "领导考勤")
                    {
                        //vmPerson.SelectPrincipalData(2);
                    }
                    else if (item.Value == "关键岗位考勤")
                    {
                        // vmPerson.SelectPrincipalData(1);
                    }//
                    else if (item.Value == "个人考勤")
                    {
                        //0为普通
                        // vmPerson.SelectPrincipalData(-1);
                    }

                }
                else if (string.IsNullOrEmpty(item.Value))
                {
                    //if (dictShower.ContainsKey(item.Key))
                    //    dictShower[item.Key]();
                }
            }
        }


        private IsolatedStorageSettings querySetting = IsolatedStorageSettings.ApplicationSettings;//本地独立存储，用来传参
        //    /// <summary>
        //    /// 查询
        //    /// </summary>
        //    /// <param name="sender"></param>
        //    /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetCondition();
                this.NavigationCacheMode = System.Windows.Navigation.NavigationCacheMode.Required;
                this.NavigationService.Navigate(new Uri(m_Url, UriKind.Relative));
            }
            catch (Exception er)
            {
                ErrorWindow err = new ErrorWindow(er);
                err.Show();
            }
        }

        private void SetCondition()
        {
            try
            {
                AttendQueryCondition qc = new AttendQueryCondition();
                if (this.dateBegin.SelectedDate != null)
                {
                    qc.beginTime = this.dateBegin.SelectedDate.Value.Date;
                }

                if (this.dateEnd.SelectedDate != null)
                {
                    qc.endTime = this.dateEnd.SelectedDate.Value.Date.AddDays(1);

                }
                //qc.nextPageTitle = lblTitle.Content;
                qc.name = txtBoxName.Text;
                qc.workSN = txtBoxWorkSn.Text;

                if (listBoxDepartment.SelectedItems.Count > 0)
                {
                    qc.departIdLst = new int[listBoxDepartment.SelectedItems.Count];
                    for(int i = 0;i< listBoxDepartment.SelectedItems.Count;i++)
                    {
                        qc.departIdLst[i] = (listBoxDepartment.SelectedItems[i] as depart).depart_id;
                    }
                }

                if (listBoxAttendType.SelectedItems.Count > 0)
                {
                    qc.attendTypeIdLst = new int[listBoxAttendType.SelectedItems.Count];
                    for (int i = 0; i < listBoxAttendType.SelectedItems.Count; i++)
                    {
                        qc.attendTypeIdLst[i] = (listBoxAttendType.SelectedItems[i] as leave_type).leave_type_id;
                    }
                }

                ///如果存在 先删除该键值对
                if (querySetting.Contains("attendConditon"))
                {
                    querySetting.Remove("attendConditon");
                }
                querySetting.Add("attendConditon", qc);
            }
            catch (Exception e)
            {
                ErrorWindow err = new ErrorWindow(e);
                err.Show();
            }
        }
        /// <summary>
        /// 设置DataPicker 日期的选择范围
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (dateBegin.SelectedDate != null)
            {
                //this.dateEnd.BlackoutDates.Clear();
                //this.dateEnd.SelectedDate = null;
                //int frontYear = dateBegin.SelectedDate.Value.AddDays(-1).Year;
                //int frontMonth = dateBegin.SelectedDate.Value.AddDays(-1).Month;
                //int frontDay = dateBegin.SelectedDate.Value.AddDays(-1).Day;
                //int backYear = dateBegin.SelectedDate.Value.AddMonths(1).Year;
                //int backMonth = dateBegin.SelectedDate.Value.AddMonths(1).Month;
                //int backDay = dateBegin.SelectedDate.Value.AddMonths(1).Day;

                //this.dateEnd.BlackoutDates.Add(new CalendarDateRange(new DateTime(0001, 1, 1), new DateTime
                //(frontYear, frontMonth, frontDay)));
                //this.dateEnd.BlackoutDates.Add(new CalendarDateRange(new DateTime
                //(backYear, backMonth, backDay), new DateTime(2100, 1, 1)));
                //dateEnd.DisplayDate = dateBegin.SelectedDate.Value;

                //暂时先写一个控件
                this.btnQuery.IsEnabled = true;
            }

            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MessageBox.Show("选择结束时间不能大于开始时间！", "提示", MessageBoxButton.OK);
                    dateBegin.SelectedDate = null;
                    this.btnQuery.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// 选择所有部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            //vmPerson.SelectAllDepart(sender as CheckBox);
        }

        /// <summary>
        /// 校验日期格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            MessageBox.Show("日期格式不对！", "提示", MessageBoxButton.OK);
            //this.btnQuery.IsEnabled = false;
        }

        /// <summary>
        /// 防止用户选择日期后 再将日期删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dateBegin_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                this.btnQuery.IsEnabled = false;
            }
            else
            {
                this.btnQuery.IsEnabled = true;
            }
        }

    
        private void dateEnd_CalendarClosed(object sender, RoutedEventArgs e)
        {
            if (null != dateBegin.SelectedDate && null != dateEnd.SelectedDate)
            {
                if (dateBegin.SelectedDate.Value > dateEnd.SelectedDate.Value)
                {
                    MessageBox.Show("选择结束时间不能大于开始时间！", "提示", MessageBoxButton.OK);
                    dateEnd.SelectedDate = null;
                }
                this.btnQuery.IsEnabled = true;
            }
            else
            {
                this.btnQuery.IsEnabled = false;
            }
        }


    }
}
//namespace csdn
//{
//    /// <summary> 
//    /// WebForm30 的摘要说明。 
//    /// </summary> 
//    public class WebForm30 : System.Web.UI.Page
//    {
//        protected System.Web.UI.WebControls.DataGrid DataGrid1;

//        private void Page_Load(object sender, System.EventArgs e)
//        {
//            // 在此处放置用户代码以初始化页面 
//            if (!IsPostBack)
//            {
//                BindGrid();
//            }
//            CreateDataGrid();//进行一些DataGrid的设置 
//        }

//        protected void CreateDataGrid()
//        {
//            DataGrid1.AutoGenerateColumns = false;//不启用自动生成列 
//            DataGrid1.CssClass = "border";//边框样式 
//            DataGrid1.BorderWidth = 0;
//            DataGrid1.CellSpacing = 1;
//            DataGrid1.CellPadding = 5;
//            DataGrid1.ItemStyle.CssClass = "item";//普通列样式 
//            DataGrid1.HeaderStyle.CssClass = "header";//头样式 
//            DataGrid1.PagerStyle.CssClass = "header";//页脚样式 
//            DataGrid1.DataKeyField = "stuid";//主键字段 
//            DataGrid1.AllowPaging = true;//允许分页 
//            DataGrid1.PageSize = 5;//分页大小 
//            DataGrid1.PagerStyle.Mode = PagerMode.NumericPages;//数字形式分页 
//            EditCommandColumn ecc = new EditCommandColumn();//更新按钮列 
//            ecc.ButtonType = ButtonColumnType.PushButton;//下压按钮 
//            ecc.EditText = "编辑";
//            ecc.CancelText = "取消";
//            ecc.UpdateText = "更新";//按钮文字 
//            DataGrid1.Columns.Add(ecc);//增加按钮列 
//            DataGrid1.EditCommand += new DataGridCommandEventHandler(DataGrid1_EditCommand);
//            DataGrid1.UpdateCommand += new DataGridCommandEventHandler(DataGrid1_UpdateCommand);
//            DataGrid1.CancelCommand += new DataGridCommandEventHandler(DataGrid1_CancelCommand);//更新、取消、编辑事件注册 
//            DataGrid1.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(DataGrid1_PageIndexChanged);//分页事件注册，这里需要注意注册事件代码的位置，不能放到BindGrid()中 
//            SetBind();   //绑定数据 
//        }

//        protected void BindGrid()
//        {
//            TemplateColumn tm = new TemplateColumn();
//            tm.ItemTemplate = new ColumnTemplate1();//普通列 
//            tm.EditItemTemplate = new ColumnTemplate2();//编辑列 
//            tm.HeaderText = "姓名";
//            DataGrid1.Columns.AddAt(0, tm);//在第一列增加第一个模板列 
//            TemplateColumn tm2 = new TemplateColumn();
//            tm2.ItemTemplate = new ColumnTemplate3();
//            tm2.EditItemTemplate = new ColumnTemplate4();
//            tm2.HeaderText = "学院";
//            DataGrid1.Columns.AddAt(1, tm2);//在第二列增加第一个模板列 
//            DataGrid1.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(DataGrid1_ItemDataBound);//数据绑定事件注册，这里需要注意注册事件代码的位置 
//            SetBind();
//        }

//        protected void SetBind()
//        {
//            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["conn"]);
//            SqlDataAdapter da = new SqlDataAdapter("select * from stu,dep where stu.studepid=dep.depid", conn);
//            DataSet ds = new DataSet();
//            da.Fill(ds, "table1");
//            this.DataGrid1.DataSource = ds.Tables["table1"];
//            this.DataGrid1.DataBind();

//        }

//        private void DataGrid1_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
//        {
//            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["conn"]);
//            SqlDataAdapter da = new SqlDataAdapter("select * from dep", conn);
//            DataSet ds = new DataSet();
//            da.Fill(ds, "table1");
//            if (e.Item.ItemType == ListItemType.EditItem)
//            {
//                DropDownList ddl = (DropDownList)e.Item.FindControl("dep");
//                ddl.DataSource = ds.Tables["table1"];
//                ddl.DataTextField = "depname";
//                ddl.DataValueField = "depid";
//                ddl.DataBind();
//                ddl.Items.FindByValue(Convert.ToString(DataBinder.Eval(e.Item.DataItem, "depid"))).Selected = true;
//            }
//        }

//        private void DataGrid1_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
//        {
//            this.DataGrid1.EditItemIndex = e.Item.ItemIndex;
//            BindGrid();
//        }

//        private void DataGrid1_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
//        {
//            this.DataGrid1.EditItemIndex = -1;
//            BindGrid();
//        }

//        private void DataGrid1_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
//        {
//            string uid = e.Item.UniqueID + ":";//注意别遗漏冒号 
//            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationSettings.AppSettings["conn"]);
//            SqlCommand comm = new SqlCommand("update stu set stuname=@name,studepid=@depid where stuid=@id", conn);
//            SqlParameter parm1 = new SqlParameter("@name", SqlDbType.NVarChar, 50);
//            parm1.Value = Request.Form[uid + "name"].ToString();
//            SqlParameter parm2 = new SqlParameter("@depid", SqlDbType.Int);
//            parm2.Value = Request.Form[uid + "dep"].ToString(); ;
//            SqlParameter parm3 = new SqlParameter("@id", SqlDbType.Int);
//            parm3.Value = this.DataGrid1.DataKeys[e.Item.ItemIndex];
//            comm.Parameters.Add(parm1);
//            comm.Parameters.Add(parm2);
//            comm.Parameters.Add(parm3);
//            conn.Open();
//            comm.ExecuteNonQuery();
//            conn.Close();
//            this.DataGrid1.EditItemIndex = -1;
//            BindGrid();
//            //之所以不能采用以前的((TextBox)e.Item.FindControl("name")).Text来取得数据时因为，DataGrid列是动态添加的，根本取不到 
//        }

//        private void DataGrid1_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
//        {
//            this.DataGrid1.CurrentPageIndex = e.NewPageIndex;
//            BindGrid();
//        }

//        #region Web 窗体设计器生成的代码
//        override protected void OnInit(EventArgs e)
//        {
//            // 
//            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。 
//            // 
//            InitializeComponent();
//            base.OnInit(e);
//        }

//        /// <summary> 
//        /// 设计器支持所需的方法 - 不要使用代码编辑器修改 
//        /// 此方法的内容。 
//        /// </summary> 
//        private void InitializeComponent()
//        {
//            this.Load += new System.EventHandler(this.Page_Load);

//        }
//        #endregion
//    }

//    public class ColumnTemplate1 : ITemplate
//    {

//        public void InstantiateIn(Control container)
//        {
//            LiteralControl l = new LiteralControl();
//            l.DataBinding += new EventHandler(this.OnDataBinding);
//            container.Controls.Add(l);
//        }

//        public void OnDataBinding(object sender, EventArgs e)
//        {
//            LiteralControl l = (LiteralControl)sender;
//            DataGridItem container = (DataGridItem)l.NamingContainer;
//            l.Text = ((DataRowView)container.DataItem)["stuname"].ToString();
//        }
//    }

//    public class ColumnTemplate2 : ITemplate
//    {
//        public void InstantiateIn(Control container)
//        {
//            TextBox t = new TextBox();
//            t.Width = 88;
//            t.ID = "name";//需要给一个id，在Request.Form的时候可以取 
//            t.DataBinding += new EventHandler(this.OnDataBinding);
//            container.Controls.Add(t);
//        }

//        public void OnDataBinding(object sender, EventArgs e)
//        {
//            TextBox t = (TextBox)sender;
//            DataGridItem container = (DataGridItem)t.NamingContainer;
//            t.Text = ((DataRowView)container.DataItem)["stuname"].ToString();//绑定stuname字段 
//        }
//    }

//    public class ColumnTemplate3 : ITemplate
//    {
//        public void InstantiateIn(Control container)
//        {
//            LiteralControl l = new LiteralControl();
//            l.DataBinding += new EventHandler(this.OnDataBinding);
//            container.Controls.Add(l);
//        }

//        public void OnDataBinding(object sender, EventArgs e)
//        {
//            LiteralControl l = (LiteralControl)sender;
//            DataGridItem container = (DataGridItem)l.NamingContainer;
//            l.Text = ((DataRowView)container.DataItem)["depname"].ToString();
//        }
//    }

//    public class ColumnTemplate4 : ITemplate
//    {
//        public void InstantiateIn(Control container)
//        {
//            DropDownList dpl = new DropDownList();
//            dpl.ID = "dep";
//            container.Controls.Add(dpl);
//        }//这里没有为这个下拉框进行数据绑定，在DataGrid1的ItemDataBound中进行了这个操作 
//    }
//}