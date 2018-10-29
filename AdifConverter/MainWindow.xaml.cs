using AdifConverter.ViewModels;
using AdifConverter.Views;
using Microsoft.Win32;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AdifConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ADIFRecordViewModel ADIFRecordViewModel { get; set; }

        public bool ManualCommit = false;

        public MainWindow(ADIFRecordViewModel adifRecordViewModel)
        {
            InitializeComponent();            

            ADIFRecordViewModel = adifRecordViewModel;

            dataGridAdif.Visibility = Visibility.Hidden;
            DisableSave();
            lblStatusBar.Text = $"QSOs: {ADIFRecordViewModel.Records.Count}";

            DataContext = ADIFRecordViewModel;

            var binding = new Binding("Records")
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                IsAsync = true                
            };

            dataGridAdif.SetBinding(DataGrid.ItemsSourceProperty, binding);
        }

        private void MenuOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "ADIF files (*.adi)|*.adi"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ADIFRecordViewModel.Records.Clear();                
                dataGridAdif.Items.Refresh();
                dataGridAdif.Columns.Clear();

                dataGridAdif.Visibility = Visibility.Hidden;               

                ADIFRecordViewModel.ReadRecords(openFileDialog.FileName);

                if (!ADIFRecordViewModel.Records.Any())
                {
                    this.Title = $"{Properties.Resources.ApplicationName}";
                    lblStatusBar.Text = $"QSOs: {ADIFRecordViewModel.Records.Count}";
                    return;
                }                

                ADIFRecordViewModel.SetupGrid(dataGridAdif);
                ADIFRecordViewModel.MainGrid = MainGrid;
                ADIFRecordViewModel.CurrentOpenedFileName = openFileDialog.SafeFileName;

                dataGridAdif.Visibility = Visibility.Visible;
                EnableSave();
                
                this.Title = $"{Properties.Resources.ApplicationName} - {openFileDialog.FileName}";
                lblStatusBar.Text = $"QSOs: {ADIFRecordViewModel.Records.Count}";

                openFileDialog = null;
            }
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();            
        }

        private void EnableSave()
        {
            saveCsv.IsEnabled = true;
            saveXlsx.IsEnabled = true;
            savePlanillaOA.IsEnabled = true;
        }

        private void DisableSave()
        {
            saveCsv.IsEnabled = false;
            saveXlsx.IsEnabled = false;
            savePlanillaOA.IsEnabled = false;
        }

        private void DataGridAdif_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) { }

        private void DataGridAdif_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) { }

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
