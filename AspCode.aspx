<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Welcome.aspx.cs" Inherits="Welcome" %>
<%@ Register src="Header.ascx" tagname="Header" tagprefix="UC1" %>
<%@ Register src="Footer.ascx" tagname="Footer" tagprefix="UC2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assignment 5</title>
</head>
<body>

    <form id="form1" runat="server" style="margin: 0px;" >
    
    <div style=" margin: 0px; background-image: url('background.jpg');">
    <UC1:Header ID="Header" runat="server" style="margin:0px;" />
    
     <div style="margin: 20px; float:right; border-style:solid; border-color:Orange; border-width:medium;">
        <asp:AdRotator ID="AdRotator1" runat="server" DataSourceID="XmlDataSource1" 
            Width="300px"   />
        <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="XmlRot.xml">
        </asp:XmlDataSource>
        <br />
    </div>  
 
    <div style="margin: 20px; float:left; border-style:solid; border-color:Orange; border-width:medium;">
        <asp:AdRotator ID="AdRotator2" runat="server" DataSourceID="XmlDataSource2" 
            Width="300px"  />
        <asp:XmlDataSource ID="XmlDataSource2" runat="server" DataFile="XmlRot2.xml">
        </asp:XmlDataSource>
    </div>  


    <div style="margin: 20px; margin-top:40px; text-align:center">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/WELCOME.gif" Width="30%" />
    </div>

    <div style="padding-left:30%; font-family:Arial Black; text-align:center; margin: 0px; ">
      
    <p>If you are our client, please<br />
    <a href="Login.aspx" style=" text-align:center; font-family:Arial Black; ">
    >>>SIGN IN<<< </a></p>
    <p>If you want to become our client, please<br />
    <a href="Register.aspx" style=" text-align:center; font-family:Arial Black; ">
    >>>REGISTER<<<</a></p>
        
    </div>
    <div style="margin-top:60px;">
    <UC2:Footer ID="Header1" runat="server"  />
    </div>
    
    </div>
    </form>
  
</body>
</html>
