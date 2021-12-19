using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using ReadMemory;
using Common;
using FileConfigManager;

namespace viceextended
{
    public partial class Form1 : Form
    {
        string[] tp = new string[10];
        bool hooked;
        bool isfalling = false;

        public Form1()
        {
            InitializeComponent();
            //Name Protection
            tp[0] = "y";
            tp[1] = "u";
            tp[2] = "s";
            tp[3] = " ";
            tp[4] = "K";
            tp[5] = " ";
            tp[6] = "t";
            tp[7] = "i";
            tp[8] = "r";
            tp[9] = "B";
            this.Text += tp[3] + tp[9] + tp[0] + tp[5] + tp[4] + tp[1] +
                tp[8] + tp[6] + tp[7] + tp[2];
        }

        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        bool iskeydown = false;
        Memory mem;
        GlobalKeyboardHook gkh = new GlobalKeyboardHook();
        FCM cfg = new FCM();
        Keys[] cfgkeys = new Keys[16];

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("With this mod you can do some stuff like in GTA:SA or IV\n\n" +
            "Some features:\n" +
            "You can press a number for changing weapons faster - perfectly works\n" +
            "MP3 player track can be changed - mostly works\n" +
            "Realistic fall damage - perfectly works\n" +
            "Surrender when wanted - perfectly works\n\n"+
            "Extra:\n" + 
            "Save Everywhere - perfectly works\n" +
            "Paint Car - perfectly works",
                "Vice City Extended 1.0",
                MessageBoxButtons.OK,MessageBoxIcon.Information);
            GC.Collect(0, GCCollectionMode.Forced);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsProcessOpen("gta-vc") && !hooked)
            {
                timer1.Enabled = true;
            }
            else
            {
                Process.Start("gta-vc.exe");
                timer1.Enabled = true;
            }
            button2.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists("gta-vc.exe"))
            {
                MessageBox.Show("gta-vc.exe not found!","Vice City Extended 1.0",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            else if (!System.IO.File.Exists("viceextended.ini"))
            {
                MessageBox.Show("viceextended.ini not found!", "Vice City Extended 1.0",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            if (IsProcessOpen("gta-vc") && !hooked)
            {
                button2.Text = "Inject Game";
            }

            //Load Keys
            KeysConverter kc = new KeysConverter();
            cfg.SetMaxLines(20);
            cfgkeys[0] = (Keys)kc.ConvertFromString(cfg.ReadData("viceextended.ini", "MP3Key"));
            cfgkeys[1] = (Keys)kc.ConvertFromString(cfg.ReadData("viceextended.ini", "RadioKey"));
            cfgkeys[2] = (Keys)kc.ConvertFromString(cfg.ReadData("viceextended.ini", "SGame"));
            for (int i = 0; i < 10; i++)
            {
                cfgkeys[i + 3] = (Keys)kc.ConvertFromString(cfg.ReadData("viceextended.ini", "WKey" + i));
            }
            cfgkeys[13] = (Keys)kc.ConvertFromString(cfg.ReadData("viceextended.ini", "CCol1"));
            cfgkeys[14] = (Keys)kc.ConvertFromString(cfg.ReadData("viceextended.ini", "CCol2"));
            cfgkeys[15] = (Keys)kc.ConvertFromString(cfg.ReadData("viceextended.ini", "GUp"));
        }

        string GetLine(string fileName, int line)
        {
            using (var sr = new System.IO.StreamReader(fileName, Encoding.UTF7))
            {
                for (int i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }

        void gkh_KeyUp(object sender, KeyEventArgs e)
        {
            iskeydown = false;
        }

        void gkh_KeyDown(object sender, KeyEventArgs e)
        {
            if (iskeydown == false)
            {
                if (e.KeyCode == cfgkeys[0])
                {
                    if (mem.ReadOffset(0x7E49C0, 0x23C) == 9 ||
                        mem.ReadPointer(0xA10B50) > 0)
                    {
                        int stindex = mem.ReadPointer(0x97881C);
                        int stmax = mem.ReadPointer(0xA108B0);
                        byte[] Buffer = BitConverter.GetBytes((stindex <= stmax-1) ? stindex + 1 : 0);
                        mem.Write(0x97881C, Buffer);
                        Buffer = BitConverter.GetBytes(8);
                        mem.WritePointer(0x7E49C0, 0x23C, Buffer);
                        SendKeys.Send(cfgkeys[1].ToString() + cfgkeys[1].ToString());
                        if (mem.ReadOffset(0x7E49C0, 0x23C) != 9)
                        {
                            Buffer = BitConverter.GetBytes(8);
                            mem.WritePointer(0x7E49C0, 0x23C, Buffer);
                            SendKeys.Send(cfgkeys[1].ToString() + cfgkeys[1].ToString());
                        }
                        iskeydown = true;
                    }
                }
                if (e.KeyCode == cfgkeys[2])
                {
                    if (mem.ReadPointer(0x869728) == 32)
                    {
                        byte[] Buffer = BitConverter.GetBytes(15);
                        mem.Write(0x869728, Buffer);
                        iskeydown = true;
                    }
                }
                if (e.KeyCode == cfgkeys[15] && mem.ReadPointer(0x978D98) > 0)
                {
                    byte[] Buffer = BitConverter.GetBytes(62);
                    mem.WritePointer(0x94AD28, 0x244, Buffer);
                    iskeydown = true;
                }
                if (IsPlayerAtPoint((float)335.4154, (float)-235.028, (float)12.1682, 10))
                {
                    if (e.KeyCode == cfgkeys[13])
                    {
                        byte[] Buffer = BitConverter.GetBytes(mem.ReadOffset(0x7E49C0, 0x1A0) < 94 ? mem.ReadOffset(0x7E49C0, 0x1A0) + 1 : 0);
                        mem.WritePointer(0x7E49C0, 0x1A0, Buffer);
                    }
                    if (e.KeyCode == cfgkeys[14])
                    {
                        byte[] Buffer = BitConverter.GetBytes(mem.ReadOffset(0x7E49C0, 0x1A1) < 94 ? mem.ReadOffset(0x7E49C0, 0x1A1) + 1 : 0);
                        mem.WritePointer(0x7E49C0, 0x1A1, Buffer);
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    if (e.KeyCode == cfgkeys[i+3]) ChangeWeapon(i);
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (IsProcessOpen("gta-vc") && !hooked)
            {
                mem = new Memory("gta-vc", 0x001F0FFF);
                gkh.Hook();
                gkh.KeyDown += new KeyEventHandler(gkh_KeyDown);
                gkh.KeyUp += new KeyEventHandler(gkh_KeyUp);
                hooked = true;
            }
            if (isfalling && mem.ReadOffset(0x94AD28, 0x244) != 50 && mem.ReadFloatOffset(0x94AD28, 0x78) == 0)
            {
                byte[] Buffer = BitConverter.GetBytes((float)0.0);
                mem.WritePointer(0x94AD28, 0x354, Buffer);
                isfalling = false;
            }
            if (mem.ReadFloatOffset(0x94AD28, 0x78) <= -0.4 && mem.ReadOffset(0x94AD28, 0x244) != 50)
            {
                isfalling = true;
            }
            else if (!IsProcessOpen("gta-vc") && hooked)
            {
                Application.Exit();
            }
        }

        private bool IsPlayerAtPoint(float x, float y, float z, float radius)
        {
            float px = mem.ReadFloatOffset(0x94AD28, 0x34);
            float py = mem.ReadFloatOffset(0x94AD28, 0x38);
            float pz = mem.ReadFloatOffset(0x94AD28, 0x3C);

            if (px + radius >= x && px - radius <= x && py + radius >= y && py - radius <= y
                && pz + radius >= z && pz - radius <= z)
            {
                return true;
            }
            return false;
        }

        private void ChangeWeapon(int slot)
        {
            byte[] Buffer = BitConverter.GetBytes(slot);
            mem.WritePointer(0x94AD28, 0x60C, Buffer);
            iskeydown = true;
        }
    }
}
