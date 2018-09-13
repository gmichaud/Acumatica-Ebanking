<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA506590.aspx.cs" Inherits="Page_CA506590"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/ListView.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Files" PageLoadBehavior="PopulateSavedValues" TypeName="NexVue.HsbcEBanking.BankTransactionsSftpImport" />
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" AllowPaging="true" AdjustPageSize="Auto" AllowSearch="true" BatchUpdate="True" Caption="Files" Width="100%" SkinID="PrimaryInquire">
        <Levels>
            <px:PXGridLevel DataMember="Files">
                <RowTemplate>
                    <px:PXTextEdit ID="txtFileName" runat="server" DataField="FileName"/>
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="Selected" Label="Selected" TextAlign="Center" Type="CheckBox" AutoCallBack="true" AllowCheckAll="true" Width="30px" />
                    <px:PXGridColumn AllowUpdate="False" DataField="FileName" Label="File Name" Width="300px" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="400" />
    </px:PXGrid>
</asp:Content>
