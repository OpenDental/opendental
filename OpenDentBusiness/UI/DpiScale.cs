using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.UI{
	///<summary>These are duplicates of functions in LayoutManagerForms, but that's not avaiable in ODB.</summary>
	public class DpiScale{
		private float _scaleMy;

		public DpiScale(float scaleMy){
			_scaleMy=scaleMy;
		}

		///<summary>Converts from 96dpi to current scale.</summary>
		public float ToFloat(float val96){
			return val96*_scaleMy;
		}

		///<summary>Converts from 96dpi to current scale.</summary>
		public int ToInt(float val96){
			return (int)(Math.Round(val96*_scaleMy));
		}
	}
}
