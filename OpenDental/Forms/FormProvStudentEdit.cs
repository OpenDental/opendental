using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProvStudentEdit:FormODBase {
		private long _autoUserName;
		private bool _isGeneratingAbbr=true;
		private Userod _userodExisting;
		///<summary>Set this when selecting a pre-existing Student.</summary>
		public Provider ProviderStudent;
		private List<SchoolClass> _listSchoolClasses;

		public FormProvStudentEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProvStudentEdit_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(GenerateAbbr,
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textFirstName,textLastName);
			_userodExisting=new Userod();
			//Load the Combo Box
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			for(int i=0;i<_listSchoolClasses.Count;i++) {
				comboClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
			}
			comboClass.SelectedIndex=0;
			//Create a provider object if none has been provided
			if(ProviderStudent==null) {
				ProviderStudent=new Provider();
			}
			//From the add button - Select as much pre-given info as possible
			if(ProviderStudent.IsNew) {
				labelPassDescription.Visible=false;
				_autoUserName=Providers.GetNextAvailableProvNum();
				textUserName.Text=POut.Long(_autoUserName);//User-names are suggested to be the ProvNum of the provider.  This can be changed at will.
				for(int i=0;i<_listSchoolClasses.Count;i++) {
					if(_listSchoolClasses[i].SchoolClassNum!=ProviderStudent.SchoolClassNum) {
						continue;
					}
					comboClass.SelectedIndex=i;
					break;
				}
				textFirstName.Text=ProviderStudent.FName;
				textLastName.Text=ProviderStudent.LName;
				return;
			}
			//Double-Clicking an existing student
			_isGeneratingAbbr=false;
			for(int i=0;i<_listSchoolClasses.Count;i++) {
				if(_listSchoolClasses[i].SchoolClassNum!=ProviderStudent.SchoolClassNum) {
					continue;
				}
				comboClass.SelectedIndex=i;
				break;
			}
			textAbbr.Text=ProviderStudent.Abbr;
			textFirstName.Text=ProviderStudent.FName;
			textLastName.Text=ProviderStudent.LName;
			List<Userod> listUserods=Providers.GetAttachedUsers(ProviderStudent.ProvNum);
			if(listUserods.Count>0) {
				textUserName.Text=listUserods[0].UserName;//Should always happen if they are a student.
				_userodExisting=listUserods[0];
			}
			textProvNum.Text=POut.Long(ProviderStudent.ProvNum);
		}

		private void textAbbr_KeyUp(object sender,KeyEventArgs e) {
			_isGeneratingAbbr=false;
		}

		private void GenerateAbbr() {
			if(!_isGeneratingAbbr) {
				return;
			}
			string abbr="";
			if(textLastName.TextLength>4) {
				abbr=textLastName.Text.Substring(0,4);
			}
			else {
				abbr=textLastName.Text;
			}
			if(textFirstName.TextLength>1) {
				abbr+=textFirstName.Text.Substring(0,1);
			}
			else {
				abbr+=textFirstName.Text;
			}
			textAbbr.Text=abbr;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textFirstName.Text=="") {
				MsgBox.Show(this,"Please fill in a first name.");
				return;
			}
			if(textLastName.Text=="") {
				MsgBox.Show(this,"Please fill in a last name.");
				return;
			}
			if(textAbbr.Text=="") {
				MsgBox.Show(this,"Please fill in an abbreviation.");
				return;
			}
			if(textUserName.Text=="") {
				MsgBox.Show(this,"Please fill in a user name.");
				return;
			}
			ProviderStudent.FName=textFirstName.Text;
			ProviderStudent.LName=textLastName.Text;
			ProviderStudent.Abbr=textAbbr.Text;
			ProviderStudent.SchoolClassNum=_listSchoolClasses[comboClass.SelectedIndex].SchoolClassNum;
			Userod userodNew=new Userod();
			bool isAutoUserName=true;
			if(!ProviderStudent.IsNew || _autoUserName.ToString()!=textUserName.Text) {
				isAutoUserName=false;
			}
			if(isAutoUserName && !PrefC.GetBool(PrefName.RandomPrimaryKeys)) {//Is a new student using the default user name given
				if(textUserName.Text!=textUserName.Text.TrimEnd()) {
					MsgBox.Show(this,"User Name cannot end with white space.");
					return;
				}
				long provNum=Providers.GetNextAvailableProvNum();
				if(_autoUserName!=provNum) {
					MsgBox.Show(this,"The default user name was already taken.  The next available user name was used.");
					_autoUserName=provNum;
				}
				provNum=Providers.Insert(ProviderStudent);
				if(provNum!=_autoUserName) {
					MsgBox.Show(this,"The default user name is unavailable.  Please set a user name manually.");
					Providers.Delete(ProviderStudent);
					return;
				}
				userodNew.UserName=_autoUserName.ToString();
				userodNew.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
				userodNew.ProvNum=provNum;
				Userods.Insert(userodNew,new List<long> { PrefC.GetLong(PrefName.SecurityGroupForStudents) });
				DialogResult=DialogResult.OK;
				return;
			}
			if(ProviderStudent.IsNew){
				if(textUserName.Text!=textUserName.Text.TrimEnd()) {
					MsgBox.Show(this,"User Name cannot end with white space.");
					return;
				}
				long provNum=Providers.Insert(ProviderStudent);
				userodNew.UserName=textUserName.Text;
				userodNew.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
				userodNew.ProvNum=provNum;
				try{
					Userods.Insert(userodNew,new List<long> { PrefC.GetLong(PrefName.SecurityGroupForStudents) });//Performs validation
				}
				catch(Exception ex) {
					if(ProviderStudent.IsNew) {
						Providers.Delete(ProviderStudent);
					}
					MessageBox.Show(ex.Message);
					return;
				}
				DialogResult=DialogResult.OK;
				return;
			}
			//not new
			Providers.Update(ProviderStudent);
			_userodExisting.UserName=textUserName.Text;
			_userodExisting.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
			try{
				Userods.Update(_userodExisting);//Performs validation
			}
			catch(Exception ex) {
				if(ProviderStudent.IsNew) {
					Providers.Delete(ProviderStudent);
				}
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