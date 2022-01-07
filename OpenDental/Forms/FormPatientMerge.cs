using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPatientMerge:FormODBase {
		///<summary>For display purposes only.  Reloaded from the db when merge actually occurs.</summary>
		private Patient _patTo;
		///<summary>For display purposes only.  Reloaded from the db when merge actually occurs.</summary>
		private Patient _patFrom;

		public FormPatientMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientMerge_Load(object sender,EventArgs e) {
		}

		private void butChangePatientInto_Click(object sender,EventArgs e) {
			using FormPatientSelect fps=new FormPatientSelect();
			if(fps.ShowDialog()==DialogResult.OK){
				long selectedPatNum=fps.SelectedPatNum;//to prevent warning about marshal-by-reference
				textPatientIDInto.Text=selectedPatNum.ToString();
				_patTo=Patients.GetPat(selectedPatNum);
				textPatientNameInto.Text=_patTo.GetNameFLFormal();
				textPatToBirthdate.Text=_patTo.Birthdate.ToShortDateString();
			}
			CheckUIState();
		}

		private void butChangePatientFrom_Click(object sender,EventArgs e) {
			using FormPatientSelect fps=new FormPatientSelect();
			if(fps.ShowDialog()==DialogResult.OK) {
				long selectedPatNum=fps.SelectedPatNum;//to prevent warning about marshal-by-reference
				textPatientIDFrom.Text=selectedPatNum.ToString();
				_patFrom=Patients.GetPat(selectedPatNum);
				textPatientNameFrom.Text=_patFrom.GetNameFLFormal();
				textPatFromBirthdate.Text=_patFrom.Birthdate.ToShortDateString();
			}
			CheckUIState();
		}

		private void CheckUIState(){
			butMerge.Enabled=(_patTo!=null && _patFrom!=null);
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(_patTo.PatNum==_patFrom.PatNum) {
				MsgBox.Show(this,"Cannot merge a patient account into itself. Please select a different patient to merge from.");
				return;
			}
			//Can have multiple insurance plans, but not an insurance plan and discount plan
			bool patFromHasDiscountPlan=DiscountPlanSubs.HasDiscountPlan(_patFrom.PatNum);
			bool patToHasDiscountPlan=DiscountPlanSubs.HasDiscountPlan(_patTo.PatNum);
			if((PatPlans.GetPatPlansForPat(_patTo.PatNum).Count>0 && patFromHasDiscountPlan)
				|| (PatPlans.GetPatPlansForPat(_patFrom.PatNum).Count>0 && patToHasDiscountPlan))
			{
				MsgBox.Show(this,"Cannot merge patients that have both insurance plans and discount plans. One patient must drop their insurance plan or discount plan.");
				return;
			}
			string msgText="";
			if(patToHasDiscountPlan && patFromHasDiscountPlan) {
				msgText="Both patients have discount plans, the patient at the top will keep their original discount plan. Continue?";
				if(MessageBox.Show(Lan.g(this,msgText),"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;//The user chose not to merge
				}
			}
			if(_patFrom.FName.Trim().ToLower()!=_patTo.FName.Trim().ToLower()
				|| _patFrom.LName.Trim().ToLower()!=_patTo.LName.Trim().ToLower()
				|| _patFrom.Birthdate!=_patTo.Birthdate) 
			{//mismatch
				msgText=Lan.g(this,"The two patients do not have the same first name, last name, and birthdate.");
				if(Programs.UsingEcwTightOrFullMode()) {
					msgText+="\r\n"+Lan.g(this,"The patients must first be merged from within eCW, then immediately merged in the same order in Open Dental.  "
						+"If the patients are not merged in this manner, some information may not properly bridge between eCW and Open Dental.");
				}
				msgText+="\r\n\r\n"
					+Lan.g(this,"Into patient name")+": "+Patients.GetNameFLnoPref(_patTo.LName,_patTo.FName,"")+", "//using Patients.GetNameFLnoPref to omit MiddleI
					+Lan.g(this,"Into patient birthdate")+": "+_patTo.Birthdate.ToShortDateString()+"\r\n"
					+Lan.g(this,"From patient name")+": "+Patients.GetNameFLnoPref(_patFrom.LName,_patFrom.FName,"")+", "//using Patients.GetNameFLnoPref to omit MiddleI
					+Lan.g(this,"From patient birthdate")+": "+_patFrom.Birthdate.ToShortDateString()+"\r\n\r\n"
					+Lan.g(this,"Merge the patient on the bottom into the patient shown on the top?");
				if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;//The user chose not to merge
				}
			}
			else {//name and bd match
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Merge the patient at the bottom into the patient shown at the top?")) {
					return;//The user chose not to merge.
				}
			}
			if(_patFrom.PatNum==_patFrom.Guarantor) {
				Family fam=Patients.GetFamily(_patFrom.PatNum);
				if(fam.ListPats.Length>1) {
					msgText=Lan.g(this,"The patient you have chosen to merge from is a guarantor.  Merging this patient into another account will cause all "
						+"family members of the patient being merged from to be moved into the same family as the patient account being merged into.")+"\r\n"
						+Lan.g(this,"Do you wish to continue with the merge?");
					if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
						return;//The user chose not to merge.
					}
				}
			}
			Cursor=Cursors.WaitCursor;
			List<Task> listPatientTasks=Tasks.RefreshPatientTickets(_patFrom.PatNum);//Get this before the merge, because the merge updates Task.KeyNum.
			bool isSuccessfulMerge=false;
			try {
				isSuccessfulMerge=Patients.MergeTwoPatients(_patTo.PatNum,_patFrom.PatNum);
			}
			catch(Exception ex) {
				SecurityLogs.MakeLogEntry(Permissions.PatientMerge,_patTo.PatNum,
					"Error occurred while merging Patient: "+_patFrom.GetNameFL()+"\r\nPatNum From: "+_patFrom.PatNum+"\r\nPatNum To: "+_patTo.PatNum);
				Cursor=Cursors.Default;
				FriendlyException.Show(Lan.g(this,"Unable to fully merge patients.  Contact support."),ex);
			}
			if(isSuccessfulMerge) {
				//The patient has been successfully merged.
				#region Refresh Patient's Tasks
				List<Signalod> listSignals=new List<Signalod>();
				foreach(Task task in listPatientTasks) {
					Signalod signal=new Signalod();
					signal.IType=InvalidType.Task;
					signal.FKeyType=KeyType.Task;
					signal.FKey=task.TaskNum;
					signal.DateViewing=DateTime.MinValue;//Mimics Signalods.SetInvalid()
					listSignals.Add(signal);
				}
				Signalods.SetInvalid(InvalidType.TaskPatient,KeyType.Undefined,_patTo.PatNum);//Ensure anyone viewing Patient tab of new pat gets refreshed.
				Signalods.Insert(listSignals.ToArray());//Refreshes existing tasks in all other tabs.
				//Causes Task area and open Task Edit windows to refresh immediately.  No popups, alright to pass empty lists for listRefreshedTaskNotes and 
				//listBlockedTaskLists.
				FormOpenDental.S_HandleRefreshedTasks(listSignals,listPatientTasks.Select(x => x.TaskNum).ToList(),listPatientTasks,new List<TaskNote>()
					,new List<UserOdPref>());
				#endregion
				//Now copy the physical images from the old patient to the new if they are using an AtoZ image share.
				//This has to happen in the UI because the middle tier server might not have access to the image share.
				//If the users are storing images within the database, those images have already been taken care of in the merge method above.
				int fileCopyFailures=0;
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					#region Copy AtoZ Documents
					//Move the patient documents within the 'patFrom' A to Z folder to the 'patTo' A to Z folder.
					//We have to be careful here of documents with the same name. We have to rename such documents
					//so that no documents are overwritten/lost.
					string atoZpath=ImageStore.GetPreferredAtoZpath();
					string atozFrom=ImageStore.GetPatientFolder(_patFrom,atoZpath);
					string atozTo=ImageStore.GetPatientFolder(_patTo,atoZpath);
					string[] fromFiles=Directory.GetFiles(atozFrom);
					if(atozFrom==atozTo) {
						//Very rarely, two patients have the same image folder.  PatFrom and PatTo both have Documents that point to the same file.  Since we 
						//are about to copy the image file for PatFrom to PatTo's directory and delete the file from PatFrom's directory, we would break the 
						//file reference for PatTo's Document.  In this case, skip deleting the original file, since PatTo's Document still references it.
						Documents.MergePatientDocuments(_patFrom.PatNum,_patTo.PatNum);
					}
					else {
						foreach(string fileCur in fromFiles) {
							string fileName=Path.GetFileName(fileCur);
							string destFileName=fileName;
							string destFilePath=ODFileUtils.CombinePaths(atozTo,fileName);
							if(File.Exists(destFilePath)) {
								//The file being copied has the same name as a possibly different file within the destination a to z folder.
								//We need to copy the file under a unique file name and then make sure to update the document table to reflect
								//the change.
								destFileName=_patFrom.PatNum.ToString()+"_"+fileName;
								destFilePath=ODFileUtils.CombinePaths(atozTo,destFileName);
								while(File.Exists(destFilePath)) {
									destFileName=_patFrom.PatNum.ToString()+"_"+fileName+"_"+DateTime.Now.ToString("yyyyMMddhhmmss");
									destFilePath=ODFileUtils.CombinePaths(atozTo,destFileName);
								}
							}
							try {
								File.Copy(fileCur,destFilePath); //Will throw exception if file already exists.
							}
							catch(Exception ex) {
								ex.DoNothing();
								fileCopyFailures++;
								continue;//copy failed, increment counter and move onto the next file
							}
							//If the copy did not fail, try to delete the old file.
							//We can now safely update the document FileName and PatNum to the "to" patient.
							Documents.MergePatientDocument(_patFrom.PatNum,_patTo.PatNum,fileName,destFileName);
							try {
								File.Delete(fileCur);
							}
							catch(Exception ex) {
								ex.DoNothing();
								//If we were unable to delete the file then it is probably because someone has the document open currently.
								//Just skip deleting the file. This means that occasionally there will be an extra file in their backup
								//which is just clutter but at least the merge is guaranteed this way.
							}
						}
					}
					#endregion Copy AtoZ Documents
				}//end if AtoZFolderUsed
				else if(CloudStorage.IsCloudStorage) {
					string atozFrom=ImageStore.GetPatientFolder(_patFrom,"");
					string atozTo=ImageStore.GetPatientFolder(_patTo,"");
					if(atozFrom==atozTo) {
						//Very rarely, two patients have the same image folder.  PatFrom and PatTo both have Documents that point to the same file.  Since we 
						//are about to copy the image file for PatFrom to PatTo's directory and delete the file from PatFrom's directory, we would break the 
						//file reference for PatTo's Document.  In this case, skip deleting the original file, since PatTo's Document still references it.
						Documents.MergePatientDocuments(_patFrom.PatNum,_patTo.PatNum);
					}
					else {
						using FormProgress FormP=new FormProgress();
						FormP.DisplayText="Moving Documents...";
						FormP.NumberFormat="F";
						FormP.NumberMultiplication=1;
						FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
						FormP.TickMS=1000;
						OpenDentalCloud.Core.TaskStateMove state=CloudStorage.MoveAsync(atozFrom
							,atozTo
							,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
						if(FormP.ShowDialog()==DialogResult.Cancel) {
							state.DoCancel=true;
							fileCopyFailures=state.CountTotal-state.CountSuccess;
						}
						else {
							fileCopyFailures=state.CountFailed;
						}
					}
				}
				Cursor=Cursors.Default;
				if(fileCopyFailures>0) {
					MessageBox.Show(Lan.g(this,"Some files belonging to the from patient were not copied.")+"\r\n"
						+Lan.g(this,"Number of files not copied")+": "+fileCopyFailures);
				}
				//Make log entry here not in parent form because we can merge multiple patients at a time.
				SecurityLogs.MakeLogEntry(Permissions.PatientMerge,_patTo.PatNum,
					"Patient: "+_patFrom.GetNameFL()+"\r\nPatNum From: "+_patFrom.PatNum+"\r\nPatNum To: "+_patTo.PatNum);
				textPatientIDFrom.Text="";
				textPatientNameFrom.Text="";
				textPatFromBirthdate.Text="";
				//This will cause CheckUIState() to disabled the merge button until the user selects a new _patFrom.
				_patFrom=null;
				CheckUIState();
				MsgBox.Show(this,"Patients merged successfully.");
			}//end MergeTwoPatients
			Cursor=Cursors.Default;
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

	}
}