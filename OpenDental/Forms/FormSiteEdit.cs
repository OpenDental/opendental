using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormSiteEdit:FormODBase {
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		//<summary></summary>
		//public bool IsNew;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label3;
		private UI.Button butEditZip;
		private TextBox textZip;
		private ComboBox comboZip;
		private TextBox textState;
		private Label labelST;
		private TextBox textAddress;
		private Label labelAddress2;
		private Label labelCity;
		private TextBox textAddress2;
		private Label labelZip;
		private TextBox textCity;
		private Label labelAddress;
		private Label label2;
		private ComboBox comboPlaceService;
		private Label labelPriProv;
		private UI.ComboBoxOD comboProv;
		public Site SiteCur;
		private UI.Button butSiteLink;
		private UI.Button butPickProvider;
		private List<ZipCode> _listZipCodes;

		///<summary></summary>
		public FormSiteEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSiteEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butEditZip = new OpenDental.UI.Button();
			this.textZip = new System.Windows.Forms.TextBox();
			this.comboZip = new System.Windows.Forms.ComboBox();
			this.textState = new System.Windows.Forms.TextBox();
			this.labelST = new System.Windows.Forms.Label();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.labelAddress2 = new System.Windows.Forms.Label();
			this.labelCity = new System.Windows.Forms.Label();
			this.textAddress2 = new System.Windows.Forms.TextBox();
			this.labelZip = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.labelAddress = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboPlaceService = new System.Windows.Forms.ComboBox();
			this.labelPriProv = new System.Windows.Forms.Label();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.butSiteLink = new OpenDental.UI.Button();
			this.butPickProvider = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(502, 363);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(421, 363);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(165, 15);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(254, 20);
			this.textDescription.TabIndex = 0;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 363);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 26);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(165, 214);
			this.textNote.MaxLength = 255;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(254, 144);
			this.textNote.TabIndex = 11;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(19, 216);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(144, 17);
			this.label3.TabIndex = 101;
			this.label3.Text = "Note";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butEditZip
			// 
			this.butEditZip.Location = new System.Drawing.Point(273, 188);
			this.butEditZip.Name = "butEditZip";
			this.butEditZip.Size = new System.Drawing.Size(73, 22);
			this.butEditZip.TabIndex = 10;
			this.butEditZip.Text = "&Edit Zip";
			this.butEditZip.Click += new System.EventHandler(this.butEditZip_Click);
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(165, 189);
			this.textZip.MaxLength = 100;
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(87, 20);
			this.textZip.TabIndex = 9;
			this.textZip.TextChanged += new System.EventHandler(this.textZip_TextChanged);
			this.textZip.Validating += new System.ComponentModel.CancelEventHandler(this.textZip_Validating);
			// 
			// comboZip
			// 
			this.comboZip.DropDownWidth = 198;
			this.comboZip.Location = new System.Drawing.Point(165, 189);
			this.comboZip.MaxDropDownItems = 20;
			this.comboZip.Name = "comboZip";
			this.comboZip.Size = new System.Drawing.Size(106, 21);
			this.comboZip.TabIndex = 15;
			this.comboZip.TabStop = false;
			this.comboZip.SelectionChangeCommitted += new System.EventHandler(this.comboZip_SelectionChangeCommitted);
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(165, 165);
			this.textState.MaxLength = 100;
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(61, 20);
			this.textState.TabIndex = 8;
			// 
			// labelST
			// 
			this.labelST.Location = new System.Drawing.Point(19, 169);
			this.labelST.Name = "labelST";
			this.labelST.Size = new System.Drawing.Size(145, 14);
			this.labelST.TabIndex = 102;
			this.labelST.Text = "ST";
			this.labelST.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(165, 93);
			this.textAddress.MaxLength = 100;
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(254, 20);
			this.textAddress.TabIndex = 5;
			// 
			// labelAddress2
			// 
			this.labelAddress2.Location = new System.Drawing.Point(19, 121);
			this.labelAddress2.Name = "labelAddress2";
			this.labelAddress2.Size = new System.Drawing.Size(145, 14);
			this.labelAddress2.TabIndex = 103;
			this.labelAddress2.Text = "Address2";
			this.labelAddress2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelCity
			// 
			this.labelCity.Location = new System.Drawing.Point(19, 145);
			this.labelCity.Name = "labelCity";
			this.labelCity.Size = new System.Drawing.Size(145, 14);
			this.labelCity.TabIndex = 104;
			this.labelCity.Text = "City";
			this.labelCity.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textAddress2
			// 
			this.textAddress2.Location = new System.Drawing.Point(165, 117);
			this.textAddress2.MaxLength = 100;
			this.textAddress2.Name = "textAddress2";
			this.textAddress2.Size = new System.Drawing.Size(254, 20);
			this.textAddress2.TabIndex = 6;
			// 
			// labelZip
			// 
			this.labelZip.Location = new System.Drawing.Point(19, 193);
			this.labelZip.Name = "labelZip";
			this.labelZip.Size = new System.Drawing.Size(145, 14);
			this.labelZip.TabIndex = 105;
			this.labelZip.Text = "Zip";
			this.labelZip.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(165, 141);
			this.textCity.MaxLength = 100;
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(254, 20);
			this.textCity.TabIndex = 7;
			// 
			// labelAddress
			// 
			this.labelAddress.Location = new System.Drawing.Point(19, 97);
			this.labelAddress.Name = "labelAddress";
			this.labelAddress.Size = new System.Drawing.Size(145, 14);
			this.labelAddress.TabIndex = 106;
			this.labelAddress.Text = "Address";
			this.labelAddress.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(148, 17);
			this.label2.TabIndex = 118;
			this.label2.Text = "Place of Service";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPlaceService
			// 
			this.comboPlaceService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPlaceService.Location = new System.Drawing.Point(165, 39);
			this.comboPlaceService.MaxDropDownItems = 30;
			this.comboPlaceService.Name = "comboPlaceService";
			this.comboPlaceService.Size = new System.Drawing.Size(254, 21);
			this.comboPlaceService.TabIndex = 1;
			// 
			// labelPriProv
			// 
			this.labelPriProv.Location = new System.Drawing.Point(19, 70);
			this.labelPriProv.Name = "labelPriProv";
			this.labelPriProv.Size = new System.Drawing.Size(145, 14);
			this.labelPriProv.TabIndex = 119;
			this.labelPriProv.Text = "Provider";
			this.labelPriProv.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(165, 66);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(230, 21);
			this.comboProv.TabIndex = 2;
			// 
			// butSiteLink
			// 
			this.butSiteLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSiteLink.Location = new System.Drawing.Point(257, 363);
			this.butSiteLink.Name = "butSiteLink";
			this.butSiteLink.Size = new System.Drawing.Size(75, 26);
			this.butSiteLink.TabIndex = 120;
			this.butSiteLink.Text = "Site Link";
			this.butSiteLink.Click += new System.EventHandler(this.butSiteLink_Click);
			// 
			// butPickProvider
			// 
			this.butPickProvider.Location = new System.Drawing.Point(399, 65);
			this.butPickProvider.Name = "butPickProvider";
			this.butPickProvider.Size = new System.Drawing.Size(20, 22);
			this.butPickProvider.TabIndex = 121;
			this.butPickProvider.Text = "...";
			this.butPickProvider.Click += new System.EventHandler(this.butPickProvider_Click);
			// 
			// FormSiteEdit
			// 
			this.ClientSize = new System.Drawing.Size(589, 401);
			this.Controls.Add(this.butPickProvider);
			this.Controls.Add(this.butSiteLink);
			this.Controls.Add(this.labelPriProv);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboPlaceService);
			this.Controls.Add(this.butEditZip);
			this.Controls.Add(this.textZip);
			this.Controls.Add(this.comboZip);
			this.Controls.Add(this.textState);
			this.Controls.Add(this.labelST);
			this.Controls.Add(this.textAddress);
			this.Controls.Add(this.labelAddress2);
			this.Controls.Add(this.labelCity);
			this.Controls.Add(this.textAddress2);
			this.Controls.Add(this.labelZip);
			this.Controls.Add(this.textCity);
			this.Controls.Add(this.labelAddress);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSiteEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Site";
			this.Load += new System.EventHandler(this.FormSiteEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormSiteEdit_Load(object sender,EventArgs e) {
			textDescription.Text=SiteCur.Description;
			comboPlaceService.Items.Clear();
			foreach(string name in Enum.GetNames(typeof(PlaceOfService))) {
				comboPlaceService.Items.Add(Lan.g("enumPlaceOfService",name));
			}
			comboPlaceService.SelectedIndex=(int)SiteCur.PlaceService;
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboProv.SetSelectedProvNum(SiteCur.ProvNum);
			textAddress.Text=SiteCur.Address;
			textAddress2.Text=SiteCur.Address2;
			textCity.Text=SiteCur.City;
			textState.Text=SiteCur.State;
			FillComboZip();
			textZip.Text=SiteCur.Zip;
			textNote.Text=SiteCur.Note;
			butSiteLink.Visible=PrefC.IsODHQ;
			butSiteLink.Enabled=(!SiteCur.IsNew);
		}

		private void butPickProvider_Click(object sender,EventArgs e) {
			//for dental schools
			using FormProviderPick FormPP=new FormProviderPick(comboProv.Items.GetAll<Provider>());
			FormPP.SelectedProvNum=comboProv.GetSelectedProvNum();
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(FormPP.SelectedProvNum);
		}

		private void FillComboZip() {
			comboZip.Items.Clear();
			_listZipCodes=ZipCodes.GetDeepCopy(true);
			for(int i=0;i<_listZipCodes.Count;i++) {
				comboZip.Items.Add((_listZipCodes[i]).ZipCodeDigits
					+" ("+(_listZipCodes[i]).City+")");
			}
		}

		private void textZip_TextChanged(object sender,EventArgs e) {
			comboZip.SelectedIndex=-1;
		}

		private void comboZip_SelectionChangeCommitted(object sender,EventArgs e) {
			//this happens when a zipcode is selected from the combobox of frequent zips.
			//The combo box is tucked under textZip because Microsoft makes stupid controls.
			textCity.Text=(_listZipCodes[comboZip.SelectedIndex]).City;
			textState.Text=(_listZipCodes[comboZip.SelectedIndex]).State;
			textZip.Text=(_listZipCodes[comboZip.SelectedIndex]).ZipCodeDigits;
		}

		private void textZip_Validating(object sender,CancelEventArgs e) {
			//fired as soon as control loses focus.
			//it's here to validate if zip is typed in to text box instead of picked from list.
			if(textZip.Text.Length<5) {
				return;
			}
			if(comboZip.SelectedIndex!=-1) {
				return;
			}
			//the autofill only works if both city and state are left blank
			if(textCity.Text!="" || textState.Text!="") {
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0) {
				//No match found. Must enter info for new zipcode
				ZipCode ZipCodeCur=new ZipCode();
				ZipCodeCur.ZipCodeDigits=textZip.Text;
				using FormZipCodeEdit FormZE=new FormZipCodeEdit();
				FormZE.ZipCodeCur=ZipCodeCur;
				FormZE.IsNew=true;
				FormZE.ShowDialog();
				if(FormZE.DialogResult!=DialogResult.OK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);//FormZipCodeEdit does not contain internal refresh
				FillComboZip();
				textCity.Text=ZipCodeCur.City;
				textState.Text=ZipCodeCur.State;
				textZip.Text=ZipCodeCur.ZipCodeDigits;
			}
			else if(listZipCodes.Count==1) {
				//only one match found.  Use it.
				textCity.Text=((ZipCode)listZipCodes[0]).City;
				textState.Text=((ZipCode)listZipCodes[0]).State;
			}
			else {
				//multiple matches found.  Pick one
				using FormZipSelect FormZS=new FormZipSelect();
				FormZS.ShowDialog();
				FillComboZip();
				if(FormZS.DialogResult!=DialogResult.OK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=FormZS.ZipSelected.City;
				textState.Text=FormZS.ZipSelected.State;
				textZip.Text=FormZS.ZipSelected.ZipCodeDigits;
			}
		}

		private void butEditZip_Click(object sender,EventArgs e) {
			if(textZip.Text.Length==0) {
				MessageBox.Show(Lan.g(this,"Please enter a zipcode first."));
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0) {
				using FormZipCodeEdit FormZE=new FormZipCodeEdit();
				FormZE.ZipCodeCur=new ZipCode();
				FormZE.ZipCodeCur.ZipCodeDigits=textZip.Text;
				FormZE.IsNew=true;
				FormZE.ShowDialog();
				FillComboZip();
				if(FormZE.DialogResult!=DialogResult.OK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=FormZE.ZipCodeCur.City;
				textState.Text=FormZE.ZipCodeCur.State;
				textZip.Text=FormZE.ZipCodeCur.ZipCodeDigits;
			}
			else {
				using FormZipSelect FormZS=new FormZipSelect();
				FormZS.ShowDialog();
				FillComboZip();
				if(FormZS.DialogResult!=DialogResult.OK) {
					return;
				}
				textCity.Text=FormZS.ZipSelected.City;
				textState.Text=FormZS.ZipSelected.State;
				textZip.Text=FormZS.ZipSelected.ZipCodeDigits;
			}
		}

		private void butSiteLink_Click(object sender,EventArgs e) {
			SiteLink siteLink=SiteLinks.GetFirstOrDefault(x => x.SiteNum==SiteCur.SiteNum);
			if(siteLink==null) {
				siteLink=new SiteLink() {
					SiteNum=SiteCur.SiteNum,
					ForeColor=Color.Black,
					InnerColor=Color.LightCyan,
					OuterColor=Color.Blue
				};
			}
			using FormSiteLinkEdit FormSE=new FormSiteLinkEdit(siteLink);
			FormSE.ShowDialog();
			if(FormSE.DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Sites);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(SiteCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Site?")) {
				return;
			}
			try{
				Sites.DeleteObject(SiteCur.SiteNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MessageBox.Show(Lan.g(this,"Description cannot be blank."));
				return;
			}
			SiteCur.Description=textDescription.Text;
			SiteCur.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			SiteCur.ProvNum=comboProv.GetSelectedProvNum();
			SiteCur.Address=textAddress.Text;
			SiteCur.Address2=textAddress2.Text;
			SiteCur.City=textCity.Text;
			SiteCur.State=textState.Text;
			SiteCur.Zip=textZip.Text;
			SiteCur.Note=textNote.Text;
			try{
				if(SiteCur.IsNew) {
					Sites.Insert(SiteCur);
				}
				else {
					Sites.Update(SiteCur);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















