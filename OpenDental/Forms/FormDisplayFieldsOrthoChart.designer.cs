using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDisplayFieldsOrthoChart {
		///<summary>Required designer variable.</summary>
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)	{
			if(disposing)	{
				if(components!=null) {
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
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDisplayFieldsOrthoChart));
			this.listAvailable = new OpenDental.UI.ListBoxOD();
			this.labelAvailable = new System.Windows.Forms.Label();
			this.labelCategory = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelCustomField = new System.Windows.Forms.Label();
			this.textCustomField = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboOrthoChartTabs = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butSetupTabs = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listAvailable
			// 
			this.listAvailable.IntegralHeight = false;
			this.listAvailable.Location = new System.Drawing.Point(441, 89);
			this.listAvailable.Name = "listAvailable";
			this.listAvailable.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAvailable.Size = new System.Drawing.Size(158, 227);
			this.listAvailable.TabIndex = 3;
			this.listAvailable.Click += new System.EventHandler(this.listAvailable_Click);
			// 
			// labelAvailable
			// 
			this.labelAvailable.Location = new System.Drawing.Point(438, 69);
			this.labelAvailable.Name = "labelAvailable";
			this.labelAvailable.Size = new System.Drawing.Size(136, 17);
			this.labelAvailable.TabIndex = 16;
			this.labelAvailable.Text = "Available Fields";
			this.labelAvailable.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelCategory
			// 
			this.labelCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCategory.Location = new System.Drawing.Point(12, 9);
			this.labelCategory.Name = "labelCategory";
			this.labelCategory.Size = new System.Drawing.Size(213, 25);
			this.labelCategory.TabIndex = 57;
			this.labelCategory.Text = "Ortho Chart";
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(12, 76);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(355, 419);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Fields Showing";
			this.gridMain.TranslationName = "FormDisplayFields";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// labelCustomField
			// 
			this.labelCustomField.Location = new System.Drawing.Point(439, 319);
			this.labelCustomField.Name = "labelCustomField";
			this.labelCustomField.Size = new System.Drawing.Size(135, 17);
			this.labelCustomField.TabIndex = 58;
			this.labelCustomField.Text = "New Field";
			this.labelCustomField.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textCustomField
			// 
			this.textCustomField.Location = new System.Drawing.Point(441, 339);
			this.textCustomField.Name = "textCustomField";
			this.textCustomField.Size = new System.Drawing.Size(158, 20);
			this.textCustomField.TabIndex = 0;
			this.textCustomField.Click += new System.EventHandler(this.textCustomField_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(551, 484);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 8;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(388, 292);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 24);
			this.butRight.TabIndex = 1;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(388, 252);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 24);
			this.butLeft.TabIndex = 2;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(109, 501);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(82, 24);
			this.butDown.TabIndex = 7;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(12, 501);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(82, 24);
			this.butUp.TabIndex = 6;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(551, 514);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboOrthoChartTabs
			// 
			this.comboOrthoChartTabs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOrthoChartTabs.FormattingEnabled = true;
			this.comboOrthoChartTabs.Location = new System.Drawing.Point(59, 51);
			this.comboOrthoChartTabs.Name = "comboOrthoChartTabs";
			this.comboOrthoChartTabs.Size = new System.Drawing.Size(160, 21);
			this.comboOrthoChartTabs.TabIndex = 4;
			this.comboOrthoChartTabs.SelectionChangeCommitted += new System.EventHandler(this.comboOrthoChartTabs_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 51);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 21);
			this.label1.TabIndex = 61;
			this.label1.Text = "Tab";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSetupTabs
			// 
			this.butSetupTabs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSetupTabs.Location = new System.Drawing.Point(222, 49);
			this.butSetupTabs.Name = "butSetupTabs";
			this.butSetupTabs.Size = new System.Drawing.Size(82, 24);
			this.butSetupTabs.TabIndex = 5;
			this.butSetupTabs.Text = "Setup Tabs";
			this.butSetupTabs.Click += new System.EventHandler(this.butSetupTabs_Click);
			// 
			// FormDisplayFieldsOrthoChart
			// 
			this.ClientSize = new System.Drawing.Size(638, 550);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butSetupTabs);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboOrthoChartTabs);
			this.Controls.Add(this.textCustomField);
			this.Controls.Add(this.labelCustomField);
			this.Controls.Add(this.labelCategory);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.labelAvailable);
			this.Controls.Add(this.listAvailable);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.gridMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDisplayFieldsOrthoChart";
			this.ShowInTaskbar = false;
			this.Text = "Setup Display Fields";
			this.Load += new System.EventHandler(this.FormDisplayFields_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.ListBoxOD listAvailable;
		private Label labelAvailable;
		private OpenDental.UI.Button butRight;
		private OpenDental.UI.Button butLeft;
		private OpenDental.UI.Button butOK;
		private Label labelCategory;
		private Label labelCustomField;
		private TextBox textCustomField;
		private ComboBox comboOrthoChartTabs;
		private UI.Button butSetupTabs;
		private Label label1;
	}
}
