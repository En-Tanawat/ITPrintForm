using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private PrintDocument printDocument1;
        public Form1()
        {
            InitializeComponent();
            printDocument1 = new PrintDocument();


            printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);

            printPreviewControl1.Document = printDocument1;
            //this.Controls.Add(printPreviewControl1);

        }

        private Bitmap generatedBarcode = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();

            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(printer);
                comboBox1.SelectedIndex = 0;
            }
            comboBox2.Items.Add("Code 128");
            comboBox2.Items.Add("Code 39");
            comboBox2.Items.Add("QR Code");
            comboBox2.Items.Add("PDF417");
            comboBox2.Items.Add("Data Matrix");

            comboBox2.SelectedIndex = 0;

            PaperSize customPaperSize = new PaperSize("CustomSize", 125, 80); // sizes in hundredths of an inch -->  30mm*85mm
            printDocument1.DefaultPageSettings.PaperSize = customPaperSize;

            printPreviewControl1.Document = printDocument1;

            //pictureBox2.Image = Image.FromFile(@"\\");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

        }
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            printPreviewDialog1.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Bitmap img = new Bitmap(pictureBox1.Image);
                int imageWidth = 20;
                int imageHeight = 20;
                e.Graphics.DrawImage(img, 92, 3, imageWidth, imageHeight);
            }


            if (generatedBarcode != null)
            {
                int imageWidth1 = 60;
                int imageHeight1 = 50;
                e.Graphics.DrawImage(generatedBarcode, 5, 25, imageWidth1, imageHeight1);
            }

            string text1 = textBox3.Text;
            string text2 = textBox5.Text;
            string text3 = textBox6.Text;

            Font font = new Font("Arial", 5);
            e.Graphics.DrawString(text1, font, Brushes.Black, 10, 10);
            e.Graphics.DrawString(text2, font, Brushes.Black, 65, 40);
            e.Graphics.DrawString(text3, font, Brushes.Black, 65, 60);



        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(textBox3.Text) && pictureBox1.Image == null)
            {
                MessageBox.Show("กรุณาใส่ข้อมูลใน TextBox หรือเลือกรูปภาพก่อนพิมพ์!", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int printCount = (int)numericUpDown1.Value;

            if (printCount <= 0)
            {
                MessageBox.Show("กรุณากำหนดจำนวนการพิมพ์ที่มากกว่า 0!", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                for (int i = 0; i < printCount; i++)
                {
                    printDocument1.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการพิมพ์: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            printPreviewControl1.Document = printDocument1;

            //printPreviewControl1.Zoom = 1.0;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Logo",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image = new Bitmap(openFileDialog.FileName);

                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"เกิดข้อผิดพลาดในการโหลดรูปภาพ: {ex.Message}");
                }
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            GenerateBarcode();
        }


        private void GenerateBarcode()
        {
            string data = textBox3.Text;
            string barcodeType = comboBox2.SelectedItem.ToString();

            if (string.IsNullOrWhiteSpace(data))
            {

                generatedBarcode = null;
                //printPreviewControl1.InvalidatePreview(); 
                return;
            }

            try
            {
                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = GetBarcodeFormat(barcodeType),
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Width = 500,
                        Height = 500,
                        Margin = 2
                    }
                };

                Bitmap barcodeBitmap = barcodeWriter.Write(data);

                generatedBarcode = barcodeBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาดในการสร้าง Barcode: {ex.Message}", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private ZXing.BarcodeFormat GetBarcodeFormat(string barcodeType)
        {
            switch (barcodeType)
            {
                case "Code 128":
                    return ZXing.BarcodeFormat.CODE_128;
                case "QR Code":
                    return ZXing.BarcodeFormat.QR_CODE;
                case "Code 39":
                    return ZXing.BarcodeFormat.CODE_39;
                case "PDF417":
                    return ZXing.BarcodeFormat.PDF_417;
                case "Data Matrix":
                    return ZXing.BarcodeFormat.DATA_MATRIX;
                default:
                    throw new ArgumentException("ไม่รองรับชนิดของ Barcode นี้");
            }
        }




        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void printPreviewControl1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click_2(object sender, EventArgs e)
        {
             
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(comboBox2.SelectedItem == null)
    {
                MessageBox.Show("กรุณาเลือกประเภทของ Barcode", "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            GenerateBarcode();

            printPreviewControl1.InvalidatePreview();
        }
    }

}
