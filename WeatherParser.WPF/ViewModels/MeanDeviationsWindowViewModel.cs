using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.ViewModels
{
    internal class MeanDeviationsWindowViewModel : NotifyPropertyChangedBase
    {
        #region fields

        private List<WeatherDataPresentation> _weatherMeanDeviationsList;

        private List<WeatherDataPresentation> _weatherDeviationsList;

        #endregion

        #region ctor
        public MeanDeviationsWindowViewModel()
        {
            SeriesMean = new ObservableCollection<ISeries>();
            SeriesDeviations = new ObservableCollection<ISeries>();

            XAxesMean = new ObservableCollection<Axis>()
            {
                new Axis()
                {
                    LabelsPaint = new SolidColorPaintTask(SKColors.Black),
                    Labels = new ObservableCollection<string>()
                }
            };
            YAxesMean = new ObservableCollection<Axis>() { new Axis() };

            XAxesDeviations = new ObservableCollection<Axis>()
            {
                new Axis()
                {
                    LabelsPaint = new SolidColorPaintTask(SKColors.Black),
                    Labels = new ObservableCollection<string>()
                }
            };
            YAxesDeviations = new ObservableCollection<Axis>() { new Axis() };
        }

        #endregion

        #region props
        public ObservableCollection<Axis> XAxesMean { get; set; }
        public ObservableCollection<Axis> XAxesDeviations { get; set; }
        public ObservableCollection<Axis> YAxesMean { get; set; }
        public ObservableCollection<Axis> YAxesDeviations { get; set; }

        public ObservableCollection<ISeries> SeriesMean { get; }
        public ObservableCollection<ISeries> SeriesDeviations { get; }

        #endregion

        #region public

        #endregion
    }
}
