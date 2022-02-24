<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatientList.aspx.cs" Inherits="MobileWeb.PatientList" %>
<%@ Import namespace="OpenDentBusiness.Mobile" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
</head>
<body>
	<div id="loggedin"><asp:Literal runat="server" ID="Message"></asp:Literal></div>
	<div id="content">
	<div class="styleError">  
				 <asp:Label ID="LabelError" runat="server" Text=""></asp:Label>
	</div>
	<div style="position:relative;left:25px"><asp:Literal runat="server" ID="MessageNoPatients"></asp:Literal></div>
		<ul>
			<asp:Repeater ID="Repeater1" runat="server">
				<ItemTemplate>
					<li class="arrow style1">
						<div>
							<a linkattib="PatientDetails.aspx?PatNum=<%#((OpenDentBusiness.Mobile.Patientm)Container.DataItem).PatNum %>"
								href="#PatientDetails">
								<%#GetPatientName(((Patientm)Container.DataItem).PatNum)%> &nbsp;&nbsp;&nbsp;&nbsp;<%#((OpenDentBusiness.Mobile.Patientm)Container.DataItem).Birthdate.ToShortDateString()%>
								</a>
						</div>
					</li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>
	</div>
</body>
</html>
