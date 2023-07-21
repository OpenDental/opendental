using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Drawing;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormInsPlanSubstitution:FormODBase {

		private InsPlan _insPlan=null;
		private List <ProcedureCode> _listProcedureCodes=null;
		///<summary>Used to sync when saving changes.</summary>
		private List <SubstitutionLink> _listSubstitutionLinks=null;
		///<summary>Used to fill the SubstitutionCondition DropDown.</summary>
		private List<string> _listSubConditions;
		///<summary>Holds the string value of the substitution code string before making changes to the cell.</summary>
		private string _oldText;
		///<summary>Holds the sub condition of a substitution link before trying to make any changes</summary>
		private SubstitutionCondition _substitutionCondition;
		///<summary>List of InsPlanSubs to keep track of tags while in this form.</summary>
		private List<InsPlanSubstitution> _listInsPlanSubstitution=new List<InsPlanSubstitution>();

		public FormInsPlanSubstitution(InsPlan insPlan) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_insPlan=insPlan;
		}

		private void FormInsPlanSubstitution_Load(object sender,EventArgs e) {
			_listProcedureCodes=ProcedureCodes.GetAllCodes();
			List<SubstitutionLink> listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(_insPlan.PlanNum);
			_listSubstitutionLinks=ListTools.DeepCopy<SubstitutionLink,SubstitutionLink>(listSubstitutionLinks);
			_listInsPlanSubstitution=FillInsPlanSubList(_listProcedureCodes,listSubstitutionLinks);
			_listSubConditions=new List<string>();
			for(int i=0;i<Enum.GetNames(typeof(SubstitutionCondition)).Length;i++) {
				_listSubConditions.Add(Lan.g("enumSubstitutionCondition",Enum.GetNames(typeof(SubstitutionCondition))[i]));
			}
			FillGridMain();
		}

		///<summary>Takes logic that was previously used to fill the main grid and now fills a list of InsPlanSubstitutions so that we can better keep track of substitution codes created here</summary>
		private List<InsPlanSubstitution> FillInsPlanSubList(List<ProcedureCode> listProcedureCodes,List<SubstitutionLink> listSubstitutionLinks) {
			List<InsPlanSubstitution> listInsPlanSubstitution=new List<InsPlanSubstitution>();
			List<ProcedureCode> listProcedureCodesSubstitution=listProcedureCodes.FindAll(x => !string.IsNullOrEmpty(x.SubstitutionCode));
			for(int i=0;i<listProcedureCodesSubstitution.Count;i++) {
				//procedure code level substitution code aka global substitution
				listInsPlanSubstitution.Add(new InsPlanSubstitution(listProcedureCodesSubstitution[i]));
			}
			//Add all substitution codes for insurance level
			for(int i=0;i<listSubstitutionLinks.Count;i++) {
				ProcedureCode procedureCode=listProcedureCodes.FirstOrDefault(x => x.CodeNum==listSubstitutionLinks[i].CodeNum);
				if(procedureCode==null) {
					continue;//This shouldn't happen.
				}
				//Procedure has a Substitution Link for this insplan.
				listInsPlanSubstitution.Add(new InsPlanSubstitution(procedureCode,listSubstitutionLinks[i]));
			}
			return listInsPlanSubstitution;
		}

		private void FillGridMain() {
			InsPlanSubstitution insPlanSubSelected=gridMain.SelectedTag<InsPlanSubstitution>();
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			if(gridMain.Columns.Count==0) {
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"ProcCode"),90));
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"AbbrDesc"),100));
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"SubstOnlyIf"),100){ListDisplayStrings=_listSubConditions });//Dropdown combobox
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"SubstCode"),90,true));//Can edit cell
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"SubstDesc"),90));
				gridMain.Columns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"InsOnly"),15){ IsWidthDynamic=true });
			}
			//Add all substitution codes for procedure code level
			List<InsPlanSubstitution> listInsPlanSubstitution=_listInsPlanSubstitution.OrderBy(x => x.ProcCode.ProcCode).ToList();
			for(int i=0;i<listInsPlanSubstitution.Count;i++) {
				//If the current insPlanSub is a global substitute, don't add it to the grid if there are insurance plan level substitutes
				if(listInsPlanSubstitution[i].SubLink==null && _listInsPlanSubstitution.Where(x=>x.SubLink!=null).Any(x=>x.ProcCode.CodeNum==listInsPlanSubstitution[i].ProcCode.CodeNum)) {
					continue;
				}
				AddRow(gridMain,listInsPlanSubstitution[i]);
			}
			gridMain.EndUpdate();
			//Try an reselect the procedure code that was already selected prior to refreshing the grid.
			if(insPlanSubSelected!=null) {
				int index=gridMain.ListGridRows.FindIndex(x => (x.Tag as InsPlanSubstitution).ProcCode.CodeNum==insPlanSubSelected.ProcCode.CodeNum);
				if(index > -1) {
					gridMain.SetSelected(new Point(2,index));
				}
			}
		}

		private void AddRow(GridOD grid,InsPlanSubstitution insPlanSubstitution) {
			//Set all of the row values for the procedure code passed in.
			string enumSubstCondition=insPlanSubstitution.SubCondition.ToString();
			string subCode=insPlanSubstitution.ProcCode.SubstitutionCode;
			ProcedureCode procedureCodeSubstitution=_listProcedureCodes.FirstOrDefault(x => x.ProcCode==insPlanSubstitution.ProcCode.SubstitutionCode);
			string subCodeDescript="";
			if(procedureCodeSubstitution!=null){
				subCodeDescript=procedureCodeSubstitution.AbbrDesc; 
			}
			string insOnly="";
			if(insPlanSubstitution.SubLink!=null) {
				//This procedure code has a SubstitutionLink for this insurance plan.
				//set the row values to the Insplan override.
				subCode=insPlanSubstitution.SubLink.SubstitutionCode;
				subCodeDescript="";
				insOnly="X";
				//Validate the subLink.SubstitutionCode. subCodeDescript blank if the substitution code is not valid.
				//User can enter an invalid procedure code if they want.
				if(!string.IsNullOrEmpty(insPlanSubstitution.SubLink.SubstitutionCode)) {
					ProcedureCode procedureCodeSubstitution2=_listProcedureCodes.FirstOrDefault(x=>x.ProcCode==insPlanSubstitution.SubLink.SubstitutionCode);
					if(procedureCodeSubstitution2!=null) {
						subCodeDescript=procedureCodeSubstitution2.AbbrDesc;
					}
				}
			}
			GridRow row=new GridRow();
			row.Cells.Add(insPlanSubstitution.ProcCode.ProcCode);
			row.Cells.Add(insPlanSubstitution.ProcCode.AbbrDesc);
			GridCell cell=new GridCell(Lan.g("enumSubstitutionCondition",enumSubstCondition));
			cell.ComboSelectedIndex=_listSubConditions.FindIndex(x => x==enumSubstCondition);
			row.Cells.Add(cell);
			row.Cells.Add(subCode);
			row.Cells.Add(subCodeDescript);
			row.Cells.Add(insOnly);
			row.Tag=insPlanSubstitution;
			grid.ListGridRows.Add(row);
		}

		///<summary>Opens FormProcCodes in SelectionMode. Creates a new SubstitutionLink for the selected Procedure.</summary>
		private void ButAdd_Click(object sender, EventArgs e){
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			_listProcedureCodes=ProcedureCodes.GetAllCodes();//in case they added a new proc code
			ProcedureCode procSelected=_listProcedureCodes.FirstOrDefault(x=>x.CodeNum==formProcCodes.CodeNumSelected);
			if(procSelected==null) {
				return;//should never happen, just in case
			}
			//Valid procedure selected. Create a new SubstitutionLink.  The user will be able to add the substition code on the cell grid.
			SubstitutionLink substitutionLink=new SubstitutionLink();
			substitutionLink.CodeNum=procSelected.CodeNum;
			substitutionLink.PlanNum=_insPlan.PlanNum;
			substitutionLink.SubstOnlyIf=SubstitutionCondition.Always;
			substitutionLink.SubstitutionCode="";//Set to blank. The user will be able to add in the cell grid
			//The substitution link will be synced on OK click.
			InsPlanSubstitution insPlanSubstitution=new InsPlanSubstitution(procSelected,substitutionLink);
			_listInsPlanSubstitution.Add(insPlanSubstitution);
			FillGridMain();
			//Set the substitution link we just added as selected.
			int index=gridMain.ListGridRows.ToList().FindIndex(x => (x.Tag as InsPlanSubstitution)==insPlanSubstitution);
			if(index>-1) {
				gridMain.SetSelected(new Point(3,index));//Invalidate gets called here which hopefully takes care of further tagging
			}
		}

		///<summary>Changes the SubstOnlyIf after the user selects a new SubstitutionCondition.
		///If the user modifies a procedure level substitution code, a new SubstitutionLink will be added for the inplan(override).</summary>
		private void gridMain_CellSelectionCommitted(object sender,ODGridClickEventArgs e) {
			if(e.Col!=2 || e.Row<0 || e.Row>=gridMain.ListGridRows.Count) {//Return if not SubstOnlyIf column or invalid row
				return;
			}
			//Get the grid tag
			InsPlanSubstitution insPlanSubstitutionOld=gridMain.SelectedTag<InsPlanSubstitution>();
			if(insPlanSubstitutionOld==null) {
				return;
			}
			//Get the selected substitution condition.
			SubstitutionCondition substitutionConditionSelected=(SubstitutionCondition)_listSubConditions.IndexOf(gridMain.ListGridRows[e.Row].Cells[e.Col].Text);
			//procedure code level substitution code
			//Changing the SubstitutionCondition will not update the procedure code level SubstitutionCondition. We will create an insplan override.
			//Create a new SubstitutionLink for ins plan override for this proccode.
			if(_substitutionCondition==substitutionConditionSelected) {
				return;
			}
			if(insPlanSubstitutionOld.SubLink==null) {
				SubstitutionLink substitutionLink=new SubstitutionLink();
				substitutionLink.CodeNum=insPlanSubstitutionOld.ProcCode.CodeNum;
				//SubstitutionCode will be the same as the procedure codes SubstitutionCode since all the user changed was the SubstitutionCondition
				substitutionLink.SubstitutionCode=insPlanSubstitutionOld.ProcCode.SubstitutionCode;
				substitutionLink.PlanNum=_insPlan.PlanNum;
				substitutionLink.SubstOnlyIf=substitutionConditionSelected;
				InsPlanSubstitution insPlanSubstitutionNew=new InsPlanSubstitution(insPlanSubstitutionOld.ProcCode,substitutionLink);
				_listInsPlanSubstitution.Add(insPlanSubstitutionNew);
				FillGridMain();
				return;
			}
			insPlanSubstitutionOld.SubCondition=substitutionConditionSelected;
			insPlanSubstitutionOld.SubLink.SubstOnlyIf=substitutionConditionSelected;
			FillGridMain();
		}

		///<summary>Sets _oldText. Used in gridMain_CellLeave to check whether the text changed when leaving the cell.</summary>
		private void gridMain_CellEnter(object sender,ODGridClickEventArgs e) { 
			_oldText=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
			_substitutionCondition=(SubstitutionCondition)_listSubConditions.IndexOf(gridMain.ListGridRows[e.Row].Cells[e.Col].Text);
		}

		///<summary>Changes the SubstitutionCode to what is entered in the cell. 
		///If the user modifies a procedure level substitution code, a new SubstitutionLink will be added for the inplan(override).
		///The new SubstitutionLink will be added to _listDbSubstLinks</summary>
		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			if(e.Col!=3 || e.Row<0) {//Return if not substitution code column or invalid row
				return;
			}
			InsPlanSubstitution insPlanSubstitutionOld=gridMain.SelectedTag<InsPlanSubstitution>();
			if(insPlanSubstitutionOld==null) {//This could happen if a substonlyif change occurred from the drop down
				return;
			}
			string newText=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
			if(_oldText==newText) {
				return;
			}
			//Substitution code changed. 
			//We will not validate the new substitution code.
			//Get SubstitutionLink for ins level substitution code if one exist.
			if(insPlanSubstitutionOld.SubLink!=null) {//Ins level sub code
				insPlanSubstitutionOld.SubLink.SubstitutionCode=newText;
				FillGridMain();
				return;
			}
			//procedure code level substitution code
			//We will not update the procedure code level substitution code. We will create an insplan override.
			//Create a new SubstitutionLink for ins plan override for this proccode.
			SubstitutionLink substitutionLink=new SubstitutionLink();
			substitutionLink.CodeNum=insPlanSubstitutionOld.ProcCode.CodeNum;
			substitutionLink.SubstitutionCode=newText;
			substitutionLink.PlanNum=_insPlan.PlanNum;
			substitutionLink.SubstOnlyIf=insPlanSubstitutionOld.ProcCode.SubstOnlyIf;
			InsPlanSubstitution insPlanSubstitutionNew=new InsPlanSubstitution(insPlanSubstitutionOld.ProcCode,substitutionLink);
			_listInsPlanSubstitution.Add(insPlanSubstitutionNew);
			FillGridMain();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a substitution code first.");
				return;
			}
			InsPlanSubstitution insPlanSubstitution=gridMain.SelectedTag<InsPlanSubstitution>();
			if(insPlanSubstitution.SubLink==null) {//User selected a procedure code level substitution code. Cannot delete
				MsgBox.Show(this,"Cannot delete a global substitution code.");
				return;
			}
			string msgText=(Lans.g(this,"Delete the selected insurance specific substitution code for procedure ")+"\""+insPlanSubstitution.ProcCode.ProcCode+"\"?");
			if(_listInsPlanSubstitution.Where(x=>x.ProcCode.CodeNum==insPlanSubstitution.ProcCode.CodeNum && x.SubLink!=null).Count()<2 && insPlanSubstitution.ProcCode.SubstitutionCode!="") {
				msgText+="\r\n"+(Lans.g(this,"Deleting the insurance specific substitution code will default to the global substitution code for this procedure")+".");
			}
			if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			_listInsPlanSubstitution.Remove(insPlanSubstitution);
			FillGridMain();
		}

		///<summary>Syncs _listDbSubstLinks and _listDbSubstLinksOld. Does not modify any of the procedure level SubstitutionCodes.</summary>
		private void butOK_Click(object sender,EventArgs e) {
			string msgText=Lan.g(this,"You have chosen to exclude all substitution codes.  "
				+"The checkbox option named 'Don't Substitute Codes (e.g. posterior composites)' "
				+"in the Other Ins Info tab of the Edit Insurance Plan window can be used to exclude all substitution codes.\r\n"
				+"Would you like to enable this option instead of excluding specific codes?");
			bool areAllGlobalSubsOverriddenWithNever=true;
			List<long> listCodeNums=_listInsPlanSubstitution.DistinctBy(x=>x.ProcCode.CodeNum).Select(x=>x.ProcCode.CodeNum).ToList();
			for(int i=0;i<listCodeNums.Count;i++){
				List<InsPlanSubstitution> listInsPlanSubstitution=_listInsPlanSubstitution.FindAll(x=>x.ProcCode.CodeNum==listCodeNums[i]);
					if(listInsPlanSubstitution.All(x=>x.SubLink==null)){ 
						areAllGlobalSubsOverriddenWithNever=false;
						break;
					}
					if(listInsPlanSubstitution.FindAll(x=>x.SubLink!=null).All(x=>x.SubCondition!=SubstitutionCondition.Never)) {
						areAllGlobalSubsOverriddenWithNever=false;
						break;
					}
			}
			if(!_insPlan.CodeSubstNone
				&& areAllGlobalSubsOverriddenWithNever
				&& MessageBox.Show(this,msgText,null,MessageBoxButtons.YesNo)==DialogResult.Yes)
			{
				_insPlan.CodeSubstNone=true;
			}
			if(InsPlanSubstitution.HasDuplicates(_listInsPlanSubstitution)) {
				MsgBox.Show(Lans.g(this,"You have made a duplicate substitutution code and need to fix this before continuing."));
				return;
			}
			List<SubstitutionLink> listSubstitutionLinks=_listInsPlanSubstitution.Where(x=>x.SubLink!=null).Select(x=>x.SubLink).ToList();
			//No need to make changes to the substitution codes at the procedure level.
			//Only syncing insurance level substitution links.
			//All we need to do now is sync the changes made with _listSubstLinksOld.
			SubstitutionLinks.Sync(listSubstitutionLinks,_listSubstitutionLinks);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
	}
}