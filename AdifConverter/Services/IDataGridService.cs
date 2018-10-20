﻿using AdifConverter.Models;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace AdifConverter.Services
{
    public interface IDataGridService
    {
        void SetupGrid(DataGrid dataGrid, ObservableCollection<ADIFRecord> records);
        void ApplyStyles(DataGrid dataGrid);
    }
}
