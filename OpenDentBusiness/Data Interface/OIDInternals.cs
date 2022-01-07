using CodeBase;
using DataConnectionBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Xml;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OIDInternals{
		public static string OpenDentalOID="2.16.840.1.113883.3.4337";
		private static long _customerPatNum=0;

		///<summary>The PatNum at Open Dental HQ associated to this database's registration key.
		///Makes a web call to WebServiceCustomerUpdates in order to get the PatNum from HQ.
		///Throws exceptions to show to the user if anything goes wrong in communicating with the web service.  Exceptions are already translated.</summary>
		public static long CustomerPatNum {
			get {
				if(_customerPatNum==0) {
					//prepare the xml document to send--------------------------------------------------------------------------------------
					XmlWriterSettings settings=new XmlWriterSettings();
					settings.Indent=true;
					settings.IndentChars=("    ");
					StringBuilder strbuild=new StringBuilder();
					using(XmlWriter writer = XmlWriter.Create(strbuild,settings)) {
						writer.WriteStartElement("CustomerIdRequest");
						writer.WriteStartElement("RegistrationKey");
						writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
						writer.WriteEndElement();
						writer.WriteStartElement("RegKeyDisabledOverride");
						writer.WriteString("true");
						writer.WriteEndElement();
						writer.WriteEndElement();
					}
#if DEBUG
					OpenDentBusiness.localhost.Service1 OIDService=new OpenDentBusiness.localhost.Service1();
#else
					OpenDentBusiness.customerUpdates.Service1 OIDService=new OpenDentBusiness.customerUpdates.Service1();
					OIDService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
					//Send the message and get the result---------------------------------------------------------------------------------------
					string result="";
					try {
						result=OIDService.RequestCustomerID(strbuild.ToString());
					}
					catch(Exception ex) {
						throw new Exception(Lans.g("OIDInternals","Error obtaining CustomerID:")+" "+ex.Message);
					}
					XmlDocument doc=new XmlDocument();
					doc.LoadXml(result);
					//Process errors------------------------------------------------------------------------------------------------------------
					XmlNode node=doc.SelectSingleNode("//Error");
					if(node!=null) {
						throw new Exception(Lans.g("OIDInternals","Error:")+" "+node.InnerText);
					}
					//Process a valid return value----------------------------------------------------------------------------------------------
					node=doc.SelectSingleNode("//CustomerIdResponse");
					if(node==null) {
						throw new ODException(Lans.g("OIDInternals","There was an error requesting your OID or processing the result of the request.  Please try again."));
					}
					if(node.InnerText=="") {
						throw new ODException(Lans.g("OIDInternals","Invalid registration key.  Your OIDs will have to be set manually."));
					}
					//CustomerIdResponse has been returned and is not blank
					_customerPatNum=PIn.Long(node.InnerText);
				}
				return _customerPatNum;
			}
		}

		///<summary>Returns the currently defined OID for a given IndentifierType.  If not defined, IDroot will be empty string.</summary>
		public static OIDInternal GetForType(IdentifierType IDType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OIDInternal>(MethodBase.GetCurrentMethod(),IDType);
			}
			InsertMissingValues();//
			string command="SELECT * FROM oidinternal WHERE IDType='"+IDType.ToString()+"'";//should only return one row.
			return Crud.OIDInternalCrud.SelectOne(command);
		}

		///<summary>There should always be one entry in the DB per IdentifierType enumeration.</summary>
		public static void InsertMissingValues() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			//string command= "SELECT COUNT(*) FROM oidinternal";
			//if(PIn.Long(Db.GetCount(command))==Enum.GetValues(typeof(IdentifierType)).Length) {
			//	return;//The DB table has the right count. Which means there is probably nothing wrong with the values in it. This may need to be enhanced if customers have any issues.
			//}
			string command="SELECT * FROM oidinternal";
			List<OIDInternal> listOIDInternals=Crud.OIDInternalCrud.SelectMany(command);
			List<IdentifierType> listIDTypes=new List<IdentifierType>();
			for(int i=0;i<listOIDInternals.Count;i++) {
				listIDTypes.Add(listOIDInternals[i].IDType);
			}
			for(int i=0;i<Enum.GetValues(typeof(IdentifierType)).Length;i++) {
				if(listIDTypes.Contains((IdentifierType)i)) {
					continue;//DB contains a row for this enum value.
				}
				//Insert missing row with blank OID.
				if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO oidinternal (IDType,IDRoot) "
						+"VALUES('"+((IdentifierType)i).ToString()+"','')";
						Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO oidinternal (OIDInternalNum,IDType,IDRoot) "
						+"VALUES((SELECT MAX(OIDInternalNum)+1 FROM oidinternal),'"+((IdentifierType)i).ToString()+"','')";
					Db.NonQ32(command);
				}
			}
		}

		///<summary></summary>
		public static List<OIDInternal> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OIDInternal>>(MethodBase.GetCurrentMethod());
			}
			InsertMissingValues();//there should always be one entry in the DB for each IdentifierType enumeration, insert any missing
			string command="SELECT * FROM oidinternal";
			return Crud.OIDInternalCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(OIDInternal oIDInternal) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),oIDInternal);
				return;
			}
			Crud.OIDInternalCrud.Update(oIDInternal);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<OIDInternal> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OIDInternal>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM oidinternal WHERE PatNum = "+POut.Long(patNum);
			return Crud.OIDInternalCrud.SelectMany(command);
		}

		///<summary>Gets one OIDInternal from the db.</summary>
		public static OIDInternal GetOne(long ehrOIDNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<OIDInternal>(MethodBase.GetCurrentMethod(),ehrOIDNum);
			}
			return Crud.OIDInternalCrud.SelectOne(ehrOIDNum);
		}

		///<summary></summary>
		public static long Insert(OIDInternal oIDInternal){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				oIDInternal.EhrOIDNum=Meth.GetLong(MethodBase.GetCurrentMethod(),oIDInternal);
				return oIDInternal.EhrOIDNum;
			}
			return Crud.OIDInternalCrud.Insert(oIDInternal);
		}

		///<summary></summary>
		public static void Delete(long ehrOIDNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ehrOIDNum);
				return;
			}
			string command= "DELETE FROM oidinternal WHERE EhrOIDNum = "+POut.Long(ehrOIDNum);
			Db.NonQ(command);
		}
		*/
	}
}