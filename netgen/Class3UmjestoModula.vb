Public Class Class3UmjestoModula

    <ThreadStatic> Private Shared _operateriId As Integer


    Public Shared Property OperateriId As Integer
        Get
            Return _operateriId
        End Get
        Set(ByVal value As Integer)
            _operateriId = value
        End Set
    End Property

End Class
