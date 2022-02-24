using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMedPat {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMedPat));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textMedName = new System.Windows.Forms.TextBox();
			this.textGenericName = new System.Windows.Forms.TextBox();
			this.labelGenericName = new System.Windows.Forms.Label();
			this.textMedNote = new System.Windows.Forms.TextBox();
			this.labelGenericNotes = new System.Windows.Forms.Label();
			this.labelPatNote = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butRxNormSelect = new OpenDental.UI.Button();
			this.textRxNormDesc = new System.Windows.Forms.TextBox();
			this.labelRxNorm = new System.Windows.Forms.Label();
			this.labelEdit = new System.Windows.Forms.Label();
			this.butEdit = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.textPatNote = new OpenDental.ODtextBox();
			this.groupOrder = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.butTodayStop = new OpenDental.UI.Button();
			this.butTodayStart = new OpenDental.UI.Button();
			this.textDateStop = new OpenDental.ValidDate();
			this.label7 = new System.Windows.Forms.Label();
			this.textDateStart = new OpenDental.ValidDate();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupOrder.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(563, 457);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(461, 457);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(38, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Drug Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textMedName
			// 
			this.textMedName.Location = new System.Drawing.Point(183, 33);
			this.textMedName.Name = "textMedName";
			this.textMedName.ReadOnly = true;
			this.textMedName.Size = new System.Drawing.Size(348, 20);
			this.textMedName.TabIndex = 3;
			// 
			// textGenericName
			// 
			this.textGenericName.Location = new System.Drawing.Point(183, 55);
			this.textGenericName.Name = "textGenericName";
			this.textGenericName.ReadOnly = true;
			this.textGenericName.Size = new System.Drawing.Size(348, 20);
			this.textGenericName.TabIndex = 5;
			// 
			// labelGenericName
			// 
			this.labelGenericName.Location = new System.Drawing.Point(19, 58);
			this.labelGenericName.Name = "labelGenericName";
			this.labelGenericName.Size = new System.Drawing.Size(163, 17);
			this.labelGenericName.TabIndex = 4;
			this.labelGenericName.Text = "Generic Name";
			this.labelGenericName.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textMedNote
			// 
			this.textMedNote.Location = new System.Drawing.Point(183, 77);
			this.textMedNote.Multiline = true;
			this.textMedNote.Name = "textMedNote";
			this.textMedNote.ReadOnly = true;
			this.textMedNote.Size = new System.Drawing.Size(348, 87);
			this.textMedNote.TabIndex = 7;
			// 
			// labelGenericNotes
			// 
			this.labelGenericNotes.Location = new System.Drawing.Point(8, 81);
			this.labelGenericNotes.Name = "labelGenericNotes";
			this.labelGenericNotes.Size = new System.Drawing.Size(174, 17);
			this.labelGenericNotes.TabIndex = 6;
			this.labelGenericNotes.Text = "Medication Notes";
			this.labelGenericNotes.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelPatNote
			// 
			this.labelPatNote.Location = new System.Drawing.Point(6, 44);
			this.labelPatNote.Name = "labelPatNote";
			this.labelPatNote.Size = new System.Drawing.Size(175, 43);
			this.labelPatNote.TabIndex = 8;
			this.labelPatNote.Text = "Notes for this Patient";
			this.labelPatNote.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butRxNormSelect);
			this.groupBox1.Controls.Add(this.textRxNormDesc);
			this.groupBox1.Controls.Add(this.labelRxNorm);
			this.groupBox1.Controls.Add(this.labelEdit);
			this.groupBox1.Controls.Add(this.butEdit);
			this.groupBox1.Controls.Add(this.textMedNote);
			this.groupBox1.Controls.Add(this.textMedName);
			this.groupBox1.Controls.Add(this.textGenericName);
			this.groupBox1.Controls.Add(this.labelGenericName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.labelGenericNotes);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(36, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(565, 198);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Medication";
			// 
			// butRxNormSelect
			// 
			this.butRxNormSelect.Location = new System.Drawing.Point(533, 11);
			this.butRxNormSelect.Name = "butRxNormSelect";
			this.butRxNormSelect.Size = new System.Drawing.Size(22, 22);
			this.butRxNormSelect.TabIndex = 65;
			this.butRxNormSelect.Text = "...";
			this.butRxNormSelect.Visible = false;
			this.butRxNormSelect.Click += new System.EventHandler(this.butRxNormSelect_Click);
			// 
			// textRxNormDesc
			// 
			this.textRxNormDesc.Location = new System.Drawing.Point(183, 12);
			this.textRxNormDesc.Name = "textRxNormDesc";
			this.textRxNormDesc.ReadOnly = true;
			this.textRxNormDesc.Size = new System.Drawing.Size(348, 20);
			this.textRxNormDesc.TabIndex = 12;
			// 
			// labelRxNorm
			// 
			this.labelRxNorm.Location = new System.Drawing.Point(55, 15);
			this.labelRxNorm.Name = "labelRxNorm";
			this.labelRxNorm.Size = new System.Drawing.Size(127, 17);
			this.labelRxNorm.TabIndex = 13;
			this.labelRxNorm.Text = "RxNorm";
			this.labelRxNorm.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelEdit
			// 
			this.labelEdit.Location = new System.Drawing.Point(266, 167);
			this.labelEdit.Name = "labelEdit";
			this.labelEdit.Size = new System.Drawing.Size(128, 28);
			this.labelEdit.TabIndex = 11;
			this.labelEdit.Text = "(edit this medication for all patients)";
			// 
			// butEdit
			// 
			this.butEdit.Location = new System.Drawing.Point(183, 167);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(75, 24);
			this.butEdit.TabIndex = 9;
			this.butEdit.Text = "&Edit";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// butRemove
			// 
			this.butRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRemove.Location = new System.Drawing.Point(49, 457);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 24);
			this.butRemove.TabIndex = 8;
			this.butRemove.Text = "&Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(20, 485);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(127, 43);
			this.label5.TabIndex = 10;
			this.label5.Text = "(remove this medication from this patient)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textPatNote
			// 
			this.textPatNote.AcceptsTab = true;
			this.textPatNote.AllowsCarriageReturns = false;
			this.textPatNote.BackColor = System.Drawing.SystemColors.Window;
			this.textPatNote.DetectLinksEnabled = false;
			this.textPatNote.DetectUrls = false;
			this.textPatNote.Location = new System.Drawing.Point(183, 44);
			this.textPatNote.MaxLength = 500;
			this.textPatNote.Name = "textPatNote";
			this.textPatNote.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationPat;
			this.textPatNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPatNote.Size = new System.Drawing.Size(348, 111);
			this.textPatNote.TabIndex = 11;
			this.textPatNote.Text = "";
			// 
			// groupOrder
			// 
			this.groupOrder.Controls.Add(this.label8);
			this.groupOrder.Controls.Add(this.comboProv);
			this.groupOrder.Controls.Add(this.butTodayStop);
			this.groupOrder.Controls.Add(this.butTodayStart);
			this.groupOrder.Controls.Add(this.textDateStop);
			this.groupOrder.Controls.Add(this.label7);
			this.groupOrder.Controls.Add(this.textDateStart);
			this.groupOrder.Controls.Add(this.label4);
			this.groupOrder.Controls.Add(this.labelPatNote);
			this.groupOrder.Controls.Add(this.textPatNote);
			this.groupOrder.Location = new System.Drawing.Point(36, 217);
			this.groupOrder.Name = "groupOrder";
			this.groupOrder.Size = new System.Drawing.Size(565, 215);
			this.groupOrder.TabIndex = 64;
			this.groupOrder.TabStop = false;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(65, 22);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(116, 17);
			this.label8.TabIndex = 32;
			this.label8.Text = "Provider";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.FormattingEnabled = true;
			this.comboProv.Location = new System.Drawing.Point(183, 19);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(207, 21);
			this.comboProv.TabIndex = 31;
			// 
			// butTodayStop
			// 
			this.butTodayStop.Location = new System.Drawing.Point(288, 183);
			this.butTodayStop.Name = "butTodayStop";
			this.butTodayStop.Size = new System.Drawing.Size(64, 23);
			this.butTodayStop.TabIndex = 17;
			this.butTodayStop.Text = "Today";
			this.butTodayStop.Click += new System.EventHandler(this.butTodayStop_Click);
			// 
			// butTodayStart
			// 
			this.butTodayStart.Location = new System.Drawing.Point(288, 159);
			this.butTodayStart.Name = "butTodayStart";
			this.butTodayStart.Size = new System.Drawing.Size(64, 23);
			this.butTodayStart.TabIndex = 16;
			this.butTodayStart.Text = "Today";
			this.butTodayStart.Click += new System.EventHandler(this.butTodayStart_Click);
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(183, 185);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.Size = new System.Drawing.Size(100, 20);
			this.textDateStop.TabIndex = 15;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(18, 186);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(163, 17);
			this.label7.TabIndex = 14;
			this.label7.Text = "Date Stop";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(183, 160);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 13;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(18, 161);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(163, 17);
			this.label4.TabIndex = 12;
			this.label4.Text = "Date Start";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormMedPat
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(685, 523);
			this.Controls.Add(this.groupOrder);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.label5);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMedPat";
			this.ShowInTaskbar = false;
			this.Text = "Medication for Patient";
			this.Load += new System.EventHandler(this.FormMedPat_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupOrder.ResumeLayout(false);
			this.groupOrder.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelGenericName;
		private System.Windows.Forms.Label labelGenericNotes;
		private System.Windows.Forms.Label labelPatNote;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelEdit;
		private System.Windows.Forms.TextBox textMedName;
		private System.Windows.Forms.TextBox textGenericName;
		private System.Windows.Forms.TextBox textMedNote;
		private OpenDental.UI.Button butRemove;
		private OpenDental.UI.Button butEdit;
		private OpenDental.ODtextBox textPatNote;
		private GroupBox groupOrder;
		private ValidDate textDateStop;
		private Label label7;
		private ValidDate textDateStart;
		private Label label4;
		private UI.Button butTodayStop;
		private UI.Button butTodayStart;
		private Label label8;
		private ComboBox comboProv;
		private TextBox textRxNormDesc;
		private Label labelRxNorm;
		private UI.Button butRxNormSelect;
	}
}
