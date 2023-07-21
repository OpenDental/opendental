using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormImageCatMerge:FormODBase {
		private long _defNumInto;
		private long _defNumFrom;

		public FormImageCatMerge() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void buttonChangeInto_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.ImageCats);
			if(formDefinitionPicker.ShowDialog()==DialogResult.OK && formDefinitionPicker.ListDefsSelected.Count>0) {
				textBoxInto.Text=formDefinitionPicker.ListDefsSelected.FirstOrDefault().ItemName;
				_defNumInto=formDefinitionPicker.ListDefsSelected.FirstOrDefault().DefNum;
				CheckUIState();
			}
		}

		private void buttonChangeFrom_Click(object sender,EventArgs e) {
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.ImageCats);
			if(formDefinitionPicker.ShowDialog()==DialogResult.OK && formDefinitionPicker.ListDefsSelected.Count>0) {
				textBoxFrom.Text=formDefinitionPicker.ListDefsSelected.FirstOrDefault().ItemName;
				_defNumFrom=formDefinitionPicker.ListDefsSelected.FirstOrDefault().DefNum;
				CheckUIState();
			}
		}
	
		//Double check the state and status of the form.
		private void CheckUIState() {
			butMerge.Enabled=(textBoxFrom.Text.Trim()!="" && textBoxInto.Text.Trim()!="");
		}

		private bool IsMergeAllowed() {
			if(PrefC.GetLong(PrefName.TaskAttachmentCategory)!=_defNumFrom) { 
				return true;
			}
			//If image category that is being merged from, and will be hidden, is being used to store task attachments.
			return MsgBox.Show(this,MsgBoxButtons.YesNo,"The 'From' image category is currently being used to store task attachments. " +
				"Images will continue to be stored in this image category but the category will be marked hidden by this tool. " +
				"Continue Anyway?");
		}

		private void butMerge_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DefEdit)) {
				return;
			}
			if(_defNumInto==_defNumFrom) { 
				MsgBox.Show(this,"Cannot merge the same Image Category. Please update either the merge Into field or the merge From field.");
				return;
			}
			if(!IsMergeAllowed()) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure? The results are permanent and cannot be undone.")) {
				return;
			}
			try {
				Defs.MergeImageCatDefNums(_defNumFrom,_defNumInto);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lans.g(this,"Image Categories failed to merge."),ex);
				return;
			}
			DefL.HideDef(Defs.GetDef(DefCat.ImageCats,_defNumFrom));
			DataValid.SetInvalid(InvalidType.Defs);
			MsgBox.Show(this,"Image Categories merged successfully.");
			string logText=Lan.g(this,"Image Category Merge from")
				+" "+Defs.GetName(DefCat.ImageCats,_defNumFrom)+" "+Lan.g(this,"to")+" "+Defs.GetName(DefCat.ImageCats,_defNumInto);
			//Make log entry here.
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,logText);
			textBoxFrom.Clear();
			textBoxInto.Clear();
			CheckUIState();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}
	}
}