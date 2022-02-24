using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormRxNorms:FormODBase {
		private List<RxNorm> rxList;
		///<summary>When this window is used for selecting an RxNorm (medication.RxCui), then use must click OK, None, or double click in grid.  In those cases, this field will have a value.  If None was clicked, it will be a new RxNorm with an RxCui of zero.</summary>
		public RxNorm SelectedRxNorm;
		public List<RxNorm> ListSelectedRxNorms;
		public bool IsSelectionMode;
		public bool IsMultiSelectMode;
		public string InitSearchCodeOrDescript;

		public FormRxNorms() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRxNorms_Load(object sender,EventArgs e) {
			if(!IsSelectionMode && !IsMultiSelectMode) {
				butNone.Visible=false;
				butOK.Visible=false;
				butCancel.Text="Close";
			}
			if(IsMultiSelectMode) {
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			checkIgnore.Checked=true;
			if(!String.IsNullOrWhiteSpace(InitSearchCodeOrDescript)) {
				textCode.Text=InitSearchCodeOrDescript;
				if(InitSearchCodeOrDescript!=Regex.Replace(InitSearchCodeOrDescript,"[0-9]","")) {//Initial search text contains digits.
					checkIgnore.Checked=false;
				}
				FillGrid(true);//Try exact match first.
				if(gridMain.ListGridRows.Count==0) {//If no exact matches, then show similar matches.
					FillGrid(false);
				}
			}
		}

		private void FormRxNorms_Shown(object sender,EventArgs e) {
			if(RxNorms.IsRxNormTableSmall()) {
				if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Incomplete RxNorm list detected.  "
					+"If you intend to use RxNorm codes, you can download them now by clicking OK.  "
					+"The RxNorm codes will add about 10 MB to your database size.  Download RxNorm codes?"))
				{
					using FormCodeSystemsImport form=new FormCodeSystemsImport(CodeSystemName.RXNORM);
					form.ShowDialog();
				}
			}
		}
		
		private void butSearch_Click(object sender,EventArgs e) {
			FillGrid(false);
		}

		private void butExact_Click(object sender,EventArgs e) {
			FillGrid(true);
		}

		private void butClearSearch_Click(object sender,EventArgs e) {
			textCode.Text="";
		}

		private void FillGrid(bool isExact) {
			Cursor=Cursors.WaitCursor;
			rxList=RxNorms.GetListByCodeOrDesc(textCode.Text,isExact,checkIgnore.Checked);
			List <string> listMedicationRxCuis=Medications.GetWhere(x => x.RxCui!=0).Select(x => x.RxCui.ToString()).Distinct().ToList();
			List <string> listMedPatRxCuis=MedicationPats.GetForRxCuis(rxList.Select(x => x.RxCui).ToList()).Select(x => x.RxCui.ToString()).ToList();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormRxNorms","Code"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormRxNorms","InMedList"),60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormRxNorms","MedCount"),60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormRxNorms","Description"),80){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(RxNorm rxNorm in rxList) {
				row=new GridRow();
				row.Cells.Add(rxNorm.RxCui);//Code
				if(listMedicationRxCuis.Exists(x => x==rxNorm.RxCui)) {
					row.Cells.Add("X");//InMedList
				}
				else {
					row.Cells.Add("");//InMedList
				}
				row.Cells.Add(listMedPatRxCuis.FindAll(x => x==rxNorm.RxCui).Count.ToString());//MedCount
				row.Cells.Add(rxNorm.Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=0;
			Cursor=Cursors.Default;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!IsSelectionMode) {
				return;
			}
			SelectedRxNorm=rxList[e.Row];
			ListSelectedRxNorms=new List<RxNorm>();
			ListSelectedRxNorms.Add(rxList[e.Row]);
			DialogResult=DialogResult.OK;
		}

		//private void butRxNorm_Click(object sender,EventArgs e) {
		//	//if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will ")) {
		//	//	return;
		//	//}
		//	Cursor=Cursors.WaitCursor;
		//	RxNorms.CreateFreshRxNormTableFromZip();
		//	Cursor=Cursors.Default;
		//	MsgBox.Show(this,"done");
		//	//just making sure it worked:
		//	/*
		//	RxNorm rxNorm=RxNorms.GetOne(1);
		//	MsgBox.Show(this,rxNorm.RxNormNum+" "+rxNorm.RxCui+" "+rxNorm.MmslCode+" "+rxNorm.Description);
		//	MsgBox.Show(this,RxNorms.GetMmslCodeByRxCui("1000005")+" <-- should be 26420");
		//	MsgBox.Show(this,RxNorms.GetMmslCodeByRxCui("1000002")+" <-- should be blank");*/
		//}

		private void butNone_Click(object sender,EventArgs e) {
			SelectedRxNorm=new RxNorm();
			ListSelectedRxNorms=new List<RxNorm>();
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()<0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			SelectedRxNorm=rxList[gridMain.GetSelectedIndex()];
			ListSelectedRxNorms=new List<RxNorm>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				ListSelectedRxNorms.Add(rxList[gridMain.SelectedIndices[i]]);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}