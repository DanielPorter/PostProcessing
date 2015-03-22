namespace PostProcessing
{
    partial class PostProcessing
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
            this.txt_H1Max = new System.Windows.Forms.TextBox();
            this.txt_H2Max = new System.Windows.Forms.TextBox();
            this.txt_H3Max = new System.Windows.Forms.TextBox();
            this.txt_H4Max = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btn_Process = new System.Windows.Forms.Button();
            this.txt_DataSource = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btn_DataDirectory = new System.Windows.Forms.Button();
            this.btn_LoadShapefiles = new System.Windows.Forms.Button();
            this.RunReportWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txt_ndviMin = new System.Windows.Forms.TextBox();
            this.txt_ndviMax = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txt_NDREMax = new System.Windows.Forms.TextBox();
            this.txt_NDREmin = new System.Windows.Forms.TextBox();
            this.lbl_Progress = new System.Windows.Forms.Label();
            this.data_destination = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.txt_DataDestination = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txt_H1Max
            // 
            this.txt_H1Max.Location = new System.Drawing.Point(71, 34);
            this.txt_H1Max.Name = "txt_H1Max";
            this.txt_H1Max.Size = new System.Drawing.Size(100, 20);
            this.txt_H1Max.TabIndex = 1;
            this.txt_H1Max.Text = "5";
            // 
            // txt_H2Max
            // 
            this.txt_H2Max.Location = new System.Drawing.Point(71, 77);
            this.txt_H2Max.Name = "txt_H2Max";
            this.txt_H2Max.Size = new System.Drawing.Size(100, 20);
            this.txt_H2Max.TabIndex = 3;
            this.txt_H2Max.Text = "7";
            // 
            // txt_H3Max
            // 
            this.txt_H3Max.Location = new System.Drawing.Point(71, 126);
            this.txt_H3Max.Name = "txt_H3Max";
            this.txt_H3Max.Size = new System.Drawing.Size(100, 20);
            this.txt_H3Max.TabIndex = 5;
            this.txt_H3Max.Text = "9";
            // 
            // txt_H4Max
            // 
            this.txt_H4Max.Location = new System.Drawing.Point(71, 170);
            this.txt_H4Max.Name = "txt_H4Max";
            this.txt_H4Max.Size = new System.Drawing.Size(100, 20);
            this.txt_H4Max.TabIndex = 7;
            this.txt_H4Max.Text = "11";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "H1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "H4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "H3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 227);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "H5";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "H2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(75, 227);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "H4 Max and up";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(40, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "3  to";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(49, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "to";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(49, 129);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(16, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "to";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(49, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "to";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(40, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(91, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "Height Categories";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(360, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(78, 13);
            this.label12.TabIndex = 21;
            this.label12.Text = "NDRE Ranges";
            // 
            // btn_Process
            // 
            this.btn_Process.Location = new System.Drawing.Point(288, 132);
            this.btn_Process.Name = "btn_Process";
            this.btn_Process.Size = new System.Drawing.Size(239, 110);
            this.btn_Process.TabIndex = 31;
            this.btn_Process.Text = "Run Calculations";
            this.btn_Process.UseVisualStyleBackColor = true;
            this.btn_Process.Click += new System.EventHandler(this.createReport_click);
            // 
            // txt_DataSource
            // 
            this.txt_DataSource.Location = new System.Drawing.Point(95, 320);
            this.txt_DataSource.Name = "txt_DataSource";
            this.txt_DataSource.Size = new System.Drawing.Size(252, 20);
            this.txt_DataSource.TabIndex = 32;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(22, 323);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(67, 13);
            this.label19.TabIndex = 33;
            this.label19.Text = "Data Source";
            // 
            // btn_DataDirectory
            // 
            this.btn_DataDirectory.Location = new System.Drawing.Point(353, 317);
            this.btn_DataDirectory.Name = "btn_DataDirectory";
            this.btn_DataDirectory.Size = new System.Drawing.Size(143, 23);
            this.btn_DataDirectory.TabIndex = 34;
            this.btn_DataDirectory.Text = "Select Data Directory";
            this.btn_DataDirectory.UseVisualStyleBackColor = true;
            this.btn_DataDirectory.Click += new System.EventHandler(this.btn_DataDirectory_Click);
            // 
            // btn_LoadShapefiles
            // 
            this.btn_LoadShapefiles.Location = new System.Drawing.Point(71, 385);
            this.btn_LoadShapefiles.Name = "btn_LoadShapefiles";
            this.btn_LoadShapefiles.Size = new System.Drawing.Size(416, 43);
            this.btn_LoadShapefiles.TabIndex = 35;
            this.btn_LoadShapefiles.Text = "Load Shapefiles (takes a long time)";
            this.btn_LoadShapefiles.UseVisualStyleBackColor = true;
            this.btn_LoadShapefiles.Click += new System.EventHandler(this.loadShapefiles_Click);
            // 
            // RunReportWorker
            // 
            this.RunReportWorker.WorkerReportsProgress = true;
            this.RunReportWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RunReportWorker_DoWork);
            this.RunReportWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.RunReportWorker_ProgressChanged);
            this.RunReportWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.RunReportWorker_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(16, 248);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(511, 23);
            this.progressBar1.TabIndex = 36;
            // 
            // txt_ndviMin
            // 
            this.txt_ndviMin.Location = new System.Drawing.Point(288, 83);
            this.txt_ndviMin.Name = "txt_ndviMin";
            this.txt_ndviMin.Size = new System.Drawing.Size(100, 20);
            this.txt_ndviMin.TabIndex = 37;
            this.txt_ndviMin.Text = "-1.5";
            // 
            // txt_ndviMax
            // 
            this.txt_ndviMax.Location = new System.Drawing.Point(427, 84);
            this.txt_ndviMax.Name = "txt_ndviMax";
            this.txt_ndviMax.Size = new System.Drawing.Size(100, 20);
            this.txt_ndviMax.TabIndex = 38;
            this.txt_ndviMax.Text = "1.5";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(365, 68);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 39;
            this.label13.Text = "NDVI Ranges";
            // 
            // txt_NDREMax
            // 
            this.txt_NDREMax.Location = new System.Drawing.Point(427, 34);
            this.txt_NDREMax.Name = "txt_NDREMax";
            this.txt_NDREMax.Size = new System.Drawing.Size(100, 20);
            this.txt_NDREMax.TabIndex = 41;
            this.txt_NDREMax.Text = "1";
            // 
            // txt_NDREmin
            // 
            this.txt_NDREmin.Location = new System.Drawing.Point(288, 33);
            this.txt_NDREmin.Name = "txt_NDREmin";
            this.txt_NDREmin.Size = new System.Drawing.Size(100, 20);
            this.txt_NDREmin.TabIndex = 40;
            this.txt_NDREmin.Text = "-1";
            // 
            // lbl_Progress
            // 
            this.lbl_Progress.AutoSize = true;
            this.lbl_Progress.Location = new System.Drawing.Point(279, 278);
            this.lbl_Progress.Name = "lbl_Progress";
            this.lbl_Progress.Size = new System.Drawing.Size(48, 13);
            this.lbl_Progress.TabIndex = 42;
            this.lbl_Progress.Text = "Progress";
            // 
            // data_destination
            // 
            this.data_destination.Location = new System.Drawing.Point(353, 347);
            this.data_destination.Name = "data_destination";
            this.data_destination.Size = new System.Drawing.Size(143, 23);
            this.data_destination.TabIndex = 43;
            this.data_destination.Text = "Select Data Destination";
            this.data_destination.UseVisualStyleBackColor = true;
            this.data_destination.Click += new System.EventHandler(this.data_destination_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 347);
            this.label14.Name = "label14";
            this.label14.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label14.Size = new System.Drawing.Size(86, 13);
            this.label14.TabIndex = 44;
            this.label14.Text = "Data Destination";
            // 
            // txt_DataDestination
            // 
            this.txt_DataDestination.Location = new System.Drawing.Point(95, 347);
            this.txt_DataDestination.Name = "txt_DataDestination";
            this.txt_DataDestination.Size = new System.Drawing.Size(252, 20);
            this.txt_DataDestination.TabIndex = 45;
            // 
            // PostProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 453);
            this.Controls.Add(this.txt_DataDestination);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.data_destination);
            this.Controls.Add(this.lbl_Progress);
            this.Controls.Add(this.txt_NDREMax);
            this.Controls.Add(this.txt_NDREmin);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txt_ndviMax);
            this.Controls.Add(this.txt_ndviMin);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btn_LoadShapefiles);
            this.Controls.Add(this.btn_DataDirectory);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txt_DataSource);
            this.Controls.Add(this.btn_Process);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_H4Max);
            this.Controls.Add(this.txt_H3Max);
            this.Controls.Add(this.txt_H2Max);
            this.Controls.Add(this.txt_H1Max);
            this.Name = "PostProcessing";
            this.Text = "Post Processing";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_H1Max;
        private System.Windows.Forms.TextBox txt_H2Max;
        private System.Windows.Forms.TextBox txt_H3Max;
        private System.Windows.Forms.TextBox txt_H4Max;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btn_Process;
        private System.Windows.Forms.TextBox txt_DataSource;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Button btn_DataDirectory;
        private System.Windows.Forms.Button btn_LoadShapefiles;
        private System.ComponentModel.BackgroundWorker RunReportWorker;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox txt_ndviMin;
        private System.Windows.Forms.TextBox txt_ndviMax;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt_NDREMax;
        private System.Windows.Forms.TextBox txt_NDREmin;
        private System.Windows.Forms.Label lbl_Progress;
        private System.Windows.Forms.Button data_destination;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txt_DataDestination;
    }
}

