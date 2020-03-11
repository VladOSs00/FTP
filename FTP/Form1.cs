using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;

namespace FTP
{
    public partial class Form1 : Form
    {
        //коллекция посещённых адрессов
        ArrayList Addresses = new ArrayList();
        //индекс текущего адресса из коллекции Adresses
        int currIndex = -1;
        //текущий адресс
        string currListViewAddress = "";
        public Form1()
        {
            InitializeComponent();
            //добавления колонок
            listView1.ColumnClick += new ColumnClickEventHandler(ClickOnColumn);
            ColumnHeader c = new ColumnHeader();
            c.Text = "Имя";
            c.Width = c.Width + 80;
            ColumnHeader c2 = new ColumnHeader();
            c2.Text = "Размер";
            c2.Width = c2.Width + 60;
            ColumnHeader c3 = new ColumnHeader();
            c3.Text = "Тип";
            c3.Width = c3.Width + 60;
            ColumnHeader c4 = new ColumnHeader();
            c4.Text = "Изменен";
            c4.Width = c4.Width + 60;
            listView1.Columns.Add(c);
            listView1.Columns.Add(c2);
            listView1.Columns.Add(c3);
            listView1.Columns.Add(c4);

            //добавления колонок
            listView2.ColumnClick += new ColumnClickEventHandler(ClickOnColumn);
            ColumnHeader c5 = new ColumnHeader();
            c5.Text = "Имя";
            c5.Width = c.Width + 80;
            ColumnHeader c6 = new ColumnHeader();
            c6.Text = "Размер";
            c6.Width = c2.Width + 60;
            ColumnHeader c7 = new ColumnHeader();
            c7.Text = "Тип";
            c7.Width = c3.Width + 60;
            ColumnHeader c8 = new ColumnHeader();
            c8.Text = "Изменен";
            c8.Width = c4.Width + 60;
            listView1.Columns.Add(c5);
            listView1.Columns.Add(c6);
            listView1.Columns.Add(c7);
            listView1.Columns.Add(c8);
            //заполнение TreeView узлами локальных дисков и заполнение дочерних узлов этих дисков
            string[] str = Environment.GetLogicalDrives();
            int n = 1;
            foreach (string s in str)
            {
                try
                {
                    TreeNode tn = new TreeNode();
                    tn.Name = s;
                    tn.Text = "Локальный диск " + s;
                    treeView1.Nodes.Add(tn.Name, tn.Text, 2);
                    FileInfo f = new FileInfo(@s);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@s);
                    foreach (string s2 in str2)
                    {
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        ((TreeNode)treeView1.Nodes[n - 1]).Nodes.Add(s2, t, 0);
                    }
                }
                catch { }
                n++;
            }
            foreach (TreeNode tn in treeView1.Nodes)
            {
                for (int i = 65; i < 91; i++)
                {
                    char sym = Convert.ToChar(i);
                    if (tn.Name == sym + ":\\")
                        tn.SelectedImageIndex = 2;
                }
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0) return;
            //обработка двойного нажатия по папке или файла в ListView
            if (listView1.SelectedItems[0].Text.IndexOf('.') == -1)
            {
                //обработка нажатия на папку
                Addresses.Add(listView1.SelectedItems[0].Name);
                currIndex++;
                currListViewAddress = ((string)Addresses[currIndex]);
                if (currIndex + 1 == Addresses.Count) ;
                //  button2.Enabled = false;
                else
                //   button2.Enabled = true;;
                if (currIndex - 1 == -1) ;
                //    button1.Enabled = false;
                else
                    //   button1.Enabled = true;
                    currListViewAddress = listView1.SelectedItems[0].Name;
                textBox1.Text = currListViewAddress;
                FileInfo f = new FileInfo(@listView1.SelectedItems[0].Name);
                string t = "";
                string[] str2 = Directory.GetDirectories(@listView1.SelectedItems[0].Name);
                string[] str3 = Directory.GetFiles(@listView1.SelectedItems[0].Name);
                listView1.Items.Clear();
                ListViewItem lw = new ListViewItem();
                if (listView1.View == View.Details)
                {
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        string type = "Папка";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    foreach (string s2 in str3)
                    {
                        f = new FileInfo(@s2);
                        string type = "Файл";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                }
                else
                {
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    foreach (string s2 in str3)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                }
            }
            else
            {
                //обработка нажатия на файл(его запуска)
                System.Diagnostics.Process MyProc = new System.Diagnostics.Process();
                MyProc.StartInfo.FileName = @listView1.SelectedItems[0].Name;
                MyProc.Start();
            }
        }

        private void ClickOnColumn(object sender, ColumnClickEventArgs e)
        {
            //обработка нажатия на колонку имя(изменение порядка сортировки)
            if (e.Column == 0)
            {
                if (listView1.Sorting == SortOrder.Descending)
                    listView1.Sorting = SortOrder.Ascending;
                else
                    listView1.Sorting = SortOrder.Descending;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //проверка на то что был нажат enter, если был нажат entet и введённый адресс синтаксически верен, то будет произведён переход
            if (e.KeyValue == 13)
            {
                var saveAddr = textBox1.Text;
                try
                {

                    //   UpdateAddresses(listView1, textBox1);
                }
                catch
                {
                    textBox1.Text = saveAddr;
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            currListViewAddress = e.Node.Name;
            textBox1.Text = currListViewAddress;
            //заполнение ListView
            try
            {
                if (listView1.View != View.Tile)
                {
                    FileInfo f = new FileInfo(@e.Node.Name);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@e.Node.Name);
                    ListViewItem lw = new ListViewItem();
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        string type = "Папка";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    str2 = Directory.GetFiles(@e.Node.Name);
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        string type = "Файл";
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                }
                else
                {
                    FileInfo f = new FileInfo(@e.Node.Name);
                    string t = "";
                    string[] str2 = Directory.GetDirectories(@e.Node.Name);
                    ListViewItem lw = new ListViewItem();
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 0);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                    str2 = Directory.GetFiles(@e.Node.Name);
                    foreach (string s2 in str2)
                    {
                        f = new FileInfo(@s2);
                        t = s2.Substring(s2.LastIndexOf('\\') + 1);
                        lw = new ListViewItem(new string[] { t }, 1);
                        lw.Name = s2;
                        listView1.Items.Add(lw);
                    }
                }
            }
            catch { }
              mass();

        }

        public void mass()
        {
            listView1.View = View.Details;
            listView1.Items.Clear();
            FileInfo f = new FileInfo(currListViewAddress);
            string t = "";
            string[] str2 = Directory.GetDirectories(currListViewAddress);
            ListViewItem lw = new ListViewItem();
            foreach (string s2 in str2)
            {
                f = new FileInfo(@s2);
                string type = "Папка";
                t = s2.Substring(s2.LastIndexOf('\\') + 1);
                lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                lw.Name = s2;
                listView1.Items.Add(lw);
            }
            str2 = Directory.GetFiles(currListViewAddress);
            foreach (string s2 in str2)
            {
                f = new FileInfo(@s2);
                string type = "Файл";
                t = s2.Substring(s2.LastIndexOf('\\') + 1);
                lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                lw.Name = s2;
                listView1.Items.Add(lw);
            }

            //Дисковод
         //   DriveInfo[] drives = DriveInfo.GetDrives();
        //    foreach (DriveInfo drive in drives)
        //    {
          //      if (drive.IsReady)
            //    {
              //      MessageBox.Show($"Название: {drive.Name}" +
                //        $"Тип: {drive.DriveType}");
              //  }
           // }

        }

        private void button1_Click(object sender, EventArgs e)
        {
         //   mass();

        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            int i = 0;
            //заполнение дочерних узлов дочерними узлами развёртываемого узала
            try
            {
                foreach (TreeNode tn in e.Node.Nodes)
                {
                    string[] str2 = Directory.GetDirectories(@tn.Name);
                    foreach (string str in str2)
                    {
                        TreeNode temp = new TreeNode();
                        temp.Name = str;
                        temp.Text = str.Substring(str.LastIndexOf('\\') + 1);
                        e.Node.Nodes[i].Nodes.Add(temp);
                    }
                    i++;
                }
            }
            catch { }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            contextMenuStrip1.Show(PointToScreen(new Point(0, 39)));
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(PointToScreen(new Point(0, 39)));
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            contextMenuStrip1.Show(Cursor.Position);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string RootFolder = "ftp://127.0.0.1:21/";
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(RootFolder);
                request.Credentials = new NetworkCredential("admin", "admin");
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.Default);

                string dir_content = reader.ReadToEnd(); //список файлов и папок фтп сервера
                char[] separator = new char[] { '\n' };
                char[] parser_separator = new char[] { '|' };
                dir_content = dir_content.Replace(' ', '|');

                string[] dir_list = dir_content.Split(separator);

                List<string> names_list = new List<string>(); //список имен файлов и папок
                for (int i = 0; i < dir_list.Length; i++)
                {
                    //находим последний символ |
                    int index_oflastsep = -1;
                    for (int j = 0; j < dir_list[i].Length; j++)
                    {
                        if (dir_list[i][j] == '|')
                            index_oflastsep = j;
                    }
                    if (index_oflastsep > -1)
                    {
                        string FName = dir_list[i].Substring(index_oflastsep + 1).Trim();
                        if ((FName != "..") && (FName != "."))
                            names_list.Add(FName);
                    }
                }

                //выводим имена всех файлов и папок
                string AllNames = "";
                for (int i = 0; i < names_list.Count; i++)
                {
                    AllNames += names_list[i] + "\n";
                }
                MessageBox.Show(AllNames);
                listView2.Items.Add(AllNames);
                treeView2.Nodes.Add(AllNames);

                // string DirectoryListDetails = reader.ReadToEnd();
                // listView2.Items.Add(DirectoryListDetails);

            }
            catch { }

        }

        private void button4_Click(object sender, EventArgs e)
        {
           


        }

        private void button5_Click(object sender, EventArgs e)
        {
           


        }

        private void button6_Click(object sender, EventArgs e)
        {
            string path = @"C:\1\1.txt";

            System.IO.FileInfo f1;
            System.IO.FileInfo f2;
                // Проверить существует ли данная ссылка или нет?
                if (File.Exists(path))
                {
                    // Удалить файл
                 //   File.Delete(path);

                    // Проверить существует ли еще файл.
                 //   if (!File.Exists(path))
                  //  {
                    //   MessageBox.Show("File deleted...");
                   // }

                }
                else
                {
                    MessageBox.Show("File test.txt cei!");
                }
            if(Equals(path))
            {

            }





        }

        private void button7_Click(object sender, EventArgs e)
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                MessageBox.Show($"Название: {drive.Name}" + $"Тип: {drive.DriveType}" + $"Файловая система: {drive.DriveFormat}");
                if (drive.IsReady)
                {
                    MessageBox.Show($"Объем диска: {drive.TotalSize}" + $"Свободное пространство: {drive.TotalFreeSpace}" + $"Метка: {drive.VolumeLabel}");
                }
            }
        }

        

        private void button8_Click(object sender, EventArgs e)
        {
            string dirName = "C:\\Program Files";
            DirectoryInfo dirInfo = new DirectoryInfo(dirName);
            MessageBox.Show($"Название каталога: {dirInfo.Name}" 
                + $"Время создания каталога: {dirInfo.CreationTime}");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

            string RootFolder = "ftp://127.0.0.1:21/";
            string DirName = "My folder6";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(RootFolder,DirName));
            request.Credentials = new NetworkCredential("admin", "admin");            request.Method = WebRequestMethods.Ftp.MakeDirectory;            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            listView2.Items.Add(response.StatusDescription);

            try
            {
                using (request.GetResponse())
                {
                    MessageBox.Show("dsfdsf");
                }
            }
            catch (WebException)
            {
                //directoryExists = false;
            }

            //return directoryExists;
        }
    }
}
