using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.ViewModels
{
    internal class ExcelWindowViewModel : NotifyPropertyChangedBase
    {
        #region fields

        private WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient _weatherParserService;

        private SitePresentation _selectedSite;

        private string _fileName;

        private string _filePath;

        private bool _isAllDataSelected = false;
        #endregion

        #region ctor

        public ExcelWindowViewModel(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService)
        {
            _weatherParserService = weatherParserService;

            ChooseFolderCommand = new RelayCommand(ChooseFolder);
            SaveInExcelCommand = new RelayCommand(SaveInExcel, x => _isAllDataSelected);

            var sitesService = _weatherParserService.GetSites(new Empty());
            Sites = new ObservableCollection<SitePresentation>();

            foreach (var site in sitesService.Sites)
            {
                Sites.Add(new SitePresentation() { ID = new Guid(site.SiteId), Name = site.SiteName });
            }
        }

        #endregion

        #region props
        public RelayCommand SaveInExcelCommand { get; }

        public RelayCommand ChooseFolderCommand { get; }

        public string FileName
        {
            get => _fileName;
            set
            {
                OnPropertyChanged(value, ref _fileName);
                _isAllDataSelected = CheckAllData();
                SaveInExcelCommand.NotifyCanExecuteChanged();
            }
        }

        public SitePresentation SelectedSite
        {
            get => _selectedSite;
            set
            {
                OnPropertyChanged(value, ref _selectedSite);
                _isAllDataSelected = CheckAllData();
                SaveInExcelCommand.NotifyCanExecuteChanged();
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                OnPropertyChanged(value, ref _filePath);
                _isAllDataSelected = CheckAllData();
                SaveInExcelCommand.NotifyCanExecuteChanged();
            }
        }

        public ObservableCollection<SitePresentation> Sites
        {
            get;
        } = new ObservableCollection<SitePresentation>();

        #endregion

        private void ChooseFolder(object? obj)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    FilePath = folderBrowser.SelectedPath;
                }
            }
        }

        public void SaveInExcel(object? obj)
        {
            try
            {
                if (!_fileName.ToLower().Contains(".xlsx") && !_fileName.ToLower().Contains(".xls"))
                {
                    _fileName = _fileName + ".xlsx";
                }
                _weatherParserService.SaveDataInExcel(new SaveExcelRequest() { Path = _filePath + '\\' + _fileName, SiteID = _selectedSite.ID.ToString() });
                System.Windows.MessageBox.Show("Saving finish", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                ((Window)obj).Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ((Window)obj).Close();
            }
        }

        private bool CheckAllData()
        {
            return !string.IsNullOrEmpty(_fileName) && !string.IsNullOrEmpty(_filePath) && _selectedSite != null;
        }
    }
}
