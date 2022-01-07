namespace OpenDental{
	partial class FormHL7MsgEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHL7MsgEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelPatient = new System.Windows.Forms.Label();
			this.textMsgTxt = new System.Windows.Forms.TextBox();
			this.labelMsgTxt = new System.Windows.Forms.Label();
			this.labelHL7Status = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.textAptNum = new System.Windows.Forms.TextBox();
			this.labelAptNum = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.labelNote = new System.Windows.Forms.Label();
			this.textDateTStamp = new OpenDental.ValidDate();
			this.labelDateTStamp = new System.Windows.Forms.Label();
			this.textHL7Status = new System.Windows.Forms.TextBox();
			this.labelHL7MsgNum = new System.Windows.Forms.Label();
			this.textHL7MsgNum = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(707,471);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(793,471);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(7,49);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(125,18);
			this.labelPatient.TabIndex = 8;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMsgTxt
			// 
			this.textMsgTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMsgTxt.Location = new System.Drawing.Point(132,119);
			this.textMsgTxt.Multiline = true;
			this.textMsgTxt.Name = "textMsgTxt";
			this.textMsgTxt.ReadOnly = true;
			this.textMsgTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textMsgTxt.Size = new System.Drawing.Size(736,334);
			this.textMsgTxt.TabIndex = 15;
			// 
			// labelMsgTxt
			// 
			this.labelMsgTxt.Location = new System.Drawing.Point(7,119);
			this.labelMsgTxt.Name = "labelMsgTxt";
			this.labelMsgTxt.Size = new System.Drawing.Size(125,18);
			this.labelMsgTxt.TabIndex = 9;
			this.labelMsgTxt.Text = "Message Text";
			this.labelMsgTxt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHL7Status
			// 
			this.labelHL7Status.Location = new System.Drawing.Point(7,71);
			this.labelHL7Status.Name = "labelHL7Status";
			this.labelHL7Status.Size = new System.Drawing.Size(125,18);
			this.labelHL7Status.TabIndex = 10;
			this.labelHL7Status.Text = "HL7 Status";
			this.labelHL7Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(132,49);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(155,20);
			this.textPatient.TabIndex = 41;
			// 
			// textAptNum
			// 
			this.textAptNum.Location = new System.Drawing.Point(132,93);
			this.textAptNum.Name = "textAptNum";
			this.textAptNum.ReadOnly = true;
			this.textAptNum.Size = new System.Drawing.Size(74,20);
			this.textAptNum.TabIndex = 44;
			// 
			// labelAptNum
			// 
			this.labelAptNum.Location = new System.Drawing.Point(7,93);
			this.labelAptNum.Name = "labelAptNum";
			this.labelAptNum.Size = new System.Drawing.Size(125,18);
			this.labelAptNum.TabIndex = 43;
			this.labelAptNum.Text = "AptNum";
			this.labelAptNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.Location = new System.Drawing.Point(402,5);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(466,108);
			this.textNote.TabIndex = 46;
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(293,5);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(109,18);
			this.labelNote.TabIndex = 45;
			this.labelNote.Text = "Note";
			this.labelNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTStamp
			// 
			this.textDateTStamp.Location = new System.Drawing.Point(132,27);
			this.textDateTStamp.Name = "textDateTStamp";
			this.textDateTStamp.ReadOnly = true;
			this.textDateTStamp.Size = new System.Drawing.Size(155,20);
			this.textDateTStamp.TabIndex = 48;
			// 
			// labelDateTStamp
			// 
			this.labelDateTStamp.Location = new System.Drawing.Point(7,28);
			this.labelDateTStamp.Name = "labelDateTStamp";
			this.labelDateTStamp.Size = new System.Drawing.Size(125,18);
			this.labelDateTStamp.TabIndex = 47;
			this.labelDateTStamp.Text = "DateTime Stamp";
			this.labelDateTStamp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHL7Status
			// 
			this.textHL7Status.Location = new System.Drawing.Point(132,71);
			this.textHL7Status.Name = "textHL7Status";
			this.textHL7Status.ReadOnly = true;
			this.textHL7Status.Size = new System.Drawing.Size(74,20);
			this.textHL7Status.TabIndex = 49;
			// 
			// labelHL7MsgNum
			// 
			this.labelHL7MsgNum.Location = new System.Drawing.Point(7,6);
			this.labelHL7MsgNum.Name = "labelHL7MsgNum";
			this.labelHL7MsgNum.Size = new System.Drawing.Size(125,18);
			this.labelHL7MsgNum.TabIndex = 50;
			this.labelHL7MsgNum.Text = "HL7MsgNum";
			this.labelHL7MsgNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textHL7MsgNum
			// 
			this.textHL7MsgNum.Location = new System.Drawing.Point(132,5);
			this.textHL7MsgNum.Name = "textHL7MsgNum";
			this.textHL7MsgNum.ReadOnly = true;
			this.textHL7MsgNum.Size = new System.Drawing.Size(74,20);
			this.textHL7MsgNum.TabIndex = 51;
			// 
			// FormHL7MsgEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(884,511);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textHL7MsgNum);
			this.Controls.Add(this.labelHL7MsgNum);
			this.Controls.Add(this.textHL7Status);
			this.Controls.Add(this.textDateTStamp);
			this.Controls.Add(this.labelDateTStamp);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelNote);
			this.Controls.Add(this.textAptNum);
			this.Controls.Add(this.labelAptNum);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.textMsgTxt);
			this.Controls.Add(this.labelMsgTxt);
			this.Controls.Add(this.labelHL7Status);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormHL7MsgEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HL7 Message Edit";
			this.Load += new System.EventHandler(this.FormHL7DefSegmentEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelPatient;
		private System.Windows.Forms.TextBox textMsgTxt;
		private System.Windows.Forms.Label labelMsgTxt;
		private System.Windows.Forms.Label labelHL7Status;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.TextBox textAptNum;
		private System.Windows.Forms.Label labelAptNum;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label labelNote;
		private ValidDate textDateTStamp;
		private System.Windows.Forms.Label labelDateTStamp;
		private System.Windows.Forms.TextBox textHL7Status;
		private System.Windows.Forms.Label labelHL7MsgNum;
		private System.Windows.Forms.TextBox textHL7MsgNum;
	}
}
