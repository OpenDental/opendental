using System;

namespace OpenDentBusiness {
	[Serializable,CrudTable(HasBatchWriteMethods=true)]
	public class LimitedBetaFeature:TableBase {
		[CrudColumn(IsPriKey=true)]
		public long LimitedBetaFeatureNum;
		///<summary>Stores the integer value of the LimitedBetaFeatureEnum. This is done to prevent out of bounds exceptions due to versioning.</summary>
		public long LimitedBetaFeatureTypeNum;
		///<summary>ClinicNum that is signed up for the feature. Clinic independant features only have one row with a clinicNum of -1.</summary>
		public long ClinicNum;
		///<summary>An office is considered signed up if they have a valid version to be using this feature on, the feature is on limited beta, and they've signed up with HQ.</summary>
		public bool IsSignedUp;

		public LimitedBetaFeature() {

		}

		public LimitedBetaFeature Copy() {
			return (LimitedBetaFeature)MemberwiseClone();
		}

		///<summary>Gets the LimitedBetaFeatureEnum for this object.</summary>
		public EServiceFeatureInfoEnum GetLimitedBetaFeatureEnum() {
			if(!Enum.TryParse<EServiceFeatureInfoEnum>(LimitedBetaFeatureTypeNum.ToString(),out EServiceFeatureInfoEnum ret)) {
				ret=EServiceFeatureInfoEnum.None;
			}
			return ret;
		}

		///<summary>Sets the LimitedBetaFeatureTypeNum for this object.</summary>
		public void SetLimitedBetaFeatureEnum(EServiceFeatureInfoEnum feature) {
			LimitedBetaFeatureTypeNum=(int)feature;
		}
	}

	///<summary>These are the servicesHQ.eServiceFeatureInfoHQ PK's. If the feature is no longer in limited beta, add an attribute to the enum.</summary>
	public enum EServiceFeatureInfoEnum : long {
		[EServiceFeatureStatus(isFinished:false)]
		None=0,
		[EServiceFeatureStatus(isFinished:false)]
		EClipPerio=1,
		[EServiceFeatureStatus(isFinished:false)]
		SecureEmail=2,
	}

	public class EServiceFeatureStatusAttribute:Attribute  {
		public bool IsFinished;

		public EServiceFeatureStatusAttribute() {
			IsFinished=false;
		}

		public EServiceFeatureStatusAttribute(bool isFinished) {
			IsFinished=isFinished;
		}
	}
}
