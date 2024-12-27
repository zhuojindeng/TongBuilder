using Emgu.CV;

namespace ImageRecognition.wf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void SelectImage(Action<string> callback)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();     //显示选择文件对话框
            openFileDialog1.Filter = "All files (*.*)|*.*|image files |*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                callback(openFileDialog1.FileName);

            }
            openFileDialog1.Dispose();

        }

        private void ShowOrgImage()
        {
            using (var img = Image.FromFile(this.orgimgpath.Text))
            {
                orgimg.Image = (Image)img.Clone();
            }
            orgimgbutton.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectImage((name) =>
            {
                this.orgimgpath.Text = name;          //显示文件路径
                ShowOrgImage();
                CvInvoke.WaitKey(0);  //Wait for the key pressing event
                CvInvoke.DestroyAllWindows(); //Destroy all windows if key is pressed
            });
        }

        private void orgimg_Click(object sender, EventArgs e)
        {
            SelectImage((name) =>
            {
                string observedImage = this.orgimgpath.Text;
                //起个线程去 进行匹配吧
                Task.Run(() => {
                    long matchTime;
                    var result = DrawMatches.Match(observedImage, name, out matchTime);
                    //ImageViewer.Show(result, String.Format("Matched in {0} milliseconds", matchTime));
                    String win1 = String.Format("Matched in {0} milliseconds", matchTime); //The name of the window
                    CvInvoke.NamedWindow(win1);
                    CvInvoke.Imshow(win1, result);
                });
            });
        }
    }
}
