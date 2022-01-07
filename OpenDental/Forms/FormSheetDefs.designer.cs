using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormSheetDefs {
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
				components?.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetDefs));
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboLabel = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butDefault = new OpenDental.UI.Button();
			this.butTools = new OpenDental.UI.Button();
			this.butCopy2 = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.grid1 = new OpenDental.UI.GridOD();
			this.grid2 = new OpenDental.UI.GridOD();
			this.butNew = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.listFilter = new OpenDental.UI.ListBoxOD();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboLabel);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(895, 51);
			this.groupBox2.TabIndex = 23;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Label assigned to patient button";
			// 
			// comboLabel
			// 
			this.comboLabel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLabel.FormattingEnabled = true;
			this.comboLabel.Location = new System.Drawing.Point(6, 19);
			this.comboLabel.MaxDropDownItems = 20;
			this.comboLabel.Name = "comboLabel";
			this.comboLabel.Size = new System.Drawing.Size(185, 21);
			this.comboLabel.TabIndex = 1;
			this.comboLabel.DropDown += new System.EventHandler(this.comboLabel_DropDown);
			this.comboLabel.SelectionChangeCommitted += new System.EventHandler(this.comboLabel_SelectionChangeCommitted);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(197, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(505, 28);
			this.label2.TabIndex = 18;
			this.label2.Text = "Most other sheet types are assigned simply by creating custom sheets of the same " +
    "type.\r\nReferral slips are set in the referral edit window of each referral.";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 70);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 15);
			this.label3.TabIndex = 21;
			this.label3.Text = "Sheet type filter";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDefault
			// 
			this.butDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDefault.Location = new System.Drawing.Point(15, 637);
			this.butDefault.Name = "butDefault";
			this.butDefault.Size = new System.Drawing.Size(75, 24);
			this.butDefault.TabIndex = 19;
			this.butDefault.Text = "Defaults";
			this.butDefault.Click += new System.EventHandler(this.butDefault_Click);
			// 
			// butTools
			// 
			this.butTools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTools.Location = new System.Drawing.Point(167, 637);
			this.butTools.Name = "butTools";
			this.butTools.Size = new System.Drawing.Size(75, 24);
			this.butTools.TabIndex = 5;
			this.butTools.Text = "Tools";
			this.butTools.Click += new System.EventHandler(this.butTools_Click);
			// 
			// butCopy2
			// 
			this.butCopy2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCopy2.Icon = OpenDental.UI.EnumIcons.Add;
			this.butCopy2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy2.Location = new System.Drawing.Point(737, 637);
			this.butCopy2.Name = "butCopy2";
			this.butCopy2.Size = new System.Drawing.Size(89, 24);
			this.butCopy2.TabIndex = 7;
			this.butCopy2.Text = "Duplicate";
			this.butCopy2.Click += new System.EventHandler(this.butCopy2_Click);
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCopy.Image = global::OpenDental.Properties.Resources.Right;
			this.butCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy.Location = new System.Drawing.Point(484, 637);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 24);
			this.butCopy.TabIndex = 4;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// grid1
			// 
			this.grid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.grid1.Location = new System.Drawing.Point(136, 69);
			this.grid1.Name = "grid1";
			this.grid1.Size = new System.Drawing.Size(380, 557);
			this.grid1.TabIndex = 2;
			this.grid1.Title = "Internal";
			this.grid1.TranslationName = "TableInternal";
			this.grid1.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid1_CellDoubleClick);
			this.grid1.Click += new System.EventHandler(this.grid1_Click);
			// 
			// grid2
			// 
			this.grid2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grid2.HasMultilineHeaders = true;
			this.grid2.Location = new System.Drawing.Point(528, 69);
			this.grid2.Name = "grid2";
			this.grid2.Size = new System.Drawing.Size(380, 557);
			this.grid2.TabIndex = 3;
			this.grid2.Title = "Custom";
			this.grid2.TranslationName = "TableCustom";
			this.grid2.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.grid2_CellDoubleClick);
			this.grid2.Click += new System.EventHandler(this.grid2_Click);
			// 
			// butNew
			// 
			this.butNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNew.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNew.Location = new System.Drawing.Point(652, 637);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(80, 24);
			this.butNew.TabIndex = 6;
			this.butNew.Text = "New";
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(832, 637);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// listFilter
			// 
			this.listFilter.Location = new System.Drawing.Point(4, 88);
			this.listFilter.Name = "listFilter";
			this.listFilter.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listFilter.Size = new System.Drawing.Size(124, 355);
			this.listFilter.TabIndex = 24;
			this.listFilter.SelectedIndexChanged += new System.EventHandler(this.listFilter_SelectedIndexChanged);
			// 
			// FormSheetDefs
			// 
			this.ClientSize = new System.Drawing.Size(919, 671);
			this.Controls.Add(this.listFilter);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butDefault);
			this.Controls.Add(this.butTools);
			this.Controls.Add(this.butCopy2);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.grid1);
			this.Controls.Add(this.grid2);
			this.Controls.Add(this.butNew);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetDefs";
			this.Text = "Sheet Defs";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSheetDefs_FormClosing);
			this.Load += new System.EventHandler(this.FormSheetDefs_Load);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butNew;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GridOD grid2;
		private UI.GridOD grid1;
		private OpenDental.UI.Button butCopy;
		private ComboBox comboLabel;
		private Label label2;
		private OpenDental.UI.Button butCopy2;
		private UI.Button butTools;
		private UI.Button butDefault;
		private Label label3;
		private GroupBox groupBox2;
		private UI.ListBoxOD listFilter;
	}
}
