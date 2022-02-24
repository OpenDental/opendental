using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestsCore {
	public class ApptViewT {

		///<summary></summary>
		public static ApptView CreateApptView(string description,TimeSpan apptTimeScrollStart=new TimeSpan()) {
			ApptView apptView=new ApptView() {
				Description=description,
				ApptTimeScrollStart=apptTimeScrollStart,
			};
			ApptViews.Insert(apptView);
			ApptViews.RefreshCache();
			return apptView;
		}

		///<summary>Creates an appointment view with amount of operatories specified from list of op nums passed in for the 
		///specified clinic. Does not handle assigning providers to operatories. </summary>
		public static ApptView SetApptView(List<long> listOpNums,long clinicNum=0) {//Create aptView items for list of providers passed in, list of ops passed in
			ApptView aptView=new ApptView();
			aptView.ClinicNum=clinicNum;
			ApptViews.Insert(aptView);
			for(int i = 0;i < listOpNums.Count;i++) {
				ApptViewItem viewItem=new ApptViewItem();
				viewItem.ApptViewNum=aptView.ApptViewNum;
				viewItem.OpNum=listOpNums[i];
				ApptViewItems.Insert(viewItem);
			}
			ApptViews.RefreshCache();
			ApptViewItems.RefreshCache();
			return aptView;
		}

		///<summary>Deletes everything from the apptview table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearApptView() {
			string command="DELETE FROM apptview WHERE ApptViewNum > 0";
			DataCore.NonQ(command);
		}
	}
}
