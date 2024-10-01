using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoProcLinks{
		#region Get Methods
		///<summary>Gets all orthoproclinks from DB.</summary>
		public static List<OrthoProcLink> GetAll() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT orthoproclink.* FROM orthoproclink";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Get a list of all OrthoProcLinks for an OrthoCase.</summary>
		public static List<OrthoProcLink> GetManyByOrthoCase(long orthoCaseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),orthoCaseNum);
			}
			string command="SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum = "+POut.Long(orthoCaseNum);
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Gets one OrthoProcLink of the specified OrthoProcType for an OrthoCase. This should only be used to get procedures of the 
		///Banding or Debond types as only one of each can be linked to an Orthocase.</summary>
		public static OrthoProcLink GetByType(long orthoCaseNum,OrthoProcType linkType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<OrthoProcLink>(MethodBase.GetCurrentMethod(),orthoCaseNum,linkType);
			}
			string command=$@"SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum={POut.Long(orthoCaseNum)}
				AND orthoproclink.ProcLinkType={POut.Int((int)linkType)}";
			return Crud.OrthoProcLinkCrud.SelectOne(command);
		}

		public static List<OrthoProcLink> GetPatientData(List<OrthoCase> listOrthoCases) {
			Meth.NoCheckMiddleTierRole();
			return GetManyByOrthoCases(listOrthoCases.Select(x => x.OrthoCaseNum).ToList());
		}

		///<summary>Returns a list of OrthoProcLinks from db of the specified type that are associated to any OrthoCaseNum from the list passed in.</summary>
		public static List<OrthoProcLink> GetManyByOrthoCases(List<long> listOrthoCaseNums) {
			if(listOrthoCaseNums.Count<=0) {
				return new List<OrthoProcLink>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),listOrthoCaseNums);
			}
			string command=$"SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum IN({string.Join(",",listOrthoCaseNums)})";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Gets all OrthoProcLinks associated to any procedures in the list passed in.</summary>
		public static List<OrthoProcLink> GetManyForProcs(List<long> listProcNums) {
			if(listProcNums.Count<=0) {
				return new List<OrthoProcLink>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),listProcNums);
			}
			string command=$@"SELECT * FROM orthoproclink
				WHERE orthoproclink.ProcNum IN({string.Join(",",listProcNums)})";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}

		///<summary>Returns a single OrthoProcLink for the procNum. There should only be one in db per procedure.</summary>
		public static OrthoProcLink GetByProcNum(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<OrthoProcLink>(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT * FROM orthoproclink WHERE ProcNum="+POut.Long(procNum);
			return Crud.OrthoProcLinkCrud.SelectOne(command);
		}

		///<summary>Returns a list of all ProcLinks of the visit type associated to an OrthoCase.</summary>
		public static List<OrthoProcLink> GetVisitLinksForOrthoCase(long orthoCaseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<OrthoProcLink>>(MethodBase.GetCurrentMethod(),orthoCaseNum);
			}
			string command=$@"SELECT * FROM orthoproclink WHERE orthoproclink.OrthoCaseNum={POut.Long(orthoCaseNum)}
			AND orthoproclink.ProcLinkType={POut.Int((int)OrthoProcType.Visit)}";
			return Crud.OrthoProcLinkCrud.SelectMany(command);
		}
		#endregion Get Methods

		#region Insert
		///<summary>Inserts an OrthoProcLink into the database. Returns the OrthoProcLinkNum.</summary>
		public static long Insert(OrthoProcLink orthoProcLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				orthoProcLink.OrthoProcLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoProcLink);
				return orthoProcLink.OrthoProcLinkNum;
			}
			return Crud.OrthoProcLinkCrud.Insert(orthoProcLink);
		}
		#endregion Insert

		#region Update
		/// <summary>Update only data that is different in newOrthoProcLink</summary>
		public static void Update(OrthoProcLink newOrthoProcLink,OrthoProcLink oldOrthoProcLink) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),newOrthoProcLink,oldOrthoProcLink);
				return;
			}
			Crud.OrthoProcLinkCrud.Update(newOrthoProcLink,oldOrthoProcLink);
		}
		#endregion Update

		#region Delete
		///<summary>Delete an OrthoProcLink from the database.</summary>
		public static void Delete(long orthoProcLinkNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoProcLinkNum);
				return;
			}
			Crud.OrthoProcLinkCrud.Delete(orthoProcLinkNum);
		}

		///<summary>Deletes all ProcLinks in the provided list of OrthoProcLinkNums.</summary>
		public static void DeleteMany(List<long> listOrthoProcLinkNums) {
			if(listOrthoProcLinkNums.Count<=0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listOrthoProcLinkNums);
				return;
			}
			string command=$"DELETE FROM orthoproclink WHERE OrthoProcLinkNum IN({string.Join(",",listOrthoProcLinkNums)})";
			Db.NonQ(command);
		}
		#endregion Delete

		#region Misc Methods
		///<summary>Does not insert it in the DB. Returns an OrthoProcLink of the specified type for the OrthoCaseNum and procNum passed in.</summary>
		public static OrthoProcLink CreateHelper(long orthoCaseNum,long procNum,OrthoProcType procType) {
			Meth.NoCheckMiddleTierRole();
			OrthoProcLink orthoProcLink=new OrthoProcLink();
			orthoProcLink.OrthoCaseNum=orthoCaseNum;
			orthoProcLink.ProcNum=procNum;
			orthoProcLink.ProcLinkType=procType;
			orthoProcLink.SecUserNumEntry=Security.CurUser.UserNum;
			return orthoProcLink;
		}

		///<summary>Returns true if the procNum is contained in at least one OrthoProcLink.</summary>
		public static bool IsProcLinked(long procNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),procNum);
			}
			string command="SELECT * FROM orthoproclink WHERE orthoproclink.ProcNum="+POut.Long(procNum);
			return Crud.OrthoProcLinkCrud.SelectMany(command).Count>0;
		}
		#endregion Misc Methods

		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		///<summary>Gets one OrthoProcLink from the db.</summary>
		public static OrthoProcLink GetOne(long orthoProcLinkNum){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<OrthoProcLink>(MethodBase.GetCurrentMethod(),orthoProcLinkNum);
			}
			return Crud.OrthoProcLinkCrud.SelectOne(orthoProcLinkNum);
		}
		///<summary></summary>
		public static void Update(OrthoProcLink orthoProcLink){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoProcLink);
				return;
			}
			Crud.OrthoProcLinkCrud.Update(orthoProcLink);
		}
		*/
	}
}