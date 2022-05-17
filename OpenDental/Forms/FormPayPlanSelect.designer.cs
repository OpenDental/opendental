using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPayPlanSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPlanSelect));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butNone = new OpenDental.UI.Button();
			this.labelExpl = new System.Windows.Forms.Label();
			this.checkShowActiveOnly = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(522, 208);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(441, 208);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 0;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 22);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(590, 180);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Payment Plans";
			this.gridMain.TranslationName = "TablePaymentPlans";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridMain_KeyDown);
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNone.Location = new System.Drawing.Point(12, 208);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 26);
			this.butNone.TabIndex = 4;
			this.butNone.Text = "None";
			this.butNone.Visible = false;
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// labelExpl
			// 
			this.labelExpl.Location = new System.Drawing.Point(12, 4);
			this.labelExpl.Name = "labelExpl";
			this.labelExpl.Size = new System.Drawing.Size(410, 16);
			this.labelExpl.TabIndex = 5;
			this.labelExpl.Text = "Select a Payment Plan to attach to, or click \'None\'.";
			this.labelExpl.Visible = false;
			// 
			// checkShowActiveOnly
			// 
			this.checkShowActiveOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowActiveOnly.Location = new System.Drawing.Point(463, 2);
			this.checkShowActiveOnly.Name = "checkShowActiveOnly";
			this.checkShowActiveOnly.Size = new System.Drawing.Size(134, 18);
			this.checkShowActiveOnly.TabIndex = 6;
			this.checkShowActiveOnly.Text = "Show Active Only";
			this.checkShowActiveOnly.UseVisualStyleBackColor = true;
			this.checkShowActiveOnly.MouseClick += new System.Windows.Forms.MouseEventHandler(this.checkShowActiveOnly_MouseClick);
			// 
			// FormPayPlanSelect
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(609, 246);
			this.Controls.Add(this.checkShowActiveOnly);
			this.Controls.Add(this.labelExpl);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayPlanSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Payment Plan";
			this.Load += new System.EventHandler(this.FormPayPlanSelect_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private UI.Button butNone;
		private UI.GridOD gridMain;
		private Label labelExpl;
		private CheckBox checkShowActiveOnly;
	}
}
