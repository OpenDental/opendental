using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormHL7DefFieldEdit:FormODBase {
		public HL7DefField HL7DefFieldCur;
		private List<FieldNameAndType> FieldNameList;
		public bool IsHL7DefInternal;

		///<summary></summary>
		public FormHL7DefFieldEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7DefFieldEdit_Load(object sender,EventArgs e) {
			FieldNameList=new List<FieldNameAndType>();
			FillFieldNameList();
			for(int i=0;i<FieldNameList.Count;i++) {
				listFieldNames.Items.Add(FieldNameList[i].Name);
				if(HL7DefFieldCur!=null && HL7DefFieldCur.FieldName==FieldNameList[i].Name) {
					listFieldNames.SelectedIndex=i;
					comboDataType.Enabled=false;
				}
			}
			if(HL7DefFieldCur!=null && HL7DefFieldCur.FixedText.Length>0) {
				textFixedText.Text=HL7DefFieldCur.FixedText;
				comboDataType.Enabled=true;
			}
			for(int i=0;i<Enum.GetNames(typeof(DataTypeHL7)).Length;i++) {
				comboDataType.Items.Add(Lan.g("enumDataTypeHL7",Enum.GetName(typeof(DataTypeHL7),i).ToString()));
			}
			if(HL7DefFieldCur!=null) {
				comboDataType.SelectedIndex=(int)HL7DefFieldCur.DataType;
				textItemOrder.Text=HL7DefFieldCur.OrdinalPos.ToString();
				textTableId.Text=HL7DefFieldCur.TableId;
			}
			if(IsHL7DefInternal) {
				butOK.Enabled=false;
				butDelete.Enabled=false;
				labelDelete.Visible=true;
			}
		}

		private void FillFieldNameList() {
			FieldNameList=FieldNameAndType.GetFullList();
		}

		private void textFixedText_KeyUp(object sender,EventArgs e) {
			listFieldNames.ClearSelected();
			comboDataType.Enabled=true;
		}

		private void listFieldNames_Click(object sender,EventArgs e) {
			textFixedText.Clear();
			comboDataType.SelectedIndex=(int)FieldNameList[listFieldNames.SelectedIndex].DataType;
			comboDataType.Enabled=false;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Field?")) {
				return;
			}
			HL7DefFields.Delete(HL7DefFieldCur.HL7DefFieldNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//This button is disabled if IsHL7DefInternal
			if(!textItemOrder.IsValid()) {
				MsgBox.Show(this,"Please fix data entry error first.");
				return;
			}
			HL7DefFieldCur.DataType=(DataTypeHL7)comboDataType.SelectedIndex;
			HL7DefFieldCur.TableId=textTableId.Text;
			HL7DefFieldCur.OrdinalPos=PIn.Int(textItemOrder.Text);
			if(listFieldNames.SelectedIndex!=-1) {
				HL7DefFieldCur.FieldName=listFieldNames.SelectedItem.ToString();
				HL7DefFieldCur.FixedText="";
			}
			else {
				HL7DefFieldCur.FieldName="";
				HL7DefFieldCur.FixedText=textFixedText.Text;
			}
			if(HL7DefFieldCur.IsNew) {
				HL7DefFields.Insert(HL7DefFieldCur);
			}
			else {
				HL7DefFields.Update(HL7DefFieldCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}



}
