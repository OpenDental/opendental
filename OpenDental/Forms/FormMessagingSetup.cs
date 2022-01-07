using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormMessagingSetup : FormODBase {

		///<summary></summary>
		public FormMessagingSetup()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMessagingSetup_Load(object sender,EventArgs e) {
			FillLists();
		}

		private void FillLists(){
			SigElementDefs.RefreshCache();
			listToFrom.Items.Clear();
			listToFrom.Items.AddList(SigElementDefs.GetSubList(SignalElementType.User),x => x.SigText);
			listExtras.Items.Clear();
			listExtras.Items.AddList(SigElementDefs.GetSubList(SignalElementType.Extra),x => x.SigText);
			listMessages.Items.Clear();
			listMessages.Items.AddList(SigElementDefs.GetSubList(SignalElementType.Message),x => x.SigText);
		}

		private void listToFrom_Click(object sender,EventArgs e) {
			listExtras.SelectedIndex=-1;
			listMessages.SelectedIndex=-1;
		}

		private void listExtras_Click(object sender,EventArgs e) {
			listToFrom.SelectedIndex=-1;
			listMessages.SelectedIndex=-1;
		}

		private void listMessages_Click(object sender,EventArgs e) {
			listToFrom.SelectedIndex=-1;
			listExtras.SelectedIndex=-1;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormSigElementDefEdit FormS=new FormSigElementDefEdit();
			FormS.ElementCur=new SigElementDef();
			FormS.ElementCur.LightColor=Color.White;
			//default is user
			if(listExtras.SelectedIndex!=-1){
				FormS.ElementCur.SigElementType=SignalElementType.Extra;
			}
			if(listMessages.SelectedIndex!=-1) {
				FormS.ElementCur.SigElementType=SignalElementType.Message;
			}
			FormS.IsNew=true;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			//set the order
			SigElementDef element=FormS.ElementCur.Copy();
			if(element.SigElementType==SignalElementType.User){
				element.ItemOrder=listToFrom.Items.Count;
				SigElementDefs.Update(element);
			}
			else if(element.SigElementType==SignalElementType.Extra) {
				element.ItemOrder=listExtras.Items.Count;
				SigElementDefs.Update(element);
			}
			else if(element.SigElementType==SignalElementType.Message) {
				element.ItemOrder=listMessages.Items.Count;
				SigElementDefs.Update(element);
			}
			FillLists();
			//Select the item
			for(int i=0;i<listToFrom.Items.Count;i++){
				if(((SigElementDef)listToFrom.Items.GetObjectAt(i)).SigElementDefNum==element.SigElementDefNum){
					listToFrom.SelectedIndex=i;
				}
			}
			for(int i=0;i<listExtras.Items.Count;i++) {
				if(((SigElementDef)listExtras.Items.GetObjectAt(i)).SigElementDefNum==element.SigElementDefNum) {
					listExtras.SelectedIndex=i;
				}
			}
			for(int i=0;i<listMessages.Items.Count;i++) {
				if(((SigElementDef)listMessages.Items.GetObjectAt(i)).SigElementDefNum==element.SigElementDefNum) {
					listMessages.SelectedIndex=i;
				}
			}
		}

		private void listToFrom_DoubleClick(object sender,EventArgs e) {
			if(listToFrom.SelectedIndex==-1){
				return;
			}
			using FormSigElementDefEdit FormS=new FormSigElementDefEdit();
			FormS.ElementCur=listToFrom.GetSelected<SigElementDef>();
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			FillLists();
			//not possible to change ItemOrder here.
		}

		private void listExtras_DoubleClick(object sender,EventArgs e) {
			if(listExtras.SelectedIndex==-1) {
				return;
			}
			using FormSigElementDefEdit FormS=new FormSigElementDefEdit();
			FormS.ElementCur=listExtras.GetSelected<SigElementDef>();
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			FillLists();
		}

		private void listMessages_DoubleClick(object sender,EventArgs e) {
			if(listMessages.SelectedIndex==-1) {
				return;
			}
			using FormSigElementDefEdit FormS=new FormSigElementDefEdit();
			FormS.ElementCur=listMessages.GetSelected<SigElementDef>();
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			FillLists();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(listToFrom.SelectedIndex==-1
				&& listExtras.SelectedIndex==-1
				&& listMessages.SelectedIndex==-1)
			{
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			int selected;
			if(listToFrom.SelectedIndex!=-1){
				selected=listToFrom.SelectedIndex;
				if(selected==0) {
					return;
				}
				SigElementDefs.MoveUp(selected,listToFrom.Items.GetAll<SigElementDef>().ToArray());
				FillLists();
				listToFrom.SelectedIndex=selected-1;
			}
			else if(listExtras.SelectedIndex!=-1) {
				selected=listExtras.SelectedIndex;
				if(selected==0) {
					return;
				}
				SigElementDefs.MoveUp(selected,listExtras.Items.GetAll<SigElementDef>().ToArray());
				FillLists();
				listExtras.SelectedIndex=selected-1;
			}
			else if(listMessages.SelectedIndex!=-1) {
				selected=listMessages.SelectedIndex;
				if(selected==0) {
					return;
				}
				SigElementDefs.MoveUp(selected,listMessages.Items.GetAll<SigElementDef>().ToArray());
				FillLists();
				listMessages.SelectedIndex=selected-1;
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(listToFrom.SelectedIndex==-1
				&& listExtras.SelectedIndex==-1
				&& listMessages.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			int selected;
			if(listToFrom.SelectedIndex!=-1) {
				selected=listToFrom.SelectedIndex;
				if(selected==listToFrom.Items.Count-1) {
					return;
				}
				SigElementDefs.MoveDown(selected,listToFrom.Items.GetAll<SigElementDef>().ToArray());
				FillLists();
				listToFrom.SelectedIndex=selected+1;
			}
			else if(listExtras.SelectedIndex!=-1) {
				selected=listExtras.SelectedIndex;
				if(selected==listExtras.Items.Count-1) {
					return;
				}
				SigElementDefs.MoveDown(selected,listExtras.Items.GetAll<SigElementDef>().ToArray());
				FillLists();
				listExtras.SelectedIndex=selected+1;
			}
			else if(listMessages.SelectedIndex!=-1) {
				selected=listMessages.SelectedIndex;
				if(selected==listMessages.Items.Count-1) {
					return;
				}
				SigElementDefs.MoveDown(selected,listMessages.Items.GetAll<SigElementDef>().ToArray());
				FillLists();
				listMessages.SelectedIndex=selected+1;
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormMessagingSetup_FormClosing(object sender,FormClosingEventArgs e) {
			DataValid.SetInvalid(InvalidType.SigMessages);
		}

		

		

	

		


	}
}





















