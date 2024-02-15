using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public class PatFieldL {
		///<summary>Adds the passed-in pat fields to the grid. Adds any fields that have been renamed at the end of the grid if the preference is enabled. The tag on the row will be the PatFieldDef or the PatField if the PatFieldDef has been renamed.</summary>
		public static void AddPatFieldsToGrid(GridOD grid,List<PatField> listPatFields,FieldLocations fieldLocation) {
			List<PatFieldDef> listPatFieldDefs=PatFieldDefs.GetDeepCopy(true);
			//Add a row for each existing PatFieldDef 
			for(int i=0;i<listPatFieldDefs.Count;i++) {
				if(FieldDefLinks.GetExists(x => x.FieldDefNum==listPatFieldDefs[i].PatFieldDefNum && x.FieldDefType==FieldDefTypes.Patient && x.FieldLocation==fieldLocation)) {
					continue;
				}
				GridRow row=new GridRow();
				PatField patField=listPatFields.FirstOrDefault(x => x.FieldName==listPatFieldDefs[i].FieldName);
				if(listPatFieldDefs[i].FieldType.ToString()=="InCaseOfEmergency") {
					//Deprecated. Should never happen.
					continue;
				}
				row.Cells.Add(listPatFieldDefs[i].FieldName);
				if(patField==null) {
					row.Cells.Add("");
				}
				else {
					if(listPatFieldDefs[i].FieldType==PatFieldType.Checkbox) {
						row.Cells.Add("X");
					}
					else if(listPatFieldDefs[i].FieldType==PatFieldType.Currency) {
						row.Cells.Add(PIn.Double(patField.FieldValue).ToString("c"));
					}
					else {
						row.Cells.Add(patField.FieldValue);
					}
				}
				row.Tag=listPatFieldDefs[i];
				grid.ListGridRows.Add(row);
			}
			if(!PrefC.GetBool(PrefName.DisplayRenamedPatFields)) {
				return;
			}
			//Now loop through the PatFields that do not have a matching PatFieldDef.
			List<PatField> listPatFieldsFiltered=listPatFields.Where(x => !listPatFieldDefs.Exists(y => y.FieldName==x.FieldName)).ToList();
			for(int i=0;i<listPatFieldsFiltered.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listPatFieldsFiltered[i].FieldName);
				row.Cells.Add(listPatFieldsFiltered[i].FieldValue);
				row.Tag=listPatFieldsFiltered[i];
				row.ColorText=Color.DarkSlateGray;
				grid.ListGridRows.Add(row);
			}
		}

		///<summary>Opens the appropriate form to edit the patient field. The patField argument can be null or the patFieldDef argument can be null,
		///but they cannot both be null.</summary>
		public static void OpenPatField(PatField patField,PatFieldDef patFieldDef,long patNum,bool isForOrtho=false) {
			if(patFieldDef==null) { 
				if(patField!=null) {//PatField for a PatFieldDef that no longer exists
					using FormPatFieldEdit formPatFieldEdit=new FormPatFieldEdit(patField);
					formPatFieldEdit.IsLaunchedFromOrtho=isForOrtho;
					formPatFieldEdit.ShowDialog();
				}
				return;
			}
			if(patField==null) {
				patField=new PatField();
				patField.PatNum=patNum;
				patField.FieldName=patFieldDef.FieldName;
				patField.FieldValue=string.Empty;
				if(patFieldDef.FieldType==PatFieldType.Text) {
					using FormPatFieldEdit formPatFieldEdit=new FormPatFieldEdit(patField);
					formPatFieldEdit.IsLaunchedFromOrtho=isForOrtho;
					formPatFieldEdit.IsNew=true;
					formPatFieldEdit.ShowDialog();
				}
				if(patFieldDef.FieldType==PatFieldType.PickList) {
					patField.IsNew=true;
					using FormPatFieldPickEdit formPatFieldPickEdit=new FormPatFieldPickEdit(patField);
					formPatFieldPickEdit.ShowDialog();
				}
				if(patFieldDef.FieldType==PatFieldType.Date) {
					using FormPatFieldDateEdit formPatFieldDateEdit=new FormPatFieldDateEdit(patField);
					formPatFieldDateEdit.IsNew=true;
					formPatFieldDateEdit.ShowDialog();
				}
				if(patFieldDef.FieldType==PatFieldType.Checkbox) {
					using FormPatFieldCheckEdit formPatFieldCheckEdit=new FormPatFieldCheckEdit(patField);
					formPatFieldCheckEdit.IsNew=true;
					formPatFieldCheckEdit.ShowDialog();
				}
				if(patFieldDef.FieldType==PatFieldType.Currency) {
					using FormPatFieldCurrencyEdit formPatFieldCurrencyEdit=new FormPatFieldCurrencyEdit(patField);
					formPatFieldCurrencyEdit.IsNew=true;
					formPatFieldCurrencyEdit.ShowDialog();
				}
				if(patFieldDef.FieldType==PatFieldType.InCaseOfEmergency) {
					//Deprecated
				}
				return;
			}
			if(patFieldDef.FieldType==PatFieldType.Text) {
				using FormPatFieldEdit formPatFieldEdit=new FormPatFieldEdit(patField);
				formPatFieldEdit.IsLaunchedFromOrtho=isForOrtho;
				formPatFieldEdit.ShowDialog();
			}
			if(patFieldDef.FieldType==PatFieldType.PickList) {
				using FormPatFieldPickEdit formPatFieldPickEdit=new FormPatFieldPickEdit(patField);
				formPatFieldPickEdit.ShowDialog();
			}
			if(patFieldDef.FieldType==PatFieldType.Date) {
				using FormPatFieldDateEdit formPatFieldDateEdit=new FormPatFieldDateEdit(patField);
				formPatFieldDateEdit.ShowDialog();
			}
			if(patFieldDef.FieldType==PatFieldType.Checkbox) {
				using FormPatFieldCheckEdit formPatFieldCheckEdit=new FormPatFieldCheckEdit(patField);
				formPatFieldCheckEdit.ShowDialog();
			}
			if(patFieldDef.FieldType==PatFieldType.Currency) {
				using FormPatFieldCurrencyEdit formPatFieldCurrencyEdit=new FormPatFieldCurrencyEdit(patField);
				formPatFieldCurrencyEdit.ShowDialog();
			}
			if(patFieldDef.FieldType==PatFieldType.InCaseOfEmergency) {
				//Deprecated
			}
		}
	}
}
