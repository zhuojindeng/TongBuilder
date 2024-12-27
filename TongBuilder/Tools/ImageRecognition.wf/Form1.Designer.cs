namespace ImageRecognition.wf
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            orgimgpath = new TextBox();
            orgimg = new PictureBox();
            orgimgbutton = new Button();
            ((System.ComponentModel.ISupportInitialize)orgimg).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(28, 17);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "选择图片";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // orgimgpath
            // 
            orgimgpath.Location = new Point(144, 20);
            orgimgpath.Name = "orgimgpath";
            orgimgpath.Size = new Size(465, 23);
            orgimgpath.TabIndex = 1;
            // 
            // orgimg
            // 
            orgimg.Location = new Point(28, 49);
            orgimg.Name = "orgimg";
            orgimg.Size = new Size(581, 548);
            orgimg.SizeMode = PictureBoxSizeMode.Zoom;
            orgimg.TabIndex = 2;
            orgimg.TabStop = false;
            // 
            // orgimgbutton
            // 
            orgimgbutton.Location = new Point(183, 603);
            orgimgbutton.Name = "orgimgbutton";
            orgimgbutton.Size = new Size(75, 23);
            orgimgbutton.TabIndex = 3;
            orgimgbutton.Text = "选择匹配图";
            orgimgbutton.UseVisualStyleBackColor = true;
            orgimgbutton.Click += orgimg_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(628, 659);
            Controls.Add(orgimgbutton);
            Controls.Add(orgimg);
            Controls.Add(orgimgpath);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)orgimg).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox orgimgpath;
        private PictureBox orgimg;
        private Button orgimgbutton;
    }
}
