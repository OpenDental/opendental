using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRefAttachEdit {
		private System.ComponentModel.IContainer components = null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRefAttachEdit));
			this.labelRefDate = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.labelRefType = new System.Windows.Forms.Label();
			this.labelOrder = new System.Windows.Forms.Label();
			this.textReferralNotes = new System.Windows.Forms.TextBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.labelReferralNotes = new System.Windows.Forms.Label();
			this.labelNote = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.comboRefToStatus = new System.Windows.Forms.ComboBox();
			this.labelRefToStatus = new System.Windows.Forms.Label();
			this.listSheets = new OpenDental.UI.ListBoxOD();
			this.labelSheets = new System.Windows.Forms.Label();
			this.listRefType = new OpenDental.UI.ListBoxOD();
			this.checkIsTransitionOfCare = new System.Windows.Forms.CheckBox();
			this.labelIsTransitionOfCare = new System.Windows.Forms.Label();
			this.textProc = new System.Windows.Forms.TextBox();
			this.labelProc = new System.Windows.Forms.Label();
			this.labelDateProcComplete = new System.Windows.Forms.Label();
			this.comboProvNum = new System.Windows.Forms.ComboBox();
			this.labelProv = new System.Windows.Forms.Label();
			this.butNoneProv = new OpenDental.UI.Button();
			this.butPickProv = new OpenDental.UI.Button();
			this.textDateProcCompleted = new OpenDental.ValidDate();
			this.butDetach = new OpenDental.UI.Button();
			this.textRefDate = new OpenDental.ValidDate();
			this.butChangeReferral = new OpenDental.UI.Button();
			this.butEditReferral = new OpenDental.UI.Button();
			this.textOrder = new OpenDental.ValidNum();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelRefDate
			// 
			this.labelRefDate.Location = new System.Drawing.Point(6, 188);
			this.labelRefDate.Name = "labelRefDate";
			this.labelRefDate.Size = new System.Drawing.Size(143, 17);
			this.labelRefDate.TabIndex = 16;
			this.labelRefDate.Text = "Date";
			this.labelRefDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(6, 58);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(143, 17);
			this.labelName.TabIndex = 17;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(151, 56);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(258, 20);
			this.textName.TabIndex = 1;
			// 
			// labelRefType
			// 
			this.labelRefType.Location = new System.Drawing.Point(6, 14);
			this.labelRefType.Name = "labelRefType";
			this.labelRefType.Size = new System.Drawing.Size(143, 17);
			this.labelRefType.TabIndex = 20;
			this.labelRefType.Text = "Referral Type";
			this.labelRefType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelOrder
			// 
			this.labelOrder.Location = new System.Drawing.Point(6, 209);
			this.labelOrder.Name = "labelOrder";
			this.labelOrder.Size = new System.Drawing.Size(143, 17);
			this.labelOrder.TabIndex = 73;
			this.labelOrder.Text = "Order";
			this.labelOrder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReferralNotes
			// 
			this.textReferralNotes.Location = new System.Drawing.Point(151, 97);
			this.textReferralNotes.Multiline = true;
			this.textReferralNotes.Name = "textReferralNotes";
			this.textReferralNotes.ReadOnly = true;
			this.textReferralNotes.Size = new System.Drawing.Size(454, 66);
			this.textReferralNotes.TabIndex = 78;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(150, 78);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(98, 17);
			this.labelPatient.TabIndex = 80;
			this.labelPatient.Text = "(a patient)";
			// 
			// labelReferralNotes
			// 
			this.labelReferralNotes.Location = new System.Drawing.Point(6, 99);
			this.labelReferralNotes.Name = "labelReferralNotes";
			this.labelReferralNotes.Size = new System.Drawing.Size(143, 38);
			this.labelReferralNotes.TabIndex = 81;
			this.labelReferralNotes.Text = "Notes about referral source";
			this.labelReferralNotes.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(6, 252);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(143, 38);
			this.labelNote.TabIndex = 83;
			this.labelNote.Text = "Patient note";
			this.labelNote.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(151, 250);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(454, 66);
			this.textNote.TabIndex = 1;
			// 
			// comboRefToStatus
			// 
			this.comboRefToStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRefToStatus.FormattingEnabled = true;
			this.comboRefToStatus.Location = new System.Drawing.Point(151, 228);
			this.comboRefToStatus.MaxDropDownItems = 20;
			this.comboRefToStatus.Name = "comboRefToStatus";
			this.comboRefToStatus.Size = new System.Drawing.Size(180, 21);
			this.comboRefToStatus.TabIndex = 84;
			// 
			// labelRefToStatus
			// 
			this.labelRefToStatus.Location = new System.Drawing.Point(6, 230);
			this.labelRefToStatus.Name = "labelRefToStatus";
			this.labelRefToStatus.Size = new System.Drawing.Size(143, 17);
			this.labelRefToStatus.TabIndex = 85;
			this.labelRefToStatus.Text = "Status (if referred out)";
			this.labelRefToStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listSheets
			// 
			this.listSheets.Location = new System.Drawing.Point(151, 317);
			this.listSheets.Name = "listSheets";
			this.listSheets.Size = new System.Drawing.Size(120, 69);
			this.listSheets.TabIndex = 90;
			this.listSheets.DoubleClick += new System.EventHandler(this.listSheets_DoubleClick);
			// 
			// labelSheets
			// 
			this.labelSheets.Location = new System.Drawing.Point(6, 319);
			this.labelSheets.Name = "labelSheets";
			this.labelSheets.Size = new System.Drawing.Size(143, 40);
			this.labelSheets.TabIndex = 91;
			this.labelSheets.Text = "Referral Slips\r\n(double click to view)";
			this.labelSheets.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listRefType
			// 
			this.listRefType.Location = new System.Drawing.Point(151, 12);
			this.listRefType.Name = "listRefType";
			this.listRefType.Size = new System.Drawing.Size(65, 43);
			this.listRefType.TabIndex = 92;
			this.listRefType.SelectedIndexChanged += new System.EventHandler(this.listRefType_SelectedIndexChanged);
			// 
			// checkIsTransitionOfCare
			// 
			this.checkIsTransitionOfCare.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsTransitionOfCare.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsTransitionOfCare.Location = new System.Drawing.Point(6, 387);
			this.checkIsTransitionOfCare.Name = "checkIsTransitionOfCare";
			this.checkIsTransitionOfCare.Size = new System.Drawing.Size(158, 18);
			this.checkIsTransitionOfCare.TabIndex = 93;
			this.checkIsTransitionOfCare.Text = "Transition of Care";
			this.checkIsTransitionOfCare.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsTransitionOfCare.UseVisualStyleBackColor = true;
			// 
			// labelIsTransitionOfCare
			// 
			this.labelIsTransitionOfCare.Location = new System.Drawing.Point(166, 387);
			this.labelIsTransitionOfCare.Name = "labelIsTransitionOfCare";
			this.labelIsTransitionOfCare.Size = new System.Drawing.Size(195, 17);
			this.labelIsTransitionOfCare.TabIndex = 94;
			this.labelIsTransitionOfCare.Text = "(From or To another doctor)";
			this.labelIsTransitionOfCare.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textProc
			// 
			this.textProc.BackColor = System.Drawing.SystemColors.Control;
			this.textProc.ForeColor = System.Drawing.Color.DarkRed;
			this.textProc.Location = new System.Drawing.Point(151, 406);
			this.textProc.Name = "textProc";
			this.textProc.ReadOnly = true;
			this.textProc.Size = new System.Drawing.Size(232, 20);
			this.textProc.TabIndex = 171;
			this.textProc.Text = "test";
			// 
			// labelProc
			// 
			this.labelProc.Location = new System.Drawing.Point(6, 408);
			this.labelProc.Name = "labelProc";
			this.labelProc.Size = new System.Drawing.Size(143, 17);
			this.labelProc.TabIndex = 170;
			this.labelProc.Text = "Procedure";
			this.labelProc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDateProcComplete
			// 
			this.labelDateProcComplete.Location = new System.Drawing.Point(6, 429);
			this.labelDateProcComplete.Name = "labelDateProcComplete";
			this.labelDateProcComplete.Size = new System.Drawing.Size(143, 17);
			this.labelDateProcComplete.TabIndex = 172;
			this.labelDateProcComplete.Text = "Date Proc Completed";
			this.labelDateProcComplete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProvNum
			// 
			this.comboProvNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvNum.Location = new System.Drawing.Point(151, 164);
			this.comboProvNum.MaxDropDownItems = 30;
			this.comboProvNum.Name = "comboProvNum";
			this.comboProvNum.Size = new System.Drawing.Size(258, 21);
			this.comboProvNum.TabIndex = 280;
			this.comboProvNum.SelectionChangeCommitted += new System.EventHandler(this.comboProvNum_SelectionChangeCommitted);
			// 
			// labelProv
			// 
			this.labelProv.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProv.Location = new System.Drawing.Point(6, 166);
			this.labelProv.Name = "labelProv";
			this.labelProv.Size = new System.Drawing.Size(143, 17);
			this.labelProv.TabIndex = 279;
			this.labelProv.Text = "Referring Provider";
			this.labelProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butNoneProv
			// 
			this.butNoneProv.Location = new System.Drawing.Point(433, 164);
			this.butNoneProv.Name = "butNoneProv";
			this.butNoneProv.Size = new System.Drawing.Size(44, 21);
			this.butNoneProv.TabIndex = 282;
			this.butNoneProv.Text = "None";
			this.butNoneProv.Click += new System.EventHandler(this.butNoneProv_Click);
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(412, 164);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(18, 21);
			this.butPickProv.TabIndex = 281;
			this.butPickProv.Text = "...";
			this.butPickProv.Click += new System.EventHandler(this.butPickProv_Click);
			// 
			// textDateProcCompleted
			// 
			this.textDateProcCompleted.Location = new System.Drawing.Point(151, 427);
			this.textDateProcCompleted.Name = "textDateProcCompleted";
			this.textDateProcCompleted.Size = new System.Drawing.Size(100, 20);
			this.textDateProcCompleted.TabIndex = 173;
			// 
			// butDetach
			// 
			this.butDetach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDetach.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDetach.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDetach.Location = new System.Drawing.Point(14, 477);
			this.butDetach.Name = "butDetach";
			this.butDetach.Size = new System.Drawing.Size(81, 24);
			this.butDetach.TabIndex = 86;
			this.butDetach.Text = "Detach";
			this.butDetach.Click += new System.EventHandler(this.butDetach_Click);
			// 
			// textRefDate
			// 
			this.textRefDate.Location = new System.Drawing.Point(151, 186);
			this.textRefDate.Name = "textRefDate";
			this.textRefDate.Size = new System.Drawing.Size(100, 20);
			this.textRefDate.TabIndex = 75;
			// 
			// butChangeReferral
			// 
			this.butChangeReferral.Location = new System.Drawing.Point(510, 54);
			this.butChangeReferral.Name = "butChangeReferral";
			this.butChangeReferral.Size = new System.Drawing.Size(95, 24);
			this.butChangeReferral.TabIndex = 74;
			this.butChangeReferral.Text = "Change Referral";
			this.butChangeReferral.Click += new System.EventHandler(this.butChangeReferral_Click);
			// 
			// butEditReferral
			// 
			this.butEditReferral.Location = new System.Drawing.Point(412, 54);
			this.butEditReferral.Name = "butEditReferral";
			this.butEditReferral.Size = new System.Drawing.Size(95, 24);
			this.butEditReferral.TabIndex = 74;
			this.butEditReferral.Text = "&Edit Referral";
			this.butEditReferral.Click += new System.EventHandler(this.butEditReferral_Click);
			// 
			// textOrder
			// 
			this.textOrder.Location = new System.Drawing.Point(151, 207);
			this.textOrder.MaxVal = 255;
			this.textOrder.MinVal = 0;
			this.textOrder.Name = "textOrder";
			this.textOrder.Size = new System.Drawing.Size(36, 20);
			this.textOrder.TabIndex = 72;
			this.textOrder.ShowZero = false;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(602, 477);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(521, 477);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 0;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormRefAttachEdit
			// 
			this.ClientSize = new System.Drawing.Size(689, 513);
			this.Controls.Add(this.butNoneProv);
			this.Controls.Add(this.comboProvNum);
			this.Controls.Add(this.butPickProv);
			this.Controls.Add(this.labelProv);
			this.Controls.Add(this.textDateProcCompleted);
			this.Controls.Add(this.labelDateProcComplete);
			this.Controls.Add(this.textProc);
			this.Controls.Add(this.labelProc);
			this.Controls.Add(this.labelIsTransitionOfCare);
			this.Controls.Add(this.checkIsTransitionOfCare);
			this.Controls.Add(this.listRefType);
			this.Controls.Add(this.labelSheets);
			this.Controls.Add(this.listSheets);
			this.Controls.Add(this.butDetach);
			this.Controls.Add(this.labelRefToStatus);
			this.Controls.Add(this.comboRefToStatus);
			this.Controls.Add(this.labelNote);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelReferralNotes);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.textReferralNotes);
			this.Controls.Add(this.textRefDate);
			this.Controls.Add(this.butChangeReferral);
			this.Controls.Add(this.butEditReferral);
			this.Controls.Add(this.textOrder);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelOrder);
			this.Controls.Add(this.labelRefType);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.labelRefDate);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRefAttachEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Referral Attachment";
			this.Load += new System.EventHandler(this.FormRefAttachEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelRefDate;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label labelRefType;
		private OpenDental.ValidNum textOrder;
		private System.Windows.Forms.Label labelOrder;
		private OpenDental.UI.Button butEditReferral;
		private OpenDental.ValidDate textRefDate;
		private System.Windows.Forms.TextBox textReferralNotes;
		private System.Windows.Forms.Label labelPatient;
		private System.Windows.Forms.Label labelReferralNotes;
		private Label labelNote;
		private TextBox textNote;
		private ComboBox comboRefToStatus;
		private Label labelRefToStatus;
		private OpenDental.UI.Button butDetach;
		private OpenDental.UI.ListBoxOD listSheets;
		private Label labelSheets;
		private OpenDental.UI.ListBoxOD listRefType;
		private CheckBox checkIsTransitionOfCare;
		private Label labelIsTransitionOfCare;
		private TextBox textProc;
		private Label labelProc;
		private ValidDate textDateProcCompleted;
		private Label labelDateProcComplete;
		private UI.Button butChangeReferral;
		private UI.Button butNoneProv;
		private ComboBox comboProvNum;
		private UI.Button butPickProv;
		private Label labelProv;
	}
}
