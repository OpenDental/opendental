using System;

namespace CodeBase {
	public class ProgressBarHelper {
		///<summary>Used to set text in other controls to be displayed to the user like labels, text boxes, etc.</summary>
		public string LabelValue;
		///<summary>Used to set the label on the right of the progress bar</summary>
		public string PercentValue;
		///<summary>Changes progress bar current block value</summary>
		public int BlockValue;
		///<summary>Changes progress bar max value</summary>
		public int BlockMax;
		///<summary>Used to uniquely identify this ODEvent for consumers. Can be null.</summary>
		public string TagString;
		///<summary>Changes progress bar style</summary>
		public ProgBarStyle ProgressStyle;
		///<summary>Indicates what event this progress bar helper represents.  Used heavily by FormProgressExtended.</summary>
		public ProgBarEventType ProgressBarEventType;
		///<summary>Changes progress bar marquee speed</summary>
		public int MarqueeSpeed;
		public string LabelTop;
		public bool IsValHidden;
		public bool IsTopHidden;
		public bool IsPercentHidden;

		///<summary>Used as a shell to store information events need to update a progress window.</summary>
		public ProgressBarHelper(string labelValue,string percentValue="",int blockValue=0,int blockMax=0
			,ProgBarStyle progressStyle=ProgBarStyle.NoneSpecified,string tagString="",int marqueeSpeed=0,string labelTop="",bool isLeftValHidden=false,
			bool isTopHidden=false,bool isPercentHidden=false,ProgBarEventType progressBarEventType=ProgBarEventType.ProgressBar) 
		{
			LabelValue=labelValue;
			PercentValue=percentValue;
			BlockValue=blockValue;
			BlockMax=blockMax;
			ProgressStyle=progressStyle;
			ProgressBarEventType=progressBarEventType;
			TagString=tagString;
			MarqueeSpeed=marqueeSpeed;
			LabelTop=labelTop;
			IsValHidden=isLeftValHidden;
			IsTopHidden=isTopHidden;
			IsPercentHidden=isPercentHidden;
		}
	}
	
}
