using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness{
	public class Promotions {
		///<summary>Returns a list of promotion analytics for the given date range.</summary>
		///<param name="clinicNum">The clinic to get analytics for. If negative, will return all regardless of clinicnum.</param>
		public static List<PromotionAnalytic> GetAnalytics(DateTime lowerBound,DateTime upperBound,long clinicNum=-1) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<PromotionAnalytic>>(MethodBase.GetCurrentMethod(),lowerBound,upperBound,clinicNum);
			}
			string query=$@"
				SELECT 
					promotionlog.PromotionStatus AS Status,
					COUNT(*) AS Count,
					promotion.*
				FROM promotion
				LEFT JOIN promotionlog
					ON promotion.PromotionNum = promotionlog.PromotionNum
				WHERE promotion.DateTimeCreated BETWEEN {POut.Date(lowerBound)} AND {POut.Date(upperBound)}
					{(clinicNum < 0 ? "" : $"AND promotion.ClinicNum = {clinicNum}")}
				GROUP BY promotion.PromotionNum,promotionlog.PromotionStatus";
			DataTable table=Db.GetTable(query);
			if(table.Rows.Count==0) {
				return new List<PromotionAnalytic>();
			}
			//Dictionary is most effecient as we have a row for every PromotionLogStatus per Promotion.
			Dictionary<long,PromotionAnalytic> dictionaryAnalytics=new Dictionary<long,PromotionAnalytic>();
			List<Promotion> listPromotions=PromotionCrud.TableToList(table);
			foreach(DataRow row in table.Rows) {
				long promotionNum=PIn.Long(row["PromotionNum"].ToString());
				if(!dictionaryAnalytics.TryGetValue(promotionNum,out PromotionAnalytic analytic)) {
					analytic=new PromotionAnalytic { 
						Promotion=listPromotions.FirstOrDefault(x => x.PromotionNum==promotionNum),
					};
					dictionaryAnalytics[promotionNum]=analytic;
				}
				PromotionLogStatus status=PIn.Enum<PromotionLogStatus>(PIn.Int(row["Status"].ToString()));
				//Save off the count for this status.
				analytic.DictionaryCounts[status]=PIn.Int(row["Count"].ToString());
			}
			return dictionaryAnalytics.Select(x => x.Value).ToList();
		}

		///<summary>Helper to validate emails being sent through Mass Email. A patient should have a destination for every valid email address unless
		///this is being sent from an automade mode in which case there should be 1 destination per patient total.</summary>
		public static List<MassEmailDestination> FilterPatsWithMultipleEmails(List<MassEmailDestination> listDestinations,PromotionType type) {
			List<MassEmailDestination> listSeparatedDestinations=new List<MassEmailDestination>();
			HashSet<long> hashSetPat=new HashSet<long>();//used to quickly look up if a pat already exists in the return list.
			foreach(MassEmailDestination destination in listDestinations) {
				if(string.IsNullOrEmpty(destination.ToAddress)) {
					continue;//no email address set, doesn't make sense to keep it in the list since nothing will be sent.
				}
				if(type==PromotionType.Birthday && hashSetPat.Contains(destination.PatNum)) {
					continue;//we only want to send 1 birthday email since it is automated and charged per email. 
				}
				List<string> listEmailAddresses=EmailAddresses.GetValidAddresses(destination.ToAddress);
				if(listEmailAddresses.IsNullOrEmpty()) {
					continue;
				}
				if(type==PromotionType.Birthday) {
					listEmailAddresses=listEmailAddresses.Take(1).ToList();//only send 1 email for birthdays per pat.
				}
				listSeparatedDestinations.AddRange(listEmailAddresses.Select(x => 
					new MassEmailDestination{PatNum=destination.PatNum,AptNum=destination.AptNum,ToAddress=x}
				));
				hashSetPat.Add(destination.PatNum);
			}
			return listSeparatedDestinations;
		}

		///<summary>Takes in the list of destinations and sees if we're sending multiple emails to the same email (i.e if a family shares an email and they're all on the list to 
		///get sent this mass email). Prioritizes the guarantor, then the one with the lowest PatNum. </summary>
		public static List<MassEmailDestination> FilterForDuplicates(List<MassEmailDestination> massEmailDestinations,Dictionary<long,Patient>dictGuarantors,PromotionType promotionType) {
			//If we have a birthday email going out, we have already filtered for this.
			if(promotionType==PromotionType.Birthday) {
				return massEmailDestinations;
			}
			List<MassEmailDestination> itemsToRemove=new List<MassEmailDestination>();
			//Grab all duplicates in listMassEmailDestinations to minimize looping. If there are no duplicates then we don't need to do any of the following logic
            Dictionary<long, List<MassEmailDestination>> dictDuplicateAddresses=massEmailDestinations
				.GroupBy(x=>x.ToAddress) //group by address to get duplicate addresses
				.Where(x=>x.Count()>1) //only worry about ones where duplicates exist
				.ToDictionary(x=>x.FirstOrDefault().PatNum,x=>x.ToList());//use the patnum as the key so we can find the guarantor
			foreach(long patNum in dictDuplicateAddresses.Keys) {
				List<MassEmailDestination> listDestsForAddress=dictDuplicateAddresses[patNum];
				//if we have an entry in the guarantor dictionary for this patient, we'll default to sending an email to them
				//otherwise use first one entered into database
				long patNumToKeep =listDestsForAddress.Where(x=>dictGuarantors.TryGetValue(x.PatNum,out _)).FirstOrDefault()?.PatNum??listDestsForAddress.Min(x => x.PatNum);
				itemsToRemove.AddRange(listDestsForAddress.Where(x=>x.PatNum!=patNumToKeep));
            }
			return massEmailDestinations.Except(itemsToRemove).ToList();
		}

		///<summary>Takes an email hosting template and a list of patients and sends the given email to them. If something goes wrong, returns false and
		///returns an error message. This potentially could take a long time. Handles filling replacements as well.</summary>
		public static MassEmailSendResult SendEmails(EmailHostingTemplate templateCur,List<MassEmailDestination> listDestinations,string senderName,string replyToAddress,
			string promotionName,PromotionType type,long clinicNum=-1,string senderAddress="",bool isVerificationBatch=false) 
		{
			if(string.IsNullOrWhiteSpace(promotionName)) {
				return new MassEmailSendResult(false,"A promotion name is required.",null,null,null);
			}
			if(listDestinations.IsNullOrEmpty()) {
				return new MassEmailSendResult(true,"",null,null,null);
			}
			if(clinicNum==-1) {
				clinicNum=Clinics.ClinicNum;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<MassEmailSendResult>(MethodBase.GetCurrentMethod(),templateCur,listDestinations,senderName,replyToAddress,promotionName,type,clinicNum,senderAddress,isVerificationBatch);
			}
			List<long> listPatNums=new List<long>();
			List<long> listAptNums=new List<long>();
			Dictionary<string,MassEmailDestination> dictUniquePatEmail=new Dictionary<string, MassEmailDestination>();
			listDestinations=FilterPatsWithMultipleEmails(listDestinations,type);
			foreach(MassEmailDestination destination in listDestinations) {
				listPatNums.Add(destination.PatNum);
				if(destination.AptNum > 0) {
					listAptNums.Add(destination.AptNum);
				}
			}
			#region GetPatients
			//A dictionary of patnums to all the patients.
			Dictionary<long,Patient> dictPatients=Patients.GetMultPats(listPatNums)
				.ToDictionary(x => x.PatNum,x => x);
			#endregion
			#region GetGuarantors
			List<long> listGuarantorNumsToQuery=new List<long>();
			//A dictionary of patnums to patients. These are all the guarantors of the patients being sent to.
			Dictionary<long,Patient> dictGuarantors=new Dictionary<long,Patient>();
			foreach(KeyValuePair<long,Patient> pat in dictPatients) {
				//Either we already have the guarantor in the current dictionary or we will have to query for it.
				if(dictPatients.TryGetValue(pat.Value.Guarantor, out Patient guarantor)) {
					dictGuarantors[pat.Key]=guarantor;
				}
				else {
					listGuarantorNumsToQuery.Add(pat.Value.Guarantor);
				}
			}
			Patient[] guarnators=Patients.GetMultPats(listGuarantorNumsToQuery);
			foreach(Patient pat in guarnators) {
				dictGuarantors[pat.PatNum]=pat;
			}
			#endregion
			#region GetAppointments
			List<Appointment> listAppointments=Appointments.GetMultApts(listAptNums);
			Dictionary<long,Appointment> dictAppointments=new Dictionary<long,Appointment>();
			foreach(Appointment appt in listAppointments) {
				dictAppointments[appt.AptNum]=appt;
			}
			#endregion
			List<string> listSubjectReplacements=EmailHostingTemplates.GetListReplacements(templateCur.Subject).Distinct().ToList();
			List<string> listBodyReplacements=EmailHostingTemplates.GetListReplacements(templateCur.BodyHTML).Distinct().ToList();
			List<TemplateDestination> listTemplateDestinations=new List<TemplateDestination>();
			int countDestinationsBeforeFiltering=listDestinations.Count;
			List<MassEmailDestination> listFilteredOutDestinations=listDestinations;
			listDestinations=FilterForDuplicates(listDestinations,dictGuarantors,type);
			listFilteredOutDestinations=listFilteredOutDestinations.FindAll(x=>!listDestinations.Contains(x));
			int countDestinationsAfterFiltering=listDestinations.Count;
			//Dictionary of patnum -> the replaced subject and body. Used afterwords to save the emails as EmailMessages.
			Dictionary<long,(string subject,string body)> dictReplaced=new Dictionary<long,(string,string)>();
			//if there are multiple patients with the same email address, last in will win.
			foreach(MassEmailDestination dest in listDestinations) {
				Patient pat=dictPatients[dest.PatNum];
				Patient guarantor=dictGuarantors[pat.Guarantor];
				dictAppointments.TryGetValue(dest.AptNum,out Appointment apt);
				Clinic clinicPat=Clinics.GetClinic(pat.ClinicNum);
				string guid=Guid.NewGuid().ToString();
				dictUniquePatEmail[guid]=dest;
				string GetReplacementValue(string replacementKey) {
					string bracketReplacement="["+replacementKey+"]";
					string result=Patients.ReplacePatient(bracketReplacement,pat);
					if(result==bracketReplacement) {
						result=Patients.ReplaceGuarantor(bracketReplacement,guarantor);
					}
					if(result==bracketReplacement) {
						result=ReplaceTags.ReplaceMisc(bracketReplacement);
					}
					if(result==bracketReplacement) {
						result=ReplaceTags.ReplaceUser(bracketReplacement,Security.CurUser);
					}
					if(result==bracketReplacement) {
						result=Appointments.ReplaceAppointment(bracketReplacement,apt);
					}
					if(result==bracketReplacement) {
						result=Clinics.ReplaceOffice(bracketReplacement,clinicPat);
					}
					return result;
				}
				string PerformAllReplacements(string message) {
					message=message.Replace("[{[{ ","[").Replace(" }]}]","]")
						.Replace("[{[{","[").Replace("}]}]","]");
					message=Patients.ReplacePatient(message,pat);
					message=Patients.ReplaceGuarantor(message,guarantor);
					message=ReplaceTags.ReplaceMisc(message);
					message=ReplaceTags.ReplaceUser(message,Security.CurUser);
					message=Appointments.ReplaceAppointment(message,apt);
					message=Clinics.ReplaceOffice(message,clinicPat);
					return message;
				}
				SerializableDictionary<string,string> subjectReplacements=new SerializableDictionary<string,string>();
				foreach(string replacement in listSubjectReplacements) {
					subjectReplacements[replacement]=GetReplacementValue(replacement);
				}
				SerializableDictionary<string,string> bodyReplacements=new SerializableDictionary<string,string>();
				foreach(string replacement in listBodyReplacements) {
					bodyReplacements[replacement]=GetReplacementValue(replacement);
				}
				listTemplateDestinations.Add(new TemplateDestination {
					UniqueID=guid,
					Destination=dest.ToAddress,
					SubjectReplacements=subjectReplacements,
					BodyReplacements=bodyReplacements,
				});
				//This has the potential to be overwritten when the same pat exists in the destination list more than once. This is expected to be the same information so overwriting is okay.
				dictReplaced[dest.PatNum]=(PerformAllReplacements(templateCur.Subject)
					,PerformAllReplacements(string.IsNullOrWhiteSpace(templateCur.BodyHTML) ? templateCur.BodyPlainText : templateCur.BodyHTML));
			}
			IAccountApi api=EmailHostingTemplates.GetAccountApi(clinicNum);
			SendMassEmailResponse response;
			try {
				response=api.SendMassEmail(new SendMassEmailRequest {
					TemplateNum=templateCur.TemplateId,
					SenderName=senderName,
					ReplyToAddress=replyToAddress,
					FromEmailAddress=senderAddress,
					ListDestinations=listTemplateDestinations,
					ExternalTag=new ExternalTag() { Type=type.ToString() },
				});
			}
			catch(Exception e) {
				return new MassEmailSendResult(false,e.Message,null,null,null);
			}
			if(isVerificationBatch) {
				promotionName=Lans.g("Promotions","VERIFICATION")+'-'+promotionName;
			}
			Promotion promotion=new Promotion {
				ClinicNum=clinicNum,
				DateTimeCreated=DateTime_.Now,
				PromotionName=promotionName,
				TypePromotion=type,
			};
			Insert(promotion);
			List<PromotionLog> listLogs=new List<PromotionLog>();
			foreach(KeyValuePair<string,long> uniqueIdPair in response.DictionaryUniqueIDToHostingID) {
				string guid=uniqueIdPair.Key;
				long patNum=dictUniquePatEmail[guid].PatNum;
				(string subject,string body)=dictReplaced[patNum];
				if(isVerificationBatch) {
					patNum=0;//Don't assign a test email to a patient since it isn't sent to the patient's email address.
				}
				EmailMessage message=new EmailMessage {
					BodyText=body,
					HideIn=HideInFlags.ApptEdit,
					HtmlType=templateCur.EmailTemplateType,
					MsgDateTime=DateTime_.Now,
					PatNum=patNum,
					PatNumSubj=patNum,
					RecipientAddress=dictUniquePatEmail[guid].ToAddress,
					SentOrReceived=EmailSentOrReceived.Sent,
					FromAddress=replyToAddress,
					ToAddress=dictUniquePatEmail[guid].ToAddress,
					UserNum=Security.CurUser?.UserNum??0,
					Subject=subject,
					MsgType=EmailMessageSource.Promotion,
				};
				//Insert so we have the primary key available.
				EmailMessages.Insert(message);
				listLogs.Add(new PromotionLog {
					EmailHostingFK=uniqueIdPair.Value,
					PatNum=patNum,
					ClinicNum=clinicNum,
					PromotionNum=promotion.PromotionNum,
					PromotionStatus=PromotionLogStatus.Pending,
					SendStatus=AutoCommStatus.SentAwaitingReceipt,
					MessageFk=message.EmailMessageNum,
					MessageType=CommType.Email,
				});
			}
			PromotionLogs.InsertMany(listLogs);
			string resultDesc="";
			if(countDestinationsAfterFiltering!=countDestinationsBeforeFiltering) {
				resultDesc=POut.Int(Math.Max(0,countDestinationsBeforeFiltering-countDestinationsAfterFiltering));
			}
			List<MassEmailDestinationFailed> listMassEmailDestinationFaileds=response.ListTemplateDestinationsRemoved.Select(x=>new MassEmailDestinationFailed() { PatNum=listDestinations.FirstOrDefault(y=>y.ToAddress==x.Destination)?.PatNum??-1,ToAddress=x.Destination,Description="Email Freq. Lim."}).ToList();
			listMassEmailDestinationFaileds.AddRange(listFilteredOutDestinations.Select(x=>new MassEmailDestinationFailed(){ PatNum=x.PatNum,ToAddress=x.ToAddress,Description="Duplicate email address."}));
			return new MassEmailSendResult(true,resultDesc,promotion,listLogs,listMassEmailDestinationFaileds);
		}

		public static long Insert(Promotion promotion) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				promotion.PromotionNum=Meth.GetLong(MethodBase.GetCurrentMethod(),promotion);
				return promotion.PromotionNum;
			}
			return PromotionCrud.Insert(promotion);
		}
	}

	public class MassEmailSendResult {
		public bool IsSuccess;
		public string ResultDescription;
		public Promotion Promotion;
		public List<PromotionLog> ListPromotionLogs;
		public List<MassEmailDestinationFailed> ListTemplateDestinationsUnableToSend;

		public MassEmailSendResult(bool isSuccess,string resultDesc,Promotion promotion,List<PromotionLog> listPromotionLogs,List<MassEmailDestinationFailed> listTemplateDestinationsUnableToSend) {
			IsSuccess=isSuccess;
			ResultDescription=resultDesc;
			Promotion=promotion;
			ListPromotionLogs=listPromotionLogs;
			ListTemplateDestinationsUnableToSend=listTemplateDestinationsUnableToSend;
		}
	}

	///<summary>Represents analytics for a single promotion.</summary>
	[Serializable]
	public class PromotionAnalytic {
		///<summary>The actual row in the database.</summary>
		public Promotion Promotion { get; set; }
		///<summary>A dictionary of promotion log statuses with the total number of promotion logs with that status.</summary>
		public SerializableDictionary<PromotionLogStatus,int> DictionaryCounts { get; set; }=new SerializableDictionary<PromotionLogStatus,int>();
	}

	[Serializable]
	public class MassEmailDestination {
		public long PatNum { get; set; }
		///<summary>The appointment to be used for appointment related replacement tags. Can be 0.</summary>
		public long AptNum { get; set; }
		/// <summary>Email address that we are sending the message to. </summary>
		public string ToAddress { get; set; }
	}

	public class MassEmailDestinationFailed {
		public long PatNum;
		public string ToAddress;
		public string Description;
	}
 }