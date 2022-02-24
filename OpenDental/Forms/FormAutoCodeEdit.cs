using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Globalization;

namespace OpenDental{
	///<summary></summary>
	public partial class FormAutoCodeEdit : FormODBase {
		///<summary></summary>
    public bool IsNew;
		///<summary>Set this before opening the form.</summary>
		public AutoCode AutoCodeCur;
		List<AutoCodeItem> listForCode;
		private List<AutoCodeCond> _listAutoCodeConds;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormAutoCodeEditCanada";
			}
			return "FormAutoCodeEdit";
		}

		///<summary></summary>
		public FormAutoCodeEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoCodeEdit_Load(object sender, System.EventArgs e) {      
      if(IsNew){
        this.Text=Lan.g(this,"Add Auto Code");
      }
      else{
        this.Text=Lan.g(this,"Edit Auto Code");
        textDescript.Text=AutoCodeCur.Description;
        checkHidden.Checked=AutoCodeCur.IsHidden;
				checkLessIntrusive.Checked=AutoCodeCur.LessIntrusive;
      }
			FillGrid();
		}

		private void FillGrid() {
			int count=0;
			AutoCodeItems.RefreshCache();
			AutoCodeConds.RefreshCache();
			_listAutoCodeConds=AutoCodeConds.GetDeepCopy();
			listForCode=AutoCodeItems.GetListForCode(AutoCodeCur.AutoCodeNum);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("Table AutoCodes","Code"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("Table AutoCodes","Description"),200);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("Table AutoCodes","Conditions"),400);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listForCode.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ProcedureCodes.GetProcCode(listForCode[i].CodeNum).ProcCode);
				row.Cells.Add(ProcedureCodes.GetProcCode(listForCode[i].CodeNum).Descript);
				string conditions="";
				count=0;
				for(int j=0;j<_listAutoCodeConds.Count;j++)	{
					if(_listAutoCodeConds[j].AutoCodeItemNum==listForCode[i].AutoCodeItemNum) {
						if(count!=0)	{
							conditions+=", ";
						}
						conditions+=_listAutoCodeConds[j].Cond.ToString();
						count++;
					}
				}
				row.Cells.Add(conditions);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			AutoCodeItem AutoCodeItemCur=listForCode[e.Row];
			using FormAutoItemEdit FormAIE=new FormAutoItemEdit();
			FormAIE.AutoCodeItemCur=AutoCodeItemCur;
			FormAIE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormAutoItemEdit FormAIE=new FormAutoItemEdit();
			FormAIE.IsNew=true;
			FormAIE.AutoCodeItemCur=new AutoCodeItem();
			FormAIE.AutoCodeItemCur.AutoCodeNum=AutoCodeCur.AutoCodeNum;
			FormAIE.ShowDialog();
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			int selectedRow=gridMain.GetSelectedIndex();
			if(selectedRow==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
        return;
			}
			AutoCodeItem AutoCodeItemCur=listForCode[selectedRow];
      AutoCodeConds.DeleteForItemNum(AutoCodeItemCur.AutoCodeItemNum);
      AutoCodeItems.Delete(AutoCodeItemCur);
			FillGrid();
		}  

		private void butOK_Click(object sender, System.EventArgs e) {
		  if(textDescript.Text==""){
        MessageBox.Show(Lan.g(this,"The Description cannot be blank"));
        return;
      }
			if(listForCode.Count==0){
				MsgBox.Show(this,"Must have at least one item in the list.");
				//This is not actually rigorous enough since items will already be deleted.
				return;
			}
      AutoCodeCur.Description=textDescript.Text;
      AutoCodeCur.IsHidden=checkHidden.Checked;
			AutoCodeCur.LessIntrusive=checkLessIntrusive.Checked;
			AutoCodes.Update(AutoCodeCur);
      DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {	
      DialogResult=DialogResult.Cancel;
		}

		private void FormAutoCodeEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult!=DialogResult.OK) {
				if(IsNew) {
					try {
						AutoCodes.Delete(AutoCodeCur);
					}
					catch(ApplicationException ex) {
						//should never happen
						MessageBox.Show(ex.Message);
					}
					return;
				}
				if(listForCode.Count==0){
					try {
						AutoCodes.Delete(AutoCodeCur);
					}
					catch{ }//fails if attached to a procedure button.
					return;
				}
			}
			AutoCodeItems.RefreshCache();
			AutoCodeConds.RefreshCache();
			for(int i=0;i<listForCode.Count;i++) {//Attach the conditions to the items for better organization
				listForCode[i].ListConditions=new List<AutoCodeCond>();
        for(int j=0;j<_listAutoCodeConds.Count;j++){//Fill conditions for this AutoCodeItem
          if(_listAutoCodeConds[j].AutoCodeItemNum==listForCode[i].AutoCodeItemNum){
						listForCode[i].ListConditions.Add(_listAutoCodeConds[j]);
          }
        }
			}
			//Must have same number of conditions for each AutoCodeItem.----------------------------------------------------------------------------------
			for(int i=1;i<listForCode.Count;i++) {//start at 1 and compare to the 0 index.
				if(listForCode[i].ListConditions.Count!=listForCode[0].ListConditions.Count) {
					MsgBox.Show(this,"All AutoCode items must have the same number of conditions.");
					e.Cancel=true;
					return;
				}
			}
			if(listForCode[0].ListConditions.Count==0) {//Rest of the checks assume at least one condition.
				return;
			}
			//Check for duplicate AutoCodeItem condition lists.-------------------------------------------------------------------------------------------
			for(int i=1;i<listForCode.Count;i++) {//start at 1
				for(int j=0;j<i;j++) {//loop through the lower-indexed entries
					int matches=0;
					for(int k=0;k<listForCode[i].ListConditions.Count;k++) {//For each condition in i, check for matches with conditions in j
						if(listForCode[i].ListConditions[k].Cond==listForCode[j].ListConditions[k].Cond) {//if the same condition is in both rows.
							matches++;
						}
					}
					if(matches==listForCode[i].ListConditions.Count) {//If the number of matches equals the number of conditions on this row
						MsgBox.Show(this,"Cannot have two AutoCode Items with duplicate conditions.");
						e.Cancel=true;
						return;
					}
				}
			}		
			//Decide which categories are involved.------------------------------------------------------------------------------------------------------
			bool isAnt=false;//Not a category, could be isAntPost or isAntPreMol
			bool isAntPost=false;
			bool isAntPreMol=false;//Anterior/premolar/molar
			bool isNumSurf=false;
			bool isFirstEachAdd=false;
			bool isMaxMand=false;
			bool isPriPerm=false;
			bool isPontRet=false;
			for(int i=0;i<listForCode.Count;i++) {
				//If the item matches the category, set the boolean to true.
				for(int j=0;j<listForCode[i].ListConditions.Count;j++) {
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.Anterior) {
						isAnt=true;
						//We want to also set either isAntPost or isAntPreMol, but we don't have enough information yet to set that.
						continue;
					}
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.Posterior) {
						isAntPost=true;
						continue;
					}
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.Premolar
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Molar
						) {
						isAntPreMol=true;
						continue;
					}
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.One_Surf
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Two_Surf
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Three_Surf
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Four_Surf
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Five_Surf
						) {
						isNumSurf=true;
						continue;
					}
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.First
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.EachAdditional
						) {
						isFirstEachAdd=true;
						continue;
					}
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.Maxillary
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Mandibular
						) {
						isMaxMand=true;
						continue;
					}
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.Primary
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Permanent
						) {
						isPriPerm=true;
						continue;
					}
					if(listForCode[i].ListConditions[j].Cond==AutoCondition.Pontic
						|| listForCode[i].ListConditions[j].Cond==AutoCondition.Retainer
						) {
						isPontRet=true;
						continue;
					}
				}
			}
			//After the loop, you had better have exactly the same number of booleans true as number of conditions on each item.--------------------
			if(isAntPost && isAntPreMol) {
				MsgBox.Show(this,"Cannot have both Posterior and Premolar/Molar categories.");
				e.Cancel=true;
				return;
			}
			if(isAnt) {//This is the only purpose of the isAnt bool.  We won't use it anymore.
				if(!isAntPost && !isAntPreMol) {
					MsgBox.Show(this,"Anterior condition is present without any corresponding posterior or premolar/molar condition.");
					e.Cancel=true;
					return;
				}
			}
			//Count how many categories were hit.
			int numCategories=0;
			if(isAntPost) {
				numCategories++;
			}
			if(isAntPreMol) {
				numCategories++;
			}
			if(isNumSurf) {
				numCategories++;
			}
			if(isFirstEachAdd) {
				numCategories++;
			}
			if(isMaxMand) {
				numCategories++;
			}
			if(isPriPerm) {
				numCategories++;
			}
			if(isPontRet) {
				numCategories++;
			}
			if(numCategories!=listForCode[0].ListConditions.Count) {//Every row has to have the same number of conditions
				MessageBox.Show(Lan.g(this,"When using ")+listForCode[0].ListConditions.Count+Lan.g(this," condition(s), you must use conditions from ")											
					+listForCode[0].ListConditions.Count+Lan.g(this," logical categories. You are using conditions from ")+numCategories+Lan.g(this," logical categories."));
				e.Cancel=true;
				return;
			}
			//Make sure that the number of AutoCodeItems is right. For example, if isAntPost and isNumSurf are the only true one, there should be 10 items.----------------------------------------
			int reqNumAutoCodeItems=1;
			if(isAntPost) {
				reqNumAutoCodeItems=reqNumAutoCodeItems*2;
			}
			if(isAntPreMol) {
				if(isPriPerm) {
					reqNumAutoCodeItems=reqNumAutoCodeItems*5;//normally this would be 2*3 but primary molars don't exist, so we have 2*3-1=5
				}
				else {
					reqNumAutoCodeItems=reqNumAutoCodeItems*3;
				}
			}
			else {
				if(isPriPerm) {
					reqNumAutoCodeItems=reqNumAutoCodeItems*2;
				}
			}
			if(isNumSurf) {
				reqNumAutoCodeItems=reqNumAutoCodeItems*5;
			}
			if(isFirstEachAdd) {
				reqNumAutoCodeItems=reqNumAutoCodeItems*2;
			}
			if(isMaxMand) {
				reqNumAutoCodeItems=reqNumAutoCodeItems*2;
			}
			if(isPontRet) {
				reqNumAutoCodeItems=reqNumAutoCodeItems*2;
			}
			if(listForCode.Count!=reqNumAutoCodeItems) {
				MessageBox.Show(Lan.g(this,"For the condition categories you are using, you should have ")
					+reqNumAutoCodeItems+Lan.g(this," entries in your list. You have ")+listForCode.Count+".");
				e.Cancel=true;
				return;
			}
		}
	}
}
