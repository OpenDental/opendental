﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Handles db actions for the HQ bugs.databaseintegrity table.</summary>
	public class DatabaseIntegrities {
		///<summary>This is only grabbed once, so the only way to refresh is to restart OD. It's lazy loaded the first time that a WarningIntegrity triangle is encountered or a plugin attempts to load.</summary>
		private static List<DatabaseIntegrity> _listDatabaseIntegrities;

		///<summary>True if the plugin is allowed to silently load without any warning. The pluginName should include the dll extension.</summary>
		public static bool IsPluginAllowed(string pluginName){
			if(_listDatabaseIntegrities is null){
				RefreshFromHQ();
			}
			if(_listDatabaseIntegrities.Count==0){
				RefreshCacheFromPref(); 
			}
			if(_listDatabaseIntegrities.Count==0){
				//must have had trouble connecting to HQ,
				//so just let them use the plugin
				return true;
			}
			DatabaseIntegrity databaseIntegrity=_listDatabaseIntegrities.Find(x=>x.PluginName.ToLower()==pluginName.ToLower() && x.WarningIntegrityType==EnumWarningIntegrityType.PluginOverride);
			if(databaseIntegrity!=null){
				if(databaseIntegrity.Behavior==EnumIntegrityBehavior.PluginAllow){
					return true;
				}
				return false;
			}
			DatabaseIntegrity databaseIntegrityDefault=_listDatabaseIntegrities.Find(x=>x.WarningIntegrityType==EnumWarningIntegrityType.DefaultPlugin);
			if(databaseIntegrityDefault is null){
				//there should always be a defaultPlugin, so there must be something wrong with db
				return true;//allow
			}
			if(databaseIntegrityDefault.Behavior==EnumIntegrityBehavior.PluginAllow){
				return true;
			}
			return false;
		}

		///<summary>Can return null. Will return a type of PluginOverride. If none, then PluginDefault. Or null.</summary>
		public static DatabaseIntegrity GetOnePlugin(string pluginName){
			if(_listDatabaseIntegrities is null) {
				RefreshFromHQ();
			}
			if(_listDatabaseIntegrities.Count==0){
				RefreshCacheFromPref();
			}
			if(_listDatabaseIntegrities.Count==0){
				//must have had trouble connecting to HQ,
				return null;
			}
			DatabaseIntegrity databaseIntegrity=_listDatabaseIntegrities.Find(x=>x.PluginName.ToLower()==pluginName.ToLower() && x.WarningIntegrityType==EnumWarningIntegrityType.PluginOverride);
			if(databaseIntegrity!=null){
				return databaseIntegrity;
			}
			DatabaseIntegrity databaseIntegrityDefault=_listDatabaseIntegrities.Find(x=>x.WarningIntegrityType==EnumWarningIntegrityType.DefaultPlugin);
			return databaseIntegrityDefault;//shouldn't be null unless something is wrong with db
		}

		///<summary>Can return null. Will return for a specific class type. If none, then Default. Can return null if couldn't connect to HQ or load preference.</summary>
		public static DatabaseIntegrity GetOneClass(EnumWarningIntegrityType warningIntegrityType){
			if(_listDatabaseIntegrities is null){
				RefreshFromHQ();
			}
			if(_listDatabaseIntegrities.Count==0){
				RefreshCacheFromPref();
			}
			if(_listDatabaseIntegrities.Count==0){
				//must have had trouble connecting to HQ,
				return null;
			}
			DatabaseIntegrity databaseIntegrity=_listDatabaseIntegrities.Find(x=>x.WarningIntegrityType==warningIntegrityType);
			if(databaseIntegrity!=null){
				return databaseIntegrity;
			}
			DatabaseIntegrity databaseIntegrityDefault=_listDatabaseIntegrities.Find(x=>x.WarningIntegrityType==EnumWarningIntegrityType.DefaultClass);
			return databaseIntegrityDefault;//shouldn't be null here unless something is wrong with db
		}

		///<summary>Attempts to get the Whitelist from HQ. Saves to preference if successful, else fails silently.</summary>
		private static void RefreshFromHQ(){
			_listDatabaseIntegrities=new List<DatabaseIntegrity>();
			#if DEBUG
				OpenDental.localhost.Service1 service1=new OpenDental.localhost.Service1();
			#else
				OpenDental.customerUpdates.Service1 service1=new OpenDental.customerUpdates.Service1();
				service1.Url=PrefC.GetString(PrefName.UpdateServerAddress);
			#endif
			string registrationKey=PrefC.GetString(PrefName.RegistrationKey);
			string result="";
			try {
				result=service1.DatabaseIntegrityGetList2(registrationKey);
			}
			catch{
				//fail silently
				return;
			}
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.LoadXml(result);
			XmlNode xmlNode=xmlDocument.SelectSingleNode("//Error");
			if(xmlNode!=null) {
				//MessageBox.Show(xmlNode.InnerText,"Error");
				//fail silently
				return;
			}
			xmlNode=xmlDocument.SelectSingleNode("//ResultTable");
			string innerXml=xmlNode.InnerXml.ToString();
			FillCache(innerXml);
			if(_listDatabaseIntegrities.Count==0) {
				return;
			}
			string whiteListObfuscated;
			whiteListObfuscated=CDT.Class1.TryEncrypt(innerXml);
			if(whiteListObfuscated!=null) {
				bool changed=false;
				changed |= Prefs.UpdateString(PrefName.DatabaseIntegritiesWhiteList,whiteListObfuscated);
				if(changed) {
					DataValid.SetInvalid(InvalidType.Prefs);
				}
			}
		}

		///<summary>Loads the whitelisted DatabaseIntegrity objects into cache. </summary>
		private static void FillCache(string xmlString){
			ODDataTable table=new ODDataTable(xmlString);
			for(int i=0;i<table.Rows.Count;i++) {
				DatabaseIntegrity databaseIntegrity=new DatabaseIntegrity();
				databaseIntegrity.DatabaseIntegrityNum=PIn.Long(table.Rows[i]["DatabaseIntegrityNum"]);
				string warningIntegrityType=table.Rows[i]["WarningIntegrityType"].ToString();
				try{
					databaseIntegrity.WarningIntegrityType=(EnumWarningIntegrityType)Enum.Parse(typeof(EnumWarningIntegrityType),warningIntegrityType);
				}
				catch{ 
					//Something is very wrong so we will exclude this row. 
					continue;
				}
				databaseIntegrity.PluginName=table.Rows[i]["PluginName"];
				try{
					databaseIntegrity.Behavior=(EnumIntegrityBehavior)Enum.Parse(typeof(EnumIntegrityBehavior),table.Rows[i]["Behavior"].ToString());
				}
				catch{ 
					//Something is very wrong so we will exclude this row.
					continue;
				}
				databaseIntegrity.Message=table.Rows[i]["Message"];
				databaseIntegrity.Note=table.Rows[i]["Note"];
				_listDatabaseIntegrities.Add(databaseIntegrity);
			}
		}

		///<summary>Attempts to load databaseIntegrities whitelist from preference into cache.</summary>
		private static void RefreshCacheFromPref() {
			string whiteListObfuscated=Prefs.GetOne(PrefName.DatabaseIntegritiesWhiteList).ValueString;
			string whiteListPlainText="";
			if(whiteListObfuscated!="") {
				whiteListPlainText=CDT.Class1.TryDecrypt(whiteListObfuscated);
				FillCache(whiteListPlainText);
			}
		}

	}
}
