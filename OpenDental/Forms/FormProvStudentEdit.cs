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
		private Userod _existingUser;
		///<summary>Set this when selecting a pre-existing Student.</summary>
		public Provider ProvStudent;
		private List<SchoolClass> _listSchoolClasses;

		public FormProvStudentEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProvStudentEdit_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => {
					if(_isGeneratingAbbr) {
						GenerateAbbr();
					}
				},
				(int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
				textFirstName,textLastName);
			_existingUser=new Userod();
			//Load the Combo Box
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			for(int i=0;i<_listSchoolClasses.Count;i++) {
				comboClass.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
			}
			comboClass.SelectedIndex=0;
			//Create a provider object if none has been provided
			if(ProvStudent==null) {
				ProvStudent=new Provider();
			}
			//From the add button - Select as much pre-given info as possible
			if(ProvStudent.IsNew) {
				labelPassDescription.Visible=false;
				_autoUserName=Providers.GetNextAvailableProvNum();
				textUserName.Text=POut.Long(_autoUserName);//User-names are suggested to be the ProvNum of the provider.  This can be changed at will.
				for(int i=0;i<_listSchoolClasses.Count;i++) {
					if(_listSchoolClasses[i].SchoolClassNum!=ProvStudent.SchoolClassNum) {
						continue;
					}
					comboClass.SelectedIndex=i;
					break;
				}
				textFirstName.Text=ProvStudent.FName;
				textLastName.Text=ProvStudent.LName;
			}
			//Double-Clicking an existing student
			else {
				_isGeneratingAbbr=false;
				for(int i=0;i<_listSchoolClasses.Count;i++) {
					if(_listSchoolClasses[i].SchoolClassNum!=ProvStudent.SchoolClassNum) {
						continue;
					}
					comboClass.SelectedIndex=i;
					break;
				}
				textAbbr.Text=ProvStudent.Abbr;
				textFirstName.Text=ProvStudent.FName;
				textLastName.Text=ProvStudent.LName;
				List<Userod> userList=Providers.GetAttachedUsers(ProvStudent.ProvNum);
				if(userList.Count>0) {
					textUserName.Text=userList[0].UserName;//Should always happen if they are a student.
					_existingUser=userList[0];
				}
				textProvNum.Text=POut.Long(ProvStudent.ProvNum);
			}
		}

		private void textAbbr_KeyUp(object sender,KeyEventArgs e) {
			_isGeneratingAbbr=false;
		}

		private void GenerateAbbr() {
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
			ProvStudent.FName=textFirstName.Text;
			ProvStudent.LName=textLastName.Text;
			ProvStudent.Abbr=textAbbr.Text;
			ProvStudent.SchoolClassNum=_listSchoolClasses[comboClass.SelectedIndex].SchoolClassNum;
			Userod newUser=new Userod();
			bool isAutoUserName=true;
			if(!ProvStudent.IsNew || _autoUserName.ToString()!=textUserName.Text) {
				isAutoUserName=false;
			}
			if(isAutoUserName && !PrefC.GetBool(PrefName.RandomPrimaryKeys)) {//Is a new student using the default user name given
				long provNum=Providers.GetNextAvailableProvNum();
				if(_autoUserName!=provNum) {
					MsgBox.Show(this,"The default user name was already taken.  The next available user name was used.");
					_autoUserName=provNum;
				}
				provNum=Providers.Insert(ProvStudent);
				if(provNum!=_autoUserName) {
					MsgBox.Show(this,"The default user name is unavailable.  Please set a user name manually.");
					Providers.Delete(ProvStudent);
					return;
				}
				newUser.UserName=_autoUserName.ToString();
				newUser.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
				newUser.ProvNum=provNum;
				Userods.Insert(newUser,new List<long> { PrefC.GetLong(PrefName.SecurityGroupForStudents) });
			}
			else {//Has changed the user name from the default or is editing a pre-existing student
				try {
					if(ProvStudent.IsNew) {
						long provNum=Providers.Insert(ProvStudent);
						newUser.UserName=textUserName.Text;
						newUser.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
						newUser.ProvNum=provNum;
						Userods.Insert(newUser,new List<long> { PrefC.GetLong(PrefName.SecurityGroupForStudents) });//Performs validation
					}
					else {
						Providers.Update(ProvStudent);
						_existingUser.UserName=textUserName.Text;
						_existingUser.LoginDetails=Authentication.GenerateLoginDetailsSHA512(textPassword.Text);
						Userods.Update(_existingUser);//Performs validation
					}
				}
				catch(Exception ex) {
					if(ProvStudent.IsNew) {
						Providers.Delete(ProvStudent);
					}
					MessageBox.Show(ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}