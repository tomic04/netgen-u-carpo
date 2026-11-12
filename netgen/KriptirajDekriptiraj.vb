Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Security.Cryptography
Imports System.IO

Namespace KriptirajDekriptiraj
    NotInheritable Class EncryptDecrypt
        Private Sub New()
        End Sub
        Public Shared Function Encrypt(ByVal stringToEncrypt As String, ByVal stringKey As String) As Byte()
            Dim key As New DESCryptoServiceProvider()

            Dim bts As Byte() = KeyToBytes(stringKey)

            key.Key = bts
            key.IV = bts

            Dim ms As New MemoryStream()
            Dim encStream As New CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write)
            Dim sw As New StreamWriter(encStream)

            sw.WriteLine(stringToEncrypt)

            sw.Close()
            encStream.Close()

            Dim buffer As Byte() = ms.ToArray()
            ms.Close()

            Return buffer
        End Function

        Public Shared Function Decrypt(ByVal CypherText As Byte(), ByVal stringKey As String) As String

            Dim key As New DESCryptoServiceProvider()

            Dim bts As Byte() = KeyToBytes(stringKey)

            key.Key = bts
            key.IV = bts

            Dim ms As New MemoryStream(CypherText)
            Dim encStream As New CryptoStream(ms, key.CreateDecryptor(), CryptoStreamMode.Read)
            Dim sr As New StreamReader(encStream)

            Dim val As String = sr.ReadToEnd

            sr.Close()
            encStream.Close()
            ms.Close()

            Return val
        End Function

        Private Shared Function KeyToBytes(ByVal k As String) As Byte()
            Dim keyLenInBytes As Integer = 8

            k = k.Trim()

            If k.Length < keyLenInBytes Then
                k = k.PadRight(keyLenInBytes, "X"c).Substring(0, keyLenInBytes)
            End If
            If k.Length > keyLenInBytes Then
                k = k.Substring(0, 8)
            End If

            Dim bts As Byte() = ASCIIEncoding.UTF8.GetBytes(k)

            Return bts
        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik, @toddanglin
'Facebook: facebook.com/telerik
'=======================================================
