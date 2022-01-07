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
		private Programs Programs=new Programs();
		private bool changed;
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
			gridProgram.ListGridColumns.Clear();
			gridProgram.ListGridColumns.Add(new GridColumn("Enabled",55,HorizontalAlignment.Center));
			gridProgram.ListGridColumns.Add(new GridColumn("Program Name",100){ IsWidthDynamic=true });
			gridProgram.ListGridRows.Clear();
			foreach(Program prog in _listPrograms){
				GridRow row=new GridRow() { Tag=prog };
				Color color = Color.FromArgb(230, 255, 238);
				row.ColorBackG=prog.Enabled ? color : row.ColorBackG;
				GridCell cell=new GridCell(prog.Enabled ? "X" : "");
				row.Cells.Add(cell);
				row.Cells.Add(prog.ProgDesc);
				gridProgram.ListGridRows.Add(row);
			}
			gridProgram.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormProgramLinkEdit FormPE=new FormProgramLinkEdit();
			FormPE.IsNew=true;
			FormPE.ProgramCur=new Program();
			FormPE.ShowDialog();
			changed=true;//because we don't really know what they did, so assume changed.
			FillList();
		}

		private void gridProgram_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			DialogResult dResult=DialogResult.None;
			Program program=_listPrograms[gridProgram.GetSelectedIndex()].Copy();
			switch(program.ProgName) {
				case "UAppoint":
					using(FormUAppoint FormU=new FormUAppoint()) {
						FormU.ProgramCur=program;
						dResult=FormU.ShowDialog();
					}
					break;
				case "eClinicalWorks":
					if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
						break;
					}
					using(FormEClinicalWorks FormECW=new FormEClinicalWorks()) {
						FormECW.ProgramCur=program;
						dResult=FormECW.ShowDialog();
					}
					break;
				case "eRx":
					using(FormErxSetup FormES=new FormErxSetup()) {
						dResult=FormES.ShowDialog();
					}
					break;
				case "Mountainside":
					using(FormMountainside FormM=new FormMountainside()) {
						FormM.ProgramCur=program;
						dResult=FormM.ShowDialog();
					}
					break;
				case "PayConnect":
					using(FormPayConnectSetup fpcs=new FormPayConnectSetup()) {
						dResult=fpcs.ShowDialog();
					}
					break;
				case "Podium":
					using(FormPodiumSetup FormPS=new FormPodiumSetup()) {
						dResult=FormPS.ShowDialog();
					}
					break;
				case "Xcharge":
					using(FormXchargeSetup fxcs=new FormXchargeSetup()) {
						dResult=fxcs.ShowDialog();
					}
					break;
				case "FHIR":
					using(FormFHIRSetup FormFS=new FormFHIRSetup()) {
						dResult=FormFS.ShowDialog();
					}
					break;
				case "Transworld":
					using(FormTransworldSetup FormTs=new FormTransworldSetup()) {
						dResult=FormTs.ShowDialog();
					}
					break;
				case "PaySimple":
					using(FormPaySimpleSetup formPS=new FormPaySimpleSetup()) {
						dResult=formPS.ShowDialog();
					}
					break;
				case "AvaTax":
					using(FormAvaTax formAT=new FormAvaTax()) {
						formAT.ProgramCur=program;
						dResult=formAT.ShowDialog();
					}
					break;
				case "XDR":
					using(FormXDRSetup FormXS=new FormXDRSetup()) {
						dResult=FormXS.ShowDialog();
					}
					break;
				case "TrojanExpressCollect":
					using(FormTrojanCollectSetup FormTro=new FormTrojanCollectSetup()) {
						dResult=FormTro.ShowDialog();
					}
					break;
				case "BencoPracticeManagement":
					using(FormBencoSetup FormBPM=new FormBencoSetup()){
						dResult=FormBPM.ShowDialog();
					}
					break;
				case "CareCredit":
					using(FormCareCreditSetup FormCCS=new FormCareCreditSetup()){
						dResult=FormCCS.ShowDialog();
						if(FormCCS.DoShowApptViewWindow) {
							using FormApptViews formApptViews=new FormApptViews();
							formApptViews.ShowDialog();
						}
					}
					break;
				case nameof(ProgramName.EdgeExpress):
					using(FormEdgeExpressSetup formEdgeExpressSetup=new FormEdgeExpressSetup()) {
						dResult=formEdgeExpressSetup.ShowDialog();
					}
					break;
				case "QuickBooksOnline":
					using(FormQuickBooksOnlineSetup formQuickBooksOnlineSetup=new FormQuickBooksOnlineSetup(true)) {
						dResult=formQuickBooksOnlineSetup.ShowDialog();
					}
					break;
				default:
					using(FormProgramLinkEdit FormPE=new FormProgramLinkEdit()) {
						if(Programs.IsStatic(program)) {
							FormPE.AllowToolbarChanges=false;
						}
						FormPE.ProgramCur=program;
						dResult=FormPE.ShowDialog();
					}
				break;
			}
			if(dResult==DialogResult.OK) {
				changed=true;
				FillList();
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormProgramLinks_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(!changed){
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
