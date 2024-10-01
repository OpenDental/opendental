using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary></summary>
	public class PatFields {
		#region Misc Methods
		public static void Upsert(PatField patField) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patField);
				return;
			}
			if(patField.PatFieldNum > 0) {
				Update(patField);
			}
			else {
				Insert(patField);
			}
		}
		#endregion

		///<summary>Gets a list of all PatFields for a given patient.</summary>
		public static PatField[] Refresh(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PatField[]>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM patfield WHERE PatNum="+POut.Long(patNum);
			return Crud.PatFieldCrud.SelectMany(command).ToArray();
		}

		///<summary></summary>
		public static List<PatField> GetPatientData(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PatField>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM patfield WHERE PatNum="+POut.Long(patNum);
			return Crud.PatFieldCrud.SelectMany(command);
		}

		///<summary>Gets all PatFields from the database. Used for API.</summary>
		public static List<PatField> GetPatFieldsForApi(int limit,int offset,long patNum,string fieldName,DateTime dateSecDateTEdit) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PatField>>(MethodBase.GetCurrentMethod(),limit,offset,patNum,fieldName,dateSecDateTEdit);
			}
			string command="SELECT * FROM patfield WHERE SecDateTEdit >= "+POut.DateT(dateSecDateTEdit)+" ";
			if(patNum>0) {
				command+="AND PatNum="+POut.Long(patNum)+" ";
			}
			if(fieldName!="") {
				command+="AND FieldName='"+POut.String(fieldName)+"' ";
			}
			command+="ORDER BY PatFieldNum "//Ensure order for limit and offset.
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.PatFieldCrud.SelectMany(command);
		}

		///<summary>Gets one PatField from the database. Used for API.</summary>
		public static PatField GetPatFieldForApi(long patFieldNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PatField>(MethodBase.GetCurrentMethod(),patFieldNum);
			}
			string command="SELECT * FROM patfield WHERE PatFieldNum="+POut.Long(patFieldNum);
			return Crud.PatFieldCrud.SelectOne(command);
		}

		///<summary>Returns whether there are more than 0 PatFields with the given field name.</summary>
		public static bool IsFieldNameInUse(string fieldName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),fieldName);
			}
			string command="SELECT COUNT(*) FROM patfield WHERE FieldName='"+POut.String(fieldName)+"'";
			return Db.GetCount(command)!="0";
		}

		///<summary>Returns list of patnums where the pickitem is still in use.</summary>
		public static List<long> GetPatNumsUsingPickItem(string patFieldPickItemName, string patFieldName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),patFieldPickItemName, patFieldName);
			}
			string command="SELECT * FROM patfield "
				+"WHERE FieldName='"+POut.String(patFieldName)+"' "
				+"AND FieldValue='"+POut.String(patFieldPickItemName)+"'";
			return Crud.PatFieldCrud.SelectMany(command).ConvertAll(x=>x.PatNum);
		}

		///<summary>Get all PatFields for the given fieldName which belong to patients who have a corresponding entry in the RegistrationKey table. DO NOT REMOVE! Used by OD WebApps solution.</summary>
		public static List<PatField> GetPatFieldsWithRegKeys(string fieldName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PatField>>(MethodBase.GetCurrentMethod(),fieldName);
			}
			string command="SELECT * FROM patfield WHERE FieldName='"+POut.String(fieldName)+"' AND PatNum IN (SELECT PatNum FROM registrationkey)";
			return Crud.PatFieldCrud.SelectMany(command);
		}

		///<summary></summary>
		public static void Update(PatField patField) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patField);
				return;
			}
			Crud.PatFieldCrud.Update(patField);
		}

		///<summary>For all patients in the entire db when a FieldName changes.</summary>
		public static void UpdateFieldName(string patFieldNameNew, string patFieldNameOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFieldNameNew,patFieldNameOld);
				return;
			}
			string command="UPDATE patfield SET FieldName='"+POut.String(patFieldNameNew)+"' "
				+"WHERE FieldName='"+POut.String(patFieldNameOld)+"'";
			Db.NonQ(command);
		}

		///<summary>For all patients in the entire db when a PatField's PickListItem value changes.</summary>
		public static void UpdatePatFieldValues(string patFieldName, string patFieldValueNew, string patFieldValueOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patFieldName,patFieldValueNew,patFieldValueOld);
				return;
			}
			string command="UPDATE patfield SET FieldValue='"+POut.String(patFieldValueNew)+"' "
				+"WHERE FieldName='"+POut.String(patFieldName)+"' AND FieldValue='"+POut.String(patFieldValueOld)+"'";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static long Insert(PatField patField) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				patField.PatFieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),patField);
				return patField.PatFieldNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			patField.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.PatFieldCrud.Insert(patField);
		}

		///<summary></summary>
		public static void Delete(PatField pf) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),pf);
				return;
			}
			string command="DELETE FROM patfield WHERE PatFieldNum ="+POut.Long(pf.PatFieldNum);
			Db.NonQ(command);
		}

		///<summary>Frequently returns null.</summary>
		public static PatField GetByName(string name,PatField[] fieldList) {
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<fieldList.Length;i++) {
				if(fieldList[i].FieldName==name) {
					return fieldList[i];
				}
			}
			return null;
		}

		///<summary>A helper method to make a security log entry for deletion.  Because we have several patient field edit windows, this will allow us to change them all at once.</summary>
		public static void MakeDeleteLogEntry(PatField patField) {
			SecurityLogs.MakeLogEntry(EnumPermType.PatientFieldEdit,patField.PatNum,"Deleted patient field "+patField.FieldName+".  Value before deletion: \""+patField.FieldValue+"\"");
		}

		///<summary>A helper method to make a security log entry for an edit.  Because we have several patient field edit windows, this will allow us to change them all at once.</summary>
		public static void MakeEditLogEntry(PatField patFieldOld,PatField patFieldCur) {
			SecurityLogs.MakeLogEntry(EnumPermType.PatientFieldEdit,patFieldCur.PatNum
					,"Edited patient field "+patFieldCur.FieldName+"\r\n"
					+"Old value"+": \""+patFieldOld.FieldValue+"\"  New value: \""+patFieldCur.FieldValue+"\"");
		}

		///<summary>Gets all PatFields for all patients in list.</summary>
		public static List<PatField> GetPatFieldsForSuperFam(List<long> listPatNumsSuperFam) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PatField>>(MethodBase.GetCurrentMethod(),listPatNumsSuperFam);
			}
			if(listPatNumsSuperFam.Count==0) {
				return new List<PatField>();
			}
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.SuperFamilyGridCols)
				.FindAll(x=>string.IsNullOrWhiteSpace(x.InternalName));//patfields have DisplayField.InternalName blank.
			if(listDisplayFields.Count==0) {
				return new List<PatField>();
			}
			string displayFieldList=string.Join(",",listDisplayFields.Select(x=>"'"+POut.String(x.Description)+"'"));
			string patNumList=string.Join(",",listPatNumsSuperFam.Select(x=>POut.Long(x)));
			string command="SELECT * FROM patfield WHERE FieldName IN("+displayFieldList+") AND PatNum IN("+patNumList+")";
			return Crud.PatFieldCrud.SelectMany(command);
		}

		///<summary>Abbreviations only exist for pick list items. The patFieldDefName passed in does not necessarily need to be a picklist.</summary>
		public static string GetAbbrOrValue(PatField patField,string displayFieldName) {
			Meth.NoCheckMiddleTierRole();
			if(patField==null) {
				return "";//Common if this patient has no patField yet.
			}
			PatFieldDef patFieldDef=PatFieldDefs.GetFieldDefByFieldName(displayFieldName);
			if(patFieldDef==null || patFieldDef.FieldType!=PatFieldType.PickList) {
				return patField.FieldValue;
			}
			//It's a picklist
			List<PatFieldPickItem> listPatFieldPickItems=PatFieldPickItems.GetWhere(x => x.PatFieldDefNum==patFieldDef.PatFieldDefNum);
			PatFieldPickItem patFieldPickItem=listPatFieldPickItems.Find(x=>x.Name==patField.FieldValue);
			if(patFieldPickItem!=null && !string.IsNullOrWhiteSpace(patFieldPickItem.Abbreviation)) {
				return patFieldPickItem.Abbreviation;
			}
			return patField.FieldValue;
		}

		public static PatField GetPatField(long patFieldNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<PatField>(MethodBase.GetCurrentMethod(),patFieldNum);
			}
			return Crud.PatFieldCrud.SelectOne(patFieldNum);
		}
	}
}