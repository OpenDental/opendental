using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormOperatoryEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private Operatory _operatory;
		public List<Operatory> ListOperatories;
		///<summary>All of the Web Sched New Pat Appt appointment type defs that this operatory is associated to.</summary>
		private List<Def> _listDefsWSNPAOperatory=new List<Def>();
		private List<Def> _listDefsWSEPOperatory=new List<Def>();

		///<summary>This reference is passed in because it's needed for the "Update Provs on Future Appts" tool.</summary>
		public ControlAppt ControlApptRef;

		///<summary></summary>
		public FormOperatoryEdit(Operatory operatory) {
			_operatory=operatory;
			ControlApptRef=null;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOperatoryEdit_Load(object sender, System.EventArgs e) {
			textOpName.Text=_operatory.OpName;
			textAbbrev.Text=_operatory.Abbrev;
			checkIsHidden.Checked=_operatory.IsHidden;
			comboClinic.ClinicNumSelected=_operatory.ClinicNum;//can be 0
			FillCombosProv();
			comboProv.SetSelectedProvNum(_operatory.ProvDentist);
			comboHyg.SetSelectedProvNum(_operatory.ProvHygienist);
			if(_operatory.ListWSNPAOperatoryDefNums!=null) {
				//This is an existing operatory with WSNPA appointment types associated.  Go get them in order to display to the user.
				_listDefsWSNPAOperatory=Defs.GetDefs(DefCat.WebSchedNewPatApptTypes,_operatory.ListWSNPAOperatoryDefNums);
			}
			if(_operatory.ListWSEPOperatoryDefNums!=null) {
				_listDefsWSEPOperatory=Defs.GetDefs(DefCat.WebSchedExistingApptTypes,_operatory.ListWSEPOperatoryDefNums);
			}
			textWSNPAApptTypes.Text=string.Join(", ",_listDefsWSNPAOperatory.Select(x => x.ItemName));//WSNPA
			textWSEPApptTypes.Text=string.Join(", ",_listDefsWSEPOperatory.Select(x => x.ItemName));//WSEP
			checkIsHygiene.Checked=_operatory.IsHygiene;
			checkSetProspective.Checked=_operatory.SetProspective;
			checkIsWebSched.Checked=_operatory.IsWebSched;
			if(ControlApptRef==null) {
				butUpdateProvs.Visible=false;
				label5.Visible=false;
				label11.Visible=false;
			}
			comboOpType.Items.AddDefNone(); //Add none so can clear the Operatory Type
			comboOpType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.OperatoryTypes,true));
			comboOpType.SetSelectedDefNum(_operatory.OperatoryType);
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick=new FrmProviderPick(comboProv.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboProv.SetSelectedProvNum(frmProviderPick.ProvNumSelected);
		}

		private void butPickHyg_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick=new FrmProviderPick(comboHyg.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=comboHyg.GetSelectedProvNum();
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboHyg.SetSelectedProvNum(frmProviderPick.ProvNumSelected);
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillCombosProv();
		}

		private void butWSNPAPickApptTypes_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.WebSchedNewPatApptTypes,_listDefsWSNPAOperatory);
			formDefinitionPicker.IsMultiSelectionMode=true;
			if(formDefinitionPicker.ShowDialog()==DialogResult.OK) {
				_listDefsWSNPAOperatory=formDefinitionPicker.ListDefsSelected.Select(x => x.Copy()).ToList();
				textWSNPAApptTypes.Text=string.Join(", ",_listDefsWSNPAOperatory.Select(x => x.ItemName));
			}
		}

		private void butWSEPPickApptTypes_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.WebSchedExistingApptTypes,_listDefsWSEPOperatory);
			formDefinitionPicker.IsMultiSelectionMode=true;
			if(formDefinitionPicker.ShowDialog()==DialogResult.OK) {
				_listDefsWSEPOperatory=formDefinitionPicker.ListDefsSelected.Select(x => x.Copy()).ToList();
				textWSEPApptTypes.Text=string.Join(", ",_listDefsWSEPOperatory.Select(x => x.ItemName));
			}
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillCombosProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.ClinicNumSelected));
			comboProv.SetSelectedProvNum(provNum);
			provNum=comboHyg.GetSelectedProvNum();
			comboHyg.Items.Clear();
			comboHyg.Items.AddProvNone();
			comboHyg.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.ClinicNumSelected));
			comboHyg.SetSelectedProvNum(provNum);
		}

		private void ButUpdateProvs_Click(object sender, EventArgs e){
			if(IsNew){
				MsgBox.Show(this,"Not for new operatories.");
				return;
			}
			//Check against cache. Instead of saving changes, make them get out and reopen. Safer and simpler.
			Operatory operatory=Operatories.GetOperatory(_operatory.OperatoryNum);
			if(operatory.OpName!=textOpName.Text
				|| operatory.Abbrev!=textAbbrev.Text
				|| operatory.IsHidden!=checkIsHidden.Checked
				|| operatory.ClinicNum!=comboClinic.ClinicNumSelected
				|| operatory.ProvDentist!=comboProv.GetSelectedProvNum()
				|| operatory.ProvHygienist!=comboHyg.GetSelectedProvNum()
				|| operatory.IsHygiene!=checkIsHygiene.Checked
				|| operatory.SetProspective!=checkSetProspective.Checked
				|| operatory.IsWebSched!=checkIsWebSched.Checked)
			{
				MsgBox.Show(this,"Changes were detected above.  Save all changes, get completely out of the operatories window, and then re-enter.");
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.Setup) || !Security.IsAuthorized(EnumPermType.AppointmentEdit)) {
				return;
			}
			//Operatory operatory=Operatories.GetOperatory(contrApptPanel.OpNumClicked);
			if(Security.CurUser.ClinicIsRestricted && !Clinics.GetForUserod(Security.CurUser).Exists(x => x.ClinicNum==_operatory.ClinicNum)) {
				MsgBox.Show(this,"You are restricted from accessing the clinic belonging to the selected operatory.  No changes will be made.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,
				"WARNING: We recommend backing up your database before running this tool.  "
				+"This tool may take a very long time to run and should be run after hours.  "
				+"In addition, this tool could potentially change hundreds of appointments.  "
				+"The changes made by this tool can only be manually reversed.  "
				+"Are you sure you want to continue?"))
			{
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.Setup,0,Lan.g(this,"Update Provs on Future Appts tool run on operatory ")+_operatory.Abbrev+".");
			List<Appointment> listAppointments=Appointments.GetAppointmentsForOpsByPeriod(new List<long>() {_operatory.OperatoryNum},DateTime.Now);//no end date, so all future
			List<Appointment> listAppointmentsOld=new List<Appointment>();
			for(int i=0;i<listAppointments.Count;i++) {
				listAppointmentsOld.Add(listAppointments[i].Copy());
			}
			ControlApptRef.MoveAppointments(listAppointments,listAppointmentsOld,_operatory);
			MsgBox.Show(this,"Done");
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textOpName.Text==""){
				MsgBox.Show(this,"Operatory name cannot be blank.");
				return;
			}
			if(checkIsHidden.Checked==true && Operatories.HasFutureApts(_operatory.OperatoryNum,ApptStatus.UnschedList)) {
				MsgBox.Show(this,"Operatory cannot be hidden if there are future appointments.");
				checkIsHidden.Checked=false;
				return;
			}
			_operatory.OpName=textOpName.Text;
			_operatory.OperatoryType=comboOpType.GetSelectedDefNum();
			_operatory.Abbrev=textAbbrev.Text;
			_operatory.IsHidden=checkIsHidden.Checked;
			_operatory.ClinicNum=comboClinic.ClinicNumSelected;
			_operatory.ProvDentist=comboProv.GetSelectedProvNum();
			_operatory.ProvHygienist=comboHyg.GetSelectedProvNum();
			_operatory.IsHygiene=checkIsHygiene.Checked;
			_operatory.SetProspective=checkSetProspective.Checked;
			_operatory.IsWebSched=checkIsWebSched.Checked;
			_operatory.ListWSNPAOperatoryDefNums=_listDefsWSNPAOperatory.Select(x => x.DefNum).ToList();
			_operatory.ListWSEPOperatoryDefNums=_listDefsWSEPOperatory.Select(x => x.DefNum).ToList();
			if(IsNew) {
				ListOperatories.Insert(_operatory.ItemOrder,_operatory);//Insert into list at appropriate spot
				for(int i=0;i<ListOperatories.Count;i++) {
					ListOperatories[i].ItemOrder=i;//reset/correct item orders
				}
			}
			DialogResult=DialogResult.OK;
		}

	}
}