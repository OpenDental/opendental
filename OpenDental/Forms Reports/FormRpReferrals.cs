using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpReferrals : FormODBase	{
		private FormQuery FormQuery2;
		private string SQLselect;
		private string SQLfrom;
		private string SQLwhere;
		private bool isText;
		private bool isDropDown;
		private string sItem;//just used in local loops
		private ArrayList ALrefSelect;
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
        case "Email":
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






