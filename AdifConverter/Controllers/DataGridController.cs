using AdifConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AdifConverter.Controllers
{
    public class DataGridController
    {
        public void SetupGrid(DataGrid dataGrid, List<ADIFRecord> records)
        {
            var columns = records.First()
                .Fields
                .Select((x, i) => new { Name = x.Name, Index = i })
                .ToArray();

            foreach (var column in columns)
            {
                var binding = new Binding(string.Format("Fields[{0}].Value", column.Index));
                var columnTemp = new DataGridTextColumn() { Header = column.Name, Binding = binding };
                dataGrid.Columns.Add(columnTemp);
            }

            ApplyStyles(dataGrid);

            dataGrid.Columns[0].IsReadOnly = true;
            dataGrid.ItemsSource = records;
        }

        public void ApplyStyles(DataGrid dataGrid)
        {
            dataGrid.ColumnHeaderStyle = (Style)dataGrid.Resources["HeaderStyle"];

            if(dataGrid.Columns.Any())
            {
                dataGrid.Columns[0].HeaderStyle = (Style)dataGrid.Resources["FirstColumnHeaderStyle"]; ;
                dataGrid.Columns[0].CellStyle = (Style)dataGrid.Resources["LineNumberColumnStyle"];
            }
            
        }

    }
}
