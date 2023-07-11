Imports SiteChiefClassLibrary
Public Class QAOverreadcomponent
    Inherits System.ComponentModel.Component
    Implements IRadQASiteChief
    Public Sub FillDataSet(ByVal dset As myOverreadDataSet)
        SqlDataAdapter1.Fill(dset, "Overread")
        AltIDsAdapter.Fill(dset, "RadAlternateIDs")
    End Sub
    Public Sub FillSiteChiefTable(ByVal dt As DataTable) Implements IRadQASiteChief.FillSiteChiefDataTable
        SqlDataAdapter3.Fill(dt)
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
    Friend WithEvents SqlDataAdapter1 As System.Data.SqlClient.SqlDataAdapter
    Friend WithEvents SqlConnection2 As System.Data.SqlClient.SqlConnection
    Friend WithEvents SqlDataAdapter2 As System.Data.SqlClient.SqlDataAdapter
    Friend WithEvents SqlSelectCommand1 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlInsertCommand1 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlUpdateCommand1 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlDeleteCommand1 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlSelectCommand2 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlInsertCommand2 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlUpdateCommand2 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlDeleteCommand2 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlSelectCommand3 As System.Data.SqlClient.SqlCommand
    Friend WithEvents AltIDsAdapter As System.Data.SqlClient.SqlDataAdapter
    Friend WithEvents SqlDataAdapter3 As System.Data.SqlClient.SqlDataAdapter
    Friend WithEvents SqlSelectCommand4 As System.Data.SqlClient.SqlCommand
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SqlDataAdapter1 = New System.Data.SqlClient.SqlDataAdapter
        Me.SqlDeleteCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlConnection2 = New System.Data.SqlClient.SqlConnection
        Me.SqlInsertCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlSelectCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlUpdateCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlDataAdapter2 = New System.Data.SqlClient.SqlDataAdapter
        Me.SqlDeleteCommand2 = New System.Data.SqlClient.SqlCommand
        Me.SqlInsertCommand2 = New System.Data.SqlClient.SqlCommand
        Me.SqlSelectCommand2 = New System.Data.SqlClient.SqlCommand
        Me.SqlUpdateCommand2 = New System.Data.SqlClient.SqlCommand
        Me.AltIDsAdapter = New System.Data.SqlClient.SqlDataAdapter
        Me.SqlSelectCommand3 = New System.Data.SqlClient.SqlCommand
        Me.SqlDataAdapter3 = New System.Data.SqlClient.SqlDataAdapter
        Me.SqlSelectCommand4 = New System.Data.SqlClient.SqlCommand
        '
        'SqlDataAdapter1
        '
        Me.SqlDataAdapter1.DeleteCommand = Me.SqlDeleteCommand1
        Me.SqlDataAdapter1.InsertCommand = Me.SqlInsertCommand1
        Me.SqlDataAdapter1.SelectCommand = Me.SqlSelectCommand1
        Me.SqlDataAdapter1.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "Overread", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("Accession", "Accession"), New System.Data.Common.DataColumnMapping("ServiceSite", "ServiceSite"), New System.Data.Common.DataColumnMapping("InitReadingRad", "InitReadingRad"), New System.Data.Common.DataColumnMapping("ReadingRes", "ReadingRes"), New System.Data.Common.DataColumnMapping("OncallReading", "OncallReading"), New System.Data.Common.DataColumnMapping("OverreadingRad", "OverreadingRad"), New System.Data.Common.DataColumnMapping("Grade", "Grade"), New System.Data.Common.DataColumnMapping("GradeComments", "GradeComments"), New System.Data.Common.DataColumnMapping("InitialReadStamp", "InitialReadStamp"), New System.Data.Common.DataColumnMapping("OverreadStamp", "OverreadStamp"), New System.Data.Common.DataColumnMapping("Status", "Status"), New System.Data.Common.DataColumnMapping("ConfCase", "ConfCase"), New System.Data.Common.DataColumnMapping("AddendumSuggested", "AddendumSuggested"), New System.Data.Common.DataColumnMapping("CommunicatedTo", "CommunicatedTo"), New System.Data.Common.DataColumnMapping("CommunicatedStamp", "CommunicatedStamp"), New System.Data.Common.DataColumnMapping("Communicated", "Communicated"), New System.Data.Common.DataColumnMapping("ClinSignifScore", "ClinSignifScore"), New System.Data.Common.DataColumnMapping("DiscrepancyCognitive", "DiscrepancyCognitive"), New System.Data.Common.DataColumnMapping("DiscrepancyCommun", "DiscrepancyCommun"), New System.Data.Common.DataColumnMapping("DiscrepancySyntax", "DiscrepancySyntax")})})
        Me.SqlDataAdapter1.UpdateCommand = Me.SqlUpdateCommand1
        '
        'SqlDeleteCommand1
        '
        Me.SqlDeleteCommand1.CommandText = "DELETE FROM dbo.Overread WHERE (Accession = @Original_Accession) AND (ConfCase = " & _
        "@Original_ConfCase OR @Original_ConfCase IS NULL AND ConfCase IS NULL) AND (Grad" & _
        "e = @Original_Grade OR @Original_Grade IS NULL AND Grade IS NULL) AND (InitReadi" & _
        "ngRad = @Original_InitReadingRad OR @Original_InitReadingRad IS NULL AND InitRea" & _
        "dingRad IS NULL) AND (InitialReadStamp = @Original_InitialReadStamp OR @Original" & _
        "_InitialReadStamp IS NULL AND InitialReadStamp IS NULL) AND (OverreadStamp = @Or" & _
        "iginal_OverreadStamp OR @Original_OverreadStamp IS NULL AND OverreadStamp IS NUL" & _
        "L) AND (OverreadingRad = @Original_OverreadingRad OR @Original_OverreadingRad IS" & _
        " NULL AND OverreadingRad IS NULL) AND (ReadingRes = @Original_ReadingRes OR @Ori" & _
        "ginal_ReadingRes IS NULL AND ReadingRes IS NULL) AND (ServiceSite = @Original_Se" & _
        "rviceSite OR @Original_ServiceSite IS NULL AND ServiceSite IS NULL) AND (Status " & _
        "= @Original_Status OR @Original_Status IS NULL AND Status IS NULL)"
        Me.SqlDeleteCommand1.Connection = Me.SqlConnection2
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Accession", System.Data.SqlDbType.VarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Accession", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ConfCase", System.Data.SqlDbType.Bit, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ConfCase", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Grade", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Grade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitReadingRad", System.Data.SqlDbType.VarChar, 30, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitReadingRad", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverreadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverreadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverreadingRad", System.Data.SqlDbType.VarChar, 15, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverreadingRad", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ReadingRes", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ReadingRes", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ServiceSite", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ServiceSite", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Status", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Status", System.Data.DataRowVersion.Original, Nothing))
        '
        'SqlConnection2
        '
        'Me.SqlConnection2.ConnectionString = "workstation id=SMGINTRANET2;packet size=4096;user id=radqaN;data source=smgsql;pe" & _
        '"rsist security info=True;initial catalog=RadQANorth;password=Strong402"
        Me.SqlConnection2.ConnectionString = Configuration.ConfigurationSettings.AppSettings("QAOverreadConnStr")
        '
        'SqlInsertCommand1
        '
        Me.SqlInsertCommand1.CommandText = "INSERT INTO dbo.Overread(Accession, ServiceSite, InitReadingRad, ReadingRes, Onca" & _
        "llReading, OverreadingRad, Grade, GradeComments, InitialReadStamp, OverreadStamp" & _
        ", Status, ConfCase) VALUES (@Accession, @ServiceSite, @InitReadingRad, @ReadingR" & _
        "es, @OncallReading, @OverreadingRad, @Grade, @GradeComments, @InitialReadStamp, " & _
        "@OverreadStamp, @Status, @ConfCase); SELECT Accession, ServiceSite, InitReadingR" & _
        "ad, ReadingRes, OncallReading, OverreadingRad, Grade, GradeComments, InitialRead" & _
        "Stamp, OverreadStamp, Status, ConfCase FROM dbo.Overread WHERE (Accession = @Acc" & _
        "ession)"
        Me.SqlInsertCommand1.Connection = Me.SqlConnection2
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Accession", System.Data.SqlDbType.VarChar, 20, "Accession"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ServiceSite", System.Data.SqlDbType.VarChar, 6, "ServiceSite"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitReadingRad", System.Data.SqlDbType.VarChar, 30, "InitReadingRad"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ReadingRes", System.Data.SqlDbType.VarChar, 6, "ReadingRes"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OncallReading", System.Data.SqlDbType.VarChar, 2147483647, "OncallReading"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverreadingRad", System.Data.SqlDbType.VarChar, 15, "OverreadingRad"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Grade", System.Data.SqlDbType.VarChar, 1, "Grade"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@GradeComments", System.Data.SqlDbType.VarChar, 2147483647, "GradeComments"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReadStamp", System.Data.SqlDbType.DateTime, 8, "InitialReadStamp"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverreadStamp", System.Data.SqlDbType.DateTime, 8, "OverreadStamp"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Status", System.Data.SqlDbType.VarChar, 1, "Status"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ConfCase", System.Data.SqlDbType.Bit, 1, "ConfCase"))
        '
        'SqlSelectCommand1
        '
        'Me.SqlSelectCommand1.CommandText = "SELECT Accession, ServiceSite, InitReadingRad, ReadingRes, OncallReading, Overrea" & _
        '"dingRad, Grade, GradeComments, InitialReadStamp, OverreadStamp, Status, ConfCase" & _
        '" FROM dbo.Overread order by Accession"
        Me.SqlSelectCommand1.CommandText = "SELECT Accession, ServiceSite, InitReadingRad, ReadingRes, OncallReading, Overrea" &
        "dingRad, Grade, GradeComments, InitialReadStamp, OverreadStamp, Status, ConfCase, AddendumSuggested, CommunicatedTo, CommunicatedStamp, Communicated, ClinSignifScore, DiscrepancyCognitive, DiscrepancyCommun,DiscrepancySyntax" &
        " FROM dbo.Overread"
        Me.SqlSelectCommand1.Connection = Me.SqlConnection2
        '
        'SqlUpdateCommand1
        '
        Me.SqlUpdateCommand1.CommandText = "UPDATE dbo.Overread SET Accession = @Accession, ServiceSite = @ServiceSite, InitR" & _
        "eadingRad = @InitReadingRad, ReadingRes = @ReadingRes, OncallReading = @OncallRe" & _
        "ading, OverreadingRad = @OverreadingRad, Grade = @Grade, GradeComments = @GradeC" & _
        "omments, InitialReadStamp = @InitialReadStamp, OverreadStamp = @OverreadStamp, S" & _
        "tatus = @Status, ConfCase = @ConfCase WHERE (Accession = @Original_Accession) AN" & _
        "D (ConfCase = @Original_ConfCase OR @Original_ConfCase IS NULL AND ConfCase IS N" & _
        "ULL) AND (Grade = @Original_Grade OR @Original_Grade IS NULL AND Grade IS NULL) " & _
        "AND (InitReadingRad = @Original_InitReadingRad OR @Original_InitReadingRad IS NU" & _
        "LL AND InitReadingRad IS NULL) AND (InitialReadStamp = @Original_InitialReadStam" & _
        "p OR @Original_InitialReadStamp IS NULL AND InitialReadStamp IS NULL) AND (Overr" & _
        "eadStamp = @Original_OverreadStamp OR @Original_OverreadStamp IS NULL AND Overre" & _
        "adStamp IS NULL) AND (OverreadingRad = @Original_OverreadingRad OR @Original_Ove" & _
        "rreadingRad IS NULL AND OverreadingRad IS NULL) AND (ReadingRes = @Original_Read" & _
        "ingRes OR @Original_ReadingRes IS NULL AND ReadingRes IS NULL) AND (ServiceSite " & _
        "= @Original_ServiceSite OR @Original_ServiceSite IS NULL AND ServiceSite IS NULL" & _
        ") AND (Status = @Original_Status OR @Original_Status IS NULL AND Status IS NULL)" & _
        "; SELECT Accession, ServiceSite, InitReadingRad, ReadingRes, OncallReading, Over" & _
        "readingRad, Grade, GradeComments, InitialReadStamp, OverreadStamp, Status, ConfC" & _
        "ase FROM dbo.Overread WHERE (Accession = @Accession)"
        Me.SqlUpdateCommand1.Connection = Me.SqlConnection2
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Accession", System.Data.SqlDbType.VarChar, 20, "Accession"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ServiceSite", System.Data.SqlDbType.VarChar, 6, "ServiceSite"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitReadingRad", System.Data.SqlDbType.VarChar, 30, "InitReadingRad"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ReadingRes", System.Data.SqlDbType.VarChar, 6, "ReadingRes"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OncallReading", System.Data.SqlDbType.VarChar, 2147483647, "OncallReading"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverreadingRad", System.Data.SqlDbType.VarChar, 15, "OverreadingRad"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Grade", System.Data.SqlDbType.VarChar, 1, "Grade"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@GradeComments", System.Data.SqlDbType.VarChar, 2147483647, "GradeComments"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReadStamp", System.Data.SqlDbType.DateTime, 8, "InitialReadStamp"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverreadStamp", System.Data.SqlDbType.DateTime, 8, "OverreadStamp"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Status", System.Data.SqlDbType.VarChar, 1, "Status"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ConfCase", System.Data.SqlDbType.Bit, 1, "ConfCase"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Accession", System.Data.SqlDbType.VarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Accession", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ConfCase", System.Data.SqlDbType.Bit, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ConfCase", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Grade", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Grade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitReadingRad", System.Data.SqlDbType.VarChar, 30, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitReadingRad", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverreadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverreadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverreadingRad", System.Data.SqlDbType.VarChar, 15, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverreadingRad", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ReadingRes", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ReadingRes", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ServiceSite", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ServiceSite", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Status", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Status", System.Data.DataRowVersion.Original, Nothing))
        '
        'SqlDataAdapter2
        '
        Me.SqlDataAdapter2.DeleteCommand = Me.SqlDeleteCommand2
        Me.SqlDataAdapter2.InsertCommand = Me.SqlInsertCommand2
        Me.SqlDataAdapter2.SelectCommand = Me.SqlSelectCommand2

        Me.SqlDataAdapter2.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "QAComplete", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("SMGID", "SMGID"), New System.Data.Common.DataColumnMapping("StudyRef", "StudyRef"), New System.Data.Common.DataColumnMapping("AccessionNumber", "AccessionNumber"), New System.Data.Common.DataColumnMapping("MRN", "MRN"), New System.Data.Common.DataColumnMapping("Grade", "Grade"), New System.Data.Common.DataColumnMapping("InitialReader", "InitialReader"), New System.Data.Common.DataColumnMapping("OverReader", "OverReader"), New System.Data.Common.DataColumnMapping("PatientName", "PatientName"), New System.Data.Common.DataColumnMapping("PatientSex", "PatientSex"), New System.Data.Common.DataColumnMapping("DOS", "DOS"), New System.Data.Common.DataColumnMapping("Modality", "Modality"), New System.Data.Common.DataColumnMapping("ProcedureName", "ProcedureName"), New System.Data.Common.DataColumnMapping("StudyComments", "StudyComments"), New System.Data.Common.DataColumnMapping("PatientHistory", "PatientHistory"), New System.Data.Common.DataColumnMapping("ReferringPhysician", "ReferringPhysician"), New System.Data.Common.DataColumnMapping("RequestingPhysician", "RequestingPhysician"), New System.Data.Common.DataColumnMapping("CPTCode", "CPTCode"), New System.Data.Common.DataColumnMapping("ExtractDate", "ExtractDate"), New System.Data.Common.DataColumnMapping("OrderNumber", "OrderNumber"), New System.Data.Common.DataColumnMapping("Status", "Status"), New System.Data.Common.DataColumnMapping("ServiceSite", "ServiceSite"), New System.Data.Common.DataColumnMapping("ReadingRes", "ReadingRes"), New System.Data.Common.DataColumnMapping("InitialReadStamp", "InitialReadStamp"), New System.Data.Common.DataColumnMapping("OverreadStamp", "OverreadStamp"), New System.Data.Common.DataColumnMapping("PerfSite", "PerfSite"), New System.Data.Common.DataColumnMapping("PatientDOB", "PatientDOB"), New System.Data.Common.DataColumnMapping("ConfCase", "ConfCase"), New System.Data.Common.DataColumnMapping("AddendumSuggested", "AddendumSuggested"), New System.Data.Common.DataColumnMapping("CommunicatedTo", "CommunicatedTo"), New System.Data.Common.DataColumnMapping("CommunicatedStamp", "CommunicatedStamp"), New System.Data.Common.DataColumnMapping("Communicated", "Communicated"), New System.Data.Common.DataColumnMapping("ClinSignifScore", "ClinSignifScore"), New System.Data.Common.DataColumnMapping("PerfSiteDept", "PerfSiteDept"), New System.Data.Common.DataColumnMapping("DiscrepancyCognitive", "DiscrepancyCognitive"), New System.Data.Common.DataColumnMapping("DiscrepancyCommun", "DiscrepancyCommun"), New System.Data.Common.DataColumnMapping("DiscrepancySyntax", "DiscrepancySyntax")})})
        Me.SqlDataAdapter2.UpdateCommand = Me.SqlUpdateCommand2
        '
        'SqlDeleteCommand2
        '
        Me.SqlDeleteCommand2.CommandText = "DELETE FROM dbo.QAComplete WHERE (SMGID = @Original_SMGID) AND (AccessionNumber =" & _
        " @Original_AccessionNumber) AND (CPTCode = @Original_CPTCode OR @Original_CPTCod" & _
        "e IS NULL AND CPTCode IS NULL) AND (ConfCase = @Original_ConfCase OR @Original_C" & _
        "onfCase IS NULL AND ConfCase IS NULL) AND (DOS = @Original_DOS OR @Original_DOS " & _
        "IS NULL AND DOS IS NULL) AND (ExtractDate = @Original_ExtractDate OR @Original_E" & _
        "xtractDate IS NULL AND ExtractDate IS NULL) AND (Grade = @Original_Grade OR @Ori" & _
        "ginal_Grade IS NULL AND Grade IS NULL) AND (InitialReadStamp = @Original_Initial" & _
        "ReadStamp OR @Original_InitialReadStamp IS NULL AND InitialReadStamp IS NULL) AN" & _
        "D (InitialReader = @Original_InitialReader OR @Original_InitialReader IS NULL AN" & _
        "D InitialReader IS NULL) AND (MRN = @Original_MRN OR @Original_MRN IS NULL AND M" & _
        "RN IS NULL) AND (Modality = @Original_Modality OR @Original_Modality IS NULL AND" & _
        " Modality IS NULL) AND (OrderNumber = @Original_OrderNumber OR @Original_OrderNu" & _
        "mber IS NULL AND OrderNumber IS NULL) AND (OverReader = @Original_OverReader OR " & _
        "@Original_OverReader IS NULL AND OverReader IS NULL) AND (OverreadStamp = @Origi" & _
        "nal_OverreadStamp OR @Original_OverreadStamp IS NULL AND OverreadStamp IS NULL) " & _
        "AND (PatientDOB = @Original_PatientDOB OR @Original_PatientDOB IS NULL AND Patie" & _
        "ntDOB IS NULL) AND (PatientName = @Original_PatientName OR @Original_PatientName" & _
        " IS NULL AND PatientName IS NULL) AND (PatientSex = @Original_PatientSex OR @Ori" & _
        "ginal_PatientSex IS NULL AND PatientSex IS NULL) AND (PerfSite = @Original_PerfS" & _
        "ite OR @Original_PerfSite IS NULL AND PerfSite IS NULL) AND (ProcedureName = @Or" & _
        "iginal_ProcedureName OR @Original_ProcedureName IS NULL AND ProcedureName IS NUL" & _
        "L) AND (ReadingRes = @Original_ReadingRes OR @Original_ReadingRes IS NULL AND Re" & _
        "adingRes IS NULL) AND (ReferringPhysician = @Original_ReferringPhysician OR @Ori" & _
        "ginal_ReferringPhysician IS NULL AND ReferringPhysician IS NULL) AND (Requesting" & _
        "Physician = @Original_RequestingPhysician OR @Original_RequestingPhysician IS NU" & _
        "LL AND RequestingPhysician IS NULL) AND (ServiceSite = @Original_ServiceSite OR " & _
        "@Original_ServiceSite IS NULL AND ServiceSite IS NULL) AND (Status = @Original_S" & _
        "tatus OR @Original_Status IS NULL AND Status IS NULL) AND (StudyRef = @Original_" & _
        "StudyRef OR @Original_StudyRef IS NULL AND StudyRef IS NULL)"
        Me.SqlDeleteCommand2.Connection = Me.SqlConnection2
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_SMGID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "SMGID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_AccessionNumber", System.Data.SqlDbType.VarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "AccessionNumber", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_CPTCode", System.Data.SqlDbType.VarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "CPTCode", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ConfCase", System.Data.SqlDbType.Bit, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ConfCase", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DOS", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DOS", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ExtractDate", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ExtractDate", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Grade", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Grade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReader", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReader", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_MRN", System.Data.SqlDbType.VarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "MRN", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Modality", System.Data.SqlDbType.VarChar, 5, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Modality", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OrderNumber", System.Data.SqlDbType.VarChar, 12, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OrderNumber", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverReader", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverReader", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverreadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverreadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PatientDOB", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PatientDOB", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PatientName", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PatientName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PatientSex", System.Data.SqlDbType.VarChar, 5, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PatientSex", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PerfSite", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PerfSite", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ProcedureName", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ProcedureName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ReadingRes", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ReadingRes", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ReferringPhysician", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ReferringPhysician", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_RequestingPhysician", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RequestingPhysician", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ServiceSite", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ServiceSite", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Status", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Status", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_StudyRef", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "StudyRef", System.Data.DataRowVersion.Original, Nothing))
        '
        'SqlInsertCommand2
        '
        Me.SqlInsertCommand2.CommandText = "INSERT INTO dbo.QAComplete(StudyRef, AccessionNumber, MRN, Grade, InitialReader, " &
        "OverReader, PatientName, PatientSex, DOS, Modality, ProcedureName, StudyComments" &
        ", PatientHistory, ReferringPhysician, RequestingPhysician, CPTCode, ExtractDate," &
        " OrderNumber, Status, ServiceSite, ReadingRes, InitialReadStamp, OverreadStamp, " &
        "PerfSite, PatientDOB, ConfCase, AddendumSuggested, CommunicatedTo, CommunicatedStamp, Communicated,ClinSignifScore,PerfSiteDept, DiscrepancyCognitive, DiscrepancyCommun,DiscrepancySyntax) VALUES (@StudyRef, @AccessionNumber, @MRN, @Grad" &
        "e, @InitialReader, @OverReader, @PatientName, @PatientSex, @DOS, @Modality, @Pro" &
        "cedureName, @StudyComments, @PatientHistory, @ReferringPhysician, @RequestingPhy" &
        "sician, @CPTCode, @ExtractDate, @OrderNumber, @Status, @ServiceSite, @ReadingRes" &
        ", @InitialReadStamp, @OverreadStamp, @PerfSite, @PatientDOB, @ConfCase, @AddendumSuggested, @CommunicatedTo, @CommunicatedStamp, @Communicated, @ClinSignifScore, @PerfSiteDept, @DiscrepancyCognitive, @DiscrepancyCommun,@DiscrepancySyntax); SELECT " &
        "SMGID, StudyRef, AccessionNumber, MRN, Grade, InitialReader, OverReader, Patient" &
        "Name, PatientSex, DOS, Modality, ProcedureName, StudyComments, PatientHistory, R" &
        "eferringPhysician, RequestingPhysician, CPTCode, ExtractDate, OrderNumber, Statu" &
        "s, ServiceSite, ReadingRes, InitialReadStamp, OverreadStamp, PerfSite, PatientDO" &
        "B, ConfCase, AddendumSuggested, CommunicatedTo, CommunicatedStamp, Communicated, ClinSignifScore, PerfSiteDept, DiscrepancyCognitive, DiscrepancyCommun, DiscrepancySyntax FROM dbo.QAComplete WHERE (SMGID = @@IDENTITY)"
        Me.SqlInsertCommand2.Connection = Me.SqlConnection2
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StudyRef", System.Data.SqlDbType.Int, 4, "StudyRef"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AccessionNumber", System.Data.SqlDbType.VarChar, 20, "AccessionNumber"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MRN", System.Data.SqlDbType.VarChar, 20, "MRN"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Grade", System.Data.SqlDbType.VarChar, 32, "Grade"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReader", System.Data.SqlDbType.VarChar, 50, "InitialReader"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverReader", System.Data.SqlDbType.VarChar, 32, "OverReader"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientName", System.Data.SqlDbType.VarChar, 50, "PatientName"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientSex", System.Data.SqlDbType.VarChar, 5, "PatientSex"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DOS", System.Data.SqlDbType.DateTime, 4, "DOS"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Modality", System.Data.SqlDbType.VarChar, 5, "Modality"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ProcedureName", System.Data.SqlDbType.VarChar, 64, "ProcedureName"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StudyComments", System.Data.SqlDbType.VarChar, 2147483647, "StudyComments"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientHistory", System.Data.SqlDbType.VarChar, 2147483647, "PatientHistory"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ReferringPhysician", System.Data.SqlDbType.VarChar, 50, "ReferringPhysician"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RequestingPhysician", System.Data.SqlDbType.VarChar, 50, "RequestingPhysician"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CPTCode", System.Data.SqlDbType.VarChar, 10, "CPTCode"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ExtractDate", System.Data.SqlDbType.DateTime, 4, "ExtractDate"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OrderNumber", System.Data.SqlDbType.VarChar, 12, "OrderNumber"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Status", System.Data.SqlDbType.VarChar, 1, "Status"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ServiceSite", System.Data.SqlDbType.VarChar, 6, "ServiceSite"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ReadingRes", System.Data.SqlDbType.VarChar, 6, "ReadingRes"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReadStamp", System.Data.SqlDbType.DateTime, 8, "InitialReadStamp"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverreadStamp", System.Data.SqlDbType.DateTime, 8, "OverreadStamp"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PerfSite", System.Data.SqlDbType.VarChar, 64, "PerfSite"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientDOB", System.Data.SqlDbType.DateTime, 8, "PatientDOB"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ConfCase", System.Data.SqlDbType.Bit, 1, "ConfCase"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AddendumSuggested", System.Data.SqlDbType.Bit, 1, "AddendumSuggested"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CommunicatedTo", System.Data.SqlDbType.VarChar, 50, "CommunicatedTo"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CommunicatedStamp", System.Data.SqlDbType.SmallDateTime, 4, "CommunicatedStamp"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Communicated", System.Data.SqlDbType.Bit, 1, "Communicated"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ClinSignifScore", System.Data.SqlDbType.VarChar, 1, "ClinSignifScore"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PerfSiteDept", System.Data.SqlDbType.VarChar, 64, "PerfSiteDept"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DiscrepancyCognitive", System.Data.SqlDbType.VarChar, 10, "DiscrepancyCognitive"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DiscrepancyCommun", System.Data.SqlDbType.Bit, 1, "DiscrepancyCommun"))
        Me.SqlInsertCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DiscrepancySyntax", System.Data.SqlDbType.VarChar, 250, "DiscrepancySyntax"))

        Me.SqlSelectCommand2.CommandText = "SELECT SMGID, StudyRef, AccessionNumber, MRN, Grade, InitialReader, OverReader, P" &
        "atientName, PatientSex, DOS, Modality, ProcedureName, StudyComments, PatientHist" &
        "ory, ReferringPhysician, RequestingPhysician, CPTCode, ExtractDate, OrderNumber," &
        " Status, ServiceSite, ReadingRes, InitialReadStamp, OverreadStamp, PerfSite, Pat" &
        "ientDOB, ConfCase, AddendumSuggested, CommunicatedTo, CommunicatedStamp,Communicated, ClinSignifScore, PerfSiteDept, DiscrepancyCognitive, DiscrepancyCommun, DiscrepancySyntax FROM dbo.QAComplete"
        Me.SqlSelectCommand2.Connection = Me.SqlConnection2
        '
        'SqlUpdateCommand2
        '
        Me.SqlUpdateCommand2.CommandText = "UPDATE dbo.QAComplete SET StudyRef = @StudyRef, AccessionNumber = @AccessionNumbe" &
        "r, MRN = @MRN, Grade = @Grade, InitialReader = @InitialReader, OverReader = @Ove" &
        "rReader, PatientName = @PatientName, PatientSex = @PatientSex, DOS = @DOS, Modal" &
        "ity = @Modality, ProcedureName = @ProcedureName, StudyComments = @StudyComments," &
        " PatientHistory = @PatientHistory, ReferringPhysician = @ReferringPhysician, Req" &
        "uestingPhysician = @RequestingPhysician, CPTCode = @CPTCode, ExtractDate = @Extr" &
        "actDate, OrderNumber = @OrderNumber, Status = @Status, ServiceSite = @ServiceSit" &
        "e, ReadingRes = @ReadingRes, InitialReadStamp = @InitialReadStamp, OverreadStamp" &
        " = @OverreadStamp, PerfSite = @PerfSite, PatientDOB = @PatientDOB, ConfCase = @C" &
        "onfCase, AddendumSuggested = @AddendumSuggested, CommunicatedTo = @CommunicatedTo, CommunicatedStamp = @CommunicatedStamp, Communicated = @Communicated, ClinSignifScore = @ClinSignifScore, PerfSiteDept = @PerfSiteDept, DiscrepancyCognitive = @DiscrepancyCognitive, DiscrepancyCommun = @DiscrepancyCommun, DiscrepancySyntax = @DiscrepancySyntax  WHERE (SMGID = @Original_SMGID) AND (AccessionNumber = @Original_Accessi" &
        "onNumber) AND (CPTCode = @Original_CPTCode OR @Original_CPTCode IS NULL AND CPTC" &
        "ode IS NULL) AND (ConfCase = @Original_ConfCase OR @Original_ConfCase IS NULL AN" &
        "D ConfCase IS NULL) AND (DOS = @Original_DOS OR @Original_DOS IS NULL AND DOS IS" &
        " NULL) AND (ExtractDate = @Original_ExtractDate OR @Original_ExtractDate IS NULL" &
        " AND ExtractDate IS NULL) AND (Grade = @Original_Grade OR @Original_Grade IS NUL" &
        "L AND Grade IS NULL) AND (InitialReadStamp = @Original_InitialReadStamp OR @Orig" &
        "inal_InitialReadStamp IS NULL AND InitialReadStamp IS NULL) AND (InitialReader =" &
        " @Original_InitialReader OR @Original_InitialReader IS NULL AND InitialReader IS" &
        " NULL) AND (MRN = @Original_MRN OR @Original_MRN IS NULL AND MRN IS NULL) AND (M" &
        "odality = @Original_Modality OR @Original_Modality IS NULL AND Modality IS NULL)" &
        " AND (OrderNumber = @Original_OrderNumber OR @Original_OrderNumber IS NULL AND O" &
        "rderNumber IS NULL) AND (OverReader = @Original_OverReader OR @Original_OverRead" &
        "er IS NULL AND OverReader IS NULL) AND (OverreadStamp = @Original_OverreadStamp " &
        "OR @Original_OverreadStamp IS NULL AND OverreadStamp IS NULL) AND (PatientDOB = " &
        "@Original_PatientDOB OR @Original_PatientDOB IS NULL AND PatientDOB IS NULL) AND" &
        " (PatientName = @Original_PatientName OR @Original_PatientName IS NULL AND Patie" &
        "ntName IS NULL) AND (PatientSex = @Original_PatientSex OR @Original_PatientSex I" &
        "S NULL AND PatientSex IS NULL) AND (PerfSite = @Original_PerfSite OR @Original_P" &
        "erfSite IS NULL AND PerfSite IS NULL) AND (ProcedureName = @Original_ProcedureNa" &
        "me OR @Original_ProcedureName IS NULL AND ProcedureName IS NULL) AND (ReadingRes" &
        " = @Original_ReadingRes OR @Original_ReadingRes IS NULL AND ReadingRes IS NULL) " &
        "AND (ReferringPhysician = @Original_ReferringPhysician OR @Original_ReferringPhy" &
        "sician IS NULL AND ReferringPhysician IS NULL) AND (RequestingPhysician = @Origi" &
        "nal_RequestingPhysician OR @Original_RequestingPhysician IS NULL AND RequestingP" &
        "hysician IS NULL) AND (ServiceSite = @Original_ServiceSite OR @Original_ServiceS" &
        "ite IS NULL AND ServiceSite IS NULL) AND (Status = @Original_Status OR @Original" &
        "_Status IS NULL AND Status IS NULL) AND (StudyRef = @Original_StudyRef OR @Origi" &
        "nal_StudyRef IS NULL AND StudyRef IS NULL); SELECT SMGID, StudyRef, AccessionNum" &
        "ber, MRN, Grade, InitialReader, OverReader, PatientName, PatientSex, DOS, Modali" &
        "ty, ProcedureName, StudyComments, PatientHistory, ReferringPhysician, Requesting" &
        "Physician, CPTCode, ExtractDate, OrderNumber, Status, ServiceSite, ReadingRes, I" &
        "nitialReadStamp, OverreadStamp, PerfSite, PatientDOB, ConfCase, AddendumSuggested, CommunicatedTo, CommunicatedStamp, Communicated, ClinSignifScore, PerfSiteDept, DiscrepancyCognitive, DiscrepancyCommun, DiscrepancySyntax FROM dbo.QAComple" &
        "te WHERE (SMGID = @SMGID)"
        Me.SqlUpdateCommand2.Connection = Me.SqlConnection2
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StudyRef", System.Data.SqlDbType.Int, 4, "StudyRef"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AccessionNumber", System.Data.SqlDbType.VarChar, 20, "AccessionNumber"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@MRN", System.Data.SqlDbType.VarChar, 20, "MRN"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Grade", System.Data.SqlDbType.VarChar, 32, "Grade"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReader", System.Data.SqlDbType.VarChar, 50, "InitialReader"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverReader", System.Data.SqlDbType.VarChar, 32, "OverReader"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientName", System.Data.SqlDbType.VarChar, 50, "PatientName"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientSex", System.Data.SqlDbType.VarChar, 5, "PatientSex"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DOS", System.Data.SqlDbType.DateTime, 4, "DOS"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Modality", System.Data.SqlDbType.VarChar, 5, "Modality"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ProcedureName", System.Data.SqlDbType.VarChar, 64, "ProcedureName"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@StudyComments", System.Data.SqlDbType.VarChar, 2147483647, "StudyComments"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientHistory", System.Data.SqlDbType.VarChar, 2147483647, "PatientHistory"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ReferringPhysician", System.Data.SqlDbType.VarChar, 50, "ReferringPhysician"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@RequestingPhysician", System.Data.SqlDbType.VarChar, 50, "RequestingPhysician"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CPTCode", System.Data.SqlDbType.VarChar, 10, "CPTCode"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ExtractDate", System.Data.SqlDbType.DateTime, 4, "ExtractDate"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OrderNumber", System.Data.SqlDbType.VarChar, 12, "OrderNumber"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Status", System.Data.SqlDbType.VarChar, 1, "Status"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ServiceSite", System.Data.SqlDbType.VarChar, 6, "ServiceSite"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ReadingRes", System.Data.SqlDbType.VarChar, 6, "ReadingRes"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReadStamp", System.Data.SqlDbType.DateTime, 8, "InitialReadStamp"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@OverreadStamp", System.Data.SqlDbType.DateTime, 8, "OverreadStamp"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PerfSite", System.Data.SqlDbType.VarChar, 64, "PerfSite"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PatientDOB", System.Data.SqlDbType.DateTime, 8, "PatientDOB"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ConfCase", System.Data.SqlDbType.Bit, 1, "ConfCase"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_SMGID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "SMGID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_AccessionNumber", System.Data.SqlDbType.VarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "AccessionNumber", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_CPTCode", System.Data.SqlDbType.VarChar, 10, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "CPTCode", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ConfCase", System.Data.SqlDbType.Bit, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ConfCase", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DOS", System.Data.SqlDbType.DateTime, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DOS", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ExtractDate", System.Data.SqlDbType.DateTime, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ExtractDate", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Grade", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Grade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReader", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReader", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_MRN", System.Data.SqlDbType.VarChar, 20, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "MRN", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Modality", System.Data.SqlDbType.VarChar, 5, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Modality", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OrderNumber", System.Data.SqlDbType.VarChar, 12, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OrderNumber", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverReader", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverReader", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_OverreadStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "OverreadStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PatientDOB", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PatientDOB", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PatientName", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PatientName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PatientSex", System.Data.SqlDbType.VarChar, 5, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PatientSex", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PerfSite", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PerfSite", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ProcedureName", System.Data.SqlDbType.VarChar, 64, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ProcedureName", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ReadingRes", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ReadingRes", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ReferringPhysician", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ReferringPhysician", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_RequestingPhysician", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "RequestingPhysician", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_ServiceSite", System.Data.SqlDbType.VarChar, 6, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "ServiceSite", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_Status", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "Status", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_StudyRef", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "StudyRef", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMGID", System.Data.SqlDbType.Int, 4, "SMGID"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@AddendumSuggested", System.Data.SqlDbType.Bit, 1, "AddendumSuggested"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CommunicatedTo", System.Data.SqlDbType.VarChar, 50, "CommunicatedTo"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@CommunicatedStamp", System.Data.SqlDbType.SmallDateTime, 4, "CommunicatedStamp"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Communicated", System.Data.SqlDbType.Bit, 1, "Communicated"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@ClinSignifScore", System.Data.SqlDbType.VarChar, 1, "ClinSignifScore"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PerfSiteDept", System.Data.SqlDbType.VarChar, 64, "PerfSiteDept"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DiscrepancyCognitive", System.Data.SqlDbType.VarChar, 10, "DiscrepancyCognitive"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DiscrepancyCommun", System.Data.SqlDbType.Bit, 1, "DiscrepancyCommun"))
        Me.SqlUpdateCommand2.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DiscrepancySyntax", System.Data.SqlDbType.VarChar, 250, "DiscrepancySyntax"))
        '
        'AltIDsAdapter
        '
        Me.AltIDsAdapter.SelectCommand = Me.SqlSelectCommand3
        Me.AltIDsAdapter.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "RadAlternateIDs", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("InitReadingRad", "InitReadingRad")})})
        '
        'SqlSelectCommand3
        '
        Me.SqlSelectCommand3.CommandText = "SELECT InitReadingRad, PACSID FROM dbo.RadAlternateIDs"
        Me.SqlSelectCommand3.Connection = Me.SqlConnection2
        '
        'SqlDataAdapter3
        '
        Me.SqlDataAdapter3.SelectCommand = Me.SqlSelectCommand4
        Me.SqlDataAdapter3.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "SiteChiefs", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("PerfSite", "PerfSite"), New System.Data.Common.DataColumnMapping("ChiefEmail", "ChiefEmail")})})
        '
        'SqlSelectCommand4
        '
        Me.SqlSelectCommand4.CommandText = "SELECT PerfSite, ChiefEmail FROM dbo.SiteChiefs"
        Me.SqlSelectCommand4.Connection = Me.SqlConnection2

    End Sub

#End Region

End Class
