namespace projektRozproszone
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
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panel1 = new Panel();
            label2 = new Label();
            label1 = new Label();
            textBoxPort = new TextBox();
            textBoxAdress = new TextBox();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            textBox1 = new TextBox();
            colorDialog1 = new ColorDialog();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(43, 43, 44);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBoxPort);
            panel1.Controls.Add(textBoxAdress);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(button1);
            panel1.Controls.Add(textBox1);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 479);
            panel1.Margin = new Padding(100);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(50);
            panel1.Size = new Size(1264, 202);
            panel1.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = SystemColors.AppWorkspace;
            label2.Location = new Point(554, 170);
            label2.Name = "label2";
            label2.Size = new Size(29, 15);
            label2.TabIndex = 7;
            label2.Text = "port";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.AppWorkspace;
            label1.Location = new Point(554, 146);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 6;
            label1.Text = "addres";
            // 
            // textBoxPort
            // 
            textBoxPort.Location = new Point(601, 168);
            textBoxPort.Margin = new Padding(3, 2, 3, 2);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(110, 23);
            textBoxPort.TabIndex = 5;
            textBoxPort.Text = "50080";
            // 
            // textBoxAdress
            // 
            textBoxAdress.Location = new Point(601, 143);
            textBoxAdress.Margin = new Padding(3, 2, 3, 2);
            textBoxAdress.Name = "textBoxAdress";
            textBoxAdress.Size = new Size(110, 23);
            textBoxAdress.TabIndex = 4;
            textBoxAdress.Text = "172.18.129.18";
            textBoxAdress.TextChanged += textBoxAdress_TextChanged;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 12F);
            button3.Location = new Point(554, 109);
            button3.Name = "button3";
            button3.Size = new Size(156, 29);
            button3.TabIndex = 3;
            button3.Text = "Random Color";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 12F);
            button2.Location = new Point(635, 74);
            button2.Name = "button2";
            button2.Size = new Size(75, 29);
            button2.TabIndex = 2;
            button2.Text = "Color";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 12F);
            button1.Location = new Point(554, 74);
            button1.Name = "button1";
            button1.Size = new Size(75, 29);
            button1.TabIndex = 1;
            button1.Text = "Play";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // textBox1
            // 
            textBox1.AcceptsTab = true;
            textBox1.BorderStyle = BorderStyle.FixedSingle;
            textBox1.Font = new Font("Segoe UI", 12F);
            textBox1.Location = new Point(554, 39);
            textBox1.MaxLength = 20;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(156, 29);
            textBox1.TabIndex = 0;
            textBox1.Text = "unnamed";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            ClientSize = new Size(1264, 681);
            Controls.Add(panel1);
            DoubleBuffered = true;
            Name = "Form1";
            Text = "Projekt rozproszone";
            Load += Form1_Load;
            Paint += Form1_Paint;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            MouseMove += Form1_MouseMove;
            MouseWheel += Form1_MouseWheel;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Panel panel1;
        private Button button2;
        private Button button1;
        private TextBox textBox1;
        private ColorDialog colorDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private Button button3;
        private Label label2;
        private Label label1;
        private TextBox textBoxPort;
        private TextBox textBoxAdress;
    }
}
