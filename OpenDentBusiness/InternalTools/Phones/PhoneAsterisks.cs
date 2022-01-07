using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;

namespace OpenDentBusiness {
	///<summary>This entire class is only used at Open Dental, Inc HQ.  So for that special environment, many things are hard-coded.</summary>
	public class PhoneAsterisks {
		///<summary>The asterisk server ip setting can change at any time via the phone tracking server application.</summary>
		public static string AsteriskServerIp {
			get {
				return PrefC.GetString(PrefName.AsteriskServerIp);
			}
		}

		///<summary>Sets the phone to the default queue for the employee passed in in regards to the phoneempdefault table.
		///Will correctly put the phone into the Triage ring group if the PhoneEmpDefault has been flagged as IsTriageOperator
		///This method does nothing if there is no corresponding phoneempdefault row for the employee passed in.</summary>
		public static void SetToDefaultQueue(long employeeNum) {
			//No need to check RemotingRole; no call to db.
			SetToDefaultQueue(PhoneEmpDefaults.GetOne(employeeNum));
		}

		///<summary>Sets the phone to the default queue for the employee passed in in regards to the phoneempdefault table.
		///Will correctly put the phone into the Triage ring group if the PhoneEmpDefault has been flagged as IsTriageOperator
		///This method does nothing if there is no corresponding phoneempdefault row for the employee passed in.</summary>
		public static void SetToDefaultQueue(PhoneEmpDefault phoneEmpDefault) {
			//No need to check RemotingRole; no call to db.
			if(phoneEmpDefault==null) {
				return;
			}
			//If the employee was set to triage, do not set them to their default queue, instead set them to the triage queue.
			AsteriskQueues defaultQueue=(phoneEmpDefault.IsTriageOperator ? AsteriskQueues.Triage : phoneEmpDefault.RingGroups);
			PhoneAsterisks.SetQueueForExtension(phoneEmpDefault.PhoneExt,defaultQueue);
		}

		public static void SetQueueForExtension(int extension,AsteriskQueues asteriskQueue) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),extension,asteriskQueue);
				return;
			}
			//Update the phone table so that the PhoneTrackingServer knows what queue to put this extension into.
			string command="UPDATE phone SET RingGroups="+POut.Int((int)asteriskQueue)+" "
				+"WHERE Extension="+POut.Int(extension);
			Db.NonQ(command);
			//Create a custom signalod so that the queue system (new way of doing ring groups) knows how to handle this extension.
			Signalods.SetInvalid(InvalidType.PhoneAsteriskReload,KeyType.PhoneExtension,(long)extension);
		}

		///<summary>Updates the queue for the phone emp default passed in based on the ClockStatus of the current phone entry.</summary>
		public static void SetQueueForClockStatus(PhoneEmpDefault phoneEmpDefault) {
			//No need to check RemotingRole; no call to db.
			if(phoneEmpDefault==null) {
				return;
			}
			//Start with the default queue for this phone emp default because that was old behavior.
			AsteriskQueues queue;
			//Query the database to get the current ClockStatus for this phone.
			Phone phone=Phones.GetPhoneForExtensionDB(phoneEmpDefault.PhoneExt);
			switch(phone.ClockStatus) {
				case ClockStatusEnum.Backup:
					queue=AsteriskQueues.Backup;
					break;
				case ClockStatusEnum.Break:
				case ClockStatusEnum.Home:
				case ClockStatusEnum.Lunch:
				case ClockStatusEnum.OfflineAssist:
				case ClockStatusEnum.TeamAssist:
				case ClockStatusEnum.Training:
				case ClockStatusEnum.Unavailable:
				case ClockStatusEnum.WrapUp:
				case ClockStatusEnum.TCResponder:
					queue=AsteriskQueues.None;
					break;
				case ClockStatusEnum.NeedsHelp:
				case ClockStatusEnum.HelpOnTheWay:
					//Do nothing and leave the queue system completely alone by returning.
					return;
				case ClockStatusEnum.Available:
				default:
					//Use the default queue.
					queue=(phoneEmpDefault.IsTriageOperator ? AsteriskQueues.Triage : phoneEmpDefault.RingGroups);
					break;
			}
			SetQueueForExtension(phoneEmpDefault.PhoneExt,queue);
		}
		


	}


}



