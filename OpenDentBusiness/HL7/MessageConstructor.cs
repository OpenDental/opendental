using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OpenDentBusiness.HL7 {
	///<summary>This is the engine that will construct our outgoing HL7 messages.</summary>
	public static class MessageConstructor {

		///<summary>Returns null if there is no HL7Def enabled or if there is no outbound DFT defined for the enabled HL7Def.</summary>
		public static MessageHL7 GenerateDFT(List<Procedure> listProcs,EventTypeHL7 eventType,Patient pat,Patient guar,long aptNum,string pdfDescription,string pdfDataString) {
			//In \\SERVERFILES\storage\OPEN DENTAL\Programmers Documents\Standards (X12, ADA, etc)\HL7\Version2.6\V26_CH02_Control_M4_JAN2007.doc
			//On page 28, there is a Message Construction Pseudocode as well as a flowchart which might help.
			MessageHL7 msgHl7=new MessageHL7(MessageTypeHL7.DFT);
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
			if(hl7Def==null) {
				return null;
			}
			//find a DFT message in the def
			HL7DefMessage hl7DefMessage=null;
			for(int i=0;i<hl7Def.hl7DefMessages.Count;i++) {
				if(hl7Def.hl7DefMessages[i].MessageType==MessageTypeHL7.DFT && hl7Def.hl7DefMessages[i].InOrOut==InOutHL7.Outgoing) {
					hl7DefMessage=hl7Def.hl7DefMessages[i];
					//continue;
					break;
				}
			}
			if(hl7DefMessage==null) {//DFT message type is not defined so do nothing and return
				return null;
			}
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				pat=Patients.GetOriginalPatientForClone(pat);
			}
			Provider prov=Providers.GetProv(Patients.GetProvNum(pat));
			Appointment apt=Appointments.GetOneApt(aptNum);
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			for(int i=0;i<hl7DefMessage.hl7DefSegments.Count;i++) {
				int repeatCount=1;
				if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.FT1) {
					repeatCount=listProcs.Count;
				}
				else if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.IN1) {
					repeatCount=listPatPlans.Count;
				}
				//for example, countRepeat can be zero in the case where we are only sending a PDF of the TP to eCW, and no procs.
				//or the patient does not have any current insplans for IN1 segments
				for(int j=0;j<repeatCount;j++) {//FT1 is optional and can repeat so add as many FT1's as procs in procList, IN1 is optional and can repeat as well, repeat for the number of patplans in patplanList
					if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.FT1 && listProcs.Count>j) {
						prov=Providers.GetProv(listProcs[j].ProvNum);
					}
					Procedure proc=null;
					if(listProcs.Count>j) {//procList could be an empty list
						proc=listProcs[j];
					}
					PatPlan patPlanCur=null;
					InsPlan insPlanCur=null;
					InsSub insSubCur=null;
					Carrier carrierCur=null;
					Patient subscriber=null;
					if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.IN1) {
						patPlanCur=listPatPlans[j];
						insSubCur=InsSubs.GetOne(patPlanCur.InsSubNum);
						insPlanCur=InsPlans.RefreshOne(insSubCur.PlanNum);
						carrierCur=Carriers.GetCarrier(insPlanCur.CarrierNum);
						subscriber=Patients.GetPat(insSubCur.Subscriber);
					}
					SegmentHL7 seg=new SegmentHL7(hl7DefMessage.hl7DefSegments[i].SegmentName);
					seg.SetField(0,hl7DefMessage.hl7DefSegments[i].SegmentName.ToString());
					for(int f=0;f<hl7DefMessage.hl7DefSegments[i].hl7DefFields.Count;f++) {
						string fieldName=hl7DefMessage.hl7DefSegments[i].hl7DefFields[f].FieldName;
						if(fieldName=="") {//If fixed text instead of field name just add text to segment
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[f].OrdinalPos,hl7DefMessage.hl7DefSegments[i].hl7DefFields[f].FixedText);
						}
						else {
							string fieldValue="";
							if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.IN1) {
								fieldValue=FieldConstructor.GenerateFieldIN1(hl7Def,fieldName,j+1,patPlanCur,insSubCur,insPlanCur,carrierCur,listPatPlans.Count,subscriber);
							}
							else {
								fieldValue=FieldConstructor.GenerateField(hl7Def,fieldName,MessageTypeHL7.DFT,pat,prov,proc,guar,apt,j+1,eventType,
								 pdfDescription,pdfDataString,MessageStructureHL7.DFT_P03,seg.Name);
							}
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[f].OrdinalPos,fieldValue);
						}
					}
					msgHl7.Segments.Add(seg);
				}
			}
			return msgHl7;
		}

		///<summary>Returns null if there is no HL7Def enabled or if there is no outbound ADT defined for the enabled HL7Def.</summary>
		public static MessageHL7 GenerateADT(Patient pat,Patient guar,EventTypeHL7 eventType) {
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
			if(hl7Def==null) {
				return null;
			}
			//find an outbound ADT message in the def
			HL7DefMessage hl7DefMessage=null;
			for(int i=0;i<hl7Def.hl7DefMessages.Count;i++) {
				if(hl7Def.hl7DefMessages[i].MessageType==MessageTypeHL7.ADT && hl7Def.hl7DefMessages[i].InOrOut==InOutHL7.Outgoing) {
					hl7DefMessage=hl7Def.hl7DefMessages[i];
					//continue;
					break;
				}
			}
			if(hl7DefMessage==null) {//ADT message type is not defined so do nothing and return
				return null;
			}
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				pat=Patients.GetOriginalPatientForClone(pat);
			}
			MessageHL7 messageHL7=new MessageHL7(MessageTypeHL7.ADT);
			Provider prov=Providers.GetProv(Patients.GetProvNum(pat));
			List<PatPlan> listPatPlans=PatPlans.Refresh(pat.PatNum);
			for(int i=0;i<hl7DefMessage.hl7DefSegments.Count;i++) {
				int countRepeat=1;
				//IN1 segment can repeat, get the number of current insurance plans attached to the patient
				if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.IN1) {
					countRepeat=listPatPlans.Count;
				}
				//countRepeat is usually 1, but for repeatable/optional fields, it may be 0 or greater than 1
				//for example, countRepeat can be zero if the patient does not have any current insplans, in which case no IN1 segments will be included
				for(int j=0;j<countRepeat;j++) {//IN1 is optional and can repeat so add as many as listPatplans
					PatPlan patplanCur=null;
					InsPlan insplanCur=null;
					InsSub inssubCur=null;
					Carrier carrierCur=null;
					Patient patSub=null;
					if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.IN1) {//index repeat is guaranteed to be less than listPatplans.Count
						patplanCur=listPatPlans[j];
						inssubCur=InsSubs.GetOne(patplanCur.InsSubNum);
						insplanCur=InsPlans.RefreshOne(inssubCur.PlanNum);
						carrierCur=Carriers.GetCarrier(insplanCur.CarrierNum);
						if(pat.PatNum==inssubCur.Subscriber) {
							patSub=pat.Copy();
						}
						else {
							patSub=Patients.GetPat(inssubCur.Subscriber);
						}
					}
					SegmentHL7 seg=new SegmentHL7(hl7DefMessage.hl7DefSegments[i].SegmentName);
					seg.SetField(0,hl7DefMessage.hl7DefSegments[i].SegmentName.ToString());
					for(int k=0;k<hl7DefMessage.hl7DefSegments[i].hl7DefFields.Count;k++) {
						string fieldName=hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].FieldName;
						if(fieldName=="") {//If fixed text instead of field name just add text to segment
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].OrdinalPos,hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].FixedText);
						}
						else {
							string fieldValue="";
							if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.IN1) {
								fieldValue=FieldConstructor.GenerateFieldIN1(hl7Def,fieldName,j+1,patplanCur,inssubCur,insplanCur,carrierCur,listPatPlans.Count,patSub);
							}
							else {
								fieldValue=FieldConstructor.GenerateFieldADT(hl7Def,fieldName,pat,prov,guar,j+1,eventType,seg.Name);
							}
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].OrdinalPos,fieldValue);
						}
					}
					messageHL7.Segments.Add(seg);
				}
			}
			return messageHL7;
		}

		///<summary>Returns null if there is no HL7Def enabled or if there is no outbound SIU defined for the enabled HL7Def.</summary>
		public static MessageHL7 GenerateSIU(Patient pat,Patient guar,EventTypeHL7 eventType,Appointment apt) {
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
			if(hl7Def==null) {
				return null;
			}
			//find an outbound SIU message in the def
			HL7DefMessage hl7DefMessage=null;
			for(int i=0;i<hl7Def.hl7DefMessages.Count;i++) {
				if(hl7Def.hl7DefMessages[i].MessageType==MessageTypeHL7.SIU && hl7Def.hl7DefMessages[i].InOrOut==InOutHL7.Outgoing) {
					hl7DefMessage=hl7Def.hl7DefMessages[i];
					//continue;
					break;
				}
			}
			if(hl7DefMessage==null) {//SIU message type is not defined so do nothing and return
				return null;
			}
			if(apt==null) {//SIU messages must have an appointment
				return null;
			}
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				pat=Patients.GetOriginalPatientForClone(pat);
			}
			MessageHL7 messageHL7=new MessageHL7(MessageTypeHL7.SIU);
			Provider prov=Providers.GetProv(apt.ProvNum);
			for(int i=0;i<hl7DefMessage.hl7DefSegments.Count;i++) {
				int repeatCount=1;
				//AIP segment can repeat, once for the dentist on the appt and once for the hygienist
				if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.AIP && apt.ProvHyg>0) {
					repeatCount=2;
				}
				for(int j=0;j<repeatCount;j++) {//AIP will be repeated if there is a dentist and a hygienist on the appt
					if(j>0) {
						prov=Providers.GetProv(apt.ProvHyg);
						if(prov==null) {
							break;//shouldn't happen, apt.ProvHyg would have to be set to an invalid ProvNum on the appt, just in case
						}
					}
					SegmentHL7 seg=new SegmentHL7(hl7DefMessage.hl7DefSegments[i].SegmentName);
					seg.SetField(0,hl7DefMessage.hl7DefSegments[i].SegmentName.ToString());
					for(int k=0;k<hl7DefMessage.hl7DefSegments[i].hl7DefFields.Count;k++) {
						string fieldName=hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].FieldName;
						if(fieldName=="") {//If fixed text instead of field name just add text to segment
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].OrdinalPos,hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].FixedText);
						}
						else {
							string fieldValue=FieldConstructor.GenerateFieldSIU(hl7Def,fieldName,pat,prov,guar,apt,j+1,eventType,seg.Name);
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].OrdinalPos,fieldValue);
						}
					}
					messageHL7.Segments.Add(seg);
				}
			}
			return messageHL7;
		}

		///<summary>Returns null if no HL7 def is enabled or no ACK is defined in the enabled def.</summary>
		public static MessageHL7 GenerateACK(string controlId,bool isAck,string ackEvent) {
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
			if(hl7Def==null) {
				return null;//no def enabled, return null
			}
			//find an ACK message in the def
			HL7DefMessage hl7DefMessage=null;
			for(int i=0;i<hl7Def.hl7DefMessages.Count;i++) {
				if(hl7Def.hl7DefMessages[i].MessageType==MessageTypeHL7.ACK && hl7Def.hl7DefMessages[i].InOrOut==InOutHL7.Outgoing) {
					hl7DefMessage=hl7Def.hl7DefMessages[i];
					break;
				}
			}
			if(hl7DefMessage==null) {//ACK message type is not defined so do nothing and return
				return null;
			}
			MessageHL7 msgHl7=new MessageHL7(MessageTypeHL7.ACK);
			msgHl7.ControlId=controlId;
			msgHl7.AckEvent=ackEvent;
			//go through each segment in the def
			for(int s=0;s<hl7DefMessage.hl7DefSegments.Count;s++) {
				SegmentHL7 seg=new SegmentHL7(hl7DefMessage.hl7DefSegments[s].SegmentName);
				seg.SetField(0,hl7DefMessage.hl7DefSegments[s].SegmentName.ToString());
				for(int f=0;f<hl7DefMessage.hl7DefSegments[s].hl7DefFields.Count;f++) {
					string fieldName=hl7DefMessage.hl7DefSegments[s].hl7DefFields[f].FieldName;
					if(fieldName=="") {//If fixed text instead of field name just add text to segment
						seg.SetField(hl7DefMessage.hl7DefSegments[s].hl7DefFields[f].OrdinalPos,hl7DefMessage.hl7DefSegments[s].hl7DefFields[f].FixedText);
					}
					else {
						seg.SetField(hl7DefMessage.hl7DefSegments[s].hl7DefFields[f].OrdinalPos,FieldConstructor.GenerateFieldACK(hl7Def,fieldName,controlId,isAck,ackEvent));
					}
				}
				msgHl7.Segments.Add(seg);
			}
			return msgHl7;
		}

		///<summary>Returns null if no HL7 def is enabled or no SRR is defined in the enabled def.  An SRR - Schedule Request Response message is sent when an SRM - Schedule Request Message is received.  The SRM is acknowledged just like any inbound message, but the SRR notifies the placer application that the requested modification took place.  Currently the only appointment modifications allowed are updating the appt note, setting the dentist and hygienist, updating the confirmation status, and changing the ClinicNum.  Setting the appointment status to Broken is also supported.</summary>
		public static MessageHL7 GenerateSRR(Patient pat,Appointment apt,EventTypeHL7 eventType,string controlId,bool isAck,string ackEvent) {
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
			if(hl7Def==null) {
				return null;
			}
			//find an outbound SRR message in the def
			HL7DefMessage hl7DefMessage=null;
			for(int i=0;i<hl7Def.hl7DefMessages.Count;i++) {
				if(hl7Def.hl7DefMessages[i].MessageType==MessageTypeHL7.SRR && hl7Def.hl7DefMessages[i].InOrOut==InOutHL7.Outgoing) {
					hl7DefMessage=hl7Def.hl7DefMessages[i];
					break;
				}
			}
			if(hl7DefMessage==null) {//SRR message type is not defined so do nothing and return
				return null;
			}
			if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
				pat=Patients.GetOriginalPatientForClone(pat);
			}
			MessageHL7 msgHl7=new MessageHL7(MessageTypeHL7.SRR);
			Provider provPri=Providers.GetProv(apt.ProvNum);
			//go through each segment in the def
			for(int i=0;i<hl7DefMessage.hl7DefSegments.Count;i++) {
				List<Provider> listProvs=new List<Provider>();
				listProvs.Add(provPri);
				if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.AIP && apt.ProvHyg>0) {
					listProvs.Add(Providers.GetProv(apt.ProvHyg));
				}
				for(int j=0;j<listProvs.Count;j++) {//AIP will be repeated if there is a dentist and a hygienist on the appt
					Provider prov=listProvs[j];
					SegmentHL7 seg=new SegmentHL7(hl7DefMessage.hl7DefSegments[i].SegmentName);
					seg.SetField(0,hl7DefMessage.hl7DefSegments[i].SegmentName.ToString());
					for(int k=0;k<hl7DefMessage.hl7DefSegments[i].hl7DefFields.Count;k++) {
						string fieldName=hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].FieldName;
						if(fieldName=="") {//If fixed text instead of field name just add text to segment
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].OrdinalPos,hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].FixedText);
						}
						else {
							string fieldValue="";
							if(hl7DefMessage.hl7DefSegments[i].SegmentName==SegmentNameHL7.MSA) {
								fieldValue=FieldConstructor.GenerateFieldACK(hl7Def,fieldName,controlId,isAck,ackEvent);
							}
							else {
								fieldValue=FieldConstructor.GenerateFieldSRR(hl7Def,fieldName,pat,prov,apt,j+1,eventType,seg.Name);
							}
							seg.SetField(hl7DefMessage.hl7DefSegments[i].hl7DefFields[k].OrdinalPos,fieldValue);
						}
					}
					msgHl7.Segments.Add(seg);
				}
			}
			return msgHl7;
		}

	}
}
