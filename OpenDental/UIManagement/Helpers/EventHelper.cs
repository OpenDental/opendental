using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenDental.UIManagement {
	/*
	public class EventHelper {
		//These InvokeEvents are terrible, but I consider them temporary.
		//Wherever we have our own control, we should instead add a series of PerformEvent methods. Example is GridOD.PerformCellDoubleClick().
		//This completely avoids reflection for the events.
		//This can be implemented on MS controls by manually specifying the eventhandler. We've already started doing this. Example is Um.ContextMenu_SetEventOpening()
		//It could alternatively be implemented on MS controls by lightly wrapping the control and swapping the controls for the wrapped ones inside OD.

		public static void InvokeClick(System.Windows.Forms.Control control){
			//This only runs when the user actually clicks, so we don't waste any time when loading window.
			//I tried many suggestions, but this one finally worked:
			//https://stackoverflow.com/questions/8790068/how-to-obtain-the-invocation-list-of-any-event
			//I've left some comments for things that might work or for debugging hints.
			//FieldInfo[] fieldInfoArray=typeof(System.Windows.Forms.Control).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			//Notice that this is Control, not any derived type:
			FieldInfo fieldInfo = typeof(System.Windows.Forms.Control).GetField("EventClick",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			//Delegate delegate2=(Delegate)fieldInfo.GetValue(button);
			PropertyInfo propertyInfo = typeof(System.Windows.Forms.Control).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			EventHandlerList eventHandlerList = propertyInfo.GetValue(control, new object[] { }) as EventHandlerList;
			object eventKey = fieldInfo.GetValue(control);
			Delegate delegateMy = eventHandlerList[eventKey] as Delegate;
			//MulticastDelegate multicastDelegate = fieldInfo.GetValue(button) as MulticastDelegate;
			//if(multicastDelegate!=null){ //at least one subscribed event consumer
				//Delegate[] delegateArray = multicastDelegate.GetInvocationList();
			if(delegateMy==null){
				return;
			}
			object[] objectArrayParameters=new object[2];
			objectArrayParameters[0]=control;//sender is ignored
			objectArrayParameters[1]=new System.EventArgs();
			delegateMy.Method.Invoke(delegateMy.Target,objectArrayParameters);
		}

		public static void InvokeTextChanged(System.Windows.Forms.Control control){
			//https://stackoverflow.com/questions/8790068/how-to-obtain-the-invocation-list-of-any-event
			FieldInfo fieldInfo = typeof(System.Windows.Forms.Control).GetField("EventText",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);
			PropertyInfo propertyInfo = typeof(System.Windows.Forms.Control).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			EventHandlerList eventHandlerList = propertyInfo.GetValue(control, new object[] { }) as EventHandlerList;
			object eventKey = fieldInfo.GetValue(control);
			Delegate delegateMy = eventHandlerList[eventKey] as Delegate;
			if(delegateMy==null){
				return;
			}
			object[] objectArrayParameters=new object[2];
			objectArrayParameters[1]=new System.EventArgs();
			delegateMy.Method.Invoke(delegateMy.Target,objectArrayParameters);
		}

		public static TestContr CreateTest(UI.TestForWpf testForWpf){
			TestContr testContr=new TestContr();
			return testContr;
		}

	}*/
}
