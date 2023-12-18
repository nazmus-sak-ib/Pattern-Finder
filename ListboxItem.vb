Public Class ListBoxItem
    ' When a search is performed with a substring size, an item is added on a listbox
    Public Property text As String    ' Text to display on Listbox
    Public Property size As Double     ' Substring size
    Public Property matchCount As Double  ' How many matches are found in the main dataset
    Public Property tableIndex As Integer  ' Coresponding table row index in DTFILTERMATCH
    Public Property enabled As Boolean = True ' If this item has already been deleted or not (True)

    Public Sub setText()
        ' Sets display text
        text = "Substring size (" & size & ") - Match found (" & matchCount & ")"
    End Sub

End Class
