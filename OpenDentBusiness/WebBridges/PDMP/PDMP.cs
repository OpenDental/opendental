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
using OpenDentBusiness.UI;
using System.Windows;

namespace OpenDentBusiness {
	public class PDMP {
		public string Message;
		public string Url;

		/// <summary>List of programs that can access PDMP bridge</summary>
		private static List<string> _listPDMPProgNames=new List<string>() {
			ProgramName.PDMP.ToString(),
			ProgramName.Appriss.ToString(),
		};

		///<summary>Returns a PDMP object after validating necessary program properties and going through API calls</summary>
		public static PDMP SendData(Program programCur,Patient pat,Provider prov) {
			PDMP pdmp=new PDMP();
			StringBuilder sbErrors=new StringBuilder();
			if(!programCur.Enabled) {
				sbErrors.AppendLine(programCur.ProgName+Lans.g("PDMP"," must be enabled in Program Links."));
				throw new Exception(sbErrors.ToString());
			}
			if(prov==null) {
				sbErrors.AppendLine(Lans.g("PDMP","Logged in user must be associated with a provider to access this feature."));
				throw new ApplicationException(sbErrors.ToString());
			}
			if(pat==null) {
				sbErrors.AppendLine(Lans.g("PDMP","Please select a patient."));
				throw new ApplicationException(sbErrors.ToString());
			}
			string strDeaNum=ProviderClinics.GetDEANum(prov.ProvNum,Clinics.ClinicNum);//If no result found, retries using clinicNum=0.
			if(string.IsNullOrWhiteSpace(strDeaNum)) {
				sbErrors.AppendLine(Lans.g("PDMP","User's provider does not have a DEA number."));
			}
			string stateWhereLicensed=ProviderClinics.GetStateWhereLicensed(prov.ProvNum,Clinics.ClinicNum);
			if(string.IsNullOrWhiteSpace(stateWhereLicensed)) {
				sbErrors.AppendLine(Lans.g("PDMP","User's provider is not licensed for any state."));
			}
			PdmpProperty propertyVals=new PdmpProperty() {
				PdmpProv=prov,
				PdmpPat=pat,
				StateAbbr=stateWhereLicensed,
				Dea=strDeaNum,
			};
			try {
				propertyVals.LoadPropertiesForProgram(programCur,Clinics.ClinicNum);
				propertyVals.Validate(programCur);
			}
			catch(Exception ex) {
				sbErrors.AppendLine(ex.Message);
			}
			//Validation failed.  We gave the user as much information to fix as possible.
			if(!string.IsNullOrWhiteSpace(sbErrors.ToString())) {
				throw new ApplicationException(sbErrors.ToString());
			}
			//Validation passed and we can now call the PDMP API.
			//We have multiple PDMP program links and there's overlap to which states they cover so we'll use different classes to manage the bridges
			if(Enum.TryParse(programCur.ProgName,out ProgramName programName)) {
				switch(programName) {
					case ProgramName.PDMP:
						PDMPLogicoy pdmpLogicoy=new PDMPLogicoy(propertyVals);
						pdmp.Url=pdmpLogicoy.GetURL();
						break;
					case ProgramName.Appriss:
						PDMPAppriss pdmpAppriss=new PDMPAppriss(propertyVals);
						pdmpAppriss.DownloadData();
						pdmp.Url=pdmpAppriss.Url;
						pdmp.Message=pdmpAppriss.Response;
						break;
					default:
						throw new Exception(Lans.g("PDMP","PDMP program link has not been implemented."));
				}
			}
			else {
				throw new Exception(Lans.g("PDMP","Could not parse PDMP program name."));
			}
			return pdmp;
		}

		///<summary>Removes program properties that users don't need to see. Namely, the url that HQ manages.</summary>
		public static List<ProgramProperty> FilterAndSortProperties(Program progCur,List<ProgramProperty> listProps) {
			if(!ListTools.In(progCur.ProgName,_listPDMPProgNames)) {
				return listProps;//Program isn't PDMP so we don't care
			}
			List<ProgramProperty> retList=new List<ProgramProperty>();
			ProgramProperty progProp=listProps.Where(x=>x.PropertyDesc==PdmpProperty.PdmpProvLicenseField).FirstOrDefault();//add license type first
			if(progProp!=null) {
				retList.Add(progProp);
			}
			if(Enum.TryParse(progCur.ProgName,out ProgramName programName)) {
				switch(programName) {
					case ProgramName.PDMP:
						retList.AddRange(listProps.Where(x=>!x.PropertyDesc.Contains("Url")//no need for visible url for user
							&& x.PropertyDesc!=PdmpProperty.PdmpProvLicenseField)//already added license above
							.OrderBy(x=>x.PropertyDesc));//order by state name		
						break;
					case ProgramName.Appriss:
						retList.AddRange(listProps.Where(x=>!x.PropertyDesc.Contains("Url")//no need for visible url for user
							&& !x.PropertyDesc.Contains("Client")//Appriss has a client authorization key and a client password only needed for us to access bridge
							&& x.PropertyDesc!=PdmpProperty.PdmpProvLicenseField));//already added above
						break;
					default:
						throw new ApplicationException(Lans.g("PDMP","Could not parse program name."));
				}
			}
			return retList;
		}
	}
}
