using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace OpenDentalWpf {
	///<summary>Neither of these copy methods work, but leaving this here for later.</summary>
	public class XamlHelper {
		/*
		public static UIElement DeepCopy(UIElement element){
			string shapestring = XamlWriter.Save(element);   
			StringReader stringReader = new StringReader(shapestring);  
			XmlTextReader xmlTextReader = new XmlTextReader(stringReader);  
			UIElement DeepCopyobject = (UIElement)XamlReader.Load(xmlTextReader);  
			return DeepCopyobject;  
		}

		public static object CopyObject(object obj) { 
			if(obj == null) { 
				return null;
			}
			object result = Activator.CreateInstance(obj.GetType()); 
			foreach(FieldInfo field in obj.GetType().GetFields()) {//could specify some binding flags here. 
				if(field.FieldType.GetInterface("IList",false) == null) { 
					field.SetValue(result,field.GetValue(obj)); 
				} 
				else { 
					IList listObject = (IList)field.GetValue(result); 
					if(listObject != null) { 
						foreach(object item in ((IList)field.GetValue(obj))) { 
							listObject.Add(CopyObject(item)); 
						} 
					} 
				} 
			} 
			return result; 
		}*/

		


	}
}
