using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Stores the necessary information for polling Pearl’s API to check if AI annotations have been generated for an image sent from the Imaging module. It's also used to prevent duplicate image submissions to the API, which would return an error response. To poll Pearl for an image, all that's required is a request_id and an organization_id. Organization_id is stored in the Pearl program link.</summary>
	[Serializable]
	public class PearlRequest:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PearlRequestNum;
		///<summary>Request ID given to Pearl to uniquely identify this request. Generated as a GUID before uploading an image to Pearl.</summary>
		public string RequestId;
		///<summary>FK to document. Links this request to the image that was sent. This is sufficient for mounts because mount images are sent individually to Pearl.</summary>
		public long DocNum;
		///<summary>Enum:EnumPearlStatus Keeps track of the request's status. Can be Polling, Received, or Error.</summary>
		public EnumPearlStatus RequestStatus;
		///<summary>The time the image was originally sent to Pearl.</summary>
		public DateTime DateTSent;
		///<summary>The most recent time an API call was made to Pearl to check the status of this request.</summary>
		public DateTime DateTChecked;
	}

	public enum EnumPearlStatus {
		///<summary>0 - An individual machine is actively polling Pearl.</summary>
		Polling,
		///<summary>1 - The image was successfully processed and AI annotations were returned from Pearl.</summary>
		Received,
		///<summary>2 - An error occurred on Pearl’s side. Only set for errors that prevent this request from ever being fulfilled.</summary>
		Error,
		///<summary>3 - Pearl did not give results within the timeout period of 10 minutes. Polling for this request can be retried.</summary>
		TimedOut,
	}
}

/*
CREATE TABLE pearlrequest (
  PearlRequestNum bigint NOT NULL auto_increment PRIMARY KEY,
  RequestId varchar(255) NOT NULL,
  DocNum bigint NOT NULL,
  RequestStatus tinyint NOT NULL,
  DateTSent datetime NOT NULL,
  DateTChecked datetime NOT NULL,
  INDEX (RequestId)
);
 */
