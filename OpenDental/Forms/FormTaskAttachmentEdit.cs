using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTaskAttachmentEdit:FormODBase {
		///<summary>Stores the task that the attachment belongs to or will belong to.</summary>
		private Task _task;
		///<summary>Stores the document if one current is linked to the attachment or one is imported to the attachment.</summary>
		private Document _document;
		///<summary>This attachment object must be set before the constructor is called. If adding a new attachment, set IsNew flag to true. If editing existing attachments, just 
		///set this field to the attachment that will be edited.</summary>
		public TaskAttachment TaskAttachmentCur;

		public FormTaskAttachmentEdit(Task task) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_task=task;
		}

		private void FormTaskAttachmentEdit_Load(object sender,EventArgs e) {
			if(!CanEditAttachments()) {
				DisableAllExcept(textValue,butCancel);
				textValue.ReadOnly=true;
			}
			if(TaskAttachmentCur.DocNum==0) {
				butViewDoc.Enabled=false;
			}
			else {
				butViewDoc.Enabled=true;
			}
			FillFields();
		}

		private void FillFields() {
			if(!TaskAttachmentCur.IsNew) {
				if(TaskAttachmentCur.DocNum>0) {
					_document=Documents.GetByNum(TaskAttachmentCur.DocNum);
					if(_document!=null) { 
						textDocNum.Text=_document.DocNum.ToString();
					}
					else {
						MsgBox.Show(this,"Document could not be found.");
					}
				}
			}
			textTaskNum.Text=_task.TaskNum.ToString();
			textDescription.Text=TaskAttachmentCur.Description;
			textValue.Text=TaskAttachmentCur.TextValue;
		}

		private void butImport_Click(object sender,EventArgs e) {
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=false;
			if(!TaskAttachmentCur.IsNew && TaskAttachmentCur.DocNum>0) { 
				openFileDialog.FileName=Documents.GetPath(_document.DocNum);
			}
			if(openFileDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			if(TryImportDoc(openFileDialog.FileName)) {
				textDocNum.Text=TaskAttachmentCur.DocNum.ToString();
				butViewDoc.Enabled=true;
				MsgBox.Show(this,"Done");				
			}
		}

		private bool TryImportDoc(string filePath) {
			long imageCategory=PrefC.GetLong(PrefName.TaskAttachmentCategory);
			if(imageCategory==0) {
				MsgBox.Show(this,"No image category to store attachments has been set in Setup->Tasks.");
				return false;
			}
			try { 
				_document=ImageStore.Import(filePath,imageCategory,Patients.GetPat(_task.KeyNum));
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g("FormTaskAttachmentEdit","An error occured while importing the document."),ex);
				return false;
			}
			TaskAttachmentCur.DocNum=_document.DocNum;
			return true;
		}

		private void butViewDoc_Click(object sender,EventArgs e) {
			if(TaskAttachmentCur.DocNum==0) {
				MsgBox.Show(this,"Document could not be found.");
				return;
			}
			if(Documents.DocExists(TaskAttachmentCur.DocNum)) {
				Documents.OpenDoc(TaskAttachmentCur.DocNum);
				return;
			}
			MsgBox.Show(this,"Document could not be found.");
		}

		private bool CanEditAttachments() {
			if(Tasks.IsTaskDeleted(_task.TaskNum)) {
				return false;
			}
			if(!_task.IsNew && !TaskAttachmentCur.IsNew) {
				if(!Security.IsAuthorized(Permissions.TaskEdit,suppressMessage:true)) {
					return false;
				}
			}
			if(PrefC.GetLong(PrefName.TaskAttachmentCategory)==0) {
				return false;
			}
			if(_task.ObjectType!=TaskObjectType.Patient) {
				return false;
			}
			if(_task.ObjectType==TaskObjectType.Patient &&_task.KeyNum==0) {
				return false;
			}
			return true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(Tasks.IsTaskDeleted(_task.TaskNum)) {
				MsgBox.Show(this,"The task for this attachment was deleted.");
				return;
			}
			if(TaskAttachmentCur==null) {
				MsgBox.Show(this,"Could not delete attachment.");
				return;
			}
			if(!TaskAttachmentCur.IsNew) { 
				StringBuilder stringBuilder=new StringBuilder();
				stringBuilder.AppendLine("Are you sure you would like to delete this attachment?");
				if(TaskAttachmentCur.DocNum>0) {
					stringBuilder.AppendLine("If yes, the document linked to this attachment will need to be deleted via the imaging module, if desired.");
				}
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,stringBuilder.ToString())) {
					TaskAttachments.Delete(TaskAttachmentCur.TaskAttachmentNum);
					DialogResult=DialogResult.OK;
					return;			
				}
				return;//User decided they did not want to delete the attachment, just return
			}
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(Tasks.IsTaskDeleted(_task.TaskNum)) {
				MsgBox.Show(this,"The task for this attachment was deleted.");
				return;
			}
			if(string.IsNullOrWhiteSpace(textDescription.Text)) {
				MsgBox.Show(this,"Description field cannot be empty.");
				return;
			}
			if(TaskAttachmentCur.DocNum==0 && string.IsNullOrWhiteSpace(textValue.Text)) {
				MsgBox.Show(this,"Text field cannot be empty if no document is linked.");
				return;
			}
			if(TaskAttachmentCur.DocNum>0) { 
				_document.Description=textDescription.Text;
				Documents.Update(_document);
			}
			TaskAttachmentCur.Description=textDescription.Text;
			TaskAttachmentCur.TextValue=textValue.Text;
			if(!TaskAttachmentCur.IsNew) { 
				TaskAttachments.Update(TaskAttachmentCur);
				DialogResult=DialogResult.OK;
				return;
			}
			TaskAttachmentCur.TaskNum=_task.TaskNum;
			TaskAttachments.Insert(TaskAttachmentCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}