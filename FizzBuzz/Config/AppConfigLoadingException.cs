using System;


namespace FizzBuzz.Config;


/// <summary>
/// A problem loading app configuration has occurred
/// </summary>
public class AppConfigLoadingException : Exception
{

    public AppConfigLoadingException( string? message )
        : base( message )
    {
    }


    public AppConfigLoadingException( string? message, Exception? innerException )
        : base( message, innerException )
    {
    }

}
