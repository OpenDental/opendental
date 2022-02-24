using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ProcButtons {
		#region CachePattern

		private class ProcButtonCache : CacheListAbs<ProcButton> {
			protected override List<ProcButton> GetCacheFromDb() {
				string command="SELECT * FROM procbutton ORDER BY ItemOrder";
				return Crud.ProcButtonCrud.SelectMany(command);
			}
			protected override List<ProcButton> TableToList(DataTable table) {
				return Crud.ProcButtonCrud.TableToList(table);
			}
			protected override ProcButton Copy(ProcButton procButton) {
				return procButton.Copy();
			}
			protected override DataTable ListToTable(List<ProcButton> listProcButtons) {
				return Crud.ProcButtonCrud.ListToTable(listProcButtons,"ProcButton");
			}
			protected override void FillCacheIfNeeded() {
				ProcButtons.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ProcButtonCache _procButtonCache=new ProcButtonCache();

		public static int GetCount(bool isShort=false) {
			return _procButtonCache.GetCount(isShort);
		}

		public static List<ProcButton> GetDeepCopy(bool isShort=false) {
			return _procButtonCache.GetDeepCopy(isShort);
		}

		public static List<ProcButton> GetWhere(Predicate<ProcButton> match,bool isShort=false) {
			return _procButtonCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_procButtonCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_procButtonCache.FillCacheFromTable(table);
				return table;
			}
			return _procButtonCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>must have already checked procCode for nonduplicate.</summary>
		public static long Insert(ProcButton but) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				but.ProcButtonNum=Meth.GetLong(MethodBase.GetCurrentMethod(),but);
				return but.ProcButtonNum;
			}
			return Crud.ProcButtonCrud.Insert(but);
		}

		///<summary></summary>
		public static void Update(ProcButton but) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),but);
				return;
			}
			Crud.ProcButtonCrud.Update(but);
		}

		///<summary></summary>
		public static void Delete(ProcButton but) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),but);
				return;
			}
			string command="DELETE FROM procbuttonitem WHERE ProcButtonNum = '"
				+POut.Long(but.ProcButtonNum)+"'";
			Db.NonQ(command);
			command="DELETE FROM procbutton WHERE ProcButtonNum = '"
				+POut.Long(but.ProcButtonNum)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static ProcButton[] GetForCat(long selectedCat) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.Category==selectedCat).ToArray();
		}

		///<summary>Deletes all current ProcButtons from the Chart module, and then adds the default ProcButtons.  Procedure codes must have already been entered or they cannot be added as a ProcButton.</summary>
		public static void SetToDefault() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			string command="DELETE FROM procbutton";
			Db.NonQ(command);
			command="DELETE FROM procbuttonitem";
			Db.NonQ(command);
			command="DELETE FROM definition WHERE Category=26";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				if(System.Globalization.CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					SetToDefaultMySQLCanada();
				}
				else {
					SetToDefaultMySQL();
				}
			}
			else {
				SetToDefaultOracle();
			}
		}

		private static void SetToDefaultMySQL() {
			long category;//defNum
			long procButtonNum;
			long autoCodeNum;
			long autoCodeNum2;
			//Db---------------------------------------------------------------------------------------------------------
			string command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
				+"VALUES (26,0,'General','',0,0)";
			category=Db.NonQ(command,true);
			//Amalgam
			autoCodeNum=AutoCodes.GetNumFromDescript("Amalgam");
			if(autoCodeNum!=0) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Amalgam',0,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA/////////////////////9bW1v+ctbX/vebv/8XW1v+tvbX/hKWt/36crf9+nKX/nLW1/97v7//m9/f/pdbe/5y9vf/3////////////////////////////////////ztbO/5y1rf+cxcX/hKWt/5S1vf+cvcX/nMXO/5S9zv+Mrb3/jK2t/629vf+cxc7/nL29//f///////////////////////////////////+9zs7/fpSM/5S1vf+cxc7/tdbe/87m5v/O5u//1u/v/8Xe5v+lxdb/jK21/4Slrf+cvbX/9////////////////////////////////////7XW1v+ErbX/rc7W/8Xm5v/F3ub/3u/v/9739//W7+//zubv/87m7/+11t7/jK21/5y9vf/////////////////////////////////39/f/nLW9/5zFzv/O5ub/3vf3/8Xe5v/m////7////+/////O5u//zu/v/9739/+11tb/lLW1/+/39////////////////////////////9bW5v9NXb3/VWbv/8XW9//v////vdbm/+b3///v////9////73e3v+UtbX/5vf3/7XF9/9ufr3/zt7m////////////////////////////xcXv/xws3v8THO//bn73/87e5v+MrbX/xd7m/+/////v////rc7W/4ylpf/e7///PU33/xwk5v+1ve////////////////////////////+1ve//HCze/xwk7/80Pe//doy9/5y9vf/O5ub/9/////f///+1ztb/nLW1/4yc9/8cJOb/EyTv/4SU7////////////////////////////5yl7/8kLOb/HCzv/xwk7/80Rc7/foze/5Sl7/+crff/nK33/36U5v9dbsX/PUXv/xwk5v8cJOb/XW7m///////////////////////m9/f/foTv/yQs5v8cLO//HCzm/xws7/8kLOb/JCzm/yQs5v8kLOb/JCzm/xws5v8cJOb/HCzm/xwk7/9FVdb/9////////////////////+/39/9dbt7/HCTm/xws5v8cLOb/HCzm/xws5v8cLOb/HCzm/xws5v8cLOb/HCzm/xws5v8cLOb/HCTv/0VVzv/3/////////////////////////1Vm1v8cJO//HCzm/xws5v8cJOb/HCTv/xwk5v8cJOb/HCTm/xwk5v8cJO//HCTm/xws5v8cJO//RVXO//f39//////////////////3////RVXO/xwk7/8cLOb/HCTm/yw05v8sNOb/ND3v/zQ97/80Pe//ND3v/yw05v8kLO//HCTm/xwk7/9FVc7/9/f3//////////////////f///9FVc7/HCTv/xwk5v8sNOb/jJzm/3aErf+1xff/xdb3/8XW//+ElL3/nLXe/4yc9/8kLOb/HCTv/0VVzv/39/f/////////////////9////0VVzv8THO//ND3v/7XF9//O5ub/lK2l/87e1v/3////9////5y1rf/W7+//9////4yU9/8cJO//RVXW//f/////////////////////////boTF/1Vm7//Fzv//9////97v9/+1ztb/xdbW//f////m9/f/rcXO/+b39//3////7////5Sl7/9NXbX/9/f3//////////////////////+91tb/nL3F//f////v////5vf3/87m5v/e7/f/7////+b39//W7+//7////+/////v////tc7O/629vf////////////////////////////////+lxcX/pcXF/+/////3////1u/v/9739//3////3u/3/97v9//3////7////7XO1v+lvcX/9/////////////////////////////////////f39/+lvcX/nL3F/+b39//3////7////+/////m////7////97v7/+txcX/pb3F//f39/////////////////////////////////////////////f39/+txcX/lLW1/629vf/O3t7/3vf3/8Xe3v+lxcX/jKWc/7XFxf/39/f//////////////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
				Db.NonQ(command);
			}
			//Composite
			autoCodeNum=AutoCodes.GetNumFromDescript("Composite");
			if(autoCodeNum!=0) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Composite',1,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA///////////e5v//RU3m/yQ05v8kNOb/JDTm/xwk7/8cJO//JCzm/36UnP9+lJT/fpSc/3aUlP9+lIz/xc7O/////////////////////////////////3Z+9/8kLOb/Znbv/3aE7/+ElO//fpTv/zQ97/8kLOb/rcXm/73e3v+11t7/tdbe/5zFzv+EpaX/1t7e////////////////////////////LDTv/11u5v/e7/f/5vf//+/////3////pbX//yQs7//e7///7////+b39//e7/f/zubv/5S1vf+tvbX///////////////////////////80Pff/XW7m/+b////3////7/////f///+1xf//JCzv/+b3///3////7////+b39//W7+//nL3F/629vf///////////////////////////z1F9/9dbu//7/f//+/////v////9////4yc//89Rff/5vf//+/////v////7////9bv7/+cvcX/rb29////////////////////////////ND3v/3aE7//3////7////+/////v////XWb3/36M9//3////7////+/////v////1u/3/5S1vf+9zs7///////////////////////////8sNO//doTv//f////v////7////+////80Pe//pbX///f////v////7////+/////e7/f/jK2t/9be3v///////////////////////////yw07/9ufu//7////+/////v////3u///zQ97/+9zv//9////+/////v////7////87m7/9+lIz/3u/v////////////////////////////JCzv/2Z27//m////7/////f///+MlP//VWb3//f////v////7////+/////v////zubm/36UlP/m7+////////////////////////////89Rff/VWbm/97v9//3////7////11m9/9mbvf/9////+/////v////7////+////+91t7/lK2t//f3/////////////////////////////5yl//80Rd7/zubv//f////O3v//LDTv/7XF///3////7////+/////v////5vf//6W9xf+ctbX/////////////////////////////////1t7//yQ05v+MnO///////4yU//89Rff/5vf//+/////v////7////+/////m9///nL3F/7XFzv/////////////////////////////////39///Zm73/0VV5v+9zvf/LDTv/5yl///3////7////+/////v////7////+b39/+UrbX/1t7e//////////////////////////////////////+UnPf/JCzm/yw07/9dbvf/5vf//+/////v////7////+/////v////1ubv/4ylpf/m7+///////////////////////////////////////97m//8sNO//XW7v/97v///3////7////+/////v////7////+/////F3t7/nK2t/////////////////////////////////////////////////25+vf+Mpc7/7////+/////v////7////+/////v////7////6W9vf/Fzs7/////////////////////////////////////////////////pb21/5Strf/O5ub/7////+/////v////7/////f////e7+//jK2t/+bv7//////////////////////////////////////////////////O5ub/fpyc/7XOzv/e9/f/9////+/////v////9////7XOzv+EpaX/5vf3/////////////////////////////////////////////////+bv7/+MvcX/lK2t/5y1tf/O5ub/5vf3/+bv7/+9zs7/lL29/4ytrf/3////////////////////////////////////////////////////3ubm/5TFxf+95u//rb29/4ylnP+cra3/lK2t/6W9vf+t1t7/nLW1/////////////////////////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
				Db.NonQ(command);
			}
			//Crown-PFM
			if(ProcedureCodes.IsValidCode("D2750") || ProcedureCodes.IsValidCode("N4118")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown-PFM',2,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+1ve//PVXW/1Vu7/9mdu//Znbv/2Z27/9mdu//XW7v/2Z27/9mbu//Zm7v/2Zu7/9dbu//XW7v/01m7/9FXeb/pa33/////////////////11m7/89Re//XWbv/11m7/9dZu//XWbv/11m7/9dZu//XWbv/11m7/9dZu//XWbv/11m7/9dZu//XWbv/z1F7/9NVe/////////////e3v//LDTv/6Wt9//39///9/f///f3///39///9/f///f3///39///9/f///f3///39///9/f///f3///39///vb3//zQ97//W1v///////5Sc9/80Re//5ub////////////////////////////////////////////////////////////////////////W1v//PUXv/73F9//39///XWbv/2Zu7////////////////////////////////////////////////////////////////////////////9bW//89Re//vcX3/87O//8sNO//tb3/////////////////////////////////////////////////////////////////////////////3t7//zRF7/+cpff/hIz3/z1F7//v7///////////////////////////////////////////////////////////////////////////////////XWbv/2529/9udvf/VV3v//f3//////////////////////////////////////////////////////////////////////////////////+MlPf/TVXv/2529/9VXe//9/f/////////////////////////////7+///6Wl9/+1vf///////////////////////////////////////6Wl9/80Re//bnb3/1Vd7//39//////////////////////////////W1v//RU3v/6Wl9///////////////////////////////////////3t7//yw07/9mdu//XWbv//f3/////////////////////////////87W//9FTe//zs7////////////////////////////////////////39///VV3v/3Z+9/9NVe//9/f/////////////////////////////1tb//1Vd7//Ozv////////////////////////////////////////f3//9VXe//vb3//yQs7//Ozv////////////////////////////+9xff/ND3v/4yU9//v7///////////////////////////////////tb3//yw07//v7///VV3v/z1F7/+cnPf/5ub///f3///m5v//jJT3/zQ97/89Re//LDTv/0VN7/+MlPf/pa33/87O///v9///5ub//5Sc9/8sNO//foT3///////e5v//foT3/z1N7/8sNO//VV3v/zQ97/9FTe//lJz3/+/v///e5v//foT3/01V7/9FTe//LDTv/0VN7/8sNO//ND3v/4yU9////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("D2750")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D2750")+",0,0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("N4118")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("N4118")+",0,1)";
					Db.NonQ(command);
				}
			}
			//Crown-Ceramic
			if(ProcedureCodes.IsValidCode("D2740") || ProcedureCodes.IsValidCode("N4119")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown-Ceramic',3,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////+1ve//PVXW/1Vu7/9mdu//Znbv/2Z27/9mdu//XW7v/2Z27/9mbu//Zm7v/2Zu7/9dbu//XW7v/01m7/9FXeb/pa33/////////////////11m7/89Re//XWbv/11m7/9dZu//XWbv/11m7/9dZu//XWbv/11m7/9dZu//XWbv/11m7/9dZu//XWbv/z1F7/9NVe/////////////e3v//LDTv/6Wt9//39///9/f///f3///39///9/f///f3///39///9/f///f3///39///9/f///f3///39///vb3//zQ97//W1v///////5Sc9/80Re//5ub////////////////////////////////////////////////////////////////////////W1v//PUXv/73F9//39///XWbv/2Zu7////////////////////////////////////////////////////////////////////////////9bW//89Re//vcX3/87O//8sNO//tb3/////////////////////////////////////////////////////////////////////////////3t7//zRF7/+cpff/hIz3/z1F7//v7///////////////////////////////////////////////////////////////////////////////////XWbv/2529/9udvf/VV3v//f3//////////////////////////////////////////////////////////////////////////////////+MlPf/TVXv/2529/9VXe//9/f/////////////////////////////7+///6Wl9/+1vf///////////////////////////////////////6Wl9/80Re//bnb3/1Vd7//39//////////////////////////////W1v//RU3v/6Wl9///////////////////////////////////////3t7//yw07/9mdu//XWbv//f3/////////////////////////////87W//9FTe//zs7////////////////////////////////////////39///VV3v/3Z+9/9NVe//9/f/////////////////////////////1tb//1Vd7//Ozv////////////////////////////////////////f3//9VXe//vb3//yQs7//Ozv////////////////////////////+9xff/ND3v/4yU9//v7///////////////////////////////////tb3//yw07//v7///VV3v/z1F7/+cnPf/5ub///f3///m5v//jJT3/zQ97/89Re//LDTv/0VN7/+MlPf/pa33/87O///v9///5ub//5Sc9/8sNO//foT3///////e5v//foT3/z1N7/8sNO//VV3v/zQ97/9FTe//lJz3/+/v///e5v//foT3/01V7/9FTe//LDTv/0VN7/8sNO//ND3v/4yU9////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("D2740")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D2740")+",0,0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("N4119")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("N4119")+",0,1)";
					Db.NonQ(command);
				}
			}
			//Crown-Gold
			if(ProcedureCodes.IsValidCode("D2790") || ProcedureCodes.IsValidCode("N4136")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown-Gold',4,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("D2790")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D2790")+",0,0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("N4136")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("N4136")+",0,1)";
					Db.NonQ(command);
				}
			}
			//RCT
			autoCodeNum=AutoCodes.GetNumFromDescript("Root Canal");
			if(autoCodeNum!=0) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('RCT',5,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA///////////////////////////////////////////F1tb/boyt/xwkpf/m5ub//////////////////////////////////////////////////////////////////////////////////////36MjP9VbpT/EySc/7XFxf//////////////////////////////////////////////////////////////////////////////////////XWZm/36lzv8kNMX/ZnZu//////////////////////////////////////////////////////////////////////////////////f39/9NXV3/fpz//wMD//9ddm7/zs7O////////////////////////////////////////////////////////////////////////////3t7e/2Z+dv9mfv//AwP//2aEfv+lpaX////////////////////////////////////////////////////////////////////////////FxcX/XXZ2/26E//8DA///jK2t/5ycnP///////////////////////////////////////////////////////////////////////////8XFxf9mfnb/fpT//wMD//+Era3/lJSU//f39///////////////////////////////////////////////////////////////////////nKWl/2aMjP9uhP//AwP//4y1tf+UnJz/9/f3//////////////////////////////////////////////////////////////////////9mbm7/ZoSE/2Z+//8DA///nMXF/4SMjP/39/f//////////////////////////////////////////////////////////////////////2Zubv9ujIT/Znb//wMD//+lzs7/foyM//f39///////////////////////////////////////////////////////////////////////PUVF/36lpf9ufv//AwP//63W1v9ufn7/9/f3/////////////////////////////////////////////////////////////////+/v7/9VVVX/jLW1/2Z2//8DA///td7m/2Z2dv/v7+//////////////////////////////////////////////////////////////////3t7e/1VVVf+Uxb3/bn7//wMD//+13ub/ZnZ2/+bm5v/////////////////////////////////////////////////////////////////e3t7/XWZu/5TFvf9mdv//AwP//7Xe5v9mdnb/3t7e/////////////////////////////////////////////////////////////////8XFxf9mbm7/nMXF/11m//8DA///zu/3/1VmXf/e3t7/////////////////////////////////////////////////////////////////zs7O/11mZv+cxcX/XW7//wMD///O7/f/VWZm/87Ozv/////////////////////////////////////////////////////////////////FxcX/ZnZ2/6XOzv9dbv//AwP//9bv9/9Vbm7/pa2t/////////////////////////////////////////////////////////////////62trf9VZmb/rdbW/01d//8DA///1u/3/01mZv+UlJT/////////////////////////////////////////////////////////////////nJyc/0VNTf+t3tb/XW7//wMD//+Urf//RV1d/4yUlP////////////////////////////////////////////////////////////////+1tbX/RV1d/7Xe3v9dZv//AwP//4yl//89VVX/foSE/////////////////////////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
				Db.NonQ(command);
			}
			//RCT BU PFM
			autoCodeNum=AutoCodes.GetNumFromDescript("Root Canal");
			autoCodeNum2=AutoCodes.GetNumFromDescript("BU/P&C");
			if(autoCodeNum!=0 || ProcedureCodes.IsValidCode("D2750")) {//we add this button if either RCT or crown is available.
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('RCT BU PFM',6,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA////////////////pcXF/63W7/89Te//Exzv/01V7/+95u//tebm/7Xm5v9+lO//HCTv/xwk7/9Vbu//pdbe/73m5v////////////////////////////////+cvb3/rdbv/z1N7/8cJO//PUXv/63W7/+15ub/tebm/11u7/8cJO//HCTv/01d7/+cztb/td7e////////////////////////////9/f//5y9tf+t1u//PU3v/xwk7/8kLO//pb33/73m5v/W7/f/PU3v/xwk7/8cJO//PU3v/4zF3v+13t7/////////////////////////////////lL21/6XW7/89Te//HCTv/xwk7/+ElPf/zvfv/5yt9/8kLO//HCTv/xwk7/89Te//lMXe/73e3v////////////////////////////////+UvbX/vd73/0VN7/8cJO//HCzv/yw97/9mbu//LDTv/xwk7/8cLO//HCTv/0VN7/+Uvc7/rc7O/////////////////////////////////6XO1v+13u//RU3v/xwk7/8cLO//HCTv/xwk7/8cJO//HCzv/xws7/8cJO//RU3v/4S13v+lztb////////////////////////////m5v//PU3v/zRF5v8kLO//EyTv/xwk7/8cJO//HCTv/xwk7/8cJO//HCTv/xMk7/8cJO//JCzv/zRF5v/W1v///////////////////////5yl9/8sNO//fn73/4SM9/+EjPf/hIz3/4SM9/+EjPf/hIz3/4SM9/+EjPf/hIz3/4SM9/+EhPf/ND3v/4yU9//////////////////39///TVXv/36E9/////////////////////////////////////////////////////////////////9mbvf/VV3v//f3/////////////8XF//8cLO//zs7//////////////////////////////////////////////////////////////////4SM9/8sPe//7+//////////////hIz3/0VN7//39///////////////////////////////////////////////////////////////////paX3/yQs7//m5v///////+bm//9FTe//hIT3///////////////////////////////////////////////////////////////////////Fxf//JCzv/7299///////xcX//yw07/+9vff//////////////////////////////////////////////////////////////////////+/v//89Re//VV3v///////Ozv//ND3v/7299////////////////////////////+/v///e3v//9/f/////////////////////////////9/f//1Vd7/9FTe///////8XO//80Pe//vb33////////////////////////////ra33/11m7//e3v//////////////////////////////////dn73/yQs7///////xcX//yw07//Fxf////////////////////////////+UlPf/Zm7v//////////////////////////////////////+1tf//JCzv///////W1v//NEXv/7299////////////////////////////5yl9/92fvf//////////////////////////////////////7299/8kLO////////f3//9dZu//fn73////////////////////////////foT3/z1F7//Ozv//////////////////////////////////dn73/zQ97////////////5yl9/8kLO//jJT3/+bm///39///5u///4SM9/8sNO//LDTv/yQs7/9+hPf/vb33/97e///39///5u///3Z+9/8kLO//vb33////////////9/f//5Sc9/9NVe//ND3v/z1F7/80Pe//TVXv/62t9//v7///nKX3/0VN7/8sNO//LDTv/01V7/80Pe//PUXv/7299////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("D2750")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D2750")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("N4118")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("N4118")+",0)";
					Db.NonQ(command);
				}
				if(autoCodeNum!=0) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
					Db.NonQ(command);
				}
				if(autoCodeNum2!=0) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum2)+")";
					Db.NonQ(command);
				}
			}
			//RCT BU Ceramic
			autoCodeNum=AutoCodes.GetNumFromDescript("Root Canal");
			autoCodeNum2=AutoCodes.GetNumFromDescript("BU/P&C");
			if(autoCodeNum!=0 || ProcedureCodes.IsValidCode("D2740")) {//we add this button if either RCT or crown is available.
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('RCT BU Ceramic',7,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA////////////////pcXF/63W7/89Te//Exzv/01V7/+95u//tebm/7Xm5v9+lO//HCTv/xwk7/9Vbu//pdbe/73m5v////////////////////////////////+cvb3/rdbv/z1N7/8cJO//PUXv/63W7/+15ub/tebm/11u7/8cJO//HCTv/01d7/+cztb/td7e////////////////////////////9/f//5y9tf+t1u//PU3v/xwk7/8kLO//pb33/73m5v/W7/f/PU3v/xwk7/8cJO//PU3v/4zF3v+13t7/////////////////////////////////lL21/6XW7/89Te//HCTv/xwk7/+ElPf/zvfv/5yt9/8kLO//HCTv/xwk7/89Te//lMXe/73e3v////////////////////////////////+UvbX/vd73/0VN7/8cJO//HCzv/yw97/9mbu//LDTv/xwk7/8cLO//HCTv/0VN7/+Uvc7/rc7O/////////////////////////////////6XO1v+13u//RU3v/xwk7/8cLO//HCTv/xwk7/8cJO//HCzv/xws7/8cJO//RU3v/4S13v+lztb////////////////////////////m5v//PU3v/zRF5v8kLO//EyTv/xwk7/8cJO//HCTv/xwk7/8cJO//HCTv/xMk7/8cJO//JCzv/zRF5v/W1v///////////////////////5yl9/8sNO//fn73/4SM9/+EjPf/hIz3/4SM9/+EjPf/hIz3/4SM9/+EjPf/hIz3/4SM9/+EhPf/ND3v/4yU9//////////////////39///TVXv/36E9/////////////////////////////////////////////////////////////////9mbvf/VV3v//f3/////////////8XF//8cLO//zs7//////////////////////////////////////////////////////////////////4SM9/8sPe//7+//////////////hIz3/0VN7//39///////////////////////////////////////////////////////////////////paX3/yQs7//m5v///////+bm//9FTe//hIT3///////////////////////////////////////////////////////////////////////Fxf//JCzv/7299///////xcX//yw07/+9vff//////////////////////////////////////////////////////////////////////+/v//89Re//VV3v///////Ozv//ND3v/7299////////////////////////////+/v///e3v//9/f/////////////////////////////9/f//1Vd7/9FTe///////8XO//80Pe//vb33////////////////////////////ra33/11m7//e3v//////////////////////////////////dn73/yQs7///////xcX//yw07//Fxf////////////////////////////+UlPf/Zm7v//////////////////////////////////////+1tf//JCzv///////W1v//NEXv/7299////////////////////////////5yl9/92fvf//////////////////////////////////////7299/8kLO////////f3//9dZu//fn73////////////////////////////foT3/z1F7//Ozv//////////////////////////////////dn73/zQ97////////////5yl9/8kLO//jJT3/+bm///39///5u///4SM9/8sNO//LDTv/yQs7/9+hPf/vb33/97e///39///5u///3Z+9/8kLO//vb33////////////9/f//5Sc9/9NVe//ND3v/z1F7/80Pe//TVXv/62t9//v7///nKX3/0VN7/8sNO//LDTv/01V7/80Pe//PUXv/7299////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("D2740")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D2740")+",0,0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("N4119")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("N4119")+",0,1)";
					Db.NonQ(command);
				}
				if(autoCodeNum!=0) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
					Db.NonQ(command);
				}
				if(autoCodeNum2!=0) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum2)+")";
					Db.NonQ(command);
				}
			}
			//Bridge-PFM
			autoCodeNum=AutoCodes.GetNumFromDescript("PFM Bridge");
			if(autoCodeNum!=0 || ProcedureCodes.IsValidCode("N4127")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge-PFM',8,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("N4127")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("N4127")+",0)";
					Db.NonQ(command);
				}
				if(autoCodeNum!=0) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
					Db.NonQ(command);
				}
			}
			//Bridge-Ceramic
			autoCodeNum=AutoCodes.GetNumFromDescript("Ceramic Bridge");
			if(autoCodeNum!=0 || ProcedureCodes.IsValidCode("N4127")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge-Ceramic',9,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("N4127")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("N4127")+",0)";
					Db.NonQ(command);
				}
				if(autoCodeNum!=0) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
					Db.NonQ(command);
				}
			}
			//Build Up
			autoCodeNum=AutoCodes.GetNumFromDescript("BU/P&C");
			if(autoCodeNum!=0) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('BU/P&C',10,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)+",0,"
					+POut.Long(autoCodeNum)+")";
				Db.NonQ(command);
			}
			//Implant Abutment PFM
			if(ProcedureCodes.IsValidCode("D6010") || ProcedureCodes.IsValidCode("D6057") || ProcedureCodes.IsValidCode("D6059")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Implant Abutment PFM',11,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("D6010")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D6010")+",0,0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("D6057")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D6057")+",0,1)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("D6059")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D6059")+",0,2)";
					Db.NonQ(command);
				}
			}
			//Implant Abutment Ceramic Crown
			if(ProcedureCodes.IsValidCode("D6010") || ProcedureCodes.IsValidCode("D6057") || ProcedureCodes.IsValidCode("D6058")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Implant Abutment Ceramic',12,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("D6010")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D6010")+",0,0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("D6057")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D6057")+",0,1)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("D6058")) {
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum,ItemOrder) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D6058")+",0,2)";
					Db.NonQ(command);
				}
			}
			//Exams/Cleanings Category--------------------------------------------------------------------------------------------
			command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
				+"VALUES (26,1,'Exams/Cleanings','',0,0)";
			category=Db.NonQ(command,true);
			//PA
			if(ProcedureCodes.IsValidCode("D0220")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('PA',0,"
					+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////9/f3/0VFRf8DAwP/AwMD/wMDA/9dXV3/jIyM/yQkJP8DAwP/AwMD/4SEhP8LCwv/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/RUVF/97e3v+MjIz/AwMD/wMDA/8DAwP/AwMD/35+fv/FxcX/JCQk/wMDA/8DAwP/5ubm/5SUlP8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/VVVV/wMDA/8DAwP/AwMD/wMDA/8LCwv/nJyc/+bm5v9dXV3/AwMD/wsLC/+MjIz/ZmZm/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wsLC/+lpaX/xcXF/zQ0NP8DAwP/NDQ0/62trf9dXV3/JCQk/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/TU1N/2ZmZv/Ozs7/dnZ2/wMDA/9dXV3/xcXF/+bm5v80NDT/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/9FRUX/lJSU/8XFxf+tra3/hISE/7W1tf/FxcX/tbW1/2ZmZv8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/01NTf/Ozs7/1tbW/+bm5v/Ozs7/7+/v/+bm5v/v7+//jIyM/yQkJP8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/ZmZm/+bm5v//////5ubm/+/v7//v7+//5ubm//f39//W1tb/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/9mZmb////////////39/f/5ubm//f39///////9/f3/+bm5v80NDT/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/1VVVf/39/f//////+/v7//m5ub/////////////////7+/v/yQkJP8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/3t7e/+/v7//39/f/9/f3/9bW1v/m5ub////////////v7+//AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/+EhIT/paWl/8XFxf//////3t7e/97e3v/v7+//5ubm/5ycnP8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/TU1N/wsLC/8DAwP/AwMD/wMDA/89PT3/1tbW/87Ozv/FxcX/lJSU/35+fv+MjIz/dnZ2/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/2ZmZv//////XV1d/wMDA/8DAwP/AwMD/wMDA/8LCwv/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/8DAwP/AwMD/wMDA/9mZmb//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////w==')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D0220")+",0)";
				Db.NonQ(command);
			}
			//SRP-4 Quads
			if(ProcedureCodes.IsValidCode("D4341")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('SRP-4 Quads',0,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D4341")+",0)";
				Db.NonQ(command);//1
				Db.NonQ(command);//2
				Db.NonQ(command);//3
				Db.NonQ(command);//4
			}
			//SRP-1 Quad
			if(ProcedureCodes.IsValidCode("D4341")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('SRP-1 Quad',0,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D4341")+",0)";
				Db.NonQ(command);
			}
			//SRP-Limited
			if(ProcedureCodes.IsValidCode("D4342")) {
				command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('SRP-Limited',0,"
					+POut.Long(category)+@",'')";
				procButtonNum=Db.NonQ(command,true);
				command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
						+","+ProcedureCodes.GetCodeNum("D4342")+",0)";
				Db.NonQ(command);
			}
		}

		private static void SetToDefaultMySQLCanadaExams() {
			long category;//defNum
			long procButtonNum;
			long autoCodeNum;
			long autoCodeNum2;
			long autoCodeNum3;
			string command;
			//Exams - Category 204
			if(ProcedureCodes.IsValidCode("01101") || ProcedureCodes.IsValidCode("01102") || ProcedureCodes.IsValidCode("01103") 
				|| ProcedureCodes.IsValidCode("01202") || ProcedureCodes.IsValidCode("01205") || ProcedureCodes.IsValidCode("01204"))
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,0,'Exams','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("01101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Complete Prim Exam',0,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("01102")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Complete Mixed Exam',1,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01102")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("01103")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Complete Perm Exam',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01103")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("01202")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Recall Exam',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01202")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("01205")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Emerg Exam',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01205")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("01204")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Specific Exam',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01204")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaXrays() {
			long category;//defNum
			long procButtonNum;
			long autoCodeNum;
			long autoCodeNum2;
			long autoCodeNum3;
			string command;
			if(ProcedureCodes.IsValidCode("02111") || ProcedureCodes.IsValidCode("02112") || ProcedureCodes.IsValidCode("02113") || ProcedureCodes.IsValidCode("02114") 
				|| ProcedureCodes.IsValidCode("02141") || ProcedureCodes.IsValidCode("02142") || ProcedureCodes.IsValidCode("02144") || ProcedureCodes.IsValidCode("02102") 
				|| ProcedureCodes.IsValidCode("07043") || ProcedureCodes.IsValidCode("02601"))
			{
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,1,'Xrays','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("02111")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('1 PA',0,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02111")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02112")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('2 PA''s',1,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02112")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02113")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('3 PA''s',2,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02113")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02114")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('4 PA''s',3,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02114")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02141")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('1 BW',4,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02141")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02142")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('2 BW''s',5,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02142")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02144")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('4 BW''s',6,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02144")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02102")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Adult FMS',7,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02102")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("07043")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('CBCT',8,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("07043")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("02601")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('PAN',9,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAE2SURBVDhP5ZQhjoYwEIUxSK7BEbBIJBKJ5QhIBBKJxCKRHAGLRCKxSORsvv6ZpLSEzSZrNjvJS6H0PV5npg3kl+OPCJZlKWEY3hBFkTf3hqqqPoJ5nnvkNE1l2zZZ11WWZZG+76VpGum6Tuq6vq21gWjgTrZtK+M4GiFi33eD8zzlOA4ZhuG23oUnqOTrugxwipAG31yODU+wKAojpGRbjGAujuMbx4YnqMklEEZQf6DvSZLcODYeBdUVI47UqabA5djwBKk61dXc8ayCjFT6R1vOssy0CVWepslUfZ5n887IDlyOjUeHkHCmPUfjI4jLt/wBT5Am1z5UMiNuEX7bLngUpHlxo0eSOXL33XZBwEJ3kmOGQ9sNPyC39joX9HBAK1AI+wPO3GpSnLcLAw1O2b+7YEW+ADLY6ogGwsBkAAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("02601")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaHygiene() {
			long category;//defNum
			long procButtonNum;
			long autoCodeNum;
			long autoCodeNum2;
			long autoCodeNum3;
			string command;

			//Hygiene - Category 298
			if(ProcedureCodes.IsValidCode("11111") || ProcedureCodes.IsValidCode("11112") || ProcedureCodes.IsValidCode("11113") || ProcedureCodes.IsValidCode("11114") 
				|| ProcedureCodes.IsValidCode("11117") || ProcedureCodes.IsValidCode("11101") || ProcedureCodes.IsValidCode("11107") || ProcedureCodes.IsValidCode("43421")
				|| ProcedureCodes.IsValidCode("43422") || ProcedureCodes.IsValidCode("43423") || ProcedureCodes.IsValidCode("43424") || ProcedureCodes.IsValidCode("43427")
				|| ProcedureCodes.IsValidCode("12111") || ProcedureCodes.IsValidCode("12112") || ProcedureCodes.IsValidCode("12113") || ProcedureCodes.IsValidCode("04403")) 
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,2,'Hygiene','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("11111")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Scaling 1U',0,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("11111")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("11112")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Scaling 2U',1,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("11112")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("11113")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Scaling 3U',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("11113")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("11114")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Scaling 4U',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("11114")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("11117")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Scaling 0.5U',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("11117")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("11101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Polish 1U',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("11101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("11107")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Polish 0.5U',6,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("11107")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("43421")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Planing 1U',7,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("43421")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("43422")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Planing 2U',8,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("43422")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("43423")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Planing 3U',9,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("43423")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("43424")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Planing 4U',10,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("43424")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("43427")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Planing 0.5U',11,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("43427")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("12111")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Flouride Rinse',12,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("12111")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("12112")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Flouride Gel/Foam',13,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("12112")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("12113")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Flouride Varnish',14,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("12113")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("04403")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Velscope',15,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("04403")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaRestorative() { 
			long category;//defNum
			long procButtonNum;
			long autoCodeNum;
			long autoCodeNum2;
			long autoCodeNum3;
			string command;
			//Restorative - Category 299
			autoCodeNum=AutoCodes.GetNumFromDescript("Composite");
			autoCodeNum2=AutoCodes.GetNumFromDescript("Amalgam- Non Bonded");
			if(autoCodeNum!=0 || autoCodeNum2!=0 || ProcedureCodes.IsValidCode("13409") || ProcedureCodes.IsValidCode("16511")) { 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,3,'Restorative','',0,0)";
				category=Db.NonQ(command,true);
				autoCodeNum=AutoCodes.GetNumFromDescript("Composite");
				if(autoCodeNum!=0) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Composite',0,"
						+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA///////////e5v//R0/m/yY25v8mNub/Jjbm/x4m7/8eJu//Ji7m/4CUnP+AlJT/gJSc/3iUlP+AlIz/xc7O/////////////////////////////////3iA9/8mLub/aHjv/3iE7/+ElO//gJTv/zY/7/8mLub/rcXm/73e3v+11t7/tdbe/5zFzv+EpaX/1t7e////////////////////////////Ljbv/19w5v/e7/f/5vf//+/////3////pbX//yYu7//e7///7////+b39//e7/f/zubv/5S1vf+tvbX///////////////////////////82P/f/X3Dm/+b////3////7/////f///+1xf//Ji7v/+b3///3////7////+b39//W7+//nL3F/629vf///////////////////////////z9H9/9fcO//7/f//+/////v////9////4yc//8/R/f/5vf//+/////v////7////9bv7/+cvcX/rb29////////////////////////////Nj/v/3iE7//3////7////+/////v////X2j3/4CM9//3////7////+/////v////1u/3/5S1vf+9zs7///////////////////////////8uNu//eITv//f////v////7////+////82P+//pbX///f////v////7////+/////e7/f/jK2t/9be3v///////////////////////////y427/9wgO//7////+/////v////3u///zY/7/+9zv//9////+/////v////7////87m7/+AlIz/3u/v////////////////////////////Ji7v/2h47//m////7/////f///+MlP//V2j3//f////v////7////+/////v////zubm/4CUlP/m7+////////////////////////////8/R/f/V2jm/97v9//3////7////19o9/9ocPf/9////+/////v////7////+////+91t7/lK2t//f3/////////////////////////////5yl//82R97/zubv//f////O3v//Ljbv/7XF///3////7////+/////v////5vf//6W9xf+ctbX/////////////////////////////////1t7//yY25v+MnO///////4yU//8/R/f/5vf//+/////v////7////+/////m9///nL3F/7XFzv/////////////////////////////////39///aHD3/0dX5v+9zvf/Ljbv/5yl///3////7////+/////v////7////+b39/+UrbX/1t7e//////////////////////////////////////+UnPf/Ji7m/y427/9fcPf/5vf//+/////v////7////+/////v////1ubv/4ylpf/m7+///////////////////////////////////////97m//8uNu//X3Dv/97v///3////7////+/////v////7////+/////F3t7/nK2t/////////////////////////////////////////////////3CAvf+Mpc7/7////+/////v////7////+/////v////7////6W9vf/Fzs7/////////////////////////////////////////////////pb21/5Strf/O5ub/7////+/////v////7/////f////e7+//jK2t/+bv7//////////////////////////////////////////////////O5ub/gJyc/7XOzv/e9/f/9////+/////v////9////7XOzv+EpaX/5vf3/////////////////////////////////////////////////+bv7/+MvcX/lK2t/5y1tf/O5ub/5vf3/+bv7/+9zs7/lL29/4ytrf/3////////////////////////////////////////////////////3ubm/5TFxf+95u//rb29/4ylnP+cra3/lK2t/6W9vf+t1t7/nLW1/////////////////////////////////w==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+",0,"+POut.Long(autoCodeNum)+")";
					Db.NonQ(command);
				}
				if(autoCodeNum2!=0) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Amalgam',1,"
						+POut.Long(category)+@",'Qk12BgAAAAAAADYAAAAoAAAAFAAAABQAAAABACAAAAAAAAAAAADEDgAAxA4AAAAAAAAAAAAA/////////////////////9bW1v+ctbX/vebv/8XW1v+tvbX/hKWt/4Ccrf+AnKX/nLW1/97v7//m9/f/pdbe/5y9vf/3////////////////////////////////////ztbO/5y1rf+cxcX/hKWt/5S1vf+cvcX/nMXO/5S9zv+Mrb3/jK2t/629vf+cxc7/nL29//f///////////////////////////////////+9zs7/gJSM/5S1vf+cxc7/tdbe/87m5v/O5u//1u/v/8Xe5v+lxdb/jK21/4Slrf+cvbX/9////////////////////////////////////7XW1v+ErbX/rc7W/8Xm5v/F3ub/3u/v/9739//W7+//zubv/87m7/+11t7/jK21/5y9vf/////////////////////////////////39/f/nLW9/5zFzv/O5ub/3vf3/8Xe5v/m////7////+/////O5u//zu/v/9739/+11tb/lLW1/+/39////////////////////////////9bW5v9PX73/V2jv/8XW9//v////vdbm/+b3///v////9////73e3v+UtbX/5vf3/7XF9/9wgL3/zt7m////////////////////////////xcXv/x4u3v8VHu//cID3/87e5v+MrbX/xd7m/+/////v////rc7W/4ylpf/e7///P0/3/x4m5v+1ve////////////////////////////+1ve//Hi7e/x4m7/82P+//eIy9/5y9vf/O5ub/9/////f///+1ztb/nLW1/4yc9/8eJub/FSbv/4SU7////////////////////////////5yl7/8mLub/Hi7v/x4m7/82R87/gIze/5Sl7/+crff/nK33/4CU5v9fcMX/P0fv/x4m5v8eJub/X3Dm///////////////////////m9/f/gITv/yYu5v8eLu//Hi7m/x4u7/8mLub/Ji7m/yYu5v8mLub/Ji7m/x4u5v8eJub/Hi7m/x4m7/9HV9b/9////////////////////+/39/9fcN7/Hibm/x4u5v8eLub/Hi7m/x4u5v8eLub/Hi7m/x4u5v8eLub/Hi7m/x4u5v8eLub/Hibv/0dXzv/3/////////////////////////1do1v8eJu//Hi7m/x4u5v8eJub/Hibv/x4m5v8eJub/Hibm/x4m5v8eJu//Hibm/x4u5v8eJu//R1fO//f39//////////////////3////R1fO/x4m7/8eLub/Hibm/y425v8uNub/Nj/v/zY/7/82P+//Nj/v/y425v8mLu//Hibm/x4m7/9HV87/9/f3//////////////////f///9HV87/Hibv/x4m5v8uNub/jJzm/3iErf+1xff/xdb3/8XW//+ElL3/nLXe/4yc9/8mLub/Hibv/0dXzv/39/f/////////////////9////0dXzv8VHu//Nj/v/7XF9//O5ub/lK2l/87e1v/3////9////5y1rf/W7+//9////4yU9/8eJu//R1fW//f/////////////////////////cITF/1do7//Fzv//9////97v9/+1ztb/xdbW//f////m9/f/rcXO/+b39//3////7////5Sl7/9PX7X/9/f3//////////////////////+91tb/nL3F//f////v////5vf3/87m5v/e7/f/7////+b39//W7+//7////+/////v////tc7O/629vf////////////////////////////////+lxcX/pcXF/+/////3////1u/v/9739//3////3u/3/97v9//3////7////7XO1v+lvcX/9/////////////////////////////////////f39/+lvcX/nL3F/+b39//3////7////+/////m////7////97v7/+txcX/pb3F//f39/////////////////////////////////////////////f39/+txcX/lLW1/629vf/O3t7/3vf3/8Xe3v+lxcX/jKWc/7XFxf/39/f//////////////////////w==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+",0,"+POut.Long(autoCodeNum2)+")";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("13401")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Sealant',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("13401")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("13409")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Sealant Add''l',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("13409")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("16511")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bite Adjustment',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("16511")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaCrownAndBridge() {
			long category;//defNum
			long procButtonNum;
			long autoCodeNum;
			long autoCodeNum2;
			long autoCodeNum3;
			string command;
			//Crown & Bridge - Category 366
			autoCodeNum=AutoCodes.GetNumFromDescript("PFM Bridge");
			autoCodeNum2=AutoCodes.GetNumFromDescript("Gold Inlay");
			autoCodeNum3=AutoCodes.GetNumFromDescript("Porcelain Inlay");
			if(ProcedureCodes.IsValidCode("27211") || ProcedureCodes.IsValidCode("N4110") || ProcedureCodes.IsValidCode("27201") || ProcedureCodes.IsValidCode("N4140") 
				|| ProcedureCodes.IsValidCode("27301") || ProcedureCodes.IsValidCode("N4136") || ProcedureCodes.IsValidCode("27311") || ProcedureCodes.IsValidCode("N4141")
				|| autoCodeNum!=0 || ProcedureCodes.IsValidCode("N4112") || autoCodeNum2!=0 || ProcedureCodes.IsValidCode("25111") || autoCodeNum3!=0
				|| ProcedureCodes.IsValidCode("25511") || ProcedureCodes.IsValidCode("25531") || ProcedureCodes.IsValidCode("27602") || ProcedureCodes.IsValidCode("N4142")
				|| ProcedureCodes.IsValidCode("23602") || ProcedureCodes.IsValidCode("25711") || ProcedureCodes.IsValidCode("25711") || ProcedureCodes.IsValidCode("25781")
				|| ProcedureCodes.IsValidCode("29301") || ProcedureCodes.IsValidCode("29302") || ProcedureCodes.IsValidCode("29101") || ProcedureCodes.IsValidCode("29102")
				|| ProcedureCodes.IsValidCode("29303") || ProcedureCodes.IsValidCode("66211") || ProcedureCodes.IsValidCode("66212") || ProcedureCodes.IsValidCode("66213")
				|| ProcedureCodes.IsValidCode("66214") || ProcedureCodes.IsValidCode("66301") || ProcedureCodes.IsValidCode("66302") || ProcedureCodes.IsValidCode("66303")
				|| ProcedureCodes.IsValidCode("66304")) 
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,4,'Crown & Bridge','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("27211") || ProcedureCodes.IsValidCode("N4110")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('PFM Crown',0,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAUxJREFUOE/NVC2Tg0AM3V9zshKLRCKRWCQSWVFZWYlFnjyJPItEIrGRkbn30tJ2btot7fRm7s1kdtgkL18bgr0Zf0Q4T6a7rUmemiQbk7IwSRN8Z6btwUzkYlPkfu92ywlbPeydKtDYCbIUzi0cZzNVP2nkxNAtBLzTz85sGGDHQLBDUOoZMFjfm2w+jgY3oN/QLxkw03vov5w06LbxMjyrWxhHr+BewDPgT7sgTWVSlRHCwUtdTahNfcwgliF716FvMUzTqeRHhOibk6FHMdBGsmQF4Ur4i8Asgu53/iyiE1wBzoHJBTabtT8qKQr2D+Vq14KQ0wG71NVJ+wI4OC4G3qyvnr90RPAteQW/CZeR+0pxOM8Ktw3bROLz38bL5s7yfFbox20DLoQYylhAUeNHwe25lhpOfl4L7ihVbnOJh8/qgDPhu/DfCc1+AJOvqQ951Xr2AAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					if(ProcedureCodes.IsValidCode("27211")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("27211")+",0)";
						Db.NonQ(command);
					}
					if(ProcedureCodes.IsValidCode("N4110")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("N4110")+",0)";
						Db.NonQ(command);
					}
				}
				if(ProcedureCodes.IsValidCode("27201") || ProcedureCodes.IsValidCode("N4140")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Porcelain Crown',1,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOxAAADsQBlSsOGwAAAUxJREFUOE/NVC2Tg0AM3V9zshKLRCKRWCQSWVFZWYlFnjyJPItEIrGRkbn30tJ2btot7fRm7s1kdtgkL18bgr0Zf0Q4T6a7rUmemiQbk7IwSRN8Z6btwUzkYlPkfu92ywlbPeydKtDYCbIUzi0cZzNVP2nkxNAtBLzTz85sGGDHQLBDUOoZMFjfm2w+jgY3oN/QLxkw03vov5w06LbxMjyrWxhHr+BewDPgT7sgTWVSlRHCwUtdTahNfcwgliF716FvMUzTqeRHhOibk6FHMdBGsmQF4Ur4i8Asgu53/iyiE1wBzoHJBTabtT8qKQr2D+Vq14KQ0wG71NVJ+wI4OC4G3qyvnr90RPAteQW/CZeR+0pxOM8Ktw3bROLz38bL5s7yfFbox20DLoQYylhAUeNHwe25lhpOfl4L7ihVbnOJh8/qgDPhu/DfCc1+AJOvqQ951Xr2AAAAAElFTkSuQmCC')";
					procButtonNum=Db.NonQ(command,true);
					if(ProcedureCodes.IsValidCode("27201")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("27201")+",0)";
						Db.NonQ(command);
					}
					if(ProcedureCodes.IsValidCode("N4140")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("N4140")+",0)";
						Db.NonQ(command);
					}
				}
				if(ProcedureCodes.IsValidCode("27301") || ProcedureCodes.IsValidCode("N4136")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Full Gold Crown',2,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAFMSURBVDhPzVQtk4NADN0/c7ISi0QikVgkEltRWVmJRZ48iTyLRCKxkZG599LSdm7aLe30Zu7NZHbYJC9fG4K9GX9EOE+mu61JnpokG5OyMEkTfGem7cFM5GJT5H7vdssJWz3snSrQ2AmyFM4tHGczVT9p5MTQLQS808/ObBhgx0CwQ1DqGTBY35tsPo4GN6Df0C8ZMNN76L+cNOi28TI8q1sYR6/gXsAz4E+7IE1lUpURwsFLXU2oTX3MIJYhe9ehbzFM06nkR4Tom5OhRzHQRrJkBeFK+IvALILud/4sohNcAc6ByQU2m7U/KikK9g/lateCkNMBu9TVSfsCODguBt6sr56/dETwLXkFvwmXkftKcTjPCrcN20Ti89/Gy+bO8nxW6MdtAy6EGMpYQFHjR8HtuZYaTn5eC+4oVW5ziYfP6oAz4bvw3wnNfgCbG6hqkJVwhgAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					if(ProcedureCodes.IsValidCode("27301")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("27301")+",0)";
						Db.NonQ(command);
					}
					if(ProcedureCodes.IsValidCode("N4136")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("N4136")+",0)";
						Db.NonQ(command);
					}
				}
				if(ProcedureCodes.IsValidCode("27311") || ProcedureCodes.IsValidCode("N4141")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('3/4 Gold Crown',3,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAFMSURBVDhPzVQtk4NADN0/c7ISi0QikVgkEltRWVmJRZ48iTyLRCKxkZG599LSdm7aLe30Zu7NZHbYJC9fG4K9GX9EOE+mu61JnpokG5OyMEkTfGem7cFM5GJT5H7vdssJWz3snSrQ2AmyFM4tHGczVT9p5MTQLQS808/ObBhgx0CwQ1DqGTBY35tsPo4GN6Df0C8ZMNN76L+cNOi28TI8q1sYR6/gXsAz4E+7IE1lUpURwsFLXU2oTX3MIJYhe9ehbzFM06nkR4Tom5OhRzHQRrJkBeFK+IvALILud/4sohNcAc6ByQU2m7U/KikK9g/lateCkNMBu9TVSfsCODguBt6sr56/dETwLXkFvwmXkftKcTjPCrcN20Ti89/Gy+bO8nxW6MdtAy6EGMpYQFHjR8HtuZYaTn5eC+4oVW5ziYfP6oAz4bvw3wnNfgCbG6hqkJVwhgAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					if(ProcedureCodes.IsValidCode("27311")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("27311")+",0)";
						Db.NonQ(command);
					}
					if(ProcedureCodes.IsValidCode("N4141")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("N4141")+",0)";
						Db.NonQ(command);
					}
				}
				if(autoCodeNum!=0 || ProcedureCodes.IsValidCode("N4112")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('PFM Bridge',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					if(autoCodeNum!=0) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+",0,"+POut.Long(autoCodeNum)+")";
						Db.NonQ(command);
					}
					if(ProcedureCodes.IsValidCode("N4112")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("N4112")+",0)";
						Db.NonQ(command);
					}
				}
				if(autoCodeNum2!=0 || ProcedureCodes.IsValidCode("25111")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Gold Inlay',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					if(autoCodeNum2!=0) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+",0,"+POut.Long(autoCodeNum2)+")";
						Db.NonQ(command);
					}
					if(ProcedureCodes.IsValidCode("25111")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("25111")+",0)";
						Db.NonQ(command);
					}
				}
				if(autoCodeNum3!=0) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Porcelain Inlay',6,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+",0,"+POut.Long(autoCodeNum3)+")";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("25511")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Gold Onlay',7,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("25511")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("25531")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Porcelain Onlay',8,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("25531")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("27602") || ProcedureCodes.IsValidCode("N4142")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Vaneer',9,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					if(ProcedureCodes.IsValidCode("27602")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("27602")+",0)";
						Db.NonQ(command);
					}
					if(ProcedureCodes.IsValidCode("N4142")) {
						command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
								+","+ProcedureCodes.GetCodeNum("N4142")+",0)";
						Db.NonQ(command);
					}
				}
				if(ProcedureCodes.IsValidCode("23602")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Composite Core Buildup',10,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("23602")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("25711")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Cast Post',11,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("25711")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("25721")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Cast Pin',12,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("25721")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("25781")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Post Removal',13,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("25781")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29301")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown Removal 1U',14,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29301")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29302")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown Removal 2U',15,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29302")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown Recement 1U',16,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29102")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown Recement 2U',17,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29102")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29303")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Crown Recement 3U',18,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29303")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66211")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Removal 1U',19,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66211")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66212")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Removal 2U',20,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66212")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66213")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Removal 3U',21,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66213")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66214")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Removal 4U',22,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66214")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66301")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Recement 1U',23,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66301")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66302")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Recement 2U',24,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66302")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66303")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Recement 3U',25,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66303")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66304")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bridge Recement 4U',26,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66304")+",0)";
					Db.NonQ(command);
				}	
			}
		}

		private static void SetToDefaultMySQLCanadaEndodontics() {
			long category;//defNum
			long procButtonNum;
			long autoCodeNum;
			long autoCodeNum2;
			long autoCodeNum3;
			long autoCodeNum4;
			string command;		
			//Endodontics - Category 300
			autoCodeNum=AutoCodes.GetNumFromDescript("RCTSimple");
			autoCodeNum2=AutoCodes.GetNumFromDescript("RCTDifficult");
			autoCodeNum3=AutoCodes.GetNumFromDescript("Open&Drain");
			autoCodeNum4=AutoCodes.GetNumFromDescript("OpenThruCrown");
			if(autoCodeNum!=0 || ProcedureCodes.IsValidCode("33141") || autoCodeNum2!=0 || ProcedureCodes.IsValidCode("33142") || autoCodeNum3!=0 || autoCodeNum4!=0) { 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,5,'Endodontics','',0,0)";
				category=Db.NonQ(command,true);
				if(autoCodeNum!=0) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Canal - Simple',0,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAF3SURBVDhPpZQhc4UwEIRr3k+oreQn1D6JREbGIiORiMjIyNjISGQlFolEYpHI617IdNoyCTO8VUw4PjaXvbxRRsMwkJQ1LctA1Eqix4PIGxLiSVr3qeqsLNA5R01T07wEIpWAwcafWGtS1VlZYAiB2lbQPAMomwjctxlrDYA2VZ2VBY7jSF3X0jT5H4cMVErgZ1jLKAucpgluJMDuF5DXRHyXU9GhUu0BTIfCQAngsiyp6qwskD9SAH2N6Be2zsAVJ85tuA0UQhzAXv0Bruuaqs7KArdti8BhQET+OeR3OWWBrLp+kvcIcXLIEerxvO97qjirCORD0Qaw5JAjZEz/IlAD1h9APnFj9H2g9wwATB9b5n46Z+8D47SwO4s+AhiCjmP3GrBDqJPDENghgl5QEchZjA5TD3v00/v8HLOKQN6a5JvGHVtWcFuaY1YRyOILYkIWx88KQDi90CWQc2dxZbnqHT3F7X2hS+A8jxQQbl99FGf4ENE3kyiUqPrs9scAAAAASUVORK5CYII=')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+",0,"+POut.Long(autoCodeNum)+")";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("33141")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Canal - Simple - 4 Canals',1,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAAXdJREFUOE+llCFzhTAQhCveT6it5CfUPolERsYiI7GIyMjI2MhIZCUWiURikcjrXsh02jIJM7xVTDg+Npe9vFFGwzCQlDUty0DUSqLHg8gbEuJJWvep6qws0DlHTVPTvAQilYDBxp9Ya1LVWVlgCIHaVtA8AyibCNy3GWsNgDZVnZUFjuNIXdfSNPkfhwxUSuBnWMsoC5ymCW4kwO4XkNdEfJdT0aFS7QFMh8JACeCyLKnqrCyQP1IAfY3oF7bOwBUnzm24DRRCHMBe/QGu65qqzsoCt22LwGFARP455Hc5ZYGsun6S9whxcsgR6vG873uqOKsI5EPRBrDkkCNkTP8iUAPWH0A+cWP0faD3DABMH1vmfjpn7wPjtLA7iz4CGIKOY/casEOok8MQ2CGCXlARyFmMDlMPe/TT+/wcs4pA3prkm8YdW1ZwW5pjVhHI4gtiQhbHzwpAOL3QJZBzZ3FlueodPcXtfaFL4DyPFBBuX30UZ/gQ0TdkqJQOZJh66AAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("33141")+",0)";
					Db.NonQ(command);
				}
				if(autoCodeNum2!=0) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Canal - Difficult Access',2,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAAXdJREFUOE+llCFzhTAQhCveT6it5CfUPolERsYiI7GIyMjI2MhIZCUWiURikcjrXsh02jIJM7xVTDg+Npe9vFFGwzCQlDUty0DUSqLHg8gbEuJJWvep6qws0DlHTVPTvAQilYDBxp9Ya1LVWVlgCIHaVtA8AyibCNy3GWsNgDZVnZUFjuNIXdfSNPkfhwxUSuBnWMsoC5ymCW4kwO4XkNdEfJdT0aFS7QFMh8JACeCyLKnqrCyQP1IAfY3oF7bOwBUnzm24DRRCHMBe/QGu65qqzsoCt22LwGFARP455Hc5ZYGsun6S9whxcsgR6vG873uqOKsI5EPRBrDkkCNkTP8iUAPWH0A+cWP0faD3DABMH1vmfjpn7wPjtLA7iz4CGIKOY/casEOok8MQ2CGCXlARyFmMDlMPe/TT+/wcs4pA3prkm8YdW1ZwW5pjVhHI4gtiQhbHzwpAOL3QJZBzZ3FlueodPcXtfaFL4DyPFBBuX30UZ/gQ0TdkqJQOZJh66AAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+",0,"+POut.Long(autoCodeNum2)+")";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("33142")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Root Canal - Difficult Access - 4 Canals',3,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAF3SURBVDhPpZQhc4UwEIQr3k+oreQn1D6JREbGIiOxiMjIyNjISGQlFolEYpHI617IdNoyCTO8VUw4PjaXvbxRRsMwkJQ1LctA1Eqix4PIGxLiSVr3qeqsLNA5R01T07wEIpWAwcafWGtS1VlZYAiB2lbQPAMomwjctxlrDYA2VZ2VBY7jSF3X0jT5H4cMVErgZ1jLKAucpgluJMDuF5DXRHyXU9GhUu0BTIfCQAngsiyp6qwskD9SAH2N6Be2zsAVJ85tuA0UQhzAXv0Bruuaqs7KArdti8BhQET+OeR3OWWBrLp+kvcIcXLIEerxvO97qjirCORD0Qaw5JAjZEz/IlAD1h9APnFj9H2g9wwATB9b5n46Z+8D47SwO4s+AhiCjmP3GrBDqJPDENghgl5QEchZjA5TD3v00/v8HLOKQN6a5JvGHVtWcFuaY1YRyOILYkIWx88KQDi90CWQc2dxZbnqHT3F7X2hS+A8jxQQbl99FGf4ENE3ZKiUDmSYeugAAAAASUVORK5CYII=')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("33142")+",0)";
					Db.NonQ(command);
				}
				if(autoCodeNum3!=0) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Open & Drain',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+",0,"+POut.Long(autoCodeNum3)+")";
					Db.NonQ(command);
				}
				if(autoCodeNum4!=0) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Open Through Crown',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+",0,"+POut.Long(autoCodeNum4)+")";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaPeriodontics() {
			long category;//defNum
			long procButtonNum;
			string command;
			//Periodontics - Category 301
			if(ProcedureCodes.IsValidCode("42421") || ProcedureCodes.IsValidCode("42311") || ProcedureCodes.IsValidCode("42201") || ProcedureCodes.IsValidCode("43521")
				|| ProcedureCodes.IsValidCode("43529") || ProcedureCodes.IsValidCode("42531") || ProcedureCodes.IsValidCode("42341"))
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,6,'Periodontics','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("42421")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Flap Approach',0,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("42421")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("42311")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Gingivectomy',1,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("42311")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("42201")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Gingivoplasty',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("42201")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("43521")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Disinfection 1U',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("43521")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("43529")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Disinfection Add''l',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("43529")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("42531")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Free Gingival Graft',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("42531")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("42341")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Re-contouring Crown Legthening',6,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("42341")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaOralAndMaxSurg() {
			long category;//defNum
			long procButtonNum;
			string command;
			//Oral & Max Surg - Category 303
			if(ProcedureCodes.IsValidCode("71101") || ProcedureCodes.IsValidCode("71109") || ProcedureCodes.IsValidCode("71201") || ProcedureCodes.IsValidCode("71209")
				|| ProcedureCodes.IsValidCode("71211") || ProcedureCodes.IsValidCode("71219") || ProcedureCodes.IsValidCode("72111") || ProcedureCodes.IsValidCode("72119")
				|| ProcedureCodes.IsValidCode("72211") || ProcedureCodes.IsValidCode("72219") || ProcedureCodes.IsValidCode("72221") || ProcedureCodes.IsValidCode("72229")
				|| ProcedureCodes.IsValidCode("72311") || ProcedureCodes.IsValidCode("74631") || ProcedureCodes.IsValidCode("74632") || ProcedureCodes.IsValidCode("77801")
				|| ProcedureCodes.IsValidCode("77802") || ProcedureCodes.IsValidCode("79601") || ProcedureCodes.IsValidCode("79602"))
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,7,'Oral & Max Surg','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("71101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Uncomplicated',0,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAEmSURBVDhPnZStlsJADEb3SVYikVjkSuTKtUgksmJlJbIWySMgsUgksnZkZDZfmJZ2mqQ9ew2H9PQ2k5/54BIipurI/HjkQAGeNyfmlHJgzFQo0Klm2v/oyyVU/zId9uYzYAq5bTmtP/V3hGSVNis/e8EW4ljHg2YzJMq8wxaC+02yWb9r1WUn8QhfSCLYbpiv19ffc8PpexdmB3yh0B8RNR3II0Jh1xzN7murWc8RC9EcGRHUki6XHIyJhQKG2Bwhh9kM0Qjt9oL6gVioo7N6bcfM/HX4QtRPJOi0uzkGvlDWSyUYbMiNzbGwhZag3BwHW9g+p0csNsdjKkR2ch9SVeXAm/9dDn0DnjkwYEFzJkIdEamfmcWC5oyFeCG43hXpvvtBZv4DTM3piTq45i4AAAAASUVORK5CYII=')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("71101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("71109")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Add''l Uncomplicated',1,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("71109")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("71201")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Complicated',2,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAEmSURBVDhPnZStlsJADEb3SVYikVjkSuTKtUgksmJlJbIWySMgsUgksnZkZDZfmJZ2mqQ9ew2H9PQ2k5/54BIipurI/HjkQAGeNyfmlHJgzFQo0Klm2v/oyyVU/zId9uYzYAq5bTmtP/V3hGSVNis/e8EW4ljHg2YzJMq8wxaC+02yWb9r1WUn8QhfSCLYbpiv19ffc8PpexdmB3yh0B8RNR3II0Jh1xzN7murWc8RC9EcGRHUki6XHIyJhQKG2Bwhh9kM0Qjt9oL6gVioo7N6bcfM/HX4QtRPJOi0uzkGvlDWSyUYbMiNzbGwhZag3BwHW9g+p0csNsdjKkR2ch9SVeXAm/9dDn0DnjkwYEFzJkIdEamfmcWC5oyFeCG43hXpvvtBZv4DTM3piTq45i4AAAAASUVORK5CYII=')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("71201")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("71209")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Add''l Complicated',3,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("71209")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("71211")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Erupted Flap/Bone Section',4,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("71211")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("71219")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Add''l Erupted Flap/Bone Section',5,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("71219")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72111")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Soft Tissue Coverage',6,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72111")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72119")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Add''l Soft Tissue Coverage',7,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEmSURBVDhPnZStlsJADEb3RVYikVjkSuTKtUgktmJlJbIWySMgsUgksnZkZDZfmJZ2mqQ9ew2H9PQ2k5/54BIipurI/HjkQAGeNyfmlHJgzFQo0Klm2v/oyyVU/zId9uYzYAq5bTmtP/V3hGSVNis/e8EW4ljHg2YzJMq8wxaC+02yWb9r1WUn8QhfSCLYbpiv19ffc8PpexdmB3yh0B8RNR3II0Jh1xzN7murWc8RC9EcGRHUki6XHIyJhQKG2Bwhh9kM0Qjt9oL6gVioo7N6bcfM/HX4QtRPJOi0uzkGvlDWSyUYbMiNzbGwhZag3BwHW9g+p0csNsdjKkR2ch9SVeXAm/9dDn0DnjkwYEFzJkIdEamfmcWC5oyFeCG43hXpvvtBZv4DaYPpQch/bQcAAAAASUVORK5CYII=')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72119")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72211")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Surg Partial Bony',8,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72211")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72219")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Add''l Surg Partial Bony',9,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72219")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72221")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Surg Full Bony',10,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwwAADsMBx2+oZAAAASZJREFUOE+dlK2WwkAMRvdFViKRWORK5Mq1SCS2YmUlshbJIyCxSCSydmRkNl+YlnaapD17DYf09DaTn/ngEiKm6sj8eORAAZ43J+aUcmDMVCjQqWba/+jLJVT/Mh325jNgCrltOa0/9XeEZJU2Kz97wRbiWMeDZjMkyrzDFoL7TbJZv2vVZSfxCF9IIthumK/X199zw+l7F2YHfKHQHxE1HcgjQmHXHM3ua6tZzxEL0RwZEdSSLpccjImFAobYHCGH2QzRCO32gvqBWKijs3ptx8z8dfhC1E8k6LS7OQa+UNZLJRhsyI3NsbCFlqDcHAdb2D6nRyw2x2MqRHZyH1JV5cCb/10OfQOeOTBgQXMmQh0RqZ+ZxYLmjIV4IbjeFem++0Fm/gNpg+lByH9tBwAAAABJRU5ErkJggg==')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72221")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72229")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Add''l Surg Full Bony',11,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEmSURBVDhPnZStlsJADEb3RVYikVjkSuTKtUgktmJlJbIWySMgsUgksnZkZDZfmJZ2mqQ9ew2H9PQ2k5/54BIipurI/HjkQAGeNyfmlHJgzFQo0Klm2v/oyyVU/zId9uYzYAq5bTmtP/V3hGSVNis/e8EW4ljHg2YzJMq8wxaC+02yWb9r1WUn8QhfSCLYbpiv19ffc8PpexdmB3yh0B8RNR3II0Jh1xzN7murWc8RC9EcGRHUki6XHIyJhQKG2Bwhh9kM0Qjt9oL6gVioo7N6bcfM/HX4QtRPJOi0uzkGvlDWSyUYbMiNzbGwhZag3BwHW9g+p0csNsdjKkR2ch9SVeXAm/9dDn0DnjkwYEFzJkIdEamfmcWC5oyFeCG43hXpvvtBZv4DaYPpQch/bQcAAAAASUVORK5CYII=')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72229")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72311")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ext Root Tip',12,"
						+POut.Long(category)+@",'iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAEmSURBVDhPnZStlsJADEb3RVYikVjkSuTKtUgktmJlJbIWySMgsUgksnZkZDZfmJZ2mqQ9ew2H9PQ2k5/54BIipurI/HjkQAGeNyfmlHJgzFQo0Klm2v/oyyVU/zId9uYzYAq5bTmtP/V3hGSVNis/e8EW4ljHg2YzJMq8wxaC+02yWb9r1WUn8QhfSCLYbpiv19ffc8PpexdmB3yh0B8RNR3II0Jh1xzN7murWc8RC9EcGRHUki6XHIyJhQKG2Bwhh9kM0Qjt9oL6gVioo7N6bcfM/HX4QtRPJOi0uzkGvlDWSyUYbMiNzbGwhZag3BwHW9g+p0csNsdjKkR2ch9SVeXAm/9dDn0DnjkwYEFzJkIdEamfmcWC5oyFeCG43hXpvvtBZv4DaYPpQch/bQcAAAAASUVORK5CYII=')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72311")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("74631")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Excision of Cyst (up to 1cm)',13,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("74631")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("74632")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Excision of Cyst (over 1cm)',14,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("74632")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("77801")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Frenectomy Upper Labial',15,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("77801")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("77802")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Frenectomy Lower Labial',16,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("77802")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("79601")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Post-op Check',17,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("79601")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("79602")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Post-op Check (other dentist)',18,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("79601")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaImplants() {
			long category;//defNum
			long procButtonNum;
			string command;
			//Implants - Category 365
			if(ProcedureCodes.IsValidCode("01703") || ProcedureCodes.IsValidCode("04721") || ProcedureCodes.IsValidCode("04911") || ProcedureCodes.IsValidCode("72421")
				|| ProcedureCodes.IsValidCode("42552") || ProcedureCodes.IsValidCode("74402") || ProcedureCodes.IsValidCode("42561") || ProcedureCodes.IsValidCode("27215")
				|| ProcedureCodes.IsValidCode("27205") || ProcedureCodes.IsValidCode("79931") || ProcedureCodes.IsValidCode("29111") || ProcedureCodes.IsValidCode("29112")
				|| ProcedureCodes.IsValidCode("79355") || ProcedureCodes.IsValidCode("79352") || ProcedureCodes.IsValidCode("62105") || ProcedureCodes.IsValidCode("67205")
				|| ProcedureCodes.IsValidCode("66213") || ProcedureCodes.IsValidCode("29331"))
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,8,'Implants','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("01703")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Consultation',0,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01703")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("04721")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Wax-Up',1,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("04721")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("04911")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Study Models',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("04911")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("72421")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Socket Preservation Graft',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("72421")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("42552")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Bone Graft - Allograft',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("42552")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("74402")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Allograft/tunnel Block',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("74402")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("42561")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ridge Augmentation - Alveoplasty',6,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("42561")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("27215")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('PFM Implant Crown',7,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("27215")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("27205")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Porcelain Implant Crown',8,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("27205")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("79931")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Implant',9,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("79931")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29111")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Recement Imp Crn 1U',10,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29111")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29112")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Recement Imp Crn 2U',11,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29112")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("79355")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Sinus Lift SA2',12,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("79355")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("79352")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Sinus Lift SA3',13,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("79352")+",0)";
					Db.NonQ(command);
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Sinus Lift SA4',14,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("79352")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("62105")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Implant Pontic',15,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("62105")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("67205")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Implant Crown Abutment',16,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("67205")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("66213")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Implant Crown Conv',17,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("66213")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("29331")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Implant Removal',18,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("29331")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaDenturesAndPartials() {
			long category;//defNum
			long procButtonNum;
			string command;
			//Dentures & Partials - Category 302
			if(ProcedureCodes.IsValidCode("51101") || ProcedureCodes.IsValidCode("51102") || ProcedureCodes.IsValidCode("52101") || ProcedureCodes.IsValidCode("52102")
				|| ProcedureCodes.IsValidCode("54201") || ProcedureCodes.IsValidCode("54202") || ProcedureCodes.IsValidCode("55101") || ProcedureCodes.IsValidCode("55102")
				|| ProcedureCodes.IsValidCode("55501") || ProcedureCodes.IsValidCode("55102") || ProcedureCodes.IsValidCode("56211") || ProcedureCodes.IsValidCode("56212"))
			{
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,9,'Dentures & Partials','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("51101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Comp Denture Max',0,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("51101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("51102")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Comp Denture Mand',1,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("51102")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("52101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Partial Acryllic Max',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("52101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("52102")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Partial Acryllic Mand',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("52102")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("54201")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Denture Adjustment 1U',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("54201")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("54202")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Denture Adjustment 2U',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("54202")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("55101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Denture Repair Max',6,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("55101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("55102")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Denture Repair Mand',7,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("55102")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("55501")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Denture Prophy 1U',8,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("55501")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("55102")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Hader Clips',9,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("55102")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("56211")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Soft Reline CUD',10,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("56211")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("56212")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Soft Reline CLD',11,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("56212")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaOrthodontics() {
			long category;//defNum
			long procButtonNum;
			string command;
			//Orthodontics - Category 304
			if(ProcedureCodes.IsValidCode("01901") || ProcedureCodes.IsValidCode("93331") || ProcedureCodes.IsValidCode("93334") || ProcedureCodes.IsValidCode("80601")
				|| ProcedureCodes.IsValidCode("93332") || ProcedureCodes.IsValidCode("80631") || ProcedureCodes.IsValidCode("84101") || ProcedureCodes.IsValidCode("85101"))
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,10,'Orthodontics','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("01901")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Consult',0,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("01901")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("93331")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Records',1,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("93331")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("93334")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Appliance',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("93334")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("80601")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Observations',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("80601")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("93332")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Monthly',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("93332")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("80631")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Appl Repair',5,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("80631")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("84101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Class I Malocclusion Perm',6,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("84101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("85101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Ortho Class I Malocclusion Mixed',7,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("85101")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanadaMisc() {
			long category;//defNum
			long procButtonNum;
			string command;
			//Misc - Category 367
			if(ProcedureCodes.IsValidCode("14611") || ProcedureCodes.IsValidCode("20111") || ProcedureCodes.IsValidCode("83101") || ProcedureCodes.IsValidCode("Ztoth")
				|| ProcedureCodes.IsValidCode("Ztoths"))
			{ 
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES (26,11,'Misc','',0,0)";
				category=Db.NonQ(command,true);
				if(ProcedureCodes.IsValidCode("14611")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Nightguard',0,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("14611")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("20111")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Silver D',1,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("20111")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("83101")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Essex Retainer',2,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("83101")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("Ztoth")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Watch Tooth',3,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("Ztoth")+",0)";
					Db.NonQ(command);
				}
				if(ProcedureCodes.IsValidCode("Ztoths")) {
					command="INSERT INTO procbutton (Description,ItemOrder,Category,ButtonImage) VALUES('Watch Tooth Surface',4,"
						+POut.Long(category)+@",'')";
					procButtonNum=Db.NonQ(command,true);
					command="INSERT INTO procbuttonitem (ProcButtonNum,CodeNum,AutoCodeNum) VALUES ("+POut.Long(procButtonNum)
							+","+ProcedureCodes.GetCodeNum("Ztoths")+",0)";
					Db.NonQ(command);
				}
			}
		}

		private static void SetToDefaultMySQLCanada() {
			SetToDefaultMySQLCanadaExams();
			SetToDefaultMySQLCanadaXrays();
			SetToDefaultMySQLCanadaHygiene();
			SetToDefaultMySQLCanadaRestorative();
			SetToDefaultMySQLCanadaCrownAndBridge();
			SetToDefaultMySQLCanadaEndodontics();
			SetToDefaultMySQLCanadaPeriodontics();
			SetToDefaultMySQLCanadaOralAndMaxSurg();
			SetToDefaultMySQLCanadaImplants();
			SetToDefaultMySQLCanadaDenturesAndPartials();
			SetToDefaultMySQLCanadaOrthodontics();
			SetToDefaultMySQLCanadaMisc();
		}

		private static void SetToDefaultOracle() {
			throw new ApplicationException("SetToDefaultOracle is not currently Oracle compatible.  Please call support.");
		}
	}

}