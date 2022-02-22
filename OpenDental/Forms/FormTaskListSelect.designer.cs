using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTaskListSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskListSelect));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelMulti = new System.Windows.Forms.Label();
			this.checkMulti = new System.Windows.Forms.CheckBox();
			this.textFilter = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkIncludeAll = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(350, 481);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(350, 451);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelMulti
			// 
			this.labelMulti.Location = new System.Drawing.Point(11, 3);
			this.labelMulti.Name = "labelMulti";
			this.labelMulti.Size = new System.Drawing.Size(270, 57);
			this.labelMulti.TabIndex = 0;
			this.labelMulti.Text = "Pick task list to send to.";
			this.labelMulti.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkMulti
			// 
			this.checkMulti.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMulti.Location = new System.Drawing.Point(12, 112);
			this.checkMulti.Name = "checkMulti";
			this.checkMulti.Size = new System.Drawing.Size(182, 18);
			this.checkMulti.TabIndex = 1;
			this.checkMulti.Text = "Send copies to multiple";
			this.checkMulti.UseVisualStyleBackColor = true;
			this.checkMulti.Visible = false;
			this.checkMulti.CheckedChanged += new System.EventHandler(this.checkMulti_CheckedChanged);
			// 
			// textFilter
			// 
			this.textFilter.Location = new System.Drawing.Point(12, 90);
			this.textFilter.Name = "textFilter";
			this.textFilter.Size = new System.Drawing.Size(181, 20);
			this.textFilter.TabIndex = 0;
			this.textFilter.TextChanged += new System.EventHandler(this.textFilter_TextChanged);
			this.textFilter.DoubleClick += new System.EventHandler(this.textFilter_DoubleClick);
			this.textFilter.Enter += new System.EventHandler(this.textFilter_Enter);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 70);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(183, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIncludeAll
			// 
			this.checkIncludeAll.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeAll.Location = new System.Drawing.Point(12, 129);
			this.checkIncludeAll.Name = "checkIncludeAll";
			this.checkIncludeAll.Size = new System.Drawing.Size(182, 18);
			this.checkIncludeAll.TabIndex = 2;
			this.checkIncludeAll.Text = "Show all task lists";
			this.checkIncludeAll.UseVisualStyleBackColor = true;
			this.checkIncludeAll.CheckedChanged += new System.EventHandler(this.checkIncludeSubTasks_CheckedChanged);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(11, 150);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(325, 355);
			this.gridMain.TabIndex = 3;
			this.gridMain.TitleVisible = false;
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormTaskListSelect
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(437, 517);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.checkIncludeAll);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textFilter);
			this.Controls.Add(this.labelMulti);
			this.Controls.Add(this.checkMulti);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTaskListSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Task List";
			this.Load += new System.EventHandler(this.FormTaskListSelect_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label labelMulti;
		private CheckBox checkMulti;
		private CheckBox checkIncludeAll;
		private TextBox textFilter;
		private UI.GridOD gridMain;
		private Label label1;
	}
}
