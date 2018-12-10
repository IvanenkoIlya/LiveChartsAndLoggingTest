using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LiveChartsAndLogging
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private double _axisMax;
        private double _axisMin;
        private double _trend;

        public MainWindow()
        {
            InitializeComponent();

            //To handle live data easily, in this case we built a specialized type
            //the MeasureModel class, it only contains 2 properties
            //DateTime and Value
            //We need to configure LiveCharts to handle MeasureModel class
            //The next code configures MeasureModel  globally, this means
            //that LiveCharts learns to plot MeasureModel and will use this config every time
            //a IChartValues instance uses this type.
            //this code ideally should only run once
            //you can configure series in many ways, learn more at 
            //http://lvcharts.net/App/examples/v1/wpf/Types%20and%20Configuration

            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            //the values property will store our values array
            ChartValues = new ChartValues<MeasureModel>();

            //lets set how to display the X Labels
            DateTimeFormatter = value => new DateTime((long)value).ToString("mm:ss");

            //AxisStep forces the distance between each separator in the X axis
            AxisStep = TimeSpan.FromSeconds(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting seconds
            //this is not always necessary, but it can prevent wrong labeling
            AxisUnit = TimeSpan.TicksPerSecond;

            SetAxisLimits(DateTime.Now);

            //The next code simulates data changes every 300 ms

            IsReading = false;

            DataContext = this;
        }

        public ChartValues<MeasureModel> ChartValues { get; set; }
        public Func<double, string> DateTimeFormatter { get; set; }
        public double AxisStep { get; set; }
        public double AxisUnit { get; set; }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged("AxisMax");
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged("AxisMin");
            }
        }

        public bool IsReading { get; set; }

        private void Read()
        {
            var r = new Random();

            int counter = 0;

            while (IsReading)
            {
                Thread.Sleep(500);
                var now = DateTime.Now;

                _trend = Math.Sin(counter / (Math.PI ));
                counter++;

                ChartValues.Add(new MeasureModel
                {
                    DateTime = now,
                    Value = _trend
                });

                SetAxisLimits(now);

                //lets only use the last 30 values
                if (ChartValues.Count > 30) ChartValues.RemoveAt(0);
                Application.Current.Dispatcher.Invoke(() => CChart.Update(false,true));
            }
        }

        private void SetAxisLimits(DateTime now)
        {
            AxisMax = now.Ticks + TimeSpan.FromSeconds(0.5).Ticks; // lets force the axis to be 1 second ahead
            AxisMin = now.Ticks - TimeSpan.FromSeconds(8).Ticks; // and 8 seconds behind
        }

        private void InjectStopOnClick(object sender, RoutedEventArgs e)
        {
            IsReading = !IsReading;
            if (IsReading) Task.Factory.StartNew(Read);
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class MeasureModel
    {
        public DateTime DateTime { get; set; }
        public double Value { get; set; }
    }
}
