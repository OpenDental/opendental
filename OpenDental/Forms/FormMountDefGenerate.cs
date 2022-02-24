using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMountDefGenerate:FormODBase {
		public MountDef MountDefCur;

		public FormMountDefGenerate() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMountDefGenerate_Load(object sender, EventArgs e){
			listType.SelectedIndex=0;
			textWidth.Text="1700";
			textHeight.Text="1300";
			//FMX width=1700x4 + 1300x3 = 10,700
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textWidth.IsValid() || !textHeight.IsValid()){
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(listType.SelectedIndex==6){
				if(!textColumns.IsValid() || !textRows.IsValid()){
					MsgBox.Show(this,"Please fix data entry errors first.");
					return;
				}
				if(textColumns.Value==0 || textRows.Value==0){
					MsgBox.Show(this,"Enter columns and rows");
					return;
				}
			}
			int w=PIn.Int(textWidth.Text);
			int h=PIn.Int(textHeight.Text);
			int rows=textRows.Value;
			int cols=textColumns.Value;
			if(h>w){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Width should normally be greater than height.  Continue anyway?")){
					return;
				}
			}
			MountItemDefs.DeleteForMount(MountDefCur.MountDefNum);
			if(listType.SelectedIndex==0){//FMX horiz BW
				if(MountDefCur.IsNew){
					MountDefCur.Description="FMX";
				}
				MountDefCur.Height=3*h;
				MountDefCur.Width=w*4+h*3;
				MountDefCur.ColorBack=Color.Black;
				//BWs first:
				AddItem(1,0,h,w,h,0,"2,3,30,31");//molar
				AddItem(2,w,h,w,h,0,"4,5,28,29");//pre
				AddItem(3,3*w+3*h,h,w,h,180,"14,15,18,19");//L molar
				AddItem(4,2*w+3*h,h,w,h,180,"12,13,20,21");//L pre
				//PAs
				AddItem(5,0,0,w,h,0,"2,3");//UR
				AddItem(6,w,0,w,h,0,"4,5");
				AddItem(7,3*w+3*h,2*h,w,h,180,"18,19");//LL
				AddItem(8,2*w+3*h,2*h,w,h,180,"20,21");
				AddItem(9,0,2*h,w,h,0,"30,31");//LR
				AddItem(10,w,2*h,w,h,0,"28,29");
				AddItem(11,3*w+3*h,0,w,h,180,"14,15");//UL
				AddItem(12,2*w+3*h,0,w,h,180,"12,13");
				//Anterior
				AddItem(13,2*w,0,h,w,90,"6,7");//max
				AddItem(14,2*w+h,0,h,w,90,"8,9");
				AddItem(15,2*w+2*h,0,h,w,90,"10,11");
				AddItem(16,2*w,3*h-w,h,w,270,"26,27");//mand
				AddItem(17,2*w+h,3*h-w,h,w,270,"24,25");
				AddItem(18,2*w+2*h,3*h-w,h,w,270,"22,23");
			}
			if(listType.SelectedIndex==1){//FMX vert BW
				if(MountDefCur.IsNew){
					MountDefCur.Description="FMX";
				}
				MountDefCur.Height=2*h+w;
				MountDefCur.Width=w*4+h*3;
				MountDefCur.ColorBack=Color.Black;
				//BWs first:
				int leftBW=(w-h)/2;
				AddItem(1,leftBW,h,h,w,90,"2,3,30,31");//molar
				AddItem(2,w+leftBW,h,h,w,90,"4,5,28,29");//pre
				AddItem(3,3*w+3*h+leftBW,h,h,w,90,"14,15,18,19");//L molar
				AddItem(4,2*w+3*h+leftBW,h,h,w,90,"12,13,20,21");//L pre
				//PAs
				AddItem(5,0,0,w,h,0,"2,3");//UR
				AddItem(6,w,0,w,h,0,"4,5");
				AddItem(7,3*w+3*h,h+w,w,h,180,"18,19");//LL
				AddItem(8,2*w+3*h,h+w,w,h,180,"20,21");
				AddItem(9,0,h+w,w,h,0,"30,31");//LR
				AddItem(10,w,h+w,w,h,0,"28,29");
				AddItem(11,3*w+3*h,0,w,h,180,"14,15");//UL
				AddItem(12,2*w+3*h,0,w,h,180,"12,13");
				//Anterior
				AddItem(13,2*w,0,h,w,90,"6,7");//max
				AddItem(14,2*w+h,0,h,w,90,"8,9");
				AddItem(15,2*w+2*h,0,h,w,90,"10,11");
				AddItem(16,2*w,2*h,h,w,270,"26,27");//mand
				AddItem(17,2*w+h,2*h,h,w,270,"24,25");
				AddItem(18,2*w+2*h,2*h,h,w,270,"22,23");
			}
			if(listType.SelectedIndex==2){//4BW horiz
				if(MountDefCur.IsNew){
					MountDefCur.Description="4BW";
				}
				MountDefCur.Height=h;
				MountDefCur.Width=w*4;
				MountDefCur.ColorBack=Color.Black;
				AddItem(1,0,0,w,h,0,"2,3,30,31");//molar
				AddItem(2,w,0,w,h,0,"4,5,28,29");//pre
				AddItem(3,3*w,0,w,h,180,"14,15,18,19");//L molar
				AddItem(4,2*w,0,w,h,180,"12,13,20,21");//L pre
			}
			if(listType.SelectedIndex==3){//4BW vert
				if(MountDefCur.IsNew){
					MountDefCur.Description="4BW";
				}
				MountDefCur.Height=w;
				MountDefCur.Width=h*5;
				MountDefCur.ColorBack=Color.Black;
				AddItem(1,0,0,h,w,0,"2,3,30,31");//molar
				AddItem(2,h,0,h,w,0,"4,5,28,29");//pre
				AddItem(3,4*h,0,h,w,180,"14,15,18,19");//L molar
				AddItem(4,3*h,0,h,w,180,"12,13,20,21");//L pre
			}
			if(listType.SelectedIndex==4){//Compare
				if(MountDefCur.IsNew){
					MountDefCur.Description="Comparison";
				}
				MountDefCur.Height=h;
				MountDefCur.Width=w*2;
				MountDefCur.ColorBack=Color.Black;
				AddItem(1,0,0,w,h);
				AddItem(2,w,0,w,h);
			}
			if(listType.SelectedIndex==5){//Blank
				if(MountDefCur.IsNew){
					MountDefCur.Description="New Mount";
				}
				MountDefCur.Height=h;
				MountDefCur.Width=w;
				MountDefCur.ColorBack=Color.Black;
			}
			if(listType.SelectedIndex==6){//Photo Grid
				if(MountDefCur.IsNew){
					MountDefCur.Description="Photos";
				}
				MountDefCur.Height=h*rows;
				MountDefCur.Width=w*cols;
				MountDefCur.ColorBack=Color.White;
				for(int c=0;c<cols;c++){
					for(int r=0;r<rows;r++){
						AddItem(cols*r+c,w*c,h*r,w,h);
					}
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void AddItem(int itemOrder,int x,int y,int w,int h,int rotate=0,string teeth=""){
			MountItemDef mountItemDef=new MountItemDef();
			mountItemDef.MountDefNum=MountDefCur.MountDefNum;
			mountItemDef.ItemOrder=itemOrder;
			mountItemDef.Xpos=x;
			mountItemDef.Ypos=y;
			mountItemDef.Width=w;
			mountItemDef.Height=h;
			mountItemDef.RotateOnAcquire=rotate;
			mountItemDef.ToothNumbers=teeth;
			MountItemDefs.Insert(mountItemDef);
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}