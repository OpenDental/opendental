using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebForms.Crud;
using System;
using System.Collections.Generic;
using System.Reflection;
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

		#region WebHostSynch

		///<summary>Returns a WebForms_SheetDef object comprised from the values found from the sheetDef passed in.
		///Mainly used for backwards compatibility with the old web methods.  Throws exceptions.</summary>
		private static WebForms_SheetDef CopySheetDefToWebForms(SheetDef sheetDef,long dentalOfficeID,long webSheetDefID,long registrationKeyNum) {
			//Convert ODBiz SheetDef to WebForms SheetDef.
			WebForms_SheetDef ret=new WebForms_SheetDef() {
				DentalOfficeID=dentalOfficeID,
				//Retain the original primary key.
				WebSheetDefID=webSheetDefID,
				Description=sheetDef.Description,
				FontName=sheetDef.FontName,
				SheetType=(SheetTypeEnum)sheetDef.SheetType,
				FontSize=sheetDef.FontSize,
				Width=sheetDef.Width,
				Height=sheetDef.Height,
				IsLandscape=sheetDef.IsLandscape,
				SheetDefNum=sheetDef.SheetDefNum,
				HasMobileLayout=sheetDef.HasMobileLayout,
				SheetFieldDefs=new List<WebForms_SheetFieldDef>(),
				RegistrationKeyNum=registrationKeyNum,
				RevID=sheetDef.RevID,
			};
			//Convert ODBiz SheetFieldDefs to WebForms SheetFieldDefs.
			foreach(SheetFieldDef sheetFieldDef in sheetDef.SheetFieldDefs) {//assign several webforms_sheetfielddef
				WebForms_SheetFieldDef sheetFieldDefWebForms=new WebForms_SheetFieldDef();
				sheetFieldDefWebForms.WebSheetDefID=ret.WebSheetDefID;
				ret.SheetFieldDefs.Add(sheetFieldDefWebForms);
				// assign each property of a single webforms_sheetfielddef with corresponding values.
				foreach(FieldInfo sheetFieldDef_fieldInfo in sheetFieldDef.GetType().GetFields()) {
					foreach(FieldInfo webFormFieldDef_fieldInfo in sheetFieldDefWebForms.GetType().GetFields()) {
						if(sheetFieldDef_fieldInfo.Name!=webFormFieldDef_fieldInfo.Name) {
							continue;
						}
						if(sheetFieldDef_fieldInfo.GetValue(sheetFieldDef)==null) {
							webFormFieldDef_fieldInfo.SetValue(sheetFieldDefWebForms,"");
						}
						else {
							webFormFieldDef_fieldInfo.SetValue(sheetFieldDefWebForms,sheetFieldDef_fieldInfo.GetValue(sheetFieldDef));
						}
					}//foreach webFormFieldDef_propertyinfo
				}//foreach sheetFieldDef_fieldinfo
			}
			return ret;
		}

		///<summary></summary>
		public static void Delete(long webSheetDefID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webSheetDefID);
				return;
			}
			DataAction.Run(() => WebForms_SheetDefCrud.Delete(webSheetDefID),ConnectionNames.WebForms);
		}

		public static void DeleteSheetDef(long dentalOfficeID,long registrationKeyNum,string officeData) {
			long sheetDefNum=WebSerializer.DeserializeTag<long>(officeData,"WebSheetDefID");
			//Delete fields defs first.
			WebForms_SheetFieldDefs.DeleteForWebSheetDefID(sheetDefNum);
			//It is now safe to delete the sheet.
			Delete(sheetDefNum);
		}

		public static List<WebForms_SheetDef> DownloadSheetDefs(long dentalOfficeID,long registrationKeyNum,string officeData) {
			//We intentionally retrieve all sheet defs for the provided regkey AND all sheet defs without a regkey set
			List<WebForms_SheetDef> listSheetDefs=RefreshForRegistrationKeyNum(registrationKeyNum);
			listSheetDefs.AddRange(RefreshForDentalOfficeID(dentalOfficeID).FindAll(x => x.RegistrationKeyNum==0));
			if(listSheetDefs.Count==0) {//We're assuming this means they are either on an old version or have not setup RKID yet.
				//Only get SheetDefs without a RegistrationKeyNum set, at this point we know anything with RegistrationKeyNum > 0 is not for this office.
				listSheetDefs=RefreshForDentalOfficeID(dentalOfficeID).FindAll(x => x.RegistrationKeyNum==0);
			}
			return listSheetDefs;
		}

		///<summary>Gets one WebForms_SheetDef from the db.</summary>
		public static WebForms_SheetDef GetOne(long webSheetDefID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<WebForms_SheetDef>(MethodBase.GetCurrentMethod(),webSheetDefID);
			}
			return DataAction.GetT(() => WebForms_SheetDefCrud.SelectOne(webSheetDefID),ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static long Insert(WebForms_SheetDef webForms_SheetDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				webForms_SheetDef.WebSheetDefID=Meth.GetLong(MethodBase.GetCurrentMethod(),webForms_SheetDef);
				return webForms_SheetDef.WebSheetDefID;
			}
			return DataAction.GetT(() => WebForms_SheetDefCrud.Insert(webForms_SheetDef),ConnectionNames.WebForms);
		}

		public static void InsertSheetDef(long dentalOfficeID,long registrationKeyNum,string officeData) {
			//Client sent us the new version.
			SheetDef sheetDef=WebSerializer.DeserializeTag<SheetDef>(officeData,"SheetDef");
			//Convert to WebForms object. This will not set FK of field defs (we don't have it yet). 
			//That needs to be done after insert of the Sheet below.
			WebForms_SheetDef sheetDefWebForms=CopySheetDefToWebForms(sheetDef,dentalOfficeID,0,registrationKeyNum);
			//Always an insert.
			Insert(sheetDefWebForms);
			//Insert new field defs.
			sheetDefWebForms.SheetFieldDefs.ForEach(x => {
				//FK was not available when these were created above so set it now.
				x.WebSheetDefID=sheetDefWebForms.WebSheetDefID;
			});
			WebForms_SheetFieldDefs.InsertMany(sheetDefWebForms.SheetFieldDefs);
		}

		///<summary></summary>
		public static List<WebForms_SheetDef> RefreshForDentalOfficeID(long dentalOfficeID) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_SheetDef>>(MethodBase.GetCurrentMethod(),dentalOfficeID);
			}
			return DataAction.GetT(() => {
				string command="SELECT * FROM webforms_sheetdef WHERE DentalOfficeID = "+POut.Long(dentalOfficeID);
				return WebForms_SheetDefCrud.SelectMany(command);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static List<WebForms_SheetDef> RefreshForRegistrationKeyNum(long RegistrationKeyNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_SheetDef>>(MethodBase.GetCurrentMethod(),RegistrationKeyNum);
			}
			return DataAction.GetT(() => {
				string command="SELECT * FROM webforms_sheetdef WHERE RegistrationKeyNum = "+POut.Long(RegistrationKeyNum);
				return WebForms_SheetDefCrud.SelectMany(command);
			},ConnectionNames.WebForms);
		}

		///<summary></summary>
		public static void Update(WebForms_SheetDef webForms_SheetDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),webForms_SheetDef);
				return;
			}
			DataAction.Run(() => WebForms_SheetDefCrud.Update(webForms_SheetDef),ConnectionNames.WebForms);
		}

		public static void UpdateSheetDef(long dentalOfficeID,long registrationKeyNum,string officeData) {
			long webSheetDefID=WebSerializer.DeserializeTag<long>(officeData,"WebSheetDefID");
			WebForms_SheetDef sheetDefWebForms=GetOne(webSheetDefID);
			if(sheetDefWebForms==null) {
				throw new ApplicationException("Corresponding Web Form not found.");
			}
			//The WebForm SheetDef is already associated to a regkey and its not this one.
			if(sheetDefWebForms.RegistrationKeyNum>0 && sheetDefWebForms.RegistrationKeyNum!=registrationKeyNum) {
				throw new ApplicationException("Corresponding Web Form is associated to a different Registration Key.  Please create and upload a new Sheet Def.");
			}
			//Client sent us the new version.
			SheetDef sheetDef=WebSerializer.DeserializeTag<SheetDef>(officeData,"SheetDef");
			//Delete existing defs associated to the WebSheetDefID.
			WebForms_SheetFieldDefs.DeleteForWebSheetDefID(sheetDefWebForms.WebSheetDefID);
			//It is important that we retain the original WebSheetDefID so copy in all new fields but retain the original db row and PK.
			//This PK is baked into the WebForm URL that the customer has likely already published.
			sheetDefWebForms=CopySheetDefToWebForms(sheetDef,dentalOfficeID,sheetDefWebForms.WebSheetDefID,registrationKeyNum);
			//Always an update.
			Update(sheetDefWebForms);
			//Original field defs were deleted above so insert all new ones.
			WebForms_SheetFieldDefs.InsertMany(sheetDefWebForms.SheetFieldDefs);
		}

		#endregion
	}
}
