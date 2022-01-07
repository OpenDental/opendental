using OpenDentBusiness;
using System.Linq;

namespace UnitTestsCore {
	public class ApptViewItemT {

		public static ApptViewItem CreateApptViewItem(long opNum,long provNum,long clinicNum=0,string apptViewDesc = "All") {
			ApptView aptView=ApptViews.GetForClinic(clinicNum).FirstOrDefault(x => x.Description.ToLower()==apptViewDesc.ToLower());
			if(aptView==null) {
				aptView=new ApptView() {
					Description=apptViewDesc,
					ItemOrder=99,
					ClinicNum=clinicNum,
				};
				ApptViews.Insert(aptView);
			}
			ApptViewItem ret=new ApptViewItem(){
				ApptViewNum=aptView.ApptViewNum,
				OpNum=opNum,
				ProvNum=provNum,
			};
			ApptViewItems.Insert(ret);
			return ret;
		}
		///<summary>Deletes everything from the apptviewitem table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearApptViewItem() {
			string command="DELETE FROM apptviewitem WHERE ApptViewItemNum > 0";
			DataCore.NonQ(command);
		}
	}
}
