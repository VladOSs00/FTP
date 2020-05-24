using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace FTP
{
    public partial class CreateFolderServer : Form
    {
        public CreateFolderServer()
        {
            InitializeComponent();
        }

        private void CreateFolder(object sender, EventArgs e)
        {
            CreateFolders();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CreateFolders();
            }
        }

        void CreateFolders()
        {
            try
            {
                string RootFolder = "ftp://127.0.0.1:21/";
                string DirName = textBox1.Text;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(RootFolder, DirName));
                request.Credentials = new NetworkCredential("admin", "admin");
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException)
            {
                MessageBox.Show("Папка "+ textBox1.Text + " уже существует!");
            }
        }
    }
}
