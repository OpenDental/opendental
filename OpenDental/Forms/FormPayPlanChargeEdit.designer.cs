using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPayPlanChargeEdit {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPlanChargeEdit));
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textPrincipal = new OpenDental.ValidDouble();
			this.textNote = new OpenDental.ODtextBox();
			this.textInterest = new OpenDental.ValidDouble();
			this.labelInterest = new System.Windows.Forms.Label();
			this.textDate = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textProv = new System.Windows.Forms.TextBox();
			this.textClinic = new System.Windows.Forms.TextBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textDateEdit = new OpenDental.ValidDate();
			this.label6 = new System.Windows.Forms.Label();
			this.textDateEntry = new OpenDental.ValidDate();
			this.SuspendLayout();
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 91);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "Note";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(9, 152);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 0;
			this.label5.Text = "Principal";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(360, 230);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(360, 262);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(13, 262);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(78, 26);
			this.butDelete.TabIndex = 7;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textPrincipal
			// 
			this.textPrincipal.Location = new System.Drawing.Point(111, 149);
			this.textPrincipal.MaxVal = 100000000D;
			this.textPrincipal.MinVal = 0D;
			this.textPrincipal.Name = "textPrincipal";
			this.textPrincipal.Size = new System.Drawing.Size(100, 20);
			this.textPrincipal.TabIndex = 1;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(111, 89);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Adjustment;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(245, 55);
			this.textNote.TabIndex = 9;
			this.textNote.TabStop = false;
			this.textNote.Text = "";
			// 
			// textInterest
			// 
			this.textInterest.Location = new System.Drawing.Point(111, 174);
			this.textInterest.MaxVal = 100000000D;
			this.textInterest.MinVal = 0D;
			this.textInterest.Name = "textInterest";
			this.textInterest.ReadOnly = true;
			this.textInterest.Size = new System.Drawing.Size(100, 20);
			this.textInterest.TabIndex = 2;
			// 
			// labelInterest
			// 
			this.labelInterest.Location = new System.Drawing.Point(9, 176);
			this.labelInterest.Name = "labelInterest";
			this.labelInterest.Size = new System.Drawing.Size(100, 16);
			this.labelInterest.TabIndex = 0;
			this.labelInterest.Text = "Interest";
			this.labelInterest.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(111, 64);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 8;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 68);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(10, 202);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(100, 14);
			this.label9.TabIndex = 0;
			this.label9.Text = "Provider";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProv
			// 
			this.textProv.Location = new System.Drawing.Point(111, 199);
			this.textProv.Name = "textProv";
			this.textProv.ReadOnly = true;
			this.textProv.Size = new System.Drawing.Size(201, 20);
			this.textProv.TabIndex = 3;
			// 
			// textClinic
			// 
			this.textClinic.Location = new System.Drawing.Point(111, 224);
			this.textClinic.Name = "textClinic";
			this.textClinic.ReadOnly = true;
			this.textClinic.Size = new System.Drawing.Size(201, 20);
			this.textClinic.TabIndex = 4;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(10, 227);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(100, 14);
			this.labelClinic.TabIndex = 0;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9, 42);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 10;
			this.label3.Text = "Date Edit";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEdit
			// 
			this.textDateEdit.Location = new System.Drawing.Point(111, 38);
			this.textDateEdit.Name = "textDateEdit";
			this.textDateEdit.ReadOnly = true;
			this.textDateEdit.Size = new System.Drawing.Size(128, 20);
			this.textDateEdit.TabIndex = 11;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(9, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 16);
			this.label6.TabIndex = 12;
			this.label6.Text = "Date Entry";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(111, 12);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(128, 20);
			this.textDateEntry.TabIndex = 13;
			// 
			// FormPayPlanChargeEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(447, 300);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateEdit);
			this.Controls.Add(this.textClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.textProv);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.textInterest);
			this.Controls.Add(this.labelInterest);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.textPrincipal);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayPlanChargeEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Payment Plan Charge";
			this.Load += new System.EventHandler(this.FormPayPlanCharge_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private OpenDental.ODtextBox textNote;
		private OpenDental.ValidDouble textPrincipal;
		private System.Windows.Forms.Label labelInterest;
		private System.Windows.Forms.Label label2;
		private OpenDental.ValidDouble textInterest;
		private Label label9;
		private TextBox textProv;
		private TextBox textClinic;
		private Label labelClinic;
		private Label label3;
		private ValidDate textDateEdit;
		private Label label6;
		private ValidDate textDateEntry;
		private OpenDental.ValidDate textDate;
	}
}
