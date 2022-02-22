using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness.AutoComm {
	public class Byod : ApptLite {
		public string ShortGuid;
		public string ShortUrl;

		public Byod(Appointment appt,PatComm patComm,string shortGuid,string shortUrl) : base(appt,patComm,false) {
			ShortGuid=shortGuid;
			ShortUrl=shortUrl;
		}
	
		///<summary>Takes a list of Appointments, and PatComms; gets existing or registers generates a human readable 'Check In' message.</summary>
		public static string GetCheckInMsg(List<Appointment> listAppts,List<PatComm> listPatComms) {
			//First see if we have registered any BYOD short guids with HQ.
			List<EServiceShortGuid> listShortGuids=EServiceShortGuids.GetByFKey(EServiceShortGuidKeyType.eClipboardApptByod,listAppts.Select(x => x.AptNum)
				.ToList());
			List<Appointment> listNoShortGuid=listAppts.Where(x => !ListTools.In(x.AptNum,listShortGuids.Select(y => y.FKey))).ToList();
			if(!listNoShortGuid.IsNullOrEmpty()) {
				//We haven't registered eClipboard BYOD short guids for these appointments with HQ.  Do it now!  This could be slow as it's a webcall.
				listShortGuids.AddRange(EServiceShortGuids.GenerateShortGuid(listNoShortGuid,eServiceCode.EClipboard,EServiceShortGuidKeyType.eClipboardApptByod));
			}
			Appointment getApt(long aptNum) {
				return listAppts.FirstOrDefault(x => x.AptNum==aptNum);
			}
			PatComm getPatComm(long aptNum) {
				Appointment appt=getApt(aptNum);
				return listPatComms.FirstOrDefault(x => x.PatNum==appt?.PatNum);
			}
			Clinic getClinic(long clinicNum) {
				clinicNum=PrefC.HasClinicsEnabled ? clinicNum : 0;
				Clinic clinic=clinicNum==0 ? Clinics.GetPracticeAsClinicZero() : Clinics.GetClinic(clinicNum);
				return clinic??Clinics.GetPracticeAsClinicZero();//guarantees we won't have a null clinic.
			}
			string getTemplate(long clinicNum) {
				return ClinicPrefs.GetPrefValue(PrefName.EClipboardByodSmsTemplate,getClinic(clinicNum).ClinicNum);
			}
			//Combines EServiceShortGuids, Appointments, and PatComms into BYOD objects.
			List<Byod> listByods=listShortGuids.Select(guid => new Byod(getApt(guid.FKey),getPatComm(guid.FKey),guid.ShortGuid,guid.ShortURL))
				.OrderBy(x => x.DateTimeEvent)
				.ThenBy(x => x.PrimaryKey)
				.ToList();
			TagReplacer tr=new ByodTagReplacer();
			//Creates a newline delimited string, each line is a tag replaced template representing a BYOD.
			return string.Join("\n",listByods.Select(byod => tr.ReplaceTags(getTemplate(byod.ClinicNum),byod,getClinic(byod.ClinicNum),false)));
		}

		///<summary>Get a safe clinicNum based on if clinics are on/off.</summary>
		private static long GetClinicNum(long clinicNum) {
			return PrefC.HasClinicsEnabled ? clinicNum : 0;
		}

		///<summary>Returns true if the given clinicNum is signed up and setup for BYOD</summary>
		public static bool IsSetup(long clinicNum,out string err) {
			StringBuilder errBldr=new StringBuilder();
			clinicNum=GetClinicNum(clinicNum);
			if(!MobileAppDevices.IsClinicSignedUpForEClipboard(clinicNum)) {//Clinic is using eClipboard.
				errBldr.AppendLine(Lans.g(nameof(Byod),"Office is not signed up to use eClipboard."));
			}
			if(!ClinicPrefs.GetBool(PrefName.EClipboardEnableByodSms,clinicNum)) {//Clinic has BYOD for sms enabled.
				errBldr.AppendLine(Lans.g(nameof(Byod),"eClipboard is not setup to allow 'Check-In' links via text message."));
			}
			err=errBldr.ToString();
			return string.IsNullOrWhiteSpace(err);
		}

		///<summary>Returns true if the given confirmed status and clinic are eligible to enable BYOD.</summary>
		public static bool IsEnabledForConfirmed(long confirmed,long clinicNum,out string err) {
			StringBuilder errBldr=new StringBuilder();
			if(IsSetup(clinicNum,out err)) {
				List<long> listDefNums=PrefC.GetString(PrefName.ApptConfirmByodEnabled).Split(',').Select(x => PIn.Long(x)).ToList();
				listDefNums.Remove(0);
				if(!listDefNums.Contains(confirmed)) {//Appointment is marked 'Arrived')
					errBldr.AppendLine(Lans.g(nameof(Byod),"Appointment is not marked as a suitable confirmation status."));
				}
				if(!SmsPhones.IsIntegratedTextingEnabled() && !Programs.IsEnabled(ProgramName.CallFire)) {
					errBldr.AppendLine(Lans.g(nameof(Byod),"Texting is not enabled."));
				}
				err=errBldr.ToString();
			}
			return string.IsNullOrWhiteSpace(err);
		}
	}
}
