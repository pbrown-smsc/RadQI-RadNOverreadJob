'Imports Dapper
Imports System.Data.SqlClient
Imports System.Configuration

Namespace ConsultInfoRepo
    Public Class ConsultInfo
        Private m_Id As Integer
        Public Property ConsultId() As Integer
            Get
                Return m_Id
            End Get
            Set(ByVal value As Integer)
                m_Id = value
            End Set
        End Property

        Private m_SMGID As Integer?
        Public Property SMGID() As Integer?
            Get
                Return m_SMGID
            End Get
            Set(ByVal value As Integer?)
                m_SMGID = value
            End Set
        End Property

        Private m_StudyRelated As Boolean
        Public Property StudyRelated() As Boolean
            Get
                Return m_StudyRelated
            End Get
            Set(ByVal value As Boolean)
                m_StudyRelated = value
            End Set
        End Property

        Private m_Mins As Integer
        Public Property Minutes() As Integer?
            Get
                Return m_Mins
            End Get
            Set(ByVal value As Integer?)
                m_Mins = value
            End Set
        End Property

        Private m_WType As Integer
        Public Property WorkTypeCode() As Integer
            Get
                Return m_WType
            End Get
            Set(ByVal value As Integer)
                m_WType = value
            End Set
        End Property

        Private m_Comms As String
        Public Property Comments() As String
            Get
                Return m_Comms
            End Get
            Set(ByVal value As String)
                m_Comms = value
            End Set
        End Property

        Private m_ConsultedRad As String
        Public Property ConsultedRad() As String
            Get
                Return m_ConsultedRad
            End Get
            Set(ByVal value As String)
                m_ConsultedRad = value
            End Set
        End Property

        Private m_Accession As String
        Public Property Accession() As String
            Get
                Return m_Accession
            End Get
            Set(ByVal value As String)
                m_Accession = value
            End Set
        End Property

        Private m_ModDateTime As DateTime
        Public Property Modified() As DateTime
            Get
                Return m_ModDateTime
            End Get
            Set(ByVal value As DateTime)
                m_ModDateTime = value
            End Set
        End Property
    End Class

    Public Class ConsultInfoRepository
        'Private _db As SqlConnection

        'Public Sub New()
        '    _db = New SqlConnection(ConfigurationManager.AppSettings("QAOverreadConnStr"))
        'End Sub

        Public Shared Function UpdateConsultInfoSMGID(consult As ConsultInfo, conn As SqlConnection, trx As SqlTransaction) As String

            Dim sqlCmd As SqlCommand = New SqlCommand(" UPDATE ConsultInfo SET SMGID = @SMGID" + " WHERE Accession = @Accession and SMGID IS NULL", conn, trx)

            sqlCmd.Parameters.AddWithValue("@SMGID", consult.SMGID)
            sqlCmd.Parameters.AddWithValue("@Accession", consult.Accession)

            Try

                If conn.State = ConnectionState.Closed Then
                    conn.Open()
                End If

                sqlCmd.ExecuteNonQuery()
            Catch generatedException As Exception
                Return generatedException.Message
            End Try

            Return String.Empty
        End Function

    End Class
End Namespace

