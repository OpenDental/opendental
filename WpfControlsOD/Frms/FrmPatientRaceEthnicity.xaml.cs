using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmPatientRaceEthnicity:FrmODBase {
		///<summary>The races for this patient when this form was opened. Must be set before opening this frm.</summary>
		public List<PatientRace> ListPatientRacesAll;
		///<summary>All races (not ethnicities) from the cdcrec table.</summary>
		private List<Cdcrec> _listCdcrecsRaceAll;
		///<summary>All ethnicities from the cdcrec table.</summary>
		private List<Cdcrec> _listCdcrecsEthnicityAll;
		///<summary>The currently selected races (not ethnicities) for this patient.</summary>
		private List<Cdcrec> _listCdcrecsRacePat;
		///<summary>The currently selected ethnicities for this patient.</summary>
		private List<Cdcrec> _listCdcrecsEthnicityPat;
		/// <summary>Must be set before opening this frm.</summary>
		public Patient PatientCur;

		///<summary>The races and ethnicities selected in this window.</summary>
		public List<PatientRace> GetPatientRaces() {
				List<PatientRace> listPatRaces=new List<PatientRace>();
				List<Cdcrec> _listCdcrecsRacePatUnion=_listCdcrecsRacePat.Union(_listCdcrecsEthnicityPat).ToList();
				for(int i=0;i<_listCdcrecsRacePatUnion.Count;i++) {
					PatientRace patientRace=new PatientRace();
					patientRace.CdcrecCode=_listCdcrecsRacePatUnion[i].CdcrecCode;
					patientRace.PatNum=PatientCur.PatNum;
					patientRace.Description=_listCdcrecsRacePatUnion[i].Description;
					patientRace.IsEthnicity=(_listCdcrecsRacePatUnion[i].HeirarchicalCode!=null && _listCdcrecsRacePatUnion[i].HeirarchicalCode.StartsWith("E")) 
						|| _listCdcrecsRacePatUnion[i].CdcrecCode==PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE;
					listPatRaces.Add(patientRace);
				}
				return listPatRaces;
		}
		
		public FrmPatientRaceEthnicity() {
			InitializeComponent();
			Lang.F(this);
			Load+=FormPatientRaceEthnicity_Load;
			treeRaces.MouseDoubleClick+=treeRaces_MouseDoubleClick;
			treeEthnicities.MouseDoubleClick+=treeEthnicities_MouseDoubleClick;
			PreviewKeyDown+=FrmPatientRaceEthnicity_PreviewKeyDown;
		}

		private void FormPatientRaceEthnicity_Load(object sender,EventArgs e) {
			FillRaceData();
			FillGrid(gridRace,_listCdcrecsRacePat);
			FillGrid(gridEthnicity,_listCdcrecsEthnicityPat);
			FillTree(treeRaces,_listCdcrecsRaceAll);
			FillTree(treeEthnicities,_listCdcrecsEthnicityAll);
		}

		///<summary>Fills the lists containing all races and all ethnicities from the database and fills the lists containing the races and ethnicities
		///for the patient.</summary>
		private void FillRaceData() {
			List<Cdcrec> listCdcrecs=Cdcrecs.GetAll();
			_listCdcrecsRaceAll=listCdcrecs.FindAll(x => x.HeirarchicalCode.StartsWith("R") && x.HeirarchicalCode!="R")//Race codes start with R. 
				.OrderBy(x => x.HeirarchicalCode).ToList();
			_listCdcrecsEthnicityAll=listCdcrecs.FindAll(x => x.HeirarchicalCode.StartsWith("E") && x.HeirarchicalCode!="E")//Ethnicity codes start with E.
				.OrderBy(x => x.HeirarchicalCode).ToList();
			Cdcrec cdcrecDeclinedSpecify=new Cdcrec {
				Description=Lang.g(this,"DECLINED TO SPECIFY"),
				CdcrecCode=PatientRace.DECLINE_SPECIFY_RACE_CODE,
				HeirarchicalCode=""
			};
			_listCdcrecsRaceAll.Add(cdcrecDeclinedSpecify);
			cdcrecDeclinedSpecify=new Cdcrec {
				Description=Lang.g(this,"DECLINED TO SPECIFY"),
				CdcrecCode=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE,
				HeirarchicalCode=""
			};
			_listCdcrecsEthnicityAll.Add(cdcrecDeclinedSpecify);
			_listCdcrecsRacePat=new List<Cdcrec>();
			_listCdcrecsEthnicityPat=new List<Cdcrec>();
			for(int i=0;i<ListPatientRacesAll.Count;i++) {
				Cdcrec cdcrec=_listCdcrecsRaceAll.FirstOrDefault(x => x.CdcrecCode==ListPatientRacesAll[i].CdcrecCode);
				if(cdcrec!=null) {
					_listCdcrecsRacePat.Add(cdcrec);
				}
				cdcrec=_listCdcrecsEthnicityAll.FirstOrDefault(x => x.CdcrecCode==ListPatientRacesAll[i].CdcrecCode);
				if(cdcrec!=null) {
					_listCdcrecsEthnicityPat.Add(cdcrec);
				}
				if(ListPatientRacesAll[i].CdcrecCode==PatientRace.MULTI_RACE_CODE) {
					cdcrec=new Cdcrec {
						Description=Lang.g(this,"MULTIRACIAL"),
						CdcrecCode=PatientRace.MULTI_RACE_CODE,
						HeirarchicalCode=""
					};
					_listCdcrecsRacePat.Add(cdcrec);
				}
			}
			if(_listCdcrecsRaceAll.Count > 1 && _listCdcrecsEthnicityAll.Count > 1) {//Greater than 1 because we always add 'Declined to Specify'
				labelNeedCodes.Visible=false;
				butImport.Visible=false;
			}
		}

		private void FillGrid(Grid grid,List<Cdcrec> listCdcrecs) {
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lang.g(this,"CDCREC Code"),100);
			grid.Columns.Add(col);
			col=new GridColumn(Lang.g(this,"Description"),150);
			grid.Columns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listCdcrecs.Count;i++) {
				row=new GridRow();
				if(listCdcrecs[i].CdcrecCode.StartsWith("ASKU")
					|| listCdcrecs[i].CdcrecCode==PatientRace.MULTI_RACE_CODE) 
				{
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(listCdcrecs[i].CdcrecCode);
				}
				row.Cells.Add(listCdcrecs[i].Description);
				row.Tag=listCdcrecs[i];
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
		}

		///<summary>Fills the specified tree with the specified list of races. The tree's nodes are based on the HierarchicalCode on the Cdcrec.
		///R1 is at node 1, R1.05 is at node 2, R1.05.009 is at node 3, etc.</summary>
		private void FillTree(TreeView treeView,List<Cdcrec> _listCdcrecs) {
			TreeViewItem treeViewItem1=null;
			TreeViewItem treeViewItem2=null;
			TreeViewItem treeViewItem3=null;
			TreeViewItem treeViewItem4=null;
			TreeViewItem treeViewItem5=null;
			TreeViewItem treeViewItem6=null;
			TreeViewItem treeViewItemPrev=null;
			for(int i=0;i<_listCdcrecs.Count;i++) {
				int curItemLevel=_listCdcrecs[i].HeirarchicalCode.Count(x => x=='.')+1;
				AddNodesRange(curItemLevel,GetNodeLevel(treeViewItemPrev),treeView,treeViewItem1,treeViewItem2,treeViewItem3,treeViewItem4,treeViewItem5,treeViewItem6);
				string treeViewItemText=_listCdcrecs[i].Description;
				if(!_listCdcrecs[i].CdcrecCode.StartsWith("ASKU")) {//If not Declined to Specify, include the CdcrecCode.
					treeViewItemText=_listCdcrecs[i].CdcrecCode+" "+treeViewItemText;
				}
				treeViewItemPrev=new TreeViewItem();//We need to know the previous node so that we can know when we need to add the current node.
				treeViewItemPrev.Text=treeViewItemText;
				treeViewItemPrev.Tag=_listCdcrecs[i];
				switch(curItemLevel) {
					case 1:
						treeViewItem1=treeViewItemPrev;
						break;
					case 2:
						treeViewItem2=treeViewItemPrev;
						break;
					case 3:
						treeViewItem3=treeViewItemPrev;
						break;
					case 4:
						treeViewItem4=treeViewItemPrev;
						break;
					case 5:
						treeViewItem5=treeViewItemPrev;
						break;
					case 6:
						treeViewItem6=treeViewItemPrev;
						break;
				}
			}
			AddNodesRange(0,GetNodeLevel(treeViewItemPrev),treeView,treeViewItem1,treeViewItem2,treeViewItem3,treeViewItem4,treeViewItem5,treeViewItem6);//Add any nodes that haven't been added.
		}

		///<summary>Adds the current node if it is the same level as the previous node or a higher level than the previous node.</summary>
		private void AddNodesRange(int curItemLevel,int prevItemLevel,TreeView treeViewBase,TreeViewItem treeViewItem1,TreeViewItem treeViewItem2,TreeViewItem treeViewItem3,TreeViewItem treeViewItem4,TreeViewItem treeViewItem5,TreeViewItem treeViewItem6) {
			if(prevItemLevel==6) {
				treeViewItem5.Items.Add(treeViewItem6);
			}
			if(curItemLevel<6 && prevItemLevel>4) {
				treeViewItem4.Items.Add(treeViewItem5);
			}
			if(curItemLevel<5 && prevItemLevel>3) {
				treeViewItem3.Items.Add(treeViewItem4);
			}
			if(curItemLevel<4 && prevItemLevel>2) {
				treeViewItem2.Items.Add(treeViewItem3);
			}
			if(curItemLevel<3 && prevItemLevel>1) {
				treeViewItem1.Items.Add(treeViewItem2);
			}
			if(curItemLevel<2 && prevItemLevel>0 && treeViewItem1!=null) {
				treeViewBase.Items.Add(treeViewItem1);
			}
		}

		///<summary>Gets the nodes level by counting the number of periods in the HierarchicalCode and adding 1. R2 returns 1, R3.002.001 returns 3, etc.</summary>
		private int GetNodeLevel(TreeViewItem treeViewItem) {
			if(treeViewItem==null) {
				return 0;
			}
			return ((Cdcrec)treeViewItem.Tag).HeirarchicalCode.Count(x => x=='.')+1;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			//Add the selected race node to the Race grid
			TreeViewItem treeViewItemSelected=treeRaces.SelectedItem;
			if(treeViewItemSelected!=null && !_listCdcrecsRacePat.Any(x => x.CdcrecCode==((Cdcrec)treeViewItemSelected.Tag).CdcrecCode)) {
				_listCdcrecsRacePat.Add((Cdcrec)treeViewItemSelected.Tag);
				FillGrid(gridRace,_listCdcrecsRacePat);
			}
			//Add the selected ethnicity node to the Ethnicity grid
			treeViewItemSelected=treeEthnicities.SelectedItem;
			if(treeViewItemSelected!=null && !_listCdcrecsEthnicityPat.Any(x => x.CdcrecCode==((Cdcrec)treeViewItemSelected.Tag).CdcrecCode)) {
				_listCdcrecsEthnicityPat.Add((Cdcrec)treeViewItemSelected.Tag);
				FillGrid(gridEthnicity,_listCdcrecsEthnicityPat);
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			//Remove the selected rows in the Race grid
			for(int i=0;i<gridRace.SelectedIndices.Length;i++) {
				Cdcrec cdcrecRace=(Cdcrec)gridRace.ListGridRows[gridRace.SelectedIndices[i]].Tag;
				_listCdcrecsRacePat.RemoveAll(x => x.CdcrecCode==cdcrecRace.CdcrecCode);
			}
			FillGrid(gridRace,_listCdcrecsRacePat);
			//Remove the selected rows in the Ethnicities grid
			for(int i=0;i<gridEthnicity.SelectedIndices.Length;i++) {
				Cdcrec cdcrecEthnicity=(Cdcrec)gridEthnicity.ListGridRows[gridEthnicity.SelectedIndices[i]].Tag;
				_listCdcrecsEthnicityPat.RemoveAll(x => x.CdcrecCode==cdcrecEthnicity.CdcrecCode);
			}
			FillGrid(gridEthnicity,_listCdcrecsEthnicityPat);
		}

		private void treeRaces_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			TreeViewItem treeViewItemSelected=treeRaces.SelectedItem;
			if(treeViewItemSelected!=null && !_listCdcrecsRacePat.Any(x => x.CdcrecCode==((Cdcrec)treeViewItemSelected.Tag).CdcrecCode)){
				_listCdcrecsRacePat.Add((Cdcrec)treeViewItemSelected.Tag);
				FillGrid(gridRace,_listCdcrecsRacePat);
			}
		}

		private void treeEthnicities_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			TreeViewItem treeViewItemSelected=treeEthnicities.SelectedItem;
			if(treeViewItemSelected!=null && !_listCdcrecsEthnicityPat.Any(x => x.CdcrecCode==((Cdcrec)treeViewItemSelected.Tag).CdcrecCode)) {
				_listCdcrecsEthnicityPat.Add((Cdcrec)treeViewItemSelected.Tag);
				FillGrid(gridEthnicity,_listCdcrecsEthnicityPat);
			}
		}

		private void FrmPatientRaceEthnicity_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
			if(butImport.IsAltKey(Key.I,e)) {
				butImport_Click(this,new EventArgs());
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			//Opens the Code System Importer tool so the user can download the CDCREC codes.
			FormLauncher formLauncher=new FormLauncher(EnumFormName.FormCodeSystemsImport);
			formLauncher.ShowDialog();
			FillRaceData();
			FillGrid(gridRace,_listCdcrecsRacePat);
			FillGrid(gridEthnicity,_listCdcrecsEthnicityPat);
			FillTree(treeRaces,_listCdcrecsRaceAll);
			FillTree(treeEthnicities,_listCdcrecsEthnicityAll);
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(_listCdcrecsRacePat.Count>1 && _listCdcrecsRacePat.Any(x => x.CdcrecCode==PatientRace.DECLINE_SPECIFY_RACE_CODE)) {
				MsgBox.Show(this,"Cannot select 'DECLINED TO SPECIFY' and any other race.");
				return;
			}
			if(_listCdcrecsEthnicityPat.Count>1 && _listCdcrecsEthnicityPat.Any(x => x.CdcrecCode==PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE)) {
				MsgBox.Show(this,"Cannot select 'DECLINED TO SPECIFY' and any other ethnicity.");
				return;
			}
			if(_listCdcrecsEthnicityPat.Count>1 && _listCdcrecsEthnicityPat.Any(x => x.CdcrecCode=="2186-5")) {
				MsgBox.Show(this,"Cannot select 'NOT HISPANIC OR LATINO' and a Hispanic or Latino ethnicity.");
				return;
			}
			//Data is not saved to the database in this form.
			IsDialogOK=true;
		}

	}
}