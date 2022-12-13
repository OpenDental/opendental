using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormHL7Defs:FormODBase {
		private List<HL7Def> _listHL7DefsInternal;
		private List<HL7Def> _listHL7DefsCustom;
		///<summary>This gets set externally beforehand.  This is passed to FormHL7Msgs for loading the HL7 messages for the currently selected patient.</summary>
		public long PatNumCur;

		///<summary></summary>
		public FormHL7Defs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormHL7Defs_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup,true)){
				butCopy.Enabled=false;
				grid2.Enabled=false;
				grid1.Enabled=false;
			}
			FillGrid1();
			FillGrid2();			
		}

		private void FillGrid1() {
			//Our strategy in this window and all sub windows is to get all data directly from the database (or internal).
			_listHL7DefsInternal=HL7Defs.GetDeepInternalList();
			grid1.BeginUpdate();
			grid1.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),100);
			grid1.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Mode"),40);
			grid1.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"In Folder / Socket"),130);
			grid1.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Out Folder / Socket"),130);
			grid1.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Enabled"),35);
			grid1.Columns.Add(col);
			grid1.ListGridRows.Clear();
			for(int i=0;i<_listHL7DefsInternal.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(_listHL7DefsInternal[i].Description);
				row.Cells.Add(Lan.g("enumModeTxHL7",_listHL7DefsInternal[i].ModeTx.ToString()));
				if(_listHL7DefsInternal[i].ModeTx==ModeTxHL7.File) {
					row.Cells.Add(_listHL7DefsInternal[i].IncomingFolder);
					row.Cells.Add(_listHL7DefsInternal[i].OutgoingFolder);
				}
				else if(_listHL7DefsInternal[i].ModeTx==ModeTxHL7.TcpIp) {
					row.Cells.Add(_listHL7DefsInternal[i].HL7Server+":"+_listHL7DefsInternal[i].IncomingPort);
					row.Cells.Add(_listHL7DefsInternal[i].OutgoingIpPort);
				}
				else {//Sftp
					row.Cells.Add(_listHL7DefsInternal[i].SftpInSocket);
					row.Cells.Add("N/A");
				}
				row.Cells.Add(_listHL7DefsInternal[i].IsEnabled?"X":"");
				grid1.ListGridRows.Add(row);
			}
			grid1.EndUpdate();
		}

		private void FillGrid2() {
			//Our strategy in this window and all sub windows is to get all data directly from the database.
			//If it's too slow in this window due to the 20-30 database calls per row in grid2, then we might later optimize to pull from the cache.
			_listHL7DefsCustom=HL7Defs.GetDeepCustomList();
			grid2.BeginUpdate();
			grid2.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Description"),100);
			grid2.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Mode"),40);
			grid2.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"In Folder / Socket"),130);
			grid2.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Out Folder / Socket"),130);
			grid2.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Enabled"),35);
			grid2.Columns.Add(col);
			grid2.ListGridRows.Clear();
			for(int i=0;i<_listHL7DefsCustom.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(_listHL7DefsCustom[i].Description);
				row.Cells.Add(Lan.g("enumModeTxHL7",_listHL7DefsCustom[i].ModeTx.ToString()));
				if(_listHL7DefsCustom[i].ModeTx==ModeTxHL7.File) {
					row.Cells.Add(_listHL7DefsCustom[i].IncomingFolder);
					row.Cells.Add(_listHL7DefsCustom[i].OutgoingFolder);
				}
				else if(_listHL7DefsCustom[i].ModeTx==ModeTxHL7.TcpIp) {
					row.Cells.Add(_listHL7DefsCustom[i].HL7Server+":"+_listHL7DefsCustom[i].IncomingPort);
					row.Cells.Add(_listHL7DefsCustom[i].OutgoingIpPort);
				}
				else {//Sftp
					row.Cells.Add(_listHL7DefsCustom[i].SftpInSocket);
					row.Cells.Add("N/A");
				}
				row.Cells.Add(_listHL7DefsCustom[i].IsEnabled?"X":"");
				grid2.ListGridRows.Add(row);
			}
			grid2.EndUpdate();
		}

		private void butDuplicate_Click(object sender,System.EventArgs e) {
			if(grid2.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a Custom HL7Def from the list on the right first.");
				return;
			}
			HL7Def hL7def=_listHL7DefsCustom[grid2.GetSelectedIndex()].Clone();
			hL7def.IsEnabled=false;
			long hL7DefNum=HL7Defs.Insert(hL7def);
			for(int m=0;m<hL7def.hl7DefMessages.Count;m++) {
				hL7def.hl7DefMessages[m].HL7DefNum=hL7DefNum;
				long hL7DefMessageNum=HL7DefMessages.Insert(hL7def.hl7DefMessages[m]);
				for(int s=0;s<hL7def.hl7DefMessages[m].hl7DefSegments.Count;s++) {
					hL7def.hl7DefMessages[m].hl7DefSegments[s].HL7DefMessageNum=hL7DefMessageNum;
					long hL7DefSegmentNum=HL7DefSegments.Insert(hL7def.hl7DefMessages[m].hl7DefSegments[s]);
					for(int f=0;f<hL7def.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields.Count;f++) {
						hL7def.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields[f].HL7DefSegmentNum=hL7DefSegmentNum;
						HL7DefFields.Insert(hL7def.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields[f]);
					}
				}
			}
			DataValid.SetInvalid(InvalidType.HL7Defs);
			FillGrid2();
			grid2.SetAll(false);
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(grid1.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an internal HL7Def from the list on the left first.");
				return;
			}
			HL7Def hl7def=_listHL7DefsInternal[grid1.GetSelectedIndex()].Clone();
			hl7def.IsInternal=false;
			hl7def.IsEnabled=false;
			long hL7DefNum=HL7Defs.Insert(hl7def);
			for(int m=0;m<hl7def.hl7DefMessages.Count;m++) {
				hl7def.hl7DefMessages[m].HL7DefNum=hL7DefNum;
				long hL7DefMessageNum=HL7DefMessages.Insert(hl7def.hl7DefMessages[m]);
				for(int s=0;s<hl7def.hl7DefMessages[m].hl7DefSegments.Count;s++) {
					hl7def.hl7DefMessages[m].hl7DefSegments[s].HL7DefMessageNum=hL7DefMessageNum;
					long hL7DefSegmentNum=HL7DefSegments.Insert(hl7def.hl7DefMessages[m].hl7DefSegments[s]);
					for(int f=0;f<hl7def.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields.Count;f++) {
						hl7def.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields[f].HL7DefSegmentNum=hL7DefSegmentNum;
						HL7DefFields.Insert(hl7def.hl7DefMessages[m].hl7DefSegments[s].hl7DefFields[f]);
					}
				}
			}
			DataValid.SetInvalid(InvalidType.HL7Defs);
			FillGrid2();
			grid1.SetAll(false);
		}

		private void butHistory_Click(object sender,EventArgs e) {
			using FormHL7Msgs formHL7Msgs=new FormHL7Msgs();
			formHL7Msgs.PatNumCur=PatNumCur;
			formHL7Msgs.ShowDialog();
			FillGrid1();
			FillGrid2();
		}

		private void grid1_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormHL7DefEdit formHL7DefEdit=new FormHL7DefEdit();
			formHL7DefEdit.HL7DefCur=_listHL7DefsInternal[e.Row];
			formHL7DefEdit.ShowDialog();
			FillGrid1();
			FillGrid2();	
		}

		private void grid2_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormHL7DefEdit formHL7DefEdit=new FormHL7DefEdit();
			formHL7DefEdit.HL7DefCur=_listHL7DefsCustom[e.Row];
			formHL7DefEdit.ShowDialog();
			FillGrid1();
			FillGrid2();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormHL7Defs_FormClosing(object sender,FormClosingEventArgs e) {
			DataValid.SetInvalid(InvalidType.HL7Defs);
			DataValid.SetInvalid(InvalidType.Prefs);
			DataValid.SetInvalid(InvalidType.ToolButsAndMounts);
		}

	}
}



