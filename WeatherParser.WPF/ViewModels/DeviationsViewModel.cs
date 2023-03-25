
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
    internal class DeviationsViewModel : NotifyPropertyChangedBase
    {

        #region ctor
        public DeviationsViewModel()
        {
            Series = new ObservableCollection<ISeries>();

            XAxes = new ObservableCollection<Axis>()
            {
                new Axis()
                {
                    LabelsPaint = new SolidColorPaintTask(SKColors.Black),
                    Labels = new ObservableCollection<string>()
                }
            };
            YAxes = new ObservableCollection<Axis>() { new Axis() };
        }

        #endregion

        #region props
        public ObservableCollection<Axis> XAxes { get; set; }
        public ObservableCollection<Axis> YAxes { get; set; }

        public ObservableCollection<ISeries> Series { get; }

        public List<WeatherDataPresentation> WeatherDataPresentations { get; set; }

        #endregion

        #region public

        public void ExecuteCommand(ICommand command, DateTime? selectedDate, ObservableCollection<TimeViewModel> times)
        {
            Series.Clear();
            command.Execute(WeatherDataPresentations, selectedDate, Series, times, XAxes);
        }

        #endregion
    }
}
