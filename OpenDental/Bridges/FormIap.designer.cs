using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormIap {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormIap));
			this.listPlans = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.textPlanSearch = new System.Windows.Forms.TextBox();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textEmp = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textPlanNum = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listPlans
			// 
			this.listPlans.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
			this.listPlans.Location = new System.Drawing.Point(11, 61);
			this.listPlans.Name = "listPlans";
			this.listPlans.Size = new System.Drawing.Size(518, 524);
			this.listPlans.TabIndex = 1;
			this.listPlans.SelectedIndexChanged += new System.EventHandler(this.listPlans_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 18);
			this.label1.TabIndex = 3;
			this.label1.Text = "Search By IAP Number";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPlanSearch
			// 
			this.textPlanSearch.Location = new System.Drawing.Point(11, 32);
			this.textPlanSearch.Name = "textPlanSearch";
			this.textPlanSearch.Size = new System.Drawing.Size(127, 20);
			this.textPlanSearch.TabIndex = 0;
			this.textPlanSearch.TextChanged += new System.EventHandler(this.textPlanSearch_TextChanged);
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(563, 157);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.ReadOnly = true;
			this.textCarrier.Size = new System.Drawing.Size(292, 20);
			this.textCarrier.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(562, 138);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 5;
			this.label2.Text = "Carrier";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textEmp
			// 
			this.textEmp.Location = new System.Drawing.Point(563, 115);
			this.textEmp.Name = "textEmp";
			this.textEmp.ReadOnly = true;
			this.textEmp.Size = new System.Drawing.Size(292, 20);
			this.textEmp.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(562, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 18);
			this.label3.TabIndex = 7;
			this.label3.Text = "Employer";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(562, 58);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "IAP Plan Number";
			// 
			// textPlanNum
			// 
			this.textPlanNum.Location = new System.Drawing.Point(565, 73);
			this.textPlanNum.Name = "textPlanNum";
			this.textPlanNum.ReadOnly = true;
			this.textPlanNum.Size = new System.Drawing.Size(290, 20);
			this.textPlanNum.TabIndex = 9;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(752, 513);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(752, 554);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormIap
			// 
			this.ClientSize = new System.Drawing.Size(879, 605);
			this.Controls.Add(this.textPlanNum);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textEmp);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPlanSearch);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listPlans);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormIap";
			this.ShowInTaskbar = false;
			this.Text = "Insurance Answers Plus";
			this.Load += new System.EventHandler(this.FormIap_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ListBoxOD listPlans;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPlanSearch;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textCarrier;
		private System.Windows.Forms.TextBox textEmp;
		private System.Windows.Forms.Label label3;
		private Label label4;
		private TextBox textPlanNum;
	}
}
