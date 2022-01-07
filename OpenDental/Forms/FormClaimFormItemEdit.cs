using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormClaimFormItemEdit : FormODBase,IFormClaimFormItemEdit {
		///<summary></summary>
		public string[] FieldNames { get; set; }
		///<summary></summary>
		public bool IsNew;
		///<summary>This is the claimformitem that is being currently edited in this window.</summary>
		public ClaimFormItem CFIcur;
		///<summary>Set to true if the Delete button was clicked.</summary>
		public bool IsDeleted;

		///<summary></summary>
		public FormClaimFormItemEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimFormItemEdit_Load(object sender, System.EventArgs e) {
			FillFieldNames();
			FillForm();
		}

		///<summary>This is called externally from Renaissance to error check each of the supplied fieldNames</summary>
		public void FillFieldNames(){
			FieldNames=new string[]
			{
				"FixedText",
				"IsPreAuth",
				"IsStandardClaim",
				"ShowPreauthorizationIfPreauth",
				"IsMedicaidClaim",
				"IsGroupHealthPlan",
				"PreAuthString",
				"PriorAuthString",
				"PriInsCarrierName",
				"PriInsAddress",
				"PriInsAddress2",
				"PriInsAddressComplete",
				"PriInsCity",
				"PriInsST",
				"PriInsZip",
				"OtherInsExists",
				"OtherInsNotExists",
				"OtherInsExistsDent",
				"OtherInsExistsMed",
				"OtherInsSubscrLastFirst",
				"OtherInsSubscrDOB",
				"OtherInsSubscrIsMale",
				"OtherInsSubscrIsFemale",
				"OtherInsSubscrIsGenderUnknown",
				"OtherInsSubscrGender",
				"OtherInsSubscrID",
				"OtherInsGroupNum",
				"OtherInsRelatIsSelf",
				"OtherInsRelatIsSpouse",
				"OtherInsRelatIsChild",
				"OtherInsRelatIsOther",
				"OtherInsCarrierName",
				"OtherInsAddress",
				"OtherInsCity",
				"OtherInsST",
				"OtherInsZip",
				"SubscrLastFirst",
				"SubscrAddress",
				"SubscrAddress2",
				"SubscrAddressComplete",
				"SubscrCity",
				"SubscrST",
				"SubscrZip",
				"SubscrPhone",
				"SubscrDOB",
				"SubscrIsMale",
				"SubscrIsFemale",
				"SubscrIsGenderUnknown",
				"SubscrIsMarried",
				"SubscrIsSingle",
				"SubscrID",
				"SubscrIDStrict",
				"SubscrIsFTStudent",
				"SubscrIsPTStudent",
				"SubscrGender",
				"GroupNum",
				"GroupName",
				"DivisionNo",
				"EmployerName",
				"RelatIsSelf",
				"RelatIsSpouse",
				"RelatIsChild",
				"RelatIsOther",
				"Relationship",
				"IsFTStudent",
				"IsPTStudent",
				"IsStudent",
				"CollegeName",
				"PatientLastFirst",
				"PatientLastFirstMiCommas",//Medical required format for UB04 printed claims
				"PatientFirstMiddleLast",
				"PatientFirstName",
				"PatientMiddleName",
				"PatientLastName",
				"PatientAddress",
				"PatientAddress2",
				"PatientAddressComplete",
				"PatientCity",
				"PatientST",
				"PatientZip",
				"PatientPhone",
				"PatientDOB",
				"PatientIsMale",
				"PatientIsFemale",
				"PatientIsGenderUnknown",
				"PatientGender",
				"PatientGenderLetter",
				"PatientIsMarried",
				"PatientIsSingle",
				"PatIDFromPatPlan",//Dependant Code in Canada
				"PatientSSN",
				"PatientMedicaidID",
				"PatientID-MedicaidOrSSN",
				"PatientPatNum",
				"PatientChartNum",
				"TotalFee",
				"Remarks",
				"PatientRelease",
				"PatientReleaseDate",
				"PatientAssignment",
				"PatientAssignmentDate",
				"PlaceIsOffice",
				"PlaceIsHospADA2002",
				"PlaceIsExtCareFacilityADA2002",
				"PlaceIsOtherADA2002",
				"PlaceIsInpatHosp",
				"PlaceIsOutpatHosp",
				"PlaceIsAdultLivCareFac",
				"PlaceIsSkilledNursFac",
				"PlaceIsPatientsHome",
				"PlaceIsOtherLocation",
				"PlaceNumericCode",
				"IsRadiographsAttached",
				"RadiographsNumAttached",
				"RadiographsNotAttached",
				"IsEnclosuresAttached",
				"AttachedImagesNum",
				"AttachedModelsNum",
				"IsNotOrtho",
				"IsOrtho",
				"DateOrthoPlaced",
				"MonthsOrthoRemaining",
				"MonthsOrthoTotal",
				"IsNotProsth",
				"IsInitialProsth",
				"IsNotReplacementProsth",
				"IsReplacementProsth",
				"DatePriorProsthPlaced",
				"IsOccupational",
				"IsNotOccupational",
				"IsAutoAccident",
				"IsNotAutoAccident",
				"IsOtherAccident",
				"IsNotOtherAccident",
				"IsNotAccident",
				"IsAccident",
				"AccidentDate",
				"AccidentST",
				"BillingDentist",
				"BillingDentistMedicaidID",
				"BillingDentistProviderID",
				"BillingDentistNPI",
				"BillingDentistLicenseNum",
				"BillingDentistSSNorTIN",
				"BillingDentistNumIsSSN",
				"BillingDentistNumIsTIN",
				"BillingDentistPh123",
				"BillingDentistPh456",
				"BillingDentistPh78910",
				"BillingDentistPhoneFormatted",
				"BillingDentistPhoneRaw",
				"PayToDentistAddress",
				"PayToDentistAddress2",
				"PayToDentistCity",
				"PayToDentistST",
				"PayToDentistZip",
				"PayToDentistPh123",
				"PayToDentistPh456",
				"PayToDentistPh78910",
				"PayToDentistPhoneFormatted",
				"PayToDentistPhoneRaw",
				"TreatingDentist",
				"TreatingDentistFName",
				"TreatingDentistLName",
				"TreatingDentistSignature",
				"TreatingDentistSigDate",
				"TreatingDentistMedicaidID",
				"TreatingDentistProviderID",
				"TreatingDentistNPI",
				"TreatingDentistLicense",
				"TreatingDentistAddress",
				"TreatingDentistAddress2",
				"TreatingDentistCity",
				"TreatingDentistST",
				"TreatingDentistZip",
				"TreatingDentistPh123",
				"TreatingDentistPh456",
				"TreatingDentistPh78910",
				"TreatingDentistPhoneRaw",
				"TreatingProviderSpecialty",
				"ReferringProvNPI",
				"ReferringProvNameFL",
				"DateService",
				"TotalPages",
				"MedInsCrossoverIndicator",
				"MedInsAName",
				"MedInsAPlanID",
				"MedInsARelInfo",
				"MedInsAAssignBen",
				"MedInsAPriorPmt",
				"MedInsAAmtDue",
				"MedInsAOtherProvID",
				"MedInsAInsuredName",
				"MedInsARelation",
				"MedInsAInsuredID",
				"MedInsAGroupName",
				"MedInsAGroupNum",
				"MedInsAAuthCode",
				"MedInsAEmployer",
				"MedInsBName",
				"MedInsBPlanID",
				"MedInsBRelInfo",
				"MedInsBAssignBen",
				"MedInsBPriorPmt",
				"MedInsBAmtDue",
				"MedInsBOtherProvID",
				"MedInsBInsuredName",
				"MedInsBRelation",
				"MedInsBInsuredID",
				"MedInsBGroupName",
				"MedInsBGroupNum",
				"MedInsBAuthCode",
				"MedInsBEmployer",
				"MedInsCName",
				"MedInsCPlanID",
				"MedInsCRelInfo",
				"MedInsCAssignBen",
				"MedInsCPriorPmt",
				"MedInsCAmtDue",
				"MedInsCOtherProvID",
				"MedInsCInsuredName",
				"MedInsCRelation",
				"MedInsCInsuredID",
				"MedInsCGroupName",
				"MedInsCGroupNum",
				"MedInsCAuthCode",
				"MedInsCEmployer",
				"MedUniformBillType",
				"MedAdmissionTypeCode",
				"MedAdmissionSourceCode",
				"MedPatientStatusCode",
				"MedAccidentCode",
				"ICDindicator",
				"AcceptAssignmentY",
				"AcceptAssignmentN",
				"ClaimIdentifier",
				"OrigRefNum",
				"CorrectionType",
				"DateIllnessInjuryPreg",
				"DateIllnessInjuryPregQualifier",
				"DateOther",
				"DateOtherQualifier",
				"IsOutsideLab",
				"IsNotOutsideLab",
				"OutsideLabFee"
			};
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				FieldNames=FieldNames.Concat(ListTools.FromSingle("OfficeNumber")).ToArray();
			}
			FieldNames=FieldNames.Concat(Enumerable.Range(1,32).Select(x => "Miss"+x))//Miss1-32
			.Concat(Enumerable.Range(18,11).Select(x => "MedConditionCode"+x))//MedConditionCode18-28
			.Concat(Enumerable.Range(39,3).SelectMany(x => Enumerable.Range(97,4)
					.SelectMany(y => new[] { "MedValCode"+x+(char)y,"MedValAmount"+x+(char)y })))//MedValCode39a-41d and MedValAmount39a-41d
			.Concat(Enumerable.Range(1,16).Select(x => "Diagnosis"+(x<5?x.ToString():((char)(x+60)).ToString())))//Diagnosis1-4 and DiagnosisA-L
			.OrderBy(x => x)//alphabatize the list before concatenating with the procedure (P1-P15...) fields.
			.Concat(Enumerable.Range(1,15).SelectMany(x => new[] {
				"Area",
				"Code",
				"CodeAndMods",
				"CodeMod1",
				"CodeMod2",
				"CodeMod3",
				"CodeMod4",
				"Date",
				"Description",
				"Diagnosis",
				"DiagnosisPoint",
				"eClaimNote",
				"Fee",
				"FeeMinusLab",
				"IsEmergency",
				"Lab",
				"Minutes",
				"PlaceNumericCode",
				"RevCode",
				"Surface",
				"System",
				"SystemAndTeeth",
				"ToothNumber",
				"ToothNumOrArea",
				"TreatDentMedicaidID",
				"TreatProvNPI",
				"UnitQty",
				"UnitQtyOrCount"
			}.Select(y => "P"+x+y)))//P1-15SystemAndTeeth..., 28 fields, these will be alphabatized at the end of the list of all fields
			.ToArray();
		}

		private void FillForm(){
			textImageFileName.Text=CFIcur.ImageFileName;
			textFormatString.Text=CFIcur.FormatString;
			listFieldName.Items.Clear();
			for(int i=0;i<FieldNames.Length;i++){
				listFieldName.Items.Add(FieldNames[i]);
				if(FieldNames[i]==CFIcur.FieldName){
					listFieldName.SelectedIndex=i;
				}
			}
			listFieldName.ColumnWidth=FieldNames.Max(x => TextRenderer.MeasureText(x,listFieldName.Font).Width);
		}

		private void listFieldName_DoubleClick(object sender, System.EventArgs e) {
			SaveAndClose();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			IsDeleted=true;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			SaveAndClose();
		}

		private void SaveAndClose(){
			CFIcur.ImageFileName=textImageFileName.Text;
			CFIcur.FormatString=textFormatString.Text;
			if(listFieldName.SelectedIndex==-1){
				CFIcur.FieldName="";
			}
			else{
				CFIcur.FieldName=FieldNames[listFieldName.SelectedIndex];
			}
			if(IsNew)
				ClaimFormItems.Insert(CFIcur);
			else
				ClaimFormItems.Update(CFIcur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		


	}
}





















