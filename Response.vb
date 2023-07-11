Public Class Response
    Inherits System.ComponentModel.Component
    Public Sub FillDataSet(ByVal dset As myResponseDataSet)
        SqlDataAdapter1.Fill(dset)
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
    Friend WithEvents SqlConnection1 As System.Data.SqlClient.SqlConnection
    Friend WithEvents SqlSelectCommand1 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlInsertCommand1 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlUpdateCommand1 As System.Data.SqlClient.SqlCommand
    Friend WithEvents SqlDeleteCommand1 As System.Data.SqlClient.SqlCommand
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SqlDataAdapter1 = New System.Data.SqlClient.SqlDataAdapter
        Me.SqlDeleteCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlConnection1 = New System.Data.SqlClient.SqlConnection
        Me.SqlInsertCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlSelectCommand1 = New System.Data.SqlClient.SqlCommand
        Me.SqlUpdateCommand1 = New System.Data.SqlClient.SqlCommand
        '
        'SqlDataAdapter1
        '
        Me.SqlDataAdapter1.DeleteCommand = Me.SqlDeleteCommand1
        Me.SqlDataAdapter1.InsertCommand = Me.SqlInsertCommand1
        Me.SqlDataAdapter1.SelectCommand = Me.SqlSelectCommand1

        Me.SqlDataAdapter1.TableMappings.AddRange(New System.Data.Common.DataTableMapping() {New System.Data.Common.DataTableMapping("Table", "QAResponses", New System.Data.Common.DataColumnMapping() {New System.Data.Common.DataColumnMapping("SMGID", "SMGID"), New System.Data.Common.DataColumnMapping("InitialReader", "InitialReader"), New System.Data.Common.DataColumnMapping("PrimaryAgree", "PrimaryAgree"), New System.Data.Common.DataColumnMapping("PrimaryAltGrade", "PrimaryAltGrade"), New System.Data.Common.DataColumnMapping("PrimaryComments", "PrimaryComments"), New System.Data.Common.DataColumnMapping("PrimaryStamp", "PrimaryStamp"), New System.Data.Common.DataColumnMapping("DivisionDirector", "DivisionDirector"), New System.Data.Common.DataColumnMapping("DivDirAltGrade", "DivDirAltGrade"), New System.Data.Common.DataColumnMapping("DivDirComments", "DivDirComments"), New System.Data.Common.DataColumnMapping("DivDirLate", "DivDirLate"), New System.Data.Common.DataColumnMapping("DivDirStamp", "DivDirStamp"), New System.Data.Common.DataColumnMapping("PrimaryAltClinSignifScore", "PrimaryAltClinSignifScore"), New System.Data.Common.DataColumnMapping("DivDirAltClinSignifScore", "DivDirAltClinSignifScore"), New System.Data.Common.DataColumnMapping("PrimaryDiscrepancyCognitive", "PrimaryDiscrepancyCognitive"), New System.Data.Common.DataColumnMapping("PrimaryDiscrepancyCommun", "PrimaryDiscrepancyCommun"), New System.Data.Common.DataColumnMapping("DivDirDiscrepancyCognitive", "DivDirDiscrepancyCognitive"), New System.Data.Common.DataColumnMapping("DivDirDiscrepancyCommun", "DivDirDiscrepancyCommun"), New System.Data.Common.DataColumnMapping("DivDirDiscrepancySyntax", "DivDirDiscrepancySyntax"), New System.Data.Common.DataColumnMapping("PrimaryDiscrepancySyntax", "PrimaryDiscrepancySyntax")})})

        Me.SqlDataAdapter1.UpdateCommand = Me.SqlUpdateCommand1
        '
        'SqlDeleteCommand1
        '
        Me.SqlDeleteCommand1.CommandText = "DELETE FROM dbo.QAResponses WHERE (SMGID = @Original_SMGID) AND (DivDirAltGrade =" & _
        " @Original_DivDirAltGrade OR @Original_DivDirAltGrade IS NULL AND DivDirAltGrade" & _
        " IS NULL) AND (DivDirLate = @Original_DivDirLate OR @Original_DivDirLate IS NULL" & _
        " AND DivDirLate IS NULL) AND (DivDirStamp = @Original_DivDirStamp OR @Original_D" & _
        "ivDirStamp IS NULL AND DivDirStamp IS NULL) AND (DivisionDirector = @Original_Di" & _
        "visionDirector OR @Original_DivisionDirector IS NULL AND DivisionDirector IS NUL" & _
        "L) AND (InitialReader = @Original_InitialReader OR @Original_InitialReader IS NU" & _
        "LL AND InitialReader IS NULL) AND (PrimaryAgree = @Original_PrimaryAgree OR @Ori" & _
        "ginal_PrimaryAgree IS NULL AND PrimaryAgree IS NULL) AND (PrimaryAltGrade = @Ori" & _
        "ginal_PrimaryAltGrade OR @Original_PrimaryAltGrade IS NULL AND PrimaryAltGrade I" & _
        "S NULL) AND (PrimaryStamp = @Original_PrimaryStamp OR @Original_PrimaryStamp IS " & _
        "NULL AND PrimaryStamp IS NULL)"
        Me.SqlDeleteCommand1.Connection = Me.SqlConnection1
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_SMGID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "SMGID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivDirAltGrade", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivDirAltGrade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivDirLate", System.Data.SqlDbType.SmallInt, 2, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivDirLate", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivDirStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivDirStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivisionDirector", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivisionDirector", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReader", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReader", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PrimaryAgree", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PrimaryAgree", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PrimaryAltGrade", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PrimaryAltGrade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlDeleteCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PrimaryStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PrimaryStamp", System.Data.DataRowVersion.Original, Nothing))
        '
        'SqlConnection1
        '
        'Me.SqlConnection1.ConnectionString = "workstation id=SMGINTRANET2;packet size=4096;user id=radqaN;data source=smgsql;pe" & _
        '"rsist security info=True;initial catalog=RadQANorth;password=Strong402"
        Me.SqlConnection1.ConnectionString = Configuration.ConfigurationSettings.AppSettings("QAOverreadConnStr")
        '
        'SqlInsertCommand1
        '
        Me.SqlInsertCommand1.CommandText = "INSERT INTO dbo.QAResponses(SMGID, InitialReader, PrimaryAgree, PrimaryAltGrade, " &
        "PrimaryComments, PrimaryStamp, DivisionDirector, DivDirAltGrade, DivDirComments," &
        " DivDirLate, DivDirStamp, PrimaryAltClinSignifScore, DivDirAltClinSignifScore, PrimaryDiscrepancyCognitive, PrimaryDiscrepancyCommun, DivDirDiscrepancyCognitive, DivDirDiscrepancyCommun, DivDirDiscrepancySyntax, PrimaryDiscrepancySyntax) VALUES (@SMGID, @InitialReader, @PrimaryAgree, @Primar" &
        "yAltGrade, @PrimaryComments, @PrimaryStamp, @DivisionDirector, @DivDirAltGrade, " &
        "@DivDirComments, @DivDirLate, @DivDirStamp, @PrimaryAltClinSignifScore, @DivDirAltClinSignifScore, @PrimaryDiscrepancyCognitive, @PrimaryDiscrepancyCommun, @DivDirDiscrepancyCognitive, @DivDirDiscrepancyCommun, @DivDirDiscrepancySyntax, @PrimaryDiscrepancySyntax); SELECT SMGID, InitialReader, Primar" &
        "yAgree, PrimaryAltGrade, PrimaryComments, PrimaryStamp, DivisionDirector, DivDir" &
        "AltGrade, DivDirComments, DivDirLate, DivDirStamp, PrimaryAltClinSignifScore, DivDirAltClinSignifScore, PrimaryDiscrepancyCognitive, PrimaryDiscrepancyCommun, DivDirDiscrepancyCognitive, DivDirDiscrepancyCommun, DivDirDiscrepancySyntax, PrimaryDiscrepancySyntax FROM dbo.QAResponses WHERE (SM" &
        "GID = @SMGID)"
        Me.SqlInsertCommand1.Connection = Me.SqlConnection1
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMGID", System.Data.SqlDbType.Int, 4, "SMGID"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReader", System.Data.SqlDbType.VarChar, 50, "InitialReader"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryAgree", System.Data.SqlDbType.VarChar, 1, "PrimaryAgree"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryAltGrade", System.Data.SqlDbType.VarChar, 32, "PrimaryAltGrade"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryComments", System.Data.SqlDbType.VarChar, 2147483647, "PrimaryComments"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryStamp", System.Data.SqlDbType.DateTime, 8, "PrimaryStamp"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivisionDirector", System.Data.SqlDbType.VarChar, 50, "DivisionDirector"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirAltGrade", System.Data.SqlDbType.VarChar, 32, "DivDirAltGrade"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirComments", System.Data.SqlDbType.VarChar, 2147483647, "DivDirComments"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirLate", System.Data.SqlDbType.SmallInt, 2, "DivDirLate"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirStamp", System.Data.SqlDbType.DateTime, 8, "DivDirStamp"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryAltClinSignifScore", System.Data.SqlDbType.VarChar, 1, "PrimaryAltClinSignifScore"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirAltClinSignifScore", System.Data.SqlDbType.VarChar, 1, "DivDirAltClinSignifScore"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryDiscrepancyCognitive", System.Data.SqlDbType.VarChar, 10, "PrimaryDiscrepancyCognitive"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryDiscrepancyCommun", System.Data.SqlDbType.Bit, 1, "PrimaryDiscrepancyCommun"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirDiscrepancyCognitive", System.Data.SqlDbType.VarChar, 10, "DivDirDiscrepancyCognitive"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirDiscrepancyCommun", System.Data.SqlDbType.Bit, 1, "DivDirDiscrepancyCommun"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirDiscrepancySyntax", System.Data.SqlDbType.VarChar, 250, "DivDirDiscrepancySyntax"))
        Me.SqlInsertCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryDiscrepancySyntax", System.Data.SqlDbType.VarChar, 250, "PrimaryDiscrepancySyntax"))
        '
        'SqlSelectCommand1
        '
        Me.SqlSelectCommand1.CommandText = "SELECT SMGID, InitialReader, PrimaryAgree, PrimaryAltGrade, PrimaryComments, Prim" &
        "aryStamp, DivisionDirector, DivDirAltGrade, DivDirComments, DivDirLate, DivDirSt" &
        "amp, PrimaryAltClinSignifScore, DivDirAltClinSignifScore, PrimaryDiscrepancyCognitive, PrimaryDiscrepancyCommun, DivDirDiscrepancyCognitive, DivDirDiscrepancyCommun, DivDirDiscrepancySyntax, PrimaryDiscrepancySyntax FROM dbo.QAResponses WHERE (DivDirLate <> 4)"
        Me.SqlSelectCommand1.Connection = Me.SqlConnection1
        '
        'SqlUpdateCommand1
        '
        Me.SqlUpdateCommand1.CommandText = "UPDATE dbo.QAResponses SET SMGID = @SMGID, InitialReader = @InitialReader, Primar" &
        "yAgree = @PrimaryAgree, PrimaryAltGrade = @PrimaryAltGrade, PrimaryComments = @P" &
        "rimaryComments, PrimaryStamp = @PrimaryStamp, DivisionDirector = @DivisionDirect" &
        "or, DivDirAltGrade = @DivDirAltGrade, DivDirComments = @DivDirComments, DivDirLa" &
        "te = @DivDirLate, DivDirStamp = @DivDirStamp , PrimaryAltClinSignifScore = @Prim" &
       "aryAltClinSignifScore, DivDirAltClinSignifScore = @DivDirAltClinSignifScore, PrimaryDiscrepancyCognitive " &
       "= @PrimaryDiscrepancyCognitive, PrimaryDiscrepancyCommun = @PrimaryDiscrepancyCommun, " &
       "DivDirDiscrepancyCognitive = @DivDirDiscrepancyCognitive, DivDirDiscrepancyCommun = @DivDirDiscrepancyCommun, DivDirDiscrepancySyntax = @DivDirDiscrepancySyntax, PrimaryDiscrepancySyntax = @PrimaryDiscrepancySyntax WHERE (SMGID = @Original_SMGID) AND" &
        " (DivDirAltGrade = @Original_DivDirAltGrade OR @Original_DivDirAltGrade IS NULL " &
        "AND DivDirAltGrade IS NULL) AND (DivDirLate = @Original_DivDirLate OR @Original_" &
        "DivDirLate IS NULL AND DivDirLate IS NULL) AND (DivDirStamp = @Original_DivDirSt" &
        "amp OR @Original_DivDirStamp IS NULL AND DivDirStamp IS NULL) AND (DivisionDirec" &
        "tor = @Original_DivisionDirector OR @Original_DivisionDirector IS NULL AND Divis" &
        "ionDirector IS NULL) AND (InitialReader = @Original_InitialReader OR @Original_I" &
        "nitialReader IS NULL AND InitialReader IS NULL) AND (PrimaryAgree = @Original_Pr" &
        "imaryAgree OR @Original_PrimaryAgree IS NULL AND PrimaryAgree IS NULL) AND (Prim" &
        "aryAltGrade = @Original_PrimaryAltGrade OR @Original_PrimaryAltGrade IS NULL AND" &
        " PrimaryAltGrade IS NULL) AND (PrimaryStamp = @Original_PrimaryStamp OR @Origina" &
        "l_PrimaryStamp IS NULL AND PrimaryStamp IS NULL); SELECT SMGID, InitialReader, P" &
        "rimaryAgree, PrimaryAltGrade, PrimaryComments, PrimaryStamp, DivisionDirector, D" &
        "ivDirAltGrade, DivDirComments, DivDirLate, DivDirStamp, PrimaryAltClinSignifScore, DivDirAltClinSignifScore, PrimaryDiscrepancyCognitive, PrimaryDiscrepancyCommun, DivDirDiscrepancyCognitive, DivDirDiscrepancyCommun, DivDirDiscrepancySyntax, PrimaryDiscrepancySyntax FROM dbo.QAResponses WHER" &
        "E (SMGID = @SMGID)"
        Me.SqlUpdateCommand1.Connection = Me.SqlConnection1
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@SMGID", System.Data.SqlDbType.Int, 4, "SMGID"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@InitialReader", System.Data.SqlDbType.VarChar, 50, "InitialReader"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryAgree", System.Data.SqlDbType.VarChar, 1, "PrimaryAgree"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryAltGrade", System.Data.SqlDbType.VarChar, 32, "PrimaryAltGrade"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryComments", System.Data.SqlDbType.VarChar, 2147483647, "PrimaryComments"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryStamp", System.Data.SqlDbType.DateTime, 8, "PrimaryStamp"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivisionDirector", System.Data.SqlDbType.VarChar, 50, "DivisionDirector"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirAltGrade", System.Data.SqlDbType.VarChar, 32, "DivDirAltGrade"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirComments", System.Data.SqlDbType.VarChar, 2147483647, "DivDirComments"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirLate", System.Data.SqlDbType.SmallInt, 2, "DivDirLate"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirStamp", System.Data.SqlDbType.DateTime, 8, "DivDirStamp"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_SMGID", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "SMGID", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivDirAltGrade", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivDirAltGrade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivDirLate", System.Data.SqlDbType.SmallInt, 2, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivDirLate", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivDirStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivDirStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_DivisionDirector", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "DivisionDirector", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_InitialReader", System.Data.SqlDbType.VarChar, 50, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "InitialReader", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PrimaryAgree", System.Data.SqlDbType.VarChar, 1, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PrimaryAgree", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PrimaryAltGrade", System.Data.SqlDbType.VarChar, 32, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PrimaryAltGrade", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@Original_PrimaryStamp", System.Data.SqlDbType.DateTime, 8, System.Data.ParameterDirection.Input, False, CType(0, Byte), CType(0, Byte), "PrimaryStamp", System.Data.DataRowVersion.Original, Nothing))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryAltClinSignifScore", System.Data.SqlDbType.VarChar, 1, "PrimaryAltClinSignifScore"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirAltClinSignifScore", System.Data.SqlDbType.VarChar, 1, "DivDirAltClinSignifScore"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryDiscrepancyCognitive", System.Data.SqlDbType.VarChar, 10, "PrimaryDiscrepancyCognitive"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryDiscrepancyCommun", System.Data.SqlDbType.Bit, 1, "PrimaryDiscrepancyCommun"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirDiscrepancyCognitive", System.Data.SqlDbType.VarChar, 10, "DivDirDiscrepancyCognitive"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirDiscrepancyCommun", System.Data.SqlDbType.Bit, 1, "DivDirDiscrepancyCommun"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@DivDirDiscrepancySyntax", System.Data.SqlDbType.VarChar, 250, "DivDirDiscrepancySyntax"))
        Me.SqlUpdateCommand1.Parameters.Add(New System.Data.SqlClient.SqlParameter("@PrimaryDiscrepancySyntax", System.Data.SqlDbType.VarChar, 250, "PrimaryDiscrepancySyntax"))

    End Sub

#End Region


End Class
