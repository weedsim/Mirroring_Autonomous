using System;

public class InGameException : ApplicationException
{
    public InGameException(string msg) : base(msg){}

    public InGameException() : base("Internal Game Error") {}
}
