using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormLabTurnaroundEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLabTurnaroundEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textDaysActual = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.textDaysPublished = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(326, 143);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(235, 143);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(164, 17);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(241, 20);
			this.textDescription.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(160, 17);
			this.label1.TabIndex = 4;
			this.label1.Text = "Service Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(33, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(130, 17);
			this.label2.TabIndex = 6;
			this.label2.Text = "Days Published";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(33, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(130, 17);
			this.label3.TabIndex = 8;
			this.label3.Text = "Actual Days";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDaysActual
			// 
			this.textDaysActual.Location = new System.Drawing.Point(164, 69);
			this.textDaysActual.MaxVal = 255;
			this.textDaysActual.MinVal = 0;
			this.textDaysActual.Name = "textDaysActual";
			this.textDaysActual.Size = new System.Drawing.Size(43, 20);
			this.textDaysActual.TabIndex = 3;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(213, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(174, 44);
			this.label4.TabIndex = 9;
			this.label4.Text = "Might be same as days published, or might include travel time.";
			// 
			// textDaysPublished
			// 
			this.textDaysPublished.Location = new System.Drawing.Point(164, 43);
			this.textDaysPublished.MaxVal = 255;
			this.textDaysPublished.MinVal = 0;
			this.textDaysPublished.Name = "textDaysPublished";
			this.textDaysPublished.Size = new System.Drawing.Size(43, 20);
			this.textDaysPublished.TabIndex = 2;
			this.textDaysPublished.ShowZero = false;
			// 
			// FormLabTurnaroundEdit
			// 
			this.ClientSize = new System.Drawing.Size(453, 194);
			this.Controls.Add(this.textDaysPublished);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDaysActual);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLabTurnaroundEdit";
			this.ShowInTaskbar = false;
			this.Text = "Lab Turnaround";
			this.Load += new System.EventHandler(this.FormLabTurnaroundEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private TextBox textDescription;
		private Label label1;
		private Label label2;
		private Label label3;
		private ValidNum textDaysActual;
		private Label label4;
		private ValidNum textDaysPublished;
	}
}
