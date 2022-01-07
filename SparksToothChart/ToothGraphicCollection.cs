using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SparksToothChart {
	///<summary>A strongly typed collection of type ToothGraphic</summary>
	public class ToothGraphicCollection:CollectionBase {
		///<summary>Returns the ToothGraphic with the given index.</summary>
		public ToothGraphic this[int index] {
			get {
				return (ToothGraphic)List[index];
			}
			set {
				List[index]=value;
			}
		}

		///<summary>Returns the ToothGraphic with the given toothID.</summary>
		public ToothGraphic this[string toothID] {
			get {
				if(toothID !="implant" && !ToothGraphic.IsValidToothID(toothID)) {
					throw new ArgumentException("Tooth ID not valid: "+toothID);
				}
				for(int i=0;i<List.Count;i++){
					if(((ToothGraphic)List[i]).ToothID==toothID){
						return (ToothGraphic)List[i];
					}
				}
				return null;
			}
			set {
				//List[index]=value;
			}
		}

		///<summary></summary>
		public int Add(ToothGraphic value) {
			return (List.Add(value));
		}

		///<summary></summary>
		public int IndexOf(ToothGraphic value) {
			return (List.IndexOf(value));
		}

		///<summary></summary>
		public void Insert(int index,ToothGraphic value) {
			List.Insert(index,value);
		}

		///<summary></summary>
		public void Remove(ToothGraphic value) {
			List.Remove(value);
		}

		///<summary></summary>
		public bool Contains(ToothGraphic value) {
			//If value is not of type ToothGraphic, this will return false.
			return (List.Contains(value));
		}

		/*
		///<summary></summary>
		public bool Contains(string toothID) {
			//If value is not of type ToothGraphic, this will return false.
			return (List.Contains(value));
		}*/

		///<summary></summary>
		protected override void OnInsert(int index,Object value) {
			if(value.GetType()!=typeof(ToothGraphic)) {
				throw new ArgumentException("value must be of type ToothGraphic.","value");
			}
		}

		///<summary></summary>
		protected override void OnRemove(int index,Object value) {
			if(value.GetType()!=typeof(ToothGraphic)) {
				throw new ArgumentException("value must be of type ToothGraphic.","value");
			}
		}

		///<summary></summary>
		protected override void OnSet(int index,Object oldValue,Object newValue) {
			if(newValue.GetType()!=typeof(ToothGraphic)) {
				throw new ArgumentException("newValue must be of type ToothGraphic.","newValue");
			}
		}

		///<summary></summary>
		protected override void OnValidate(Object value) {
			if(value.GetType()!=typeof(ToothGraphic)) {
				throw new ArgumentException("value must be of type ToothGraphic.");
			}
		}

		///<summary></summary>
		public ToothGraphicCollection Copy() {
			ToothGraphicCollection collect=new ToothGraphicCollection();
			for(int i=0;i<this.Count;i++) {
				collect.Add(this[i].Copy());
			}
			return collect;
		}



	}
}
