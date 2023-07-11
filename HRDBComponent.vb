Public Class HRDBComponent
    Inherits System.ComponentModel.Component
    Public Sub FillDataSet(ByVal dSet As HREmpsDataSet)
        OleDbDataAdapter2.Fill(dSet)
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
    Friend WithEvents OleDbDataAdapter2 As System.Data.OleDb.OleDbDataAdapter
    Friend WithEvents OleDbSelectCommand2 As System.Data.OleDb.OleDbCommand
    Friend WithEvents OleDbConnection2 As System.Data.OleDb.OleDbConnection
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.OleDbDataAdapter2 = New System.Data.OleDb.OleDbDataAdapter
        Me.OleDbSelectCommand2 = New System.Data.OleDb.OleDbCommand
        Me.OleDbConnection2 = New System.Data.OleDb.OleDbConnection
        '
        'OleDbDataAdapter2
        '
        Me.OleDbDataAdapter2.SelectCommand = Me.OleDbSelectCommand2
        Me.OleDbDataAdapter2.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "Employees", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("ID", "ID"), New System.Data.Common.DataColumnMapping("PACSID", "PACSID"), New System.Data.Common.DataColumnMapping("LastName", "LastName"), New System.Data.Common.DataColumnMapping("FirstName", "FirstName"), New System.Data.Common.DataColumnMapping("FullName", "FullName"), New System.Data.Common.DataColumnMapping("Current Employee", "Current Employee"), New System.Data.Common.DataColumnMapping("Email Address", "Email Address"), New System.Data.Common.DataColumnMapping("Division", "Division"), New System.Data.Common.DataColumnMapping("Sub-division", "Sub-division"), New System.Data.Common.DataColumnMapping("RevealOverreader", "RevealOverreader")})})
        '
        'OleDbSelectCommand2
        '
        Me.OleDbSelectCommand2.CommandText = "SELECT ID, PACSID, LastName, FirstName, LastName + ', ' + LEFT (FirstName, 1) AS " &
        "FullName, [Current Employee], [Email Address], Division, [Sub-division], 0 AS Re" &
        "vealOverreader FROM dbo.vwEmployeesforRadQAReports Employees WHERE (PACSID IS NO" &
        "T NULL AND LEN(PACSID)>0) ORDER BY LastName + ', ' + LEFT (FirstName, 1)"
        Me.OleDbSelectCommand2.Connection = Me.OleDbConnection2
        '
        'OleDbConnection2
        '
        Me.OleDbConnection2.ConnectionString = Configuration.ConfigurationSettings.AppSettings("HRConnStr")

    End Sub

#End Region

End Class
