using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormHL7DefMessageEdit:FormODBase {
		public HL7DefMessage HL7DefMesCur;
		public bool IsHL7DefInternal;
		public HL7InternalType InternalType;

		///<summary></summary>
		public FormHL7DefMessageEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7DefMessageEdit_Load(object sender,EventArgs e) {
			FillGrid();
			for(int i=1;i<Enum.GetNames(typeof(MessageTypeHL7)).Length;i++) {//Start at enum 1, 0 is NotDefined and is not displayed for user to select.  Used for unsupported message types
				comboMsgType.Items.Add(Lan.g("enumMessageTypeHL7",Enum.GetName(typeof(MessageTypeHL7), i).ToString()));
			}
			for(int i=1;i<Enum.GetNames(typeof(MessageStructureHL7)).Length;i++) {//start at enum 1, 0 is NotDefined and is not displayed for user to select.  Used for unsupported message structures
				comboMessageStructure.Items.Add(Lan.g("enumMessageStructureHL7",Enum.GetName(typeof(MessageStructureHL7),i).ToString()));
			}
			if(HL7DefMesCur!=null) {
				comboMsgType.SelectedIndex=(int)HL7DefMesCur.MessageType-1;//enum 0 is the NotDefined message type and is not in the list to select, so minus 1
				comboMessageStructure.SelectedIndex=(int)HL7DefMesCur.MessageStructure-1;//enum 0 is the NotDefined event type and is not in the list to select, so minus 1
				textItemOrder.Text=HL7DefMesCur.ItemOrder.ToString();
				textNote.Text=HL7DefMesCur.Note;
				if(HL7DefMesCur.InOrOut==InOutHL7.Incoming) {
					radioIn.Checked=true;
				}
				else {//outgoing
					radioOut.Checked=true;
				}
			}
			if(IsHL7DefInternal || InternalType==HL7InternalType.MedLabv2_3) {
				butAdd.Enabled=false;
				butOK.Enabled=false;
				butDelete.Enabled=false;
				labelDelete.Visible=true;
				if(InternalType==HL7InternalType.MedLabv2_3) {
					labelDelete.Text=Lan.g(this,"The messages and segments, and their item orders cannot be modified in a MedLabv2_3 definition.");
				}
			}
		}

		private void FillGrid() {
			if(!IsHL7DefInternal && !HL7DefMesCur.IsNew) {
				HL7DefMesCur.hl7DefSegments=HL7DefSegments.GetDeepFromDb(HL7DefMesCur.HL7DefMessageNum);
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Seg"),35);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Order"),40,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Can Repeat"),73,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Is Optional"),67,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Note"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			if(HL7DefMesCur!=null && HL7DefMesCur.hl7DefSegments!=null) {
				for(int i=0;i<HL7DefMesCur.hl7DefSegments.Count;i++) {
					GridRow row=new GridRow();
					row.Cells.Add(Lan.g("enumSegmentNameHL7",HL7DefMesCur.hl7DefSegments[i].SegmentName.ToString()));
					row.Cells.Add(HL7DefMesCur.hl7DefSegments[i].ItemOrder.ToString());
					row.Cells.Add(HL7DefMesCur.hl7DefSegments[i].CanRepeat?"X":"");
					row.Cells.Add(HL7DefMesCur.hl7DefSegments[i].IsOptional?"X":"");
					row.Cells.Add(HL7DefMesCur.hl7DefSegments[i].Note);
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormHL7DefSegmentEdit FormS=new FormHL7DefSegmentEdit();
			FormS.HL7DefSegCur=HL7DefMesCur.hl7DefSegments[e.Row];
			FormS.IsHL7DefInternal=IsHL7DefInternal;
			FormS.InternalType=InternalType;
			FormS.ShowDialog();
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Message?")) {
				return;
			}
			for(int s=0;s<HL7DefMesCur.hl7DefSegments.Count;s++) {
				for(int f=0;f<HL7DefMesCur.hl7DefSegments[s].hl7DefFields.Count;f++) {
					HL7DefFields.Delete(HL7DefMesCur.hl7DefSegments[s].hl7DefFields[f].HL7DefFieldNum);
				}
				HL7DefSegments.Delete(HL7DefMesCur.hl7DefSegments[s].HL7DefSegmentNum);
			}
			HL7DefMessages.Delete(HL7DefMesCur.HL7DefMessageNum);
			DialogResult=DialogResult.OK;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(HL7DefMesCur.IsNew) {
				HL7DefMessages.Insert(HL7DefMesCur);
				HL7DefMesCur.IsNew=false;
			}
			using FormHL7DefSegmentEdit FormS=new FormHL7DefSegmentEdit();
			FormS.HL7DefSegCur=new HL7DefSegment();
			FormS.HL7DefSegCur.HL7DefMessageNum=HL7DefMesCur.HL7DefMessageNum;
			FormS.HL7DefSegCur.IsNew=true;
			FormS.IsHL7DefInternal=false;
			FormS.InternalType=InternalType;
			FormS.ShowDialog();
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//This button is disabled if IsHL7DefInternal
			if(radioOut.Checked && !textItemOrder.IsValid()) {
				MsgBox.Show(this,"Please fix data entry error first.");
				return;
			}
			if(HL7DefMesCur.hl7DefSegments[0].SegmentName!=SegmentNameHL7.MSH) {
				MsgBox.Show(this,"The first segment in any message must be the MSH - Message Header segment.");
				return;
			}
			HL7DefMesCur.MessageType=(MessageTypeHL7)comboMsgType.SelectedIndex+1;//+1 because 0 is NotDefined and is not displayed for user to select
			HL7DefMesCur.MessageStructure=(MessageStructureHL7)comboMessageStructure.SelectedIndex+1;//+1 because 0 is NotDefined and is not displayed for user to select
			if(radioIn.Checked) {
				HL7DefMesCur.InOrOut=InOutHL7.Incoming;
			}
			else {
				HL7DefMesCur.InOrOut=InOutHL7.Outgoing;
			}
			HL7DefMesCur.ItemOrder=PIn.Int(textItemOrder.Text);
			HL7DefMesCur.Note=textNote.Text;
			if(HL7DefMesCur.IsNew) {
				HL7DefMessages.Insert(HL7DefMesCur);
			}
			else {
				HL7DefMessages.Update(HL7DefMesCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
