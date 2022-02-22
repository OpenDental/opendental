using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDunningSetup:FormODBase {
		private List<Dunning> _listDunningsAll;
		private List<Def> _listDefsBillingTypes;

		public FormDunningSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDunningSetup_Load(object sender,EventArgs e) {
			_listDefsBillingTypes=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			listBill.Items.Add("("+Lan.g(this,"all")+")");
			listBill.SetSelected(0,true);
			listBill.Items.AddStrings(_listDefsBillingTypes.Select(x => x.ItemName));
			comboClinics.SelectedClinicNum=Clinics.ClinicNum;
			FillGrids(true);
		}

		private void FillGrids(bool doRefreshList=false) {
			if(doRefreshList) {
				_listDunningsAll=Dunnings.Refresh(Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum).ToList());
			}
			List<Dunning> listDunningsSubs=_listDunningsAll.FindAll(x => ValidateDunningFilters(x));
			if(!PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				listDunningsSubs.RemoveAll(x => x.IsSuperFamily);
			}
			gridDunning.BeginUpdate();
			gridDunning.Columns.Clear();
			gridDunning.Columns.Add(new GridColumn("Billing Type",80));
			gridDunning.Columns.Add(new GridColumn("Aging",70));
			gridDunning.Columns.Add(new GridColumn("Ins",40));
			gridDunning.Columns.Add(new GridColumn("Message",150));
			gridDunning.Columns.Add(new GridColumn("Bold Message",150));
			gridDunning.Columns.Add(new GridColumn("Email",35,HorizontalAlignment.Center));
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				gridDunning.Columns.Add(new GridColumn("SF",30,HorizontalAlignment.Center));
			}
			if(PrefC.HasClinicsEnabled) {
				gridDunning.Columns.Add(new GridColumn("Clinic",50));
			}
			gridDunning.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listDunningsSubs.Count;i++) {
				row=new GridRow();
				if(listDunningsSubs[i].BillingType==0){
					row.Cells.Add(Lan.g(this,"all"));
				}
				else{
					row.Cells.Add(Defs.GetName(DefCat.BillingTypes,listDunningsSubs[i].BillingType));
				}
				if(listDunningsSubs[i].AgeAccount==0){
					row.Cells.Add(Lan.g(this,"any"));
				}
				else{
					row.Cells.Add(Lan.g(this,"Over ")+listDunningsSubs[i].AgeAccount.ToString());
				}
				if(listDunningsSubs[i].InsIsPending==YN.Yes) {
					row.Cells.Add(Lan.g(this,"Y"));
				}
				else if(listDunningsSubs[i].InsIsPending==YN.No) {
					row.Cells.Add(Lan.g(this,"N"));
				}
				else {//YN.Unknown
					row.Cells.Add(Lan.g(this,"any"));
				}
				row.Cells.Add(listDunningsSubs[i].DunMessage);
				GridCell gridCell=new GridCell(listDunningsSubs[i].MessageBold);
				gridCell.Bold=YN.Yes;
				gridCell.ColorText=Color.DarkRed;
				row.Cells.Add(gridCell);
				row.Cells.Add((!string.IsNullOrEmpty(listDunningsSubs[i].EmailBody) || !string.IsNullOrEmpty(listDunningsSubs[i].EmailSubject))?"X":"");
				if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
					row.Cells.Add(listDunningsSubs[i].IsSuperFamily?"X":"");
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(listDunningsSubs[i].ClinicNum));
				}
				row.Tag=listDunningsSubs[i];
				gridDunning.ListGridRows.Add(row);
			}
			gridDunning.EndUpdate();
		}

		private bool ValidateDunningFilters(Dunning dunning) {
			if((!comboClinics.IsAllSelected && comboClinics.SelectedClinicNum!=dunning.ClinicNum)
				||(!listBill.SelectedIndices.Contains(0) && !listBill.SelectedIndices.OfType<int>().Select(x => _listDefsBillingTypes[x-1].DefNum).Contains(dunning.BillingType))
				||(!radioAny.Checked && dunning.AgeAccount!=(byte)(30*new List<RadioButton> { radioAny,radio30,radio60,radio90 }.FindIndex(x => x.Checked)))//0, 30, 60, or 90
				||(!string.IsNullOrWhiteSpace(textAdv.Text) && dunning.DaysInAdvance!=PIn.Int(textAdv.Text,false))//blank=0
				||(!radioU.Checked && dunning.InsIsPending!=(YN)new List<RadioButton> { radioU,radioY,radioN }.FindIndex(x => x.Checked)))//0=Unknown, 1=Yes, 2=No+
			{
				return false;
			}
			return true;
		}

		private void OnFilterChanged(object sender,EventArgs e) {
		 if(_listDunningsAll==null) {//Not initialized yet
			return;
		 }
			FillGrids();
		}

		private void gridDunning_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormDunningEdit formDunningEdit=new FormDunningEdit((Dunning)gridDunning.ListGridRows[e.Row].Tag);
			if(formDunningEdit.ShowDialog()!=DialogResult.OK) {
				return;
			}
			FillGrids(true);
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Dunning dunning=new Dunning();
			long clinicNum=0;
			if(!comboClinics.IsAllSelected) {
				clinicNum=comboClinics.SelectedClinicNum;
			}
			dunning.ClinicNum=clinicNum;
			using FormDunningEdit formDunningEdit=new FormDunningEdit(dunning);
			formDunningEdit.IsNew=true;
			if(formDunningEdit.ShowDialog()==DialogResult.OK) {
				FillGrids(true);
			}
		}
		
		private void butDuplicate_Click(object sender,EventArgs e) {
			if(gridDunning.SelectedIndices.Count()==0) {
				MsgBox.Show(this,"Please select a message to duplicate first.");
				return;
			}
			Dunning dunning=((Dunning)gridDunning.ListGridRows[gridDunning.GetSelectedIndex()].Tag).Copy();
			Dunnings.Insert(dunning);
			FillGrids(true);
			gridDunning.SetSelected(gridDunning.ListGridRows.OfType<GridRow>().ToList().FindIndex(x => ((Dunning)x.Tag).DunningNum==dunning.DunningNum),true);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
		
	}
}
