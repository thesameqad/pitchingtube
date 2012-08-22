<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Test.Registration" %>

<asp:Content ID="hc" runat="server" ContentPlaceHolderID="hc"></asp:Content>
<asp:Content ID="mc" runat="server" ContentPlaceHolderID="mc">
    <div class="containerRegistration">
        <div class="frame">
            <table style="width: 100%">
                <tr>
                    <td></td> 
                    <td colspan="3" class="style3"> REGISTRATION FORM</td>
                </tr>
                <tr class="TableTr">
                    <td class="Grand">Grand finale brings</td>
                    <td class="style1">
                        <asp:Label runat="server" id="lblName">Name</asp:Label>
                    </td>
                    <td class="style2" >
                        <asp:TextBox runat="server" id="txtName" CssClass="TextBox"></asp:TextBox>
                    </td>
                    <td class="free"></td>
                </tr>
                <tr class="TableTr">
                    <td class="Games">Games to end</td>
                    <td class="style1">
                        <asp:Label runat="server" id="Label1">Email</asp:Label>
                    </td>
                    <td class="style2">
                        <asp:TextBox runat="server" id="TextBox1" CssClass="TextBox"></asp:TextBox>
                    </td>
                    <td class="free"></td>
                </tr>
                <tr class="TableTr">
                    <td rowspan="2" class="Her"><strong>Her world record-setting victory has now been dissected more times than a medical school cadaver. Sports commentators, scientists, and swimming fans have produced charts, statistics and thousends of world both defending and questioning the authenticity of Yes performance</strong></td>
                    <td class="style1">
                        <asp:Label runat="server" id="Label2">Skype</asp:Label>
                    </td>
                    <td class="style2">
                        <asp:TextBox runat="server" id="TextBox2" CssClass="TextBox"></asp:TextBox>
                    </td>
                    <td class="free"></td>
                </tr>
                <tr class="TableTr" >
                    <td class="style1" >
                        <asp:Label runat="server" id="Label3">Phone</asp:Label>
                    </td>
                    <td class="style2" >
                        <asp:TextBox runat="server" id="TextBox3" CssClass="TextBox"></asp:TextBox>
                    </td>
                    <td class="free"></td>
                </tr>
                <tr class="TableTr">
                    <td rowspan="2" align="center" ><a href="#" >
                            <asp:Label class="sectionHeader" runat="server" ID="Label4">
                            <img src="Images/F.png" alt="" /><br/>
                            </asp:Label></a></td>
                    <td class="style1">
                        <asp:Label runat="server" id="Label5">Password</asp:Label>
                    </td>
                    <td class="style2">
                        <asp:TextBox runat="server" id="TextBox5" CssClass="TextBox" ></asp:TextBox>
                    </td>
                    <td rowspan="3" class="button"><a href="#" style="padding-left:15px; font-size:25px">
                            <asp:Label class="sectionHeader" runat="server" ID="Label7">
                            <img src="Images/btn-go.png" alt="" /><br/><span style="padding-left:25px">Start</span>
                            </asp:Label></a></td>
                </tr>
                <tr class="TableTr">
                    <td class="style1">
                        <asp:Label runat="server" id="Label6">Retype</asp:Label>
                    </td>
                    <td class="style2">
                        <asp:TextBox runat="server" id="TextBox6" CssClass="TextBox"></asp:TextBox>
                    </td>

                </tr>
                <tr class="TableTr">
                    <td style="font-size:18px" align="center">Вход через <br>Facebook</td>
                    <td class="style1">    
                    <td class="style2">
                        <asp:RadioButton runat="server" id="RadioBut2" Text="Investor"></asp:RadioButton>
                        <asp:RadioButton runat="server" id="RadioBut" Text="Intrepreneur"></asp:RadioButton>
                    </td>
                </tr>
            </table>
        </div>
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