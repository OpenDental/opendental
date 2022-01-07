using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProcEditExplain:FormODBase {
		public static string Changes;
		public static string Explanation;
		public bool dpcChange;
		private bool radioChange;
		private string radioText;

		public FormProcEditExplain() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			textSummary.Text=Changes;
		}

		private void FormProcEditExplain_Load(object sender,EventArgs e) {
			textSummary.Text=Changes;
			if(dpcChange) {
				groupBoxDPC.Enabled=true;
			}
		}

		public static string GetChanges(Procedure procCur, Procedure procOld, OrionProc orionProcCur, OrionProc orionProcOld){
			Changes="";
			if(orionProcOld.DPC != orionProcCur.DPC) {
				if(Changes!="") { Changes+="\r\n"; }
				Changes+="DPC changed from "+POut.String(orionProcOld.DPC.ToString())+" to "+POut.String(orionProcCur.DPC.ToString())+".";
			}
			if(orionProcOld.DPCpost != orionProcCur.DPCpost) {
				if(Changes!=""){ Changes+="\r\n";}
		    Changes+="DPC Post Visit changed from "+POut.String(orionProcOld.DPCpost.ToString())+" to "+POut.String(orionProcCur.DPCpost.ToString())+".";
			}
			//PatNum, AptNum, PlannedAptNum should never change---------------------------------------------------------------------------------------------
			if(procOld.PatNum != procCur.PatNum) {
				if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Patient Num changed from "+procOld.PatNum+" to "+procCur.PatNum+".";
		  }
		  if(procOld.AptNum != procCur.AptNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Apt Num changed from "+procOld.AptNum+" to "+procCur.AptNum+".";
		  }
		  if(procOld.PlannedAptNum != procCur.PlannedAptNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Planned Apt Num changed from "+procOld.PlannedAptNum+" to "+procCur.PlannedAptNum+".";
			}
			//Date and time related fields------------------------------------------------------------------------------------------------------------------
		  if(procOld.DateEntryC.Date != procCur.DateEntryC.Date) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Date Entry changed from "+procOld.DateEntryC.ToShortDateString()+" to "+procCur.DateEntryC.ToShortDateString()+".";
		  }
		  if(procOld.ProcDate.Date != procCur.ProcDate.Date) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Proc Date changed from "+procOld.ProcDate.ToShortDateString()+" to "+procCur.ProcDate.ToShortDateString()+".";
		  }
		  //if(procOld.StartTime != procCur.StartTime) {
		  //  if(Changes!=""){ Changes+="\r\n";}
		  //  Changes+="Start Time changed from "+procOld.StartTime+" to "+procCur.StartTime+".";
		  //}
		  //if(procOld.StopTime != procCur.StopTime) {
		  //  if(Changes!=""){ Changes+="\r\n";}
		  //  Changes+="Stop Time changed from "+procOld.StopTime+" to "+procCur.StopTime+".";
		  //}
		  if(procOld.ProcTime != procCur.ProcTime) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Procedure Time changed from "
					+(PIn.DateT(procOld.ProcTime.ToString()).ToShortTimeString()=="12:00 AM"?"none":PIn.DateT(procOld.ProcTime.ToString()).ToShortTimeString())
					+" to "+(PIn.DateT(procCur.ProcTime.ToString()).ToShortTimeString()=="12:00 AM"?"none":PIn.DateT(procCur.ProcTime.ToString()).ToShortTimeString())+".";
		  }
		  if(procOld.ProcTimeEnd != procCur.ProcTimeEnd) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Procedure End Time changed from "
					+(PIn.DateT(procOld.ProcTimeEnd.ToString()).ToShortTimeString()=="12:00 AM"?"none":PIn.DateT(procOld.ProcTimeEnd.ToString()).ToShortTimeString())
					+" to "+(PIn.DateT(procCur.ProcTimeEnd.ToString()).ToShortTimeString()=="12:00 AM"?"none":PIn.DateT(procCur.ProcTimeEnd.ToString()).ToShortTimeString())+".";
		  }
			//Procedure, related areas, amount, hide graphics, etc.-----------------------------------------------------------------------------------------
		  if(procOld.CodeNum != procCur.CodeNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Procedure changed from "+ProcedureCodes.GetLaymanTerm(procOld.CodeNum)+" to "+ProcedureCodes.GetLaymanTerm(procCur.CodeNum)+".";
		  }
		  if(procOld.ProcFee != procCur.ProcFee) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Proc Fee changed from $"+procOld.ProcFee.ToString("F")+" to $"+procCur.ProcFee.ToString("F")+".";
		  }
		  if(procOld.ToothNum != procCur.ToothNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Tooth Num changed from "+procOld.ToothNum+" to "+procCur.ToothNum+".";
		  }
		  if(procOld.Surf != procCur.Surf) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Surface changed from "+procOld.Surf+" to "+procCur.Surf+".";
		  }
		  if(procOld.ToothRange != procCur.ToothRange) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Tooth Range changed from "+procOld.ToothRange+" to "+procCur.ToothRange+".";
		  }
			if(procOld.HideGraphics != procCur.HideGraphics) {
				if(Changes!=""){ Changes+="\r\n";}
				Changes+="Hide Graphics changed from "+(procOld.HideGraphics?"Hide Graphics":"Do Not Hide Graphics")
					+" to "+(procCur.HideGraphics?"Hide Graphics":"Do Not Hide Graphics")+".";
			}
			//Provider, Diagnosis, Priority, Place of Service, Clinic, Site---------------------------------------------------------------------------------
		  if(procOld.ProvNum != procCur.ProvNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Provider changed from "+Providers.GetAbbr(procOld.ProvNum)+" to "+Providers.GetAbbr(procCur.ProvNum)+".";
		  }
		  if(procOld.Dx != procCur.Dx) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Diagnosis changed from "+Defs.GetDef(DefCat.Diagnosis,procOld.Dx).ItemName
					+" to "+Defs.GetDef(DefCat.Diagnosis,procCur.Dx).ItemName+".";
		  }
		  if(procOld.Priority != procCur.Priority) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Priority changed from "+((procOld.Priority!=0)?Defs.GetDef(DefCat.TxPriorities,procOld.Priority).ItemName:"no priority")
					+" to "+((procCur.Priority!=0)?Defs.GetDef(DefCat.TxPriorities,procCur.Priority).ItemName:"no priority")+".";
		  }
		  if(procOld.PlaceService != procCur.PlaceService) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Place of Service changed from "+procOld.PlaceService.ToString()+" to "+procCur.PlaceService.ToString()+".";
		  }
		  if(procOld.ClinicNum != procCur.ClinicNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Clinic changed from "+Clinics.GetAbbr(procOld.ClinicNum)+" to "+Clinics.GetAbbr(procCur.ClinicNum)+".";
		  }
		  if(procOld.SiteNum != procCur.SiteNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Site changed from "+(procOld.SiteNum==0?"none":Sites.GetDescription(procOld.SiteNum))
					+" to "+(procCur.SiteNum==0?"none":Sites.GetDescription(procCur.SiteNum))+".";
		  }
			//Prosthesis reverse lookup---------------------------------------------------------------------------------------------------------------------
		  if(procOld.Prosthesis != procCur.Prosthesis) {
		    if(Changes!=""){ Changes+="\r\n";}
				string prosthesisOld;
				switch(procOld.Prosthesis.ToString()){
					case "": prosthesisOld="no"; break;
					case "I":	prosthesisOld="Initial"; break;
					case "R": prosthesisOld="Replacement"; break;
					default: prosthesisOld="error"; break;
				}
				string prosthesisCur;
				switch(procCur.Prosthesis.ToString()){
					case "": prosthesisCur="no"; break;
					case "I": prosthesisCur="Initial"; break;
					case "R": prosthesisCur="Replacement"; break;
					default: prosthesisCur="error"; break;
				}
		    Changes+="Prosthesis changed from "+prosthesisOld+" to "+prosthesisCur+".";
		  }
		  if(procOld.DateOriginalProsth.Date != procCur.DateOriginalProsth.Date) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Date of Original Prosthesis changed from "+procOld.DateOriginalProsth.ToShortDateString()
					+" to "+procCur.DateOriginalProsth.ToShortDateString()+".";
		  }
			//Claim Note & Orion Proc Fields----------------------------------------------------------------------------------------------------------------
		  if(procOld.ClaimNote != procCur.ClaimNote) {
		    if(Changes!=""){ Changes+="\r\n";}
				Changes+="Claim Note changed from "+(procOld.ClaimNote==""?"none":"'"+procOld.ClaimNote+"'")
					+" to "+(procCur.ClaimNote==""?"none":"'"+procCur.ClaimNote+"'");
		  }
			if(orionProcOld.OrionProcNum != orionProcCur.OrionProcNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Orion Proc Num changed from "+POut.Long(orionProcOld.OrionProcNum)+" to "+POut.Long(orionProcCur.OrionProcNum)+".";
		  }
			if(orionProcOld.ProcNum != orionProcCur.ProcNum) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Proc Num changed from "+POut.Long(orionProcOld.ProcNum)+" to "+POut.Long(orionProcCur.ProcNum)+".";
		  }
			//Orion Status Reverse Lookup for Description----------------------------------//None is equivalent to TP---------------------------------------
			if(orionProcOld.Status2 != orionProcCur.Status2 && !(orionProcOld.Status2==OrionStatus.None && orionProcCur.Status2==OrionStatus.TP)) {
		    if(Changes!=""){ Changes+="\r\n";}
				string[] status2=new string[2];
				string[] status2Desc=new string[2];
				status2[0]=orionProcOld.Status2.ToString();
				status2[1]=orionProcCur.Status2.ToString();
				for(int i=0;i<2;i++){
					switch(status2[i]){
						case "None":		status2Desc[i]="TP-treatment planned"; break;
						case "TP":			status2Desc[i]="TP-treatment planned"; break;
						case "C":				status2Desc[i]="C-completed";	break;
						case "E":				status2Desc[i]="E-existing prior to incarceration"; break;
						case "R":				status2Desc[i]="R-refused treatment"; break;
						case "RO":			status2Desc[i]="RO-referred out to specialist"; break;
						case "CS":			status2Desc[i]="CS-completed by specialist"; break;
						case "CR":			status2Desc[i]="CR-completed by registry"; break;
						case "CA_Tx":		status2Desc[i]="CA_Tx-cancelled, tx plan changed"; break;
						case "CA_EPRD": status2Desc[i]="CA_EPRD-cancelled, eligible parole"; break;
						case "CA_P/D":	status2Desc[i]="CA_P/D--cancelled, parole/discharge"; break;
						case "S":				status2Desc[i]="S-suspended, unacceptable plaque"; break;
						case "ST":			status2Desc[i]="ST-stop clock, multi visit"; break;
						case "W":				status2Desc[i]="W-watch"; break;
						case "A":				status2Desc[i]="A-alternative"; break;
						default:				status2Desc[i]="error"; break;
					}
				}
		    Changes+="Orion Procedure Status changed from "+status2Desc[0]+" to "+status2Desc[1]+".";
		  }
			//Other orion fields----------------------------------------------------------------------------------------------------------------------------
			if(orionProcOld.DateScheduleBy.Date != orionProcCur.DateScheduleBy.Date) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Date Schedule By changed from "+orionProcOld.DateScheduleBy.ToShortDateString()
					+" to "+orionProcCur.DateScheduleBy.ToShortDateString()+".";
		  }
			if(orionProcOld.DateStopClock.Date != orionProcCur.DateStopClock.Date) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Date Stop Clock changed from "+orionProcOld.DateStopClock.ToShortDateString()
					+" to "+orionProcCur.DateStopClock.ToShortDateString()+".";
		  }
			if(orionProcOld.IsOnCall != orionProcCur.IsOnCall) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Is On Call changed from "+(orionProcOld.IsOnCall?"Is On Call":"Is Not On Call")
					+" to "+(orionProcCur.IsOnCall?"Is On Call":"Is Not On Call")+".";
		  }
			if(orionProcOld.IsEffectiveComm != orionProcCur.IsEffectiveComm) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Is Effective Comm changed from "+(orionProcOld.IsEffectiveComm?"Is an Effective Communicator":"Is Not an Effective Communicator")
					+" to "+(orionProcCur.IsEffectiveComm?"Is an Effective Communicator":"Is Not an Effective Communicator")+".";
		  }
			if(orionProcOld.IsRepair != orionProcCur.IsRepair) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Is Repair changed from "+(orionProcOld.IsRepair?"Is a Repair":"Is Not a Repair")
					+" to "+(orionProcCur.IsRepair?"Is a Repair":"Is Not a Repair")+".";
		  }
			//Medical fields--------------------------------------------------------------------------------------------------------------------------------
		  if(procOld.MedicalCode != procCur.MedicalCode) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Medical Code changed from "+(procOld.MedicalCode==""?"none":procOld.MedicalCode)
					+" to "+(procCur.MedicalCode==""?"none":procCur.MedicalCode)+".";
		  }
		  if(procOld.DiagnosticCode != procCur.DiagnosticCode) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Diagnostic Code changed from "+(procOld.DiagnosticCode==""?"none":procOld.DiagnosticCode)
					+" to "+(procCur.DiagnosticCode==""?"none":procCur.DiagnosticCode)+".";
			}
		  if(procOld.IsPrincDiag != procCur.IsPrincDiag) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Is Princ Diag changed from "+(procOld.IsPrincDiag?"Principal Diagnosis":"Not Principal Diagnosis")
					+" to "+(procCur.IsPrincDiag?"Principal Diagnosis":"Not Principal Diagnosis")+".";
		  }
		  //if(procOld.RevCode != procCur.RevCode) {
		  //  if(Changes!=""){ Changes+="\r\n";}
		  //  Changes+="Rev Code changed from "+POut.String(procOld.RevCode)+"' to '"+POut.String(procCur.RevCode)+".";
		  //}
			//Proc status and billing fields----------------------------------------------------------------------------------------------------------------
		  if(procOld.ProcStatus != procCur.ProcStatus) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Procedure Status changed from "+procOld.ProcStatus.ToString()+" to "+procCur.ProcStatus.ToString()+".";
		  }
		  if(procOld.DateTP.Date != procCur.DateTP.Date && procOld.DateTP.Date!=DateTime.MinValue.Date) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Date TP changed from "+procOld.DateTP.ToShortDateString()+" to "+procCur.DateTP.ToShortDateString()+".";
		  }
		  //if(procOld.BillingTypeOne != procCur.BillingTypeOne) {
		  //  if(Changes!=""){ Changes+="\r\n";}
		  //  Changes+="Billing Type One changed from "+(procOld.BillingTypeOne!=0?Defs.GetDef(DefCat.BillingTypes,procOld.BillingTypeOne).ItemName:"none")
			//		+" to "+(procCur.BillingTypeOne!=0?Defs.GetDef(DefCat.BillingTypes,procCur.BillingTypeOne).ItemName:"none")+".";
		  //}
		  //if(procOld.BillingTypeTwo != procCur.BillingTypeTwo) {
		  //  if(Changes!=""){ Changes+="\r\n";}
		  //  Changes+="Billing Type Two changed from "+(procOld.BillingTypeTwo!=0?Defs.GetDef(DefCat.BillingTypes,procOld.BillingTypeTwo).ItemName:"none")
			//		+" to "+(procCur.BillingTypeTwo!=0?Defs.GetDef(DefCat.BillingTypes,procCur.BillingTypeTwo).ItemName:"none")+".";
		  //}
		  if(procOld.ProcNumLab != procCur.ProcNumLab) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Proc Num Lab changed from "+POut.Long(procOld.ProcNumLab)+" to "+POut.Long(procCur.ProcNumLab)+".";
		  }
		  //if(procOld.UnitCode != procCur.UnitCode) {
		  //  if(Changes!=""){ Changes+="\r\n";}
		  //  Changes+="Unit Code changed from "+POut.String(procOld.UnitCode)+" to "+POut.String(procCur.UnitCode)+".";
		  //}
			//UnitQty, Canadian Type Codes, and Note--------------------------------------------------------------------------------------------------------
		  if(procOld.UnitQty != procCur.UnitQty) {
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Unit Quantity changed from "+POut.Int(procOld.UnitQty)+" to "+POut.Int(procCur.UnitQty)+".";
		  }
		  //if(procOld.CanadianTypeCodes != procCur.CanadianTypeCodes) {
		  //  if(Changes!=""){ Changes+="\r\n";}
		  //  Changes+="Canadian Code Type changed from "+POut.String(procOld.CanadianTypeCodes)+" to "+POut.String(procCur.CanadianTypeCodes)+".";
		 // }
			if(procOld.Note != procCur.Note && !(procOld.Note==null && procCur.Note=="")) {//Null note is equivalent to an empty note string.
		    if(Changes!=""){ Changes+="\r\n";}
		    Changes+="Note changed from "+(procOld.Note==""?"none":"'"+procOld.Note+"'")
					+" to "+(procCur.Note==""?"none":"'"+procCur.Note+"'");
		  }
			return Changes;
		}

		private void radioButtonError_CheckedChanged(object sender,EventArgs e) {
			radioChange=true;
			radioText="Entry error";
		}

		private void radioButtonNewProv_CheckedChanged(object sender,EventArgs e) {
			radioChange=true;
			radioText="New provider";
		}

		private void radioButtonReAssign_CheckedChanged(object sender,EventArgs e) {
			radioChange=true;
			radioText="Re-assignment";
		}

		private void radioButtonOther_CheckedChanged(object sender,EventArgs e) {
			radioChange=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(groupBoxDPC.Enabled){
				if(radioButtonOther.Checked	&& textExplanation.Text.Trim()=="") {
					MsgBox.Show(this,"Please explain why the DPC was changed.");
					return;
				}
			}
			else if(textExplanation.Text.Trim()==""){
				MsgBox.Show(this,"Please explain why the above changes were made.");
				return;
			}
			if(groupBoxDPC.Enabled && !radioChange) {
				MsgBox.Show(this,"Please select a reason for DPC change.");
				return;
			}
			Explanation="Summary of Changes Made:\r\n"+Changes+"\r\nExplanation:\r\n"+textExplanation.Text;
			if(radioChange) {
				if(!radioButtonOther.Checked) {
					if(textExplanation.Text.Trim()!="") {
						Explanation+="\r\n";//New line if user typed explanation for other things changed.
					}
					Explanation+="DPC change due to: "+radioText;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	






	}
}