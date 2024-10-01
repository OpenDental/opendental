using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.Bridges;
using OpenDental.UI;

namespace OpenDental{
	
	///<summary></summary>
	public class ProgramL{
		///<summary>Any changes here should also be made in WpfControls.ProgramL.LoadToolBar.</summary>
		public static void LoadToolBar(ToolBarOD toolBar,EnumToolBar toolBarsAvail) {
			List<ToolButItem> listToolButItems=ToolButItems.GetForToolBar(toolBarsAvail);
			for(int i=0;i<listToolButItems.Count;i++){
				Program program=Programs.GetProgram(listToolButItems[i].ProgramNum);
				if(PrefC.HasClinicsEnabled) {
					//User should not have PL hidden if Clinics are not Enabled, otherwise this could create a situation where users may turn clinics off but 
					//have hidden the PL button for HQ and then be unable to turn the button back on without re-enabling Clinics.
					ProgramProperty programProperty=ProgramProperties.GetPropForProgByDesc(program.ProgramNum
						,ProgramProperties.PropertyDescs.ClinicHideButton,Clinics.ClinicNum);
					if(programProperty!=null) {
						continue;//If there exists a programProp for a clinic which should have its buttons hidden, carry on and do not display the button.
					}
				}
				if(ProgramProperties.IsAdvertisingDisabled(program)) {
					continue;
				}
				string strProgramNum=program.ProgramNum.ToString()+program.ProgName.ToString();
				if(toolBar.ImageList.Images.ContainsKey(strProgramNum)) {
					//Dispose the existing image only if it already exists, because the image might have changed.
					toolBar.ImageList.Images[toolBar.ImageList.Images.IndexOfKey(strProgramNum)].Dispose();
					toolBar.ImageList.Images.RemoveByKey(strProgramNum);
				}
				if(program.ButtonImage!="") {
					Image image=PIn.Bitmap(program.ButtonImage);
					toolBar.ImageList.Images.Add(strProgramNum,image);
				}
				else if(program.ProgName==ProgramName.PracticeBooster.ToString()) {
					Image image=global::OpenDental.Properties.Resources.Practice_Booster_Icon_22x22;
					toolBar.ImageList.Images.Add(strProgramNum,image);
				}
				if(toolBarsAvail!=EnumToolBar.MainToolbar) {
					toolBar.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				}
				ODToolBarButton toolBarButton=new ODToolBarButton(listToolButItems[i].ButtonText,-1,"",program);
				AddDropDown(toolBarButton,program);
				toolBar.Buttons.Add(toolBarButton);
			}
			for(int i=0;i<toolBar.Buttons.Count;i++) {//Reset the new index, because it might have changed due to removing/adding to the Images list.
				if(toolBar.Buttons[i].Tag.GetType()!=typeof(Program)) {
					continue;
				}
				Program program=(Program)toolBar.Buttons[i].Tag;
				string strProgramNum=program.ProgramNum.ToString()+program.ProgName.ToString();
				if(toolBar.ImageList.Images.ContainsKey(strProgramNum)) {
					toolBar.Buttons[i].ImageIndex=toolBar.ImageList.Images.IndexOfKey(strProgramNum);
				}
			}
		}

		///<summary>Adds a drop down menu if this program requires it.</summary>
		private static void AddDropDown(ODToolBarButton toolBarButton,Program program) {
			if(program.ProgName==ProgramName.Oryx.ToString()) {
				ContextMenu contextMenuOryx=new ContextMenu();
				MenuItem menuItemUserSettings=new MenuItem();
				menuItemUserSettings.Index=0;
				menuItemUserSettings.Text=Lans.g("Oryx","User Settings");
				menuItemUserSettings.Click+=Oryx.menuItemUserSettingsClick;
				contextMenuOryx.MenuItems.AddRange(new MenuItem[] {
						menuItemUserSettings,
				});
				toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
				toolBarButton.DropDownMenu=contextMenuOryx;
			}
			else if(program.ProgName==ProgramName.CareCredit.ToString()) {
				if(Programs.IsEnabled(ProgramName.CareCredit)) {
					return; //no need to create the drop down if CareCredit is already enabled
				}
				ContextMenu contextMenuCareCredit=new ContextMenu();
				MenuItem menuItem=new MenuItem();
				menuItem.Index=0;
				menuItem.Text=Lans.g("CareCredit","Disable Advertising");
				menuItem.Click+=(s,e) => {
					List<ProgramProperty> listProgProps=ProgramProperties.GetForProgram(program.ProgramNum);
					for(int i=0;i<listProgProps.Count;i++){
						if(listProgProps[i].PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditDoDisableAdvertising){ 
							listProgProps[i].PropertyValue=POut.Bool(true);
						}
					}
					ProgramProperties.Sync(listProgProps,program.ProgramNum);
					DataValid.SetInvalid(InvalidType.Programs, InvalidType.ToolButsAndMounts);
				};
				contextMenuCareCredit.MenuItems.AddRange(new MenuItem[] {
					menuItem
				});
				toolBarButton.Style=ODToolBarButtonStyle.DropDownButton;
				toolBarButton.DropDownMenu=contextMenuCareCredit;
			}
		}
	}
}