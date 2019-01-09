using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Drawing.Drawing2D;
using Microsoft.Win32;
using System.Drawing.Text;
using System.Reflection;


namespace ImageTranslat
{
    public partial class FrmImageTranslate : Form
    {
        public static string gInputPath = null;
        public static string gOutputPath = null, gSrcFileType = null, gTrgFileType=null;

        //RegistryKey key;
        public FrmImageTranslate()
        {
            InitializeComponent();
                       
        }
       
        private void btnProcess_Click(object sender, EventArgs e)
        {
          



        }
        static bool ExecuteCommand(string command)
        {
            bool val = true;
            try
            {
                int exitCode;

                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe");
                processStartInfo.CreateNoWindow = true;
                processStartInfo.RedirectStandardInput = true;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;

                Process process = Process.Start(processStartInfo);

                if (process != null)
                {
                    process.StandardInput.WriteLine(command);

                    process.StandardInput.Close(); // line added to stop process from hanging on ReadToEnd()

                    string outputString = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                    Console.WriteLine("output>>" + (String.IsNullOrEmpty(outputString) ? "(none)" : outputString));
                    Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                    process.Close();

                }

            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return val;

        }
        public static void ProcessConversion()
        {
            string[] files = Directory.GetFiles(gInputPath, "*." + gSrcFileType);
            foreach (string echFilenme in files)
            {
                string destFile = gOutputPath + "\\" + Path.GetFileNameWithoutExtension(echFilenme)+ "." + gTrgFileType;
                string command = "Magick " + echFilenme + " " + destFile;
                ExecuteCommand(command);
            }
        }
        private void ProcessSlice(int r,int c)
        {
            string[] files = Directory.GetFiles(gInputPath, "*." + gSrcFileType);
            foreach (string echFilenme in files)
            {
                string destFile = gOutputPath + "\\" + Path.GetFileNameWithoutExtension(echFilenme) + "." + gTrgFileType;
                string command = "Magick "+echFilenme+" -crop "+r+"x"+c+"@ +repage  +adjoin "+destFile;
                ExecuteCommand(command);
            }
        }
        public static void ProcessSlice(string filePath)
        {
            StreamReader swr = null;
            try
            {
                swr = new StreamReader(filePath);
                swr.ReadLine();
                while (!swr.EndOfStream)
                {
                    string[] split = swr.ReadLine().Split(',');
                    if (split.Length == 4)
                    {
                        if (File.Exists(split[0]))
                        {
                            string command = "Magick " + split[0] + " -crop " + split[1] + "x" + split[2] + "@ +repage  +adjoin " + split[3];
                            ExecuteCommand(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
            finally
            {
                if (swr != null)
                    swr.Close();
            }
        }
        public static void ProcessResize(string filePath)
        {
            StreamReader swr = null;
            try
            {
                swr = new StreamReader(filePath);
                swr.ReadLine();
                while (!swr.EndOfStream)
                {
                    string[] split = swr.ReadLine().Split(',');
                    if (split.Length == 4)
                    {
                        if (File.Exists(split[0]))
                        {
                            string command = "Magick " + split[0] + " -resize " + split[1] + "x" + split[2] + " " + split[3];
                            ExecuteCommand(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
            finally
            {
                if (swr != null)
                    swr.Close();
            }           
        }
        public static void ProcessTranlate(string filePath)
        {
            StreamReader swr = null;
            try
            {
                swr = new StreamReader(filePath);
                swr.ReadLine();
                while (!swr.EndOfStream)
                {
                    string[] split = swr.ReadLine().Split(',');
                    if (split.Length == 3)
                    {
                        if (File.Exists(split[0]))
                        {
                            string Points = split[1].Replace(":", ",").Replace("|", " ");
                            string command = "Magick " + split[0] + " -distort Affine \"" + Points + "\" " + split[2];
                            ExecuteCommand(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
            finally
            {
                if (swr != null)
                    swr.Close();
            }
        }
        public static void ProcessReprojection(string filePath)
        {
            StreamReader swr = null;
            try
            {
                swr = new StreamReader(filePath);
                swr.ReadLine();
                while (!swr.EndOfStream)
                {
                    string[] split = swr.ReadLine().Split(',');
                    if (split.Length == 4)
                    {
                        if (File.Exists(split[0]))
                        {
                            string Points = split[1].Replace(":", ",").Replace("|", " ");
                            string command = "gdalwarp -s_srs " + split[1] + " -t_srs " + split[2] + " " + split[0] + " " + split[3];
                            ExecuteCommand(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
            finally
            {
                if (swr != null)
                    swr.Close();
            }
        }
        public static void ProcessReprojection(string SrcPrj, string TrgPrj, string srcFile, string trgFilePath)
        {
            if (File.Exists(Path.GetDirectoryName(srcFile) + "\\" + Path.GetFileNameWithoutExtension(srcFile) + ".tfw"))
                {
                    string command = "gdalwarp -s_srs " + SrcPrj + " -t_srs " + TrgPrj + " " + srcFile + " " + trgFilePath;
                    ExecuteCommand(command);
                }
                else
                {
                }
            
        }
        private void ProcessReprojection(string SrcPrj, string TrgPrj)
        {
            string[] files = Directory.GetFiles(gInputPath, "*." + gSrcFileType);
            foreach (string echFilenme in files)
            {
                if (File.Exists(gInputPath + "\\" + Path.GetFileNameWithoutExtension(echFilenme) + ".tfw"))
                {
                    string destFile = gOutputPath + "\\" + Path.GetFileNameWithoutExtension(echFilenme) + "_Prj." + gTrgFileType;
                    string command = "gdalwarp -s_srs " + SrcPrj + " -t_srs " + TrgPrj + " " + echFilenme + " " + destFile;
                    ExecuteCommand(command);
                }
                else
                {
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBxInputPath.Text) || !Directory.Exists(txtBxInputPath.Text))
            {
                MessageBox.Show("Please Specify valid Input directory.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
           }
            if (string.IsNullOrEmpty(txtBxOutPutPath.Text) || !Directory.Exists(txtBxOutPutPath.Text))
            {
                MessageBox.Show("Please Specify valid Output directory.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBox4.SelectedItem == null || comboBox4.SelectedItem.ToString() == string.Empty)
            {
                MessageBox.Show("Please Specify valid Source File Type.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBox1.SelectedItem == null || comboBox1.SelectedItem.ToString() == string.Empty)
            {
                MessageBox.Show("Please Specify valid Target File Type.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            gInputPath = txtBxInputPath.Text;
            gOutputPath = txtBxOutPutPath.Text;
            gSrcFileType = comboBox4.SelectedItem.ToString();
            gTrgFileType = comboBox1.SelectedItem.ToString();
            ProcessConversion();           

            
        }

        private void btnInputBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fldrBwse = new FolderBrowserDialog();
            if (fldrBwse.ShowDialog() == DialogResult.OK)
                txtBxInputPath.Text = fldrBwse.SelectedPath;
            else
                txtBxInputPath.Text = string.Empty;
        }

        private void btnOutPutBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fldrBwse = new FolderBrowserDialog();
            if (fldrBwse.ShowDialog() == DialogResult.OK)
                txtBxOutPutPath.Text = fldrBwse.SelectedPath;
            else
                txtBxOutPutPath.Text = string.Empty;
        }

        private void btnSlicing_Click(object sender, EventArgs e)
        {
            int r=0;
            int c=0;
            if (string.IsNullOrEmpty(txtBxInputPath.Text) || !Directory.Exists(txtBxInputPath.Text))
            {
                MessageBox.Show("Please Specify valid Input directory.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtBxOutPutPath.Text) || !Directory.Exists(txtBxOutPutPath.Text))
            {
                MessageBox.Show("Please Specify valid Output directory.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBox3.SelectedItem == null || comboBox3.SelectedItem.ToString() == string.Empty)
            {
                MessageBox.Show("Please Specify valid Source File Type.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBox2.SelectedItem == null || comboBox2.SelectedItem.ToString() == string.Empty)
            {
                MessageBox.Show("Please Specify valid Target File Type.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
              if (string.IsNullOrEmpty(txtBxRows.Text))
            {
                MessageBox.Show("Please Specify valid Rows count.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
               if (string.IsNullOrEmpty(txtBxColumns.Text))
            {
                MessageBox.Show("Please Specify valid columns count.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
              if (!int.TryParse(txtBxRows.Text,out r))
            {
                MessageBox.Show("Please Specify valid Rows count.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
               if (!int.TryParse(txtBxColumns.Text,out c))
            {
                MessageBox.Show("Please Specify valid columns count.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            gInputPath = txtBxInputPath.Text;
            gOutputPath = txtBxOutPutPath.Text;
            gSrcFileType = comboBox3.SelectedItem.ToString();
            gTrgFileType = comboBox2.SelectedItem.ToString();

            ProcessSlice(r, c);
        }

        private void btnReproject_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(txtBxInputPath.Text) || !Directory.Exists(txtBxInputPath.Text))
            {
                MessageBox.Show("Please Specify valid Input directory.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(txtBxOutPutPath.Text) || !Directory.Exists(txtBxOutPutPath.Text))
            {
                MessageBox.Show("Please Specify valid Output directory.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Please Specify valid Source Projection.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Please Specify valid Target Projection.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            gInputPath = txtBxInputPath.Text;
            gOutputPath = txtBxOutPutPath.Text;
            gSrcFileType = "TIF";
            gTrgFileType = "TIF";
            ProcessReprojection(textBox4.Text, textBox3.Text);
        }

        private void FrmImageTranslate_Load(object sender, EventArgs e)
        {
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnfle = new OpenFileDialog();
            opnfle.Filter = "CSV file(*.csv)|*.csv";
            if (DialogResult.OK == opnfle.ShowDialog())
                textBox6.Text = opnfle.FileName;
            else
                textBox6.Text=string.Empty;
        }

        private void btnTranslate_Click(object sender, EventArgs e)
        {
            if(!File.Exists( textBox6.Text))
            {
                MessageBox.Show("Please Specify valid Input CSV file.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ProcessTranlate(textBox6.Text);
        }

        private void btnResize_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox5.Text))
            {
                MessageBox.Show("Please Specify valid Input CSV file.", "Image Transformation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ProcessResize(textBox5.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog opnfle = new OpenFileDialog();
            opnfle.Filter = "CSV file(*.csv)|*.csv";
            if (DialogResult.OK == opnfle.ShowDialog())
                textBox5.Text = opnfle.FileName;
            else
                textBox5.Text = string.Empty;
        }


    }
}
