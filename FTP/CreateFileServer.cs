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
    public partial class CreateFileServer : Form
    {
        public CreateFileServer()
        {
            InitializeComponent();
        }

        private void CreateFiles(object sender, EventArgs e)
        {
            string RootFolder = "ftp://127.0.0.1:21/";
            string DirName = textBox1.Text;
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(RootFolder, DirName));
            request.Credentials = new NetworkCredential("qwerty", "qwerty");
            request.Method = WebRequestMethods.Ftp.AppendFile;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            response.Close();
            textBox1.Text = response.StatusDescription;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {

            }
        }
    }
}
