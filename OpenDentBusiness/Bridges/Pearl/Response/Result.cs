using System;

namespace OpenDentBusiness.Pearl {
	public class Result {
    public string request_id { get; set; }
    public bool is_completed { get; set; }
    public bool is_rejected { get; set; }
    public string phash { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string enml_image_id { get; set; }
    public bool is_deleted { get; set; }
    public string patient_id { get; set; }
    public string pearl_patient_id { get; set; }
    public string image_ingestion_id { get; set; }
    public DateTime study_date { get; set; }
    public string organization_id { get; set; }
    public string office_id { get; set; }
    public string result_id { get; set; }
    public string message_type { get; set; }
    public Annotation[] annotations { get; set; }
    public ToothPart[] toothParts { get; set; }
  }
}