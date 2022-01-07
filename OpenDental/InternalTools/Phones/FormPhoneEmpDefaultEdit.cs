using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPhoneEmpDefaultEdit:FormODBase {
		///<summary></summary>
		public bool IsNew;
		public PhoneEmpDefault PedCur;
		private PhoneEmpDefault _pedOld;
		private bool _isLoading;

		///<summary></summary>
		public FormPhoneEmpDefaultEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPhoneEmpDefaultEdit_Load(object sender, System.EventArgs e) {
			_isLoading=true;
			_pedOld=PedCur.Copy();
			if(!IsNew){
				textEmployeeNum.ReadOnly=true;
			}
			textEmployeeNum.Text=PedCur.EmployeeNum.ToString();
			textEmpName.Text=PedCur.EmpName;
			Employee employee=Employees.GetEmp(PedCur.EmployeeNum);
			if(employee!=null){
				Employee employeeSuper=Employees.GetEmp(employee.ReportsTo);
				if(employeeSuper!=null){
					textReportsTo.Text=employeeSuper.FName;
				}
				textWirelessPhone.Text=employee.WirelessPhone;
				textEmailWork.Text=employee.EmailWork;
				textEmailPersonal.Text=employee.EmailPersonal;
				checkIsFurloughed.Checked=employee.IsFurloughed;
				checkIsWorkingHome.Checked=employee.IsWorkingHome;
			}
			checkIsGraphed.Checked=PedCur.IsGraphed;
			checkHasColor.Checked=PedCur.HasColor;
			listRingGroup.Items.AddEnums<AsteriskQueues>();
			listRingGroup.SetSelectedEnum(PedCur.RingGroups);
			textPhoneExt.Text=PedCur.PhoneExt.ToString();
			listStatusOverride.Items.AddEnums<PhoneEmpStatusOverride>();
			listStatusOverride.SetSelectedEnum(PedCur.StatusOverride);
			textNotes.Text=PedCur.Notes;
			List<Site> _listSites=Sites.GetDeepCopy();
			for(int i=0;i<_listSites.Count;i++) {
				comboSite.Items.Add(_listSites[i].Description,_listSites[i]);
				if(_listSites[i].SiteNum==_pedOld.SiteNum) {
					comboSite.SelectedIndex=i;
				}
			}
			checkIsPrivateScreen.Checked=true;//we no longer capture screen shots.
			checkIsTriageOperator.Checked=PedCur.IsTriageOperator;
			_isLoading=false;
		}

		private void checkIsPrivateScreen_Click(object sender,EventArgs e) {
			if(Security.CurUser.EmployeeNum!=10			//Debbie
				&& Security.CurUser.EmployeeNum!=13		//Shannon
				&& Security.CurUser.EmployeeNum!=17		//Nathan
				&& Security.CurUser.EmployeeNum!=22)	//Jordan
			{
				//Put the checkbox back the way it was before user clicked on it.
				if(checkIsPrivateScreen.Checked) {
					checkIsPrivateScreen.Checked=false;
				}
				else {
					checkIsPrivateScreen.Checked=true;
				}
				MsgBox.Show(this,"You do not have permission to halt screen captures.");
			}
		}

		private void checkIsGraphed_Click(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.Schedules) && Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			//Put the checkbox back the way it was before user clicked on it.
			if(checkIsGraphed.Checked) {
				checkIsGraphed.Checked=false;
			}
			else {
				checkIsGraphed.Checked=true;
			}
		}

		private void checkHasColor_Click(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.Schedules) && Security.IsAuthorized(Permissions.Setup)) {
				return;//allowed to change if user has both permissions. 
			}
			//Put the checkbox back the way it was before user clicked on it.
			if(checkHasColor.Checked) {
				checkHasColor.Checked=false;
			}
			else {
				checkHasColor.Checked=true;
			}
		}

		private void listRingGroup_SelectedIndexChanged(object sender,EventArgs e) {
			if(_isLoading || listRingGroup.GetSelected<AsteriskQueues>()==_pedOld.RingGroups || Security.IsAuthorized(Permissions.Setup)) {
				return;//either we're loading, or index clicked on is the same as inital, or user is okay to change. 
			}
			//user is not allowed, switch index back to what it was when loading.
			listRingGroup.SetSelectedEnum(_pedOld.RingGroups);
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			PhoneEmpDefaults.Delete(PedCur.EmployeeNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//Using a switch statement in case we want special functionality for the other statuses later on.
			switch(listStatusOverride.GetSelected<PhoneEmpStatusOverride>()) {
				case PhoneEmpStatusOverride.None:
					if(_pedOld.StatusOverride==PhoneEmpStatusOverride.Unavailable) {
						MsgBox.Show(this,"Change your status from unavailable by using the small phone panel.");
						return;
					}
					break;
				case PhoneEmpStatusOverride.OfflineAssist:
					if(_pedOld.StatusOverride==PhoneEmpStatusOverride.Unavailable) {
						MsgBox.Show(this,"Change your status from unavailable by using the small phone panel.");
						return;
					}
					break;
			}
			if(IsNew) {
				if(textEmployeeNum.Text=="") {
					MsgBox.Show(this,"Unique EmployeeNum is required.");
					return;
				}
				if(textEmpName.Text=="") {
					MsgBox.Show(this,"Employee name is required.");
					return;
				}
				PedCur.EmployeeNum=PIn.Long(textEmployeeNum.Text);
			}
			//Get the current database state of the phone emp default (before we change it)
			PhoneEmpDefault pedFromDatabase=PhoneEmpDefaults.GetOne(PedCur.EmployeeNum);
			if(pedFromDatabase==null) {
				pedFromDatabase=new PhoneEmpDefault();
			}
			else if(pedFromDatabase!=null && IsNew) {
				MessageBox.Show("Employee Num already in use.\r\nEdit their current phone settings entry instead of creating a duplicate.");
				return;
			}
			int newExtension=PIn.Int(textPhoneExt.Text);
			bool extensionChange=pedFromDatabase.PhoneExt!=newExtension;
			if(extensionChange) { //Only check when extension has changed and clocked in.
				//We need to prevent changes to phoneempdefault table which involve employees who are currently logged in.
				//Failing to do so would cause subtle race conditions between the phone table and phoneempdefault.
				//Net result would be the phone panel looking wrong.			
				if(ClockEvents.IsClockedIn(PedCur.EmployeeNum)) {//Prevent any change if employee being edited is currently clocked in.
					MsgBox.Show(this,"You must first clock out before making changes");
					return;
				}
				//Find out if the target extension is already being occuppied by a different employee.
				Phone phoneOccuppied=Phones.GetPhoneForExtensionDB(PIn.Int(textPhoneExt.Text));
				if(phoneOccuppied!=null) {
					if(ClockEvents.IsClockedIn(phoneOccuppied.EmployeeNum)) { //Prevent change if employee's new extension is occupied by a different employee who is currently clocked in.
						MessageBox.Show(Lan.g(this,"This extension cannot be inherited because it is currently occuppied by an employee who is currently logged in.\r\n\r\nExisting employee: ")+phoneOccuppied.EmployeeName);
						return;
					}
					if(phoneOccuppied.EmployeeNum!=PedCur.EmployeeNum) {
						//We are setting to a new employee so let's clean up the old employee.
						//This will prevent duplicates in the phone table and subsequently prevent duplicates in the phone panel.
						Phones.UpdatePhoneToEmpty(phoneOccuppied.EmployeeNum,-1);
						PhoneEmpDefault pedOccuppied=PhoneEmpDefaults.GetOne(phoneOccuppied.EmployeeNum);
						if(pedOccuppied!=null) {//prevent duplicate in phoneempdefault
							pedOccuppied.PhoneExt=0;
							PhoneEmpDefaults.Update(pedOccuppied);
						}
					}
				}
				//Get the employee that is normally assigned to this extension (assigned ext set in the employee table).
				long permanentLinkageEmployeeNum=Employees.GetEmpNumAtExtension(pedFromDatabase.PhoneExt);
				if(permanentLinkageEmployeeNum>=1) { //Extension is nomrally assigned to an employee.
					if(PedCur.EmployeeNum!=permanentLinkageEmployeeNum) {//This is not the normally linked employee so let's revert back to the proper employee.
						PhoneEmpDefault pedRevertTo=PhoneEmpDefaults.GetOne(permanentLinkageEmployeeNum);
						//Make sure the employee we are about to revert is not logged in at yet a different workstation. This would be rare but it's worth checking.
						if(pedRevertTo!=null && !ClockEvents.IsClockedIn(pedRevertTo.EmployeeNum)) {
							//Revert to the permanent extension for this PhoneEmpDefault.
							pedRevertTo.PhoneExt=pedFromDatabase.PhoneExt;
							PhoneEmpDefaults.Update(pedRevertTo);
							//Update phone table to match this change.
							Phones.SetPhoneStatus(ClockStatusEnum.Home,pedRevertTo.PhoneExt,pedRevertTo.EmployeeNum);
						}
					}
				}
			}
			//Ordering of these updates is IMPORTANT!!!
			//Phone Emp Default must be updated first
			PedCur.EmpName=textEmpName.Text;
			PedCur.IsGraphed=checkIsGraphed.Checked;
			PedCur.HasColor=checkHasColor.Checked;
			PedCur.RingGroups=listRingGroup.GetSelected<AsteriskQueues>();
			PedCur.PhoneExt=PIn.Int(textPhoneExt.Text);
			PedCur.StatusOverride=listStatusOverride.GetSelected<PhoneEmpStatusOverride>();
			PedCur.Notes=textNotes.Text;
			if(comboSite.SelectedIndex > -1) {
				PedCur.SiteNum=comboSite.GetSelected<Site>().SiteNum;
			}
			PedCur.IsPrivateScreen=true;//we no longer capture screen shots.
			PedCur.IsTriageOperator=checkIsTriageOperator.Checked;
			if(IsNew){
				PhoneEmpDefaults.Insert(PedCur);
				DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
				//insert a new Phone record to keep the 2 tables in sync an entry for the new extension in the phone table doesn't already exist.
				if(PedCur.PhoneExt!=0 && Phones.GetPhoneForExtensionDB(PedCur.PhoneExt)==null) {
					Phone phoneNew=new Phone();
					phoneNew.EmployeeName=PedCur.EmpName;
					phoneNew.EmployeeNum=PedCur.EmployeeNum;
					phoneNew.Extension=PedCur.PhoneExt;
					phoneNew.ClockStatus=ClockStatusEnum.Home;
					Phones.Insert(phoneNew);
				}
			}
			else{
				PhoneEmpDefaults.Update(PedCur);
				DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			}
			//It is now safe to update Phone table as it will draw from the newly updated Phone Emp Default row
			if(listStatusOverride.GetSelected<PhoneEmpStatusOverride>()==PhoneEmpStatusOverride.Unavailable &&
				ClockEvents.IsClockedIn(PedCur.EmployeeNum)) {
				//We set ourselves unavailable from this window because we require an explanation.
				//This is the only status that will synch with the phone table, all others should be handled by the small phone panel.
				Phones.SetPhoneStatus(ClockStatusEnum.Unavailable,PedCur.PhoneExt,PedCur.EmployeeNum);
			}
			if(extensionChange) {
				//Phone extension has changed so update the phone table as well. 
				//We have already guaranteed that this employee is Clocked Out (above) so set to home and update phone table.
				Phones.SetPhoneStatus(ClockStatusEnum.Home,PedCur.PhoneExt,PedCur.EmployeeNum);
			}
			//The user just flagged themselves as a triage operator
			//OR
			//This user used to be a triage operator and they no longer want to be one which will need their ring group set back to their default.
			if((!_pedOld.IsTriageOperator && checkIsTriageOperator.Checked)
				|| (_pedOld.IsTriageOperator && !checkIsTriageOperator.Checked))
			{
				//Set the queue for this phone emp default to whatever the current ClockStatus is for the phone row associated to this PED.
				PhoneAsterisks.SetQueueForClockStatus(PedCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
		

	

	

		

		

		


	}
}





















