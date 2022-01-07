using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class ToothInitials {
		///<summary>Gets all toothinitial entries for the current patient.</summary>
		public static List<ToothInitial> Refresh(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ToothInitial>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM toothinitial"
				+" WHERE PatNum = "+POut.Long(patNum);
			return Crud.ToothInitialCrud.SelectMany(command);
		}
	
		///<summary></summary>
		public static long Insert(ToothInitial init) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				init.ToothInitialNum=Meth.GetLong(MethodBase.GetCurrentMethod(),init);
				return init.ToothInitialNum;
			}
			return Crud.ToothInitialCrud.Insert(init);
		}

		///<summary></summary>
		public static void Update(ToothInitial init) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),init);
				return;
			}
			Crud.ToothInitialCrud.Update(init);
		}

		///<summary></summary>
		public static void Delete(ToothInitial init) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),init);
				return;
			}
			string command= "DELETE FROM toothinitial WHERE ToothInitialNum="+POut.Long(init.ToothInitialNum);
			Db.NonQ(command);
		}
		
		///<summary>Sets teeth missing, or sets primary, or sets movement values.  It first clears the value from the database, then adds a new row to represent that value.  Movements require an amount.  If movement amt is 0, then no row gets added.</summary>
		public static void SetValue(long patNum,string tooth_id,ToothInitialType initialType) {
			//No need to check RemotingRole; no call to db.
			SetValue(patNum,tooth_id,initialType,0);
		}

		///<summary>Sets teeth missing, or sets primary, or sets movement values.  It first clears the value from the database, then adds a new row to represent that value.  Movements require an amount.  If movement amt is 0, then no row gets added.</summary>
		public static void SetValue(long patNum,string tooth_id,ToothInitialType initialType,float moveAmt) {
			//No need to check RemotingRole; no call to db.
			ClearValue(patNum,tooth_id,initialType);
			SetValueQuick(patNum,tooth_id,initialType,moveAmt);
		}

		///<summary>Same as SetValue, but does not clear any values first.  Only use this if you have first run ClearAllValuesForType.</summary>
		public static void SetValueQuick(long patNum,string tooth_id,ToothInitialType initialType,float moveAmt) {
			//No need to check RemotingRole; no call to db.
			//if initialType is a movement and the movement amt is 0, then don't add a row, just return;
			if(moveAmt==0
				&& ListTools.In(initialType,ToothInitialType.ShiftM,ToothInitialType.ShiftO,ToothInitialType.ShiftB,ToothInitialType.Rotate,ToothInitialType.TipM,
					ToothInitialType.TipB))
			{
				return;
			}
			ToothInitial ti=new ToothInitial();
			ti.PatNum=patNum;
			ti.ToothNum=tooth_id;
			ti.InitialType=initialType;
			ti.Movement=moveAmt;
			ToothInitials.Insert(ti);
		}

		///<summary>Only used for incremental tooth movements.  Automatically adds a movement to any existing movement.  Supply a list of all toothInitials for the patient.</summary>
		public static void AddMovement(List<ToothInitial> initialList,long patNum,string tooth_id,ToothInitialType initialType,float moveAmt) {
			//No need to check RemotingRole; no call to db.
			if(moveAmt==0) {
				return;
			}
			ToothInitial ti=initialList.Find(x => x.ToothNum==tooth_id && x.InitialType==initialType)?.Copy();
			if(ti==null){
				ti=new ToothInitial();
				ti.PatNum=patNum;
				ti.ToothNum=tooth_id;
				ti.InitialType=initialType;
				ti.Movement=moveAmt;
				ToothInitials.Insert(ti);
				return;
			}
			ti.Movement+=moveAmt;
			if(ti.Movement==0) {
				ClearValue(patNum,tooth_id,initialType);
			}
			else {
				ToothInitials.Update(ti);
			}
		}

		///<summary>Sets teeth not missing, or sets to perm, or clears movement values.</summary>
		public static void ClearValue(long patNum,string tooth_id,ToothInitialType initialType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,tooth_id,initialType);
				return;
			}
			string command="DELETE FROM toothinitial WHERE PatNum="+POut.Long(patNum)
				+" AND ToothNum='"+POut.String(tooth_id)
				+"' AND InitialType="+POut.Long((int)initialType);
			Db.NonQ(command);
		}

		///<summary>Sets teeth not missing, or sets to perm, or clears movement values.  Clears all the values of one type for all teeth in the mouth.</summary>
		public static void ClearAllValuesForType(long patNum,ToothInitialType initialType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,initialType);
				return;
			}
			string command="DELETE FROM toothinitial WHERE PatNum="+POut.Long(patNum)
				+" AND InitialType="+POut.Long((int)initialType);
			Db.NonQ(command);
		}

		///<summary>Gets a list of missing teeth as strings. Includes "1"-"32", and "A"-"Z".</summary>
		public static List<string> GetMissingOrHiddenTeeth(List<ToothInitial> initialList) {
			//No need to check RemotingRole; no call to db.
			List<string> missing=new List<string>();
			for(int i=0;i<initialList.Count;i++) {
				if((initialList[i].InitialType==ToothInitialType.Missing || initialList[i].InitialType==ToothInitialType.Hidden)
					&& Tooth.IsValidDB(initialList[i].ToothNum)
					&& !Tooth.IsSuperNum(initialList[i].ToothNum)
					&& !missing.Contains(initialList[i].ToothNum))
				{
					missing.Add(initialList[i].ToothNum);
				}
			}
			return missing;
		}

		///<summary>Gets a list of primary teeth as strings. Includes "1"-"32".</summary>
		public static List<string> GetPriTeeth(List<ToothInitial> initialList) {
			//No need to check RemotingRole; no call to db.
			ArrayList pri=new ArrayList();
			for(int i=0;i<initialList.Count;i++) {
				if(initialList[i].InitialType==ToothInitialType.Primary
					&& Tooth.IsValidDB(initialList[i].ToothNum)
					&& !Tooth.IsPrimary(initialList[i].ToothNum)
					&& !Tooth.IsSuperNum(initialList[i].ToothNum))
				{
					pri.Add(initialList[i].ToothNum);
				}
			}
			return pri.AsEnumerable<string>().ToList();
		}

		///<summary>Loops through supplied initial list to see if the specified tooth is already marked as missing or hidden.
		///Tooth numbers 1-32 or A-T.  Supernumeraries not supported here yet.</summary>
		public static bool ToothIsMissingOrHidden(List<ToothInitial> initialList,string toothNum){
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<initialList.Count;i++){
				if(initialList[i].InitialType!=ToothInitialType.Missing
					&& initialList[i].InitialType!=ToothInitialType.Hidden)
				{
					continue;
				}
				if(initialList[i].ToothNum!=toothNum){
					continue;
				}
				return true;
			}
			return false;
		}

		///<summary>Gets the current movement value for a single tooth by looping through the supplied list.</summary>
		public static float GetMovement(List<ToothInitial> initialList,string toothNum,ToothInitialType initialType){
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<initialList.Count;i++) {
				if(initialList[i].InitialType==initialType
					&& initialList[i].ToothNum==toothNum)
				{
					return initialList[i].Movement;
				}
			}
			return 0;
		}

		///<summary>Gets a list of the hidden teeth as strings. Includes "1"-"32", and "A"-"Z".</summary>
		public static ArrayList GetHiddenTeeth(List<ToothInitial> initialList) {
			//No need to check RemotingRole; no call to db.
			ArrayList hidden=new ArrayList();
			if(initialList.IsNullOrEmpty()) {
				return hidden;
			}
			for(int i=0;i<initialList.Count;i++) {
				if(initialList[i].InitialType==ToothInitialType.Hidden
					&& Tooth.IsValidDB(initialList[i].ToothNum)
					&& !Tooth.IsSuperNum(initialList[i].ToothNum))
				{
					hidden.Add(initialList[i].ToothNum);
				}
			}
			return hidden;
		}
	}

	




}

















