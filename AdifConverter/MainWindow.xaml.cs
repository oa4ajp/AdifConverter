using AdifConverter.ADIF;
using AdifConverter.Controllers;
using AdifConverter.Models;
using AdifConverter.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ObservableCollection<ADIFRecord> Records { get; set; }

        public bool ManualCommit = false;

        public MainWindow()
        {
            InitializeComponent();
            Records = new ObservableCollection<ADIFRecord>();                        

            dataGridAdif.Visibility = Visibility.Hidden;
            DisableSave();
            lblStatusBar.Text = $"QSOs: {Records.Count}";

            DataContext = this;

            var binding = new Binding("Records")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            dataGridAdif.SetBinding(DataGrid.ItemsSourceProperty, binding);
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

                if (!Records.Any()) return;

                var gridController = new DataGridController();  
                gridController.SetupGrid(dataGridAdif, Records);

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

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {            
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}
