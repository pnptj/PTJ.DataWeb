<%@ Page Title="Home Page" Language="C#"  Debug="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Data Explorer.</h2>
            </hgroup>
            <p>
                <asp:DropDownList ID="ddlTables" runat="server" Height="18px" Width="658px" OnSelectedIndexChanged="ddlTables_SelectedIndexChanged" AutoPostBack="True">
                </asp:DropDownList>
                <asp:Button ID="btnClearNavigation" runat="server" Text="Clear" Height ="44px" OnClick="btnClearNavigation_Click" />
                <asp:GridView ID="gvDataResults" AutoGenerateSelectButton="true"  runat="server" OnSelectedIndexChanged="gvDataResults_SelectedIndexChanged"></asp:GridView>
            </p>
        </div>
    </section>
</asp:Content>
