using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для UpdaterWindow.xaml
    /// </summary>
    public partial class UpdaterWindow : Window
    {
        BackgroundWorker worker = new BackgroundWorker();
        private bool isWorkerRun = false;
        private int Iteration;

        public UpdaterWindow()
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            ProgressBar.IsIndeterminate = false;
            ProgressBar.Visibility = Visibility.Visible;
            ProgressBar.Value = 0;
            Iteration = 1000;
        }

        public void RunWorker()
        {
            if (worker.IsBusy != true)
            {
                ProgressBar.IsIndeterminate = true;
                isWorkerRun = true;
                worker.RunWorkerAsync();    // Start the asynchronous operation.
            }
        }

        public void StopWorker()
        {
            isWorkerRun = false;            
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int i = Iteration; i >= 0; i--)
            {
                System.Threading.Thread.Sleep(1);
                if (isWorkerRun == false)
                {
                    e.Cancel = true;                                   
                }
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                worker.CancelAsync();
                ProgressBar.IsIndeterminate = false;
                // isWorkerRun = false;
                Close();
            }
        }

        public bool GetRunWorker()
        {
            return isWorkerRun;
        }

    }
}
