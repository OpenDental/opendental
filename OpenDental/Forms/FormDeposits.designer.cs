using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDeposits {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDeposits));
			this.butClose = new OpenDental.UI.Button();
			this.grid = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(365, 574);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// grid
			// 
			this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grid.Location = new System.Drawing.Point(18, 34);
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(339, 567);
			this.grid.TabIndex = 1;
			this.grid.Title = "Deposit Slips";
			this.grid.TranslationName = "TableDepositSlips";
			this.grid.WrapText = false;
			this.grid.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(363, 293);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(77, 26);
			this.butAdd.TabIndex = 0;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(365, 542);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(72, 7);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(200, 21);
			this.comboClinics.TabIndex = 131;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.ComboClinics_SelectionChangeCommitted);
			// 
			// FormDeposits
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(454, 613);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDeposits";
			this.ShowInTaskbar = false;
			this.Text = "Deposit Slips";
			this.Load += new System.EventHandler(this.FormDeposits_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.GridOD grid;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ComboBoxClinicPicker comboClinics;
	}
}
