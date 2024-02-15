using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EhrSummaryCcds{
		///<summary>This will be null if EHR didn't load up.  EHRTEST conditional compilation constant is used because the EHR project is only part of the solution here at HQ.  We need to use late binding in a few places so that it will still compile for people who download our sourcecode.  But late binding prevents us from stepping through for debugging, so the EHRTEST lets us switch to early binding.</summary>
		public static object ObjFormEhrMeasures;
		///<summary>This will be null if EHR didn't load up.</summary>
		public static Assembly AssemblyEHR;
		#region CachePattern

		private class EhrSummaryCcdCache : CacheListAbs<EhrSummaryCcd> {
			protected override List<EhrSummaryCcd> GetCacheFromDb() {
				string command="SELECT * FROM ehrsummaryccd";
				return Crud.EhrSummaryCcdCrud.SelectMany(command);
			}
			protected override List<EhrSummaryCcd> TableToList(DataTable table) {
				return Crud.EhrSummaryCcdCrud.TableToList(table);
			}
			protected override EhrSummaryCcd Copy(EhrSummaryCcd ehrSummaryCcd) {
				return ehrSummaryCcd.Copy();
			}
			protected override DataTable ListToTable(List<EhrSummaryCcd> listEhrSummaryCcds) {
				return Crud.EhrSummaryCcdCrud.ListToTable(listEhrSummaryCcds,"EhrSummaryCcd");
			}
			protected override void FillCacheIfNeeded() {
				EhrSummaryCcds.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EhrSummaryCcdCache _ehrSummaryCcdCache=new EhrSummaryCcdCache();

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_ehrSummaryCcdCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_ehrSummaryCcdCache.FillCacheFromTable(table);
				return table;
			}
			return _ehrSummaryCcdCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		
		///<summary></summary>
		public static List<EhrSummaryCcd> Refresh(long patNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<EhrSummaryCcd>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM ehrsummaryccd WHERE PatNum = "+POut.Long(patNum)+" ORDER BY DateSummary";
			return Crud.EhrSummaryCcdCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(EhrSummaryCcd ehrSummaryCcd){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				ehrSummaryCcd.EhrSummaryCcdNum=Meth.GetLong(MethodBase.GetCurrentMethod(),ehrSummaryCcd);
				return ehrSummaryCcd.EhrSummaryCcdNum;
			}
			return Crud.EhrSummaryCcdCrud.Insert(ehrSummaryCcd);
		}

		///<summary>Returns null if no record is found.</summary>
		public static EhrSummaryCcd GetOneForEmailAttach(long emailAttachNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<EhrSummaryCcd>(MethodBase.GetCurrentMethod(),emailAttachNum);
			}
			string command="SELECT * FROM ehrsummaryccd WHERE EmailAttachNum="+POut.Long(emailAttachNum)+" LIMIT 1";
			return Crud.EhrSummaryCcdCrud.SelectOne(command);
		}

		///<summary></summary>
		public static void Update(EhrSummaryCcd ehrSummaryCcd) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrSummaryCcd);
				return;
			}
			Crud.EhrSummaryCcdCrud.Update(ehrSummaryCcd);
		}

		///<summary>Constructs the ObjFormEhrMeasures fro use with late binding.</summary>
		private static void constructObjFormEhrMeasuresHelper() {
			string dllPathEHR=ODFileUtils.CombinePaths(System.Windows.Forms.Application.StartupPath,"EHR.dll");
			ObjFormEhrMeasures=null;
			AssemblyEHR=null;
			if(File.Exists(dllPathEHR)) {//EHR.dll is available, so load it up
				AssemblyEHR=Assembly.LoadFile(dllPathEHR);
				Type type=AssemblyEHR.GetType("EHR.FormEhrMeasures");//namespace.class
				ObjFormEhrMeasures=Activator.CreateInstance(type);
				return;
			}
			#if EHRTEST
				ObjFormEhrMeasures=new FormEhrMeasures();
			#endif
		}

		///<summary>Loads a resource file from the EHR assembly and returns the file text as a string.
		///Returns "" is the EHR assembly did not load. strResourceName can be either "CCD" or "CCR".
		///This function performs a late binding to the EHR.dll, because resellers do not have EHR.dll necessarily.</summary>
		public static string GetEhrResource(string strResourceName) {
			if(AssemblyEHR==null) {
				constructObjFormEhrMeasuresHelper();
				if(AssemblyEHR==null) {
					return "";
				}
			}
			Stream stream=AssemblyEHR.GetManifestResourceStream("EHR.Properties.Resources.resources");
			System.Resources.ResourceReader resourceReader=new System.Resources.ResourceReader(stream);
			string strResourceType="";
			byte[] byteArrayResource=null;
			resourceReader.GetResourceData(strResourceName,out strResourceType,out byteArrayResource);
			resourceReader.Dispose();
			stream.Dispose();
			MemoryStream memoryStream=new MemoryStream(byteArrayResource);
			BinaryReader binaryReader=new BinaryReader(memoryStream);
			string retVal=binaryReader.ReadString();//Removes the leading binary characters from the string.
			memoryStream.Dispose();
			binaryReader.Dispose();
			return retVal;
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.


		///<summary>Gets one EhrSummaryCcd from the db.</summary>
		public static EhrSummaryCcd GetOne(long ehrSummaryCcdNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<EhrSummaryCcd>(MethodBase.GetCurrentMethod(),ehrSummaryCcdNum);
			}
			return Crud.EhrSummaryCcdCrud.SelectOne(ehrSummaryCcdNum);
		}

		///<summary></summary>
		public static void Delete(long ehrSummaryCcdNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrSummaryCcdNum);
				return;
			}
			string command= "DELETE FROM ehrsummaryccd WHERE EhrSummaryCcdNum = "+POut.Long(ehrSummaryCcdNum);
			Db.NonQ(command);
		}
		*/
	}
}