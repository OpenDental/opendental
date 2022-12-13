using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormSheetTools:FormODBase {
		///<summary>Whether or not sheets have been edited/added/deleted from the DB from within this form.</summary>
		public bool HasSheetsChanged=false;
		///<summary>The primary key of the last sheet that was imported.</summary>
		public long SheetDefNumImported=0;
		///<summary>Form was opened via Dashboard Setup.</summary>
		private bool _isOpenedFromDashboardSetup;

		public FormSheetTools(bool isOpenedFromDashboardSetup=false) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isOpenedFromDashboardSetup=isOpenedFromDashboardSetup;
		}

		private void butImport_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			string initDir=PrefC.GetString(PrefName.ExportPath);
			if(Directory.Exists(initDir)) {
				openFileDialog.InitialDirectory=initDir;
			}
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				Cursor=Cursors.Default;
				return;
			}
			SheetDef sheetDef=new SheetDef();
			XmlSerializer serializer=new XmlSerializer(typeof(SheetDef));
			if(openFileDialog.FileName=="") {
				Cursor=Cursors.Default;
				return;
			}
			if(!File.Exists(openFileDialog.FileName)){
				Cursor=Cursors.Default;
				MsgBox.Show(this,"File not found");
				return;
			}
			try {
				using TextReader reader=new StreamReader(openFileDialog.FileName);
				sheetDef=(SheetDef)serializer.Deserialize(reader);
			}
			catch {
				Cursor=Cursors.Default;
				throw new ApplicationException(Lan.g("FormSheetDefs","Invalid file format"));
			}
			sheetDef.IsNew=true;
			if(ValidateSheetDef(sheetDef)){
				MsgBox.Show(this,"Imported.");
			}
			//Cursor.Default handled in above method
		}

		/// <summary>Validates the SheetDef being imported.</summary>
		private bool ValidateSheetDef(SheetDef sheetDef) {
			//the Form that called this is not the PatientDashboard but the sheetDefs are of type PatientDashboard
			if(!_isOpenedFromDashboardSetup && SheetDefs.IsDashboardType(sheetDef)) {
				Cursor=Cursors.Default;
				MsgBox.Show(Lan.g("FormSheetDefs","Sheets of type '"+sheetDef.SheetType.GetDescription()+"' cannot be imported from Sheets. Use Dashboard Setup instead."));
				return false;
			}
			//the Form that called this is the PatientDashboard but the sheetDefs are not of type PatientDashboard
			if(_isOpenedFromDashboardSetup && !SheetDefs.IsDashboardType(sheetDef)) {
				Cursor=Cursors.Default;
				MsgBox.Show(Lan.g("FormSheetDefs","Sheets of type '"+sheetDef.SheetType.GetDescription()+"' cannot be imported from Dashboard Setup. Use Sheets instead."));
				return false;
			}
			List<SheetFieldDef> listSheetFieldDefsUnsupported=new List<SheetFieldDef>();
			for(int i=sheetDef.SheetFieldDefs.Count-1;i>=0;i--) {
				SheetFieldDef sheetFieldDef=sheetDef.SheetFieldDefs[i];
				//If user is importing a Dashboard SheetDef with SheetDefFields that are not supported by Dashboards, remove them from the field list
				if(SheetDefs.IsDashboardType(sheetDef) && !UserControlDashboardWidget.IsSheetFieldDefSupported(sheetFieldDef)) {
					listSheetFieldDefsUnsupported.Add(sheetFieldDef);
					sheetDef.SheetFieldDefs.Remove(sheetFieldDef);
				}
				//Users might be importing a sheet that was developed in an older version that does not support ItemColor.  Default them to black if necessary.
				//Static text, lines, and rectangles are the only field types that support ItemColor.
				if(sheetFieldDef.FieldType!=SheetFieldType.StaticText 
					&& sheetFieldDef.FieldType!=SheetFieldType.Line 
					&& sheetFieldDef.FieldType!=SheetFieldType.Rectangle)  
				{
					continue;
				}
				//ItemColor will be set to "Empty" if this is a sheet that was exported from a previous version that didn't support ItemColor.
				//Color.Empty will actually draw but will be 'invisible' to the user.  For this reason, we considered this a bug and defaulted the color to black.
				if(sheetFieldDef.ItemColor==Color.Empty) {
					sheetFieldDef.ItemColor=Color.Black;//Old sheet behavior was to always draw these field types in black.
				}
			}
			if(!listSheetFieldDefsUnsupported.IsNullOrEmpty()) {
				Cursor=Cursors.Default;
				//If the user was importing from the Dashboard with SheetDef fields that were unsupported, use the previously saved list to
				//present them a message about the fields that will be lost and see if they wish to proceed.
				if(!MsgBox.Show("FormSheetDefs",MsgBoxButtons.YesNo,"Field(s): "
					+string.Join(",",listSheetFieldDefsUnsupported.Select(x => x.FieldName+"("+x.FieldType+")"))//Displays as FieldName(FeildType)
					+" are not supported by Patient Dashboards and will not be imported.\r\n"+"Continue?")) 
				{
					return false;
				}
			}
			if(SheetDefs.IsDashboardType(sheetDef)) {
				//PatImage fields use FieldName as a FK to the Patient Image definition.  Since this FK may not be accurate across databases, determine 
				//which Image Category Definition is set as PatImage
				SheetDefs.SetPatImageFieldNames(sheetDef);
			}
			SheetDefs.InsertOrUpdate(sheetDef,isOldSheetDuplicate:sheetDef.DateTCreated.Year < 1880);
			HasSheetsChanged=true;//Flag as true so we know to refresh the grid in FormSheetDefs.cs
			SheetDefNumImported=sheetDef.SheetDefNum;//Set this so when we return to FormSheetDefs.cs we can select that row.
			Cursor=Cursors.Default;
			return true;
		}

		private void butExport_Click(object sender,EventArgs e) {
			using FormSheetExport formSheetExport=new FormSheetExport(_isOpenedFromDashboardSetup);
			formSheetExport.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}