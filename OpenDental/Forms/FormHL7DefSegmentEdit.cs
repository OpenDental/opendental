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
	public partial class FormHL7DefSegmentEdit:FormODBase {
		public HL7DefSegment HL7DefSegmentCur;
		public bool IsHL7DefInternal;
		public HL7InternalType HL7InternalType_;

		///<summary></summary>
		public FormHL7DefSegmentEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7DefSegmentEdit_Load(object sender,EventArgs e) {
			FillGrid();
			for(int i=0;i<Enum.GetNames(typeof(SegmentNameHL7)).Length;i++) {
				comboSegmentName.Items.Add(Lan.g("enumSegmentNameHL7",Enum.GetName(typeof(SegmentNameHL7),i).ToString()));
			}
			if(HL7DefSegmentCur!=null) {
				comboSegmentName.SelectedIndex=(int)HL7DefSegmentCur.SegmentName;
				textItemOrder.Text=HL7DefSegmentCur.ItemOrder.ToString();
				checkCanRepeat.Checked=HL7DefSegmentCur.CanRepeat;
				checkIsOptional.Checked=HL7DefSegmentCur.IsOptional;
				textNote.Text=HL7DefSegmentCur.Note;
			}
			if(IsHL7DefInternal || HL7InternalType_==HL7InternalType.MedLabv2_3) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
				labelDelete.Visible=true;
				butAdd.Enabled=false;
				if(HL7InternalType_==HL7InternalType.MedLabv2_3) {
					labelDelete.Text=Lan.g(this,"The segments and their item orders cannot be modified in a MedLabv2_3 definition.");
				}
			}
		}

		private void FillGrid() {
			if(!IsHL7DefInternal && !HL7DefSegmentCur.IsNew) {
				HL7DefSegmentCur.hl7DefFields=HL7DefFields.GetFromDb(HL7DefSegmentCur.HL7DefSegmentNum);
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Field Name"),180);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Fixed Text"),240);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Type"),40);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Order"),40,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Table ID"),75);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			if(HL7DefSegmentCur!=null && HL7DefSegmentCur.hl7DefFields!=null) {
				for(int i=0;i<HL7DefSegmentCur.hl7DefFields.Count;i++) {
					GridRow row=new GridRow();
					row.Cells.Add(HL7DefSegmentCur.hl7DefFields[i].FieldName);
					row.Cells.Add(HL7DefSegmentCur.hl7DefFields[i].FixedText);
					row.Cells.Add(Lan.g("enumDataTypeHL7",HL7DefSegmentCur.hl7DefFields[i].DataType.ToString()));
					row.Cells.Add(HL7DefSegmentCur.hl7DefFields[i].OrdinalPos.ToString());
					row.Cells.Add(HL7DefSegmentCur.hl7DefFields[i].TableId);
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormHL7DefFieldEdit formHL7DefFieldEdit=new FormHL7DefFieldEdit();
			formHL7DefFieldEdit.HL7DefFieldCur=HL7DefSegmentCur.hl7DefFields[e.Row];
			formHL7DefFieldEdit.IsHL7DefInternal=IsHL7DefInternal;
			formHL7DefFieldEdit.ShowDialog();
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Segment?")) {
				return;
			}
			for(int f=0;f<HL7DefSegmentCur.hl7DefFields.Count;f++) {
				HL7DefFields.Delete(HL7DefSegmentCur.hl7DefFields[f].HL7DefFieldNum);
			}
			HL7DefSegments.Delete(HL7DefSegmentCur.HL7DefSegmentNum);
			DialogResult=DialogResult.OK;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(HL7DefSegmentCur.IsNew) {
				HL7DefSegments.Insert(HL7DefSegmentCur);
				HL7DefSegmentCur.IsNew=false;
			}
			using FormHL7DefFieldEdit formHL7DefFieldEdit=new FormHL7DefFieldEdit();
			formHL7DefFieldEdit.HL7DefFieldCur=new HL7DefField();
			formHL7DefFieldEdit.HL7DefFieldCur.HL7DefSegmentNum=HL7DefSegmentCur.HL7DefSegmentNum;
			formHL7DefFieldEdit.HL7DefFieldCur.IsNew=true;
			formHL7DefFieldEdit.HL7DefFieldCur.FixedText="";
			formHL7DefFieldEdit.IsHL7DefInternal=false;
			formHL7DefFieldEdit.ShowDialog();
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not enabled if internal
			if(!textItemOrder.IsValid()) {
				MsgBox.Show(this,"Please fix data entry error first.");
				return;
			}
			HL7DefSegmentCur.SegmentName=(SegmentNameHL7)comboSegmentName.SelectedIndex;
			HL7DefSegmentCur.ItemOrder=PIn.Int(textItemOrder.Text);
			HL7DefSegmentCur.CanRepeat=checkCanRepeat.Checked;
			HL7DefSegmentCur.IsOptional=checkIsOptional.Checked;
			HL7DefSegmentCur.Note=textNote.Text;
			if(HL7DefSegmentCur.ItemOrder==0 && HL7DefSegmentCur.SegmentName==SegmentNameHL7.MSH) {
				for(int i=0;i<HL7DefSegmentCur.hl7DefFields.Count;i++) {
					if(HL7DefSegmentCur.hl7DefFields[i].FieldName=="separators^~\\&" && HL7DefSegmentCur.hl7DefFields[i].OrdinalPos!=1) {//we force the separators^~\\& to be in field 1 of the message header segment or we will not know how to split the messageType field (usually messageType^eventType, but the '^' is defined in the separator segment) to get the message type
						MsgBox.Show(this,"The separators^~\\& field must be in position 1 of the message header segment.");
						return;
					}
					if(HL7DefSegmentCur.hl7DefFields[i].FieldName=="messageType" && HL7DefSegmentCur.hl7DefFields[i].OrdinalPos!=8) {//we force messageType to be in field 8 of the message header segment or we will not be able to retrieve a definition for this type of message when processing
						MsgBox.Show(this,"The messageType field must be in position 8 of the message header segment.");
						return;
					}
				}
			}
			if(HL7DefSegmentCur.IsNew) {
				HL7DefSegments.Insert(HL7DefSegmentCur);
			}
			else {
				HL7DefSegments.Update(HL7DefSegmentCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}