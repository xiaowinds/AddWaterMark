using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AddWaterMark
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Text = "下中";
            // comboBox1.
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Clear();
                for (int i = 0; i < openFileDialog1.FileNames.Count(); i++)
                {
                    int index = dataGridView1.Rows.Add();

                    this.dataGridView1.Rows[index].Cells[0].Value = i + 1;
                    this.dataGridView1.Rows[index].Cells[1].Value = openFileDialog1.FileNames[i];
                    this.dataGridView1.Rows[index].Cells[2].Value = "";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null)
                {
                    return;
                }

                var path = dataGridView1.Rows[i].Cells[1].Value.ToString();
                Image img = Image.FromFile(path);
                Form1.AddImageSignText(
                          path,
                          Path.GetFileName(path),
                          dataGridView1.Rows[i].Cells[2].Value.ToString(),
                          comboBox1.Text,
                          100,
                         int.Parse(textBox1.Text)
                           );

            }
            MessageBox.Show("水印添加成功，保存在当前文件目录下");
        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="imgPath">服务器图片相对路径</param>
        /// <param name="filename">保存文件名</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkStatus">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="fontname">字体</param>
        public static void AddImageSignText(string imgPath, string filename, string watermarkText, string watermarkStatus, int quality, int fontsize, string fontname = "微软雅黑")
        {
            byte[] _ImageBytes = File.ReadAllBytes(imgPath);
            Image img = Image.FromStream(new System.IO.MemoryStream(_ImageBytes));


            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {

                case "上左":
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case "上中":
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case "上右":
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case "中左":
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case "中中":
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case "中右":
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case "下左":
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case "下中":
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case "下右":
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Black), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            var np = Path.GetDirectoryName(imgPath) + "\\水印";
            if (!Directory.Exists(np))
            {
                Directory.CreateDirectory(np);
            }

            if (ici != null)
                img.Save(np + "\\" + filename, ici, encoderParams);
            else
                img.Save(np + "\\" + filename);

            g.Dispose();
            img.Dispose();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData != Keys.Enter)
            {
                //继续原来base.ProcessCmdKey中的处理 
                return base.ProcessCmdKey(ref msg, keyData);
            }
            if (!this.dataGridView1.IsCurrentCellInEditMode)   //如果当前单元格处于编辑模式
            {
                //继续原来base.ProcessCmdKey中的处理 
                return base.ProcessCmdKey(ref msg, keyData);
            }

            TextBox textBox = this.dataGridView1.EditingControl as TextBox;
            int nStart = textBox.SelectionStart;//得到当前光标的位置
            string text = textBox.Text;
            if (nStart < 0 || nStart > text.Length)
                return false;
            //光标签名的字
            string text1 = "";
            if (nStart > 0)
            {
                text1 = text.Substring(0, nStart);
            }
            //光标后面的字
            string text2 = "";
            if (nStart < text.Length)
            {
                text2 = text.Substring(nStart, text.Length - nStart);
            }
            text = text1 + "\r\n" + text2;
            textBox.Text = text;
            this.dataGridView1.CurrentCell.Value = text;
            textBox.Select(nStart + 2, 0);

            return true;


        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 8 && !Char.IsDigit(e.KeyChar))//如果不是输入数字就不让输入
            {
                e.Handled = true;
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int iMax = 999;//首先设置上限值
            if (textBox1.Text != null && textBox1.Text != "")//判断TextBox的内容不为空，如果不判断会导致后面的非数字对比异常
            {
                if (int.Parse(textBox1.Text) > iMax)//num就是传进来的值,如果大于上限（输入的值），那就强制为上限-1，或者就是上限值？
                {
                    textBox1.Text = (iMax - 1).ToString();
                }
            }
        }
    }
}
