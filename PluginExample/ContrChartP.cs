using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental;
using OpenDentBusiness;
using OpenDental.UI;

namespace PluginExample {
	//This doesn't really need to be a user control unless we want to stash our own controls here for UI.
	public partial class ContrChartP:UserControl {

		public ContrChartP() {
			InitializeComponent();
		}

		public static void FillPtInfo(OpenDental.ControlChart sender,Patient patient) {
			//first, get all the objects we need. Because they are by ref, the original gets altered.
			GridOD gridPtInfo=(GridOD)sender.Controls.Find("gridPtInfo",true)[0];
			OpenDental.UI.TabControl tabControlImages=(OpenDental.UI.TabControl)sender.Controls.Find("tabControlImages",true)[0];
			ODtextBox textTreatmentNotes=(ODtextBox)sender.Controls.Find("textTreatmentNotes",true)[0];
			//bool TreatmentNoteChanged=sender.TreatmentNoteChanged;//might have to do a by ref here?
			//Then get some data that we need
			Family family=null;
			PatientNote patientNote=null;
			List<PatPlan> listPatPlans=null;
			List<InsSub> listInsSubs=null;
			List<InsPlan> listInsPlans=null;
			if(patient!=null) {
				family=Patients.GetFamily(patient.PatNum);
				patientNote=PatientNotes.Refresh(patient.PatNum,patient.Guarantor);
				listPatPlans=PatPlans.Refresh(patient.PatNum);
				listInsSubs=InsSubs.RefreshForFam(family);
				listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			}
			//Then, continue with the slightly altered original method.
			gridPtInfo.Height=tabControlImages.Top-gridPtInfo.Top;
			textTreatmentNotes.Text="";
			if(patient!=null) {
				textTreatmentNotes.Text=patientNote.Treatment;
				textTreatmentNotes.Enabled=true;
				textTreatmentNotes.Select(textTreatmentNotes.Text.Length+2,1);
				textTreatmentNotes.ScrollToCaret();
				sender.IsTreatmentNoteChanged=false;
			}
			gridPtInfo.BeginUpdate();
			gridPtInfo.Columns.Clear();
			GridColumn col=new GridColumn("",100);//Lan.g("TableChartPtInfo",""),);
			gridPtInfo.Columns.Add(col);
			col=new GridColumn("",300);
			gridPtInfo.Columns.Add(col);
			gridPtInfo.ListGridRows.Clear();
			if(patient==null) {
				gridPtInfo.EndUpdate();
				return;
			}
			GridRow row;
			//Age
			row=new GridRow();
			row.Cells.Add("Age");
			row.Cells.Add(PatientLogic.DateToAgeString(patient.Birthdate));
			row.Tag=null;
			gridPtInfo.ListGridRows.Add(row);
			//Credit type
			row=new GridRow();
			row.Cells.Add(Lan.g("TableChartPtInfo","ABC0"));
			row.Cells.Add(patient.CreditType);
			row.Tag=null;
			gridPtInfo.ListGridRows.Add(row);
			//Billing type
			row=new GridRow();
			row.Cells.Add(Lan.g("TableChartPtInfo","Billing Type"));
			row.Cells.Add(Defs.GetName(DefCat.BillingTypes,patient.BillingType));
			row.Tag=null;
			gridPtInfo.ListGridRows.Add(row);
			//Referrals
			List<RefAttach> listRefAttaches=RefAttaches.Refresh(patient.PatNum);
			for(int i=0;i<listRefAttaches.Count;i++) {
				row=new GridRow();
				row.ColorBackG=Color.Aquamarine;
				if(listRefAttaches[i].RefType==ReferralType.RefFrom) {
					row.Cells.Add("Referred From");
				}
				else {
					row.Cells.Add("Referred To");
				}
				row.Cells.Add(Referrals.GetNameLF(listRefAttaches[i].ReferralNum));
				row.Tag=null;
				gridPtInfo.ListGridRows.Add(row);
			}
			//Date First Visit
			row=new GridRow();
			row.Cells.Add(Lan.g("TableChartPtInfo","Date First Visit"));
			if(patient.DateFirstVisit.Year<1880)
				row.Cells.Add("??");
			else if(patient.DateFirstVisit==DateTime.Today)
				row.Cells.Add(Lan.g("TableChartPtInfo","NEW PAT"));
			else
				row.Cells.Add(patient.DateFirstVisit.ToShortDateString());
			row.Tag=null;
			gridPtInfo.ListGridRows.Add(row);
			//Prov - Pri & Sec
			row = new GridRow();
			row.Cells.Add(Lan.g("TableChartPtInfo","Prov. (Pri, Sec)"));
			if(patient.SecProv != 0)
				row.Cells.Add(Providers.GetAbbr(patient.PriProv) + ", " + Providers.GetAbbr(patient.SecProv));
			else
				row.Cells.Add(Providers.GetAbbr(patient.PriProv) + ", " + "None");
			row.Tag = null;
			gridPtInfo.ListGridRows.Add(row);
			//PriIns
			row=new GridRow();
			row.Cells.Add(Lan.g("TableChartPtInfo","Pri Ins"));
			string name;
			if(listPatPlans.Count>0) {
				name=InsPlans.GetCarrierName(InsSubs.GetOne(listPatPlans[0].InsSubNum).PlanNum,listInsPlans);
				if(listPatPlans[0].IsPending)
					name+=Lan.g("TableChartPtInfo"," (pending)");
				row.Cells.Add(name);
			}
			else {
				row.Cells.Add("");
			}
			row.Tag=null;
			gridPtInfo.ListGridRows.Add(row);
			//SecIns
			row=new GridRow();
			row.Cells.Add(Lan.g("TableChartPtInfo","Sec Ins"));
			if(listPatPlans.Count>1) {
				name=InsPlans.GetCarrierName(InsSubs.GetOne(listPatPlans[1].InsSubNum).PlanNum,listInsPlans);
				if(listPatPlans[1].IsPending)
					name+=Lan.g("TableChartPtInfo"," (pending)");
				row.Cells.Add(name);
			}
			else {
				row.Cells.Add("");
			}
			row.Tag=null;
			gridPtInfo.ListGridRows.Add(row);
			//Registration keys-------------------------------------------------------------------------------------------
			if(PrefC.GetBool(PrefName.DistributorKey)) {
				RegistrationKey[] registrationKeyArray=RegistrationKeys.GetForPatient(patient.PatNum);
				for(int i=0;i<registrationKeyArray.Length;i++) {
					row=new GridRow();
					row.Cells.Add(Lan.g("TableChartPtInfo","Registration Key"));
					string str=registrationKeyArray[i].RegKey.Substring(0,4)+"-"+registrationKeyArray[i].RegKey.Substring(4,4)+"-"+
						registrationKeyArray[i].RegKey.Substring(8,4)+"-"+registrationKeyArray[i].RegKey.Substring(12,4);
					if(registrationKeyArray[i].IsForeign) {
						str+="\r\nForeign";
					}
					else {
						str+="\r\nUSA";
					}
					str+="\r\nStarted: "+registrationKeyArray[i].DateStarted.ToShortDateString();
					if(registrationKeyArray[i].DateDisabled.Year>1880) {
						str+="\r\nDisabled: "+registrationKeyArray[i].DateDisabled.ToShortDateString();
					}
					if(registrationKeyArray[i].DateEnded.Year>1880) {
						str+="\r\nEnded: "+registrationKeyArray[i].DateEnded.ToShortDateString();
					}
					if(registrationKeyArray[i].Note!="") {
						str+=registrationKeyArray[i].Note;
					}
					row.Cells.Add(str);
					row.Tag=registrationKeyArray[i].Copy();
					gridPtInfo.ListGridRows.Add(row);
				}
			}
			GridCell cell;
			//medical fields-----------------------------------------------------------------
			bool showMed=true;
			if(Programs.IsEnabled(ProgramName.eClinicalWorks) && ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"IsStandalone")=="0") {
				showMed=false;
			}
			if(showMed) {
				//Get the Chart Module Medical color from the MiscColors category.
				Color colorChartModuleMedical=Defs.GetDefsForCategory(DefCat.MiscColors)[3].ItemColor;
				//premed flag.
				if(patient.Premed) {
					row=new GridRow();
					row.Cells.Add("");
					cell=new GridCell();
					cell.Text=Lan.g("TableChartPtInfo","Premedicate");
					cell.ColorText=Color.Red;
					cell.Bold=YN.Yes;
					row.Cells.Add(cell);
					row.ColorBackG=colorChartModuleMedical;
					row.Tag="med";
					gridPtInfo.ListGridRows.Add(row);
				}
				//diseases
				List<Disease> listDiseases=Diseases.Refresh(patient.PatNum);
				row=new GridRow();
				cell=new GridCell(Lan.g("TableChartPtInfo","Diseases"));
				cell.Bold=YN.Yes;
				row.Cells.Add(cell);
				if(listDiseases.Count>0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("TableChartPtInfo","none"));
				}
				row.ColorBackG=colorChartModuleMedical;
				row.Tag="med";
				gridPtInfo.ListGridRows.Add(row);
				for(int i=0;i<listDiseases.Count;i++) {
					row=new GridRow();
					cell=new GridCell(DiseaseDefs.GetName(listDiseases[i].DiseaseDefNum));
					cell.ColorText=Color.Red;
					cell.Bold=YN.Yes;
					row.Cells.Add(cell);
					row.Cells.Add(listDiseases[i].PatNote);
					row.ColorBackG=colorChartModuleMedical;
					row.Tag="med";
					gridPtInfo.ListGridRows.Add(row);
				}
				//MedUrgNote 
				row=new GridRow();
				row.Cells.Add(Lan.g("TableChartPtInfo","Med Urgent"));
				cell=new GridCell();
				cell.Text=patient.MedUrgNote;
				cell.ColorText=Color.Red;
				cell.Bold=YN.Yes;
				row.Cells.Add(cell);
				row.ColorBackG=colorChartModuleMedical;
				row.Tag="med";
				gridPtInfo.ListGridRows.Add(row);
				//Medical
				row=new GridRow();
				row.Cells.Add(Lan.g("TableChartPtInfo","Medical Summary"));
				row.Cells.Add(patientNote.Medical);
				row.ColorBackG=colorChartModuleMedical;
				row.Tag="med";
				gridPtInfo.ListGridRows.Add(row);
				//Service
				row=new GridRow();
				row.Cells.Add(Lan.g("TableChartPtInfo","Service Notes"));
				row.Cells.Add(patientNote.Service);
				row.ColorBackG=colorChartModuleMedical;
				row.Tag="med";
				gridPtInfo.ListGridRows.Add(row);
				//medications
				Medications.RefreshCache();
				List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patient.PatNum,false);
				row=new GridRow();
				cell=new GridCell(Lan.g("TableChartPtInfo","Medications"));
				cell.Bold=YN.Yes;
				row.Cells.Add(cell);
				if(listMedicationPats.Count>0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Lan.g("TableChartPtInfo","none"));
				}
				row.ColorBackG=colorChartModuleMedical;
				row.Tag="med";
				gridPtInfo.ListGridRows.Add(row);
				string text;
				Medication medication;
				for(int i=0;i<listMedicationPats.Count;i++) {
					row=new GridRow();
					medication=Medications.GetMedication(listMedicationPats[i].MedicationNum);
					text=medication.MedName;
					if(medication.MedicationNum != medication.GenericNum) {
						text+="("+Medications.GetMedication(medication.GenericNum).MedName+")";
					}
					row.Cells.Add(text);
					text=listMedicationPats[i].PatNote
						+"("+Medications.GetGeneric(listMedicationPats[i].MedicationNum).Notes+")";
					row.Cells.Add(text);
					row.ColorBackG=colorChartModuleMedical;
					row.Tag="med";
					gridPtInfo.ListGridRows.Add(row);
				}
			}//if !eCW.enabled
			gridPtInfo.EndUpdate();
		}



	}
}
