using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using RJCP.IO.Ports;


namespace Wpf_Forms
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WindowForm secondForm;
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        private float[] sinusBufer;
        List<double> signal_input = new List<double>();
        Phone phone = new Phone(); 
        
        public MainWindow()
        {
            InitializeComponent();
            
            secondForm = new WindowForm();
            GenerateSinus();
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,7 }
                },
            };

            DataContext = this;
        }

        private void btn_NewForm_Click(object sender, RoutedEventArgs e)
        {
            //modifying any series values will also animate and update the chart

            SeriesCollection.Add(new LineSeries
            {
                Values = new ChartValues<double>(signal_input.ToArray()),
                LineSmoothness = 1 //straight lines, 1 really smooth lines
            }); 
                     

            // mainWindow.Hide();           
            // secondForm.Show();
        }

        private void GenerateSinus()
        {
            double t,sign = 0;
            double samples = 100;
            double freq = 200;
            t = 0;

            for(int i=0;i< samples; i++)
            {
                t = t + (1.0 / freq);
                sign = (Math.Sin(2*Math.PI* samples*t) / freq) * 100;
                signal_input.Add(sign);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
                SeriesCollection.Clear();
        }

        private byte[] ReceiveData(SerialPortStream sp, int size)
        {
            var buffer = new byte[size];
            var dataReceived = 0;
            while (dataReceived < size)
            {
                dataReceived += sp.Read(buffer, dataReceived, size - dataReceived);
            }
            return buffer;
        }
    }
}
