
using Autofac;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.Commands;

namespace WeatherParser.WPF.ViewModels
{
    internal class MeanDeviationsWindowViewModel : NotifyPropertyChangedBase
    {

        #region ctor
        public MeanDeviationsWindowViewModel()
        {
            SeriesMean = new ObservableCollection<ISeries>();

            XAxesMean = new ObservableCollection<Axis>()
            {
                new Axis()
                {
                    LabelsPaint = new SolidColorPaintTask(SKColors.Black),
                    Labels = new ObservableCollection<string>()
                }
            };
            YAxesMean = new ObservableCollection<Axis>() { new Axis() };
        }

        #endregion

        #region props
        public ObservableCollection<Axis> XAxesMean { get; set; }
        public ObservableCollection<Axis> YAxesMean { get; set; }

        public ObservableCollection<ISeries> SeriesMean { get; }

        public List<WeatherDataPresentation> WeatherDataPresentations { get; set; }

        #endregion

        #region public

        public void ExecuteCommand(ICommand command, DateTime? selectedDate, ObservableCollection<TimeViewModel> times)
        {
            SeriesMean.Clear();
            command.Execute(WeatherDataPresentations, selectedDate, SeriesMean, times, XAxesMean);
        }

        #endregion
    }
}
