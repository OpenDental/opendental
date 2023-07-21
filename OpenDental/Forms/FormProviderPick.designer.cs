using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProviderPick {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProviderPick));
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.labelProvNum = new System.Windows.Forms.Label();
			this.textProvNum = new System.Windows.Forms.TextBox();
			this.groupDentalSchools = new OpenDental.UI.GroupBox();
			this.labelClass = new System.Windows.Forms.Label();
			this.comboClass = new OpenDental.UI.ComboBox();
			this.textLName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butSelectNone = new OpenDental.UI.Button();
			this.checkShowAll = new OpenDental.UI.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textFilter = new OpenDental.ODtextBox();
			this.groupDentalSchools.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(461, 627);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 6;
			this.butClose.Text = "&Cancel";
			this.butClose.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(16, 30);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(325, 624);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Providers";
			this.gridMain.TranslationName = "TableProviders";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(461, 595);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelProvNum
			// 
			this.labelProvNum.Location = new System.Drawing.Point(6, 19);
			this.labelProvNum.Name = "labelProvNum";
			this.labelProvNum.Size = new System.Drawing.Size(68, 18);
			this.labelProvNum.TabIndex = 27;
			this.labelProvNum.Text = "ProvNum";
			this.labelProvNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvNum
			// 
			this.textProvNum.Location = new System.Drawing.Point(76, 19);
			this.textProvNum.MaxLength = 15;
			this.textProvNum.Name = "textProvNum";
			this.textProvNum.Size = new System.Drawing.Size(118, 20);
			this.textProvNum.TabIndex = 0;
			// 
			// groupDentalSchools
			// 
			this.groupDentalSchools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupDentalSchools.Controls.Add(this.labelClass);
			this.groupDentalSchools.Controls.Add(this.comboClass);
			this.groupDentalSchools.Controls.Add(this.textLName);
			this.groupDentalSchools.Controls.Add(this.label2);
			this.groupDentalSchools.Controls.Add(this.textFName);
			this.groupDentalSchools.Controls.Add(this.label1);
			this.groupDentalSchools.Controls.Add(this.textProvNum);
			this.groupDentalSchools.Controls.Add(this.labelProvNum);
			this.groupDentalSchools.Location = new System.Drawing.Point(350, 32);
			this.groupDentalSchools.Name = "groupDentalSchools";
			this.groupDentalSchools.Size = new System.Drawing.Size(200, 110);
			this.groupDentalSchools.TabIndex = 3;
			this.groupDentalSchools.Text = "Dental School Filters";
			// 
			// labelClass
			// 
			this.labelClass.Location = new System.Drawing.Point(6, 82);
			this.labelClass.Name = "labelClass";
			this.labelClass.Size = new System.Drawing.Size(68, 18);
			this.labelClass.TabIndex = 33;
			this.labelClass.Text = "Class";
			this.labelClass.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClass
			// 
			this.comboClass.Location = new System.Drawing.Point(76, 82);
			this.comboClass.Name = "comboClass";
			this.comboClass.Size = new System.Drawing.Size(118, 21);
			this.comboClass.TabIndex = 3;
			this.comboClass.SelectionChangeCommitted += new System.EventHandler(this.comboClass_SelectionChangeCommitted);
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(76, 40);
			this.textLName.MaxLength = 15;
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(118, 20);
			this.textLName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 18);
			this.label2.TabIndex = 31;
			this.label2.Text = "LName";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(76, 61);
			this.textFName.MaxLength = 15;
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(118, 20);
			this.textFName.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 18);
			this.label1.TabIndex = 29;
			this.label1.Text = "FName";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSelectNone
			// 
			this.butSelectNone.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.butSelectNone.Location = new System.Drawing.Point(461, 545);
			this.butSelectNone.Name = "butSelectNone";
			this.butSelectNone.Size = new System.Drawing.Size(75, 26);
			this.butSelectNone.TabIndex = 4;
			this.butSelectNone.Text = "None";
			this.butSelectNone.UseVisualStyleBackColor = true;
			this.butSelectNone.Click += new System.EventHandler(this.butSelectNone_Click);
			// 
			// checkShowAll
			// 
			this.checkShowAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowAll.Location = new System.Drawing.Point(253, 7);
			this.checkShowAll.Name = "checkShowAll";
			this.checkShowAll.Size = new System.Drawing.Size(88, 21);
			this.checkShowAll.TabIndex = 1;
			this.checkShowAll.Text = "Show All";
			this.checkShowAll.CheckedChanged += new System.EventHandler(this.checkShowAll_CheckedChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(71, 18);
			this.label3.TabIndex = 17;
			this.label3.Text = "Search";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFilter
			// 
			this.textFilter.AcceptsTab = true;
			this.textFilter.AllowsCarriageReturns = false;
			this.textFilter.BackColor = System.Drawing.SystemColors.Window;
			this.textFilter.DetectLinksEnabled = false;
			this.textFilter.DetectUrls = false;
			this.textFilter.Location = new System.Drawing.Point(88, 5);
			this.textFilter.Name = "textFilter";
			this.textFilter.QuickPasteType = OpenDentBusiness.QuickPasteType.Office;
			this.textFilter.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textFilter.Size = new System.Drawing.Size(158, 20);
			this.textFilter.TabIndex = 0;
			this.textFilter.Text = "";
			this.textFilter.TextChanged += new System.EventHandler(this.textFilter_TextChanged);
			this.textFilter.DoubleClick += new System.EventHandler(this.textFilter_DoubleClick);
			this.textFilter.Enter += new System.EventHandler(this.textFilter_Enter);
			// 
			// FormProviderPick
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(558, 670);
			this.Controls.Add(this.textFilter);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkShowAll);
			this.Controls.Add(this.butSelectNone);
			this.Controls.Add(this.groupDentalSchools);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProviderPick";
			this.ShowInTaskbar = false;
			this.Text = "Providers";
			this.Load += new System.EventHandler(this.FormProviderSelect_Load);
			this.groupDentalSchools.ResumeLayout(false);
			this.groupDentalSchools.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butOK;
		private Label labelProvNum;
		private TextBox textProvNum;
		private OpenDental.UI.GroupBox groupDentalSchools;
		private TextBox textLName;
		private Label label2;
		private OpenDental.UI.CheckBox checkShowAll;
		private TextBox textFName;
		private Label label1;
		private Label labelClass;
		private OpenDental.UI.ComboBox comboClass;
		private UI.Button butSelectNone;
		private Label label3;
		private ODtextBox textFilter;
	}
}
