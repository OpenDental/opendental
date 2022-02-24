using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormInsFilingCodes:FormODBase {
		private bool changed;
		public bool IsSelectionMode;
		///<summary>Only used if IsSelectionMode.  On OK, contains selected InsFilingCodeNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		public long SelectedInsFilingCodeNum;
		private List<InsFilingCode> _listInsFilingCodes;

		///<summary></summary>
		public FormInsFilingCodes()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsFilingCodes_Load(object sender,EventArgs e) {
			if(IsSelectionMode) {
				butClose.Text=Lan.g(this,"Cancel");
			} 
			else {
				butOK.Visible=false;
				butNone.Visible=false;
			}
			_listInsFilingCodes=InsFilingCodes.GetDeepCopy();
			//synch the itemorders just in case
			for(int i=0;i<_listInsFilingCodes.Count;i++) {
			  if(_listInsFilingCodes[i].ItemOrder!=i) {
			    _listInsFilingCodes[i].ItemOrder=i;
			    InsFilingCodes.Update(_listInsFilingCodes[i]);
			    changed=true;
			  }
			}
			FillGrid();
			if(SelectedInsFilingCodeNum!=0) {
				for(int i=0;i<_listInsFilingCodes.Count;i++) {
					if(_listInsFilingCodes[i].InsFilingCodeNum==SelectedInsFilingCodeNum) {
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void FillGrid(){
			InsFilingCodes.RefreshCache();
			_listInsFilingCodes=InsFilingCodes.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableInsFilingCodes","Description"),250);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsFilingCodes","Group"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableInsFilingCodes","EclaimCode"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listInsFilingCodes.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listInsFilingCodes[i].Descript);
				string group="";
				if(_listInsFilingCodes[i].GroupType > 0) {
					group=Defs.GetDef(DefCat.InsuranceFilingCodeGroup,_listInsFilingCodes[i].GroupType)?.ItemName??"";
				}
				row.Cells.Add(group);
				row.Cells.Add(_listInsFilingCodes[i].EclaimCode);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormInsFilingCodeEdit FormIE=new FormInsFilingCodeEdit();
			FormIE.InsFilingCodeCur=new InsFilingCode();
			FormIE.InsFilingCodeCur.ItemOrder=_listInsFilingCodes.Count;
			FormIE.InsFilingCodeCur.IsNew=true;
			FormIE.ShowDialog();
			FillGrid();
			changed=true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode){
				SelectedInsFilingCodeNum=_listInsFilingCodes[e.Row].InsFilingCodeNum;
				DialogResult=DialogResult.OK;
				return;
			}
			else{
				using FormInsFilingCodeEdit FormI=new FormInsFilingCodeEdit();
				FormI.InsFilingCodeCur=_listInsFilingCodes[e.Row];
				FormI.ShowDialog();
				FillGrid();
				changed=true;
			}
		}

		private void butNone_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			SelectedInsFilingCodeNum=0;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			if(gridMain.GetSelectedIndex()==-1){
				SelectedInsFilingCodeNum=0;
			}
			else{
				SelectedInsFilingCodeNum=_listInsFilingCodes[gridMain.GetSelectedIndex()].InsFilingCodeNum;
			}
			DialogResult=DialogResult.OK;
		}

		private void butUp_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1) {
				MsgBox.Show(this,"Please select an insurance filing code first.");
				return;
			}
			if(idx==0) {
				return;
			}
			//swap the orders.
			int order1=_listInsFilingCodes[idx-1].ItemOrder;
			int order2=_listInsFilingCodes[idx].ItemOrder;
			_listInsFilingCodes[idx-1].ItemOrder=order2;
			InsFilingCodes.Update(_listInsFilingCodes[idx-1]);
			_listInsFilingCodes[idx].ItemOrder=order1;
			InsFilingCodes.Update(_listInsFilingCodes[idx]);
			changed=true;
			FillGrid();
			gridMain.SetSelected(idx-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1) {
				MsgBox.Show(this,"Please select an insurance filing code first.");
				return;
			}
			if(idx==_listInsFilingCodes.Count-1) {
				return;
			}
			int order1=_listInsFilingCodes[idx].ItemOrder;
			int order2=_listInsFilingCodes[idx+1].ItemOrder;
			_listInsFilingCodes[idx].ItemOrder=order2;
			InsFilingCodes.Update(_listInsFilingCodes[idx]);
			_listInsFilingCodes[idx+1].ItemOrder=order1;
			InsFilingCodes.Update(_listInsFilingCodes[idx+1]);
			changed=true;
			FillGrid();
			gridMain.SetSelected(idx+1,true);
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

		private void FormInsFilingCodes_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed) {
				DataValid.SetInvalid(InvalidType.InsFilingCodes);
			}
		}
		
	}
}





















