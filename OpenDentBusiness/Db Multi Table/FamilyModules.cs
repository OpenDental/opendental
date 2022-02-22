using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	public class FamilyModules {

		///<summary>Gets the data necessary to load the Family Module.</summary>
		public static LoadData GetLoadData(long patNum,bool doCreateSecLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<LoadData>(MethodBase.GetCurrentMethod(),patNum,doCreateSecLog);
			}
			LoadData data=new LoadData();
			data.Fam=Patients.GetFamily(patNum);
			data.Pat=data.Fam.GetPatient(patNum);
			data.ListPatPlans=PatPlans.Refresh(patNum);
			if(!PatPlans.IsPatPlanListValid(data.ListPatPlans)) {//PatPlans had invalid references and need to be refreshed.
				data.ListPatPlans=PatPlans.Refresh(patNum);
			}
			data.PatNote=PatientNotes.Refresh(patNum,data.Pat.Guarantor);
			data.DiscountPlanSub=DiscountPlanSubs.GetSubForPat(patNum);
			data.ListInsSubs=InsSubs.RefreshForFam(data.Fam);
			data.ListInsPlans=InsPlans.RefreshForSubList(data.ListInsSubs);
			data.ListBenefits=Benefits.Refresh(data.ListPatPlans,data.ListInsSubs);
			data.ListRecalls=Recalls.GetList(data.Fam.ListPats.Select(x => x.PatNum).ToList());
			data.ArrPatFields=PatFields.Refresh(patNum);
			data.SuperFamilyMembers=Patients.GetBySuperFamily(data.Pat.SuperFamily);
			data.SuperFamilyGuarantors=Patients.GetSuperFamilyGuarantors(data.Pat.SuperFamily);
			data.DictCloneSpecialities=Patients.GetClonesAndSpecialties(patNum);
			data.PatPict=Documents.GetPatPictFromDb(patNum);
			data.HasPatPict=(data.PatPict==null ? YN.No : YN.Yes);
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.PatientInformation);
			foreach(DisplayField field in listDisplayFields) {
				switch(field.InternalName) {
					case "Guardians":
						data.ListGuardians=Guardians.Refresh(patNum);
						break;
					case "Pat Restrictions":
						data.ListPatRestricts=PatRestrictions.GetAllForPat(patNum);
						break;
					case "Payor Types":
						data.PayorTypeDesc=PayorTypes.GetCurrentDescription(patNum);
						break;
					case "PatFields":
						data.ListPatFieldDefLinks=FieldDefLinks.GetForLocation(FieldLocations.Family);
						break;
					case "References":
						data.ListCustRefEntries=CustRefEntries.GetEntryListForCustomer(patNum);
						break;
					case "Referrals":
						data.ListRefAttaches=RefAttaches.Refresh(patNum);
						break;
					case "ResponsParty":
						if(data.Pat.ResponsParty!=0) {
							data.ResponsibleParty=Patients.GetLim(data.Pat.ResponsParty);
						}
						break;
				}
			}
			if(data.DiscountPlanSub!=null) {
				data.DiscountPlan=DiscountPlans.GetPlan(data.DiscountPlanSub.DiscountPlanNum);
			}
			//always check immediate family for patientlinks, regardless of if they are in the super family or not.
			data.ListMergeLinks=PatientLinks.GetLinks(data.Fam.ListPats.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			//If there is a super family, include those patientlink records as well.
			if(data.SuperFamilyMembers!=null && data.SuperFamilyMembers.Count>0) {
				//Need to include super family merge links to identify patients that were merged.
				data.ListMergeLinks.AddRange(PatientLinks.GetLinks(data.SuperFamilyMembers.Select(x => x.PatNum).ToList(),PatientLinkType.Merge));
			}
			if(doCreateSecLog) {
				SecurityLogs.MakeLogEntry(Permissions.FamilyModule,patNum,"");
			}
			return data;
		}

		///<summary>All the data necessary to load the Family Module.</summary>
		[Serializable]
		public class LoadData {
			public Family Fam;
			public Patient Pat;
			public PatientNote PatNote;
			public List<InsSub> ListInsSubs;
			public List<InsPlan> ListInsPlans;
			public List<PatPlan> ListPatPlans;
			public List<Benefit> ListBenefits;
			public List<Recall> ListRecalls;
			public PatField[] ArrPatFields;
			public List<Patient> SuperFamilyMembers;
			public List<Patient> SuperFamilyGuarantors;
			public SerializableDictionary<Patient,Def> DictCloneSpecialities;
			public Document PatPict;
			///<summary>Is yes if we have retrieved the PatPict from the db. No if we have tried but PatPict is null. Unknown if we have not attempted
			///to retrieve the PatPict.</summary>
			public YN HasPatPict;
			public string PayorTypeDesc;
			public List<RefAttach> ListRefAttaches;
			public List<Guardian> ListGuardians;
			public List<CustRefEntry> ListCustRefEntries;
			public List<PatRestriction> ListPatRestricts;
			public List<FieldDefLink> ListPatFieldDefLinks;
			public DiscountPlan DiscountPlan;
			///<summary>Only has the fields from Patients.GetLim.</summary>
			public Patient ResponsibleParty;
			public List<PatientLink> ListMergeLinks;
			public DiscountPlanSub DiscountPlanSub;
		}
	}
}
