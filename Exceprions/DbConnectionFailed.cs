using System.Collections;

namespace ContactList.Exceptions;

public class DbConnectionFailed : Exception
{
    public DbConnectionFailed(string message) : base(message) { }
    
}