using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
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
using OfficeOpenXml;
using RJCP.IO.Ports;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;

namespace Wpf_Forms
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Serializable]
        unsafe struct ByteBufer
        {
            public fixed byte testByte[6];
            public fixed byte FidName[20];
        }

        WindowForm secondForm;
        UpdaterWindow updaterWindow;
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
        private float[] sinusBufer;
        List<double> signal_input = new List<double>();
        ObservableCollection<Phone> phoneCollect { get; set; }

        public Phone phone;
        List<Phone> myPhonesList = new List<Phone>();
        // public ExcelPackage package;
        public SerialPortStream serial = new SerialPortStream();

        public Sensors mysens;
        private ByteBufer bufer;

        [Serializable]
        unsafe struct Point
        {
            public byte W;
            public byte Z;
            public int X;
            public int Y;
        }

        public MainWindow()
        {
            InitializeComponent();                        
            GenerateSinus();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,7 }
                },
            };

            phoneCollect = new ObservableCollection<Phone>();                       
            serial.PortName = "COM1";
            serial.BaudRate = 115200;
            serial.Parity = Parity.None;
            serial.DataBits = 8;
            serial.Open();
            mysens = new Sensors();
            myPhonesList.Capacity = 512;
            PhoneListAdding();
            phoneCollect.Clear();
                    
            // InitPhones();
        }

        private void btn_NewForm_Click(object sender, RoutedEventArgs e)
        {
            //modifying any series values will also animate and update the chart
            // secondForm = new WindowForm();
            // secondForm.Show();

            updaterWindow = new UpdaterWindow();
            // updaterWindow.Owner = this;
            updaterWindow.Show();
                updaterWindow.RunWorker();
                SeriesCollection.Add(new LineSeries
                {
                    Values = new ChartValues<double>(signal_input.ToArray()),
                    LineSmoothness = 1 //straight lines, 1 really smooth lines
                });
 
             updaterWindow.StopWorker();
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

        private void PhoneListAdding()
        {
            string[] myList;
            phone = new Phone();
            phone.Status.Add("Activity");
            phone.Status.Add("Bliding");
            phone.Status.Add("Striking");
            phone.Status.Add("Running");
            phone.Status.Add("Hiking");

            myList = phone.Status.ToArray();
            
            foreach (string port in myList)
            {
                CmbBoxType.Items.Add(port);
            }

            CmbBoxType.SelectedIndex = 1;
        }

        private void Btn_AddSensors_Click(object sender, RoutedEventArgs e)
        {
            SensorsDataGrid.ItemsSource = phoneCollect;
            phone = new Phone();
            if (TxtBoxPrice.Text != "")
            {
                phone.Price = Convert.ToInt32(TxtBoxPrice.Text);
            }
            else
            {
                phone.Price = 1;
            }

            phone.StateType = CmbBoxType.SelectedItem.ToString();
            phone.Company = "Apple";
            phone.Title = "Phones";
            myPhonesList.Add(phone);
            phoneCollect.Add(phone);
        }

        private void Btn_DeleteSensors_Click(object sender, RoutedEventArgs e)
        {
            if(myPhonesList.Count != 0)
            {
                phone = new Phone();
                phone = myPhonesList[myPhonesList.Count - 1];
                phoneCollect.Remove(phone);
                // SensorsDataGrid.Items.RemoveAt(myPhonesList.IndexOf(phone, myPhonesList.Count - 1));       // Удалить из Спсика
                myPhonesList.RemoveAt(myPhonesList.IndexOf(phone,myPhonesList.Count-1));                  // Удалить из Колллекции
            }
            else
            {
                MessageBox.Show("Нечего Удалять!");
            }

        }

        private void Btn_ToExcel_Click(object sender, RoutedEventArgs e)
        {
            byte[] buffer;

            // FileStream instream = new FileStream(@"Worksheet.xlsx", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileInfo fileInfo = new FileInfo("Worksheet.xlsx");
            
            if(!fileInfo.Exists)
            {
                fileInfo.Create();
            }

            ExcelPackage package = new ExcelPackage(fileInfo);
            // MemoryStream stream = new MemoryStream();
            ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Market");
            sheet.Cells["A1"].Value = "Company:";
            sheet.Cells["B1"].Value = "Counts:";
            sheet.Cells["C1"].Value = "Hex:";

            package.Workbook.Worksheets.Add("Worksheet2", sheet);

            package.SaveAsync();
            // instream.Close();

            //GetFileInfo("myWorkBook.xlsx");
            // save our new workbook in the output directory and we are done!
            //package.SaveAs(xlFile);

            mysens.SetDefaultParams();
                       
            buffer = mysens.GetSerialize();
            serial.Flush();
            serial.WriteAsync(buffer,0,buffer.Length);
            // serial.Close();
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {            
            Close();
            Application.Current.Shutdown();
        }

        private async void Btn_FromExcel_Click(object sender, RoutedEventArgs e)
        {
            byte[] buffer;
            int countBytes = 0;
            
            buffer = new byte[mysens.GetSizeSens()];
            serial.Flush();
            serial.ReadTimeout = 2000;
            Thread.Sleep(1000);

            countBytes = await serial.ReadAsync(buffer, 0, mysens.GetSizeSens());
            mysens.SetSerialize(buffer, countBytes);
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            updaterWindow = new UpdaterWindow();
            updaterWindow.Owner = this;
            updaterWindow.Show();
            if((updaterWindow.IsLoaded) && (updaterWindow.IsInitialized))
            updaterWindow.RunWorker();
            unsafe
            {
                Point point = new Point();
                bufer = new ByteBufer();

                bufer.testByte[0] = 0xA5;
                bufer.testByte[1] = 0xA6;

                bufer.testByte[2] = 0x00;
                bufer.testByte[3] = 0x10;
                bufer.testByte[4] = 0x20;
                bufer.testByte[5] = 0x30;
                SetFidName("Присоединение №1");

                point.X = 5;
                point.Y = 10;

                MemoryStream streamBuffer = new MemoryStream();
                MemoryStream streamStruct = new MemoryStream();
                BinaryFormatter formatter = new BinaryFormatter();

                try
                {
                   //  formatter.Serialize(streamBuffer, bufer);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                    // throw;
                }

       
            // streamBuffer.Write(testByte, 0, testByte.Length);

            // point.X =  stream.ReadByte();
            // point.Y = stream.ReadByte();
            
            streamBuffer.Close();
            streamStruct.Close();
            
                // MessageBox.Show(point.X.ToString() + " " + point.Y.ToString());
            }

            updaterWindow.StopWorker();
            updaterWindow.Close();
        }

        private void SetFidName(string fidName)
        {
            bufer = new ByteBufer();
            string getfid = "";

            byte[] copyBuf = new byte[fidName.Length];
            //string name = Encoding.UTF8.GetString(fidName, 0);
            char[] textcopy;
            //fidName.CopyTo(0, textcopy, 0, fidName.Length);

            //Encoding encoding = new Encoding(866);
            copyBuf = ToByteArray(fidName, Encoding.GetEncoding(1251));    // Encoding.ASCII.GetBytes(fidName);
            // copyBuf = GetBytes(fidName);
            unsafe
            {
                for(int i=0;i<fidName.Length;i++)
                {
                    // bufer.FidName[i] = copyBuf[i]; // Convert.ToByte(textcopy[i]);
                }
            }

            getfid = ToStringArray(copyBuf, Encoding.GetEncoding(1251));    // GetString(copyBuf); // Encoding.ASCII.GetString(copyBuf);
        }

        public byte[] mGetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public string mGetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public byte[] ToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public string ToStringArray(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }
    }
}
