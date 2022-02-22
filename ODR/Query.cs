using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenDentBusiness;

namespace ODR{
	///<summary></summary>
	public class Query{
		///<summary></summary>
		private ParameterCollection Parameters;
		private bool Cancel;

		///<summary>Constructor</summary>
		public Query(){
			Parameters=new ParameterCollection();
		}

		///<summary>Parameters only get displayed the first time this function is run.  After that, those parameters are used repeatedly without reprompting the user again.</summary>
		public string Get(string query,string parameters){
			if(Cancel){
				return "";
			}
			if(parameters!="" && Parameters.Count==0){//if this is the first time through
				//assemble parameters
				string[] paramList=parameters.Split(new char[] {'~'});
				string[] paramFields;
				Parameter param;
				//string fieldValue;
				for(int i=0;i<paramList.Length;i++){
					paramFields=paramList[i].Split(new char[] {';'});
					param=new Parameter();
					for(int f=0;f<paramFields.Length;f++){
						if(paramFields[f].StartsWith("Name=")){
							param.Name=paramFields[f].Replace("Name=","");
						}
						//param.OutputValue="1";//never passed in
						else if(paramFields[f].StartsWith("ValueType=")){
							switch(paramFields[f].Replace("ValueType=","")){
								case "Date":
									param.ValueType=ParamValueType.Date;
									break;
								case "String":
									param.ValueType=ParamValueType.String;
									break;
								case "Boolean":
									param.ValueType=ParamValueType.Boolean;
									break;
								case "Integer":
									param.ValueType=ParamValueType.Integer;
									break;
								case "Number":
									param.ValueType=ParamValueType.Number;
									break;
								case "Enum":
									param.ValueType=ParamValueType.Enum;
									break;
								case "QueryData":
									param.ValueType=ParamValueType.QueryData;
									break;
								default:
									MessageBox.Show("Error in ValueType field: "+paramFields[f]);
									return "";
							}
						}
						//param.CurrentValues=new ArrayList();//never passed in
						//param.defaultValues=new ArrayList();//not functional yet
						else if(paramFields[f].StartsWith("Prompt=")){
							param.Prompt=paramFields[f].Replace("Prompt=","");
						}
						else if(paramFields[f].StartsWith("Snippet=")){
							param.Snippet=paramFields[f].Replace("Snippet=","");
						}
						else if(paramFields[f].StartsWith("EnumerationType=")){
							switch(paramFields[f].Replace("EnumerationType=","")){
								case "PatientStatus":
									param.EnumerationType=EnumType.PatientStatus;
									break;
							}
						}
						else{
							MessageBox.Show("Error. Unrecognized field: "+paramFields[f]);
							return "";
						}
					}
					Parameters.Add(param);
				}
				//pass parameters to a form for display
				using FormParameterInput FormP=new FormParameterInput(ref Parameters);
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel){
					Cancel=true;
					return "";
				}
				//MessageBox.Show("parameter info: "+parameters);
			}
			return Get(query);
		}

		///<summary>This is used for subsequent queries in a report since only one query passes in parameters.</summary>
		public string Get(string query){
			if(Cancel){
				return "";
			}
			string outputQuery=query;
			string replacement="";//the replacement value to put into the outputQuery for each match
			//first replace all parameters with values:
			MatchCollection mc;
			Regex regex=new Regex(@"\?\w+");//? followed by one or more text characters
			mc=regex.Matches(outputQuery);
			//loop through each occurance of "?etc"
			for(int i=0;i<mc.Count;i++){
				replacement=Parameters[mc[i].Value.Substring(1)].OutputValue;
				regex=new Regex(@"\"+mc[i].Value);
				outputQuery=regex.Replace(outputQuery,replacement);
			}			
			//MessageBox.Show("output query: "+outputQuery);
			//string retVal="SELECT * FROM patient "+DbHelper.LimitWhere(10);
			return outputQuery;
		}

		public string GetParameterValue(string name){
			if(Parameters.Count==0){
				return "";
			}
			foreach(Parameter p in Parameters) {
				if(p.Name==name){
					string retVal="";
					for(int i=0;i<p.CurrentValues.Count;i++){
						if(i>0){
							retVal+=", ";
						}
						switch(p.ValueType){
							case ParamValueType.Date:
								retVal+=((DateTime)p.CurrentValues[i]).ToShortDateString();
								break;
							case ParamValueType.String:
								retVal+=p.CurrentValues[i].ToString();
								break;
							case ParamValueType.Boolean:
								if(((bool)p.CurrentValues[i])){
									retVal+="true";
								}
								else{
									retVal+="false";
								}
								break;
							case ParamValueType.Integer:
								retVal+=((int)p.CurrentValues[i]).ToString();
								break;
							case ParamValueType.Number:
								retVal+=((Double)p.CurrentValues[i]).ToString("n");
								break;
							case ParamValueType.Enum:
								retVal+=((int)p.CurrentValues[i]).ToString();//needs some work to convert # to string
								break;
							case ParamValueType.QueryData:
								retVal+=((int)p.CurrentValues[i]).ToString();//again, we should convert this to string
								break;
						}//switch
					}//for CurrentValues
					return retVal;
				}
			}
			return "";
		}


	}



	

}
