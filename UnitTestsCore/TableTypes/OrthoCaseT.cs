using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenDentBusiness;

namespace UnitTestsCore {
	/// <summary>	/// </summary>
	public class OrthoCaseT {
		///<summary>Inserts the OrthoCase, OrthoSchedule, Schedule OrthoPlanLink, and banding OrthoProcLink for an Ortho Case.</summary>
		public static long CreateOrthoCase(long patNum,double fee,double feeInsPrimary,double feeInsSecondary,double feePat,DateTime bandingOrTransferDate
			,bool isTransfer,DateTime debondDateExpected,double bandingAmount,double debondAmount,double visitAmount,Procedure bandingProc=null)
		{
			//No remoting role check; no call to db
			//Ortho Case
			OrthoCase newOrthoCase=new OrthoCase();
			newOrthoCase.PatNum=patNum;
			newOrthoCase.Fee=fee;
			newOrthoCase.FeeInsPrimary=feeInsPrimary;
			newOrthoCase.FeeInsSecondary=feeInsSecondary;
			newOrthoCase.FeePat=feePat;
			newOrthoCase.BandingDate=bandingOrTransferDate;
			newOrthoCase.DebondDateExpected=debondDateExpected;
			newOrthoCase.IsTransfer=isTransfer;
			newOrthoCase.SecUserNumEntry=Security.CurUser.UserNum;
			newOrthoCase.IsActive=true;//New Ortho Cases can only be added if there are no other active ones. So we automatically set a new ortho case as active.
			if(bandingProc!=null && bandingProc.AptNum!=0) {//If banding is scheduled save the appointment date instead.
				newOrthoCase.BandingDate=bandingProc.ProcDate;
			}
			long orthoCaseNum=OrthoCases.Insert(newOrthoCase);
			//Ortho Schedule
			OrthoSchedule newOrthoSchedule=new OrthoSchedule();
			newOrthoSchedule.BandingAmount=bandingAmount;
			newOrthoSchedule.DebondAmount=debondAmount;
			newOrthoSchedule.VisitAmount=visitAmount;
			newOrthoSchedule.IsActive=true;
			long orthoScheduleNum=OrthoSchedules.Insert(newOrthoSchedule);
			//Ortho Plan Link
			OrthoPlanLink newOrthoPlanLink=new OrthoPlanLink();
			newOrthoPlanLink.OrthoCaseNum=orthoCaseNum;
			newOrthoPlanLink.LinkType=OrthoPlanLinkType.OrthoSchedule;
			newOrthoPlanLink.FKey=orthoScheduleNum;
			newOrthoPlanLink.IsActive=true;
			newOrthoPlanLink.SecUserNumEntry=Security.CurUser.UserNum; 
			OrthoPlanLinks.Insert(newOrthoPlanLink);
			//Banding Proc Link
			if(!newOrthoCase.IsTransfer && bandingProc!=null) {
				OrthoProcLinks.Insert(OrthoProcLinks.CreateHelper(orthoCaseNum,bandingProc.ProcNum,OrthoProcType.Banding));
			}
			return orthoCaseNum;
		}

	}
}
