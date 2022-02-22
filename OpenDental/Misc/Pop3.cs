/*using System;
using System.Collections;
using System.Net.Sockets;

namespace OpenDental{
	/// <summary>Retrieves email from a POP3 server.</summary>
	public class Pop3 : System.Net.Sockets.TcpClient {
		//public Pop3(){
		//}

		/// <summary>Connect to the server.</summary>
		public void Connect(string server,string username,string password){ 
			string message; 
			string response; 
			Connect(server, 110); 
			response = Response(); 
			if (response.Substring(0, 3) != "+OK"){ 
				throw new Pop3Exception(response); 
			}
			message = "USER " + username + "\r\n"; 
			Write(message); 
			response = Response(); 
			if (response.Substring(0, 3) != "+OK"){ 
				throw new Pop3Exception(response); 
			}
			message = "PASS " + password + "\r\n"; 
			Write(message); 
			response = Response(); 
			if (response.Substring(0, 3) != "+OK") { 
				throw new Pop3Exception(response); 
			} 
		}	

		///<summary>Disconnect from the server</summary>
		public void Disconnect(){ 
			string message; 
			string response;
			message = "QUIT\r\n"; 
			Write(message); 
			response = Response(); 
			if (response.Substring(0, 3) != "+OK"){ 
				throw new Pop3Exception(response); 
			} 
		}

		///<summary>Returns a collection of Pop3Message objects, but only the number and bytes are filled in.  Each message should then be passed through Retrieve to fill in the text.</summary>
		public ArrayList List(){ 
			string message; 
			string response; 
			ArrayList retval = new ArrayList(); 
			message = "LIST\r\n"; 
			Write(message); 
			response = Response(); 
			if (response.Substring(0, 3) != "+OK"){ 
				throw new Pop3Exception(response); 
			}
			while(true){ 
				response = Response(); 
				if (response == ".\r\n"){ 
					return retval; 
				} 
				else{ 
					Pop3Message msg = new Pop3Message(); 
					char[] seps = { ' ' }; 
					string[] values = response.Split(seps); 
					msg.number = Int32.Parse(values[0]); 
					msg.bytes = Int32.Parse(values[1]); 
					msg.retrieved = false; 
					retval.Add(msg); 
					continue; 
				} 
			}//while 
		}

		///<summary>Retrieve the messages based on the previously filled list of messageNumbers.</summary>
		public Pop3Message Retrieve(Pop3Message rhs){ 
			string message; 
			string response; 
			Pop3Message msg = new Pop3Message(); 
			msg.bytes = rhs.bytes; 
			msg.number = rhs.number; 
			message = "RETR " + rhs.number + "\r\n"; 
			Write(message); 
			response = Response(); 
			if(response.Substring(0,3) != "+OK") { 
				throw new Pop3Exception(response); 
			}
			msg.retrieved = true; 
			while(true){ 
				response = Response(); 
				if (response == ".\r\n"){ 
					break; 
				} 
				else{ 
					msg.message += response; 
				} 
			}
			return msg; 
		}

		///<summary>After retrieving a message from the server, this will delete it from the server.</summary>
		public void Delete(Pop3Message rhs){ 
			string message; 
			string response;
			message = "DELE " + rhs.number + "\r\n"; 
			Write(message); 
			response = Response(); 
			if (response.Substring(0, 3) != "+OK"){ 
				throw new Pop3Exception(response); 
			} 
		}

		///<summary>Used by List, Retrieve, and Delete.</summary>
		private void Write(string message){ 
			System.Text.ASCIIEncoding en = new System.Text.ASCIIEncoding() ;
			byte[] WriteBuffer = new byte[1024] ; 
			WriteBuffer = en.GetBytes(message) ;
			NetworkStream stream = GetStream() ; 
			stream.Write(WriteBuffer, 0, WriteBuffer.Length);
			//Debug.WriteLine("WRITE:" + message); 
		}

		///<summary>Used by List, Retrieve, and Delete.</summary>
		private string Response(int length){ 
			System.Text.ASCIIEncoding enc=new System.Text.ASCIIEncoding(); 
			byte[] serverbuff=new Byte[length]; 
			NetworkStream stream=GetStream(); 
			int count = 0; 
			while(true){ 
				byte[] buff=new Byte[2]; 
				int bytes=stream.Read(buff, 0, 1 ); 
				if(bytes==1){ 
					serverbuff[count]=buff[0]; 
					count++;
					if(buff[0]=='\n'){ 
						break; 
					} 
				} 
				else{ 
					break; 
				}
			}
			string retval = enc.GetString(serverbuff, 0, count ); 
			//Debug.WriteLine("READ:" + retval); 
			return retval; 
		}

		///<summary></summary>
		private string Response(){
			return Response(1024);
		}

		sample usage
		try{ 
      Pop3 obj = new Pop3(); 
      obj.Connect("mail.xxx.com", "yyy", "zzz"); 
      ArrayList list = obj.List(); 
      foreach (Pop3Message msg in list ){ 
        Pop3Message msg2 = obj.Retrieve(msg); 
        System.Console.WriteLine("Message {0}: {1}", 
          msg2.number, msg2.message); 
      } 
      obj.Disconnect(); 
    } 
    catch(Pop3Exception e){ 
      System.Console.WriteLine(e.ToString()); 
    } 
    catch (System.Exception e) { 
			System.Console.WriteLine(e.ToString()); 
    }





	}

	public class Pop3Message { 
    public long number; 
    public long bytes; 
    public bool retrieved; 
    public string message; 
}

	public class Pop3Exception : System.ApplicationException{ 
    public Pop3Exception( string str) : base( str) { 

    } 
	}




}
*/