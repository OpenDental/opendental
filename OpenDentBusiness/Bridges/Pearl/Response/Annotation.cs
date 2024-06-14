namespace OpenDentBusiness.Pearl {
	public class Annotation {
    public string enml_annotation_id { get; set; }
    public EnumCategory category_id { get; set; }
    public string condition { get; set; }
    public string category { get; set; }
    public ContourBox contour_box { get; set; }
    public string level { get; set; }
    public double confidence { get; set; }
    public Polygon[] polygon { get; set; }
    public Relationship[] relationships { get; set; }
    public LineSegment line_segment { get; set; }
    public Text text { get; set; }
    public string stroke_color { get; set; }
    public int stroke_width { get; set; }
  }
}