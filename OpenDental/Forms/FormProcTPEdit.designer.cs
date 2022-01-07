using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcTPEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcTPEdit));
			this.textToothNumTP = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelToothNum = new System.Windows.Forms.Label();
			this.labelSurface = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.comboPriority = new System.Windows.Forms.ComboBox();
			this.textSurf = new System.Windows.Forms.TextBox();
			this.textCode = new System.Windows.Forms.TextBox();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textPrognosis = new System.Windows.Forms.TextBox();
			this.textDx = new System.Windows.Forms.TextBox();
			this.labelDx = new System.Windows.Forms.Label();
			this.labelWarning = new System.Windows.Forms.Label();
			this.textDiscount = new OpenDental.ValidDouble();
			this.textPatAmt = new OpenDental.ValidDouble();
			this.textSecInsAmt = new OpenDental.ValidDouble();
			this.textPriInsAmt = new OpenDental.ValidDouble();
			this.textFeeAmt = new OpenDental.ValidDouble();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textProcAbbr = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textFeeAllowed = new OpenDental.ValidDouble();
			this.labelFeeAllowed = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textToothNumTP
			// 
			this.textToothNumTP.Location = new System.Drawing.Point(175, 63);
			this.textToothNumTP.Name = "textToothNumTP";
			this.textToothNumTP.Size = new System.Drawing.Size(50, 20);
			this.textToothNumTP.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(82, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 16);
			this.label1.TabIndex = 9;
			this.label1.Text = "Priority";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelToothNum
			// 
			this.labelToothNum.Location = new System.Drawing.Point(34, 65);
			this.labelToothNum.Name = "labelToothNum";
			this.labelToothNum.Size = new System.Drawing.Size(137, 16);
			this.labelToothNum.TabIndex = 10;
			this.labelToothNum.Text = "Tooth Num";
			this.labelToothNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSurface
			// 
			this.labelSurface.Location = new System.Drawing.Point(82, 89);
			this.labelSurface.Name = "labelSurface";
			this.labelSurface.Size = new System.Drawing.Size(89, 16);
			this.labelSurface.TabIndex = 11;
			this.labelSurface.Text = "Surf";
			this.labelSurface.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(82, 113);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(89, 16);
			this.label6.TabIndex = 12;
			this.label6.Text = "Code";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(82, 207);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(89, 16);
			this.label7.TabIndex = 13;
			this.label7.Text = "Description";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(82, 259);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(89, 16);
			this.label8.TabIndex = 14;
			this.label8.Text = "Fee";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(82, 283);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(89, 16);
			this.label9.TabIndex = 15;
			this.label9.Text = "Pri Ins";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(82, 307);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(89, 16);
			this.label10.TabIndex = 16;
			this.label10.Text = "Sec Ins";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(43, 355);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(128, 16);
			this.label11.TabIndex = 17;
			this.label11.Text = "Patient Portion";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPriority
			// 
			this.comboPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPriority.Location = new System.Drawing.Point(175, 38);
			this.comboPriority.Name = "comboPriority";
			this.comboPriority.Size = new System.Drawing.Size(94, 21);
			this.comboPriority.TabIndex = 15;
			// 
			// textSurf
			// 
			this.textSurf.Location = new System.Drawing.Point(175, 87);
			this.textSurf.Name = "textSurf";
			this.textSurf.Size = new System.Drawing.Size(50, 20);
			this.textSurf.TabIndex = 1;
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(175, 111);
			this.textCode.Name = "textCode";
			this.textCode.Size = new System.Drawing.Size(81, 20);
			this.textCode.TabIndex = 2;
			// 
			// textDescript
			// 
			this.textDescript.AcceptsReturn = true;
			this.textDescript.Location = new System.Drawing.Point(175, 205);
			this.textDescript.Multiline = true;
			this.textDescript.Name = "textDescript";
			this.textDescript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textDescript.Size = new System.Drawing.Size(377, 48);
			this.textDescript.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(43, 331);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 16);
			this.label2.TabIndex = 67;
			this.label2.Text = "Discount";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(43, 158);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(128, 16);
			this.label3.TabIndex = 69;
			this.label3.Text = "Prognosis";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPrognosis
			// 
			this.textPrognosis.Location = new System.Drawing.Point(175, 157);
			this.textPrognosis.Name = "textPrognosis";
			this.textPrognosis.Size = new System.Drawing.Size(81, 20);
			this.textPrognosis.TabIndex = 4;
			// 
			// textDx
			// 
			this.textDx.Location = new System.Drawing.Point(175, 181);
			this.textDx.Name = "textDx";
			this.textDx.Size = new System.Drawing.Size(81, 20);
			this.textDx.TabIndex = 5;
			// 
			// labelDx
			// 
			this.labelDx.Location = new System.Drawing.Point(43, 182);
			this.labelDx.Name = "labelDx";
			this.labelDx.Size = new System.Drawing.Size(128, 16);
			this.labelDx.TabIndex = 71;
			this.labelDx.Text = "Dx";
			this.labelDx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWarning.Location = new System.Drawing.Point(116, 435);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(358, 16);
			this.labelWarning.TabIndex = 73;
			this.labelWarning.Text = "Not allowed to edit signed Treatment Plan Procedures.";
			this.labelWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWarning.Visible = false;
			// 
			// textDiscount
			// 
			this.textDiscount.Location = new System.Drawing.Point(175, 329);
			this.textDiscount.MaxVal = 100000000D;
			this.textDiscount.MinVal = -100000000D;
			this.textDiscount.Name = "textDiscount";
			this.textDiscount.Size = new System.Drawing.Size(81, 20);
			this.textDiscount.TabIndex = 10;
			// 
			// textPatAmt
			// 
			this.textPatAmt.Location = new System.Drawing.Point(175, 353);
			this.textPatAmt.MaxVal = 100000000D;
			this.textPatAmt.MinVal = -100000000D;
			this.textPatAmt.Name = "textPatAmt";
			this.textPatAmt.Size = new System.Drawing.Size(81, 20);
			this.textPatAmt.TabIndex = 11;
			// 
			// textSecInsAmt
			// 
			this.textSecInsAmt.Location = new System.Drawing.Point(175, 305);
			this.textSecInsAmt.MaxVal = 100000000D;
			this.textSecInsAmt.MinVal = -100000000D;
			this.textSecInsAmt.Name = "textSecInsAmt";
			this.textSecInsAmt.Size = new System.Drawing.Size(81, 20);
			this.textSecInsAmt.TabIndex = 9;
			// 
			// textPriInsAmt
			// 
			this.textPriInsAmt.Location = new System.Drawing.Point(175, 281);
			this.textPriInsAmt.MaxVal = 100000000D;
			this.textPriInsAmt.MinVal = -100000000D;
			this.textPriInsAmt.Name = "textPriInsAmt";
			this.textPriInsAmt.Size = new System.Drawing.Size(81, 20);
			this.textPriInsAmt.TabIndex = 8;
			// 
			// textFeeAmt
			// 
			this.textFeeAmt.Location = new System.Drawing.Point(175, 257);
			this.textFeeAmt.MaxVal = 100000000D;
			this.textFeeAmt.MinVal = -100000000D;
			this.textFeeAmt.Name = "textFeeAmt";
			this.textFeeAmt.Size = new System.Drawing.Size(81, 20);
			this.textFeeAmt.TabIndex = 7;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(24, 430);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(86, 26);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(479, 392);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(479, 430);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textProcAbbr
			// 
			this.textProcAbbr.Location = new System.Drawing.Point(175, 134);
			this.textProcAbbr.Name = "textProcAbbr";
			this.textProcAbbr.Size = new System.Drawing.Size(81, 20);
			this.textProcAbbr.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(68, 136);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(103, 16);
			this.label4.TabIndex = 74;
			this.label4.Text = "Abbreviation";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFeeAllowed
			// 
			this.textFeeAllowed.Location = new System.Drawing.Point(175, 378);
			this.textFeeAllowed.MaxVal = 100000000D;
			this.textFeeAllowed.MinVal = -100000000D;
			this.textFeeAllowed.Name = "textFeeAllowed";
			this.textFeeAllowed.Size = new System.Drawing.Size(81, 20);
			this.textFeeAllowed.TabIndex = 75;
			// 
			// labelFeeAllowed
			// 
			this.labelFeeAllowed.Location = new System.Drawing.Point(43, 380);
			this.labelFeeAllowed.Name = "labelFeeAllowed";
			this.labelFeeAllowed.Size = new System.Drawing.Size(128, 16);
			this.labelFeeAllowed.TabIndex = 76;
			this.labelFeeAllowed.Text = "Allowed";
			this.labelFeeAllowed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormProcTPEdit
			// 
			this.ClientSize = new System.Drawing.Size(606, 484);
			this.Controls.Add(this.textFeeAllowed);
			this.Controls.Add(this.labelFeeAllowed);
			this.Controls.Add(this.textProcAbbr);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.textDx);
			this.Controls.Add(this.labelDx);
			this.Controls.Add(this.textPrognosis);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDiscount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPatAmt);
			this.Controls.Add(this.textSecInsAmt);
			this.Controls.Add(this.textPriInsAmt);
			this.Controls.Add(this.textFeeAmt);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.textSurf);
			this.Controls.Add(this.comboPriority);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.labelSurface);
			this.Controls.Add(this.labelToothNum);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textToothNumTP);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcTPEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Treatment Plan Procedure";
			this.Load += new System.EventHandler(this.FormProcTPEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelToothNum;
		private System.Windows.Forms.Label labelSurface;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textToothNumTP;
		private System.Windows.Forms.ComboBox comboPriority;
		private System.Windows.Forms.TextBox textSurf;
		private System.Windows.Forms.TextBox textCode;
		private System.Windows.Forms.TextBox textDescript;
		private OpenDental.ValidDouble textFeeAmt;
		private OpenDental.ValidDouble textPriInsAmt;
		private OpenDental.ValidDouble textSecInsAmt;
		private ValidDouble textDiscount;
		private Label label2;
		private OpenDental.ValidDouble textPatAmt;
		private Label label3;
		private TextBox textPrognosis;
		private TextBox textDx;
		private Label labelDx;
		private Label labelWarning;
		private TextBox textProcAbbr;
		private Label label4;
		private ValidDouble textFeeAllowed;
		private Label labelFeeAllowed;
	}
}
