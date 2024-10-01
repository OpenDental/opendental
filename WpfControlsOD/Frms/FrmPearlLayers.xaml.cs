using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using OpenDentBusiness.Pearl;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmPearlLayers : FrmODBase {
		///<summary>List of EnumCategoryODs that are currently toggled on to be visible. Used in ControlImages.</summary>
		public List<EnumCategoryOD> ListEnumCategoryODsShown=new List<EnumCategoryOD>();
		///<summary>Used to refresh shown categories on ControlImages. Applied to click event of every checkbox.</summary>
		public EventHandler EventRefreshControlImages=null;
		///<summary>When set to false via the All Annotations checkbox, all Pearl annotations and toothparts are hidden from view.</summary>
		public bool ShowPearlLayers=true;
		///<summary>Used to set the starting location of the form.</summary>
		public System.Drawing.Point Location;

		///<summary></summary>
		public FrmPearlLayers() {
			InitializeComponent();
			Load+=FrmPearlLayers_Load;
		}

		#region Event Handlers
		private void FrmPearlLayers_Load(object sender, EventArgs e) {
			Lang.F(this);
			SetAllCheckBoxClickEventHandlers();
			SetAllCheckBoxTags();
			SetAllCheckBoxCheckeds();
			SetAllCheckBoxColors();
			SetAllToothPartLegendColors();
			radioShow.Checked=ShowPearlLayers;
			radioHide.Checked=!ShowPearlLayers;
			SetShowPearlLayers(ShowPearlLayers);
			_formFrame.Location=Location;
		}

		private void radioShow_Click(object sender,EventArgs e) {
			SetShowPearlLayers(true);
			EventRefreshControlImages?.Invoke(sender,e);
		}

		private void radioHide_Click(object sender,EventArgs e) {
			SetShowPearlLayers(false);
			EventRefreshControlImages?.Invoke(sender,e);
		}
		#endregion Event Handlers

		///<summary>Adds Checked event handler for every checkbox that represents an EnumCategoryOD. </summary>
		private void SetAllCheckBoxClickEventHandlers() {
			//Set checked status of each checkbox depending on whether all tag categories are included in the list of shown categories.
			for(int i=0;i<groupPathology.Items.Count;i++) {
				if(groupPathology.Items[i] is CheckBox) {
					CheckBox checkbox=(CheckBox)groupPathology.Items[i];
					checkbox.Click+=(s,e) => UpdateListForCheckBox(checkbox);
					checkbox.Click+=EventRefreshControlImages;
				}
			}
			for(int i=0;i<groupNonPathology.Items.Count;i++) {
				if(groupNonPathology.Items[i] is CheckBox) {
					CheckBox checkbox=(CheckBox)groupNonPathology.Items[i];
					checkbox.Click+=(s,e) => UpdateListForCheckBox(checkbox);
					checkbox.Click+=EventRefreshControlImages;
				}
			}
			checkPearlToothParts.Click+=(s,e) => UpdateListForCheckBox(checkPearlToothParts);
			checkPearlToothParts.Click+=EventRefreshControlImages;
		}
		
		///<summary>Sets all checkbox tags to indicate what ImageDraw categories should be hidden/shown by each checkbox.</summary>
		private void SetAllCheckBoxTags() {
			checkPearlPeriapicalRadiolucency.Tag=new List<EnumCategoryOD>() { EnumCategoryOD.PeriapicalRadiolucency };
			checkPearlCalculus.Tag=         new List<EnumCategoryOD>() { EnumCategoryOD.Calculus };
			checkPearlCariesProgressed.Tag= new List<EnumCategoryOD>() { EnumCategoryOD.CariesProgressed };
			checkPearlCariesIncipient.Tag=  new List<EnumCategoryOD>() { EnumCategoryOD.CariesIncipient };
			checkPearlNotableMargin.Tag=    new List<EnumCategoryOD>() { EnumCategoryOD.NotableMargin };
			checkPearlBridge.Tag=           new List<EnumCategoryOD>() { EnumCategoryOD.Bridge };
			checkPearlCrown.Tag=            new List<EnumCategoryOD>() { EnumCategoryOD.Crown };
			checkPearlFilling.Tag=          new List<EnumCategoryOD>() { EnumCategoryOD.Filling };
			checkPearlImplant.Tag=          new List<EnumCategoryOD>() { EnumCategoryOD.Implant };
			checkPearlRootCanal.Tag=        new List<EnumCategoryOD>() { EnumCategoryOD.RootCanal };
			checkPearlMeasurements.Tag=     new List<EnumCategoryOD>() { EnumCategoryOD.Measurements };
			checkPearlToothParts.Tag=       OpenDentBusiness.Bridges.Pearl.GetToothPartsCategoryODs();
		}

		///<summary>Set checked status of each checkbox depending on whether all of its tag categories are included in the list of shown categories.</summary>
		private void SetAllCheckBoxCheckeds() {
			for(int i=0;i<groupPathology.Items.Count;i++) {
				if(groupPathology.Items[i] is CheckBox) {
					CheckBox checkbox=(CheckBox)groupPathology.Items[i];
					SetCheckBoxFromList(checkbox);
				}
			}
			for(int i=0;i<groupNonPathology.Items.Count;i++) {
				if(groupNonPathology.Items[i] is CheckBox) {
					CheckBox checkbox=(CheckBox)groupNonPathology.Items[i];
					SetCheckBoxFromList(checkbox);
				}
			}
			SetCheckBoxFromList(checkPearlToothParts);
		}

		///<summary>Set checkbox background to legend color. Colors are given through API response but hardcoded here for simplicity. 
		///May be updated in the future to match API response.</summary>
		private void SetAllCheckBoxColors() {
			//Colors taken from Second Opinion UI
			checkPearlPeriapicalRadiolucency.ColorBack=System.Windows.Media.Color.FromRgb(31,146,252);
			checkPearlCalculus.ColorBack=         System.Windows.Media.Color.FromRgb(0,224,170);
			checkPearlCariesProgressed.ColorBack= System.Windows.Media.Color.FromRgb(196,0,91);
			checkPearlCariesIncipient.ColorBack=  System.Windows.Media.Color.FromRgb(255,0,229);
			checkPearlNotableMargin.ColorBack=    System.Windows.Media.Color.FromRgb(135,68,255);
			checkPearlBridge.ColorBack=           System.Windows.Media.Color.FromRgb(196,196,196);
			checkPearlCrown.ColorBack=            System.Windows.Media.Color.FromRgb(196,196,196);
			checkPearlFilling.ColorBack=          System.Windows.Media.Color.FromRgb(196,196,196);
			checkPearlImplant.ColorBack=          System.Windows.Media.Color.FromRgb(196,196,196);
			checkPearlRootCanal.ColorBack=        System.Windows.Media.Color.FromRgb(196,196,196);
		}

		///<summary>Sets colors in tooth parts legend. May be updated in the future to match API response.</summary>
		private void SetAllToothPartLegendColors() {
			//Colors taken from Second Opinion UI
			panelColorBone.ColorBack=        System.Windows.Media.Color.FromRgb(255,224,133);
			panelColorEnamel.ColorBack=      System.Windows.Media.Color.FromRgb(255,255,255);
			panelColorDentin.ColorBack=      System.Windows.Media.Color.FromRgb(146,247,199);
			panelColorPulp.ColorBack=        System.Windows.Media.Color.FromRgb(172,102,255);
			panelColorCementum.ColorBack=    System.Windows.Media.Color.FromRgb(133,218,255);
			panelColorRestoration.ColorBack= System.Windows.Media.Color.FromRgb(255,190,161);
		}

		///<summary>Updates ListEnumCategoryODsShown to match the state of a checkbox in this form. If the checkbox is checked, it will
		///add its tag categories to the list. If unchecked, it will remove its tag categories from the list.</summary>
		private void UpdateListForCheckBox(CheckBox checkBox) {
			if(checkBox.Checked==true) {
				//Add tag categories to list. Union ensures we only add if they don't already exist in the list, avoiding duplicates.
				ListEnumCategoryODsShown=ListEnumCategoryODsShown.Union((List<EnumCategoryOD>)checkBox.Tag).ToList();
			}
			else {
				//Remove categories from list. Except ensures we only remove if they exist in the list.
				ListEnumCategoryODsShown=ListEnumCategoryODsShown.Except((List<EnumCategoryOD>)checkBox.Tag).ToList();
			}
		}

		///<summary>Updates checkbox state. Sets unchecked if any of its tag categories are not present in 
		///ListEnumCategoryODsShown, otherwise sets checked.</summary>
		private void SetCheckBoxFromList(CheckBox checkBox) {
			List<EnumCategoryOD> listEnumCategoryODs=(List<EnumCategoryOD>)checkBox.Tag;
			//If any categories for this checkbox are not toggled on, then load checkbox as unchecked.
			for(int i=0;i<listEnumCategoryODs.Count;i++) {
				if(!ListEnumCategoryODsShown.Contains(listEnumCategoryODs[i])) {
					checkBox.Checked=false;
					return;
				}
			}
			checkBox.Checked=true;
		}

		///<summary>Sets ShowPearlLayers and IsEnabled for each of the checkbox groups.</summary>
		private void SetShowPearlLayers(bool showLayers) {
			ShowPearlLayers=showLayers;
			//Disable layer filter checkboxes when hiding all annotations.
			groupNonPathology.IsEnabled=ShowPearlLayers;
			groupPathology.IsEnabled=ShowPearlLayers;
			groupToothPartsLegend.IsEnabled=ShowPearlLayers;
		}
	}
}