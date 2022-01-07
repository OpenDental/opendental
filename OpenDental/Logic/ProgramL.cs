using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using OpenDental.Bridges;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing;
using System.IO;
using CodeBase;
using System.Linq;
using xBridges=Bridges;//Bridges is ambiguous with OpenDental.Bridges
using System.Windows;
using System.Text;

namespace OpenDental{
	

	///<summary></summary>
	public class ProgramL{

		///<summary>Typically used when user clicks a button to a Program link.  This method attempts to identify and execute the program based on the given programNum.</summary>
		public static void Execute(long programNum,Patient pat) {
			Program prog=Programs.GetFirstOrDefault(x => x.ProgramNum==programNum);
			if(prog==null) {//no match was found
				MsgBox.Show("Program","Error, program entry not found in database.");
				return;
			}
			if(!Programs.IsEnabledByHq(prog,out string err)) {
				MessageBox.Show(err);
				return;
			}
			if(pat!=null && PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				pat=Patients.GetOriginalPatientForClone(pat);
			}
			if(prog.PluginDllName!="") {
				if(pat==null) {
					Plugins.LaunchToolbarButton(programNum,0);
				}
				else{
					Plugins.LaunchToolbarButton(programNum,pat.PatNum);
				}
				return;
			}
			if(ODBuild.IsWeb() && ListTools.In(prog.ProgName,Programs.GetListDisabledForWeb().Select(x => x.ToString()))) {
				MsgBox.Show("ProgramLinks","Bridge is not available while viewing through the web.");
				return;//bridge is not available for web users at this time. 
			}
			if(prog.ProgName==ProgramName.ActeonImagingSuite.ToString()) {
				ActeonImagingSuite.SendData(prog,pat);
				return;
			}
			if(prog.ProgName==ProgramName.Adstra.ToString()) {
				Adstra.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Apixia.ToString()) {
				Apixia.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Apteryx.ToString()) {
				Apteryx.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.AudaxCeph.ToString()) {
				AudaxCeph.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.BencoPracticeManagement.ToString()) {
				Benco.SendData(prog);
				return;
			}
			else if(prog.ProgName==ProgramName.BioPAK.ToString()) {
				BioPAK.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.CADI.ToString()) {
				CADI.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Camsight.ToString()) {
				Camsight.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.CaptureLink.ToString()) {
				CaptureLink.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.CareCredit.ToString()) {
				if(!prog.Enabled) {
					xBridges.CareCredit.ShowPage(xBridges.CareCredit.ProviderSignupURL);
					return;
				}
				if(pat==null) {
					MsgBox.Show("ProgramLinks","No patient selected.");
					return;
				}
				using FormCareCredit FormCareCredit=new FormCareCredit(pat.PatNum);
				FormCareCredit.ShowDialog();
				return;
			}
			else if(prog.ProgName==ProgramName.Carestream.ToString()) {
				Carestream.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Cerec.ToString()) {
				Cerec.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.CleaRay.ToString()) {
				CleaRay.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.CliniView.ToString()) {
				Cliniview.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.ClioSoft.ToString()) {
				ClioSoft.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.DBSWin.ToString()) {
				DBSWin.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.DemandForce.ToString()) {
				DemandForce.SendData(prog,pat);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(prog.ProgName==ProgramName.DentalEye.ToString()) {
				DentalEye.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.DentalStudio.ToString()) {
				DentalStudio.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.DentX.ToString()) {
				DentX.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.DrCeph.ToString()) {
				DrCeph.SendData(prog,pat);
				return;
			}
#endif
			else if(prog.ProgName==ProgramName.DentalTekSmartOfficePhone.ToString()) {
				DentalTek.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.DentForms.ToString()) {
				DentForms.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Dexis.ToString()) {
				Dexis.SendData(prog,pat);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(prog.ProgName==ProgramName.DexisIntegrator.ToString()) {
				DexisIntegrator.SendData(prog,pat);
				return;
			}
#endif
			else if(prog.ProgName==ProgramName.Digora.ToString()) {
				Digora.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Dimaxis.ToString()) {
				Planmeca.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Office.ToString()) {
				Office.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Dolphin.ToString()) {
				Dolphin.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.DXCPatientCreditScore.ToString()) {
				DentalXChange.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Dxis.ToString()) {
				Dxis.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.EvaSoft.ToString()) {
				EvaSoft.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.EwooEZDent.ToString()) {
				Ewoo.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.FloridaProbe.ToString()) {
				FloridaProbe.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Guru.ToString()) {
				Guru.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.HandyDentist.ToString()) {
				HandyDentist.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.HouseCalls.ToString()) {
				using FormHouseCalls FormHC=new FormHouseCalls();
				FormHC.ProgramCur=prog;
				FormHC.ShowDialog();
				return;
			}
			else if(prog.ProgName==ProgramName.iCat.ToString()) {
				ICat.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.HdxWill.ToString()) {
				HdxWill.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.iDixel.ToString()) {
				iDixel.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.ImageFX.ToString() || prog.ProgName==ProgramName.PatientGallery.ToString()) {
				ImageFX.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.iRYS.ToString()) {
				Irys.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Lightyear.ToString()) {
				Lightyear.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.NewTomNNT.ToString()) {
				NewTomNNT.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.MediaDent.ToString()) {
				MediaDent.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Midway.ToString()) {
				Midway.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.MiPACS.ToString()) {
				MiPACS.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.OrthoCAD.ToString()) {
				OrthoCad.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Oryx.ToString()) {
				Oryx.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.OrthoInsight3d.ToString()) {
				OrthoInsight3d.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Owandy.ToString()) {
				Owandy.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.PandaPerio.ToString()) {
				PandaPerio.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.PandaPerioAdvanced.ToString()) {
				PandaPerioAdvanced.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Patterson.ToString()) {
				Patterson.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.PDMP.ToString() || prog.ProgName==ProgramName.Appriss.ToString()) {
				PDMP pdmp=null;
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() => pdmp=PDMP.SendData(prog,pat,Providers.GetProv(Security.CurUser.ProvNum));
				progressOD.StartingMessage=Lans.g("PDMP","Fetching data...");
				try {
					progressOD.ShowDialogProgress();
					bool hasUrl=!string.IsNullOrWhiteSpace(pdmp.Url);				
					if(prog.ProgName==ProgramName.PDMP.ToString()) {
						if(hasUrl) {//Logicoy errors throw exceptions.
							RxPats.CreatePdmpAccessLog(pat,Security.CurUser,prog);
							FormWebBrowser formWebBrowser=new FormWebBrowser(pdmp.Url);
							formWebBrowser.Show();
						}
						else {
							throw new ApplicationException(Lans.g("PDMP","Unable to get report URL."));
						}
					}
					else {
						MessageBoxButtons options=MessageBoxButtons.OKCancel;
						StringBuilder result=new StringBuilder();
						if(!string.IsNullOrWhiteSpace(pdmp.Message)) {
							result.AppendLine(pdmp.Message);
							if(!hasUrl) {
								result.Append(Lans.g("PDMP","No report URL retrieved from Appriss."));
								options=MessageBoxButtons.OK;
							}
						}
						if(MessageBox.Show(result.ToString(),Lans.g("PDMP","Appriss"),options)==DialogResult.OK && hasUrl) {
							RxPats.CreatePdmpAccessLog(pat,Security.CurUser,prog);
							FormWebBrowser formWebBrowser=new FormWebBrowser(pdmp.Url);
							formWebBrowser.IsUrlSingleUse=true;
							formWebBrowser.Show();
						}
					}
				}
				catch(ApplicationException aex) {
					MsgBox.Show(Lans.g("PDMP","An error occurred while loading ")+prog.ProgName+"\n"+aex.Message);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lans.g("PDMP","An error occurred while loading ")+prog.ProgName+". \n"+ex.Message,ex);
				}
				return;
			}
			else if(prog.ProgName==ProgramName.PerioPal.ToString()) {
				PerioPal.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.PracticeByNumbers.ToString()) {
				PracticeByNumbers.ShowPage();
				return;
			}
			else if(prog.ProgName==ProgramName.PreXionAcquire.ToString()) {
				PreXion.SendDataAcquire(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.PreXionViewer.ToString()) {
				PreXion.SendDataViewer(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Progeny.ToString()) {
				Progeny.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.PT.ToString()) {
				PaperlessTechnology.SendData(prog,pat,false);
				return;
			}
			else if(prog.ProgName==ProgramName.PTupdate.ToString()) {
				PaperlessTechnology.SendData(prog,pat,true);
				return;
			}
			else if(prog.ProgName==ProgramName.RayBridge.ToString()) {
				RayBridge.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.RayMage.ToString()) {
				RayMage.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Romexis.ToString()) {
				Romexis.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Scanora.ToString()) {
				Scanora.SendData(prog,pat);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(prog.ProgName==ProgramName.Schick.ToString()) {
				Schick.SendData(prog,pat);
				return;
			}
#endif
			else if(prog.ProgName==ProgramName.Sirona.ToString()) {
				Sirona.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.SMARTDent.ToString()) {
				SmartDent.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Sopro.ToString()) {
				Sopro.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.ThreeShape.ToString()) {
				ThreeShape.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.TigerView.ToString()) {
				TigerView.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Triana.ToString()) {
				Triana.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.TrojanExpressCollect.ToString()) {
				using(FormTrojanCollect FormT=new FormTrojanCollect(pat)) {
					FormT.ShowDialog();
				}
				return;
			}
			else if(prog.ProgName==ProgramName.Trophy.ToString()) {
				Trophy.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.TrophyEnhanced.ToString()) {
				TrophyEnhanced.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.Tscan.ToString()) {
				Tscan.SendData(prog,pat);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(prog.ProgName==ProgramName.Vipersoft.ToString()) {
				Vipersoft.SendData(prog,pat);
				return;
			}
#endif
			else if(prog.ProgName==ProgramName.visOra.ToString()) {
				Visora.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.VisionX.ToString()) {
				VisionX.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.VistaDent.ToString()) {
				VistaDent.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.VixWin.ToString()) {
				VixWin.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.VixWinBase36.ToString()) {
				VixWinBase36.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.VixWinBase41.ToString()) {
				VixWinBase41.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.VixWinNumbered.ToString()) {
				VixWinNumbered.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.VixWinOld.ToString()) {
				VixWinOld.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.XDR.ToString()) {
				XDR.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.XVWeb.ToString()) {
				XVWeb.SendData(prog,pat);
				return;
			}
			else if(prog.ProgName==ProgramName.ZImage.ToString()) {
				ZImage.SendData(prog,pat);
				return;
			}
			//all remaining programs:
			try{
				string cmdline=prog.CommandLine;
				string path=Programs.GetProgramPath(prog);
				string outputFilePath=prog.FilePath;
				string fileTemplate=prog.FileTemplate;
				if(pat!=null) {
					cmdline=ReplaceHelper(cmdline,pat);
					path=ReplaceHelper(path,pat);
					if(!String.IsNullOrEmpty(outputFilePath) && !String.IsNullOrEmpty(fileTemplate)) {
						fileTemplate=ReplaceHelper(fileTemplate,pat);
						fileTemplate=fileTemplate.Replace("\n","\r\n");
						ODFileUtils.WriteAllTextThenStart(outputFilePath,fileTemplate,path,cmdline);
						//Nothing else to do so return;
						return;
					}
				}
				//We made it this far and we haven't started the program yet so start it.
				ODFileUtils.ProcessStart(path,cmdline);
			}
			catch(Exception e){
				FriendlyException.Show(prog.ProgDesc+" is not available.",e);
				return;
			}
		}

		///<summary>Helper method that replaces the message with all of the Message Replacements available for ProgramLinks.</summary>
		private static string ReplaceHelper(string message,Patient pat) {
			string retVal=message;
			retVal=Patients.ReplacePatient(retVal,pat);
			retVal=Patients.ReplaceGuarantor(retVal,pat);
			retVal=Referrals.ReplaceRefProvider(retVal,pat);
			return retVal;
		}

		public static void LoadToolbar(ToolBarOD toolBar,ToolBarsAvail toolBarsAvail) {
			List<ToolButItem> toolButItems=ToolButItems.GetForToolBar(toolBarsAvail);
			foreach(ToolButItem toolButItemCur in toolButItems) { 
				Program programCur=Programs.GetProgram(toolButItemCur.ProgramNum);
				if(PrefC.HasClinicsEnabled) {
					//User should not have PL hidden if Clinics are not Enabled, otherwise this could create a situation where users may turn clinics off but 
					//have hidden the PL button for HQ and then be unable to turn the button back on without re-enabling Clinics.
					ProgramProperty programProp=ProgramProperties.GetPropForProgByDesc(programCur.ProgramNum
						,ProgramProperties.PropertyDescs.ClinicHideButton,Clinics.ClinicNum);
					if(programProp!=null) {
						continue;//If there exists a programProp for a clinic which should have its buttons hidden, carry on and do not display the button.
					}
				}
				if(ProgramProperties.IsAdvertisingDisabled(programCur)) {
					continue;
				}
				string key=programCur.ProgramNum.ToString()+programCur.ProgName.ToString();
				if(toolBar.ImageList.Images.ContainsKey(key)) {
					//Dispose the existing image only if it already exists, because the image might have changed.
					toolBar.ImageList.Images[toolBar.ImageList.Images.IndexOfKey(key)].Dispose();
					toolBar.ImageList.Images.RemoveByKey(key);
				}
				if(programCur.ButtonImage!="") {
					Image image=PIn.Bitmap(programCur.ButtonImage);
					toolBar.ImageList.Images.Add(key,image);
				}
				else if(programCur.ProgName==ProgramName.Midway.ToString()) {
					Image image=global::OpenDental.Properties.Resources.Midway_Icon_22x22;
					toolBar.ImageList.Images.Add(key,image);
				}
				if(toolBarsAvail!=ToolBarsAvail.MainToolbar) {
					toolBar.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				}
				ODToolBarButton button=new ODToolBarButton(toolButItemCur.ButtonText,-1,"",programCur);
				AddDropDown(button,programCur);
				toolBar.Buttons.Add(button);
			}
			for(int i=0;i<toolBar.Buttons.Count;i++) {//Reset the new index, because it might have changed due to removing/adding to the Images list.
				if(toolBar.Buttons[i].Tag.GetType()!=typeof(Program)) {
					continue;
				}
				Program programCur=(Program)toolBar.Buttons[i].Tag;
				string key=programCur.ProgramNum.ToString()+programCur.ProgName.ToString();
				if(toolBar.ImageList.Images.ContainsKey(key)) {
					toolBar.Buttons[i].ImageIndex=toolBar.ImageList.Images.IndexOfKey(key);
				}
			}
		}

		///<summary>Adds a drop down menu if this program requires it.</summary>
		private static void AddDropDown(ODToolBarButton toolBarButton,Program programCur) {
			if(programCur.ProgName==ProgramName.Oryx.ToString()) {
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
			else if(programCur.ProgName==ProgramName.CareCredit.ToString()) {
				if(Programs.IsEnabled(ProgramName.CareCredit)) {
					return; //no need to create the drop down if CareCredit is already enabled
				}
				ContextMenu contextMenuCareCredit=new ContextMenu();
				MenuItem menuItem=new MenuItem();
				menuItem.Index=0;
				menuItem.Text=Lans.g("CareCredit","Disable Advertising");
				menuItem.Click+=(s,e) => {
					List<ProgramProperty> listProgProps=ProgramProperties.GetForProgram(programCur.ProgramNum);
					listProgProps.FindAll(x => x.PropertyDesc==ProgramProperties.PropertyDescs.CareCredit.CareCreditDoDisableAdvertising)
					.ForEach(x => x.PropertyValue=POut.Bool(true));//sets the hide advertising program property to true
					ProgramProperties.Sync(listProgProps,programCur.ProgramNum);
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