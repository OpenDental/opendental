using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using CodeBase;
using System.Linq;

namespace OpenDentBusiness{
	public class SheetsInternal {

		public static SheetDef GetSheetDef(SheetInternalType internalType){
			switch(internalType){
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
						sheetDef.SheetFieldDefs.RemoveAll(x => ListTools.In(x.FieldName,"ButtonErxAccess","ButtonPhoneNums","ButtonForeignKey","ButtonUSAKey"));
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
			XmlSerializer serializer=new XmlSerializer(typeof(SheetDef));
			using(TextReader reader = new StringReader(xmlDoc)) {
				sheetDef=(SheetDef)serializer.Deserialize(reader);
			}
			if(sheetDef.Parameters==null) {
				sheetDef.Parameters=SheetParameter.GetForType(sheetDef.SheetType);
			}
			//All new SheetDefs should start with RevID=1. Setting that value here means we don't have to edit every resource file.
			sheetDef.RevID=1;
			return sheetDef;
		}

		public static List<SheetDef> GetAllInternal() {
			return Enum.GetValues(typeof(SheetInternalType)).OfType<SheetInternalType>()
				.Where(x => EnumTools.GetAttributeOrDefault<SheetInternalAttribute>(x).DoShowInInternalList)
				.Select(x => GetSheetDef(x))
				.ToList();
		}
		
		private static SheetDef LabelPatientMail(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelPatient);
			sheet.Description="Label Patient Mail";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=10;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("nameFL",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("address",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH,GrowthBehaviorEnum.DownLocal));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("cityStateZip",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			return sheet;
		}

		private static SheetDef LabelPatientLFAddress() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelPatient);
			sheet.Description="Label PatientLFAddress";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=10;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("nameLF",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("address",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH,GrowthBehaviorEnum.DownLocal));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("cityStateZip",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			return sheet;
		}

		private static SheetDef LabelPatientLFChartNumber(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelPatient);
			sheet.Description="Label PatientLFChartNumber";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=30;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("nameLF",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("ChartNumber",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			return sheet;
		}

		private static SheetDef LabelPatientLFPatNum() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelPatient);
			sheet.Description="Label PatientLFPatNum";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=30;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("nameLF",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PatNum",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			return sheet;
		}

		private static SheetDef LabelPatientRadiograph(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelPatient);
			sheet.Description="Label Patient Radiograph";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=30;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput( "nameLF",sheet.FontSize,sheet.FontName,false,25,yPos,150,rowH,GrowthBehaviorEnum.None));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput( "dateTime.Today",sheet.FontSize,sheet.FontName,false,180,yPos,100,rowH,GrowthBehaviorEnum.None));
			yPos += rowH;
			//smallfont:
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput( "birthdate",9,sheet.FontName,false,25,yPos,105,rowH, GrowthBehaviorEnum.None));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput( "priProvName",9,sheet.FontName,false,130,yPos,150,rowH, GrowthBehaviorEnum.None));
			return sheet;
		}

		private static SheetDef LabelText() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelPatient);
			sheet.Description="Label Text";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			//int rowH=19;
			int yPos=30;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("text",sheet.FontSize,sheet.FontName,false,25,yPos,300,315,GrowthBehaviorEnum.None));
			return sheet;
		}

		private static SheetDef LabelCarrier(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelCarrier);
			sheet.Description="Label Carrier";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=10;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("CarrierName",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("address",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH,GrowthBehaviorEnum.DownLocal));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("cityStateZip",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			return sheet;
		}

		private static SheetDef LabelReferral(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelReferral);
			sheet.Description="Label Referral";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=12f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=10;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("nameFL",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("address",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH,GrowthBehaviorEnum.DownLocal));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("cityStateZip",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			return sheet;
		}

		private static SheetDef ReferralSlip(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.ReferralSlip);
			sheet.Description="Referral Slip";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=9f;
			sheet.Width=450;
			sheet.Height=650;
			int rowH=17;
			int yPos=50;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Referral to",10,sheet.FontName,true,170,yPos,200,19));
			yPos+=rowH+5;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.nameFL",sheet.FontSize,sheet.FontName,false,150,yPos,200,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.address",sheet.FontSize,sheet.FontName,false,150,yPos,200,rowH,GrowthBehaviorEnum.DownLocal));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.cityStateZip",sheet.FontSize,sheet.FontName,false,150,yPos,200,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.phone",sheet.FontSize,sheet.FontName,false,150,yPos,200,rowH));
			yPos+=rowH+30;
			//Patient--------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient",9,sheet.FontName,true,25,yPos,100,rowH));
			yPos+=rowH+5;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.nameFL",sheet.FontSize,sheet.FontName,false,25,yPos,200,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("dateTime.Today",sheet.FontSize,sheet.FontName,false,300, yPos,100,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Work:",sheet.FontSize,sheet.FontName,false,25,yPos,38,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.WkPhone",sheet.FontSize,sheet.FontName,false,63,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Home:",sheet.FontSize,sheet.FontName,false,25,yPos,42,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.HmPhone",sheet.FontSize,sheet.FontName,false,67,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Wireless:",sheet.FontSize,sheet.FontName,false,25,yPos,58,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.WirelessPhone",sheet.FontSize,sheet.FontName,false,83,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.address",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH,GrowthBehaviorEnum.DownLocal));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.cityStateZip",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH+30;
			//Provider--------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Referred by",9,sheet.FontName,true,25,yPos,300,rowH));
			yPos+=rowH+5;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.provider",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH+20;
			//Notes--------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Notes",9,sheet.FontName,true,25,yPos,300,rowH));
			yPos+=rowH+5;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("notes",sheet.FontSize,sheet.FontName,false,25,yPos,400,240));
			return sheet;
		}

		private static SheetDef LabelAppointment() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabelAppointment);
			sheet.Description="Label Appointment";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=108;
			sheet.Height=346;
			sheet.IsLandscape=true;
			int rowH=19;
			int yPos=15;
			//if(PrefC.GetBool(PrefName.FuchsOptionsOn")) yPos = 50;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("nameFL",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Your appointment is scheduled for:",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("weekdayDateTime",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Appointment length:",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			yPos+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("length",sheet.FontSize,sheet.FontName,false,25,yPos,300,rowH));
			return sheet;
		}

		private static SheetDef Rx() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.Rx);
			sheet.Description="Rx";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=8f;
			sheet.Width=425;
			sheet.Height=550;
			sheet.IsLandscape=true;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(0,0,550,0));//top
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(0,0,0,425));//left
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(549,0,0,425));//right
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(0,424,550,0));//bottom
			int x;
			int y;
			int rowH=14;//for font of 8.
			//Dr--------------------------------------------------------------------------------------------------
			//Left Side
			x=50;
			y=37;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("prov.nameFL",sheet.FontSize,sheet.FontName,true,x,y,170,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("clinic.address",sheet.FontSize,sheet.FontName,false,x,y,170,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("clinic.cityStateZip",sheet.FontSize,sheet.FontName,false,x,y,170,rowH));
			y=100;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(25,y,500,0));
			//Right Side
			x=280;
			y=38;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("clinic.phone",sheet.FontSize,sheet.FontName,false,x,y,170,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("RxDate",sheet.FontSize,sheet.FontName,false,x,y,170,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("DEA#:",sheet.FontSize,sheet.FontName,true,x,y,40,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("prov.dEANum",sheet.FontSize,sheet.FontName,false,x+40,y,130,rowH));
			//Patient---------------------------------------------------------------------------------------------------
			//Upper Left
			x=90;
			y=105;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("pat.nameFL",sheet.FontSize,sheet.FontName,true,x,y,150,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("DOB:",sheet.FontSize,sheet.FontName,true,x,y,40,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("pat.Birthdate",sheet.FontSize,sheet.FontName,true,x+40,y,110,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("pat.HmPhone",sheet.FontSize,sheet.FontName,false,x,y,150,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("pat.address",sheet.FontSize,sheet.FontName,false,x,y,170,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("pat.cityStateZip",sheet.FontSize,sheet.FontName,false,x,y,170,rowH));
			//RX-----------------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Rx",24,"Times New Roman",true,35,190,55,33));
			y=205;
			x=90;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("Drug",sheet.FontSize,sheet.FontName,true,x,y,300,rowH));
			y+=(int)(rowH*1.5);
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Disp:",sheet.FontSize,sheet.FontName,false,x,y,35,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("Disp",sheet.FontSize,sheet.FontName,false,x+35,y,300,rowH));
			y+=(int)(rowH*1.5);
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Sig:",sheet.FontSize,sheet.FontName,false,x,y,30,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("Sig",sheet.FontSize,sheet.FontName,false,x+30,y,325,rowH*2));
			y+=(int)(rowH*2.5);
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Refills:",sheet.FontSize,sheet.FontName,false,x,y,45,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("Refills",sheet.FontSize,sheet.FontName,false,x+45,y,110,rowH));
			//Generic Subst----------------------------------------------------------------------------------------------
			x=50;
			y=343;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y,12,12));
			x+=17;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Dispense as Written",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			x-=17;
			y+=25;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y,12,12));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y,12,12));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x+12,y,-12,12));
			x+=17;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Generic Substitution Permitted",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			//Signature Line--------------------------------------------------------------------------------------------
			y=360;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(295,y,230,0));
			y+=4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Signature of Prescriber",sheet.FontSize,sheet.FontName,false,340,y,150,rowH));
			return sheet;
		}

		private static SheetDef Consent(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.Consent);
			sheet.Description="Extraction Consent";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=11f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=19;
			int x=200;
			int y=40;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Extraction Consent",12,sheet.FontName,true,x,y,170,20));
			y+=35;
			x=50;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("dateTime.Today",sheet.FontSize,sheet.FontName,false,x,y,120,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.nameFL",sheet.FontSize,sheet.FontName,false,x,y,220,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Tooth number(s): ",sheet.FontSize,sheet.FontName,false,x,y,120,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("toothNum",sheet.FontSize,sheet.FontName,false,x+120,y,100,rowH));
			y+=rowH;
			y+=20;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(@"Extraction(s) are to be performed on the tooth/teeth listed above.  While we expect no complications, there are some risks involved with this procedure.  The more common complications are:

Pain, infection, swelling, bruising, and discoloration.  Adjacent teeth may be chipped or damaged during the extraction.

Nerves that run near the area of extraction may be bruised or damaged.  You may experience some temporary numbness and tingling of the lip and chin, or in rare cases, the tongue.  In some extremely rare instances, the lack of sensation could be permanent.

In the upper arch, sinus complications can occur because the roots of some upper teeth extend near or into the sinuses.  After extraction, a hole may be present between the sinus and the mouth.  If this happens, you will be informed and the area repaired.

By signing below you acknowledge that you understand the information presented, have had all your questions answered satisfactorily, and give consent to perform this procedure.",sheet.FontSize,sheet.FontName,false,x,y,500,360));
			y+=360;
			y+=20;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewSigBox(x,y,364,81));
			y+=82;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Signature",sheet.FontSize,sheet.FontName,false,x,y,90,rowH));
			return sheet;
		}

		private static SheetDef PatientLetter(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.PatientLetter);
			sheet.Description="Patient Letter";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=11f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=19;
			int x=100;
			int y=100;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticeTitle",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticeAddress",sheet.FontSize,sheet.FontName,false,x,y,200,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("practiceCityStateZip",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.nameFL",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.address",sheet.FontSize,sheet.FontName,false,x,y,200,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.cityStateZip",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("today.DayDate",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.salutation",sheet.FontSize,sheet.FontName,false,x,y,280,rowH));
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("letter text",sheet.FontSize,sheet.FontName,false,x,y,650,200,
				GrowthBehaviorEnum.DownGlobal));
			y+=200;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Sincerely,",sheet.FontSize,sheet.FontName,false,x,y,100,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.priProvNameFL",sheet.FontSize,sheet.FontName,false,x,y,240,rowH));
			return sheet;
		}

		private static SheetDef PatientLetterTxFinder(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.PatientLetter);
			sheet.Description="Patient Letter Tx Finder";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=11f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=19;
			int x=100;
			int y=100;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticeTitle",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticeAddress",sheet.FontSize,sheet.FontName,false,x,y,200,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("practiceCityStateZip",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.nameFL",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.address",sheet.FontSize,sheet.FontName,false,x,y,200,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.cityStateZip",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("today.DayDate",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(@"Your insurance benefits will renew soon.  You have [insRemaining] remaining that can be applied towards important dental treatment."
				+@"  Our records show that the following treatment has not yet been completed."
				+"\r\n\r\n[treatmentPlanProcs]\r\n\r\n"
				+"Please call our office at your earliest convenience to schedule an appointment.",sheet.FontSize,sheet.FontName,false,x,y,650,200,
				GrowthBehaviorEnum.DownGlobal));
			y+=200;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.priProvNameFL",sheet.FontSize,sheet.FontName,false,x,y,240,rowH));
			return sheet;
		}

		private static SheetDef ReferralLetter(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.ReferralLetter);
			sheet.Description="Referral Letter";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=11f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=19;
			int x=100;
			int y=100;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticeTitle",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("PracticeAddress",sheet.FontSize,sheet.FontName,false,x,y,200,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("practiceCityStateZip",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.nameFL",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.address",sheet.FontSize,sheet.FontName,false,x,y,200,rowH,
				GrowthBehaviorEnum.DownLocal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.cityStateZip",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("today.DayDate",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("RE patient:",sheet.FontSize,sheet.FontName,false,x,y,90,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.nameFL",sheet.FontSize,sheet.FontName,false,x+90,y,200,rowH));
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("referral.salutation",sheet.FontSize,sheet.FontName,false,x,y,280,rowH));
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("letter text",sheet.FontSize,sheet.FontName,false,x,y,650,rowH,
				GrowthBehaviorEnum.DownGlobal));
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Sincerely,",sheet.FontSize,sheet.FontName,false,x,y,100,rowH));
			y+=rowH;
			y+=rowH;
			y+=rowH;
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("patient.priProvNameFL",sheet.FontSize,sheet.FontName,false,x,y,250,rowH));
			return sheet;
		}

		private static SheetDef PatientRegistration() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.PatientForm);
			sheet.Description="Registration Form";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=16;
			int y=31;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewImage("Patient Info.gif",39,y,761,988));
			y=204;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("LName",sheet.FontSize,sheet.FontName,false,126,y,150,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("FName",sheet.FontSize,sheet.FontName,false,293,y,145,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("MiddleI",sheet.FontSize,sheet.FontName,false,447,y,50,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Preferred",sheet.FontSize,sheet.FontName,false,507,y,150,rowH));
			y=236;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Birthdate",sheet.FontSize,sheet.FontName,false,133,y,105,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("SSN",sheet.FontSize,sheet.FontName,false,292,y,140,rowH));
			y=241;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("Gender","Male",499,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("Gender","Female",536,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("Position","Married",649,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("Position","Single",683,y,10,10));
			y=255;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("WkPhone",sheet.FontSize,sheet.FontName,false,152,y,120,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("WirelessPhone",sheet.FontSize,sheet.FontName,false,381,y,120,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("wirelessCarrier",sheet.FontSize,sheet.FontName,false,631,y,130,rowH));
			y=274;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Email",sheet.FontSize,sheet.FontName,false,114,y,575,rowH));
			y=299;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferContactMethod","HmPhone",345,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferContactMethod","WkPhone",429,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferContactMethod","WirelessPh",513,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferContactMethod","Email",607,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("PreferContactMethodIsTextMessage",666,y,10,10));
			y=318;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferConfirmMethod","HmPhone",343,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferConfirmMethod","WkPhone",428,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferConfirmMethod","WirelessPh",511,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferConfirmMethod","Email",605,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("PreferConfirmMethodIsTextMessage",664,y,10,10));
			y=337;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferRecallMethod","HmPhone",343,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferRecallMethod","WkPhone",428,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferRecallMethod","WirelessPh",512,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("PreferRecallMethod","Email",605,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("PreferRecallMethodIsTextMessage",665,y,10,10));
			//cover up the options for text messages since we don't support that yet
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(".",sheet.FontSize,sheet.FontName,false,660,293,100,70));
			y=356;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("StudentStatus","Nonstudent",346,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("StudentStatus","Fulltime",443,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("StudentStatus","Parttime",520,y,10,10));
			y+=33;//375;
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("guarantorIsSelf",270,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("guarantorIsOther",320,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("guarantorNameF",sheet.FontSize,sheet.FontName,false,378,370,150,rowH));
			//y=409;-34
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("referredFrom",sheet.FontSize,sheet.FontName,false,76,y,600,rowH));
			y+=64;//439;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("addressAndHmPhoneIsSameEntireFamily",283,y,10,10));
			y+=15;//453;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Address",sheet.FontSize,sheet.FontName,false,128,y,425,rowH));
			y+=19;//472;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Address2",sheet.FontSize,sheet.FontName,false,141,y,425,rowH));
			y+=19;//491;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("City",sheet.FontSize,sheet.FontName,false,103,y,200,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("State",sheet.FontSize,sheet.FontName,false,359,y,45,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Zip",sheet.FontSize,sheet.FontName,false,439,y,100,rowH));
			y+=19;//510;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("HmPhone",sheet.FontSize,sheet.FontName,false,156,y,120,rowH));
			//Ins 1--------------------------------------------------------------------------------------------------------------
			y+=58;//569;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins1Relat","Self",267,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins1Relat","Spouse",320,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins1Relat","Child",394,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins1RelatIsNotSelfSpouseChild",457,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1RelatDescript",sheet.FontSize,sheet.FontName,false,515,598,200,rowH));
			y+=16;//585;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1SubscriberNameF",sheet.FontSize,sheet.FontName,false,184,y,250,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1SubscriberID",sheet.FontSize,sheet.FontName,false,565,y,140,rowH));
			y+=20;//604;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1CarrierName",sheet.FontSize,sheet.FontName,false,201,y,290,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1CarrierPhone",sheet.FontSize,sheet.FontName,false,552,y,170,rowH));
			y+=19;//623;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1EmployerName",sheet.FontSize,sheet.FontName,false,136,y,190,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1GroupName",sheet.FontSize,sheet.FontName,false,419,y,160,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins1GroupNum",sheet.FontSize,sheet.FontName,false,638,y,120,rowH));
			//Ins 2-------------------------------------------------------------------------------------------------------------
			y+=72;//695;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins2Relat","Self",267,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins2Relat","Spouse",320,y,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins2Relat","Child",394,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("ins2RelatIsNotSelfSpouseChild",457,y,10,10));
			//sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2RelatDescript",sheet.FontSize,sheet.FontName,false,515,598+126,200,rowH));
			y+=16;//585+126;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2SubscriberNameF",sheet.FontSize,sheet.FontName,false,184,y,250,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2SubscriberID",sheet.FontSize,sheet.FontName,false,565,y,140,rowH));
			y+=19;//604+126;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2CarrierName",sheet.FontSize,sheet.FontName,false,201,y,290,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2CarrierPhone",sheet.FontSize,sheet.FontName,false,552,y,170,rowH));
			y+=19;//623+126;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2EmployerName",sheet.FontSize,sheet.FontName,false,136,y,190,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2GroupName",sheet.FontSize,sheet.FontName,false,419,y,160,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("ins2GroupNum",sheet.FontSize,sheet.FontName,false,638,y,120,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,136,821,600,200));
			return sheet;
		}

		private static SheetDef RoutingSlip() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.RoutingSlip);
			sheet.Description="Routing Slip";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=18;
			int x=75;
			int y=50;
			//Title----------------------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Routing Slip",12f,sheet.FontName,true,373,y,200,22));
			y+=35;
			//Today's appointment, including procedures-----------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("[nameFL]",sheet.FontSize,sheet.FontName,true,x,y,500,19));
			y+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("appt.timeDate",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("appt.length",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("appt.providers",sheet.FontSize,sheet.FontName,false,x,y,500,rowH,GrowthBehaviorEnum.DownGlobal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Procedures:",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("appt.procedures",sheet.FontSize,sheet.FontName,false,x+10,y,490,rowH,GrowthBehaviorEnum.DownGlobal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Note:",sheet.FontSize,sheet.FontName,false,x,y,40,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("appt.Note",sheet.FontSize,sheet.FontName,false,x+40,y,460,rowH,GrowthBehaviorEnum.DownGlobal));
			y+=rowH;
			y+=3;
			//Patient/Family Info---------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y,725,0));
			y+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient Info",sheet.FontSize,sheet.FontName,true,x,y,500,19));
			y+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("PatNum: [PatNum]",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Age: [age]",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date of First Visit: [DateFirstVisit]",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Billing Type: [BillingType]",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Recall Due Date: [dateRecallDue]",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Medical notes: [MedUrgNote]",sheet.FontSize,sheet.FontName,false,x,y,725,rowH,GrowthBehaviorEnum.DownGlobal));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Other Family Members",sheet.FontSize,sheet.FontName,true,x,y,500,19));
			y+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("otherFamilyMembers",sheet.FontSize,sheet.FontName,false,x,y,500,rowH,GrowthBehaviorEnum.DownGlobal));
			y+=rowH;
			y+=3;
			//Insurance Info---------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y,725,0));
			y+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Primary Insurance",sheet.FontSize,sheet.FontName,true,x,y,360,19));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Secondary Insurance",sheet.FontSize,sheet.FontName,true,x+365,y,360,19));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x+362,y,0,133));
			y+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(
@"[carrierName]
Subscriber: [subscriberNameFL]
Annual Max: [insAnnualMax], Pending: [insPending], Used: [insUsed]
Deductible: [insDeductible], Ded Used: [insDeductibleUsed]
[insPercentages]"
				,sheet.FontSize,sheet.FontName,false,x,y,360,114,GrowthBehaviorEnum.DownGlobal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(
@"[carrier2Name]
Subscriber: [subscriber2NameFL]
Annual Max: [ins2AnnualMax], Pending: [ins2Pending], Used: [ins2Used]
Deductible: [ins2Deductible], Ded Used: [ins2DeductibleUsed]
[ins2Percentages]"
				,sheet.FontSize,sheet.FontName,false,x+365,y,360,114,GrowthBehaviorEnum.DownGlobal));
			y+=114;
			y+=3;
			//Account Info---------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y,725,0));
			y+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Account Info",sheet.FontSize,sheet.FontName,true,x,y,500,19));
			y+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(
				@"Guarantor: [guarantorNameFL]
Balance: [balTotal]
-Ins Est: [balInsEst]
=Total: [balTotalMinusInsEst]
Aging: 0-30:[bal_0_30]  31-60:[bal_31_60]  61-90:[bal_61_90]  90+:[balOver90]
Fam Urgent Fin Note: [famFinUrgNote]"
				,sheet.FontSize,sheet.FontName,false,x,y,725,98,GrowthBehaviorEnum.DownGlobal));
			y+=98;
			y+=3;
			//Insurance Info---------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y,725,0));
			y+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Treatment Plan",sheet.FontSize,sheet.FontName,true,x,y,500,19,GrowthBehaviorEnum.DownGlobal));
			y+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("[treatmentPlanProcsPriority]",sheet.FontSize,sheet.FontName,false,x,y,500,19,GrowthBehaviorEnum.DownGlobal));
			y+=rowH;
			return sheet;
		}

		private static SheetDef FinancialAgreement(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.PatientForm);
			sheet.Description="Financial Agreement";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=575;
			int rowH=18;
			int yOffset=25;
			int y=135;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Financial Agreement",12f,sheet.FontName,true,332,65,200,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Last Name:",sheet.FontSize,sheet.FontName,false,91,y,75,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("LName",sheet.FontSize,sheet.FontName,false,166,y,150,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("First Name:",sheet.FontSize,sheet.FontName,false,321,y,75,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("FName",sheet.FontSize,sheet.FontName,false,396,y,150,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Birthdate:",sheet.FontSize,sheet.FontName,false,551,y,65,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Birthdate",sheet.FontSize,sheet.FontName,false,616,y,145,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date: [dateToday]",sheet.FontSize,sheet.FontName,false,92,135+yOffset,120,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(@"* For my convenience, this office may release my information to my insurance company, and receive payment directly from them.
* I understand that if I begin major treatment that involves lab work, I will be responsible for the fee at that time.
* If sent to collections, I agree to pay all related fees and court costs.
* Every effort will be made to help me with my insurance, but if they do not pay as expected, I will still be responsible.
* I agree to pay finance charges of 1.5% per month (18% APR) on any balance 90 days past due.
* I will pay a fee for appointments broken without 24 hours notice. 
* Treatment plans may change, and I will be responsible for the work actually done.",sheet.FontSize,sheet.FontName,false,92,167+yOffset,670,155));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("I agree to let this office run a credit report.  If no, then all fees are due at time of service.",sheet.FontSize,sheet.FontName,false,92,337+yOffset,550,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(93,360+yOffset,11,11));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(93,378+yOffset,11,11));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("misc",94,361+yOffset,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("misc",94,379+yOffset,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Yes",sheet.FontSize,sheet.FontName,false,108,358+yOffset,40,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("No",sheet.FontSize,sheet.FontName,false,108,376+yOffset,40,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewSigBox(258,416+yOffset,364,81));
			return sheet;
		}

		private static SheetDef HIPAA(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.PatientForm);
			sheet.Description="HIPAA";
			sheet.FontName="Microsoft Sans Serif";
			int rowH=18;
			int yOffset=25;
			int y=127;
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=575;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Notice of Privacy Policies",12f,sheet.FontName,true,332,65,220,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Last Name:",sheet.FontSize,sheet.FontName,false,91,y,75,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("LName",sheet.FontSize,sheet.FontName,false,166,y,150,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("First Name:",sheet.FontSize,sheet.FontName,false,321,y,75,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("FName",sheet.FontSize,sheet.FontName,false,396,y,150,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Birthdate:",sheet.FontSize,sheet.FontName,false,551,y,65,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Birthdate",sheet.FontSize,sheet.FontName,false,616,y,145,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date: [dateToday]",sheet.FontSize,sheet.FontName,false,92,135+yOffset,120,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("I have had full opportunity to read and consider the contents of  the Notice of Privacy Practices.  I understand that I am giving my permission to your use and disclosure of my protected health information in order to carry out treatment, payment activities, and healthcare operations.  I also understand that I have the right to revoke permission.",sheet.FontSize,sheet.FontName,false,92,167+yOffset,670,80));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewSigBox(261,295+yOffset,364,81));
			
			return sheet;
		}

		private static SheetDef MedicalHistSimple() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.MedicalHistory);
			sheet.Description="Medical History Simple";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=18;
			int y=60;
			int x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Medical History",12f,sheet.FontName,true,345,y,180,20));
			y=105;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Last Name:",sheet.FontSize,sheet.FontName,false,76,y,75,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("LName",sheet.FontSize,sheet.FontName,false,151,y,155,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("First Name:",sheet.FontSize,sheet.FontName,false,311,y,76,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("FName",sheet.FontSize,sheet.FontName,false,387,y,155,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Birthdate:",sheet.FontSize,sheet.FontName,false,547,y,65,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Birthdate",sheet.FontSize,sheet.FontName,false,612,y,145,rowH));
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Name of Medical Doctor:",sheet.FontSize,sheet.FontName,false,x,y,155,rowH));
			x=230;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,265,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,265,rowH));
			x=500;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("City/State:",sheet.FontSize,sheet.FontName,false,x,y,67,rowH));
			x=567;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,190,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,190,rowH));
			x=75;
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Emergency Contact",sheet.FontSize,sheet.FontName,false,x,y,124,rowH));
			x=199;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,138,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,138,rowH));
			x=342;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Phone",sheet.FontSize,sheet.FontName,false,x,y,44,rowH));
			x=386;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,99,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,99,rowH));
			x=490;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Relationship",sheet.FontSize,sheet.FontName,false,x,y,80,rowH));
			x=570;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,187,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,187,rowH));
			x=75;
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("List all medications or drugs you are now taking:",sheet.FontSize,sheet.FontName,false,x,y,292,rowH));
			y+=rowH+1;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+2,11,11));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("misc",x+1,y+3,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("None",sheet.FontSize,sheet.FontName,false,x+13,y,37,rowH));
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,682,140));
			y+=142;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("List all medications or drugs you are allergic to:",sheet.FontSize,sheet.FontName,false,x,y,286,rowH));
			y+=rowH+1;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+2,11,11));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("misc",x+1,y+3,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("None",sheet.FontSize,sheet.FontName,false,x+13,y,37,rowH));
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,682,140));
			y+=142;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("List any medical conditions you may have including: asthma, bleeding problems, cancer, diabetes, heart murmur, heart trouble, high blood pressure, joint replacement, kidney disease, liver disease, pregnancy, psychiatric treatment, sinus trouble, stroke, ulcers, or history of  rheumatic fever or of taking fen-phen:",sheet.FontSize,sheet.FontName,false,x,y,682,55));
			y+=56;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+2,11,11));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("misc",x+1,y+3,10,10));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("None",sheet.FontSize,sheet.FontName,false,x+13,y,37,rowH));
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,682,140));
			y+=142;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Tobacco use?  If so, what kind and how much?",sheet.FontSize,sheet.FontName,false,x,y,289,rowH));
			x=364;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,393,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,393,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Unusual reaction to dental injections?",sheet.FontSize,sheet.FontName,false,x,y,232,rowH));
			x=307;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,450,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,450,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Reason for today's visit",sheet.FontSize,sheet.FontName,false,x,y,145,rowH));
			x=220;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,275,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,275,rowH));
			x=500;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Are you in pain?",sheet.FontSize,sheet.FontName,false,x,y,103,rowH));
			x=603;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,154,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,154,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("New patients:",sheet.FontSize,sheet.FontName,false,x,y,87,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have a Panoramic x-ray or Full Mouth x-rays that are less than 5 years old?",sheet.FontSize,sheet.FontName,false,x,y,507,rowH));
			x=602;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,155,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,155,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have BiteWing x-rays that are less than 1 year old?",sheet.FontSize,sheet.FontName,false,x,y,360,rowH));
			x=455;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,302,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,302,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Name of former dentist",sheet.FontSize,sheet.FontName,false,x,y,143,rowH));
			x=238;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,275,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,275,rowH));
			x=518;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("City/State",sheet.FontSize,sheet.FontName,false,x,y,64,rowH));
			x=582;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,175,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,175,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date of last cleaning and exam",sheet.FontSize,sheet.FontName,false,x,y,192,rowH));
			x=287;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,470,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,470,rowH));
			y+=40;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date: [dateToday]",sheet.FontSize,sheet.FontName,false,x,y,120,rowH));
			y+=40;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewSigBox(261,y,364,81));
			return sheet;
		}

		private static SheetDef MedicalHistNewPat() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.MedicalHistory);
			sheet.Description="Medical History New Patient";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=18;
			int y=60;
			int x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Medical History for New Patient",12f,sheet.FontName,true,312,y,275,20));
			y=105;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Last Name:",sheet.FontSize,sheet.FontName,false,76,y,75,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("LName",sheet.FontSize,sheet.FontName,false,151,y,155,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("First Name:",sheet.FontSize,sheet.FontName,false,311,y,76,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("FName",sheet.FontSize,sheet.FontName,false,387,y,155,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Birthdate:",sheet.FontSize,sheet.FontName,false,547,y,65,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Birthdate",sheet.FontSize,sheet.FontName,false,612,y,145,rowH));
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Name of Medical Doctor:",sheet.FontSize,sheet.FontName,false,x,y,155,rowH));
			x=230;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,265,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,265,rowH));
			x=500;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("City/State:",sheet.FontSize,sheet.FontName,false,x,y,67,rowH));
			x=567;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,190,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,190,rowH));
			x=75;
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Emergency Contact",sheet.FontSize,sheet.FontName,false,x,y,124,rowH));
			x=199;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,138,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,138,rowH));
			x=342;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Phone",sheet.FontSize,sheet.FontName,false,x,y,44,rowH));
			x=386;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,99,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,99,rowH));
			x=490;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Relationship",sheet.FontSize,sheet.FontName,false,x,y,80,rowH));
			x=570;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,187,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,187,rowH));
			x=75;
			y+=rowH+15;
			#region medications
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("List all medications that you are now taking:",sheet.FontSize,sheet.FontName,false,x,y,400,rowH));
			y+=rowH+8;
			int inputW=327;
			int inputGap=355;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed1",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed6",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed2",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed7",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed3",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed8",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed4",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed9",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed5",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed10",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			#endregion
			x=75;
			y+=rowH+15;
			#region allergies
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Are you allergic to any of the following?",sheet.FontSize,sheet.FontName,false,x,y,400,rowH));
			y+=rowH+3;
			//x+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=316;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x=75;
			y+=rowH;
			MedicalCheckboxRowYN(sheet,"allergy:","Anesthetic","Iodine",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"allergy:","Aspirin","Latex",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"allergy:","Codeine","Penicillin",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"allergy:","Ibuprofen","Sulfa",x,y);
			#endregion
			y+=rowH+15;
			#region problems
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have any of the following medical conditions?",sheet.FontSize,sheet.FontName,false,x,y,400,rowH));
			y+=rowH+3;
			//x+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=316;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x=75;
			y+=rowH;
			MedicalCheckboxRowYN(sheet,"problem:","Asthma","Kidney Disease",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Bleeding Problems","Liver Disease",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Cancer","Pregnancy",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Diabetes","Psychiatric Treatment",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Heart Murmur","Sinus Trouble",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Heart Trouble","Stroke",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","High Blood Pressure","Ulcers",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Joint Replacement","Rheumatic Fever",x,y);
			#endregion
			y+=rowH+18;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Tobacco use?  If so, what kind and how much?",sheet.FontSize,sheet.FontName,false,x,y,289,rowH));
			x=364;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,393,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,393,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Unusual reaction to dental injections?",sheet.FontSize,sheet.FontName,false,x,y,232,rowH));
			x=307;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,450,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,450,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Reason for today's visit",sheet.FontSize,sheet.FontName,false,x,y,145,rowH));
			x=220;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,275,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,275,rowH));
			x=500;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Are you in pain?",sheet.FontSize,sheet.FontName,false,x,y,103,rowH));
			x=603;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,154,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,154,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("New patients:",sheet.FontSize,sheet.FontName,false,x,y,87,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have a Panoramic x-ray or Full Mouth x-rays that are less than 5 years old?",sheet.FontSize,sheet.FontName,false,x,y,507,rowH));
			x=602;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,155,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,155,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have BiteWing x-rays that are less than 1 year old?",sheet.FontSize,sheet.FontName,false,x,y,360,rowH));
			x=455;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,302,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,302,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Name of former dentist",sheet.FontSize,sheet.FontName,false,x,y,143,rowH));
			x=238;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,275,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,275,rowH));
			x=518;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("City/State",sheet.FontSize,sheet.FontName,false,x,y,64,rowH));
			x=582;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,175,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,175,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date of last cleaning and exam",sheet.FontSize,sheet.FontName,false,x,y,192,rowH));
			x=287;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,470,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,470,rowH));
			y+=40;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date: [dateToday]",sheet.FontSize,sheet.FontName,false,x,y,120,rowH));
			y+=40;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewSigBox(261,y,364,81));
			return sheet;
		}

		private static SheetDef MedicalHistUpdate() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.MedicalHistory);
			sheet.Description="Medical History Update";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=18;
			int y=60;
			int x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Medical History Update",12f,sheet.FontName,true,325,y,220,20));
			y=105;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Last Name:",sheet.FontSize,sheet.FontName,false,76,y,75,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("LName",sheet.FontSize,sheet.FontName,false,151,y,155,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("First Name:",sheet.FontSize,sheet.FontName,false,311,y,76,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("FName",sheet.FontSize,sheet.FontName,false,387,y,155,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Birthdate:",sheet.FontSize,sheet.FontName,false,547,y,65,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("Birthdate",sheet.FontSize,sheet.FontName,false,612,y,145,rowH));
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Name of Medical Doctor:",sheet.FontSize,sheet.FontName,false,x,y,155,rowH));
			x=230;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,265,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,265,rowH));
			x=500;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("City/State:",sheet.FontSize,sheet.FontName,false,x,y,67,rowH));
			x=567;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,190,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,190,rowH));
			x=75;
			y+=rowH+2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Emergency Contact",sheet.FontSize,sheet.FontName,false,x,y,124,rowH));
			x=199;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,138,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,138,rowH));
			x=342;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Phone",sheet.FontSize,sheet.FontName,false,x,y,44,rowH));
			x=386;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,99,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,99,rowH));
			x=490;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Relationship",sheet.FontSize,sheet.FontName,false,x,y,80,rowH));
			x=570;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,187,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,187,rowH));
			x=75;
			y+=rowH+15;
			#region medications
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Mark any medications that you are no longer taking and add any new ones:",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH+8;
			int inputW=300;
			int inputGap=329;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed1","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed1",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed6","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed6",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed2","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed2",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed7","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed7",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed3","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed3",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed8","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed8",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed4","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed4",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed9","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed9",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed5","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed5",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x+=inputGap;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,y+3,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton("checkMed10","N",x,y+5,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("inputMed10",sheet.FontSize,sheet.FontName,false,x,y,inputW,18));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+18,inputW,0));
			x=75;
			#endregion
			x=75;
			y+=rowH+15;
			#region allergies
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Are you allergic to any of the following?",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH+3;
			//x+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=316;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x=75;
			y+=rowH;
			MedicalCheckboxRowYN(sheet,"allergy:","Anesthetic","Iodine",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"allergy:","Aspirin","Latex",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"allergy:","Codeine","Penicillin",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"allergy:","Ibuprofen","Sulfa",x,y);
			#endregion
			y+=rowH+15;
			#region problems
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have any of the following medical conditions?",sheet.FontSize,sheet.FontName,false,x,y,500,rowH));
			y+=rowH+3;
			//x+=3;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=316;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Y",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x+=21;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("N",sheet.FontSize,sheet.FontName,false,x,y,16,14));
			x=75;
			y+=rowH;
			MedicalCheckboxRowYN(sheet,"problem:","Asthma","Kidney Disease",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Bleeding Problems","Liver Disease",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Cancer","Pregnancy",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Diabetes","Psychiatric Treatment",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Heart Murmur","Sinus Trouble",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Heart Trouble","Stroke",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","High Blood Pressure","Ulcers",x,y);
			y+=rowH+4;
			MedicalCheckboxRowYN(sheet,"problem:","Joint Replacement","Rheumatic Fever",x,y);
			#endregion
			y+=rowH+18;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Tobacco use?  If so, what kind and how much?",sheet.FontSize,sheet.FontName,false,x,y,289,rowH));
			x=364;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,393,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,393,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Unusual reaction to dental injections?",sheet.FontSize,sheet.FontName,false,x,y,232,rowH));
			x=307;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,450,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,450,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Reason for today's visit",sheet.FontSize,sheet.FontName,false,x,y,145,rowH));
			x=220;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,275,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,275,rowH));
			x=500;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Are you in pain?",sheet.FontSize,sheet.FontName,false,x,y,103,rowH));
			x=603;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,154,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,154,rowH));
			y+=rowH+2;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("New patients:",sheet.FontSize,sheet.FontName,false,x,y,87,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have a Panoramic x-ray or Full Mouth x-rays that are less than 5 years old?",sheet.FontSize,sheet.FontName,false,x,y,507,rowH));
			x=602;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,155,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,155,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Do you have BiteWing x-rays that are less than 1 year old?",sheet.FontSize,sheet.FontName,false,x,y,360,rowH));
			x=455;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,302,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,302,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Name of former dentist",sheet.FontSize,sheet.FontName,false,x,y,143,rowH));
			x=238;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,275,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,275,rowH));
			x=518;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("City/State",sheet.FontSize,sheet.FontName,false,x,y,64,rowH));
			x=582;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,175,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,175,rowH));
			y+=rowH+2;
			x=95;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date of last cleaning and exam",sheet.FontSize,sheet.FontName,false,x,y,192,rowH));
			x=287;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x,y+rowH,470,0));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,470,rowH));
			y+=40;
			x=75;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date: [dateToday]",sheet.FontSize,sheet.FontName,false,x,y,120,rowH));
			y+=40;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewSigBox(261,y,364,81));
			return sheet;
		}

		public static SheetDef LabSlip() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.LabSlip);
			sheet.Description="Lab Slip";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=10f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=18;
			int x=75;
			int y=50;
			//Title----------------------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Lab Slip",12f,sheet.FontName,true,270,y,200,22));
			y+=35;
			//Lab-----------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("lab.Description",sheet.FontSize,sheet.FontName,true,x,y,300,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("lab.Address",sheet.FontSize,sheet.FontName,false,x,y,300,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("lab.CityStZip",sheet.FontSize,sheet.FontName,false,x,y,300,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("lab.Phone",sheet.FontSize,sheet.FontName,false,x,y,300,rowH));
			y+=rowH;
			y+=15;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date:  [dateToday]",sheet.FontSize,sheet.FontName,false,x,y,140,rowH));
			y+=rowH;
			//Prov-----------------------------------------------------------------------
			y+=15;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Doctor:",sheet.FontSize,sheet.FontName,false,x,y,50,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("prov.nameFormal",sheet.FontSize,sheet.FontName,false,x+50,y,300,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("License No:",sheet.FontSize,sheet.FontName,false,x,y,78,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("prov.stateLicence",sheet.FontSize,sheet.FontName,false,x+78,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Address:  [clinicAddress],  [clinicCityStZip]",sheet.FontSize,sheet.FontName,false,x,y,600,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Phone:  [clinicPhone]",sheet.FontSize,sheet.FontName,false,x,y,300,rowH));
			y+=rowH;
			//Patient-----------------------------------------------------------------------
			y+=15;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Patient:  [nameFL]",sheet.FontSize,sheet.FontName,false,x,y,300,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Age: [age]      Gender: [gender]",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Due Date/Time:",sheet.FontSize,sheet.FontName,false,x,y,100,rowH));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("labcase.DateTimeDue",sheet.FontSize,sheet.FontName,false,x+100,y,200,rowH));
			y+=rowH;
			//Instructions-----------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Instructions:",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("labcase.Instructions",sheet.FontSize,sheet.FontName,false,x,y,600,200));
			y+=220;
			//sig-------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Dr. Signature:",sheet.FontSize,sheet.FontName,false,x,y,200,rowH));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewSigBox(x,y,364,81));
			return sheet;
		}

		public static SheetDef ExamSheet(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.ExamSheet);
			sheet.Description="Exam";
			sheet.FontName="Microsoft Sans Serif";
			sheet.FontSize=11f;
			sheet.Width=850;
			sheet.Height=1100;
			int rowH=25;
			int y=100;
			//Title----------------------------------------------------------------------------------------------------------
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Exam for [nameFL]",12f,sheet.FontName,true,275,y,325,20));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("sheet.DateTimeSheet",12f,sheet.FontName,false,350,y,180,20));
			y+=rowH;
			int x=100;
			y+=30;
			string[] fieldText=new string[] {"TMJ","Neck","Tongue","Palate","Floor of Mouth"};
			for(int i=0;i<fieldText.Length;i++){
				sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(fieldText[i],sheet.FontSize,sheet.FontName,false,x,y,120,20));
				sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x+120,y+2,15,15));
				sheet.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("misc",x+121,y+4,13,13));
				sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("WNL",sheet.FontSize,sheet.FontName,false,x+140,y,40,20));
				//sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(x+200,y+22,450,0));
				sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x+200,y,450,40));
				y+=45;//rowH;
			}
			y+=25;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Additional Notes",sheet.FontSize,sheet.FontName,false,x,y,140,20));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewInput("misc",sheet.FontSize,sheet.FontName,false,x,y,600,100));
			return sheet;
		}

		public static SheetDef DepositSlip(){
			SheetDef sheet=new SheetDef(SheetTypeEnum.DepositSlip);
			sheet.Description="Deposit Slip";
			sheet.FontName=FontFamily.GenericMonospace.Name;
			sheet.FontSize=9f;
			sheet.Width=850;
			sheet.Height=1100;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("deposit.DateDeposit",11f,sheet.FontName,false,89,156,120,17));
			//col 1, 6 boxes
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem01",8f,sheet.FontName,false,338,62,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem02",8f,sheet.FontName,false,338,90,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem03",8f,sheet.FontName,false,338,118,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem04",8f,sheet.FontName,false,338,146,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem05",8f,sheet.FontName,false,338,173,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem06",8f,sheet.FontName,false,338,201,100,20));
			//col 2, 7 boxes
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem07",8f,sheet.FontName,false,530,34,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem08",8f,sheet.FontName,false,530,62,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem09",8f,sheet.FontName,false,530,90,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem10",8f,sheet.FontName,false,530,118,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem11",8f,sheet.FontName,false,530,146,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem12",8f,sheet.FontName,false,530,173,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem13",8f,sheet.FontName,false,530,201,100,20));
			//col 3, 5 boxes
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem14",8f,sheet.FontName,false,720,34,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem15",8f,sheet.FontName,false,720,62,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem16",8f,sheet.FontName,false,720,90,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem17",8f,sheet.FontName,false,720,118,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItem18",8f,sheet.FontName,false,720,146,100,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositTotal",8f,sheet.FontName,false,720,173,100,20));
			//
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositItemCount",8f,sheet.FontName,false,556,275,50,20));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositTotal",8f,sheet.FontName,false,720,275,130,20));
			int rowH=20;
			int y=399;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Deposit Slip",16f,sheet.FontName,true,323,y,300,30));
			y+=30;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("[practiceTitle]",10f,sheet.FontName,true,323,y,300,20));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("deposit.DateDeposit",10f,sheet.FontName,false,352,y,200,20));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date: [dateToday]",8f,sheet.FontName,false,50,y,160,20));
			y+=rowH;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,y,750,0));
			y+=4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositList",8f,sheet.FontName,false,75,y,700,20,GrowthBehaviorEnum.DownGlobal));
			y+=26;//The actual y-value of the proceeding elements will be changed depending on the size of the depositList, since we are using DownGlobal growth.
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,y,750,0));
			y+=4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("depositTotal",8f,sheet.FontName,true,562,y,200,20));
			y+=rowH+4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(50,y,750,0));
			y+=4;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("deposit.BankAccountInfo",8f,sheet.FontName,true,50,y,700,0,GrowthBehaviorEnum.DownGlobal));
			y+=1;//The actual y-value of the proceeding elements will be changed depending on the size of the deposit.BankAccountInfo, since we are using DownGlobal growth.
			return sheet;
		}

		private static SheetDef StmtSheet() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.Statement);
			sheet.Description="Statement";
			sheet.FontName="Arial";
			sheet.FontSize=9f;
			sheet.Width=850;
			sheet.Height=1100;
			//From top to bottom
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("statementReceiptInvoice",15f,"Arial",true,310,60,230,22,HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("returnAddress",10f,"Arial",false,40,70,250,80,GrowthBehaviorEnum.DownLocal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("statement.DateSent",9f,"Arial",false,310,83,230,13,HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("accountNumber",9f,"Arial",false,310,97,230,13,HorizontalAlignment.Center));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewGrid("StatementEnclosed",445,120,321,31));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("CREDIT CARD TYPE",8f,"Arial",false,445,185,125,14,isPaymentOption:true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(566,201,200,0,true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("#",8f,"Arial",false,445,210,12,14,isPaymentOption:true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(458,226,308,0,true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("3 DIGIT CSV",8f,"Arial",false,445,235,75,14,isPaymentOption:true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("billingAddress",10f,"Arial",false,85,240,250,60));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(521,251,245,0,true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("EXPIRES",8f,"Arial",false,445,260,59,14,isPaymentOption:true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(505,276,261,0,true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("AMOUNT APPROVED",8f,"Arial",false,445,285,127,14,isPaymentOption:true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(573,301,193,0,true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("NAME",8f,"Arial",false,445,310,42,14,isPaymentOption:true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(488,326,278,0,true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("SIGNATURE",8f,"Arial",false,445,335,80,14,isPaymentOption:true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(521,351,245,0,true));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewLine(0,359,850,0,true,KnownColor.LightGray));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("PLEASE DETACH AND RETURN THE UPPER PORTION WITH YOUR PAYMENT",6f,"Arial",false,225,360,400,10,isPaymentOption:true,itemColor:KnownColor.LightGray));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("totalLabel",9f,"Arial",false,554,385,150,14,HorizontalAlignment.Right));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("totalValue",9f,"Arial",false,705,385,80,14,HorizontalAlignment.Right));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewGrid("StatementAging",60,444,650,31));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("insEstLabel",9f,"Arial",false,554,400,150,14,HorizontalAlignment.Right));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("insEstValue",9f,"Arial",false,705,400,80,14,HorizontalAlignment.Right));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("balanceLabel",9f,"Arial",true,554,415,150,14,HorizontalAlignment.Right));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("balanceValue",9f,"Arial",true,705,415,80,14,HorizontalAlignment.Right));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("statement.NoteBold",10f,"Arial",true,50,490,725,14,GrowthBehaviorEnum.DownGlobal,KnownColor.DarkRed));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewGrid("StatementPayPlan",170,506,510,49));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewGrid("StatementDynamicPayPlan",170,571,510,49));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewGrid("StatementMain",65,646,720,31));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("futureAppointments",9f,"Arial",false,50,683,725,14,GrowthBehaviorEnum.DownGlobal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("statement.Note",9f,"Arial",false,50,698,725,14,GrowthBehaviorEnum.DownGlobal));
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewOutput("statement.NoteBold",10f,"Arial",true,50,713,725,14,GrowthBehaviorEnum.DownGlobal,KnownColor.DarkRed));
			return sheet;
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

		///<summary>This will add two check box rows.  Set the prefix string to "allergy:" or "problem:", item1 and item2 should be set to the desired allergy/med in the row. Leave item2 blank for just one check box in the row.</summary>
		private static void MedicalCheckboxRowYN(SheetDef sheet,string prefix,string item1,string item2,int x,int y) {
			int rectH=y+3;
			int checkH=y+5;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,rectH,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton(prefix+item1,"Y",x,checkH,11,11));
			x+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,rectH,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton(prefix+item1,"N",x,checkH,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(item1,sheet.FontSize,sheet.FontName,false,x,y,280,18));
			x+=290;
			if(item2=="") {
				return;
			}
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,rectH,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton(prefix+item2,"Y",x,checkH,11,11));
			x+=19;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRect(x,rectH,14,14));
			x+=2;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewRadioButton(prefix+item2,"N",x,checkH,11,11));
			x+=24;
			sheet.SheetFieldDefs.Add(SheetFieldDef.NewStaticText(item2,sheet.FontSize,sheet.FontName,false,x,y,280,18));
		}

		///<summary>Deprecated. Now stored in XML resource file.</summary>
		private static SheetDef TreatmentPlan() {
			//SheetDef sheet=new SheetDef(SheetTypeEnum.TreatmentPlan);
			SheetDef sheet=new SheetDef(SheetTypeEnum.TreatmentPlan);
			sheet.Description="Treatment Plan";
			sheet.FontName="Arial";
			sheet.FontSize=9f;
			sheet.Width=850;
			sheet.Height=1100;
			using(Font fontTitle  =new Font("Arial",13f,FontStyle.Bold,GraphicsUnit.Point))
			using(Font fontHeading=new Font("Arial",10f,FontStyle.Bold,GraphicsUnit.Point))
			using(Font fontBody  =new Font("Arial",9f,FontStyle.Regular,GraphicsUnit.Point))
			using(Font fontGrid  =new Font("Arial",8.5f,FontStyle.Regular,GraphicsUnit.Point))  
			{
				sheet.SheetFieldDefs=new List<SheetFieldDef> {
					//Heading---------------------------------------------------------------------------------------------------------------	
					new SheetFieldDef(SheetFieldType.OutputText,225,60,400,22,fontTitle) {FieldName="Heading",TextAlign=HorizontalAlignment.Center,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,225,88,400,20,fontHeading) {FieldName="defaultHeading",TextAlign=HorizontalAlignment.Center,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					//Graphic---------------------------------------------------------------------------------------------------------------
					new SheetFieldDef(SheetFieldType.Special,175,114,500,370) {FieldName="toothChart"},
					new SheetFieldDef(SheetFieldType.StaticText,120,280,50,44,fontBody,"Your\r\nRight") {TextAlign=HorizontalAlignment.Center},
					new SheetFieldDef(SheetFieldType.StaticText,680,280,50,44,fontBody,"Your\r\nLeft") {TextAlign=HorizontalAlignment.Center},
					new SheetFieldDef(SheetFieldType.Special,100,500,650,14) {FieldName="toothChartLegend"}, 
					//Grids---------------------------------------------------------------------------------------------------------------
					new SheetFieldDef(SheetFieldType.Grid,51,520,845,31,fontGrid) {FieldName="TreatPlanMain",GrowthBehavior=GrowthBehaviorEnum.DownGlobal}, 
					new SheetFieldDef(SheetFieldType.Grid,275,556,300,49,fontGrid) {FieldName="TreatPlanBenefitsFamily",GrowthBehavior=GrowthBehaviorEnum.DownGlobal}, 
					new SheetFieldDef(SheetFieldType.Grid,275,609,300,49,fontGrid) {FieldName="TreatPlanBenefitsIndividual",GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,50,664,750,50,fontBody) {FieldName="Note",GrowthBehavior=GrowthBehaviorEnum.DownGlobal}, 
					new SheetFieldDef(SheetFieldType.SigBox,244,720,362,79)//matches size of FormTPsign sig box control
				};
			}//end using
			return sheet;
		}

		private static SheetDef Screening() {
			SheetDef sheetDef=new SheetDef(SheetTypeEnum.Screening);
			sheetDef.Description="Screening";
			sheetDef.FontName="Microsoft Sans Serif";
			sheetDef.FontSize=9f;
			sheetDef.Width=850;
			sheetDef.Height=1100;
			int rowH=18;
			int y=60;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Screening",12f,sheetDef.FontName,true,312,y,275,20));
			y=105;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Last Name:",sheetDef.FontSize,sheetDef.FontName,false,76,y,75,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewInput("LName",sheetDef.FontSize,sheetDef.FontName,false,151,y,155,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("First Name:",sheetDef.FontSize,sheetDef.FontName,false,311,y,76,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewInput("FName",sheetDef.FontSize,sheetDef.FontName,false,387,y,155,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Middle I:",sheetDef.FontSize,sheetDef.FontName,false,547,y,65,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewInput("MiddleI",sheetDef.FontSize,sheetDef.FontName,false,612,y,145,rowH));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Gender:",sheetDef.FontSize,sheetDef.FontName,false,76,y,75,rowH));
			string fieldValue=";"+String.Join("|",Enum.GetNames(typeof(PatientGender)));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewComboBox("Gender",fieldValue,151,y));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Birthdate:",sheetDef.FontSize,sheetDef.FontName,false,311,y,76,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewInput("Birthdate",sheetDef.FontSize,sheetDef.FontName,false,387,y,155,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Age:",sheetDef.FontSize,sheetDef.FontName,false,547,y,65,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewInput("Age",sheetDef.FontSize,sheetDef.FontName,false,612,y,145,rowH));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Description:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewOutput("Description",sheetDef.FontSize,sheetDef.FontName,false,235,y,500,rowH));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Screener:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewOutput("ProvName",sheetDef.FontSize,sheetDef.FontName,false,235,y,500,rowH));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("County:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewOutput("County",sheetDef.FontSize,sheetDef.FontName,false,235,y,500,rowH));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Date of Screening:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewOutput("DateScreenGroup",sheetDef.FontSize,sheetDef.FontName,false,235,y,500,rowH));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Place of Service:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			fieldValue=";"+String.Join("|",Enum.GetNames(typeof(PlaceOfService)));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewComboBox("PlaceOfService",fieldValue,235,y));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Urgency:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			fieldValue=";"+String.Join("|",Enum.GetNames(typeof(TreatmentUrgency)));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewComboBox("Urgency",fieldValue,235,y));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Race / Ethnicity:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			fieldValue=";"+String.Join("|",Enum.GetNames(typeof(PatientRaceOld)));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewComboBox("Race/Ethnicity",fieldValue,235,y));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Grade Level:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			fieldValue=";"+String.Join("|",Enum.GetNames(typeof(PatientGrade)));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewComboBox("GradeLevel",fieldValue,235,y));
			y+=rowH+2;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Preferred:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewInput("Preferred",sheetDef.FontSize,sheetDef.FontName,false,235,y,500,rowH));
			y+=rowH+20;
			//Proc buttons.  One for D0191 and one for D1206
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Assessment Proc:",sheetDef.FontSize,sheetDef.FontName,false,60,y-5,100,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("AssessmentProc",170,y,10,10));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewRect(169,y-1,11,11));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Fluoride Proc:",sheetDef.FontSize,sheetDef.FontName,false,200,y-5,80,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewCheckBox("FluorideProc",290,y,10,10));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewRect(289,y-1,11,11));
			y+=rowH+5;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Screening Chart",12f,sheetDef.FontName,true,312,y,500,20));
			y+=rowH+20;
			//X,Y,Z; is the pattern that is used, per tooth.  Each surface on the tooth is separated by a comma.  #; at the beginning indicates if it's a primary tooth chart or not.
			fieldValue="0;d,m,ling;d,m,ling;,,;,,;,,;,,;m,d,ling;m,d,ling;m,d,buc;m,d,buc;,,;,,;,,;,,;d,m,buc;d,m,buc";
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewScreenChart("ChartSealantTreatment",fieldValue,60,y));
			y+=rowH+110;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewRect(76,y,200,150));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("CODE:",sheetDef.FontSize,sheetDef.FontName,true,86,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("S = Sealant Needed",sheetDef.FontSize,sheetDef.FontName,false,86,y+20,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("PS = Previously Sealed/Intact",sheetDef.FontSize,sheetDef.FontName,false,86,y+40,190,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("C = Carious",sheetDef.FontSize,sheetDef.FontName,false,86,y+60,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("F = Filled",sheetDef.FontSize,sheetDef.FontName,false,86,y+80,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("NFE = Not Fully Erupted",sheetDef.FontSize,sheetDef.FontName,false,86,y+100,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("NN = Not Needed",sheetDef.FontSize,sheetDef.FontName,false,86,y+120,150,rowH));
			y+=160;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Sealant Placement",12f,sheetDef.FontName,true,312,y,500,20));
			y+=rowH+20;
			//X,Y,Z; is the pattern that is used, per tooth.  Each surface on the tooth is separated by a comma.  #; at the beginning indicates if it's a primary tooth chart or not.
			fieldValue="0;d,m,ling;d,m,ling;,,;,,;,,;,,;m,d,ling;m,d,ling;m,d,buc;m,d,buc;,,;,,;,,;,,;d,m,buc;d,m,buc";
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewScreenChart("ChartSealantComplete",fieldValue,60,y));
			y+=rowH+110;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewRect(76,y,400,50));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("CODE:",sheetDef.FontSize,sheetDef.FontName,true,86,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("S = Mark each tooth where a sealant was placed with an \"S\"",sheetDef.FontSize,sheetDef.FontName,false,86,y+20,350,rowH));
			y+=70;
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewStaticText("Comments:",sheetDef.FontSize,sheetDef.FontName,false,76,y,150,rowH));
			sheetDef.SheetFieldDefs.Add(SheetFieldDef.NewInput("Comments",sheetDef.FontSize,sheetDef.FontName,false,235,y,500,500));
			return sheetDef;
		}

		///<summary>Deprecated. Now stored in XML resource file.</summary>
		private static SheetDef PayPlanAgreement() {
			SheetDef sheet=new SheetDef(SheetTypeEnum.PaymentPlan);
			sheet.Description="Payment Plan";
			sheet.FontName="Arial";
			sheet.FontSize=9f;
			sheet.Width=850;
			sheet.Height=1100;
			using(Font fontTitle = new Font("Tahoma",17f,FontStyle.Bold,GraphicsUnit.Point))
			using(Font fontHeading = new Font("Tahoma",10f,FontStyle.Bold,GraphicsUnit.Point))
			using(Font fontBody = new Font("Tahoma",9f,FontStyle.Regular,GraphicsUnit.Point)){
				sheet.SheetFieldDefs=new List<SheetFieldDef> {
					//Heading---------------------------------------------------------------------------------------------------------------
					new SheetFieldDef(SheetFieldType.StaticText,215,50,400,30,fontTitle,"Payment Plan Terms") {TextAlign=HorizontalAlignment.Center},
					new SheetFieldDef(SheetFieldType.OutputText,290,84,250,20,fontHeading) {FieldName="PracticeTitle",TextAlign=HorizontalAlignment.Center,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,368,106,100,15,fontHeading) {FieldName="dateToday",TextAlign=HorizontalAlignment.Center,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					//Main-Static---------------------------------------------------------------------------------------------------------------
					new SheetFieldDef(SheetFieldType.StaticText,175,210,160,22,fontBody,"Patient") {TextAlign=HorizontalAlignment.Left},
					new SheetFieldDef(SheetFieldType.StaticText,175,240,160,22,fontBody,"Guarantor") {TextAlign=HorizontalAlignment.Left},
					new SheetFieldDef(SheetFieldType.StaticText,175,270,160,22,fontBody,"Date of Agreement") {TextAlign=HorizontalAlignment.Left},
					new SheetFieldDef(SheetFieldType.StaticText,175,300,160,22,fontBody,"Principal") {TextAlign=HorizontalAlignment.Left},
					new SheetFieldDef(SheetFieldType.StaticText,175,330,160,22,fontBody,"Annual Percentage Rate") {TextAlign=HorizontalAlignment.Left},
					new SheetFieldDef(SheetFieldType.StaticText,175,360,160,22,fontBody,"Total Finance Charges") {TextAlign=HorizontalAlignment.Left},
					new SheetFieldDef(SheetFieldType.StaticText,175,390,160,22,fontBody,"Total Cost of Loan") {TextAlign=HorizontalAlignment.Left},
					////Main-Output---------------------------------------------------------------------------------------------------------------
					new SheetFieldDef(SheetFieldType.OutputText,475,210,160,22,fontBody) {FieldName="nameLF",TextAlign=HorizontalAlignment.Right,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,475,240,160,22,fontBody) {FieldName="guarantor",TextAlign=HorizontalAlignment.Right,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,475,270,160,22,fontBody) {FieldName="DateOfAgreement",TextAlign=HorizontalAlignment.Right,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,475,300,160,22,fontBody) {FieldName="Principal",TextAlign=HorizontalAlignment.Right,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,475,330,160,22,fontBody) {FieldName="APR",TextAlign=HorizontalAlignment.Right,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,475,360,160,22,fontBody) {FieldName="totalFinanceCharge",TextAlign=HorizontalAlignment.Right,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.OutputText,475,390,160,22,fontBody) {FieldName="totalCostOfLoan",TextAlign=HorizontalAlignment.Right,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					////Grids---------------------------------------------------------------------------------------------------------------
					new SheetFieldDef(SheetFieldType.Grid,80,455,720,30,fontBody) {FieldName="PayPlanMain",GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					////Notes and Signature---------------------------------------------------------------------------------------------------------------
					new SheetFieldDef(SheetFieldType.OutputText,175,525,550,50,fontBody) {FieldName="Note",TextAlign=HorizontalAlignment.Left,GrowthBehavior=GrowthBehaviorEnum.DownGlobal},
					new SheetFieldDef(SheetFieldType.StaticText,175,725,550,20,fontBody,"Signature of Guarantor: _____________________________________________________") {TextAlign=HorizontalAlignment.Left}
				};
			}//end using
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
