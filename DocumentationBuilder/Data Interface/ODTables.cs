using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentationBuilder {
	public class ODTables {
		///<summary>Returns a list of all tables for the db connection in dcon.</summary>
		public static List<ODTable> GetODTables(DataConnection dcon) {
			List<ODTable> listOdTables=new List<ODTable>();
			//Get the dbName from the ConnectionString. The connection string will have the database name.
			string dbName=dcon.ConnStr.Split(';').Where(x=>x.ToLower().Contains("database")).ToList()[0].ToLower().Remove(0,"database=".Count());
			string command="SELECT * FROM information_schema.columns WHERE table_schema='"+dbName+"'";
			DataTable table=dcon.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				string tableName=table.Rows[i]["TABLE_NAME"].ToString();
				if(listOdTables.Select(x => x.Name).Contains(tableName)) {
					//The data table will have a row for every column. 
					//Table name already exist in our List of ODTables
					continue;
				}
				//Get a list of DataRows for the current table that contains all of the columns for the table.
				List<DataRow> listTableColumns=table.Rows.OfType<DataRow>().Where(x => x["TABLE_NAME"].ToString()==tableName).ToList();
				List<ODColumn> listColumns=new List<ODColumn>();
				for(int j=0;j<listTableColumns.Count;j++) {
					listColumns.Add(new ODColumn(listTableColumns[j]["COLUMN_NAME"].ToString(),listTableColumns[j]["COLUMN_TYPE"].ToString(),j.ToString(),new List<ODEnum>()));
				}
				//Create a new ODTable.
				listOdTables.Add(new ODTable(tableName,listColumns));
			}
			return listOdTables;
		}
	}
}
