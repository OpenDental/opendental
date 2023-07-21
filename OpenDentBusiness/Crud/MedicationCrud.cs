//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Crud{
	public class MedicationCrud {
		///<summary>Gets one Medication object from the database using the primary key.  Returns null if not found.</summary>
		public static Medication SelectOne(long medicationNum) {
			string command="SELECT * FROM medication "
				+"WHERE MedicationNum = "+POut.Long(medicationNum);
			List<Medication> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Medication object from the database using a query.</summary>
		public static Medication SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Medication> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Medication objects from the database using a query.</summary>
		public static List<Medication> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Medication> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Medication> TableToList(DataTable table) {
			List<Medication> retVal=new List<Medication>();
			Medication medication;
			foreach(DataRow row in table.Rows) {
				medication=new Medication();
				medication.MedicationNum= PIn.Long  (row["MedicationNum"].ToString());
				medication.MedName      = PIn.String(row["MedName"].ToString());
				medication.GenericNum   = PIn.Long  (row["GenericNum"].ToString());
				medication.Notes        = PIn.String(row["Notes"].ToString());
				medication.DateTStamp   = PIn.DateT (row["DateTStamp"].ToString());
				medication.RxCui        = PIn.Long  (row["RxCui"].ToString());
				retVal.Add(medication);
			}
			return retVal;
		}

		///<summary>Converts a list of Medication into a DataTable.</summary>
		public static DataTable ListToTable(List<Medication> listMedications,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Medication";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("MedicationNum");
			table.Columns.Add("MedName");
			table.Columns.Add("GenericNum");
			table.Columns.Add("Notes");
			table.Columns.Add("DateTStamp");
			table.Columns.Add("RxCui");
			foreach(Medication medication in listMedications) {
				table.Rows.Add(new object[] {
					POut.Long  (medication.MedicationNum),
					            medication.MedName,
					POut.Long  (medication.GenericNum),
					            medication.Notes,
					POut.DateT (medication.DateTStamp,false),
					POut.Long  (medication.RxCui),
				});
			}
			return table;
		}

		///<summary>Inserts one Medication into the database.  Returns the new priKey.</summary>
		public static long Insert(Medication medication) {
			return Insert(medication,false);
		}

		///<summary>Inserts one Medication into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Medication medication,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				medication.MedicationNum=ReplicationServers.GetKey("medication","MedicationNum");
			}
			string command="INSERT INTO medication (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="MedicationNum,";
			}
			command+="MedName,GenericNum,Notes,RxCui) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(medication.MedicationNum)+",";
			}
			command+=
				 "'"+POut.String(medication.MedName)+"',"
				+    POut.Long  (medication.GenericNum)+","
				+    DbHelper.ParamChar+"paramNotes,"
				//DateTStamp can only be set by MySQL
				+    POut.Long  (medication.RxCui)+")";
			if(medication.Notes==null) {
				medication.Notes="";
			}
			OdSqlParameter paramNotes=new OdSqlParameter("paramNotes",OdDbType.Text,POut.StringParam(medication.Notes));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramNotes);
			}
			else {
				medication.MedicationNum=Db.NonQ(command,true,"MedicationNum","medication",paramNotes);
			}
			return medication.MedicationNum;
		}

		///<summary>Inserts many Medications into the database.  Provides option to use the existing priKey.</summary>
		public static void InsertMany(List<Medication> listMedications,bool useExistingPK=false) {
			if(!useExistingPK && PrefC.RandomKeys) {
				foreach(Medication medication in listMedications) {
					Insert(medication);
				}
			}
			else {
				StringBuilder sbCommands=null;
				int index=0;
				int countRows=0;
				while(index < listMedications.Count) {
					Medication medication=listMedications[index];
					StringBuilder sbRow=new StringBuilder("(");
					bool hasComma=false;
					if(sbCommands==null) {
						sbCommands=new StringBuilder();
						sbCommands.Append("INSERT INTO medication (");
						if(useExistingPK) {
							sbCommands.Append("MedicationNum,");
						}
						sbCommands.Append("MedName,GenericNum,Notes,RxCui) VALUES ");
						countRows=0;
					}
					else {
						hasComma=true;
					}
					if(useExistingPK) {
						sbRow.Append(POut.Long(medication.MedicationNum)); sbRow.Append(",");
					}
					sbRow.Append("'"+POut.String(medication.MedName)+"'"); sbRow.Append(",");
					sbRow.Append(POut.Long(medication.GenericNum)); sbRow.Append(",");
					sbRow.Append("'"+POut.String(medication.Notes)+"'"); sbRow.Append(",");
					//DateTStamp can only be set by MySQL
					sbRow.Append(POut.Long(medication.RxCui)); sbRow.Append(")");
					if(sbCommands.Length+sbRow.Length+1 > TableBase.MaxAllowedPacketCount && countRows > 0) {
						Db.NonQ(sbCommands.ToString());
						sbCommands=null;
					}
					else {
						if(hasComma) {
							sbCommands.Append(",");
						}
						sbCommands.Append(sbRow.ToString());
						countRows++;
						if(index==listMedications.Count-1) {
							Db.NonQ(sbCommands.ToString());
						}
						index++;
					}
				}
			}
		}

		///<summary>Inserts one Medication into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Medication medication) {
			return InsertNoCache(medication,false);
		}

		///<summary>Inserts one Medication into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Medication medication,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO medication (";
			if(!useExistingPK && isRandomKeys) {
				medication.MedicationNum=ReplicationServers.GetKeyNoCache("medication","MedicationNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="MedicationNum,";
			}
			command+="MedName,GenericNum,Notes,RxCui) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(medication.MedicationNum)+",";
			}
			command+=
				 "'"+POut.String(medication.MedName)+"',"
				+    POut.Long  (medication.GenericNum)+","
				+    DbHelper.ParamChar+"paramNotes,"
				//DateTStamp can only be set by MySQL
				+    POut.Long  (medication.RxCui)+")";
			if(medication.Notes==null) {
				medication.Notes="";
			}
			OdSqlParameter paramNotes=new OdSqlParameter("paramNotes",OdDbType.Text,POut.StringParam(medication.Notes));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramNotes);
			}
			else {
				medication.MedicationNum=Db.NonQ(command,true,"MedicationNum","medication",paramNotes);
			}
			return medication.MedicationNum;
		}

		///<summary>Updates one Medication in the database.</summary>
		public static void Update(Medication medication) {
			string command="UPDATE medication SET "
				+"MedName      = '"+POut.String(medication.MedName)+"', "
				+"GenericNum   =  "+POut.Long  (medication.GenericNum)+", "
				+"Notes        =  "+DbHelper.ParamChar+"paramNotes, "
				//DateTStamp can only be set by MySQL
				+"RxCui        =  "+POut.Long  (medication.RxCui)+" "
				+"WHERE MedicationNum = "+POut.Long(medication.MedicationNum);
			if(medication.Notes==null) {
				medication.Notes="";
			}
			OdSqlParameter paramNotes=new OdSqlParameter("paramNotes",OdDbType.Text,POut.StringParam(medication.Notes));
			Db.NonQ(command,paramNotes);
		}

		///<summary>Updates one Medication in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Medication medication,Medication oldMedication) {
			string command="";
			if(medication.MedName != oldMedication.MedName) {
				if(command!="") { command+=",";}
				command+="MedName = '"+POut.String(medication.MedName)+"'";
			}
			if(medication.GenericNum != oldMedication.GenericNum) {
				if(command!="") { command+=",";}
				command+="GenericNum = "+POut.Long(medication.GenericNum)+"";
			}
			if(medication.Notes != oldMedication.Notes) {
				if(command!="") { command+=",";}
				command+="Notes = "+DbHelper.ParamChar+"paramNotes";
			}
			//DateTStamp can only be set by MySQL
			if(medication.RxCui != oldMedication.RxCui) {
				if(command!="") { command+=",";}
				command+="RxCui = "+POut.Long(medication.RxCui)+"";
			}
			if(command=="") {
				return false;
			}
			if(medication.Notes==null) {
				medication.Notes="";
			}
			OdSqlParameter paramNotes=new OdSqlParameter("paramNotes",OdDbType.Text,POut.StringParam(medication.Notes));
			command="UPDATE medication SET "+command
				+" WHERE MedicationNum = "+POut.Long(medication.MedicationNum);
			Db.NonQ(command,paramNotes);
			return true;
		}

		///<summary>Returns true if Update(Medication,Medication) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Medication medication,Medication oldMedication) {
			if(medication.MedName != oldMedication.MedName) {
				return true;
			}
			if(medication.GenericNum != oldMedication.GenericNum) {
				return true;
			}
			if(medication.Notes != oldMedication.Notes) {
				return true;
			}
			//DateTStamp can only be set by MySQL
			if(medication.RxCui != oldMedication.RxCui) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Medication from the database.</summary>
		public static void Delete(long medicationNum) {
			string command="DELETE FROM medication "
				+"WHERE MedicationNum = "+POut.Long(medicationNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many Medications from the database.</summary>
		public static void DeleteMany(List<long> listMedicationNums) {
			if(listMedicationNums==null || listMedicationNums.Count==0) {
				return;
			}
			string command="DELETE FROM medication "
				+"WHERE MedicationNum IN("+string.Join(",",listMedicationNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}