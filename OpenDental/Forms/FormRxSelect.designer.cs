using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRxSelect {
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRxSelect));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelInstructions = new System.Windows.Forms.Label();
			this.butBlank = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.checkControlledOnly = new System.Windows.Forms.CheckBox();
			this.textDisp = new System.Windows.Forms.TextBox();
			this.textDrug = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(848, 636);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(756, 636);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelInstructions
			// 
			this.labelInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelInstructions.Location = new System.Drawing.Point(9, 643);
			this.labelInstructions.Name = "labelInstructions";
			this.labelInstructions.Size = new System.Drawing.Size(470, 16);
			this.labelInstructions.TabIndex = 15;
			this.labelInstructions.Text = "Please select a Prescription from the list or click Blank to start with a blank p" +
    "rescription.";
			// 
			// butBlank
			// 
			this.butBlank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butBlank.Location = new System.Drawing.Point(436, 636);
			this.butBlank.Name = "butBlank";
			this.butBlank.Size = new System.Drawing.Size(75, 26);
			this.butBlank.TabIndex = 0;
			this.butBlank.Text = "&Blank";
			this.butBlank.Click += new System.EventHandler(this.butBlank_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(738, 611);
			this.gridMain.TabIndex = 16;
			this.gridMain.Title = "Prescriptions";
			this.gridMain.TranslationName = "TableRxSetup";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butRefresh);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.checkControlledOnly);
			this.groupBox1.Controls.Add(this.textDisp);
			this.groupBox1.Controls.Add(this.textDrug);
			this.groupBox1.Location = new System.Drawing.Point(756, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(174, 173);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search";
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(93, 140);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 26);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "Search";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Disp";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Drug";
			// 
			// checkControlledOnly
			// 
			this.checkControlledOnly.Location = new System.Drawing.Point(11, 110);
			this.checkControlledOnly.Name = "checkControlledOnly";
			this.checkControlledOnly.Size = new System.Drawing.Size(157, 24);
			this.checkControlledOnly.TabIndex = 2;
			this.checkControlledOnly.Text = "Controlled Only";
			this.checkControlledOnly.UseVisualStyleBackColor = true;
			// 
			// textDisp
			// 
			this.textDisp.Location = new System.Drawing.Point(8, 84);
			this.textDisp.Name = "textDisp";
			this.textDisp.Size = new System.Drawing.Size(160, 20);
			this.textDisp.TabIndex = 1;
			this.textDisp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDisp_KeyDown);
			// 
			// textDrug
			// 
			this.textDrug.Location = new System.Drawing.Point(7, 42);
			this.textDrug.Name = "textDrug";
			this.textDrug.Size = new System.Drawing.Size(160, 20);
			this.textDrug.TabIndex = 0;
			this.textDrug.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDrug_KeyDown);
			// 
			// FormRxSelect
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(942, 674);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butBlank);
			this.Controls.Add(this.labelInstructions);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRxSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Prescription";
			this.Load += new System.EventHandler(this.FormRxSelect_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelInstructions;
		private OpenDental.UI.Button butBlank;
		private OpenDental.UI.GridOD gridMain;
		private GroupBox groupBox1;
		private Label label2;
		private Label label1;
		private CheckBox checkControlledOnly;
		private TextBox textDisp;
		private TextBox textDrug;
		private UI.Button butRefresh;
	}
}
