using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MySql.Data.MySqlClient;
using MySql.Data;
using JRPC_Client;
using XDevkit;
namespace Ghost_Tool.Forms
{
    public partial class Login : DevExpress.XtraEditors.XtraForm
    {
        private MySqlConnection connection;
        IXboxConsole XBOX;
        public Login()
        {
            string connectionString;
            connectionString = "Server=64.94.238.79; database=ghost; Uid=Ryan; Pwd=COREi7;";
            connection = new MySqlConnection(connectionString);
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void groupControl1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("Please Make Sure You Do The Following\n\nHave Your Console ON\nHave JRPC2 as a Plugin\nConsole Set As Default Console In NabourHood\n\nOr Login Will Fail");
            if (XBOX.Connect(out XBOX))
            {
                string XBOX_CPU = XBOX.GetCPUKey();
                string LoginQ = "Select * from ghost.clients WHERE Username=@par1 AND Password=@par2 AND CPUKEY=@par3;";
                MySqlCommand LoginCMD = new MySqlCommand(LoginQ, connection);
                LoginCMD.Parameters.AddWithValue("@par1", this.textEdit1.Text);
                LoginCMD.Parameters.AddWithValue("@par2", this.textEdit2.Text);
                LoginCMD.Parameters.AddWithValue("@par3", XBOX_CPU);
            try
                {
                    connection.Open();
                    LoginCMD.ExecuteNonQuery();
                    XtraMessageBox.Show("Welcome - " + textEdit1.Text + "\nWe Hope You Enjoy Using\n Ghost Tool");
                    Properties.Settings.Default.Username = textEdit1.Text;
                    Properties.Settings.Default.Save();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.Message);
                }
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {

        }
    }
}