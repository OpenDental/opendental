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
			gridCustomSheet.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableSheetDef","Description"),170);
			gridCustomSheet.Columns.Add(col);
			col=new GridColumn(Lan.g("TableSheetDef","Type"),100);
			gridCustomSheet.Columns.Add(col);
			gridCustomSheet.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listSheetDefs.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listSheetDefs[i].Description);
				row.Cells.Add(_listSheetDefs[i].SheetType.ToString());
				gridCustomSheet.ListGridRows.Add(row);
			}
			gridCustomSheet.EndUpdate();
		}

		private void butExport_Click(object sender,EventArgs e) {
			if(gridCustomSheet.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a sheet from the list first.");
				return;
			}
			SheetDef sheetDef=SheetDefs.GetSheetDef(_listSheetDefs[gridCustomSheet.GetSelectedIndex()].SheetDefNum);
			List<SheetFieldDef> listFieldDefImages=sheetDef.SheetFieldDefs.FindAll(x => x.FieldType==SheetFieldType.Image && x.FieldName!="Patient Info.gif");
			if(!listFieldDefImages.IsNullOrEmpty()) {//Alert them of any images they need to copy if there are any.
				string sheetImagesPath="";
				ODException.SwallowAnyException(() => {
					sheetImagesPath=SheetUtil.GetImagePath();
				});
				StringBuilder stringBuilder=new StringBuilder();
				stringBuilder.AppendLine(Lan.g(this,"The following images will need to be manually imported with the same file name when importing this "
					+"sheet to a new environment."));
				stringBuilder.AppendLine();
				for(int i=0;i<listFieldDefImages.Count;i++){
					stringBuilder.AppendLine(ODFileUtils.CombinePaths(sheetImagesPath,listFieldDefImages[i].FieldName));
				}
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(stringBuilder.ToString());
				msgBoxCopyPaste.ShowDialog();
			}
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(SheetDef));
			string fileName="SheetDefCustom.xml";
			if(ODEnvironment.IsCloudServer) {
				StringBuilder stringBuilder2=new StringBuilder();
				using XmlWriter xmlWriter=XmlWriter.Create(stringBuilder2);
				xmlSerializer.Serialize(xmlWriter,sheetDef);
				xmlWriter.Close();
				if(ODCloudClient.IsAppStream) {
					File.WriteAllText(fileName,stringBuilder2.ToString());
					CloudClientL.ExportForCloud(fileName);
				}
				else {
					ThinfinityUtils.ExportForDownload(fileName,stringBuilder2.ToString());
				}
			}
			else {
				using SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				saveFileDialog.FileName=fileName;
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				using TextWriter textWriter=new StreamWriter(saveFileDialog.FileName);
				xmlSerializer.Serialize(textWriter,sheetDef);
				textWriter.Close();
			}
			MsgBox.Show(this,"Exported");
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}




	}
}