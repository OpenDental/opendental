using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeBase;
using System.Linq;

namespace OpenDentBusiness{
	public class SheetsInternal {

		public static SheetDef GetSheetDef(SheetInternalType sheetInternalType){
			switch(sheetInternalType){
				case SheetInternalType.LabelPatientMail:
					return GetSheetFromResource(Properties.Resources.LabelPatientMail);
				case SheetInternalType.LabelPatientLFAddress:
					return GetSheetFromResource(Properties.Resources.LabelPatientLFAddress);
				case SheetInternalType.LabelPatientLFChartNumber:
					return GetSheetFromResource(Properties.Resources.LabelPatientLFChartNumber);
				case SheetInternalType.LabelPatientLFPatNum:
					return GetSheetFromResource(Properties.Resources.LabelPatientLFPatNum);
				case SheetInternalType.LabelPatientRadiograph:
					return GetSheetFromResource(Properties.Resources.LabelPatientRadiograph);
				case SheetInternalType.LabelText:
					return GetSheetFromResource(Properties.Resources.LabelText);
				case SheetInternalType.LabelCarrier:
					return GetSheetFromResource(Properties.Resources.LabelCarrier);
				case SheetInternalType.LabelReferral:
					return GetSheetFromResource(Properties.Resources.LabelReferral);
				case SheetInternalType.ReferralSlip:
					return GetSheetFromResource(Properties.Resources.ReferralSlip);
				case SheetInternalType.LabelAppointment:
					return GetSheetFromResource(Properties.Resources.LabelAppointment);
				case SheetInternalType.Rx:
					return GetSheetFromResource(Properties.Resources.Rx);
				case SheetInternalType.Consent:
					return GetSheetFromResource(Properties.Resources.Consent);
				case SheetInternalType.PatientLetter:
					return GetSheetFromResource(Properties.Resources.PatientLetter);
				case SheetInternalType.PatientLetterTxFinder:
					return GetSheetFromResource(Properties.Resources.PatientLetterTxFinder);
				case SheetInternalType.ReferralLetter:
					return GetSheetFromResource(Properties.Resources.ReferralLetter);
				case SheetInternalType.PatientRegistration:
					return GetSheetFromResource(Properties.Resources.PatientRegistration);
				case SheetInternalType.PatientTransferCEMT:
					return GetSheetFromResource(Properties.Resources.PatientTransferCEMT);
				case SheetInternalType.RoutingSlip:
					return GetSheetFromResource(Properties.Resources.RoutingSlip);
				case SheetInternalType.FinancialAgreement:
					return GetSheetFromResource(Properties.Resources.FinancialAgreement);
				case SheetInternalType.HIPAA:
					return GetSheetFromResource(Properties.Resources.HIPAA);
				case SheetInternalType.MedicalHistSimple:
					return GetSheetFromResource(Properties.Resources.MedicalHistSimple);
				case SheetInternalType.MedicalHistNewPat:
					return GetSheetFromResource(Properties.Resources.MedicalHistNewPat);
				case SheetInternalType.MedicalHistUpdate:
					return GetSheetFromResource(Properties.Resources.MedicalHistUpdate);
				case SheetInternalType.LabSlip:
					return GetSheetFromResource(Properties.Resources.LabSlip);
				case SheetInternalType.ExamSheet:
					return GetSheetFromResource(Properties.Resources.ExamSheet);
				case SheetInternalType.DepositSlip:
					return GetSheetFromResource(Properties.Resources.DepositSlip);
				case SheetInternalType.Statement:
					return GetSheetFromResource(Properties.Resources.Statement);
				case SheetInternalType.MedLabResults:
					return MedLabResultReport();
				case SheetInternalType.TreatmentPlan:
					return GetSheetFromResource(Properties.Resources.TreatmentPlan);
				case SheetInternalType.Screening:
					return GetSheetFromResource(Properties.Resources.Screening);
				case SheetInternalType.PaymentPlan:
					return GetSheetFromResource(Properties.Resources.PaymentPlan);
				case SheetInternalType.RxMulti:
					return GetSheetFromResource(Properties.Resources.RXMulti);
				case SheetInternalType.ERA:
					return GetSheetFromResource(Properties.Resources.ERA);
				case SheetInternalType.ERAGridHeader:
					return GetSheetFromResource(Properties.Resources.EraGridHeader);
				case SheetInternalType.RxInstruction:
					return GetSheetFromResource(Properties.Resources.RxInstruction);
				case SheetInternalType.ChartModule:
					SheetDef sheetDef=GetSheetFromResource(Properties.Resources.ChartModuleStandardLayout);
					if(!PrefC.IsODHQ && !PrefC.GetBool(PrefName.DistributorKey)) {
						//Exclude HQ specific controls when not HQ.
						sheetDef.SheetFieldDefs.RemoveAll(x => x.FieldName.In("ButtonErxAccess","ButtonPhoneNums","ButtonForeignKey","ButtonUSAKey"));
					}
					return sheetDef;
				case SheetInternalType.PatientDashboard:
					sheetDef=GetSheetFromResource(Properties.Resources.PatientDashboard);
					SheetDefs.SetPatImageFieldNames(sheetDef);
					return sheetDef;
				case SheetInternalType.PatientDashboardToothChart:
					sheetDef=GetSheetFromResource(Properties.Resources.PatientDashboardToothChart);
					SheetDefs.SetPatImageFieldNames(sheetDef);
					return sheetDef;
				case SheetInternalType.COVID19:
					return GetSheetFromResource(Properties.Resources.COVID19);
				default:
					throw new ApplicationException("Invalid SheetInternalType.");
			}
		}

		private static SheetDef GetSheetFromResource(string xmlDoc) {
			SheetDef sheetDef=new SheetDef();
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(SheetDef));
			using(TextReader reader = new StringReader(xmlDoc)) {
				sheetDef=(SheetDef)xmlSerializer.Deserialize(reader);
			}
			//XML parsers are required to normalize \r\n to just \n.
			//That's a problem, because without the \r, it will display improperly in textboxes.
			//So, we convert \n that are by themselves to \r\n.
			//In the XML, line feeds (\n) within tag values simply show as white space, but we could theoretically use &#10; if we want.
			//Before this code was added, a number of lone \n made their way into the various sheet fields.
			//They do not cause a problem when drawing, but they do cause a problem in textboxes.
			//To clean up any existing line ending issues, we run identical Replace code whenever anyone opens a textbox.
			//The two places where that's done are FormSheetFieldStatic.Load() and FormSheetFillEdit.CreateFloatingTextBox().
			for(int i=0;i<sheetDef.SheetFieldDefs.Count;i++){
				if(sheetDef.SheetFieldDefs[i].FieldType!=SheetFieldType.StaticText){
					continue;
				}
				sheetDef.SheetFieldDefs[i].FieldValue=Regex.Replace(sheetDef.SheetFieldDefs[i].FieldValue,@"(?<!\r)\n","\r\n");
			}
			if(sheetDef.Parameters==null) {
				sheetDef.Parameters=SheetParameter.GetForType(sheetDef.SheetType);
			}
			//All new SheetDefs should start with RevID=1. Setting that value here means we don't have to edit every resource file.
			sheetDef.RevID=1;
			return sheetDef;
		}

		public static List<SheetDef> GetAllInternal() {
			List<SheetInternalType> listSheetInternalTypes=Enum.GetValues(typeof(SheetInternalType)).OfType<SheetInternalType>().ToList();//all
			listSheetInternalTypes=listSheetInternalTypes.FindAll(x=>EnumTools.GetAttributeOrDefault<SheetInternalAttribute>(x).DoShowInInternalList);//filtered
			List<SheetDef> listSheetDefs=new List<SheetDef>();
			for(int i=0;i<listSheetInternalTypes.Count;i++){
				SheetDef sheetDef=GetSheetDef(listSheetInternalTypes[i]);
				listSheetDefs.Add(sheetDef);
			}
			return listSheetDefs;
		}

		private static SheetDef MedLabResultReport() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.MedLabResults);
			sheet.Description="MedLab Result Report";
			sheet.FontName="Arial";
			sheet.FontSize=8.5f;
			sheet.Width=850;
			sheet.Height=1100;
			//From top to bottom
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,40,750,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("MedLab Result Report",14,"Arial",true,52,53,746,22,textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,80,221,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(271,80,180,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(451,80,112,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(563,80,112,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(675,80,125,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Specimen Number",sheet.FontSize,"Arial",false,54,83,214,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient ID",sheet.FontSize,"Arial",false,275,83,173,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Control Number",sheet.FontSize,"Arial",false,455,83,105,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Account Number",sheet.FontSize,"Arial",false,567,83,105,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Account Phone #",sheet.FontSize,"Arial",false,679,83,119,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.PatIDLab",9,"Arial",false,53,102,216,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.PatIDAlt",9,"Arial",false,274,102,175,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.SpecimenIDAlt",9,"Arial",false,454,102,107,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.PatAccountNum",9,"Arial",false,566,102,107,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticePh",9,"Arial",false,678,102,120,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,120,401,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(451,120,349,160));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient Last Name",sheet.FontSize,"Arial",false,54,123,394,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Account Address",sheet.FontSize,"Arial",false,455,123,341,15,itemColor:KnownColor.GrayText,
				textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.LName",9,"Arial",false,53,142,396,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,160,221,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(271,160,180,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient First Name",sheet.FontSize,"Arial",false,54,163,214,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient Middle Name",sheet.FontSize,"Arial",false,275,163,173,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticeTitle",9,"Arial",false,483,162,315,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.FName",9,"Arial",false,53,182,216,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.MiddleI",9,"Arial",false,274,182,175,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("practiceAddrCityStZip",9,"Arial",false,483,182,315,57,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,200,165,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(215,200,110,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(325,200,126,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient SSN",sheet.FontSize,"Arial",false,54,203,158,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient Phone",sheet.FontSize,"Arial",false,219,203,103,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Total Volume",sheet.FontSize,"Arial",false,329,203,119,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.SSN",9,"Arial",false,53,222,160,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.HmPhone",9,"Arial",false,218,222,105,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.TotalVolume",9,"Arial",false,328,222,121,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,240,165,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(215,240,110,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(325,240,63,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(388,240,63,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Age (Y/M/D)",sheet.FontSize,"Arial",false,54,243,158,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date of Birth",sheet.FontSize,"Arial",false,219,243,103,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Gender",sheet.FontSize,"Arial",false,329,243,56,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Fasting",sheet.FontSize,"Arial",false,392,243,56,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.PatAge",9,"Arial",false,53,262,160,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.Birthdate",9,"Arial",false,218,262,105,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.Gender",9,"Arial",false,328,262,58,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.PatFasting",9,"Arial",false,391,262,58,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,280,401,80));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(451,280,349,80));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient Address",sheet.FontSize,"Arial",false,54,283,393,15,itemColor:KnownColor.GrayText,
				textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Additional Information",sheet.FontSize,"Arial",false,455,283,341,15,itemColor:KnownColor.GrayText,
				textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.addrCityStZip",9,"Arial",false,53,302,396,57,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.ClinicalInfo",9,"Arial",false,454,302,344,57,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,360,149,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(199,360,103,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(302,360,149,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(451,360,168,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(619,360,81,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(700,360,100,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date & Time Collected",sheet.FontSize,"Arial",false,54,363,142,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date Entered",sheet.FontSize,"Arial",false,203,363,96,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date & Time Reported",sheet.FontSize,"Arial",false,306,363,142,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Physician Name",sheet.FontSize,"Arial",false,455,363,161,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("NPI",sheet.FontSize,"Arial",false,623,363,74,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Physician ID",sheet.FontSize,"Arial",false,704,363,93,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.DateTimeCollected",9,"Arial",false,53,382,144,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.dateEntered",9,"Arial",false,202,382,98,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.DateTimeReported",9,"Arial",false,305,382,144,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.provNameLF",9,"Arial",false,454,382,163,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.ProvNPI",9,"Arial",false,622,382,76,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.ProvID",9,"Arial",false,703,382,95,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,420,750,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Tests Ordered",sheet.FontSize,"Arial",false,54,423,742,15,itemColor:KnownColor.GrayText,
				textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.obsTests",9,"Arial",false,52,442,746,17,HorizontalAlignment.Left,
				growthBehavior:GrowthBehaviorEnum.DownGlobal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,460,750,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(50,480,165,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(215,480,110,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(325,480,126,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(451,480,168,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(619,480,81,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(700,480,100,40));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Client Accession (ACC)",sheet.FontSize,"Arial",false,54,483,158,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Client Alt PatID (PID)",sheet.FontSize,"Arial",false,329,483,119,15,itemColor:KnownColor.GrayText));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.SpecimenID",9,"Arial",false,53,502,160,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.PatNum",9,"Arial",false,328,502,121,17,HorizontalAlignment.Left));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,540,750,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("General Comments",sheet.FontSize,"Arial",false,54,543,742,15,itemColor:KnownColor.GrayText,
				textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medlab.NoteLab",9,"Arial",false,52,562,746,17,HorizontalAlignment.Left,
				growthBehavior:GrowthBehaviorEnum.DownGlobal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,580,750,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Lab Results",sheet.FontSize,"Arial",false,54,583,742,15,itemColor:KnownColor.GrayText,
				textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewGrid("MedLabResults",40,604,770,37,7.5f,"Courier New"));//LabCorp requested we use a fixed width font for the grids
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,644,750,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Lab Facilities",sheet.FontSize,"Arial",false,54,649,742,15,itemColor:KnownColor.GrayText,
				textAlign:HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medLabFacilityAddr",9,"Arial",false,52,668,371,37,HorizontalAlignment.Left,
				growthBehavior:GrowthBehaviorEnum.DownLocal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("medLabFacilityDir",9,"Arial",false,427,668,371,37,HorizontalAlignment.Left,
				growthBehavior:GrowthBehaviorEnum.DownLocal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,708,750,0));
			return sheet;
		}

		///<summary>This will throw an exception if you pass in MedicalHistory, PatientForm, or LabelPatient.  
		///The listed internal types have multiple sheets to choose from and assume which one is wanted at any given time from here.</summary>
		public static SheetInternalType GetInternalType(SheetTypeEnum sheetType) {
			SheetInternalType retVal=SheetInternalType.Consent;
			switch(sheetType) {
				case SheetTypeEnum.Consent:
					retVal=SheetInternalType.Consent;
					break;
				case SheetTypeEnum.DepositSlip:
					retVal=SheetInternalType.DepositSlip;
					break;
				case SheetTypeEnum.ExamSheet:
					retVal=SheetInternalType.ExamSheet;
					break;
				case SheetTypeEnum.LabelAppointment:
					retVal=SheetInternalType.LabelAppointment;
					break;
				case SheetTypeEnum.LabelCarrier:
					retVal=SheetInternalType.LabelCarrier;
					break;
				case SheetTypeEnum.LabelReferral:
					retVal=SheetInternalType.LabelReferral;
					break;
				case SheetTypeEnum.LabSlip:
					retVal=SheetInternalType.LabSlip;
					break;
				case SheetTypeEnum.MedLabResults:
					retVal=SheetInternalType.MedLabResults;
					break;
				case SheetTypeEnum.PatientLetter:
					retVal=SheetInternalType.PatientLetter;
					break;
				case SheetTypeEnum.PaymentPlan:
					retVal=SheetInternalType.PaymentPlan;
					break;
				case SheetTypeEnum.ReferralLetter:
					retVal=SheetInternalType.ReferralLetter;
					break;
				case SheetTypeEnum.ReferralSlip:
					retVal=SheetInternalType.ReferralSlip;
					break;
				case SheetTypeEnum.RoutingSlip:
					retVal=SheetInternalType.RoutingSlip;
					break;
				case SheetTypeEnum.Rx:
					retVal=SheetInternalType.Rx;
					break;
				case SheetTypeEnum.RxMulti:
					retVal=SheetInternalType.RxMulti;
					break;
				case SheetTypeEnum.RxInstruction:
					retVal=SheetInternalType.RxInstruction;
					break;
				case SheetTypeEnum.Screening:
					retVal=SheetInternalType.Screening;
					break;
				case SheetTypeEnum.Statement:
					retVal=SheetInternalType.Statement;
					break;
				case SheetTypeEnum.TreatmentPlan:
					retVal=SheetInternalType.TreatmentPlan;
					break;
				case SheetTypeEnum.ERA:
					retVal=SheetInternalType.ERA;
					break;
				case SheetTypeEnum.ERAGridHeader:
					retVal=SheetInternalType.ERAGridHeader;
					break;
				case SheetTypeEnum.ChartModule:
					retVal=SheetInternalType.ChartModule;
					break;
				case SheetTypeEnum.MedicalHistory://This SheetTypeEnum is grouped into PatientForm
				case SheetTypeEnum.PatientForm://This should be handled outside of this method because it has 5 internal form types
				case SheetTypeEnum.LabelPatient://This should be handled outside of this method because it has 5 internal label types
				default:
					throw new Exception(Lans.g("SheetsInternal","Unsupported SheetTypeEnum")+"\r\n"+sheetType.ToString());
			}
			return retVal;
		}

		public static SheetDef GetSheetDef(SheetTypeEnum sheetType) {
			return SheetsInternal.GetSheetDef(SheetsInternal.GetInternalType(sheetType));
		}

	}
}
