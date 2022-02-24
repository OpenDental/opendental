using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;
using CodeBase;
using Newtonsoft.Json;

namespace OpenDentBusiness{
/*A better name for this object would be a Form, but that name is obviously too ambiguous and has been overused.  There are two different aspects of the database tables:
	1. Customization of sheets.
	2. Saving data filled in on sheets.
	Sheets do not include reports, which are better handled by the RDL framework or something even simpler.  Examples of what sheets might be used for include statements, tx plans, rx, lab slips, postcards, referral slips, patient registration forms, medical histories, consent forms, and labels.
	The interesting thing about this framework is that it is able to support incoming data as well as outgoing data using the following elements:
	-background image
	-static text
	-text generated from database
	-user input
	Class names:
	Data: Sheet, SheetField. (Parameters are saved as part of fields, except PatNum is part of Sheet)
	Defs:	SheetDef, SheetFieldDef (SheetParameters are hardcoded based on type)
  SheetImage (handling this with files for now)
	
	Note that we have tried to do similar things before, but not with as much clarity and organization.  See the ReportingOld2 folder for an example of a similar framework that never took off because:
	a) It was overwhelming because it was trying to handle 'reporting' functions as its main purpose.
	b) It did not start with a simpler framework and build iteratively.
	c) It was modeled after Crystal Reports, which was only designed for reports, not forms.
	d) We did not have generics.
	*/
	///<summary>One sheet for one patient.</summary>
	[Serializable()]
	public class Sheet:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SheetNum;
		///<summary>Enum:SheetTypeEnum</summary>
		public SheetTypeEnum SheetType;
		///<summary>FK to patient.PatNum.  A saved sheet is always attached to a patient (except deposit slip).  There are a few sheets that are so minor that they don't get saved, such as a Carrier label.</summary>
		public long PatNum;
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
		///<summary>An internal note for the use of the office staff regarding the sheet.  Not to be printed on the sheet in any way.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string InternalNote;
		///<summary>Copied from the SheetDef description.</summary>
		public string Description;
		///<summary>The order that this sheet will show in the patient terminal for the patient to fill out.  Or zero if not set.</summary>
		public byte ShowInTerminal;
		///<summary>True if this sheet was downloaded from the webforms service.</summary>
		public bool IsWebForm;
		///<summary>Forces old single page behavior, ignoring page breaks.</summary>
		public bool IsMultiPage;
		///<summary>Indicates whether or not this sheet has been marked deleted.</summary>
		public bool IsDeleted;
		///<summary>FK to sheetdef.SheetDefNum.  The SheetDef that was used to create this sheet.  Will be 0 if this sheet was created from a Web Form or 
		///an internal sheet or if the sheet was created before 17.2.  Currently only used to determine the sheet can bypass the global lock date.
		///</summary>
		public long SheetDefNum;
		/// <summary>FK to document.DocNum.  Referrel letters are stored as PDF in the A to Z folder.</summary>
		public long DocNum;
		/// <summary>FK to clinic.ClinicNum. Used by webforms to limit the sheets displayed based on the currently selected clinic.</summary>
		public long ClinicNum;
		///<summary>The date and time the sheet was inserted or last time someone opened the sheet and clicked OK on FormSheetFillEdit.
		///Gets updated even if no changes were made to the sheet or sheetfields, because we don't want to do the lengthy work of comparing all fields.
		///Used when editing a sheet to warn user if the sheet has been edited by someone else.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntryEditable)]
		public DateTime DateTSheetEdited;
		///<summary>If true then this Sheet has been designed for mobile and will be displayed as a mobile-friendly WebForm.</summary>
		public bool HasMobileLayout;
		///<summary>Revision ID. Used to determine in conjustion with PrefillMode for eClipboard to determine whether to show a patient a new form 
		///or have them update their last filled out form. Must match up with SheetDef RevID to show a previously filled out form.</summary>
		public int RevID;

		public Sheet Copy(){
			Sheet retVal=(Sheet)this.MemberwiseClone();
			retVal.Parameters=Parameters.Select(x => x.Copy()).ToList();
			retVal.SheetFields=SheetFields.Select(x => x.Copy()).ToList();
			return retVal;
		}	

		///<Summary>A collection of all parameters for this sheetdef.  There's usually only one parameter.  The first parameter will be a List long if it's a batch.  If a sheet has already been filled, saved to the database, and printed, then there is no longer any need for the parameters in order to fill the data.  So a retrieved sheet will have no parameters, signalling a skip in the fill phase.  There will still be parameters tucked away in the Field data in the database, but they won't become part of the sheet.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<SheetParameter> Parameters;
		///<Summary></Summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<SheetField> SheetFields;

		///<summary>Vertical height per page taking into account the IsLandscape flag.</summary>
		public int HeightPage {
			get {
				if(IsLandscape) {
					return Width;
				}
				else {
					return Height;
				}
			}
		}

		///<summary>Horizontal width per page taking into account the IsLandscape flag.</summary>
		public int WidthPage {
			get {
				if(IsLandscape) {
					return Height;
				}
				else {
					return Width;
				}
			}
		}

		///<summary>Will return null if there are no SheetFields for the sheet or if there isn't one with the fieldName requested.</summary>
		public SheetField GetSheetFieldByName(string fieldName) {
			return SheetFields?.FirstOrDefault(x => x.FieldName==fieldName);
		}

		///<summary>Finds the lowest lower bound of all of the SheeFields. Recalculated every time this property is called.</summary>
		[XmlIgnore,JsonIgnore]
		public int HeightLastField {
			get {
				int retVal=0;
				foreach(SheetField f in SheetFields) {
					if(f.Bounds.Bottom>retVal)
						retVal=f.Bounds.Bottom;
				}
				return retVal;
			}
		}

		///<summary>Returns true if the sheet is a CEMT Transfer.</summary>
		public bool IsCemtTransfer {
			get {
				//The rules applied here should coincide with Sheets.GetTransferSheets()
				return (SheetType==SheetTypeEnum.PatientForm 
					&& SheetFields!=null 
					&& SheetFields.Any(x => x.FieldName.ToLower()=="istransfer"));
			}
		}

		/*Parameters are not serialized as part of a sheet because it causes serialization to fail.
		///<summary>Used only for serialization purposes</summary>
		[XmlElement("Parameters",typeof(SheetParameter[]))]
		public SheetParameter[] ParametersXml {
			get {
				if(Parameters==null || Parameters.Count==0) {
					return new SheetParameter[0];
				}
				return Parameters.ToArray();
			}
			set {
				Parameters=new List<SheetParameter>();
				for(int i=0;i<value.Length;i++) {
					Parameters.Add(value[i]);
				}
			}
		}*/

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("SheetFields",typeof(SheetField[]))]
		public SheetField[] SheetFieldsXml {
			get {
				if(SheetFields==null) {
					return new SheetField[0];
				}
				return SheetFields.ToArray();
			}
			set {
				SheetFields=new List<SheetField>();
				for(int i=0;i<value.Length;i++) {
					SheetFields.Add(value[i]);
				}
			}
		}
		






	}
}


