using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness {
	///<summary></summary>
	public class SigButDefs {
		#region CachePattern

		private class SigButDefCache : CacheListAbs<SigButDef> {
			protected override List<SigButDef> GetCacheFromDb() {
				string command="SELECT * FROM sigbutdef ORDER BY ButtonIndex";
				return Crud.SigButDefCrud.SelectMany(command);
			}
			protected override List<SigButDef> TableToList(DataTable table) {
				return Crud.SigButDefCrud.TableToList(table);
			}
			protected override SigButDef Copy(SigButDef sigButDef) {
				return sigButDef.Copy();
			}
			protected override DataTable ListToTable(List<SigButDef> listSigButDefs) {
				return Crud.SigButDefCrud.ListToTable(listSigButDefs,"SigButDef");
			}
			protected override void FillCacheIfNeeded() {
				SigButDefs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static SigButDefCache _sigButDefCache=new SigButDefCache();

		public static List<SigButDef> GetDeepCopy(bool isShort=false) {
			return _sigButDefCache.GetDeepCopy(isShort);
		}

		public static List<SigButDef> GetWhere(Predicate<SigButDef> match,bool isShort=false) {
			return _sigButDefCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_sigButDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_sigButDefCache.FillCacheFromTable(table);
				return table;
			}
			return _sigButDefCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary></summary>
		public static void Update(SigButDef def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			Crud.SigButDefCrud.Update(def);
		}

		///<summary></summary>
		public static long Insert(SigButDef def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				def.SigButDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),def);
				return def.SigButDefNum;
			}
			return Crud.SigButDefCrud.Insert(def);
		}

		///<summary>No need to surround with try/catch, because all deletions are allowed.</summary>
		public static void Delete(SigButDef def) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),def);
				return;
			}
			string command="DELETE FROM sigbutdef WHERE SigButDefNum ="+POut.Long(def.SigButDefNum);
			Db.NonQ(command);
		}

		///<summary>Loops through the SigButDefs passed in and updates the database if any of the ButtonIndexes changed.  Returns true if any changes were made to the database so that the calling class can invalidate the cache.</summary>
		public static bool UpdateButtonIndexIfChanged(SigButDef[] sigButDefs) {
			bool hasChanges=false;
			List<SigButDef> listSigButDefs=GetDeepCopy(false);
			for(int i=0;i<listSigButDefs.Count;i++) {
				for(int j=0;j<sigButDefs.Length;j++) {
					if(listSigButDefs[i].SigButDefNum!=sigButDefs[j].SigButDefNum) {
						continue;
					}
					//This is the same SigButDef
					if(listSigButDefs[i].ButtonIndex!=sigButDefs[j].ButtonIndex) {
						hasChanges=true;
						Update(sigButDefs[j]);//Update the database with the new button index.
					}
				}
			}
			return hasChanges;
		}

		///<summary>Used in Setup.  The returned list also includes defaults.  The supplied computer name can be blank for the default setup.</summary>
		public static SigButDef[] GetByComputer(string computerName) {
			//No need to check MiddleTierRole; no call to db.
			List<SigButDef> listSigButDefs=GetWhere(x => x.ComputerName=="" || x.ComputerName.ToUpper()==computerName.ToUpper());
			listSigButDefs.Sort(CompareButtonsByIndex);
			return listSigButDefs.ToArray();
		}

		private static int CompareButtonsByIndex(SigButDef x,SigButDef y) {
			//No need to check MiddleTierRole; no call to db.
			if(x.ButtonIndex!=y.ButtonIndex) {
				return x.ButtonIndex.CompareTo(y.ButtonIndex);
			}
			//we compair y to x here due to a nuance in the way light buttons are drawn. This makes computer specific
			//buttons drawn "on-top-of" the default buttons.
			return y.ComputerName.CompareTo(x.ComputerName);
		}

		///<summary>Moves the selected item up in the supplied sub list.  Does not update the cache because the user could want to potentially move buttons around a lot.</summary>
		public static SigButDef[] MoveUp(SigButDef selected,SigButDef[] subList) {
			//No need to check MiddleTierRole; no call to db.
			if(selected.ButtonIndex==0) {//already at top
				return subList;
			}
			SigButDef occupied=null;
			int occupiedIdx=-1;
			int selectedIdx=-1;
			for(int i=0;i<subList.Length;i++) {
				if(subList[i].SigButDefNum!=selected.SigButDefNum//if not the selected object
					&& subList[i].ButtonIndex==selected.ButtonIndex-1
					&& (subList[i].ComputerName!="" || selected.ComputerName==""))
				{
					//We want to swap positions with the selected button, which happens if we are moving a default button or moving to a non-default button.
					occupied=subList[i].Copy();
					occupiedIdx=i;
				}
				if(subList[i].SigButDefNum==selected.SigButDefNum) {
					selectedIdx=i;
				}
			}
			if(occupied!=null) {
				subList[occupiedIdx].ButtonIndex++;
			}
			subList[selectedIdx].ButtonIndex--;
			List<SigButDef> listSigButDefs=new List<SigButDef>();
			for(int i=0;i<subList.Length;i++) {
				listSigButDefs.Add(subList[i].Copy());
			}
			listSigButDefs.Sort(CompareButtonsByIndex);
			SigButDef[] retVal=new SigButDef[listSigButDefs.Count];
			listSigButDefs.CopyTo(retVal);
			return retVal;
		}

		///<summary>Moves the selected item down in the supplied sub list.  Does not update the cache because the user could want to potentially move buttons around a lot.</summary>
		public static SigButDef[] MoveDown(SigButDef selected,SigButDef[] subList) {
			//No need to check MiddleTierRole; no call to db.
			int occupiedIdx=-1;
			int selectedIdx=-1;
			SigButDef occupied=null;
			for(int i=0;i<subList.Length;i++) {
				if(subList[i].SigButDefNum!=selected.SigButDefNum//if not the selected object
					&& subList[i].ButtonIndex==selected.ButtonIndex+1 
					&& (subList[i].ComputerName!="" || selected.ComputerName=="")) 
				{
					//We want to swap positions with the selected button, which happens if we are moving a default button or moving to a non-default button.
					occupied=subList[i].Copy();
					occupiedIdx=i;
				}
				if(subList[i].SigButDefNum==selected.SigButDefNum) {
					selectedIdx=i;
				}
			}
			if(occupied!=null) {
				subList[occupiedIdx].ButtonIndex--;
			}
			subList[selectedIdx].ButtonIndex++;
			List<SigButDef> listSigButDefs=new List<SigButDef>();
			for(int i=0;i<subList.Length;i++) {
				listSigButDefs.Add(subList[i].Copy());
			}
			listSigButDefs.Sort(CompareButtonsByIndex);
			SigButDef[] retVal=new SigButDef[listSigButDefs.Count];
			listSigButDefs.CopyTo(retVal);
			return retVal;
		}

		///<summary>Returns the SigButDef with the specified buttonIndex.  Used from the setup page.  The supplied list will already have been filtered by computername.  Supply buttonIndex in 0-based format.</summary>
		public static SigButDef GetByIndex(int buttonIndex,List<SigButDef> subList) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<subList.Count;i++) {
				if(subList[i].ButtonIndex==buttonIndex) {
					//Will always return a specific computer's button over a default if there are 2 buttons with the same index.  See CompareButtonsByIndex.
					return subList[i].Copy();
				}
			}
			return null;
		}

		///<summary>Returns the SigButDef with the specified buttonIndex.  Used from the setup page.  The supplied list will already have been filtered by computername.  Supply buttonIndex in 0-based format.</summary>
		public static SigButDef GetByIndex(int buttonIndex,SigButDef[] subList) {
			//No need to check MiddleTierRole; no call to db.
			for(int i=0;i<subList.Length;i++) {
				if(subList[i].ButtonIndex==buttonIndex) {
					//Will always return a specific computer's button over a default if there are 2 buttons with the same index.  See CompareButtonsByIndex.
					return subList[i].Copy();
				}
			}
			return null;
		}

		///<summary>A unique synchronization method designed for HQ only. Propagates the messaging buttons for the 'All' computer to computers found in the phonecomp table.</summary>
		public static void SynchTheAllComputerWithPhoneComps() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			ProgressBarEvent.Fire(ODEventType.ProgressBar,"Collecting data for synchronization process...");
			List<SigButDef> listSigButDefsOld=SigButDefs.GetDeepCopy();
			//Find all of the SigButDefs for the 'All' computer (empty ComputerName).
			List<SigButDef> listSigButDefsAllComputer=listSigButDefsOld.FindAll(x => string.IsNullOrEmpty(x.ComputerName));
			//Start a new list of SigButDefs for the synch method. Initialize the new list with a shallow copy of the 'All' computer items since they need to stay.
			List<SigButDef> listSigButDefsNew=new List<SigButDef>(listSigButDefsAllComputer);
			//Organize the current data so that the Sync invocation towards the end of this method doesn't have to do a lot of unnecessary work.
			//Make deep copies of the SigButDefs since they will potentially be manipulated later on.
			List<ComputerNameSigButDefs> listComputerNameSigButDefs=listSigButDefsOld.Where(x => !string.IsNullOrEmpty(x.ComputerName))
				.GroupBy(x => x.ComputerName)
				.ToDictionary(x => x.Key,x => x.Select(y => y.Copy()).ToList())
				.Select(x => new ComputerNameSigButDefs() { ComputerName=x.Key,ListSigButDefs=x.Value })
				.ToList();
			//Get all of the SigElementDefs of type SignalElementType.User in order to make sure there is one for every extension within the phonecomp table.
			List<SigElementDef> listSigElementDefsForUsers=SigElementDefs.GetWhere(x => x.SigElementType==SignalElementType.User);
			//Loop through all of the phonecomp rows and create new SigElementDef items for new extensions while populating the new list of SigButDefs as needed.
			List<PhoneComp> listPhoneComps=PhoneComps.GetDeepCopy();
			for(int i=0;i<listPhoneComps.Count;i++) {
				ProgressBarEvent.Fire(ODEventType.ProgressBar,$"Processing PhoneComp {(i + 1)}/{listPhoneComps.Count}");
				SigElementDef sigElementDefForExt=listSigElementDefsForUsers.FirstOrDefault(x => PIn.Int(x.SigText,hasExceptions:false)==listPhoneComps[i].PhoneExt);
				//Make sure that a SigElementDef of type SignalElementType.User exists for the current phone extension.
				if(sigElementDefForExt==null) {
					//Create a new SigElementDef of SignalElementType.User for the current phone extension.
					sigElementDefForExt=new SigElementDef() {
						SigElementType=SignalElementType.User,
						SigText=listPhoneComps[i].PhoneExt.ToString(),
						LightColor=Color.White,
						ItemOrder=listSigElementDefsForUsers.Count,
					};
					SigElementDefs.Insert(sigElementDefForExt);
					listSigElementDefsForUsers.Add(sigElementDefForExt);
				}
				//Get the list of SigButDefs for the current phonecomp entity.
				List<SigButDef> listSigButDefsForPhoneComp=new List<SigButDef>();
				ComputerNameSigButDefs computerNameSigButDefs=listComputerNameSigButDefs.FirstOrDefault(x => x.ComputerName.ToUpper()==listPhoneComps[i].ComputerName.ToUpper());
				if(computerNameSigButDefs!=null) {
					listSigButDefsForPhoneComp=computerNameSigButDefs.ListSigButDefs;
				}
				//Make sure that every single 'All' SigButDef exists for the current phonecomp entity.
				for(int j=0;j<listSigButDefsAllComputer.Count;j++) {
					//Use the ButtonText as a sort of key for knowing if the SigButDef exists already.
					SigButDef sigButDefForPhoneComp=listSigButDefsForPhoneComp.FirstOrDefault(x => x.ButtonText==listSigButDefsAllComputer[j].ButtonText);//Case sensitive on purpose.
					if(sigButDefForPhoneComp==null) {
						//There is no button for the current phonecomp entity so make a new one (with no PK set so that the Synch knows to insert it).
						sigButDefForPhoneComp=new SigButDef() {
							IsNew=true,
							ComputerName=listPhoneComps[i].ComputerName,
						};
					}
					//Always synchronize the following settings with the 'All' SigButDef.
					sigButDefForPhoneComp.ButtonText=listSigButDefsAllComputer[j].ButtonText;
					sigButDefForPhoneComp.ButtonIndex=listSigButDefsAllComputer[j].ButtonIndex;
					sigButDefForPhoneComp.SynchIcon=listSigButDefsAllComputer[j].SynchIcon;
					sigButDefForPhoneComp.SigElementDefNumExtra=listSigButDefsAllComputer[j].SigElementDefNumExtra;
					sigButDefForPhoneComp.SigElementDefNumMsg=listSigButDefsAllComputer[j].SigElementDefNumMsg;
					//Always utilize the SigElementDef for the current phonecomp entity that was found or created above.
					sigButDefForPhoneComp.SigElementDefNumUser=sigElementDefForExt.SigElementDefNum;
					//Add the SigButDef that was just synchronized with the 'All' SigButDef to the list of 'new' SigButDefs for the Sync method.
					//The Sync method is smart enough to not run any queries if nothing actually changed for this SigButDef.
					listSigButDefsNew.Add(sigButDefForPhoneComp);
				}
			}
			ProgressBarEvent.Fire(ODEventType.ProgressBar,"Synchronizing SigButDef changes to the database...");
			Crud.SigButDefCrud.Sync(listSigButDefsNew,listSigButDefsOld);
			ProgressBarEvent.Fire(ODEventType.ProgressBar,"Done");
		}

		///<summary>A helper class used to avoid using Key and Value from the Dictionary class.</summary>
		private class ComputerNameSigButDefs {
			public string ComputerName;
			public List<SigButDef> ListSigButDefs;
		}
	}













}










