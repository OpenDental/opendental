using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/*
Conversion Checklist====================================================================================================================
Questions (do not edit)                                              |Answers might include "yes", "ok", "no", "n/a", "done", etc.
-Review this form. Any unsupported controls or properties?           |Done
   Search for "progress". Any progress bars?                         |Done
   Anything in the Tray?                                             |No
   Search for "filter". Any use of SetFilterControlsAndAction?       |Done
   If yes, then STOP here. Talk to Jordan for strategy               |
-Look in the code for any references to other Forms. If those forms  |Done
   have not been converted, then STOP.  Convert those forms first.   |
-Will we include TabIndexes?  If so, up to what index?  This applies |No
   even if one single control is set so that cursor will start there |
-Grids: get familiar with properties in bold and with events.        |Done
-Run UnitTests FormWpfConverter, type in Form name, TabI, and convert|Done
-Any conversion exceptions? Consider reverting.                      |No
-In WpfControlsOD/Frms, include the new files in the project.        |Done
-Switch to using this checklist in the new Frm. Delete the other one-|Done
-Do the red areas and issues at top look fixable? Consider reverting |Yes
-Does convert script need any changes instead of fixing manually?    |No
-Fix all the red areas.                                              |Done (except 171,192, and 262 in FormHelpBrowser)
-Address all the issues at the top. Leave in place for review.       |n/a
-Verify that the Button click events converted automatically.        |Done
-Attach all orphaned event handlers to events in constructor.        |Done
-Possibly make some labels or other controls slightly bigger due to  |Yes, (label:"Manual Page Name")
   font change.                                                      |
-Change OK button to Save and get rid of Cancel button (in Edit      |Done
   windows). Put any old Cancel button functionality into a Close    |
   event handler.                                                    |
-Change all places where the form is called to now call the new Frm. |Done
-Test thoroughly                                                     |Can't open window anymore
-Are behavior and look absolutely identical? List any variation.     |Getting MysqlUE : "Unknown database 'jordans_mp_test"
   and minor control color variations if they are not annoying       |Grid displays a title of "Grid", columns aren't showing, window can be stretched when before it couldn't
-Copy original Form.cs into WpfControlsOD/Frms temporarily for review|
-Review with Jordan                                                  |
-Commit                                                              |
-Delete the old Winform files. That gets reviewed on the next round  |
End of Checklist=========================================================================================================================
*/
	public partial class FrmFaqPicker:FrmODBase {
		private string _manualPageUrl;
		private string _programVersion;
		private List<ManualPage> _listManualPagesUnlinked=new List<ManualPage>();

		public FrmFaqPicker(string manualUrl="", string programVersion="") {
			_manualPageUrl=manualUrl;
			_programVersion=programVersion;
			InitializeComponent();
			Load+=FrmFaqPicker_Load;
			gridODMain.CellDoubleClick+=gridODMain_CellDoubleClick;
		}

		private void FrmFaqPicker_Load(object sender,EventArgs e) {
			textManualPage.Text=_manualPageUrl;
			textManualVersion.Text=_programVersion;
			checkShowUnlinked.Checked=false;
			FillGrid();
		}

		///<summary>Fills the grid with Faq objects for the given manual page and version.
		///Should almost always call RefreshGrid() instead of this method directly.</summary>
		private void FillGrid() {
			if(!int.TryParse(textManualVersion.Text,out int version) && version!=0) {
				MsgBox.Show(this,"Please enter a valid manual version. For example, 19.1 should be entered as '191'.");
				return;
			}
			List<Faq> listFaqsForPageName=new List<Faq>();
			if(checkShowUnlinked.Checked==true) {
				listFaqsForPageName=Faqs.GetAll();
				_listManualPagesUnlinked=Faqs.GetListUnlinkedFaq();
			}
			else {
				listFaqsForPageName=Faqs.GetAllForNameAndVersion(textManualPage.Text,version);
			}
			listFaqsForPageName=Faqs.GetManualPageNamesForFaqs(listFaqsForPageName);
			gridODMain.BeginUpdate();
			gridODMain.Columns.Clear();
			GridColumn gridColumn=new GridColumn("Version",50);
			gridODMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Sticky",40,HorizontalAlignment.Center);
			gridODMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Page",100);
			gridODMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Question Text",420);
			gridColumn.IsWidthDynamic=true;
			gridODMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Created",50);
			gridODMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Edited",50);
			gridODMain.Columns.Add(gridColumn);
			gridODMain.ListGridRows.Clear();
			for(int i=0;i<listFaqsForPageName.Count();i++){
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(listFaqsForPageName[i].ManualVersion.ToString());
				gridRow.Cells.Add(listFaqsForPageName[i].IsStickied?"X":"");
				if(checkShowUnlinked.Checked==true) {
					ManualPage manualPageUnlinkedItem=_listManualPagesUnlinked.Find(x=>x.FaqNum==listFaqsForPageName[i].FaqNum);
					if(manualPageUnlinkedItem is null) {//Null because no unlinked manual page has that FaqNum
						gridRow.Cells.Add(listFaqsForPageName[i].ManualPageName);
					}
					else if(manualPageUnlinkedItem.ManualPageNum==0) {//FaqNum not linked to manual page 
						gridRow.Cells.Add("Unlinked");
					}
					else {//FaqNum is linked to a manual page that no longer exists in jordans_mp database
						gridRow.Cells.Add("Manual page does not exist");
					}
				}
				else {
					gridRow.Cells.Add(listFaqsForPageName[i].ManualPageName);
				}
				gridRow.Cells.Add(listFaqsForPageName[i].QuestionText);
				gridRow.Cells.Add(Userods.GetName(listFaqsForPageName[i].UserNumCreated));
				gridRow.Cells.Add(Userods.GetName(listFaqsForPageName[i].UserNumEdited));
				gridRow.Tag=listFaqsForPageName[i];
				gridODMain.ListGridRows.Add(gridRow);
			}
			gridODMain.EndUpdate();
		}

		/// <summary>Helper method to initialize _formFaqEdit, set its faq if given, and show FormFaqEdit.</summary>
		private void ShowFrmFaqEdit(Faq faq=null) {
			FrmFaqEdit frmFaqEdit=new FrmFaqEdit();
			if(faq is null){
				faq=new Faq();
				faq.IsNew=true;
			}
			frmFaqEdit.FaqCur=faq;
			frmFaqEdit.ShowDialog();
			FillGrid();
		}

		private void ButRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridODMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			Faq faq=null;
			if(gridODMain.SelectedGridRows!=null) {
				faq=(Faq)gridODMain.SelectedTag<Faq>();
			}
			ShowFrmFaqEdit(faq);
		}

		private void ButAdd_Click(object sender,EventArgs e) {
			ShowFrmFaqEdit();
		}
	}
}
