using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	///<summary></summary>
	public class LabelSingle{
		//private PrintDocument pd;
		//private Patient Pat;
		//private Carrier CarrierCur;
		//private Referral ReferralCur;

		///<summary></summary>
		public LabelSingle(){
			
		}

		///<summary></summary>
		public static void PrintPat(long patNum) {
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientMail);
			if(PrefC.GetLong(PrefName.LabelPatientDefaultSheetDefNum)!=0) {//Try to use custom label sheet.
				try {
					sheetDef=SheetDefs.GetSheetDef(PrefC.GetLong(PrefName.LabelPatientDefaultSheetDefNum));
				}
				catch {//The default label could not be retrieved so just use the internal sheet.
				}
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			SheetFiller.FillFields(sheet);
			try{
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		public static void PrintCustomPatient(long patNum,SheetDef sheetDef) {
			SheetDefs.GetFieldsAndParameters(sheetDef);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		public static void PrintPatientLFAddress(long patNum) {
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientLFAddress);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		public static void PrintPatientLFChartNumber(long patNum) {
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientLFChartNumber);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		public static void PrintPatientLFPatNum(long patNum) {
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientLFPatNum);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		public static void PrintPatRadiograph(long patNum) {
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelPatientRadiograph);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		public static void PrintText(long patNum,string text) {
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelText);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			sheet.Parameters.Add(new SheetParameter(false,"text"));
			SheetParameter.SetParameter(sheet,"text",text);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary></summary>
		public static void PrintCarriers(List<long> carrierNums) {
			SheetDef sheetDef;
			List<SheetDef> customSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.LabelCarrier);
			if(customSheetDefs.Count==0) {
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelCarrier);
			}
			else {
				sheetDef=customSheetDefs[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			List<Sheet> sheetBatch=SheetUtil.CreateBatch(sheetDef,carrierNums);
			try{
				SheetPrinting.PrintBatch(sheetBatch);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		///<summary></summary>
		public static void PrintCarrier(long carrierNum) {
			SheetDef sheetDef;
			List<SheetDef> customSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.LabelCarrier);
			if(customSheetDefs.Count==0){
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelCarrier);
			}
			else{
				sheetDef=customSheetDefs[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"CarrierNum",carrierNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary></summary>
		public static void PrintReferral(long referralNum) {
			SheetDef sheetDef;
			List<SheetDef> customSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.LabelReferral);
			if(customSheetDefs.Count==0){
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelReferral);
			}
			else{
				sheetDef=customSheetDefs[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"ReferralNum",referralNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		///<summary></summary>
		public static void PrintAppointment(long aptNum) {
			SheetDef sheetDef;
			List<SheetDef> customSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.LabelAppointment);
			if(customSheetDefs.Count==0){
				sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.LabelAppointment);
			}
			else{
				sheetDef=customSheetDefs[0];
				SheetDefs.GetFieldsAndParameters(sheetDef);
			}
			Sheet sheet=SheetUtil.CreateSheet(sheetDef);
			SheetParameter.SetParameter(sheet,"AptNum",aptNum);
			SheetFiller.FillFields(sheet);
			try {
				SheetPrinting.Print(sheet);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		

		

	}

}
