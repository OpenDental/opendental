using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.AutoComm {
	///<summary>An object to store data used to send a PatientPortalInvite.</summary>
	public class PatPortalInviteLite:ApptLite {
		///<summary>The UserWeb and password info generated for this PatientPortalInvite.
		///Item1=UserWeb currently in db. Item2=New plain text password. Item3=New hashed password container.</summary>
		public Tuple<UserWeb,string,PasswordContainer> UserWeb;
		///<summary>FK to emailmessage.EmailMessageNum. The email that was sent for this invite.</summary>
		public long EmailMessageNum;
		///<summary>Any notes regarding this invite. Usually error messages.</summary>
		public string Note;

		public PatPortalInviteLite(ApptLite appt) {
			FieldInfo[] arrayFieldInfos=typeof(ApptLite).GetFields();
			foreach(FieldInfo aptField in arrayFieldInfos) {
				FieldInfo aptDelField=typeof(PatPortalInviteLite).GetField(aptField.Name);
				aptDelField.SetValue(this,aptField.GetValue(appt));
			}
		}
	}
}
