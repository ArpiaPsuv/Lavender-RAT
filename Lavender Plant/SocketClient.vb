﻿Imports System.Net
Imports System.Net.Sockets
Public Class SocketClient
    Private C As TcpClient
    Public Event Connected()
    Public Event Disconnected()
    Public Event Data(ByVal b As Byte())
    Private IsBusy As Boolean = False

    Public Function Statconnected() As Boolean
        Try
            If C.Client.Connected = True Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
    Sub Connect(ByVal h As String, ByVal p As Integer)
        Try
            Try
                If C IsNot Nothing Then
                    C.Close()
                    C = Nothing
                End If
            Catch ex As Exception
                ''MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: REMOVE!
            End Try
            Do Until IsBusy = False
                Threading.Thread.Sleep(1)
            Loop
            Try
                C = New TcpClient
                C.Connect(h, p)
                Dim t As New Threading.Thread(AddressOf RC, 10)
                t.Start()
                RaiseEvent Connected()
            Catch ex As Exception
                ''MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: REMOVE!
            End Try
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: REMOVE!
            RaiseEvent Disconnected()
        End Try
    End Sub
    Private SPL As String = Crypt.RC4.rc4("=0-0=", Main.pw) ' split packets by this word
    Sub DisConnect()
        ''MsgBox("Discon")
        Try
            C.Close()
        Catch ex As Exception
            ''MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: REMOVE!
        End Try
        C = Nothing
        RaiseEvent Disconnected()
    End Sub
    Sub Send(ByVal s As String)
        Try
            Send(SB(Crypt.RC4.rc4(s, Main.pw)))
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: ABSOLUTELY REMOVE!
        End Try

    End Sub
    Sub Send(ByVal b As Byte())
        Try
            Dim m As New IO.MemoryStream
            m.Write(b, 0, b.Length)
            m.Write(SB(SPL), 0, SPL.Length)
            C.Client.Send(m.ToArray, 0, m.Length, SocketFlags.None)
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.ToString) ''TODO: REMOVE!
            DisConnect()
        End Try
    End Sub
    Private Sub RC()
        IsBusy = True
        Dim M As New IO.MemoryStream
        Dim cc As Integer = 0
re:
        Threading.Thread.Sleep(1)

        Try
            If C Is Nothing Then
                GoTo co
            Else
                If C.Client.Connected = False Then
                    GoTo co
                Else
                    cc += 1
                    If cc > 100 Then
                        cc = 0
                        Try
                            If C.Client.Poll(-1, Net.Sockets.SelectMode.SelectRead) And C.Client.Available <= 0 Then
                                GoTo co
                            End If
                        Catch ex As Exception
                            ''MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: REMOVE!
                            GoTo co
                        End Try
                    End If

                End If
            End If
            If C.Available > 0 Then
                Dim B(C.Available - 1) As Byte
                C.Client.Receive(B, 0, B.Length, Net.Sockets.SocketFlags.None)
                M.Write(B, 0, B.Length)
rr:
                If BS(M.ToArray).Contains(SPL) Then
                    Dim A As Array = fx(M.ToArray, SPL)
                    RaiseEvent Data(A(0))
                    M.Dispose()
                    M = New IO.MemoryStream
                    If A.Length = 2 Then
                        M.Write(A(1), 0, A(1).length)
                        Threading.Thread.Sleep(1)
                        GoTo rr
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message & vbNewLine & ex.StackTrace) ''TODO: REMOVE!
            GoTo co
        End Try
        GoTo re
co:
        IsBusy = False
        DisConnect()
    End Sub
End Class
