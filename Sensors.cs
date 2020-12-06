using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;

namespace Wpf_Forms
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Sensors
    {
        private bool flg_RqCmplt_1;                   // Флаг опроса Датчиков 1-го Типа 
        private bool flg_RqCmplt_2;                   // Флаг опроса Датчиков 2-го Типа
        private bool flg_RqCmplt_3;                   // Флаг опроса Датчиков 3-го Типа
        private bool flg_RqCmplt_4;                   // Флаг опроса Датчиков 4-го Типа

        private byte status_Konfig_PLC;            // статус Конфигурации ПЛК
        private byte status_Konfig_DSK;            // статус Конфигурации Desktop
        private byte status_Konfig_USPD;           // статус Конфигурации УСПД

        private byte Valid_Data;                   // данные с УСПД получены 
        private byte id_PPM;                       // Адрес ППМ 
        private byte statusKomplex;                // статус работы Комплекса 
        private short Val_Hard_ID;                // Идентификатор Контроллера
        private short Val_Soft_Ver;               // Версия ПО

        private byte[] IP_ADDR;
        private byte[] NETMASK_ADDR;
        private byte[] GATEWAY_ADDR;
        private byte[] SNTP_ADDR;
        private byte[] MAC_ADDR;
        private byte[] BufferSensorActivity;           // 512
        private char[] deviceNameString;               // 20

        public void SetDefaultParams()
        {
            flg_RqCmplt_1 = true;
            flg_RqCmplt_2 = true;
            flg_RqCmplt_3 = true;
            flg_RqCmplt_4 = true;

            status_Konfig_PLC = 1;
            status_Konfig_DSK = 1;
            status_Konfig_USPD = 1;

            Valid_Data = 0;
            id_PPM = 0x0A;
            statusKomplex = 1;
            Val_Hard_ID = 0x01;
            Val_Soft_Ver = 0x0101;
        }

        public byte[] GetSerialize()
        {
            string path = "sensors.dat";
            byte[] buffer;

            // BinaryFormatter formatter = new BinaryFormatter();
            // MemoryStream mstream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            
            writer.Write(flg_RqCmplt_1);
            writer.Write(flg_RqCmplt_2);
            writer.Write(flg_RqCmplt_3);
            writer.Write(flg_RqCmplt_4);

            writer.Write(status_Konfig_PLC);
            writer.Write(status_Konfig_DSK);
            writer.Write(status_Konfig_USPD);

            writer.Write(Valid_Data);
            writer.Write(id_PPM);
            writer.Write(statusKomplex);
            writer.Write(Val_Hard_ID);
            writer.Write(Val_Soft_Ver);
            writer.Close();
            // lengthBuf = writer.BaseStream.Length;

            // buffer = new byte[lengthBuf];
            buffer = File.ReadAllBytes(path);
            
            return buffer;
        }

        public void SetSerialize(byte[] buffer,int count)
        {
            // Sensors mysens = new Sensors();
            MemoryStream mstream = new MemoryStream(buffer,0,count);
            // mstream.Read(buffer, 0, buffer.Length);
            BinaryReader reader = new BinaryReader(mstream);

            flg_RqCmplt_1= reader.ReadBoolean();
            flg_RqCmplt_2 = reader.ReadBoolean();
            flg_RqCmplt_3 = reader.ReadBoolean();
            flg_RqCmplt_4 = reader.ReadBoolean();            
        }

        public int GetSizeSens()
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(Sensors));
            return size;
        }
    }
}
