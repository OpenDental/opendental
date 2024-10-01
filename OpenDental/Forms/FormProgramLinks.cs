using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing;
using System;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormProgramLinks : FormODBase {
		private bool _didChange;
		private List<Program> _listPrograms;

		///<summary></summary>
		public FormProgramLinks(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProgramLinks_Load(object sender, System.EventArgs e) {
			FillList();
		}

		private void FillList(){
			Programs.RefreshCache();
			_listPrograms=Programs.GetListDeep();
			if(!PrefC.IsODHQ) {
				_listPrograms.RemoveAll(x => x.ProgName==ProgramName.AvaTax.ToString());
			}
			_listPrograms.RemoveAll(x => !Programs.IsEnabledByHq(x,out string _));//Remove all programs that are disabled by HQ from the list to fill.
			gridProgram.BeginUpdate();
			gridProgram.Columns.Clear();
			gridProgram.Columns.Add(new GridColumn("Enabled",55,HorizontalAlignment.Center));
			gridProgram.Columns.Add(new GridColumn("Description",100){ IsWidthDynamic=true });
			gridProgram.ListGridRows.Clear();
			for(int i=0;i<_listPrograms.Count;i++){
				GridRow row=new GridRow();
				row.Tag=_listPrograms[i];
				Color color = Color.FromArgb(230, 255, 238);
				row.ColorBackG=row.ColorBackG;
				if(_listPrograms[i].Enabled){
					row.ColorBackG=color;
				}
				GridCell cell=new GridCell(_listPrograms[i].Enabled ? "X" : "");
				row.Cells.Add(cell);
				row.Cells.Add(_listPrograms[i].ProgDesc);
				gridProgram.ListGridRows.Add(row);
			}
			gridProgram.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormProgramLinkEdit formProgramLinkEdit=new FormProgramLinkEdit();
			formProgramLinkEdit.IsNew=true;
			formProgramLinkEdit.ProgramCur=new Program();
			formProgramLinkEdit.ShowDialog();
			_didChange=true;//because we don't really know what they did, so assume changed.
			FillList();
		}

		private void gridProgram_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DialogResult dialogResult=DialogResult.None;
			Program program=_listPrograms[gridProgram.GetSelectedIndex()].Copy();
			switch(program.ProgName) {
				case "eClinicalWorks":
					if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
						break;
					}
					using(FormEClinicalWorks formEClinicalWorks=new FormEClinicalWorks()) {
						formEClinicalWorks.ProgramCur=program;
						dialogResult=formEClinicalWorks.ShowDialog();
					}
					break;
				case "eRx":
					using(FormErxSetup formErxSetup=new FormErxSetup()) {
						dialogResult=formErxSetup.ShowDialog();
					}
					break;
				case "Mountainside":
					using(FormMountainside formMountainside=new FormMountainside()) {
						formMountainside.ProgramCur=program;
						dialogResult=formMountainside.ShowDialog();
					}
					break;
				case "PayConnect":
					using(FormPayConnectSetup formPayConnectSetup=new FormPayConnectSetup()) {
						dialogResult=formPayConnectSetup.ShowDialog();
					}
					break;
				case "Podium":
					using(FormPodiumSetup formPodiumSetup=new FormPodiumSetup()) {
						dialogResult=formPodiumSetup.ShowDialog();
					}
					break;
				case "Xcharge":
					using(FormXchargeSetup fromXChargeSetup=new FormXchargeSetup()) {
						dialogResult=fromXChargeSetup.ShowDialog();
					}
					break;
				case "FHIR":
					using(FormFHIRSetup formFHIRSetup=new FormFHIRSetup()) {
						dialogResult=formFHIRSetup.ShowDialog();
					}
					break;
				case "Transworld":
					using(FormTransworldSetup formTransworldSetup=new FormTransworldSetup()) {
						dialogResult=formTransworldSetup.ShowDialog();
					}
					break;
				case "PaySimple":
					using(FormPaySimpleSetup formPaySimpleSetup=new FormPaySimpleSetup()) {
						dialogResult=formPaySimpleSetup.ShowDialog();
					}
					break;
				case "AvaTax":
					using(FormAvaTax formAvaTax=new FormAvaTax()) {
						formAvaTax.ProgramCur=program;
						dialogResult=formAvaTax.ShowDialog();
					}
					break;
				case "XDR":
					using(FormXDRSetup formXDRSetup=new FormXDRSetup()) {
						dialogResult=formXDRSetup.ShowDialog();
					}
					break;
				case "TrojanExpressCollect":
					using(FormTrojanCollectSetup formTrojanCollectSetup=new FormTrojanCollectSetup()) {
						dialogResult=formTrojanCollectSetup.ShowDialog();
					}
					break;
				case "BencoPracticeManagement":
					FrmBencoSetup frmBencoSetup=new FrmBencoSetup();
					frmBencoSetup.ShowDialog();
					if(frmBencoSetup.IsDialogOK){
						dialogResult=DialogResult.OK;
					}
					break;
				case "CareCredit":
					using(FormCareCreditSetup formCareCreditSetup=new FormCareCreditSetup()){
						dialogResult=formCareCreditSetup.ShowDialog();
						if(formCareCreditSetup.DoShowApptViewWindow) {
							using FormApptViews formApptViews=new FormApptViews();
							formApptViews.ShowDialog();
						}
					}
					break;
				case nameof(ProgramName.EdgeExpress):
					using(FormEdgeExpressSetup formEdgeExpressSetup=new FormEdgeExpressSetup()) {
						dialogResult=formEdgeExpressSetup.ShowDialog();
					}
					break;
				case "QuickBooksOnline":
					using(FormQuickBooksOnlineSetup formQuickBooksOnlineSetup=new FormQuickBooksOnlineSetup(true)) {
						dialogResult=formQuickBooksOnlineSetup.ShowDialog();
					}
					break;
				default:
					using(FormProgramLinkEdit formProgramLinkEdit=new FormProgramLinkEdit()) {
						if(Programs.IsStatic(program)) {
							formProgramLinkEdit.AllowToolbarChanges=false;
						}
						formProgramLinkEdit.ProgramCur=program;
						dialogResult=formProgramLinkEdit.ShowDialog();
					}
				break;
			}
			if(dialogResult==DialogResult.OK) {
				_didChange=true;
				FillList();
			}
		}

		private void FormProgramLinks_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(!_didChange){
				return;
			}
			Cursor=Cursors.WaitCursor;
			try {
				//Let HQ know the program link change.
				Programs.SendEnabledProgramsToHQ();
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			Cursor=Cursors.Default;
			DataValid.SetInvalid(InvalidType.Programs, InvalidType.ToolButsAndMounts);
		}

	}
}