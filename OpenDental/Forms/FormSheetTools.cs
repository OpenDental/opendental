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
		public long ImportedSheetDefNum=0;
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
			using OpenFileDialog openDlg=new OpenFileDialog();
			string initDir=PrefC.GetString(PrefName.ExportPath);
			if(Directory.Exists(initDir)) {
				openDlg.InitialDirectory=initDir;
			}
			if(openDlg.ShowDialog()!=DialogResult.OK) {
				Cursor=Cursors.Default;
				return;
			}
			try {
				//ImportCustomSheetDef(openDlg.FileName);
				SheetDef sheetdef=new SheetDef();
				XmlSerializer serializer=new XmlSerializer(typeof(SheetDef));
				if(openDlg.FileName!="") {
					if(!File.Exists(openDlg.FileName)) {
						throw new ApplicationException(Lan.g("FormSheetDefs","File does not exist."));
					}
					try {
						using(TextReader reader=new StreamReader(openDlg.FileName)) {
							sheetdef=(SheetDef)serializer.Deserialize(reader);
						}
					}
					catch {
						throw new ApplicationException(Lan.g("FormSheetDefs","Invalid file format"));
					}
				}
				sheetdef.IsNew=true;
				//the Form that called this is not the PatientDashboard but the sheetdefs are of type PatientDashboard
				if(!_isOpenedFromDashboardSetup && SheetDefs.IsDashboardType(sheetdef)) {
					throw new ApplicationException(Lan.g("FormSheetDefs",
						"Sheets of type '"+sheetdef.SheetType.GetDescription()+"' cannot be imported from Sheets. Use Dashboard Setup instead."));
				}
				//the Form that called this is the PatientDashboard but the sheetdefs are not of type PatientDashboard
				if(_isOpenedFromDashboardSetup && !SheetDefs.IsDashboardType(sheetdef)) {
					throw new ApplicationException(Lan.g("FormSheetDefs",
						"Sheets of type '"+sheetdef.SheetType.GetDescription()+"' cannot be imported from Dashboard Setup. Use Sheets instead."));
				}
				List<SheetFieldDef> listUnsupportedSheetDefs=new List<SheetFieldDef>();
				for(int i=sheetdef.SheetFieldDefs.Count-1;i>=0;i--) {
					SheetFieldDef fieldDef=sheetdef.SheetFieldDefs[i];
					//If user is importing a Dashboard SheetDef with SheetDefFields that are not supported by Dashboards, remove them from the field list
					if(SheetDefs.IsDashboardType(sheetdef) && !UserControlDashboardWidget.IsSheetFieldDefSupported(fieldDef)) {
						listUnsupportedSheetDefs.Add(fieldDef);
						sheetdef.SheetFieldDefs.Remove(fieldDef);
					}
					//Users might be importing a sheet that was developed in an older version that does not support ItemColor.  Default them to black if necessary.
					//Static text, lines, and rectangles are the only field types that support ItemColor.
					if(fieldDef.FieldType!=SheetFieldType.StaticText 
						&& fieldDef.FieldType!=SheetFieldType.Line 
						&& fieldDef.FieldType!=SheetFieldType.Rectangle)  
					{
						continue;
					}
					//ItemColor will be set to "Empty" if this is a sheet that was exported from a previous version that didn't support ItemColor.
					//Color.Empty will actually draw but will be 'invisible' to the user.  For this reason, we considered this a bug and defaulted the color to black.
					if(fieldDef.ItemColor==Color.Empty) {
						fieldDef.ItemColor=Color.Black;//Old sheet behavior was to always draw these field types in black.
					}
				}
				if(!listUnsupportedSheetDefs.IsNullOrEmpty()) {
					Cursor=Cursors.Default;
					//If the user was importing from the Dashboard with SheetDef fields that were unsupported, use the previously saved list to
					//present them a message about the fields that will be lost and see if they wish to proceed.
					if(!MsgBox.Show("FormSheetDefs",MsgBoxButtons.YesNo,"Field(s): "
						+string.Join(",",listUnsupportedSheetDefs.Select(x => x.FieldName+"("+x.FieldType+")"))//Displays as FieldName(FeildType)
						+" are not supported by Patient Dashboards and will not be imported.\r\n"+"Continue?")) 
					{
						return;
					}
				}
				if(SheetDefs.IsDashboardType(sheetdef)) {
					//PatImage fields use FieldName as a FK to the Patient Image definition.  Since this FK may not be accurate across databases, determine 
					//which Image Category Definition is set as PatImage
					SheetDefs.SetPatImageFieldNames(sheetdef);
				}
				SheetDefs.InsertOrUpdate(sheetdef,isOldSheetDuplicate:sheetdef.DateTCreated.Year < 1880);
				HasSheetsChanged=true;//Flag as true so we know to refresh the grid in FormSheetDefs.cs
				ImportedSheetDefNum=sheetdef.SheetDefNum;//Set this so when we return to FormSheetDefs.cs we can select that row.
			}
			catch(ApplicationException ex) {
				Cursor=Cursors.Default;
				MessageBox.Show(ex.Message);
				return;
			}
			Cursor=Cursors.Default;
			MsgBox.Show(this,"Imported.");
		}

		private void butExport_Click(object sender,EventArgs e) {
			using FormSheetExport formSE=new FormSheetExport(_isOpenedFromDashboardSetup);
			formSE.ShowDialog();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}