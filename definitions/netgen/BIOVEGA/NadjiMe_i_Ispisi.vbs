

REM Set conn= CreateObject("ADODB.Connection")
REM Set rs= CreateObject("ADODB.Recordset")

REM ConnStr = "Driver={SQL Server};Server=" & inputbox("Server ?","","Dejan") & ";Database=" & inputbox("Baza ?","","JupiterSystem") & ";UID=sa;PWD=;"

REM conn.Open ConnStr 
REM Conn.CursorLocation = 3


Const ForReading = 1 
REM Const ForWriting = 2 
Dim ShtoGdje
Dim ShtoGdjeOpet
Dim searchword
Dim objFSO
Dim objFile
Dim strFile
Dim sText
Dim StrFolderName
Dim ObjFolder
Dim ObjFilesCollection
Dim ObjFoldersCollection
dim myfilterarray
Dim DLLName
Dim FormName
Dim ZaSpremiti
dim sql
On Error Resume Next   



Dim folder, files, NewsFile,sFolder 
    
Set objFSO = CreateObject("Scripting.FileSystemObject")   
sFolder = ScriptPath
If sFolder = "" Then       
    Wscript.Echo "Mapa ne postoji"      
    Wscript.Quit   
End If   

 Set folder = objFSO.GetFolder(sFolder)   
 Set files = folder.Files   
 Set ObjFilesCollection=files'ObjFolder.Files
   
 

		Set objdir = objFSO.GetFolder(sFolder)
		ZaSpremiti = objDir.path
		getInfo(objDir)

		Sub getInfo(pCurrentDir)

		For Each objFile In pCurrentDir.Files
		   'wscript.Echo aItem.Name
		   shtogdje = ""
		   strfile = objfile
		    shtoGdje = objfile.path
			Call ReadFile 
		Next

		For Each aItem In pCurrentDir.SubFolders
		   'wscript.Echo aItem.Name & " passing recursively"
		   getInfo(aItem)
		Next

		End Sub

 		call NotApplicable


Function ReadFile()
    On Error Resume Next
    Set objFSO = CreateObject("Scripting.FileSystemObject")     ' kreiranje objekta
    Set objFile = objFSO.OpenTextFile(strFile,ForReading) 	' otvaranje dokumenta
    sText = objFile.ReadAll					' citanje dokumenta
    'Set objFile = Nothing 					' zatvaranje dokumenta
    'Set objFSO = Nothing 					' zatvaranje objekta
    If Err = 0 Then
        ReadFile = True
		searchword = "EPotvrdeNarudzbiRedok"
		if inStr(sText,searchword) > 0 then
			ShtoGdjeOpet = shtogdjeopet & vbcrlf & shtogdje
		end if
    End If
End Function 


Function ScriptPath()
  ScriptPath = CreateObject("Scripting.FileSystemObject")._
               GetParentFolderName(Wscript.ScriptFullName)
End Function


sub NotApplicable
	on error resume next
	REM Wscript.Echo shtogdje
	if err.number<>0 then
          msgbox err.description & chr(13) & sql & chr(13) & sql,16,"VBStxt"
	  exit sub
        end if


	set fso = CreateObject("Scripting.FileSystemObject")
	set f =fso.CreateTextFile(ZaSpremiti & "\vbstxt.txt",true)

	f.Write(shtogdjeopet)
	
	f.Write(chr(13) & chr(10))
	f.WriteLine("--------------------------------------------------------------------------------")


	f.close
	set f=nothing
	set fso=nothing

	With CreateObject("WScript.Shell")
		Call .Run(ZaSpremiti & "\vbstxt.txt", 1, False)
	End With


end sub

REM Call ParseFile

Wscript.Echo "Gotovo"

on error resume next
