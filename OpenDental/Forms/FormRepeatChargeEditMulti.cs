using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormRepeatChargeEditMulti:FormODBase {
		private List<RepeatCharge> _listRepeatCharges;
		private int _isEnabledCount;
		private int _isDisabledCount;

		public FormRepeatChargeEditMulti() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRepeatChargeEditMulti_Load(object sender,EventArgs e) {
			comboIsEnabled.Items.Add(Lan.g(this,"Enabled"));
			comboIsEnabled.Items.Add(Lan.g(this,"Disabled"));
			comboIsEnabled.Items.Add(Lan.g(this,"Both"));
			comboIsEnabled.SelectedIndex=0;//Default to Enabled.
		}

		private void butRun_Click(object sender,EventArgs e) {
			//Validate input.
			if(textProcCode.Text.IsNullOrEmpty() || !ProcedureCodes.IsValidCode(textProcCode.Text)) {
				MsgBox.Show(this,"Please enter a valid Procedure Code.");
				return;
			}
			if(textChargeAmount.Text.IsNullOrEmpty() || textChargeAmountNew.Text.IsNullOrEmpty()) {
				MsgBox.Show(this,"Please enter both Charge Amounts.");
				return;
			}
			if(PIn.Double(textChargeAmount.Text)<0 || PIn.Double(textChargeAmountNew.Text)<0) {
				MsgBox.Show(this,"Please enter a Charge Amount greater than zero.");
				return;
			}
			if(PIn.Double(textChargeAmount.Text)==PIn.Double(textChargeAmountNew.Text)) {
				MsgBox.Show(this,"Current Charge Amount and New Charge Amount cannot be the same.");
				return;
			}
			long patNumSuperFamily=PIn.Long(textPatNumSuperFamilyHead.Text);
			Patient patientSuperFamilyHead=Patients.GetPat(patNumSuperFamily);
			if(patNumSuperFamily!=0 && patientSuperFamilyHead==null) {
				MsgBox.Show(this,"Please enter a valid PatNum.");
				return;
			}
			string procCode=PIn.String(textProcCode.Text);
			double chargeAmount=PIn.Double(textChargeAmount.Text);
			DateTime dateStart=PIn.Date(textDateStart.Text);
			//A list of repeat charges, both IsEnabled and !IsEnabled that match the search criteria.
			List<RepeatCharge> listRepeatChargesBoth=RepeatCharges.GetRepeatChargesMulti(patNumSuperFamily,procCode,chargeAmount,dateStart);
			//A list of repeat charges, !IsEnabled.
			List<RepeatCharge> listRepeatChargesDisabled=listRepeatChargesBoth.FindAll(x=>!x.IsEnabled);
			//A list of repeat charges, IsEnabled.
			List<RepeatCharge> listRepeatChargesIsEnabled=listRepeatChargesBoth.FindAll(x=>x.IsEnabled);
			if(comboIsEnabled.SelectedIndex==0) {//Enabled
				UpdateRepeatCharge(listRepeatChargesIsEnabled);
			}
			if(comboIsEnabled.SelectedIndex==1) {//Disabled
				UpdateRepeatCharge(listRepeatChargesDisabled);
			}
			if(comboIsEnabled.SelectedIndex==2) {//Both
				UpdateRepeatCharge(listRepeatChargesBoth);
			}
		}

		private void UpdateRepeatCharge(List<RepeatCharge> listRepeatCharges) {
			_listRepeatCharges=listRepeatCharges;
			if(_listRepeatCharges.Count==0) {
				MsgBox.Show(this,"There are no repeat charges to update for given criteria.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This cannot be undone. Do you wish to continue?")) {
				return;
			}
			//Create a commlog, securitylog and update the charge amount for each patient in the list.
			for(int i=0;i<_listRepeatCharges.Count;i++) {
				CreateLogs(_listRepeatCharges[i]);
				_listRepeatCharges[i].ChargeAmt=PIn.Double(textChargeAmountNew.Text);
				RepeatCharges.Update(_listRepeatCharges[i]);
			}
			WriteToDesktop();
		}

		private void CreateLogs(RepeatCharge repeatChargeOld) {
			Commlog commlogRepeatChargeMulti=new Commlog();
			commlogRepeatChargeMulti.PatNum=repeatChargeOld.PatNum;
			commlogRepeatChargeMulti.CommDateTime=DateTime.Now;
			commlogRepeatChargeMulti.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.FIN);
			commlogRepeatChargeMulti.Mode_=CommItemMode.None;
			commlogRepeatChargeMulti.UserNum=Security.CurUser.UserNum;
			commlogRepeatChargeMulti.Note="From Multi Repeat Charge Edit: \nProcCode: "+textProcCode.Text+
				", Old Charge Amount: "+textChargeAmount.Text+", New Charge Amount: "+textChargeAmountNew.Text;
			Commlogs.Insert(commlogRepeatChargeMulti);
			RepeatCharge repeatChargeNew=repeatChargeOld.Copy();
			repeatChargeNew.ChargeAmt=PIn.Double(textChargeAmountNew.Text);
			Patient patient=Patients.GetPat(repeatChargeOld.PatNum);
			RepeatCharges.InsertRepeatChargeChangeSecurityLogEntry(repeatChargeOld,EnumPermType.RepeatChargeUpdate,patient,repeatChargeNew);
		}

		private void WriteToDesktop() {
			//Values for output file.
			string procCode=textProcCode.Text;
			string chargeAmount=textChargeAmount.Text;
			string chargeAmountNew=textChargeAmountNew.Text;
			long patNumSuperFamily=PIn.Long(textPatNumSuperFamilyHead.Text);
			DateTime dateStart=PIn.Date(textDateStart.Text);
			//Append today's date to the output filename.
			string dateTime=DateTime.Now.ToString("MM-dd-yy");
			//Write file to desktop:
			string path=Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\RepeatCharges_"+dateTime+".txt";
			using StreamWriter streamWriter=new StreamWriter(path, append: true);//Append=true in case we run this more than once on the same day.
			streamWriter.WriteLine("Search Criteria: ");
			streamWriter.WriteLine("ProcCode:\t\t"+procCode);
			streamWriter.WriteLine("Current Charge Amount:\t"+chargeAmount);
			streamWriter.WriteLine("New Charge Amount:\t"+chargeAmountNew);
			if(patNumSuperFamily==0) {
				streamWriter.WriteLine("Super Family PatNum:\tAll");
			}
			else {
				streamWriter.WriteLine("Super Family PatNum:\t"+patNumSuperFamily);
			}
			if(textDateStart.Text.IsNullOrEmpty()) {
				streamWriter.WriteLine("Start Date:\t\tAll");
			}
			else {
				streamWriter.WriteLine("Start Date:\t\t"+dateStart.ToString("MM-dd-yy"));
			}
			streamWriter.WriteLine("Is Enabled:\t\t"+comboIsEnabled.SelectedItem.ToString());
			streamWriter.WriteLine("Number of Repeat Charges updated: "+_listRepeatCharges.Count);
			streamWriter.Write("\n");
			for(int i=0;i<_listRepeatCharges.Count;i++) {
				if(_listRepeatCharges[i].IsEnabled) {
					_isEnabledCount++;
				}
				if(!_listRepeatCharges[i].IsEnabled) {
					_isDisabledCount++;
				}
				Patient patient=Patients.GetPat(_listRepeatCharges[i].PatNum);
				if(i!=0) {
					streamWriter.Write("\n");
				}
				streamWriter.Write("PatNum: "+_listRepeatCharges[i].PatNum+", ");
				streamWriter.Write("Is Enabled: "+_listRepeatCharges[i].IsEnabled+",\t");
				streamWriter.Write("Patient: "+patient.GetNameLF().ToString());
			}
			streamWriter.WriteLine("\n\nTotal Enabled Updated: "+_isEnabledCount+"\tTotal Disabled Updated: "+_isDisabledCount);
			streamWriter.Write("\n");
			streamWriter.Close();
			_isEnabledCount=0;
			_isDisabledCount=0;
			MsgBox.Show(this,_listRepeatCharges.Count+" repeat charges were updated.\n" +
				"RepeatCharges_"+dateTime+".txt was saved to your desktop.");
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}