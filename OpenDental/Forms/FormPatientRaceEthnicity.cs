using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormPatientRaceEthnicity:FormODBase {
		///<summary>The races for this patient when this form was opened.</summary>
		private List<PatientRace> _listAllPatientRaces;
		///<summary>All races (not ethnicities) from the cdcrec table.</summary>
		private List<Cdcrec> _listAllRaces;
		///<summary>All ethnicities from the cdcrec table.</summary>
		private List<Cdcrec> _listAllEthnicities;
		///<summary>The currently selected races (not ethnicities) for this patient.</summary>
		private List<Cdcrec> _listPatRaces;
		///<summary>The currently selected ethnicities for this patient.</summary>
		private List<Cdcrec> _listPatEthnicities;
		private Patient _patCur;

		///<summary>The races and ethnicities selected in this window.</summary>
		public List<PatientRace> PatientRaces {
			get {
				List<PatientRace> listPatRaces=new List<PatientRace>();
				foreach(Cdcrec cdcrec in _listPatRaces.Union(_listPatEthnicities)) {
					PatientRace patRace=new PatientRace();
					patRace.CdcrecCode=cdcrec.CdcrecCode;
					patRace.PatNum=_patCur.PatNum;
					patRace.Description=cdcrec.Description;
					patRace.IsEthnicity=(cdcrec.HeirarchicalCode!=null && cdcrec.HeirarchicalCode.StartsWith("E")) 
						|| cdcrec.CdcrecCode==PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE;
					listPatRaces.Add(patRace);
				}
				return listPatRaces;
			}
		}
		
		public FormPatientRaceEthnicity(Patient pat,List<PatientRace> listPatientRaces) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patCur=pat;
			_listAllPatientRaces=listPatientRaces;
		}

		private void FormPatientRaceEthnicity_Load(object sender,EventArgs e) {
			FillRaceData();
			FillGrid(gridRace,_listPatRaces);
			FillGrid(gridEthnicity,_listPatEthnicities);
			FillTree(treeRaces,_listAllRaces);
			FillTree(treeEthnicities,_listAllEthnicities);
		}

		///<summary>Fills the lists containing all races and all ethnicities from the database and fills the lists containing the races and ethnicities
		///for the patient.</summary>
		private void FillRaceData() {
			List<Cdcrec> listCdcrecs=Cdcrecs.GetAll();
			_listAllRaces=listCdcrecs.FindAll(x => x.HeirarchicalCode.StartsWith("R") && x.HeirarchicalCode!="R")//Race codes start with R. 
				.OrderBy(x => x.HeirarchicalCode).ToList();
			_listAllEthnicities=listCdcrecs.FindAll(x => x.HeirarchicalCode.StartsWith("E") && x.HeirarchicalCode!="E")//Ethnicity codes start with E.
				.OrderBy(x => x.HeirarchicalCode).ToList();
			Cdcrec declinedSpecify=new Cdcrec {
				Description=Lan.g(this,"DECLINED TO SPECIFY"),
				CdcrecCode=PatientRace.DECLINE_SPECIFY_RACE_CODE,
				HeirarchicalCode=""
			};
			_listAllRaces.Add(declinedSpecify);
			declinedSpecify=new Cdcrec {
				Description=Lan.g(this,"DECLINED TO SPECIFY"),
				CdcrecCode=PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE,
				HeirarchicalCode=""
			};
			_listAllEthnicities.Add(declinedSpecify);
			_listPatRaces=new List<Cdcrec>();
			_listPatEthnicities=new List<Cdcrec>();
			foreach(PatientRace patRace in _listAllPatientRaces) {
				Cdcrec cdcrec=_listAllRaces.FirstOrDefault(x => x.CdcrecCode==patRace.CdcrecCode);
				if(cdcrec!=null) {
					_listPatRaces.Add(cdcrec);
				}
				cdcrec=_listAllEthnicities.FirstOrDefault(x => x.CdcrecCode==patRace.CdcrecCode);
				if(cdcrec!=null) {
					_listPatEthnicities.Add(cdcrec);
				}
				if(patRace.CdcrecCode==PatientRace.MULTI_RACE_CODE) {
					cdcrec=new Cdcrec {
						Description=Lan.g(this,"MULTIRACIAL"),
						CdcrecCode=PatientRace.MULTI_RACE_CODE,
						HeirarchicalCode=""
					};
					_listPatRaces.Add(cdcrec);
				}
			}
			if(_listAllRaces.Count > 1 && _listAllEthnicities.Count > 1) {//Greater than 1 because we always add 'Declined to Specify'
				labelNeedCodes.Visible=false;
				butImport.Visible=false;
			}
		}

		private void FillGrid(GridOD grid,List<Cdcrec> listCdcRecs) {
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"CDCREC Code"),100);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),150);
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			foreach(Cdcrec cdcrec in listCdcRecs) {
				row=new GridRow();
				if(cdcrec.CdcrecCode.StartsWith("ASKU")
					|| cdcrec.CdcrecCode==PatientRace.MULTI_RACE_CODE) 
				{
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(cdcrec.CdcrecCode);
				}
				row.Cells.Add(cdcrec.Description);
				row.Tag=cdcrec;
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
		}

		///<summary>Fills the specified tree with the specified list of races. The tree's nodes are based on the HierarchicalCode on the Cdcrec.
		///R1 is at node 1, R1.05 is at node 2, R1.05.009 is at node 3, etc.</summary>
		private void FillTree(TreeView treeView,List<Cdcrec> _listCdcrecs) {
			TreeNode node1=null;
			TreeNode node2=null;
			TreeNode node3=null;
			TreeNode node4=null;
			TreeNode node5=null;
			TreeNode node6=null;
			TreeNode prevNode=null;
			foreach(Cdcrec cdcrec in _listCdcrecs) {
				int curNodeLevel=cdcrec.HeirarchicalCode.Count(x => x=='.')+1;
				AddNodesRange(curNodeLevel,GetNodeLevel(prevNode),treeView,node1,node2,node3,node4,node5,node6);
				string nodeText=cdcrec.Description;
				if(!cdcrec.CdcrecCode.StartsWith("ASKU")) {//If not Declined to Specify, include the CdcrecCode.
					nodeText=cdcrec.CdcrecCode+" "+nodeText;
				}
				prevNode=new TreeNode(nodeText);//We need to know the previous node so that we can know when we need to add the current node.
				prevNode.Tag=cdcrec;
				switch(curNodeLevel) {
					case 1:
						node1=prevNode;
						break;
					case 2:
						node2=prevNode;
						break;
					case 3:
						node3=prevNode;
						break;
					case 4:
						node4=prevNode;
						break;
					case 5:
						node5=prevNode;
						break;
					case 6:
						node6=prevNode;
						break;
				}
			}
			AddNodesRange(0,GetNodeLevel(prevNode),treeView,node1,node2,node3,node4,node5,node6);//Add any nodes that haven't been added.
		}

		///<summary>Adds the current node if it is the same level as the previous node or a higher level than the previous node.</summary>
		private void AddNodesRange(int curNodeLevel,int prevNodeLevel,TreeView treeBase,TreeNode node1,TreeNode node2,TreeNode node3,TreeNode node4,
			TreeNode node5,TreeNode node6) 
		{
			if(prevNodeLevel==6) {
				node5.Nodes.Add(node6);
			}
			if(curNodeLevel<6 && prevNodeLevel>4) {
				node4.Nodes.Add(node5);
			}
			if(curNodeLevel<5 && prevNodeLevel>3) {
				node3.Nodes.Add(node4);
			}
			if(curNodeLevel<4 && prevNodeLevel>2) {
				node2.Nodes.Add(node3);
			}
			if(curNodeLevel<3 && prevNodeLevel>1) {
				node1.Nodes.Add(node2);
			}
			if(curNodeLevel<2 && prevNodeLevel>0 && node1!=null) {
				treeBase.Nodes.Add(node1);
			}
		}

		///<summary>Gets the nodes level by counting the number of periods in the HierarchicalCode and adding 1. R2 returns 1, R3.002.001 returns 3, etc.</summary>
		private int GetNodeLevel(TreeNode node) {
			if(node==null) {
				return 0;
			}
			return ((Cdcrec)node.Tag).HeirarchicalCode.Count(x => x=='.')+1;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			//Add the selected race node to the Race grid
			TreeNode selectedNode=treeRaces.SelectedNode;
			if(selectedNode!=null && !_listPatRaces.Any(x => x.CdcrecCode==((Cdcrec)selectedNode.Tag).CdcrecCode)) {
				_listPatRaces.Add((Cdcrec)selectedNode.Tag);
				FillGrid(gridRace,_listPatRaces);
			}
			//Add the selected ethnicity node to the Ethnicity grid
			selectedNode=treeEthnicities.SelectedNode;
			if(selectedNode!=null && !_listPatEthnicities.Any(x => x.CdcrecCode==((Cdcrec)selectedNode.Tag).CdcrecCode)) {
				_listPatEthnicities.Add((Cdcrec)selectedNode.Tag);
				FillGrid(gridEthnicity,_listPatEthnicities);
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			//Remove the selected rows in the Race grid
			foreach(int selectedIndex in gridRace.SelectedIndices) {
				Cdcrec race=(Cdcrec)gridRace.ListGridRows[selectedIndex].Tag;
				_listPatRaces.RemoveAll(x => x.CdcrecCode==race.CdcrecCode);
			}
			FillGrid(gridRace,_listPatRaces);
			//Remove the selected rows in the Ethnicities grid
			foreach(int selectedIndex in gridEthnicity.SelectedIndices) {
				Cdcrec ethnicity=(Cdcrec)gridEthnicity.ListGridRows[selectedIndex].Tag;
				_listPatEthnicities.RemoveAll(x => x.CdcrecCode==ethnicity.CdcrecCode);
			}
			FillGrid(gridEthnicity,_listPatEthnicities);
		}

		private void treeRaces_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(!_listPatRaces.Any(x => x.CdcrecNum==((Cdcrec)e.Node.Tag).CdcrecNum)) {
				_listPatRaces.Add((Cdcrec)e.Node.Tag);
				FillGrid(gridRace,_listPatRaces);
			}
		}

		private void treeEthnicities_NodeMouseDoubleClick(object sender,TreeNodeMouseClickEventArgs e) {
			if(!_listPatEthnicities.Any(x => x.CdcrecNum==((Cdcrec)e.Node.Tag).CdcrecNum)) {
				_listPatEthnicities.Add((Cdcrec)e.Node.Tag);
				FillGrid(gridEthnicity,_listPatEthnicities);
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			//Opens the Code System Importer tool so the user can download the CDCREC codes.
			using FormCodeSystemsImport FormCSI=new FormCodeSystemsImport();
			FormCSI.ShowDialog();
			FillRaceData();
			FillGrid(gridRace,_listPatRaces);
			FillGrid(gridEthnicity,_listPatEthnicities);
			FillTree(treeRaces,_listAllRaces);
			FillTree(treeEthnicities,_listAllEthnicities);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_listPatRaces.Count>1 && _listPatRaces.Any(x => x.CdcrecCode==PatientRace.DECLINE_SPECIFY_RACE_CODE)) {
				MsgBox.Show(this,"Cannot select 'DECLINED TO SPECIFY' and any other race.");
				return;
			}
			if(_listPatEthnicities.Count>1 && _listPatEthnicities.Any(x => x.CdcrecCode==PatientRace.DECLINE_SPECIFY_ETHNICITY_CODE)) {
				MsgBox.Show(this,"Cannot select 'DECLINED TO SPECIFY' and any other ethnicity.");
				return;
			}
			if(_listPatEthnicities.Count>1 && _listPatEthnicities.Any(x => x.CdcrecCode=="2186-5")) {
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