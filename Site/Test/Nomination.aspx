<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Nomination.aspx.cs" Inherits="Test.Nomination" %>
<asp:Content ID="Content1" ContentPlaceHolderID="hc" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mc" runat="server">
    <div class="ConteinerNomination">
        <p style="font-size:22px;padding-left:10px">INVESTOR</p>
        <table class="">
            <tr class="Tr">
                <td> <img src="Images/NominationAvator.png"> </td>
                <td class="FIO"> Billy <br> Chester</td>
                <td class="Idea"> IDEA <br><p class="DescribeIdea">One day, a poor man why had only one piece </p></td>
                <td>
                    <img src="Images/BlackStar.png" onclick="func1()" onmousemove="SelectStar()">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                </td>
            </tr>
            <tr class="Tr">
                <td> <img src="Images/NominationAvator.png"> </td>
                <td class="FIO"> Bobby <br> Green</td>
                <td class="Idea"> IDEA <br><p class="DescribeIdea">One day, a poor man why had only one piece </p></td>
                <td>
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                </td>
            </tr>
            <tr class="Tr">
                <td> <img src="Images/NominationAvator.png"> </td>
                <td class="FIO"> Billy <br> Chester</td>
                <td class="Idea"> IDEA <br><p class="DescribeIdea">One day, a poor man why had only one piece </p></td>
                <td>
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                </td>
            </tr>
            <tr class="Tr">
                <td> <img src="Images/NominationAvator.png"> </td>
                <td class="FIO"> Bobby <br> Green</td>
                <td class="Idea"> IDEA <br><p class="DescribeIdea">One day, a poor man why had only one piece </p></td>
                <td>
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                </td>
            </tr>
            <tr class="Tr">
                <td> <img src="Images/NominationAvator.png"> </td>
                <td class="FIO"> Bobby <br> Green</td>
                <td class="Idea"> IDEA <br><p class="DescribeIdea">One day, a poor man why had only one piece </p></td>
                <td>
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                    <img src="Images/BlackStar.png">
                </td>
            </tr>
        </table>
        <asp:Button runat="server" id="Button1" CssClass="Button1" Text="Done" ForeColor="White"></asp:Button><p style="font-size:18px; float:right">If you are finished, puch </p>
    </div>
</asp:Content>
