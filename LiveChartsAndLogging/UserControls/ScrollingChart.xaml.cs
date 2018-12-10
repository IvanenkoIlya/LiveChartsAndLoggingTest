using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LiveChartsAndLogging.UserControls
{
    /// <summary>
    /// Interaction logic for ScrollingChart.xaml
    /// </summary>
    public partial class ScrollingChart : UserControl, INotifyPropertyChanged
    {
        private SeriesCollection _dataSet;
        public SeriesCollection DataSet
        {
            get { return _dataSet; }
            set { _dataSet = value; OnPropertyChanged("DataSet"); }
        }

        public ScrollingChart()
        {
            InitializeComponent();

            DataSet = new SeriesCollection
            {
                new LineSeries
                {
                    AreaLimit = 0,
                    Values = new ChartValues<double>
                    {
                        1,2,4,7,10,12,13,13,12,10,7,4,2,1,1,2,4,7,10,12,13,13,12,10,7,4,2,1,1,2,4,7,10,12,13,13,12,10,7,4,2,1,1,2,4,7,10,12,13,13,12,10,7,4,2,1,1,2,4,7,10,12,13,13,12,10,7,4,2,1,1,2,4,7,10,12,13,13,12,10,7,4,2,1,
                    }
                }
            };

            DataContext = this;

            Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    var temp = sw.ElapsedMilliseconds;
                    Thread.Sleep(200);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var val = DataSet[0].Values[0];
                        DataSet[0].Values.RemoveAt(0);
                        DataSet[0].Values.Add(val);
                        MyChart.Update(false, true);
                    });
                    Console.WriteLine(sw.ElapsedMilliseconds - temp);
                }
            });

            
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
