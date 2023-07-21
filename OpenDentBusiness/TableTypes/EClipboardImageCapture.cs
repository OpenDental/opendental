using System;

namespace OpenDentBusiness {
	///<summary>Linker table between patients and the images they have submitted to the office via eClipboard. Lets office know when a patient last submitted
  ///a certain image. Is used in conjuction with EClipboardImageCaptureDef table to allow offices to set frequencies for how often patients should 
  ///submit certain images, similar to sheets.</summary>
	[Serializable]
  public class EClipboardImageCapture:TableBase{
    ///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long EClipboardImageCaptureNum;
    ///<summary>FK to patient.PatNum.</summary>
    public long PatNum;
    ///<summary>FK to def.DefNum. Should match a DefNum that that is in the in 'EClipboard Images' defcat or has (or had) the 'P' (Patient Pictures)  
    ///usage in the 'Image Categories' defcat.</summary>
    public long DefNum;
    ///<summary>Using DefNum to identify the self portrait is unreliable as the image category that is used to store self portraits may change. Instead, set this field to
    ///true for any image capture that is a self portrait. Only for self-portraits tied to the pref 'EClipboardAllowSelfPortraitOnCheckIn', not 'eClipboard Images' defcat.</summary>
    public bool IsSelfPortrait;
    ///<summary>Records the date and time the patient took the image. If patient has submitted this eclipboard image before, then we simply update the
    ///DateUpserted field and DocNum field. We do not insert an entirely new record.</summary>
    [CrudColumn(SpecialType=CrudSpecialColType.DateT)]
    public DateTime DateTimeUpserted;
    ///<summary>FK to document.DocNum. If a document is deleted, need to also delete any record from this table with the same DocNum.</summary>
    public long DocNum;

    ///<summary></summary>
    public EClipboardImageCapture Copy() {
        return (EClipboardImageCapture)this.MemberwiseClone();
    }
  }
}
