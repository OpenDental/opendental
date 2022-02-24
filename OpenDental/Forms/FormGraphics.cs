using System;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;
using Tao.Platform.Windows;
using OpenDental.UI;
using SparksToothChart;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormGraphics : FormODBase {
		private OpenGLWinFormsControl.PixelFormatValue[] formats=new OpenGLWinFormsControl.PixelFormatValue[0];
		private ToothChartDirectX.DirectXDeviceFormat[] xformats=new ToothChartDirectX.DirectXDeviceFormat[0];
		private int selectedFormatNum=0;
		private string selectedDirectXFormat="";
		//private bool refreshAllowed=false;
		public ComputerPref ComputerPrefCur;
		///<summary>True when we are editing another computers ComputerPrefs.</summary>
		private bool _isRemoteEdit;

		///<summary></summary>
		public FormGraphics(){
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			
		}

		private void FormGraphics_Load(object sender,EventArgs e) {
			if(ComputerPrefCur==null) {
				ComputerPrefCur=ComputerPrefs.LocalComputer;
			}
			else {
				if(ComputerPrefCur!=ComputerPrefs.LocalComputer) {
					_isRemoteEdit=true;
				}
				MsgBox.Show(this,"Warning, editing another computers graphical settings should be done from that computer to ensure the selected settings work." 
					+" We do not recommend editing this way. If you make changes for another computer you should still verifiy that they work on that machine.");
					SecurityLogs.MakeLogEntry(Permissions.GraphicsRemoteEdit,0,"Edited graphical settings for "+ComputerPrefCur.ComputerName);
			}
			Text+=" - "+ComputerPrefCur.ComputerName;
			if(ComputerPrefCur.ComputerOS==PlatformOD.Undefined){//Remote computer has not updated yet. 
				MsgBox.Show(this,"Selected computer needs to be updated before being able to remotely change its graphical settings.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(ComputerPrefCur.ComputerOS==PlatformOD.Unix) {//Force simple mode on Unix systems.
				MsgBox.Show(this,"Linux users must use simple tooth chart.");
				radioDirectXChart.Enabled=false;
				radioOpenGLChart.Enabled=false;
				group3DToothChart.Enabled=false;
				return;
			}
			//ComputerPref computerPref=ComputerPrefs.GetForLocalComputer();
			checkHardwareAccel.Checked=ComputerPrefCur.GraphicsUseHardware;
			checkDoubleBuffering.Checked=ComputerPrefCur.GraphicsDoubleBuffering;
			selectedFormatNum=ComputerPrefCur.PreferredPixelFormatNum;
			selectedDirectXFormat=ComputerPrefCur.DirectXFormat;
			textSelected.Text="";
			if(PrefC.GetBool(PrefName.DirectX11ToothChartUseIfAvail)){
				radioDirectX11Use.Checked=true;
			}
			else{
				radioDirectX11DontUse.Checked=true;
			}
			if(ComputerPrefCur.GraphicsUseDirectX11==YN.Yes) {
				radioDirectX11ThisCompYes.Checked=true;
			}
			else if(ComputerPrefCur.GraphicsUseDirectX11==YN.No) {
				radioDirectX11ThisCompNo.Checked=true;
			}
			else {
				radioDirectX11ThisCompUseGlobal.Checked=true;
			}
			if(ToothChartRelay.IsSparks3DPresent){
				radioDirectX11Avail.Checked=true;
			}
			else{
				radioDirectX11NotFound.Checked=true;
			}
			if(ComputerPrefCur.GraphicsSimple==DrawingMode.Simple2D) {
				radioSimpleChart.Checked=true;
				group3DToothChart.Enabled=false;
			}
			else if(ComputerPrefCur.GraphicsSimple==DrawingMode.DirectX) {
				radioDirectXChart.Checked=true;
				group3DToothChart.Enabled=true;
				groupFilters.Enabled=false;
			}
			else{//OpenGL
				radioOpenGLChart.Checked=true;
				group3DToothChart.Enabled=true;
				groupFilters.Enabled=true;
			}
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				radioDirectXChart.Text="Use DirectX Graphics (recommended)";
				radioSimpleChart.Text="Use Simple Graphics";
				radioOpenGLChart.Text="Use OpenGL Graphics";
				group3DToothChart.Text="Options For 3D Graphics";
				label3DChart.Text="Most users will never need to change any of these options.  These are only used when the 3D graphics are not working "
					+"properly.";
			}
			RefreshFormats();
		}

		private void FillGrid() {
			int selectionIndex=-1;
			gridFormats.BeginUpdate();
			gridFormats.ListGridRows.Clear();
			if(this.radioDirectXChart.Checked){
				textSelected.Text="";
				gridFormats.BeginUpdate();
				gridFormats.ListGridColumns.Clear();
				GridColumn col=new GridColumn(Lan.g(this,"FormatNum"),80);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Adapter"),60);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Accelerated"),80);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Buffered"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"ColorBits"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"ColorFormat"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"DepthBits"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"DepthFormat"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Antialiasing"),75);
				gridFormats.ListGridColumns.Add(col);
				gridFormats.EndUpdate();
				for(int i=0;i<xformats.Length;i++) {
					GridRow row=new GridRow();
					row.Cells.Add((i+1).ToString());
					row.Cells.Add(xformats[i].adapterIndex.ToString());
					row.Cells.Add(xformats[i].IsHardware?"Yes":"No");//Supports hardware acceleration?
					row.Cells.Add("Yes");//Supports double-buffering. All DirectX devices support double-buffering as required.
					row.Cells.Add(ToothChartDirectX.GetFormatBitCount(xformats[i].BackBufferFormat).ToString());//Color bits.
					row.Cells.Add(xformats[i].BackBufferFormat);
					row.Cells.Add(ToothChartDirectX.GetFormatBitCount(xformats[i].DepthStencilFormat).ToString());//Depth buffer bits.
					row.Cells.Add(xformats[i].DepthStencilFormat);
					row.Cells.Add(xformats[i].MultiSampleCount.ToString());
					gridFormats.ListGridRows.Add(row);
					if(xformats[i].ToString()==selectedDirectXFormat) {
					  selectionIndex=i;
						textSelected.Text=(i+1).ToString();
					}
				}
			}
			else if(this.radioOpenGLChart.Checked){
				textSelected.Text=selectedFormatNum.ToString();
				gridFormats.BeginUpdate();
				gridFormats.ListGridColumns.Clear();
				GridColumn col=new GridColumn(Lan.g(this,"FormatNum"),80);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"OpenGL"),60);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Windowed"),80);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Bitmapped"),80);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Palette"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Accelerated"),80);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Buffered"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"ColorBits"),75);
				gridFormats.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"DepthBits"),75);
				gridFormats.ListGridColumns.Add(col);
				gridFormats.EndUpdate();
				for(int i=0;i<formats.Length;i++) {
					GridRow row=new GridRow();
					row.Cells.Add(formats[i].formatNumber.ToString());
					row.Cells.Add(OpenGLWinFormsControl.FormatSupportsOpenGL(formats[i].pfd)?"Yes":"No");
					row.Cells.Add(OpenGLWinFormsControl.FormatSupportsWindow(formats[i].pfd)?"Yes":"No");
					row.Cells.Add(OpenGLWinFormsControl.FormatSupportsBitmap(formats[i].pfd)?"Yes":"No");
					row.Cells.Add(OpenGLWinFormsControl.FormatUsesPalette(formats[i].pfd)?"Yes":"No");
					row.Cells.Add(OpenGLWinFormsControl.FormatSupportsAcceleration(formats[i].pfd)?"Yes":"No");
					row.Cells.Add(OpenGLWinFormsControl.FormatSupportsDoubleBuffering(formats[i].pfd)?"Yes":"No");
					row.Cells.Add(formats[i].pfd.cColorBits.ToString());
					row.Cells.Add(formats[i].pfd.cDepthBits.ToString());
					gridFormats.ListGridRows.Add(row);
					if(formats[i].formatNumber==selectedFormatNum) {
						selectionIndex=i;
					}
				}
			}
			gridFormats.EndUpdate();
			if(selectionIndex>=0) {
				gridFormats.SetSelected(selectionIndex,true);
			}
		}

		///<Summary>Get all formats for the grid based on the current filters.</Summary>
		private void RefreshFormats() {
			this.Cursor=Cursors.WaitCursor;
			gridFormats.ListGridRows.Clear();
			gridFormats.Invalidate();
			textSelected.Text="";
			Application.DoEvents();
			if(this.radioDirectXChart.Checked){
				xformats=ToothChartDirectX.GetStandardDeviceFormats();
			}
			else if(this.radioOpenGLChart.Checked){
				OpenGLWinFormsControl contextFinder=new OpenGLWinFormsControl();
				//Get raw formats.
				Gdi.PIXELFORMATDESCRIPTOR[] rawformats=OpenGLWinFormsControl.GetPixelFormats(contextFinder.GetHDC());
				if(checkAllFormats.Checked){
					formats=new OpenGLWinFormsControl.PixelFormatValue[rawformats.Length];
					for(int i=0;i<rawformats.Length;i++) {
						formats[i]=new OpenGLWinFormsControl.PixelFormatValue();
						formats[i].formatNumber=i+1;
						formats[i].pfd=rawformats[i];
					}
				}
				else{
					//Prioritize formats as requested by the user.
					formats=OpenGLWinFormsControl.PrioritizePixelFormats(rawformats,checkDoubleBuffering.Checked,checkHardwareAccel.Checked);
				}
				contextFinder.Dispose();
			}
			//Update the format grid to reflect possible changes in formats.
			FillGrid();
			this.Cursor=Cursors.Default;
		}

		private void radioSimpleChart_Click(object sender,EventArgs e) {
			group3DToothChart.Enabled=false;
		}

		private void radioDirectXChart_Click(object sender,EventArgs e) {
			group3DToothChart.Enabled=true;
			groupFilters.Enabled=false;
			RefreshFormats();
		}

		private void radioOpenGLChart_Click(object sender,EventArgs e) {
			group3DToothChart.Enabled=true;
			groupFilters.Enabled=true;
			RefreshFormats();
		}

		private void checkHardwareAccel_Click(object sender,EventArgs e) {
			RefreshFormats();
		}

		private void checkDoubleBuffering_Click(object sender,EventArgs e) {
			RefreshFormats();
		}

		private void checkAllFormats_Click(object sender,EventArgs e) {
			checkHardwareAccel.Enabled=!checkAllFormats.Checked;
			checkDoubleBuffering.Enabled=!checkAllFormats.Checked;
			RefreshFormats();
		}

		private void gridFormats_CellClick(object sender,ODGridClickEventArgs e) {
			int formatNum=Convert.ToInt32(gridFormats.ListGridRows[e.Row].Cells[0].Text);
			textSelected.Text=formatNum.ToString();
			if(radioDirectXChart.Checked) {
				selectedDirectXFormat=xformats[formatNum-1].ToString();
			}
			else if(this.radioOpenGLChart.Checked){
				selectedFormatNum=formatNum;
			}
		}

		///<summary>CAUTION: Runs slowly. May take minutes. Returns the string "invalid" if no suitable Direct X format can be found.</summary>
		public static string GetPreferredDirectXFormat(Form callingForm) {
			ToothChartDirectX.DirectXDeviceFormat[] possibleStandardFormats=ToothChartDirectX.GetStandardDeviceFormats();
			for(int i=0;i<possibleStandardFormats.Length;i++) {
				if(TestDirectXFormat(callingForm,possibleStandardFormats[i].ToString())) {
					return possibleStandardFormats[i].ToString();
				}
			}
			return "invalid";
		}

		///<summary>Returns true if the given directXFormat works for a DirectX tooth chart on the local computer.</summary>
		public static bool TestDirectXFormat(Form callingForm,string directXFormat) {
			ToothChartWrapper toothChartTest=new ToothChartWrapper();
			toothChartTest.Visible=false;
			//We add the invisible tooth chart to our form so that the device context will initialize properly
			//and our device creation test will then be accurate.
			callingForm.Controls.Add(toothChartTest);
			toothChartTest.DeviceFormat=new ToothChartDirectX.DirectXDeviceFormat(directXFormat);
			toothChartTest.DrawMode=DrawingMode.DirectX;//Creates the device.
			if(toothChartTest.DrawMode==DrawingMode.Simple2D) {
				//The chart is set back to 2D mode when there is an error initializing.
				callingForm.Controls.Remove(toothChartTest);
				toothChartTest.Dispose();
				return false;
			}
			//Now we check to be sure that one can draw and retrieve a screen shot from a DirectX control
			//using the specified device format.
			try {
				Bitmap screenShot=toothChartTest.GetBitmap();
				screenShot.Dispose();
			} 
			catch {
				callingForm.Controls.Remove(toothChartTest);
				toothChartTest.Dispose();
				return false;
			}
			callingForm.Controls.Remove(toothChartTest);
			toothChartTest.Dispose();
			return true;
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			bool changed=Prefs.UpdateBool(PrefName.DirectX11ToothChartUseIfAvail,radioDirectX11Use.Checked);
			//ComputerPrefCur doesn't change until here, so make the old copy exactly when we need it instead of when the form is created.
			ComputerPref computerPrefOld=ComputerPrefCur.Copy();
			if(radioDirectX11ThisCompUseGlobal.Checked) {
				ComputerPrefCur.GraphicsUseDirectX11=YN.Unknown;
			}
			else if(radioDirectX11ThisCompYes.Checked) {
				ComputerPrefCur.GraphicsUseDirectX11=YN.Yes;
			}
			else if(radioDirectX11ThisCompNo.Checked) {
				ComputerPrefCur.GraphicsUseDirectX11=YN.No;
			}
			//ComputerPref computerPref=ComputerPrefs.GetForLocalComputer();
			if(radioDirectXChart.Checked) {
				if(!_isRemoteEdit && !TestDirectXFormat(this,selectedDirectXFormat)){
					MessageBox.Show(Lan.g(this,"Please choose a different device format, "+
						"the selected device format will not support the DirectX 3D tooth chart on this computer"));
					return;
				}
				ComputerPrefCur.GraphicsSimple=DrawingMode.DirectX;
				ComputerPrefCur.DirectXFormat=selectedDirectXFormat;
			}
			else if(radioSimpleChart.Checked) {
				ComputerPrefCur.GraphicsSimple=DrawingMode.Simple2D;
			}
			else { //OpenGL
				OpenGLWinFormsControl contextTester=new OpenGLWinFormsControl();
				try {
					if(!_isRemoteEdit && contextTester.TaoInitializeContexts(selectedFormatNum)!=selectedFormatNum) {
						throw new Exception(Lan.g(this,"Could not initialize pixel format ")+selectedFormatNum.ToString()+".");
					}
				} 
				catch(Exception ex) {
					MessageBox.Show(Lan.g(this,"Please choose a different pixel format, the selected pixel format will not support the 3D tooth chart on this computer: "+ex.Message));
					contextTester.Dispose();
					return;
				}
				contextTester.Dispose();
				ComputerPrefCur.GraphicsUseHardware=checkHardwareAccel.Checked;
				ComputerPrefCur.GraphicsDoubleBuffering=checkDoubleBuffering.Checked;
				ComputerPrefCur.PreferredPixelFormatNum=selectedFormatNum;
				ComputerPrefCur.GraphicsSimple=DrawingMode.OpenGL;
			}
			ComputerPrefs.Update(ComputerPrefCur,computerPrefOld);
			if(changed){
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















