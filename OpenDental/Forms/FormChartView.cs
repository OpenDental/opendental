using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormChartView : FormODBase {
		private bool changed;
		private List<DisplayField> _listDisplayFieldsShowing;
		public ChartView ChartViewCur;

		///<summary></summary>
		public FormChartView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormChartView_Load(object sender,EventArgs e) {
			string[] chartViewDateTypes=Enum.GetNames(typeof(ChartViewDates));
			Array chartViewDateValues=Enum.GetValues(typeof(ChartViewDates));
			for(int i=0;i<chartViewDateTypes.Length;i++) {
				comboDatesShowing.Items.Add(chartViewDateTypes[i]);
				if(ChartViewCur.DatesShowing==(ChartViewDates)chartViewDateValues.GetValue(i)){
					comboDatesShowing.SelectedIndex=i;
				}
			}
			if(!ChartViewCur.IsNew) {
				textBoxViewDesc.Text=ChartViewCur.Description;
			}
			else {
				ChartViewCur.ItemOrder+=1;
				textBoxViewDesc.Text=ChartViewCur.ItemOrder.ToString();
			}
			checkAppt.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.Appointments)==ChartViewObjs.Appointments;
			checkComm.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.CommLog)==ChartViewObjs.CommLog;
			checkCommFamily.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.CommLogFamily)==ChartViewObjs.CommLogFamily;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				checkCommSuperFamily.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.CommLogSuperFamily)==ChartViewObjs.CommLogSuperFamily;
			}
			else {
				checkCommSuperFamily.Checked=false;
				checkCommSuperFamily.Visible=false;
			}
			checkTasks.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.Tasks)==ChartViewObjs.Tasks;
			checkEmail.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.Email)==ChartViewObjs.Email;
			checkLabCase.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.LabCases)==ChartViewObjs.LabCases;
			checkRx.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.Rx)==ChartViewObjs.Rx;
			checkSheets.Checked=(ChartViewCur.ObjectTypes & ChartViewObjs.Sheets)==ChartViewObjs.Sheets;
			checkShowTP.Checked=(ChartViewCur.ProcStatuses & ChartViewProcStat.TP)==ChartViewProcStat.TP;
			checkShowC.Checked=(ChartViewCur.ProcStatuses & ChartViewProcStat.C)==ChartViewProcStat.C;
			checkShowE.Checked=(ChartViewCur.ProcStatuses & ChartViewProcStat.EC)==ChartViewProcStat.EC;
			checkShowR.Checked=(ChartViewCur.ProcStatuses & ChartViewProcStat.R)==ChartViewProcStat.R;
			checkShowCn.Checked=(ChartViewCur.ProcStatuses & ChartViewProcStat.Cn)==ChartViewProcStat.Cn;
			checkShowTeeth.Checked=ChartViewCur.SelectedTeethOnly;
			checkNotes.Checked=ChartViewCur.ShowProcNotes;
			checkAudit.Checked=ChartViewCur.IsAudit;
			checkTPChart.Checked=ChartViewCur.IsTpCharting;
			DisplayFields.RefreshCache();
			_listDisplayFieldsShowing=DisplayFields.GetForChartView(ChartViewCur.ChartViewNum);//This will be zero for a new ChartView
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkShowTeeth.Visible=false;
			}
			FillGrids();
		}

		private void FillGrids() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormChartView","FieldName"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormChartView","New Descript"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormChartView","Width"),60);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listDisplayFieldsShowing.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listDisplayFieldsShowing[i].InternalName);
				row.Cells.Add(_listDisplayFieldsShowing[i].Description);
				row.Cells.Add(_listDisplayFieldsShowing[i].ColumnWidth.ToString());
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			List<DisplayField> listDisplayFieldsAvail=DisplayFields.GetAllAvailableList(DisplayFieldCategory.None);
			for(int i=0;i<_listDisplayFieldsShowing.Count;i++) {
				for(int j=0;j<listDisplayFieldsAvail.Count;j++) {
					if(_listDisplayFieldsShowing[i].InternalName==listDisplayFieldsAvail[j].InternalName) {
						listDisplayFieldsAvail.RemoveAt(j);
						break;
					}
				}
			}
			listAvailable.Items.Clear();
			for(int i=0;i<listDisplayFieldsAvail.Count;i++) {
				listAvailable.Items.Add(listDisplayFieldsAvail[i].ToString(),listDisplayFieldsAvail[i]);
			}
		}

		#region Show
		private void checkShowTP_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkShowC_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkShowE_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkShowR_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkShowCn_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkNotes_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkAppt_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkComm_Click(object sender,System.EventArgs e) {
			if(!checkComm.Checked) {
				checkCommFamily.Checked=false;//uncheck family when comm is unchecked
				checkCommSuperFamily.Checked=false;//uncheck super family when comm is unchecked
			}
			changed=true;
		}

		private void checkCommFamily_Click(object sender,System.EventArgs e) {
			if(checkCommFamily.Checked) {
				checkComm.Checked=true;//check comm when family is checked
			}
			else {
				checkCommSuperFamily.Checked=false;//uncheck super family when family is unchecked
			}
			changed=true;
		}
		private void checkCommSuperFamily_Click(object sender,EventArgs e) {
			if(checkCommSuperFamily.Checked) {
				checkCommFamily.Checked=true;//check family when super family is checked
				checkComm.Checked=true;//check comm when super family is checked
			}
			changed=true;
		}

		private void checkLabCase_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkRx_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkTasks_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkEmail_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkSheets_Click(object sender,System.EventArgs e) {
			changed=true;
		}

		private void checkShowTeeth_Click(object sender,System.EventArgs e) {
			if(checkShowTeeth.Checked) {
				checkShowTP.Checked=true;
				checkShowC.Checked=true;
				checkShowE.Checked=true;
				checkShowR.Checked=true;
				checkShowCn.Checked=true;
				checkNotes.Checked=true;
				checkAppt.Checked=false;
				checkComm.Checked=false;
				checkCommFamily.Checked=false;
				checkCommSuperFamily.Checked=false;
				checkLabCase.Checked=false;
				checkRx.Checked=false;
				checkEmail.Checked=false;
				checkTasks.Checked=false;
				checkSheets.Checked=false;
			}
			else {
				checkShowTP.Checked=true;
				checkShowC.Checked=true;
				checkShowE.Checked=true;
				checkShowR.Checked=true;
				checkShowCn.Checked=true;
				checkNotes.Checked=true;
				checkAppt.Checked=true;
				checkComm.Checked=true;
				checkCommFamily.Checked=true;
				if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
					checkCommSuperFamily.Checked=true;
				}
				checkLabCase.Checked=true;
				checkRx.Checked=true;
				checkEmail.Checked=true;
				checkTasks.Checked=true;
				checkSheets.Checked=true;
			}
			changed=true;
		}

		private void checkAudit_Click(object sender,EventArgs e) {
			changed=true;
		}

		private void checkTPChart_Click(object sender,EventArgs e) {
			changed=true;
		}

		private void butShowAll_Click(object sender,EventArgs e) {
			checkShowTP.Checked=true;
			checkShowC.Checked=true;
			checkShowE.Checked=true;
			checkShowR.Checked=true;
			checkShowCn.Checked=true;
			checkNotes.Checked=true;
			checkAppt.Checked=true;
			checkComm.Checked=true;
			checkCommFamily.Checked=true;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				checkCommSuperFamily.Checked=true;
			}
			checkLabCase.Checked=true;
			checkRx.Checked=true;
			checkShowTeeth.Checked=false;
			checkTasks.Checked=true;
			checkEmail.Checked=true;
			checkSheets.Checked=true;
			changed=true;
		}

		private void butShowNone_Click(object sender,EventArgs e) {
			checkShowTP.Checked=false;
			checkShowC.Checked=false;
			checkShowE.Checked=false;
			checkShowR.Checked=false;
			checkShowCn.Checked=false;
			checkNotes.Checked=false;
			checkAppt.Checked=false;
			checkComm.Checked=false;
			checkCommFamily.Checked=false;
			checkCommSuperFamily.Checked=false;
			checkLabCase.Checked=false;
			checkRx.Checked=false;
			checkShowTeeth.Checked=false;
			checkTasks.Checked=false;
			checkEmail.Checked=false;
			checkSheets.Checked=false;
			changed=true;
		}
		#endregion Show

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormDisplayFieldEdit formD=new FormDisplayFieldEdit();
			formD.AllowZeroWidth=true;
			formD.FieldCur=_listDisplayFieldsShowing[e.Row];
			formD.ShowDialog();
			FillGrids();
			changed=true;
		}

		private void listProcStatusCodes_MouseUp(object sender,MouseEventArgs e) {
			changed=true;
		}

		private void butDefault_Click(object sender,EventArgs e) {
			_listDisplayFieldsShowing=DisplayFields.GetDefaultList(DisplayFieldCategory.None);
			FillGrids();
			changed=true;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(listAvailable.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select an item in the list on the right first.");
				return;
			}
			List<DisplayField> listDisplayFieldsSelected=listAvailable.GetListSelected<DisplayField>();
			for(int i=0;i<listDisplayFieldsSelected.Count;i++) {
				_listDisplayFieldsShowing.Add(listDisplayFieldsSelected[i]);
			}
			FillGrids();
			changed=true;
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid on the left first.");
				return;
			}
			for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--) {//go backwards
				_listDisplayFieldsShowing.RemoveAt(gridMain.SelectedIndices[i]);
			}
			FillGrids();
			changed=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				selected[i]=gridMain.SelectedIndices[i];
			}
			if(selected[0]==0) {
				return;
			}
			for(int i=0;i<selected.Length;i++) {
				_listDisplayFieldsShowing.Reverse(selected[i]-1,2);
			}
			FillGrids();
			for(int i=0;i<selected.Length;i++) {
				gridMain.SetSelected(selected[i]-1,true);
			}
			changed=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridMain.SelectedIndices.Length];
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				selected[i]=gridMain.SelectedIndices[i];
			}
			if(selected[selected.Length-1]==_listDisplayFieldsShowing.Count-1) {
				return;
			}
			for(int i=selected.Length-1;i>=0;i--) {//go backwards
				_listDisplayFieldsShowing.Reverse(selected[i],2);
			}
			FillGrids();
			for(int i=0;i<selected.Length;i++) {
				gridMain.SetSelected(selected[i]+1,true);
			}
			changed=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ChartViewCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this chart view?")) {
				return;
			}
			try {
				ChartViews.Delete(ChartViewCur.ChartViewNum);
				DisplayFields.DeleteForChartView(ChartViewCur.ChartViewNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textBoxViewDesc.Text.Trim()=="") {
				MsgBox.Show(this,"View description cannot be blank.");
				return;
			}
			if(ChartViewCur.Description!=textBoxViewDesc.Text) {
				changed=true;
			}
			if(!changed) {
				DialogResult=DialogResult.OK;
				return;
			}
			ChartViewCur.Description=textBoxViewDesc.Text;
			ChartViewCur.DatesShowing=(ChartViewDates)Enum.GetValues(typeof(ChartViewDates)).GetValue(comboDatesShowing.SelectedIndex);
			ChartViewCur.ObjectTypes=ChartViewObjs.None;
			if(checkAppt.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.Appointments;
			}
			if(checkCommFamily.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.CommLogFamily;
			}
			if(checkCommSuperFamily.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.CommLogSuperFamily;
			}
			if(checkTasks.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.Tasks;
			}
			if(checkEmail.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.Email;
			}
			if(checkLabCase.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.LabCases;
			}
			if(checkRx.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.Rx;
			}
			if(checkComm.Checked) {
			ChartViewCur.ObjectTypes|=ChartViewObjs.CommLog;
			}
			if(checkSheets.Checked) {
				ChartViewCur.ObjectTypes|=ChartViewObjs.Sheets;
			}
			ChartViewCur.ProcStatuses=ChartViewProcStat.None;
			if(checkShowTP.Checked) {
				ChartViewCur.ProcStatuses|=ChartViewProcStat.TP;
			}
			if(checkShowC.Checked) {
				ChartViewCur.ProcStatuses|=ChartViewProcStat.C;
			}
			if(checkShowE.Checked) {
				ChartViewCur.ProcStatuses|=ChartViewProcStat.EC;
			}
			if(checkShowR.Checked) {
				ChartViewCur.ProcStatuses|=ChartViewProcStat.R;
			}
			if(checkShowCn.Checked) {
				ChartViewCur.ProcStatuses|=ChartViewProcStat.Cn;
			}	
			ChartViewCur.SelectedTeethOnly=checkShowTeeth.Checked;
			ChartViewCur.ShowProcNotes=checkNotes.Checked;
			ChartViewCur.IsAudit=checkAudit.Checked;
			ChartViewCur.IsTpCharting=checkTPChart.Checked;		
			if(!ChartViewCur.IsNew) {
				ChartViews.Update(ChartViewCur);
			}
			else {
				ChartViewCur.ItemOrder=-1;
				ChartViews.Insert(ChartViewCur);
			}
			DisplayFields.SaveListForChartView(_listDisplayFieldsShowing,ChartViewCur.ChartViewNum);
			DisplayFields.RefreshCache();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void comboDatesShowing_SelectedIndexChanged(object sender,EventArgs e) {
			changed=true;
		}




	}
}
