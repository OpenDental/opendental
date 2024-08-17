using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDental.Thinfinity;

namespace OpenDental {
	public partial class FormMedLabEdit:FormODBase {
		///<summary>Passed in, or set here if selecting a new patient for medlab. Can be null.</summary>
		public Patient PatCur;
		///<summary>List of all MedLabs linked to this patient and specimen. Passed in from calling class, or set here if newly attached to patient.</summary>
		public List<MedLab> ListMedLabs;
		///<summary>Aggregated final results from all of the med lab orders in ListMedLabs.</summary>
		private List<MedLabResult> _listMedlabResults;
		///<summary>Usually the first MedLab in ListMedLabs. Used for convenience instead of continuously referencing ListMedLabs[0]. 
		///Since all MedLabs in ListMedLabs have the same SpecimenID and SpecimenIDFiller, it is safe to assume all MedLab objects have
		///the same value for some of the fields and we will just pull from the first MedLab in the list.</summary>
		private MedLab _medLab;
		private List<MedLabFacility> _listMedlabFacilities;

		public FormMedLabEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMedLabEdit_Load(object sender,EventArgs e) {
			_medLab=ListMedLabs[0];
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				butShowHL7.Visible=false;//messages are not archived if storing images in the database
				labelShowHL7.Visible=false;
			}
			//list of MedLabFacilityNums used by all results, the position in the list will be the facility id
			//also fills the classwide variable _listResults used in FillGridResults
			_listMedlabFacilities=MedLabFacilities.GetFacilityList(ListMedLabs,out _listMedlabResults);
			SetFields();
		}

		///<summary>Used to set all of the fields on the form.
		///Called on load and when a HL7 message is reprocessed to refresh the fields with the new object information.</summary>
		private void SetFields() {
			#region Patient and Patient Address Group Box
			RefreshPatientData();
			textSpecimenNumber.Text=_medLab.PatIDLab;
			textFasting.Text=_medLab.PatFasting.ToString();
			if(_medLab.PatFasting==YN.Unknown) {
				textFasting.Text="";
			}
			#endregion Patient and Patient Address Group Box
			textDateTCollect.Text=_medLab.DateTimeCollected.ToString("MM/dd/yyyy hh:mm tt");
			textDateEntered.Text=_medLab.DateTimeEntered.ToShortDateString();
			textDateTReport.Text=_medLab.DateTimeReported.ToString("MM/dd/yyyy hh:mm tt");
			textTotVol.Text=_medLab.TotalVolume;
			#region Odering Physician Group Box
			textPhysicianName.Text=_medLab.OrderingProvLName;
			if(_medLab.OrderingProvLName!="") {
				textPhysicianName.Text+=", ";
			}
			textPhysicianName.Text+=_medLab.OrderingProvFName;
			textPhysicianNPI.Text=_medLab.OrderingProvNPI;
			textPhysicianID.Text=_medLab.OrderingProvLocalID;
			#endregion Odering Physician Group Box
			string patNote="";
			string labNote="";
			string clinicalInfo="";
			List<string> listTestIds=new List<string>();
			string testsOrdered="";
			for(int i=0;i<ListMedLabs.Count;i++) {
				//Each NotePat may be repeated and we do not want to display multiple copies of the same note. Only add unique notes to PatNote.
				if(!Regex.IsMatch(patNote,Regex.Escape(ListMedLabs[i].NotePat),RegexOptions.IgnoreCase)) {
					if(patNote!="") {
						patNote+="\r\n";
					}
					patNote+=ListMedLabs[i].NotePat;
				}
				//Each NoteLab may be repeated and we do not want to display multiple copies of the same note. Only add unique notes to NoteLab.
				if(!Regex.IsMatch(labNote,Regex.Escape(ListMedLabs[i].NoteLab),RegexOptions.IgnoreCase)) {
					if(labNote!="") {
						labNote+="\r\n";
					}
					labNote+=ListMedLabs[i].NoteLab;
				}
				//Each Clinical Info may be repeated and we do not want to display multiple copies of the same note. Only add unique information to Clinical Info.
				if(!Regex.IsMatch(clinicalInfo,Regex.Escape(ListMedLabs[i].ClinicalInfo),RegexOptions.IgnoreCase)) {
					if(clinicalInfo!="") {
						clinicalInfo+="\r\n";
					}
					clinicalInfo+=ListMedLabs[i].ClinicalInfo;
				}
				//Build list of ordered tests
				if(!listTestIds.Contains(ListMedLabs[i].ObsTestID)
					&& ListMedLabs[i].ActionCode!=ResultAction.G)//"G" indicates a reflex test, not a test originally ordered, so skip these
				{
					listTestIds.Add(ListMedLabs[i].ObsTestID);
					if(testsOrdered!="") {
						testsOrdered+="\r\n";
					}
					testsOrdered+=ListMedLabs[i].ObsTestID+" - "+ListMedLabs[i].ObsTestDescript;
				}
			}
			if(patNote!="" && labNote!="") {
				patNote+="\r\n";
			}
			//concatenate all notes together for display in the same textbox
			patNote+=labNote;
			textGenComments.Text=patNote;
			textAddlInfo.Text=clinicalInfo;
			textTestsOrd.Text=testsOrdered;
			FillGridResults();
			FillGridFacilities();
		}

		///<summary>Formatting for fields in this grid designed to emulate as accurately as possible the sample provided by LabCorp.</summary>
		private void FillGridResults() {
			gridResults.BeginUpdate();
			gridResults.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Test / Result",500);
			gridResults.Columns.Add(col);
			col=new GridColumn("Flag",115);
			gridResults.Columns.Add(col);
			col=new GridColumn("Units",110);
			gridResults.Columns.Add(col);
			col=new GridColumn("Reference Interval",145);
			gridResults.Columns.Add(col);
			col=new GridColumn("Lab",60);
			gridResults.Columns.Add(col);
			gridResults.ListGridRows.Clear();
			GridRow row;
			string obsDescriptPrev="";
			for(int i=0;i<_listMedlabResults.Count;i++) {
				//LabCorp requested that these non-performance results not be displayed on the report
				if((_listMedlabResults[i].ResultStatus==ResultStatus.F || _listMedlabResults[i].ResultStatus==ResultStatus.X)
					&& _listMedlabResults[i].ObsValue==""
					&& _listMedlabResults[i].Note=="")
				{
					continue;
				}
				string obsDescript="";
				MedLab medLab=MedLabs.GetOne(_listMedlabResults[i].MedLabNum);
				//Only dipslay full medLabCur.ObsTestDescript if different than previous Descript.
				if(i==0 || _listMedlabResults[i].MedLabNum!=_listMedlabResults[i-1].MedLabNum) {
					if(medLab.ActionCode!=ResultAction.G) {
						if(obsDescriptPrev==medLab.ObsTestDescript) {
							obsDescript=".";
						}
						else {
							obsDescript=medLab.ObsTestDescript;
							obsDescriptPrev=obsDescript;
						}
					}
				}
				//Set tabs using spaces and spaces2, can be changed further down in the code
				string spaces="    ";
				string spaces2="        ";
				string obsVal="";
				int padR=78;
				string newLine="";
				if(obsDescript!="") {
					if(obsDescript==_listMedlabResults[i].ObsText) {
						spaces="";
						spaces2="    ";
						padR=80;
					}
					else {
						obsVal+=obsDescript+"\r\n";
						newLine+="\r\n";
					}
				}
				if(_listMedlabResults[i].ObsValue=="Test Not Performed") {
					obsVal+=spaces+_listMedlabResults[i].ObsText;
				}
				else if(_listMedlabResults[i].ObsText=="."
					|| _listMedlabResults[i].ObsValue.Contains(":")
					|| _listMedlabResults[i].ObsValue.Length>20
					|| medLab.ActionCode==ResultAction.G)
				{
					obsVal+=spaces+_listMedlabResults[i].ObsText+"\r\n"+spaces2+_listMedlabResults[i].ObsValue.Replace("\r\n","\r\n"+spaces2);
					newLine+="\r\n";
				}
				else {
					obsVal+=spaces+_listMedlabResults[i].ObsText.PadRight(padR,' ')+_listMedlabResults[i].ObsValue;
				}
				if(_listMedlabResults[i].Note!="") {
					obsVal+="\r\n"+spaces2+_listMedlabResults[i].Note.Replace("\r\n","\r\n"+spaces2);
				}
				row=new GridRow();
				row.Cells.Add(obsVal);
				row.Cells.Add(newLine+MedLabResults.GetAbnormalFlagDescript(_listMedlabResults[i].AbnormalFlag));
				row.Cells.Add(newLine+_listMedlabResults[i].ObsUnits);
				row.Cells.Add(newLine+_listMedlabResults[i].ReferenceRange);
				row.Cells.Add(newLine+_listMedlabResults[i].FacilityID);
				row.Tag=_listMedlabResults[i];
				gridResults.ListGridRows.Add(row);
			}
			gridResults.EndUpdate();
		}

		private void FillGridFacilities() {
			gridFacilities.BeginUpdate();
			gridFacilities.Columns.Clear();
			GridColumn col;
			col=new GridColumn("ID",40);//Facility ID from the MedLabResult
			gridFacilities.Columns.Add(col);
			col=new GridColumn("Name",200);
			gridFacilities.Columns.Add(col);
			col=new GridColumn("Address",165);
			gridFacilities.Columns.Add(col);
			col=new GridColumn("City",90);
			gridFacilities.Columns.Add(col);
			col=new GridColumn("State",35);
			gridFacilities.Columns.Add(col);
			col=new GridColumn("Zip",70);
			gridFacilities.Columns.Add(col);
			col=new GridColumn("Phone",130);
			gridFacilities.Columns.Add(col);
			col=new GridColumn("Director",200);//FName LName, Title
			gridFacilities.Columns.Add(col);
			gridFacilities.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listMedlabFacilities.Count;i++) {
				MedLabFacility medLabFacility=_listMedlabFacilities[i];
				row=new GridRow();
				row.Cells.Add((i+1).ToString().PadLeft(2,'0'));//Actually more of a local renumbering of labs referenced by each Lab Result Row.
				row.Cells.Add(medLabFacility.FacilityName);
				row.Cells.Add(medLabFacility.Address);
				row.Cells.Add(medLabFacility.City);
				row.Cells.Add(medLabFacility.State);
				row.Cells.Add(medLabFacility.Zip);
				row.Cells.Add(medLabFacility.Phone);
				string directorName=medLabFacility.DirectorFName;
				if(medLabFacility.DirectorFName!="" && medLabFacility.DirectorLName!="") {
					directorName+=" ";
				}
				directorName+=medLabFacility.DirectorLName;
				if(directorName!="" && medLabFacility.DirectorTitle!="") {
					directorName+=", "+medLabFacility.DirectorTitle;
				}
				row.Cells.Add(directorName);//could be blank
				gridFacilities.ListGridRows.Add(row);
			}
			gridFacilities.EndUpdate();
		}

		///<summary>Shows result history. Example: Show that a Corrected result had a Final and a Preliminary result in the past.</summary>
		private void gridResults_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMedLabResultHist formMedLabResultHist=new FormMedLabResultHist();
			formMedLabResultHist.PatientCur=PatCur;
			formMedLabResultHist.MedLabResultCur=(MedLabResult)gridResults.ListGridRows[e.Row].Tag;
			formMedLabResultHist.ShowDialog();
		}

		///<summary>Allows lab to be reassociated with a new patient.</summary>
		private void butPatSelect_Click(object sender,EventArgs e) {
			using FormMedLabPatSelect formMedLabPatSelect=new FormMedLabPatSelect();
			formMedLabPatSelect.PatientCur=PatCur;
			formMedLabPatSelect.ListMedLabs=ListMedLabs;
			formMedLabPatSelect.ShowDialog();
			if(formMedLabPatSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			PatCur=formMedLabPatSelect.PatientCur;
			RefreshPatientData();
		}

		private void RefreshPatientData() {
			if(PatCur==null) {//the MedLab object(s) are not attached to a patient, clear all pat fields
				textPatID.Text="";
				textPatLName.Text="";
				textPatFName.Text="";
				textPatMiddleI.Text="";
				textPatSSN.Text="";
				textBirthdate.Text="";
				textGender.Text="";
				return;
			}
			textPatID.Text=PatCur.PatNum.ToString();
			textPatLName.Text=PatCur.LName;
			textPatFName.Text=PatCur.FName;
			textPatMiddleI.Text=PatCur.MiddleI;
			textPatSSN.Text="****-**-"+PatCur.SSN.PadLeft(4,' ').Substring(PatCur.SSN.PadLeft(4,' ').Length-4,4);//mask all but the last 4 digits. Ex: ****-**-1234
			textBirthdate.Text=PatCur.Birthdate.ToShortDateString();
			if(PatCur.Birthdate.Year < 1880) {
				textBirthdate.Text="";
			}
			textGender.Text=PatCur.Gender.ToString();
		}

		private void butProvSelect_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formProviderPick.ProvNumSelected!=_medLab.ProvNum) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Update all lab tests and results for this specimen with the selected ordering provider?")) {
					return;
				}
			}
			Provider provider=Providers.GetProv(formProviderPick.ProvNumSelected);
			for(int i=0;i<ListMedLabs.Count;i++) {
				ListMedLabs[i].OrderingProvLName=provider.LName;
				ListMedLabs[i].OrderingProvFName=provider.FName;
				ListMedLabs[i].OrderingProvNPI=provider.NationalProvID;
				ListMedLabs[i].OrderingProvLocalID=provider.ProvNum.ToString();
				ListMedLabs[i].ProvNum=provider.ProvNum;
				MedLabs.Update(ListMedLabs[i]);
			}
			string provName=provider.LName;
			if(provName!="" && provider.FName!="") {
				provName+=", ";
			}
			provName+=provider.FName;
			textPhysicianName.Text=provName;
			textPhysicianNPI.Text=provider.NationalProvID;
			textPhysicianID.Text=provider.ProvNum.ToString();
		}

		///<summary>Uses sheet framework to generate a PDF file, save it to patient's image folder, and attempt to launch file with defualt reader.
		///If using ImagesStoredInDB it will not launch PDF. If no valid patient is selected you cannot perform this action.</summary>
		private void butPDF_Click(object sender,EventArgs e) {
			if(PatCur==null) {//not attached to a patient when form loaded and they haven't selected a patient to attach to yet
				MsgBox.Show(this,"The Medical Lab must be attached to a patient before the PDF can be saved.");
				return;
			}
			if(PatCur.PatNum>0 && _medLab.PatNum!=PatCur.PatNum) {//save the current patient attached to the MedLab if it has been changed
				MoveLabsAndImagesHelper();
			}
			Cursor=Cursors.WaitCursor;
			SheetDef sheetDef=SheetUtil.GetMedLabResultsSheetDef();
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,_medLab.PatNum);
			SheetFiller.FillFields(sheet,null,null,_medLab);
			//create the file in the temp folder location, then import so it works when storing images in the db
			string tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),_medLab.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,tempPath,null,_medLab);
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled(true);
			long category=hl7Def.LabResultImageCat;
			if(category==0) {
				category=Defs.GetFirstForCategory(DefCat.ImageCats,true).DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			OpenDentBusiness.Document document=null;
			try {
				document=ImageStore.Import(tempPath,category,Patients.GetPat(_medLab.PatNum));
			}
			catch(Exception ex) {
				ex.DoNothing();
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Error saving document.");
				return;
			}
			finally {
				//Delete the temp file since we don't need it anymore.
				try {
					File.Delete(tempPath);
				}
				catch {
					//Do nothing.  This file will likely get cleaned up later.
				}
			}
			document.Description=Lan.g(this,"MedLab Result");
			document.DateCreated=DateTime.Now;
			Documents.Update(document);
			string filePathAndName="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string patFolder=ImageStore.GetPatientFolder(Patients.GetPat(_medLab.PatNum),ImageStore.GetPreferredAtoZpath());
				filePathAndName=ODFileUtils.CombinePaths(patFolder,document.FileName);
			}
			else if(CloudStorage.IsCloudStorage) {
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(
					ImageStore.GetPatientFolder(Patients.GetPat(_medLab.PatNum),ImageStore.GetPreferredAtoZpath())
					,document.FileName
					,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				if(formProgress.ShowDialog()==DialogResult.Cancel) {
					state.DoCancel=true;
					return;
				}
				filePathAndName=PrefC.GetRandomTempFile(Path.GetExtension(document.FileName));
				File.WriteAllBytes(filePathAndName,state.FileContent);
			}
			Cursor=Cursors.Default;
			if(filePathAndName!="") {
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(filePathAndName);
				}
				else {
					Process.Start(filePathAndName);
				}
			}
			SecurityLogs.MakeLogEntry(Permissions.SheetEdit,sheet.PatNum,sheet.Description+" from "+sheet.DateTimeSheet.ToShortDateString()+" pdf was created");
			DialogResult=DialogResult.OK;
		}

		private void butShowHL7_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			List<string[]> listFileNamesDateMod=new List<string[]>();
			for(int i=0;i<ListMedLabs.Count;i++) {
				string filePath=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),ListMedLabs[i].FileName);
				bool isFileAdded=false;
				for(int j=0;j<listFileNamesDateMod.Count;j++) {
					if(listFileNamesDateMod[j][0]==filePath) {
						isFileAdded=true;
						break;
					}
				}
				if(isFileAdded) {
					continue;
				}
				string dateModified=DateTime.MinValue.ToString();
				try {
					dateModified=File.GetLastWriteTime(filePath).ToString();
				}
				catch(Exception ex) {
					ex.DoNothing();
					//dateModified will be min value, do nothing?
				}
				listFileNamesDateMod.Add(new string[] { filePath,dateModified });
			}
			using FormMedLabHL7MsgText formMedLabHL7MsgText=new FormMedLabHL7MsgText();
			formMedLabHL7MsgText.ListFileNamesDatesMod=listFileNamesDateMod;
			Cursor=Cursors.Default;
			formMedLabHL7MsgText.ShowDialog();
		}

		///<summary>Moves all MedLab objects and any embedded PDFs tied to the MedLabResults to the PatCur.
		///If the MedLab objects were not originally attached to a patient, any embedded PDFs will be in the image folder in a directory called
		///"MedLabEmbeddedFiles" and will be moved to PatCur's image folder.
		///If PatCur is null or the MedLabs are already attached to PatCur, does nothing.</summary>
		private void MoveLabsAndImagesHelper() {
			//if they have selected the same patient, nothing to do
			if(PatCur==null || PatCur.PatNum==_medLab.PatNum) {
				return;
			}
			//if the MedLab object(s) were attached to a patient and they are being moved to another patient, move the associated documents
			Patient patOld=Patients.GetPat(_medLab.PatNum);
			MedLabs.UpdateAllPatNums(ListMedLabs.Select(x => x.MedLabNum).ToList(),PatCur.PatNum);
			string atozFrom="";
			string atozTo="";
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string atozPath=ImageStore.GetPreferredAtoZpath();
				//if patOld is null, the file was placed into the image folder in a directory named MedLabEmbeddedFiles, not a patient's image folder
				if(patOld==null) {
					atozFrom=ODFileUtils.CombinePaths(atozPath,"MedLabEmbeddedFiles");
				}
				else {
					atozFrom=ImageStore.GetPatientFolder(patOld,atozPath);
				}
				atozTo=ImageStore.GetPatientFolder(PatCur,atozPath);
			}
			else if(CloudStorage.IsCloudStorage) {
				atozFrom=ODFileUtils.CombinePaths(ImageStore.GetPreferredAtoZpath(),"MedLabEmbeddedFiles",'/');
				atozTo=ImageStore.GetPatientFolder(PatCur,"");
			}
			//get list of all DocNums of files referenced by MedLabResults which were embedded in the MedLab HL7 message as base64 text
			//in order to move the file (if not storing images in db) and assign (or reassign) the FileName
			List<long> listDocNums=ListMedLabs
				.SelectMany(x => x.ListMedLabResults
					.Select(y => y.DocNum)
					.Where(y => y>0))
					.Distinct().ToList();
			List<Document> listDocuments=Documents.GetByNums(listDocNums);
			int fileMoveFailures=0;
			for(int i=0;i<listDocuments.Count;i++) {
				Document document=listDocuments[i];
				string destFileName=Documents.GetUniqueFileNameForPatient(PatCur,document.DocNum,Path.GetExtension(document.FileName));
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					string fromFilePath=ODFileUtils.CombinePaths(atozFrom,document.FileName);
					if(!File.Exists(fromFilePath)) {
						//the DocNum in the MedLabResults table is pointing to a file that either doesn't exist or is not accessible, can't move/copy it
						fileMoveFailures++;
						continue;
					}
					string destFilePath=ODFileUtils.CombinePaths(atozTo,destFileName);
					if(File.Exists(destFilePath)) {//should never happen, since we already got a unique file name, but just in case
						//The file being copied has the same name as a file that exists in the destination folder, use a unique file name and update document table
						destFileName=patOld.PatNum.ToString()+"_"+document.FileName;//try to prepend patient's PatNum to the original file name
						destFilePath=ODFileUtils.CombinePaths(atozTo,destFileName);
						while(File.Exists(destFilePath)) {
							//if still not unique, try appending date/time to seconds precision until the file name is unique
							destFileName=patOld.PatNum.ToString()+"_"+Path.GetFileNameWithoutExtension(document.FileName)
							+"_"+DateTime.Now.ToString("yyyyMMddhhmmss")+Path.GetExtension(document.FileName);
							destFilePath=ODFileUtils.CombinePaths(atozTo,destFileName);
						}
					}
					try {
						File.Copy(fromFilePath,destFilePath);
					}
					catch(Exception ex) {
						ex.DoNothing();
						fileMoveFailures++;
						continue;
					}
					//try to delete the original file
					try {
						File.Delete(fromFilePath);
					}
					catch(Exception ex) {
						ex.DoNothing();
						//If we cannot delete the file, could be a permission issue or someone has the file open currently
						//Just skip deleting the file, which means there could be an image in the old pat's folder that may need to be deleted manually
						fileMoveFailures++;
					}
				}
				else if(CloudStorage.IsCloudStorage) {
					//move files around in the cloud
					using FormProgress formProgress=new FormProgress(false);
					formProgress.DisplayText="Uploading...";
					formProgress.NumberFormat="F";
					formProgress.NumberMultiplication=1;
					formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					formProgress.TickMS=1000;
					OpenDentalCloud.Core.TaskStateMove state=CloudStorage.MoveAsync(atozFrom
						,atozTo
						,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
					formProgress.ShowDialog();//Don't allow users to cancel from here due to the limitations of the current feature for figuring out which files were moved successfully.
				}
				//if we get here the file was copied successfully or not storing images in the database, so update the document row
				//Safe to update the document FileName and PatNum to PatCur and new file name
				document.PatNum=PatCur.PatNum;
				document.FileName=destFileName;
				Documents.Update(document);
			}
			ListMedLabs.ForEach(x => x.PatNum=PatCur.PatNum);//update local list, done after moving files
			_medLab=ListMedLabs[0];
			if(fileMoveFailures>0) {//will never be > 0 if storing images in the db
				MessageBox.Show(Lan.g(this,"Some files attached to the MedLab objects could not be moved.")+"\r\n"
					+Lan.g(this,"This could be due to a missing file, a file being open, or a permission issue on the file which is preventing the move.")+"\r\n"
					+Lan.g(this,"The file(s) will have to be moved manually from the Image module.")+"\r\n"
					+Lan.g(this,"Number of files not moved")+": "+fileMoveFailures.ToString());
			}
		}

		///<summary>This will delete all MedLab objects for the specimen referenced by _medLabCur and all MedLabResult, MedLabSpecimen,
		///and MedLabFacAttach objects, as well as any documents referenced by the results.  The original HL7 message will remain in the image folder,
		///but this MedLab will not point to it.  We won't remove the HL7 message since there may be other MedLab rows that point to it.</summary>
		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will delete all orders, results, and specimens for this MedLab as well as "
				+"any associated pdf files."))
			{
				return;
			}
			int failedCount=MedLabs.DeleteLabsAndResults(_medLab);
			if(failedCount>0) {
				MessageBox.Show(this,Lans.g(this,"Some images referenced by the MedLabResults could not be deleted and will have to be removed manually.")
					+"\r\n"+Lans.g(this,"Number failed")+": "+failedCount);
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(PatCur!=null && PatCur.PatNum>0 && _medLab.PatNum!=PatCur.PatNum) {
				MoveLabsAndImagesHelper();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}