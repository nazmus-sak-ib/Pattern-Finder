Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Microsoft.Office.Interop

Module Module1
    Public FOLDERPATH As String = Path.Combine(System.IO.Directory.GetCurrentDirectory, "temp")
    Public TEMPFILEPATH As String = Path.Combine(FOLDERPATH, "temp.csv")    ' Dedicated for pattern finding
    Public TEMPFILEPATH2 As String = Path.Combine(FOLDERPATH, "temp2.csv")  ' Dedicated for digit matching
    Public TIME As String = DateTime.Now.ToString("MMddyyyy_HHmmss")

    Public TEMPMP As String = Path.Combine(FOLDERPATH, "main")        ' Temporary main file path (For Design A)
    Public TEMPBP As String = Path.Combine(FOLDERPATH, "black")       ' Temporary black file path (For Design A)

    Public PATTERN As String                        ' Concatted pattern
    Public DSPATTERNS As New System.Data.DataSet     ' Contains the unique substring chunks 
    Public DTMAIN As New System.Data.DataTable        ' Main Datatabl
    Public DTBLACK As New System.Data.DataTable       ' Black Datasete
    Public DSFILTERMATCH As New System.Data.DataSet    ' Filtered Dataset
    Public DTMATCHED As New System.Data.DataTable     ' Matched Dataset (Second Part of the project)

    Public Sub ExportCSV(ByRef datatable As Data.DataTable, ByVal path As String)
        ' Saves a CSV file
        '
        Dim csvText As String = String.Join(Environment.NewLine, datatable.Rows.Cast(Of DataRow)().[Select](Function(x) x.Field(Of String)("ID")))

        'Try
        '    IO.File.Delete(path)
        'Catch ex As Exception

        'End Try

        Try
            File.WriteAllText(path, csvText)
        Catch ex As Exception

        End Try

    End Sub
    Public Function FileBrowseAndShow(ByRef listBox As ListBox) As String
        ' Browses for a file and adds it to a listbox
        ' @param listBox {ListBox} Listbox to add to
        ' @returns {String} Filepath

        Dim openFileDialog As New OpenFileDialog() With {
            .Filter = "Excel files (*.xls)|*.xl*",
            .FilterIndex = 1,
            .Multiselect = False
            }

        FileBrowseAndShow = ""

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            ' Get the selected file path
            Dim filePath As String = openFileDialog.FileName

            ' Do something with the file...
            If listBox.Items.Count = 1 Then listBox.Items.RemoveAt(0)
            listBox.Items.Add(filePath)

            FileBrowseAndShow = filePath
        End If

    End Function
    Public Function saveDirectory(ByVal filePath As String) As Boolean
        ' Browses for a file and moves the temporary file to that location selected by the user
        ' @param filePath Temporary file path
        '

        Dim saveFileDialog As New SaveFileDialog() With {
            .FileName = "output",
            .DefaultExt = ".csv",
            .Filter = "CSV Files (*.csv)|*.csv",
            .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            }


        ' Show the dialog box and get the result
        Dim result As DialogResult = saveFileDialog.ShowDialog()

        If result = DialogResult.OK Then
            ' Get the selected file path
            Dim filePathDest As String = saveFileDialog.FileName

            ' Get the selected folder path
            Dim folderPath As String = Path.GetDirectoryName(filePathDest)

            ' Get the selected file name without extension
            Dim fileName As String = Path.GetFileNameWithoutExtension(filePathDest)

            ' Save the file to the selected location with the specified name
            Try
                File.Copy(filePath, filePathDest, True)
            Catch ex As Exception

            End Try


        End If

        Return True
    End Function

    Public Function loadMainDS(ByVal path As String) As Long
        ' Loads main dataset, stores it, displays it.
        ' @param path File path
        ' @returns Datatable rows count
        '
        Dim size As Long = 0                    ' Table rows count
        Try
            Dim csvData As String = File.ReadAllText(path)

            Dim mainTable As New Data.DataTable
        mainTable.Columns.Add("ID")

        Using csvParser As New Microsoft.VisualBasic.FileIO.TextFieldParser(New StringReader(csvData))
            csvParser.Delimiters = New String() {","}
            csvParser.HasFieldsEnclosedInQuotes = False

            ' Looping through all other rows
            Dim rowFields As String
            While Not csvParser.EndOfData

                Dim newRow As DataRow = mainTable.NewRow()
                rowFields = csvParser.ReadLine()

                newRow.ItemArray = {rowFields}
                mainTable.Rows.Add(newRow)
                size += 1

            End While

            mainTable.AcceptChanges()
        End Using


        DTMAIN = mainTable
        loadMainDS = size
        Catch
        MsgBox("Error loading the file!", vbOK, "Warning")
        loadMainDS = 0
        End Try

    End Function

    Public Function loadBlackDS(ByVal blackPath As String) As Long
        ' Loads blacklisted dataset, stores it, displays it.
        ' @param path File path
        ' @returns Datatable rows count
        '
        Dim size As Long = 0                    ' Table rows count
        Try
            Dim csvData As String = File.ReadAllText(blackPath)

            Dim blackTable As New Data.DataTable
            blackTable.Columns.Add("ID")

            Using csvParser As New Microsoft.VisualBasic.FileIO.TextFieldParser(New StringReader(csvData))
                csvParser.Delimiters = New String() {","}
                csvParser.HasFieldsEnclosedInQuotes = False

                ' Looping through all other rows
                Dim rowFields As String
                While Not csvParser.EndOfData

                    Dim newRow As DataRow = blackTable.NewRow()
                    rowFields = csvParser.ReadLine()

                    newRow.ItemArray = {rowFields}
                    blackTable.Rows.Add(newRow)
                    size += 1

                End While

                blackTable.AcceptChanges()
            End Using

            DTBLACK = blackTable
            loadBlackDS = size
        Catch
            MsgBox("Error loading the file!", vbOK, "Warning")
            loadBlackDS = 0
        End Try

    End Function

    Public Function generatePattern(ByVal blackPath As String, ByVal chunk As Byte)
        ' Loads a blacklist file, generates a regex pattern for regex match, adds to DSPATTERNS dataset
        ' @param blackPath Path of the blacklist file
        ' @param chunk Substring chunk size
        '
        Try
            Dim blackNumber As String                       ' An individual blacklisted number
            Dim table As New System.Data.DataTable
            table.Columns.Add("ID")


            For Each row As DataRow In DTBLACK.Rows
                ' Access the values of each column in the current row
                blackNumber = row("ID")

                If blackNumber.Length < chunk Then Continue For

                For i = 0 To blackNumber.Length - chunk         ' Taking substrings out of a blackNumber
                    Dim temp As String = blackNumber.Substring(i, chunk)

                    Dim newRow As DataRow = table.NewRow()
                    newRow("ID") = temp
                    table.Rows.Add(newRow)

                Next
            Next

            ' Possibly remove here
            Dim uniqueValues() As Object = table.AsEnumerable().Select(Function(row) row("ID")).Distinct().Where(Function(value) Not String.IsNullOrWhiteSpace(value)).ToArray()

            table = New System.Data.DataTable

            Dim newColumn As New DataColumn("ID", GetType(String))
            table.Columns.Add("ID")
            table.PrimaryKey = New DataColumn() {table.Columns("ID")}

            For Each value As String In uniqueValues
                Dim newRow As DataRow = table.NewRow()
                newRow("ID") = value
                table.Rows.Add(newRow)
            Next
            ' Possibly remove here ends

            PATTERN = String.Join("|", table.AsEnumerable().Select(Function(row) row.Field(Of String)("ID")))

            DSPATTERNS.Tables.Add(table)
            DSPATTERNS.AcceptChanges()
            Return True
        Catch
            MsgBox("Error loading the file!", vbOK, "Warning")
            Return False
        End Try

    End Function

    Public Sub matchDataset(ByVal digit As String)
        ' Takes a digit and simply regex matches it and lists out the matched rows
        ' @param blackPath Path of the blacklist file
        ' @param chunk Substring chunk size
        '

        Dim table As System.Data.DataTable = DTMAIN
        Dim table2 As System.Data.DataTable = table.Clone
        Dim newColumn As DataColumn = New DataColumn("Index", GetType(Double))
        table2.Columns.Add(newColumn)


        Dim i As Double = 0
        For Each row As DataRow In table.Rows
            Dim val As String = row("ID").ToString()

            If Regex.IsMatch(val, digit) Then
                Dim newRow As DataRow = table2.NewRow()
                newRow("Index") = i
                newRow("ID") = row("ID")
                table2.Rows.Add(newRow)
            End If
            i += 1
        Next

        table2.AcceptChanges()

        DTMATCHED = table2

    End Sub

    Public Function splitMainFile(ByVal mainFilePath As String) As Boolean
        ' Splits the main excel into 2 separate CSVs
        ' For pattern finder Design A
        ' @param mainFilePath Path of the main file

        'Try
        ' Starting Excel
        Dim excelApp As New Microsoft.Office.Interop.Excel.Application()
            Dim workbook As Microsoft.Office.Interop.Excel.Workbook
            Dim worksheet As Microsoft.Office.Interop.Excel.Worksheet
            excelApp.DisplayAlerts = False

            ' Loading file
            workbook = excelApp.Workbooks.Open(mainFilePath)

            ' Loading Main WS
            worksheet = workbook.Sheets(1)

            '' Get the range of used cells in the worksheet
            Dim usedRange As Excel.Range = worksheet.UsedRange
            Dim rowCount As Integer = usedRange.Rows.Count
            Dim columnCount As Integer = usedRange.Columns.Count

            '' Processing

            If columnCount > 1 Then
                For i = 1 To columnCount - 1
                    worksheet.Columns(2).delete
                Next
            End If

            If Regex.IsMatch(worksheet.Cells(1, 1).value, "[a-zA-Z]") Then
                worksheet.Rows(1).delete
            End If


            '' Exporting
            'Try
            '    File.Delete(TEMPMP & ".csv")
            '    'My.Computer.FileSystem.DeleteFile(TEMPMP & ".csv")
            'Catch ex As Exception
            'End Try

            worksheet.SaveAs(TEMPMP, Excel.XlFileFormat.xlCSV)


        ' Loading Black WS
        workbook.Close()
        workbook = Nothing

        workbook = excelApp.Workbooks.Open(mainFilePath)
        worksheet = workbook.Sheets(2)

            '' Get the range of used cells in the worksheet
            usedRange = worksheet.UsedRange
            rowCount = usedRange.Rows.Count
            columnCount = usedRange.Columns.Count

            '' Processing
            If columnCount > 1 Then
                For i = 1 To columnCount - 1
                    worksheet.Columns(2).delete
                Next
            End If

            If Regex.IsMatch(worksheet.Cells(1, 1).value, "[a-zA-Z]") Then
                worksheet.Rows(1).delete
            End If

            '' Exporting
            'Try
            '    File.Delete(TEMPBP & ".csv")
            '    'My.Computer.FileSystem.DeleteFile(TEMPBP & ".csv")
            'Catch ex As Exception

            'End Try

            worksheet.SaveAs(TEMPBP, Excel.XlFileFormat.xlCSV)

            ' Wrapping up
            workbook.Close()
            excelApp.Quit()
            workbook = Nothing
            excelApp = Nothing

            Return True
        'Catch
        'MsgBox("Try closing the Excel file if it is open")
        'Return False
        'End Try

    End Function

    Public Sub showTable(ByRef dgv As DataGridView, ByRef table As Data.DataTable)
        ' Sets a datatable as the source for a datagridview

        If table.Rows.Count = 0 Then
            dgv.DataSource = Nothing
            Exit Sub
        End If

        dgv.DataSource = table

        For Each column As DataGridViewColumn In dgv.Columns
            ' Check if the column is the one you want to show
            If column.Name <> "ID" Then
                ' Hide the column
                column.Visible = False
            End If
        Next
    End Sub



End Module
