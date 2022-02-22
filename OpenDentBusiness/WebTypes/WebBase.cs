using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.WebTypes {
	///<summary>This is a base class for all classes that are not associated to a table in the database but both OpenDentBusiness and OpenDentalWebCore 
	///need to know about.  Currently this class is used via reflection to quickly find all classes that extend this base class.</summary>
	public abstract class WebBase {
	}
}
