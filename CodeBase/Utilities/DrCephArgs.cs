using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace CodeBase {
	///<summary>Class used to carry arguments to pass into the DrCeph launch method.</summary>
	public class DrCephArgs {
		#region DrCeph fields
		public string ID;
		public string FName;
		public string LName;
		public string MiddleI;
		public string Address1;
		public string Address2;
		public string City;
		public string State;
		public string Zip;
		public string Phone;
		public string SSN;
		public string Sex;
		public string Race;
		public string AngleClass;
		public string Birthdate;
		public string RecordsDate;
		public string ReferringDr;
		public string TreatingDr;
		public string ResponsibleName;
		public string ResponsibleAddress1;
		public string ResponsibleAddress2;
		public string ResponsibleCity;
		public string ResponsibleState;
		public string ResponsibleZip;
		public string ResponsiblePhone;
		public string ResponsibleRelationship;
		public int TreatmentPhase;
		public string CephXRayLocation;
		public string PhotoFileLocation;
		public string XRayDate;
		#endregion
	}

	public class DrCephUtils {
		public static void Launch(DrCephArgs args) {
			VBbridges.DrCephNew.Launch(args.ID,args.FName,args.MiddleI,args.LName,args.Address1,args.Address2,args.City,args.State,args.Zip,args.Phone,args.SSN,args.Sex,args.Race,
				"",args.Birthdate,args.RecordsDate,args.ReferringDr,args.TreatingDr,args.ResponsibleName,args.ResponsibleAddress1,args.ResponsibleAddress2,args.ResponsibleCity,
				args.ResponsibleState,args.ResponsibleZip,args.ResponsiblePhone,args.ResponsibleRelationship,args.TreatmentPhase,args.CephXRayLocation,args.PhotoFileLocation,args.XRayDate);
		}

		public static void LaunchFromCloud(ODCloudClient.ODCloudClientData data) {
			try {
				DrCephArgs args=JsonConvert.DeserializeObject<DrCephArgs>(data.Args);
				Launch(args);
			}
			catch(Exception ex) {
				throw new ODException("An error occurred when launching Dr Ceph in web mode: ",ex);
			}
		}
	}
}
