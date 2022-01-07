  //VisiQuick integration code written by Thomas Jensen tje@thomsystems.com
/*
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenDental
{

	class VQDDE32
	{

		[DllImport("vqdde32.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
					unsafe public static extern int VISI_EXIT();
		[DllImport("vqdde32.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
					unsafe public static extern int VISI_INITI(String inifn, UInt32 pm, UInt32 prevwnd);
		[DllImport("vqdde32.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
					unsafe public static extern int VISI_INIT2W(String inifn, UInt32 pm2, UInt32 prevwnd);
		[DllImport("vqdde32.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
					unsafe public static extern int VISI_GETVER();
		[DllImport("vqdde32.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
					unsafe public static extern int VISI_COMMAND(String cmd);
		[DllImport("vqdde32.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
					unsafe public static extern String VISI_GETERRORSTR();
		[DllImport("vqdde32.dll", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
					unsafe public static extern int VISI_GETERRORSTRBUF(String buf, Int32 buflen);
					
	}

	public class VisiQuick
	{
		
		public const int
			flagsfld		= 10,
			index1fld		= 11, 	// search tooth status: index1=1 == return 32 bits, low bit = q1 
			arg1fld			= 12,	// getimage
			arg2fld			= 13,	// getimage rect.left
			arg3fld			= 14,	// getimage rect.top
			arg4fld			= 15,	// getimage rect.right
			arg5fld			= 16,	// getimage rect.bottom
			dentistidfld	= 50,
			patidfld		= 100,
			patfnamefld		= 101,
			patlnamefld		= 102,
			patbdatefld		= 103,
			patgenderfld	= 104,
			patstreetfld	= 105,
			patzipfld		= 106,
			patcityfld		= 107,
			patcountryfld	= 108,
			patphonefld		= 109,
			patfaxfld		= 110,
			patemailfld		= 111,
			patnotesfld		= 112,
			patsofinumfld	= 113,
			patsrcidfld		= 114,
			patordernumfld	= 115,
			patdiagnosefld	= 116,
			photoidfld		= 200,
			photosrchfld	= 201,

			npf_showmenu	= 1,

			npi_defaultview	= 0,
			npi_compareview	= 1,
			npi_xrayview	= 2,
			npi_colorview	= 3,
			npi_panview		= 4,
			npi_cephview	= 5,
			npi_docview		= 6,
			npi_fileview	= 7,

			nphf_newpat		= 1,	// newphoto also does newpat
			nphi_acqxray	= 0,	// newphoto acquire xray
			nphi_acqvideo	= 1,	// newphoto acquire video

			gii_paintclient	= 1,	// getimage: paint image in clientarea
			gii_paintrect	= 2,	// getimage: paint image in rectangle in client area
			gii_paintrectdc	= 3,	// getimage: paint image in rectangle on dc
			gii_copydata	= 4,	// getimage: send wm_copydata with dib

			gif_fullimage	= 1,	// getimage: paint full image instead of thumb
			gif_photoid		= 2,	// getimage: use exact photoID

			spf_onlycheck	= 1,
			spf_oldfirst	= 2,	// oldest photos first
			spf_addtoend	= 4,	// do not first clear image area
			spf_2horizontal	= 8,	// set 2 views horizontal
			spf_single		= 16,	// set single view
			spf_bwsep		= 32,	// seperate bitewings to 4x2
			spf_tinymode	= 0x100,	// set tiny mode

			spi_bitewings	= 1,	// 2x2 or 4x2 bitewing view
			spi_defaultview	= 2,
			spi_compareview	= 3,
			spi_xrayview	= 4,
			spi_colorview	= 5,
			spi_panview		= 6,
			spi_cephview	= 7,
			spi_docview		= 8,
			spi_fileview	= 9,

			pht_unknown		= 0,
			pht_xray		= 1,
			pht_color		= 2,
			pht_doc			= 3,
			pht_imp			= 4,
			pht_ceph		= 5,

			spis_toothpos	= 1,

			lf_setclipresult= 0x1000,
			lf_oem2ansi		= 0x2000;

		VQDDE32	VQDDE;
		bool	FirstTime = true;
		bool	VQLoaded = false;
		UInt32	AppWnd = 0;

		public VisiQuick(IntPtr wnd)
		{
			AppWnd=(UInt32)wnd;
			VQDDE=new VQDDE32();
			VQLoaded = false;
		}

		~VisiQuick()
		{
			if (VQDDE != null)
				DoneVQ();
		}

		public void DoneVQ()
		{
			if (VQLoaded) 
				VQDDE32.VISI_EXIT();
			VQLoaded = false;
		}

		unsafe void InitVQ()
		{
			int		rv;

			DoneVQ();
			rv = VQDDE32.VISI_INITI(null, 0, AppWnd);
			if (rv != 0)
			{
				DoneVQ();
				MessageBox.Show( "FreeDental: VisiQuick DDE connection failed");
				return;
			}
			VQLoaded = true;
		}

		bool VQNotLoaded()
		{
			if ( FirstTime )
			{
				FirstTime = false;
				InitVQ();
			}
			if (!VQLoaded)
				MessageBox.Show( "FreeDental: VisiQuick is not loaded");
			return !VQLoaded;
		}

		unsafe bool reportvqerrors(int rv)
		{
			if (rv != 0)
			{
				String s = VQDDE32.VISI_GETERRORSTR();				
				MessageBox.Show(String.Format("FreeDental: VisiQuick returned an error: {0:D}, '{1:S}'",rv,s));
			}
			return rv!=0;
		}

		public bool VQStart(bool newphoto, String tooth, int flags, int index)
		{
			String	s,cmd;

			if (VQNotLoaded())
				return false;
			if (newphoto)
				s="newphoto";
			else
				s="newpat";

			cmd=String.Format("%VQCMD%={0}|100~{1:D}", s, Patients.Cur.PatNum);
			cmd=cmd+String.Format("|101~{0}", Patients.Cur.FName);
			cmd=cmd+String.Format("|102~{0}", Patients.Cur.LName);
			cmd=cmd+String.Format("|103~{0:d}", Patients.Cur.Birthdate);
			switch (Patients.Cur.Gender)
			{
				case PatientGender.Male	:
					s="M";
					break;
				default : 
					s="F";
					break;
			}
			cmd=cmd+String.Format("|104~{0}", s);
			cmd=cmd+String.Format("|105~{0}", Patients.Cur.Address);
			cmd=cmd+String.Format("|106~{0}", Patients.Cur.Zip);
			cmd=cmd+String.Format("|107~{0}", Patients.Cur.City);
			cmd=cmd+String.Format("|108~{0}", Patients.Cur.State);
			cmd=cmd+String.Format("|109~{0}", Patients.Cur.HmPhone);
			cmd=cmd+String.Format("|111~{0}", Patients.Cur.Email);
			cmd=cmd+String.Format("|113~{0}", Patients.Cur.SSN);

			if (newphoto)
				cmd=cmd+String.Format("|201~{0}",tooth);

			cmd=cmd+String.Format("|10~{0}", flags);
			cmd=cmd+String.Format("|11~{0}", index);

			int rv=VQDDE32.VISI_COMMAND(cmd);		// call VisiQuick
			return reportvqerrors(rv);
		}

		public bool SearchPhotos(String tc, int flags, int index)
		{
			String	cmd;

			if (VQNotLoaded())
				return false;

			cmd=String.Format("%VQCMD%=searchphotos|100~{0:D}", Patients.Cur.PatNum);
			cmd=cmd+String.Format("|10~{0}", flags);
			cmd=cmd+String.Format("|11~{0}", index);
			cmd=cmd+String.Format("|201~{0}", tc);

			int rv=VQDDE32.VISI_COMMAND(cmd);		// call VisiQuick
			return reportvqerrors(rv);
		}

		public String SearchTStatus(int PatNum)
		{
			String	cmd,result;

			result="";
			if (VQNotLoaded())
				return result;

			cmd=String.Format("%VQCMD%=searchtstatus|100~{0:D}", Patients.Cur.PatNum);
			cmd=cmd+String.Format("|11~{0}", 1);

			int rv=VQDDE32.VISI_COMMAND(cmd);		// call VisiQuick
			if (rv==0)
				result=VQDDE32.VISI_GETERRORSTR();	// Fetch tooth status string
			return result;
		}
	}
}
*/