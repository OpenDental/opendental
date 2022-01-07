namespace TestForm {
	partial class Form1 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.button1 = new System.Windows.Forms.Button();
			this.butBlue = new System.Windows.Forms.Button();
			this.butRed = new System.Windows.Forms.Button();
			this.butInvalid = new System.Windows.Forms.Button();
			this.chart = new SparksToothChart.GraphicalToothChart();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(356,12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75,23);
			this.button1.TabIndex = 1;
			this.button1.Text = "refresh";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// butBlue
			// 
			this.butBlue.Location = new System.Drawing.Point(551,12);
			this.butBlue.Name = "butBlue";
			this.butBlue.Size = new System.Drawing.Size(75,23);
			this.butBlue.TabIndex = 2;
			this.butBlue.Text = "blue";
			this.butBlue.UseVisualStyleBackColor = true;
			this.butBlue.Click += new System.EventHandler(this.butBlue_Click);
			// 
			// butRed
			// 
			this.butRed.Location = new System.Drawing.Point(588,41);
			this.butRed.Name = "butRed";
			this.butRed.Size = new System.Drawing.Size(75,23);
			this.butRed.TabIndex = 3;
			this.butRed.Text = "red";
			this.butRed.UseVisualStyleBackColor = true;
			this.butRed.Click += new System.EventHandler(this.butRed_Click);
			// 
			// butInvalid
			// 
			this.butInvalid.Location = new System.Drawing.Point(230,12);
			this.butInvalid.Name = "butInvalid";
			this.butInvalid.Size = new System.Drawing.Size(75,23);
			this.butInvalid.TabIndex = 4;
			this.butInvalid.Text = "Invalidate";
			this.butInvalid.UseVisualStyleBackColor = true;
			this.butInvalid.Click += new System.EventHandler(this.butInvalid_Click);
			// 
			// chart
			// 
			this.chart.Location = new System.Drawing.Point(186,105);
			this.chart.Name = "chart";
			this.chart.TabIndex = 5;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(943,757);
			this.Controls.Add(this.chart);
			this.Controls.Add(this.butInvalid);
			this.Controls.Add(this.butRed);
			this.Controls.Add(this.butBlue);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button butBlue;
		private System.Windows.Forms.Button butRed;
		private System.Windows.Forms.Button butInvalid;
		private SparksToothChart.GraphicalToothChart chart;

	}
}

