using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRxDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRxDefEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textDrug = new System.Windows.Forms.TextBox();
			this.textNotes = new System.Windows.Forms.TextBox();
			this.textRefills = new System.Windows.Forms.TextBox();
			this.textDisp = new System.Windows.Forms.TextBox();
			this.textSig = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listAlerts = new OpenDental.UI.ListBoxOD();
			this.butDelete = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.checkControlled = new System.Windows.Forms.CheckBox();
			this.butAddAllergy = new OpenDental.UI.Button();
			this.labelRxNorm = new System.Windows.Forms.Label();
			this.butRxNormSelect = new OpenDental.UI.Button();
			this.textRxCui = new System.Windows.Forms.TextBox();
			this.butAddProblem = new OpenDental.UI.Button();
			this.butAddMedication = new OpenDental.UI.Button();
			this.checkProcRequired = new System.Windows.Forms.CheckBox();
			this.textPatInstruction = new OpenDental.ODtextBox();
			this.labelPatInstruct = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(52, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Drug";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(20, 197);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(114, 37);
			this.label3.TabIndex = 2;
			this.label3.Text = "Notes (will not show on printout)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(46, 154);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 14);
			this.label4.TabIndex = 3;
			this.label4.Text = "Refills";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(46, 133);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(88, 14);
			this.label5.TabIndex = 4;
			this.label5.Text = "Disp";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(56, 88);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(78, 14);
			this.label6.TabIndex = 5;
			this.label6.Text = "Sig";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDrug
			// 
			this.textDrug.Location = new System.Drawing.Point(134, 24);
			this.textDrug.Name = "textDrug";
			this.textDrug.Size = new System.Drawing.Size(254, 20);
			this.textDrug.TabIndex = 0;
			// 
			// textNotes
			// 
			this.textNotes.AcceptsReturn = true;
			this.textNotes.Location = new System.Drawing.Point(134, 193);
			this.textNotes.Multiline = true;
			this.textNotes.Name = "textNotes";
			this.textNotes.Size = new System.Drawing.Size(386, 92);
			this.textNotes.TabIndex = 7;
			// 
			// textRefills
			// 
			this.textRefills.Location = new System.Drawing.Point(134, 150);
			this.textRefills.Name = "textRefills";
			this.textRefills.Size = new System.Drawing.Size(114, 20);
			this.textRefills.TabIndex = 5;
			// 
			// textDisp
			// 
			this.textDisp.Location = new System.Drawing.Point(134, 129);
			this.textDisp.Name = "textDisp";
			this.textDisp.Size = new System.Drawing.Size(114, 20);
			this.textDisp.TabIndex = 4;
			// 
			// textSig
			// 
			this.textSig.AcceptsReturn = true;
			this.textSig.Location = new System.Drawing.Point(134, 84);
			this.textSig.Multiline = true;
			this.textSig.Name = "textSig";
			this.textSig.Size = new System.Drawing.Size(254, 44);
			this.textSig.TabIndex = 3;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(542, 511);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(542, 551);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(50, 441);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(84, 14);
			this.label2.TabIndex = 7;
			this.label2.Text = "Alerts";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listAlerts
			// 
			this.listAlerts.Location = new System.Drawing.Point(134, 440);
			this.listAlerts.Name = "listAlerts";
			this.listAlerts.Size = new System.Drawing.Size(120, 95);
			this.listAlerts.TabIndex = 9;
			this.listAlerts.DoubleClick += new System.EventHandler(this.listAlerts_DoubleClick);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(36, 551);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 24);
			this.butDelete.TabIndex = 15;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(132, 551);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(260, 26);
			this.label7.TabIndex = 17;
			this.label7.Text = "This does not damage any patient records";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkControlled
			// 
			this.checkControlled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkControlled.Location = new System.Drawing.Point(3, 45);
			this.checkControlled.Name = "checkControlled";
			this.checkControlled.Size = new System.Drawing.Size(145, 20);
			this.checkControlled.TabIndex = 1;
			this.checkControlled.Text = "Controlled Substance";
			this.checkControlled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkControlled.UseVisualStyleBackColor = true;
			// 
			// butAddAllergy
			// 
			this.butAddAllergy.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddAllergy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddAllergy.Location = new System.Drawing.Point(260, 511);
			this.butAddAllergy.Name = "butAddAllergy";
			this.butAddAllergy.Size = new System.Drawing.Size(117, 24);
			this.butAddAllergy.TabIndex = 12;
			this.butAddAllergy.Text = "Add Allergy";
			this.butAddAllergy.Click += new System.EventHandler(this.butAddAllergy_Click);
			// 
			// labelRxNorm
			// 
			this.labelRxNorm.Location = new System.Drawing.Point(46, 174);
			this.labelRxNorm.Name = "labelRxNorm";
			this.labelRxNorm.Size = new System.Drawing.Size(88, 14);
			this.labelRxNorm.TabIndex = 21;
			this.labelRxNorm.Text = "RxNorm";
			this.labelRxNorm.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butRxNormSelect
			// 
			this.butRxNormSelect.Location = new System.Drawing.Point(498, 170);
			this.butRxNormSelect.Name = "butRxNormSelect";
			this.butRxNormSelect.Size = new System.Drawing.Size(22, 22);
			this.butRxNormSelect.TabIndex = 260;
			this.butRxNormSelect.Text = "...";
			this.butRxNormSelect.Click += new System.EventHandler(this.butRxNormSelect_Click);
			// 
			// textRxCui
			// 
			this.textRxCui.Location = new System.Drawing.Point(134, 171);
			this.textRxCui.Name = "textRxCui";
			this.textRxCui.ReadOnly = true;
			this.textRxCui.Size = new System.Drawing.Size(358, 20);
			this.textRxCui.TabIndex = 6;
			// 
			// butAddProblem
			// 
			this.butAddProblem.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddProblem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddProblem.Location = new System.Drawing.Point(260, 440);
			this.butAddProblem.Name = "butAddProblem";
			this.butAddProblem.Size = new System.Drawing.Size(117, 24);
			this.butAddProblem.TabIndex = 10;
			this.butAddProblem.Text = "Add Problem";
			this.butAddProblem.Click += new System.EventHandler(this.butAddProblem_Click);
			// 
			// butAddMedication
			// 
			this.butAddMedication.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddMedication.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddMedication.Location = new System.Drawing.Point(260, 475);
			this.butAddMedication.Name = "butAddMedication";
			this.butAddMedication.Size = new System.Drawing.Size(117, 24);
			this.butAddMedication.TabIndex = 11;
			this.butAddMedication.Text = "Add Medication";
			this.butAddMedication.Click += new System.EventHandler(this.butAddMedication_Click);
			// 
			// checkProcRequired
			// 
			this.checkProcRequired.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcRequired.Enabled = false;
			this.checkProcRequired.Location = new System.Drawing.Point(3, 63);
			this.checkProcRequired.Name = "checkProcRequired";
			this.checkProcRequired.Size = new System.Drawing.Size(145, 20);
			this.checkProcRequired.TabIndex = 2;
			this.checkProcRequired.Text = "Is Proc Required";
			this.checkProcRequired.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcRequired.UseVisualStyleBackColor = true;
			// 
			// textPatInstruction
			// 
			this.textPatInstruction.AcceptsTab = true;
			this.textPatInstruction.BackColor = System.Drawing.SystemColors.Window;
			this.textPatInstruction.DetectLinksEnabled = false;
			this.textPatInstruction.DetectUrls = false;
			this.textPatInstruction.Location = new System.Drawing.Point(134, 291);
			this.textPatInstruction.Name = "textPatInstruction";
			this.textPatInstruction.QuickPasteType = OpenDentBusiness.QuickPasteType.Rx;
			this.textPatInstruction.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPatInstruction.Size = new System.Drawing.Size(386, 143);
			this.textPatInstruction.TabIndex = 8;
			this.textPatInstruction.Text = "";
			// 
			// labelPatInstruct
			// 
			this.labelPatInstruct.Location = new System.Drawing.Point(7, 295);
			this.labelPatInstruct.Name = "labelPatInstruct";
			this.labelPatInstruct.Size = new System.Drawing.Size(126, 14);
			this.labelPatInstruct.TabIndex = 264;
			this.labelPatInstruct.Text = "Patient Instructions";
			this.labelPatInstruct.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormRxDefEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(634, 587);
			this.Controls.Add(this.textPatInstruction);
			this.Controls.Add(this.labelPatInstruct);
			this.Controls.Add(this.checkProcRequired);
			this.Controls.Add(this.butAddMedication);
			this.Controls.Add(this.butAddProblem);
			this.Controls.Add(this.textRxCui);
			this.Controls.Add(this.butRxNormSelect);
			this.Controls.Add(this.labelRxNorm);
			this.Controls.Add(this.butAddAllergy);
			this.Controls.Add(this.checkControlled);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.listAlerts);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textSig);
			this.Controls.Add(this.textDisp);
			this.Controls.Add(this.textRefills);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.textDrug);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRxDefEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Rx Template";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRxDefEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormRxDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textDrug;
		private System.Windows.Forms.TextBox textNotes;
		private System.Windows.Forms.TextBox textRefills;
		private System.Windows.Forms.TextBox textDisp;
		private System.Windows.Forms.TextBox textSig;
		private Label label2;
		private OpenDental.UI.ListBoxOD listAlerts;
		private OpenDental.UI.Button butDelete;
		private Label label7;// Required designer variable.
		private CheckBox checkControlled;
		private UI.Button butAddAllergy;
		private Label labelRxNorm;
		private UI.Button butRxNormSelect;
		private TextBox textRxCui;
		private UI.Button butAddProblem;
		private UI.Button butAddMedication;
		private CheckBox checkProcRequired;
		private ODtextBox textPatInstruction;
		private Label labelPatInstruct;
	}
}
