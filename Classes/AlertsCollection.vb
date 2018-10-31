﻿Option Strict On
Option Explicit On
Public Class AlertsCollection
    Inherits Collections.Generic.Dictionary(Of String, AlertsItem)
    Private mAlertsLoaded As Boolean = False
    Public Sub Load()
        Dim pobjConn As New SqlClient.SqlConnection(UtilitiesDB.ConnectionStringPNR) ' ActiveConnection)
        Dim pobjComm As New SqlClient.SqlCommand
        Dim pobjReader As SqlClient.SqlDataReader
        Dim pobjClass As AlertsItem

        pobjConn.Open()
        pobjComm = pobjConn.CreateCommand

        MyBase.Clear()

        With pobjComm
            .CommandType = CommandType.Text
            .CommandText = "SELECT pnaID " &
                               "     , ISNULL(pnaBOId_fkey, 0) AS pnaBOId_fkey " &
                               "     , ISNULL(pnaClientCode, '') AS pnaClientCode " &
                               "     , pnaAlert " &
                               "     , ISNULL(pnaOriginCountry, '') AS pnaOriginCountry " &
                               "     , ISNULL(pnaDestinationCountry, '') AS pnaDestinationCountry " &
                               "     , ISNULL(pnaAirline, '') AS pnaAirline " &
                               "     , ISNULL(pnaAmadeusQueue, '') AS pnaAmadeusQueue " &
                               "     , ISNULL(pnaGalileoQueue, '') AS pnaGalileoQueue " &
                               "FROM [AmadeusReports].[dbo].[PNRFinisherAlerts]"
            pobjReader = .ExecuteReader
        End With

        With pobjReader
            Do While .Read
                pobjClass = New AlertsItem
                pobjClass.SetValues(CInt(.Item("pnaBOId_fkey")), CStr(.Item("pnaClientCode")), CStr(.Item("pnaAlert")), CStr(.Item("pnaOriginCountry")), CStr(.Item("pnaDestinationCountry")), CStr(.Item("pnaAirline")), CStr(.Item("pnaAmadeusQueue")), CStr(.Item("pnaGalileoQueue")))
                MyBase.Add(.Item("pnaID").ToString, pobjClass)
            Loop
        End With
        mAlertsLoaded = True
    End Sub
    Public ReadOnly Property Alert(ByVal pBackOfficeId As Integer, ByVal pClientCode As String) As String
        Get
            Alert = ""
            For Each pItem As AlertsItem In MyBase.Values
                If pItem.BackOfficeID = pBackOfficeId And pClientCode = pItem.ClientCode Then
                    Alert = pItem.Alert
                    Exit For
                End If
            Next
        End Get
    End Property
    Public ReadOnly Property Alert(ByVal pOriginCountry As String, ByVal pDestinationCountry As String) As String
        Get
            Alert = ""
            For Each pItem As AlertsItem In MyBase.Values
                If (pItem.OriginCountry = pOriginCountry And pItem.DestinationCountry = pDestinationCountry) _
                        Or (pItem.OriginCountry = pOriginCountry And pItem.DestinationCountry = "") _
                        Or (pItem.DestinationCountry = pDestinationCountry And pItem.OriginCountry = "") Then
                    Alert &= pItem.Alert & vbCrLf
                End If
            Next
        End Get
    End Property
    Public ReadOnly Property AirlineAlert(ByVal AirlineCode As String) As String
        Get
            If Not mAlertsLoaded Then
                Load()
            End If
            AirlineAlert = ""
            For Each pItem As AlertsItem In MyBase.Values
                If pItem.Airline = AirlineCode AndAlso AirlineAlert.IndexOf(pItem.Alert) = -1 Then
                    AirlineAlert &= pItem.Alert & vbCrLf
                End If
            Next
        End Get
    End Property
    Public ReadOnly Property AmadeusQueue(ByVal AirlineCode As String) As String
        Get
            If Not mAlertsLoaded Then
                Load()
            End If
            AmadeusQueue = ""
            For Each pItem As AlertsItem In MyBase.Values
                If pItem.Airline = AirlineCode And pItem.AmadeusQueue <> "" Then
                    AmadeusQueue = pItem.AmadeusQueue
                End If
            Next
        End Get
    End Property
    Public ReadOnly Property GalileoQueue(ByVal AirlineCode As String) As String
        Get
            If Not mAlertsLoaded Then
                Load()
            End If
            GalileoQueue = ""
            For Each pItem As AlertsItem In MyBase.Values
                If pItem.Airline = AirlineCode And pItem.GalileoQueue <> "" Then
                    GalileoQueue = pItem.GalileoQueue
                End If
            Next
        End Get
    End Property

End Class
