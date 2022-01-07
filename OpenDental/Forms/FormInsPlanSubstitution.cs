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
		private List <ProcedureCode> _listAllProcCodes=null;
		///<summary>Used to sync when saving changes.</summary>
		private List <SubstitutionLink> _listSubstLinksOld=null;
		///<summary>Used to fill the SubstitutionCondition DropDown.</summary>
		private List<string> _listSubConditions;
		///<summary>Holds the string value of the substitution code string before making changes to the cell.</summary>
		private string _oldText;
		///<summary>Holds the sub condition of a substitution link before trying to make any changes</summary>
		private SubstitutionCondition _oldCondition;
		///<summary>List of InsPlanSubs to keep track of tags while in this form.</summary>
		private List<InsPlanSubstitution> _listInsPlanSubs=new List<InsPlanSubstitution>();

		public FormInsPlanSubstitution(InsPlan insPlan) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_insPlan=insPlan;
		}

		private void FormInsPlanSubstitution_Load(object sender,EventArgs e) {
			_listAllProcCodes=ProcedureCodes.GetAllCodes();
			List<SubstitutionLink> listSubstLinks=SubstitutionLinks.GetAllForPlans(_insPlan.PlanNum);
			_listSubstLinksOld=ListTools.DeepCopy<SubstitutionLink,SubstitutionLink>(listSubstLinks);
			_listInsPlanSubs=FillInsPlanSubList(_listAllProcCodes,listSubstLinks);
			_listSubConditions=new List<string>();
			for(int i=0;i<Enum.GetNames(typeof(SubstitutionCondition)).Length;i++) {
				_listSubConditions.Add(Lan.g("enumSubstitutionCondition",Enum.GetNames(typeof(SubstitutionCondition))[i]));
			}
			FillGridMain();
		}

		///<summary>Takes logic that was previously used to fill the main grid and now fills a list of InsPlanSubstitutions so that we can better keep track of substitution codes created here</summary>
		private List<InsPlanSubstitution> FillInsPlanSubList(List<ProcedureCode> listProcCodes,List<SubstitutionLink> listSubLinks) {
			List<InsPlanSubstitution> retVal=new List<InsPlanSubstitution>();
			List<ProcedureCode> listSubstProcCodes=listProcCodes.FindAll(x => !string.IsNullOrEmpty(x.SubstitutionCode));
			foreach(ProcedureCode procCode in listSubstProcCodes) {
				//procedure code level substitution code aka global substitution
				retVal.Add(new InsPlanSubstitution(procCode));
			}
			//Add all substitution codes for insurance level
			foreach(SubstitutionLink subLink in listSubLinks) {
				ProcedureCode procCode=listProcCodes.FirstOrDefault(x => x.CodeNum==subLink.CodeNum);
				if(procCode==null) {
					continue;//This shouldn't happen.
				}
				//Procedure has a Substitution Link for this insplan.
				retVal.Add(new InsPlanSubstitution(procCode,subLink));
			}
			return retVal;
		}

		private void FillGridMain() {
			InsPlanSubstitution insPlanSubSelected=gridMain.SelectedTag<InsPlanSubstitution>();
			gridMain.BeginUpdate();
			gridMain.ListGridRows.Clear();
			if(gridMain.ListGridColumns.Count==0) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"ProcCode"),90));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"AbbrDesc"),100));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"SubstOnlyIf"),100){ListDisplayStrings=_listSubConditions });//Dropdown combobox
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"SubstCode"),90,true));//Can edit cell
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"SubstDesc"),90));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(gridMain.TranslationName,"InsOnly"),15){ IsWidthDynamic=true });
			}
			//Add all substitution codes for procedure code level
			foreach(InsPlanSubstitution insPlanSub in _listInsPlanSubs.OrderBy(x => x.ProcCode.ProcCode)) {
				//If the current insPlanSub is a global substitute, don't add it to the grid if there are insurance plan level substitutes
				if(insPlanSub.SubLink==null && _listInsPlanSubs.Where(x=>x.SubLink!=null).Any(x=>x.ProcCode.CodeNum==insPlanSub.ProcCode.CodeNum)) {
					continue;
				}
				AddRow(gridMain,insPlanSub);
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

		private void AddRow(GridOD grid,InsPlanSubstitution insPlanSub) {
			//Set all of the row values for the procedure code passed in.
			string enumSubstCondition=insPlanSub.SubCondition.ToString();
			string subCode=insPlanSub.ProcCode.SubstitutionCode;
			ProcedureCode procCodeSubst=_listAllProcCodes.FirstOrDefault(x => x.ProcCode==insPlanSub.ProcCode.SubstitutionCode);
			string subCodeDescript=(procCodeSubst==null)?"":procCodeSubst.AbbrDesc;
			string insOnly="";
			if(insPlanSub.SubLink!=null) {
				//This procedure code has a SubstitutionLink for this insurance plan.
				//set the row values to the Insplan override.
				subCode=insPlanSub.SubLink.SubstitutionCode;
				subCodeDescript="";
				insOnly="X";
				//Validate the subLink.SubstitutionCode. subCodeDescript blank if the substitution code is not valid.
				//User can enter an invalid procedure code if they want.
				if(!string.IsNullOrEmpty(insPlanSub.SubLink.SubstitutionCode)) {
					ProcedureCode procCodeSub=_listAllProcCodes.FirstOrDefault(x=>x.ProcCode==insPlanSub.SubLink.SubstitutionCode);
					if(procCodeSub!=null) {
						subCodeDescript=procCodeSub.AbbrDesc;
					}
				}
			}
			GridRow row=new GridRow();
			row.Cells.Add(insPlanSub.ProcCode.ProcCode);
			row.Cells.Add(insPlanSub.ProcCode.AbbrDesc);
			GridCell cell=new GridCell(Lan.g("enumSubstitutionCondition",enumSubstCondition));
			cell.ComboSelectedIndex=_listSubConditions.FindIndex(x => x==enumSubstCondition);
			row.Cells.Add(cell);
			row.Cells.Add(subCode);
			row.Cells.Add(subCodeDescript);
			row.Cells.Add(insOnly);
			row.Tag=insPlanSub;
			grid.ListGridRows.Add(row);
		}

		///<summary>Opens FormProcCodes in SelectionMode. Creates a new SubstitutionLink for the selected Procedure.</summary>
		private void ButAdd_Click(object sender, EventArgs e){
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			_listAllProcCodes=ProcedureCodes.GetAllCodes();//in case they added a new proc code
			ProcedureCode procSelected=_listAllProcCodes.FirstOrDefault(x=>x.CodeNum==FormP.SelectedCodeNum);
			if(procSelected==null) {
				return;//should never happen, just in case
			}
			//Valid procedure selected. Create a new SubstitutionLink.  The user will be able to add the substition code on the cell grid.
			SubstitutionLink subLink=new SubstitutionLink();
			subLink.CodeNum=procSelected.CodeNum;
			subLink.PlanNum=_insPlan.PlanNum;
			subLink.SubstOnlyIf=SubstitutionCondition.Always;
			subLink.SubstitutionCode="";//Set to blank. The user will be able to add in the cell grid
			//The substitution link will be synced on OK click.
			InsPlanSubstitution insPlanSubNew=new InsPlanSubstitution(procSelected,subLink);
			_listInsPlanSubs.Add(insPlanSubNew);
			FillGridMain();
			//Set the substitution link we just added as selected.
			int index=gridMain.ListGridRows.ToList().FindIndex(x => (x.Tag as InsPlanSubstitution)==insPlanSubNew);
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
			InsPlanSubstitution insPlanSubOld=gridMain.SelectedTag<InsPlanSubstitution>();
			if(insPlanSubOld==null) {
				return;
			}
			//Get the selected substitution condition.
			SubstitutionCondition selectedCondition=(SubstitutionCondition)_listSubConditions.IndexOf(gridMain.ListGridRows[e.Row].Cells[e.Col].Text);
			//procedure code level substitution code
			//Changing the SubstitutionCondition will not update the procedure code level SubstitutionCondition. We will create an insplan override.
			//Create a new SubstitutionLink for ins plan override for this proccode.
			if(_oldCondition==selectedCondition) {
				return;
			}
			if(insPlanSubOld.SubLink==null) {
				SubstitutionLink subLink=new SubstitutionLink();
				subLink.CodeNum=insPlanSubOld.ProcCode.CodeNum;
				//SubstitutionCode will be the same as the procedure codes SubstitutionCode since all the user changed was the SubstitutionCondition
				subLink.SubstitutionCode=insPlanSubOld.ProcCode.SubstitutionCode;
				subLink.PlanNum=_insPlan.PlanNum;
				subLink.SubstOnlyIf=selectedCondition;
				InsPlanSubstitution insPlanSubNew=new InsPlanSubstitution(insPlanSubOld.ProcCode,subLink);
				_listInsPlanSubs.Add(insPlanSubNew);
			}
			else {
				insPlanSubOld.SubCondition=selectedCondition;
				insPlanSubOld.SubLink.SubstOnlyIf=selectedCondition;
			}
			FillGridMain();
		}

		///<summary>Sets _oldText. Used in gridMain_CellLeave to check whether the text changed when leaving the cell.</summary>
		private void gridMain_CellEnter(object sender,ODGridClickEventArgs e) { 
			_oldText=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
			_oldCondition=(SubstitutionCondition)_listSubConditions.IndexOf(gridMain.ListGridRows[e.Row].Cells[e.Col].Text);
		}

		///<summary>Changes the SubstitutionCode to what is entered in the cell. 
		///If the user modifies a procedure level substitution code, a new SubstitutionLink will be added for the inplan(override).
		///The new SubstitutionLink will be added to _listDbSubstLinks</summary>
		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			if(e.Col!=3 || e.Row<0) {//Return if not substitution code column or invalid row
				return;
			}
			InsPlanSubstitution insPlanSubOld=gridMain.SelectedTag<InsPlanSubstitution>();
			if(insPlanSubOld==null) {//This could happen if a substonlyif change occurred from the drop down
				return;
			}
			string newText=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
			if(_oldText==newText) {
				return;
			}
			//Substitution code changed. 
			//We will not validate the new substitution code.
			//Get SubstitutionLink for ins level substitution code if one exist.
			if(insPlanSubOld.SubLink!=null) {//Ins level sub code
				insPlanSubOld.SubLink.SubstitutionCode=newText;
			}
			else {//procedure code level substitution code
				//We will not update the procedure code level substitution code. We will create an insplan override.
				//Create a new SubstitutionLink for ins plan override for this proccode.
				SubstitutionLink subLink=new SubstitutionLink();
				subLink.CodeNum=insPlanSubOld.ProcCode.CodeNum;
				subLink.SubstitutionCode=newText;
				subLink.PlanNum=_insPlan.PlanNum;
				subLink.SubstOnlyIf=insPlanSubOld.ProcCode.SubstOnlyIf;
				InsPlanSubstitution insPlanSubNew=new InsPlanSubstitution(insPlanSubOld.ProcCode,subLink);
				_listInsPlanSubs.Add(insPlanSubNew);
			}
			FillGridMain();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a substitution code first.");
				return;
			}
			InsPlanSubstitution insPlanSub=gridMain.SelectedTag<InsPlanSubstitution>();
			if(insPlanSub.SubLink==null) {//User selected a procedure code level substitution code. Cannot delete
				MsgBox.Show(this,"Cannot delete a global substitution code.");
				return;
			}
			string msgText=(Lans.g(this,"Delete the selected insurance specific substitution code for procedure ")+"\""+insPlanSub.ProcCode.ProcCode+"\"?");
			if(_listInsPlanSubs.Where(x=>x.ProcCode.CodeNum==insPlanSub.ProcCode.CodeNum && x.SubLink!=null).Count()<2 && insPlanSub.ProcCode.SubstitutionCode!="") {
				msgText+="\r\n"+(Lans.g(this,"Deleting the insurance specific substitution code will default to the global substitution code for this procedure")+".");
			}
			if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;
			}
			_listInsPlanSubs.Remove(insPlanSub);
			FillGridMain();
		}

		///<summary>Syncs _listDbSubstLinks and _listDbSubstLinksOld. Does not modify any of the procedure level SubstitutionCodes.</summary>
		private void butOK_Click(object sender,EventArgs e) {
			string msgText=Lan.g(this,"You have chosen to exclude all substitution codes.  "
				+"The checkbox option named 'Don't Substitute Codes (e.g. posterior composites)' "
				+"in the Other Ins Info tab of the Edit Insurance Plan window can be used to exclude all substitution codes.\r\n"
				+"Would you like to enable this option instead of excluding specific codes?");
			bool areAllGlobalSubsOverriddenWithNever=true;
			foreach(var procCodeGroup in _listInsPlanSubs.GroupBy(x => x.ProcCode.CodeNum)) {
				//no insplan sublinks and any proccodes with subcodes have a condition other than never
				if(procCodeGroup.All(x=>x.SubLink==null) || procCodeGroup.Where(x=>x.SubLink!=null).ToList().All(x=>x.SubCondition!=SubstitutionCondition.Never)) {
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
			if(InsPlanSubstitution.HasDuplicates(_listInsPlanSubs)) {
				MsgBox.Show(Lans.g(this,"You have made a duplicate substitutution code and need to fix this before continuing."));
				return;
			}
			List<SubstitutionLink> listLinks=_listInsPlanSubs.Where(x=>x.SubLink!=null).Select(x=>x.SubLink).ToList();
			//No need to make changes to the substitution codes at the procedure level.
			//Only syncing insurance level substitution links.
			//All we need to do now is sync the changes made with _listSubstLinksOld.
			SubstitutionLinks.Sync(listLinks,_listSubstLinksOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
	}
}