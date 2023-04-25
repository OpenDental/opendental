using OpenDentBusiness;

namespace UnitTestsCore {
	public class PromotionT {
		public static Promotion CreatePromotion(long clinicNum=0,PromotionType type=PromotionType.Manual) {
			Promotion promo = new Promotion {
				ClinicNum=clinicNum,
				TypePromotion=type,
			};
			Promotions.Insert(promo);
			return promo;
		}

		public static void ClearPromotionTable() {
			string command = "DELETE FROM promotion WHERE PromotionNum > 0";
			DataCore.NonQ(command);
		}
	}
}
