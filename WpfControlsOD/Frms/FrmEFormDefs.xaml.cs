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
		///<summary>This is the list that shows in the grid, a mix of internal and custom.</summary>
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
			FillGrid();
		}

		///<summary></summary>
		private void FillGrid() {
			//This code is similar to that in FrmEFormPicker.
			_listEFormDefs=new List<EFormDef>();
			List<EFormDef> listEFormDefsInternal=EFormInternal.GetAllInternal();
			List<EFormDef> listEFormDefsCustom=EFormDefs.GetDeepCopy();
			List<EFormDef> listEFormDefsInternalShort=new List<EFormDef>();//this one will not have hidden/deleted
			for(int i=0;i<listEFormDefsInternal.Count;i++){
				if(listEFormDefsCustom.Exists(x=>x.IsInternalHidden && x.Description==listEFormDefsInternal[i].Description)){
					continue;//don't add forms that user had hidden/deleted
				}
				listEFormDefsInternalShort.Add(listEFormDefsInternal[i]);
			}
			//patient forms
			_listEFormDefs.AddRange(listEFormDefsCustom.FindAll(x=>x.FormType==EnumEFormType.PatientForm && !x.IsInternalHidden));//ignore the ones used for internalHidden
			_listEFormDefs.AddRange(listEFormDefsInternalShort.FindAll(x=>x.FormType==EnumEFormType.PatientForm));
			//medical history
			_listEFormDefs.AddRange(listEFormDefsCustom.FindAll(x=>x.FormType==EnumEFormType.MedicalHistory && !x.IsInternalHidden));
			_listEFormDefs.AddRange(listEFormDefsInternalShort.FindAll(x=>x.FormType==EnumEFormType.MedicalHistory));
			//consent
			_listEFormDefs.AddRange(listEFormDefsCustom.FindAll(x=>x.FormType==EnumEFormType.Consent && !x.IsInternalHidden));
			_listEFormDefs.AddRange(listEFormDefsInternalShort.FindAll(x=>x.FormType==EnumEFormType.Consent));
			//user won't be able to see which ones are internal and which are custom.
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
			FrmEFormDefEdit frmEFormDefEdit=new FrmEFormDefEdit();
			frmEFormDefEdit.EFormDefCur=eFormDef;
			string description=eFormDef.Description;//in case they change it inside.
			if(eFormDef.IsInternal){
				//This is going to work differently than sheets. When a user double clicks on an internal eFormDef,
				//it will open the editor with essentially a copy of the internal eFormDef.
				//Nothing gets inserted into the database until the user clicks save in the editor.
				frmEFormDefEdit.EFormDefCur.IsNew=true;//causes it to insert in that window when click Save.
			}
			else{//custom, so it's in cache
				eFormDef.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==eFormDef.EFormDefNum);//also works for 0 with new form.
			}
			frmEFormDefEdit.ShowDialog();
			if(frmEFormDefEdit.IsInternalDeleted){
				//They can't really delete an internal eForm, of course, but we don't tell them that.
				//If they were in an internal form, then what we really do is mark that internal form as hidden.
				EFormDef eFormDef2=new EFormDef();
				eFormDef2.IsInternalHidden=true;
				eFormDef2.Description=description;
				EFormDefs.Insert(eFormDef2);
			}
			else if(frmEFormDefEdit.IsDialogCancel){
				return;
			}
			else if(eFormDef.IsInternal){//this means they "saved" from an internal form.
				//must mark the internal form as hidden because we only want to show their custom form
				EFormDef eFormDef2=new EFormDef();
				eFormDef2.IsInternalHidden=true;
				eFormDef2.Description=description;
				EFormDefs.Insert(eFormDef2);
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
			if(eFormDef.IsInternal){
				//eFormDef.ListEFormFieldDefs //already attached
			}
			else{//custom, so it's in cache
				eFormDef.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==eFormDef.EFormDefNum);
			}
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
			string description=frmEFormPicker.EFormDefSelected.Description;//in case they change it inside.
			FrmEFormDefEdit frmEFormDefEdit=new FrmEFormDefEdit();
			frmEFormDefEdit.EFormDefCur=frmEFormPicker.EFormDefSelected;//fields already attached
			frmEFormDefEdit.EFormDefCur.IsNew=true;
			frmEFormDefEdit.ShowDialog();
			//since they're adding, nothing to worry about deleting.
			if(frmEFormDefEdit.IsDialogCancel){
				return;
			}
			else if(frmEFormPicker.EFormDefSelected.IsInternal){//this means they "saved" from an internal form.
				//must mark the internal form as hidden because we only want to show their custom form
				EFormDef eFormDef2=new EFormDef();
				eFormDef2.IsInternalHidden=true;
				eFormDef2.Description=description;
				EFormDefs.Insert(eFormDef2);
			}
			EFormDefs.RefreshCache();
			EFormFieldDefs.RefreshCache();
			_isChanged=true;
			FillGrid();
			//can't think of any easy way to highlight the one they just added.
			EFormDef eFormDef=_listEFormDefs.OrderByDescending(x=>x.DateTCreated).FirstOrDefault();
			if(eFormDef!=null){
				gridMain.SetSelected(_listEFormDefs.IndexOf(eFormDef));
			}
		}

		///<summary>Duplicates an internal or existing custom eFormDef. Adds " Copy" to the eFormDef description.</summary>
		private void butDuplicate_Click(object sender,EventArgs e) { 
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			EFormDef eFormDef=_listEFormDefs[idx];
			if(eFormDef.IsInternal){
				//eFormDef.ListEFormFieldDefs //already attached
			}
			else{//custom, so it's in cache
				eFormDef.ListEFormFieldDefs=EFormFieldDefs.GetDeepCopy().FindAll(x=>x.EFormDefNum==eFormDef.EFormDefNum);
			}
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