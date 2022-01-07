using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	
	///<summary>A provider is usually a dentist or a hygienist.  But a provider might also be a denturist, a dental student, or a dental hygiene student.  A provider might also be a 'dummy', used only for billing purposes or for notes in the Appointments module.  There is no limit to the number of providers that can be added.</summary>
	[Serializable()]
	public class Provider:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProvNum;
		///<summary>Abbreviation.  There was a limit of 5 char before version 5.4.  The new limit is 255 char.  This will allow more elegant solutions to various problems.  Providers will no longer be referred to by FName and LName.  Abbr is used as a human readable primary key.</summary>
		public string Abbr;
		///<summary>Order that provider will show in lists. 0-based.</summary>
		public int ItemOrder;
		///<summary>Last name.</summary>
		public string LName;
		///<summary>First name.</summary>
		public string FName;
		///<summary>Middle inital or name.</summary>
		public string MI;
		///<summary>eg. DMD or DDS.</summary>
		public string Suffix;
		///<summary>FK to feesched.FeeSchedNum.</summary>
		public long FeeSched;
		///<summary>FK to definition.DefNum.</summary>
		public long Specialty;
		///<summary>or TIN.  No punctuation</summary>
		public string SSN;
		///<summary>DEPRECATED. Can include punctuation</summary>
		public string StateLicense;
		///<summary>DEPRECATED.  DEANum can be found in the providerclinic table.</summary>
		public string DEANum;
		///<summary>True if hygienist.</summary>
		public bool IsSecondary;//
		///<summary>Color that shows in appointments</summary>
		[XmlIgnore]
		public Color ProvColor;
		///<summary>If true, provider will not show on any lists</summary>
		public bool IsHidden;
		///<summary>True if the SSN field is actually a Tax ID Num</summary>
		public bool UsingTIN;
		///<summary>No longer used since each state assigns a different ID.  Use the providerident instead which allows you to assign a different BCBS ID for each Payor ID.</summary>
		public string BlueCrossID;
		///<summary>Signature on file.</summary>
		public bool SigOnFile;
		///<summary>.</summary>
		public string MedicaidID;
		///<summary>Color that shows in appointments as outline when highlighted.</summary>
		[XmlIgnore]
		public Color OutlineColor;
		///<summary>FK to schoolclass.SchoolClassNum Used in dental schools.  Each student is a provider.  This keeps track of which class they are in.</summary>
		public long SchoolClassNum;
		///<summary>US NPI, and Canadian CDA provider number.</summary>
		public string NationalProvID;
		///<summary>Canadian field required for e-claims.  Assigned by CDA.  It's OK to have multiple providers with the same OfficeNum.  Max length should be 4.</summary>
		public string CanadianOfficeNum;
		/// <summary>.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		/// <summary> FK to ??. Field used to set the Anesthesia Provider type. Used to filter the provider dropdowns on FormAnestheticRecord</summary>
		public long AnesthProvType;
		///<summary>If none of the supplied taxonomies works.  This will show on claims.</summary>
		public string TaxonomyCodeOverride;
		///<summary>For Canada. Set to true if CDA Net provider.</summary>
		public bool IsCDAnet;
		///<summary>The name of this field is bad and will soon be changed to MedicalSoftID.  This allows an ID field that can be used for HL7 synch with other software.  Before this field was added, we were using prov abbreviation, which did not work well.</summary>
		public string EcwID;
		///<summary>DEPRECATED. Provider medical State ID.</summary>
		public string StateRxID;
		///<summary>Default is false because most providers are persons.  But some dummy providers used for practices or billing entities are not persons.  This is needed on 837s.</summary>
		public bool IsNotPerson;
		///<summary>DEPRECATED. The state abbreviation where the state license number in the StateLicense field is legally registered.</summary>
		public string StateWhereLicensed;
		///<summary>Not currently used.  FK to emailaddress.EmailAddressNum.  Optional, can be 0.</summary>
		public long EmailAddressNum;
		///<summary>Default is false because most providers will not be instructors.  Used in Dental Schools</summary>
		public bool IsInstructor;
		///<summary>Used to determine which stage of MU the provider is shown. 0=Global preference(Default), 1=Stage 1, 2=Stage 2, 3=Modified Stage 2.</summary>
		public int EhrMuStage;
		///<summary>FK to provider.ProvNum</summary>
		public long ProvNumBillingOverride;
		///<summary>Custom ID used for reports or bridges only.</summary>
		public string CustomID;
		///<summary>Enum:ProviderStatus </summary>
		public ProviderStatus ProvStatus;
		///<summary>Determines whether the provider will show on standard reports.</summary>
		public bool IsHiddenReport;
		///<summary>Enum:ErxEnabledStatus Indicates whether or not the provider has individually agreed to accept eRx charges.  Defaults to Disabled for new providers.</summary>
		public ErxEnabledStatus IsErxEnabled;
		///<summary>Indicates if the provider should only be scheduled in a certain way (e.g. Root canals only)</summary>
		public string SchedNote;
		/// <summary>The birthdate of the provider.</summary>
		public DateTime Birthdate;
		///<summary>The description of the provider that is displayed to patients in Web Sched.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string WebSchedDescript;
		///<summary>The image of the provider that is displayed to patients in Web Sched. File name only (path not included).
		///This should be a file name in the A to Z folder.</summary>
		public string WebSchedImageLocation;
		/// <summary>The hourly production goal amount of the provider.</summary>
		public double HourlyProdGoalAmt;
		///<summary>The date that the provider's term ends. This can be used to prevent appointments from being scheduled, appointments from being 
		///marked complete, prescriptions from being prescribed, and claims from being sent.</summary>
		public DateTime DateTerm;
		///<summary>The preferred name of the provider, shows what will be displayed to patients in eClipboard.</summary>
		public string PreferredName;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ProvColor",typeof(int))]
		public int ProvColorXml {
			get {
				return ProvColor.ToArgb();
			}
			set {
				ProvColor = Color.FromArgb(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("OutlineColor",typeof(int))]
		public int OutlineColorXml {
			get {
				return OutlineColor.ToArgb();
			}
			set {
				OutlineColor = Color.FromArgb(value);
			}
		}

		///<summary>Returns a copy of this Provider.</summary>
		public Provider Copy(){
			return (Provider)MemberwiseClone();
		}

		public Provider(){

		}

		public Provider(long provNum,string abbr,int itemOrder,string lName,string fName,string mI,string suffix,long feeSched,
			long specialty,string sSN,string stateLicense,string dEANum,bool isSecondary,Color provColor,bool isHidden,
			bool usingTIN,string blueCrossID,bool sigOnFile,string medicaidID,Color outlineColor,long schoolClassNum,
			string nationalProvID,string canadianOfficeNum,long anesthProvType)
		{
			ProvNum=provNum;
			Abbr=abbr;
			ItemOrder=itemOrder;
			LName=lName;
			FName=fName;
			MI=mI;
			Suffix=suffix;
			FeeSched=feeSched;
			Specialty=specialty;
			SSN=sSN;
			StateLicense=stateLicense;
			DEANum=dEANum;
			IsSecondary=isSecondary;
			ProvColor=provColor;
			IsHidden=isHidden;
			UsingTIN=usingTIN;
			BlueCrossID=blueCrossID;
			SigOnFile=sigOnFile;
			MedicaidID=medicaidID;
			OutlineColor=outlineColor;
			SchoolClassNum=schoolClassNum;
			NationalProvID=nationalProvID;
			CanadianOfficeNum=canadianOfficeNum;
			//DateTStamp
			AnesthProvType = anesthProvType;
		}


		///<Summary>For use in areas of the program where we have more room than just simple abbr.  Such as pick boxes in reports.  This will give Abbr - LName, FName (hidden).  If dental schools is turned on then the Abbr will be replaced with the ProvNum.</Summary>
		public string GetLongDesc(){
			if(ProvNum==0) {
				return Abbr;//this is only useful for spoofed providers in a list. I.e. "none" or "Select Provider" items.
			}
			string retval=Abbr+"- "+LName+", "+FName;
			if(!PrefC.GetBool(PrefName.EasyHideDentalSchools)) {
				retval=ProvNum+"- "+LName+", "+FName;
			}
			if(IsHidden){
				retval+=" "+Lans.g("Providers","(hidden)");
			}
			return retval;
		}


		///<Summary>For use in areas of the program where we have only have room for the simple abbr.  Such as pick boxes in the claim edit window.  This will give Abbr (hidden).</Summary>
		public string GetAbbr() {
			string retval=Abbr;
			if(IsHidden) {
				retval+=" "+Lans.g("Providers","(hidden)");
			}
			return retval;
		}

		///<summary>FName MI. LName, Suffix</summary>
		public string GetFormalName() {
			string retVal=FName+" "+MI;
			if(MI.Length==1){
				retVal+=".";
			}
			if(MI!=""){
				retVal+=" ";
			}
			retVal+=LName;
			if(Suffix!=""){
				retVal+=", "+Suffix;
			}
			return retVal;
		}

		
	}
	
	///<summary>Status of the provider.</summary>
	public enum ProviderStatus {
		///<summary>0</summary>
		Active,
		///<summary>1</summary>
		Deleted
	}

	public enum ErxEnabledStatus {
		///<summary>0.</summary>
		Disabled,
		///<summary>1.</summary>
		Enabled,
		///<summary>2.</summary>
		EnabledWithLegacy,
	}


}










