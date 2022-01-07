using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFeesForIns {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeesForIns));
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboFeeSchedWithout = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboFeeSchedWith = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboFeeSchedNew = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textCarrierNot = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.butSelectAll = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butChange = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(235, 53);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(180, 20);
			this.textCarrier.TabIndex = 0;
			this.textCarrier.TextChanged += new System.EventHandler(this.textCarrier_TextChanged);
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(136, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 17);
			this.label2.TabIndex = 19;
			this.label2.Text = "Carrier Like";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(416, 78);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 17);
			this.label1.TabIndex = 20;
			this.label1.Text = "Without Fee Schedule";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFeeSchedWithout
			// 
			this.comboFeeSchedWithout.FormattingEnabled = true;
			this.comboFeeSchedWithout.Location = new System.Drawing.Point(550, 74);
			this.comboFeeSchedWithout.MaxDropDownItems = 40;
			this.comboFeeSchedWithout.Name = "comboFeeSchedWithout";
			this.comboFeeSchedWithout.Size = new System.Drawing.Size(228, 21);
			this.comboFeeSchedWithout.TabIndex = 1;
			this.comboFeeSchedWithout.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSchedWithout_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label3.Location = new System.Drawing.Point(13, 4);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(432, 15);
			this.label3.TabIndex = 22;
			this.label3.Text = "You are searching for Insurance Plans that might have the wrong fee schedule atta" +
    "ched.";
			// 
			// comboFeeSchedWith
			// 
			this.comboFeeSchedWith.FormattingEnabled = true;
			this.comboFeeSchedWith.Location = new System.Drawing.Point(550, 52);
			this.comboFeeSchedWith.MaxDropDownItems = 40;
			this.comboFeeSchedWith.Name = "comboFeeSchedWith";
			this.comboFeeSchedWith.Size = new System.Drawing.Size(228, 21);
			this.comboFeeSchedWith.TabIndex = 23;
			this.comboFeeSchedWith.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSchedWith_SelectionChangeCommitted);
			// 
			// label4
			// 
			this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label4.Location = new System.Drawing.Point(416, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 17);
			this.label4.TabIndex = 24;
			this.label4.Text = "With Fee Schedule";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFeeSchedNew
			// 
			this.comboFeeSchedNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboFeeSchedNew.FormattingEnabled = true;
			this.comboFeeSchedNew.Location = new System.Drawing.Point(316, 633);
			this.comboFeeSchedNew.MaxDropDownItems = 40;
			this.comboFeeSchedNew.Name = "comboFeeSchedNew";
			this.comboFeeSchedNew.Size = new System.Drawing.Size(228, 21);
			this.comboFeeSchedNew.TabIndex = 25;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label5.Location = new System.Drawing.Point(161, 637);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(149, 17);
			this.label5.TabIndex = 26;
			this.label5.Text = "New Fee Schedule";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCarrierNot
			// 
			this.textCarrierNot.Location = new System.Drawing.Point(235, 74);
			this.textCarrierNot.Name = "textCarrierNot";
			this.textCarrierNot.Size = new System.Drawing.Size(180, 20);
			this.textCarrierNot.TabIndex = 27;
			this.textCarrierNot.TextChanged += new System.EventHandler(this.textCarrierNot_TextChanged);
			// 
			// label6
			// 
			this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label6.Location = new System.Drawing.Point(136, 77);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(93, 17);
			this.label6.TabIndex = 28;
			this.label6.Text = "Carrier Not Like";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(13, 25);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(120, 69);
			this.listType.TabIndex = 29;
			this.listType.Click += new System.EventHandler(this.listType_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSelectAll.Location = new System.Drawing.Point(12, 630);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(75, 24);
			this.butSelectAll.TabIndex = 30;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(13, 100);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(799, 524);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Ins Plans that might need to be changed";
			this.gridMain.TranslationName = "TablePlans";
			// 
			// butChange
			// 
			this.butChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butChange.Location = new System.Drawing.Point(560, 631);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(75, 24);
			this.butChange.TabIndex = 2;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(737, 632);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(93, 630);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 31;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// FormFeesForIns
			// 
			this.ClientSize = new System.Drawing.Size(824, 668);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.textCarrierNot);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.comboFeeSchedNew);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboFeeSchedWith);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboFeeSchedWithout);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butChange);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormFeesForIns";
			this.ShowInTaskbar = false;
			this.Text = "Check Insurance Plan Fees";
			this.Load += new System.EventHandler(this.FormFeesForIns_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butChange;
		private OpenDental.UI.GridOD gridMain;
		private TextBox textCarrier;
		private Label label2;
		private Label label1;
		private ComboBox comboFeeSchedWithout;
		private Label label3;
		private ComboBox comboFeeSchedWith;
		private Label label4;
		private ComboBox comboFeeSchedNew;
		private Label label5;
		private TextBox textCarrierNot;
		private Label label6;
		private OpenDental.UI.ListBoxOD listType;
		private OpenDental.UI.Button butSelectAll;
		private UI.Button butPrint;
	}
}
