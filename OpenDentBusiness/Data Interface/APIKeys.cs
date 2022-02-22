using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace OpenDentBusiness{
	///<summary></summary>
	public class APIKeys{
		///<summary>This list is filled the first time it's needed. There are two ways to "refresh the cache": from the API setup window, or by restarting the program.</summary>
		private static List<ApiKeyVisibleInfo> _listApiKeyVisibleInfos;

		///<summary>Saves the list of APIKeys to the database. Deletes all existing ones first.</summary>
		public static void SaveListAPIKeys(List<APIKey> listAPIKeys) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAPIKeys);
				return;
			}
			string command="DELETE FROM apikey";
			Db.NonQ(command);
			for(int i=0;i<listAPIKeys.Count;i++) {
				Insert(listAPIKeys[i]);
			}
		}

		///<summary>Inserts an APIKey into database. </summary>
		public static long Insert(APIKey aPIKey) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				aPIKey.APIKeyNum=Meth.GetLong(MethodBase.GetCurrentMethod(),aPIKey);
				return aPIKey.APIKeyNum;
			}
			return Crud.APIKeyCrud.Insert(aPIKey);
		}

		#region Methods - Modify


		/*
		///<summary></summary>
		public static void Update(APIKey aPIKey){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aPIKey);
				return;
			}
			Crud.APIKeyCrud.Update(aPIKey);
		}
		///<summary></summary>
		public static void Delete(long aPIKeyNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),aPIKeyNum);
				return;
			}
			Crud.APIKeyCrud.Delete(aPIKeyNum);
		}
		*/
		#endregion Methods - Modify

		///<summary>Surround with try/catch.</summary>
		public static List<ApiKeyVisibleInfo> GetListApiKeyVisibleInfos(bool forceRefresh=false) {
			if(_listApiKeyVisibleInfos!=null && !forceRefresh){
				return _listApiKeyVisibleInfos;
			}
			_listApiKeyVisibleInfos=new List<ApiKeyVisibleInfo>();
			//prepare the xml document to send--------------------------------------------------------------------------------------
			XmlWriterSettings xmlWriterSettings=new XmlWriterSettings();
			xmlWriterSettings.Indent=true;
			xmlWriterSettings.IndentChars=("    ");
			StringBuilder stringBuilder=new StringBuilder();
			//Send the message and get the result-------------------------------------------------------------------------------------
			string result="";
			string officeData=PayloadHelper.CreatePayload(stringBuilder.ToString(),eServiceCode.FHIR);
			try {
				result=WebServiceMainHQProxy.GetWebServiceMainHQInstance().GetFHIRAPIKeysForOffice(officeData);
			}
			catch(Exception ex) {
				throw ex;
			}
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(result);
			XPathNavigator xPathNavigator=xmlDocument.CreateNavigator();
			//Process errors------------------------------------------------------------------------------------------------------------
			XPathNavigator xPathNavigatorNode=xPathNavigator.SelectSingleNode("//Error");
			if(xPathNavigatorNode!=null) {
				throw new Exception(xPathNavigatorNode.Value);
			}
			//Process a valid return value------------------------------------------------------------------------------------------------{
			xPathNavigatorNode=xPathNavigator.SelectSingleNode("//ListAPIKeys");
			if(xPathNavigatorNode==null || !xPathNavigatorNode.MoveToFirstChild()) {
				return _listApiKeyVisibleInfos;
			}
			do {
				ApiKeyVisibleInfo apiKeyVisibleInfo=new ApiKeyVisibleInfo();
				apiKeyVisibleInfo.CustomerKey=xPathNavigatorNode.SelectSingleNode("APIKeyValue").Value;
				apiKeyVisibleInfo.FHIRAPIKeyNum=PIn.Long(xPathNavigatorNode.SelectSingleNode("FHIRAPIKeyNum").Value);
				apiKeyVisibleInfo.DateDisabled=DateTime.Parse(xPathNavigatorNode.SelectSingleNode("DateDisabled").Value);
				if(!Enum.TryParse(xPathNavigatorNode.SelectSingleNode("KeyStatus").Value,out apiKeyVisibleInfo.FHIRKeyStatusCur)) {
					APIKeyStatus status;
					if(Enum.TryParse(xPathNavigatorNode.SelectSingleNode("KeyStatus").Value,out status)) {
						apiKeyVisibleInfo.FHIRKeyStatusCur=FHIRUtils.ToFHIRKeyStatus(status);
					}
					else {
						apiKeyVisibleInfo.FHIRKeyStatusCur=FHIRKeyStatus.DisabledByHQ;
					}
				}
				apiKeyVisibleInfo.DeveloperName=xPathNavigatorNode.SelectSingleNode("DeveloperName").Value;
				apiKeyVisibleInfo.DeveloperEmail=xPathNavigatorNode.SelectSingleNode("DeveloperEmail").Value;
				apiKeyVisibleInfo.DeveloperPhone=xPathNavigatorNode.SelectSingleNode("DeveloperPhone").Value;
				apiKeyVisibleInfo.FHIRDeveloperNum=PIn.Long(xPathNavigatorNode.SelectSingleNode("FHIRDeveloperNum").Value);
				XPathNavigator xPathNavigatorNodePerms=xPathNavigatorNode.SelectSingleNode("ListAPIPermissions");
				if(xPathNavigatorNodePerms==null || !xPathNavigatorNodePerms.MoveToFirstChild()) {
					_listApiKeyVisibleInfos.Add(apiKeyVisibleInfo);
					continue;
				}
				do {
					APIPermission apiPermission;
					if(Enum.TryParse(xPathNavigatorNodePerms.Value,out apiPermission)) {
						apiKeyVisibleInfo.ListAPIPermissions.Add(apiPermission);
					}
				} while(xPathNavigatorNodePerms.MoveToNext());
				_listApiKeyVisibleInfos.Add(apiKeyVisibleInfo);
			} while(xPathNavigatorNode.MoveToNext());
			return _listApiKeyVisibleInfos;
		}



	}
}