using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormLetterMerges : FormODBase {
		//private bool localChanged;
		//private int pagesPrinted=0;
		private Patient PatCur;
		private List<LetterMerge> ListForCat;
		private bool changed;
		private string mergePath;
#if !DISABLE_MICROSOFT_OFFICE
		//private Word.Application wrdApp;
		private Word._Document wrdDoc;
		private Object oMissing = System.Reflection.Missing.Value;
		private Object oFalse = false;
#endif
		private List<Def> _listLetterMergeCatDefs;

		///<summary></summary>
		public FormLetterMerges(Patient patCur){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			PatCur=patCur;
			Lan.F(this);
		}

		
		private void FormLetterMerges_Load(object sender, System.EventArgs e) {
			mergePath=PrefC.GetString(PrefName.LetterMergePath);
			FillCats();
			if(listCategories.Items.Count>0){
				listCategories.SelectedIndex=0;
			}
			FillLetters();
			if(listLetters.Items.Count>0){
				listLetters.SelectedIndex=0;
			}
			comboImageCategory.Items.AddDefNone();
			comboImageCategory.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ImageCats,true));
			SelectImageCat();
		}

		private void FillCats() {
			_listLetterMergeCatDefs=Defs.GetDefsForCategory(DefCat.LetterMergeCats,true);
			listCategories.Items.Clear();
			for(int i=0;i<_listLetterMergeCatDefs.Count;i++){
				listCategories.Items.Add(_listLetterMergeCatDefs[i].ItemName);
			}
		}

		private void FillLetters(){
			listLetters.Items.Clear();
			if(listCategories.SelectedIndex==-1){
				ListForCat=new List<LetterMerge>();
				return;
			}
			LetterMergeFields.RefreshCache();
			LetterMerges.RefreshCache();
			ListForCat=LetterMerges.GetListForCat(listCategories.SelectedIndex);
			for(int i=0;i<ListForCat.Count;i++){
				listLetters.Items.Add(ListForCat[i].Description);
			}
		}

		private void SelectImageCat() {
			long defNumLetter=0;
			if(listLetters.Items.Count>0 && listLetters.SelectedIndex>=0) {
				LetterMerge letterMergeSelected=ListForCat[listLetters.SelectedIndex];
				if(letterMergeSelected!=null) {
					defNumLetter=letterMergeSelected.ImageFolder;
				}
			}
			comboImageCategory.SetSelectedDefNum(defNumLetter); 
		}

		private void listCategories_Click(object sender, System.EventArgs e) {
			//selectedIndex already changed.
			FillLetters();
			if(listLetters.Items.Count>0){
				listLetters.SelectedIndex=0;
			}
			SelectImageCat();
		}

		private void butEditCats_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)){
				return;
			}
			using FormDefinitions FormD=new FormDefinitions(DefCat.LetterMergeCats);
			FormD.ShowDialog();
			FillCats();
		}

		private void listLetters_DoubleClick(object sender, System.EventArgs e) {
			if(listLetters.SelectedIndex==-1){
				return;
			}
			int selectedRow=listLetters.SelectedIndex;
			LetterMerge letter=ListForCat[listLetters.SelectedIndex];
			using FormLetterMergeEdit FormL=new FormLetterMergeEdit(letter);
			FormL.ShowDialog();
			FillLetters();
			if(listLetters.Items.Count>selectedRow){
				listLetters.SetSelected(selectedRow);
			}
			if(listLetters.SelectedIndex==-1 && listLetters.Items.Count>0) {
				listLetters.SelectedIndex=0;
			}
			SelectImageCat();
			changed=true;
		}

		private void listLetters_Click(object sender,EventArgs e) {
			if(listLetters.SelectedIndex==-1) {
				return;
			}
			SelectImageCat();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(listCategories.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			LetterMerge letter=new LetterMerge();
			letter.Category=_listLetterMergeCatDefs[listCategories.SelectedIndex].DefNum;
			using FormLetterMergeEdit FormL=new FormLetterMergeEdit(letter);
			FormL.IsNew=true;
			FormL.ShowDialog();
			FillLetters();
			changed=true;
		}

		///<summary>Shows and error message and returns false if there is a problem creating the data file; Otherwise true.</summary>
		private bool CreateDataFile(string fileName,LetterMerge letter){
 			DataTable table;
			try {
				table=LetterMergesQueries.GetLetterMergeInfo(PatCur,letter);
			}
			catch(Exception ex) {
				string message=Lan.g(this,"There was a error getting letter merge info:");
				MessageBox.Show(message+"\r\n"+ex.Message);
				return false;
			}
			table=FormQuery.MakeReadable(table,null,false);
			try{
			  using(StreamWriter sw=new StreamWriter(fileName,false)){
					string line="";  
					for(int i=0;i<letter.Fields.Count;i++){
						if(letter.Fields[i].StartsWith("referral.")){
							line+="Ref"+letter.Fields[i].Substring(9);
						}
						else{
							line+=letter.Fields[i];
						}
						if(i<letter.Fields.Count-1){
							line+="\t";
						}
					}
					sw.WriteLine(line);
					string cell;
					for(int i=0;i<table.Rows.Count;i++){
						line="";
						for(int j=0;j<table.Columns.Count;j++){
							cell=table.Rows[i][j].ToString();
							cell=cell.Replace("\r","");
							cell=cell.Replace("\n","");
							cell=cell.Replace("\t","");
							cell=cell.Replace("\"","");
							line+=cell;
							if(j<table.Columns.Count-1){
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}
      }
      catch{
        MsgBox.Show(this,"File in use by another program.  Close and try again.");
				return false;
			}
			return true;
		}

		private void butCreateData_Click(object sender, System.EventArgs e) {
			if(!CreateData()){
				return;
			}
			MsgBox.Show(this,"done");
		}

		private void butViewData_Click(object sender,EventArgs e) {
			if(!CreateData()){
				return;
			}
			LetterMerge letterCur=ListForCat[listLetters.SelectedIndex];
			string dataFile=PrefC.GetString(PrefName.LetterMergePath)+letterCur.DataFileName;
			Process.Start(dataFile);
		}

		private bool CreateData(){
			if(listLetters.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a letter first.");
				return false;
			}
			LetterMerge letterCur=ListForCat[listLetters.SelectedIndex];
			string dataFile=PrefC.GetString(PrefName.LetterMergePath)+letterCur.DataFileName;
			if(!Directory.Exists(PrefC.GetString(PrefName.LetterMergePath))){
				MsgBox.Show(this,"Letter merge path not valid.");
				return false;
			}
			Cursor=Cursors.WaitCursor;
			if(!CreateDataFile(dataFile,letterCur)){
				Cursor=Cursors.Default;
				return false;
			}
			Cursor=Cursors.Default;
			return true;
		}

		private void butPrint_Click(object sender, System.EventArgs e) {//TODO: Implement ODprintout pattern
#if DISABLE_MICROSOFT_OFFICE
			MessageBox.Show(this, "This version of Open Dental does not support Microsoft Word.");
			return;
#endif
			if(listLetters.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a letter first.");
				return;
			}
			LetterMerge letterCur=ListForCat[listLetters.SelectedIndex];
			letterCur.ImageFolder=comboImageCategory.GetSelectedDefNum();
			string templateFile=ODFileUtils.CombinePaths(PrefC.GetString(PrefName.LetterMergePath),letterCur.TemplateName);
			string dataFile=ODFileUtils.CombinePaths(PrefC.GetString(PrefName.LetterMergePath),letterCur.DataFileName);
			if(!File.Exists(templateFile)){
				MsgBox.Show(this,"Template file does not exist.");
				return;
			}
			PrintDocument pd=new PrintDocument();
			if(!PrinterL.SetPrinter(pd,PrintSituation.Default,PatCur.PatNum,"Letter merge "+letterCur.Description+" printed")) {
				return;
			}
			if(!CreateDataFile(dataFile,letterCur)){
				return;
			}
			Word.MailMerge wrdMailMerge;
			//Create an instance of Word.
			Word.Application WrdApp;
			try {
				WrdApp=LetterMerges.WordApp;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error. Is MS Word installed?"),ex);
				return;
			}
			//Open a document.
			Object oName=templateFile;
			wrdDoc=WrdApp.Documents.Open(ref oName,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
			wrdDoc.Select();
			wrdMailMerge=wrdDoc.MailMerge;
			//Attach the data file.
			wrdDoc.MailMerge.OpenDataSource(dataFile,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
			wrdMailMerge.Destination = Word.WdMailMergeDestination.wdSendToPrinter;
			//WrdApp.ActivePrinter=pd.PrinterSettings.PrinterName;
			//replaced with following 4 lines due to MS bug that changes computer default printer
			object oWBasic = WrdApp.WordBasic;
			object[] oWBValues = new object[] { pd.PrinterSettings.PrinterName, 1 };
			String[] sWBNames = new String[] { "Printer", "DoNotSetAsSysDefault" };
			oWBasic.GetType().InvokeMember("FilePrintSetup", BindingFlags.InvokeMethod, null, oWBasic, oWBValues, null, null, sWBNames);
			wrdMailMerge.Execute(ref oFalse);
			if(letterCur.ImageFolder!=0) {//if image folder exist for this letter, save to AtoZ folder
				try {
					wrdDoc.Select();
					wrdMailMerge.Destination = Word.WdMailMergeDestination.wdSendToNewDocument;
					wrdMailMerge.Execute(ref oFalse);
					WrdApp.Activate();
					string tempFilePath=ODFileUtils.CreateRandomFile(Path.GetTempPath(),GetFileExtensionForWordDoc(templateFile));
					Object oFileName=tempFilePath;
					WrdApp.ActiveDocument.SaveAs(oFileName);//save the document 
					WrdApp.ActiveDocument.Close();
					SaveToImageFolder(tempFilePath,letterCur);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Error saving file to the Image module:")+"\r\n"+ex.Message,ex);
				}
			}
			//Close the original form document since just one record.
			wrdDoc.Saved=true;
			wrdDoc.Close(ref oFalse,ref oMissing,ref oMissing);
			//At this point, Word remains open with no documents.
			WrdApp.WindowState=Word.WdWindowState.wdWindowStateMinimize;
			wrdMailMerge=null;
			wrdDoc=null;
			Commlog CommlogCur=new Commlog();
			CommlogCur.CommDateTime=DateTime.Now;
			CommlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			CommlogCur.Mode_=CommItemMode.Mail;
			CommlogCur.SentOrReceived=CommSentOrReceived.Sent;
			CommlogCur.PatNum=PatCur.PatNum;
			CommlogCur.Note="Letter sent: "+letterCur.Description+". ";
			CommlogCur.UserNum=Security.CurUser.UserNum;
			Commlogs.Insert(CommlogCur);
			DialogResult=DialogResult.OK;
		}

		private void butPreview_Click(object sender, System.EventArgs e) {
#if !DISABLE_MICROSOFT_OFFICE
			if(listLetters.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a letter first.");
				return;
			}
			LetterMerge letterCur=ListForCat[listLetters.SelectedIndex];
			letterCur.ImageFolder=comboImageCategory.GetSelectedDefNum();
			string templateFile=ODFileUtils.CombinePaths(PrefC.GetString(PrefName.LetterMergePath),letterCur.TemplateName);
			string dataFile=ODFileUtils.CombinePaths(PrefC.GetString(PrefName.LetterMergePath),letterCur.DataFileName);
			if(!File.Exists(templateFile)){
				MsgBox.Show(this,"Template file does not exist.");
				return;
			}
			if(!CreateDataFile(dataFile,letterCur)){
				return;
			}
			Word.MailMerge wrdMailMerge;
			//Create an instance of Word.
			Word.Application WrdApp;
			try{
				WrdApp=LetterMerges.WordApp;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error. Is MS Word installed?"),ex);
				return;
			}
			string errorMessage="";
			//Open a document.
			try {
				Object oName=templateFile;
				wrdDoc=WrdApp.Documents.Open(ref oName,ref oMissing,ref oMissing,
					ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
					ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
				wrdDoc.Select();
			}
			catch(Exception ex) {
				errorMessage=Lan.g(this,"Error opening document:")+"\r\n"+ex.Message;
				MessageBox.Show(errorMessage);
				return;
			}
			//Attach the data file.
			try {
				wrdMailMerge=wrdDoc.MailMerge;
				wrdDoc.MailMerge.OpenDataSource(dataFile,ref oMissing,ref oMissing,ref oMissing,
					ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
					ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
				wrdMailMerge.Destination = Word.WdMailMergeDestination.wdSendToNewDocument;
				wrdMailMerge.Execute(ref oFalse);
			}
			catch(Exception ex) {
				errorMessage=Lan.g(this,"Error attaching data file:")+"\r\n"+ex.Message;
				MessageBox.Show(errorMessage);
				return;
			}
			if(letterCur.ImageFolder!=0) {//if image folder exist for this letter, save to AtoZ folder
				//Open document from the atoz folder.
				try {
					WrdApp.Activate();
					string tempFilePath=ODFileUtils.CreateRandomFile(Path.GetTempPath(),GetFileExtensionForWordDoc(templateFile));
					Object oFileName=tempFilePath;
					WrdApp.ActiveDocument.SaveAs(oFileName);//save the document to temp location
					Document doc=SaveToImageFolder(tempFilePath,letterCur);
					string patFolder=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
					string fileName=ImageStore.GetFilePath(doc,patFolder);
					if(!FileAtoZ.Exists(fileName)) {
						throw new ApplicationException(Lans.g("LetterMerge","Error opening document"+" "+doc.FileName));
					}
					FileAtoZ.StartProcess(fileName);
					WrdApp.ActiveDocument.Close();//Necessary since we created an extra document
					try {
						File.Delete(tempFilePath);//Clean up the temp file
					}
					catch(Exception ex) {
						ex.DoNothing();
					}
				}
				catch(Exception ex) {
					FriendlyException.Show(Lan.g(this,"Error saving file to the Image module:")+"\r\n"+ex.Message,ex);
				}
			}
			//Close the original form document since just one record.
			try {
				wrdDoc.Saved=true;
				wrdDoc.Close(ref oFalse,ref oMissing,ref oMissing);
			}
			catch(Exception ex) {
				errorMessage=Lan.g(this,"Error closing document:")+"\r\n"+ex.Message;
				MessageBox.Show(errorMessage);
				return;
			}
			//At this point, Word remains open with just one new document.
			try {
				WrdApp.Activate();
				if(WrdApp.WindowState==Word.WdWindowState.wdWindowStateMinimize) {
					WrdApp.WindowState=Word.WdWindowState.wdWindowStateMaximize;
				}
			}
			catch(Exception ex) {
				errorMessage=Lan.g(this,"Error showing Microsoft Word:")+"\r\n"+ex.Message;
				MessageBox.Show(errorMessage);
				return;
			}
			wrdMailMerge=null;
			wrdDoc=null;
			Commlog CommlogCur=new Commlog();
			CommlogCur.CommDateTime=DateTime.Now;
			CommlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			CommlogCur.Mode_=CommItemMode.Mail;
			CommlogCur.SentOrReceived=CommSentOrReceived.Sent;
			CommlogCur.PatNum=PatCur.PatNum;
			CommlogCur.Note="Letter sent: "+letterCur.Description+". ";
			CommlogCur.UserNum=Security.CurUser.UserNum;
			Commlogs.Insert(CommlogCur);
#else
			MessageBox.Show(this, "This version of Open Dental does not support Microsoft Word.");
#endif
			//this window now closes regardless of whether the user saved the comm item.
			DialogResult=DialogResult.OK;
		}

		///<summary>Returns default Microsoft Word extension of .docx. Returns extension .doc If the file passed in has an extension of .dot,.doc,or .dotm.</summary>
		private string GetFileExtensionForWordDoc(string filePath) {
			string retVal=".docx";//default file extension
			string ext=Path.GetExtension(filePath).ToLower();
			List<string> listBackwardCompat=new List<string> { ".dot",".doc",".dotm" };
			if(listBackwardCompat.Contains(ext)) {
				retVal=".doc";
			}
			return retVal;
		}

		private Document SaveToImageFolder(string fileSourcePath,LetterMerge letterCur) {
			if(letterCur.ImageFolder==0) {//This shouldn't happen
				return new Document();
			}
			string rawBase64="";
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				rawBase64=Convert.ToBase64String(File.ReadAllBytes(fileSourcePath));
			}
			Document docSave=new Document();
			docSave.DocNum=Documents.Insert(docSave);
			docSave.ImgType=ImageType.Document;
			docSave.DateCreated=DateTime.Now;
			docSave.PatNum=PatCur.PatNum;
			docSave.DocCategory=letterCur.ImageFolder;
			docSave.Description=letterCur.Description+docSave.DocNum;//no extension.
			docSave.RawBase64=rawBase64;//blank if using AtoZfolder
			docSave.FileName=ODFileUtils.CleanFileName(docSave.Description)+GetFileExtensionForWordDoc(fileSourcePath);
			string fileDestPath=ImageStore.GetFilePath(docSave,ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath()));
			FileAtoZ.Copy(fileSourcePath,fileDestPath,FileAtoZSourceDestination.LocalToAtoZ);
			Documents.Update(docSave);
			return docSave;
		}

		private void butEditTemplate_Click(object sender, System.EventArgs e) {
#if !DISABLE_MICROSOFT_OFFICE
			if(listLetters.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a letter first.");
				return;
			}
			LetterMerge letterCur=ListForCat[listLetters.SelectedIndex];
			string templateFile=ODFileUtils.CombinePaths(PrefC.GetString(PrefName.LetterMergePath),letterCur.TemplateName);
			string dataFile=ODFileUtils.CombinePaths(PrefC.GetString(PrefName.LetterMergePath),letterCur.DataFileName);
			if(!File.Exists(templateFile)){
				MessageBox.Show(Lan.g(this,"Template file does not exist:")+"  "+templateFile);
				return;
			}
			if(!CreateDataFile(dataFile,letterCur)){
				return;
			}
			//Create an instance of Word.
			Word.Application WrdApp;
			try {
				WrdApp=LetterMerges.WordApp;
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"Error. Is MS Word installed?"),ex);
				return;
			}
			//Open a document.
			Object oName=templateFile;
			wrdDoc=WrdApp.Documents.Open(ref oName,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
			wrdDoc.Select();
			//Attach the data file.
			wrdDoc.MailMerge.OpenDataSource(dataFile,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,
				ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing,ref oMissing);
			//At this point, Word remains open with just one new document.
			if(WrdApp.WindowState==Word.WdWindowState.wdWindowStateMinimize){
				WrdApp.WindowState=Word.WdWindowState.wdWindowStateMaximize;
			}
			wrdDoc=null;
#else
			MessageBox.Show(this, "This version of Open Dental does not support Microsoft Word.");
#endif
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormLetterMerges_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.LetterMerge);
			}
		}

		

		

		

		

		

		

		

		

		


		

		

		

		

		

		

		

		


	}
}





















