using System;
using System.Windows.Forms;

namespace CodeBase {
	public partial class ODProgressBar:UserControl {
		private string _tagString;
		private ProgBarStyle _progressStyle;
		private bool _isLabelTopHidden;
		private bool _isLabelRightHidden;
		private bool _isLabelLeftHidden;
		
		public bool LabelLeftIsHidden	{
			get { return _isLabelLeftHidden; }
			set { _isLabelLeftHidden=value; }
		}

		public bool LabelTopIsHidden	{
			get { return _isLabelTopHidden; }
			set { _isLabelTopHidden=value; }
		}

		public bool LabelRightIsHidden	{
			get { return _isLabelRightHidden; }
			set { _isLabelRightHidden=value; }
		}

		public string LabelLeftText	{
			get { return labelLeftText.Text; }
			set { labelLeftText.Text=value; }
		}

		public string LabelTopText	{
			get { return labelTopText.Text; }
			set { labelTopText.Text=value; }
		}

		public string LabelRightText {
			get { return labelPercentComplete.Text; }
			set { labelPercentComplete.Text=value; }
		}

		///<summary>Changes progress bar current block value</summary>
		public int BlockValue	{
			get {return progressBar.Value; }
		  set { progressBar.Value=value; }
		}

		///<summary>Changes progress bar max value</summary>
		public int BlockMax	{
			get {return progressBar.Maximum; }
			set { progressBar.Maximum=value; }
		}

		///<summary>Used to uniquely identify this ODEvent for consumers. Can be null</summary>
		public string TagString	{
			get {return _tagString; }
			set { _tagString=value; }
		}

		///<summary>Changes progress bar style</summary>
		public ProgBarStyle ProgressStyle	{
			get {return _progressStyle; }
			set { _progressStyle=value; }
		}

		///<summary>Changes progress bar marquee speed</summary>
		public int MarqueeSpeed	{
			get {return progressBar.MarqueeAnimationSpeed; }
			set { progressBar.MarqueeAnimationSpeed=value; }
		}

		public ODProgressBar() {
			InitializeComponent();
			labelLeftText.Text="";
			labelTopText.Text="";
			labelPercentComplete.Text="";
			progressBar.Maximum=100;
			progressBar.Value=0;
			_tagString="";
			_progressStyle=ProgBarStyle.NoneSpecified;
			progressBar.MarqueeAnimationSpeed=0;
			_isLabelLeftHidden=false;
			_isLabelTopHidden=false;
			_isLabelRightHidden=false; 
		}

		public ODProgressBar(string labelLeftText,string labelTopText="",string labelPercentText="",int blockValue=0,int blockMax=100,string tagString="",
			ProgBarStyle progressStyle=ProgBarStyle.NoneSpecified,int marqueeSpeed=0,bool isLabelLeftHidden=false,bool isLabelTopHidden=false
			,bool isLabelRightHidden=false) 
		{
			InitializeComponent();
			this.labelLeftText.Text=labelLeftText;
			this.labelTopText.Text=labelTopText;
			labelPercentComplete.Text=labelPercentText;
			progressBar.Maximum=blockMax;
			progressBar.Value=blockValue;
			_tagString=tagString;
			switch(progressStyle) {
				case ProgBarStyle.Blocks:
					progressBar.Style=ProgressBarStyle.Blocks;
					break;
				case ProgBarStyle.Marquee:
					progressBar.Style=ProgressBarStyle.Marquee;
					break;
				case ProgBarStyle.NoneSpecified:
				case ProgBarStyle.Continuous:
				default:
					progressBar.Style=ProgressBarStyle.Continuous;
					break;
			}
			progressBar.MarqueeAnimationSpeed=marqueeSpeed;
			_isLabelLeftHidden=isLabelLeftHidden;
			_isLabelTopHidden=isLabelTopHidden;
			_isLabelRightHidden=isLabelRightHidden;
			LayoutHelper();
		}

		public void ODProgUpdate(string labelLeftText,string labelTopText,string labelPercentText,int blockValue,int blockMax,string tagString, 
			ProgBarStyle progressStyle,int marqueeSpeed=0,bool isLabelLeftHidden=false,bool isLabelTopHidden=false,bool isLabelRightHidden=false) 
		{
			this.labelLeftText.Text=labelLeftText;
			this.labelTopText.Text=labelTopText;
			if(labelPercentText!=null) {
				labelPercentComplete.Text=labelPercentText;
			}
			if(blockMax!=0) {
				progressBar.Maximum=blockMax;
			}
			if(blockValue!=0) {
				if(blockValue>progressBar.Maximum || blockValue<progressBar.Minimum) {
					progressBar.Value=progressBar.Maximum;
				}
				else {
					progressBar.Value=blockValue;
				}
			}
			progressBar.Tag=tagString;
			switch(progressStyle) {
				case ProgBarStyle.Blocks:
					progressBar.Style=ProgressBarStyle.Blocks;
					break;
				case ProgBarStyle.Marquee:
					progressBar.Style=ProgressBarStyle.Marquee;
					break;
				case ProgBarStyle.NoneSpecified:
				case ProgBarStyle.Continuous:
				default:
					progressBar.Style=ProgressBarStyle.Continuous;
					break;
			}
			if(marqueeSpeed!=0) {
				progressBar.MarqueeAnimationSpeed=marqueeSpeed;
			}
			progressBar.MarqueeAnimationSpeed=marqueeSpeed;
			_isLabelLeftHidden=isLabelLeftHidden;
			_isLabelTopHidden=isLabelTopHidden;
			_isLabelRightHidden=isLabelRightHidden;
			LayoutHelper();
		}

		///<summary>Changes the layout of the labels and other controls based on current property values.</summary>
		private void LayoutHelper() {
			//Checks if any labels are hidden and hides them accordingly.
			//Size the progress bar based on if the left or top labels are hidden.
			//only adjust if either top or left labels are hidden. If both are marked hidden no adjustment will be made. 
			int padding=6;
			if(_isLabelLeftHidden) {
				labelLeftText.Visible=false;
				progressBar.SetBounds(labelLeftText.Location.X,progressBar.Location.Y
					,(labelLeftText.Location.X) + (labelPercentComplete.Location.X - padding),progressBar.Height);
			}
			if(_isLabelRightHidden) {
				labelPercentComplete.Visible=false;
				//Make the progress bar span all the way to the right edge of the control.
				progressBar.Width=this.Width - (progressBar.Location.X + padding);
			}
			if(_isLabelTopHidden) {
				labelTopText.Visible=false;
				this.Height-=labelTopText.Height-5; //-5 to prevent the bottom line of the bar from disappearing. 
			}
		}

	}

	
}

