using OpenDentBusiness;

namespace UnitTestsCore {
	public class LaboratoryT {
		///<summary>Creates a laboratory.</summary>
		public static Laboratory CreateLaboratory(string description="",string phone="",string notes="",long slip=0,string address="",string city="",string state="",string zip="",string email="",string wirelessPhone="",bool isHidden=false)
		{
			Laboratory laboratory=new Laboratory() {
				Description=description,
				Phone=phone,
				Notes=notes,
				Slip=slip,
				Address=address,
        City=city,
        State=state,
        Zip=zip,
        Email=email,
        WirelessPhone=wirelessPhone,
        IsHidden=isHidden
			};
			Laboratories.Insert(laboratory);
			return laboratory;
		}

		///<summary>Deletes everything from the laboratory table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearLaboratoryTable() {
			string command="DELETE FROM laboratory";
			DataCore.NonQ(command);
		}

	}
}
