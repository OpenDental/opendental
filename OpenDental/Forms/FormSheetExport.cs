using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDental.Thinfinity;

namespace OpenDental {
	public partial class FormSheetExport:FormODBase {
		private List<SheetDef> _listSheetDefs;
		///<summary>Form was opened via Dashboard Setup.</summary>
		private bool _isOpenedFromDashboardSetup;

		public FormSheetExport(bool isOpenedFromDashboardSetup) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isOpenedFromDashboardSetup=isOpenedFromDashboardSetup;
		}

		private void FormSheetExport_Load(object sender,EventArgs e) {
			FillGridCustomSheet();
		}

		private void FillGridCustomSheet() {
			SheetDefs.RefreshCache();
			SheetFieldDefs.RefreshCache();
			//If from Dashboard Setup, populate when SheetDef is a Dashboard type.
			//If from normal Sheet window, populate all Sheets except Dashboard types.
			_listSheetDefs=SheetDefs.GetDeepCopy(false).FindAll(x => SheetDefs.IsDashboardType(x)==_isOpenedFromDashboardSetup);
			gridCustomSheet.BeginUpdate();
			gridCustomSheet.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableSheetDef","Description"),170);
			gridCustomSheet.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableSheetDef","Type"),100);
			gridCustomSheet.ListGridColumns.Add(col);
			gridCustomSheet.ListGridRows.Clear();
			GridRow row;
			foreach(SheetDef sheetDef in _listSheetDefs) {
				row=new GridRow();
				row.Cells.Add(sheetDef.Description);
				row.Cells.Add(sheetDef.SheetType.ToString());
				gridCustomSheet.ListGridRows.Add(row);
			}
			gridCustomSheet.EndUpdate();
		}

		private void butExport_Click(object sender,EventArgs e) {
			if(gridCustomSheet.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a sheet from the list first.");
				return;
			}
			SheetDef sheetdef=SheetDefs.GetSheetDef(_listSheetDefs[gridCustomSheet.GetSelectedIndex()].SheetDefNum);
			List<SheetFieldDef> listFieldDefImages=sheetdef.SheetFieldDefs
				.Where(x => x.FieldType==SheetFieldType.Image && x.FieldName!="Patient Info.gif")
				.ToList();
			if(!listFieldDefImages.IsNullOrEmpty()) {//Alert them of any images they need to copy if there are any.
				string sheetImagesPath="";
				ODException.SwallowAnyException(() => {
					sheetImagesPath=SheetUtil.GetImagePath();
				});
				StringBuilder strBuilder=new StringBuilder();
				strBuilder.AppendLine(Lan.g(this,"The following images will need to be manually imported with the same file name when importing this "
					+"sheet to a new environment."));
				strBuilder.AppendLine();
				listFieldDefImages.ForEach(x => strBuilder.AppendLine(ODFileUtils.CombinePaths(sheetImagesPath,x.FieldName)));
				using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(strBuilder.ToString());
				msgBox.ShowDialog();
			}
			XmlSerializer serializer=new XmlSerializer(typeof(SheetDef));
			string filename="SheetDefCustom.xml";
			if(ODBuild.IsWeb()) {
				StringBuilder strbuild=new StringBuilder();
				using(XmlWriter writer=XmlWriter.Create(strbuild)) {
					serializer.Serialize(writer,sheetdef);
				}
				ThinfinityUtils.ExportForDownload(filename,strbuild.ToString());
			}
			else {
				using SaveFileDialog saveDlg=new SaveFileDialog();
				saveDlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				saveDlg.FileName=filename;
				if(saveDlg.ShowDialog()!=DialogResult.OK) {
					return;
				}
				using(TextWriter writer=new StreamWriter(saveDlg.FileName)) {
					serializer.Serialize(writer,sheetdef);
				}
			}
			MsgBox.Show(this,"Exported");
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

	}
}