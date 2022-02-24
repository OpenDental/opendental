using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormSnomeds:FormODBase {
		public bool IsSelectionMode;
		public bool IsMultiSelectMode;
		public Snomed SelectedSnomed;
		public List<Snomed> ListSelectedSnomeds;
		private List<Snomed> SnomedList;
		private bool _showingInfoButton;//used when filling grid. for increased speed.
		private int _showingInfobuttonShift;//used when sorting grid rows. 1 if showing, 0 if hidden

		public FormSnomeds() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSnomeds_Load(object sender,EventArgs e) {
			_showingInfoButton=CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton;
			_showingInfobuttonShift=(_showingInfoButton?1:0);
			if(IsSelectionMode || IsMultiSelectMode) {
				butClose.Text=Lan.g(this,"Cancel");
			}
			else {
				butOK.Visible=false;
			}
			if(IsMultiSelectMode) {
				gridMain.SelectionMode=GridSelectionMode.MultiExtended;
			}
			ActiveControl=textCode;
			//This check is here to prevent Snomeds from being used in non-member nations.
			groupBox1.Visible=false;
			Provider prov=Providers.GetProv(Security.CurUser.ProvNum);
			if(prov==null) {
				return;
			}
			string ehrKey="";
			int yearValue=0;
			List<EhrProvKey> listProvKeys=EhrProvKeys.GetKeysByFLName(prov.LName,prov.FName);
			if(listProvKeys.Count!=0) {
				ehrKey=listProvKeys[0].ProvKey;
				yearValue=listProvKeys[0].YearValue;
			}
			if(FormEHR.ProvKeyIsValid(prov.LName,prov.FName,yearValue,ehrKey)) {
				//EHR has been valid.
				groupBox1.Visible=true;
			}
		}
		
		private void butSearch_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			if(_showingInfoButton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				col=new GridColumn("",18);//infoButton
				col.ImageList=imageListInfoButton;
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"SNOMED CT"),125);//column width of 125 holds the longest Snomed CT code as of 8/7/15 which is 900000000000002006
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn("Deprecated",75,HorizontalAlignment.Center);
			//gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),500);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Used By CQM's"),185);//width 185 so all of our CQM measure nums as of 8/7/15 will fit 68,69,74,75,127,138,147,155,165
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn("Date Of Standard",100);
			//gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			if(textCode.Text.Contains(",")) {
				SnomedList=Snomeds.GetByCodes(textCode.Text);
			}
			else {
				SnomedList=Snomeds.GetByCodeOrDescription(textCode.Text);
			}
			if(SnomedList.Count>=10000) {//Max number of results returned.
				MsgBox.Show(this,"Too many results. Only the first 10,000 results will be shown.");
			}
			List<GridRow> listAll=new List<GridRow>();
			for(int i=0;i<SnomedList.Count;i++) {
				row=new GridRow();
				if(_showingInfoButton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
					row.Cells.Add("0");//index of infobutton
				}
				row.Cells.Add(SnomedList[i].SnomedCode);
				//row.Cells.Add("");//IsActive==NotDeprecated
				row.Cells.Add(SnomedList[i].Description);
				row.Cells.Add(EhrCodes.GetMeasureIdsForCode(SnomedList[i].SnomedCode,"SNOMEDCT"));
				row.Tag=SnomedList[i];
				//row.Cells.Add("");
				listAll.Add(row);
			}
			listAll.Sort(SortMeasuresMet);
			for(int i=0;i<listAll.Count;i++) {
				gridMain.ListGridRows.Add(listAll[i]);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(!CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton) {//Security.IsAuthorized(Permissions.EhrInfoButton,true)) {
				return;
			}
			if(e.Col!=0) {
				return;
			}
			List<KnowledgeRequest> listKnowledgeRequests=EhrTriggers.ConvertToKnowledgeRequests(Snomeds.GetByCode(gridMain.ListGridRows[e.Row].Cells[1].Text));
			using FormInfobutton FormIB=new FormInfobutton(listKnowledgeRequests);
			FormIB.ShowDialog();
		}

		///<summary>Sort function to put the codes that apply to the most number of CQM's at the top so the user can see which codes they should select.</summary>
		private int SortMeasuresMet(GridRow row1,GridRow row2) {
			//int i=(CDSPermissions.GetForUser(Security.CurUser.UserNum).ShowInfobutton?1:0);//used to accomodate infobutton column.
			//First sort by the number of measures the codes apply to in a comma delimited list
			int diff=row2.Cells[2+_showingInfobuttonShift].Text.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries).Length
				-row1.Cells[2+_showingInfobuttonShift].Text.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries).Length;
			if(diff!=0) {
				return diff;
			}
			try {
				//if the codes apply to the same number of CQMs, order by the code values
				//return PIn.Long(row1.Cells[2+_showingInfobuttonShift].Text).CompareTo(PIn.Long(row2.Cells[2+_showingInfobuttonShift].Text));
				//Just string compare
				return row1.Cells[2+_showingInfobuttonShift].Text.CompareTo(row2.Cells[2+_showingInfobuttonShift].Text);
			}
			catch(Exception) {
				return 0;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode || IsMultiSelectMode) {
				SelectedSnomed=(Snomed)gridMain.ListGridRows[e.Row].Tag;
				ListSelectedSnomeds=new List<Snomed>();
				ListSelectedSnomeds.Add((Snomed)gridMain.ListGridRows[e.Row].Tag);
				DialogResult=DialogResult.OK;
				return;
			}
		}

		private void butMapToSnomed_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Will add SNOMED CT code to existing problems list only if the ICD9 code correlates to exactly one SNOMED CT code. If there is any ambiguity at all the code will not be added.")) {
				return;
			}
			int changeCount=0;
			Dictionary<string,string> dictionaryIcd9ToSnomed = Snomeds.GetICD9toSNOMEDDictionary();
			DiseaseDefs.RefreshCache();
			List<DiseaseDef> listDiseaseDefs=DiseaseDefs.GetWhere(x => x.SnomedCode=="" && dictionaryIcd9ToSnomed.ContainsKey(x.ICD9Code));
			foreach(DiseaseDef def in listDiseaseDefs) {
				def.SnomedCode=dictionaryIcd9ToSnomed[def.ICD9Code];
				DiseaseDefs.Update(def);
				changeCount++;
			}
			MessageBox.Show(Lan.g(this,"SNOMED CT codes added: ")+changeCount);
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless IsSelectionMode
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			SelectedSnomed=(Snomed)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			ListSelectedSnomeds=new List<Snomed>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				ListSelectedSnomeds.Add((Snomed)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

	}
}