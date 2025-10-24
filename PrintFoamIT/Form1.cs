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
using WindowsFormsApp1.Properties;
using ZXing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
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

            
            pictureBox1.Image = Properties.Resources._2;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            


            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(printer);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }


            comboBox2.Items.Clear();
            comboBox2.Items.Add("Code 128");
            comboBox2.Items.Add("Code 39");
            comboBox2.Items.Add("QR Code");
            comboBox2.Items.Add("PDF417");
            comboBox2.Items.Add("Data Matrix");

            comboBox2.SelectedIndex = 0;

            // ตัวอย่าง: 125 = 1.25 นิ้ว, 80 = 0.8 นิ้ว (≈ 31.75mm x 20.32mm)
            PaperSize customPaperSize = new PaperSize("CustomSize", 125, 80);
            printDocument1.DefaultPageSettings.PaperSize = customPaperSize;
            printPreviewControl1.Document = printDocument1;

            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            pictureBox2.Image = Properties.Resources._1;
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
                int imageHeight = 15;
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
