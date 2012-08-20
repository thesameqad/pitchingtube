<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Investors.aspx.cs" Inherits="Test.Investors" %>

<asp:Content ID="hc" runat="server" ContentPlaceHolderID="hc"></asp:Content>
<asp:Content ID="mc" ContentPlaceHolderID="mc" runat="server">
    <div class="containerInvestors">
        <h1 class="header">DESCRIBE YOUR IDEA IN 3 WORDS...</h1>
        <table>
            <tr>
                <td style="height: 81px">
                    <asp:TextBox runat="server" ID="txtIdeaDescription" CssClass="txtIdea"></asp:TextBox>
                </td>
                <td>
                    <asp:ImageButton ID="ibGo" runat="server" ImageUrl="Images/btn-go.png" ImageAlign="AbsBottom" style="right: 10px; position: relative" />
                </td>
            </tr>
        </table>
    </div>
    <div class="boxSections">
        <div class="subheader">
            <asp:Label runat="server" ID="lblSteps"><b>3 STEPS</b> TO GO</asp:Label>
        </div>
        <div class="cell">
            <a href="#">
                <asp:Label class="sectionHeader" runat="server" ID="lblPitch">
                    <img src="Images/icon-timer.png" alt="" /><br/>
                    Prepare the <b>one minute</b> pitch</asp:Label>&nbsp;
                <img src="Images/icon-more.png" alt="" align="absbottom" /></a>
        </div>
        <div class="spacer">&nbsp;</div>
        <div class="cell">
            <a href="#">
                <asp:Label class="sectionHeader" runat="server" ID="lblShare">
                    <img src="Images/icon-lamp.png" alt="" /><br/>
                    Share it with <b>investors</b></asp:Label>&nbsp;
                <img src="Images/icon-more.png" alt="" align="absbottom" /></a>
        </div>
        <div class="spacer">&nbsp;</div>
        <div class="cell">
            <a href="#">
                <asp:Label class="sectionHeader" runat="server" ID="lblFeedback">
                    <img src="Images/icon-handshake.png" alt="" /><br/>
                    Get useful <b>contacts</b> and feedback</asp:Label>&nbsp;
                <img src="Images/icon-more.png" alt="" align="absbottom" /></a>
        </div>
    </div>
</asp:Content>