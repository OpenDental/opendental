namespace OpenDentHL7 {
	partial class FormDebug {
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
			this.label1 = new System.Windows.Forms.Label();
			this.butStart = new System.Windows.Forms.Button();
			this.textOutput = new System.Windows.Forms.TextBox();
			this.textServerName = new System.Windows.Forms.TextBox();
			this.textDatabaseName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textHL7Name = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(101, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(312, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "WARNING!!  When run in debug from VS, must be run as admin.";
			// 
			// butStart
			// 
			this.butStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.butStart.Location = new System.Drawing.Point(190, 313);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(135, 36);
			this.butStart.TabIndex = 1;
			this.butStart.Text = "Start";
			this.butStart.UseVisualStyleBackColor = true;
			this.butStart.Click += new System.EventHandler(this.butStart_Click);
			// 
			// textOutput
			// 
			this.textOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textOutput.Location = new System.Drawing.Point(12, 141);
			this.textOutput.Multiline = true;
			this.textOutput.Name = "textOutput";
			this.textOutput.ReadOnly = true;
			this.textOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textOutput.Size = new System.Drawing.Size(490, 166);
			this.textOutput.TabIndex = 0;
			// 
			// textServerName
			// 
			this.textServerName.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textServerName.Enabled = false;
			this.textServerName.Location = new System.Drawing.Point(190, 71);
			this.textServerName.Name = "textServerName";
			this.textServerName.Size = new System.Drawing.Size(188, 20);
			this.textServerName.TabIndex = 3;
			// 
			// textDatabaseName
			// 
			this.textDatabaseName.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textDatabaseName.Enabled = false;
			this.textDatabaseName.Location = new System.Drawing.Point(190, 97);
			this.textDatabaseName.Name = "textDatabaseName";
			this.textDatabaseName.Size = new System.Drawing.Size(188, 20);
			this.textDatabaseName.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(115, 74);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Server Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(100, 100);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(84, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "Database Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(87, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(97, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "HL7 Service Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textHL7Name
			// 
			this.textHL7Name.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.textHL7Name.Location = new System.Drawing.Point(190, 45);
			this.textHL7Name.Name = "textHL7Name";
			this.textHL7Name.Size = new System.Drawing.Size(188, 20);
			this.textHL7Name.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(9, 125);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(39, 13);
			this.label5.TabIndex = 9;
			this.label5.Text = "Output";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormDebug
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(514, 361);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textHL7Name);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDatabaseName);
			this.Controls.Add(this.textServerName);
			this.Controls.Add(this.textOutput);
			this.Controls.Add(this.butStart);
			this.Controls.Add(this.label1);
			this.MinimumSize = new System.Drawing.Size(530, 400);
			this.Name = "FormDebug";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "OpenDentHL7 Debug";
			this.Load += new System.EventHandler(this.FormDebug_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button butStart;
		private System.Windows.Forms.TextBox textOutput;
		private System.Windows.Forms.TextBox textServerName;
		private System.Windows.Forms.TextBox textDatabaseName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textHL7Name;
		private System.Windows.Forms.Label label5;
	}
}