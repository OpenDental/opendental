<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppointmentDetails.aspx.cs" Inherits="MobileWeb.AppointmentDetails" %>
<%@ Import namespace="OpenDentBusiness.Mobile" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Appointment</title>
</head>
<body>
<div id="loggedin"><asp:Literal runat="server" ID="Message"></asp:Literal></div>
<div id="content">
<div class="styleError">  
				 <asp:Label ID="LabelError" runat="server" Text=""></asp:Label>
</div>
	<ul>
		<li class="arrow style1">
		<div>
			<a linkattib="PatientDetails.aspx?PatNum=<%Response.Write(pat.PatNum);%>" href="#PatientDetails">
			<asp:Label ID="Label1" runat="server" Text=""><%Response.Write(PatName);%></asp:Label></a>
		</div>
		</li>
	</ul>

<ul>
	<li class="style1">
		<div>
			<div style="float:left;margin-right:15px;display:table-cell;">
				<%Response.Write(apt.AptDateTime.ToShortDateString());%>&nbsp;&nbsp;<%Response.Write(apt.AptDateTime.ToString("dddd"));%><br />
					<div><%Response.Write(apt.AptDateTime.ToString("hh:mm tt"));%>, <%Response.Write((apt.Pattern.Length*5).ToString()+" min");%></div>
			</div>
			<div style="display:table-cell;">
					<div style="word-wrap:break-word;white-space:normal;"><%Response.Write(apt.ProcDescript);%></div>
					<div class="infocolumn"><%Response.Write(apt.Note);%></div>
			</div>
			<div style='clear:both'></div>								
		</div>			
	</li>
</ul>
</div>

</body>
</html>
