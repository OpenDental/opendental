namespace OpenDental{
	partial class FormApptViewItemEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptViewItemEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelBeforeTime = new System.Windows.Forms.Label();
			this.panelColor = new System.Windows.Forms.Panel();
			this.butColor = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textDesc = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listAlignment = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(377, 147);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(377, 188);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelBeforeTime
			// 
			this.labelBeforeTime.Location = new System.Drawing.Point(14, 59);
			this.labelBeforeTime.Name = "labelBeforeTime";
			this.labelBeforeTime.Size = new System.Drawing.Size(117, 17);
			this.labelBeforeTime.TabIndex = 59;
			this.labelBeforeTime.Text = "Text Color";
			this.labelBeforeTime.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// panelColor
			// 
			this.panelColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.panelColor.Location = new System.Drawing.Point(139, 57);
			this.panelColor.Name = "panelColor";
			this.panelColor.Size = new System.Drawing.Size(24, 24);
			this.panelColor.TabIndex = 60;
			// 
			// butColor
			// 
			this.butColor.Location = new System.Drawing.Point(176, 57);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(75, 24);
			this.butColor.TabIndex = 61;
			this.butColor.Text = "Change";
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(14, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(117, 17);
			this.label1.TabIndex = 62;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textDesc
			// 
			this.textDesc.Location = new System.Drawing.Point(139, 23);
			this.textDesc.Name = "textDesc";
			this.textDesc.ReadOnly = true;
			this.textDesc.Size = new System.Drawing.Size(264, 20);
			this.textDesc.TabIndex = 63;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 97);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(117, 17);
			this.label2.TabIndex = 64;
			this.label2.Text = "Alignment";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// listAlignment
			// 
			this.listAlignment.Location = new System.Drawing.Point(139, 97);
			this.listAlignment.Name = "listAlignment";
			this.listAlignment.Size = new System.Drawing.Size(120, 43);
			this.listAlignment.TabIndex = 65;
			// 
			// FormApptViewItemEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(477, 239);
			this.Controls.Add(this.listAlignment);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDesc);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.panelColor);
			this.Controls.Add(this.labelBeforeTime);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormApptViewItemEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Appointment View Item Edit";
			this.Load += new System.EventHandler(this.FormApptViewItemEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelBeforeTime;
		private System.Windows.Forms.Panel panelColor;
		private OpenDental.UI.Button butColor;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDesc;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listAlignment;
	}
}