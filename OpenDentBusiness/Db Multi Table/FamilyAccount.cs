using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Class used to help in the family balancer tool.</summary>
	public class FamilyAccount {
		public FamilyAccount(List<Patient> listFamily,Patient guar) {
			ListFamilyMembers=listFamily;
			Guarantor=guar;
			ListSplits=new List<PaySplit>();
			Account=new PaymentEdit.ConstructResults();
		}

		public List<Patient> ListFamilyMembers { get; private set; }

		public PaymentEdit.ConstructResults Account { get; set; }

		public Patient Guarantor { get; private set; }

		///<summary>A list of PaySplits generated as a result of running a transfer for this FamilyAccount.</summary>
		public List<PaySplit> ListSplits { get; private set; }
	}
}
