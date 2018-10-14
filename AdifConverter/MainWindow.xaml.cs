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

        public MainWindow()
        {
            InitializeComponent();
            Records = new List<ADIFRecord>();
            dataGridAdif.Visibility = Visibility.Hidden;
            DisableSave();
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ADIF files (*.adi)|*.adi";
            if (openFileDialog.ShowDialog() == true)
            {
                Records.Clear();                
                dataGridAdif.Items.Clear();
                dataGridAdif.Items.Refresh();
                dataGridAdif.Columns.Clear();

                dataGridAdif.Visibility = Visibility.Hidden;

                var adifRecordController = new ADIFRecordController();
                Records = adifRecordController.ReadRecords(openFileDialog.FileName);
                
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

                dataGridAdif.ItemsSource = Records;

                dataGridAdif.Visibility = Visibility.Visible;
                EnableSave();
            }
        }

        private void MenuSaveCSV_Click(object sender, RoutedEventArgs e)
        {
            if (!Records.Any()) {
                MessageBox.Show("No records found");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Csv file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                var csvController = new CSVController(saveFileDialog.FileName);
                csvController.SaveCsv(Records);                
                MessageBox.Show($"{saveFileDialog.SafeFileName} saved.");
            }
        }

        private void MenuSavePlanillaCSV_Click(object sender, RoutedEventArgs e)
        {
            if (!Records.Any())
            {
                MessageBox.Show("No records found");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Csv file (*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == true)
            {
                var csvController = new CSVController(saveFileDialog.FileName);
                csvController.SavePlanillaCsv(Records);
                MessageBox.Show($"{saveFileDialog.SafeFileName} saved.");
            }
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
    }
}
