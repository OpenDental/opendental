using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public class FormRpReferrals : FormODBase	{
		private System.Windows.Forms.TabPage tabData;
		private OpenDental.UI.Button butCheckAll;
		private OpenDental.UI.Button butClear;
		private System.Windows.Forms.TabPage tabFilters;
		private OpenDental.UI.ListBoxOD listOptions;
		private OpenDental.UI.ListBoxOD listPrerequisites;
		private OpenDental.UI.Button butAddFilter;
		private OpenDental.UI.ListBoxOD listConditions;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.ComboBox DropListFilter;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textSQL;
		private System.Windows.Forms.TabControl tabReferrals;
		private System.ComponentModel.Container components = null;
		private FormQuery FormQuery2;
		private string SQLselect;
		private string SQLfrom;
		private string SQLwhere;
		private bool isText;
		private bool isDropDown;
		private string sItem;//just used in local loops
		private ArrayList ALrefSelect;
		private OpenDental.UI.ListBoxOD listSelect;
		private OpenDental.UI.Button butDeleteFilter;  //fields used in SELECT 
		private ArrayList ALrefFilter;

		///<summary></summary>
		public FormRpReferrals(){
			InitializeComponent();
			InitializeLayoutManager();
			Text=Lan.g(this,"Referral Report");
      ALrefSelect=new ArrayList();
			ALrefFilter=new ArrayList();
      Fill();
			SQLselect="";
			SQLfrom="FROM referral ";
			SQLwhere="";
			listConditions.SelectedIndex=0;
			Lan.F(this);
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpReferrals));
			this.tabReferrals = new System.Windows.Forms.TabControl();
			this.tabData = new System.Windows.Forms.TabPage();
			this.listSelect = new OpenDental.UI.ListBoxOD();
			this.butCheckAll = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.tabFilters = new System.Windows.Forms.TabPage();
			this.butDeleteFilter = new OpenDental.UI.Button();
			this.listOptions = new OpenDental.UI.ListBoxOD();
			this.listPrerequisites = new OpenDental.UI.ListBoxOD();
			this.butAddFilter = new OpenDental.UI.Button();
			this.listConditions = new OpenDental.UI.ListBoxOD();
			this.textBox = new System.Windows.Forms.TextBox();
			this.DropListFilter = new System.Windows.Forms.ComboBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textSQL = new System.Windows.Forms.TextBox();
			this.tabReferrals.SuspendLayout();
			this.tabData.SuspendLayout();
			this.tabFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabReferrals
			// 
			this.tabReferrals.Controls.Add(this.tabData);
			this.tabReferrals.Controls.Add(this.tabFilters);
			this.tabReferrals.Location = new System.Drawing.Point(14,16);
			this.tabReferrals.Name = "tabReferrals";
			this.tabReferrals.SelectedIndex = 0;
			this.tabReferrals.Size = new System.Drawing.Size(814,492);
			this.tabReferrals.TabIndex = 39;
			// 
			// tabData
			// 
			this.tabData.Controls.Add(this.listSelect);
			this.tabData.Controls.Add(this.butCheckAll);
			this.tabData.Controls.Add(this.butClear);
			this.tabData.Location = new System.Drawing.Point(4,22);
			this.tabData.Name = "tabData";
			this.tabData.Size = new System.Drawing.Size(806,466);
			this.tabData.TabIndex = 1;
			this.tabData.Text = "SELECT";
			// 
			// listSelect
			// 
			this.listSelect.Location = new System.Drawing.Point(8,8);
			this.listSelect.Name = "listSelect";
			this.listSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listSelect.Size = new System.Drawing.Size(184,407);
			this.listSelect.TabIndex = 3;
			this.listSelect.SelectedIndexChanged += new System.EventHandler(this.listSelect_SelectedIndexChanged);
			// 
			// butCheckAll
			// 
			this.butCheckAll.Location = new System.Drawing.Point(10,430);
			this.butCheckAll.Name = "butCheckAll";
			this.butCheckAll.Size = new System.Drawing.Size(80,26);
			this.butCheckAll.TabIndex = 1;
			this.butCheckAll.Text = "&All";
			this.butCheckAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(100,430);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(80,26);
			this.butClear.TabIndex = 2;
			this.butClear.Text = "&None";
			this.butClear.Click += new System.EventHandler(this.butNone_Click);
			// 
			// tabFilters
			// 
			this.tabFilters.Controls.Add(this.butDeleteFilter);
			this.tabFilters.Controls.Add(this.listOptions);
			this.tabFilters.Controls.Add(this.listPrerequisites);
			this.tabFilters.Controls.Add(this.butAddFilter);
			this.tabFilters.Controls.Add(this.listConditions);
			this.tabFilters.Controls.Add(this.textBox);
			this.tabFilters.Controls.Add(this.DropListFilter);
			this.tabFilters.Location = new System.Drawing.Point(4,22);
			this.tabFilters.Name = "tabFilters";
			this.tabFilters.Size = new System.Drawing.Size(806,466);
			this.tabFilters.TabIndex = 0;
			this.tabFilters.Text = "WHERE";
			this.tabFilters.Visible = false;
			// 
			// butDeleteFilter
			// 
			this.butDeleteFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDeleteFilter.Image = ((System.Drawing.Image)(resources.GetObject("butDeleteFilter.Image")));
			this.butDeleteFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteFilter.Location = new System.Drawing.Point(10,426);
			this.butDeleteFilter.Name = "butDeleteFilter";
			this.butDeleteFilter.Size = new System.Drawing.Size(110,26);
			this.butDeleteFilter.TabIndex = 34;
			this.butDeleteFilter.Text = "&Delete Row";
			this.butDeleteFilter.Click += new System.EventHandler(this.butDeleteFilter_Click);
			// 
			// comboBox
			// 
			this.listOptions.Location = new System.Drawing.Point(358,12);
			this.listOptions.Name = "comboBox";
			this.listOptions.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listOptions.Size = new System.Drawing.Size(266,199);
			this.listOptions.TabIndex = 12;
			this.listOptions.Visible = false;
			this.listOptions.SelectedIndexChanged += new System.EventHandler(this.listOptions_SelectedIndexChanged);
			// 
			// ListPrerequisites
			// 
			this.listPrerequisites.Location = new System.Drawing.Point(10,234);
			this.listPrerequisites.Name = "ListPrerequisites";
			this.listPrerequisites.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPrerequisites.Size = new System.Drawing.Size(608,173);
			this.listPrerequisites.TabIndex = 7;
			this.listPrerequisites.SelectedIndexChanged += new System.EventHandler(this.ListPrerequisites_SelectedIndexChanged);
			// 
			// butAddFilter
			// 
			this.butAddFilter.Enabled = false;
			this.butAddFilter.Location = new System.Drawing.Point(664,12);
			this.butAddFilter.Name = "butAddFilter";
			this.butAddFilter.Size = new System.Drawing.Size(75,23);
			this.butAddFilter.TabIndex = 6;
			this.butAddFilter.Text = "&Add";
			this.butAddFilter.Click += new System.EventHandler(this.butAddFilter_Click);
			// 
			// ListConditions
			// 
			this.listConditions.Items.AddRange(new object[] {
            "LIKE",
            "=",
            ">",
            "<",
            ">=",
            "<=",
            "<>"});
			this.listConditions.Location = new System.Drawing.Point(232,12);
			this.listConditions.Name = "ListConditions";
			this.listConditions.Size = new System.Drawing.Size(78,95);
			this.listConditions.TabIndex = 5;
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(358,12);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(262,20);
			this.textBox.TabIndex = 2;
			this.textBox.Visible = false;
			// 
			// DropListFilter
			// 
			this.DropListFilter.Location = new System.Drawing.Point(8,12);
			this.DropListFilter.MaxDropDownItems = 45;
			this.DropListFilter.Name = "DropListFilter";
			this.DropListFilter.Size = new System.Drawing.Size(172,21);
			this.DropListFilter.TabIndex = 1;
			this.DropListFilter.Text = "WHERE";
			this.DropListFilter.SelectedIndexChanged += new System.EventHandler(this.DropListFilter_SelectedIndexChanged);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(750,640);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 41;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(750,602);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 40;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textSQL
			// 
			this.textSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSQL.Location = new System.Drawing.Point(16,542);
			this.textSQL.Multiline = true;
			this.textSQL.Name = "textSQL";
			this.textSQL.ReadOnly = true;
			this.textSQL.Size = new System.Drawing.Size(692,124);
			this.textSQL.TabIndex = 42;
			// 
			// FormRpReferrals
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(842,683);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.tabReferrals);
			this.Controls.Add(this.textSQL);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpReferrals";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormRpReferrals";
			this.tabReferrals.ResumeLayout(false);
			this.tabData.ResumeLayout(false);
			this.tabFilters.ResumeLayout(false);
			this.tabFilters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void Fill()  {
			FillALrefFilter();
			FillSelectList();
			FillDropListFilter();
		}

		private void FillSelectList(){
      listSelect.Items.Add("Address");
      listSelect.Items.Add("Address2");
      listSelect.Items.Add("City");
      listSelect.Items.Add("Email");
      listSelect.Items.Add("FName");
      listSelect.Items.Add("IsHidden");
      listSelect.Items.Add("LName");
      listSelect.Items.Add("MName");
      listSelect.Items.Add("Note");
      listSelect.Items.Add("NotPerson");
      listSelect.Items.Add("Phone2");
			listSelect.Items.Add("ReferralNum");
      listSelect.Items.Add("Specialty");
      listSelect.Items.Add("SSN");
      listSelect.Items.Add("ST");
      listSelect.Items.Add("Telephone");
      listSelect.Items.Add("Title");
      listSelect.Items.Add("UsingTIN");
      listSelect.Items.Add("Zip");
		}

		private void FillALrefFilter(){//FillALrefFilter
      ALrefFilter.Add("Address"); 
      ALrefFilter.Add("Address2"); 
      ALrefFilter.Add("City");
      ALrefFilter.Add("Email");  
      ALrefFilter.Add("FName"); 
      ALrefFilter.Add("IsHidden"); 
      ALrefFilter.Add("LName");
      ALrefFilter.Add("MName");
      ALrefFilter.Add("Note");  
      ALrefFilter.Add("NotPerson");   
      ALrefFilter.Add("Phone2");
			ALrefFilter.Add("ReferralNum"); 
      ALrefFilter.Add("Specialty"); 
      ALrefFilter.Add("SSN"); 
      ALrefFilter.Add("ST"); 
      ALrefFilter.Add("Telephone");
      ALrefFilter.Add("Title"); 
      ALrefFilter.Add("UsingTIN");
      ALrefFilter.Add("Zip");  
		}

		private void FillDropListFilter(){
			for(int i=0;i<ALrefFilter.Count;i++){
				DropListFilter.Items.Add(ALrefFilter[i]);
			}
		}

		private void fillSQLbox(){
			textSQL.Text=SQLselect+SQLfrom+SQLwhere;
		}

		private void createSQLfrom(){
			List<string> listfieldsSelected=new List<string>(); 
			if(listSelect.SelectedIndices.Count>0){
				SQLselect="SELECT ";
				for(int i=0;i<listSelect.SelectedIndices.Count;i++){
					if(i>0){
						SQLselect+=", ";
					}
					SQLselect+=listSelect.Items.GetTextShowingAt(listSelect.SelectedIndices[i]);
				}
				SQLselect+=" ";
				fillSQLbox();
				butOK.Enabled=true;
			}
			else{
				SQLselect="";
				fillSQLbox();
				butOK.Enabled=false;
			}
		}

		private void listSelect_SelectedIndexChanged(object sender, System.EventArgs e) {
			createSQLfrom();
		}

		private void createSQLwhere(){
			SQLwhere="WHERE ";
			for(int i=0;i<listPrerequisites.Items.Count;i++){
				if(i==0) {
					SQLwhere+=listPrerequisites.Items.GetTextShowingAt(i);
				}
				else if(listPrerequisites.Items.GetTextShowingAt(i).Substring(0,2)=="OR") {
					SQLwhere+=" "+listPrerequisites.Items.GetTextShowingAt(i);
				}
				else {
					SQLwhere+=" AND "+listPrerequisites.Items.GetTextShowingAt(i);
				}
			}
			fillSQLbox();
			if(listSelect.SelectedIndices.Count>0) {
				butOK.Enabled=true;
			}
		}

		
		
		private void DropListFilter_SelectedIndexChanged(object sender, System.EventArgs e) {
			switch(DropListFilter.SelectedItem.ToString()){
   		  case "Address":
   		  case "Address2":
   		  case "City":
        case "EMail":
        case "FName":
  		  case "LName":
   		  case "MName":
        case "Note":
   		  case "Phone2":
        case "ReferralNum":
   		  case "ST":
   		  case "SSN":
   		  case "Telephone":
   		  case "Title":
   		  case "Zip":
					textBox.Clear();
			    listConditions.Enabled=true;
          textBox.Visible=true;
					listOptions.Visible=false;
					textBox.Select();
					isText=true;
					isDropDown=false;
    			butAddFilter.Enabled=true;
					break;
   		  case "Specialty":
          setListBoxConditions();
          listOptions.Items.Clear();
					Def[] specDefs=Defs.GetDefsForCategory(DefCat.ProviderSpecialties,true).ToArray();
					for(int i=0;i<specDefs.Length;i++) {
						listOptions.Items.Add(Lan.g("enumDentalSpecialty",specDefs[i].ItemName));
					}
					break;
   		  case "UsingTIN":
        case "IsHidden":
        case "NotPerson": 
          setListBoxConditions();
					listOptions.Items.Clear();
					listOptions.Items.Add("False");
					listOptions.Items.Add("True");
					break;
			}
		}

		private void setListBoxConditions(){
			listOptions.Visible=true;
      textBox.Visible=false;
			isDropDown=true;
			isText=false;
			listConditions.Enabled=true;
			listOptions.SelectedIndex=-1;
			butAddFilter.Enabled=false;
		}

		private void butAddFilter_Click(object sender, System.EventArgs e) {
			if(isText){
				if(listConditions.SelectedIndex==0){
					listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()+" LIKE '%"+textBox.Text+"%'");
				}
				else{
					listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()+" "+listConditions.SelectedItem.ToString()+" '"+textBox.Text+"'");
				}
  		}//end if(isText)
			else if(isDropDown){
				if(DropListFilter.SelectedItem.ToString()=="Specialty"){
					sItem="";
					Def[] specDefs=Defs.GetDefsForCategory(DefCat.ProviderSpecialties,true).ToArray();
					for(int i=0;i<listOptions.SelectedIndices.Count;i++){
						if(i==0){ 
              sItem="(";
            }
						else{ 
              sItem="OR ";
            }
						sItem+="Specialty "+listConditions.SelectedItem.ToString()+" '"+
							specDefs[listOptions.SelectedIndices[i]].DefNum.ToString()+"'"; 
						if(i==listOptions.SelectedIndices.Count-1) {
							sItem+=")";
						}
						listPrerequisites.Items.Add(sItem);
					}
				}
				else{
					for(int i=0;i<listOptions.SelectedIndices.Count;i++){
						if(listConditions.SelectedIndex==0){ 
							listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()+" LIKE '%"+listOptions.SelectedIndices[i].ToString()+"%'");  
						}
						else{
							listPrerequisites.Items.Add(DropListFilter.SelectedItem.ToString()+" "
								+listConditions.SelectedItem.ToString()+" '"
								+listOptions.SelectedIndices[i].ToString()+"'");   
						}
					}
 				}
				listOptions.SelectedIndex=-1;
				butAddFilter.Enabled=false;
			}//end else if(isDropDown)
			createSQLwhere();
			listConditions.Enabled=true;
			textBox.Clear();
		}

		private void butDeleteFilter_Click(object sender, System.EventArgs e) {
			for(int i=listPrerequisites.SelectedIndices.Count;i>0;i--){
				listPrerequisites.Items.RemoveAt(listPrerequisites.SelectedIndices[i-1]);
			}
			if(listPrerequisites.Items.Count>0){
				createSQLwhere();
			}
			else{
				SQLwhere="";
			}
			fillSQLbox();
		}

		private void butAll_Click(object sender, System.EventArgs e){		
			listSelect.SetAll(true);
			createSQLfrom();
		}

		private void butNone_Click(object sender, System.EventArgs e) {
			listSelect.SetAll(false);
			createSQLfrom();	
		}

		private void listOptions_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(listOptions.SelectedIndices.Count>0) {
				butAddFilter.Enabled=true;
			}
			else {
				butAddFilter.Enabled=false;
			}
		}

		private void ListPrerequisites_SelectedIndexChanged(object sender, System.EventArgs e) {
			butDeleteFilter.Enabled=false;
			if(listPrerequisites.Items.Count>0 && listPrerequisites.SelectedIndices.Count>0){
				butDeleteFilter.Enabled=true;
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query=textSQL.Text;
			FormQuery2=new FormQuery(report);
			FormQuery2.IsReport=false;
			FormQuery2.SubmitQuery();	
      FormQuery2.textQuery.Text=report.Query;					
			FormQuery2.ShowDialog();
			FormQuery2.Dispose();
			//DialogResult=DialogResult.OK;				

		}



	}
}






