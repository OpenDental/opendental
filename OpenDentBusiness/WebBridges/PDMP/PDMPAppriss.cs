using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using ApprissScript;
using CodeBase;
using Health.Direct.Common.Extensions;
using Newtonsoft.Json;
using static OpenDentBusiness.PDMP;

namespace OpenDentBusiness {
	public class PDMPAppriss {
		private PdmpProperty _pdmpProperties;
		private USStateCodeType  _stateCode;
		private SexCodeType _patGender;
		public string Url;
		public string Response;

		public PDMPAppriss(PdmpProperty props) {
			_pdmpProperties=props;
			Enum.TryParse(_pdmpProperties.StateAbbr,out _stateCode);
			_patGender=GetPatGender(_pdmpProperties.PdmpPat);
		}
		
		///<summary>Returns ApprissScript.SexCodeType. Defaults to 'Unknown'</summary>
		private SexCodeType GetPatGender(Patient pat) {
			SexCodeType gender=SexCodeType.U;
			if(pat.Gender==PatientGender.Male) {
				gender=SexCodeType.M;
			}
			else if(pat.Gender==PatientGender.Female) {
				gender=SexCodeType.F;
			}
			return gender;	
		}

		///<summary>Download method that will parse narx scores and url for use. Throws exceptions</summary>
		public void DownloadData() {
			PatientRequestType request=MakeRequester();
			request.PrescriptionRequest=MakePrescriptionRequest();
			ApprissAuth auth=new ApprissAuth(ApprissAuth.GetEpoch(DateTime.Now),ApprissAuth.GetNonce());
			PatientResponseType patientResponse=Request(Serialize(request),new PatientResponseType(),auth,_pdmpProperties.Url);
			if(patientResponse.Item is ReportType report) {
				Url=GetURL(report);
				Response=GetResponse(report,Url);
				return;
			}
			string errMsg="";
			if(patientResponse.Item is PatientResponseTypeError error) {
				errMsg=GetErrorResponse(error.Message,error.Details,error.Source);
			}
			else if(patientResponse.Response!=null) {
				ResponseType[] response=patientResponse.Response;
				for(int i = 0;i<response.Length;i++) {
					if(response[i].Item is ResponseTypeDisallowed dis) {
						errMsg=GetErrorResponse(dis.Message,dis.Details,dis.Source);
					}
					else if(response[i].Item is ResponseTypeError err) {
						errMsg=GetErrorResponse(err.Message,err.Details,err.Source);
					}
				}
			}
			if(!string.IsNullOrWhiteSpace(errMsg)) {
				throw new ApplicationException(errMsg);
			}
			throw new ApplicationException(Lans.g("Appriss","An error occurred.\n")+JsonConvert.SerializeObject(patientResponse));
		}

		private string GetErrorResponse(string message, string[] details, ReportResponseTypeErrorSource source) {
			StringBuilder strErr=new StringBuilder();
			if(message!=null) {
				strErr.AppendLine(message);
			}
			if(details!=null) {
				strErr.AppendLine(Lans.g("Appriss","Details:")+" "+string.Join("\n",details));
			}
			if(!string.IsNullOrWhiteSpace(strErr.ToString())) {
				strErr.AppendLine(Lans.g("Appriss","Source:")+" "+source.ToString());
			}
			return strErr.ToString();
		}

		///<summary>Creates response for pop up to display before user can continue on to the returned link. The hierarchy of perferred responses is as follows:
		///	1. Narx Scores if present, plus any messages included in the report
		///	2. The message in the report. Testing shows that if multiple matches are found for a patient then it will let the user know that this the case and they will need to log into Appriss directly.
		///	3. A resonse indicating that no patients have been found in the Appriss database that match the selected patients.
		///	Note: This method is public for testing purposes only
		///</summary>
		public string GetResponse(ReportType report,string url) {
			string strFurtherActionNeeded=Lans.g("Appriss","You will need to log into your Appriss account to search for the selected patient.");
			StringBuilder retVal=new StringBuilder();
			retVal.AppendLine(Lans.g("Appriss","Risk assessment for ")+_pdmpProperties.PdmpPat.LName+", "+_pdmpProperties.PdmpPat.FName);
			if(report.NarxScores!=null) {
				NarxScoreType[] narxMessage=report.NarxScores;
				for(int i=0;i<narxMessage.Length;i++) {
					retVal.AppendLine(narxMessage[i].ScoreType+": "+narxMessage[i].ScoreValue);
				}
				if(!report.Message.IsNullOrEmpty()) {
					retVal.AppendLine(report.Message);
				}
				else {
					retVal.AppendLine(Lans.g("Appriss","Some states do not require the full PDMP report to be viewed."));
				}
			}
			else if (!report.Message.IsNullOrEmpty()) {
				retVal.AppendLine(report.Message);
				retVal.AppendLine(strFurtherActionNeeded);
			}
			else {
				retVal.AppendLine(Lans.g("Appriss","No matches found for the selected patient."));
				retVal.AppendLine(strFurtherActionNeeded);
			}
			if(!string.IsNullOrEmpty(url)) {
				retVal.AppendLine(Lans.g("Appriss","Click 'OK' to continue on to the full report. Clicking 'Cancel' will result in an audit log not being created."));
			}
			return retVal.ToString();
		}

		///<summary>Submits a request to the appropriate API and returns a string containing the URL supplied by the API.  Throws exceptions.</summary>
		private string GetURL(ReportType report) {
			ApprissAuth auth=new ApprissAuth(ApprissAuth.GetEpoch(DateTime.Now),ApprissAuth.GetNonce());
			ReportRequestType reportRequest=MakeReportRequest();
			ReportResponseType reportResponse=Request(Serialize(reportRequest),new ReportResponseType(),auth,report.ReportRequestURLs.ViewableReport.Value);
			//Items array from report response could return several different items. From the documentation it looks like only one would show up. 
			//The url will show up as a string and any other response is it's own type in the Appriss script that indicates some sort of error.
			//For now we'll just throw an exception if we get anything else other than a string. 
			for(int i = 0;i<reportResponse.Items.Length;i++) {
				if(reportResponse.Items[i] is string url) {
					return url;
				}
			}
			string errMsg="";
			for(int i = 0;i<reportResponse.Items.Length;i++) {//we didn't find a url so we try, try again
				if(errMsg=="" && reportResponse.Items[i] is ReportResponseTypeError err) {
					errMsg=GetErrorResponse(err.Message,err.Details,err.Source);
				}
				else if(errMsg=="" && reportResponse.Items[i] is ReportResponseTypeDisallowed dis) {
					errMsg=GetErrorResponse(dis.Message,dis.Details,dis.Source);
				}
			}
			if(!string.IsNullOrWhiteSpace(errMsg)) {
				throw new ApplicationException(errMsg);
			}
			throw new ApplicationException(Lans.g("Appriss","An error occurred.\n")+JsonConvert.SerializeObject(reportResponse));
		}

		#region Requesters
		///<summary>Method is public for testing purposes only.</summary>
		public PatientRequestTypePrescriptionRequest MakePrescriptionRequest() {
			PatientRequestTypePrescriptionRequest prescrRequest=new PatientRequestTypePrescriptionRequest {
				Patient=new ApprissScript.PatientType {
					Name=new PatientTypeName {
						First=_pdmpProperties.PdmpPat.FName,
						Middle=_pdmpProperties.PdmpPat.MiddleI,
						Last=_pdmpProperties.PdmpPat.LName,
						},
					Birthdate=_pdmpProperties.PdmpPat.Birthdate,
					SexCode=_patGender,
					Items=new object[1] {
						new AddressRequiredZipType(){ 
							Street=new string[] {
								_pdmpProperties.PdmpPat.Address,
							},
							City=_pdmpProperties.PdmpPat.City,
							StateCodeSpecified=true,//This value needed to be set in order for the state code to show up in the prescription request
							StateCode=_stateCode,
							ZipCode=_pdmpProperties.PdmpPat.Zip
						},
					},
				},
			};
			return prescrRequest;
		}

		private ReportRequestType MakeReportRequest() {
			ReportRequestType reportRequest=new ReportRequestType();
			reportRequest.Requester=new ReportRequestTypeRequester {
				Provider=new ProviderTypeForReports {
					Role=ApprissScript.RoleType.Dentist,
					FirstName=_pdmpProperties.PdmpProv.FName,
					LastName=_pdmpProperties.PdmpProv.LName,
					NPINumber=_pdmpProperties.PdmpProv.NationalProvID,
					DEANumber=_pdmpProperties.Dea,
					ProfessionalLicenseNumber=new ProfessionalLicenseNumberType {
						Type=_pdmpProperties.ProvLicenseType,
						Value=_pdmpProperties.ProvLicenseNum,
						StateCode=_stateCode,
					}
				},
				Location=new ApprissScript.LocationType {
					Name=_pdmpProperties.FacilityId,
					DEANumber=_pdmpProperties.Dea,
					NPINumber=_pdmpProperties.PdmpProv.NationalProvID,
					Address=new LocationTypeAddress {
						Street=new string[]{ _pdmpProperties.PdmpPat.Address },
						City=_pdmpProperties.PdmpPat.City,
						StateCode=_stateCode,
						ZipCode=_pdmpProperties.PdmpPat.Zip
					}
				}
			};
			return reportRequest;
		}
		#endregion

		///<summary>Creates an X509 Certificate from certificate generated via documentation. These certificates will expire 5 years from their creation date.</summary>
		private X509Certificate2 GetCertificate() {
			X509Certificate2 pKey=new X509Certificate2();
			try {
				CDT.Class1.Decrypt(_pdmpProperties.ApprissClientKey,out string clientKey);
				CDT.Class1.Decrypt(_pdmpProperties.ApprissClientPassword,out string clientPassword);
				byte[] clientKeyBytes=Convert.FromBase64String(clientKey);
				pKey.Import(clientKeyBytes,clientPassword,X509KeyStorageFlags.MachineKeySet);
			}
			catch (Exception ex) {
				throw new Exception(Lans.g("Appriss","An error occurred while creating the security certificate.")+"\n"+ex.Message);
			}
			return pKey;
		}

		///<summary>Throws exception if the response from the server returned an http code of 300 or greater.</summary>
		private T Request<T>(string body,T responseType,ApprissAuth auth,string url) {
			using(WebClient client=new WebClient()) {
				HttpWebRequest req=WebRequest.Create(url) as HttpWebRequest;
				req.Method="POST";
				req.Accept="application/xml";
				req.Headers["X-Auth-PasswordDigest"]=auth.GetPasswordDigest(_pdmpProperties.Password);
				req.Headers["X-Auth-Nonce"]=auth.Nonce;
				req.Headers["X-Auth-Timestamp"]=auth.EpochTime.ToString();
				req.Headers["X-Auth-Username"]=_pdmpProperties.Username;
				if(!Introspection.IsTestingMode) {
					req.ClientCertificates.Add(GetCertificate());
				}
				try {
					UTF8Encoding encoding=new UTF8Encoding();
					byte[] bytes=encoding.GetBytes(body);
					using(Stream streamOut=req.GetRequestStream()) {
						streamOut.Write(bytes,0,bytes.Length);
						streamOut.Close();
					}
					string strResponse="";
					using(WebResponse response=req.GetResponse()) {
						using(StreamReader readStream=new StreamReader(response.GetResponseStream(),Encoding.UTF8)) {
							strResponse=readStream.ReadToEnd();
							readStream.Close();
						}
						if(ODBuild.IsDebug()) {
							if((typeof(T)==typeof(string))) {//If user wants the entire json response as a string
								return (T)Convert.ChangeType(response,typeof(T));
							}
							Console.WriteLine(response);
						}
					}
					return Deserialize<T>(strResponse,responseType);
				}
				catch(WebException wex) {
					string res="";
					using(var sr=new StreamReader(((HttpWebResponse)wex.Response).GetResponseStream())) {
						res=sr.ReadToEnd();
						sr.Close();
					}
					if(string.IsNullOrWhiteSpace(res)) {
						//The response didn't contain a body. Unlikely that this would happen but to be safe we can still throw if the error is null
						throw new ODException(Lans.g("Appriss","Unable to connect to Appriss."));
					}
					string errorMsg=wex.Message+"\r\nRaw response:\r\n"+res;
					throw new Exception(errorMsg,wex);//If we got this far and haven't rethrown, simply throw the entire exception.
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
					//Keeping this in for Appriss in case they suffer the same fate
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

		///<summary>Method is public for testing purposes only</summary>
		public PatientRequestType MakeRequester() {
			//There is no documentation identifying what the use case of "RoleType" is.  Setting to Dentist for now seems like the safe choice.
			ApprissScript.RoleType role=ApprissScript.RoleType.Dentist;
			PatientRequestType requester=new PatientRequestType {
				Requester=new RequesterType {
					Provider=new ProviderType {
						Role=role,
						FirstName=_pdmpProperties.PdmpProv.FName,
						LastName=_pdmpProperties.PdmpProv.LName,
						DEANumber=_pdmpProperties.Dea,
						NPINumber=_pdmpProperties.PdmpProv.NationalProvID,
						ProfessionalLicenseNumber=new ProfessionalLicenseNumberType {
							Type=_pdmpProperties.ProvLicenseType,
							Value=_pdmpProperties.ProvLicenseNum,
							StateCode=_stateCode
						},
					},
					Location=new ApprissScript.LocationType {
						Name=_pdmpProperties.FacilityId,
						DEANumber=_pdmpProperties.Dea,
						NPINumber=_pdmpProperties.PdmpProv.NationalProvID,
						Address=new LocationTypeAddress {
							StateCode=_stateCode,
						},
					},
				},
			};
			return requester;
		}
	}

	public partial class ApprissAuth {
		public readonly long EpochTime;
		public readonly string Nonce;

		public ApprissAuth(long epoch,string nonce) {
			EpochTime=epoch;
			Nonce=nonce;
		}

		///<summary>Documentation indicates that part of the password digest needs to be Epoch time in seconds</summary>
		public static long GetEpoch(DateTime dt) {
			return (long)dt.ToUniversalTime().Subtract(new DateTime(1970,1,1)).TotalSeconds;
		}

		///<summary>According to Appriss documentation, the nonce cannot have ":" in it as that would mess up the hashing</summary>
		public static string GetNonce() {
			return Guid.NewGuid().ToString().Replace(":","");
		}

		///<summary>Password digest is needed to get authorization to access site</summary>
		public string GetPasswordDigest(string password) {
			string strPlainText=$"{password}:{Nonce}:{EpochTime}";
			using(var sha256=SHA256.Create()) {
				return ToHex(sha256.ComputeHash(Encoding.Default.GetBytes(strPlainText)));
			}
		}

		///<summary>Appriss requires a hex encoded SHA hash</summary>
		private static string ToHex(byte[] bytes) {
			return string.Join("",bytes.Select(x => x.ToString("x2")));
    }
	}
}
