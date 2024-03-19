namespace Server_Multi_Client_Project
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
            listBox1 = new ListBox();
            startBtn = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            disconnectBtn = new Button();
            showDataBtn = new Button();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.Font = new Font("Simplified Arabic", 14.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 32;
            listBox1.Location = new Point(34, 31);
            listBox1.Margin = new Padding(3, 2, 3, 2);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(792, 228);
            listBox1.TabIndex = 0;
            // 
            // startBtn
            // 
            startBtn.BackColor = Color.Lime;
            startBtn.FlatStyle = FlatStyle.Popup;
            startBtn.Font = new Font("Showcard Gothic", 18F, FontStyle.Italic);
            startBtn.ForeColor = Color.White;
            startBtn.Location = new Point(69, 336);
            startBtn.Margin = new Padding(3, 2, 3, 2);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(170, 52);
            startBtn.TabIndex = 1;
            startBtn.Text = "Start";
            startBtn.UseVisualStyleBackColor = false;
            startBtn.Click += startBtn_Click;
            // 
            // disconnectBtn
            // 
            disconnectBtn.BackColor = Color.Red;
            disconnectBtn.FlatStyle = FlatStyle.Popup;
            disconnectBtn.Font = new Font("Showcard Gothic", 18F, FontStyle.Italic);
            disconnectBtn.ForeColor = Color.White;
            disconnectBtn.Location = new Point(635, 336);
            disconnectBtn.Margin = new Padding(3, 2, 3, 2);
            disconnectBtn.Name = "disconnectBtn";
            disconnectBtn.Size = new Size(170, 52);
            disconnectBtn.TabIndex = 2;
            disconnectBtn.Text = "Stop";
            disconnectBtn.UseVisualStyleBackColor = false;
            disconnectBtn.Click += disconnectBtn_Click;
            // 
            // showDataBtn
            // 
            showDataBtn.BackColor = Color.Gray;
            showDataBtn.FlatStyle = FlatStyle.Popup;
            showDataBtn.Font = new Font("Showcard Gothic", 18F, FontStyle.Italic);
            showDataBtn.ForeColor = Color.White;
            showDataBtn.Location = new Point(353, 336);
            showDataBtn.Margin = new Padding(3, 2, 3, 2);
            showDataBtn.Name = "showDataBtn";
            showDataBtn.Size = new Size(170, 52);
            showDataBtn.TabIndex = 3;
            showDataBtn.Text = "show Data";
            showDataBtn.UseVisualStyleBackColor = false;
            showDataBtn.Click += showDataBtn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 64, 64);
            ClientSize = new Size(879, 445);
            Controls.Add(showDataBtn);
            Controls.Add(disconnectBtn);
            Controls.Add(startBtn);
            Controls.Add(listBox1);
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            MaximumSize = new Size(895, 484);
            MinimumSize = new Size(895, 484);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Server ";
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBox1;
        private Button startBtn;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Button disconnectBtn;
        private Button showDataBtn;
    }
}
