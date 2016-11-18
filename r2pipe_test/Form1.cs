﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace r2pipe_test
{    
    public partial class Form1 : Form
    {        
        R2PIPE_WRAPPER r2pw = null;
        private RConfig rconfig = null;
        private string fileName = null;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            rconfig = new RConfig();
            CheckR2path();

            r2pw    = new R2PIPE_WRAPPER(rconfig);
            //assign controls
            r2pw.add_control("output", txtOutput);
            r2pw.add_control("dissasembly", webBrowser1);
            r2pw.add_control("strings", txtStrings);
            r2pw.add_control("functions_listview", listView1);
            r2pw.add_control("imports_listview", lstImports);
            r2pw.add_control("hexview", webBrowser2);
            //assign menu optrions
            r2pw.add_menucmd("View", "Functions", "afl", mainMenu);
            //load some example file
            LoadFile(@"c:\windows\SysWOW64\notepad.exe");            
        }
        private void DoLoadFile()
        {
            if (!File.Exists(fileName))
            {
                r2pw.Show(string.Format("Wops!\n{0}\nfile not found...", fileName), "LoadFile");
                //MessageBox.Show(string.Format("Wops!\n{0}\nfile not found...", fileName), "LoadFile", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            r2pw.open(fileName);
            r2pw.run("pd 100", "dissasembly");
            r2pw.run("izq", "strings");
            r2pw.run("aaa;aflj", "functions_listview",false,new List<string> { "name", "offset" });            
            r2pw.run("iij", "imports_listview", false, new List<string> { "name", "plt" });
            r2pw.run("px 2000", "hexview");            
        }
        private void CheckR2path()
        {
            string r2path = rconfig.r2path;
            if (r2path == null)
            {
                r2pw.Show("Form1: CheckR2path(): Path for 'radare2.exe' not found...", "radare2.exe not found");
                openFileDialog1.FileName = "radare2.exe";
                openFileDialog1.Title = "Please, locate your radare2.exe binary";
                openFileDialog1.ShowDialog();
                rconfig.save("r2path", openFileDialog1.FileName);
            }
        }
        private void LoadFile(String fileName)
        {
            Text = String.Format("r2pipe gui .net v1.0 - {0}", fileName);
            if (!File.Exists(rconfig.r2path))
            {
                r2pw.Show(string.Format("Form1: LoadFile(): {0}\nWops! {1} not found...\n", fileName, this.rconfig.r2path), "radare2.exe not found");
                return;
            }
            webBrowser1.Refresh();
            webBrowser2.Refresh();
            this.fileName = fileName;
            Thread newThread = new Thread(new ThreadStart(this.DoLoadFile));
            newThread.Start();
            cmbCmdline.Focus();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = r2pw.fileName;
            openFileDialog1.ShowDialog();
            if(openFileDialog1.FileName.Length>0)
                LoadFile(openFileDialog1.FileName);
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            r2pw.exit();
        }
        private void txtOutput_TextChanged(object sender, EventArgs e)
        {
            if (txtOutput.TextLength > 0)
            {
                txtOutput.SelectionStart = txtOutput.TextLength - 1;
                txtOutput.SelectionLength = 0;
                txtOutput.Focus();
                cmbCmdline.Focus();
            }
        }
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtOutput.Clear();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            r2pw.exit();
            Environment.Exit(0);
        }
        private void cmbCmdline_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                cmbCmdline.Items.Remove("");
                cmbCmdline.Items.Add(cmbCmdline.Text);
                r2pw.run(cmbCmdline.Text, "output", true); // append
                cmbCmdline.Items.Add("");
                cmbCmdline.Text = "";
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string msg=((ListView)sender).SelectedItems[0].Text;
            string res=r2pw.run("? " + msg,"output");
            string address=res.Split(' ')[1];
            r2pw.run("pd "+address, "output",true);
        }
    }
}
