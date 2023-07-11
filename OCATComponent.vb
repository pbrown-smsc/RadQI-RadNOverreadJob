Imports System.Configuration
Imports Oracle.ManagedDataAccess.Client

Public Class OCATComponent
    Inherits System.ComponentModel.Component
    Public Sub FillDataSet(ByVal dset As myOCATDataSet)
        OleDbDataAdapter1.Fill(dset)
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
    Friend WithEvents OleDbDataAdapter1 As OracleDataAdapter
    Friend WithEvents OleDbSelectCommand1 As OracleCommand
    Friend WithEvents OleDbConnection1 As OracleConnection
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.OleDbDataAdapter1 = New OracleDataAdapter
        Me.OleDbSelectCommand1 = New OracleCommand
        Me.OleDbConnection1 = New OracleConnection
        '
        'OleDbDataAdapter1
        '
        Me.OleDbDataAdapter1.SelectCommand = Me.OleDbSelectCommand1
        Me.OleDbDataAdapter1.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "DOSR_STUDY", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("STUDY_REF", "STUDY_REF"), New System.Data.Common.DataColumnMapping("ACCESSION_NUMBER", "ACCESSION_NUMBER"), New System.Data.Common.DataColumnMapping("PATIENT_ID", "PATIENT_ID"), New System.Data.Common.DataColumnMapping("PATIENT_NAME", "PATIENT_NAME"), New System.Data.Common.DataColumnMapping("PATIENT_SEX", "PATIENT_SEX"), New System.Data.Common.DataColumnMapping("PATIENT_BIRTH_DATE", "PATIENT_BIRTH_DATE"), New System.Data.Common.DataColumnMapping("PATIENT_AGE", "PATIENT_AGE"), New System.Data.Common.DataColumnMapping("STUDY_DATE", "STUDY_DATE"), New System.Data.Common.DataColumnMapping("MODALITY", "MODALITY"), New System.Data.Common.DataColumnMapping("INSTITUTION_NAME", "INSTITUTION_NAME"), New System.Data.Common.DataColumnMapping("STUDY_DESCRIPTION", "STUDY_DESCRIPTION"), New System.Data.Common.DataColumnMapping("STUDY_COMMENTS", "STUDY_COMMENTS"), New System.Data.Common.DataColumnMapping("PATIENT_HISTORY", "PATIENT_HISTORY"), New System.Data.Common.DataColumnMapping("REFERRING_PHYSICIAN", "REFERRING_PHYSICIAN"), New System.Data.Common.DataColumnMapping("REQUESTING_PHYSICIAN", "REQUESTING_PHYSICIAN"), New System.Data.Common.DataColumnMapping("REQUESTING_SERVICE", "REQUESTING_SERVICE"), New System.Data.Common.DataColumnMapping("REQUESTED_PROCEDURE_CODE", "REQUESTED_PROCEDURE_CODE"), New System.Data.Common.DataColumnMapping("ORIG_READER", "ORIG_READER"), New System.Data.Common.DataColumnMapping("INSTITUTIONAL_DEPARTMENT_NAME", "INSTITUTIONAL_DEPARTMENT_NAME")})})
        '
        'OleDbSelectCommand1
        '
        Me.OleDbSelectCommand1.BindByName = True
        Me.OleDbSelectCommand1.CommandText = "SELECT STUDY_REF, ACCESSION_NUMBER, PATIENT_ID, PATIENT_NAME, PATIENT_SEX, PATIEN" &
        "T_BIRTH_DATE, PATIENT_AGE, STUDY_DATE, MODALITY, INSTITUTION_NAME, STUDY_DESCRIP" &
        "TION, STUDY_COMMENTS, PATIENT_HISTORY, REFERRING_PHYSICIAN, REQUESTING_PHYSICIAN" &
        ", REQUESTING_SERVICE, REQUESTED_PROCEDURE_CODE, 'Unknown' AS ORIG_READER," &
        " replace(PHYSICIAN_READING_STUDY,'''','') as PHYSICIAN_READING_STUDY, INSTITUTIONAL_DEPARTMENT_NAME FROM DB" &
        "ADMIN.DOSR_STUDY WHERE (ACCESSION_NUMBER = :ACCESSION_NUMBER)"
        Me.OleDbSelectCommand1.Connection = Me.OleDbConnection1
        Me.OleDbSelectCommand1.Parameters.Add(New OracleParameter(":ACCESSION_NUMBER", OracleDbType.Varchar2, 16, "ACCESSION_NUMBER"))
        '
        'OleDbConnection1
        '
        'Me.OleDbConnection1.ConnectionString = "Provider=""MSDAORA.1"";User ID=spectrum;Data Source=""MVF.WORLD"";Password=spect"
        Me.OleDbConnection1.ConnectionString = ConfigurationManager.AppSettings("EMHPACSConnStr") '  
    End Sub

#End Region

End Class
