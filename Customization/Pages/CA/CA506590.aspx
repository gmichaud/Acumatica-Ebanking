<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="CA506590.aspx.cs" Inherits="Page_CA506590"
    Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" PrimaryView="Filter" PageLoadBehavior="PopulateSavedValues" TypeName="NexVue.HsbcEBanking.BankTransactionsSftpImport" />
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Width="100%" Caption="Selection" DataMember="Filter">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="M" ControlSize="L" />
            <px:PXSelector CommitChanges="True" runat="server" DataField="CashAccountID" ID="edCashAccountID"  />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" ActionsPosition="Top" Caption="Files" Width="100%" SkinID="Inquire">
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
