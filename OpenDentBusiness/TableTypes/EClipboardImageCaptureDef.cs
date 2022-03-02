using System;

namespace OpenDentBusiness{
  ///<summary>Used to set rules for how often a patient should submit an image when checking in for their appointment via eClipboard.</summary>
  [Serializable]
  [CrudTable(IsSynchable=true)]
  public class EClipboardImageCaptureDef:TableBase{
    ///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long EClipboardImageCaptureDefNum;
     ///<summary>FK to def.DefNum. Should match a DefNum that is in the in 'EClipboard Images' defcat or has (or had) the 'P' (Patient Pictures)  
    ///usage in the 'Image Categories' defcat.</summary>
    public long DefNum;
    ///<summary>True if the rule pertains to the patient self portrait. False if the rule is for an 'Eclipboard images' defcat definition.</summary>
    public bool IsSelfPortrait;
    ///<summary>Frequency at which the patient should update the image, in days. If frequency is 0, patient will be prompted to submit image at each checkin.</summary>
    public int FrequencyDays;
    ///<summary>FK to clinic.ClinicNum. Clinic the rule pertains to.</summary>
    public long ClinicNum;

    ///<summary></summary>
    public EClipboardImageCaptureDef Copy() {
        return (EClipboardImageCaptureDef)this.MemberwiseClone();
    }
  }
}
