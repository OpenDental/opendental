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
		private Operatory OpCur;
		public List<Operatory> ListOps;
		///<summary>All of the Web Sched New Pat Appt appointment type defs that this operatory is associated to.</summary>
		private List<Def> _listWSNPAOperatoryDefs=new List<Def>();
		private List<Def> _listWSEPOperatoryDefs=new List<Def>();

		///<summary>This reference is passed in because it's needed for the "Update Provs on Future Appts" tool.</summary>
		public ControlAppt ContrApptRef;

		///<summary></summary>
		public FormOperatoryEdit(Operatory opCur) {
			OpCur=opCur;
			ContrApptRef=null;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOperatoryEdit_Load(object sender, System.EventArgs e) {
			textOpName.Text=OpCur.OpName;
			textAbbrev.Text=OpCur.Abbrev;
			checkIsHidden.Checked=OpCur.IsHidden;
			comboClinic.SelectedClinicNum=OpCur.ClinicNum;//can be 0
			FillCombosProv();
			comboProv.SetSelectedProvNum(OpCur.ProvDentist);
			comboHyg.SetSelectedProvNum(OpCur.ProvHygienist);
			if(OpCur.ListWSNPAOperatoryDefNums!=null) {
				//This is an existing operatory with WSNPA appointment types associated.  Go get them in order to display to the user.
				_listWSNPAOperatoryDefs=Defs.GetDefs(DefCat.WebSchedNewPatApptTypes,OpCur.ListWSNPAOperatoryDefNums);
			}
			if(OpCur.ListWSEPOperatoryDefNums!=null) {
				_listWSEPOperatoryDefs=Defs.GetDefs(DefCat.WebSchedExistingApptTypes,OpCur.ListWSEPOperatoryDefNums);
			}
			textWSNPAApptTypes.Text=string.Join(", ",_listWSNPAOperatoryDefs.Select(x => x.ItemName));//WSNPA
			textWSEPApptTypes.Text=string.Join(", ",_listWSEPOperatoryDefs.Select(x => x.ItemName));//WSEP
			checkIsHygiene.Checked=OpCur.IsHygiene;
			checkSetProspective.Checked=OpCur.SetProspective;
			checkIsWebSched.Checked=OpCur.IsWebSched;
			if(ContrApptRef==null) {
				butUpdateProvs.Visible=false;
				label5.Visible=false;
				label11.Visible=false;
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP=new FormProviderPick(comboProv.Items.GetAll<Provider>());
			FormPP.SelectedProvNum=comboProv.GetSelectedProvNum();
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(FormPP.SelectedProvNum);
		}

		private void butPickHyg_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP=new FormProviderPick(comboHyg.Items.GetAll<Provider>());
			FormPP.SelectedProvNum=comboHyg.GetSelectedProvNum();
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboHyg.SetSelectedProvNum(FormPP.SelectedProvNum);
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillCombosProv();
		}

		private void butWSNPAPickApptTypes_Click(object sender,EventArgs e) {
			using FormDefinitionPicker FormDP=new FormDefinitionPicker(DefCat.WebSchedNewPatApptTypes,_listWSNPAOperatoryDefs);
			FormDP.IsMultiSelectionMode=true;
			if(FormDP.ShowDialog()==DialogResult.OK) {
				_listWSNPAOperatoryDefs=FormDP.ListDefsSelected.Select(x => x.Copy()).ToList();
				textWSNPAApptTypes.Text=string.Join(", ",_listWSNPAOperatoryDefs.Select(x => x.ItemName));
			}
		}

		private void butWSEPPickApptTypes_Click(object sender,EventArgs e) {
			using FormDefinitionPicker FormDP=new FormDefinitionPicker(DefCat.WebSchedExistingApptTypes,_listWSEPOperatoryDefs);
			FormDP.IsMultiSelectionMode=true;
			if(FormDP.ShowDialog()==DialogResult.OK) {
				_listWSEPOperatoryDefs=FormDP.ListDefsSelected.Select(x => x.Copy()).ToList();
				textWSEPApptTypes.Text=string.Join(", ",_listWSEPOperatoryDefs.Select(x => x.ItemName));
			}
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillCombosProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			comboProv.SetSelectedProvNum(provNum);
			provNum=comboHyg.GetSelectedProvNum();
			comboHyg.Items.Clear();
			comboHyg.Items.AddProvNone();
			comboHyg.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			comboHyg.SetSelectedProvNum(provNum);
		}

		private void ButUpdateProvs_Click(object sender, EventArgs e){
			if(IsNew){
				MsgBox.Show(this,"Not for new operatories.");
				return;
			}
			//Check against cache. Instead of saving changes, make them get out and reopen. Safer and simpler.
			Operatory op=Operatories.GetOperatory(OpCur.OperatoryNum);
			if(op.OpName!=textOpName.Text
				|| op.Abbrev!=textAbbrev.Text
				|| op.IsHidden!=checkIsHidden.Checked
				|| op.ClinicNum!=comboClinic.SelectedClinicNum
				|| op.ProvDentist!=comboProv.GetSelectedProvNum()
				|| op.ProvHygienist!=comboHyg.GetSelectedProvNum()
				|| op.IsHygiene!=checkIsHygiene.Checked
				|| op.SetProspective!=checkSetProspective.Checked
				|| op.IsWebSched!=checkIsWebSched.Checked)
			{
				MsgBox.Show(this,"Changes were detected above.  Save all changes, get completely out of the operatories window, and then re-enter.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.Setup) || !Security.IsAuthorized(Permissions.AppointmentEdit)) {
				return;
			}
			//Operatory operatory=Operatories.GetOperatory(contrApptPanel.OpNumClicked);
			if(Security.CurUser.ClinicIsRestricted && !Clinics.GetForUserod(Security.CurUser).Exists(x => x.ClinicNum==OpCur.ClinicNum)) {
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
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,Lan.g(this,"Update Provs on Future Appts tool run on operatory ")+OpCur.Abbrev+".");
			List<Appointment> listAppts=Appointments.GetAppointmentsForOpsByPeriod(new List<long>() {OpCur.OperatoryNum},DateTime.Now);//no end date, so all future
			List<Appointment> listApptsOld=new List<Appointment>();
			foreach(Appointment appt in listAppts) {
				listApptsOld.Add(appt.Copy());
			}
			ContrApptRef.MoveAppointments(listAppts,listApptsOld,OpCur);
			MsgBox.Show(this,"Done");
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textOpName.Text==""){
				MsgBox.Show(this,"Operatory name cannot be blank.");
				return;
			}
			if(checkIsHidden.Checked==true && Operatories.HasFutureApts(OpCur.OperatoryNum,ApptStatus.UnschedList)) {
				MsgBox.Show(this,"Operatory cannot be hidden if there are future appointments.");
				checkIsHidden.Checked=false;
				return;
			}
			OpCur.OpName=textOpName.Text;
			OpCur.Abbrev=textAbbrev.Text;
			OpCur.IsHidden=checkIsHidden.Checked;
			OpCur.ClinicNum=comboClinic.SelectedClinicNum;
			OpCur.ProvDentist=comboProv.GetSelectedProvNum();
			OpCur.ProvHygienist=comboHyg.GetSelectedProvNum();
			OpCur.IsHygiene=checkIsHygiene.Checked;
			OpCur.SetProspective=checkSetProspective.Checked;
			OpCur.IsWebSched=checkIsWebSched.Checked;
			OpCur.ListWSNPAOperatoryDefNums=_listWSNPAOperatoryDefs.Select(x => x.DefNum).ToList();
			OpCur.ListWSEPOperatoryDefNums=_listWSEPOperatoryDefs.Select(x => x.DefNum).ToList();
			if(IsNew) {
				ListOps.Insert(OpCur.ItemOrder,OpCur);//Insert into list at appropriate spot
				for(int i=0;i<ListOps.Count;i++) {
					ListOps[i].ItemOrder=i;//reset/correct item orders
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}

}





















