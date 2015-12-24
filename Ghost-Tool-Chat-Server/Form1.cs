using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
namespace Ghost_Tool_Chat_Server
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        delegate void SetTextCallback(string text);
        TcpListener listener;
        TcpClient Client;
        NetworkStream ns;
        Thread t = null;
        public Form1()
        {
            InitializeComponent();
            listener = new TcpListener(4545);
            listener.Start();
            Client = listener.AcceptTcpClient();
            ns = Client.GetStream();
            t = new Thread(DoWork);
            t.Start();

        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            String s = textBox2.Text;
            byte[] byteTime = Encoding.ASCII.GetBytes(s);
            ns.Write(byteTime, 0, byteTime.Length);
        }

        public void DoWork()
        {
            byte[] bytes = new byte[1024];
            while(true)
            {
                int bytesRead = ns.Read(bytes, 0, bytes.Length);
                this.SetText(Encoding.ASCII.GetString(bytes, 0, bytesRead));
            }
        }
        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text = this.textBox1.Text + text;
            }
        }
    }
}