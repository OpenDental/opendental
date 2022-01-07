using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>This table is the link between eServices and procedure codes.
	///Users will not be able to manually add any of these procedure codes as repeating charges.  They are handled by the sign up portal.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class EServiceCodeLink:TableBase {
		///<summary>Primary key</summary>
		[CrudColumn(IsPriKey=true)]
		public long EServiceCodeLinkNum;
		///<summary>FK to procedurecode.CodeNum</summary>
		public long CodeNum;
		///<summary>Enum:eServiceCode Service which the corresponding procedure code (CodeNum) applies to.</summary>
		public eServiceCode EService;

		[XmlIgnore]
		[CrudColumn(IsNotDbColumn=true)]
		public string ProcCode;

		public EServiceCodeLink Copy() {
			return (EServiceCodeLink)this.MemberwiseClone();
		}

		///<summary>Since repeating charges are saved using the ProcCode of the procedurecode table instead of the PK, this method will join on the 
		///procedurecode table in order to see if the procCode passed in is linked to within the eservicecodelink table.
		///Returns true if there are any links to the procedure code passed in.  This method and class are only used by HQ.</summary>
		public static bool IsProcCodeAnEService(string procCode,out eServiceCode eService) {
			//No need to check RemotingRole; no call to db.
			eService=GetEService(procCode);
			return eService!=eServiceCode.Undefined;
		}

		///<summary>Gets the eService code associated to the procCode. Returns Undefined if the procedure is not associated to one.</summary>
		public static eServiceCode GetEService(string procCode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<eServiceCode>(MethodBase.GetCurrentMethod(),procCode);
			}
			string command= @"SELECT MIN(EService) FROM eservicecodelink
				INNER JOIN procedurecode ON eservicecodelink.CodeNum=procedurecode.CodeNum
				WHERE procedurecode.ProcCode='"+POut.String(procCode)+"'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return eServiceCode.Undefined;
			}
			return PIn.Enum(table.Rows[0][0].ToString(),false,eServiceCode.Undefined);
		}

		///<summary>Gets a list of unique proc codes for all eService code links in the table.</summary>
		public static List<string> GetProcCodesForAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT procedurecode.ProcCode FROM procedurecode "
				+"INNER JOIN eservicecodelink ON procedurecode.CodeNum=eservicecodelink.CodeNum "
				+"GROUP BY eservicecodelink.CodeNum";
			return Db.GetListString(command);
		}

		///<summary>Get all all eService code links in the table and merges with proc codes.</summary>
		public static List<EServiceCodeLink> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<EServiceCodeLink>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT eservicecodelink.EServiceCodeLinkNum,eservicecodelink.CodeNum,eservicecodelink.EService,procedurecode.ProcCode FROM procedurecode "
				+"INNER JOIN eservicecodelink ON procedurecode.CodeNum=eservicecodelink.CodeNum";
			return DataCore.GetTable(command)
					.AsEnumerable()
					.Select(x => new EServiceCodeLink {
						EServiceCodeLinkNum=PIn.Long(x["EServiceCodeLinkNum"].ToString()),
						CodeNum=PIn.Long(x["CodeNum"].ToString()),
						EService=PIn.Enum<eServiceCode>(x["EService"].ToString()),
						ProcCode=PIn.String(x["ProcCode"].ToString()),
					}).ToList();
		}

		/* The following queries were ran when we upgraded HQ to v17.1.1
			CREATE TABLE eservicecodelink (
				EServiceCodeLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				CodeNum bigint NOT NULL,
				EService tinyint NOT NULL,
				INDEX(CodeNum)
				) DEFAULT CHARSET=utf8;


			INSERT INTO eservicecodelink (EService,CodeNum) 
			VALUES(4,699)  -- Mobile Web							ProcCode "027"
					 ,(5,771)  -- Patient Portal					ProcCode "033"
					 ,(6,824)  -- Web Sched Recalls				ProcCode "037"
					 ,(7,823)  -- Web Forms								ProcCode "036"
					 ,(10,888) -- ConfirmationRequest			ProcCode "040"
					 ,(13,889) -- Web Sched New Patient		ProcCode "041"
					 ,(15,892) -- Bundle									ProcCode "042"
					 ,(2,881)  -- Text Message Access			ProcCode "038"
					 ,(16,882) -- Text Message Usaged			ProcCode "039"
					 ,(17,705) -- Software Only						ProcCode "030"
			;
		 */

	}
}
