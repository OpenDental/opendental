using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReportsMore {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportsMore));
			this.groupBusiness = new System.Windows.Forms.GroupBox();
			this.picturePracticeByNumbers = new OpenDental.UI.ODPictureBox();
			this.groupPatientReviews = new System.Windows.Forms.GroupBox();
			this.picturePodium = new OpenDental.UI.ODPictureBox();
			this.labelArizonaPrimaryCare = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.labelDaily = new System.Windows.Forms.Label();
			this.labelProdInc = new System.Windows.Forms.Label();
			this.labelMonthly = new System.Windows.Forms.Label();
			this.labelLists = new System.Windows.Forms.Label();
			this.labelPublicHealth = new System.Windows.Forms.Label();
			this.butPatExport = new OpenDental.UI.Button();
			this.butPatList = new OpenDental.UI.Button();
			this.listArizonaPrimaryCare = new OpenDental.UI.ListBoxOD();
			this.butLaserLabels = new OpenDental.UI.Button();
			this.listDaily = new OpenDental.UI.ListBoxOD();
			this.listProdInc = new OpenDental.UI.ListBoxOD();
			this.butPW = new OpenDental.UI.Button();
			this.butUserQuery = new OpenDental.UI.Button();
			this.listPublicHealth = new OpenDental.UI.ListBoxOD();
			this.listLists = new OpenDental.UI.ListBoxOD();
			this.listMonthly = new OpenDental.UI.ListBoxOD();
			this.butClose = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.groupBusiness.SuspendLayout();
			this.groupPatientReviews.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBusiness
			// 
			this.groupBusiness.Controls.Add(this.picturePracticeByNumbers);
			this.groupBusiness.Location = new System.Drawing.Point(532, 72);
			this.groupBusiness.Name = "groupBusiness";
			this.groupBusiness.Size = new System.Drawing.Size(120, 53);
			this.groupBusiness.TabIndex = 28;
			this.groupBusiness.TabStop = false;
			this.groupBusiness.Text = "Business Analytics";
			// 
			// picturePracticeByNumbers
			// 
			this.picturePracticeByNumbers.HasBorder = false;
			this.picturePracticeByNumbers.Image = global::OpenDental.Properties.Resources.PracticeByNumbers_100x24;
			this.picturePracticeByNumbers.Location = new System.Drawing.Point(8, 19);
			this.picturePracticeByNumbers.Name = "picturePracticeByNumbers";
			this.picturePracticeByNumbers.Size = new System.Drawing.Size(95, 24);
			this.picturePracticeByNumbers.TabIndex = 31;
			this.picturePracticeByNumbers.TextNullImage = "Practice By Numbers";
			this.picturePracticeByNumbers.Click += new System.EventHandler(this.picturePracticeByNumbers_Click);
			// 
			// groupPatientReviews
			// 
			this.groupPatientReviews.Controls.Add(this.picturePodium);
			this.groupPatientReviews.Location = new System.Drawing.Point(532, 159);
			this.groupPatientReviews.Name = "groupPatientReviews";
			this.groupPatientReviews.Size = new System.Drawing.Size(120, 54);
			this.groupPatientReviews.TabIndex = 26;
			this.groupPatientReviews.TabStop = false;
			this.groupPatientReviews.Text = "Patient Reviews";
			// 
			// picturePodium
			// 
			this.picturePodium.HasBorder = false;
			this.picturePodium.Image = global::OpenDental.Properties.Resources.Podium_Button_100x24;
			this.picturePodium.Location = new System.Drawing.Point(8, 19);
			this.picturePodium.Name = "picturePodium";
			this.picturePodium.Size = new System.Drawing.Size(95, 24);
			this.picturePodium.TabIndex = 28;
			this.picturePodium.TextNullImage = null;
			this.picturePodium.Click += new System.EventHandler(this.picturePodium_Click);
			// 
			// labelArizonaPrimaryCare
			// 
			this.labelArizonaPrimaryCare.Location = new System.Drawing.Point(281, 410);
			this.labelArizonaPrimaryCare.Name = "labelArizonaPrimaryCare";
			this.labelArizonaPrimaryCare.Size = new System.Drawing.Size(156, 13);
			this.labelArizonaPrimaryCare.TabIndex = 20;
			this.labelArizonaPrimaryCare.Text = "Arizona Primary Care";
			this.labelArizonaPrimaryCare.Visible = false;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(9, 574);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(568, 82);
			this.label6.TabIndex = 17;
			this.label6.Text = resources.GetString("label6.Text");
			// 
			// labelDaily
			// 
			this.labelDaily.Location = new System.Drawing.Point(9, 200);
			this.labelDaily.Name = "labelDaily";
			this.labelDaily.Size = new System.Drawing.Size(118, 18);
			this.labelDaily.TabIndex = 15;
			this.labelDaily.Text = "Daily";
			this.labelDaily.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelProdInc
			// 
			this.labelProdInc.Location = new System.Drawing.Point(9, 54);
			this.labelProdInc.Name = "labelProdInc";
			this.labelProdInc.Size = new System.Drawing.Size(207, 18);
			this.labelProdInc.TabIndex = 13;
			this.labelProdInc.Text = "Production and Income";
			this.labelProdInc.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelMonthly
			// 
			this.labelMonthly.Location = new System.Drawing.Point(9, 349);
			this.labelMonthly.Name = "labelMonthly";
			this.labelMonthly.Size = new System.Drawing.Size(118, 18);
			this.labelMonthly.TabIndex = 6;
			this.labelMonthly.Text = "Monthly";
			this.labelMonthly.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelLists
			// 
			this.labelLists.Location = new System.Drawing.Point(281, 54);
			this.labelLists.Name = "labelLists";
			this.labelLists.Size = new System.Drawing.Size(118, 18);
			this.labelLists.TabIndex = 4;
			this.labelLists.Text = "Lists";
			this.labelLists.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelPublicHealth
			// 
			this.labelPublicHealth.Location = new System.Drawing.Point(281, 305);
			this.labelPublicHealth.Name = "labelPublicHealth";
			this.labelPublicHealth.Size = new System.Drawing.Size(118, 18);
			this.labelPublicHealth.TabIndex = 2;
			this.labelPublicHealth.Text = "Public Health";
			this.labelPublicHealth.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butPatExport
			// 
			this.butPatExport.Location = new System.Drawing.Point(538, 262);
			this.butPatExport.Name = "butPatExport";
			this.butPatExport.Size = new System.Drawing.Size(101, 24);
			this.butPatExport.TabIndex = 24;
			this.butPatExport.Text = "EHR Pat Export";
			this.butPatExport.Visible = false;
			this.butPatExport.Click += new System.EventHandler(this.butPatExport_Click);
			// 
			// butPatList
			// 
			this.butPatList.Location = new System.Drawing.Point(538, 232);
			this.butPatList.Name = "butPatList";
			this.butPatList.Size = new System.Drawing.Size(101, 24);
			this.butPatList.TabIndex = 23;
			this.butPatList.Text = "EHR Patient List";
			this.butPatList.Visible = false;
			this.butPatList.Click += new System.EventHandler(this.butPatList_Click);
			// 
			// listArizonaPrimaryCare
			// 
			this.listArizonaPrimaryCare.Location = new System.Drawing.Point(284, 424);
			this.listArizonaPrimaryCare.Name = "listArizonaPrimaryCare";
			this.listArizonaPrimaryCare.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listArizonaPrimaryCare.Size = new System.Drawing.Size(242, 43);
			this.listArizonaPrimaryCare.TabIndex = 19;
			this.listArizonaPrimaryCare.Visible = false;
			this.listArizonaPrimaryCare.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listArizonaPrimaryCare_MouseDown);
			// 
			// butLaserLabels
			// 
			this.butLaserLabels.Location = new System.Drawing.Point(294, 26);
			this.butLaserLabels.Name = "butLaserLabels";
			this.butLaserLabels.Size = new System.Drawing.Size(81, 24);
			this.butLaserLabels.TabIndex = 18;
			this.butLaserLabels.Text = "Laser Labels";
			this.butLaserLabels.UseVisualStyleBackColor = true;
			this.butLaserLabels.Click += new System.EventHandler(this.butLaserLabels_Click);
			// 
			// listDaily
			// 
			this.listDaily.Location = new System.Drawing.Point(12, 221);
			this.listDaily.Name = "listDaily";
			this.listDaily.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listDaily.Size = new System.Drawing.Size(242, 121);
			this.listDaily.TabIndex = 16;
			this.listDaily.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listDaily_MouseDown);
			// 
			// listProdInc
			// 
			this.listProdInc.Location = new System.Drawing.Point(12, 75);
			this.listProdInc.Name = "listProdInc";
			this.listProdInc.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listProdInc.Size = new System.Drawing.Size(242, 121);
			this.listProdInc.TabIndex = 14;
			this.listProdInc.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listProdInc_MouseDown);
			// 
			// butPW
			// 
			this.butPW.Location = new System.Drawing.Point(135, 26);
			this.butPW.Name = "butPW";
			this.butPW.Size = new System.Drawing.Size(84, 24);
			this.butPW.TabIndex = 12;
			this.butPW.Text = "PW Reports";
			this.butPW.Click += new System.EventHandler(this.butPW_Click);
			// 
			// butUserQuery
			// 
			this.butUserQuery.Location = new System.Drawing.Point(12, 26);
			this.butUserQuery.Name = "butUserQuery";
			this.butUserQuery.Size = new System.Drawing.Size(84, 24);
			this.butUserQuery.TabIndex = 11;
			this.butUserQuery.Text = "User Query";
			this.butUserQuery.Click += new System.EventHandler(this.butUserQuery_Click);
			// 
			// listPublicHealth
			// 
			this.listPublicHealth.Location = new System.Drawing.Point(284, 325);
			this.listPublicHealth.Name = "listPublicHealth";
			this.listPublicHealth.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listPublicHealth.Size = new System.Drawing.Size(242, 82);
			this.listPublicHealth.TabIndex = 10;
			this.listPublicHealth.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listPublicHealth_MouseDown);
			// 
			// listLists
			// 
			this.listLists.Location = new System.Drawing.Point(284, 75);
			this.listLists.Name = "listLists";
			this.listLists.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listLists.Size = new System.Drawing.Size(242, 225);
			this.listLists.TabIndex = 9;
			this.listLists.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listLists_MouseDown);
			// 
			// listMonthly
			// 
			this.listMonthly.Location = new System.Drawing.Point(12, 370);
			this.listMonthly.Name = "listMonthly";
			this.listMonthly.SelectionMode = OpenDental.UI.SelectionMode.None;
			this.listMonthly.Size = new System.Drawing.Size(242, 199);
			this.listMonthly.TabIndex = 8;
			this.listMonthly.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listMonthly_MouseDown);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(583, 618);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(680, 24);
			this.menuMain.TabIndex = 29;
			// 
			// FormReportsMore
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(680, 665);
			this.Controls.Add(this.groupBusiness);
			this.Controls.Add(this.groupPatientReviews);
			this.Controls.Add(this.butPatExport);
			this.Controls.Add(this.butPatList);
			this.Controls.Add(this.labelArizonaPrimaryCare);
			this.Controls.Add(this.listArizonaPrimaryCare);
			this.Controls.Add(this.butLaserLabels);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.listDaily);
			this.Controls.Add(this.labelDaily);
			this.Controls.Add(this.listProdInc);
			this.Controls.Add(this.labelProdInc);
			this.Controls.Add(this.butPW);
			this.Controls.Add(this.butUserQuery);
			this.Controls.Add(this.listPublicHealth);
			this.Controls.Add(this.listLists);
			this.Controls.Add(this.listMonthly);
			this.Controls.Add(this.labelMonthly);
			this.Controls.Add(this.labelLists);
			this.Controls.Add(this.labelPublicHealth);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReportsMore";
			this.ShowInTaskbar = false;
			this.Text = "Reports";
			this.Load += new System.EventHandler(this.FormReportsMore_Load);
			this.groupBusiness.ResumeLayout(false);
			this.groupPatientReviews.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private Label labelPublicHealth;
		private Label labelLists;
		private Label labelMonthly;
		private OpenDental.UI.ListBoxOD listLists;
		private OpenDental.UI.ListBoxOD listPublicHealth;
		private OpenDental.UI.Button butUserQuery;
		private OpenDental.UI.Button butPW;
		private OpenDental.UI.ListBoxOD listProdInc;
		private Label labelProdInc;
		private OpenDental.UI.ListBoxOD listDaily;
		private Label labelDaily;
		private Label label6;
		private OpenDental.UI.Button butLaserLabels;
		private OpenDental.UI.ListBoxOD listArizonaPrimaryCare;
		private Label labelArizonaPrimaryCare;
		private OpenDental.UI.ListBoxOD listMonthly;
		private UI.Button butPatList;
		private UI.Button butPatExport;
		private GroupBox groupPatientReviews;
		private UI.ODPictureBox picturePodium;
		private GroupBox groupBusiness;
		private UI.ODPictureBox picturePracticeByNumbers;
		private UI.MenuOD menuMain;
	}
}
