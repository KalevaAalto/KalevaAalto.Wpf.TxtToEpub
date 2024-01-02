using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace KalevaAalto.Wpf.TxtToEpub.Forms
{
    /// <summary>
    /// datatable_messagebox.xaml 的交互逻辑
    /// </summary>
    public partial class datatable_messagebox : Window
    {
        private ObservableCollection<ExpandoObject> list = new ObservableCollection<ExpandoObject>();


        
        

        

        public bool Confirmed { get; private set; } = false;
        public datatable_messagebox(DataTable table,string name="请确认！")
        {
            this.InitializeComponent();
            this.Title = name;
            foreach (DataColumn column in table.Columns)
            {
                DataGridTextColumn textColumn = new DataGridTextColumn();
                textColumn.Header = column.ColumnName;
                textColumn.Binding = new Binding(column.ColumnName);

                Type type = column.DataType;
                if (type == typeof(int) || type == typeof(double) || type == typeof(decimal))
                {
                    textColumn.CellStyle = new Style(typeof(DataGridCell));
                    textColumn.CellStyle.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right));
                }


                this.datagrid.Columns.Add(textColumn);

            }


            foreach (DataRow row in table.Rows)
            {
                var my_type = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;
                foreach (DataColumn column in table.Columns) my_type.Add(column.ColumnName, row[column.ColumnName]);
                this.datagrid.Items.Add(my_type);
            }





        }





        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmed = true;
            this.Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public static bool DataTable_Comfrimd(DataTable table, string name = "请确认！")
        {
            datatable_messagebox win = new datatable_messagebox(table,name);
            win.ShowDialog();
            return win.Confirmed;

        }

    }
}
