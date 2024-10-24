using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using CodeBase;
using Microsoft.Win32;
using OpenDental.Thinfinity;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmEFormDefs : FrmODBase {
		///<summary>This is the list that shows in the grid.</summary>
		private List<EFormDef> _listEFormDefs;
		///<summary>So we know whether to tell other computers to refresh cache.</summary>
		private bool _isChanged;

		///<summary></summary>
		public FrmEFormDefs() {
			InitializeComponent();
			Load+=FrmModFormDefs_Load;
			FormClosed+=FrmEFormDefs_FormClosed;
			gridMain.CellDoubleClick+=grid_CellDoubleClick;
		}

		///<summary></summary>
		private void FrmModFormDefs_Load(object sender, EventArgs e) {
			Lang.F(this);
			InsertInternalEFormsIfNeeded();
			FillGrid();
		}

		///<summary>Whenever this form loads and there are no EFormDefs in the db, then this will copy all of the internal forms into the db. Users will never see internal forms after that unless they click the Add button. Also used in FormEServicesEClipboard</summary>
		private void InsertInternalEFormsIfNeeded(){
			bool didInsertInternal=EForms.InsertInternalToDb();
			if(!didInsertInternal){
				return;//Custom EForms already exist in the db.
			}
			EFormDefs.RefreshCache();
			EFormFieldDefs.RefreshCache();
			_isChanged=true;
		}

		///<summary></summary>
		private void FillGrid() {
			//Note that the EFormDefs do not have any EFormFieldDefs attached. That happens when user double clicks.
			_listEFormDefs=EFormDefs.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn(Lang.g(this,"Description"),130);
			gridColumn.IsWidthDynamic=true;
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<_listEFormDefs.Count;i++){
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(_listEFormDefs[i].Description);
				//gridRow.Tag=;//not needed
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		///<summary></summary>
		private void grid_CellDoubleClick(object sender,GridClickEventArgs e) {
			EFormDef eFormDef=_listEFormDefs[e.Row];
			eFormDef.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==eFormDef.EFormDefNum);
			FrmEFormDefEdit frmEFormDefEdit=new FrmEFormDefEdit();
			frmEFormDefEdit.EFormDefCur=eFormDef;
			frmEFormDefEdit.ShowDialog();
			//frmEFormDefEdit.EFormDefCur.IsNew=true;//this is not needed because all decisions happen here, not inside the edit window.
			if(frmEFormDefEdit.IsDialogCancel){
				//nothing to do if they clicked Cancel
				return;
			}
			if(frmEFormDefEdit.IsDeleted){
				//We must delete the form and the fields from the db
				EFormDefs.Delete(eFormDef.EFormDefNum);
				for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++) {
					EFormFieldDefs.Delete(eFormDef.ListEFormFieldDefs[i].EFormFieldDefNum);
				}
				frmEFormDefEdit.DeleteAllMarked();
			}
			else{
				//They clicked save
				EFormDefs.Update(eFormDef);
				//Any new fields were already inserted a few lines up or within the form editor.
				//Also, when user clicked save, it converted fields back to fieldDefs, all properly attached to the eFormDef.
				//But all of them still need to be updated to the db
				for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++) {
					EFormFieldDefs.Update(eFormDef.ListEFormFieldDefs[i]);
				}
				frmEFormDefEdit.DeleteAllMarked();
			}
			EFormDefs.RefreshCache();
			EFormFieldDefs.RefreshCache();
			_isChanged=true;
			FillGrid();
			for(int i=0;i<_listEFormDefs.Count;i++){
				if(_listEFormDefs[i].EFormDefNum==frmEFormDefEdit.EFormDefCur.EFormDefNum){
					gridMain.SetSelected(i);
				}
			}
		}

		private void butSetup_Click(object sender,EventArgs e) {
			FrmEFormSetup frmEFormSetup=new FrmEFormSetup();
			frmEFormSetup.ShowDialog();
		}

		///<summary>Opens File Explorer and lets the user select a previously exported eFormDef. This should only allow users to import an eFormDef that was exported using Open Dental.</summary>
		private void butImport_Click(object sender,EventArgs e) {
			Cursor=Cursors.Wait;
			string importFilePath="";
			if(!ODBuild.IsThinfinity() && ODCloudClient.IsAppStream) {
				importFilePath=ODCloudClient.ImportFileForCloud();
				if(importFilePath.IsNullOrEmpty()) {
					Cursor=Cursors.Arrow;
					return; //User cancelled out of OpenFileDialog
				}
			}
			else {
				OpenFileDialog openFileDialog=new OpenFileDialog();
				string initDir=PrefC.GetString(PrefName.ExportPath);
				if(Directory.Exists(initDir)) {
					openFileDialog.InitialDirectory=initDir;
				}
				if(openFileDialog.ShowDialog()==false) {
					Cursor=Cursors.Arrow;
					return;
				}
				importFilePath=openFileDialog.FileName;
			}
			EFormDef eFormDef=new EFormDef();
			XmlSerializer serializer=new XmlSerializer(typeof(EFormDef));
			if(importFilePath=="") {
				Cursor=Cursors.Arrow;
				return;
			}
			if(!File.Exists(importFilePath)){
				Cursor=Cursors.Arrow;
				MsgBox.Show(this,"File not found");
				return;
			}
			try {
				using TextReader reader=new StreamReader(importFilePath);
				eFormDef=(EFormDef)serializer.Deserialize(reader);
			}
			catch {
				Cursor=Cursors.Arrow;
				MsgBox.Show(this,"Invalid file format.");
				return;
			}
			eFormDef.DateTCreated=DateTime.Now;
			//Insert the eFormWrapper.EFormDef and eFormWrapper.ListEFormFieldDefs
			EFormDefs.Insert(eFormDef);
			for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++) {
				eFormDef.ListEFormFieldDefs[i].EFormDefNum=eFormDef.EFormDefNum;
				eFormDef.ListEFormFieldDefs[i].ItemOrder=i;
				EFormFieldDefs.Insert(eFormDef.ListEFormFieldDefs[i]);//ignores any existing PK when inserting
			}
			EFormDefs.RefreshCache();
			EFormFieldDefs.RefreshCache();
			_isChanged=true;
			FillGrid();
			Cursor=Cursors.Arrow;
			MsgBox.Show(this,"Imported.");
		}

		///<summary>Allows users to export custom eFormDefs from Open Dental. This would be useful if they want to move an eFormDef to a different Open Dental database.</summary>
		private void butExport_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1) {
				MsgBox.Show(this,"Please select an eForm from the list first.");
				return;
			}
			EFormDef eFormDef=_listEFormDefs[idx];
			eFormDef.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==eFormDef.EFormDefNum);
			eFormDef.EFormDefNum=0;
			for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++){
				eFormDef.ListEFormFieldDefs[0].EFormDefNum=0;
			}
			XmlSerializer xmlSerializer=new XmlSerializer(typeof(EFormDef));
			string fileName="eFormDefCustom.xml";
			if(ODEnvironment.IsCloudServer) {
				StringBuilder stringBuilder=new StringBuilder();
				using XmlWriter xmlWriter=XmlWriter.Create(stringBuilder);
				xmlSerializer.Serialize(xmlWriter,eFormDef);
				xmlWriter.Close();
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.ExportForDownload(fileName,stringBuilder.ToString());
				}
				else {//Is AppStream
					File.WriteAllText(fileName,stringBuilder.ToString());
					CloudClientL.ExportForCloud(fileName);
				}
			}
			else {
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
				if(ODBuild.IsDebug()) {
					if(Environment.MachineName.ToLower()=="ryanr"){
						saveFileDialog.InitialDirectory="C:\\Users\\ryanr\\Desktop";
					}
					if(Environment.MachineName.ToLower()=="jordanhome"){
						saveFileDialog.InitialDirectory=@"E:\Documents\GIT REPOS\Versioned\OpenDental\OpenDentBusiness\Resources\EForms";
					}
				}
				else { 
					if(Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
						saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					}
				}
				saveFileDialog.FileName=fileName;
				if(saveFileDialog.ShowDialog()==false) {
					return;
				}
				using StreamWriter streamWriter=new StreamWriter(saveFileDialog.FileName);
				xmlSerializer.Serialize(streamWriter,eFormDef);
				streamWriter.Close();
			}
			//we changed some things on the selected object, so refresh
			FillGrid();
			gridMain.SetSelected(idx);
			MsgBox.Show(this,"Exported");
		}

		///<summary>Either blank or from an internal eForm.</summary>
		private void butAdd_Click(object sender,EventArgs e) {
			FrmEFormPicker frmEFormPicker=new FrmEFormPicker();
			frmEFormPicker.IsInternalPicker=true;
			frmEFormPicker.ShowDialog();
			if(frmEFormPicker.IsDialogCancel){
				return;
			}
			//could be blank or internal at this point
			EFormDef eFormDef=frmEFormPicker.EFormDefSelected;
			//The form will have all fields attached, whether it's internal or blank
			//In both cases, we must add the eForm and all fields to the db right now
			//so that we have PKs available to attach languagepats to.
			eFormDef.DateTCreated=DateTime.Now;
			EFormDefs.Insert(eFormDef);
			for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++){//emptly list for blank form
				EFormFieldDefs.Insert(eFormDef.ListEFormFieldDefs[i]);
			}
			FrmEFormDefEdit frmEFormDefEdit=new FrmEFormDefEdit();
			frmEFormDefEdit.EFormDefCur=eFormDef;//fields already attached and everything is already saved to db
			//frmEFormDefEdit.EFormDefCur.IsNew=true;//this is not needed because all decisions happen here, not inside the edit window.
			frmEFormDefEdit.ShowDialog();
			if(frmEFormDefEdit.IsDialogCancel || frmEFormDefEdit.IsDeleted){
				//They either clicked Cancel or Delete. Both have the exact same effect here.
				EFormDefs.Delete(eFormDef.EFormDefNum);
				for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++) {
					EFormFieldDefs.Delete(eFormDef.ListEFormFieldDefs[i].EFormFieldDefNum);
				}
				frmEFormDefEdit.DeleteAllMarked();
				return;
			}
			//They clicked Save
			EFormDefs.Update(eFormDef);
			//Any new fields were already inserted a few lines up or within the form editor.
			//Also, when user clicked save, it converted fields back to fieldDefs, all properly attached to the eFormDef.
			//But all of them still need to be updated to the db
			for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++) {
				EFormFieldDefs.Update(eFormDef.ListEFormFieldDefs[i]);
			}
			frmEFormDefEdit.DeleteAllMarked();
			EFormDefs.RefreshCache();
			EFormFieldDefs.RefreshCache();
			_isChanged=true;
			FillGrid();
			//Highlight the one they just added
			int idx=_listEFormDefs.FindIndex(x=>x.EFormDefNum==eFormDef.EFormDefNum);
			gridMain.SetSelected(idx);
		}

		///<summary>Duplicates an internal or existing custom eFormDef. Adds " Copy" to the eFormDef description.</summary>
		private void butDuplicate_Click(object sender,EventArgs e) { 
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			EFormDef eFormDef=_listEFormDefs[idx];
			eFormDef.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==eFormDef.EFormDefNum);
			//List<EFormFieldDef> listEFormFieldDef=EFormFieldDefs.GetWhere(x => x.EFormDefNum==_listEFormDefsCustom[idx].EFormDefNum);
			eFormDef.Description+=" Copy";
			eFormDef.DateTCreated=DateTime.Now;
			//_listEFormDefsCustom.Add(eFormDef);
			long eFormDefNum=EFormDefs.Insert(eFormDef);
			for(int i=0;i<eFormDef.ListEFormFieldDefs.Count;i++){
				eFormDef.ListEFormFieldDefs[i].EFormDefNum=eFormDefNum;
				eFormDef.ListEFormFieldDefs[i].ItemOrder=i;
				EFormFieldDefs.Insert(eFormDef.ListEFormFieldDefs[i]);
			}
			EFormDefs.RefreshCache();
			EFormFieldDefs.RefreshCache();
			_isChanged=true;
			FillGrid();
		}

		private void FrmEFormDefs_FormClosed(object sender,EventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.Sheets);
			}
		}
	}
}