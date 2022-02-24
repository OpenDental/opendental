using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OIDExternals {
		#region Get Methods

		///<summary>Will return an OIDExternal if both the root and extension match exactly, returns null if not found.</summary>
		/// <param name="root">The OID of the object.</param>
		/// <param name="extension">If object is identified by only the root, this value should be an empty string.</param>
		public static OIDExternal GetByRootAndExtension(string root,string extension) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OIDExternal>(MethodBase.GetCurrentMethod(),root,extension);
			}
			string command="SELECT * FROM oidexternal WHERE rootExternal='"+POut.String(root)+"' AND IDExternal='"+POut.String(extension)+"'";
			return Crud.OIDExternalCrud.SelectOne(command);
		}

		///<summary>Gets a list of all external ID's for the internal ID and type provided.  Used to construct outbound HL7 messages.</summary>
		public static List<OIDExternal> GetByInternalIDAndType(long idInternal,IdentifierType idType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OIDExternal>>(MethodBase.GetCurrentMethod(),idInternal,idType);
			}
			string command="SELECT * FROM oidexternal WHERE IDType='"+idType.ToString()+"' AND IDInternal="+POut.Long(idInternal);
			return Crud.OIDExternalCrud.SelectMany(command);
		}

		///<summary>Gets the OIDExternal of the corresponding external root.
		///This is useful when Open Dental is registering OIDs for other organizations where we don't want to include the office's PatNum at HQ.
		///Returns null if a root was not found.</summary>
		public static OIDExternal GetByPartialRootExternal(string rootExternalPartial) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OIDExternal>(MethodBase.GetCurrentMethod(),rootExternalPartial);
			}
			string command="SELECT * FROM oidexternal WHERE rootExternal LIKE '"+POut.String(rootExternalPartial)+"%'"
				+" AND IDType='"+IdentifierType.Root.ToString()+"'";
			return Crud.OIDExternalCrud.SelectOne(command);
		}

		///<summary>Gets the OidExternal for the given root/internal id/id type.  Should be unique.  Returns null if no match found.</summary>
		public static OIDExternal GetOidExternal(string root,long idInternal,IdentifierType idType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<OIDExternal>(MethodBase.GetCurrentMethod(),root,idInternal,idType);
			}
			string command="SELECT * FROM oidexternal WHERE rootExternal='"+POut.String(root)+"'"
				+" AND IDType='"+idType.ToString()+"' AND IDInternal="+POut.Long(idInternal);
			return Crud.OIDExternalCrud.SelectOne(command);
		}

		#endregion

		///<summary></summary>
		public static long Insert(OIDExternal oIDExternal) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				oIDExternal.OIDExternalNum=Meth.GetLong(MethodBase.GetCurrentMethod(),oIDExternal);
				return oIDExternal.OIDExternalNum;
			}
			return Crud.OIDExternalCrud.Insert(oIDExternal);
		}

		///<summary>Under most circumstances this should not be used.</summary>
		public static void Update(OIDExternal oIDExternal) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),oIDExternal);
				return;
			}
			Crud.OIDExternalCrud.Update(oIDExternal);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<OIDExternal> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OIDExternal>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM oidexternal WHERE PatNum = "+POut.Long(patNum);
			return Crud.OIDExternalCrud.SelectMany(command);
		}

		///<summary>Gets one OIDExternal from the db.</summary>
		public static OIDExternal GetOne(long oIDExternalNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<OIDExternal>(MethodBase.GetCurrentMethod(),oIDExternalNum);
			}
			return Crud.OIDExternalCrud.SelectOne(oIDExternalNum);
		}

		///<summary></summary>
		public static void Delete(long oIDExternalNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),oIDExternalNum);
				return;
			}
			string command= "DELETE FROM oidexternal WHERE OIDExternalNum = "+POut.Long(oIDExternalNum);
			Db.NonQ(command);
		}
		*/
	}
}