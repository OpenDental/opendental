using System;
using System.Collections;
using System.Data;

namespace ODR{
	///<summary></summary>
	public class MakeReadablePatNum{
		private Hashtable hash;

		///<summary>Constructor</summary>
		public MakeReadablePatNum(){
			hash=new Hashtable();
			string command="SELECT PatNum,LName,FName,Preferred,MiddleI FROM patient";
			DataConnection dcon=new DataConnection();
			DataTable table=dcon.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++){
				if(table.Rows[i][3].ToString()==""){//no preferred
					hash.Add(table.Rows[i][0].ToString(),//key is patNum, in string format
						table.Rows[i][1].ToString()+", "//LName
						+table.Rows[i][2].ToString()+" "//FName
						+table.Rows[i][4].ToString());//MiddleI
				}
				else{
					hash.Add(table.Rows[i][0].ToString(),//key is patNum, in string format
						table.Rows[i][1].ToString()+", '"//LName
						+table.Rows[i][3].ToString()+"' "//Preferred
						+table.Rows[i][2].ToString()+" "//FName
						+table.Rows[i][4].ToString());//MiddleI
				}
			}
		}

		public string Get(string patNum){
			if(hash.ContainsKey(patNum)){
				return (string)hash[patNum];
			}
			return "";
			/*if(patNum=="1"){
				return "one1";
			}
			else if(patNum=="2"){
				return "two2";
			}
			return "other3";*/
		}
		
	}

}
