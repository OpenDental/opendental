using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControls.UI;
using OpenDentBusiness;
using OpenDental;
using OpenDental.Bridges;
using CodeBase;
using xBridges=Bridges;//Bridges is ambiguous with OpenDental.Bridges

namespace WpfControls {
	public class ProgramL {
		///<summary>Typically used when user clicks a button to a Program link.  This method attempts to identify and execute the program based on the given programNum.</summary>
		public static void Execute(long programNum,Patient patient) {
			Program program=Programs.GetFirstOrDefault(x => x.ProgramNum==programNum);
			if(program==null) {//no match was found
				MsgBox.Show("Program","Error, program entry not found in database.");
				return;
			}
			if(!Programs.IsEnabledByHq(program,out string err)) {
				MsgBox.Show(err);
				return;
			}
			if(patient!=null && PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				patient=Patients.GetOriginalPatientForClone(patient);
			}
			if(program.PluginDllName!="") {
				if(patient==null) {
					Plugins.LaunchToolbarButton(programNum,0);
				}
				else{
					Plugins.LaunchToolbarButton(programNum,patient.PatNum);
				}
				return;
			}
			if(ODEnvironment.IsCloudServer && Programs.GetListDisabledForWeb().Select(x => x.ToString()).Contains(program.ProgName)) {
				MsgBox.Show("ProgramLinks","Bridge is not available while using Open Dental Cloud.");
				return;//bridge is not available for web users at this time. 
			}
			if(program.ProgName==ProgramName.ActeonImagingSuite.ToString()) {
				ActeonImagingSuite.SendData(program,patient);
				return;
			}
			if(program.ProgName==ProgramName.Adstra.ToString()) {
				Adstra.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.AiDental.ToString()) {
				AiDental.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Apixia.ToString()) {
				Apixia.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Apteryx.ToString()) {
				Apteryx.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.AudaxCeph.ToString()) {
				AudaxCeph.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.BencoPracticeManagement.ToString()) {
				Benco.SendData(program);
				return;
			}
			else if(program.ProgName==ProgramName.BioPAK.ToString()) {
				BioPAK.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.CADI.ToString()) {
				CADI.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Camsight.ToString()) {
				Camsight.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.CaptureLink.ToString()) {
				CaptureLink.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.CareCredit.ToString()) {
				if(!program.Enabled) {
					xBridges.CareCredit.ShowPage(xBridges.CareCredit.ProviderSignupURL);
					return;
				}
				if(patient==null) {
					MsgBox.Show("ProgramLinks","No patient selected.");
					return;
				}
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormCareCredit);
				formLauncher.SetField("PatientCur",patient);
				formLauncher.ShowDialog();
				return;
			}
			else if(program.ProgName==ProgramName.Carestream.ToString()) {
				Carestream.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Cerec.ToString()) {
				Cerec.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.CleaRay.ToString()) {
				CleaRay.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.CliniView.ToString()) {
				Cliniview.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.ClioSoft.ToString()) {
				ClioSoft.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.DBSWin.ToString()) {
				DBSWin.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.DemandForce.ToString()) {
				DemandForce.SendData(program,patient);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(program.ProgName==ProgramName.DentalEye.ToString()) {
				DentalEye.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.DentalStudio.ToString()) {
				DentalStudio.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.DentX.ToString()) {
				DentX.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.DrCeph.ToString()) {
				DrCeph.SendData(program,patient);
				return;
			}
#endif
			else if(program.ProgName==ProgramName.DentalTekSmartOfficePhone.ToString()) {
				DentalTek.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.DentForms.ToString()) {
				DentForms.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Dexis.ToString()) {
				Dexis.SendData(program,patient);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(program.ProgName==ProgramName.DexisIntegrator.ToString()) {
				DexisIntegrator.SendData(program,patient);
				return;
			}
#endif
			else if(program.ProgName==ProgramName.Digora.ToString()) {
				Digora.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Dimaxis.ToString()) {
				Planmeca.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Office.ToString()) {
				Office.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Dolphin.ToString()) {
				Dolphin.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.DXCPatientCreditScore.ToString()) {
				OpenDental.Bridges.DentalXChange.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Dxis.ToString()) {
				Dxis.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.EvaSoft.ToString()) {
				EvaSoft.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.EwooEZDent.ToString()) {
				Ewoo.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.FloridaProbe.ToString()) {
				FloridaProbe.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Guru.ToString()) {
				Guru.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.HandyDentist.ToString()) {
				HandyDentist.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.HouseCalls.ToString()) {
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormHouseCalls);
				formLauncher.SetField("ProgramCur",program);
				formLauncher.ShowDialog();
				return;
			}
			else if(program.ProgName==ProgramName.iCat.ToString()) {
				ICat.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.HdxWill.ToString()) {
				HdxWill.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.iDixel.ToString()) {
				iDixel.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.ImageFX.ToString() || program.ProgName==ProgramName.PatientGallery.ToString()) {
				ImageFX.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.iRYS.ToString()) {
				Irys.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.JazzClassicCapture.ToString()) {
				JazzClassicCapture.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.JazzClassicExamView.ToString()) {
				JazzClassicExamView.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.JazzClassicPatientUpdate.ToString()) {
				JazzClassicPatientUpdate.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Lightyear.ToString()) {
				Lightyear.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.NewTomNNT.ToString()) {
				NewTomNNT.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.MediaDent.ToString()) {
				MediaDent.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.MeditLink.ToString()) {
				MeditLink.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Midway.ToString()) {
				Midway.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.MiPACS.ToString()) {
				MiPACS.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.One2.ToString()){
				One2.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.OrthoCAD.ToString()) {
				OrthoCad.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Oryx.ToString()) {
				Oryx.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.OrthoInsight3d.ToString()) {
				OrthoInsight3d.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Owandy.ToString()) {
				Owandy.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.PandaPerio.ToString()) {
				PandaPerio.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.PandaPerioAdvanced.ToString()) {
				PandaPerioAdvanced.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Patterson.ToString()) {
				Patterson.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.PDMP.ToString() || program.ProgName==ProgramName.Appriss.ToString()) {
				PDMP pDMP=null;
				OpenDental.UI.ProgressWin progressWin=new OpenDental.UI.ProgressWin();
				Provider provider = Providers.GetProv(Security.CurUser.ProvNum);
				progressWin.ActionMain=() => pDMP=PDMP.SendData(program,patient,provider);
				progressWin.StartingMessage=Lans.g("PDMP","Fetching data...");
				try {
					progressWin.ShowDialog();
					bool hasUrl=!string.IsNullOrWhiteSpace(pDMP.Url);				
					if(program.ProgName==ProgramName.PDMP.ToString()) {
						if(hasUrl) {//Logicoy errors throw exceptions.
							RxPats.CreatePdmpAccessLog(patient,Security.CurUser,program);
							FormLauncher formLauncher=new FormLauncher(EnumFormName.FormWebBrowser);
							formLauncher.SetField("UrlBrowseTo",pDMP.Url);
							formLauncher.Show();
						}
						else {
							throw new ApplicationException(Lans.g("PDMP","Unable to get report URL."));
						}
					}
					else {
						bool showCancel=true;
						StringBuilder stringBuilder=new StringBuilder();
						if(!string.IsNullOrWhiteSpace(pDMP.Message)) {
							stringBuilder.AppendLine(pDMP.Message);
							if(!hasUrl) {
								stringBuilder.Append(Lans.g("PDMP","No report URL retrieved from Appriss."));
								showCancel=false;
							}
						}
						bool isOK=false;
						if(showCancel){
							isOK=MsgBox.Show(MsgBoxButtons.OKCancel,stringBuilder.ToString(),Lans.g("PDMP","Appriss"));
						}
						else{
							MsgBox.Show(stringBuilder.ToString());
							isOK=true;
						}
						if(isOK && hasUrl) {
							RxPats.CreatePdmpAccessLog(patient,Security.CurUser,program);
							FormLauncher formLauncher=new FormLauncher(EnumFormName.FormWebBrowser);
							formLauncher.SetField("UrlBrowseTo",pDMP.Url);
							formLauncher.SetField("IsUrlSingleUse",true);
							formLauncher.Show();
						}
					}
				}
				catch(ApplicationException appEx) {
					MsgBox.Show(Lans.g("PDMP","An error occurred while loading ")+program.ProgName+"\n"+appEx.Message);
				}
				catch(Exception ex) {
					FriendlyException.Show(Lans.g("PDMP","An error occurred while loading ")+program.ProgName+". \n"+ex.Message,ex);
				}
				return;
			}
			else if(program.ProgName==ProgramName.Pearl.ToString()) {
				//Pearl is handled separately in ControlImagesJ
				return;
			}
			else if(program.ProgName==ProgramName.PerioPal.ToString()) {
				PerioPal.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Pixel.ToString()) {
				//Pixel is treated like a generic program link which gets handled down below.
			}
			else if(program.ProgName==ProgramName.PORTRAY.ToString()) {
				PORTRAY.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.PracticeBooster.ToString()) {
				PracticeBooster.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.PracticeByNumbers.ToString()) {
				PracticeByNumbers.ShowPage();
				return;
			}
			else if(program.ProgName==ProgramName.PreXionAcquire.ToString()) {
				PreXion.SendDataAcquire(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.PreXionViewer.ToString()) {
				PreXion.SendDataViewer(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Progeny.ToString()) {
				Progeny.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.PT.ToString()) {
				PaperlessTechnology.SendData(program,patient,false);
				return;
			}
			else if(program.ProgName==ProgramName.PTupdate.ToString()) {
				PaperlessTechnology.SendData(program,patient,true);
				return;
			}
			else if(program.ProgName==ProgramName.RayBridge.ToString()) {
				RayBridge.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.RayMage.ToString()) {
				RayMage.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Romexis.ToString()) {
				Romexis.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Scanora.ToString()) {
				Scanora.SendData(program,patient);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(program.ProgName==ProgramName.Schick.ToString()) {
				Schick.SendData(program,patient);
				return;
			}
#endif
			else if(program.ProgName==ProgramName.Shining3D.ToString()) {
				Shining3D.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Sirona.ToString()) {
				Sirona.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.SMARTDent.ToString()) {
				SmartDent.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Sopro.ToString()) {
				Sopro.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.SOTACloud.ToString()) {
				SOTACloud.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.SteriSimple.ToString()) {
				SteriSimple.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.ThreeShape.ToString()) {
				ThreeShape.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.TigerView.ToString()) {
				TigerView.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Triana.ToString()) {
				Triana.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.TrojanExpressCollect.ToString()) {
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormTrojanCollect);
				formLauncher.SetField("PatientCur",patient);
				formLauncher.ShowDialog();
				return;
			}
			else if(program.ProgName==ProgramName.Trophy.ToString()) {
				Trophy.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.TrophyEnhanced.ToString()) {
				TrophyEnhanced.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.Tscan.ToString()) {
				Tscan.SendData(program,patient);
				return;
			}
#if !DISABLE_WINDOWS_BRIDGES
			else if(program.ProgName==ProgramName.Vipersoft.ToString()) {
				Vipersoft.SendData(program,patient);
				return;
			}
#endif
			else if(program.ProgName==ProgramName.visOra.ToString()) {
				Visora.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.VisionX.ToString()) {
				VisionX.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.VistaDent.ToString()) {
				VistaDent.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.VixWin.ToString()) {
				VixWin.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.VixWinBase36.ToString()) {
				VixWinBase36.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.VixWinBase41.ToString()) {
				VixWinBase41.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.VixWinNumbered.ToString()) {
				VixWinNumbered.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.VixWinOld.ToString()) {
				VixWinOld.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.XDR.ToString()) {
				XDR.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.XVWeb.ToString()) {
				XVWeb.SendData(program,patient);
				return;
			}
			else if(program.ProgName==ProgramName.ZImage.ToString()) {
				ZImage.SendData(program,patient);
				return;
			}
			//all remaining programs:
			string cmdline=program.CommandLine;
			string path=Programs.GetProgramPath(program);
			string outputFilePath=program.FilePath;
			string fileTemplate=program.FileTemplate;
			if(patient!=null) {
				cmdline=ReplaceHelper(cmdline,patient);
				path=ReplaceHelper(path,patient);
				if(!String.IsNullOrEmpty(outputFilePath) && !String.IsNullOrEmpty(fileTemplate)) {
					fileTemplate=ReplaceHelper(fileTemplate,patient);
					fileTemplate=fileTemplate.Replace("\n","\r\n");
					try{
						ODFileUtils.WriteAllTextThenStart(outputFilePath,fileTemplate,path,cmdline);
					}
					catch(Exception e){
						FriendlyException.Show(program.ProgDesc+" is not available.",e);
						return;
					}
					//Nothing else to do so return;
					return;
				}
			}
			try{	
				//We made it this far and we haven't started the program yet so start it.
				ODFileUtils.ProcessStart(path,cmdline);
			}
			catch(Exception e){
				FriendlyException.Show(program.ProgDesc+" is not available.",e);
				return;
			}
		}

		///<summary>Helper method that replaces the message with all of the Message Replacements available for ProgramLinks.</summary>
		private static string ReplaceHelper(string message,Patient patient) {
			string retVal=message;
			retVal=Patients.ReplacePatient(retVal,patient);
			retVal=Patients.ReplaceGuarantor(retVal,patient);
			retVal=Referrals.ReplaceRefProvider(retVal,patient);
			retVal=retVal.Replace("[UserName]",Security.CurUser.UserName);
			return retVal;
		}


		///<summary>Any changes here should also be made in OpenDental.ProgramL.LoadToolBar.</summary>
		public static void LoadToolBar(ToolBar toolBar,EnumToolBar toolBarsAvail,EventHandler eventHandlerClick) {
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
				ToolBarButton toolBarButton=new ToolBarButton(listToolButItems[i].ButtonText,eventHandlerClick:eventHandlerClick,tag:program);
				if(program.ButtonImage!="") {
					System.Drawing.Bitmap bitmap=PIn.Bitmap(program.ButtonImage);
					toolBarButton.SetBitmap(bitmap);
				}
				else if(program.ProgName==ProgramName.Midway.ToString()) {
					System.Drawing.Bitmap bitmap=Properties.Resources.Midway_Icon_22x22;
					toolBarButton.SetBitmap(bitmap);
				}
				else if(program.ProgName==ProgramName.PracticeBooster.ToString()) {
					System.Drawing.Bitmap bitmap=Properties.Resources.PracticeBooster;
					toolBarButton.SetBitmap(bitmap);
				}
				//Add a drop down menu if this program requires it-----------------------------------------------------
				if(program.ProgName==ProgramName.Oryx.ToString()) {
					ContextMenu contextMenu=new ContextMenu();
					MenuItem menuItem=new MenuItem();
					menuItem.Text=Lans.g("Oryx","User Settings");
					menuItem.Click+=OpenDental.Bridges.Oryx.menuItemUserSettingsClick;
					contextMenu.Add(menuItem);
					toolBarButton.ToolBarButtonStyle=ToolBarButtonStyle.DropDownButton;
					toolBarButton.ContextMenuDropDown=contextMenu;
				}
				else if(program.ProgName==ProgramName.CareCredit.ToString()) {
					if(Programs.IsEnabled(ProgramName.CareCredit)) {
						return; //no need to create the drop down if CareCredit is already enabled
					}
					ContextMenu contextMenu=new ContextMenu();
					MenuItem menuItem=new MenuItem();
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
					contextMenu.Add(menuItem);
					toolBarButton.ToolBarButtonStyle=ToolBarButtonStyle.DropDownButton;
					toolBarButton.ContextMenuDropDown=contextMenu;
				}
				if(toolBarsAvail!=EnumToolBar.MainToolbar) {
					toolBar.AddSeparator();
				}
				toolBar.Add(toolBarButton);
			}
		}

		
	}
}
