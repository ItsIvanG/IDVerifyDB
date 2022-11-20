using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.Data.OleDb;
using ZXing;

namespace IDVerifyDB
{
    public partial class Form1 : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        string con_string = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\itsiv\\source\\repos\\IDVerifyDB\\IDVerifyDB\\IDVerify.accdb";
        bool camOn=false;
        
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

            startButton.Enabled = false;
            stopButton.Enabled = true;
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();
            camOn= true;

        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            startButton.Enabled = true;

            if (captureDevice.IsRunning)
                captureDevice.Stop();

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
            if (pictureBox1.Image != null)
            {
                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                    newLog(result.Text);
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "JPG|*.jpg" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image = Image.FromFile(ofd.FileName);
                    BarcodeReader reader = new BarcodeReader();
                    Result result = reader.Decode((Bitmap)pictureBox2.Image);
                    if (result != null)
                        newLog(result.Text);
                }
            }
        }

        private void newLog(string log)
        {
            OleDbConnection connection = new OleDbConnection(con_string);
            connection.Open();
            OleDbCommand cmd = new OleDbCommand("Select * from students where id=@id", connection);
            cmd.Parameters.AddWithValue("id", log);
            OleDbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                listBox1.Items.Add(reader["id"]+" "+ reader["lastname"]+" " + reader["firstname"] + " " +reader["middlename"] + " "+DateTime.Now);
                connection.Close();
            }
            else
            {
                connection.Close();
            }
            
        }

    }
}
