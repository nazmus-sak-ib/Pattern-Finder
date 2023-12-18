Imports System.ComponentModel
Imports System.IO
Imports System.Reflection.Emit
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar
Public Class PatternA
    Private LISTITEMS As New List(Of ListBoxItem)
    Private STARTTIME As DateTime                   ' For estimated time remaining calculations

    Private Sub ProceedPattern_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Start button
        Dim setSize As Byte = DTMAIN.Rows(0)("ID").Length

        ' Datasets management
        DSPATTERNS = New DataSet
        DSFILTERMATCH = New DataSet

        For i = 1 To setSize
            Try
                '' Deleting "delete" columns to start over
                DTMAIN.Columns.Remove("Delete" & i)
            Catch
            End Try

            '' Recreating "delete" columns
            Dim newColumn As New DataColumn("Delete" & i, GetType(Integer)) With {
                .DefaultValue = 0
            }

            DTMAIN.Columns.Add(newColumn)
        Next


        ' Refreshing display items
        ProgressBar1.Value = 0
        ProgressBar1.Maximum = DTMAIN.Rows.Count
        Button1.Enabled = False
        Button2.Enabled = False
        Button3.Enabled = False
        Button4.Enabled = False
        Button5.Enabled = True
        LISTITEMS = New List(Of ListBoxItem)
        ListBox1.Items.Clear()
        DataGridView1.DataSource = Nothing

        ' Start Asynchronous background process
        BWGenPattern.RunWorkerAsync()
        STARTTIME = DateTime.Now

        Button2.Enabled = True
        Button3.Enabled = True
        Button4.Enabled = True
        Button5.Enabled = False
    End Sub
    Private Sub BWGenPattern_DoWork(sender As Object, e As DoWorkEventArgs) Handles BWGenPattern.DoWork
        ' Perform the background operation
        Dim setSize As Byte = DTMAIN.Rows(0)("ID").Length

        Dim table As System.Data.DataTable = DTMAIN

        Dim progressCount As Long = 0                           ' Progress bar reporting
        Dim rowCounts As New List(Of Long)                       ' Count of rows in a DSFILTERMATCH table

        ' Generating patterns for each possible chunk suze
        For chunk = 1 To setSize
            Dim temp As Boolean = generatePattern(TEMPBP, chunk)
            If Not temp Then
                MsgBox("Process aborted due to an error",, "Warning")
                Exit Sub
            End If

            rowCounts.Add(0)
        Next


        ' Looping through rows of DTMAIN
        For Each row As DataRow In table.Rows
            If BWGenPattern.CancellationPending Then
                e.Cancel = True
                Exit For
            End If

            progressCount += 1

            ' Calculate the time remaining
            If progressCount Mod 1000 = 0 Then
                Dim currentTime As DateTime = DateTime.Now
                Dim timeElapsed As TimeSpan = currentTime - STARTTIME
                Dim averageStepDuration As Double = timeElapsed.TotalMilliseconds / progressCount
                Dim remainingSteps As Integer = ProgressBar1.Maximum - progressCount
                Dim timeRemaining As TimeSpan = TimeSpan.FromMilliseconds(averageStepDuration * remainingSteps)

                ' Update the time remaining label
                Me.Invoke(Sub() Label3.Text = String.Format("Time Remaining: {0:mm\:ss}", timeRemaining))
                ' Report progress to update the UI
                BWGenPattern.ReportProgress(progressCount)
            End If

            ' Heart of the code
            Dim val As String = row("ID").ToString()

            '' For each chunk size
            For chunk = 1 To setSize
                Dim table2 As Data.DataTable = DSPATTERNS.Tables(chunk - 1)

                '' 'Taking substrings out of a blackNumber
                For k = 0 To val.Length - chunk
                    Dim temp2 As String = val.Substring(k, chunk)

                    If table2.Rows.Contains(temp2) Then
                        row("Delete" & chunk) = 1
                        rowCounts(chunk - 1) += 1
                        Exit For
                    Else
                    End If
                Next
            Next
        Next

        ' Adding to listbox
        For chunk = 1 To setSize
            Dim listItem As New ListBoxItem With {
            .size = chunk,
            .tableIndex = chunk - 1,
            .matchCount = rowCounts(chunk - 1)
            }

            listItem.setText()

            LISTITEMS.Add(listItem)

        Next
    End Sub
    Private Sub BWGenPattern_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BWGenPattern.ProgressChanged
        ' Update the UI based on the progress
        ProgressBar1.Value = e.ProgressPercentage
    End Sub
    Private Sub BWGenPattern_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BWGenPattern.RunWorkerCompleted
        ' Check if the operation was completed successfully or canceled
        If e.Cancelled Then
            MessageBox.Show("Operation canceled.")
            Label3.Text = "Cancelled"
        ElseIf e.Error IsNot Nothing Then
            MessageBox.Show("Error occurred: " & e.Error.Message)
        Else
            For Each l As ListBoxItem In LISTITEMS
                ListBox1.Items.Add(l)
            Next
            MessageBox.Show("Operation completed successfully.")
            Button2.Enabled = False
            Button3.Enabled = False
            Button4.Enabled = False
            Button5.Enabled = True
            ProgressBar1.Value = ProgressBar1.Maximum
            Label3.Text = "Complete"
        End If

        Button1.Enabled = True
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Cancel the background operation if it is running
        If BWGenPattern.IsBusy Then
            BWGenPattern.CancelAsync()
        End If
    End Sub


    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        ' Exit form
        Dim response As MsgBoxResult = MsgBox("Do you want return to the main page?", vbOKCancel, "Confirm")

        If response = vbOK Then
            MyBase.Close()
        End If
    End Sub

    Private Sub ViewPattern_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' View Pattern
        Dim ind As Integer = ListBox1.SelectedIndex

        If ind <> -1 Then
            Dim item As ListBoxItem = ListBox1.SelectedItem
            Dim tabInd As Integer = item.tableIndex

            Dim formP As New FormPattern
            formP.DataGridView1.DataSource = DSPATTERNS.Tables(tabInd)
            formP.ShowDialog()
        End If
    End Sub

    Private Sub DeleteRows_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ' Delete rows button
        Dim response As MsgBoxResult = MsgBox("Do you want to proceed to delete?", vbOKCancel, "Confirm")
        Dim item As ListBoxItem = ListBox1.SelectedItem
        Dim ind As Integer = ListBox1.SelectedIndex

        If response = vbOK AndAlso ind <> -1 AndAlso item.enabled Then
            Dim filteredRows As DataRow() = DTMAIN.Select("Delete" & ind + 1 & " = 0")

            If filteredRows.Count > 0 Then
                DTMAIN = New Data.DataTable
                DTMAIN = filteredRows.CopyToDataTable()
                DTMAIN.AcceptChanges()
                item.enabled = False
                TextBox1.Text = DTMAIN.Rows.Count
            Else
                MsgBox("All rows will be deleted, process aborted", vbOK, "Warning")
            End If
        ElseIf Not item.enabled Then
            MsgBox("This item has been deleted already!")
        End If
    End Sub


    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        ' Change datagridview
        Dim ind As Integer = ListBox1.SelectedIndex
        If ind <> -1 Then
            Dim tabInd As Integer = ListBox1.SelectedItem.tableIndex
            Dim filteredRows As IEnumerable(Of Data.DataRow) = DTMAIN.AsEnumerable().Where(Function(row) row("Delete" & tabInd + 1) = 1)

            If filteredRows.Count > 0 Then
                Dim filteredDataTable As Data.DataTable = filteredRows.CopyToDataTable()
                showTable(DataGridView1, filteredDataTable)
            Else
                DataGridView1.DataSource = Nothing
            End If

            Button3.Enabled = True
            Button4.Enabled = True
        End If
    End Sub

    Private Sub Export_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ' Export as CSV to user's selected destination
        ExportCSV(DTMAIN, TEMPFILEPATH)
        saveDirectory(TEMPFILEPATH)
    End Sub

    Private Sub PatternA_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ListBox1.DisplayMember = "text"
    End Sub

    Private Sub PatternA_Close(sender As Object, e As EventArgs) Handles MyBase.Closed

        showTable(Form1.DataGridView3, DTMAIN)
        Form1.TextBox3.Text = DTMAIN.Rows.Count
    End Sub
End Class