using Autofac;
using System.Collections.ObjectModel;
using System;
using System.IO;
using System.Windows.Forms;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using System.Threading.Tasks;
using System.Windows;

namespace WeatherParser.WPF.ViewModels
{
    internal class ExcelWindowViewModel: NotifyPropertyChangedBase
    {
        #region fields

        private WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient _weatherParserService;

        private SitePresentation _selectedSite;

        private string _fileName;

        private string _filePath;
        #endregion

        #region ctor

        public ExcelWindowViewModel(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService)
        {
            _weatherParserService = weatherParserService;

            ChooseFolderCommand = new RelayCommand(ChooseFolder);
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
            }
        }

        public SitePresentation SelectedSite
        {
            get => _selectedSite;
            set
            {
                OnPropertyChanged(value, ref _selectedSite);
            }
        }

        #endregion

        private void ChooseFolder(object? obj)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    _filePath = folderBrowser.SelectedPath;
                }
            }
        }

        public async Task SaveInExcel()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            DialogResult result = folderBrowser.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                {
                    try
                    {
                        await _weatherParserService.SaveDataInExcelAsync(new SaveExcelRequest() { Path = _filePath + '\\' + _fileName, SiteID = _selectedSite.ID.ToString() });
                    }
                    catch (Exception ex)
                    {
                        //show error
                    }
                }
            }
        }
    }
}
