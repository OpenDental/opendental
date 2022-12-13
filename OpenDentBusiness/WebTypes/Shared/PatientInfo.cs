using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using CodeBase;

namespace OpenDentBusiness {
	public class PatientInfo {
		public long PatNum;
		public string Name;
		public string FName;
		public string LName;
		public string Preferred;
		public DateTime Birthdate;
		public string Address;
		public string Address2;
		public string City;
		public string State;
		public string Zip;
		public string Email;
		public string Country;
		public ContactMethod ContactMethod;
		public PatientStatus Status;
		public long ClinicNum;
		//DateTime of patient's most recent appointment
		public DateTime DateTimeLastAppt;
		public long NextAptNum;
		//DateTime of patient's next scheduled appointment
		public DateTime DateTimeNextAppt;

		public static List<PatientInfo> GetListPatientInfos(DataTable table) {
			List<PatientInfo> listPatientInfos=new List<PatientInfo>();
			foreach(DataRow row in table.Rows) {
				PatientInfo patInfo=new PatientInfo();
				patInfo.PatNum=PIn.Long(row["PatNum"].ToString());
				patInfo.Name=PIn.String(row["LName"].ToString())+", "+PIn.String(row["FName"].ToString());
				patInfo.FName=PIn.String(row["FName"].ToString());
				patInfo.LName=PIn.String(row["lName"].ToString());
				patInfo.Preferred=PIn.String(row["Preferred"].ToString());
				patInfo.Birthdate=PIn.Date(row["Birthdate"].ToString());
				patInfo.Address=PIn.String(row["Address"].ToString());
				patInfo.Address2=PIn.String(row["Address2"].ToString());
				patInfo.City=PIn.String(row["City"].ToString());
				patInfo.State=PIn.String(row["State"].ToString());
				patInfo.Zip=PIn.String(row["Zip"].ToString());
				patInfo.Country=PIn.String(row["Country"].ToString());
				patInfo.Email=PIn.String(row["Email"].ToString());
				patInfo.Status=PIn.Enum<PatientStatus>(row["PatStatus"].ToString());
				patInfo.ContactMethod=PIn.Enum<ContactMethod>(row["PreferContactMethod"].ToString());
				patInfo.DateTimeLastAppt=PIn.Date(row["DateTimeLastApt"].ToString());
				patInfo.DateTimeNextAppt=PIn.Date(row["DateTimeNextApt"].ToString());
				patInfo.NextAptNum=PIn.Long(row["NextAptNum"].ToString(),false);
				patInfo.ClinicNum=PIn.Long(row["ClinicNum"].ToString(),false);
				listPatientInfos.AddRange(CreateListForEmails(patInfo));//add a row for each email the patient has
			}
			return listPatientInfos;
		}

		///<summary>Returns a list for each unique email the patient has, or just a list containing the patient if no unique email addresses.</summary>
		private static List<PatientInfo> CreateListForEmails(PatientInfo patient) {
			List<PatientInfo> listPatInfos=(patient.Email??"")
				.Split(new string[] { ",",";" },StringSplitOptions.RemoveEmptyEntries)
				.Select(email => {
					PatientInfo patEmail=null;
					if(EmailAddresses.IsValidEmail(email,out MailAddress mailAddress)) {
						patEmail=GenericTools.DeepCopy<PatientInfo,PatientInfo>(patient);
						patEmail.Email=mailAddress.Address;
					}
					return patEmail;
				})
				.Where(x => x!=null)
				.DistinctBy(x => x.Email)
				.ToList();
			if(listPatInfos.IsNullOrEmpty()) {
				//Still include the patient even if they didn't have any valid email addresses.
				listPatInfos=new List<PatientInfo> { patient };
			}
			return listPatInfos;
		}
	}
}
