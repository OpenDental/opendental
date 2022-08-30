using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetFieldExam:FormODBase {
		public string ExamFieldSelected;

		public FormSheetFieldExam() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldDefEdit_Load(object sender,EventArgs e) {
			List<SheetDef> listSheetDefsAvail=SheetDefs.GetCustomForType(SheetTypeEnum.ExamSheet);
			if(listSheetDefsAvail==null || listSheetDefsAvail.Count==0) {
				MsgBox.Show(this,"No custom Exam Sheets are defined.");
				return;
			}
			listBoxExamSheets.Items.Clear();
			listBoxExamSheets.Items.AddList(listSheetDefsAvail,x => x.Description);
			listBoxExamSheets.SetSelected(0);
			FillFieldList();
		}

		private void FillFieldList() {
			List<string> listSortedAvailFields=new List<string>(); //will alphabetize, since we are adding either FieldName,ReportableName, or RadioButtonGroup depending on field type
			//Add exam sheet fields to the list
			List<SheetFieldDef> listSheetFieldDefsAvail=SheetFieldDefs.GetForExamSheet(listBoxExamSheets.GetSelected<SheetDef>().SheetDefNum);
			for(int i=0;i<listSheetFieldDefsAvail.Count;i++) {
				if(listSheetFieldDefsAvail[i].FieldName=="") {
					continue;
				}
				else if(listSheetFieldDefsAvail[i].FieldName!="misc") {//This is an internally defined field
					listSortedAvailFields.Add(listSheetFieldDefsAvail[i].FieldName);
				}
				//misc:
				else if(listSheetFieldDefsAvail[i].RadioButtonGroup!="") {//Only gets set if field is a 'misc' check box and assigned to a group
					if(listSortedAvailFields.Contains(listSheetFieldDefsAvail[i].RadioButtonGroup)) {
						continue;
					}
					else {
						listSortedAvailFields.Add(listSheetFieldDefsAvail[i].RadioButtonGroup);
					}
				}
				else if(listSheetFieldDefsAvail[i].ReportableName!="") {//Not internal type or part of a RadioButtonGroup so just add the reportable name if available
					listSortedAvailFields.Add(listSheetFieldDefsAvail[i].ReportableName);
				}
			}
			listSortedAvailFields.Sort();
			listBoxAvailFields.Items.Clear();
			listBoxAvailFields.Items.AddList(listSortedAvailFields,x => x.ToString());
		}

		private void listExamSheets_MouseClick(object sender,MouseEventArgs e) {
			FillFieldList();
		}

		private void listAvailFields_DoubleClick(object sender,EventArgs e) {
			if(listBoxAvailFields.SelectedIndex==-1) {
				return;
			}
			ExamFieldSelected=SheetTypeEnum.ExamSheet.ToString()+":"+listBoxExamSheets.GetSelected<SheetDef>().Description+";"
				+listBoxAvailFields.SelectedItem.ToString();//either RadioButtonGroup or ReportableName or internally defined field name
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(listBoxAvailFields.SelectedIndex==-1) {//if there is no selected reportable field
				MsgBox.Show(this,"You must select a field first.");
				return;
			}
			//example:  ExamSheet:NewPatient;Race
			ExamFieldSelected="ExamSheet:"+listBoxExamSheets.GetSelected<SheetDef>().Description+";"
				+listBoxAvailFields.SelectedItem.ToString();//either RadioButtonGroup or ReportableName or internally defined field name
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}