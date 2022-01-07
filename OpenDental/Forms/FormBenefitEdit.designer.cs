using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormBenefitEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBenefitEdit));
			this.labelCode = new System.Windows.Forms.Label();
			this.labelAmount = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listCategory = new OpenDental.UI.ListBoxOD();
			this.checkPat = new System.Windows.Forms.CheckBox();
			this.textProcCode = new System.Windows.Forms.TextBox();
			this.listBenefitType = new OpenDental.UI.ListBoxOD();
			this.labelType = new System.Windows.Forms.Label();
			this.labelPercent = new System.Windows.Forms.Label();
			this.listTimePeriod = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.listQuantityQualifier = new OpenDental.UI.ListBoxOD();
			this.label8 = new System.Windows.Forms.Label();
			this.groupQuantity = new System.Windows.Forms.GroupBox();
			this.textQuantity = new OpenDental.ValidNum();
			this.listCoverageLevel = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.textAmount = new OpenDental.ValidDouble();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textPercent = new OpenDental.ValidNum();
			this.groupQuantity.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelCode
			// 
			this.labelCode.Location = new System.Drawing.Point(21, 215);
			this.labelCode.Name = "labelCode";
			this.labelCode.Size = new System.Drawing.Size(104, 16);
			this.labelCode.TabIndex = 0;
			this.labelCode.Text = "or Proc Code";
			this.labelCode.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelAmount
			// 
			this.labelAmount.Location = new System.Drawing.Point(267, 62);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(100, 16);
			this.labelAmount.TabIndex = 4;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(23, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 10;
			this.label2.Text = "Category";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listCategory
			// 
			this.listCategory.Location = new System.Drawing.Point(126, 36);
			this.listCategory.Name = "listCategory";
			this.listCategory.Size = new System.Drawing.Size(100, 173);
			this.listCategory.TabIndex = 5;
			// 
			// checkPat
			// 
			this.checkPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPat.Location = new System.Drawing.Point(126, 2);
			this.checkPat.Name = "checkPat";
			this.checkPat.Size = new System.Drawing.Size(493, 20);
			this.checkPat.TabIndex = 4;
			this.checkPat.Text = "Patient Override (Rare. Usually if percentages are different for family members)";
			this.checkPat.UseVisualStyleBackColor = true;
			// 
			// textProcCode
			// 
			this.textProcCode.Location = new System.Drawing.Point(126, 211);
			this.textProcCode.Name = "textProcCode";
			this.textProcCode.Size = new System.Drawing.Size(100, 20);
			this.textProcCode.TabIndex = 6;
			// 
			// listType
			// 
			this.listBenefitType.Location = new System.Drawing.Point(126, 251);
			this.listBenefitType.Name = "listType";
			this.listBenefitType.Size = new System.Drawing.Size(100, 82);
			this.listBenefitType.TabIndex = 7;
			this.listBenefitType.Click += new System.EventHandler(this.listType_Click);
			// 
			// labelType
			// 
			this.labelType.Location = new System.Drawing.Point(24, 251);
			this.labelType.Name = "labelType";
			this.labelType.Size = new System.Drawing.Size(100, 16);
			this.labelType.TabIndex = 26;
			this.labelType.Text = "Type";
			this.labelType.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelPercent
			// 
			this.labelPercent.Location = new System.Drawing.Point(263, 40);
			this.labelPercent.Name = "labelPercent";
			this.labelPercent.Size = new System.Drawing.Size(104, 16);
			this.labelPercent.TabIndex = 27;
			this.labelPercent.Text = "Percent";
			this.labelPercent.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listTimePeriod
			// 
			this.listTimePeriod.Location = new System.Drawing.Point(368, 82);
			this.listTimePeriod.Name = "listTimePeriod";
			this.listTimePeriod.Size = new System.Drawing.Size(100, 69);
			this.listTimePeriod.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(266, 82);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 30;
			this.label4.Text = "Time Period";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listQuantityQualifier
			// 
			this.listQuantityQualifier.Location = new System.Drawing.Point(67, 41);
			this.listQuantityQualifier.Name = "listQuantityQualifier";
			this.listQuantityQualifier.Size = new System.Drawing.Size(100, 82);
			this.listQuantityQualifier.TabIndex = 1;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(1, 43);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(65, 38);
			this.label8.TabIndex = 34;
			this.label8.Text = "Qualifier";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupQuantity
			// 
			this.groupQuantity.Controls.Add(this.textQuantity);
			this.groupQuantity.Controls.Add(this.listQuantityQualifier);
			this.groupQuantity.Controls.Add(this.label8);
			this.groupQuantity.Location = new System.Drawing.Point(301, 151);
			this.groupQuantity.Name = "groupQuantity";
			this.groupQuantity.Size = new System.Drawing.Size(180, 135);
			this.groupQuantity.TabIndex = 3;
			this.groupQuantity.TabStop = false;
			this.groupQuantity.Text = "Quantity";
			// 
			// textQuantity
			// 
			this.textQuantity.Location = new System.Drawing.Point(67, 17);
			this.textQuantity.MaxVal = 100;
			this.textQuantity.MinVal = 0;
			this.textQuantity.Name = "textQuantity";
			this.textQuantity.Size = new System.Drawing.Size(68, 20);
			this.textQuantity.TabIndex = 0;
			// 
			// listCoverageLevel
			// 
			this.listCoverageLevel.Location = new System.Drawing.Point(368, 290);
			this.listCoverageLevel.Name = "listCoverageLevel";
			this.listCoverageLevel.Size = new System.Drawing.Size(100, 43);
			this.listCoverageLevel.TabIndex = 31;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(255, 290);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(111, 16);
			this.label1.TabIndex = 32;
			this.label1.Text = "Coverage Level";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(368, 59);
			this.textAmount.MaxVal = 100000000D;
			this.textAmount.MinVal = -100000000D;
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(68, 20);
			this.textAmount.TabIndex = 1;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 378);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 17;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(555, 378);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(555, 340);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 8;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textPercent
			// 
			this.textPercent.Location = new System.Drawing.Point(368, 36);
			this.textPercent.MaxVal = 100;
			this.textPercent.MinVal = 0;
			this.textPercent.Name = "textPercent";
			this.textPercent.Size = new System.Drawing.Size(68, 20);
			this.textPercent.TabIndex = 33;
			this.textPercent.ShowZero = false;
			// 
			// FormBenefitEdit
			// 
			this.ClientSize = new System.Drawing.Size(642, 416);
			this.Controls.Add(this.textPercent);
			this.Controls.Add(this.listTimePeriod);
			this.Controls.Add(this.listCoverageLevel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupQuantity);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelPercent);
			this.Controls.Add(this.listBenefitType);
			this.Controls.Add(this.labelType);
			this.Controls.Add(this.textProcCode);
			this.Controls.Add(this.checkPat);
			this.Controls.Add(this.listCategory);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelAmount);
			this.Controls.Add(this.labelCode);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormBenefitEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Benefit";
			this.Load += new System.EventHandler(this.FormBenefitEdit_Load);
			this.groupQuantity.ResumeLayout(false);
			this.groupQuantity.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelCode;
		private System.Windows.Forms.Label labelAmount;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private OpenDental.ValidDouble textAmount;
		private OpenDental.UI.ListBoxOD listCategory;
		private TextBox textProcCode;
		private OpenDental.UI.ListBoxOD listBenefitType;
		private Label labelType;
		private Label labelPercent;
		private OpenDental.UI.ListBoxOD listTimePeriod;
		private Label label4;
		private ValidNum textQuantity;
		private OpenDental.UI.ListBoxOD listQuantityQualifier;
		private Label label8;
		private GroupBox groupQuantity;
		private CheckBox checkPat;
		private OpenDental.UI.ListBoxOD listCoverageLevel;
		private Label label1;
		private ValidNum textPercent;
	}
}
