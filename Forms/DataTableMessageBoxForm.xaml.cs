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
    public partial class DataTableMessageBoxForm : Window
    {
        public bool Confirmed { get; private set; } = false;
        public DataTableMessageBoxForm(DataTable table,string name)
        {
            InitializeComponent();
            Title = name;
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


                MainDatagrid.Columns.Add(textColumn);

            }


            foreach (DataRow row in table.Rows)
            {
                var my_type = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;
                foreach (DataColumn column in table.Columns) my_type.Add(column.ColumnName, row[column.ColumnName]);
                MainDatagrid.Items.Add(my_type);
            }


        }





        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        public static bool Comfrimd(DataTable table, string name = @"请确认！")
        {
            DataTableMessageBoxForm win = new DataTableMessageBoxForm(table,name);
            win.ShowDialog();
            return win.Confirmed;

        }

    }
}
