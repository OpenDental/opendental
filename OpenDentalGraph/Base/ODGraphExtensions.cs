using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenDentalGraph.Extensions {

	public static class GraphExtensions {
		public static double RoundSignificant(this double val) {
			int asInt=(int)Math.Abs(val); //the minus sign was being counted as a digit below, inflating the value of the return value if it was negative.
			double ret=(Math.Truncate(asInt/Math.Pow(10,asInt.ToString().Length-1))+1)*Math.Pow(10,asInt.ToString().Length-1);
			if(val<0) {
				ret*=-1;
			}
			return ret;
		}
	}

	public class ComboItemIntValue:ComboItem<int> {
		public enum AllTrueFalseFlag { All, False, True }

		public static AllTrueFalseFlag GetAllTrueFalseFilter(ComboBox combo) {
			if(combo.SelectedItem==null || (!(combo.SelectedItem is ComboItemIntValue))) {
				return AllTrueFalseFlag.All;
			}
			return (AllTrueFalseFlag)((ComboItemIntValue)combo.SelectedItem).Value;
		}

		public static bool IncludeInFilter(AllTrueFalseFlag filterFlag,bool value) {
			if(filterFlag==AllTrueFalseFlag.All) {
				return true;
			}
			return value?filterFlag==AllTrueFalseFlag.True:filterFlag==AllTrueFalseFlag.False;
		}
	}

	public class ComboItem<T> {
		public T Value { get; set; }
		public string Display { get; set; }

		public static string GetDisplay(ComboBox combo) {
			ComboItem<string> item=(ComboItem<string>)combo.SelectedItem;
			if(item is null) {
				return "N/A";
			}
			if(!string.IsNullOrWhiteSpace(item.Display)) {
				return item.Display;
			}
			return item.Value??"N/A";
		}

		public static long GetValueLong(ComboBox combo) {
			ComboItem<long> item=(ComboItem<long>)combo.SelectedItem;
			return item.Value;
		}
	}


	public static class ComboBoxEx {
		public static void SetDataToAllTrueFalse(this ComboBox combo) {
			SetDataToEnums<ComboItemIntValue.AllTrueFalseFlag>(combo,false,false);
		}

		public delegate string StringFromEnumArgs<T>(T item);
		public delegate bool BoolFromEnumArgs<T>(T item);

		public static void SetDataToEnums<T>(this ComboBox combo,bool includeAllAtTop,bool showValueIndDisplay=true,int min=-1,int max=-1,StringFromEnumArgs<T> getStringFromEnum=null) where T:struct, IConvertible {
			//Make sure the IConvertibleType is actually an enum.
			if(!typeof(T).IsEnum) {
				throw new Exception("T must be an Enum type");
			}
			SetDataToEnums<T>(combo,Enum.GetValues(typeof(T)).Cast<T>().ToList(),includeAllAtTop,showValueIndDisplay,min,max,getStringFromEnum);
		}

		public static void SetDataToEnumsPrimitive<T>(this ComboBox combo,StringFromEnumArgs<T> getStringFromEnum) where T : struct, IConvertible {
			//Make sure the IConvertibleType is actually an enum.
			if(!typeof(T).IsEnum) {
				throw new Exception("T must be an Enum type");
			}
			List<T> list=Enum.GetValues(typeof(T)).Cast<T>().ToList();
			SetDataToEnumsPrimitive<T>(combo,list,0,list.Count-1,getStringFromEnum);
		}
		
		public static void SetDataToEnumsPrimitive<T>(this ComboBox combo,int min=-1,int max=-1,StringFromEnumArgs<T> getStringFromEnum=null,BoolFromEnumArgs<T> includeEnumValue = null) where T:struct, IConvertible {
			//Make sure the IConvertibleType is actually an enum.
			if(!typeof(T).IsEnum) {
				throw new Exception("T must be an Enum type");
			}
			SetDataToEnumsPrimitive<T>(combo,Enum.GetValues(typeof(T)).Cast<T>().ToList(),min,max,getStringFromEnum,includeEnumValue);
		}

		public static T GetValue<T>(this ComboBox combo) {
			ComboItem<T> item=(ComboItem<T>)combo.SelectedItem;
			if(item==null) {
				return default(T);
			}
			return item.Value;
		}

		public static void SetValueAtIndex<T>(this ComboBox combo,int index,T val) {
			ComboItem<T> item=(ComboItem<T>)combo.Items[index];
			if(item!=null) {
				item.Value=val;
			}
		}

		public static string GetDisplay<T>(this ComboBox combo) {
			ComboItem<T> item=(ComboItem<T>)combo.SelectedItem;
			return item.Display;
		}

		public static ComboItem<T> GetItem<T>(this ComboBox combo,T item) where T:struct, IConvertible {
			for(int i=0; i<combo.Items.Count; i++){
				ComboItem<T> comboItem=(ComboItem<T>)combo.Items[i];
				if(comboItem.Value.ToString()==item.ToString()) {
					return comboItem;
				}				
			}
			return null;
		}

		public static void SetItem<T>(this ComboBox combo,T item) where T:struct, IConvertible {
			for(int i=0;i<combo.Items.Count;i++) {
				ComboItem<T> comboItem=(ComboItem<T>)combo.Items[i];
				if(comboItem.Value.ToString()==item.ToString()) {
					combo.SelectedItem=comboItem;
					return;
				}
			}
		}

		public static void SetDataToEnumsPrimitive<T>(this ComboBox combo,List<T> enumValues,int min=-1,int max=-1,StringFromEnumArgs<T> getStringFromEnum=null,BoolFromEnumArgs<T> includeEnumValue = null) where T:struct, IConvertible {
			List<ComboItem<T>> listItems=new List<ComboItem<T>>();			
			enumValues.ForEach(x => {
				int val=Convert.ToInt32(x);
				if(min>=0 && val < min) {
					return;
				}
				if(max>=0 && val > max) {
					return;
				}
				if(includeEnumValue!=null) {
					if(!includeEnumValue(x)) {
						return;
					}
				}
				string display=x.ToString();
				if(getStringFromEnum!=null) {
					display=getStringFromEnum(x);
				}
				listItems.Add(new ComboItem<T>() { Value=x,Display=display });
			});
			//Try to retrain previous selection.
			int selIdx=-1;
			//if(combo.SelectedItem!=null && (combo.SelectedItem is ComboItemIntValue)) {
			//	selIdx=listItems.FindIndex(x => x.Value==((ComboItemIntValue)combo.SelectedItem).Value);
			//}
			combo.BindData(listItems);
			if(selIdx>=0) {
				combo.SelectedIndex=selIdx;
			}
		}

		///<summary>Updates the display name of the passed in comboBox and EnumValue to the specified display name.</summary>
		public static void UpdateDisplayName<T>(this ComboBox combo,T atValue,string newDisplayName) where T : struct, IConvertible {
			combo.GetItem<T>(atValue).Display=newDisplayName;
			((BindingList<ComboItem<T>>)combo.DataSource).ResetBindings();
		}

		///<summary>Removes the specified quantity type from the list bound to comboQuantityType.  Does nothing if the specified item doesn't exist.</summary>
		public static void RemoveItem<T>(this ComboBox combo,T atValue) where T : struct, IConvertible {
			((BindingList<ComboItem<T>>)combo.DataSource).Remove(combo.GetItem<T>(atValue));
			((BindingList<ComboItem<T>>)combo.DataSource).ResetBindings();
		}

		///<summary>Adds a quantity type to the end of the list bound to comboQuantityType.  
		///Checks to see if the item already exists first and does nothing if it does.</summary>
		public static void AddItem<T>(this ComboBox combo,T displayValue,string displayName) where T : struct, IConvertible {
			if(combo.GetItem<T>(displayValue)!=null) {
				combo.UpdateDisplayName(displayValue,displayName);
				return;
			}
			((BindingList<ComboItem<T>>)combo.DataSource).Add(new ComboItem<T>() { Display=displayName,Value=displayValue });
			((BindingList<ComboItem<T>>)combo.DataSource).ResetBindings();
		}

		///<summary>Inserts a quantity type into the chart's comboQuantityType at the specified location.
		///Checks to see if the item already exists first and does nothing if it does.  Calls AddItem() if the specified index is out of bounds.</summary>
		public static void InsertItem<T>(this ComboBox combo,T displayValue,string displayName,int index) where T : struct, IConvertible {
			if(combo.GetItem<T>(displayValue)!=null) {
				combo.UpdateDisplayName(displayValue,displayName);
				return;
			}
			try {
				((BindingList<ComboItem<T>>)combo.DataSource).Insert(index,new ComboItem<T>() { Display=displayName,Value=displayValue });
				((BindingList<ComboItem<T>>)combo.DataSource).ResetBindings();
			}
			catch {
				combo.AddItem(displayValue,displayName);
			}
		}

		public static void SetDataToEnums<T>(this ComboBox combo,List<T> enumValues,bool includeAllAtTop,bool showValueIndDisplay=true,int min=-1,int max=-1,StringFromEnumArgs<T> getStringFromEnum=null) where T:struct, IConvertible {			
			List<ComboItemIntValue> listItems=new List<ComboItemIntValue>();
			if(includeAllAtTop) {
				listItems.Add(new ComboItemIntValue() { Value=-1,Display="All" });
			}
			enumValues.ForEach(x => {
				int val=Convert.ToInt32(x);
				if(min>=0 && val < min) {
					return;
				}
				if(max>=0 && val > max) {
					return;
				}
				string display=(showValueIndDisplay?(Convert.ToInt32(x).ToString()+" - "):"")+x.ToString();
				if(getStringFromEnum!=null) {
					display=getStringFromEnum(x);
				}
				listItems.Add(new ComboItemIntValue() { Value=Convert.ToInt32(x),Display=display });
			});
			//Try to retrain previous selection.
			int selIdx=-1;
			if(combo.SelectedItem!=null && (combo.SelectedItem is ComboItemIntValue)) {
				selIdx=listItems.FindIndex(x => x.Value==((ComboItemIntValue)combo.SelectedItem).Value);
			}			
			combo.BindData(listItems);
			if(selIdx>=0) {
				combo.SelectedIndex=selIdx;
			}
		}

		private static void BindData<T>(this ComboBox combo,List<T> list,string valueMember="Value",string displayMember = "Display") {
			BindingList<T> binder=new BindingList<T>(list);
			combo.DataSource=binder;
			combo.ValueMember=valueMember;
			combo.DisplayMember=displayMember;
			//((BindingList<T>)combo.DataSource).ResetBindings();
		}

		public static void SetDataToComboItems<T>(this ComboBox combo,bool includeAllAtTop,bool includeBlank,List<ComboItem<T>> items
			,Func<T> defaultValue=null) where T:IEquatable<T> {
			T defaultVal=(defaultValue is null) ? default : defaultValue();
			if(includeBlank) {
				items.Insert(0,new ComboItem<T>() { Display="Invalid",Value=defaultVal });
				items.Insert(0,new ComboItem<T>() { Display="N/A",Value=defaultVal });
				items.Insert(0,new ComboItem<T>() { Display="",Value=defaultVal });
			}
			if(includeAllAtTop) {
				items.Insert(0,new ComboItem<T>() { Display="All",Value=defaultVal });
			}
			//Try to retrain previous selection.
			int selIdx=-1;
			if(combo.SelectedItem!=null && (combo.SelectedItem is ComboItem<T>)) {
				T sel=((ComboItem<T>)combo.SelectedItem).Value;
				if(sel!=null) {
					selIdx=items.FindIndex(x => x.Value!=null && x.Value.Equals(sel));
				}
			}
			combo.BindData(items);
			if(selIdx>=0) {
				combo.SelectedIndex=selIdx;
			}
		}
	}
}
