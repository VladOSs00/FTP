using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace FTP
{
    public partial class Main : Form
    {
        //коллекция посещённых адрессов
        ArrayList Addresses = new ArrayList();
        //индекс текущего адресса из коллекции Adresses
        int currIndex = -1;
        //текущий адресс
        string currListViewAddress = "";
        private string Delete;

        public Main()
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
                //  MessageBox.Show(AllNames);
                listView2.Items.Add(AllNames);
                // treeView2.Nodes.Add(AllNames);

                string DirectoryListDetails = reader.ReadToEnd();
                listView2.Items.Add(DirectoryListDetails);

            }
            catch { }
            //  mass();

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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
           // button4.Enabled = true;
            /////////
            string strtmp = "";
            if (Addresses.Count != 0)
            {
                strtmp = ((string)Addresses[Addresses.Count - 1]);
                Addresses.Clear();
                Addresses.Add(strtmp);
                currIndex = 0;
            }
            Addresses.Add(e.Node.Name);
            currIndex++;
            //проверка возможности перехода назад/вперёд
            if (currIndex + 1 == Addresses.Count)
                button2.Enabled = false;
            else
                button2.Enabled = true;
            if (currIndex - 1 == -1)
                button1.Enabled = false;
            else
                button1.Enabled = true;
            listView1.Items.Clear();
            currListViewAddress = e.Node.Name;
            textBox1.Text = currListViewAddress;
            /*
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
            */

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
        }

        public void mass()
        {
            listView1.View = View.Details;
            listView1.Items.Clear();
            FileInfo ff = new FileInfo(currListViewAddress);
            string tt = "";
            string[] str2 = Directory.GetDirectories(currListViewAddress);
            ListViewItem lww = new ListViewItem();
            foreach (string s2 in str2)
            {
                ff = new FileInfo(@s2);
                string type = "Папка";
                tt = s2.Substring(s2.LastIndexOf('\\') + 1);
                lww = new ListViewItem(new string[] { tt, "", type, ff.LastWriteTime.ToString() }, 0);
                lww.Name = s2;
                listView1.Items.Add(lww);
            }
            str2 = Directory.GetFiles(currListViewAddress);
            foreach (string s2 in str2)
            {
                ff = new FileInfo(@s2);
                string type = "Файл";
                tt = s2.Substring(s2.LastIndexOf('\\') + 1);
                lww = new ListViewItem(new string[] { tt, ff.Length.ToString() + " байт", type, ff.LastWriteTime.ToString() }, 1);
                lww.Name = s2;
                listView1.Items.Add(lww);
            }

            //Дисковод
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    MessageBox.Show($"Название: {drive.Name}" +
                         $"Тип: {drive.DriveType}");
                }
            }

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

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //проверка на то что был нажат enter, если был нажат entet и введённый адресс синтаксически верен, то будет произведён переход
            if (e.KeyValue == 13)
            {
                var saveAddr = textBox1.Text;
                try
                {

                    UpdateAddresses(listView1, textBox1);
                }
                catch
                {
                    textBox1.Text = saveAddr;
                }
            }
        }

   

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
         
        }

        private void UpdateAddresses(ListView lb, TextBox tb)
        {
            string[] str2 = Directory.GetDirectories(@tb.Text);
            string[] str3 = Directory.GetFiles(@tb.Text);
            currIndex++;
            Addresses.Add(tb.Text);
            if (currIndex + 1 == Addresses.Count)
                button2.Enabled = false;
            else
                button2.Enabled = true;
            if (currIndex - 1 == -1)
                button1.Enabled = false;
            else
                button1.Enabled = true;
                
            FileInfo f = new FileInfo(@tb.Text);
            string t = "";
            var selected = new List<string>();
            foreach (ListViewItem item in lb.SelectedItems)
            {
                selected.Add(item.Text);
            }
            lb.Items.Clear();

            ListViewItem lw = new ListViewItem();
            if (lb.View == View.Details)
            {
                foreach (string s2 in str2)
                {
                    f = new FileInfo(@s2);
                    string type = "Папка";
                    t = s2.Substring(s2.LastIndexOf('\\') + 1);
                    lw = new ListViewItem(new string[] { t, "", type, f.LastWriteTime.ToString() }, 0);
                    lw.Name = s2;
                    lb.Items.Add(lw);
                }
                foreach (string s2 in str3)
                {
                    f = new FileInfo(@s2);
                    string type = "Файл";
                    t = s2.Substring(s2.LastIndexOf('\\') + 1);
                    lw = new ListViewItem(new string[] { t, f.Length.ToString() + " байт", type, f.LastWriteTime.ToString() }, 1);
                    lw.Name = s2;
                    lb.Items.Add(lw);
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
                    lb.Items.Add(lw);
                }
                foreach (string s2 in str3)
                {
                    f = new FileInfo(@s2);
                    t = s2.Substring(s2.LastIndexOf('\\') + 1);
                    lw = new ListViewItem(new string[] { t }, 1);
                    lw.Name = s2;
                    lb.Items.Add(lw);
                }
            }

            foreach (string item in selected)
            {
                foreach (ListViewItem newItem in lb.Items)
                {
                    if (item == newItem.Text)
                    {
                        newItem.Selected = true;
                        break;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
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
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.View = View.Tile;
            listView1.Items.Clear();
            FileInfo f = new FileInfo(currListViewAddress);
            string t = "";
            string[] str2 = Directory.GetDirectories(currListViewAddress);
            ListViewItem lw = new ListViewItem();
            foreach (string s2 in str2)
            {
                f = new FileInfo(@s2);
                t = s2.Substring(s2.LastIndexOf('\\') + 1);
                lw = new ListViewItem(new string[] { t }, 0);
                lw.Name = s2;
                listView1.Items.Add(lw);
            }
            str2 = Directory.GetFiles(currListViewAddress);
            foreach (string s2 in str2)
            {
                f = new FileInfo(@s2);
                t = s2.Substring(s2.LastIndexOf('\\') + 1);
                lw = new ListViewItem(new string[] { t }, 1);
                lw.Name = s2;
                listView1.Items.Add(lw);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            //проверка на то что был нажат enter, если был нажат entet и введённый адресс синтаксически верен, то будет произведён переход
            if (e.KeyValue == 13)
            {
               
            }
        }

       
        private void Back_Click(object sender, EventArgs e)
        {
            //обработка "Назад"
            if (currIndex - 1 != -1)
            {
                currIndex--;
                currListViewAddress = ((string)Addresses[currIndex]);
                if (currIndex + 1 == Addresses.Count)
                    button2.Enabled = false;
                else
                    button2.Enabled = true;
                if (currIndex - 1 == -1)
                    button1.Enabled = false;
                else
                    button1.Enabled = true;
                textBox1.Text = currListViewAddress;
                FileInfo f = new FileInfo(currListViewAddress);
                string t = "";
                string[] str2 = Directory.GetDirectories(currListViewAddress);
                string[] str3 = Directory.GetFiles(currListViewAddress);
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
        }

        private void Forward_Click(object sender, EventArgs e)
        {
            //обработка "Вперёд"
            if (currIndex + 1 != Addresses.Count)
            {
                currIndex++;
                currListViewAddress = ((string)Addresses[currIndex]);
                if (currIndex + 1 == Addresses.Count)
                    button2.Enabled = false;
                else
                    button2.Enabled = true;
                if (currIndex - 1 == -1)
                    button1.Enabled = false;
                else
                    button1.Enabled = true;
                textBox1.Text = currListViewAddress;
                FileInfo f = new FileInfo(currListViewAddress);
                string t = "";
                string[] str2 = Directory.GetDirectories(currListViewAddress);
                string[] str3 = Directory.GetFiles(currListViewAddress);
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
        }

        private void Up_Click(object sender, EventArgs e)
        {
            //обработка "Вверх"
            var index = textBox1.Text.IndexOf(@":\");
            if (index > -1 && textBox1.Text.Length - 1 > index + 2)
            {
                var diskPath = textBox1.Text.Remove(index + 2);
                var saveAddr = textBox1.Text;
                try
                {
                    textBox1.Text = diskPath;
                    UpdateAddresses(listView1, textBox1);
                }
                catch
                {
                    textBox1.Text = saveAddr;
                }
            }
        }


        private void Copy_Click(object sender, EventArgs e) => CopySelected();


        // определяет выбранный файл и копирует в другой listview
        private void CopySelected()
        {
            if (listView1.SelectedItems.Count > 0) listView1.Focus();
            

            ListView source;
            ListView destination;
            TextBox destinationPath;
            string sourcePath;

            if (listView1.SelectedItems.Count > 0)
            {
                source = listView1;
                destination = listView1;
                sourcePath = textBox1.Text;
                destinationPath = textBox2;
            }
            else
            {
                source = listView1;
                destination = listView1;
                sourcePath = textBox2.Text;
                destinationPath = textBox1;
            }

            try
            {
                foreach (ListViewItem item in source.SelectedItems)
                {
                    string s = sourcePath + @"\" + item.Text;
                    string dest = destinationPath.Text;
                    if (Utils.IsDirectory(s)) dest = dest + @"\" + item.Text;
                    Utils.Copy(s, dest);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Delete_Click(object sender, EventArgs e) => DeleteSelected();

        //  Удаляем выбранные файлы и папки
        private void DeleteSelected()
        {

            {    //  чтобы в listView1 и listView2 не снимался фокус после нажатия клавиши
                if (listView1.SelectedItems.Count > 0) listView1.Focus();
         

                ListView source;
                TextBox sourcePath;

                if (listView1.SelectedItems.Count > 0)
                {
                    source = listView1;
                    sourcePath = textBox1;
                }
                else
                {
                    source = listView1;
                    sourcePath = textBox2;
                }

                try
                {
                    foreach (ListViewItem item in source.SelectedItems)
                    {
                        string target = sourcePath.Text + @"\" + item.Text;
                        if (Utils.IsDirectory(target)) Utils.DeleteDirectory(target);
                        else File.Delete(target);

                        if (!string.IsNullOrWhiteSpace(textBox1.Text))
                        {
                            try
                            {  // отображает в listview1 файлы и папки пути указанного в textbox1
                                UpdateAddresses(listView1, textBox1);
                            }
                            catch
                            {

                            }
                        }
                        /*
                        //обработка "Вверх"
                        var index = textBox1.Text.IndexOf(@":\");
                        if (index > -1 && textBox1.Text.Length - 1 > index + 2)
                        {
                            var diskPath = textBox1.Text.Remove(index + 2);
                            var saveAddr = textBox1.Text;
                            try
                            {
                                textBox1.Text = diskPath;
                                UpdateAddresses(listView1, textBox1);
                            }
                            catch
                            {
                                textBox1.Text = saveAddr;
                            }
                        }
                        */
                    }
                   

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
                if (currIndex + 1 == Addresses.Count)
                    button2.Enabled = false;
                else
                    button2.Enabled = true;
                if (currIndex - 1 == -1)
                    button1.Enabled = false;
                else
                    button1.Enabled = true;
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

        private void Refresh_Click(object sender, EventArgs e)
        {
          //  listView1.Refresh();
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                try
                {  // отображает в listview1 файлы и папки пути указанного в textbox1
                    UpdateAddresses(listView1, textBox1);
                }
                catch
                {

                }
            }
        }

        private void CreateFolder_Click(object sender, EventArgs e)
        {
            CreateFolder newForm = new CreateFolder();
            newForm.Show();

            try
            {
                // если не строка null или не нажат пробел
                if (!string.IsNullOrWhiteSpace(newForm.NameF))
                {           // обьявляем путь 
                    var path = textBox1.Text + @"\" + newForm.NameF;

                    if (!Directory.Exists(path))
                        { 
                         // если папка не существует тогда папку создаем
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        // если папка существует выкидывает ошибку
                        else throw new Exception("Такая папка уже существует.");
                    }
                   
                   
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateFolsedServer_Click(object sender, EventArgs e)
        {
            CreateFolderServer newForm = new CreateFolderServer();
            newForm.Show();
        }

        private void BackServer_Click(object sender, EventArgs e)
        {

        }

        private void DeleteServer_Click(object sender, EventArgs e)
        {
            /*
            string RootFolder = "ftp://127.0.0.1:21/";
            string FileName = @"1/12.txt";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(RootFolder, FileName));
            request.Credentials = new NetworkCredential("admin", "admin");
            request.Method = WebRequestMethods.Ftp.DeleteFile;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            string status = response.StatusDescription;
            response.Close();
            */

            string RootFolder = "ftp://127.0.0.1:21/";
            string FileName = @"daf1";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(RootFolder, FileName));
            request.Credentials = new NetworkCredential("admin", "admin");
            request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            string status = response.StatusDescription;
            response.Close();

        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
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
                //  MessageBox.Show(AllNames);
                listView2.Items.Add(AllNames);
                // treeView2.Nodes.Add(AllNames);

                string DirectoryListDetails = reader.ReadToEnd();
                listView2.Items.Add(DirectoryListDetails);

            }
            catch { }
        }

        private void InformationServer_Click(object sender, EventArgs e)
        {
            string RootFolder = "ftp://127.0.0.1:21/";
            string FileName = "AzureDevOpsData";
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(RootFolder, FileName));
            request.Credentials = new NetworkCredential("admin", "admin");
            request.Method = WebRequestMethods.Ftp.GetFileSize;
            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
         //   request.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
            
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            long size = response.ContentLength;
            MessageBox.Show($"Размер {FileName} - {size} байт"
                           + $"Дата и время модификации файла  {FileName} - {response.LastModified}  ");


          //  string status = response.StatusDescription;
            response.Close();
        }

        private void Update_Click(object sender, EventArgs e)
        {

        }
    }
}
