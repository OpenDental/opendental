namespace OpenDentBusiness.Pearl {
	public class ToothPart {
    public string id { get; set; }
    public Contour contour { get; set; }
    public object region_representation { get; set; }
    public string category { get; set; }
    public string condition { get; set; }
    public double confidence { get; set; }
    public EnumCategory category_id { get; set; }
    public EnumCondition condition_id { get; set; }
    public Polygon[] polygon { get; set; }
    public Color color { get; set; }
  }
}