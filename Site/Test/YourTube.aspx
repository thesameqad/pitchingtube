<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="YourTube.aspx.cs" Inherits="Test.YourTube" %>
<asp:Content ID="Content1" ContentPlaceHolderID="hc" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mc" runat="server">
    <div class="ConteinerPpreloader">
       <div class="Preloader"><img src="Images/301.gif"></div>
       <p class="Searching">Searching for the pitching tube.</p>
       <p class="Minutes">This may take a few minutes.</p>
       <div class="Message">
           <table class="title"> 
            <tr>
               <td colspan="2"> No free tube were found </td>
               <td></td>
            </tr>
            <tr>
                <td><asp:Button runat="server" id="Button1" CssClass="Button1" Text="Main page" ForeColor="White"></asp:Button></td>
                <td><asp:Button runat="server" id="Button2" CssClass="Button2" Text="Search again" ForeColor="White"></asp:Button></td>
            </tr>
           </table>
       </div>
    </div>
</asp:Content>
