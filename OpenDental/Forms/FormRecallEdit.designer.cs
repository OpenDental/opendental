using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRecallEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRecallEdit));
			this.textDatePrevious = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateDueCalc = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textWeeks = new OpenDental.ValidNum();
			this.label7 = new System.Windows.Forms.Label();
			this.textDays = new OpenDental.ValidNum();
			this.label6 = new System.Windows.Forms.Label();
			this.textMonths = new OpenDental.ValidNum();
			this.label5 = new System.Windows.Forms.Label();
			this.textYears = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.checkIsDisabled = new System.Windows.Forms.CheckBox();
			this.label9 = new System.Windows.Forms.Label();
			this.comboType = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textBalance = new OpenDental.ValidDouble();
			this.textDisableDate = new OpenDental.ValidDate();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textScheduledDate = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textDateDue = new OpenDental.ValidDate();
			this.butCancel = new OpenDental.UI.Button();
			this.checkASAP = new System.Windows.Forms.CheckBox();
			this.textNote = new OpenDental.ODtextBox();
			this.labelPattern = new System.Windows.Forms.Label();
			this.textPattern = new System.Windows.Forms.TextBox();
			this.labelPatternDesc = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textDatePrevious
			// 
			this.textDatePrevious.Location = new System.Drawing.Point(184, 171);
			this.textDatePrevious.Name = "textDatePrevious";
			this.textDatePrevious.ReadOnly = true;
			this.textDatePrevious.Size = new System.Drawing.Size(85, 20);
			this.textDatePrevious.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 170);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(170, 19);
			this.label1.TabIndex = 2;
			this.label1.Text = "Previous Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 196);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(170, 19);
			this.label2.TabIndex = 4;
			this.label2.Text = "Calculated Due Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateDueCalc
			// 
			this.textDateDueCalc.Location = new System.Drawing.Point(184, 197);
			this.textDateDueCalc.Name = "textDateDueCalc";
			this.textDateDueCalc.ReadOnly = true;
			this.textDateDueCalc.Size = new System.Drawing.Size(85, 20);
			this.textDateDueCalc.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 223);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(170, 19);
			this.label3.TabIndex = 5;
			this.label3.Text = "Actual Due Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textWeeks);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.textDays);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.textMonths);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textYears);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(78, 50);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(170, 115);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Recall Interval";
			// 
			// textWeeks
			// 
			this.textWeeks.Location = new System.Drawing.Point(105, 63);
			this.textWeeks.MaxVal = 255;
			this.textWeeks.MinVal = 0;
			this.textWeeks.Name = "textWeeks";
			this.textWeeks.Size = new System.Drawing.Size(51, 20);
			this.textWeeks.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(11, 63);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(92, 19);
			this.label7.TabIndex = 11;
			this.label7.Text = "Weeks";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(105, 85);
			this.textDays.MaxVal = 255;
			this.textDays.MinVal = 0;
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(51, 20);
			this.textDays.TabIndex = 12;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(11, 85);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(92, 19);
			this.label6.TabIndex = 9;
			this.label6.Text = "Days";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMonths
			// 
			this.textMonths.Location = new System.Drawing.Point(105, 40);
			this.textMonths.MaxVal = 255;
			this.textMonths.MinVal = 0;
			this.textMonths.Name = "textMonths";
			this.textMonths.Size = new System.Drawing.Size(51, 20);
			this.textMonths.TabIndex = 8;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(11, 40);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(92, 19);
			this.label5.TabIndex = 7;
			this.label5.Text = "Months";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textYears
			// 
			this.textYears.Location = new System.Drawing.Point(105, 17);
			this.textYears.MaxVal = 127;
			this.textYears.MinVal = 0;
			this.textYears.Name = "textYears";
			this.textYears.Size = new System.Drawing.Size(51, 20);
			this.textYears.TabIndex = 6;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(11, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(92, 19);
			this.label4.TabIndex = 5;
			this.label4.Text = "Years";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(12, 275);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(170, 19);
			this.label8.TabIndex = 8;
			this.label8.Text = "Status";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.Location = new System.Drawing.Point(184, 275);
			this.comboStatus.MaxDropDownItems = 50;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(188, 21);
			this.comboStatus.TabIndex = 9;
			// 
			// checkIsDisabled
			// 
			this.checkIsDisabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDisabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsDisabled.Location = new System.Drawing.Point(24, 20);
			this.checkIsDisabled.Name = "checkIsDisabled";
			this.checkIsDisabled.Size = new System.Drawing.Size(132, 18);
			this.checkIsDisabled.TabIndex = 10;
			this.checkIsDisabled.Text = "Always Disabled";
			this.checkIsDisabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDisabled.Click += new System.EventHandler(this.checkIsDisabled_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(65, 351);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(117, 92);
			this.label9.TabIndex = 11;
			this.label9.Text = "Administrative Note (this note will get deleted every time recall gets reset)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboType
			// 
			this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboType.Location = new System.Drawing.Point(184, 18);
			this.comboType.MaxDropDownItems = 50;
			this.comboType.Name = "comboType";
			this.comboType.Size = new System.Drawing.Size(188, 21);
			this.comboType.TabIndex = 17;
			this.comboType.SelectionChangeCommitted += new System.EventHandler(this.comboType_SelectionChangeCommitted);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 18);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(170, 19);
			this.label10.TabIndex = 16;
			this.label10.Text = "Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textBalance);
			this.groupBox2.Controls.Add(this.textDisableDate);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.checkIsDisabled);
			this.groupBox2.Location = new System.Drawing.Point(423, 50);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(243, 115);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Disable Recall";
			// 
			// textBalance
			// 
			this.textBalance.BackColor = System.Drawing.SystemColors.Window;
			this.textBalance.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textBalance.Location = new System.Drawing.Point(141, 48);
			this.textBalance.MaxVal = 100000000D;
			this.textBalance.MinVal = -100000000D;
			this.textBalance.Name = "textBalance";
			this.textBalance.Size = new System.Drawing.Size(86, 20);
			this.textBalance.TabIndex = 19;
			// 
			// textDisableDate
			// 
			this.textDisableDate.Location = new System.Drawing.Point(141, 80);
			this.textDisableDate.Name = "textDisableDate";
			this.textDisableDate.Size = new System.Drawing.Size(86, 20);
			this.textDisableDate.TabIndex = 13;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(40, 80);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(101, 18);
			this.label12.TabIndex = 12;
			this.label12.Text = "Until Date";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(3, 39);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(138, 37);
			this.label11.TabIndex = 11;
			this.label11.Text = "Until family Account balance is below";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textScheduledDate
			// 
			this.textScheduledDate.Location = new System.Drawing.Point(184, 249);
			this.textScheduledDate.Name = "textScheduledDate";
			this.textScheduledDate.ReadOnly = true;
			this.textScheduledDate.Size = new System.Drawing.Size(85, 20);
			this.textScheduledDate.TabIndex = 19;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(11, 248);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(170, 19);
			this.label13.TabIndex = 20;
			this.label13.Text = "Scheduled Date";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(26, 440);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 24);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(640, 398);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textDateDue
			// 
			this.textDateDue.Location = new System.Drawing.Point(184, 223);
			this.textDateDue.Name = "textDateDue";
			this.textDateDue.Size = new System.Drawing.Size(85, 20);
			this.textDateDue.TabIndex = 6;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(640, 440);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkASAP
			// 
			this.checkASAP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkASAP.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkASAP.Location = new System.Drawing.Point(78, 329);
			this.checkASAP.Name = "checkASAP";
			this.checkASAP.Size = new System.Drawing.Size(121, 16);
			this.checkASAP.TabIndex = 185;
			this.checkASAP.Text = "Schedule ASAP";
			this.checkASAP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkASAP.UseVisualStyleBackColor = true;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(183, 351);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Recall;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(350, 112);
			this.textNote.TabIndex = 15;
			this.textNote.Text = "";
			// 
			// labelPattern
			// 
			this.labelPattern.Location = new System.Drawing.Point(51, 303);
			this.labelPattern.Name = "labelPattern";
			this.labelPattern.Size = new System.Drawing.Size(131, 19);
			this.labelPattern.TabIndex = 186;
			this.labelPattern.Text = "Time Pattern Override";
			this.labelPattern.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPattern
			// 
			this.textPattern.Location = new System.Drawing.Point(184, 303);
			this.textPattern.Name = "textPattern";
			this.textPattern.Size = new System.Drawing.Size(165, 20);
			this.textPattern.TabIndex = 187;
			// 
			// labelPatternDesc
			// 
			this.labelPatternDesc.Location = new System.Drawing.Point(355, 304);
			this.labelPatternDesc.Name = "labelPatternDesc";
			this.labelPatternDesc.Size = new System.Drawing.Size(100, 19);
			this.labelPatternDesc.TabIndex = 188;
			this.labelPatternDesc.Text = "(only /\'s and X\'s)";
			this.labelPatternDesc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormRecallEdit
			// 
			this.ClientSize = new System.Drawing.Size(736, 489);
			this.Controls.Add(this.labelPatternDesc);
			this.Controls.Add(this.textPattern);
			this.Controls.Add(this.labelPattern);
			this.Controls.Add(this.checkASAP);
			this.Controls.Add(this.textScheduledDate);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.comboType);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textDateDue);
			this.Controls.Add(this.textDateDueCalc);
			this.Controls.Add(this.textDatePrevious);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.comboStatus);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRecallEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Recall";
			this.Load += new System.EventHandler(this.FormRecallEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label4;
		private OpenDental.ValidNum textYears;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox comboStatus;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textDatePrevious;
		private OpenDental.ValidNum textMonths;
		private OpenDental.ValidNum textDays;
		private OpenDental.ValidNum textWeeks;
		private System.Windows.Forms.CheckBox checkIsDisabled;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textDateDueCalc;
		private OpenDental.ValidDate textDateDue;
		private OpenDental.ODtextBox textNote;
		private ComboBox comboType;
		private GroupBox groupBox2;
		private Label label11;
		private ValidDate textDisableDate;
		private Label label12;
		private ValidDouble textBalance;
		private TextBox textScheduledDate;
		private Label label13;
		private CheckBox checkASAP;
		private Label label10;
		private Label labelPattern;
		private TextBox textPattern;
		private Label labelPatternDesc;
	}
}
