using AdifConverter.ADIF;
using AdifConverter.Controllers;
using AdifConverter.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdifConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ADIFRecord> Records { get; set; }

        public bool ManualCommit = false;

        public MainWindow()
        {
            InitializeComponent();
            Records = new List<ADIFRecord>();
            dataGridAdif.Visibility = Visibility.Hidden;
            DisableSave();
            lblStatusBar.Text = $"QSOs: {Records.Count}";
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ADIF files (*.adi)|*.adi";
            if (openFileDialog.ShowDialog() == true)
            {
                Records.Clear();                
                dataGridAdif.Items.Refresh();
                dataGridAdif.Columns.Clear();

                dataGridAdif.Visibility = Visibility.Hidden;

                var adifRecordController = new ADIFRecordController();
                Records = adifRecordController.ReadRecords(openFileDialog.FileName);
                
                if(!Records.Any()) return;

                var columns = Records.First()
                    .Fields
                    .Select((x, i) => new { Name = x.Name, Index = i })
                    .ToArray();

                foreach (var column in columns)
                {
                    var binding = new Binding(string.Format("Fields[{0}].Value", column.Index));
                    var columnTemp = new DataGridTextColumn() { Header = column.Name, Binding = binding };
                    dataGridAdif.Columns.Add(columnTemp);
                }

                var colorGray = (Color)ColorConverter.ConvertFromString("#F0F0F0");
                var brushGray = new SolidColorBrush(colorGray);

                var lineNumberColumnStyle = new Style
                {
                    TargetType = typeof(DataGridCell) //sets target type as DataGrid cell
                };

                var setterLineNumberColumnBackground = new Setter
                {
                    Property = DataGridCell.BackgroundProperty,
                    Value = brushGray
                };

                var setterLineNumberColumnForeground = new Setter
                {
                    Property = DataGridCell.ForegroundProperty,
                    Value = Brushes.Black
                };

                var setterLineColumnBorder = new Setter
                {
                    Property = DataGridCell.BorderBrushProperty,
                    Value = Brushes.Black
                };

                var setterLineColumnBorderThickness = new Setter
                {
                    Property = DataGridCell.BorderThicknessProperty,
                    Value = new Thickness(1, 0, 0, 0)
                };

                lineNumberColumnStyle.Setters.Add(setterLineNumberColumnBackground);
                lineNumberColumnStyle.Setters.Add(setterLineColumnBorder);
                lineNumberColumnStyle.Setters.Add(setterLineColumnBorderThickness);
                lineNumberColumnStyle.Setters.Add(setterLineNumberColumnForeground);

                var styleHeader = new Style
                {
                    TargetType = typeof(DataGridColumnHeader) //sets target type as DataGrid cell
                };

                var setterBackgroundHeader = new Setter
                {
                    Property = DataGridCell.BackgroundProperty,
                    Value = brushGray
                };

                var setterBorder = new Setter
                {
                    Property = DataGridCell.BorderBrushProperty,
                    Value = Brushes.Black
                };

                var setterBorderThickness = new Setter
                {
                    Property = DataGridCell.BorderThicknessProperty,
                    Value = new Thickness(0, 0, 1, 1)
                };

                var setterPadding = new Setter
                {
                    Property = DataGridCell.PaddingProperty,
                    Value = new Thickness(6, 3, 6, 3)
                };

                styleHeader.Setters.Add(setterBackgroundHeader);
                styleHeader.Setters.Add(setterBorder);
                styleHeader.Setters.Add(setterBorderThickness);
                styleHeader.Setters.Add(setterPadding);

                var styleHeaderFirstColumn = new Style
                {
                    TargetType = typeof(DataGridColumnHeader) 
                };

                var setterBackgroundHeaderFirstColumn = new Setter
                {
                    Property = DataGridCell.BackgroundProperty,
                    Value = brushGray
                };

                var setterBorderFirstColumn = new Setter
                {
                    Property = DataGridCell.BorderBrushProperty,
                    Value = Brushes.Black
                };

                var setterBorderThicknessFirstColumn = new Setter
                {
                    Property = DataGridCell.BorderThicknessProperty,
                    Value = new Thickness(1, 0, 1, 1)
                };

                var setterPaddingFirstColumn = new Setter
                {
                    Property = DataGridCell.PaddingProperty,
                    Value = new Thickness(6, 3, 6, 3)
                };

                styleHeaderFirstColumn.Setters.Add(setterBackgroundHeaderFirstColumn);
                styleHeaderFirstColumn.Setters.Add(setterBorderFirstColumn);
                styleHeaderFirstColumn.Setters.Add(setterBorderThicknessFirstColumn);
                styleHeaderFirstColumn.Setters.Add(setterPaddingFirstColumn);

                dataGridAdif.ColumnHeaderStyle = styleHeader;

                dataGridAdif.Columns[0].HeaderStyle = styleHeaderFirstColumn;
                dataGridAdif.Columns[0].CellStyle = lineNumberColumnStyle;
                dataGridAdif.Columns[0].IsReadOnly = true;

                dataGridAdif.ItemsSource = Records;

                dataGridAdif.Visibility = Visibility.Visible;
                EnableSave();
                
                this.Title = $"{Properties.Resources.ApplicationName} - {openFileDialog.FileName}";
                lblStatusBar.Text = $"QSOs: {Records.Count}";

                openFileDialog = null;
            }
        }

        private void MenuSaveCSV_Click(object sender, RoutedEventArgs e)
        {
            if (!Records.Any()) {
                MessageBox.Show("No records found", Properties.Resources.ApplicationName);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Csv file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                var csvController = new CSVController(saveFileDialog.FileName);
                csvController.SaveCsv(Records);                
                MessageBox.Show($"{saveFileDialog.SafeFileName} saved.", Properties.Resources.ApplicationName);
            }
            saveFileDialog = null;
        }

        private void MenuSavePlanillaCSV_Click(object sender, RoutedEventArgs e)
        {
            if (!Records.Any())
            {
                MessageBox.Show("No records found", Properties.Resources.ApplicationName);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Csv file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                var csvController = new CSVController(saveFileDialog.FileName);
                csvController.SavePlanillaCsv(Records);
                MessageBox.Show($"{saveFileDialog.SafeFileName} saved.", Properties.Resources.ApplicationName);
            }
            saveFileDialog = null;
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();            
        }

        private void EnableSave()
        {
            saveCsv.IsEnabled = true;
            savePlanillaOA.IsEnabled = true;
        }

        private void DisableSave()
        {
            saveCsv.IsEnabled = false;
            savePlanillaOA.IsEnabled = false;
        }

        private void DataGridAdif_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
        }

        private void DataGridAdif_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
        }

        private void DataGridAdif_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!ManualCommit)
            {
                ManualCommit = true;
                dataGridAdif.CommitEdit(DataGridEditingUnit.Row, true);
                ManualCommit = false;
            }
        }
    }
}
