namespace OpenDental{
	partial class FormMapAreaContainerDetail {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapAreaContainerDetail));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textWidth = new OpenDental.ValidDouble();
			this.label2 = new System.Windows.Forms.Label();
			this.labelHeight = new System.Windows.Forms.Label();
			this.textHeight = new OpenDental.ValidDouble();
			this.label8 = new System.Windows.Forms.Label();
			this.comboSite = new OpenDental.UI.ComboBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(144, 155);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 113;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(225, 155);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 112;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 16);
			this.label4.TabIndex = 115;
			this.label4.Text = "Description";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(119, 23);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(100, 20);
			this.textDescription.TabIndex = 114;
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(119, 45);
			this.textWidth.MaxVal = 100000000D;
			this.textWidth.MinVal = 0D;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(43, 20);
			this.textWidth.TabIndex = 118;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(44, 45);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 16);
			this.label2.TabIndex = 116;
			this.label2.Text = "Width";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHeight
			// 
			this.labelHeight.Location = new System.Drawing.Point(44, 68);
			this.labelHeight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(71, 16);
			this.labelHeight.TabIndex = 117;
			this.labelHeight.Text = "Height";
			this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(119, 67);
			this.textHeight.MaxVal = 100000000D;
			this.textHeight.MinVal = 0D;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(43, 20);
			this.textHeight.TabIndex = 119;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(71, 91);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(45, 16);
			this.label8.TabIndex = 120;
			this.label8.Text = "Site";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSite
			// 
			this.comboSite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboSite.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.comboSite.ForeColor = System.Drawing.Color.Black;
			this.comboSite.Location = new System.Drawing.Point(119, 89);
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(134, 21);
			this.comboSite.TabIndex = 121;
			// 
			// FormMapAreaContainerDetail
			// 
			this.ClientSize = new System.Drawing.Size(312, 191);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelHeight);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMapAreaContainerDetail";
			this.Text = "Map Area Container Detail";
			this.Load += new System.EventHandler(this.FormMapAreaContainers_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textDescription;
		private ValidDouble textWidth;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelHeight;
		private ValidDouble textHeight;
		private System.Windows.Forms.Label label8;
		private UI.ComboBox comboSite;
	}
}