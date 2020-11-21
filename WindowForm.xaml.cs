using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using System.Windows.Shapes;

namespace Wpf_Forms
{
    /// <summary>
    /// Логика взаимодействия для WindowForn.xaml
    /// </summary>
    public partial class WindowForm : Window
    {
        public Calibration m_Calib = new Calibration();
        private BackgroundWorker worker = new BackgroundWorker();

        private float U1_Plus;
        private float U1_Minus;
        private float U2_Plus;
        private float U2_Minus;

        private ushort U1_PlusCode;
        private ushort U1_MinusCode;
        private ushort U2_PlusCode;
        private ushort U2_MinusCode;

        private ushort I_PlusCode;
        private ushort I_MinusCode;
        private int RealValue;

        private int IntValue
        {
            get
            {
                return RealValue;
            }
             
            set
            {
                RealValue = value;
            }
        }
        public WindowForm()
        {
            InitializeComponent();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void btn_U1_Click(object sender, RoutedEventArgs e)
        {
            U1_Plus = 0.01f;            
            U1_PlusCode = 0x01;
            IntValue = 0x05;
           
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            
            // worker.RunWorkerAsync();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = Convert.ToDouble( e.ProgressPercentage);
        }
        public Window GetForm()
        {
            return secondForm;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // This is called on the UI thread when the DoWork method completes
            // so it's a good place to hide busy indicators, or put clean up code
            worker.CancelAsync();
        }

        private void btn_U2_Click(object sender, RoutedEventArgs e)
        {
            U2_Plus = 100.0f;
            U2_PlusCode = 8500;
        }

        private void btn_Calc_Click(object sender, RoutedEventArgs e)
        {

            if (worker.IsBusy != true)
            {
                // Start the asynchronous operation.
                worker.RunWorkerAsync();
            }
            
            m_Calib.Set_UPlus(U1_Plus, U2_Plus);
            m_Calib.Set_UPlusCode(U1_PlusCode, U2_PlusCode);
            m_Calib.Calc_Koeff_Uplus();

            koeff_B.Text = Convert.ToString(m_Calib.B_Up);
            koeff_K.Text = Convert.ToString(m_Calib.K_Up);
        }

        private void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            secondForm.Close();
            App.Current.MainWindow.Show();
        }

        public void Parse(string str, ref float Value)
        {

            if (str == null)
                return;
            str = str.Replace(",", ".");

            try
            {
                Value = float.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Введите число !");
                Value = 0.0f;
            }
        }
    }
}
