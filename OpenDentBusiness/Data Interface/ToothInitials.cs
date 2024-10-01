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
		public static List<ToothInitial> GetPatientData(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ToothInitial>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM toothinitial"
				+" WHERE PatNum = "+POut.Long(patNum);
			return Crud.ToothInitialCrud.SelectMany(command);
		}
	
		///<summary></summary>
		public static long Insert(ToothInitial toothInitial) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				toothInitial.ToothInitialNum=Meth.GetLong(MethodBase.GetCurrentMethod(),toothInitial);
				return toothInitial.ToothInitialNum;
			}
			return Crud.ToothInitialCrud.Insert(toothInitial);
		}

		///<summary></summary>
		public static void Update(ToothInitial toothInitial) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),toothInitial);
				return;
			}
			Crud.ToothInitialCrud.Update(toothInitial);
		}

		///<summary></summary>
		public static void Delete(ToothInitial toothInitial) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),toothInitial);
				return;
			}
			string command= "DELETE FROM toothinitial WHERE ToothInitialNum="+POut.Long(toothInitial.ToothInitialNum);
			Db.NonQ(command);
		}
		
		///<summary>Sets teeth missing, or sets primary, or sets movement values.  It first clears the value from the database, then adds a new row to represent that value.  Movements require an amount.  If movement amt is 0, then no row gets added.</summary>
		public static void SetValue(long patNum,string toothId,ToothInitialType toothInitialType) {
			Meth.NoCheckMiddleTierRole();
			SetValue(patNum,toothId,toothInitialType,0);
		}

		///<summary>Sets teeth missing, or sets primary, or sets movement values.  It first clears the value from the database, then adds a new row to represent that value.  Movements require an amount.  If movement amt is 0, then no row gets added.</summary>
		public static void SetValue(long patNum,string toothId,ToothInitialType toothInitialType,float moveAmt) {
			Meth.NoCheckMiddleTierRole();
			ClearValue(patNum,toothId,toothInitialType);
			SetValueQuick(patNum,toothId,toothInitialType,moveAmt);
		}

		///<summary>Same as SetValue, but does not clear any values first.  Only use this if you have first run ClearAllValuesForType.</summary>
		public static void SetValueQuick(long patNum,string toothId,ToothInitialType toothInitialType,float moveAmt) {
			Meth.NoCheckMiddleTierRole();
			//if initialType is a movement and the movement amt is 0, then don't add a row, just return;
			if(moveAmt==0
				&& toothInitialType.In(ToothInitialType.ShiftM,ToothInitialType.ShiftO,ToothInitialType.ShiftB,ToothInitialType.Rotate,ToothInitialType.TipM,
					ToothInitialType.TipB))
			{
				return;
			}
			ToothInitial toothInitial=new ToothInitial();
			toothInitial.PatNum=patNum;
			toothInitial.ToothNum=toothId;
			toothInitial.InitialType=toothInitialType;
			toothInitial.Movement=moveAmt;
			ToothInitials.Insert(toothInitial);
		}

		///<summary>Only used for incremental tooth movements.  Automatically adds a movement to any existing movement.  Supply a list of all toothInitials for the patient.</summary>
		public static void AddMovement(List<ToothInitial> listToothInitials,long patNum,string toothId,ToothInitialType toothInitialType,float moveAmt) {
			Meth.NoCheckMiddleTierRole();
			if(moveAmt==0) {
				return;
			}
			ToothInitial toothInitial=listToothInitials.Find(x => x.ToothNum==toothId && x.InitialType==toothInitialType)?.Copy();
			if(toothInitial==null){
				toothInitial=new ToothInitial();
				toothInitial.PatNum=patNum;
				toothInitial.ToothNum=toothId;
				toothInitial.InitialType=toothInitialType;
				toothInitial.Movement=moveAmt;
				ToothInitials.Insert(toothInitial);
				return;
			}
			toothInitial.Movement+=moveAmt;
			if(toothInitial.Movement==0) {
				ClearValue(patNum,toothId,toothInitialType);
				return;
			}
			ToothInitials.Update(toothInitial);
		}

		///<summary>Sets teeth not missing, or sets to perm, or clears movement values.</summary>
		public static void ClearValue(long patNum,string toothId,ToothInitialType toothInitialType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,toothId,toothInitialType);
				return;
			}
			string command="DELETE FROM toothinitial WHERE PatNum="+POut.Long(patNum)
				+" AND ToothNum='"+POut.String(toothId)
				+"' AND InitialType="+POut.Long((int)toothInitialType);
			Db.NonQ(command);
		}

		///<summary>Sets teeth not missing, or sets to perm, or clears movement values.  Clears all the values of one type for all teeth in the mouth.</summary>
		public static void ClearAllValuesForType(long patNum,ToothInitialType toothInitialType) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum,toothInitialType);
				return;
			}
			string command="DELETE FROM toothinitial WHERE PatNum="+POut.Long(patNum)
				+" AND InitialType="+POut.Long((int)toothInitialType);
			Db.NonQ(command);
		}

		///<summary>Gets a ToothInitial by ToothInitialNum from the database. Returns null if not found.</summary>
		public static ToothInitial GetOne(long toothInitialNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<ToothInitial>(MethodBase.GetCurrentMethod(),toothInitialNum);
			}
			string command="SELECT * FROM toothinitial "
				+"WHERE ToothInitialNum = "+POut.Long(toothInitialNum);
			return Crud.ToothInitialCrud.SelectOne(command);
		}

		///<summary>Gets a list of ToothInitials from the database. Returns an empty list if not found.</summary>
		public static List<ToothInitial> GetToothInitialsForApi(int limit,int offset,long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<ToothInitial>>(MethodBase.GetCurrentMethod(),limit,offset,patNum);
			}
			string command="SELECT * from toothinitial";
			if(patNum>0) {
				command+=" WHERE PatNum="+POut.Long(patNum);
			}
			command+=" ORDER BY ToothInitialNum"
				+" LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.ToothInitialCrud.SelectMany(command);
		}

		///<summary>Gets a list of missing teeth as strings. Includes "1"-"32", and "A"-"Z".</summary>
		public static List<string> GetMissingOrHiddenTeeth(List<ToothInitial> listToothInitials) {
			Meth.NoCheckMiddleTierRole();
			List<string> listMissingTeeth=new List<string>();
			for(int i=0;i<listToothInitials.Count;i++) {
				if((listToothInitials[i].InitialType==ToothInitialType.Missing || listToothInitials[i].InitialType==ToothInitialType.Hidden)
					&& Tooth.IsValidDB(listToothInitials[i].ToothNum)
					&& !Tooth.IsSuperNum(listToothInitials[i].ToothNum)
					&& !listMissingTeeth.Contains(listToothInitials[i].ToothNum))
				{
					listMissingTeeth.Add(listToothInitials[i].ToothNum);
				}
			}
			return listMissingTeeth;
		}

		///<summary>Gets a list of primary teeth as strings. Includes "1"-"32".</summary>
		public static List<string> GetPriTeeth(List<ToothInitial> listToothInitials) {
			Meth.NoCheckMiddleTierRole();
			List<string> listPrimaryTeeth=new List<string>();
			for(int i=0;i<listToothInitials.Count;i++) {
				if(listToothInitials[i].InitialType==ToothInitialType.Primary
					&& Tooth.IsValidDB(listToothInitials[i].ToothNum)
					&& !Tooth.IsPrimary(listToothInitials[i].ToothNum)
					&& !Tooth.IsSuperNum(listToothInitials[i].ToothNum))
				{
					listPrimaryTeeth.Add(listToothInitials[i].ToothNum);
				}
			}
			return listPrimaryTeeth;
		}

		///<summary>Loops through supplied initial list to see if the specified tooth is already marked as missing or hidden.
		///Tooth numbers 1-32 or A-T.  Supernumeraries not supported here yet.</summary>
		public static bool ToothIsMissingOrHidden(List<ToothInitial> listToothInitials,string strToothNum){
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<listToothInitials.Count;i++){
				if(listToothInitials[i].InitialType!=ToothInitialType.Missing
					&& listToothInitials[i].InitialType!=ToothInitialType.Hidden)
				{
					continue;
				}
				if(listToothInitials[i].ToothNum!=strToothNum){
					continue;
				}
				return true;
			}
			return false;
		}

		///<summary>Gets the current movement value for a single tooth by looping through the supplied list.</summary>
		public static float GetMovement(List<ToothInitial> listToothInitals,string strToothNum,ToothInitialType toothInitialType){
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<listToothInitals.Count;i++) {
				if(listToothInitals[i].InitialType==toothInitialType
					&& listToothInitals[i].ToothNum==strToothNum)
				{
					return listToothInitals[i].Movement;
				}
			}
			return 0;
		}

		///<summary>Gets a list of the hidden teeth as strings. Includes "1"-"32", and "A"-"Z".</summary>
		public static List<string> GetHiddenTeeth(List<ToothInitial> listToothInitials) {
			Meth.NoCheckMiddleTierRole();
			List<string> listHiddenTeeth=new List<string>();
			if(listToothInitials.IsNullOrEmpty()) {
				return listHiddenTeeth;
			}
			for(int i=0;i<listToothInitials.Count;i++) {
				if(listToothInitials[i].InitialType==ToothInitialType.Hidden
					&& Tooth.IsValidDB(listToothInitials[i].ToothNum)
					&& !Tooth.IsSuperNum(listToothInitials[i].ToothNum))
				{
					listHiddenTeeth.Add(listToothInitials[i].ToothNum);
				}
			}
			return listHiddenTeeth;
		}
	}

	




}

















