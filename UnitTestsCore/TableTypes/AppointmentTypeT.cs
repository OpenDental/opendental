using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class AppointmentTypeT {

		public static AppointmentType CreateAppointmentType(string appointmentTypeName,Color appointmentTypeColor=new Color(),string codeStr=""
			,bool isHidden=false,int itemOrder=0,string pattern="")
		{
			AppointmentType appointmentType=new AppointmentType() {
				AppointmentTypeName=appointmentTypeName,
				AppointmentTypeColor=appointmentTypeColor,
				CodeStr=codeStr,
				IsHidden=isHidden,
				ItemOrder=itemOrder,
				Pattern=pattern,
			};
			AppointmentTypes.Insert(appointmentType);
			AppointmentTypes.RefreshCache();
			return appointmentType;
		}

		///<summary>dump_dentaloffice inserts 'WebSched New Patient Default' AppointmentType. Delete everything else.</summary>
		public static void ClearAppointmentTypeTableAdditions() {
			string command="DELETE FROM appointmenttype WHERE AppointmentTypeNum > 1";
			DataCore.NonQ(command);
		}

	}
}
