using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EhrMeasureEventT {
		///<summary>Clears the ehrmeasureevent table.</summary>
		public static void ClearEhrMeasureEventTable() {
			string command="DELETE FROM ehrmeasureevent WHERE EhrMeasureEventNum > 0";
			DataCore.NonQ(command);
		}

		///<summary>Returns a list of all EhrMeasureEvent for All the patients passed in for the specified type. Defaults to EhrMeasureEventType.SummaryOfCareProvidedToDrElectronic</summary>
		public static List<EhrMeasureEvent> GetForPatsForType(List<long> listPatNums,EhrMeasureEventType type=EhrMeasureEventType.SummaryOfCareProvidedToDrElectronic) {
			List<EhrMeasureEvent> listEhrMeasureEvents=EhrMeasureEvents.GetAllByTypeFromDB(DateTime.MinValue,DateTime.MaxValue,
				type,false);
			return listEhrMeasureEvents.FindAll(x => ListTools.In(x.PatNum,listPatNums));
		}
	}
}
