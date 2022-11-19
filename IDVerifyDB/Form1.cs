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
using ZXing;

namespace IDVerifyDB
{
    public partial class Form1 : Form
    {
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        string con_string = "Data Source=DESKTOP-Q9SE38B\\SQLEXPRESS;Initial Catalog=qr_codes;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
        }



        private void startButton_Click(object sender, EventArgs e)
        {

            startButton.Enabled = false;
            stopButton.Enabled = true;
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureDevice_NewFrame;
            captureDevice.Start();

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



        private void Form1_Load(object sender, EventArgs e)
        {

            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cboDevice.Items.Add(filterInfo.Name);
            cboDevice.SelectedIndex = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice.IsRunning)
                captureDevice.Stop();
        }
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                BarcodeReader reader = new BarcodeReader();
                Result result = reader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                    listBox1.Items.Add(result.Text);
                //try
                //{
                //    SqlConnection connection = new SqlConnection(con_string);
                //    SqlCommand command = connection.CreateCommand();

                //    connection.Open();

                //    command.CommandText = "INSERT INTO dbo.log (studentno) VALUES ()";

                //    connection.Close();
                //}
                //catch (Exception ex)
                //{
                //    listBox1.Items.Add(ex.Message);
                //}
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
                        MessageBox.Show(result.Text);
                }
            }
        }

    }
}
