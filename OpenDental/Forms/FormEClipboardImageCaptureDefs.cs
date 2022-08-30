using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEClipboardImageCaptureDefs:FormODBase {
		///<summay>Currently selected clinic for which we are editing the eclipboard image capture defs.</summay>
		private long _clinicNum;
		///<summary>List of all definitions in 'EClipboardImageCapture' defcat. </summary>
		private List<Def> _listDefsAllEClipboardImages;
		///<summary>The corresponding EClipboardImageCaptureDef objects for the list of EClipboard Images patients are allowed to take. Must be
		///set before opening this form.</summary>
		public List<EClipboardImageCaptureDef> ListEClipboardImageCaptureDefs;	
		
		
		public FormEClipboardImageCaptureDefs(long clinicNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicNum=clinicNum;
		}

		private void FormEClipboardImageCaptureDefs_Load(object sender,EventArgs e) { 
			//Get all 'EClipboardImageCapture' defcat definitions. These are the images that patients may be prompted to submit when checking in via eClipboard.
			_listDefsAllEClipboardImages=Defs.GetDefsForCategory(DefCat.EClipboardImageCapture);
			FillGrids();
		}

		///<summary>Fills both grid on this form. The left grid (Available) contains Defs. The right grid (In Use) contains EClipboardImageCaptureDefs.</summary>
		private void FillGrids() { 
			//Since 'EClipboardAllowSelfPortraitOnCheckIn' pref is now edited in this form, we have to treat the self-portrait as a def so that it behaves like the other
			//'eClipboard Images' definitions in the form. So we create this mock def for the self-portrait. This def should NOT be inserted inserted into the DB.
			//By defaul, this Def's DefNum will be 0. This will indicate this is the self-portrait def since it is not in the DB and does not have a primary key.
			Def defSelfPortrait=new Def();
			//Set the name and the description of the self-portrait.
			defSelfPortrait.ItemName="Self Portrait";
			defSelfPortrait.ItemValue="Allows patient to submit a self-portrait upon checkin";
			//If the eClipboard image def has a corresponding EClipboardImageCaptureDef, users are allowed to submit that image, so we add these defs to the 'In Use' list.
			List<Def> listDefsEClipboardImagesInUse=_listDefsAllEClipboardImages.FindAll(x => ListEClipboardImageCaptureDefs.Any(y => y.ClinicNum==_clinicNum && y.DefNum==x.DefNum));
			//If the eClipboard image def does not have a corresponding EClipboardImageCaptureDef, users are not allowed to submit that image, so we add these defs to
			//the 'available' list as long the def is not marked as 'hidden'.
			List<Def> listDefsAvailableEClipboardImages=_listDefsAllEClipboardImages.FindAll(x => !listDefsEClipboardImagesInUse.Any(y => y.DefNum==x.DefNum) && !x.IsHidden);
			bool isSelfPortraitAllowed=ListEClipboardImageCaptureDefs.Any(x => x.IsSelfPortrait && x.ClinicNum==_clinicNum);
			//If the self portrait has a correspoding EClipboardImageCaptureDef, then we add it the 'In Use' list, since users are able to submit self portraits.
			if(isSelfPortraitAllowed) { 
				listDefsEClipboardImagesInUse.Add(defSelfPortrait);
			}
			else { //Otherwise we add it to the available list since patients are not currently allowed to submit self-portraits. 
				listDefsAvailableEClipboardImages.Add(defSelfPortrait);
			}
			#region Filling available EclipboardImageCaptureDef (left grid)
			gridAvailableEClipboardImages.BeginUpdate();
      gridAvailableEClipboardImages.Columns.Clear();
      GridColumn col;
      col=new GridColumn(Lan.g(this,"Definition"),120);
      gridAvailableEClipboardImages.Columns.Add(col);
      col=new GridColumn(Lan.g(this,"ItemValue"),240);
      gridAvailableEClipboardImages.Columns.Add(col);
      gridAvailableEClipboardImages.ListGridRows.Clear();
      GridRow row;
      for(int i=0;i<listDefsAvailableEClipboardImages.Count;i++) {
				Def def=listDefsAvailableEClipboardImages[i];
        row=new GridRow();
				row.Cells.Add(def.ItemName);
				row.Cells.Add(def.ItemValue);
				row.Tag=def;
				gridAvailableEClipboardImages.ListGridRows.Add(row);
      }
      gridAvailableEClipboardImages.EndUpdate();
			#endregion
			#region Filling 'in use' EclipboardImageCaptureDefs (right grid).
			gridEClipboardImagesInUse.BeginUpdate();
      gridEClipboardImagesInUse.Columns.Clear();
      col=new GridColumn(Lan.g(this,"Definition"),120);
      gridEClipboardImagesInUse.Columns.Add(col);
      col=new GridColumn(Lan.g(this,"ItemValue"),210);
      gridEClipboardImagesInUse.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Frequency(Days)"),120);
      gridEClipboardImagesInUse.Columns.Add(col);
      gridEClipboardImagesInUse.ListGridRows.Clear();
      for(int i=0;i<listDefsEClipboardImagesInUse.Count;i++) {
				Def def=listDefsEClipboardImagesInUse[i];
				EClipboardImageCaptureDef eClipboardImageCaptureDef=new EClipboardImageCaptureDef();
				if(def.DefNum==0) {
					//This is the self portrait since the defNum is 0. So we search in our list of eclipboardimagecapturedefs for the one that marked as being the self portrait capture def
					eClipboardImageCaptureDef=ListEClipboardImageCaptureDefs.FirstOrDefault(x => x.ClinicNum==_clinicNum && x.IsSelfPortrait);
				}
				else{
					//Not the self portrait. So we search for the capturedef with the same defnum.
					eClipboardImageCaptureDef=ListEClipboardImageCaptureDefs.FirstOrDefault(x => x.ClinicNum==_clinicNum && x.DefNum==def.DefNum);
				}
				row=new GridRow();
				row.Cells.Add(def.ItemName);
				row.Cells.Add(def.ItemValue);
				row.Cells.Add(eClipboardImageCaptureDef.FrequencyDays.ToString());
				row.Tag=eClipboardImageCaptureDef;
				gridEClipboardImagesInUse.ListGridRows.Add(row);
      }
      gridEClipboardImagesInUse.EndUpdate();
			#endregion
		}

		///<summary>Moves an EClipboard image from the 'in use' grid to the 'available' grid. Indicating that patients will not be asked to submit this
		///image when checking in via eclipboard.</summary>
		private void butLeft_Click(object sender,EventArgs e) {
			List<EClipboardImageCaptureDef> listEClipboardImageCaptureDefsSelected=gridEClipboardImagesInUse.SelectedTags<EClipboardImageCaptureDef>();
			if(listEClipboardImageCaptureDefsSelected.IsNullOrEmpty()) {
				MsgBox.Show(this,"Please select at least one image from 'In Use' grid.");
				return;
			}
			//Delete all the associated EClipboardImageCaptureDefs for the eClipboard Image defs that we no longer allow users to submit.
			ListEClipboardImageCaptureDefs.RemoveAll(x => listEClipboardImageCaptureDefsSelected.Any(y => x.ClinicNum==_clinicNum && x.DefNum==y.DefNum));
			FillGrids();
		}

		///<summary>Moves an EClipboard image from the 'available' grid to the 'In Use' grid. Indicating that patients may be asked to submit this
		///image when checking in via eclipboard, if the patient has never submitted this image before, or it is outdated.</summary>
		private void butRight_Click(object sender,EventArgs e) {
			List<Def> listDefsSelected=gridAvailableEClipboardImages.SelectedTags<Def>();
			if(listDefsSelected.IsNullOrEmpty()) {
				MsgBox.Show(this,"Please select at least one image from 'Available' grid.");
				return;
			}
			//Create all the EClipboardImageCaptureDefs for the eClipboard Image defs that we now allow users to submit.
			for(int i=0;i<listDefsSelected.Count;i++) {
				bool isSelfPortrait=false;
				if(listDefsSelected[i].DefNum==0) {
					//If DefNum is 0, then this def is the dummy def we created for the self-portrait. So we need to mark this capturedef as being the self-portrait.
					isSelfPortrait=true;
					//We will also set the defnum to the image category that has the patient pictures usage, if any. 
					listDefsSelected[i].DefNum=OpenDentBusiness.Defs.GetDefsForCategory(OpenDentBusiness.DefCat.ImageCats,true).FirstOrDefault(x => x.ItemValue.Contains("P"))?.DefNum??0;
				}
				EClipboardImageCaptureDef eClipboardImageCaptureDef=new EClipboardImageCaptureDef();
				eClipboardImageCaptureDef.DefNum=listDefsSelected[i].DefNum;
				eClipboardImageCaptureDef.IsSelfPortrait=isSelfPortrait;
				eClipboardImageCaptureDef.FrequencyDays=0;
				eClipboardImageCaptureDef.ClinicNum=_clinicNum;
				ListEClipboardImageCaptureDefs.Add(eClipboardImageCaptureDef);
			}
			FillGrids();
		}

		///<summary>Opens an input box with a textbox for the user to set the frequency at which patients will be prompted to submit the selected image.</summary>
		private void gridEClipboardImagesInUse_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			EClipboardImageCaptureDef eClipboardImageCaptureDefSelected=gridEClipboardImagesInUse.SelectedTag<EClipboardImageCaptureDef>();
			if(eClipboardImageCaptureDefSelected==null) {
				return;
			}
			//Label for the single textbox on our inputbox
			string inputBoxMessage=Lan.g(this,"How often should the patient be prompted to resubmit this image (In days, where 0 or blank indicates at each checkin)?");
			//Single textbox for the user to input their desired frequency 
			InputBoxParam inputBoxParam=new InputBoxParam(InputBoxType.TextBox,inputBoxMessage,text:eClipboardImageCaptureDefSelected.FrequencyDays.ToString());
			//Func that will be called when the user clicks 'Ok' on our inputbux. Verifies the inputted text can be converted to int and the int is >= 0. Blank is converted to 0.
			Func<string,bool> funcOkClick=new Func<string,bool>((text) => {
				int frequency;
				try {
					frequency=PIn.Int(text);
				}
				catch {
					MsgBox.Show(this, "Frequency (days) must be a valid whole number, 0 or greater.");
					return false;
				}
				if(frequency<0) {
					MsgBox.Show(this,"Frequency (days) must be a valid whole number, 0 or greater.");
					return false;
				}
				return true;
			});
			using InputBox inputBox=new InputBox(funcOkClick,inputBoxParam);
			inputBox.setTitle(Lan.g(this,"Image Capture Frequency (Days)"));
			inputBox.SizeInitial=new Size(500,170);
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//Update the frequency for the EClipboardImageCaptureDef the user selected.
			EClipboardImageCaptureDef eClipboardImageCaptureDef=ListEClipboardImageCaptureDefs.Find(x => x.ClinicNum==_clinicNum && x.DefNum==eClipboardImageCaptureDefSelected.DefNum);
			if(eClipboardImageCaptureDef!=null) { 
				eClipboardImageCaptureDef.FrequencyDays=PIn.Int(inputBox.textResult.Text);
			}
			FillGrids();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}