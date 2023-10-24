using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmZoom:FrmODBase {
		private static System.Windows.Forms.Screen _screen1;
		private static System.Windows.Forms.Screen _screen2;
		///<summary>Example 150</summary>
		private static int _scaleMS1=100;
		///<summary>Example 150</summary>
		private static int _scaleMS2=100;
		private static Size _sizeDesign=new Size(1246,735);
		private bool _fits;

		public FrmZoom() {
			InitializeComponent();
			//Lan.F(this);
		}

		//from our wiki: For now, all screens are assumed to have available 1246x735.  That would be a screen resolution of 1280x768 with a single width taskbar docked to any one of the four sides of the screen. The 7 main controls are slightly smaller due to menu bar on left of 51 and the toolbars on the top of 29. Subtracting the 10w and 31h for the form border, each module max size is 1185x675
		//Minor detail: MS borders are not exactly the same as our borders.  Theirs are 8 around 3 sides, and 39 on top, while ours are 5 around 3 sides and 26 on top.
		//So all forms will actually display 6x16 smaller than they were in the designer.  We are ignoring this small discrepancy in order to keep it simple.
		
		private void FrmODBase_Loaded(object sender,RoutedEventArgs e) {
			CalcScreens();
			textResolution1.Text=SizeToString(new Size(_screen1.Bounds.Width,_screen1.Bounds.Height));
			textWorkingArea1.Text=SizeToString(new Size(_screen1.WorkingArea.Width,_screen1.WorkingArea.Height));
			textScale1.Text=_scaleMS1.ToString()+"%";
			if(_screen2!=null){
				textResolution2.Text=SizeToString(new Size(_screen2.Bounds.Width,_screen2.Bounds.Height));
				textWorkingArea2.Text=SizeToString(new Size(_screen2.WorkingArea.Width,_screen2.WorkingArea.Height));
				textScale2.Text=_scaleMS2.ToString()+"%";
			}
			int zoom=ComputerPrefs.LocalComputer.Zoom;
			if(zoom==0) {
				zoom=100;
			}
			textZoom.Text=zoom.ToString();
			//always triggers textZoom_TextChanged
		}

		private static void CalcScreens(){
			_screen1=System.Windows.Forms.Screen.PrimaryScreen;
			System.Windows.Forms.Screen[] screenArray=System.Windows.Forms.Screen.AllScreens;
			int dpi1=96;
			int dpi2=96;
			if(screenArray.Length>1){
				if(screenArray[0]==_screen1){
					_screen2=screenArray[1];
				}
				else{
					_screen2=screenArray[0];
				}
			}
			dpi1=Dpi.GetScreenDpi(_screen1);
			_scaleMS1=(int)(100*dpi1/96f);
			if(_screen2!=null){	
				dpi2=Dpi.GetScreenDpi(_screen2);
				_scaleMS2=(int)(100*dpi2/96f);
			}
		}

		private void textZoom_TextChanged(object sender, EventArgs e) {
			_fits=true;
			int zoom=100;
			try{
				zoom=PIn.Int(textZoom.Text);//blank=0
			}
			catch{}
			if(zoom<=0 || zoom>=300){
				zoom=100;
			}
			Size sizeMax1;
			Size sizeMax2;
			int scaleTotal1=(int)(_scaleMS1*zoom/100f);//example 180
			textScaleTotal1.Text=scaleTotal1.ToString()+"%";
			sizeMax1=new Size((int)(_sizeDesign.Width*scaleTotal1/100f),(int)(_sizeDesign.Height*scaleTotal1/100f));
			textMax1.Text=SizeToString(sizeMax1);
			if(sizeMax1.Width<=_screen1.WorkingArea.Width && sizeMax1.Height<=_screen1.WorkingArea.Height){
				textFit1.Text="YES";
				textFit1.ColorText=ColorOD.ToWpf(System.Drawing.Color.DarkGreen);
			}
			else{
				textFit1.Text="NO";
				textFit1.ColorText=ColorOD.ToWpf(System.Drawing.Color.Red);
				_fits=false;
			}
			if(_screen2==null){
				return;
			}
			int scaleTotal2=(int)(_scaleMS2*zoom/100f);
			textScaleTotal2.Text=scaleTotal2.ToString()+"%";
			textDesign2.Text="1246 x 735";
			sizeMax2=new Size((int)(_sizeDesign.Width*scaleTotal2/100f),(int)(_sizeDesign.Height*scaleTotal2/100f));
			textMax2.Text=SizeToString(sizeMax2);
			if(sizeMax2.Width<=_screen2.WorkingArea.Width && sizeMax2.Height<=_screen2.WorkingArea.Height){
				textFit2.Text="YES";
				textFit2.ColorText=ColorOD.ToWpf(System.Drawing.Color.DarkGreen);
				return;
			}
			textFit2.Text="NO";
			textFit2.ColorText=ColorOD.ToWpf(System.Drawing.Color.Red);
			_fits=false;
		}

		private void butReset_Click(object sender, EventArgs e){
			textZoom.Text="100";
		}

		private void butFit_Click(object sender, EventArgs e){
			//calculate 4 max scales, based on the W and H of each screen
			int scaleW1=(int)(100f*_screen1.WorkingArea.Width/_sizeDesign.Width/_scaleMS1*100);//example 190/150*100=127
			int scaleH1=(int)(100f*_screen1.WorkingArea.Height/_sizeDesign.Height/_scaleMS1*100);
			List<int> listInts=new List<int>(){scaleW1,scaleH1 };
			if(_screen2!=null){
				int scaleW2=(int)(100f*_screen2.WorkingArea.Width/_sizeDesign.Width/_scaleMS2*100);
				int scaleH2=(int)(100f*_screen2.WorkingArea.Height/_sizeDesign.Height/_scaleMS2*100);
				listInts.Add(scaleW2);
				listInts.Add(scaleH2);
			}
			//use the smallest one.  The others would make it spill outside in at least one dimension.
			int suggestedZoom=listInts.Min();
			//Values between 95 and 105 are not allowed, so just keep them at 0 if within those ranges.
			if(suggestedZoom>=95 && suggestedZoom<=105) {
				suggestedZoom=0;
			}
			textZoom.Text=suggestedZoom.ToString();
		}

		private string SizeToString(Size size){
			return size.Width+" x "+size.Height;
		}

		/*
		public static bool Fits(){
			CalcScreens();
			int scaleTotal1=_scaleMS1+ComputerPrefs.LocalComputer.Zoom;
			Size sizeMax1=new Size((int)(_sizeDesign.Width*scaleTotal1/100f),(int)(_sizeDesign.Height*scaleTotal1/100f));
			if(sizeMax1.Width>_screen1.WorkingArea.Width || sizeMax1.Height>_screen1.WorkingArea.Height){
				return false;
			}
			if(_screen2==null){
				return true;
			}
			int scaleTotal2=_scaleMS2+ComputerPrefs.LocalComputer.Zoom;
			Size sizeMax2=new Size((int)(_sizeDesign.Width*scaleTotal2/100f),(int)(_sizeDesign.Height*scaleTotal2/100f));
			if(sizeMax2.Width>_screen2.WorkingArea.Width || sizeMax2.Height>_screen2.WorkingArea.Height){
				return false;
			}
			return true;
		}*/

		private void butOK_Click(object sender,EventArgs e) {
			int zoom=0;
			try{
				zoom=PIn.Int(textZoom.Text);//blank=0
			}
			catch{
				MsgBox.Show(this,"Please fix zoom, first.");
				return;
			}
			if(zoom==0 || zoom==100){
				//that's fine
			}
			else if(zoom<0){
				MsgBox.Show("Zoom cannot be negative.");
				return;
			}
			else if(zoom<60){//Anything less than this seems to have overlapping control issues, and < 10 can cause out of memory errors.
				string msg=Lans.g(this,"Zoom number should be greater than or equal to 60. Maybe you meant")
					+" "+(100+zoom).ToString()+".";//untranslated
				MsgBox.Show(msg);
				return;
			}
			else if(zoom>=300){
				MsgBox.Show(this,"Zoom number should be less than 300.");
				return;
			}
			else if(zoom>=95 && zoom<=105){
				MsgBox.Show(this,"Zoom number cannot be so close to 100.  Make it smaller than 95, bigger than 105, or leave blank.");
				return;
			}
			else if(!_fits){
				MsgBox.Show(this,"Zoom setting would not fit.");
				return;
			}
			else if(zoom<80){
				string msg=Lans.g(this,"You have chosen a zoom level that will make things smaller. Maybe you meant")
					+" 1"+zoom.ToString()+". "//untranslated
					+Lans.g(this,"Continue anyway?");
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,msg)){
					return;
				}
			}
			ComputerPrefs.LocalComputer.Zoom=zoom;
			ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			IsDialogOK=true;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			IsDialogOK=false;
		}

		private void butSettings_Click(object sender, EventArgs e){
			try {
				Process.Start("ms-settings:display");
			}
			catch(Exception ex) {
				MsgBox.Show(ex.Message);
			}
		}

		
	}
}