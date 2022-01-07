using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	[Serializable()]
	[CrudTable(IsMissingInGeneral=true)]
    public class Faq:TableBase {
        ///<summary>Primary key.</summary>
        [CrudColumn(IsPriKey=true)]
        public long FaqNum;
        ///<summary>The question part of the FAQ.</summary>
        public string QuestionText;
        ///<summary>The answer part of the FAQ.</summary>
        public string AnswerText;
        ///<summary>Stores a URL to display in an IFrame (ex. YouTube video)</summary>
        public string EmbeddedMediaUrl;
        ///<summary>The FAQ system orders by this field in an attempt to show stickied FAQ's first.</summary>
        public bool IsStickied;
        ///<summary>The version of the manual this FAQ object is linked to. Should only show for that version.</summary>
        public int ManualVersion;
        ///<summary>Optional URL path to an image.</summary>
        public string ImageUrl;
        ///<summary></summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateTEntry)]
        public DateTime DateTEntry;

        ///<summary></summary>
        public Faq Copy() {
            return (Faq)this.MemberwiseClone();
        }
    }
}
