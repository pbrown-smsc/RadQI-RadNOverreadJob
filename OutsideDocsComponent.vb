Public Class OutsideDocsComponent
    Inherits System.ComponentModel.Component
    Public Sub FillDataSet(ByVal dset As OutsideDocsDataSet)
        Me.OleDbDataAdapter1.Fill(dset)
    End Sub
#Region " Component Designer generated code "

    Public Sub New(ByVal Container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        Container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    Friend WithEvents OleDbDataAdapter1 As System.Data.OleDb.OleDbDataAdapter
    Friend WithEvents OleDbConnection1 As System.Data.OleDb.OleDbConnection
    Friend WithEvents OleDbSelectCommand1 As System.Data.OleDb.OleDbCommand
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.OleDbDataAdapter1 = New System.Data.OleDb.OleDbDataAdapter
        Me.OleDbSelectCommand1 = New System.Data.OleDb.OleDbCommand
        Me.OleDbConnection1 = New System.Data.OleDb.OleDbConnection
        '
        'OleDbDataAdapter1
        '
        Me.OleDbDataAdapter1.SelectCommand = Me.OleDbSelectCommand1
        Me.OleDbDataAdapter1.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "OutsideDocs", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("ID", "ID"), New System.Data.Common.DataColumnMapping("PACSID", "PACSID"), New System.Data.Common.DataColumnMapping("LastName", "LastName"), New System.Data.Common.DataColumnMapping("FirstName", "FirstName"), New System.Data.Common.DataColumnMapping("FullName", "FullName"), New System.Data.Common.DataColumnMapping("CurrentEmployee", "CurrentEmployee"), New System.Data.Common.DataColumnMapping("Email Address", "Email Address"), New System.Data.Common.DataColumnMapping("Division", "Division"), New System.Data.Common.DataColumnMapping("Sub-division", "Sub-division")})})
        '
        'OleDbSelectCommand1
        '
        Me.OleDbSelectCommand1.CommandText = "SELECT ID, ReadingRadID AS PACSID, '' AS LastName, '' AS FirstName, ReadingRadNam" & _
        "e AS FullName, - 1 AS CurrentEmployee, [Email Address], '' AS Division, '' AS [S" & _
        "ub-division] FROM dbo.OutsideDocs"
        Me.OleDbSelectCommand1.Connection = Me.OleDbConnection1
        '
        'OleDbConnection1
        '
        'Me.OleDbConnection1.ConnectionString = "User ID=radqaN;Tag with column collation when possible=False;Data Source=smg-db-hag;P" & _
        '"assword=Strong402;Initial Catalog=RadQANorth;Use Procedure for Prepare=1;Auto Tr" & _
        '"anslate=True;Persist Security Info=True;Provider=""SQLOLEDB.1"";Workstation ID=PBR" & _
        '"OWNLAPTOP;Use Encryption for Data=False;Packet Size=4096"
        Me.OleDbConnection1.ConnectionString = Configuration.ConfigurationSettings.AppSettings("OutsideDocsConnStr")


    End Sub

#End Region

End Class
