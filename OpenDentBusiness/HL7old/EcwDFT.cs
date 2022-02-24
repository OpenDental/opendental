using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace OpenDentBusiness.HL7 {
	///<summary>A DFT message is a Charge Specification.  There are different kinds.  The kind we have implemented passes information about completed procedures and their charges to external programs for billing purposes.</summary>
	public class EcwDFT {
		private MessageHL7 msg;
		private SegmentHL7 seg;

		///<summary></summary>
		public EcwDFT() {
			
		}

		///<summary>Creates the Message object and fills it with data.</summary>
		public void InitializeEcw(long aptNum,long provNum,Patient pat,string pdfDataAsBase64,string pdfDescription,bool justPDF,List<Procedure> listProcs){
			msg=new MessageHL7(MessageTypeHL7.DFT);
			MSH();
			EVN();
			PID(pat);
			PV1(aptNum,provNum);
			FT1(listProcs,justPDF);
			DG1();
			ZX1(pdfDataAsBase64,pdfDescription);
		}

		///<summary>Message Header Segment</summary>
		private void MSH(){
			seg=new SegmentHL7(@"MSH|^~\&|OD||ECW||"+DateTime.Now.ToString("yyyyMMddHHmmss")+"||DFT^P03||P|2.3");
			msg.Segments.Add(seg);
		}

		///<summary>Event type segment.</summary>
		private void EVN(){
			seg=new SegmentHL7("EVN|P03|"+DateTime.Now.ToString("yyyyMMddHHmmss")+"|");
			msg.Segments.Add(seg);
		}

		///<summary>Patient identification.</summary>
		private void PID(Patient pat){
			seg=new SegmentHL7(SegmentNameHL7.PID);
			seg.SetField(0,"PID");
			seg.SetField(1,"1");
			seg.SetField(2,pat.ChartNumber);//Account number.  eCW requires this to be the same # as came in on PID.4.
			seg.SetField(3,pat.PatNum.ToString());//??what is this MRN?
			seg.SetField(5,pat.LName,pat.FName,pat.MiddleI);
			//we assume that dob is always valid because eCW should always pass us a dob.
			seg.SetField(7,pat.Birthdate.ToString("yyyyMMdd"));
			seg.SetField(8,ConvertGender(pat.Gender));
			seg.SetField(10,ConvertRace(PatientRaces.GetPatientRaceOldFromPatientRaces(pat.PatNum)));//Passes in the deprecated PatientRaceOld enum retrieved from the PatientRace table.
			seg.SetField(11,pat.Address,pat.Address2,pat.City,pat.State,pat.Zip);
			seg.SetField(13,ConvertPhone(pat.HmPhone));
			seg.SetField(14,ConvertPhone(pat.WkPhone));
			seg.SetField(16,ConvertMaritalStatus(pat.Position));
			seg.SetField(19,pat.SSN);
			msg.Segments.Add(seg);
		}

		///<summary>Patient visit.</summary>
		private void PV1(long aptNum,long provNum){
			seg=new SegmentHL7(SegmentNameHL7.PV1);
			seg.SetField(0,"PV1");
			Provider prov=Providers.GetProv(provNum);
			seg.SetField(7,prov.EcwID,prov.LName,prov.FName,prov.MI);
			seg.SetField(19,aptNum.ToString());
			msg.Segments.Add(seg);
		}

		///<summary>Financial transaction segment.</summary>
		private void FT1(List<Procedure> listProcs,bool justPDF){
			if(justPDF){
				return;//FT1 segment is not necessary when sending only a PDF.
			}
			ProcedureCode procCode;
			for(int i=0;i<listProcs.Count;i++) {
				seg=new SegmentHL7(SegmentNameHL7.FT1);
				seg.SetField(0,"FT1");
				seg.SetField(1,(i+1).ToString());
				seg.SetField(4,listProcs[i].ProcDate.ToString("yyyyMMdd"));
				seg.SetField(5,listProcs[i].ProcDate.ToString("yyyyMMdd"));
				seg.SetField(6,"CG");
				seg.SetField(10,"1.0");
				seg.SetField(16,"");//location code and description???
				seg.SetField(19,listProcs[i].DiagnosticCode);
				Provider prov=Providers.GetProv(listProcs[i].ProvNum);
				seg.SetField(20,prov.EcwID,prov.LName,prov.FName,prov.MI);//performed by provider.
				seg.SetField(21,prov.EcwID,prov.LName,prov.FName,prov.MI);//ordering provider.
				seg.SetField(22,listProcs[i].ProcFee.ToString("F2"));
				procCode=ProcedureCodes.GetProcCode(listProcs[i].CodeNum);
				if(procCode.ProcCode.Length>5 && procCode.ProcCode.StartsWith("D")) {
					seg.SetField(25,procCode.ProcCode.Substring(0,5));//Remove suffix from all D codes.
				}
				else {
					seg.SetField(25,procCode.ProcCode);
				}
				if(procCode.TreatArea==TreatmentArea.ToothRange){
					seg.SetField(26,listProcs[i].ToothRange,"");
				}
				else if(procCode.TreatArea==TreatmentArea.Surf){//probably not necessary
					seg.SetField(26,Tooth.ToInternat(listProcs[i].ToothNum),Tooth.SurfTidyForClaims(listProcs[i].Surf,listProcs[i].ToothNum));
				}
				//this property will not exist if using Oracle, eCW will never use Oracle
				else if(procCode.TreatArea==TreatmentArea.Quad && ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"IsQuadAsToothNum")=="1") {
					seg.SetField(26,listProcs[i].Surf,"");
				}
				else{
					seg.SetField(26,Tooth.ToInternat(listProcs[i].ToothNum),listProcs[i].Surf);
				}
				msg.Segments.Add(seg);
			}
		}

		///<summary>Diagnosis segment. Optional.</summary>
		private void DG1(){
			//DG1 optional, so we'll skip for now---------------------------------
			//seg=new SegmentHL7(SegmentName.DG1);
			//msg.Segments.Add(seg);
		}

		///<summary>PDF data segment.</summary>
		private void ZX1(string pdfDataAsBase64,string pdfDescription){
			seg=new SegmentHL7(SegmentNameHL7.ZX1);
			seg.SetField(0,"ZX1");
			seg.SetField(1,"6");
			seg.SetField(2,"PDF");
			seg.SetField(3,"PATHOLOGY^Pathology Report^L");
			seg.SetField(4,pdfDescription);
			seg.SetField(5,pdfDataAsBase64);
			msg.Segments.Add(seg);
		}

		public string GenerateMessage() {
			return msg.ToString();
		}

		private string ConvertGender(PatientGender gender){
			if(gender==PatientGender.Female) {
				return "F";
			}
			if(gender==PatientGender.Male) {
				return "M";
			}
			return "U";
		}

		//=======================================================================================================================================
		//DO NOT ALTER any of these Convert... methods for use with any other HL7 bridge.  
		//Each bridge tends to have slightly different implementation.  
		//No bridge can share any of these.
		//Instead, copy them into other classes.
		//This set of methods is ONLY for ECW, and will have to be renamed and grouped if any other DFT bridge is built.
		//=======================================================================================================================================

		///<summary>Convert the patient race entries to the deprecated PatientRaceOld enum before calling this method.</summary>
		private string ConvertRace(PatientRaceOld race) {
			switch(race) {
				case PatientRaceOld.AmericanIndian:
					return "American Indian Or Alaska Native";
				case PatientRaceOld.Asian:
					return "Asian";
				case PatientRaceOld.HawaiiOrPacIsland:
					return "Native Hawaiian or Other Pacific";
				case PatientRaceOld.AfricanAmerican:
					return "Black or African American";
				case PatientRaceOld.White:
					return "White";
				case PatientRaceOld.HispanicLatino:
					return "Hispanic";
				case PatientRaceOld.Other:
					return "Other Race";
				default:
					return "Other Race";
			}
		}

		private string ConvertPhone(string phone) {
			string retVal="";
			for(int i=0;i<phone.Length;i++){
				if(Char.IsNumber(phone,i)){
					if(retVal=="" && phone.Substring(i,1)=="1"){
						continue;//skip leading 1.
					}
					retVal+=phone.Substring(i,1);
				}
				if(retVal.Length==10){
					return retVal;
				}
			}
			//never made it to 10
			return "";
		}

		private string ConvertMaritalStatus(PatientPosition patpos) {
			switch(patpos){
				case PatientPosition.Single:
					return "Single";
				case PatientPosition.Married:
					return "Married";
				case PatientPosition.Divorced:
					return "Divorced";
				case PatientPosition.Widowed:
					return "Widowed";
				case PatientPosition.Child:
					return "Single";
				default:
					return "Single";
			}
		}






	}
}
