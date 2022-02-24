using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDiseaseEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiseaseEdit));
			this.textNote = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textProblem = new System.Windows.Forms.TextBox();
			this.textIcd9 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textSnomed = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboSnomedProblemType = new System.Windows.Forms.ComboBox();
			this.labelSnomedProblemType = new System.Windows.Forms.Label();
			this.butTodayStop = new OpenDental.UI.Button();
			this.butTodayStart = new OpenDental.UI.Button();
			this.textDateStop = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.labelFunctionalStatus = new System.Windows.Forms.Label();
			this.comboEhrFunctionalStatus = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textIcd10 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(118, 195);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(322, 120);
			this.textNote.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 195);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 19);
			this.label2.TabIndex = 5;
			this.label2.Text = "Problem";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProblem
			// 
			this.textProblem.Location = new System.Drawing.Point(118, 12);
			this.textProblem.Name = "textProblem";
			this.textProblem.ReadOnly = true;
			this.textProblem.Size = new System.Drawing.Size(199, 20);
			this.textProblem.TabIndex = 7;
			// 
			// textIcd9
			// 
			this.textIcd9.Location = new System.Drawing.Point(118, 38);
			this.textIcd9.Name = "textIcd9";
			this.textIcd9.ReadOnly = true;
			this.textIcd9.Size = new System.Drawing.Size(321, 20);
			this.textIcd9.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(14, 39);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 19);
			this.label3.TabIndex = 8;
			this.label3.Text = "ICD9";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStatus
			// 
			this.labelStatus.Location = new System.Drawing.Point(3, 118);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(111, 15);
			this.labelStatus.TabIndex = 82;
			this.labelStatus.Text = "Status";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.Cursor = System.Windows.Forms.Cursors.Default;
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.Location = new System.Drawing.Point(118, 116);
			this.comboStatus.MaxDropDownItems = 10;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(126, 21);
			this.comboStatus.TabIndex = 83;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(14, 143);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 19);
			this.label4.TabIndex = 5;
			this.label4.Text = "Start Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(14, 169);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 19);
			this.label5.TabIndex = 5;
			this.label5.Text = "Stop Date";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSnomed
			// 
			this.textSnomed.Location = new System.Drawing.Point(118, 90);
			this.textSnomed.Name = "textSnomed";
			this.textSnomed.ReadOnly = true;
			this.textSnomed.Size = new System.Drawing.Size(322, 20);
			this.textSnomed.TabIndex = 87;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(14, 90);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 19);
			this.label6.TabIndex = 88;
			this.label6.Text = "SNOMED CT";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSnomedProblemType
			// 
			this.comboSnomedProblemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSnomedProblemType.FormattingEnabled = true;
			this.comboSnomedProblemType.Location = new System.Drawing.Point(118, 321);
			this.comboSnomedProblemType.Name = "comboSnomedProblemType";
			this.comboSnomedProblemType.Size = new System.Drawing.Size(209, 21);
			this.comboSnomedProblemType.TabIndex = 89;
			// 
			// labelSnomedProblemType
			// 
			this.labelSnomedProblemType.Location = new System.Drawing.Point(12, 322);
			this.labelSnomedProblemType.Name = "labelSnomedProblemType";
			this.labelSnomedProblemType.Size = new System.Drawing.Size(100, 17);
			this.labelSnomedProblemType.TabIndex = 90;
			this.labelSnomedProblemType.Text = "Problem Type";
			this.labelSnomedProblemType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butTodayStop
			// 
			this.butTodayStop.Location = new System.Drawing.Point(263, 167);
			this.butTodayStop.Name = "butTodayStop";
			this.butTodayStop.Size = new System.Drawing.Size(64, 23);
			this.butTodayStop.TabIndex = 86;
			this.butTodayStop.Text = "Today";
			this.butTodayStop.Click += new System.EventHandler(this.butTodayStop_Click);
			// 
			// butTodayStart
			// 
			this.butTodayStart.Location = new System.Drawing.Point(263, 141);
			this.butTodayStart.Name = "butTodayStart";
			this.butTodayStart.Size = new System.Drawing.Size(65, 23);
			this.butTodayStart.TabIndex = 85;
			this.butTodayStart.Text = "Today";
			this.butTodayStart.Click += new System.EventHandler(this.butTodayStart_Click);
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(118, 169);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.Size = new System.Drawing.Size(126, 20);
			this.textDateStop.TabIndex = 84;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(118, 143);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(126, 20);
			this.textDateStart.TabIndex = 84;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 388);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 26);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(327, 388);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(408, 388);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(333, 322);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(150, 17);
			this.label7.TabIndex = 91;
			this.label7.Text = "Only for CCD";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelFunctionalStatus
			// 
			this.labelFunctionalStatus.Location = new System.Drawing.Point(17, 346);
			this.labelFunctionalStatus.Name = "labelFunctionalStatus";
			this.labelFunctionalStatus.Size = new System.Drawing.Size(97, 23);
			this.labelFunctionalStatus.TabIndex = 93;
			this.labelFunctionalStatus.Text = "Functional Status";
			this.labelFunctionalStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboEhrFunctionalStatus
			// 
			this.comboEhrFunctionalStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboEhrFunctionalStatus.FormattingEnabled = true;
			this.comboEhrFunctionalStatus.Location = new System.Drawing.Point(118, 348);
			this.comboEhrFunctionalStatus.Name = "comboEhrFunctionalStatus";
			this.comboEhrFunctionalStatus.Size = new System.Drawing.Size(210, 21);
			this.comboEhrFunctionalStatus.TabIndex = 92;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(333, 349);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(150, 17);
			this.label8.TabIndex = 94;
			this.label8.Text = "Only for CCD";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(14, 65);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(100, 19);
			this.label9.TabIndex = 95;
			this.label9.Text = "ICD10";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textIcd10
			// 
			this.textIcd10.Location = new System.Drawing.Point(118, 64);
			this.textIcd10.Name = "textIcd10";
			this.textIcd10.ReadOnly = true;
			this.textIcd10.Size = new System.Drawing.Size(321, 20);
			this.textIcd10.TabIndex = 96;
			// 
			// FormDiseaseEdit
			// 
			this.ClientSize = new System.Drawing.Size(495, 426);
			this.Controls.Add(this.textIcd10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.labelFunctionalStatus);
			this.Controls.Add(this.comboEhrFunctionalStatus);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.labelSnomedProblemType);
			this.Controls.Add(this.comboSnomedProblemType);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textSnomed);
			this.Controls.Add(this.butTodayStop);
			this.Controls.Add(this.butTodayStart);
			this.Controls.Add(this.textDateStop);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.comboStatus);
			this.Controls.Add(this.textIcd9);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textProblem);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDiseaseEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Problem";
			this.Load += new System.EventHandler(this.FormDiseaseEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private TextBox textNote;
		private Label label1;
		private Label label2;
		private OpenDental.UI.Button butDelete;
		private TextBox textProblem;
		private TextBox textIcd9;
		private Label label3;
		private Label labelStatus;
		private ComboBox comboStatus;
		private Label label4;
		private Label label5;
		private ValidDate textDateStart;
		private ValidDate textDateStop;
		private UI.Button butTodayStart;
		private UI.Button butTodayStop;
		private TextBox textSnomed;
		private Label label6;
		private ComboBox comboSnomedProblemType;
		private Label labelSnomedProblemType;
		private Label label7;
		private Label labelFunctionalStatus;
		private ComboBox comboEhrFunctionalStatus;
		private Label label8;
		private Label label9;
		private TextBox textIcd10;
	}
}
