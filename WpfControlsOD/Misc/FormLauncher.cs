using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	/*
Jordan is the only one allowed to edit this file.

How to use this class:
This lets you launch Open Dental forms from the WPFControlsOD project even though you don't have a ref to OD.
You are only allowed to launch Forms that are in the OpenDental namespace and project.
Since you won't have a reference to the type, the formName must be passed in using the enum.
You'll need to add your form to the enum.
The form you are launching must have a constructor with zero parameters.
You will need to rewrite any ctor with parameters to use public fields instead.
One significant downside to this late binding is that if you change a public member name in code,
		it will still compile even though the FormLaucher is now broken without changing the string version.
		There is no easy solution to that, but it will be an obvious failure and an obvious fix.

Example code:
			FormLauncher formLauncher=new FormLauncher(EnumFormName.FormEServicesTexting);
			formLauncher.ShowDialog();

Complex example showing both setting fields and getting results:
				FormLauncher formLauncher=new FormLauncher(EnumFormName.FormPatientSelect);
				Patient patient=new Patient();
				patient.LName="Montana";
				formLauncher.SetField("PatientInitial",patient);
				formLauncher.ShowDialog();
				if(formLauncher.IsDialogCancel){
					return;
				}
				long patNum=(long)formLauncher.GetResult(1);

Example non-modal:
			FormLauncher formLauncher=new FormLauncher(EnumFormName.FormWiki);
			formLauncher.Show();

Complex example of non-modal
			(class level in order to re-use) FormLauncher _formLauncherWiki;
			if(_formLauncherWiki is null){
				_formLauncherWiki=new FormLauncher(EnumFormName.FormWiki);//even better: do this in ctor so you don't have to use an "if".
			}
			if(_formLauncherWiki.IsNullOrDisposed()){
				_formLauncherWiki.Show();
			}
			else{
				_formLauncherWiki.RestoreAndFront();
			}

Setting fields:
			//This is usually done before showing the form.
			//Works for either modal or non-modal.
			//For non-modal Show(), this can also be done after showing the form.
			_formLauncherWiki.SetField("MyFieldName",newVal);
Getting fields:
			//Always done after showing the form.
			//Works for modal and non-modal
			int intVal=_formLauncherWiki.GetField<int>("MyFieldName");
Calling methods:
			//This can only be done for non-modal Show().
			int intResult=_formLauncherWiki.MethodGetObject<int>("TestGetInt","parameter1","parameter2");
			_formLauncherWiki.MethodGetVoid("DoSomething",25,"etc");
Properties:
			(not yet implemented until we need it)
Events:
			_formLauncherWiki.SetEvent("FormClosed",FormWiki_FormClosed);
			//To allow the simple syntax, we have many overloads for SetEvent.
			//There's no limit, so we can freely add more overloads as needed.
			//But you will frequently run across events that use types from OD.
			//Example: EventHandler<ToolBarButtonState> or EventHandler<VideoEventArgs>
			//To solve this, move the type into WpfControlsOD, then add it as an overload to SetEvent.
			//There will also frequently be ambiguity between the different overloads. In those cases, name your arguments.
*/
	///<summary>This allows code in WpfControls to launch OD Forms. Read instructions for use at the top of this file.</summary>
	public class FormLauncher{
		public bool IsDialogOK;
		private FormLauncherEventArgs _formLauncherEventArgs;

		///<summary></summary>
		public FormLauncher(EnumFormName enumFormName){
			_formLauncherEventArgs=new FormLauncherEventArgs();
			_formLauncherEventArgs.EnumFormName_=enumFormName;
		}

		public static event EventHandler<FormLauncherEventArgs> EventLaunch;

		#region Properties
		///<summary>Just for convenience and readability. Opposite of IsDialogOK.</summary>
		public bool IsDialogCancel{
			get=>!IsDialogOK;
		}
		#endregion Properties

		public T GetField<T>(string fieldName){
			if(IsNullOrDisposed()){
				throw new Exception("Form is null.");
			}
			Type type=_formLauncherEventArgs.Form.GetType();
			FieldInfo fieldInfo=type.GetField(fieldName);
			return (T)fieldInfo.GetValue(_formLauncherEventArgs.Form);
		}

		///<summary>This checks null or disposed of the form we are trying to launch. Also checks Visible property.</summary>
		public bool IsNullOrDisposed(){
			if(_formLauncherEventArgs.Form is null){
				return true;
			}
			if(_formLauncherEventArgs.Form.IsDisposed){
				return true;
			}
			if(!_formLauncherEventArgs.Form.Visible){//In WinForms, this equates to whether a form is open or closed.
				return true;
			}
			return false;
		}

		///<summary>methodName example: "GetSomeVal"</summary>
		public T MethodGetObject<T>(string methodName,params object[] parameters){
			if(IsNullOrDisposed()){
				throw new Exception("Only for non-modal. Form is not open.");
			}
			Type type=_formLauncherEventArgs.Form.GetType();
			MethodInfo methodInfo=type.GetMethod(methodName);
			object obj=methodInfo.Invoke(_formLauncherEventArgs.Form,parameters);
			return (T)obj;
		}

		public void MethodGetVoid(string methodName,params object[] parameters){
			if(IsNullOrDisposed()){
				throw new Exception("Only for non-modal. Form is not open.");
			}
			Type type=_formLauncherEventArgs.Form.GetType();
			MethodInfo methodInfo=type.GetMethod(methodName);
			methodInfo.Invoke(_formLauncherEventArgs.Form,parameters);
		}

		///<summary>RestoreAndFront is also available.</summary>
		public void BringToFront(){
			if(IsNullOrDisposed()){
				return;
			}
			_formLauncherEventArgs.Form.BringToFront();
		}

		///<summary>Restores from Minimized to Normal and calls BringToFront. You can also just call BringToFront.</summary>
		public void RestoreAndFront(){
			if(IsNullOrDisposed()){
				return;
			}
			if(_formLauncherEventArgs.Form.WindowState==System.Windows.Forms.FormWindowState.Minimized){
				_formLauncherEventArgs.Form.WindowState=System.Windows.Forms.FormWindowState.Normal;
			}
			_formLauncherEventArgs.Form.BringToFront();
		}

		public void SetEvent(string eventName,EventHandler eventHandler){
			SetEvent(eventName,(Delegate)eventHandler);
		}

		public void SetEvent(string eventName,FormClosedEventHandler formClosedEventHandler){
			SetEvent(eventName,(Delegate)formClosedEventHandler);
		}

		public void SetEvent(string eventName,EventHandler<System.Drawing.Bitmap> bitmapCaptured){
			SetEvent(eventName,(Delegate)bitmapCaptured);
		}

		public void SetEvent(string eventName,EventHandler<int> intVal){
			SetEvent(eventName,(Delegate)intVal);
		}

		public void SetEvent(string eventName,Delegate eventHandler){
			if(IsNullOrDisposed()){
				//setting the event prior to showing the form is the typical approach
				_formLauncherEventArgs.AddEvent(eventName,eventHandler);
				return;
			}
			//Nobody adds events after launching a form, but it's allowed
			Type type=_formLauncherEventArgs.Form.GetType();
			EventInfo eventInfo=type.GetEvent(eventName);
			eventInfo.AddEventHandler(_formLauncherEventArgs.Form,eventHandler);
		}

		public void SetField(string fieldName,object fieldValue){
			if(IsNullOrDisposed()){
				//setting the field prior to showing the form
				_formLauncherEventArgs.AddField(fieldName,fieldValue);
				return;
			}
			//setting a field after showing the form is far less common
			Type type=_formLauncherEventArgs.Form.GetType();
			FieldInfo fieldInfo=type.GetField(fieldName);
			fieldInfo.SetValue(_formLauncherEventArgs.Form,fieldValue);
		}

		///<summary>Shows a non-modal form that you can interact with.</summary>
		public void Show(){
			_formLauncherEventArgs.IsDialog=false;
			EventLaunch?.Invoke(null,_formLauncherEventArgs);
			//A reference to the form will now be in the EAs.
			//Because we now have a reference, we won't need the global event anymore.
			//We will just use reflection each time we need to do something.
		}

		///<summary></summary>
		public bool ShowDialog(){
			EventLaunch?.Invoke(null,_formLauncherEventArgs);
			//continues after dialog closes
			IsDialogOK=_formLauncherEventArgs.IsDialogOK;
			return IsDialogOK;
		}

		public void Close(){
			if(IsNullOrDisposed()){
				return;
			}
			_formLauncherEventArgs.Form.Close();
		}
	}

	public class FormLauncherEventArgs{
		///<summary>Example: OpenDental.FormEserviceText</summary>
		public EnumFormName EnumFormName_;
		///<summary></summary>
		public System.Windows.Forms.Form Form;
		public bool IsDialogOK;
		///<summary>Default is true to launch a dialog. If this is set to false, then it will launch as Show() which is non-modal.</summary>
		public bool IsDialog=true;
		public List<EventPair> ListEventPairs=new List<EventPair>();
		public List<FieldPair> ListFieldPairs=new List<FieldPair>();

		public class EventPair{
			public string EventName;
			public Delegate EventHandler;
		}

		public class FieldPair{
			public string FieldName;
			public object FieldValue;
		}

		public void AddEvent(string eventName,Delegate eventHandler){
			EventPair eventPair=new EventPair();
			eventPair.EventName=eventName;
			eventPair.EventHandler=eventHandler;
			ListEventPairs.Add(eventPair);
		}

		public void AddField(string fieldName,object fieldValue){
			FieldPair fieldPair=new FieldPair();
			fieldPair.FieldName=fieldName;
			fieldPair.FieldValue=fieldValue;
			ListFieldPairs.Add(fieldPair);
		}
	}

	/// <summary>Jordan will be involved in any changes to this enum. Items can be freely added. Keep it alphabetical. Also add to the switch statement in FormLauncherHelper.</summary>
	public enum EnumFormName{
		FormCareCredit,
		FormCodeSystemsImport,
		FormDrCeph,
		FormEServicesTexting,
		FormHouseCalls,
		FormImageFloatWindows,
		FormNotePick,
		FormOryxUserSettings,
		FormPatientSelect,
		FormPrintTrojan,
		FormSheetFillEdit,
		FormTrojanCollect,
		FormTrophyNamePick,
		FormVideo,
		FormWebBrowser,
		FormWebView
	}
}
