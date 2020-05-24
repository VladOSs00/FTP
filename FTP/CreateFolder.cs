using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTP
{
    public partial class CreateFolder : Form
    {
        public string NameF { get; private set; }
        public CreateFolder()
        {
            InitializeComponent();
        }

        public void Set()
        {
            NameF = tbName.Text;
            Close();
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Set();
        }

        private void tbName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Set();
        }
    }
}
