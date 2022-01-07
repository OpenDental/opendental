using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class EmailSecureT {

		///<summary>Deletes everything from the table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEmailSecureTable() {
			string command="DELETE FROM emailsecure WHERE EmailSecureNum > 0";
			DataAction.RunPractice(() => DataCore.NonQ(command));
		}

		///<summary>Gets all database entries.</summary>
		public static List<EmailSecure> GetAll() {
			string command="SELECT * FROM emailsecure";
			return DataAction.GetPractice(() => OpenDentBusiness.Crud.EmailSecureCrud.SelectMany(command));
		}
	}
}
