using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;
using WebServiceSerializer;

namespace OpenDentBusiness.WebTypes.WebForms {
	public class WebForms_SheetDefs {

		/// <summary></summary>
		/// <param name="regKey"></param>
		/// <returns></returns>
		public static bool TryDownloadSheetDefs(out List<WebForms_SheetDef> listWebFormSheetDefs,string regKey=null) {
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			listWebFormSheetDefs=new List<WebForms_SheetDef>();
			try {
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,new PayloadItem(regKey,"RegKey"));
				listWebFormSheetDefs=WebSerializer.DeserializeTag<List<WebForms_SheetDef>>(
					SheetsSynchProxy.GetWebServiceInstance().DownloadSheetDefs(payload),"Success"
				);
			}
			catch(Exception ex) {
				ex.DoNothing();
				return false;
			}
			return true;
		}

		///<summary>This method can throw an exception. Tries to upload a sheet def to HQ.</summary>
		///<param name="sheetDef">The SheetDef object to be uploaded.</param>
		public static void TryUploadSheetDef(SheetDef sheetDef) {
			string regKey=PrefC.GetString(PrefName.RegistrationKey);
			List<PayloadItem> listPayloadItems=new List<PayloadItem> {
				new PayloadItem(regKey,"RegKey"),
				new PayloadItem(sheetDef,"SheetDef")
			};
			string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
			string result=SheetsSynchProxy.GetWebServiceInstance().UpLoadSheetDef(payload);
			PayloadHelper.CheckForError(result);
		}

		/// <summary>Takes a provided sheetDef and packages it into a payload string. The payload string is broken into chunks of equal or lesser size
		/// than the provided chunkSize. Size is measured in bytes.</summary>
		public static void TryUploadSheetDefChunked(SheetDef sheetDef,int chunkSize) {
			string regKey=PrefC.GetString(PrefName.RegistrationKey);
			List<PayloadItem> listPayloadItems=new List<PayloadItem> {
				new PayloadItem(regKey,"RegKey"),
				new PayloadItem(sheetDef,"SheetDef")
			};
			string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
			string fileNamePayload=UploadSheetChunks(payload,chunkSize);
			string result=SheetsSynchProxy.GetWebServiceInstance().UploadSheetDefFromFile(fileNamePayload);
			PayloadHelper.CheckForError(result);
		}

		/// <summary></summary>
		/// <param name="regKey"></param>
		/// <param name="webSheetDefId"></param>
		public static bool DeleteSheetDef(long webSheetDefId,string regKey=null) {
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			try {
				List<PayloadItem> listPayloadItems=new List<PayloadItem> {
					new PayloadItem(regKey,"RegKey"),
					new PayloadItem(webSheetDefId,"WebSheetDefID")
				};
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
				SheetsSynchProxy.GetWebServiceInstance().DeleteSheetDef(payload);
			}
			catch (Exception ex) {
				ex.DoNothing();
				return false;
			}
			return true;
		}

		/// <summary></summary>
		/// <param name="regKey"></param>
		/// <param name="webSheetDefId"></param>
		/// <param name="sheetDef"></param>
		public static bool UpdateSheetDef(long webSheetDefId,SheetDef sheetDef,string regKey=null,bool doCatchExceptions=true) {
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			try {
				List<PayloadItem> listPayloadItems=new List<PayloadItem> {
					new PayloadItem(regKey,"RegKey"),
					new PayloadItem(webSheetDefId,"WebSheetDefID"),
					new PayloadItem(sheetDef,"SheetDef")
				};
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
				SheetsSynchProxy.GetWebServiceInstance().UpdateSheetDef(payload);
			}
			catch (Exception ex) {
				if(!doCatchExceptions) {
					throw;
				}
				ex.DoNothing();
				return false;
			}
			return true;
		}

		/// <summary>Takes a provided webSheetDefId and sheetDef and packages them. The package is then broken into chunks of equal or lesser size
		/// than the provided chunkSize. Size is measured in bytes.</summary>
		public static bool UpdateSheetDefChunked(long webSheetDefId,SheetDef sheetDef,int chunkSize) {
			string regKey=PrefC.GetString(PrefName.RegistrationKey);
			List<PayloadItem> listPayloadItems=new List<PayloadItem> {
				new PayloadItem(regKey,"RegKey"),
				new PayloadItem(webSheetDefId,"WebSheetDefID"),
				new PayloadItem(sheetDef,"SheetDef")
			};
			string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
			string fileNamePayload=UploadSheetChunks(payload,chunkSize);
			string result=SheetsSynchProxy.GetWebServiceInstance().UpdateSheetDefFromFile(fileNamePayload);
			PayloadHelper.CheckForError(result);
			return true;
		}

		/// <summary>Takes a payload string and has it broken into chunks of equal or less size than the provided chunk size.</summary>
		/// <returns>xml payload containing the name of the file where chunk pieces were stored on the server.</returns>
		private static string UploadSheetChunks(string payload, int chunkSize) {
			List<string> listChunks=MiscUtils.CutStringIntoSimilarSizedChunks(payload,chunkSize);
			string fileName="";
			foreach(string chunk in listChunks) {
				List<PayloadItem> listChunkPayloadItems=new List<PayloadItem>{
					new PayloadItem(fileName,"FileName"),
					new PayloadItem(chunk,"ChunkData")
				};
				string chunkPayload=PayloadHelper.CreatePayloadContent(listChunkPayloadItems);
				string result=SheetsSynchProxy.GetWebServiceInstance().UploadSheetDefChunk(chunkPayload);
				PayloadHelper.CheckForError(result);
				fileName=WebSerializer.DeserializeTag<string>(result,"FileName");
			}
			return PayloadHelper.CreatePayloadContent(fileName,"FileName");
		}
	}
}
