using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpReceivablesBreakdown {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpReceivablesBreakdown));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.label1 = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.labelProvider = new System.Windows.Forms.Label();
			this.listClinic = new OpenDental.UI.ListBoxOD();
			this.labClinic = new System.Windows.Forms.Label();
			this.groupInsBox = new System.Windows.Forms.GroupBox();
			this.radioWriteoffProc = new System.Windows.Forms.RadioButton();
			this.radioWriteoffPay = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.groupInsBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(468, 286);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(378, 286);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(317, 56);
			this.date1.Name = "date1";
			this.date1.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(314, 37);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(230, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Up to the following date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(27, 58);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(126, 147);
			this.listProv.TabIndex = 30;
			// 
			// labelProvider
			// 
			this.labelProvider.Location = new System.Drawing.Point(24, 39);
			this.labelProvider.Name = "labelProvider";
			this.labelProvider.Size = new System.Drawing.Size(103, 16);
			this.labelProvider.TabIndex = 7;
			this.labelProvider.Text = "Providers";
			this.labelProvider.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listClinic
			// 
			this.listClinic.Location = new System.Drawing.Point(171, 58);
			this.listClinic.Name = "listClinic";
			this.listClinic.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClinic.Size = new System.Drawing.Size(126, 147);
			this.listClinic.TabIndex = 31;
			// 
			// labClinic
			// 
			this.labClinic.Location = new System.Drawing.Point(171, 39);
			this.labClinic.Name = "labClinic";
			this.labClinic.Size = new System.Drawing.Size(103, 16);
			this.labClinic.TabIndex = 32;
			this.labClinic.Text = "Clinic";
			this.labClinic.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupInsBox
			// 
			this.groupInsBox.Controls.Add(this.radioWriteoffProc);
			this.groupInsBox.Controls.Add(this.radioWriteoffPay);
			this.groupInsBox.Location = new System.Drawing.Point(27, 217);
			this.groupInsBox.Name = "groupInsBox";
			this.groupInsBox.Size = new System.Drawing.Size(270, 66);
			this.groupInsBox.TabIndex = 33;
			this.groupInsBox.TabStop = false;
			this.groupInsBox.Text = "Show Insurance Writeoffs";
			// 
			// radioWriteoffProc
			// 
			this.radioWriteoffProc.Location = new System.Drawing.Point(7, 38);
			this.radioWriteoffProc.Name = "radioWriteoffProc";
			this.radioWriteoffProc.Size = new System.Drawing.Size(199, 17);
			this.radioWriteoffProc.TabIndex = 1;
			this.radioWriteoffProc.TabStop = true;
			this.radioWriteoffProc.Text = "Using procedure date.";
			this.radioWriteoffProc.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffPay
			// 
			this.radioWriteoffPay.Location = new System.Drawing.Point(7, 20);
			this.radioWriteoffPay.Name = "radioWriteoffPay";
			this.radioWriteoffPay.Size = new System.Drawing.Size(240, 17);
			this.radioWriteoffPay.TabIndex = 0;
			this.radioWriteoffPay.TabStop = true;
			this.radioWriteoffPay.Text = "Using insurance payment date.";
			this.radioWriteoffPay.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(561, 20);
			this.label2.TabIndex = 34;
			this.label2.Text = "This report only takes payment plans into account when using the default charge l" +
    "ogic (Age Credits and Debits).";
			// 
			// FormRpReceivablesBreakdown
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(567, 324);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupInsBox);
			this.Controls.Add(this.labClinic);
			this.Controls.Add(this.listClinic);
			this.Controls.Add(this.labelProvider);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpReceivablesBreakdown";
			this.ShowInTaskbar = false;
			this.Text = "Receivables Breakdown";
			this.Load += new System.EventHandler(this.FormRpReceivablesBreakdown_Load);
			this.groupInsBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private MonthCalendar date1;
		private Label label1;
		private OpenDental.UI.ListBoxOD listProv;
		private Label labelProvider;
		private OpenDental.UI.ListBoxOD listClinic;
		private Label labClinic;
		private GroupBox groupInsBox;
		private RadioButton radioWriteoffPay;
		private RadioButton radioWriteoffProc;
		private Label label2;
	}
}
