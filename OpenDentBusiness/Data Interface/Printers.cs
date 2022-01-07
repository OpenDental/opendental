using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness{

	///<summary>Handles all the business logic for printers.  Used heavily by the UI.  Every single function that makes changes to the database must be completely autonomous and do ALL validation itself.</summary>
	public class Printers{
		#region CachePattern

		private class PrinterCache : CacheListAbs<Printer> {
			protected override List<Printer> GetCacheFromDb() {
				string command="SELECT * FROM printer";
				return Crud.PrinterCrud.SelectMany(command);
			}
			protected override List<Printer> TableToList(DataTable table) {
				return Crud.PrinterCrud.TableToList(table);
			}
			protected override Printer Copy(Printer Printer) {
				return Printer.Clone();
			}
			protected override DataTable ListToTable(List<Printer> listPrinters) {
				return Crud.PrinterCrud.ListToTable(listPrinters,"Printer");
			}
			protected override void FillCacheIfNeeded() {
				Printers.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PrinterCache _PrinterCache=new PrinterCache();

		public static Printer GetFirstOrDefault(Func<Printer,bool> match,bool isShort=false) {
			return _PrinterCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_PrinterCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_PrinterCache.FillCacheFromTable(table);
				return table;
			}
			return _PrinterCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Gets directly from database</summary>
		public static Printer GetOnePrinter(PrintSituation sit,long compNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Printer>(MethodBase.GetCurrentMethod(),sit,compNum);
			}
			string command="SELECT * FROM printer WHERE "
				+"PrintSit = '"      +POut.Long((int)sit)+"' "
				+"AND ComputerNum ='"+POut.Long(compNum)+"'";
			return Crud.PrinterCrud.SelectOne(command);
		}

		///<summary></summary>
		private static long Insert(Printer cur) {
			//No need to check RemotingRole; private static.
			return Crud.PrinterCrud.Insert(cur);
		}

		///<summary></summary>
		private static void Update(Printer cur){
			//No need to check RemotingRole; private static.
			Crud.PrinterCrud.Update(cur);
		}

		///<summary></summary>
		private static void Delete(Printer cur){
			//No need to check RemotingRole; private static.
			string command="DELETE FROM printer "
				+"WHERE PrinterNum = "+POut.Long(cur.PrinterNum);
			Db.NonQ(command);
		}

		public static bool PrinterIsInstalled(string name){
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<PrinterSettings.InstalledPrinters.Count;i++){
				if(PrinterSettings.InstalledPrinters[i]==name){
					return true;
				}
			}
			return false;
		}

		///<summary>Gets the set printer whether or not it is valid.  Returns null if the current computer OR printer cannot be found.</summary>
		public static Printer GetForSit(PrintSituation sit) {
			//No need to check RemotingRole; no call to db.
			Computer compCur=Computers.GetCur();
			if(compCur==null) {
				return null;
			}
			return GetFirstOrDefault(x => x.ComputerNum==compCur.ComputerNum && x.PrintSit==sit);
		}

		///<summary>Either does an insert or an update to the database if need to create a Printer object.  Or it also deletes a printer object if needed.</summary>
		public static void PutForSit(PrintSituation sit,string computerName, string printerName,bool displayPrompt){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sit,computerName,printerName,displayPrompt);
				return;
			}
			//Computer[] compList=Computers.Refresh();
			//Computer compCur=Computers.GetCur();
			string command="SELECT ComputerNum FROM computer "
				+"WHERE CompName = '"+POut.String(computerName)+"'";
 			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0){
				return;//computer not yet entered in db.
			}
			long compNum=PIn.Long(table.Rows[0][0].ToString());
			//only called from PrinterSetup window. Get info directly from db, then refresh when closing window. 
			Printer existing=GetOnePrinter(sit,compNum);   //GetForSit(sit);
			if(printerName=="" && !displayPrompt){//then should not be an entry in db
				if(existing!=null){//need to delete Printer
					Delete(existing);
				}
			}
			else if(existing==null){
				Printer cur=new Printer();
				cur.ComputerNum=compNum;
				cur.PrintSit=sit;
				cur.PrinterName=printerName;
				cur.DisplayPrompt=displayPrompt;
				Insert(cur);
			}
			else{
				existing.PrinterName=printerName;
				existing.DisplayPrompt=displayPrompt;
				Update(existing);
			}
		}

		///<summary>Called from FormPrinterSetup if user selects the easy option.  Since the other options will be hidden, we have to clear them.  User should be sternly warned before this happens.</summary>
		public static void ClearAll(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//first, delete all entries
			string command="DELETE FROM printer";
 			Db.NonQ(command);
			//then, add one printer for each computer. Default and show prompt
			Computers.RefreshCache();
			Printer cur;
			List<Computer> listComputers=Computers.GetDeepCopy();
			for(int i=0;i<listComputers.Count;i++) {
				cur=new Printer();
				cur.ComputerNum=listComputers[i].ComputerNum;
				cur.PrintSit=PrintSituation.Default;
				cur.PrinterName="";
				cur.DisplayPrompt=true;
				Insert(cur);
			}
		}
	}
}