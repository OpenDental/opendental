using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LabTurnarounds {
		///<summary></summary>
		public static List<LabTurnaround> GetForLab(long laboratoryNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabTurnaround>>(MethodBase.GetCurrentMethod(),laboratoryNum);
			}
			string command="SELECT * FROM labturnaround WHERE LaboratoryNum="+POut.Long(laboratoryNum);
			DataTable table=Db.GetTable(command);
			List<LabTurnaround> listLabTurnarounds=new List<LabTurnaround>();
			for(int i=0;i<table.Rows.Count;i++) {
				LabTurnaround labTurnaround=new LabTurnaround();
				labTurnaround.LabTurnaroundNum= PIn.Long   (table.Rows[i][0].ToString());
				labTurnaround.LaboratoryNum   = PIn.Long   (table.Rows[i][1].ToString());
				labTurnaround.Description     = PIn.String(table.Rows[i][2].ToString());
				labTurnaround.DaysPublished   = PIn.Int   (table.Rows[i][3].ToString());
				labTurnaround.DaysActual      = PIn.Int   (table.Rows[i][4].ToString());
				listLabTurnarounds.Add(labTurnaround);
			}
			return listLabTurnarounds;
		}

		///<summary>Gets one labturnaround for the API. Returns null if not found.</summary>
		public static LabTurnaround GetOneLabTurnaroundForApi(long labTurnaroundNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<LabTurnaround>(MethodBase.GetCurrentMethod(),labTurnaroundNum);
			}
			string command="SELECT * FROM labturnaround WHERE LabTurnaroundNum="+POut.Long(labTurnaroundNum);
			return Crud.LabTurnaroundCrud.SelectOne(command);
		}

		///<summary>Gets a list of labturnarounds optionally filtered for the API. Returns an empty list if not found.</summary>
		public static List<LabTurnaround> GetLabTurnaroundsForApi(int limit,int offset,long laboratoryNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<LabTurnaround>>(MethodBase.GetCurrentMethod(),limit,offset,laboratoryNum);
			}
			string command="SELECT * FROM labturnaround ";
			if(laboratoryNum>0) {
				command+="WHERE LaboratoryNum="+POut.Long(laboratoryNum)+" ";
			}
			command+="ORDER BY LabTurnaroundNum "//Ensure order for limit and offset.
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.LabTurnaroundCrud.SelectMany(command);
		}

		///<summary>This is used when saving a laboratory.  All labturnarounds for the lab are deleted and recreated.  So the list that's passed in will not have the correct keys set.  The key columns will be ignored.</summary>
		public static void SetForLab(long laboratoryNum,List<LabTurnaround> listLabTurnarounds) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),laboratoryNum,listLabTurnarounds);
				return;
			}
			string command="DELETE FROM labturnaround WHERE LaboratoryNum="+POut.Long(laboratoryNum);
			Db.NonQ(command);
			for(int i=0;i<listLabTurnarounds.Count;i++){
				listLabTurnarounds[i].LaboratoryNum=laboratoryNum;
				Insert(listLabTurnarounds[i]);
			}
		}

		///<summary></summary>
		public static long Insert(LabTurnaround labTurnaround) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				labTurnaround.LabTurnaroundNum=Meth.GetLong(MethodBase.GetCurrentMethod(),labTurnaround);
				return labTurnaround.LabTurnaroundNum;
			}
			return Crud.LabTurnaroundCrud.Insert(labTurnaround);
		}

		///<summary>Calculates the due date by adding the number of business days listed.  Adds an additional day for each office holiday.</summary>
		public static DateTime ComputeDueDate(DateTime dateStart,int days){
			Meth.NoCheckMiddleTierRole();
			DateTime date=dateStart;
			int counter=0;
			while(true){
				if(counter>=days) {
					break;
				}
				date=date.AddDays(1);
				if(date.DayOfWeek==DayOfWeek.Saturday || date.DayOfWeek==DayOfWeek.Sunday){
					continue;
				}
				if(Schedules.DateIsHoliday(date)){
					continue;
				}
				counter++;
			}
			return date+TimeSpan.FromHours(17);//always due at 5pm on day specified.
		}
	}
	


}













