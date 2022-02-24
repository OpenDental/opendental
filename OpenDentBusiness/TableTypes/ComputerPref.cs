using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {

	///<summary>Enables preference specification for individual computers on a customer network.</summary>
	[Serializable]
	public class ComputerPref:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ComputerPrefNum;
		///<summary>The human-readable name of the computer on the network (not the IP address).</summary>
		public string ComputerName;
		///<summary>Set to true if the tooth chart is to use a hardware accelerated OpenGL window when available. Set to false to use software rendering when available. Of course, the final pixel format on the customer machine depends on the list of available formats. Best match pixel format is always used. This option only applies if GraphicsSimple is set to false.</summary>
		public bool GraphicsUseHardware;
		///<summary>Enum:DrawingMode Set to 1 to use the low-quality 2D tooth chart in the chart module. Set to 0 to use a 3D DirectX based tooth chart in the chart module. This option helps the program run even when the local graphics hardware is buggy or unavailable.</summary>
		public DrawingMode GraphicsSimple;
		///<summary>Indicates the type of Suni sensor connected to the local computer (if any). This can be a value of A, B, C, or D.</summary>
		public string SensorType;
		///<summary>Indicates wether or not the Suni sensor uses binned operation.</summary>
		public bool SensorBinned;
		///<summary>Indicates which Suni box port to connect with. There are 2 ports on a box (ports 0 and 1).</summary>
		public int SensorPort;
		///<summary>Indicates the exposure level to use when capturing from a Suni sensor. Values can be 1 through 7.</summary>
		public int SensorExposure;
		///<summary>Indicates if the user prefers double-buffered 3D tooth-chart (where applicable).</summary>
		public bool GraphicsDoubleBuffering;
		///<summary>Indicates the current OpenGL pixel format by number which the user prefers (if using OpenGL).</summary>
		public int PreferredPixelFormatNum;
		///<summary>The path of the A-Z folder for the specified computer.  Overrides the officewide default.  Used when multiple locations are on a single virtual database and they each want to look to the local data folder for images.</summary>
		public string AtoZpath;
		///<summary>If the global setting for showing the Task List is on, this controls if it should be hidden on this specified computer</summary>
		public bool TaskKeepListHidden;
		///<summary>Dock task bar on bottom (0) or right (1).</summary>
		public int TaskDock;
		///<summary>X pos for right docked task list.</summary>
		public int TaskX;
		///<summary>Y pos for bottom docked task list.</summary>
		public int TaskY;
		///<summary>Holds a semi-colon separated list of enumeration names and values representing a DirectX format. If blank, then
		///no format is currently set and the best theoretical format will be chosen at program startup. If this value is set to
		///'opengl' then this computer is using OpenGL and a DirectX format will not be picked.</summary>
		public string DirectXFormat;
		///<summary>Show the select scanner dialog when scanning documents.</summary>
		public bool ScanDocSelectSource;
		///<summary>Show the scanner options dialog when scanning documents.</summary>
		public bool ScanDocShowOptions;
		///<summary>Attempt to scan in duplex mode when scanning multipage documents with an ADF.</summary>
		public bool ScanDocDuplex;
		///<summary>Scan in gray scale when scanning documents.</summary>
		public bool ScanDocGrayscale;
		///<summary>Scan at the specified resolution when scanning documents.  Example: 150.</summary>
		public int ScanDocResolution;
		///<summary>0-100. Quality of jpeg after compression when scanning documents.  100 indicates full quality.  Opposite of compression.</summary>
		public byte ScanDocQuality;
		///<summary>FK to clinic.ClinicNum.  The most recent clinic for this computer.  Determines which clinic is used when loading Open Dental.</summary>
		public long ClinicNum;
		///<summary>FK to apptview.ApptViewNum.  The most recent appt view num for this computer.  Used when opening with the Appts module in conjunction with ClinicNum if this ApptViewNum is associated to the ClinicNum.</summary>
		public long ApptViewNum;
		///<summary>Deprecated.  The index of the most recent appt view for this computer.  Uses it when opening.  This column cannot be dropped due to older versions using it upon opening (prior to calling the update file copier code) so they will throw a UE if this column is ever dropped.</summary>
		public byte RecentApptView;
		///<summary>Enum:SearchMode The search mode that is used when loading the patient select window, and while typing.
		///When 0 the patient select window will use the DB wide pref PatientSelectUsesSearchButton.</summary>
		public SearchMode PatSelectSearchMode;
		public bool NoShowLanguage;
		///<summary>If true, don't warn user if the region's decimal setting is not 2.</summary>
		public bool NoShowDecimal;
		///<summary>Enum:PlatformOD The current operating system platform for the computer.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public PlatformOD ComputerOS;
		///<summary>Starting X value if user has moved the help button. This is stored as a percentage (100%=1.0).</summary>
		public double HelpButtonXAdjustment;
		///<summary>Enum:YN Unknown, Yes, No.</summary>
		public YN GraphicsUseDirectX11;
		///<summary>Default 0.  Can be pos or neg.  In addition to normal monitor zoom set in Windows.</summary>
		public int Zoom;

		public ComputerPref Copy(){
			return (ComputerPref)MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum DrawingMode{
		///<summary>0</summary>
		DirectX,
		///<summary>1</summary>
		Simple2D,
		///<summary>2</summary>
		OpenGL
	}

	///<summary>The search mode that is used when loading the patient select window, and while typing</summary>
	public enum SearchMode {
		///<summary>0</summary>
		Default,
		///<summary>1</summary>
		UseSearchButton,
		///<summary>2</summary>
		RefreshWhileTyping
	}

	///<summary>Mimics .NET enum PlatformID.</summary>
	public enum PlatformOD {
		///<summary>Only happens when workstation has not ran through convert script yet.</summary>
		Undefined = -1,
		///<summary>The operating system is Win32s. Win32s is a layer that runs on 16-bit versions of Windows to provide access to 32-bit applications.</summary>
		Win32S = 0,
		///<summary> The operating system is Windows 95 or Windows 98.</summary>    
		Win32Windows = 1,
		///<summary>The operating system is Windows NT or later.</summary>
		Win32NT = 2,
		///<summary>The operating system is Windows CE.</summary>
		WinCE = 3,
		///<summary>The operating system is Unix.</summary>
		Unix = 4,
		///// <summary></summary>
		//Xbox = 5,
		///<summary>The operating system is Macintosh.</summary>
		MacOSX = 6
	}
}
