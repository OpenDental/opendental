using OpenDental.UI;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPatientRaceEthnicity:FormODBase {
		///<summary>The races for this patient when this form was opened.</summary>
		private List<PatientRace> _listPatientRacesAll;
		///<summary>All races (not ethnicities) from the cdcrec table.</summary>
		private List<Cdcrec> _listCdcrecsRaceAll;
		///<summary>All ethnicities from the cdcrec table.</summary>
		private List<Cdcrec> _listCdcrecsEthnicityAll;
		///<summary>The currently selected races (not ethnicities) for this patient.</summary>
		private List<Cdcrec> _listCdcrecsRacePat;
		///<summary>The currently selected ethnicities for this patient.</summary>
		private List<Cdcrec> _listCdcrecsEthnicityPat;
		private Patient _patient;

		///<summary>The races and ethnicities selected in this window.</summary>
		public List<PatientRace> PatientRaces() {
				List<PatientRace> listPatRaces=new List<PatientRace>();
				List<Cdcrec> _listCdcrecsRacePatUnion=_listCdcrecsRacePat.Union(_listCdcrecsEthnicityPat).ToList();
				for(int i=0;i<_listCdcrecsRacePatUnion.Count;i++) {
					PatientRace patientRace=new PatientRace();
					patientRace.CdcrecCode=_listCdcrecsRacePatUnion[i].CdcrecCode;
					patientRace.PatNum=_patient.PatNum;
					patientRace.Description=_listCdcrecsRacePatUnion[i].Description;
					patientRace.IsEthnicity=(_listCdcrecsRacePatUnion[i].HeirarchicalCode!=null && _listCdcrecsRacePatUnion[i].HeirarchicalCode.StartsWith("E")) 
						|| _listCdcrecsRacePatUnion[i].CdcrecCode==PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE;
					listPatRaces.Add(patientRace);
				}
				return listPatRaces;
		}
		
		public FormPatientRaceEthnicity(Patient patient,List<PatientRace> listPatientRaces) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patient=patient;
			_listPatientRacesAll=listPatientRaces;
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
				Description=Lan.g(this,"DECLINED TO SPECIFY"),
				CdcrecCode=PatientRace.DECLINE_SPECIFY_RACE_CODE,
				HeirarchicalCode=""
			};
			_listCdcrecsRaceAll.Add(cdcrecDeclinedSpecify);
			cdcrecDeclinedSpecify=new Cdcrec {
				Description=Lan.g(this,"DECLINED TO SPECIFY"),
				CdcrecCode=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE,
				HeirarchicalCode=""
			};
			_listCdcrecsEthnicityAll.Add(cdcrecDeclinedSpecify);
			_listCdcrecsRacePat=new List<Cdcrec>();
			_listCdcrecsEthnicityPat=new List<Cdcrec>();
			for(int i=0;i<_listPatientRacesAll.Count;i++) {
				Cdcrec cdcrec=_listCdcrecsRaceAll.FirstOrDefault(x => x.CdcrecCode==_listPatientRacesAll[i].CdcrecCode);
				if(cdcrec!=null) {
					_listCdcrecsRacePat.Add(cdcrec);
				}
				cdcrec=_listCdcrecsEthnicityAll.FirstOrDefault(x => x.CdcrecCode==_listPatientRacesAll[i].CdcrecCode);
				if(cdcrec!=null) {
					_listCdcrecsEthnicityPat.Add(cdcrec);
				}
				if(_listPatientRacesAll[i].CdcrecCode==PatientRace.MULTI_RACE_CODE) {
					cdcrec=new Cdcrec {
						Description=Lan.g(this,"MULTIRACIAL"),
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

		private void FillGrid(GridOD grid,List<Cdcrec> listCdcrecs) {
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"CDCREC Code"),100);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),150);
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
			TreeNode treeNode1=null;
			TreeNode treeNode2=null;
			TreeNode treeNode3=null;
			TreeNode treeNode4=null;
			TreeNode treeNode5=null;
			TreeNode treeNode6=null;
			TreeNode treeNodePrev=null;
			for(int i=0;i<_listCdcrecs.Count;i++) {
				int curNodeLevel=_listCdcrecs[i].HeirarchicalCode.Count(x => x=='.')+1;
				AddNodesRange(curNodeLevel,GetNodeLevel(treeNodePrev),treeView,treeNode1,treeNode2,treeNode3,treeNode4,treeNode5,treeNode6);
				string nodeText=_listCdcrecs[i].Description;
				if(!_listCdcrecs[i].CdcrecCode.StartsWith("ASKU")) {//If not Declined to Specify, include the CdcrecCode.
					nodeText=_listCdcrecs[i].CdcrecCode+" "+nodeText;
				}
				treeNodePrev=new TreeNode(nodeText);//We need to know the previous node so that we can know when we need to add the current node.
				treeNodePrev.Tag=_listCdcrecs[i];
				switch(curNodeLevel) {
					case 1:
						treeNode1=treeNodePrev;
						break;
					case 2:
						treeNode2=treeNodePrev;
						break;
					case 3:
						treeNode3=treeNodePrev;
						break;
					case 4:
						treeNode4=treeNodePrev;
						break;
					case 5:
						treeNode5=treeNodePrev;
						break;
					case 6:
						treeNode6=treeNodePrev;
						break;
				}
			}
			AddNodesRange(0,GetNodeLevel(treeNodePrev),treeView,treeNode1,treeNode2,treeNode3,treeNode4,treeNode5,treeNode6);//Add any nodes that haven't been added.
		}

		///<summary>Adds the current node if it is the same level as the previous node or a higher level than the previous node.</summary>
		private void AddNodesRange(int curNodeLevel,int prevNodeLevel,TreeView treeViewBase,TreeNode treeNode1,TreeNode treeNode2,TreeNode treeNode3,TreeNode treeNode4,
			TreeNode treeNode5,TreeNode treeNode6) 
		{
			if(prevNodeLevel==6) {
				treeNode5.Nodes.Add(treeNode6);
			}
			if(curNodeLevel<6 && prevNodeLevel>4) {
				treeNode4.Nodes.Add(treeNode5);
			}
			if(curNodeLevel<5 && prevNodeLevel>3) {
				treeNode3.Nodes.Add(treeNode4);
			}
			if(curNodeLevel<4 && prevNodeLevel>2) {
				treeNode2.Nodes.Add(treeNode3);
			}
			if(curNodeLevel<3 && prevNodeLevel>1) {
				treeNode1.Nodes.Add(treeNode2);
			}
			if(curNodeLevel<2 && prevNodeLevel>0 && treeNode1!=null) {
				treeViewBase.Nodes.Add(treeNode1);
			}
		}

		///<summary>Gets the nodes level by counting the number of periods in the HierarchicalCode and adding 1. R2 returns 1, R3.002.001 returns 3, etc.</summary>
		private int GetNodeLevel(TreeNode treeNode) {
			if(treeNode==null) {
				return 0;
			}
			return ((Cdcrec)treeNode.Tag).HeirarchicalCode.Count(x => x=='.')+1;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			//Add the selected race node to the Race grid
			TreeNode treeNodeSelected=treeRaces.SelectedNode;
			if(treeNodeSelected!=null && !_listCdcrecsRacePat.Any(x => x.CdcrecCode==((Cdcrec)treeNodeSelected.Tag).CdcrecCode)) {
				_listCdcrecsRacePat.Add((Cdcrec)treeNodeSelected.Tag);
				FillGrid(gridRace,_listCdcrecsRacePat);
			}
			//Add the selected ethnicity node to the Ethnicity grid
			treeNodeSelected=treeEthnicities.SelectedNode;
			if(treeNodeSelected!=null && !_listCdcrecsEthnicityPat.Any(x => x.CdcrecCode==((Cdcrec)treeNodeSelected.Tag).CdcrecCode)) {
				_listCdcrecsEthnicityPat.Add((Cdcrec)treeNodeSelected.Tag);
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

		private void treeRaces_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(!_listCdcrecsRacePat.Any(x => x.CdcrecNum==((Cdcrec)e.Node.Tag).CdcrecNum)) {
				_listCdcrecsRacePat.Add((Cdcrec)e.Node.Tag);
				FillGrid(gridRace,_listCdcrecsRacePat);
			}
		}

		private void treeEthnicities_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(!_listCdcrecsEthnicityPat.Any(x => x.CdcrecNum==((Cdcrec)e.Node.Tag).CdcrecNum)) {
				_listCdcrecsEthnicityPat.Add((Cdcrec)e.Node.Tag);
				FillGrid(gridEthnicity,_listCdcrecsEthnicityPat);
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			//Opens the Code System Importer tool so the user can download the CDCREC codes.
			using FormCodeSystemsImport formCodeSystemsImport=new FormCodeSystemsImport();
			formCodeSystemsImport.ShowDialog();
			FillRaceData();
			FillGrid(gridRace,_listCdcrecsRacePat);
			FillGrid(gridEthnicity,_listCdcrecsEthnicityPat);
			FillTree(treeRaces,_listCdcrecsRaceAll);
			FillTree(treeEthnicities,_listCdcrecsEthnicityAll);
		}

		private void butOK_Click(object sender,EventArgs e) {
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
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}