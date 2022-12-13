using System;

namespace OpenDentBusiness {
	///<summary>An attachment to a task. Attachment can be a document or a string.</summary>
	[Serializable]
  public class TaskAttachment:TableBase {
    ///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long TaskAttachmentNum;
    ///<summary>FK to task.TaskNum.</summary>
    public long TaskNum;
    ///<summary>FK to document.DocNum. If no document is attached, then this field will be 0.</summary>
    public long DocNum;
    ///<summary>Used to store text that doesn't need to be visible from the main task edit window at all times.</summary>
 		[CrudColumn(SpecialType=CrudSpecialColType.IsText | CrudSpecialColType.CleanText)]
    public string TextValue;
    ///<summary>A brief description of this attachment. If document is linked, used for document description as well.</summary>
    public string Description;

    ///<summary>Returns a copy of this TaskAttachment.</summary>
    public TaskAttachment Copy() {
        return (TaskAttachment)this.MemberwiseClone();
    }
   }
}
