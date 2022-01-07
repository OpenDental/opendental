namespace CodeBase {
	partial class FormProgressExtended {
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
			this.textMsg = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.labelWarning = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.butCancel = new System.Windows.Forms.Button();
			this.butPause = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textMsg
			// 
			this.textMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMsg.Location = new System.Drawing.Point(12, 25);
			this.textMsg.Multiline = true;
			this.textMsg.Name = "textMsg";
			this.textMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textMsg.Size = new System.Drawing.Size(439, 70);
			this.textMsg.TabIndex = 0;
			this.textMsg.TabStop = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(9, 3);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(93, 19);
			this.label4.TabIndex = 0;
			this.label4.Text = "Progress Log";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelWarning.Location = new System.Drawing.Point(9, 98);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(292, 17);
			this.labelWarning.TabIndex = 0;
			this.labelWarning.Text = "Finishing actions. Please wait...";
			this.labelWarning.Visible = false;
			// 
			// panel1
			// 
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.butCancel);
			this.panel1.Controls.Add(this.textMsg);
			this.panel1.Controls.Add(this.butPause);
			this.panel1.Controls.Add(this.labelWarning);
			this.panel1.Location = new System.Drawing.Point(3, 9);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(460, 155);
			this.panel1.TabIndex = 0;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(376, 118);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butPause
			// 
			this.butPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPause.Location = new System.Drawing.Point(12, 118);
			this.butPause.Name = "butPause";
			this.butPause.Size = new System.Drawing.Size(75, 23);
			this.butPause.TabIndex = 0;
			this.butPause.Text = "Pause";
			this.butPause.UseVisualStyleBackColor = true;
			this.butPause.Click += new System.EventHandler(this.butPause_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.AutoScroll = true;
			this.flowLayoutPanel1.AutoSize = true;
			this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel1);
			this.flowLayoutPanel1.Controls.Add(this.panel1);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 1);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(466, 260);
			this.flowLayoutPanel1.TabIndex = 0;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AllowDrop = true;
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(460, 0);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// FormProgressExtended
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoScroll = true;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(469, 262);
			this.Controls.Add(this.flowLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "FormProgressExtended";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox textMsg;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelWarning;
		private System.Windows.Forms.Button butPause;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}