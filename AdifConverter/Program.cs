using System;
using System.Windows;
using SimpleInjector;
using AdifConverter.ViewModels;
using AdifConverter.Services;
using AdifConverter.Services.Interfaces;
using AdifConverter.Strategy;

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
            container.Register<IDataGridService, DataGridService>();
            container.Register<IADIFRecordService, ADIFRecordService>();            
            container.Register<IADIFFieldService, ADIFFieldService>();

            //Strategy Pattern
            container.RegisterConditional<IFileServiceStrategy, FileCSVService>(WithParamName("fileCSVService"));
            container.RegisterConditional<IFileServiceStrategy, FileCSVPlanillaService>(WithParamName("fileCSVPlanillaService"));
            container.RegisterConditional<IFileServiceStrategy, FileOpenXmlService>(WithParamName("fileOpenXmlService"));

            container.RegisterConditional<IOpenXmlRowBuilderStrategy, OpenXmlRowHeaderBuilder>(WithParamName("openXmlRowHeaderBuilder"));
            container.RegisterConditional<IOpenXmlRowBuilderStrategy, OpenXmlRowDataBuilder>(WithParamName("openXmlRowDataBuilder"));
            
            // Register your windows and view models:
            container.Register<MainWindow>();
            container.Register<ADIFRecordViewModel>();

            //It call the Mainwindow Constructor
            container.Verify();

            return container;
        }

        // Helper method to compare against the Constructor's parameter name
        private static Predicate<PredicateContext> WithParamName(string name)
        {
            return c => c.Consumer.Target.Name == name;
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
                throw;
            }
        }
    }
}
