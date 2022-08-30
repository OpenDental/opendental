using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	class SheetUtilL {
		///<summary>Fills the given comboBox with valid GrowthBehaviorEnum options. Centralizes fill of growth behavior into comboboxes in several forms to ensure consistent behavior regarding the available options.</summary>
		public static void FillComboGrowthBehavior(UI.ComboBoxOD combo,GrowthBehaviorEnum growthBehaviorSelected,bool isDynamicSheetType=false,bool isGridCombo=false) {
			List<GrowthBehaviorEnum> listGrowthOptions=new List<GrowthBehaviorEnum>();
			foreach(GrowthBehaviorEnum growthBehaviorEnum in Enum.GetValues(typeof(GrowthBehaviorEnum)).OfType<GrowthBehaviorEnum>().ToList()) {
				SheetGrowthAttribute sheetGrowthAttribute=EnumTools.GetAttributeOrDefault<SheetGrowthAttribute>(growthBehaviorEnum);
				if(growthBehaviorEnum==GrowthBehaviorEnum.None || isDynamicSheetType==sheetGrowthAttribute.IsDynamic) {
					//When filling a comboBox associated to setting the growthBehavior of a grid we skip GridOnly growthAttributes.
					if(!isGridCombo && sheetGrowthAttribute.IsGridOnly) {
						continue;
					}
					listGrowthOptions.Add(growthBehaviorEnum);
				}
			}
			combo.Items.AddList(listGrowthOptions,(growthOption) => growthOption.ToString());//can't use AddEnum
			//since this is a filtered enum, we can't just set the index
			for(int i=0;i<combo.Items.Count;i++){
				if((GrowthBehaviorEnum)combo.Items.GetObjectAt(i)==growthBehaviorSelected){
					combo.SetSelected(i);
				}
			}
		}

		///<summary>Displays a Sheet, first checking if the sheet is linked to an existing Document and if so, displaying the file.  Otherwise, opens the Sheet via FormSheetFillEdit.</summary>
		public static void ShowSheet(Sheet sheet,Patient pat,FormClosingEventHandler onFormClosing) {
			if(sheet==null) {
				MsgBox.Show("Sheets","Error opening sheet.");
				return;
			}
			if(sheet.DocNum!=0) {
				if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
					MsgBox.Show("Sheets","Unable to view saved sheet when storing images in database.");
					return;
				}
				Document sheetDoc=Documents.GetByNum(sheet.DocNum,true);
				if(sheetDoc==null) {
					MsgBox.Show("Sheets","Saved sheet no longer exists.");
					return;
				}
				string patFolder=ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath());
				FileAtoZ.OpenFile(ImageStore.GetFilePath(sheetDoc,patFolder));
			}
			else {
				FormSheetFillEdit.ShowForm(sheet,onFormClosing);
			}	
		}
	}
}
