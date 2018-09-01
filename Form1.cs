using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Anaglyph3DS
{
    public partial class Form1 : Form
    {
        List<Image> imgList = new List<Image>();
        public static Dictionary<string, string> meta = new Dictionary<string, string>();

        public static Color leftColor, rightColor;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            leftColor = Color.Red;
            rightColor = Color.Cyan;

            LeftColorButton.BackColor = leftColor;
            RightColorButton.BackColor = rightColor;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void framesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = OFD.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                String fileNom = OFD.FileName;
                imgList = new List<Image>();

                if (File.Exists(fileNom))
                {
                    meta = new Dictionary<string, string>();
                    imgList = MPOUtils.GetJPEGImages(fileNom);

                    for (int i = 0; i < imgList.Count; i++)
                    {
                        //String fnom = Path.GetFileNameWithoutExtension(fileNom) + i + ".jpg";
                        Bitmap b = new Bitmap(imgList[i]);

                        imgList[i] = b;
                        
                        //imgList[i].Save(fnom, System.Drawing.Imaging.ImageFormat.Jpeg);
                       // Console.WriteLine("Saving " + fnom);
                    }

                    pictureBox2.Image = imgList[0];
                    pictureBox3.Image = imgList[1];

                    Bitmap anna = ImgProc.MultiplyStereo(imgList[0], imgList[1]);

                    pictureBox1.Image = anna;
                  
                  //  anna.Save("anaglyph.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                    DataTable dt = new DataTable();

                    dt.Columns.Add(new DataColumn("Attribute"));
                    dt.Columns.Add(new DataColumn("Value"));

                    
                   /* Dictionary<string, string>.KeyCollection.Enumerator anouma = meta.Keys.GetEnumerator();

                    for (int i = 0; i < meta.Keys.Count; i++)
                    {
                        String val;

                        String key = anouma.Current;
                        anouma.MoveNext();
                        meta.TryGetValue(key, out val);
                        dt.Rows.Add(new object[] { key, val   });
                    }

                    dataGridView1.DataSource = dt; */

                    

                }


            }

        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            anaglyphSFD.Filter = " "+comboBox1.SelectedItem+"|"+"*"+comboBox1.SelectedItem;
            frameSFD.Filter = " " + comboBox1.SelectedItem + "|" + "*" + comboBox1.SelectedItem;
            
        }

        private void anaglyphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            anaglyphSFD.FileName = "";
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("It doesn't look like an image has been loaded yet.", "I AM ERROR.");
                return;
            }

            DialogResult dr = anaglyphSFD.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                String outNom = anaglyphSFD.FileName;

                pictureBox1.Image.Save(outNom);
            }

        }

        private void frameAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frameSFD.FileName = "";
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("It doesn't look like an image has been loaded yet.", "I AM ERROR.");
                return;
            }

            DialogResult dr = frameSFD.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                String outNom = frameSFD.FileName;

                pictureBox2.Image.Save(outNom);
            }
        }

        private void frameBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frameSFD.FileName = "";
            if (pictureBox3.Image == null)
            {
                MessageBox.Show("It doesn't look like an image has been loaded yet.", "I AM ERROR.");
                return;
            }

            DialogResult dr = frameSFD.ShowDialog();

            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                String outNom = frameSFD.FileName;

                pictureBox3.Image.Save(outNom);
            }
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {
            

        }

        private void button3_Click(object sender, EventArgs e)
        {
            anaglyphToolStripMenuItem_Click(null, null);
        }

        private void LeftColorButton_Click(object sender, EventArgs e)
        {
            leftColorChooser.ShowDialog();
            leftColor = leftColorChooser.Color;
       
            if (pictureBox1.Image != null)
            pictureBox1.Image = ImgProc.MultiplyStereo(imgList[0], imgList[1]);
            LeftColorButton.BackColor = leftColor;
                   
        }

        private void RightColorButton_Click(object sender, EventArgs e)
        {
            rightColorChooser.ShowDialog();
            rightColor = rightColorChooser.Color;
            if (pictureBox1.Image != null)
            pictureBox1.Image = ImgProc.MultiplyStereo(imgList[0], imgList[1]);
            RightColorButton.BackColor = rightColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frameAToolStripMenuItem_Click(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            frameBToolStripMenuItem_Click(null, null);
        }
    }
}
