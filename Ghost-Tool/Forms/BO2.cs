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
using XDevkit;
using JRPC_Client;
using System.Text.RegularExpressions;
using System.Threading;
using XDevkitPlusPlus;
using Ghost_Tool;
using System.IO;

namespace Ghost_Tool.Forms
{
    public partial class BO2 : DevExpress.XtraEditors.XtraForm
    {
        IXboxConsole Console;
        private byte[] Buffer;
        private uint clientCommand = 0x824015E0;
        public uint BO2SV = 0x8242FB70;
        private uint giveWeap = 0x823acb78;
        private uint weaponId = 0x822535d8;
        private uint initializeAmmo = 0x823120e0;
        private uint num = 0;
        byte[] NULL = { 0x60, 0x00, 0x00, 0x00 },
          Orange = { 0x38, 0xC0, 0x00, 0x0A },
          Green = { 0x38, 0xC0, 0x00, 0x16 },
          OrangeOFF = { 0x7F, 0xC6, 0xF3, 0x78 },
          BO2CHAMSON = { 0x38, 0xc0, 0xff, 0xff },
          BO2CHAMSOFF = { 0x7f, 0xa6, 0xeb, 0x12 },
          UAVON = { 0x3B, 0xA0, 0x00, 0x01 },
          ForceHostOff = { 0x89, 0x6B, 0x00, 0x0C };
        public BO2()
        {
            InitializeComponent();
        }

        public uint PlayerState(int Client)
        {
            return (uint)(0x83551A10 + (Client * 0x57F8));
        }

        private void Refreshplayers()
        {
            GamertagListBox.Items.Clear();
            for (int clientIndex = 0; clientIndex < 18; clientIndex++)
            {
                string Counter = clientIndex.ToString();
                byte[] gts = Console.GetMemory(PlayerState(clientIndex) + 0x5534, 0x19);
                string GetGT = System.Text.ASCIIEncoding.UTF8.GetString(gts);
                GamertagListBox.Items.Add(string.Format("{0} {1}", Counter, GetGT));
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Refreshplayers();
            timer1.Start();
        }

        private void BO2_Load(object sender, EventArgs e)
        { 

        }

        private void ConsoleStatus()
        {
            if (Console.Connect(out Console))
            {
                labelControl2.Text = "Connected!";
            }
            else
            {
                labelControl2.Text = "NOT Connected!";
            }
        }

        private byte[] WideChar(string text)
        {
            byte[] buffer = new byte[(text.Length * 2) + 2];
            int index = 1;
            buffer[0] = 0;
            foreach (char ch in text)
            {
                buffer[index] = Convert.ToByte(ch);
                index += 2;
            }
            return buffer;
        }

        private void XShowMessageBoxUI(int UserIndex, string Title, string Text, int NumButtons, string[] Buttons, int FocusButton = 0, int Flags = 3)
        {
            uint num = JRPC.ResolveFunction(this.Console, "xam.xex", 0x2ca);
            uint num2 = 0x81b01480;
            byte[] buffer = new byte[6];
            byte[] buffer2 = new byte[0x1c];
            byte[] buffer3 = new byte[0];
            byte[] buffer4 = new byte[0];
            byte[] buffer5 = new byte[0];
            uint num3 = 0;
            uint num4 = 0;
            uint num5 = 0;
            uint num6 = 0;
            JRPC.SetMemory(this.Console, num2, buffer);
            JRPC.SetMemory(this.Console, num2 + 6, buffer2);
            uint num7 = 0x22;
            byte[] buffer6 = this.WideChar(Title);
            byte[] buffer7 = this.WideChar(Text);
            if (NumButtons >= 1)
            {
                buffer3 = this.WideChar(Buttons[0]);
            }
            if (NumButtons >= 2)
            {
                buffer4 = this.WideChar(Buttons[1]);
            }
            if (NumButtons == 3)
            {
                buffer5 = this.WideChar(Buttons[2]);
            }
            JRPC.SetMemory(this.Console, num2 + num7, buffer6);
            uint num8 = num2 + num7;
            num7 += (uint)buffer6.Length;
            JRPC.SetMemory(this.Console, num2 + num7, buffer7);
            uint num9 = num2 + num7;
            num7 += (uint)buffer7.Length;
            if (NumButtons >= 1)
            {
                JRPC.SetMemory(this.Console, num2 + num7, buffer3);
                num3 = num2 + num7;
                num7 += (uint)buffer3.Length;
            }
            if (NumButtons >= 2)
            {
                JRPC.SetMemory(this.Console, num2 + num7, buffer4);
                num4 = num2 + num7;
                num7 += (uint)buffer4.Length;
            }
            if (NumButtons == 3)
            {
                JRPC.SetMemory(this.Console, num2 + num7, buffer5);
                num5 = num2 + num7;
                num7 += (uint)buffer5.Length;
            }
            if (NumButtons >= 1)
            {
                JRPC.WriteInt32(this.Console, num2 + num7, (int)num3);
                num6 = num2 + num7;
                num7 += 4;
            }
            if (NumButtons >= 2)
            {
                JRPC.WriteInt32(this.Console, num2 + num7, (int)num4);
                num7 += 4;
            }
            if (NumButtons == 3)
            {
                JRPC.WriteInt32(this.Console, num2 + num7, (int)num5);
                num7 += 4;
            }
            object[] objArray1 = new object[] { UserIndex, num8, num9, NumButtons, num6, FocusButton, Flags, num2, num2 + 0x1c };
            JRPC.Call<uint>(this.Console, num, objArray1);
            byte[] buffer8 = new byte[num7];
            JRPC.SetMemory(this.Console, num2, buffer8);
        }

        private void SendMessageBoxUI()
        {
            string[] buttons = new string[] { this.textEdit2.Text };
            this.XShowMessageBoxUI(0, this.textEdit1.Text, this.richTextBox1.Text, buttons.Length, buttons, 0, 3);
        }

        private void ActivatePublicCheater()
        {
            Console.WriteUInt32(0x826BB6C4, 0x60000000);//crosshairs
            Console.WriteUInt32(0x82000DCC, 0x39600000);//crosshairs2
            Console.WriteUInt32(0x826BB6C8, 0x39600000);//crosshair3
            Console.WriteByte(0x821f5b7f, 0x01); //Redbox
            Console.WriteByte(0x821b8fd3, 0x01); //Blackbird
            Console.SetMemory(0x821F608C, NULL); //Blackbird
            Console.SetMemory(0x82259BC8, NULL); //NoRecoil
            Console.SetMemory(0x821fc04c, BO2CHAMSON); //Charms
            Console.SetMemory(0x82255E1C, new byte[] { 0x2b, 11, 00, 01 }); //Lazer
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (Console.Connect(out Console))
            {
                SendMessageBoxUI();
                ActivatePublicCheater();
                ConsoleStatus();
            }
            else
            {
                
            }
        }

        private void simpleButton3_Click(object sender, EventArgs e)
        {
            ConsoleStatus();
        }

        private void groupControl3_Paint(object sender, PaintEventArgs e)
        {

        }

        bool DiscoLaser = false;
        Thread DiscoLaserThread;

        public void DoDiscoChams()
        {
            if (DiscoLaser == false)
            {
                DiscoLaserThread = new Thread(() => _DiscoChams()) { IsBackground = true };
                DiscoLaserThread.Start();
            }
        }


        public void _DiscoChams()
        {
            DiscoLaser = true;
            string[] Colors = { "^1", "^2", "^3", "^4", "^5", "^6" };
            while (DiscoLaser)
            {
                Thread.Sleep(250);
                Console.SetMemory(0xC035261D, reverseBytes(Gamertag.Text.ToString()));
                Console.SetMemory(0xC035261C, getBytes(Encoding.ASCII.GetBytes(Colors[0] + Gamertag.Text.ToString() + "\0")));
                Thread.Sleep(250);
                Console.SetMemory(0xC035261D, reverseBytes(Gamertag.Text.ToString()));
                Console.SetMemory(0xC035261C, getBytes(Encoding.ASCII.GetBytes(Colors[1] + Gamertag.Text.ToString() + "\0")));
                Thread.Sleep(250);
                Console.SetMemory(0xC035261D, reverseBytes(Gamertag.Text.ToString()));
                Console.SetMemory(0xC035261C, getBytes(Encoding.ASCII.GetBytes(Colors[2] + Gamertag.Text.ToString() + "\0")));
                Thread.Sleep(250);
                Console.SetMemory(0xC035261D, reverseBytes(Gamertag.Text.ToString()));
                Console.SetMemory(0xC035261C, getBytes(Encoding.ASCII.GetBytes(Colors[3] + Gamertag.Text.ToString() + "\0")));
                Thread.Sleep(250);
                Console.SetMemory(0xC035261D, reverseBytes(Gamertag.Text.ToString()));
                Console.SetMemory(0xC035261C, getBytes(Encoding.ASCII.GetBytes(Colors[4] + Gamertag.Text.ToString() + "\0")));
            }
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            DoDiscoChams();
        }

        public string ByteToString(byte[] input)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            return encoding.GetString(input);
        }

        public static byte[] reverseBytes(string gamertag)
        {
            byte[] numArray = new byte[gamertag.Length * 2 + 2];
            numArray[0] = (byte)0;
            uint num1 = 0x01;
            for (int index = 0; index < gamertag.Length; ++index)
            {
                char ch = gamertag[index];
                numArray[(int)(uint)(UIntPtr)num1] = (byte)ch;
                uint num2 = num1 + 0x01;
                numArray[(int)(uint)(UIntPtr)num2] = (byte)0;
                num1 = num2 + 0x01;
            }
            numArray[(int)(uint)(UIntPtr)num1] = (byte)0;
            return numArray;
        }

        private Byte[] getBytes(Byte[] test)
        {
            byte[] data = new byte[test.Length * 2];
            int i = 0;
            foreach (byte t in test)
            {
                data[i] = 0x0;
                data[(i + 1)] = t;
                i += 2;
            }
            return data;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        private void DefaultColorGT(int client)
        {
            uint num;
            byte[] data = new byte[20];
            Console.DebugTarget.GetMemory(PlayerState(client) + 0x5534, 20, data, out num);
            Console.DebugTarget.InvalidateMemoryCache(true, PlayerState(client) + 0x5534, 20);
            string str = RemoveSpecialCharacters(ByteToString(data));
            if (str == "")
            {
                str = "";
            }
            string str2 = Gamertag.Text;
            byte[] bytes = Encoding.UTF8.GetBytes(str2);
            Console.SetMemory(PlayerState(client) + 0x5534, new byte[20]);
            Console.SetMemory(PlayerState(client) + 0x5534, bytes);
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            if (PreIngame.Text == "In-Game")
            {
                DefaultColorGT(client);
                XtraMessageBox.Show("Gamertag Changed to: " + Gamertag.Text);
                
            }
            else if (PreIngame.Text == "Pre-Game")
            {
                Console.SetMemory(0xC035261D, reverseBytes(Gamertag.Text.ToString()));
                Console.SetMemory(0xC035261C, getBytes(Encoding.ASCII.GetBytes(Gamertag.Text.ToString() + "\0")));
                MessageBox.Show("Gamertag Changed to: " + Gamertag.Text);
            }
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            DiscoLaser = false;
        }

        private void simpleButton7_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            uint num;
            uint num2;
            uint num3;
            byte[] data = new byte[] { 0xff };
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x42e, 1, data, out num);
            byte[] buffer2 = new byte[] { 0xff };
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x432, 1, buffer2, out num2);
            byte[] buffer3 = new byte[] { 0xff };
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x437, 1, buffer3, out num3);
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7Score Streaks: ^2Given!\"" });
        }

        private void simpleButton8_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            float X = Console.ReadFloat(PlayerState(client) + 0x28);
            float Y = Console.ReadFloat(PlayerState(client) + 0x2C);
            float Z = Console.ReadFloat(PlayerState(client) + 0x30);
            Xtext.Text = X.ToString();
            Ytext.Text = Y.ToString();
            Ztext.Text = Z.ToString();
        }

        private void groupControl6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void simpleButton9_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            Console.WriteFloat(PlayerState(client) + 0x28, Convert.ToSingle(Xtext.Text));
            Console.WriteFloat(PlayerState(client) + 0x2C, Convert.ToSingle(Ytext.Text));
            Console.WriteFloat(PlayerState(client) + 0x30, Convert.ToSingle(Ztext.Text));
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7Teleported To: ^2location!\"" });
        }

        private void simpleButton11_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            Console.CallVoid(0x8242FB70, -1, 0, "< \"^7Max Ammo ^2Off\"");
            uint num;
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x43A, 1, new byte[] { 0x00 }, out num);
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x43E, 1, new byte[] { 0x00 }, out num);
        }

        private void simpleButton10_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            Console.CallVoid(0x8242FB70, -1, 0, "< \"^7Max Ammo ^2On\"");
            uint num;
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x43A, 1, new byte[] { 0xFF }, out num);
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x43E, 1, new byte[] { 0xFF }, out num);
        }

        private void simpleButton13_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            uint IsEmp = 0;
            uint num;
            IsEmp = Console.ReadUInt32(PlayerState(client) + 0x1B);
            if (IsEmp == 1962934272) { Console.DebugTarget.SetMemory(PlayerState(client) + 0x1B, 1, new byte[] { 0x48 }, out num); }
            else { Console.DebugTarget.SetMemory(PlayerState(client) + 0x1B, 1, new byte[] { 0x00 }, out num); }
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7GodMode ^1Taken!\"" });
        }

        private void simpleButton12_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            uint IsEmp = 0;
            uint IsGravity = 0;
            uint num;
            IsGravity = Console.ReadUInt32(PlayerState(client) + 0x1B);
            if (IsEmp == 620756992) { Console.DebugTarget.SetMemory(PlayerState(client) + 0x1B, 1, new byte[] { 0x78 }, out num); }
            else { Console.DebugTarget.SetMemory(PlayerState(client) + 0x1B, 1, new byte[] { 0x85 }, out num); }
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7GodMode ^2Given!\"" });
        }

        private void simpleButton14_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            uint num;
            Console.DebugTarget.SetMemory(PlayerState(client) + 0xFF, 1, new byte[1] { 0xFF }, out num);
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7Invisibility ^2Given!\"" });
        }

        private void simpleButton15_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            uint num;
            byte[] buffer = new byte[1];
            Console.DebugTarget.SetMemory(PlayerState(client) + 0xFF, 1, new byte[1] { 0x00 }, out num);
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7Invisibility ^1Taken!\"" });
        }

        public void SMO(uint Address, uint BytesToWrite, byte[] D)
        {
            uint Index;
            Console.DebugTarget.SetMemory(Address, BytesToWrite, D, out Index);
        }

        private void simpleButton16_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            int num = 0;
            SMO(PlayerState(client) + 0x54ea, 1, new byte[1]);
            SMO(PlayerState(client) + 0x5413, 1, new byte[] { 1 });
            Thread.Sleep(num += 500);
            if (num == 0x7d0)
            {
                SMO(PlayerState(client) + 0x5413, 1, new byte[] { 2 });
                Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7No clip ^2Given!\"" });
             return;
            }
        }

        private void simpleButton17_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            SMO(PlayerState(client) + 0x54ea, 1, new byte[1]);
            SMO(PlayerState(client) + 0x5413, 1, new byte[1]);
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7No clip ^1Taken!\"" });
        }

        private void simpleButton18_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            uint num;
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x100, 1, new byte[] { 0x10 }, out num);
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7Red Boxes ^2Given!\"" });
        }

        private void simpleButton19_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            byte[] buffer = new byte[1];
            Console.DebugTarget.SetMemory(PlayerState(client) + 0x100, 1, buffer, out num);
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7Red Boxes ^1Taken!\"" });
        }

        private void simpleButton20_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            Console.SetMemory(PlayerState(client) + 0xA3B, new byte[] { 0xFF });
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7More Damge ^2Given!\"" });
        }

        private void simpleButton21_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            Console.SetMemory(PlayerState(client) + 0xA3B, new byte[] { 0xFF });
            Console.CallVoid(0x8242FB70, new object[] { (client), 0, "< \"^7More Damge ^1Taken!\"" });
        }

        public uint getWeapId(string weap)
        {
            return Console.Call<uint>(this.weaponId, new object[] { weap });
        }

        public void initAmmo(uint clientIndex, uint weaponIndex, uint ammo = 0x3e7)
        {
            int client = (int)Client.Value;
            Console.CallVoid(this.initializeAmmo, new object[] { (client), weaponIndex, 1, ammo });
        }


        public void giveWeapon(uint clientIndex, string weapon, int dualBool, int camo = 0)
        {
            int client = (int)Client.Value;
            uint weaponIndex = this.getWeapId(weapon);
            Console.CallVoid(this.giveWeap, new object[] { (client), weaponIndex, (uint)dualBool, (uint)camo });
            this.initAmmo((uint)clientIndex, weaponIndex, 0x3e7);
            Console.CallVoid(0x8242FB70, new object[] { (client), "h " + weaponIndex.ToString() });
        }

        private void simpleButton22_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            giveWeapon((PlayerState(client)), this.comboBoxEdit1.SelectedItem.ToString(), 0, 0);
        }

        public void _changeCamo(string camoName, uint client1)
        {
            int client = (int)Client.Value;
            uint num;
            if (camoName == "DEVGRU")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 1 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 1 }, out num);
            }
            if (camoName == "A-TACS AU")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 2 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 2 }, out num);
            }
            if (camoName == "ERDL")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 3 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 3 }, out num);
            }
            if (camoName == "Siberia")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 4 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 4 }, out num);
            }
            if (camoName == "Choco")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 5 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 5 }, out num);
            }
            if (camoName == "Blue Tiger")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 6 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 6 }, out num);
            }
            if (camoName == "Bloodshot")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 7 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 7 }, out num);
            }
            if (camoName == "Ghostex: Delta 6")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 8 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 8 }, out num);
            }
            if (camoName == "Kryptek: Typhon")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 9 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 9 }, out num);
            }
            if (camoName == "Carbon Fiber")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 10 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 10 }, out num);
            }
            if (camoName == "Cherry Blossom")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 11 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 11 }, out num);
            }
            if (camoName == "Art of War")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 12 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client)+ 0x2DB, 1, new byte[] { 12 }, out num);
            }
            if (camoName == "Ronin")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 13 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 13 }, out num);
            }
            if (camoName == "Skulls")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 14 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 14 }, out num);
            }
            if (camoName == "Gold")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 15 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 15 }, out num);
            }
            if (camoName == "Diamond Camouflage")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x10 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x10 }, out num);
            }
            if (camoName == "CE Digital")
            {
               Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x12 }, out num);
               Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x12 }, out num);
            }
            if (camoName == "Elite Member")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x11 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x11 }, out num);
            }
            if (camoName == "Jungle Warfare")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x13 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x13 }, out num);
            }
            if (camoName == "Benjamins")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x15 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x15 }, out num);
            }
            if (camoName == "Dia de Muertos")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x16 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x16 }, out num);
            }
            if (camoName == "Graffiti")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x17 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x17 }, out num);
            }
            if (camoName == "Kawaii")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x18 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x18 }, out num);
            }
            if (camoName == "Party Rock")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x19 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x19 }, out num);
            }
            if (camoName == "Zombies")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x1a }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x1a }, out num);
            }
            if (camoName == "Viper")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x1b }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x1b }, out num);
            }
            if (camoName == "Bacon")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x1c }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x1c }, out num);
            }
            if (camoName == "Random Color")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 20 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 20 }, out num);
            }
            if (camoName == "Ghost")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x1d }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x1d }, out num);
            }
            if (camoName == "Paladin")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 30 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 30 }, out num);
            }
            if (camoName == "Comics")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x21 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x21 }, out num);
            }
            if (camoName == "Dragon")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x20 }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x20 }, out num);
            }
            if (camoName == "Cyborg")
            {
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2BF, 1, new byte[] { 0x1f }, out num);
                Console.DebugTarget.SetMemory(PlayerState(client) + 0x2DB, 1, new byte[] { 0x1f }, out num);
            }
        }


        private void simpleButton23_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            string sringggu = comboBoxEdit2.Text;
            _changeCamo(sringggu, (PlayerState(client)));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Refreshplayers();
        }

        private void simpleButton24_Click(object sender, EventArgs e)
        {
            Console.CallVoid(0x8242FB70, -1, 0, "< \"^7Super Jump ^2On\"");
            Console.DebugTarget.WriteFloat(0x82085654, (float)Convert.ToInt32(trackBarControl2.Value));
            Console.DebugTarget.WriteFloat(0x82001650, (float)Convert.ToInt32(trackBarControl2.Value));
        }

        private void groupControl5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void simpleButton25_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            Console.CallVoid(0x8242FB70, new object[] { client, 0, "< \"" + textEdit3.Text + " \"" });
            Console.CallVoid(0x8242FB70, new object[] { client, 0, "; \"" + textEdit3.Text + " \"" });
        }

        private void simpleButton26_Click(object sender, EventArgs e)
        {
            int client = (int)Client.Value;
            Console.CallVoid(0x8242FB70, client, 0, "^" + textEdit4.Text);
        }

        private void BO2_FormClosing(object sender, FormClosingEventArgs e)
        {
 
        }

        private void simpleButton27_Click(object sender, EventArgs e)
        {
            
        }

        private void BO2CMDStats(string Command)
        {
            Console.CallVoid(0x824015e0, new object[] { 0, Command });
        }

        private enum BO2Stats : uint
        {
            Cbuf_addText = 0x82110108,
            StatEntry = 0x840C0500, //the actual offset: 0x843489C8
            deaths = 0x84348AD2, //93U
            hits = 0x84348A45,
            gamesplayed = 0x83583941,
            head_shots = 2207964081U, //need bytes
            kills = 0x84348D00,
            killstreak = 0x84348FE8,
            losses = 0x84348D72,
            score = 0x843491E0,
            time_played = 0x8434929A,
            total_shots = 2207964166U, //need bytes
            winLossratio = 0x839ADC0A, //need bytes
            wins = 0x843492E2,
            winstreak = 0x835839A3, //guessed
            prestige = 0x843491A4,
            XP = 0x843491BC,//0x843491A6 for fixed XP
            misses = 0x843492CA,
            PTokens = 0x8435292E,//9F66
        }

        private void WriteStat(BO2.BO2Stats STAT, int VALUE)
        {
            Buffer = BitConverter.GetBytes(VALUE);
            Console.SetMemory((uint)STAT, Buffer);
        }

        private void simpleButton28_Click(object sender, EventArgs e)
        {
            WriteStat(BO2.BO2Stats.prestige, (int)spinEdit1.Value);
            WriteStat(BO2.BO2Stats.XP, (int)spinEdit2.Value);
            WriteStat(BO2.BO2Stats.kills, (int)spinEdit3.Value);
            WriteStat(BO2.BO2Stats.deaths, (int)spinEdit4.Value);
            WriteStat(BO2.BO2Stats.killstreak, (int)spinEdit5.Value);
            WriteStat(BO2.BO2Stats.time_played, (int)spinEdit6.Value);
            WriteStat(BO2.BO2Stats.PTokens, (int)spinEdit7.Value);
            WriteStat(BO2.BO2Stats.hits, (int)spinEdit8.Value);
            WriteStat(BO2.BO2Stats.wins, (int)spinEdit9.Value);
            WriteStat(BO2.BO2Stats.losses, (int)spinEdit10.Value);
            WriteStat(BO2.BO2Stats.winLossratio, (int)spinEdit11.Value);
            WriteStat(BO2.BO2Stats.gamesplayed, (int)spinEdit12.Value);
            WriteStat(BO2.BO2Stats.winstreak, (int)spinEdit13.Value);
        }

        private void simpleButton29_Click(object sender, EventArgs e)
        {
            Console.SetMemory(0x843489C8 + 0x243, new byte[] { 0xff, 0xff, 0xff });
            Console.SetMemory(0x843489C8 + 0x5f, new byte[] { 0x7f });
            Console.SetMemory(0x843489C8 + 0x24f, new byte[] { 
                        0x7f, 0xff, 0xff, 0xff, 0x7f, 0xff, 0xff, 0xff, 0x7f, 0xff, 0xff, 0xff, 0x7f, 0xff, 0xff, 0xff, 
                        0x7f, 0xff, 0xff, 0xff, 0x7f, 0xff, 0xff, 0xff, 0x7f, 0xff, 0xff, 0xff, 0x7f, 0xff, 0xff, 0xff
                     });
        }

        public void sendClientCommand(string cmd)
        {
            Console.CallVoid(clientCommand, 0, cmd);
        }

        private void simpleButton31_Click(object sender, EventArgs e)
        {
            
        }

        private void simpleButton32_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            byte[] Data = Console.GetMemory(0xC4921000, 0x00001000);
            if (Data != null)
            {
                MemoryStream s = new MemoryStream(Data);
                BinaryReader br = new BinaryReader(s);
                for (int i = 0; i <= 12; i++)
                {
                    br.BaseStream.Position = 0x251 + (i * 0x100);
                    string gt = Encoding.ASCII.GetString(br.ReadBytes(15));
                    br.BaseStream.Position = 0x251 + (i * 0x100) + 0xDC;
                    byte[] ip = br.ReadBytes(4);
                    string ipStr = "";
                    ipStr += Convert.ToInt32(ip[3]) + ".";
                    ipStr += Convert.ToInt32(ip[2]) + ".";
                    ipStr += Convert.ToInt32(ip[1]) + ".";
                    ipStr += Convert.ToInt32(ip[0]);
                    if (ipStr != "0.0.0.0")
                    {
                        ListViewItem lvi = new ListViewItem(gt);
                        lvi.SubItems.Add(ipStr);
                        listView1.Items.Add(lvi);
                    }
                }
                br.Close();
                s.Close();
            }
            
        }

        private void xtraTabPage3_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}