<%@ Control Language="C#" AutoEventWireup="true" CodeFile="edit.ascx.cs" Inherits="Widgets.FileSystemManager.Edit" %>
<%@ Import Namespace="BlogEngine.Core" %>

<style type="text/css">
    #divWrapper 
    {
        overflow: auto;
    }
</style>
<script type="text/javascript">
</script>

<%--<asp:TextBox runat="server" ID="txtText" TextMode="multiLine" Columns="100" Rows="10" style="width:700px;height:372px" /><br />--%>

<div id="divWrapper">
    <div id="divPanel">
        <p>
            <asp:Label runat="server" ID="lblCurrentPath" Text="Current Path: "></asp:Label>
            <asp:TextBox runat="server" ID="txtCurrentPath" style="width: 500px;"></asp:TextBox>
            <asp:Button runat="server" ID="txtGo" Text="Go" OnClick="btnGo_Click" />
            <asp:Button runat="server" ID="btnUp" Text="<< Up" onclick="btnUp_Click" />
        </p>
        <div id="divExplorer">
            <p>Sub Directories:</p>
            <p>New Directory: <asp:TextBox runat="server" ID="txtNewDirectoryName" Text="NewFolder"></asp:TextBox> 
                <asp:Button runat="server" ID="btnCreateNewDirectory" Text="Create" 
                    onclick="btnCreateNewDirectory_Click" /></p>
            <asp:GridView runat="server" ID="gvSubDirectories" AutoGenerateColumns="False" OnRowDeleting="gvSubDirectories_RowDeleting">
                <Columns>
                    <%--<asp:BoundField DataField="Name" HeaderText="Name" />--%>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkName" runat="server" Text='<%# Eval("Name") %>' OnClick="lnkName_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Extension" HeaderText="Extension" />
                    <asp:BoundField DataField="LastWriteTime" HeaderText="Last Modified Time" />
                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" />
                     <%--OnClientClick="return confirm('Are you sure your want to delete this folder? All sub folders and files in it will be deleted and this operation can not be recovered!');"--%>
                </Columns>
            </asp:GridView>
            <p>Files:</p>
            <p>New File: <asp:FileUpload runat="server" ID="fileUpload" /> <asp:Button runat="server" ID="btnUpload" Text="Upload" OnClick="btnUpload_Click" /></p>
            <asp:GridView runat="server" ID="gvFiles" AutoGenerateColumns="False" OnRowDeleting="gvFiles_RowDeleting">
                <Columns>
                    <%--<asp:BoundField DataField="Name" HeaderText="Name" />--%>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkFileName" runat="server" Text='<%# Eval("Name") %>' OnClick="lnkFileName_Click"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Extension" HeaderText="Extension" />
                    <asp:BoundField DataField="Length" HeaderText="Size" />
                    <asp:BoundField DataField="LastWriteTime" HeaderText="Last Modified Time" />
                    <asp:BoundField DataField="IsReadOnly" HeaderText="Is Readonly" />
                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
    <div id="divStatus">
        <p>Error Logs:</p>
        <asp:TextBox runat="server" ID="txtLog" TextMode="MultiLine" Columns="100" Rows="20" style="width: 700px; height: 150px;"></asp:TextBox>
    </div>
</div>