using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using OpenDental.UI;
using OpenDental;
using OpenDentBusiness;
using System.IO;
using CodeBase;

namespace OpenDental.User_Controls.SetupWizard {
	public partial class UserControlSetupWizDefinitions:SetupWizControl {
		private List<Def> _listDefsAll;
		private bool _isDefChanged;

		///<summary>Gets the currently selected DefCat along with its options.</summary>
		private DefCatOptions _selectedDefCatOpt {
			get { return listCategory.GetSelected<DefCatOptions>(); }
		}

		///<summary>All definitions for the current category, hidden and non-hidden.</summary>
		private List<Def> _listDefsCur {
			get { return _listDefsAll.Where(x => x.Category == _selectedDefCatOpt.DefCat).OrderBy(x => x.ItemOrder).ToList(); }
		}

		public UserControlSetupWizDefinitions() {
			InitializeComponent();
			this.OnControlDone += ControlDone;
		}

		private void UserControlSetupWizDefinitions_Load(object sender,EventArgs e) {
			IsDone=true;//this is optional, so the user is done whenever they choose
			List<DefCat> listDefCats=new List<DefCat>();
			//Only including the most important categories so the user is not intimidated with all the options.
			listDefCats.Add(DefCat.AccountColors);
			listDefCats.Add(DefCat.AdjTypes);
			listDefCats.Add(DefCat.AppointmentColors);
			listDefCats.Add(DefCat.ApptConfirmed);
			listDefCats.Add(DefCat.ApptProcsQuickAdd);
			listDefCats.Add(DefCat.AutoNoteCats);
			listDefCats.Add(DefCat.BillingTypes);
			listDefCats.Add(DefCat.BlockoutTypes);
			listDefCats.Add(DefCat.ChartGraphicColors);
			listDefCats.Add(DefCat.CommLogTypes);
			listDefCats.Add(DefCat.ImageCats);
			listDefCats.Add(DefCat.PaymentTypes);
			listDefCats.Add(DefCat.ProcCodeCats);
			listDefCats.Add(DefCat.RecallUnschedStatus);
			listDefCats.Add(DefCat.TxPriorities);
			List<DefCatOptions> listDefCatsOrdered = new List<DefCatOptions>();
			listDefCatsOrdered=DefL.GetOptionsForDefCats(listDefCats.ToArray());
			listDefCatsOrdered = listDefCatsOrdered.OrderBy(x => x.DefCat.GetDescription()).ToList(); //orders alphabetically.
			foreach(DefCatOptions defCOpt in listDefCatsOrdered) {
				listCategory.Items.Add(Lans.g(this,defCOpt.DefCat.GetDescription()),defCOpt);
				if(defCOpt.DefCat==listDefCatsOrdered[0].DefCat) {
					listCategory.SelectedItem=defCOpt;
				}
			}
		}
		
		private void FillGridDefs(){
			if(_listDefsAll == null || _listDefsAll.Count == 0) {
				RefreshDefs();
			}
			DefL.FillGridDefs(gridDefs,_selectedDefCatOpt,_listDefsCur);
			//the following do not require a refresh of the table:
			if(_selectedDefCatOpt.CanHide) {
				butHide.Visible=true;
			}
			else {
				butHide.Visible=false;
			}
			if(_selectedDefCatOpt.CanEditName) {
				groupEdit.Enabled=true;
				groupEdit.Text=Lans.g(this,"Edit Items");
			}
			else {
				groupEdit.Enabled=false;
				groupEdit.Text=Lans.g(this,"Not allowed");
			}
			textGuide.Text=_selectedDefCatOpt.HelpText;
		}

		private void gridDefs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Def selectedDef = (Def)gridDefs.ListGridRows[e.Row].Tag;
			_isDefChanged=DefL.GridDefsDoubleClick(selectedDef,gridDefs,_selectedDefCatOpt,_listDefsCur,_listDefsAll,_isDefChanged);
			if(_isDefChanged) {
				RefreshDefs();
				FillGridDefs();
			}
		}
			

		private void butAdd_Click(object sender,EventArgs e) {
			if(DefL.AddDef(gridDefs,_selectedDefCatOpt)) {
				RefreshDefs();
				FillGridDefs();
				_isDefChanged=true;
			}
		}

		private void listCategory_SelectedIndexChanged(object sender,EventArgs e) {
			FillGridDefs();
		}

		private void RefreshDefs() {
			Defs.RefreshCache();
			_listDefsAll=Defs.GetDeepCopy().SelectMany(x => x.Value).ToList();
		}

		private void butHide_Click(object sender,EventArgs e) {
			if(DefL.TryHideDefSelectedInGrid(gridDefs,_selectedDefCatOpt)) {
				RefreshDefs();
				FillGridDefs();
				_isDefChanged=true;
			}
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(DefL.UpClick(gridDefs)) {
				_isDefChanged=true;
				FillGridDefs();
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(DefL.DownClick(gridDefs)){
				_isDefChanged=true;
				FillGridDefs();
			}
		}
		
		private void ControlDone(object sender, EventArgs e) {
			//Correct the item orders of all definition categories.
			List<Def> listDefUpdates=new List<Def>();
			foreach(KeyValuePair<DefCat,List<Def>> kvp in Defs.GetDeepCopy()) {
				for(int i=0;i<kvp.Value.Count;i++) {
					if(kvp.Value[i].ItemOrder!=i) {
						kvp.Value[i].ItemOrder=i;
						listDefUpdates.Add(kvp.Value[i]);
					}
				}
			}
			listDefUpdates.ForEach(x => Defs.Update(x));
			if(_isDefChanged || listDefUpdates.Count>0) {
				DataValid.SetInvalid(InvalidType.Defs);
			}
		}
		
	}
}
