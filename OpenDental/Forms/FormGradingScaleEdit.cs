using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormGradingScaleEdit:FormODBase {
		private List<GradingScaleItem> _listGradingScaleItems;
		private GradingScale _gradingScaleCur;
		///<summary>False when grading scale is in use by an evaluation.</summary>
		private bool _isEditable=true;

		public FormGradingScaleEdit(GradingScale gradingScaleCur) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_gradingScaleCur=gradingScaleCur;
		}

		private void FormGradingScaleEdit_Load(object sender,EventArgs e) {
			comboScaleType.SelectedIndex=-1;
			for(int i=0;i<Enum.GetNames(typeof(EnumScaleType)).Length;i++) {
				comboScaleType.Items.Add(Lan.g("FormGradingScaleEdit",Enum.GetNames(typeof(EnumScaleType))[i]));
				if(i==(int)_gradingScaleCur.ScaleType) {
					comboScaleType.SelectedIndex=i;
				}
			}
			if(comboScaleType.SelectedIndex==(int)EnumScaleType.Percentage) {
				labelPercent.Visible=true;
			}
			textDescription.Text=_gradingScaleCur.Description;
			if(_gradingScaleCur.IsNew) {
				return;
			}
			LoadScaleType();
			if(GradingScales.IsInUseByEvaluation(_gradingScaleCur)) {
				//Locking grading scales from being edited is necessary with the current schema since changing grading scales that are in use 
				//would result in changing grades for previously filled out evaluations. 
				//This could be changed later by creating copies of grading scales and attaching them to the evaluation/criterion similarly to evaluationdefs.
				labelWarning.Text=Lan.g(this,"Grading scale is not editable.  It is currently in use by an evaluation.");
				labelWarning.Visible=true;
				_isEditable=false;
				butAdd.Enabled=false;
				butOK.Enabled=false;
				butDelete.Enabled=false;
				textDescription.ReadOnly=true;
			}
			FillGrid();
		}

		private void FillGrid() {
			_listGradingScaleItems=GradingScaleItems.Refresh(_gradingScaleCur.GradingScaleNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormGradingScaleEdit","Shown"),60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormGradingScaleEdit","Number"),60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormGradingScaleEdit","Description"),160);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listGradingScaleItems.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listGradingScaleItems[i].GradeShowing);
				row.Cells.Add(_listGradingScaleItems[i].GradeNumber.ToString());
				row.Cells.Add(_listGradingScaleItems[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_DoubleClick(object sender,EventArgs e) {
			if(!_isEditable) {
				return;
			}
			using FormGradingScaleItemEdit FormGSIE=new FormGradingScaleItemEdit(_listGradingScaleItems[gridMain.GetSelectedIndex()]);
			FormGSIE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			GradingScaleItem gradingScaleItemNew=new GradingScaleItem();
			gradingScaleItemNew.GradingScaleNum=_gradingScaleCur.GradingScaleNum;//Must be set prior to edit window being open if a new item.
			gradingScaleItemNew.IsNew=true;
			using FormGradingScaleItemEdit FormGSIE=new FormGradingScaleItemEdit(gradingScaleItemNew);
			FormGSIE.ShowDialog();
			FillGrid();
		}

		private void comboScaleType_SelectionChangeCommitted(object sender,EventArgs e) {
			LoadScaleType();
		}

		/// <summary>Reloads the UI for the grade scale type currently selected in the combo box.</summary>
		private void LoadScaleType() {
			//Maximum points isn't really used for anything besides points.  
			//Later it may be useful to change maximum points to be on criterion instead and have this points grading scale
			//be more like a flag that designates whether or not those criterion need to be given maximum points.
			//For now MaximumPoints is used to determine a point max for the evaluation, it is still possible to give them less than the maximum.
			if(comboScaleType.SelectedIndex==(int)EnumScaleType.PickList) {
				butAdd.Enabled=true;
				labelWarning.Visible=false;
				labelPercent.Visible=false;
			}
			else if(comboScaleType.SelectedIndex==(int)EnumScaleType.Percentage) {
				butAdd.Enabled=false;
				labelWarning.Visible=true;
				labelPercent.Visible=true;
			}
			else {//ScaleType.Points
				butAdd.Enabled=false;
				labelWarning.Visible=true;
				labelPercent.Visible=false;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will delete the grading scale.  Continue?")) {
				return;
			}
			try {
				GradingScales.Delete(_gradingScaleCur.GradingScaleNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text=="") {
				MsgBox.Show(this,"Please input a description.");
				return;
			}
			_gradingScaleCur.Description=textDescription.Text;
			_gradingScaleCur.ScaleType=(EnumScaleType)comboScaleType.SelectedIndex;
			if(GradingScales.IsDupicateDescription(_gradingScaleCur)) {//This will check it for like types.
				MsgBox.Show(this,"The selected grading scale description is already used by another grading scale.  Please input a unique description.");
				return;
			}
			_gradingScaleCur.Description=textDescription.Text;
			_gradingScaleCur.ScaleType=(EnumScaleType)comboScaleType.SelectedIndex;
			if(comboScaleType.SelectedIndex!=(int)EnumScaleType.PickList && comboScaleType.Visible) {//Deletes all items if not picklist scaletype.
				GradingScaleItems.DeleteAllByGradingScale(_gradingScaleCur.GradingScaleNum);
			}
			GradingScales.Update(_gradingScaleCur);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			if(_gradingScaleCur.IsNew) {
				try {
					GradingScales.Delete(_gradingScaleCur.GradingScaleNum);
				}
				catch(Exception ex) {
					MessageBox.Show(this,ex.Message);
					return;
				}
			}
			DialogResult=DialogResult.Cancel;
		}



	}
}