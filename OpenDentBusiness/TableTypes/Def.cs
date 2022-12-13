using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>The info in the definition table is used by other tables extensively.  Almost every table in the database links to definition. Almost all links to this table will be to a DefNum.  Using the DefNum, you can find any of the other fields of interest, usually the ItemName. Make sure to look at the Defs class to see how the definitions are used.  Loaded into memory ahead of time for speed.</summary>
	[Serializable]
	[CrudTable(TableName="definition")]
	public class Def:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DefNum;
		///<summary>Enum:DefCat</summary>
		public DefCat Category;
		///<summary>Order that each item shows on various lists. 0-indexed.</summary>
		public int ItemOrder;
		///<summary>Each category is a little different.  This field is usually the common name of the item.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.CleanText)]
		public string ItemName;
		///<summary>This field can be used to store extra info about the item. Used extensively by ImageCategories to store single letter codes.</summary>
		public string ItemValue;
		///<summary>Some categories include a color option.</summary>
		[XmlIgnore]
		public Color ItemColor;
		///<summary>If hidden, the item will not show on any list, but can still be referenced.</summary>
		public bool IsHidden;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ItemColor",typeof(int))]
		public int ItemColorXml {
			get {
				return ItemColor.ToArgb();
			}
			set {
				ItemColor = Color.FromArgb(value);
			}
		}

		///<summary>Returns a copy of the def.</summary>
		public Def Copy() {
			return (Def)MemberwiseClone();
		}

		/*
		public Def(){

		}

		public Def(long defNum,DefCat category,int itemOrder,string itemName,string itemValue,Color itemColor,bool isHidden){
			DefNum=defNum;
			Category=category;
			ItemOrder=itemOrder;
			ItemName=itemName;
			ItemValue=itemValue;
			ItemColor=itemColor;
			IsHidden=isHidden;
		}*/

	}

	///<summary>Definition Category. Go to the definition setup window (FormDefinitions) in the program to see how each of these categories is used.
	/// If you add a category, make sure to add it to the switch statement in DefL so the user can edit it.
	/// Add a "NotUsed" description attribute to defs that shouldn't show up in FormDefinitions.</summary>
	public enum DefCat {
		///<summary>0- Colors to display in Account module.</summary>
		[Description("Account Colors")]
		AccountColors,
		///<summary>1- Adjustment types.</summary>
		[Description("Adj Types")]
		AdjTypes,
		///<summary>2- Appointment confirmed types.</summary>
		[Description("Appt Confirmed")]
		ApptConfirmed,
		///<summary>3- Procedure quick add list for appointments. Example: D1023,D1024. Single tooth numbers are allowed, example D1151#8,D0220#15. This is really only useful for PAs. Tooth number is stored in user's nomenclature, not American numbering.</summary>
		[Description("Appt Procs Quick Add")]
		ApptProcsQuickAdd,
		///<summary>4- Billing types.</summary>
		[Description("Billing Types")]
		BillingTypes,
		///<summary>5- Not used.</summary>
		[Description("NotUsed")]
		ClaimFormats,
		///<summary>6- Not used.</summary>
		[Description("NotUsed")]
		DunningMessages,
		///<summary>7- Not used.</summary>
		[Description("NotUsed")]
		FeeSchedNamesOld,
		///<summary>8- Not used.</summary>
		[Description("NotUsed")]
		MedicalNotes,
		///<summary>9- Not used.</summary>
		[Description("NotUsed")]
		OperatoriesOld,
		///<summary>10- Payment types.</summary>
		[Description("Payment Types")]
		PaymentTypes,
		///<summary>11- Procedure code categories.</summary>
		[Description("Proc Code Categories")]
		ProcCodeCats,
		///<summary>12- Progress note colors.</summary>
		[Description("Prog Notes Colors")]
		ProgNoteColors,
		///<summary>13- Statuses for recall, reactivation, unscheduled, and next appointments.</summary>
		[Description("Recall/Unsched Status")]
		RecallUnschedStatus,
		///<summary>14- Not used.</summary>
		[Description("NotUsed")]
		ServiceNotes,
		///<summary>15- Not used.</summary>
		[Description("NotUsed")]
		DiscountTypes,
		///<summary>16- Diagnosis types.</summary>
		[Description("Diagnosis Types")]
		Diagnosis,
		///<summary>17- Colors to display in the Appointments module.</summary>
		[Description("Appointment Colors")]
		AppointmentColors,
		///<summary>18- Image categories. ItemValue can be one or more of the following, no delimiters. X = Show in Chart Module, M=Show Thumbnails, F = Show in Patient Forms, L = Show in Patient Portal, P = Show in Patient Pictures, S = Statements, T = Graphical Tooth Charts, R = Treatment Plans, E = Expanded, A = Payment Plans, C = Claim Attachments, B = Lab Cases, U = Autosave Forms, Y = Task Attachments.</summary>
		[Description("Image Categories")]
		ImageCats,
		///<summary>19- Not used.</summary>
		[Description("NotUsed")]
		ApptPhoneNotes,
		///<summary>20- Treatment plan priority names.</summary>
		[Description("Treat' Plan Priorities")]
		TxPriorities,
		///<summary>21- Miscellaneous color options. See enum DefCatMisColors.</summary>
		[Description("Misc Colors")]
		MiscColors,
		///<summary>22- Colors for the graphical tooth chart.</summary>
		[Description("Chart Graphic Colors")]
		ChartGraphicColors,
		///<summary>23- Categories for the Contact list.</summary>
		[Description("Contact Categories")]
		ContactCategories,
		///<summary>24- Categories for Letter Merge.</summary>
		[Description("Letter Merge Cats")]
		LetterMergeCats,
		///<summary>25- Types of Schedule Blockouts.</summary>
		[Description("Blockout Types")]
		BlockoutTypes,
		///<summary>26- Categories of procedure buttons in Chart module</summary>
		[Description("Proc Button Categories")]
		ProcButtonCats,
		///<Summary>27- Types of commlog entries.</Summary>
		[Description("Commlog Types")]
		CommLogTypes,
		///<summary>28- Categories of Supplies</summary>
		[Description("Supply Categories")]
		SupplyCats,
		///<summary>29- Types of unearned income used in accrual accounting.</summary>
		[Description("PaySplit Unearned Types")]
		PaySplitUnearnedType,
		///<summary>30- Prognosis types.</summary>
		[Description("Prognosis")]
		Prognosis,
		///<summary>31- Custom Tracking, statuses such as 'review', 'hold', 'riskmanage', etc.</summary>
		[Description("Claim Custom Tracking")]
		ClaimCustomTracking,
		///<summary>32- PayType for claims such as 'Check', 'EFT', etc.</summary>
		[Description("Insurance Payment Types")]
		InsurancePaymentType,
		///<summary>33- Categories of priorities for tasks.</summary>
		[Description("Task Priorities")]
		TaskPriorities,
		///<summary>34- Categories for fee override colors.</summary>
		[Description("Fee Colors")]
		FeeColors,
		///<summary>35- Provider specialties.  General, Hygienist, Pediatric, Primary Care Physician, etc.</summary>
		[Description("Provider Specialties")]
		ProviderSpecialties,
		///<summary>36- Reason why a claim proc was rejected. This must be set on each individual claim proc.</summary>
		[Description("Claim Payment Tracking")]
		ClaimPaymentTracking,
		///<summary>37- Procedure quick charge list for patient accounts.</summary>
		[Description("Account Procs Quick Add")]
		AccountQuickCharge,
		///<summary>38- Insurance verification status such as 'Verified', 'Unverified', 'Pending Verification'.</summary>
		[Description("Insurance Verification Status")]
		InsuranceVerificationStatus,
		///<summary>39- Regions that clinics can be assigned to.</summary>
		[Description("Regions")]
		Regions,
		///<summary>40- ClaimPayment Payment Groups.</summary>
		[Description("Claim Payment Groups")]
		ClaimPaymentGroups,
		///<summary>41 - Auto Note Categories.  Used to categorize autonotes into custom categories.</summary>
		[Description("Auto Note Categories")]
		AutoNoteCats,
		///<summary>42 - Web Sched New Patient Appointment Types. Displays in Web Sched. Each appointment can be assigned one appointment.AppointmentTypeNum. Multiple AppointmentTypes are linked to this definition through the DefLink table, where deflink.DefNum=definition.DefNum,  deflink.LinkType=2, and deflink.FKey=appointmenttype.AppointmentTypeNum.</summary>
		[Description("Web Sched New Pat Appt Types")]
		WebSchedNewPatApptTypes,
		///<summary>43 - Custom Claim Status Error Code.</summary>
		[Description("Claim Error Code")]
		ClaimErrorCode,
		///<summary>44 - Specialties that clinics perform.  Useful for separating patient clones across clinics.</summary>
		[Description("Clinic Specialties")]
		ClinicSpecialty,
		///<summary>45 - HQ Only job priorities.</summary>
		[Description("Job Priorities HqOnly")]
		JobPriorities,
		///<summary>46 - Carrier Group Name.</summary>
		[Description("Carrier Group Names")]
		CarrierGroupNames,
		///<summary>47 - PayPlanCategory</summary>
		[Description("Payment Plan Categories")]
		PayPlanCategories,
		///<summary>48 - Associates an insurance payment to an account number.  Currently only used with "Auto Deposits".</summary>
		[Description("Auto Deposit Account")]
		AutoDeposit,
		///<summary>49 - Code Group used for insurance filing.</summary>
		[Description("Insurance Filing Code Group")]
		InsuranceFilingCodeGroup,
		///<summary>50 - Time card adjustment types.
		///Currently for PTO, but in future could be used for other types as well if we implement the Usage def field.</summary>
		[Description("Time Card Adj Types")]
		TimeCardAdjTypes,
		///<summary>51 - Web Sched Existing Appt Types. Each appointment can be assigned one appointment.AppointmentTypeNum. Multiple AppointmentTypes are linked to this definition through the DefLink table, where deflink.DefNum=definition.DefNum,  deflink.LinkType=2, and deflink.FKey=appointmenttype.AppointmentTypeNum.</summary>
		[Description("Web Sched Existing Appt Types")]
		WebSchedExistingApptTypes,
		///<summary>52 - Categories for the Certifications feature.</summary>
		[Description("Certification Categories")]
		CertificationCategories,
		///<summary>53 - Images the office prompts the patient to submit when checking in via eClipboard</summary>
		[Description("eClipboard Images")]
		EClipboardImageCapture,
		///<summary>54 - HQ Only task categories.</summary>
		[Description("Task Categories HqOnly")]
		TaskCategories,
		///<summary>55 - Operatory Types. This field is only informational. The value isn't used for functionality.</summary>
		[Description("Operatory Types")]
		OperatoryTypes,
	}

	///<summary>For DefCat MiscColors(Category 21).</summary>
	public enum DefCatMiscColors {
		///<summary></summary>
		FamilyModuleCoverage=0,
		///<summary></summary>
		PerioBleeding=1,
		///<summary></summary>
		PerioSuppuration=2,
		///<summary></summary>
		ChartModuleMedical=3,
		///<summary></summary>
		PerioPlaque=4,
		///<summary></summary>
		PerioCalculus=5,
		///<summary></summary>
		ChartTodaysProcs=6,
		///<summary></summary>
		CommlogApptRelated=7,
		///<summary></summary>
		FamilyModuleReferral=8,
		///<summary>9 - This color is used for the fields in the family module for the in case of emergency contacts.</summary>
		FamilyModuleICE=9,
		///<summary></summary>
		FamilyModPatRestrict=10,
		///<summary></summary>
		MainBorder=11,
		///<summary></summary>
		MainBorderOutline=12,
		///<summary></summary>
		MainBorderText=13
	}

	public class DefCatOptions {
		public DefCat DefCat;
		public bool CanEditName;
		public bool EnableValue;
		public bool EnableColor;
		///<summary>This is the text that will show up in the Guidelines section of the Definitions window.</summary>
		public string HelpText;
		public bool CanDelete;
		public bool CanHide;
		///<summary>This is the text that will show up in the second column of the Definitions window and above the second text box in the edit window.
		///It is typically left blank unless each definition item has something special that it can do (image categories, etc).</summary>
		public string ValueText;
		///<summary>Only used for AutoNotes to select parent def.</summary>
		public bool IsValueDefNum;
		public bool DoShowItemOrderInValue;
		///<summary>Shows a checkbox for No Color.  This is a 0 in the database.</summary>
		public bool DoShowNoColor;

		public DefCatOptions(DefCat defCat,bool canDelete=false,bool canEditName=true,bool canHide=true,bool enableColor=false,bool enableValue=false,
			bool isValidDefNum=false,bool showNoColor=false) 
		{
			DefCat=defCat;
			CanDelete=canDelete;
			CanEditName=canEditName;
			CanHide=canHide;
			EnableColor=enableColor;
			EnableValue=enableValue;
			IsValueDefNum=isValidDefNum;
			DoShowNoColor=showNoColor;
		}
	}





}
