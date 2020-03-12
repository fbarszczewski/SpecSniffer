namespace Spec.Sniffer_WPF
{
    partial class AudioIO
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.RecTimer = new System.Windows.Forms.Timer(this.components);
            this.PlayTimer = new System.Windows.Forms.Timer(this.components);
            this.MicLevelLabel = new System.Windows.Forms.Label();
            this.pictureBox_front = new System.Windows.Forms.PictureBox();
            this.pictureBox_back = new System.Windows.Forms.PictureBox();
            this.AudioLevelLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.StopButton = new System.Windows.Forms.Button();
            this.SoundButton = new System.Windows.Forms.Button();
            this.PlayButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_front)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_back)).BeginInit();
            this.SuspendLayout();
            // 
            // RecTimer
            // 
            this.RecTimer.Enabled = true;
            this.RecTimer.Interval = 10;
            this.RecTimer.Tick += new System.EventHandler(this.RecTimer_Tick);
            // 
            // PlayTimer
            // 
            this.PlayTimer.Enabled = true;
            this.PlayTimer.Interval = 10;
            this.PlayTimer.Tick += new System.EventHandler(this.PlayTimer_Tick);
            // 
            // MicLevelLabel
            // 
            this.MicLevelLabel.AutoSize = true;
            this.MicLevelLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.MicLevelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.MicLevelLabel.Location = new System.Drawing.Point(0, 0);
            this.MicLevelLabel.Margin = new System.Windows.Forms.Padding(0);
            this.MicLevelLabel.Name = "MicLevelLabel";
            this.MicLevelLabel.Padding = new System.Windows.Forms.Padding(10, 4, 0, 0);
            this.MicLevelLabel.Size = new System.Drawing.Size(91, 24);
            this.MicLevelLabel.TabIndex = 7;
            this.MicLevelLabel.Text = "Input level";
            // 
            // pictureBox_front
            // 
            this.pictureBox_front.BackColor = System.Drawing.Color.MidnightBlue;
            this.pictureBox_front.Location = new System.Drawing.Point(14, 30);
            this.pictureBox_front.Name = "pictureBox_front";
            this.pictureBox_front.Size = new System.Drawing.Size(555, 48);
            this.pictureBox_front.TabIndex = 9;
            this.pictureBox_front.TabStop = false;
            // 
            // pictureBox_back
            // 
            this.pictureBox_back.BackColor = System.Drawing.SystemColors.MenuText;
            this.pictureBox_back.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox_back.Location = new System.Drawing.Point(12, 26);
            this.pictureBox_back.Name = "pictureBox_back";
            this.pictureBox_back.Size = new System.Drawing.Size(560, 55);
            this.pictureBox_back.TabIndex = 8;
            this.pictureBox_back.TabStop = false;
            // 
            // AudioLevelLabel
            // 
            this.AudioLevelLabel.AutoSize = true;
            this.AudioLevelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.AudioLevelLabel.Location = new System.Drawing.Point(0, 83);
            this.AudioLevelLabel.Margin = new System.Windows.Forms.Padding(0);
            this.AudioLevelLabel.Name = "AudioLevelLabel";
            this.AudioLevelLabel.Padding = new System.Windows.Forms.Padding(10, 4, 0, 0);
            this.AudioLevelLabel.Size = new System.Drawing.Size(103, 24);
            this.AudioLevelLabel.TabIndex = 10;
            this.AudioLevelLabel.Text = "Output level";
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.SystemColors.MenuText;
            this.progressBar1.ForeColor = System.Drawing.Color.MidnightBlue;
            this.progressBar1.Location = new System.Drawing.Point(13, 111);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(559, 54);
            this.progressBar1.TabIndex = 11;
            // 
            // StopButton
            // 
            this.StopButton.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.StopButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.StopButton.Location = new System.Drawing.Point(109, 171);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(91, 35);
            this.StopButton.TabIndex = 15;
            this.StopButton.Text = "Stop [F2]";
            this.StopButton.UseVisualStyleBackColor = false;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // SoundButton
            // 
            this.SoundButton.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.SoundButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.SoundButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.SoundButton.Location = new System.Drawing.Point(414, 171);
            this.SoundButton.Name = "SoundButton";
            this.SoundButton.Size = new System.Drawing.Size(158, 35);
            this.SoundButton.TabIndex = 14;
            this.SoundButton.Text = "Sound Settings [F3]";
            this.SoundButton.UseVisualStyleBackColor = false;
            this.SoundButton.Click += new System.EventHandler(this.SoundButton_Click);
            // 
            // PlayButton
            // 
            this.PlayButton.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.PlayButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.PlayButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.PlayButton.Location = new System.Drawing.Point(12, 171);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(91, 35);
            this.PlayButton.TabIndex = 13;
            this.PlayButton.Text = "Play [F1]";
            this.PlayButton.UseVisualStyleBackColor = false;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // AudioIO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.ClientSize = new System.Drawing.Size(584, 218);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.SoundButton);
            this.Controls.Add(this.PlayButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.pictureBox_front);
            this.Controls.Add(this.pictureBox_back);
            this.Controls.Add(this.AudioLevelLabel);
            this.Controls.Add(this.MicLevelLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "AudioIO";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Audio Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AudioIO_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AudioIO_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_front)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_back)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer RecTimer;
        private System.Windows.Forms.Timer PlayTimer;
        private System.Windows.Forms.Label MicLevelLabel;
        private System.Windows.Forms.PictureBox pictureBox_front;
        private System.Windows.Forms.PictureBox pictureBox_back;
        private System.Windows.Forms.Label AudioLevelLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button SoundButton;
        private System.Windows.Forms.Button PlayButton;
    }
}