using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace OpenDentBusiness {
	public class ODDataRow:SortedList<string,string>{//Dictionary<string,string>{
		public string this[int index]{
      get{
				return this.Values[index];
      }
		}

		public Object ToObject(Type objectType) {
			ConstructorInfo constructor=objectType.GetConstructor(System.Type.EmptyTypes);
			Object obj=constructor.Invoke(null);
			FieldInfo[] fieldInfo=objectType.GetFields();
			for(int f=0;f<fieldInfo.Length;f++){
				if(fieldInfo[f].FieldType==typeof(int)){
					fieldInfo[f].SetValue(obj,PIn.Long(this[f]));
				}
				else if(fieldInfo[f].FieldType==typeof(bool)){
					fieldInfo[f].SetValue(obj,PIn.Bool(this[f]));
				}
				else if(fieldInfo[f].FieldType==typeof(string)){
					fieldInfo[f].SetValue(obj,PIn.String(this[f]));
				}
				else if(fieldInfo[f].FieldType.IsEnum){
					object val=((object[])Enum.GetValues(fieldInfo[f].FieldType))[PIn.Long(this[f])];
					fieldInfo[f].SetValue(obj,val);
				}
				else{
					throw new System.NotImplementedException();
				}
			}
			return obj;
		}

	}
}
