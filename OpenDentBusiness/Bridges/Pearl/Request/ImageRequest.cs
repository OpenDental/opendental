namespace OpenDentBusiness.Pearl {
	public class ImageRequest {
		public string request_id { get; set; }
		public string extension { get; set; }
		//public string image_url { get; set; }
		//public string submit_url { get; set; }
		public string patient_id { get; set; }
		public string study_date { get; set; }
		public string patient_name { get; set; }
		public string patient_dob { get; set; }
		public string office_id { get; set; }
		public string organization_id { get; set; }
		//public TransformationOperation[] transformation_operations { get; set; }
		//public int flip { get; set; }
		//public String acquisition_date { get; set; }
		//public string workstation_id { get; set; }
		//public bool historical { get; set; }
	}
}
