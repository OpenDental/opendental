using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OpenDentBusiness {
	public class NTPv4 {
		private const long TicksPerSecond=TimeSpan.TicksPerSecond;
		private static readonly DateTime Epoch=new DateTime(1900,1,1);

		public NTPv4() {
		}

		///<summary>Sends an NTPv4 request to the passed url and returns the offset from DateTime.Now.  Returns double.MaxValue if request timed out.  Will throw exception if nistServerUrl is invalid.</summary>
		public double getTime(string nistServerUrl) {
			byte[] arrayReceivedPacket=new byte[48];
			IPAddress[] addresses=Dns.GetHostEntry(nistServerUrl).AddressList;
			IPEndPoint ipEndPoint=new IPEndPoint(addresses[0],123);
			Socket socket=new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
			socket.ReceiveTimeout=2000;//Two seconds.  Too short?
			socket.Connect(ipEndPoint);
			//Create packet for sending then send to NIST server.
			socket.Send(MakePacket());
			try {
				socket.Receive(arrayReceivedPacket);
			}
			catch {//Response not received before Timeout.
				socket.Shutdown(SocketShutdown.Both);
				socket.Close();
				socket.Dispose();
				string messageTextFail="NTPv4 time request from "+nistServerUrl+" timed out.";
				EventLog.WriteEntry("OpenDental",messageTextFail,EventLogEntryType.Information);
				return (double.MaxValue);
			}
			//Convert the received NTP packet to a usable time stamp.
			DateTime destination=DateTime.Now.ToUniversalTime();
			DateTime originate=RawToDateTime(arrayReceivedPacket,24);
			DateTime receive=RawToDateTime(arrayReceivedPacket,32);
			DateTime transmit=RawToDateTime(arrayReceivedPacket,40);
			double offset=(((receive-originate)-(destination-transmit)).TotalMilliseconds)/2; //Offset calculation based off Ntpv4 specification.
			//Close connection
			socket.Shutdown(SocketShutdown.Both);
			socket.Close();
			socket.Dispose();
			string messageText="NTPv4 time request received from "+nistServerUrl+"."
				+"\nOriginate:  "+originate.ToString("hh:mm:ss.fff tt")
				+"\nReceive:  "+receive.ToString("hh:mm:ss.fff tt")
				+"\nTransmit:  "+transmit.ToString("hh:mm:ss.fff tt")
				+"\nDestination:  "+destination.ToString("hh:mm:ss.fff tt")
				+"\nOffset:  "+offset+" milliseconds";
			EventLog.WriteEntry("OpenDental",messageText,EventLogEntryType.Information);
			return offset;
		}

		///<summary>Convert byte array to a DateTime.</summary>
		private DateTime RawToDateTime(byte[] arraySource,int startIdx) {
			UInt64 seconds=0;
			byte[] arraySeconds=new byte[8];
			for(int i=0;i<=3;i++) {
				arraySeconds[3-i]=arraySource[startIdx + i];
			}
			seconds=BitConverter.ToUInt64(arraySeconds,0);
			UInt64 fractions=0;
			byte[] arrayFractions=new byte[8];
			for(int i=4;i<=7;i++) {
				arrayFractions[7-i]=arraySource[startIdx + i];
			}
			fractions=BitConverter.ToUInt64(arrayFractions,0);
			UInt64 ticks=(seconds * TicksPerSecond) + ((fractions * TicksPerSecond) / 0x100000000L);
			return Epoch + TimeSpan.FromTicks((Int64)ticks);
		}

		///<summary>Create request packet for sending.</summary>
		private byte[] MakePacket() {
			byte[] arrayPacket=new byte[48];
			//byte 0
			arrayPacket[0]=0x1B;//Identifies us as a Client, and using Version NTPv4
			//byte 1-39 don't fill
			//byte 40-47 (Current system time)
			UInt64 ticks=(UInt64)(DateTime.Now.ToUniversalTime()-Epoch).Ticks;
			UInt64 seconds=ticks / TicksPerSecond;
			UInt64 fractions=((ticks % TicksPerSecond) * 0x100000000L) / TicksPerSecond;
			byte[] arraySeconds=BitConverter.GetBytes(seconds);
			byte[] arrayFractions=BitConverter.GetBytes(fractions);
			for(int i=3;i>=0;i--) {
				arrayPacket[40 + i]=arraySeconds[3-i];
			}
			for(int i=7;i>=4;i--) {
				arrayPacket[40 + i]=arrayFractions[7-i];
			}
			return arrayPacket;
		}

	}

}
