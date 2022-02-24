using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public class FormZipSelect : FormODBase {
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listMatches;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butEdit;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butAdd;
		private System.ComponentModel.Container components=null;
		private bool changed;
		public ZipCode ZipSelected;
		private List<ZipCode> _listZipCodes;
		private string _zipCodeDigits;

		///<summary></summary>
		public FormZipSelect(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Pass in a 5 digit zipcode to filter down the loaded list of ZipCodes.</summary>
		public FormZipSelect(string zipCodeDigits): this(){
			_zipCodeDigits=zipCodeDigits;
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormZipSelect));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listMatches = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butEdit = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(336, 153);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 0;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(336, 186);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listMatches
			// 
			this.listMatches.Location = new System.Drawing.Point(24, 45);
			this.listMatches.Name = "listMatches";
			this.listMatches.Size = new System.Drawing.Size(197, 95);
			this.listMatches.TabIndex = 3;
			this.listMatches.DoubleClick += new System.EventHandler(this.listMatches_DoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(25, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(377, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Cities attached to this zipcode:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEdit.Image = global::OpenDental.Properties.Resources.editPencil;
			this.butEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEdit.Location = new System.Drawing.Point(208, 186);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(75, 26);
			this.butEdit.TabIndex = 12;
			this.butEdit.Text = "&Edit";
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(119, 186);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 11;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(30, 186);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 26);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// FormZipSelect
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(430, 235);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listMatches);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormZipSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Zipcode";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormZipSelect_Closing);
			this.Load += new System.EventHandler(this.FormZipSelect_Load);
			this.ResumeLayout(false);

		}
		#endregion

		//This form is only accessed directly from the patient edit window, either by pushing the
		//button, or when user enters a zipcode that has more than one city available.
		
		private void FormZipSelect_Load(object sender, System.EventArgs e) {
		  FillList();
		}
		
		private void FillList(){
			//refreshing is done within each routine
			listMatches.Items.Clear();
			string itemText="";
			_listZipCodes=ZipCodes.GetDeepCopy();
			if(!string.IsNullOrWhiteSpace(_zipCodeDigits)) {
				_listZipCodes.RemoveAll(x => x.ZipCodeDigits!=_zipCodeDigits);
			}
			for(int i=0;i<_listZipCodes.Count;i++){ 
				itemText=(_listZipCodes[i]).City+" "
					+(_listZipCodes[i]).State;
				if((_listZipCodes[i]).IsFrequent){
					itemText+=Lan.g(this," (freq)");
				}
				listMatches.Items.Add(itemText);
			}
			listMatches.SelectedIndex=-1;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormZipCodeEdit FormZCE=new FormZipCodeEdit();
			FormZCE.ZipCodeCur=new ZipCode();
			FormZCE.ZipCodeCur.ZipCodeDigits=(_listZipCodes[0]).ZipCodeDigits;
			FormZCE.IsNew=true;
			FormZCE.ShowDialog();
			if(FormZCE.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(FormZCE.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butEdit_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			using FormZipCodeEdit FormZCE=new FormZipCodeEdit();
			FormZCE.ZipCodeCur=_listZipCodes[listMatches.SelectedIndex];
			FormZCE.ShowDialog();
			if(FormZCE.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(FormZCE.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ZipCode ZipCodeCur=_listZipCodes[listMatches.SelectedIndex];
			ZipCodes.Delete(ZipCodeCur);
			changed=true;
			ZipCodes.RefreshCache();
			FillList();
		}

		private void listMatches_DoubleClick(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				return;
			}
			ZipSelected=_listZipCodes[listMatches.SelectedIndex];
			DialogResult=DialogResult.OK;		
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ZipSelected=_listZipCodes[listMatches.SelectedIndex];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormZipSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.ZipCodes);
			}
		}

		


	}
}





