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
using IriskingAttend.ViewModel.SystemViewModel;
using System.Windows.Data;
using System.IO;

using Lite.ExcelLibrary.CompoundDocumentFormat;
using Lite.ExcelLibrary.BinaryFileFormat;
using Lite.ExcelLibrary.SpreadSheet;

using System.Collections.ObjectModel;
using System.Collections;
using System.Reflection;

namespace IriskingAttend.View.ExportExcelView
{
    public partial class DeviceInfoExcel: Page
    {
        private VmSystemDeviceMng m_DeviceInfo = new VmSystemDeviceMng();
        public DeviceInfoExcel()
        {
            InitializeComponent();
            
            m_DeviceInfo.GetDeviceInfoTable_Ria();
            this.DataContext = m_DeviceInfo;

            this.dataGrid_Device.ItemsSource = m_DeviceInfo.SystemDeviceInfos;

            System.Windows.Data.Binding binding = new System.Windows.Data.Binding("SelectDeviceInfoItem") { Mode = BindingMode.TwoWay, };
            dataGrid_Device.SetBinding(DataGrid.SelectedItemProperty, binding);          
        }

        private void But_Export_Click(object sender, RoutedEventArgs e)
        {
            // open file dialog to select an export file.   
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.Filter = "Excel Files(*.xls)|*.xls";

            if (sDialog.ShowDialog() == true)
            {

                // create an instance of excel workbook
                Workbook workbook = new Workbook();
                // create a worksheet object
                Worksheet worksheet = new Worksheet("Device");

                worksheet.Cells[0, 0] = new Cell("                             设备管理                             ");               
                //worksheet.Cells.ColumnWidth[0,0] = 24000;

                Int16 ColumnCount = 0;
                Int16 RowCount = 1;

                //Writing Column Names 
                foreach (DataGridColumn dgcol in dataGrid_Device.Columns)
                {
                    worksheet.Cells[RowCount, ColumnCount] = new Cell(dgcol.Header.ToString());
                    worksheet.Cells.ColumnWidth[(byte)RowCount, (byte)ColumnCount] = 6000;
                    ColumnCount++;
                }

                //Extracting values from grid and writing to excell sheet
                //
                foreach (object data in dataGrid_Device.ItemsSource)
                {
                    ColumnCount = 0;
                    RowCount++;
                    foreach (DataGridColumn col in dataGrid_Device.Columns)
                    {
                        string strValue = "";
                        Binding objBinding = null;
                        if (col is DataGridBoundColumn)
                        {
                            objBinding = (col as DataGridBoundColumn).Binding;
                        }
                        if (col is DataGridTemplateColumn)
                        {
                            //This is a template column... let us see the underlying dependency object
                            DependencyObject objDO = (col as DataGridTemplateColumn).CellTemplate.LoadContent();
                            FrameworkElement oFE = (FrameworkElement)objDO;
                            FieldInfo oFI = oFE.GetType().GetField("TextProperty");
                            if (oFI != null)
                            {
                                if (oFI.GetValue(null) != null)
                                {
                                    if (oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)) != null)
                                    {
                                        objBinding = oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)).ParentBinding;
                                    }
                                }
                            }
                        }
                        if (objBinding != null)
                        {
                            if (objBinding.Path.Path != "")
                            {
                                PropertyInfo pi = data.GetType().GetProperty(objBinding.Path.Path);
                                if (pi != null)
                                {
                                    strValue = Convert.ToString(pi.GetValue(data, null));
                                }
                            }
                            if (objBinding.Converter != null)
                            {
                                if (strValue != "")
                                {
                                    strValue = objBinding.Converter.Convert(strValue, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                                }
                                //else
                                //    strValue = objBinding.Converter.Convert(data, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
                            }
                        }
                        // writing extracted value in excell cell
                        worksheet.Cells[RowCount, ColumnCount] = new Cell(strValue);
                        ColumnCount++;
                    }


                }

                //add worksheet to workbook
                workbook.Worksheets.Add(worksheet);
                // get the selected file's stream
                Stream sFile = sDialog.OpenFile();
                workbook.Save(sFile);
            }
        }
    }
}
