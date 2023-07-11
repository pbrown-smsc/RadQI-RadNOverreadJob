'Imports GroupWiseClass
Imports System.Text
Imports System.Data.Odbc
Imports System.Data.SqlClient
Imports System.Globalization
Imports CommandLineParse
Imports System.Runtime.InteropServices
Imports System.Text.RegularExpressions
Imports SiteChiefClassLibrary
Imports System.Web.Mail
Imports ParsePACSXMLComments
Imports EMMCRADQaParse.ConsultInfoRepo
Imports System.Configuration
Imports Oracle.ManagedDataAccess.Client
Imports ExchangeClassLibraryDOTNET

Module MainModule
    Private OCATComponent As New OCATComponent ' tried using the Oracle provider, but it wouldn't work (kept giving transaction error msg when trying to read)

    Private OverreadComponent As New QAOverreadcomponent
    Private WithEvents myOverreadDS As New myOverreadDataSet
    Private HRComp As New HRDBComponent
    Private WithEvents myHRDS As New HREmpsDataSet

    Private myResponseComp As New Response
    Private myResponseDS As New myResponseDataSet

    Private parser As CommandLineParser
    'Removed for Groupwise/Exchange migration March 2011


    'This is how the Risk Manager gets cc'ed on Grade 3 courtesy notification
    ' See Project Properties to set these (ends up in app.config)
    Private strCC As String = My.Settings.RiskEmail

    Private email As ExchangeWebMail

    Private log As New LogWriter(ConfigurationManager.AppSettings("LogFilePath"))

    ' This is an old site that is defunct

    Private Const constRadQAResponseURL = "IF YOU WISH TO RESPOND TO/REBUT this over-reading, please click: "
    Private Const constDaystoWaitforDivDir = 14
    Private Const constDisclaimer = "This information is maintained as part of a hospital quality program for the identification and prevention of medical injury (including education) pursuant to the Maine Health Security Act (24 MRSA, chapter 21)"

    'The next block of consts have to do with giving up on PACS Lookup and placing case in QAComplete table as-is
    Private Const constUnknownPerformingSite = "Unknown Institution"
    Private Const constUnknownPat = "Unknown Patient"
    Private Const constUnknownModality = "ZZ"
    Private Const constUnknownPatID = "99999999"
    Private Const constUnknownDOS = #1/1/1900#
    Private Const constUnknownStudy = "UNKNOWN STUDY"
#If DEBUG Then
    Private Const constDaystoWaitforPACSLookup = 1
#Else
    Private Const constDaystoWaitforPACSLookup = 14
#End If

    Dim intCountExtracts As Integer = 0

    Sub Main()
        Dim boolOK As Boolean = True 'Changed for email migration March 2011
        Dim dtQAComplete As DataTable, dtQADemos As DataTable
        Dim dr As myOverreadDataSet.OverreadRow
        Dim DemoDR As OracleDataReader
        Dim strIR As String
        Dim strPRS As String
        ' Parse out the Command line
        parser = New CommandLineParser(Command())
        ' Set up the command line entries: due to bug in CommandLineParser, order is important
        ' -gwuser <user> -gwpwd <pwd> {-gwtries <#>} -whocall <monitor> -callwt <hrs to wait> {-gwserv <server parms>} 
        SetupCommandLineEntries(parser)

        If Not parser.Parse() Then
            ' Log the parse errors and we're done
            For Each sErr As String In parser.Errors
                log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name, "Parse error reason: " & sErr)
            Next
        Else
            Dim strGWUser As String = parser.Entries.Item(1).Value
            Dim strGWPwd As String = parser.Entries.Item(3).Value
            Dim intMaxGWTries As Integer = IIf(parser.Entries.Item(5).HasValue, CType(parser.Entries.Item(5).Value, Integer), 4) ' default to 4 tries if no value passed in
            Dim intHourstoWait As Integer = CType(parser.Entries.Item(9).Value, Integer)
            Dim strGWServerPams As String = IIf(parser.Entries.Item(11).HasValue, CType(parser.Entries.Item(11).Value, String), "")

            Dim intGWTries As Integer = 0
            Dim boolOverreadDeletes As Boolean = False

            email = New ExchangeWebMail(strGWUser, strGWServerPams)

            If boolOK Then ' if we logged into GroupWise OK
                'Dim lngOverreads As Long = 0
                Dim values() As Object, CompleteValues() As Object
                Try
                    ' Fill up the Overread table in the dataset (will be all since the last run)
                    OverreadComponent.FillDataSet(myOverreadDS)

                    ' Add the QAComplete table which is the resultant table
                    dtQAComplete = myOverreadDS.Tables.Add("QAComplete")
                    ' Fill its schema
                    OverreadComponent.SqlDataAdapter2.FillSchema(dtQAComplete, SchemaType.Mapped)

                    ' Add the QADemos table which is from the PACS OracleDB
                    dtQADemos = myOverreadDS.Tables.Add("QADemos")
                    ' Fill its schema
                    OCATComponent.OleDbDataAdapter1.FillSchema(dtQADemos, SchemaType.Mapped)

                    ' Fill the HR component for later emailing
                    HRComp.FillDataSet(myHRDS)
                    FillHRDatasetwithOutsideDocs(myHRDS)

                    ReDim values(dtQADemos.Columns.Count - 1)
                    ReDim CompleteValues(dtQAComplete.Columns.Count - 1)

                    OCATComponent.OleDbConnection1.Open()

                Catch ex As Exception
                    ' here there was some error accessing SQL Server, HR DB or the Oracle server
                    log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                    "Could not connect to either SMGSQL, HR DB or Oracle view. Full message: " & ex.Message)
                    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                    "Could not connect to either SMGSQL, HR DB or Oracle view. Full message: " & ex.Message, "DB connect failed", log)
                End Try

                ' No longer needed as getting data from dosr_study tmp 6-3-2010 Dim oBroker As New BrokerConnection ', oAltID As New AlternateIDs
                Dim boolDemoDataRead As Boolean

                ' need this since the transaction in UpdateRowandRemoveOverread modifies the iteration For Each... Next
                Dim dtOverreadTableIteration As DataTable
                dtOverreadTableIteration = myOverreadDS.Tables("Overread").Copy()

                ' For each row in the Overread table, get the Oracle data from qa_demographics view
                ' and place it in the QADemos table...
                For Each dr In dtOverreadTableIteration.Rows
                    ' If this is a status "H" (= Hold means keep the reading around for 24 hours)
                    ' and it's been > 24 hours since overread timestamp, then get rid of it
                    If dr("Status") = "H" AndAlso (DateDiff(DateInterval.Hour, dr("OverreadStamp"), Now()) >= 24) _
                     Or (Not dr("OverreadingRad") Is System.DBNull.Value AndAlso dr("OverreadingRad") = "msmek1") Then
                        RemoveOverreadRow(dr)
                        boolOverreadDeletes = True
                    Else

                        ' ...but only get it if it's not an "I"ncomplete or if it is,
                        ' then make sure we waited the # hours after the initial reading
                        ' as passed in on command line
                        ' Also don't process the "H"s that haven't yet expired
                        If dr("Status") <> "H" AndAlso (dr("Status") <> "I" OrElse
                        (DateDiff(DateInterval.Hour, dr("InitialReadStamp"), Now()) >= intHourstoWait)) Then

                            Try
                                ' Use the accession number to lookup the demos in the Oracle view...
                                OCATComponent.OleDbSelectCommand1.Parameters(0).Value = Trim(dr.Item("Accession"))
                            Catch ex As Exception
                                log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                                "Unexpected Error in setting Oracle acession Param" & " Full message: " & ex.Message)
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                "Unexpected Error in setting Oracle acession Param" & " Full message: " & ex.Message, "Unknown EMMC QA Error", log)
                            End Try

                            Try
                                ' ... and load them into the DEMODR datareader...
                                DemoDR = OCATComponent.OleDbSelectCommand1.ExecuteReader(CommandBehavior.SingleRow)
                                boolDemoDataRead = DemoDR.Read()
                            Catch ex As Exception
                                ' Error here means that can't get to Oracle
                                ' warn monitor and write to log
                                log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                                "Could not connect to PACS database. Full message: " & ex.Message)
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                "Could not connect to PACS database. Full message: " & ex.Message, "PACS db connect failed", log)
                                Exit Sub
                            End Try

                            ' compute the # of days since the initial and overread'ings
                            Dim intNumDaysSinceOverread As Long, intNumDaysSinceInitread As Long
                            If Not dr("OverreadStamp") Is System.DBNull.Value Then
                                intNumDaysSinceOverread = DateDiff(DateInterval.Day, dr("OverreadStamp"), Now())
                            Else
                                intNumDaysSinceOverread = -1
                            End If
                            If Not dr("InitialReadStamp") Is System.DBNull.Value Then
                                intNumDaysSinceInitread = DateDiff(DateInterval.Day, dr("InitialReadStamp"), Now())
                            Else
                                intNumDaysSinceInitread = -1
                            End If

                            ' was data actually read from demographics ? or have we spent 3 weeks tying and RIS/PACS not providing ?
                            If boolDemoDataRead OrElse
                            (intNumDaysSinceOverread >= constDaystoWaitforPACSLookup OrElse intNumDaysSinceInitread >= constDaystoWaitforPACSLookup) Then
                                '... then into the values array...
                                Try
                                    If boolDemoDataRead Then ' here the demo data from PACS/RIS was returned OK
                                        DemoDR.GetValues(values)

                                        Try
                                            dtQADemos.Clear()

                                            dtQADemos.BeginLoadData()
                                            '... and finally into the QADemos data table
                                            ' (could have loaded directly from the values array, but not by using field names)
                                            dtQADemos.LoadDataRow(values, True)
                                            dtQADemos.EndLoadData()
                                        Catch exLoadQADemos As ConstraintException
                                            ' do nothing
                                            ' this is here since there was a problem sometimes with a constraint
                                            ' exception being thrown - could not figure why and it seems to be
                                            ' of no consequence
                                        Catch exLoadQADemos As Exception
                                            Throw ' let outer try handle all others
                                        End Try

                                        'Array.Clear(values, 0, values.Length)
                                        DemoDR.Close()
                                        ' Dim strAccNoPrefix As String = ParseAccNoPrefix(dr.Item("Accession"))
                                        'strIR = GetInitialReader(dr, oBroker.OpenBroker(strAccNoPrefix), _
                                        ' oAltID.GetPACSID(), _
                                        ' oAltID.GetInitReadingRad())
                                        'Commented out TMP 6-3-2010, getting info from DOSR_STUDY field for intial_reader

                                        If Not (dtQADemos.Rows(0)("PHYSICIAN_READING_STUDY") Is System.DBNull.Value) AndAlso Trim(dtQADemos.Rows(0)("PHYSICIAN_READING_STUDY")) <> "" Then
                                            strPRS = dtQADemos.Rows(0)("PHYSICIAN_READING_STUDY")
                                        Else
                                            strPRS = ""
                                        End If

                                        strIR = GetInitialReader(dr, strPRS, myOverreadDS.Tables("RadAlternateIDs"))

                                        'If Not strIR = "" Then ' Is there a record in Broker yet ?
                                        ' Now start filling in the CompleteValues array which we'll then use LoadDataRow into 
                                        ' the QAComplete data table
                                        dtQAComplete.BeginLoadData()
                                        'CompleteValues(0) ===>   This is an autoincrement column, leave at Nothing
                                        If Not dtQADemos.Rows(0)("STUDY_REF") Is Nothing Then
                                            ' It appears that Study_ref could be null
                                            CompleteValues(1) = CType(dtQADemos.Rows(0)("STUDY_REF").ToString, Integer)
                                            'CompleteValues(1) = dtQADemos.Rows(0)("STUDY_REF")
                                        Else
                                        End If

                                        CompleteValues(2) = dtQADemos.Rows(0)("ACCESSION_NUMBER").ToString.Trim
                                        CompleteValues(3) = dtQADemos.Rows(0)("PATIENT_ID")
                                        CompleteValues(4) = dr("Grade")

                                        CompleteValues(5) = strIR
                                        CompleteValues(6) = dr("OverreadingRad")
                                        CompleteValues(7) = dtQADemos.Rows(0)("PATIENT_NAME")
                                        CompleteValues(8) = dtQADemos.Rows(0)("PATIENT_SEX")
                                        If Not (dtQADemos.Rows(0)("STUDY_DATE") Is System.DBNull.Value) AndAlso Trim(dtQADemos.Rows(0)("STUDY_DATE")) <> "" Then
                                            CompleteValues(9) = Date.ParseExact(dtQADemos.Rows(0)("STUDY_DATE"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                                        Else
                                            CompleteValues(9) = System.DBNull.Value
                                        End If

                                        CompleteValues(10) = dtQADemos.Rows(0)("MODALITY")
                                        CompleteValues(11) = dtQADemos.Rows(0)("STUDY_DESCRIPTION")

                                        Dim parser As New PACSUtility

                                        ' if present, "Glue" the Oncall reading and Grade comments into Study comments
                                        CompleteValues(12) = parser.ParseXMLStudyComments(IIf(Not (dtQADemos.Rows(0)("STUDY_COMMENTS") Is System.DBNull.Value) AndAlso Trim(dtQADemos.Rows(0)("STUDY_COMMENTS")) <> "", dtQADemos.Rows(0)("STUDY_COMMENTS"), "")) &
                                        IIf(Not (dr("OncallReading") Is System.DBNull.Value) AndAlso Trim(dr("OncallReading")) <> "", vbCrLf & "***On-call reading: " & dr("OncallReading") & " ***", "") &
                                        IIf(Not (dr("GradeComments") Is System.DBNull.Value) AndAlso Trim(dr("GradeComments")) <> "", vbCrLf & "***Over-read Comments: " & dr("GradeComments") & " ***", "")

                                        CompleteValues(13) = dtQADemos.Rows(0)("PATIENT_HISTORY")
                                        CompleteValues(14) = dtQADemos.Rows(0)("REFERRING_PHYSICIAN")
                                        CompleteValues(15) = dtQADemos.Rows(0)("REQUESTING_PHYSICIAN")
                                        If Not (dtQADemos.Rows(0)("REQUESTED_PROCEDURE_CODE") Is System.DBNull.Value) AndAlso Trim(dtQADemos.Rows(0)("REQUESTED_PROCEDURE_CODE")) <> "" Then
                                            CompleteValues(16) = Left(dtQADemos.Rows(0)("REQUESTED_PROCEDURE_CODE"), 10)
                                        Else
                                            CompleteValues(16) = System.DBNull.Value
                                        End If
                                        CompleteValues(17) = Now()
                                        CompleteValues(18) = System.DBNull.Value
                                        CompleteValues(19) = dr("Status")
                                        CompleteValues(20) = dr("ServiceSite")
                                        CompleteValues(21) = dr("ReadingRes")
                                        CompleteValues(22) = dr("InitialReadStamp")
                                        CompleteValues(23) = dr("OverreadStamp")
                                        CompleteValues(24) = dtQADemos.Rows(0)("INSTITUTION_NAME")
                                        If Not (dtQADemos.Rows(0)("PATIENT_BIRTH_DATE") Is System.DBNull.Value) AndAlso Trim(dtQADemos.Rows(0)("PATIENT_BIRTH_DATE")) <> "" Then
                                            CompleteValues(25) = Date.ParseExact(dtQADemos.Rows(0)("PATIENT_BIRTH_DATE"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
                                        Else
                                            CompleteValues(25) = System.DBNull.Value
                                        End If

                                        CompleteValues(26) = dr("ConfCase")
                                        CompleteValues(27) = dr("AddendumSuggested")
                                        CompleteValues(28) = dr("CommunicatedTo")
                                        CompleteValues(29) = dr("CommunicatedStamp")
                                        CompleteValues(30) = dr("Communicated")
                                        If Not (dr("ClinSignifScore") Is System.DBNull.Value) Then
                                            CompleteValues(31) = dr("ClinSignifScore")
                                        Else
                                            CompleteValues(31) = String.Empty
                                        End If

                                        CompleteValues(32) = dtQADemos.Rows(0)("INSTITUTIONAL_DEPARTMENT_NAME")

                                        If Not (dr("DiscrepancyCognitive") Is DBNull.Value) Then
                                            CompleteValues(33) = dr("DiscrepancyCognitive")
                                        Else
                                            CompleteValues(33) = String.Empty
                                        End If

                                        CompleteValues(34) = dr("DiscrepancyCommun")

                                        If Not (dr("DiscrepancySyntax") Is DBNull.Value) Then
                                            CompleteValues(35) = dr("DiscrepancySyntax")
                                        Else
                                            CompleteValues(35) = String.Empty
                                        End If

                                    Else ' did not get any info from PACS/RIS on this accession and we're giving up

                                        ' Now start filling in the CompleteValues array which we'll then use LoadDataRow into 
                                        ' the QAComplete data table
                                        dtQAComplete.BeginLoadData()

                                        LoadCompleteValuesArraywithEmpty(CompleteValues, dr)

                                        log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                                        "This accession number not found in PACS database: " &
                                        dr.Item("Accession") & " ; will be added to QAComplete with empty info ")

                                    End If

                                    If Not DemoDR.IsClosed Then DemoDR.Close()

                                    ' Load the row into the QAComplete data table
                                    ' Jan 2013 - the EndLoadData call actually knows about the column metdata, like nullable, data type, size
                                    ' so wrap in a try...catch so we can warn more accurately if exception
                                    Dim drQAComp As DataRow
                                    Try
                                        drQAComp = dtQAComplete.LoadDataRow(CompleteValues, False)   ' False means do not AcceptChanges which would've caused no updating later (if True was set)
                                        dtQAComplete.EndLoadData()

                                        ' update - passing the accession and the status for a unique match in Overread table
                                        ' this now does a transaction to wrap both the QAComplete and Overread mods into an atomic action 8-3-12 PCB
                                        ' UpdateRowandRemoveOverread(CompleteValues(2).ToString, CompleteValues(19).ToString, drQAComp)
                                        ' 10-2-17 error whereby there was a leading space in some Accessions, so pass exactly as from OVerread tabler with no Trim
                                        UpdateRowandRemoveOverread(dr("Accession"), CompleteValues(19).ToString, drQAComp)
                                    Catch ex As Exception
                                        log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                                        "The row for this accession number likely has a mismatch on column datatype, size, nullability between PACS - DOSR_STUDY- and RadQANorth: " &
                                        dr.Item("Accession") & " Full message: " & ex.Message)

                                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                        "The row for this accession number likely has a mismatch on column datatype, size, nullability between PACS - DOSR_STUDY- and RadQANorth: " &
                                        dr.Item("Accession") & " Full message: " & ex.Message, "Unable to Insert to QAComplete", log)

                                    End Try

                                Catch ex As Exception
                                    ' exception here likely means the Accession # was not found in PACS
                                    ' warn monitor and write to log

                                    ' 7-12-04 PCB But Cayla explains that there could be a delay in data getting
                                    ' from PACS to QuadRIS (where the attending rad is picked up) so that after a
                                    ' "reasonable time", the Oracle view should return a match on Accession #
                                    ' this should be primarily be an issue for on call overreads

                                    ' So let's only warn if this "reasonable time" (=24 hours) is up (or it's an
                                    ' "I"ncomplete which means it must be older than intHourstoWait)
                                    If dr("Status") = "I" OrElse (DateDiff(DateInterval.Hour, dr("OverreadStamp"), Now()) >= 24) Then

                                        log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                                        "This accession number probably not found in PACS database: " &
                                        dr.Item("Accession") & " Full message: " & ex.Message)
                                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                        "This accession number probably not found in PACS database: " &
                                        dr.Item("Accession") & " Full message: " & ex.Message, "No Accession #", log)
                                    End If
                                    DemoDR.Close()
                                Finally
                                    Array.Clear(CompleteValues, 1, CompleteValues.Length - 1)
                                    Array.Clear(values, 0, values.Length)
                                End Try
                            Else  ' No record read from DEMODR
                                If Not DemoDR.IsClosed Then DemoDR.Close()
                            End If ' If boolDemoDataRead
                        End If
                    End If
                Next
                Try
                    OCATComponent.OleDbConnection1.Close()

                    ' Close connection to the SQL Broker
                    ' Connection no longer need as info coming from dosr_study TMP 6-3-2010 oBroker.CloseBroker()

                    ' if there were deletions of H records, write them to the database
                    If boolOverreadDeletes Then FlushOverreadRow()

                    If Not OverreadComponent.SqlConnection2.State = ConnectionState.Open Then _
                     OverreadComponent.SqlConnection2.Open()

                    'UpdateRowsandRemoveOverreads()

                    If OverreadComponent.SqlConnection2.State = ConnectionState.Open Then _
                        OverreadComponent.SqlConnection2.Close()

                    ' Log number of recs extracted
                    log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                    "Successful extraction of " & intCountExtracts & " QA records")

                    ' Here we can open the QAResponses table selecting only
                    ' non-Finalized records
                    ProcessQAResponses()

                Catch exCleanUp As Exception
                    log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                    "Unexpected Error in Cleanup section" & " Full message: " & exCleanUp.Message)
                    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                    "Unexpected Error in Cleanup section" & " Full message: " & exCleanUp.Message, "Unknown EMMC QA Error", log)
                End Try

            Else ' Failed intMaxGWTries times to log into GroupWise
            End If

            email = Nothing
        End If

    End Sub
    Private Sub UpdateRowandRemoveOverread(ByVal strQACompleteAccession As String, ByVal strQACompleteStatus As String, ByVal drQAComp As DataRow)
        ' this code does one QAComplete and its Overread record as an atomic operation
        ' the passed accession and status defines a unique record in Overread

        Dim QIInsertUpdateTrx As SqlClient.SqlTransaction
        Try
            If OverreadComponent.SqlConnection2.State = ConnectionState.Closed Then
                OverreadComponent.SqlConnection2.Open()
            End If

            ' 8-1-12  PCB   Wrap in Trx since it appears sometimes the Overread table upd/delete fails
            QIInsertUpdateTrx = OverreadComponent.SqlConnection2.BeginTransaction

            ' Do the update (insert only) and don't stop if a problem
            With OverreadComponent.SqlDataAdapter2
                .UpdateCommand.Transaction = QIInsertUpdateTrx
                .SelectCommand.Transaction = QIInsertUpdateTrx
                .InsertCommand.Transaction = QIInsertUpdateTrx
                .ContinueUpdateOnError = False
                .UpdateCommand.CommandTimeout = 60
                .Update(myOverreadDS, "QAComplete")
            End With


            ' Because ContinueUpdateOnError = False, only get here if the prior op - Update to QAComplete - succeeded

            Dim drOverreads() As DataRow

            ' Select which Overread rec matches the QAComplete rec just inserted
            drOverreads = myOverreadDS.Tables("Overread").Select("Accession = '" & strQACompleteAccession & "' and Status = '" &
                                                                 strQACompleteStatus & "'")

            ' Get Identity if this is a Consult
            If drOverreads(0)("Status") = "T" Then
                ' Select the QAComplete row just inserted (in memory copy so acc,status will be unique) so we can get the SMGID
                Dim matchingRow() As DataRow = myOverreadDS.Tables("QAComplete").Select("AccessionNumber = '" & strQACompleteAccession.Trim & "' and Status = '" &
                                                                     strQACompleteStatus & "'")
                Dim intSMGID As Integer = matchingRow(0)("SMGID")

                ' Now if this is a Consult and associated, update the SMGID in ConsultInfo table

                Dim consult As New ConsultInfo()
                consult.Accession = drOverreads(0)("Accession").ToString.Trim
                consult.SMGID = intSMGID
                Dim strMsg As String = ConsultInfoRepository.UpdateConsultInfoSMGID(consult, OverreadComponent.SqlConnection2, QIInsertUpdateTrx)
                If strMsg <> String.Empty Then
                    ' failed to update ConsultInfo
                    Throw New Exception("ConsultInfo update failed: " & strMsg)
                End If

            End If

            ' need this since NotifyInitialReaders depends on an overread record (which is likely to be deleted soon)
            Dim dt As DataTable = myOverreadDS.Tables("Overread").Copy()
            Dim drOverread As DataRow = dt.Select("Accession = '" & strQACompleteAccession & "' and Status = '" & strQACompleteStatus & "'")(0)

            ' Right here don't delete the Overread rec if it is a "C", but change to an "H" status
            ' so the initial reading will stay around for a predetermined length of time
            If drOverreads(0)("Status") = "C" Then
                drOverreads(0)("Status") = "H"
            Else
                ' Otherwise (not a C), delete the Overread rec
                log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                 "Status: attempted deleting acc# " & drOverreads(0)("Accession") & " and Status:" & drOverreads(0)("Status") & " from Overread table")
                drOverreads(0).Delete()
            End If

            ' Do the update/delete on the Overreads Data adapter
            With OverreadComponent.SqlDataAdapter1
                .UpdateCommand.Transaction = QIInsertUpdateTrx
                .SelectCommand.Transaction = QIInsertUpdateTrx
                .DeleteCommand.Transaction = QIInsertUpdateTrx
                .ContinueUpdateOnError = False
                .UpdateCommand.CommandTimeout = 60
                .Update(myOverreadDS, "Overread")
            End With


            QIInsertUpdateTrx.Commit()
            QIInsertUpdateTrx = Nothing

            intCountExtracts += 1 ' For total count

            ' Now Notify the initial readers 
            ' 
            NotifyInitialReaders(drQAComp, drOverread)


            ' Now, if this contains an addendum request, process that
            If Not drQAComp("AddendumSuggested") Is System.DBNull.Value AndAlso drQAComp("AddendumSuggested") = True Then
                ' An addendum has been suggested by the overreader
                ProcessAddendumRequest(drQAComp)
            End If


            dt = Nothing

        Catch ex As Exception
            ' some problem with either the QAComplete insert or the Overread update/delete (or ConsultInfo if applicable) - rollback both
            If Not (QIInsertUpdateTrx Is Nothing) Then QIInsertUpdateTrx.Rollback()
            QIInsertUpdateTrx = Nothing

            ' the error here is that couldn't delete or update records from Overread table or insert into QAComplete 
            ' write this to the log and send an email
            log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
            "Not able to delete/update accession number from Overread or insert into QAComplete: " & strQACompleteAccession & " and Status:" & strQACompleteStatus)
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
            "Not able to delete/update accession number from Overread or insert into QAComplete: " & strQACompleteAccession & " and Status:" & strQACompleteStatus &
            vbCrLf & vbCrLf & "Attempted operations to Overread and QAComplete were rolled back.  Here's error: " & ex.Message, "Problem settling QI case data", log)

        End Try
    End Sub

    Private Sub LoadCompleteValuesArraywithEmpty(ByVal CompleteValues() As Object, ByVal dr As myOverreadDataSet.OverreadRow)
        'If Not dtQADemos.Rows(0)("STUDY_REF") Is Nothing Then
        '    ' It appears that Study_ref could be null
        '    CompleteValues(1) = CType(dtQADemos.Rows(0)("STUDY_REF").ToString, Integer)
        'Else
        'End If
        CompleteValues(1) = System.DBNull.Value
        CompleteValues(2) = dr("Accession").ToString.Trim
        CompleteValues(3) = constUnknownPatID
        CompleteValues(4) = dr("Grade")

        CompleteValues(5) = dr("InitReadingRad")
        CompleteValues(6) = dr("OverreadingRad")
        CompleteValues(7) = constUnknownPat 'dtQADemos.Rows(0)("PATIENT_NAME")
        CompleteValues(8) = "U" 'dtQADemos.Rows(0)("PATIENT_SEX")

        CompleteValues(9) = constUnknownDOS ' Date.ParseExact(dtQADemos.Rows(0)("STUDY_DATE"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)

        CompleteValues(10) = constUnknownModality
        CompleteValues(11) = constUnknownStudy 'dtQADemos.Rows(0)("STUDY_DESCRIPTION")

        ' if present, "Glue" the Oncall reading and Grade comments into Study comments
        Dim strStudyComms As String = ""
        If Not (dr("OncallReading") Is System.DBNull.Value) AndAlso Trim(dr("OncallReading")) <> "" Then
            strStudyComms = "***On-call reading: " & dr("OncallReading") & " ***"
        End If
        If Not (dr("GradeComments") Is System.DBNull.Value) AndAlso Trim(dr("GradeComments")) <> "" Then
            strStudyComms = strStudyComms & vbCrLf & "***Over-read Comments: " & dr("GradeComments") & " ***"
        End If

        CompleteValues(12) = strStudyComms

        CompleteValues(13) = System.DBNull.Value 'CompleteValues(13) = dtQADemos.Rows(0)("PATIENT_HISTORY")
        CompleteValues(14) = System.DBNull.Value 'CompleteValues(14) = dtQADemos.Rows(0)("REFERRING_PHYSICIAN")
        CompleteValues(15) = System.DBNull.Value 'CompleteValues(15) = dtQADemos.Rows(0)("REQUESTING_PHYSICIAN")
        CompleteValues(16) = System.DBNull.Value
        CompleteValues(17) = Now()
        CompleteValues(18) = System.DBNull.Value
        CompleteValues(19) = dr("Status")
        CompleteValues(20) = dr("ServiceSite")
        CompleteValues(21) = dr("ReadingRes")
        CompleteValues(22) = dr("InitialReadStamp")
        CompleteValues(23) = dr("OverreadStamp")
        CompleteValues(24) = constUnknownPerformingSite 'dtQADemos.Rows(0)("INSTITUTION_NAME")
        CompleteValues(25) = System.DBNull.Value 'Date.ParseExact(dtQADemos.Rows(0)("PATIENT_BIRTH_DATE"), "yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo)
        CompleteValues(26) = dr("ConfCase")
        CompleteValues(27) = dr("AddendumSuggested")
        CompleteValues(28) = dr("CommunicatedTo")
        CompleteValues(29) = dr("CommunicatedStamp")
        CompleteValues(30) = dr("Communicated")
        If Not (dr("ClinSignifScore") Is System.DBNull.Value) Then
            CompleteValues(31) = dr("ClinSignifScore")
        Else
            CompleteValues(31) = String.Empty
        End If
        CompleteValues(32) = System.DBNull.Value

        If Not (dr("DiscrepancyCognitive") Is DBNull.Value) Then
            CompleteValues(33) = dr("DiscrepancyCognitive")
        Else
            CompleteValues(33) = String.Empty
        End If

        CompleteValues(34) = dr("DiscrepancyCommun")

        If Not (dr("DiscrepancySyntax") Is DBNull.Value) Then
            CompleteValues(35) = dr("DiscrepancySyntax")
        Else
            CompleteValues(35) = String.Empty
        End If

        If (dr("InitReadingRad") Is System.DBNull.Value OrElse String.IsNullOrEmpty(dr("InitReadingRad"))) AndAlso
        (Not (dr("GradeComments") Is System.DBNull.Value) AndAlso Trim(dr("GradeComments")) <> "") Then
            ' here there were comments but the Initial reader could not be found
            ' notify the overreader that could not reach that person
            NotifyOverreaderOfNoSend(dr)
        End If
    End Sub
    Private Sub ProcessAddendumRequest(ByVal dr As DataRow)
        ' Added Jan 2013 to support Addendum notifcation feature
        ' This is where we implement what to do when the overreader checks Addendum requested in the Peer Review page
        ' This consists of emailing the addendum request to: (1) the initial reader, and (2) any contacts for that reader's Division/Service as indicated
        ' in the AddendumNoticeContacts table
        ' Note: Need to process addendum requests for Synergy docs as well as SMG docs

        ' General flow: find the initial reader's Division and Sub-division in the HR Dataset and then look those up in the AddendumNoticeContacts table
        ' Question: who to send to when the initial reader is unknown ? - Answer: to the entry which should be found in table as Unknown Div

        Dim dta As New myOverreadDataSetTableAdapters.AddendumNoticeContactsTableAdapter
        Dim dt As New myOverreadDataSet.AddendumNoticeContactsDataTable
        Dim strMessage As String
        Dim strMessageHead As String
        Dim strInitReaderFullName As String = "", strOverReaderFullName As String = ""
        Dim boolEmailSent As Boolean = False, boolAdminSent As Boolean = False
        Dim strContactsforUnknown As String = "", ANRow As myOverreadDataSet.AddendumNoticeContactsRow, strContactsforDivService As String = ""

        If Not dr("Overreader") Is System.DBNull.Value Then _
            strOverReaderFullName = GetOverreaderFullName(dr("Overreader"))

        Dim HRTable As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
        Dim HRRow As HREmpsDataSet.EmployeesRow

        ' Here we'll attempt to notify the intial reader of this addendum request
        Try
            ' Access the Addendum Notice table to get contact emails for admin staff who should be notified
            dta.Fill(dt)

            Try  ' to find the unknown recs for default notice if cannot find an initial reader
                ANRow = dt.Select("[Division-Service] like 'Unknown' and [Sub-Division] like 'Unknown'")(0)
                strContactsforUnknown = ANRow.EmailContacts
            Catch ex As Exception
                ' can't find an "Unknown" entry in table: notify
                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "AddendumNoticeContacts table in RadQA needs an entry that matches [Division-Service] like 'Unknown' and [Sub-Division] like 'Unknown'  Please add this entry and its email contacts ", "AddendumNoticeContacts 'Unknown' Entry is missing", log)
            End Try

            Dim strCommunMsg As String, strFollowUpLink As String

            strFollowUpLink = "PLEASE INDICATE WHETHER YOU HAVE/HAVE NOT CREATED AN ADDENDUM FOR THIS CASE BY CLICKING ON THE LINK HERE: https://qarad.spectrummg.com:887/RadQA/loopcloseN.aspx?studyref=" & dr("AccessionNumber").ToString().Trim() & "&site=EMMC"

            If Not dr("Communicated") Is DBNull.Value AndAlso dr("Communicated") = True Then
                strCommunMsg = vbCrLf & vbCrLf & "THE FINDINGS WERE COMMUNICATED TO: " & dr("CommunicatedTo") & " on/at: " & dr("CommunicatedStamp") & " AND INCLUDED THE FOLLOWING."
            Else
                strCommunMsg = vbCrLf & vbCrLf & "THE FINDINGS WERE NOT COMMUNICATED TO THE CLINICIAN."
            End If

            'save state before RM's suggested format changes
            'Dim strMessageInit As String = "***This email was sent to you automatically, please do not reply to this email***" & _
            ' strCommunMsg & vbCrLf & vbCrLf & _
            ' strFollowUpLink & vbCrLf & vbCrLf & _
            ' "A final report should be addended when a failure to do so may affect patient care. Please note that the addendum should be made by the original interpreter." & _
            ' vbCrLf & vbCrLf & _
            ' "An Addendum was suggested for the following study by the over-reader, " & IIf(Len(strOverReaderFullName) = 0, "Unknown", strOverReaderFullName) & ", via the Peer Review button " & _
            ' vbCrLf & vbCrLf & "Patient Name: " & dr("PatientName") & vbCrLf & "Accession: " & dr("AccessionNumber") & "  MRN: " & dr("MRN") & _
            ' " DOS: " & dr("DOS") & vbCrLf & vbCrLf & "Study: " & dr("ProcedureName") & _
            ' vbCrLf & "Indications/History: " & dr("PatientHistory") & vbCrLf & vbCrLf & " Study Comments: " & _
            ' dr("StudyComments") & vbCrLf & vbCrLf & "Referring provider:" & dr("ReferringPhysician") & _
            ' vbCrLf & "Requesting provider:" & dr("RequestingPhysician")

            Dim strMessageInit As String = "***This email was sent to you automatically, please do not reply to this email as the sending account is not monitored***" &
             vbCrLf & vbCrLf & strFollowUpLink &
             vbCrLf & vbCrLf & "Over-reader: " & IIf(Len(strOverReaderFullName) = 0, "Unknown", strOverReaderFullName) & " has suggested an addendum on the case below: " &
             strCommunMsg &
             vbCrLf & vbCrLf & "Over-read Comments: " & dr("StudyComments") &
             vbCrLf & vbCrLf & "Patient Name: " & dr("PatientName") & vbCrLf & "Accession: " & dr("AccessionNumber") & "  MRN: " & dr("MRN") &
             " DOS: " & dr("DOS") & vbCrLf & vbCrLf & "Study: " & dr("ProcedureName") &
             vbCrLf & "Indications/History: " & dr("PatientHistory") &
             vbCrLf & vbCrLf & "Referring provider:" & dr("ReferringPhysician") &
             vbCrLf & "Requesting provider:" & dr("RequestingPhysician") &
             vbCrLf & vbCrLf & strFollowUpLink

            strMessage = "DON'T REPLY TO THIS MESSAGE, BUT IF YOU REQUIRE HELP, PLEASE CONTACT SMG Radiology North Support.  If you are having trouble logging in please contact help@logically.com" &
             vbCrLf & vbCrLf & strMessageInit &
             vbCrLf & vbCrLf & constDisclaimer

            ' only send an Addendum email to the Initial (rad) reader if it is present
            If Not dr("InitialReader") Is System.DBNull.Value AndAlso dr("InitialReader") <> "None" _
                AndAlso Not String.IsNullOrEmpty(dr("InitialReader")) Then

                Try
                    HRRow = HRTable.Select("PACSID = '" & dr("InitialReader") & "'")(0)
                    If Not HRRow.IsFullNameNull Then _
                        strInitReaderFullName = HRRow.FullName

                    Try ' now that we have the HR record of the initial reader, attempt to look up the addendum contacts for 
                        ' their div/subdiv in the AddendumNoticeContacts table
                        If HRRow.Is_Sub_divisionNull OrElse HRRow._Sub_division = String.Empty Then ' No subdiv should match to "None"
                            ANRow = dt.Select("[Division-Service] like '" & HRRow.Division & "' and [Sub-Division] like 'None'")(0)
                        Else
                            ANRow = dt.Select("[Division-Service] like '" & HRRow.Division & "' and [Sub-Division] like '" & HRRow._Sub_division & "'")(0)
                        End If
                        strContactsforDivService = ANRow.EmailContacts
                    Catch exContacts As Exception
                        ' can't find an entry in table for a div/sub-div: notify
                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "AddendumNoticeContacts table in RadQA needs an entry that matches [Division-Service] like '" & HRRow.Division & "' and [Sub-Division] like '" & HRRow._Sub_division & "'  Please add this entry and its email contacts ", "AddendumNoticeContacts Entry is missing", log)
                    End Try

                    ' use HRRow.Email_Address to send email - make sure it is not blank
                    If Not HRRow.Email_Address Is System.DBNull.Value And Not HRRow.Email_Address.Trim = "" Then

                        strMessageHead = "Addendum Suggested"

#If DEBUG Then

                        boolEmailSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value,
                                                                         strMessage & vbCrLf & ">> this email sent to: " & HRRow.Email_Address & " as address on file for initial reader: " &
                                                                         IIf(strInitReaderFullName = "", "Unknown", strInitReaderFullName) & " <<",
                                                                         strMessageHead, log, "", "", MailPriority.High)
#Else
                        boolEmailSent = email.EmailFileToRecipListSECURE(HRRow.Email_Address, _
                                 strMessage & vbCrLf & ">> this email sent to: " & HRRow.Email_Address & " as address on file for initial reader: " & _
                                 IIf(strInitReaderFullName = "", "Unknown", strInitReaderFullName) & " <<", _
                                 strMessageHead, log, "", "", MailPriority.High)
#End If


                        If Not boolEmailSent Then
                            ' try to notify the monitor about this unsent email
                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                            "Failed send email notif to " & HRRow.Email_Address, "Addendum Email Notif to Rad failed", log)
                        End If
                        If dr("InitialReader") = dr("OverReader") Then
                            ' This shouldn't really happen - send an email to monitor to check the validity of this (i.e. did resident enter the wrong initial reading rad?)
                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                            strMessage, "Addendum Rad QA - This case has initial reader = overreader", log)
                        End If
                    Else ' Email address of the rad initial reader is blank 
                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "For addendum attemtped to send to initial reader but HR database missing email address for " & dr("InitialReader"), "Addendum Email of Rad missing", log)
                    End If
                Catch ex As Exception
                    ' No PACSID for this particular rad - notify someone
                    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "For addendum attemtped to send to initial reader but HR database missing PACSID  for " & dr("InitialReader"), "Addendum PACS Code of Rad missing", log)
                End Try
            Else ' Had no initial reader to send email to -> alert contact(s) for the 'Unknown'
                ' strContactsforUnknown has the emails
            End If ' initial reading rad field present?

            ' Now need to send copy of the email to the appropriate Admin/QI contacts as provided in the AddendumNoticeContacts table

            ' check to see whether initial reader got addendum request email
            If boolEmailSent Then
                strMessage = "Below is a copy of the email that was sent to initial reader: " & strInitReaderFullName & " via email address: " & HRRow.Email_Address &
                    vbCrLf & vbCrLf & strMessage
            Else ' here the Initial reader did not get an email
                strMessage = "Below is a copy of the email that was NOT successfully sent to the initial reader. PACS may not have returned who reader was, or email address, or SMG HRIS not populated with correct data.  Please notify initial reader that addendum needs to be done as they have NOT received notice." &
                    vbCrLf & vbCrLf & strMessage
            End If

            If Not strContactsforDivService = String.Empty Then
                ' Here we have Div/Service specific contacts
                strMessageHead = "Addendum Suggested for Division/Service: " & HRRow.Division & " and Sub-Division: " & HRRow._Sub_division
#If DEBUG Then
                boolAdminSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value, strMessage, strMessageHead, log, "", "", MailPriority.High)
#Else
                boolAdminSent = email.EmailFileToRecipListSECURE(strContactsforDivService, strMessage, strMessageHead, log, "", "", MailPriority.High)
#End If

                If Not boolAdminSent Then _
                   email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "In Sub ProcessAddendumRequest, the attempted send to the admin contacts: " & strContactsforDivService & " was NOT successful.", "ProcessAddendumRequest Problem", log)
            Else ' Here we DO NOT have Div/Service specific contacts, use the ones for Unknown - default
                strMessageHead = "Addendum Suggested for Division/Service: Unknown"

                If Not strContactsforUnknown = String.Empty Then
#If DEBUG Then
                    boolAdminSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value, strMessage, strMessageHead, log, "", "", MailPriority.High)
#Else
                    boolAdminSent = email.EmailFileToRecipListSECURE(strContactsforUnknown, strMessage, strMessageHead, log, "", "", MailPriority.High)
#End If

                    If Not boolAdminSent Then _
                       email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "In Sub ProcessAddendumRequest, the attempted send to the admin contacts for Unknown was NOT successful.", "ProcessAddendumRequest Problem", log)

                Else ' here we got no contacts for the Unknown intial reader div/service and we needed them; notify
                    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "In Sub ProcessAddendumRequest, the AddendumNoticeContacts table in RadQA did NOT have an entry for Unknown Div/Service and it was needed.  A notice for an addendum was not sent to either initial reader nor admin/QI staff.", "RadQA table missing entry", log)
                End If
            End If

        Catch ex As Exception
            ' can't access the Addendum Notice table: notify
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, $"In Sub ProcessAddendumRequest, the AddendumNoticeContacts table in RadQA cannot be accessed.  A notice for an addendum was not sent to either initial reader nor admin/QI staff.  Error details: {ex.Message}", "RadQA table access problem", log)
        End Try

    End Sub

    Private Sub NotifyOverreaderOfNoSend(ByVal dr As myOverreadDataSet.OverreadRow)
        Dim boolEmailSent As Boolean = True
        Dim strMessage As String, strReviewType As String
        Dim strMessageHead As String
        Dim boolInfo As Boolean, boolErrorCode As Boolean, boolCommunication As Boolean = False
        Try
            If Not (dr("Status") = "T" Or dr("Status") = "A" Or dr("Status") = "L") Then ' these types don't require an email
                ' First lookup the Review Type (to place in the email)
                Try
                    If OverreadComponent.SqlConnection2.State = ConnectionState.Closed Then
                        OverreadComponent.SqlConnection2.Open()
                    End If
                    Dim strSQLLUP = "select [Review Type] from ReviewTypes where Status = '" & dr("Status") & "'"
                    Dim cmdLookup As New SqlCommand(strSQLLUP, OverreadComponent.SqlConnection2)
                    strReviewType = cmdLookup.ExecuteScalar().ToString()
                Catch ex As Exception
                    ' Problem getting Review Type
                    ' write this to the log and send an email
                    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                    "Problem getting Review Type for email notif. " & ex.Message, "Review Type lookup failure", log)
                    log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                    "Problem getting Review Type for email notif. " & ex.Message)
                    strReviewType = ""
                Finally
                    OverreadComponent.SqlConnection2.Close()
                End Try

                ' Decide on type of notify
                If (dr("Grade") = "1") AndAlso (Not (dr("GradeComments") Is System.DBNull.Value) AndAlso Len(dr("GradeComments")) > 0) Then
                    ' if this is a Grade = 1 and there are some comments, then this is an informational item only
                    boolInfo = True
                    boolErrorCode = False
                ElseIf (dr("Grade") = "0") AndAlso (Not (dr("GradeComments") Is System.DBNull.Value) AndAlso Len(dr("GradeComments")) > 0) Then
                    ' if this is a Grade = 0 and there are some comments, then this is an informational item only
                    boolInfo = False
                    boolCommunication = True
                    boolErrorCode = True
                ElseIf (dr("Grade") = "2" Or dr("Grade") = "3" Or dr("Grade") = "4") Then
                    boolErrorCode = True
                    boolInfo = False
                Else
                    boolInfo = False
                    boolErrorCode = False
                End If

                Dim strClinSignif As String = GetClinSignifScore(dr("ClinSignifScore"))
                Dim strDiscrepancy As String = GetDiscrepancyVerbiage(dr("DiscrepancyCognitive").ToString(),
                                                                      IIf(dr("DiscrepancySyntax") Is DBNull.Value, String.Empty, dr("DiscrepancySyntax").ToString()),
                                                                      IIf(dr("DiscrepancyCommun") Is DBNull.Value, False, dr("DiscrepancyCommun")))

                'Dim strCommunFailText As String = IIf(strDiscrepancy.Contains("communication"), "COMMUNICATION DISCREPANCY", String.Empty)

                ' Build the message for the email
                Dim strMessageInit As String = "***This email was sent to you automatically, please do not reply to this email***" &
                 vbCrLf & vbCrLf & strReviewType & vbCrLf & vbCrLf &
                 "The following study was graded " & dr("Grade") & strClinSignif & strDiscrepancy &
                 vbCrLf & vbCrLf & "Patient Name: " & "Unknown" & vbCrLf & "Accession: " & dr("Accession") &
                 vbCrLf & vbCrLf & " Study Comments: " &
                 dr("GradeComments") & vbCrLf & vbCrLf

                ' only send an email to the overreader (rad) reader if it is present
                If Not dr("OverreadingRad") Is System.DBNull.Value AndAlso dr("OverreadingRad") <> "None" Then
                    Dim HRTable As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
                    Dim HRRow As HREmpsDataSet.EmployeesRow

                    Try
                        HRRow = HRTable.Select("PACSID = '" & dr("OverreadingRad") & "'")(0)

                        ' use HRRow.Email_Address to send email - make sure it is not blank
                        If Not HRRow.Email_Address Is System.DBNull.Value And Not HRRow.Email_Address.Trim = "" Then
                            strMessage = "PLEASE DON'T REPLY, THIS EMAIL ACCOUNT IS NOT MONITORED" &
                            vbCrLf & vbCrLf & "THE QI SYSTEM WAS UNABLE TO ASCERTAIN WHO THE INITIAL READER WAS ON THIS CASE.  IF YOU WISH TO HAVE THAT PERSON SEE THE COMMENTS/GRADE YOU PROVIDED YOU MUST FORWARD TO THEM.  THEY HAVE NOT YET SEEN YOUR COMMENTS AND WILL NOT UNLESS YOU FORWARD." &
                            vbCrLf & vbCrLf & strMessageInit &
                            IIf(Not dr("ReadingRes") Is System.DBNull.Value, vbCrLf & vbCrLf & "Reading Resident: " & dr("ReadingRes"), "") &
                             vbCrLf & vbCrLf & constDisclaimer
                            ' also add the Response URL if not boolInfo=True  3/15/2005

                            If boolInfo Then
                                If strDiscrepancy = String.Empty Then
                                    strMessageHead = "COULD NOT SEND TO READER: Study Informational Notification"
                                Else
                                    strMessageHead = "COULD NOT SEND TO READER: Study Over-read Grade 1 Discrepancy Notification"
                                End If
                            ElseIf boolCommunication Then
                                strMessageHead = "COULD NOT SEND TO READER: Study Communication Notification"
                            Else
                                strMessageHead = "COULD NOT SEND TO READER: Study Over-read Grade 2 or 3 Notification"
                            End If

#If DEBUG Then
                            boolEmailSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value,
                                                                                  strMessage, strMessageHead, log)
#Else
                            boolEmailSent = email.EmailFileToRecipListSECURE(HRRow.Email_Address,
                             strMessage, strMessageHead, log)
#End If


                            ' Eff August 2018, no more grade = 4
                            '  for Grade = 3, Send an email notif to the appropriate chief;
                            '  as of 10/10/07 gwchief is used only if the chief lookup not known
                            If dr("Grade") = "3" Then
                                ' check for ChiefEmail for this PerfSite
                                Try
                                    boolEmailSent = email.ExchangeMailFiletoThisRecipientSECURE(parser.Entries.Item(7).Value,
                                    "THE QI SYSTEM WAS UNABLE TO ASCERTAIN WHO THE INITIAL READER WAS ON THIS CASE. YOU MUST PROVIDE THIS TO THE CHIEF, EMAIL WAS SENT TO THE OVERREADING RAD, BUT NOT TO INITIAL READER UNLESS OVERREADER FORWARDED" &
                                         vbCrLf & vbCrLf & strMessageInit &
                                         vbCrLf & vbCrLf & "Over-Reading Rad: " & HRRow.FullName & IIf(Not dr("ReadingRes") Is System.DBNull.Value, vbCrLf & "Reading Resident: " & dr("ReadingRes"), "") & IIf(strReviewType Like "*call*", vbCrLf & vbCrLf & "note: both the rad and resident are per the input of the resident on-call", "") &
                                         vbCrLf & vbCrLf & constDisclaimer, "COULD NOT FIND CASE DETAILS: COURTESY NOTIFICATION: Study Over-read Grade 3", log)

                                Catch ex As Exception
                                    log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                                    "Problem sending 'COULD NOT FIND CASE DETAILS: COURTESY NOTIFICATION: Study Over-read Grade 3' for email notif. " & ex.Message)
                                End Try
                            End If

                            If Not boolEmailSent Then
                                ' try to notify the monitor about this unsent email
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                "Failed send email notif to " & HRRow.Email_Address, "Email Notif to Rad failed", log)
                            End If
                        Else ' Email address of the rad initial reader is blank 
                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing email address for " & dr("OverreadingRad"), "Email of Rad missing", log)
                        End If
                    Catch ex As Exception
                        ' No PACSID for this particular rad - notify someone
                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing PACSID  for " & dr("OverreadingRad"), "PACS Code of Rad missing", log)
                    End Try

                End If ' initial reading rad field present?
            End If ' this is a T, O, or A

        Catch exOuter As Exception
            ' unexpected result - trouble doing something
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
            "Unexpected error: " & exOuter.Message & " while running in NotifyOverreaderOfNoSend!",
            "PACS QA Notify Overreader of No Send: Unexpected error", log)
            log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
            "Problem while running in NotifyOverreaderOfNoSend: " & exOuter.Message)
        End Try

    End Sub

    '  Imports System.Text.RegularExpressions

    '  Regular expression built for Visual Basic on: Tue, Oct 17, 2006, 01:46:42 PM
    '  Using Expresso Version: 2.1.1822, http://www.ultrapico.com
    '  
    '  A description of the regular expression:
    '  
    '  [1]: A numbered capture group. [[a-zA-Z]*]
    '      Any character in this class: [a-zA-Z], any number of repetitions
    '  Match expression but don't capture it. [[^a-zA-Z]*]
    '      Any character that is not in this class: [a-zA-Z], any number of repetitions
    '  
    '  

    Private regexAccNo As Regex = New Regex(
        "([a-zA-Z]*)(?:[^a-zA-Z]*)",
        RegexOptions.IgnoreCase _
        Or RegexOptions.CultureInvariant _
        Or RegexOptions.IgnorePatternWhitespace _
        Or RegexOptions.Compiled
        )

    Private Function ParseAccNoPrefix(ByVal strAccNo As String) As String
        Dim mc As MatchCollection = regexAccNo.Matches(strAccNo)
        Dim m As Match
        If mc.Count <> 0 Then
            m = mc.Item(0)
            'm.Groups(1).Value()
            'Debug.WriteLine(m.Value)
            ParseAccNoPrefix = m.Groups(1).Value
        Else
            ' no matches found for Accession Number known pattern - make it "" 
            ParseAccNoPrefix = ""
        End If

    End Function
    Private Function GetOverreaderFullName(strOverreaderID As String) As String
        ' Given a PACS ID of an Overreader, search the HR table to build full name in format: FirstName<space>Lastname
        Dim HRTable As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
        Dim HRRow As HREmpsDataSet.EmployeesRow
        GetOverreaderFullName = ""
        Try
            HRRow = HRTable.Select("PACSID = '" & strOverreaderID & "'")(0)
            ' build Full Name
            If Not HRRow.LastName Is System.DBNull.Value AndAlso Not HRRow.LastName.Trim = "" Then

                If Not HRRow.FirstName Is System.DBNull.Value AndAlso Not HRRow.FirstName.Trim = "" Then
                    GetOverreaderFullName = HRRow.FirstName & " " & HRRow.LastName
                Else
                    ' First name is missing, just use last
                    GetOverreaderFullName = HRRow.LastName
                End If
            Else '  LastName of the overreader is blank 
                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing last name for " & strOverreaderID, "HR database missing last name", log)
            End If
        Catch ex2 As Exception
            ' No PACSID for this particular rad - notify someone
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing PACSID for " & strOverreaderID,
            "PACS Code of Rad missing", log)
        End Try

    End Function

    Private Function GetInitialReader(ByVal drOverread As DataRow, ByVal IReader As String _
    , ByVal dtAltIDs As DataTable) As String
        Dim dr As SqlDataReader, drPACSID As SqlDataReader, drIRR As SqlDataReader
        Dim drIDs() As DataRow
        Try
            If drOverread("InitReadingRad") Is System.DBNull.Value Then
                If drOverread("Status") = "C" Or drOverread("Status") = "I" Then
                    ' then here we have an on-call overread with no init rad indicated, don't use the 
                    ' init reading rad from the PACS since this is the final reader!  10/14/04 PCB fix
                    GetInitialReader = ""
                Else
                    GetInitialReader = ""

                    ' We are given accession of the case.  Lookup this accession in the BROKER's ISIS_INTERPRETATION
                    ' table.  The field INTERPRETATION_AUTHOR will likely (1) have a value from field InitReadingRad
                    ' in SMGSQL table RadAlternateIDs (if DOS after about June 2004).  If no match, try (2) matching against
                    ' PACSID in table RadAlternateIDs.  If still no match then unknown.
                    ' 

                    If Len(IReader) > 1 Then

                        drIDs = dtAltIDs.Select("InitReadingRad = '" & IReader & "'")
                        ' Found a match in ISIS_INTERPRETATION
                        If drIDs.Length >= 1 Then 'drPACSID.HasRows
                            ' we matched, now return PACSID we found
                            GetInitialReader = IIf(drIDs(0).Item("PACSID") Is System.DBNull.Value, "", Trim(drIDs(0).Item("PACSID")))
                        Else ' try matching on PACSID
                            drIDs = dtAltIDs.Select("PACSID = '" & IIf(IReader Is System.DBNull.Value, "", Trim(IReader)) & "'")
                            If drIDs.Length >= 1 Then
                                ' we matched, now return PACSID we found
                                GetInitialReader = IIf(drIDs(0).Item("PACSID") Is System.DBNull.Value, "", Trim(drIDs(0).Item("PACSID")))
                            Else ' PCB 9-14-11 what was returned from PACS for init reader not in RadAltIDs table yet, make it known
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                "Couldn't find a match for " & drOverread("Accession") & " in RadAltIDs for doc value: " & IReader & " table. Check this out for accession: " & drOverread("Accession"), "Missing Original Reader in RadAltIDs", log)
                                log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                                "Couldn't find a match for " & drOverread("Accession") & " in RadAltIDs for doc value: " & IReader & " table. Check this out for accession: " & drOverread("Accession"))
                            End If
                        End If ' drPACSID.HasRows
                    Else ' nothing found matching Accession # in ISIS_INTERPRETATION
                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                        "Couldn't find a match for " & drOverread("Accession") & " in DOSR_STUDY table. Check this out for accession: " & drOverread("Accession"), "Missing Original Reader", log)
                        log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                        "Couldn't find a match for " & drOverread("Accession") & " in DOSR_STUDY table. Check this out for accession: " & drOverread("Accession"))
                    End If ' dr.HasRows
                    'dr.Close()
                    dr = Nothing


                End If ' is this a C or I (on-call case)
            Else ' here initial reader was given in the overread (on call case)
                GetInitialReader = drOverread("InitReadingRad")
            End If
        Catch ex As Exception
            ' Unknown error in GetInitialReader routine
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
            "Unknown error in GetInitialReader routine for Acc #" & drOverread("Accession") & " Full error: " & ex.Message, "Missing Original Reader", log)
            log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
            "Unknown error in GetInitialReader routine for Acc #" & drOverread("Accession") & " Full error: " & ex.Message)
            GetInitialReader = ""
            If Not dr Is Nothing AndAlso Not dr.IsClosed Then dr.Close()
        End Try

    End Function
    Private Sub NotifyInitialReaders(ByVal drComplete As DataRow, ByVal drOverread As DataRow)
        Dim boolEmailSent As Boolean = True
        Dim strMessage As String, strReviewType As String, strOverreaderName As String, strMessageHead As String
        Dim boolInfo As Boolean, boolErrorCode As Boolean, boolCommunication As Boolean = False
        Dim oChief As RetrieveSiteChief, strChiefEmail As String
        Dim strInitReaderFullName As String = String.Empty
        Dim strDivisionService As String = String.Empty
        Dim strInitialReaderFallback As String = IIf(parser.Entries.Item(15).HasValue, CType(parser.Entries.Item(15).Value, String), "")
        Dim strFallbackText As String = String.Empty
        Dim strServicingLocation As String
        Dim strCommunFailText As String = String.Empty

        ' Have not yet implemented the QA Response/Rebuttal system for EMMC yet  (12/30/2005)
        ' Is now in place 1/10/06

        ' 9/23/10 Per D Ahola, send email if FPPE (status = 'O')
        'If Not (dr("Status") = "T" Or dr("Status") = "A" Or dr("Status") = "O") Then
        If Not (drComplete("Status") = "T" Or drComplete("Status") = "A" Or drComplete("Status") = "L") Then

            strServicingLocation = If(Not (drComplete("PerfSite") Is DBNull.Value), drComplete("PerfSite"), "Unknown") & If(Not (drComplete("PerfSiteDept") Is DBNull.Value), " : " & drComplete("PerfSiteDept"), "")

            If Not (drComplete("Grade") Is System.DBNull.Value) AndAlso (drComplete("Grade") = "1" Or drComplete("Grade") = "2" Or drComplete("Grade") = "3" Or drComplete("Grade") = "4" Or drComplete("Grade") = "0") Then
                ' Find the matching Overread table record so we can examine the GradeComments column
                'drOverreads = myOverreadDS.Tables("Overread").Select("Accession = '" & CType(dr("AccessionNumber"), String).Trim & "'")
                'drOverreads = dt.Select("Accession = '" & CType(dr("AccessionNumber"), String).Trim & "' and Status = '" & dr("Status") & "'")
                ' First classify this study: boolInfo or boolErrorCode
                ' boolInfo = True means this is a GR=1 and comments present - info only
                ' boolErrorCode = True means this is a GR=2,3,4
                If (drComplete("Grade") = "1") AndAlso (Not (drOverread.Item("GradeComments") Is System.DBNull.Value) AndAlso Len(drOverread.Item("GradeComments")) > 0) Then
                    ' if this is a Grade = 1 and there are some comments, then this is an informational item only
                    boolInfo = True
                    boolErrorCode = False
                ElseIf (drComplete("Grade") = "0") AndAlso (Not (drOverread.Item("GradeComments") Is System.DBNull.Value) AndAlso Len(drOverread.Item("GradeComments")) > 0) Then
                    ' if this is a Grade = 0 and there are some comments, then this is a Communication item only
                    boolInfo = False
                    boolCommunication = True
                    boolErrorCode = True
                ElseIf (drComplete("Grade") = "2" Or drComplete("Grade") = "3" Or drComplete("Grade") = "4") Then
                    boolErrorCode = True
                    boolInfo = False
                Else
                    boolInfo = False
                    boolErrorCode = False
                End If

                If boolErrorCode Or boolInfo Then
                    ' First lookup the Review Type (to place in the email)
                    Try
                        If OverreadComponent.SqlConnection2.State = ConnectionState.Closed Then
                            OverreadComponent.SqlConnection2.Open()
                        End If
                        Dim strSQLLUP = "select [Review Type] from ReviewTypes where Status = '" & drComplete("Status") & "'"
                        Dim cmdLookup As New SqlCommand(strSQLLUP, OverreadComponent.SqlConnection2)
                        strReviewType = cmdLookup.ExecuteScalar().ToString()
                    Catch ex As Exception
                        ' Problem getting Review Type
                        ' write this to the log and send an email
                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                        "Problem getting Review Type for email notif. " & ex.Message, "Review Type lookup failure", log)
                        log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name,
                        "Problem getting Review Type for email notif. " & ex.Message)
                        strReviewType = ""
                    Finally
                        OverreadComponent.SqlConnection2.Close()
                    End Try
                    ' Look up Overreader code for EMMC since PACS ID is cryptic
                    Try
                        Dim HRTable_Overreader As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
                        Dim HRRow_Overreader As HREmpsDataSet.EmployeesRow
                        HRRow_Overreader = HRTable_Overreader.Select("PACSID = '" & Trim(drComplete("Overreader")) & "'")(0)
                        strOverreaderName = HRRow_Overreader.LastName & ", " & HRRow_Overreader.FirstName
                    Catch ex As Exception
                        strOverreaderName = Trim(drComplete("Overreader"))
                        ' No PACSID for this particular rad - notify someone
                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing PACSID for overreader: " & drComplete("Overreader"), "PACS Code of Rad missing", log)
                    End Try

                    Dim strClinSignif As String = GetClinSignifScore(drComplete("ClinSignifScore"))
                    Dim strDiscrepancy As String = GetDiscrepancyVerbiage(drComplete("DiscrepancyCognitive"),
                                                                          IIf(drComplete("DiscrepancySyntax") Is DBNull.Value, String.Empty, drComplete("DiscrepancySyntax").ToString()),
                                                                          drComplete("DiscrepancyCommun"))

                    ' Build the message for the email
                    Dim strMessageInit As String = "***This email was sent to you automatically, please do not reply to this email***" &
                     vbCrLf & vbCrLf & strReviewType & vbCrLf & vbCrLf &
                     "The following study was graded " & drComplete("Grade") & strClinSignif & strDiscrepancy &
                     vbCrLf & vbCrLf & "Name: " & drComplete("PatientName") & vbCrLf & "Accession: " & drComplete("AccessionNumber") & "  MRN: " & drComplete("MRN") &
                     " DOS: " & drComplete("DOS") & vbCrLf & vbCrLf & "Study: " & drComplete("ProcedureName") &
                     vbCrLf & "Indications/History: " & drComplete("PatientHistory") & vbCrLf & vbCrLf & " Study Comments: " &
                     drComplete("StudyComments") & vbCrLf & vbCrLf & "Referring provider:" & drComplete("ReferringPhysician") &
                     vbCrLf & "Requesting provider:" & drComplete("RequestingPhysician") &
                     vbCrLf & "Servicing location:" & strServicingLocation

                    If boolInfo Then

                        Dim strOpeningPhrase As String = IIf(strDiscrepancy <> String.Empty,
                                                             "The following study was graded 1, " & strDiscrepancy,
                                                             "The following informational comments for this study were provided (there was no disagreement)")

                        strMessageInit = "***This email was sent to you automatically, please do not reply to this email***" &
                                     vbCrLf & vbCrLf & strReviewType & vbCrLf & vbCrLf & strOpeningPhrase &
                             vbCrLf & vbCrLf & "Name: " & drComplete("PatientName") & vbCrLf & "Accession: " & drComplete("AccessionNumber") & "  MRN: " & drComplete("MRN") &
                             " DOS: " & drComplete("DOS") & vbCrLf & vbCrLf & "Study: " & drComplete("ProcedureName") &
                             vbCrLf & "Indications/History: " & drComplete("PatientHistory") & vbCrLf & vbCrLf & " Study Comments: " &
                             drComplete("StudyComments") & vbCrLf & vbCrLf & "Referring provider:" & drComplete("ReferringPhysician") &
                             vbCrLf & "Requesting provider:" & drComplete("RequestingPhysician") &
                             vbCrLf & "Servicing location:" & strServicingLocation
                    End If

                    ' only send an email to the Initial (rad) reader if it is present
                    If (Not drComplete("InitialReader") Is System.DBNull.Value AndAlso drComplete("InitialReader") <> "None" _
                        AndAlso Not String.IsNullOrEmpty(drComplete("InitialReader"))) OrElse (Not String.IsNullOrEmpty(strInitialReaderFallback)) Then

                        ' ****
                        Dim strEmailRecip As String
                        ' This is to catch emails that will not go to Initial (rad) reader when not present
                        ' this was introduced in May 2013 to handle NCMH lack of matching on initial readers
                        strInitReaderFullName = ""
                        strEmailRecip = strInitialReaderFallback
                        strDivisionService = drComplete("PerfSite") ' reasonable default valie for this if reader not known

                        If Not drComplete("InitialReader") Is System.DBNull.Value AndAlso drComplete("InitialReader") <> "None" _
                            AndAlso Not String.IsNullOrEmpty(drComplete("InitialReader")) Then
                            Dim HRTable As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
                            Dim HRRow As HREmpsDataSet.EmployeesRow

                            Try
                                HRRow = HRTable.Select("PACSID = '" & drComplete("InitialReader") & "'")(0)
                                If Not HRRow.IsFullNameNull Then _
                                    strInitReaderFullName = HRRow.FullName
                                If Not HRRow.IsDivisionNull Then _
                                 strDivisionService = HRRow.Division

                                strEmailRecip = HRRow.Email_Address

                            Catch ex As Exception
                                ' No PACSID for this particular rad - notify someone
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing PACSID  for " & drComplete("InitialReader"), "PACS Code of Rad missing", log)
                            End Try
                        Else ' here we must use the strInitialReaderFallback as recipient - add some explanatory text
                            strFallbackText = "This message unable to be sent to initial rad reader since person not known."
                        End If

                        ' ****

                        'Dim HRTable As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
                        'Dim HRRow As HREmpsDataSet.EmployeesRow
                        'Try
                        'HRRow = HRTable.Select("PACSID = '" & dr("InitialReader") & "'")(0)

                        ' use Email_Address to send email - make sure it is not blank
                        If Not strEmailRecip Is System.DBNull.Value AndAlso Not strEmailRecip.Trim = "" Then
                            strMessage = CreateResponseText(boolInfo, boolCommunication, drComplete, strFallbackText, strMessageInit, strCommunFailText)
                            ' also add the Response URL if not boolInfo=True  3/15/2005

                            If boolInfo Then
                                ' means grade = 1, check to see if communication failure
                                If strCommunFailText = String.Empty Then
                                    strMessageHead = "Study Informational Notification"
                                Else
                                    strMessageHead = "Study Over-read Grade 1 Discrepancy Notification"
                                End If
                            ElseIf boolCommunication Then
                                strMessageHead = "Study Communication Notification"
                            Else
                                strMessageHead = "Study Over-read Grade 2 or 3 Notification"
                            End If
#If DEBUG Then
                            boolEmailSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value,
                                                                             strMessage & vbCrLf & ">> this email sent to: " & strEmailRecip & " as address on file for initial reader: " &
                                                                             IIf(strInitReaderFullName = "", "Unknown", strInitReaderFullName) & " of Div/Service: " & IIf(strDivisionService = "", "Unknown", strDivisionService) & " <<",
                                                                             strMessageHead, log)
#Else
                            boolEmailSent = email.EmailFileToRecipListSECURE(strEmailRecip,
                             strMessage & vbCrLf & ">> this email sent to: " & strEmailRecip & " as address on file for initial reader: " &
                             IIf(strInitReaderFullName = "", "Unknown", strInitReaderFullName) & " of Div/Service: " & IIf(strDivisionService = "", "Unknown", strDivisionService) & " <<",
                             strMessageHead, log)
#End If


                            If Not boolEmailSent Then
                                ' try to notify the monitor about this unsent email
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                "Failed send email notif to " & strEmailRecip, "Email Notif to Rad failed", log)
                            End If
                            If Not drComplete("InitialReader") Is System.DBNull.Value _
                                AndAlso Not drComplete("OverReader") Is System.DBNull.Value _
                                AndAlso drComplete("InitialReader") = drComplete("OverReader") Then
                                ' This shouldn't really happen - send an email to monitor to check the validity of this (i.e. did resident enter the wrong initial reading rad?)
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                strMessage, "Rad QA - This case has initial reader = overreader", log)
                            End If
                        Else ' Email address of the rad initial reader is blank 
                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing email address for " & drComplete("InitialReader"), "Email of Rad missing", log)
                        End If
                        'Catch ex As Exception
                        '    ' No PACSID for this particular rad - notify someone
                        '    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing PACSID  for " & dr("InitialReader"), "PACS Code of Rad missing", log)
                        'End Try

                    End If ' initial reading rad field present?

                    ' effective Aug 2018 no more grade = 4
                    '  for Grade = 3, Send an email notif to the appropriate chief;
                    '  as of 10/10/07 gwchief is used only if the chief lookup not known
                    ' 12/23/13: Per DAhola email response as of ACR grades - sent 3 and 4
                    If drComplete("Grade") = "3" Then
                        Try
                            ' check for ChiefEmail for this PerfSite
                            strChiefEmail = oChief.GetSiteChiefEmail(OverreadComponent, drComplete("PerfSite"))
                            ' if what came back was nothing, use gwchief param
                            If strChiefEmail = "" Then strChiefEmail = CType(parser.Entries.Item(13).Value, String)
                        Catch ex As MissingPerfSiteException
                            ' here the PerFsite was not found
                            strChiefEmail = ""
                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                            "Missing PerfSite: " & drComplete("PerfSite") & " in RadQAEMMC.SiteChiefs table. Please add it and then manually send a Courtesy Grade=3 email to the correct chief", "Rad QA N Missing PerfSite", log, "")
                        Catch ex As Exception
                            strChiefEmail = CType(parser.Entries.Item(13).Value, String)
                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                            "Could not read SiteChiefs table for PerfSite: " & drComplete("PerfSite") & " in RadQAEMMC.SiteChiefs table. Courtesy Grade=3 email will be sent to the default chief, " &
                            IIf(parser.Entries.Item(13).HasValue, CType(parser.Entries.Item(13).Value, String), "empty!"), "Rad QA N Problem reading SiteChiefs", log, "")
                        End Try

                        If Not strChiefEmail = "" Then
                            Try
#If DEBUG Then
                                boolEmailSent = email.ExchangeMailFiletoThisRecipientSECURE(parser.Entries.Item(7).Value,
                                                                                            strMessageInit &
                                                                                            vbCrLf & vbCrLf & "Initial Reading Rad: " & IIf(strInitReaderFullName = "", "Unknown", strInitReaderFullName) & IIf(Not drComplete("ReadingRes") Is System.DBNull.Value AndAlso Not drComplete("ReadingRes") = "None", vbCrLf & "Reading Resident: " & drComplete("ReadingRes"), "") & vbCrLf & vbCrLf & constDisclaimer,
                                                                                            "COURTESY NOTIFICATION: Study Over-read Grade 3",
                                                                                            log, "", strCC)
#Else
                                boolEmailSent = email.ExchangeMailFiletoThisRecipientSECURE(strChiefEmail,
                                 strMessageInit &
                                 vbCrLf & vbCrLf & "Initial Reading Rad: " & IIf(strInitReaderFullName = "", "Unknown", strInitReaderFullName) & IIf(Not drComplete("ReadingRes") Is System.DBNull.Value AndAlso Not drComplete("ReadingRes") = "None", vbCrLf & "Reading Resident: " & drComplete("ReadingRes"), "") & vbCrLf & vbCrLf & constDisclaimer,
                                 "COURTESY NOTIFICATION: Study Over-read Grade 3",
                                 log, "", strCC)
#End If

                            Catch ex As Exception
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                "Emailed failed to site chief: " & strChiefEmail & ", message: " & ex.Message & ". Courtesy Grade=3 email was NOT sent to the chief. " _
                                , "Rad QA N Problem emailing Site Chief", log)
                            End Try

                        End If

                    End If ' dr("Grade") = "4" or "3" 
                End If ' is this an Info or ErrorCode case ?
            End If 'this is Grade 2 3 or 4
        End If ' this is T, O or A and we do not notify initial readers.
    End Sub

    Private Function CreateResponseText(boolInfo As Boolean, boolCommunication As Boolean, drComplete As DataRow,
                                        strFallbackText As String, strMessageInit As String, strCommunFailText As String) As String

        Return strFallbackText & vbCrLf & vbCrLf & "IF YOU WISH TO RESPOND TO THE PERSON WHO OVERREAD, DON'T REPLY, BUT" &
               IIf(boolCommunication OrElse (boolInfo AndAlso strCommunFailText = String.Empty), " FORWARD THIS EMAIL TO HIM/HER", " DO CLICK ON THE RESPONSE LINK HERE (https://qarad.spectrummg.com:887/radqa/PrimReadRespN.aspx?studyref=" & drComplete("SMGID") & ") or below.") & vbCrLf & vbCrLf & strMessageInit &
               IIf(Not drComplete("ReadingRes") Is DBNull.Value, vbCrLf & vbCrLf & "Reading Resident: " & drComplete("ReadingRes"), String.Empty) &
               vbCrLf & vbCrLf &
               IIf(boolCommunication OrElse (boolInfo AndAlso strCommunFailText = String.Empty), String.Empty, vbCrLf & vbCrLf & constRadQAResponseURL & "https://qarad.spectrummg.com:887/radqa/PrimReadRespN.aspx?studyref=" & drComplete("SMGID")) &
               vbCrLf & vbCrLf & constDisclaimer

    End Function

    Private Function GetClinSignifScore(strScore As String) As String
        Dim strClinSignif As String
        Try
            strClinSignif = IIf(Len(strScore) < 1, String.Empty, strScore)
        Catch ex As Exception
            ' if db value is null we'll get here - just make it empty and return
            strClinSignif = String.Empty
            Return strClinSignif
        End Try

        If strClinSignif.ToLower() = "a" Then
            strClinSignif = strClinSignif & " not clinically significant"
        ElseIf strClinSignif.ToLower() = "b" Then
            strClinSignif = strClinSignif & " clinically significant"
        End If

        Return strClinSignif
    End Function

    Private Function GetDiscrepancyVerbiage(strCognitive As String, strSyntax As String, boolCommunication As Boolean) As String
        Dim strVerbiage As String
        Try
            strVerbiage = IIf(Len(strCognitive) < 1, String.Empty, strCognitive)
        Catch ex As Exception
            ' if db value is null we'll get here - just make it empty
            strVerbiage = String.Empty
        End Try

        ' this assumes that strSyntax cannot be DBNull.Value
        If Not strVerbiage = String.Empty AndAlso Not strSyntax = String.Empty Then
            strVerbiage = strVerbiage & ", " & strSyntax
        ElseIf Not strSyntax = String.Empty Then
            strVerbiage = strSyntax
        End If

        Dim strCommun As String
        Try
            strCommun = IIf(Not boolCommunication, String.Empty, "communication")
        Catch ex As Exception
            ' if db value is null we'll get here - just make it empty
            strCommun = String.Empty
        End Try

        If Not strVerbiage = String.Empty AndAlso Not strCommun = String.Empty Then
            strVerbiage = strVerbiage & ", " & strCommun
        ElseIf Not strCommun = String.Empty Then
            strVerbiage = strCommun
        End If

        If Not strVerbiage = String.Empty Then
            strVerbiage = " with type(s) of discrepancy: " & strVerbiage
        End If

        Return strVerbiage
    End Function
    Private Sub FlushOverreadRow()
        ' Do the update on the Overreads Data adapter

        With OverreadComponent.SqlDataAdapter1
            .UpdateCommand.Transaction = Nothing
            .SelectCommand.Transaction = Nothing
            .DeleteCommand.Transaction = Nothing
            .ContinueUpdateOnError = True
            .Update(myOverreadDS, "Overread")
        End With

        ' Now check to see if a problem deleting
        Dim dtOverread As DataTable = myOverreadDS.Tables("Overread")
        Dim dtOverreadChanges As DataTable = dtOverread.GetChanges()
        If Not dtOverreadChanges Is Nothing AndAlso dtOverreadChanges.Rows().Count <> 0 Then ' if still changes=> not successful
            ' the error here is that couldn't delete records from Overread table 
            ' write this to the log and send an email
            log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name, _
            "Not able to delete at least one record from Overread table")
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
            "Not able to delete at least one record from Overread table", "Problem Deleting Overread Record", log)
        End If
    End Sub
    Private Sub RemoveOverreadRow(ByRef drOverread As DataRow)
        log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name, _
         "Status: deleting acc# " & drOverread("Accession") & " and Status:" & drOverread("Status") & " from Overread table")
        Dim drOverreads() As DataRow = myOverreadDS.Tables("Overread").Select("Accession = '" & drOverread.Item("Accession").Trim & "' and Status = '" & _
                                                                 drOverread.Item("Status").Trim & "'")
        drOverreads(0).Delete()
    End Sub

    Private Sub SetupCommandLineEntries(ByVal parser As CommandLineParser)

        Dim anEntry As CommandLineEntry
        parser.Errors.Clear()
        parser.Entries.Clear()
        ' create a flag type entry that accepts a -gwuser 
        ' flag, (meaning the next parameter is a Gwise user 
        ' name), and is required 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "gwuser")
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        ' store the new Entry in a local reference
        ' for use with the next CommandLineEntry's 
        ' MustFollow property.
        Dim gwEntry As CommandLineEntry
        gwEntry = anEntry

        ' now create a Value type entry that must
        ' follow the -gwuser flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.Value)
        anEntry.MustFollowEntry = gwEntry
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        ' create a flag type entry that accepts a -gwpwd 
        ' flag, (meaning the next parameter is a Gwise pwd 
        ' ), and is required 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "gwpwd")
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        gwEntry = anEntry

        ' now create a Value type entry that must
        ' follow the -gwpwd flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.Value)
        anEntry.MustFollowEntry = gwEntry
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        ' create a flag type entry that accepts a -gwtries 
        ' flag, (meaning the next parameter is # times to try 
        ' login), and is NOT required 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "gwtries")
        anEntry.Required = False
        parser.Entries.Add(anEntry)

        gwEntry = anEntry

        ' now create a RegExpression type entry that must
        ' follow the -gwtries flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.RegExpression, "\d{1,2}")
        anEntry.MustFollowEntry = gwEntry
        'anEntry.Required = True
        parser.Entries.Add(anEntry)

        ' create a flag type entry that accepts a -whocall 
        ' flag, (meaning the next parameter is GWise user 
        ' to notify if problems), and is required 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "whocall")
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        gwEntry = anEntry

        ' now create a Value type entry that must
        ' follow the -whocall flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.Value)
        anEntry.MustFollowEntry = gwEntry
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        ' create a flag type entry that accepts a -callwt 
        ' flag, (meaning the next parameter is # hours to wait 
        ' for an on-call reading to be completed 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "callwt")
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        gwEntry = anEntry

        ' now create a RegExpression type entry that must
        ' follow the -callwt flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.RegExpression, "\d{1,3}")
        anEntry.MustFollowEntry = gwEntry
        anEntry.Required = True
        parser.Entries.Add(anEntry)

        ' create a flag type entry that accepts a -gwserv 
        ' flag, (meaning the next parameter is # times to try 
        ' login), and is NOT required 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "gwserv")
        anEntry.Required = False
        parser.Entries.Add(anEntry)

        gwEntry = anEntry

        ' now create a Value type entry that must
        ' follow the -gwserv flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.Value)
        anEntry.MustFollowEntry = gwEntry
        'anEntry.Required = True
        parser.Entries.Add(anEntry)

        ' create a flag type entry that accepts a -gwchief 
        ' flag, (meaning the next parameter is GWise email address 
        ' of the person who gets all Grade 4 notices), and is NOT required 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "gwchief")
        anEntry.Required = False
        parser.Entries.Add(anEntry)

        gwEntry = anEntry

        ' now create a Value type entry that must
        ' follow the -gwchief flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.Value)
        anEntry.MustFollowEntry = gwEntry
        'anEntry.Required = True
        parser.Entries.Add(anEntry)


        ' create a flag type entry that accepts a -initreaderfallback 
        ' flag, (meaning the next parameter is GWise email address 
        ' of the person who gets all initial reader emails when reader not known), and is NOT required 
        anEntry = parser.CreateEntry _
           (CommandLineParse.CommandTypeEnum.Flag, "initreaderfallback")
        anEntry.Required = False
        parser.Entries.Add(anEntry)

        gwEntry = anEntry

        ' now create a Value type entry that must
        ' follow the -edfup flag.
        anEntry = parser.CreateEntry _
        (CommandTypeEnum.Value)
        anEntry.MustFollowEntry = gwEntry
        'anEntry.Required = True
        parser.Entries.Add(anEntry)
    End Sub
    Private Sub ProcessQAResponses()
        ' DirDivLate values determine what to do with each record:
        ' = 1 : the primary reader has responded, but email has not gone out
        ' = 1 :ACTION - send email to the Div Director and mark as 2 if disagree
        ' = 1 :ACTION - or send email to overreader and mark as 4 if agree
        ' = 2 : the email to DivDir has been sent but he hasn't responded
        ' = 2 :ACTION - if > 2 weeks, send email to monitor otherwise nothing
        ' = 3 : the DivDir has responded, but final email not gone out
        ' = 3 :ACTION - if DivDir agrees w/ overreader, send email to all
        ' = 3 :ACTION - if DivDir disagree "   "      ,send email to all and 
        ' = 3 :ACTION - send email to MONITOR to have him change grade and
        ' = 3 :ACTION - mark record as 4 (finalized)
        Dim dr As myResponseDataSet.QAResponsesRow
        Dim boolEmailSent As Boolean

        Try
            myResponseComp.FillDataSet(myResponseDS)
            ' Do we get here if there are no rows <> 4 ? - yes, but we fall out of the for each below
            Dim strQACompleteSQL As String = "select * from QAComplete where smgid = @sid"
            OverreadComponent.SqlConnection2.Open()
            Dim cmd As New SqlCommand(strQACompleteSQL, OverreadComponent.SqlConnection2)
            cmd.Parameters.Add(New System.Data.SqlClient.SqlParameter("@sid", System.Data.SqlDbType.Int, 4, "SMGID"))
            Dim drQAComplete As SqlDataReader

            ' Do something with each QAResponse record that is not a status 4 (which means that all intereaction is complete)
            For Each dr In myResponseDS.Tables("QAResponses").Rows
                If Not dr.IsDivDirLateNull Then
                    Select Case dr.DivDirLate
                        Case 1
                            ' = 1 : the primary reader has responded, but email has not gone out
                            ' = 1 :ACTION - send email to the Div Director and mark as 2 if disagree
                            ' = 1 :ACTION - or send email to overreader and mark as 4 if agree
                            Dim strDivDirMsg As String = _
                            "You have received this email from the PACS QA system." & _
                            vbCrLf & vbCrLf & _
                            "***This email was sent to you automatically, please do not reply to this email***"

                            Try
                                cmd.Parameters(0).Value = dr.SMGID
                                drQAComplete = cmd.ExecuteReader(CommandBehavior.SingleRow)
                                drQAComplete.Read()

                                Dim strClinSignif As String = GetClinSignifScore(drQAComplete("ClinSignifScore"))
                                Dim strDiscrepancy As String = GetDiscrepancyVerbiage(drQAComplete("DiscrepancyCognitive"),
                                                                                      IIf(drQAComplete("DiscrepancySyntax") Is DBNull.Value, String.Empty, drQAComplete("DiscrepancySyntax").ToString()),
                                                                                      drQAComplete("DiscrepancyCommun"))

                                strDivDirMsg = strDivDirMsg & vbCrLf & vbCrLf & _
                                "The following study was graded " & drQAComplete("Grade") & strClinSignif & strDiscrepancy & _
                                               vbCrLf & vbCrLf & "Name: " & drQAComplete("PatientName") & vbCrLf & "Accession: " & drQAComplete("AccessionNumber") & "  MRN: " & drQAComplete("MRN") & _
                                " DOS: " & drQAComplete("DOS") & vbCrLf & vbCrLf & "Study: " & drQAComplete("ProcedureName") & _
                                vbCrLf & "Indications/History: " & drQAComplete("PatientHistory") & vbCrLf & vbCrLf & " Study Comments: " & _
                                drQAComplete("StudyComments") & vbCrLf & vbCrLf & "Referring provider:" & drQAComplete("ReferringPhysician") & _
                                vbCrLf & "Requesting provider:" & drQAComplete("RequestingPhysician")

                                Dim strOverreader As String = ""

                                If dr.PrimaryAgree = "1" Then
                                    'Primary agrees with overreader, send overreader and email and we're done
                                    strOverreader = drQAComplete("Overreader")
                                    strDivDirMsg = strDivDirMsg & vbCrLf & vbCrLf & "The primary reader" & _
                                   " agrees with this overreading and responds with the following comments (if any): " & _
                                   IIf(dr.IsPrimaryCommentsNull OrElse dr.PrimaryComments = "", "<No Comments>", vbCrLf & " Primary Reader Comments: " & dr.PrimaryComments)
                                Else ' Primary disagrees with overreader, tell the div dir
                                    Dim strClinSignifPrim As String = GetClinSignifScore(dr.PrimaryAltClinSignifScore)
                                    Dim strDiscrepancyPrim As String = GetDiscrepancyVerbiage(
                                                                            IIf(dr.PrimaryDiscrepancyCognitive Is DBNull.Value, String.Empty, dr.PrimaryDiscrepancyCognitive.ToString()),
                                                                            IIf(dr.PrimaryDiscrepancySyntax Is DBNull.Value, String.Empty, dr.PrimaryDiscrepancySyntax.ToString()),
                                                                            dr.PrimaryDiscrepancyCommun)
                                    strDivDirMsg = strDivDirMsg & vbCrLf & vbCrLf & "The primary reader" & _
                                    " disagrees with this overreading and responds with the following: " & _
                                    vbCrLf & "Primary Reader Proposed grade: " & dr.PrimaryAltGrade & strClinSignifPrim & strDiscrepancyPrim & vbCrLf & _
                                    vbCrLf & " Primary Reader Comments: " & dr.PrimaryComments & _
                                    vbCrLf & vbCrLf & "You have been indicated as division director for this type of study and are asked " & _
                                    "to render an opinion in this case.  Please click on the link below, indicate your grade " & _
                                    "and comments.  Your grade will used as the final one on this overreading.  Your comments and grade will " & _
                                    "be made available to both the primary reader and overreader." & _
                                    vbCrLf & vbCrLf & "Click -> https://qarad.spectrummg.com:887/radqa/divdirrespn.aspx?studyref=" & _
                                    dr.SMGID.ToString
                                End If

                                drQAComplete.Close()

                                ' Get email address of div dir
                                Dim HRTable As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
                                Dim HRRow As HREmpsDataSet.EmployeesRow
                                Try
                                    Dim HRRowFound As Boolean = False
                                    Dim HRRows As DataRow() = HRTable.Select("PACSID = '" & IIf(dr.PrimaryAgree = "1", strOverreader, dr.DivisionDirector) & "'")

                                    If Not HRRows Is Nothing AndAlso HRRows.Length > 0 Then
                                        HRRow = HRRows(0)
                                        HRRowFound = True
                                    End If
                                    'HRRow = HRTable.Select("PACSID = '" & IIf(dr.PrimaryAgree = "1", strOverreader, dr.DivisionDirector) & "'")(0)
                                    ' use HRRow.Email_Address to send email - make sure it is not blank
                                    If HRRowFound AndAlso Not HRRow.Email_Address Is DBNull.Value _
                                        AndAlso Not HRRow.Email_Address.Trim = "" Then
                                        ' send email
#If DEBUG Then
                                        boolEmailSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value,
                                                                                              strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, IIf(dr.PrimaryAgree = "1", "PACS QA Response", "PACS QA Response Requested"), log)
#Else
                                        boolEmailSent = email.EmailFileToRecipListSECURE(HRRow.Email_Address, _
                                        strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, IIf(dr.PrimaryAgree = "1", "PACS QA Response", "PACS QA Response Requested"), log)
#End If


                                        If Not boolEmailSent Then
                                            ' try to notify the monitor about this unsent email
                                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                            "Failed send " & IIf(dr.PrimaryAgree = "1", "overreader agree", "div director") & " response email to " & HRRow.Email_Address, "Email Notif to Rad failed", log)
                                        Else
                                            ' Mark this rec as having been emailed to div dir
                                            ' or Done if this was an agree sent to the overreader
                                            dr.DivDirLate = IIf(dr.PrimaryAgree = "1", 4, 2)
                                        End If
                                    Else ' Email address of the div dir or overreader is blank 
                                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing email address for " & IIf(dr.PrimaryAgree = "1", strOverreader, dr.DivisionDirector), "Email of Rad missing", log)
                                    End If
                                Catch ex2 As Exception
                                    ' No PACSID for this particular rad - notify someone
                                    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
                                    "HR database missing PACSID  for " & IIf(dr.PrimaryAgree = "1", strOverreader, dr.DivisionDirector), _
                                    "PACS Code of Rad missing", log)
                                End Try

                            Catch ex3 As Exception
                                ' here an exception trying to read the QAComplete rec for this case
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
                                "Read of QAComplete for SMGID: " & dr.SMGID.ToString & " failed for reason: " & ex3.Message, _
                                "PACS QA response: Not Finding QAComplete", log)
                            End Try

                        Case 2
                            ' = 2 : the email to DivDir has been sent but he hasn't responded
                            ' = 2 :ACTION - if > 2 weeks, send email to monitor otherwise nothing
                            Dim ts As System.TimeSpan
                            ts = Now().Subtract(dr.PrimaryStamp)
                            If ts.Days >= constDaystoWaitforDivDir Then
                                ' late DivDir, alert monitor
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
                                "It has been more than 2 weeks w/o response from Div. Director: " & dr.DivisionDirector & " was sent an email re: response for SMGID: " & dr.SMGID.ToString, _
                                "PACS QA response: Div Director not responding", log)
                            Else
                                ' not enough time passed to alert about late DivDir
                                ' don't do anything
                            End If
                        Case 3
                            ' = 3 : the DivDir has responded, but final email not gone out
                            ' = 3 :ACTION - if DivDir agrees w/ overreader, send email to all
                            ' = 3 :ACTION - if DivDir disagree "   "      ,send email to all and 
                            ' = 3 :ACTION - send email to MONITOR to have him change grade and
                            ' = 3 :ACTION - mark record as 4 (finalized)
                            Dim strDivDirMsg As String = _
                            "You have received this email from the PACS QA system." & _
                            vbCrLf & vbCrLf & "***This email was sent to you automatically, please do not reply to this email***"
                            Try
                                cmd.Parameters(0).Value = dr.SMGID
                                drQAComplete = cmd.ExecuteReader(CommandBehavior.SingleRow)
                                drQAComplete.Read()

                                Dim strClinSignif As String = GetClinSignifScore(drQAComplete("ClinSignifScore"))
                                Dim strClinSignifPrim As String = GetClinSignifScore(dr.PrimaryAltClinSignifScore)
                                Dim strClinSignifDivDir As String = GetClinSignifScore(dr.DivDirAltClinSignifScore)
                                Dim strDiscrepancy As String = GetDiscrepancyVerbiage(drQAComplete("DiscrepancyCognitive"),
                                                                                      IIf(drQAComplete("DiscrepancySyntax") Is DBNull.Value, String.Empty, drQAComplete("DiscrepancySyntax").ToString()),
                                                                                      IIf(drQAComplete("DiscrepancyCommun") Is DBNull.Value, False, drQAComplete("DiscrepancyCommun")))
                                Dim strDiscrepancyPrim As String = GetDiscrepancyVerbiage(
                                    IIf(dr.PrimaryDiscrepancyCognitive Is DBNull.Value, String.Empty, dr.PrimaryDiscrepancyCognitive.ToString()),
                                    IIf(dr.PrimaryDiscrepancySyntax Is DBNull.Value, String.Empty, dr.PrimaryDiscrepancySyntax.ToString()),
                                    dr.PrimaryDiscrepancyCommun)
                                Dim strDiscrepancyDivDir As String = GetDiscrepancyVerbiage(
                                    IIf(dr.DivDirDiscrepancyCognitive Is DBNull.Value, String.Empty, dr.DivDirDiscrepancyCognitive.ToString()),
                                    IIf(dr.DivDirDiscrepancySyntax Is DBNull.Value, String.Empty, dr.DivDirDiscrepancySyntax.ToString()),
                                    dr.DivDirDiscrepancyCommun)


                                strDivDirMsg = strDivDirMsg & vbCrLf & vbCrLf & _
                                "The following study was graded " & drQAComplete("Grade") & strClinSignif & strDiscrepancy & _
                                vbCrLf & vbCrLf & "Name: " & drQAComplete("PatientName") & vbCrLf & "Accession: " & drQAComplete("AccessionNumber") & "  MRN: " & drQAComplete("MRN") & _
                                " DOS: " & drQAComplete("DOS") & vbCrLf & vbCrLf & "Study: " & drQAComplete("ProcedureName") & _
                                vbCrLf & "Indications/History: " & drQAComplete("PatientHistory") & vbCrLf & vbCrLf & " Study Comments: " & _
                                drQAComplete("StudyComments") & vbCrLf & vbCrLf & "Referring provider:" & drQAComplete("ReferringPhysician") & _
                                vbCrLf & "Requesting provider:" & drQAComplete("RequestingPhysician")

                                strDivDirMsg = strDivDirMsg & vbCrLf & vbCrLf & "The primary reader" & _
                                " disagreed with this overreading and responded with the following: " & _
                                vbCrLf & "Primary Reader Proposed grade: " & dr.PrimaryAltGrade & strClinSignifPrim & strDiscrepancyPrim & vbCrLf & _
                                vbCrLf & " Primary Reader Comments: " & dr.PrimaryComments & _
                                vbCrLf & vbCrLf & _
                                "The division director: " & dr.DivisionDirector & ", for this type of study was asked " & _
                                "to render an opinion in this case.  That person's response follows: " & _
                                vbCrLf & "Division Dir Proposed grade: " & dr.DivDirAltGrade & strClinSignifDivDir & strDiscrepancyDivDir & vbCrLf & _
                                vbCrLf & " Division Dir Comments: " & dr.DivDirComments

                                Select Case drQAComplete("Grade") = dr.DivDirAltGrade
                                    Case True
                                        ' Div Director agrees with the overreader's grade
                                        strDivDirMsg = strDivDirMsg & vbCrLf & vbCrLf & _
                                        "The division director's grade matches the overreader's, so the grade will not be changed."
                                    Case False
                                        ' Div Director does NOT agree with the overreader's grade
                                        strDivDirMsg = strDivDirMsg & vbCrLf & vbCrLf & _
                                        "The division director's grade does not match the overreader's; the grade will be changed to: " & _
                                        dr.DivDirAltGrade
                                End Select
                                Dim strOverreader As String = drQAComplete("Overreader")
                                Dim strInitReader As String = IIf(drQAComplete("InitialReader") Is DBNull.Value, String.Empty, drQAComplete("InitialReader"))
                                Dim strDivDir As String = dr.DivisionDirector

                                drQAComplete.Close()

                                ' Get email address of div dir
                                Dim HRTable As HREmpsDataSet.EmployeesDataTable = myHRDS.Tables(0)
                                Dim HRRow As HREmpsDataSet.EmployeesRow
                                Try
                                    ' send email to monitor
                                    boolEmailSent = email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
                                    strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "PACS QA Response Final", log)
                                    ' send to chief as courtesy
#If DEBUG Then
                                    boolEmailSent = email.ExchangeMailFiletoThisRecipientSECURE(CType(parser.Entries.Item(7).Value, String),
                                    strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "COURTESY NOTIFICATION: PACS QA Response Final", log)
#Else
                                    boolEmailSent = email.ExchangeMailFiletoThisRecipientSECURE(CType(parser.Entries.Item(13).Value, String), _
                                    strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "COURTESY NOTIFICATION: PACS QA Response Final", log)
#End If

                                    Dim HRRowFound As Boolean = False
                                    Dim HRRows As DataRow() = HRTable.Select("PACSID = '" & strDivDir & "'")

                                    If Not HRRows Is Nothing AndAlso HRRows.Length > 0 Then
                                        HRRow = HRRows(0)
                                        HRRowFound = True
                                    End If

                                    If HRRowFound AndAlso Not HRRow.Email_Address Is DBNull.Value _
                                        AndAlso Not HRRow.Email_Address.Trim = "" Then
                                        ' send email
#If DEBUG Then
                                        boolEmailSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value,
                                        strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "PACS QA Response Final", log)
#Else
                                        boolEmailSent = email.EmailFileToRecipListSECURE(HRRow.Email_Address, _
                                        strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "PACS QA Response Final", log)
#End If

                                        If Not boolEmailSent Then email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "Failed send email notif to " & HRRow.Email_Address, "PACS QA response:Email Notif to Rad failed", log)
                                    Else ' Email address of the DivDir is blank 
                                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing email address for " & strDivDir, $"Email of DivDir Rad missing Case 3 DivDirLate Id: {dr.SMGID}", log)
                                    End If

                                    If Not String.IsNullOrEmpty(strInitReader) Then
                                        HRRowFound = False
                                        HRRows = HRTable.Select("PACSID = '" & strInitReader & "'")

                                        If Not HRRows Is Nothing AndAlso HRRows.Length > 0 Then
                                            HRRow = HRRows(0)
                                            HRRowFound = True
                                        End If

                                        If HRRowFound AndAlso Not HRRow.Email_Address Is DBNull.Value _
                                            And Not HRRow.Email_Address.Trim = "" Then
                                            ' send email
#If DEBUG Then
                                            boolEmailSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value,
                                        strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "PACS QA Response Final", log)
#Else
                                        boolEmailSent = email.EmailFileToRecipListSECURE(HRRow.Email_Address, _
                                        strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "PACS QA Response Final", log)
#End If

                                            If Not boolEmailSent Then email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "Failed send email notif to " & HRRow.Email_Address, "PACS QA response:Email Notif to Rad failed", log)
                                        Else ' Email address of the InitReader is blank 
                                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing email address for " & strInitReader, $"Email of Rad missing Case 3 DivDirLate Id: {dr.SMGID}", log)
                                        End If
                                    End If

                                    HRRowFound = False
                                    HRRows = HRTable.Select("PACSID = '" & strOverreader & "'")

                                    If Not HRRows Is Nothing AndAlso HRRows.Length > 0 Then
                                        HRRow = HRRows(0)
                                        HRRowFound = True
                                    End If

                                    If HRRowFound AndAlso Not HRRow.Email_Address Is DBNull.Value _
                                        AndAlso Not HRRow.Email_Address.Trim = "" Then
                                        ' send email
#If DEBUG Then
                                        boolEmailSent = email.EmailFileToRecipListSECURE(parser.Entries.Item(7).Value,
                                        strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "PACS QA Response Final", log)
#Else
                                        boolEmailSent = email.EmailFileToRecipListSECURE(HRRow.Email_Address, _
                                        strDivDirMsg & vbCrLf & vbCrLf & constDisclaimer, "PACS QA Response Final", log)
#End If

                                        If Not boolEmailSent Then email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "Failed send email notif to " & HRRow.Email_Address, "PACS QA response:Email Notif to Rad failed", log)
                                    Else ' Email address of the Overreader is blank 
                                        email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, "HR database missing email address for " & strOverreader, $"Email of Rad missing Case 3 DivDirLate Id: {dr.SMGID}", log)
                                    End If

                                    dr.DivDirLate = 4

                                Catch ex33 As Exception
                                    ' No PACSID for a rad - notify someone
                                    email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value,
                                    $"Unexpected exception: {ex33.Message}, could be HR database missing PACSID for one or more of: {strOverreader}, {strInitReader}, {strDivDir}",
                                    $"PACS Code of Rad(s) missing for Response Id: {dr.SMGID}", log)
                                End Try

                            Catch ex23 As Exception
                                ' here an exception trying to read the QAComplete rec for this case
                                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
                                "Read of QAComplete for SMGID: " & dr.SMGID.ToString & " failed for reason: " & ex23.Message, _
                                "PACS QA response: Not Finding QAComplete", log)
                            End Try

                        Case Else
                            ' This is an unexpected result - error to log
                            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
                            "Unexpected value for DivDirLate field for SMGID: " & dr.SMGID.ToString & " don't know how to process!", _
                            "PACS QA response: Unexpected value for DivDirLate", log)
                    End Select

                End If
            Next
        Catch ex As Exception
            ' unexpected result - trouble reading QAResponses table for <> 4
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
            "Unexpected error: " & ex.Message & " while reading QAResponses table!", _
            "PACS QA response: Unexpected error", log)
        Finally
            OverreadComponent.SqlConnection2.Close()
        End Try

        ' if there actually were changes to the DivDirLate field, process these to the database
        If myResponseDS.HasChanges Then
            ' Update DivDirLate field to the database
            With myResponseComp.SqlDataAdapter1
                .ContinueUpdateOnError = True
                .Update(myResponseDS)
            End With
            ' Now check to see if a problem updating
            Dim dtResp As DataTable = myResponseDS.Tables("QAResponses")
            Dim dtRespChanges As DataTable = dtResp.GetChanges()
            If Not dtRespChanges Is Nothing AndAlso dtRespChanges.Rows().Count <> 0 Then ' if still changes=> not successful
                ' the error here is that couldn't update records in QAResponses table 
                ' write this to the log and send an email
                log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name, _
                "Not able to update at least one record in the QAResponses table")
                email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
                "Not able to update at least one record in the QAResponses table", "Problem Updating QAResponses Record", log)
            End If
        End If
    End Sub
#Region "GCCollect"
    ' This call is required in order to fully release the GroupWise COM object.
    ' What happens without this is that you still have the COM object even if
    ' you set it to Nothing (due to .Net garbage collection being non-deterministic).
    ' The symptom is that subsequent tries to login will fail
    ' after the first one failed due to someone else being logged in - even after the 
    ' other user logged out (this happens commonly on SMGINTRANET).
    Private Sub CallGCAndWait()

        ' I always got a type cast exception when calling Marshal.ReleaseComObject
        ' so I now just invoke the garbage collector and that seems to work

        'Dim i As Integer = 1
        'Do While i <> 0
        'i = Marshal.ReleaseComObject(o)
        'Loop

        System.GC.Collect()
        System.GC.WaitForPendingFinalizers()
    End Sub
#End Region
    Private Sub FillHRDatasetwithOutsideDocs(ByVal dsetHR As HREmpsDataSet)
        Dim compOD As New OutsideDocsComponent
        Dim dsetOD As New OutsideDocsDataSet
        Try
            compOD.FillDataSet(dsetOD)

            For Each drOD As OutsideDocsDataSet.OutsideDocsRow In dsetOD.OutsideDocs.Rows
                dsetHR.Employees.Rows.Add(drOD.ItemArray)
            Next

        Catch ex As Exception
            ' here there was some error accessing SQL Server or copying the OutsideDocs to the HR dset
            log.WriteFile(System.Reflection.MethodBase.GetCurrentMethod.Name, _
            "Could not copy OutsideDocs to the HR dset. Full message: " & ex.Message)
            email.ExchangeMailFiletoThisRecipient(parser.Entries.Item(7).Value, _
            "Could not copy OutsideDocs to the HR dset. Full message: " & ex.Message, "Rad QA Processor error", log)
        Finally
            dsetOD.Dispose()
            dsetOD = Nothing
            compOD.Dispose()
            compOD = Nothing
        End Try

    End Sub

    Private Class ExchangeWebMail
        Private strGWUser As String
        Private strGWServerPams As String
        Public Sub New(ByVal sGWUser As String, ByVal sGWServerPams As String)
            strGWUser = sGWUser
            strGWServerPams = sGWServerPams
        End Sub
        Public Function ExchangeMailFiletoThisRecipient(ByVal strRecip As String, ByVal strMsg As String, ByVal strSubj As String,
        ByVal LogWrite As LogWriter, Optional ByVal strFile As String = "", Optional ByVal strCC As String = "",
        Optional oPriority As System.Web.Mail.MailPriority = MailPriority.Normal) As Boolean

            Dim mail As New MailMessage, sendmailkludge As New MailMessage

            ExchangeMailFiletoThisRecipient = True
            Try
                mail.To = strRecip
                mail.From = strGWUser
                mail.Subject = strSubj
                mail.Body = strMsg
                mail.Priority = oPriority
                If strFile <> "" Then
                    Dim attachment As New MailAttachment(strFile) 'create the attachment
                    mail.Attachments.Add(attachment) 'add the attachment
                End If
                If strCC <> "" Then
                    mail.Cc = strCC
                End If

                sendmailkludge.To = strRecip
                sendmailkludge.From = strGWUser
                sendmailkludge.Subject = strSubj
                sendmailkludge.Body = strMsg
                sendmailkludge.Priority = oPriority
                If strFile <> "" Then
                    Dim attachment As New MailAttachment(strFile) 'create the attachment
                    sendmailkludge.Attachments.Add(attachment) 'add the attachment
                End If

                SmtpMail.SmtpServer = strGWServerPams
                SmtpMail.Send(mail)

                ' send one to special account for "sent items" record
                ' note: since this is a SMTP send there is no "sending" account 
                sendmailkludge.To = strGWUser
                SmtpMail.Send(sendmailkludge)

            Catch ex As Exception
                ExchangeMailFiletoThisRecipient = False
                LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                strMsg & " for this reason: " & ex.Message)
                If Not ex.InnerException Is Nothing Then
                    LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                                    strMsg & " for this reason: " & ex.InnerException.Message)
                End If
                If Not ex.InnerException.InnerException Is Nothing Then
                    LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                                    strMsg & " for this reason: " & ex.InnerException.InnerException.Message)
                End If
                If Not ex.InnerException.InnerException.InnerException Is Nothing Then
                    LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                                    strMsg & " for this reason: " & ex.InnerException.InnerException.InnerException.Message)
                End If
            End Try
        End Function
        'Public Function ExchangeMailFiletoThisRecipient(ByVal strRecip As String, ByVal strMsg As String, ByVal strSubj As String, _
        'ByVal LogWrite As GroupWiseClass.LogWriter, Optional ByVal strFile As String = "") As Boolean

        '    Dim mail As New MailMessage
        '    ExchangeMailFiletoThisRecipient = True
        '    Try
        '        mail.To = strRecip
        '        mail.From = strGWUser
        '        mail.Subject = strSubj
        '        mail.Body = strMsg
        '        If strFile <> "" Then
        '            Dim attachment As New MailAttachment(strFile) 'create the attachment
        '            mail.Attachments.Add(attachment) 'add the attachment
        '        End If
        '        SmtpMail.SmtpServer = strGWServerPams
        '        SmtpMail.Send(mail)

        '        ' send one to special account for "sent items" record
        '        ' note: since this is a SMTP send there is no "sending" account 
        '        mail.To = strGWUser
        '        SmtpMail.Send(mail)

        '    Catch ex As Exception
        '        ExchangeMailFiletoThisRecipient = False
        '        LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in VerQuadRat", "Failed to send email: " & _
        '        strMsg & " for this reason: " & ex.Message)
        '    End Try
        'End Function

        Public Function EmailFileToRecipListSECURE(ByVal strRecips As String, ByVal strMsg As String, ByVal strSubj As String,
        ByVal LogWrite As LogWriter, Optional ByVal strFile As String = "", Optional ByVal strCC As String = "",
        Optional ByVal oPriority As System.Web.Mail.MailPriority = MailPriority.Normal) As Boolean
            ' strRecips is assumed to be a ; delimited list of email addresses
            ' So this Function will SECURE email all addresses on the list
            ' but will allow comma, pipe and space delimited as well
            Dim delims As String() = New String() {";", ",", "|", " "}
            Dim boolSent As Boolean = False

            Try
                Dim strRecipsArray() As String = strRecips.Split(delims, StringSplitOptions.RemoveEmptyEntries)
                For i As Integer = 0 To strRecipsArray.GetUpperBound(0)
                    boolSent = ExchangeMailFiletoThisRecipientSECURE(strRecipsArray(i), strMsg, strSubj, LogWrite, strFile, strCC, oPriority)
                Next
            Catch ex As Exception
                boolSent = False
                LogWrite.WriteFile("EmailFileToRecipListSECURE in console1", "Failed to send email: " &
                                strMsg & " for this reason: " & ex.Message)
            Finally
                EmailFileToRecipListSECURE = boolSent
            End Try

        End Function

        Public Function ExchangeMailFiletoThisRecipientSECURE(ByVal strRecip As String, ByVal strMsg As String, ByVal strSubj As String,
ByVal LogWrite As LogWriter, Optional ByVal strFile As String = "", Optional ByVal strCC As String = "",
        Optional ByVal oPriority As System.Web.Mail.MailPriority = MailPriority.Normal) As Boolean
            ' uses the MMC Secure email system (puts CONFMSG in Subj line) to transmit secure email
            ' otherwise same functionality as Function ExchangeMailFiletoThisRecipient
            Dim mail As New MailMessage, sendmailkludge As New MailMessage

            ExchangeMailFiletoThisRecipientSECURE = True
            Try
                mail.To = strRecip
                mail.From = strGWUser
                mail.Subject = strSubj & " - CONFMSG"
                mail.Body = strMsg
                mail.Priority = oPriority
                If strFile <> "" Then
                    Dim attachment As New MailAttachment(strFile) 'create the attachment
                    mail.Attachments.Add(attachment) 'add the attachment
                End If
                If strCC <> "" Then
                    mail.Cc = strCC
                End If

                sendmailkludge.To = strRecip
                sendmailkludge.From = strGWUser
                sendmailkludge.Subject = strSubj
                sendmailkludge.Body = "Was sent to: " & strRecip & " " & vbCrLf & vbCrLf & strMsg
                sendmailkludge.Priority = oPriority
                If strFile <> "" Then
                    Dim attachment As New MailAttachment(strFile) 'create the attachment
                    sendmailkludge.Attachments.Add(attachment) 'add the attachment
                End If

                SmtpMail.SmtpServer = strGWServerPams
                SmtpMail.Send(mail)

                ' send one to special account for "sent items" record
                ' note: since this is a SMTP send there is no "sending" account 
                sendmailkludge.To = strGWUser
                SmtpMail.Send(sendmailkludge)

            Catch ex As Exception
                ExchangeMailFiletoThisRecipientSECURE = False
                LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                strMsg & " for this reason: " & ex.Message)
                If Not ex.InnerException Is Nothing Then
                    LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                                    strMsg & " for this reason: " & ex.InnerException.Message)
                End If
                If Not ex.InnerException.InnerException Is Nothing Then
                    LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                                    strMsg & " for this reason: " & ex.InnerException.InnerException.Message)
                End If
                If Not ex.InnerException.InnerException.InnerException Is Nothing Then
                    LogWrite.WriteFile("ExchangeMailFiletoThisRecipient in console1", "Failed to send email: " &
                                    strMsg & " for this reason: " & ex.InnerException.InnerException.InnerException.Message)
                End If
            End Try
        End Function

    End Class

End Module
