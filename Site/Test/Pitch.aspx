<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Pitch.aspx.cs" Inherits="Test.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="hc" runat="server">
    <style type="text/css">
        .style1
        {
            height: 18px;
        }
    </style>
    </asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mc" runat="server">
    <div class="containerPitch">
        <table >
            <tr class="Tr">
                    <td class="time" ><%=DateTime.Now.Hour%>:<%=DateTime.Now.Minute%><br/><p class="question"> ASKING <br/>QUESTIONS</p></td>
                    <td class="video" rowspan="5" ><img src="Images/Video.png" alt="" /> </td>
                    <td class="investor"  >CURENT <br/> INVESTOR<br/> <img src="Images/Avatar.png" alt="" /></td>
                    <td class="style3">John Smith</td>
                </tr>
                <tr class="Tr">
                    <td ></td>
                    <td >HISTORY</td>
                    <td  ></td>
                </tr>
                <tr class="Tr">
                    <td  ></td>
                    <td > <img src="Images/historuAvator.png" alt="" /></td>
                    <td class="style4" > Billy <br/>Chester</td>
                </tr>
                <tr class="Tr">
                    <td  ></td>
                    <td ><img src="Images/historuAvator.png" alt="" /> </td>
                    <td class="style4" > Fillip<br/>Green</td>
                </tr>
                <tr class="Tr">
                    <td  ></td>
                    <td ><img src="Images/historuAvator.png" alt="" /></td>
                    <td class="style4" >Billy <br/>Chester3 </td>
                </tr>
                <tr class="Tr">
                    <td colspan="2" align="left" style="padding-left:10px; vertical-align:bottom">OTHER ROOMS</td>
                    <td ><img src="Images/historuAvator.png" alt="" /></td>
                    <td class="style4" >Fillip<br/>Green </td>
                </tr>
        </table>
        <table class="AtherRooms">
        <tr class="RoomTr">
                <td></td>
                <td class="style1" rowspan="3"><img src="Images/Romb.png" alt="" /></td>
                <td></td>
                <td></td>
                <td></td>
                <td class="style1" rowspan="3"><img src="Images/Romb.png" alt="" /></td>
                <td></td>
                <td></td>
                <td></td>
                <td class="style1" rowspan="3"><img src="Images/Romb.png" alt="" /></td>
                <td></td>
                <td></td>
                <td></td>
                <td class="style1" rowspan="3"><img src="Images/Romb.png" alt="" /></td>
            </tr>
            <tr class="RoomTr">
                <td class="LeftFoto"><img src="Images/RoomsAvator.png" alt="" /></td>
                <td ><img src="Images/RoomsAvator.png" alt="" /></td>
                <td class="Probel" ></td>
                <td class="LeftFoto"><img src="Images/RoomsAvator.png" alt="" /></td>        
                <td ><img src="Images/RoomsAvator.png" alt="" /></td>
                <td class="Probel" ></td>
                <td class="LeftFoto" ><img src="Images/RoomsAvator.png" alt="" /></td>
                <td ><img src="Images/RoomsAvator.png" alt="" /></td>
                <td class="Probel" ></td>
                <td class="LeftFoto" ><img src="Images/RoomsAvator.png" alt="" /></td>
                <td ><img src="Images/RoomsAvator.png" alt="" /></td>
            </tr>
            <tr>
                <td class="LeftRoomTrFIO">Derek <br/> Green</td>
                <td class="RightRoomTrFIO">Billy <br/> Chester</td>
                <td ></td>
                <td class="LeftRoomTrFIO"> Derek <br/> Green</td>
                <td class="RightRoomTrFIO">Billy <br/> Chester</td>
                <td></td>
                <td class="LeftRoomTrFIO">Derek <br/> Green</td>
                <td class="RightRoomTrFIO">Billy <br/> Chester</td>
                <td ></td>
                <td class="LeftRoomTrFIO">Derek <br/> Green</td>
                <td class="RightRoomTrFIO">Billy <br/> Chester</td>
            </tr>
            </table>
    </div>
</asp:Content>
