using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using OpenDentalWebCore.Extensions;

namespace OpenDentalWebCore.Serializing {
	///<summary>This file is generated automatically by the crud, do not make any changes to this file because they will get overwritten.</summary>
	public class CareCreditWebResponse {

		///<summary></summary>
		public static string Serialize(OpenDentBusiness.CareCreditWebResponse carecreditwebresponse) {
			StringBuilder sb=new StringBuilder();
			if(carecreditwebresponse==null) {
				sb.Append("<CareCreditWebResponse null='true'/>");
				return sb.ToString();
			}
			sb.Append("<CareCreditWebResponse>");
			sb.Append("<CareCreditWebResponseNum>").Append(carecreditwebresponse.CareCreditWebResponseNum).Append("</CareCreditWebResponseNum>");
			sb.Append("<PatNum>").Append(carecreditwebresponse.PatNum).Append("</PatNum>");
			sb.Append("<PayNum>").Append(carecreditwebresponse.PayNum).Append("</PayNum>");
			sb.Append("<RefNumber>").Append(SerializeStringEscapes.EscapeForXml(carecreditwebresponse.RefNumber)).Append("</RefNumber>");
			sb.Append("<Amount>").Append(carecreditwebresponse.Amount.ToString(System.Globalization.CultureInfo.GetCultureInfo("en-US").NumberFormat)).Append("</Amount>");
			sb.Append("<WebToken>").Append(SerializeStringEscapes.EscapeForXml(carecreditwebresponse.WebToken)).Append("</WebToken>");
			sb.Append("<ProcessingStatus>").Append(SerializeStringEscapes.EscapeForXml(carecreditwebresponse.ProcessingStatus.ToString())).Append("</ProcessingStatus>");
			sb.Append("<DateTimeEntry>").Append(carecreditwebresponse.DateTimeEntry.ToString("yyyy-MM-dd HH:mm:ss")).Append("</DateTimeEntry>");
			sb.Append("<DateTimePending>").Append(carecreditwebresponse.DateTimePending.ToString("yyyy-MM-dd HH:mm:ss")).Append("</DateTimePending>");
			sb.Append("<DateTimeCompleted>").Append(carecreditwebresponse.DateTimeCompleted.ToString("yyyy-MM-dd HH:mm:ss")).Append("</DateTimeCompleted>");
			sb.Append("<DateTimeExpired>").Append(carecreditwebresponse.DateTimeExpired.ToString("yyyy-MM-dd HH:mm:ss")).Append("</DateTimeExpired>");
			sb.Append("<DateTimeLastError>").Append(carecreditwebresponse.DateTimeLastError.ToString("yyyy-MM-dd HH:mm:ss")).Append("</DateTimeLastError>");
			sb.Append("<LastResponseStr>").Append(SerializeStringEscapes.EscapeForXml(carecreditwebresponse.LastResponseStr)).Append("</LastResponseStr>");
			sb.Append("<ClinicNum>").Append(carecreditwebresponse.ClinicNum).Append("</ClinicNum>");
			sb.Append("<ServiceType>").Append(SerializeStringEscapes.EscapeForXml(carecreditwebresponse.ServiceType.ToString())).Append("</ServiceType>");
			sb.Append("<TransType>").Append(SerializeStringEscapes.EscapeForXml(carecreditwebresponse.TransType.ToString())).Append("</TransType>");
			sb.Append("<MerchantNumber>").Append(SerializeStringEscapes.EscapeForXml(carecreditwebresponse.MerchantNumber)).Append("</MerchantNumber>");
			sb.Append("<HasLogged>").Append((carecreditwebresponse.HasLogged)?1:0).Append("</HasLogged>");
			sb.Append("</CareCreditWebResponse>");
			return sb.ToString();
		}

		///<summary></summary>
		public static OpenDentBusiness.CareCreditWebResponse Deserialize(string xml) {
			OpenDentBusiness.CareCreditWebResponse carecreditwebresponse=new OpenDentBusiness.CareCreditWebResponse();
			using(XmlReader reader=XmlReader.Create(new StringReader(xml))) {
				reader.MoveToContent();
				if(reader.HasAttributes && reader.GetAttribute("null")!=null) {
					return null;
				}
				if(reader.IsEmptyElement) {
					return carecreditwebresponse;
				}
				while(reader.Read()) {
					//Only detect start elements.
					if(!reader.IsStartElement()) {
						continue;
					}
					//save field name and move to the value
					string fieldName=reader.Name;
					reader.Read();
					switch(fieldName) {
						case "CareCreditWebResponseNum":
							carecreditwebresponse.CareCreditWebResponseNum=System.Convert.ToInt64(reader.ReadContentAsString());
							break;
						case "PatNum":
							carecreditwebresponse.PatNum=System.Convert.ToInt64(reader.ReadContentAsString());
							break;
						case "PayNum":
							carecreditwebresponse.PayNum=System.Convert.ToInt64(reader.ReadContentAsString());
							break;
						case "RefNumber":
							carecreditwebresponse.RefNumber=SerializeStringEscapes.ReplaceEscapes(reader.ReadContentAsString());
							break;
						case "Amount":
							carecreditwebresponse.Amount=System.Convert.ToDouble(reader.ReadContentAsString(),System.Globalization.CultureInfo.GetCultureInfo("en-US").NumberFormat);
							break;
						case "WebToken":
							carecreditwebresponse.WebToken=SerializeStringEscapes.ReplaceEscapes(reader.ReadContentAsString());
							break;
						case "ProcessingStatus":
							carecreditwebresponse.ProcessingStatus=(OpenDentBusiness.CareCreditWebStatus)Enum.Parse(typeof(OpenDentBusiness.CareCreditWebStatus),SerializeStringEscapes.ReplaceEscapes(reader.ReadContentAsString()));
							break;
						case "DateTimeEntry":
							carecreditwebresponse.DateTimeEntry=DateTime.ParseExact(reader.ReadContentAsString(),"yyyy-MM-dd HH:mm:ss",null);
							break;
						case "DateTimePending":
							carecreditwebresponse.DateTimePending=DateTime.ParseExact(reader.ReadContentAsString(),"yyyy-MM-dd HH:mm:ss",null);
							break;
						case "DateTimeCompleted":
							carecreditwebresponse.DateTimeCompleted=DateTime.ParseExact(reader.ReadContentAsString(),"yyyy-MM-dd HH:mm:ss",null);
							break;
						case "DateTimeExpired":
							carecreditwebresponse.DateTimeExpired=DateTime.ParseExact(reader.ReadContentAsString(),"yyyy-MM-dd HH:mm:ss",null);
							break;
						case "DateTimeLastError":
							carecreditwebresponse.DateTimeLastError=DateTime.ParseExact(reader.ReadContentAsString(),"yyyy-MM-dd HH:mm:ss",null);
							break;
						case "LastResponseStr":
							carecreditwebresponse.LastResponseStr=SerializeStringEscapes.ReplaceEscapes(reader.ReadContentAsString());
							break;
						case "ClinicNum":
							carecreditwebresponse.ClinicNum=System.Convert.ToInt64(reader.ReadContentAsString());
							break;
						case "ServiceType":
							carecreditwebresponse.ServiceType=(OpenDentBusiness.CareCreditServiceType)Enum.Parse(typeof(OpenDentBusiness.CareCreditServiceType),SerializeStringEscapes.ReplaceEscapes(reader.ReadContentAsString()));
							break;
						case "TransType":
							carecreditwebresponse.TransType=(OpenDentBusiness.CareCreditTransType)Enum.Parse(typeof(OpenDentBusiness.CareCreditTransType),SerializeStringEscapes.ReplaceEscapes(reader.ReadContentAsString()));
							break;
						case "MerchantNumber":
							carecreditwebresponse.MerchantNumber=SerializeStringEscapes.ReplaceEscapes(reader.ReadContentAsString());
							break;
						case "HasLogged":
							carecreditwebresponse.HasLogged=reader.ReadContentAsString()!="0";
							break;
					}
				}
			}
			return carecreditwebresponse;
		}

		///<summary></summary>
		public static List<OpenDentBusiness.CareCreditWebResponse> FromDataTable(DataTable dataTable) {
			List<OpenDentBusiness.CareCreditWebResponse> carecreditwebresponses=new List<OpenDentBusiness.CareCreditWebResponse>();
			for(int i=0;i<dataTable.Rows.Count;i++) {
				try{
					OpenDentBusiness.CareCreditWebResponse carecreditwebresponse=new OpenDentBusiness.CareCreditWebResponse();
					carecreditwebresponse.CareCreditWebResponseNum=System.Convert.ToInt64(dataTable.Rows[i]["CareCreditWebResponseNum"]);
					carecreditwebresponse.PatNum=System.Convert.ToInt64(dataTable.Rows[i]["PatNum"]);
					carecreditwebresponse.PayNum=System.Convert.ToInt64(dataTable.Rows[i]["PayNum"]);
					carecreditwebresponse.RefNumber=SerializeStringEscapes.ReplaceEscapes(System.Convert.ToString(dataTable.Rows[i]["RefNumber"]));
					carecreditwebresponse.Amount=System.Convert.ToDouble(dataTable.Rows[i]["Amount"]);
					carecreditwebresponse.WebToken=SerializeStringEscapes.ReplaceEscapes(System.Convert.ToString(dataTable.Rows[i]["WebToken"]));
					carecreditwebresponse.ProcessingStatus=(OpenDentBusiness.CareCreditWebStatus)Enum.Parse(typeof(OpenDentBusiness.CareCreditWebStatus),SerializeStringEscapes.ReplaceEscapes(System.Convert.ToString(dataTable.Rows[i]["ProcessingStatus"])));
					carecreditwebresponse.DateTimeEntry=OpenDentBusiness.PIn.DateT(System.Convert.ToString(dataTable.Rows[i]["DateTimeEntry"]));
					carecreditwebresponse.DateTimePending=OpenDentBusiness.PIn.DateT(System.Convert.ToString(dataTable.Rows[i]["DateTimePending"]));
					carecreditwebresponse.DateTimeCompleted=OpenDentBusiness.PIn.DateT(System.Convert.ToString(dataTable.Rows[i]["DateTimeCompleted"]));
					carecreditwebresponse.DateTimeExpired=OpenDentBusiness.PIn.DateT(System.Convert.ToString(dataTable.Rows[i]["DateTimeExpired"]));
					carecreditwebresponse.DateTimeLastError=OpenDentBusiness.PIn.DateT(System.Convert.ToString(dataTable.Rows[i]["DateTimeLastError"]));
					carecreditwebresponse.LastResponseStr=SerializeStringEscapes.ReplaceEscapes(System.Convert.ToString(dataTable.Rows[i]["LastResponseStr"]));
					carecreditwebresponse.ClinicNum=System.Convert.ToInt64(dataTable.Rows[i]["ClinicNum"]);
					carecreditwebresponse.ServiceType=(OpenDentBusiness.CareCreditServiceType)Enum.Parse(typeof(OpenDentBusiness.CareCreditServiceType),SerializeStringEscapes.ReplaceEscapes(System.Convert.ToString(dataTable.Rows[i]["ServiceType"])));
					carecreditwebresponse.TransType=(OpenDentBusiness.CareCreditTransType)Enum.Parse(typeof(OpenDentBusiness.CareCreditTransType),SerializeStringEscapes.ReplaceEscapes(System.Convert.ToString(dataTable.Rows[i]["TransType"])));
					carecreditwebresponse.MerchantNumber=SerializeStringEscapes.ReplaceEscapes(System.Convert.ToString(dataTable.Rows[i]["MerchantNumber"]));
					carecreditwebresponse.HasLogged=System.Convert.ToBoolean(Convert.ToInt16(dataTable.Rows[i]["HasLogged"]));
					carecreditwebresponses.Add(carecreditwebresponse);
				}
				catch{ }
			}
			return carecreditwebresponses;
		}

		///<summary>Converts a list of OpenDentBusiness.CareCreditWebResponse into a DataTable.</summary>
		public static DataTable ListToTable(List<OpenDentBusiness.CareCreditWebResponse> listCareCreditWebResponses,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="CareCreditWebResponse";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("CareCreditWebResponseNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("PayNum");
			table.Columns.Add("RefNumber");
			table.Columns.Add("Amount");
			table.Columns.Add("WebToken");
			table.Columns.Add("ProcessingStatus");
			table.Columns.Add("DateTimeEntry");
			table.Columns.Add("DateTimePending");
			table.Columns.Add("DateTimeCompleted");
			table.Columns.Add("DateTimeExpired");
			table.Columns.Add("DateTimeLastError");
			table.Columns.Add("LastResponseStr");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("ServiceType");
			table.Columns.Add("TransType");
			table.Columns.Add("MerchantNumber");
			table.Columns.Add("HasLogged");
			foreach(OpenDentBusiness.CareCreditWebResponse careCreditWebResponse in listCareCreditWebResponses) {
				table.Rows.Add(new object[] {
					OpenDentBusiness.POut.Long  (careCreditWebResponse.CareCreditWebResponseNum),
					OpenDentBusiness.POut.Long  (careCreditWebResponse.PatNum),
					OpenDentBusiness.POut.Long  (careCreditWebResponse.PayNum),
					                             careCreditWebResponse.RefNumber,
					OpenDentBusiness.POut.Double(careCreditWebResponse.Amount),
					                             careCreditWebResponse.WebToken,
					                             careCreditWebResponse.ProcessingStatus.ToString(),
					OpenDentBusiness.POut.DateT (careCreditWebResponse.DateTimeEntry,false),
					OpenDentBusiness.POut.DateT (careCreditWebResponse.DateTimePending,false),
					OpenDentBusiness.POut.DateT (careCreditWebResponse.DateTimeCompleted,false),
					OpenDentBusiness.POut.DateT (careCreditWebResponse.DateTimeExpired,false),
					OpenDentBusiness.POut.DateT (careCreditWebResponse.DateTimeLastError,false),
					                             careCreditWebResponse.LastResponseStr,
					OpenDentBusiness.POut.Long  (careCreditWebResponse.ClinicNum),
					                             careCreditWebResponse.ServiceType.ToString(),
					                             careCreditWebResponse.TransType.ToString(),
					                             careCreditWebResponse.MerchantNumber,
					OpenDentBusiness.POut.Bool  (careCreditWebResponse.HasLogged),
				});
			}
			return table;
		}

	}
}