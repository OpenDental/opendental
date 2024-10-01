using System;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace OpenDentBusiness {
	///<summary></summary>
	public class PatFieldDefs {
		#region CachePattern

		private class PatFieldDefCache : CacheListAbs<PatFieldDef> {
			protected override List<PatFieldDef> GetCacheFromDb() {
				string command="SELECT * FROM patfielddef ORDER BY ItemOrder";
				return Crud.PatFieldDefCrud.SelectMany(command);
			}
			protected override List<PatFieldDef> TableToList(DataTable table) {
				return Crud.PatFieldDefCrud.TableToList(table);
			}
			protected override PatFieldDef Copy(PatFieldDef patFieldDef) {
				return patFieldDef.Copy();
			}
			protected override DataTable ListToTable(List<PatFieldDef> listPatFieldDefs) {
				return Crud.PatFieldDefCrud.ListToTable(listPatFieldDefs,"PatFieldDef");
			}
			protected override void FillCacheIfNeeded() {
				PatFieldDefs.GetTableFromCache(false);
			}
			protected override bool IsInListShort(PatFieldDef patFieldDef) {
				return !patFieldDef.IsHidden;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static PatFieldDefCache _patFieldDefCache=new PatFieldDefCache();

		public static int GetCount(bool isShort=false) {
			return _patFieldDefCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<PatFieldDef> match,bool isShort=false) {
			return _patFieldDefCache.GetExists(match,isShort);
		}

		public static List<PatFieldDef> GetDeepCopy(bool isShort=false) {
			return _patFieldDefCache.GetDeepCopy(isShort);
		}

		public static PatFieldDef GetFirstOrDefault(Func<PatFieldDef,bool> match,bool isShort=false) {
			return _patFieldDefCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_patFieldDefCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_patFieldDefCache.FillCacheFromTable(table);
				return table;
			}
			return _patFieldDefCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_patFieldDefCache.ClearCache();
		}
		#endregion

		///<summary></summary>
		public static void Update(PatFieldDef patFieldDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFieldDef);
				return;
			}
			Crud.PatFieldDefCrud.Update(patFieldDef);
		}

		///<summary></summary>
		public static long Insert(PatFieldDef patFieldDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				patFieldDef.PatFieldDefNum=Meth.GetLong(MethodBase.GetCurrentMethod(),patFieldDef);
				return patFieldDef.PatFieldDefNum;
			}
			return Crud.PatFieldDefCrud.Insert(patFieldDef);
		}

		///<summary>Surround with try/catch, because it will throw an exception if any patient is using this def.</summary>
		public static void Delete(PatFieldDef patFieldDef) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFieldDef);
				return;
			}
			string command="SELECT LName,FName FROM patient,patfield WHERE "
				+"patient.PatNum=patfield.PatNum "
				+"AND FieldName='"+POut.String(patFieldDef.FieldName)+"'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				string s=Lans.g("PatFieldDef","Not allowed to delete. Already in use by ")+table.Rows.Count.ToString()
					+" "+Lans.g("PatFieldDef","patients, including")+" \r\n";
				for(int i=0;i<table.Rows.Count;i++) {
					if(i>5) {
						break;
					}
					s+=table.Rows[i][0].ToString()+", "+table.Rows[i][1].ToString()+"\r\n";
				}
				throw new ApplicationException(s);
			}
			if(PatFieldDefs.GetListPatFieldTypesCareCredit().Contains(patFieldDef.FieldType)) {
				long careCreditProgNum=Programs.GetProgramNum(ProgramName.CareCredit);
				command=$@"SELECT COUNT(*) FROM programproperty 
					WHERE ProgramNum={POut.Long(careCreditProgNum)} 
					AND PropertyDesc IN ('{POut.String(ProgramProperties.PropertyDescs.CareCredit.GetForPatFieldType(patFieldDef.FieldType))}') 
					AND PropertyValue='{POut.String(patFieldDef.PatFieldDefNum.ToString())}'";
				if(Db.GetLong(command)!=0) {
					//CareCredit type has a reference to program property, don't allow
					throw new ApplicationException(Lans.g("PatFieldDef","Not allowed to delete. Already in use by CareCredit."));
				}
			}
			command="DELETE FROM patfielddef WHERE PatFieldDefNum ="+POut.Long(patFieldDef.PatFieldDefNum);
			Db.NonQ(command);
		}

		public static PatFieldDef GetPatFieldCareCredit(PatFieldType patFieldType=PatFieldType.CareCreditStatus) {
			List<PatFieldType> listPatFieldTypes=Enum.GetValues(typeof(PatFieldType)).Cast<PatFieldType>().ToList();
			List<PatFieldType> listPatFieldTypesCareCredit=GetListPatFieldTypesCareCredit();
			if(!listPatFieldTypesCareCredit.Contains(patFieldType)) {
				return null;
			}
			string propertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditPatField;
			if(patFieldType==PatFieldType.CareCreditPreApprovalAmt) {
				propertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditPatFieldPreApprovalAmt;
			}
			else if(patFieldType==PatFieldType.CareCreditAvailableCredit) {
				propertyDesc=ProgramProperties.PropertyDescs.CareCredit.CareCreditPatFieldAvailableCredit;
			}
			ProgramProperty propCC=ProgramProperties.GetPropForProgByDesc(Programs.GetProgramNum(ProgramName.CareCredit),propertyDesc);
			if(propCC==null) {
				return null;
			}
			long propertyValue=PIn.Long(propCC.PropertyValue,hasExceptions:false);
			PatFieldDef patFieldDef=GetFirstOrDefault(x => x.PatFieldDefNum==propertyValue && x.FieldType==patFieldType);
			return patFieldDef;
		}

		public static List<PatFieldType> GetListPatFieldTypesCareCredit() {
			List<PatFieldType> listPatFieldTypes=Enum.GetValues(typeof(PatFieldType)).Cast<PatFieldType>().ToList();
			List<PatFieldType> listPatFieldTypesCareCredit=listPatFieldTypes.FindAll(x=>x.ToString().ToLower().Contains("carecredit"));
			return listPatFieldTypesCareCredit;
		}

		///<summary>Returns the PatFieldDef for the specified field name. Returns null if an PatFieldDef does not exist for that field name.</summary>
		public static PatFieldDef GetFieldDefByFieldName(string fieldName) {
			Meth.NoCheckMiddleTierRole();
			return GetFirstOrDefault(x => x.FieldName==fieldName);
		}

		/// <summary>GetFieldName returns the field name identified by the field definition number passed as a parameter.</summary>
		public static string GetFieldName(long patFieldDefNum) {
			Meth.NoCheckMiddleTierRole();
			PatFieldDef patFieldDef=GetFirstOrDefault(x => x.PatFieldDefNum==patFieldDefNum,true);
			return (patFieldDef==null ? "" : patFieldDef.FieldName);
		}

		/// <summary>GetPickListByFieldName returns the pick list identified by the field name passed as a parameter.</summary>
		public static string GetPickListByFieldName(string FieldName) {
			Meth.NoCheckMiddleTierRole();
			PatFieldDef patFieldDef=GetFirstOrDefault(x => x.FieldName==FieldName,true);
			return (patFieldDef==null ? "" : patFieldDef.PickList);
		}

		///<summary>Gets one PatFieldDef from the DB. Used in the API.</summary>
		public static PatFieldDef GetPatFieldDefForApi(long patFieldDefNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PatFieldDef>(MethodBase.GetCurrentMethod(),patFieldDefNum);
			}
			string command="SELECT * FROM patfielddef"
				+" WHERE PatFieldDefNum = "+POut.Long(patFieldDefNum);
			return Crud.PatFieldDefCrud.SelectOne(patFieldDefNum);
		}

		///<summary>Gets all PatFieldDefs from the DB. Used in the API.</summary>
		public static List<PatFieldDef> GetPatFieldDefsForApi(int limit,int offset) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PatFieldDef>>(MethodBase.GetCurrentMethod(),limit,offset);
			}
			string command="SELECT * FROM patfielddef ";
			command+="ORDER BY PatFieldDefNum "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit)+"";
			return Crud.PatFieldDefCrud.SelectMany(command);
		}

		///<summary>Sync pattern, must sync entire table. Probably only to be used in the master problem list window.</summary>
		public static void Sync(List<PatFieldDef> listDefs,List<PatFieldDef> listDefsOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDefs,listDefsOld);
				return;
			}
			Crud.PatFieldDefCrud.Sync(listDefs,listDefsOld);
		}
	}
}