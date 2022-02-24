using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDiseaseDefs {
		private System.ComponentModel.IContainer components=null;

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiseaseDefs));
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textSnoMed = new OpenDental.ODtextBox();
			this.textDescript = new OpenDental.ODtextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textICD10 = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textICD9 = new OpenDental.ODtextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butAlphabetize = new OpenDental.UI.Button();
			this.labelAlphabetize = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkShowHidden);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textSnoMed);
			this.groupBox1.Controls.Add(this.textDescript);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.textICD10);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.textICD9);
			this.groupBox1.Location = new System.Drawing.Point(18, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(537, 128);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search";
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Checked = true;
			this.checkShowHidden.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowHidden.Location = new System.Drawing.Point(277, 106);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(252, 18);
			this.checkShowHidden.TabIndex = 28;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.CheckedChanged += new System.EventHandler(this.checkShowHidden_CheckedChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(7, 78);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 20);
			this.label4.TabIndex = 27;
			this.label4.Text = "Description";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSnoMed
			// 
			this.textSnoMed.AcceptsTab = true;
			this.textSnoMed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSnoMed.BackColor = System.Drawing.SystemColors.Window;
			this.textSnoMed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textSnoMed.DetectLinksEnabled = false;
			this.textSnoMed.DetectUrls = false;
			this.textSnoMed.Location = new System.Drawing.Point(88, 59);
			this.textSnoMed.Multiline = false;
			this.textSnoMed.Name = "textSnoMed";
			this.textSnoMed.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textSnoMed.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textSnoMed.Size = new System.Drawing.Size(234, 20);
			this.textSnoMed.TabIndex = 26;
			this.textSnoMed.Text = "";
			// 
			// textDescript
			// 
			this.textDescript.AcceptsTab = true;
			this.textDescript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescript.BackColor = System.Drawing.SystemColors.Window;
			this.textDescript.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textDescript.DetectLinksEnabled = false;
			this.textDescript.DetectUrls = false;
			this.textDescript.Location = new System.Drawing.Point(88, 80);
			this.textDescript.Multiline = false;
			this.textDescript.Name = "textDescript";
			this.textDescript.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textDescript.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescript.Size = new System.Drawing.Size(441, 20);
			this.textDescript.TabIndex = 25;
			this.textDescript.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 59);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 20);
			this.label3.TabIndex = 24;
			this.label3.Text = "SNOMED";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 39);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 20);
			this.label2.TabIndex = 22;
			this.label2.Text = "ICD10";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textICD10
			// 
			this.textICD10.AcceptsTab = true;
			this.textICD10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textICD10.BackColor = System.Drawing.SystemColors.Window;
			this.textICD10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textICD10.DetectLinksEnabled = false;
			this.textICD10.DetectUrls = false;
			this.textICD10.Location = new System.Drawing.Point(88, 39);
			this.textICD10.Multiline = false;
			this.textICD10.Name = "textICD10";
			this.textICD10.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textICD10.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textICD10.Size = new System.Drawing.Size(234, 20);
			this.textICD10.TabIndex = 21;
			this.textICD10.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 20);
			this.label1.TabIndex = 20;
			this.label1.Text = "ICD9";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textICD9
			// 
			this.textICD9.AcceptsTab = true;
			this.textICD9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textICD9.BackColor = System.Drawing.SystemColors.Window;
			this.textICD9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textICD9.DetectLinksEnabled = false;
			this.textICD9.DetectUrls = false;
			this.textICD9.Location = new System.Drawing.Point(88, 19);
			this.textICD9.Multiline = false;
			this.textICD9.Name = "textICD9";
			this.textICD9.QuickPasteType = OpenDentBusiness.QuickPasteType.MedicationEdit;
			this.textICD9.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textICD9.Size = new System.Drawing.Size(234, 20);
			this.textICD9.TabIndex = 19;
			this.textICD9.Text = "";
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(18, 137);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(537, 548);
			this.gridMain.TabIndex = 16;
			this.gridMain.Title = null;
			this.gridMain.TranslationName = "TableProblems";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(561, 137);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(79, 26);
			this.butAdd.TabIndex = 21;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(592, 629);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(79, 26);
			this.butOK.TabIndex = 15;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(561, 309);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(79, 26);
			this.butDown.TabIndex = 14;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(561, 277);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(79, 26);
			this.butUp.TabIndex = 13;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(592, 661);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(79, 26);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAlphabetize
			// 
			this.butAlphabetize.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butAlphabetize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlphabetize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAlphabetize.Location = new System.Drawing.Point(561, 245);
			this.butAlphabetize.Name = "butAlphabetize";
			this.butAlphabetize.Size = new System.Drawing.Size(79, 26);
			this.butAlphabetize.TabIndex = 22;
			this.butAlphabetize.Text = "Alphabetize";
			this.butAlphabetize.Click += new System.EventHandler(this.butAlphabetize_Click);
			// 
			// labelAlphabetize
			// 
			this.labelAlphabetize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAlphabetize.Location = new System.Drawing.Point(561, 209);
			this.labelAlphabetize.Name = "labelAlphabetize";
			this.labelAlphabetize.Size = new System.Drawing.Size(118, 33);
			this.labelAlphabetize.TabIndex = 29;
			this.labelAlphabetize.Text = "Orders the problem list alphabetically";
			this.labelAlphabetize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormDiseaseDefs
			// 
			this.ClientSize = new System.Drawing.Size(680, 697);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelAlphabetize);
			this.Controls.Add(this.butAlphabetize);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDiseaseDefs";
			this.ShowInTaskbar = false;
			this.Text = "Problems";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDiseaseDefs_FormClosing);
			this.Load += new System.EventHandler(this.FormDiseaseDefs_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butDown;
		private System.Windows.Forms.ToolTip toolTip1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.GridOD gridMain;
		private GroupBox groupBox1;
		private ODtextBox textSnoMed;
		private ODtextBox textDescript;
		private Label label3;
		private Label label2;
		private ODtextBox textICD10;
		private Label label1;
		private ODtextBox textICD9;
		private Label label4;
		private UI.Button butUp;
		private UI.Button butAdd;
		private CheckBox checkShowHidden;
		private UI.Button butAlphabetize;
		private Label labelAlphabetize;
	}
}
