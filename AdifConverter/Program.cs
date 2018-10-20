using System;
using System.Windows;
using SimpleInjector;
using AdifConverter.ViewModels;
using AdifConverter.Services;

namespace AdifConverter
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var container = Bootstrap();
            // Any additional other configuration, e.g. of your desired MVVM toolkit.
            RunApplication(container);
        }

        private static Container Bootstrap()
        {
            // Create the container as usual.
            var container = new Container();

            // Register your types, for instance:
            container.Register<ICSVService, CSVService>();
            container.Register<IDataGridService, DataGridService>();
            container.Register<IADIFFieldService, ADIFFieldService>();

            // Register your windows and view models:
            container.Register<MainWindow>();
            container.Register<ADIFRecordViewModel>();

            container.Verify();

            return container;
        }

        private static void RunApplication(Container container)
        {
            try
            {
                var app = new App();
                var mainWindow = container.GetInstance<MainWindow>();
                app.Run(mainWindow);
            }
            catch (Exception ex)
            {
                //Log the exception and exit
            }
        }
    }
}
