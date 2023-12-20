using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormBenefitFrequencies:FormODBase {
		public List<Benefit> ListBenefitsAll;
		public long PlanNum;
		public long PatPlanNum;
		public bool IsCalendar;

		public FormBenefitFrequencies() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormBenefitFrequencies_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			List<CodeGroup> listCodeGroups=CodeGroups.GetDeepCopy();//including hidden
			List<Benefit> listBenefits=ListBenefitsAll.FindAll(x=>Benefits.IsFrequencyLimitation(x) && x.CodeGroupNum!=0 
				//We want to exclude the Patient Overrides from this grid because they should only be visible in FormInsBenefits.GridBenefits
				//This was actually causing a bug in 23.2. In 23.3, this window was deprecated and this grid was moved into FormInsBenefits.
				//In 23.3, we added full support for patient overrides.
				//But in 23.2, to do a patient override, it can only be done in the main grid in the non-simple view.
				//Any patient override would only show in the main grid (including simple at bottom), not here.
				&& x.PatPlanNum==0);
			listBenefits.Sort();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn;
			gridColumn=new GridColumn("Code Group",120);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("#",35);
			gridColumn.IsEditable=true;
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Frequency",70);
			List<string> listStringsFrequencyOptions = new List<string>() {
				FrequencyOptions.Every_Years.GetDescription(),
				FrequencyOptions._PerBenefitYear.GetDescription(),
				FrequencyOptions.Every_Months.GetDescription(),
				FrequencyOptions._InLast12Months.GetDescription(),
			};
			gridColumn.ListDisplayStrings=listStringsFrequencyOptions;
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			for(int i=0;i<listCodeGroups.Count;i++) {
				List<Benefit> listBenefitsGroup=listBenefits.FindAll(x=>x.CodeGroupNum==listCodeGroups[i].CodeGroupNum);
				//there could be duplicates for a codegroup for various reasons
				if(listBenefitsGroup.Count==0) {
					if(listCodeGroups[i].IsHidden) {
						continue;
					}
					gridRow=new GridRow();
					Benefit benefit=new Benefit();
					benefit.CodeGroupNum=listCodeGroups[i].CodeGroupNum; // Give benefit a CodeGroupNum so the 'Save' button knows how to handle it.
					gridRow.Cells.Add(listCodeGroups[i].GroupName);
					gridRow.Cells.Add("");//#
					GridCell gridCell=new GridCell(listStringsFrequencyOptions[0]);//Frequency
					gridCell.ComboSelectedIndex=0;
					gridRow.Cells.Add(gridCell);
					gridRow.Tag=benefit;
					gridMain.ListGridRows.Add(gridRow);
					// Add benefit to ListBenefitsAll so that changes to this new frequency are kept.
					// If it's left unchanged, it will get removed on save anyway.
					ListBenefitsAll.Add(benefit);
					continue;
				}
				for(int k=0;k<listBenefitsGroup.Count;k++){//one or more
					gridRow=new GridRow();
					gridRow.Cells.Add(Benefits.GetCategoryString(listBenefitsGroup[k]));
					gridRow.Cells.Add(listBenefitsGroup[k].Quantity.ToString());
					FrequencyOptions frequencyOptions=DetermineBenefitFrequencyOption(listBenefitsGroup[k]);
					GridCell gridCell=new GridCell(listStringsFrequencyOptions[(int)frequencyOptions]);
					gridCell.ComboSelectedIndex=(int)frequencyOptions;
					gridRow.Cells.Add(gridCell);
					gridRow.Tag=listBenefitsGroup[k];
					gridMain.ListGridRows.Add(gridRow);
				}
			}
			gridMain.EndUpdate();
		}

		/// <summary> Create a frequency benefit with a given code group, frequency, and quantity. </summary>
		private Benefit MakeFrequencyBenefit(long codeGroupNum, int comboFrequencyIndex, byte quantity) {
			Benefit benefit=new Benefit();
			benefit.BenefitType=InsBenefitType.Limitations;
			benefit.MonetaryAmt=-1;
			benefit.Percent=-1;
			benefit.CodeNum=0;
			benefit.CovCatNum=0;
			benefit.PlanNum=PlanNum;
			benefit.IsNew=true;
			benefit.CodeGroupNum=codeGroupNum;
			benefit.TimePeriod=BenefitTimePeriod.None;
			switch(comboFrequencyIndex) {
				case (int)FrequencyOptions.Every_Years:
					benefit.QuantityQualifier=BenefitQuantity.Years;
					break;
				case (int)FrequencyOptions._PerBenefitYear:
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					if(IsCalendar) {
						benefit.TimePeriod=BenefitTimePeriod.CalendarYear;
					}
					else {
						benefit.TimePeriod=BenefitTimePeriod.ServiceYear;
					}
					break;
				case (int)FrequencyOptions.Every_Months:
					benefit.QuantityQualifier=BenefitQuantity.Months;
					break;
				case (int)FrequencyOptions._InLast12Months:
					benefit.QuantityQualifier=BenefitQuantity.NumberOfServices;
					benefit.TimePeriod=BenefitTimePeriod.NumberInLast12Months;
					break;
			}
			benefit.Quantity=quantity;
			benefit.IsNew=false;
			return benefit;
		}

		/// <summary> Will determine the FrequencyOptions enum value that was used to create a given frequency benefit. </summary>
		private FrequencyOptions DetermineBenefitFrequencyOption(Benefit benefit) {
			if(benefit.QuantityQualifier==BenefitQuantity.Years) {
				return FrequencyOptions.Every_Years;
			}
			else if(benefit.TimePeriod==BenefitTimePeriod.ServiceYear || benefit.TimePeriod==BenefitTimePeriod.CalendarYear) {
				return FrequencyOptions._PerBenefitYear;
			}
			else if(benefit.QuantityQualifier==BenefitQuantity.Months) {
				return FrequencyOptions.Every_Months;
			}
			else if(benefit.TimePeriod==BenefitTimePeriod.NumberInLast12Months) {
				return FrequencyOptions._InLast12Months;
			}
			throw new Exception("Frequency Benefit did not have a matching Frequency Option");
		}

		private void butSave_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			List<Benefit> listBenefits=gridMain.ListGridRows.Select(x=>(Benefit)x.Tag).ToList();
			for(int i=0;i<listBenefits.Count;i++){
				int indexFrequencySelected=gridMain.ListGridRows[i].Cells[2].ComboSelectedIndex;
				Byte byteProvided=PIn.Byte(gridMain.ListGridRows[i].Cells[1].Text, hasExceptions:false);
				if(byteProvided<1) {
					ListBenefitsAll.Remove(listBenefits[i]);
					continue;
				}
				// Need to alter the shared list to reflect changes.
				int indexInListAll=ListBenefitsAll.IndexOf(listBenefits[i]);
				ListBenefitsAll[indexInListAll]=MakeFrequencyBenefit(listBenefits[i].CodeGroupNum,indexFrequencySelected,byteProvided);
			}
		}

		//private void FormBenefitFrequencies_KeyDown(object sender,KeyEventArgs e) {
		//	if(e.KeyData!=Keys.Tab && e.KeyData!=Keys.Enter) {
		//		return;
		//	}
		//	Point pointCellSelected=gridMain.SelectedCell;
		//	pointCellSelected.X++;
		//	if(pointCellSelected.X==gridMain.Columns.Count) {
		//		pointCellSelected.X=1;
		//		pointCellSelected.Y++;
		//		// Need to mark event handled, otherwise the text cell we're moving onto will get the 'tab' event and move to the next column.
		//		e.Handled=true;
		//	}
		//	if(pointCellSelected.Y==gridMain.ListGridRows.Count) {
		//		pointCellSelected.Y=0;
		//	}
		//	gridMain.SetSelected(pointCellSelected);
		//}
	}
}