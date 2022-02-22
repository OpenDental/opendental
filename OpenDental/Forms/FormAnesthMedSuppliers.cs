using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormAnesthMedSuppliers:System.Windows.Forms.Form {
		private OpenDental.UI.Button butAddNew;
		private OpenDental.UI.Button butClose;
		public HorizontalAlignment textAlign;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private OpenDental.UI.ODGrid gridMain;
		//private bool changed;
		public bool IsSelectionMode;
		///<summary>Only used if IsSelectionMode.  On OK, contains selected anesthMedSuppliersNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		public int SelectedSupplierIDNum;



		///<summary></summary>
		public FormAnesthMedSuppliers() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Lan.F(this);
		}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAnesthMedSuppliers));
			this.gridMain = new OpenDental.UI.ODGrid();
			this.butAddNew = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.HScrollVisible = false;
			this.gridMain.Location = new System.Drawing.Point(17,12);
			this.gridMain.Name = "gridMain";
			this.gridMain.ScrollValue = 0;
			this.gridMain.Size = new System.Drawing.Size(1030,291);
			this.gridMain.TabIndex = 11;
			this.gridMain.Title = "Anesthetic Medication Suppliers";
			this.gridMain.TranslationName = "TableAnesthMedSuppliers";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAddNew
			// 
			this.butAddNew.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddNew.Autosize = true;
			this.butAddNew.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddNew.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddNew.CornerRadius = 4F;
			this.butAddNew.Image = global::OpenDental.Properties.Resources.Add;
			this.butAddNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddNew.Location = new System.Drawing.Point(17,317);
			this.butAddNew.Name = "butAddNew";
			this.butAddNew.Size = new System.Drawing.Size(85,24);
			this.butAddNew.TabIndex = 10;
			this.butAddNew.Text = "&Add New";
			this.butAddNew.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.CornerRadius = 4F;
			this.butClose.Location = new System.Drawing.Point(955,317);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75,24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormAnesthMedSuppliers
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(1058,357);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAddNew);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAnesthMedSuppliers";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Anesthetic Medication Suppliers";
			this.Load += new System.EventHandler(this.FormAnesthMedSuppliers_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAnesthMedSuppliers_FormClosing);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormAnesthMedSuppliers_Load(object sender,System.EventArgs e) {

			FillGrid();

		}

		private void FillGrid() {
			AnesthMedSuppliers.RefreshCache();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			textAlign=HorizontalAlignment.Center;
			ODGridColumn col=new ODGridColumn(Lan.g(this,"SupplierName"),200,textAlign);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Phone"),100,textAlign);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"Fax"),100,textAlign);
			gridMain.Columns.Add(col);
			col=new ODGridColumn(Lan.g(this,"City"),140,textAlign);
			gridMain.Columns.Add(col);
			col = new ODGridColumn(Lan.g(this,"State"),160,textAlign);
			gridMain.Columns.Add(col);
			col = new ODGridColumn(Lan.g(this,"WebSite"),140,textAlign);
			gridMain.Columns.Add(col);
			gridMain.Rows.Clear();
			ODGridRow row;
			for(int i=0;i<AnesthMedSupplierC.Listt.Count;i++) {
				row=new ODGridRow();
				row.Cells.Add(AnesthMedSupplierC.Listt[i].SupplierName);
				row.Cells.Add(AnesthMedSupplierC.Listt[i].Phone);
				row.Cells.Add(AnesthMedSupplierC.Listt[i].Fax);
				row.Cells.Add(AnesthMedSupplierC.Listt[i].City);
				row.Cells.Add(AnesthMedSupplierC.Listt[i].State);
				row.Cells.Add(AnesthMedSupplierC.Listt[i].WebSite);
				gridMain.Rows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender,System.EventArgs e) {

			AnesthMedSupplier supplCur = new AnesthMedSupplier();
			supplCur.IsNew=true;
			FormAnesthMedSuppliersEdit FormME=new FormAnesthMedSuppliersEdit();
			FormME.SupplCur = supplCur;
			FormME.ShowDialog();
			if(FormME.DialogResult == DialogResult.OK) {
				FillGrid();
			}

		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {

			FormAnesthMedSuppliersEdit FormME=new FormAnesthMedSuppliersEdit();
			FormME.SupplCur=AnesthMedSupplierC.Listt[e.Row];
			FormME.ShowDialog();
			AnesthMedSuppliers.RefreshCache();
			FillGrid();

		}


		private void butClose_Click(object sender,System.EventArgs e) {
			DialogResult = DialogResult.OK;
			Close();
		}

		private void FormAnesthMedSuppliers_FormClosing(object sender,FormClosingEventArgs e) {

		}












	}
}





















