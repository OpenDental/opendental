using System;

namespace OpenDentBusiness.Pearl {
	public class ImageResponse {
    /// <summary>This is given to us by Pearl.</summary>
    public string id { get; set; }
    /// <summary>Our GUID created to identify the image request. This id is used to poll the image from Pearl.</summary>
    public string request_id { get; set; }
    /// <summary>This is given to us by Pearl. Where our image is located on their aws bucket</summary>
    public string image_url { get; set; }
    /// <summary>If our image can be accessed using a URL we send this to Pearl. Pearl will upload the image themselves.</summary>
    public string submit_url { get; set; }
    /// <summary>Our patient patNum.</summary>
    public string patient_id { get; set; }
    /// <summary>Date yyyy-MM-dd this request was created.</summary>
    public string study_date { get; set; }
    /// <summary>The organization this request is being sent from to Pearl.</summary>
    public string organization_id { get; set; }
    /// <summary>The office this request is being sent from to Pearl.</summary>
    public string office_id { get; set; }
    /// <summary>The image format that will be sent. By default it is set to 'jpg'.</summary>
    public string extension { get; set; }
    /// <summary>This is given to us by Pearl. This is another form of id that can be used to poll the image from Pearl.</summary>
    public string image_ingestion_id { get; set; }
    /// <summary>This is given to us by Pearl. The date the response was created from Pearl.</summary>
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    ///<summary>This is a URL object given to us by Pearl. We send the image to their AWS bucket using the fields provided in the URL object.</summary>
    public PresignedUrl presigned_url { get; set; }
  }
}