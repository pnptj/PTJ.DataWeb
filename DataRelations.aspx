<%@ Page Title="Home Page" Language="C#"  Debug="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="DataRelations.aspx.cs" Inherits="DataRelations" %>

<asp:Content runat="server" ID="FeaturedContent" ContentPlaceHolderID="FeaturedContent">
    <section class="featured2">
        <div class="content-wrapper">
            <hgroup class="title">
                <h1><%: Title %>.</h1>
                <h2>Choose Datarelation for .</h2>
              </hgroup>
              <asp:TextBox ID="txtTableName" runat="server" BackColor="#c8c8c8"></asp:TextBox>
                 <hgroup>
                 </hgroup>
            <hgroup>
            </hgroup>

            <p>
                 <asp:GridView ID="gvDataResults" AutoGenerateSelectButton="true"  runat="server" OnSelectedIndexChanged="gvDataResults_SelectedIndexChanged"></asp:GridView>
            </p>
        </div>
    </section>
</asp:Content>