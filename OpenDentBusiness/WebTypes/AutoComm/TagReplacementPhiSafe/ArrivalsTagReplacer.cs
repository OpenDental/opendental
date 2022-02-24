using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.AutoComm {
	public class ArrivalsTagReplacer : ApptTagReplacer {
		public const string ARRIVED_TAG="[Arrived]";
		public const string ARRIVED_CODE="A";
		protected override void ReplaceTagsChild(StringBuilder sbTemplate,AutoCommObj autoCommObj,bool isEmail) {
			base.ReplaceTagsChild(sbTemplate,autoCommObj,isEmail);
			ReplaceOneTag(sbTemplate,ARRIVED_TAG,ARRIVED_CODE,isEmail);
		}
	}
}
