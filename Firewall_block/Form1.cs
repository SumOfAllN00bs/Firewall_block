using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;

namespace Firewall_block
{
    public partial class Form1 : Form
    {
        public bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        public Form1()
        {
            InitializeComponent();
            //if (!IsUserAdministrator())
            //{
            //    MessageBox.Show("Need Admin Privileges");
            //    this.Close();
            //}
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                MessageBox.Show(file);
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.WorkingDirectory = @"C:\Windows\System32";
                startInfo.FileName = "cmd.exe";
                startInfo.Verb = "runas";
                startInfo.Arguments = "/user:Administrator \"cmd /K \"netsh advfirewall firewall add rule name=\"" + Path.GetFileNameWithoutExtension(file) + "_block\" dir=in protocol=tcp program=\"" + file + "\" action=block\"" + "\"";
                process.StartInfo = startInfo;
                process.Start();
                Process process2 = new Process();
                ProcessStartInfo startInfo2 = new ProcessStartInfo();
                startInfo2.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo2.WorkingDirectory = @"C:\Windows\System32";
                startInfo2.FileName = "cmd.exe";
                startInfo2.Verb = "runas";
                startInfo2.Arguments = "/user:Administrator \"cmd /K \"netsh advfirewall firewall add rule name=\"" + Path.GetFileNameWithoutExtension(file) + "_block\" dir=out protocol=tcp program=\"" + file + "\" action=block\"" + "\"";
                process2.StartInfo = startInfo2;
                process2.Start();
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
            //MessageBox.Show("Test");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
