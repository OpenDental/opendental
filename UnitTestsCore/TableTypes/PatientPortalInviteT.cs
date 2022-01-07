using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class PatientPortalInviteT {

		///<summary>Returns nothing because this method utilizes InsertMany which does not correctly return the PK of the item(s) inserted.</summary>
		public static void CreateInvite(long patNum,long aptNum=0,long clinicNum=0,long emailMessageNum=0,
			AutoCommStatus emailSendStatus=AutoCommStatus.Undefined,string note="",string templateEmail="",string templateEmailSubj="",
			TimeSpan tSPrior=default(TimeSpan))
		{
			PatientPortalInvite patientPortalInvite=new PatientPortalInvite() {
				IsNew=true,
				AptNum=aptNum,
				ClinicNum=clinicNum,
				EmailMessageNum=emailMessageNum,
				EmailSendStatus=emailSendStatus,
				Note=note,
				PatNum=patNum,
				TemplateEmail=templateEmail,
				TemplateEmailSubj=templateEmailSubj,
				TSPrior=tSPrior,
			};
			//Insert was not an available method at the time that this method was created.  Not worth enhancing S class ATM.  Simply use InsertMany().
			PatientPortalInvites.InsertMany(new List<PatientPortalInvite>() { patientPortalInvite });
		}

		///<summary>Clears the table. Does not truncate so that primary keys are not re-used.</summary>
		public static void ClearPatientPortalInviteTable() {
			string command="DELETE FROM patientportalinvite WHERE PatientPortalInviteNum > 0";
			DataCore.NonQ(command);
		}

	}
}
