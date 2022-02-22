<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppointmentFilter.aspx.cs" Inherits="MobileWeb.AppointmentFilter" %>
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
	<div style="height:15px;"></div>
	<span class="style1" style="margin-left:10px;font-weight:bold;">Choose Provider:</span>
	<select class="style1" style="margin-left:20px;font-weight:bold;"  id="provlist">
		<option value="0">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;All
						&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
		</option>
			<asp:Repeater ID="Repeater1" runat="server">
				<ItemTemplate>
				<option value="<%#((Providerm)Container.DataItem).ProvNum %>"<%#GetSelected((Providerm)Container.DataItem)%>">
				<%#((Providerm)Container.DataItem).Abbr%>
				</option>
				</ItemTemplate>
			</asp:Repeater>
	</select>
	<div style="height:100px"></div>
	</div>
	
</body>
</html>
