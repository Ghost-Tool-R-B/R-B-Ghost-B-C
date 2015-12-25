using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using XDevkit;
using JRPC_Client;
using Ghost_Tool.Forms;
using System.Diagnostics;
using Client.ChatService;

namespace Ghost_Tool
{
    public partial class Main : DevExpress.XtraEditors.XtraForm
    {
        IXboxConsole Console;
        BO2 BO2 = new BO2();
        Client.Chat Chat = new Client.Chat();
        public Main()
        {
            InitializeComponent();
        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XtraMessageBox.Show("Welcome To Ghost Tool.\nMade By Ryan Borland\nYour current version is: v1.1");
            labelControl1.Text = "Welcome - " + Properties.Settings.Default.Username + " - To Ghost Tool.\nMade By Ryan Borland\nYour current version is: v1.1";
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            this.Hide();
            BO2.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
          XtraMessageBox.Show("Ghost Tool | Creators facebook now opening!");
          Process.Start("www.facebook.com/modz.ryan");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("Ghost Tool | Administrators facebook now opening!");
            Process.Start("www.facebook.com/Nate.UnboundSmexyLexy");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("skype:facebook:modz.ryan?chat");
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Process.Start("skype:live:dawsonfennema?chat");
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            //this.Hide();
            Chat.Show();
        }
    }
}
