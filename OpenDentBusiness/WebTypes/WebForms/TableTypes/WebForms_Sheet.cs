using System;
using OpenDentBusiness;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace OpenDentBusiness.WebTypes.WebForms {
	[Serializable]
	[CrudTable(IsMissingInGeneral=true,CrudLocationOverride=@"..\..\..\OpenDentBusiness\WebTypes\WebForms\Crud",NamespaceOverride="OpenDentBusiness.WebTypes.WebForms.Crud",CrudExcludePrefC=true)]
	public class WebForms_Sheet:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SheetID;
		///<summary>FK to customers.patient.PatNum.</summary>
		public long DentalOfficeID;
		///<summary>Copied from the SheetDef description.</summary>
		public string Description;
		///<summary>Enum:SheetTypeEnum  The type of sheet.  Only PatientForm, MedicalHistory, and Consent sheet types are supported in Web Forms. !!!NOTE!!! The SheetType field was originally created with the wrong enumeration type (SheetFieldType). The enumeration was corrected but this class is used by older versions of Open Dental which will continue to use SheetFieldType values. Therefore, always treat the value passed in as if it were a SheetFieldType first. If the value is not a SheetFieldType, then it will be the correct SheetTypeEnum type. E.g. Older versions are expecting the value in the XML payload to be a string representation of SheetFieldType values (SigBox instead of PatientForm).</summary>
		[XmlIgnore]
		public SheetTypeEnum SheetType;
		///<summary>The date and time of the sheet as it will be displayed in the commlog.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSheet;
		///<summary>The default fontSize for the sheet.  The actual font must still be saved with each sheetField.</summary>
		public float FontSize;
		///<summary>The default fontName for the sheet.  The actual font must still be saved with each sheetField.</summary>
		public string FontName;
		///<summary>Width of each page in the sheet in pixels, 100 pixels per inch.</summary>
		public int Width;
		///<summary>Height of each page in the sheet in pixels, 100 pixels per inch.</summary>
		public int Height;
		///<summary>.</summary>
		public bool IsLandscape;
		///<summary>FK to clinic.ClinicNum. Used by webforms to limit the sheets displayed based on the currently selected clinic.</summary>
		public long ClinicNum;
		///<summary>If true then this Sheet has been designed for mobile and will be displayed as a mobile-friendly WebForm.</summary>
		public bool HasMobileLayout;
		///<summary>FK to sheetdef.SheetDefNum.  The SheetDef that was used to create this sheet.</summary>
		public long SheetDefNum;

		///<Summary></Summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<WebForms_SheetField> SheetFields;

		public WebForms_Sheet(){

		}
		
    public WebForms_Sheet Copy(){
			return (WebForms_Sheet)this.MemberwiseClone();
		}
		
		///<summary>Used only for serialization purposes. The SheetType field was originally created with the wrong enumeration type (SheetFieldType). The enumeration was corrected but this class is used by older versions of Open Dental which will continue to use SheetFieldType values. Therefore, always treat the value passed in as if it were a SheetFieldType first. If the value is not a SheetFieldType, then it will be the correct SheetTypeEnum type.</summary>
		[XmlElement("SheetType",typeof(string))]
		public string SheetFieldTypeXml {
			get {
				//Always treat the value as a SheetFieldType first because there was a bug in older versions where SheetType was the wrong enum.
				//Older versions of Open Dental are expecting the string representation of the SheetFieldType enum value.
				string retVal=Enum.GetName(typeof(SheetFieldType),SheetType);
				//When SheetType value is out of range of the SheetFieldType enum then we need to use SheetTypeEnum instead.
				if(retVal==null) {
					retVal=Enum.GetName(typeof(SheetTypeEnum),SheetType);
				}
				//If the value of SheetType could is not a valid SheetFieldType or SheetTypeEnum then return null which will exclude the XML node.
				//This should cause consuming entities to treat the SheetType field as 'OutputText' or 'LabelPatient'. This should never happen.
				return retVal;
			}
			set {
				//Always try to parse the value as a SheetFieldType first because there was a bug in older versions where SheetType was the wrong enum.
				if(Enum.TryParse(value,out SheetFieldType sheetType)) {
					//Cast the parsed SheetFieldType to SheetTypeEnum because more SheetTypeEnum values were present when this was written.
					//This means that there will never be a backwards compatible enum value that is an invalid value for the SheetTypeEnum.
					//If this ever causes a UE, we want to know about it so let the error bubble up.
					SheetType=(SheetTypeEnum)sheetType;
				}
				else {
					SheetType=(SheetTypeEnum)Enum.Parse(typeof(SheetTypeEnum),value);
				}
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("SheetFields",typeof(WebForms_SheetField[]))]
		public WebForms_SheetField[] SheetFieldsXml {
			get {
				if(SheetFields==null) {
					return new WebForms_SheetField[0];
				}
				return SheetFields.ToArray();
			}
			set {
				SheetFields=new List<WebForms_SheetField>();
				for(int i=0;i<value.Length;i++) {
					SheetFields.Add(value[i]);
				}
			}
		}

	}
}