using System;

namespace OpenDentBusiness{

	///<summary>Each row is a bridge to an outside program, frequently an imaging program.  Most of the bridges are hard coded, and simply need to be enabled.  But user can also add their own custom bridge.</summary>
	[Serializable]
	public class Program:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProgramNum;
		///<summary>Unique name for built-in program bridges. Not user-editable. enum ProgramName</summary>
		public string ProgName;
		///<summary>Description that shows.</summary>
		public string ProgDesc;
		///<summary>True if enabled.</summary>
		public bool Enabled;
		///<summary>The path of the executable to run or file to open.</summary>
		public string Path;
		///<summary>Some programs will accept command line arguments.</summary>
		public string CommandLine;
		///<summary>Notes about this program link. Peculiarities, etc.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;
		///<summary>If this is a Plugin, then this is the filename of the dll.  The dll must be located in the application directory.</summary>
		public string PluginDllName;
		///<summary>If no image, then will be an empty string.  In this case, the bitmap will be null when loaded from the database.
		///Must be a 22 x 22 image, and thus needs (width) x (height) x (depth) = 22 x 22 x 4 = 1936 bytes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ButtonImage;
		/// <summary>For custom program links only.  Stores the template of a file to be generated when launching the program link.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string FileTemplate;
		/// <summary>For custom program links only.  Stores the path of a file to be generated when launching the program link.</summary>
		public string FilePath;
		///<summary>Do not use directly.  Call Programs.IsEnabledByHq() instead.  Has HQ disabled this program for all customers via WebServiceHq.EnableAdditionalFeatures().  Using 'Disabled' because the web method will only send Programs HQ cares about.  Any user defined Programs should not be marked as 'Disabled' by default.</summary>
		public bool IsDisabledByHq;
		///<summary>Typically blank. A value is added to this if we have disabled this program at HQ's side and will be updated during HqProgram.Download()</summary>
		public string CustErr;

		public Program Copy(){
			return (Program)this.MemberwiseClone();
		}

	}

	///<summary>This enum is stored in the database as strings rather than as numbers, so we can do the order alphabetically and we can change it whenever we want.</summary>
	public enum ProgramName {
		None,
		ActeonImagingSuite,
		Adstra,
		Apixia,
		///<summary>PDMP program</summary>
		Appriss,
		Apteryx,
		AudaxCeph,
		///<summary>Avalara Inc.</summary>
		AvaTax,
		BencoPracticeManagement,
		BioPAK,
		///<summary>Newer version of MediaDent. Uses OLE COM interface.</summary>
		CADI,
		CallFire,
		Camsight,
		CaptureLink,
		CareCredit,
		Carestream,
		CentralDataStorage,
		Cerec,
		CleaRay,
		CliniView,
		ClioSoft,
		DBSWin,
		DemandForce,
		DentalEye,
		DentalIntel,
		DentalStudio,
		DentalTekSmartOfficePhone,
		DentForms,
		DentX,
		Dexis,
		DexisIntegrator,
		Digora,
		Dimaxis,
		Divvy,
		Dolphin,
		DrCeph,
		Dropbox,
		DXCPatientCreditScore,
		Dxis,
		EasyNotesPro,
		eClinicalWorks,
		EdgeExpress,
		///<summary>electronic Rx.</summary>
		eRx,
		EvaSoft,
		EwooEZDent,
		FHIR,
		FloridaProbe,
		Guru,
		HandyDentist,
		HdxWill,
		HouseCalls,
		IAP,
		iCat,
		iDixel,
		///<summary>FxImage was renamed to PatientGallery.</summary>
		ImageFX,
		iRYS,
		Lightyear,
		MediaDent,
		Midway,
		MiPACS,
		Mountainside,
		NewTomNNT,
		Office,
		///<summary>Obsolete. All instances may be removed as you run across them.  Also see Programs.UsingOrion.</summary>
		Orion,
		OrthoCAD,
		OrthoInsight3d,
		OrthoPlex,
		Oryx,
		Owandy,
		PandaPerio,
		PandaPerioAdvanced,
		PayConnect,
		PaySimple,
		///<summary>FxImage was renamed to PatientGallery.</summary>
		PatientGallery,
		Patterson,
		PerioPal,
		PDMP,
		Podium,
		PracticeByNumbers,
		PracticeWebReports,
		PreXionAcquire,
		PreXionViewer,
		Progeny,
		///<summary>Paperless Technology.</summary>
		PT,
		///<summary>Paperless Technology.</summary>
		PTupdate,
		QuickBooksOnline,
		RapidCall,
		///<summary>New version of SMARTDent.</summary>
		RayBridge,
		RayMage,
		Romexis,
		Scanora,
		Schick,
		SFTP,
		Sirona,
		SMARTDent,
		Sopro,
		ThreeShape,
		TigerView,
		Transworld,
		Triana,
		Trojan,
		TrojanExpressCollect,
		Trophy,
		TrophyEnhanced,
		Tscan,
		UAppoint,
		Vipersoft,
		visOra,
		VisionX,
		VistaDent,
		VixWin,
		VixWinBase36,
		VixWinBase41,
		VixWinNumbered,
		VixWinOld,
		Xcharge,
		XDR,
		XVWeb,
		ZImage
	}

	


}










