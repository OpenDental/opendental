using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public class FormLauncherHelper {
		public static void Launch(object sender,FormLauncherEventArgs e) {
			List<string> listNames=new List<string>();
			Type type =null; 
			//This switch doesn't actually do anything except ensure that every type of form has an actual reference here.
			//This is needed because of our late binding. We want to know about these references.
			switch(e.EnumFormName_){
				//FullName looks like this: OpenDental.FormWhatever
				case EnumFormName.FormAllergySetup:
					type=Type.GetType((typeof(FormAllergySetup)).FullName);
					break;
				case EnumFormName.FormCareCredit:
					type=Type.GetType((typeof(FormCareCredit)).FullName);
					break;
				case EnumFormName.FormCodeSystemsImport:
					type=Type.GetType((typeof(FormCodeSystemsImport)).FullName);
					break;
				case EnumFormName.FormDiseaseDefs:
					type=Type.GetType((typeof(FormDiseaseDefs)).FullName);
					break;
				case EnumFormName.FormDrCeph:
					type=Type.GetType((typeof(FormDrCeph)).FullName);
					break;
				case EnumFormName.FormEServicesTexting:
					type=Type.GetType((typeof(FormEServicesTexting)).FullName);
					break;
				case EnumFormName.FormHouseCalls:
					type=Type.GetType((typeof(FormHouseCalls)).FullName);
					break;
				case EnumFormName.FormMedications:
					type=Type.GetType((typeof(FormMedications)).FullName);
					break;
				case EnumFormName.FormNotePick:
					type=Type.GetType((typeof(FormNotePick)).FullName);
					break;
				case EnumFormName.FormOryxUserSettings:
					type=Type.GetType((typeof(FormOryxUserSettings)).FullName);
					break;
				case EnumFormName.FormPatientEdit:
					type=Type.GetType((typeof(FormPatientEdit)).FullName);
					break;
				case EnumFormName.FormPrintTrojan:
					type=Type.GetType((typeof(FormPrintTrojan)).FullName);
					break;
				case EnumFormName.FormSheetFillEdit:
					type=Type.GetType((typeof(FormSheetFillEdit)).FullName);
					break;
				case EnumFormName.FormTrojanCollect:
					type=Type.GetType((typeof(FormTrojanCollect)).FullName);
					break;
				case EnumFormName.FormTrophyNamePick:
					type=Type.GetType((typeof(FormTrophyNamePick)).FullName);
					break;
				case EnumFormName.FormVideo:
					type=Type.GetType((typeof(FormVideo)).FullName);
					break;
				case EnumFormName.FormWebBrowser:
					type=Type.GetType((typeof(FormWebBrowser)).FullName);
					break;
				case EnumFormName.FormWebView:
					type=Type.GetType((typeof(FormWebView)).FullName);
					break;
			}
			if(type==null){
				throw new InvalidOperationException("Form type not found.");
			}
			Form form = (Form)Activator.CreateInstance(type);
			e.Form=form;
			for(int i=0;i<e.ListEventPairs.Count;i++){
				EventInfo eventInfo=type.GetEvent(e.ListEventPairs[i].EventName);
				eventInfo.AddEventHandler(e.Form,e.ListEventPairs[i].EventHandler);
			}
			for(int i=0;i<e.ListFieldPairs.Count;i++){
				FieldInfo fieldInfo=type.GetField(e.ListFieldPairs[i].FieldName);
				fieldInfo.SetValue(e.Form,e.ListFieldPairs[i].FieldValue);
			}
			if(!e.IsDialog){
				form.Show();
				return;
			}
			form.ShowDialog();//We don't need sender because we never use the TopMost property which would cause child to show under.
			if(form.DialogResult==DialogResult.OK){
				e.IsDialogOK=true;
			}
			else{
				e.IsDialogOK=false;
			}
		}
	}
}
