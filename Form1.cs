using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Win32;
using FirebirdSql.Data.FirebirdClient;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string someText = "";
            //Если в буфере обмен содержится текст
            if (Clipboard.ContainsText() == true)
            {
                //Извлекаем (точнее копируем) его и сохраняем в переменную
                someText = Clipboard.GetText();

                //Выводим показываем сообщение с текстом, скопированным из буфера обмена
             //   MessageBox.Show(this, someText, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
             //   return;
            }
            else
            {
                //Выводим сообщение о том, что в буфере обмена нет текста
                MessageBox.Show(this, "В буфере обмена нет текста", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string strCon = GetConnectionString();

            FbConnectionStringBuilder fb_con = new FbConnectionStringBuilder();
            fb_con.Charset = "WIN1251";     //используемая кодировка
            fb_con.UserID = "SYSDBA";      //логин
            fb_con.Password = "masterkey"; //пароль
            fb_con.Database = @strCon;     //путь к файлу базы данных
            fb_con.ServerType = 0;         //указываем тип сервера 

            //создаем подключение
            var fb = new FbConnection(fb_con.ToString()); //передаем нашу строку подключения объекту класса FbConnection
            string MarkCod = @"select mark_code
                                  from  mark_codes
                                  where mark_codes.sgtin = '" + someText + @"'";

            FbDataAdapter myAdapter = new FbDataAdapter(MarkCod, fb);
            DataSet ds = new DataSet();
            myAdapter.Fill(ds);

            //            MessageBox.Show(" подключение.");
            string mark_code = "";
            foreach (DataRow row in ds.Tables[0].Rows) 
            {
                if (row.ItemArray[0].ToString() != "" )
                {
                    mark_code = row.ItemArray[0].ToString();
                    break;
                }
            }
            try
            {
            if (mark_code.Substring(0,2)=="01") 
            {
            Clipboard.SetText(mark_code);
            MessageBox.Show(this, "Нашла. Поместила.", "Сообщение", MessageBoxButtons.OK);
            }
            else 
            { MessageBox.Show(this, " Формат SGTIN  не Але! "+ mark_code, "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

            }
            catch
            {
                MessageBox.Show(this, "Возможно, в буфере нет SGTIN. Не нашла.", "Предупреждение!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        public static string GetConnectionString()
        {
            string path1 = "       ";
            string strConnect = "   ";
            if (System.IO.File.Exists(@"c:\IApteka\IApteka.ini"))
            { path1 = @"c:\IApteka\IApteka.ini"; }
            if (System.IO.File.Exists(@"D:\IApteka\IApteka.ini"))
            { path1 = @"D:\IApteka\IApteka.ini"; }
            if (System.IO.File.Exists(@"E:\IApteka\IApteka.ini"))
            { path1 = @"E:\IApteka\IApteka.ini"; }
            foreach (string line in System.IO.File.ReadLines(path1))
            {
                if (line.IndexOf("Path") == 0)
                {
                    strConnect = line.Substring(5, line.Length - 5);
                }

            }
            return (strConnect);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey Connect = currentUserKey.OpenSubKey("Connect");

            Connect.Close();

        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            // сохраняем путь в реестре
            RegistryKey localMachineKey = Registry.LocalMachine;
            RegistryKey currentUserKey = Registry.CurrentUser;

            RegistryKey Connect = currentUserKey.CreateSubKey("Connect");

            Connect.Close();
        }
    }
}
