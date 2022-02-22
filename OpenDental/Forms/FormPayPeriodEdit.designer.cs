using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPayPeriodEdit {
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
			OpenDental.UI.Button butDelete;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPeriodEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textDateStart = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.textDateStop = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.textDatePaycheck = new OpenDental.ValidDate();
			this.label3 = new System.Windows.Forms.Label();
			butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butDelete
			// 
			butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			butDelete.Location = new System.Drawing.Point(15, 137);
			butDelete.Name = "butDelete";
			butDelete.Size = new System.Drawing.Size(75, 26);
			butDelete.TabIndex = 16;
			butDelete.Text = "&Delete";
			butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(314, 137);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(314, 105);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 8;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(111, 24);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 10;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 20);
			this.label1.TabIndex = 11;
			this.label1.Text = "Start Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(111, 50);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.Size = new System.Drawing.Size(100, 20);
			this.textDateStop.TabIndex = 12;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 20);
			this.label2.TabIndex = 13;
			this.label2.Text = "End Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDatePaycheck
			// 
			this.textDatePaycheck.Location = new System.Drawing.Point(111, 76);
			this.textDatePaycheck.Name = "textDatePaycheck";
			this.textDatePaycheck.Size = new System.Drawing.Size(100, 20);
			this.textDatePaycheck.TabIndex = 14;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 76);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 20);
			this.label3.TabIndex = 15;
			this.label3.Text = "Paycheck Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormPayPeriodEdit
			// 
			this.ClientSize = new System.Drawing.Size(415, 181);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(butDelete);
			this.Controls.Add(this.textDatePaycheck);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateStop);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayPeriodEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Pay Period";
			this.Load += new System.EventHandler(this.FormPayPeriodEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private ValidDate textDateStart;
		private Label label1;
		private ValidDate textDateStop;
		private Label label2;
		private ValidDate textDatePaycheck;
		private Label label3;
	}
}
