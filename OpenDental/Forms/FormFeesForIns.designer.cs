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
			this.comboFeeSchedWithout = new OpenDental.UI.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboFeeSchedWith = new OpenDental.UI.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboFeeSchedNew = new OpenDental.UI.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textCarrierNot = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.listType = new OpenDental.UI.ListBox();
			this.butSelectAll = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butChangeFeeSchedule = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.comboInsPlanTypeWith = new OpenDental.UI.ComboBox();
			this.comboInsPlanType = new OpenDental.UI.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butChangeInsPlanType = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(236, 49);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(180, 20);
			this.textCarrier.TabIndex = 0;
			this.textCarrier.TextChanged += new System.EventHandler(this.textCarrier_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(137, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 17);
			this.label2.TabIndex = 19;
			this.label2.Text = "Carrier Like";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(450, 73);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 17);
			this.label1.TabIndex = 20;
			this.label1.Text = "Without Fee Schedule";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFeeSchedWithout
			// 
			this.comboFeeSchedWithout.Location = new System.Drawing.Point(584, 69);
			this.comboFeeSchedWithout.Name = "comboFeeSchedWithout";
			this.comboFeeSchedWithout.Size = new System.Drawing.Size(228, 21);
			this.comboFeeSchedWithout.TabIndex = 1;
			this.comboFeeSchedWithout.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSchedWithout_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(140, 12);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(276, 27);
			this.label3.TabIndex = 22;
			this.label3.Text = "You are searching for Insurance Plans that might have the wrong fee schedule atta" +
    "ched.";
			// 
			// comboFeeSchedWith
			// 
			this.comboFeeSchedWith.Location = new System.Drawing.Point(584, 47);
			this.comboFeeSchedWith.Name = "comboFeeSchedWith";
			this.comboFeeSchedWith.Size = new System.Drawing.Size(228, 21);
			this.comboFeeSchedWith.TabIndex = 23;
			this.comboFeeSchedWith.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSchedWith_SelectionChangeCommitted);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(450, 51);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(128, 17);
			this.label4.TabIndex = 24;
			this.label4.Text = "With Fee Schedule";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFeeSchedNew
			// 
			this.comboFeeSchedNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboFeeSchedNew.Location = new System.Drawing.Point(341, 664);
			this.comboFeeSchedNew.Name = "comboFeeSchedNew";
			this.comboFeeSchedNew.Size = new System.Drawing.Size(228, 21);
			this.comboFeeSchedNew.TabIndex = 25;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(186, 668);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(149, 17);
			this.label5.TabIndex = 26;
			this.label5.Text = "New Fee Schedule";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCarrierNot
			// 
			this.textCarrierNot.Location = new System.Drawing.Point(236, 70);
			this.textCarrierNot.Name = "textCarrierNot";
			this.textCarrierNot.Size = new System.Drawing.Size(180, 20);
			this.textCarrierNot.TabIndex = 27;
			this.textCarrierNot.TextChanged += new System.EventHandler(this.textCarrierNot_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(137, 73);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(93, 17);
			this.label6.TabIndex = 28;
			this.label6.Text = "Carrier Not Like";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(14, 12);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(120, 78);
			this.listType.TabIndex = 29;
			this.listType.Click += new System.EventHandler(this.listType_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSelectAll.Location = new System.Drawing.Point(12, 661);
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
			this.gridMain.Location = new System.Drawing.Point(13, 96);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(799, 533);
			this.gridMain.TabIndex = 2;
			this.gridMain.Title = "Ins Plans that might need to be changed";
			this.gridMain.TranslationName = "TablePlans";
			// 
			// butChangeFeeSchedule
			// 
			this.butChangeFeeSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butChangeFeeSchedule.Location = new System.Drawing.Point(578, 662);
			this.butChangeFeeSchedule.Name = "butChangeFeeSchedule";
			this.butChangeFeeSchedule.Size = new System.Drawing.Size(74, 24);
			this.butChangeFeeSchedule.TabIndex = 2;
			this.butChangeFeeSchedule.Text = "Change";
			this.butChangeFeeSchedule.Click += new System.EventHandler(this.butChangeFeeSchedule_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(93, 661);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 31;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// comboInsPlanTypeWith
			// 
			this.comboInsPlanTypeWith.Location = new System.Drawing.Point(584, 25);
			this.comboInsPlanTypeWith.Name = "comboInsPlanTypeWith";
			this.comboInsPlanTypeWith.Size = new System.Drawing.Size(228, 21);
			this.comboInsPlanTypeWith.TabIndex = 32;
			this.comboInsPlanTypeWith.SelectionChangeCommitted += new System.EventHandler(this.comboInsPlanTypeWith_SelectionChangeCommitted);
			// 
			// comboInsPlanType
			// 
			this.comboInsPlanType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboInsPlanType.Location = new System.Drawing.Point(341, 637);
			this.comboInsPlanType.Name = "comboInsPlanType";
			this.comboInsPlanType.Size = new System.Drawing.Size(228, 21);
			this.comboInsPlanType.TabIndex = 34;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(186, 641);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(149, 17);
			this.label7.TabIndex = 35;
			this.label7.Text = "New Insurance Plan Type";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butChangeInsPlanType
			// 
			this.butChangeInsPlanType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butChangeInsPlanType.Location = new System.Drawing.Point(578, 635);
			this.butChangeInsPlanType.Name = "butChangeInsPlanType";
			this.butChangeInsPlanType.Size = new System.Drawing.Size(74, 24);
			this.butChangeInsPlanType.TabIndex = 33;
			this.butChangeInsPlanType.Text = "Change";
			this.butChangeInsPlanType.Click += new System.EventHandler(this.butChangeInsPlanType_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(412, 29);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(166, 17);
			this.label8.TabIndex = 36;
			this.label8.Text = "With Insurance Plan Type";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormFeesForIns
			// 
			this.ClientSize = new System.Drawing.Size(824, 696);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.comboInsPlanType);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butChangeInsPlanType);
			this.Controls.Add(this.comboInsPlanTypeWith);
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
			this.Controls.Add(this.butChangeFeeSchedule);
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
		private OpenDental.UI.Button butChangeFeeSchedule;
		private OpenDental.UI.GridOD gridMain;
		private TextBox textCarrier;
		private Label label2;
		private Label label1;
		private OpenDental.UI.ComboBox comboFeeSchedWithout;
		private Label label3;
		private OpenDental.UI.ComboBox comboFeeSchedWith;
		private Label label4;
		private OpenDental.UI.ComboBox comboFeeSchedNew;
		private Label label5;
		private TextBox textCarrierNot;
		private Label label6;
		private OpenDental.UI.ListBox listType;
		private OpenDental.UI.Button butSelectAll;
		private UI.Button butPrint;
		private UI.ComboBox comboInsPlanTypeWith;
		private UI.ComboBox comboInsPlanType;
		private Label label7;
		private UI.Button butChangeInsPlanType;
		private Label label8;
	}
}
