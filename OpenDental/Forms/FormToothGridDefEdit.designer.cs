using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormToothGridDefEdit {
		/// <summary>Required designer variable.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormToothGridDefEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textInternalName = new System.Windows.Forms.TextBox();
			this.textNameShowing = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textWidth = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textCodeNum = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.listCellType = new OpenDental.UI.ListBoxOD();
			this.listProcStatus = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-1, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 17);
			this.label1.TabIndex = 8;
			this.label1.Text = "Internal Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInternalName
			// 
			this.textInternalName.Location = new System.Drawing.Point(141, 15);
			this.textInternalName.Name = "textInternalName";
			this.textInternalName.ReadOnly = true;
			this.textInternalName.Size = new System.Drawing.Size(348, 20);
			this.textInternalName.TabIndex = 7;
			// 
			// textNameShowing
			// 
			this.textNameShowing.Location = new System.Drawing.Point(141, 41);
			this.textNameShowing.Name = "textNameShowing";
			this.textNameShowing.Size = new System.Drawing.Size(348, 20);
			this.textNameShowing.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-1, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140, 17);
			this.label2.TabIndex = 9;
			this.label2.Text = "Name Showing";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(-1, 130);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(140, 17);
			this.label3.TabIndex = 11;
			this.label3.Text = "Column Width";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(141, 129);
			this.textWidth.MaxVal = 2000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(71, 20);
			this.textWidth.TabIndex = 2;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(414, 310);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(414, 351);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(-1, 156);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(140, 17);
			this.label4.TabIndex = 12;
			this.label4.Text = "Code Number";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCodeNum
			// 
			this.textCodeNum.Location = new System.Drawing.Point(141, 155);
			this.textCodeNum.Name = "textCodeNum";
			this.textCodeNum.Size = new System.Drawing.Size(71, 20);
			this.textCodeNum.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(-1, 182);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(140, 17);
			this.label5.TabIndex = 13;
			this.label5.Text = "Procedure Status";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(-1, 68);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(140, 17);
			this.label6.TabIndex = 10;
			this.label6.Text = "Cell Type";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listCellType
			// 
			this.listCellType.Location = new System.Drawing.Point(141, 67);
			this.listCellType.Name = "listCellType";
			this.listCellType.Size = new System.Drawing.Size(120, 56);
			this.listCellType.TabIndex = 14;
			// 
			// listProcStatus
			// 
			this.listProcStatus.Location = new System.Drawing.Point(141, 181);
			this.listProcStatus.Name = "listProcStatus";
			this.listProcStatus.Size = new System.Drawing.Size(120, 56);
			this.listProcStatus.TabIndex = 15;
			// 
			// FormToothGridDefEdit
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(541, 402);
			this.Controls.Add(this.listProcStatus);
			this.Controls.Add(this.listCellType);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textCodeNum);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textNameShowing);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textInternalName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormToothGridDefEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Toothgrid Def Field";
			this.Load += new System.EventHandler(this.FormDisplayFieldEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private TextBox textInternalName;
		private TextBox textNameShowing;
		private Label label2;
		private Label label3;
		private ValidNum textWidth;
		private Label label4;
		private TextBox textCodeNum;
		private Label label5;
		private Label label6;
		private OpenDental.UI.ListBoxOD listCellType;
		private OpenDental.UI.ListBoxOD listProcStatus;
	}
}
