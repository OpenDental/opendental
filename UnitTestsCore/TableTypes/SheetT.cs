using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	///<summary>Has methods for both Sheet and SheetField. Use SheetDefT for defs.</summary>
	public class SheetT {

		///<summary>Allows for the creation of a new Sheet with control over key fields, instead of them being set per 
		///ExamSheet explicitly. It ensures the SheetNum is set and this does insert to the DB.</summary>
		public static OpenDentBusiness.Sheet CreateSheet(long sheetDefNum,List<SheetField> listSheetFields, long patNum,long docNum=0,long clinicNum=0, SheetTypeEnum sheetType=SheetTypeEnum.ExamSheet,string internalNote="",string description="") {
			OpenDentBusiness.Sheet sheet=new OpenDentBusiness.Sheet {
				SheetDefNum=sheetDefNum,
				SheetFields=listSheetFields,
				DateTimeSheet=DateTime.Today.AddDays(-1),
				DateTSheetEdited=DateTime.Today,
				DocNum=docNum,
				ClinicNum=clinicNum,
				SheetType=sheetType,
				PatNum=patNum,
				InternalNote=internalNote,
				Description=description
			};
			sheet.SheetNum=OpenDentBusiness.Sheets.Insert(sheet);
			OpenDentBusiness.Sheets.Update(sheet);
			return sheet;
		}

		///<summary>Creates a list of SheetFields for a given SheetFieldDef List and sets their SheetNum to match the 
		///one passed in.</summary>
		public static List<OpenDentBusiness.SheetField> CreateFieldListForSheet(List<SheetFieldDef> sheetFieldDefList,long sheetNum) {
				List<SheetField> retVal=new List<SheetField>();
				SheetField field;
				foreach(SheetFieldDef sheetFieldDef in sheetFieldDefList) {
					field=new SheetField {
						IsNew=true,
						FieldName=sheetFieldDef.FieldName,
						FieldType=sheetFieldDef.FieldType,
						FieldValue=sheetFieldDef.FieldValue,
						FontIsBold=sheetFieldDef.FontIsBold,
						FontName=sheetFieldDef.FontName,
						FontSize=sheetFieldDef.FontSize,
						GrowthBehavior=sheetFieldDef.GrowthBehavior,
						Height=sheetFieldDef.Height,
						RadioButtonValue=sheetFieldDef.RadioButtonValue,
						SheetNum=sheetNum,
						Width=sheetFieldDef.Width,
						XPos=sheetFieldDef.XPos,
						YPos=sheetFieldDef.YPos,
						RadioButtonGroup=sheetFieldDef.RadioButtonGroup,
						IsRequired=sheetFieldDef.IsRequired,
						TabOrder=sheetFieldDef.TabOrder,
						ReportableName=sheetFieldDef.ReportableName,
						SheetFieldDefNum=sheetFieldDef.SheetFieldDefNum,
						TextAlign=sheetFieldDef.TextAlign,
						ItemColor=sheetFieldDef.ItemColor,
						IsLocked=sheetFieldDef.IsLocked,
						TabOrderMobile=sheetFieldDef.TabOrderMobile,
						UiLabelMobile=sheetFieldDef.UiLabelMobile,
						UiLabelMobileRadioButton=sheetFieldDef.UiLabelMobileRadioButton,
					};
					SheetFields.Insert(field);
					retVal.Add(field);
				}
				return retVal;
			}

		///<summary>Deletes everything from the sheet and sheetfield table.  Does not truncate the table so that PKs 
		///are not reused on accident.</summary>
		public static void ClearSheetAndSheetFieldTable() {
			string command="DELETE FROM sheet WHERE sheetnum > 0";
			DataCore.NonQ(command);
			command="DELETE FROM sheetfield WHERE sheetfieldnum > 0";
			DataCore.NonQ(command);
		}
	}
}
