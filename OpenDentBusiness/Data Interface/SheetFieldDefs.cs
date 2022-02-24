using CodeBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SheetFieldDefs{
		#region CachePattern

		private class SheetFieldDefCache : CacheListAbs<SheetFieldDef> {
			protected override List<SheetFieldDef> GetCacheFromDb() {
				string command="SELECT * FROM sheetfielddef ORDER BY SheetDefNum";
				return Crud.SheetFieldDefCrud.SelectMany(command);
			}
			protected override List<SheetFieldDef> TableToList(DataTable table) {
				return Crud.SheetFieldDefCrud.TableToList(table);
			}
			protected override SheetFieldDef Copy(SheetFieldDef sheetFieldDef) {
				return sheetFieldDef.Copy();
			}
			protected override DataTable ListToTable(List<SheetFieldDef> listSheetFieldDefs) {
				return Crud.SheetFieldDefCrud.ListToTable(listSheetFieldDefs,"SheetFieldDef");
			}
			protected override void FillCacheIfNeeded() {
				SheetFieldDefs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static SheetFieldDefCache _sheetFieldDefCache=new SheetFieldDefCache();

		public static List<SheetFieldDef> GetWhere(Predicate<SheetFieldDef> match,bool isShort=false) {
			return _sheetFieldDefCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_sheetFieldDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_sheetFieldDefCache.FillCacheFromTable(table);
				return table;
			}
			return _sheetFieldDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Gets all internal SheetFieldDefs from the database for a specific sheet, used in FormSheetFieldExam.</summary>
		public static List<SheetFieldDef> GetForExamSheet(long sheetDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SheetFieldDef>>(MethodBase.GetCurrentMethod(),sheetDefNum);
			}
			string command="SELECT * FROM sheetfielddef WHERE SheetDefNum="+POut.Long(sheetDefNum)+" "
				+"AND ((FieldName!='misc' AND FieldName!='') OR (ReportableName!='')) "
				+"GROUP BY FieldName,ReportableName";
			return Crud.SheetFieldDefCrud.SelectMany(command);
		}

		///<summary>Gets all SheetFieldDefs from the database for a specific sheet, used in FormSheetFieldExam.</summary>
		public static List<SheetFieldDef> GetForSheetDef(long sheetDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SheetFieldDef>>(MethodBase.GetCurrentMethod(),sheetDefNum);
			}
			string command="SELECT * FROM sheetfielddef WHERE SheetDefNum="+POut.Long(sheetDefNum);
			return Crud.SheetFieldDefCrud.SelectMany(command);
		}

		///<summary>Evaluates input fields an determines if this is a SheetField (or SheetFieldDef) that would be shown in mobile layout.
		///Returns true if criteria is met to show this field in mobile layout. Otherwise returns false.</summary>
		public static bool IsMobileFieldType(SheetFieldType sheetFieldType,int tabOrderMobile,string fieldName) {
			//No remoting role needed.
			//Always include these FieldTypes
			return ListTools.In(sheetFieldType,
				SheetFieldType.ComboBox,
				SheetFieldType.InputField,
				SheetFieldType.MobileHeader,
				SheetFieldType.SigBox,
				SheetFieldType.OutputText
			)
			//Only include StaticText if it has a valid TabOrderMobile.  Treat OutputText fields as a StaticText
			||(ListTools.In(sheetFieldType,SheetFieldType.StaticText,SheetFieldType.OutputText) && tabOrderMobile>=1)
			//checkMed checkboxes are ignored. All other checkboxes are included.
			||(sheetFieldType==SheetFieldType.CheckBox && !fieldName.StartsWith("checkMed"));
		}

		///<Summary>Gets one SheetFieldDef from the database.</Summary>
		public static SheetFieldDef CreateObject(long sheetFieldDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SheetFieldDef>(MethodBase.GetCurrentMethod(),sheetFieldDefNum);
			}
			return Crud.SheetFieldDefCrud.SelectOne(sheetFieldDefNum);
		}

		///<summary></summary>
		public static long Insert(SheetFieldDef sheetFieldDef) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				sheetFieldDef.SheetFieldDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sheetFieldDef);
				return sheetFieldDef.SheetFieldDefNum;
			}
			return Crud.SheetFieldDefCrud.Insert(sheetFieldDef);
		}

		///<summary></summary>
		public static void Update(SheetFieldDef sheetFieldDef) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetFieldDef);
				return;
			}
			Crud.SheetFieldDefCrud.Update(sheetFieldDef);
		}

		///<summary></summary>
		public static void Delete(long sheetFieldDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetFieldDefNum);
				return;
			}
			Crud.SheetFieldDefCrud.Delete(sheetFieldDefNum);
		}

		///<summary>Inserts, updates, or deletes database rows to match supplied list. Must always pass in sheetDefNum.
		///This function uses a DB comparison rather than a stale list because we are not worried about concurrency of a single sheet and enhancing the
		///functions that call this would take a lot of restructuring.</summary>
		public static void Sync(List<SheetFieldDef> listNew,long sheetDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,sheetDefNum);//never pass DB list through the web service
				return;
			}
			List<SheetFieldDef> listDB=SheetFieldDefs.GetForSheetDef(sheetDefNum);
			Crud.SheetFieldDefCrud.Sync(listNew,listDB);
		}


		///<summary>Sorts fields in the order that they shoudl be drawn on top of eachother. First Images, then Drawings, Lines, Rectangles, Text, Check Boxes, and SigBoxes. In that order.</summary>
		public static int SortDrawingOrderLayers(SheetFieldDef f1,SheetFieldDef f2) {
			if(f1.FieldType!=f2.FieldType) {
				return SheetFields.FieldTypeSortOrder(f1.FieldType).CompareTo(SheetFields.FieldTypeSortOrder(f2.FieldType));
			}
			return f1.YPos.CompareTo(f2.YPos);
			//return f1.SheetFieldNum.CompareTo(f2.SheetFieldNum);
		}

		///<summary>This is a comparator function used by List&lt;T&gt;.Sort() 
		///When compairing SheetFieldDef.TabOrder it returns a negative number if def1&lt;def2, 0 if def1==def2, and a positive number if def1&gt;def2.
		///Does not handle null values, but there should never be any instances of null being passed in. 
		///Must always return 0 when compairing item to itself.
		///This function should probably be moved to SheetFieldDefs.</summary>
		public static int CompareTabOrder(SheetFieldDef def1,SheetFieldDef def2) {
			if(def1.FieldType==def2.FieldType) {
				//do nothing
			}
			else if(def1.FieldType==SheetFieldType.Image) { //Always move images to the top of the list. This is because of the way the sheet is drawn.
				return -1;
			}
			else if(def2.FieldType==SheetFieldType.Image) { //Always move images to the top of the list. This is because of the way the sheet is drawn.
				return 1;
			}
			else if(def1.FieldType==SheetFieldType.PatImage) { //Move PatImage to the top of the list under images.
				return -1;
			}
			else if(def2.FieldType==SheetFieldType.PatImage) { //Move PatImage to the top of the list under images.
				return 1;
			}
			else if(def1.FieldType==SheetFieldType.Special) { //Move Special to the top of the list under PatImages.
				return -1;
			}
			else if(def2.FieldType==SheetFieldType.Special) { //Move Special to the top of the list under PatImages.
				return 1;
			}
			else if(def1.FieldType==SheetFieldType.OutputText) { //Move Output text to the top of the list under Special.
				return -1;
			}
			else if(def2.FieldType==SheetFieldType.OutputText) { //Move Output text to the top of the list under Special.
				return 1;
			}
			else if(def1.FieldType==SheetFieldType.MobileHeader) { //Move MobileHeader to bottom
				return 1;
			}
			else if(def2.FieldType==SheetFieldType.MobileHeader) { //Move MobileHeader to bottom
				return -1;
			}
			if(def1.TabOrder-def2.TabOrder==0) {
				int comp=(def1.FieldName+def1.RadioButtonValue).CompareTo(def2.FieldName+def2.RadioButtonValue); //RadioButtionValuecan be filled or ""
				if(comp!=0) {
					return comp;
				}
				comp=def1.YPos-def2.YPos; //arbitrarily order by YPos if both controls have the same tab orer and name. This will only happen if both fields are either identical or if they are both misc fields.
				if(comp!=0) {
					return comp;
				}
				comp=def1.XPos-def2.XPos; //If tabOrder, Name, and YPos are equal then compare based on X coordinate. 
				if(comp!=0) {
					return comp;
				}
				return def1.TabOrderMobile-def2.TabOrderMobile;
			}
			return def1.TabOrder-def2.TabOrder;
		}
	}
}