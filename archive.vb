'Imports System.ComponentModel
'Imports System.Text.RegularExpressions
'Imports System.Windows.Forms.VisualStyles.VisualStyleElement

'Module archive
'    Private Sub BWGenPattern_DoWork(sender As Object, e As DoWorkEventArgs)
'        ' Pattern C form -- When start button was pressed for a substring size, the background process
'        ' Perform the background operation
'        Dim setSize As Byte = DTMAIN.Rows(0)("ID").Length

'        Dim table As System.Data.DataTable = DTMAIN

'        Dim chunk As Integer = TextBox3.Text

'        'Dim filteredRows As IEnumerable(Of DataRow) = table.AsEnumerable().Where(Function(row) Not Regex.IsMatch(row.Field(Of String)("ID"), PATTERN))
'        'Dim filteredDataTable As DataTable = filteredRows.CopyToDataTable()
'        Dim i As Long = 0
'        Dim j As Long = 0


'        Dim table2 As New System.Data.DataTable
'        table2.Columns.Add("ID")

'        Dim temp As Boolean = generatePattern(TEMPBP, chunk)
'        If Not temp Then
'            MsgBox("Process aborted due to an error",, "Warning")
'            Exit Sub
'        End If

'        DSFILTERMATCH.Tables.Add(table2)

'        'Looping through rows
'        For Each row As DataRow In table.Rows
'            If BWGenPattern.CancellationPending Then
'                e.Cancel = True
'                Exit For
'            End If

'            i += 1

'            ' Calculate the time remaining
'            If i Mod 1000 = 0 Then
'                Dim currentTime As DateTime = DateTime.Now
'                Dim timeElapsed As TimeSpan = currentTime - startTime
'                Dim averageStepDuration As Double = timeElapsed.TotalMilliseconds / i
'                Dim remainingSteps As Integer = ProgressBar1.Maximum - i
'                Dim timeRemaining As TimeSpan = TimeSpan.FromMilliseconds(averageStepDuration * remainingSteps)

'                ' Update the time remaining label-
'                Me.Invoke(Sub() Label3.Text = String.Format("Time Remaining: {0:mm\:ss}", timeRemaining))
'                ' Report progress to update the UI
'                BWGenPattern.ReportProgress(i)
'            End If


'            ' Heart of the code
'            Dim val As String = row("ID").ToString()

'            Dim tempTable As Data.DataTable = DSFILTERMATCH.Tables(0)

'            If Regex.IsMatch(val, PATTERN) Then
'                'tempTable.Rows.Add(val)
'                row("Delete") = 1
'                j += 1
'            Else
'                row("Delete") = 0
'            End If


'        Next

'        ' Adding to listbox
'        Dim listItem As New ListBoxItem With {
'        .size = chunk,
'        .tableIndex = 0,
'        .matchCount = j
'        }

'        listItem.setText()

'        LISTITEMS.Add(listItem)

'    End Sub
'End Module
