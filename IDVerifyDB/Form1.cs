using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.Data.OleDb;
using ZXing;
using System.Data;

namespace IDVerifyDB
{
    public partial class Form1 : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        string con_string = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=IDVerify.accdb";
        bool camOn=false;
        string lastscanned="";
        public Form1()
        {
            InitializeComponent();

            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cboDevice.Items.Add(filterInfo.Name);
            cboDevice.SelectedIndex = 0;

        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startFeed();
        }

        private void startFeed()
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;
            cboDevice.Enabled = false;
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            camOn= true;
            linkLabel1.Visible = false;
            linkLabel2.Visible = false;
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            startButton.Enabled = true;
            cboDevice.Enabled = true;
            linkLabel1.Visible = true;
            linkLabel2.Visible = true;

            if (captureDevice.IsRunning)
            {
                captureDevice.Stop();
                camOn= false;
                pictureBox1.Image = null;
            }
                

        }
        private void CaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
  
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = bitmap;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (camOn)
                captureDevice.Stop();
        }
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && camOn)
            {
                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                {
                    newLog(result.Text);
                    studentIDeditbox.Text = result.Text;
                }
                    
            }

        }

        private void newLog(string log)
        {
            OleDbConnection connection = new OleDbConnection(con_string);
            connection.Open();
            OleDbCommand cmd = new OleDbCommand("Select * from students where firstname=@id", connection);
            cmd.Parameters.AddWithValue("id", log);
            OleDbDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                if ( lastscanned != reader["id"].ToString())
                {
                    listBox1.Items.Insert(0, reader["id"] + " " + reader["lastname"] + " " + reader["firstname"] + " " + reader["middlename"] + " " + DateTime.Now);
                    labelStudentID.Text = reader["id"].ToString();
                    labelStudentName.Text = reader["lastname"].ToString() + " " + reader["firstname"].ToString() + " " + reader["middlename"].ToString();
                    labelAddress.Text = reader["address"].ToString();
                    lastscanned = reader["id"].ToString();
                }
                
            }connection.Close();
            //else
            //{
            //    MessageBox.Show("Can't find query in database.");
            //    connection.Close();
            //}
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            startFeed();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            importQr();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            importQr();
        }

        private void importQr()
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "JPG|*.jpg" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    BarcodeReader reader = new BarcodeReader();
                    Result result = reader.Decode((Bitmap)Image.FromFile(ofd.FileName));
                    if (result != null)
                    {
                        newLog(result.Text);
                    }
                    else
                    {
                        MessageBox.Show("No QR code found.");
                    }
                }
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                newLog(studentIDeditbox.Text);
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            newLog(studentIDeditbox.Text);
        }
    }
}
