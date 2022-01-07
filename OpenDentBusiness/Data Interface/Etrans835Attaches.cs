using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Etrans835Attaches{
		///<summary>Get all claim attachments for every 835 in the list.  Ran as a batch for efficiency purposes.
		///Returned list is ordered by Etrans835Attach.DateTimeEntry, this is very important when identifying claims split from an ERA.</summary>
		public static List<Etrans835Attach> GetForClaimNums(params long[] listClaimNums) {
			return GetForEtransNumOrClaimNums(true,0,listClaimNums);
		}

		///<summary>Returns a list of Etrans835Attach for the given etransNum and/or listClaimNums.
		///Set isSimple to false to run a simpiler query and if attach.DateTimeTrans is not needed.
		///Returned list is ordered by Etrans835Attach.DateTimeEntry, this is very important when identifying claims split from an ERA.</summary>
		public static List<Etrans835Attach> GetForEtransNumOrClaimNums(bool isSimple,long etransNum = 0,params long[] listClaimNums) {
			return 	GetForEtransNumOrClaimNums(isSimple,new List<long>() { etransNum },listClaimNums);
		}
		
		///<summary>Returns a list of Etrans835Attach for the given list of etransNums and/or listClaimNums.
		///Set isSimple to false to run a simpiler query and if attach.DateTimeTrans is not needed.
		///Returned list is ordered by Etrans835Attach.DateTimeEntry, this is very important when identifying claims split from an ERA.</summary>
		public static List<Etrans835Attach> GetForEtransNumOrClaimNums(bool isSimple,List<long> listEtransNum=null,params long[] listClaimNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans835Attach>>(MethodBase.GetCurrentMethod(),isSimple,listEtransNum,listClaimNums);
			}
			if((listEtransNum==null || listEtransNum.Count==0) && (listClaimNums==null || listClaimNums.Length==0)) {
				return new List<Etrans835Attach>();//Both are either not defined or contain no information, there would be no WHERE clause.
			}
			List<string> listWhereClauses=new List<string>();
			if(listClaimNums.Length!=0) {
				listWhereClauses.Add("etrans835attach.ClaimNum IN ("+String.Join(",",listClaimNums.Select(x => POut.Long(x)))+")");
			}
			if(!isSimple && listEtransNum!=null && listEtransNum.Count>0) {//Includes manually detached and split attaches created when spliting procs from ERA.
				listWhereClauses.Add("etrans.EtransNum IN ("+string.Join(",",listEtransNum.Select(x => POut.Long(x)))+")");
			}
			if(listWhereClauses.Count==0) {
				return new List<Etrans835Attach>();
			}
			string command="SELECT etrans835attach.* "
				+(isSimple?"":",etrans.DateTimeTrans ")
				+"FROM etrans835attach "
				+(isSimple?"":"INNER JOIN etrans ON etrans.EtransNum=etrans835attach.EtransNum ")
				+"WHERE "+string.Join(" OR ",listWhereClauses)+" "
				+"ORDER BY etrans835attach.DateTimeEntry";//Attaches created from splitting an ERA need to be after the original claim attach.
			DataTable table=Db.GetTable(command);
			if(isSimple) {
				return Crud.Etrans835AttachCrud.TableToList(table);
			}
			List<Etrans835Attach> listAttaches=Crud.Etrans835AttachCrud.TableToList(table);
			for(int i=0;i<listAttaches.Count;i++) { 
					Etrans835Attach attach=listAttaches[i];
					DataRow row=table.Rows[i];
					attach.DateTimeTrans=PIn.DateT(row["DateTimeTrans"].ToString());
			}
			return listAttaches;
		}

		///<summary>Get all claim attachments for every 835 in the list.  Ran as a batch for efficiency purposes.
		///Returned list is ordered by Etrans835Attach.DateTimeEntry, this is very important when identifying claims split from an ERA.</summary></summary>
		public static List<Etrans835Attach> GetForEtrans(params long[] listEtrans835Nums) {
			return GetForEtrans(true,listEtrans835Nums);
		}

		///<summary>Returns a list of Etrans835Attachs for given etransNums.
		///Set isSimple to false to run a simpiler query and if attach.DateTimeTrans is not needed.
		///Returned list is ordered by Etrans835Attach.DateTimeEntry, this is very important when identifying claims split from an ERA.</summary>
		public static List<Etrans835Attach> GetForEtrans(bool isSimple,params long[] listEtrans835Nums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans835Attach>>(MethodBase.GetCurrentMethod(),isSimple,listEtrans835Nums);
			}
			if(listEtrans835Nums.Length==0) {
				return new List<Etrans835Attach>();
			}
			string command="SELECT etrans835attach.* "
				+(isSimple?"":",etrans.DateTimeTrans,claim.ClinicNum,insplan.CarrierNum ")
				+"FROM etrans835attach "
				+(isSimple?"":"INNER JOIN etrans ON etrans.EtransNum=etrans835attach.EtransNum ")
				+(isSimple?"":"INNER JOIN claim ON claim.ClaimNum=etrans835attach.ClaimNum ")
				+(isSimple?"":"INNER JOIN insplan ON insplan.PlanNum=claim.PlanNum ")
				+"WHERE etrans835attach.EtransNum IN ("+String.Join(",",listEtrans835Nums.Select(x => POut.Long(x)))+") "
				+"ORDER BY etrans835attach.DateTimeEntry";//Attaches created from splitting an ERA need to be after the original claim attach.
			DataTable table=Db.GetTable(command);
			if(isSimple) {
				return Crud.Etrans835AttachCrud.TableToList(table);
			}
			List<Etrans835Attach> listAttaches=Crud.Etrans835AttachCrud.TableToList(table);
			for(int i=0;i<listAttaches.Count;i++) { 
				Etrans835Attach attach=listAttaches[i];
				DataRow row=table.Rows[i];
				attach.DateTimeTrans=PIn.DateT(row["DateTimeTrans"].ToString());
				attach.ClinicNum=PIn.Long(row["ClinicNum"].ToString());
				attach.CarrierNum=PIn.Long(row["CarrierNum"].ToString());
			}
			return listAttaches;
		}

		///<summary>Create a single attachment for a claim to an 835.</summary>
		public static long Insert(Etrans835Attach etrans835Attach){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				etrans835Attach.Etrans835AttachNum=Meth.GetLong(MethodBase.GetCurrentMethod(),etrans835Attach);
				return etrans835Attach.Etrans835AttachNum;
			}
			return Crud.Etrans835AttachCrud.Insert(etrans835Attach);
		}

		///<summary>Delete the attachment for the claim currently attached to the 835 with the specified segment index.
		///Safe to run even if no claim is currently attached at the specified index.
		///Set clpSegmentIndex equal to a negative number if not </summary>
		public static void DeleteMany(int clpSegmentIndex,params long[] arrayEtranNums) {
			if(arrayEtranNums.Length==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clpSegmentIndex,arrayEtranNums);
				return;
			}
			string command="DELETE FROM etrans835attach "
				+"WHERE EtransNum IN ("+string.Join(",",arrayEtranNums.Select(x => POut.Long(x)))+")";
			if(clpSegmentIndex >= 0) {
				command+=" AND ClpSegmentIndex="+POut.Int(clpSegmentIndex);
			}
			Db.NonQ(command);
		}

		///<summary>Deletes all attachments associated to the given listEtrans835AttachNums.  Can handle null.</summary>
		public static void DeleteMany(List<long> listEtrans835AttachNums) {
			if(listEtrans835AttachNums==null || listEtrans835AttachNums.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listEtrans835AttachNums);
				return;
			}
			Db.NonQ("DELETE FROM etrans835attach WHERE Etrans835AttachNum IN ("+string.Join(",",listEtrans835AttachNums.Select(x => POut.Long(x)))+")");
		}

		public static void DetachEraClaim(Hx835_Claim claimPaid) {
			Etrans835Attaches.DeleteMany(claimPaid.ClpSegmentIndex,claimPaid.Era.EtransSource.EtransNum);
			Etrans835Attach attach=new Etrans835Attach();
			attach.EtransNum=claimPaid.Era.EtransSource.EtransNum;
			attach.ClaimNum=0;
			attach.ClpSegmentIndex=claimPaid.ClpSegmentIndex;
			Etrans835Attaches.Insert(attach);
			claimPaid.IsAttachedToClaim=true;
			claimPaid.ClaimNum=0;
		}

		///<summary>Inserts new Etrans835Attach for given claimPaid and claim.
		///Deletes any existing Etrans835Attach prior to inserting new one.
		///Sets claimPaid.ClaimNum and claimPaid.IsAttachedToClaim.
		///Removes deleted attaches from list and adds a new one if it is created when canModifyList is true.</summary>
		public static void CreateForClaim(X835 x835,Hx835_Claim claimPaid,
			long claimNum,bool isNewAttachNeeded,List<Etrans835Attach> listAttaches,bool canModifyList=false) {
			if(!isNewAttachNeeded
				&& listAttaches.Exists(
				x => x.ClaimNum==claimNum
				&& x.EtransNum==x835.EtransSource.EtransNum
				&& x.ClpSegmentIndex==claimPaid.ClpSegmentIndex)
				) {
				//Not forcing a new attach and one already exists.
				return;
			}
			//Create a hard link between the selected claim and the claim info on the 835.
			Etrans835Attaches.DeleteMany(claimPaid.ClpSegmentIndex,x835.EtransSource.EtransNum);//Detach existing if any.
			//Remove deleted attaches from list.
			if(canModifyList) {
				listAttaches.RemoveAll(x => x.EtransNum==x835.EtransSource.EtransNum && x.ClpSegmentIndex==claimPaid.ClpSegmentIndex);
			}
			Etrans835Attach attach=new Etrans835Attach();
			attach.EtransNum=x835.EtransSource.EtransNum;
			attach.ClaimNum=claimNum;
			attach.ClpSegmentIndex=claimPaid.ClpSegmentIndex;
			Etrans835Attaches.Insert(attach);
			claimPaid.ClaimNum=claimNum;
			claimPaid.IsAttachedToClaim=true;
			if(canModifyList) {
				listAttaches.Add(attach);
			}
		}


		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<Etrans835Attach> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans835Attach>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM etrans835attach WHERE PatNum = "+POut.Long(patNum);
			return Crud.Etrans835AttachCrud.SelectMany(command);
		}
		
		///<summary>Gets one Etrans835Attach from the db.</summary>
		public static Etrans835Attach GetOne(long etrans835AttachNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Etrans835Attach>(MethodBase.GetCurrentMethod(),etrans835AttachNum);
			}
			return Crud.Etrans835AttachCrud.SelectOne(etrans835AttachNum);
		}
		#endregion
		#region Modification Methods
			#region Update
			///<summary></summary>
		public static void Update(Etrans835Attach etrans835Attach){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etrans835Attach);
				return;
			}
			Crud.Etrans835AttachCrud.Update(etrans835Attach);
		}
			#endregion
			#region Delete
		///<summary></summary>
		public static void Delete(long etrans835AttachNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etrans835AttachNum);
				return;
			}
			Crud.Etrans835AttachCrud.Delete(etrans835AttachNum);
		}
			#endregion
		#endregion
		#region Misc Methods
		

		
		#endregion
		*/
	}
}