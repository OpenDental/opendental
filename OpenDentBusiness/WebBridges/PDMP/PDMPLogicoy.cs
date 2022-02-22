using CodeBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using static OpenDentBusiness.PDMP;

namespace OpenDentBusiness {
	public class PDMPLogicoy {
		private PdmpProperty _pdmpProperties;
		private PDMPScript.USStateCodeBaseType _stateCode;

		public PDMPLogicoy(PdmpProperty props) {
			_pdmpProperties=props;
			Enum.TryParse(_pdmpProperties.StateAbbr,out _stateCode);
		}

		///<summary>Submits a request to the appropriate API and returns a string containing the URL supplied by the API.  Throws exceptions.</summary>
		public string GetURL() {
			PDMPScript.PrescriptionSummaryRequestType request=new PDMPScript.PrescriptionSummaryRequestType();
			request.Requester=MakeRequester();
			request.PrescriptionRequest=MakePrescriptionRequest();
			PDMPScript.PrescriptionSummaryResponseType response=Request(HttpMethod.Post
				,MakeBasicAuthHeader(),Serialize(request),new PDMPScript.PrescriptionSummaryResponseType());
			if(response.Error!=null) {
				throw new ApplicationException(response.Error.Message);//Request resulted in an error.
			}
			return response.Report.ReportRequestURLs.ViewableReport.Value;
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private T Request<T>(HttpMethod method,string authHeader,string body,T responseType) {
			using(WebClient client=new WebClient()) {
				client.Headers[HttpRequestHeader.Accept]="application/xml";
				client.Headers[HttpRequestHeader.ContentType]="application/xml";
				client.Headers[HttpRequestHeader.Authorization]=authHeader;
				client.Encoding=UnicodeEncoding.UTF8;
				try {
					string res="";
					if(method==HttpMethod.Get) {
						res=client.DownloadString(_pdmpProperties.Url);
					}
					else if(method==HttpMethod.Post) {
						res=client.UploadString(_pdmpProperties.Url,HttpMethod.Post.Method,body);
					}
					else if(method==HttpMethod.Put) {
						res=client.UploadString(_pdmpProperties.Url,HttpMethod.Put.Method,body);
					}
					else {
						throw new Exception("Unsupported HttpMethod type: "+method.Method);
					}
					if(ODBuild.IsDebug()) {
						if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
							return (T)Convert.ChangeType(res,typeof(T));
						}
						Console.WriteLine(res);
					}
					return Deserialize<T>(res,responseType);
				}
				catch(WebException wex) {
					string res="";
					using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
					}
					if(string.IsNullOrWhiteSpace(res)) {
						//The response didn't contain a body.  Through my limited testing, it only happens for 401 (Unauthorized) requests.
						if(wex.Response.GetType()==typeof(HttpWebResponse)) {
							HttpStatusCode statusCode=((HttpWebResponse)wex.Response).StatusCode;
							if(statusCode==HttpStatusCode.Unauthorized) {
								throw new ODException(Lans.g("PDMP","Invalid PDMP credentials."));
							}
						}
					}
					string errorMsg=wex.Message+(string.IsNullOrWhiteSpace(res) ? "" : "\r\nRaw response:\r\n"+res);
					throw new Exception(errorMsg,wex);//If we got this far and haven't rethrown, simply throw the entire exception.
				}
				catch(Exception ex) {
					//WebClient returned an http status code >= 300
					ex.DoNothing();
					//For now, rethrow error and let whoever is expecting errors to handle them.
					//We may enhance this to care about codes at some point.
					throw;
				}
			}
		}

		private string Serialize<T>(T request) {
			using(MemoryStream memoryStream=new MemoryStream()) {
				XmlSerializer xmlSerializer=new XmlSerializer(request.GetType());
				xmlSerializer.Serialize(memoryStream,request);
				byte[] memoryStreamInBytes=memoryStream.ToArray();
				return Encoding.UTF8.GetString(memoryStreamInBytes,0,memoryStreamInBytes.Length);
			}
		}

		private T Deserialize<T>(string strXml,T responseType) {
			XmlDocument doc=new XmlDocument();
			doc.LoadXml(strXml);
			using(XmlReader reader=new XmlNodeReader(doc)) {
				XmlSerializer xmlSerializer=new XmlSerializer(typeof(T));
				T response;
				try {
					response=(T)xmlSerializer.Deserialize(reader);
				}
				catch(Exception ex) {
					ex.DoNothing();
					reader.Close();
					//Responses from PDMP Logicoy have contained "xsi:type" attributes in the Pmp node.  This causes deserialization to fail.  
					//Removing it has been the only thing that has worked in our extensive testing when this occurs.
					StripAttributeFromNode(doc.ChildNodes,"Pmp","xsi:type");
					using(XmlReader strippedReader=new XmlNodeReader(doc)){
						response=(T)xmlSerializer.Deserialize(strippedReader);
						strippedReader.Close();
					}
				}
				reader.Close();
				return response;
			}
		}

		///<summary>Strips out an attribute of Name=strAttributeName from XmlNode.Name=nodeName in an XmlNodeList nodeList, and sub lists.</summary>
		private void StripAttributeFromNode(XmlNodeList nodeList,string nodeName,params string[] arrAttributeNames) {
			if(nodeList==null) {
				return;
			}
			foreach(XmlNode node in nodeList) {
				StripAttributeFromNode(node.ChildNodes,nodeName,arrAttributeNames);//Recursively process all ChildNodes.
				if(node.Name!=nodeName) {
					continue;
				}
				for(int i=node.Attributes.Count-1;i>=0;i--) {
					if(ListTools.In(node.Attributes[i].Name,arrAttributeNames)) {
						node.Attributes.RemoveAt(i);
					}
				}
			}
		}

		public PDMPScript.RequesterType MakeRequester() {
			//There is no documentation identifying what the use case of "RoleType" is.  Setting to Dentist for now seems like the safe choice.
			PDMPScript.RoleType role=PDMPScript.RoleType.Dentist;
			string[] arrRequestDestinations=new string[] { _pdmpProperties.StateAbbr, };
			PDMPScript.RequesterType requester=new PDMPScript.RequesterType {
				SenderSoftware=new PDMPScript.SenderSoftware {
					Developer="Open Dental Software",
					Product="PDMP-LINK",
					Version="v1",
				},
				RequestDestinations=arrRequestDestinations,
				Provider=new PDMPScript.Provider {
					Role=role,
					FirstName=_pdmpProperties.PdmpProv.FName,
					LastName=_pdmpProperties.PdmpProv.LName,
					DEANumber=new PDMPScript.DEANumberType {
						Value=_pdmpProperties.Dea,
					},
					NPINumber=new PDMPScript.NPINumberType {
						Value=_pdmpProperties.PdmpProv.NationalProvID,
					},
					ProfessionalLicenseNumber=new PDMPScript.ProfessionalLicenseNumber {
						Value=new PDMPScript.LimitedStringType {
							Value=_pdmpProperties.ProvLicenseNum,
						},
					},
					LocationStateCode=new PDMPScript.USStateCodeType {
						Value=_stateCode,
					},
				},
				Location=new PDMPScript.Location {
					Name=_pdmpProperties.FacilityId,
					DEANumber=new PDMPScript.DEANumberType {
						Value=_pdmpProperties.Dea,
					},
					NPINumber=new PDMPScript.NPINumberType {
						Value=_pdmpProperties.PdmpProv.NationalProvID,
					},
				},
			};
			return requester;
		}

		public PDMPScript.PrescriptionSummaryRequestTypePrescriptionRequest MakePrescriptionRequest() {
			PDMPScript.SexCodeBaseType sex;
			switch(_pdmpProperties.PdmpPat.Gender) {
				case PatientGender.Female:
					sex=PDMPScript.SexCodeBaseType.F;
					break;
				case PatientGender.Male:
					sex=PDMPScript.SexCodeBaseType.M;
					break;
				case PatientGender.Unknown:
				default:
					sex=PDMPScript.SexCodeBaseType.U;
					break;
			}
			PDMPScript.PrescriptionSummaryRequestTypePrescriptionRequest prescrRequest=new PDMPScript.PrescriptionSummaryRequestTypePrescriptionRequest {
				Patient=new PDMPScript.RequestPatientType {
					Name=new PDMPScript.RequestPatientTypeName {
						First=new PDMPScript.LimitedStringType {
							Value=_pdmpProperties.PdmpPat.FName,
						},
						Last=new PDMPScript.LimitedStringType {
							Value=_pdmpProperties.PdmpPat.LName,
						},
					},
					Birthdate=_pdmpProperties.PdmpPat.Birthdate,
					SexCode=new PDMPScript.SexCodeType {
						Value=sex,
					},
					Items=new object[1] {
						new PDMPScript.AddressRequiredZipType(){ 
							Street=new PDMPScript.LimitedStringType[] {
								new PDMPScript.LimitedStringType {
									Value=_pdmpProperties.PdmpPat.Address,
								},
							},
							City=new PDMPScript.LimitedStringType {
								Value=_pdmpProperties.PdmpPat.City
							},
							ZipCode=new PDMPScript.ZipCodeType {
								Value=_pdmpProperties.PdmpPat.Zip
							},
						},
					},
					SSN=_pdmpProperties.PdmpPat.SSN,					
				},

			};
			return prescrRequest;
		}

		private string MakeBasicAuthHeader() {
			string basicAuthContent=System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_pdmpProperties.Username+":"+_pdmpProperties.Password));
			return "Basic "+basicAuthContent;
		}
	}
}
