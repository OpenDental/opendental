using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;

namespace DataConnectionBase {
	public class MySqlHelper {
    ///<summary>The new open source MySqlConnector does not have this method like the old Oracle/MySQL connector. Create our own using the same logic as the old Oracle/MySQL connector.</summary>
    public static DataSet ExecuteDataset(string connectionString, string commandText,params MySqlParameter[] commandParameters) {
      using(MySqlConnection connection=new MySqlConnection(connectionString)) {
        connection.Open();
        MySqlCommand selectCommand=new MySqlCommand();
        selectCommand.Connection=connection;
        selectCommand.CommandText=commandText;
        selectCommand.CommandType=CommandType.Text;
        if(commandParameters!=null) {
          for(int i=0;i<commandParameters.Count();i++) {
            selectCommand.Parameters.Add(commandParameters[i]);
          }
        }
        MySqlDataAdapter mySqlDataAdapter=new MySqlDataAdapter(selectCommand);
        DataSet dataSet=new DataSet();
        mySqlDataAdapter.Fill(dataSet);
        selectCommand.Parameters.Clear();
        return dataSet;
      }
		}

    public static DataRow ExecuteDataRow(string connectionString, string commandText,params MySqlParameter[] commandParameters) {
      DataSet dataSet=MySqlHelper.ExecuteDataset(connectionString, commandText, commandParameters);
      if(dataSet==null || dataSet.Tables.Count==0) {
        return null;
      }
      return dataSet.Tables[0].Rows.Count==0 ? null:dataSet.Tables[0].Rows[0];
    }
	}
}
