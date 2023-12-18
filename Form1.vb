Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar
Imports Microsoft.Office.Interop.Excel
Imports Listbox = System.Windows.Forms.ListBox

Public Class Form1
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Dim response = MsgBox("Do you want to exit the program?", vbOKCancel, "Exit")
        If response = vbOK Then Me.Close()
    End Sub

    Private Sub HelpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpToolStripMenuItem.Click
        FormHelp.Show()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Path management
        Try
            Directory.CreateDirectory(FOLDERPATH)
        Catch ex As Exception

        End Try


    End Sub

    Private Sub ExportMatch_Click(sender As Object, e As EventArgs) Handles Button7.Click
        ' Export Matched Digits
        ExportCSV(DTMATCHED, TEMPFILEPATH2)
        saveDirectory(TEMPFILEPATH2)
    End Sub

    Private Sub ProceedMatch_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ' Proceed button for Match tab
        Button7.Enabled = False
        TextBox6.Text = ""

        Dim digitText As String = TextBox5.Text
        Dim digit As Byte
        Byte.TryParse(digitText, digit)

        If digit < 0 Then
            MsgBox("Digit should be any integer starting from 0", vbOK, "Warning")
            Exit Sub
        ElseIf ListBox1.Items.Count = 0 Then
            MsgBox("Select a source file first!", vbOK, "Warning")
            TabPage1.Enabled = True
            Exit Sub
        End If

        matchDataset(digit)

        Button7.Enabled = True
        TextBox6.Text = DTMATCHED.Rows.Count

        showTable(DataGridView4, DTMATCHED)


    End Sub

    Private Sub TabPage4_Click(sender As Object, e As EventArgs) Handles TabPage3.Click, TabPage4.Click
        ActiveControl = Nothing
    End Sub

    Private Sub Browse_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' Browse File
        Dim success As Boolean = False          ' Success of splitting the main file
        Dim size As Long                        ' main/black dataset row count

        Dim path As String = FileBrowseAndShow(ListBox1)    ' main Excel path

        If path <> "" Then
            success = splitMainFile(path)
        End If


        If success Then
            ' Main dataset loading
            size = loadMainDS(TEMPMP & ".csv")

            showTable(DataGridView1, DTMAIN)

            TextBox1.Text = size

            showTable(DataGridView3, DTMAIN)

            TextBox3.Text = DTMAIN.Rows.Count

            ' Black dataset loading
            size = loadBlackDS(TEMPBP & ".csv")
            DataGridView2.DataSource = DTBLACK
            TextBox2.Text = size
        End If


    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Go button for a pattern finder mode selection
        Dim checkedIndex As Integer = CheckedListBox2.CheckedIndices.Item(0)

        If checkedIndex = 0 Then
            ' Design A
            Dim form As New PatternA
            form.TextBox1.Text = DTMAIN.Rows.Count
            form.ShowDialog()
        ElseIf checkedIndex = 1 Then
            ' Design C
            Dim form As New PatternC
            form.TextBox1.Text = DTMAIN.Rows.Count
            form.ShowDialog()
        End If

    End Sub

End Class
