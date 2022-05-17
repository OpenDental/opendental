<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PharmacyList.aspx.cs" Inherits="MobileWeb.PharmacyList" %>
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
		<ul>
			<asp:Repeater ID="Repeater1" runat="server">
				<ItemTemplate>
					<li class="arrow style1">
						<div>
							<a linkattib="PharmacyDetails.aspx?PharmacyNum=<%#((Pharmacym)Container.DataItem).PharmacyNum %>"
								href="#PharmacyDetails">
								<div style="padding-top:4px">
								<%#((Pharmacym)Container.DataItem).StoreName%>							
								</div>
								</a>
								
						</div>
					</li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>
	</div>
</body>
</html>
