using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpRouting {
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
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpRouting));
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDate = new OpenDental.ValidDate();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butToday = new OpenDental.UI.Button();
			this.butDisplayed = new OpenDental.UI.Button();
			this.checkClinAll = new System.Windows.Forms.CheckBox();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.checkProvAll = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(27, 41);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(120, 186);
			this.listProv.TabIndex = 33;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 1);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(75, 16);
			this.label1.TabIndex = 32;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(150, 236);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(51, 18);
			this.label2.TabIndex = 37;
			this.label2.Text = "Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(207, 233);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(82, 20);
			this.textDate.TabIndex = 43;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(356, 244);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 44;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(356, 212);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 43;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butToday
			// 
			this.butToday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butToday.Location = new System.Drawing.Point(335, 18);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(96, 23);
			this.butToday.TabIndex = 46;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// butDisplayed
			// 
			this.butDisplayed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDisplayed.Location = new System.Drawing.Point(335, 47);
			this.butDisplayed.Name = "butDisplayed";
			this.butDisplayed.Size = new System.Drawing.Size(96, 23);
			this.butDisplayed.TabIndex = 45;
			this.butDisplayed.Text = "Displayed";
			this.butDisplayed.Click += new System.EventHandler(this.butDisplayed_Click);
			// 
			// checkClinAll
			// 
			this.checkClinAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkClinAll.Location = new System.Drawing.Point(156, 20);
			this.checkClinAll.Name = "checkClinAll";
			this.checkClinAll.Size = new System.Drawing.Size(118, 14);
			this.checkClinAll.TabIndex = 57;
			this.checkClinAll.Text = "All (Includes hidden)";
			this.checkClinAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClinAll.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listBoxClin
			// 
			this.listClin.Location = new System.Drawing.Point(156, 41);
			this.listClin.Name = "listBoxClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(133, 186);
			this.listClin.TabIndex = 56;
			this.listClin.Click += new System.EventHandler(this.listBoxClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(153, 1);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(87, 16);
			this.labelClin.TabIndex = 58;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkProvAll
			// 
			this.checkProvAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProvAll.Location = new System.Drawing.Point(27, 19);
			this.checkProvAll.Name = "checkProvAll";
			this.checkProvAll.Size = new System.Drawing.Size(50, 16);
			this.checkProvAll.TabIndex = 59;
			this.checkProvAll.Text = "All";
			this.checkProvAll.Click += new System.EventHandler(this.checkProvAll_Click);
			// 
			// FormRpRouting
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(471, 292);
			this.Controls.Add(this.checkProvAll);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkClinAll);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.butDisplayed);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpRouting";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Routing Slips";
			this.Load += new System.EventHandler(this.FormRpRouting_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.ListBoxOD listProv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.ValidDate textDate;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDisplayed;
		private OpenDental.UI.Button butToday;
		private CheckBox checkClinAll;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private CheckBox checkProvAll;
	}
}
