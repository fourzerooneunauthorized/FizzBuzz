using System;


namespace FizzBuzz.Config;


/// <summary>
/// A problem loading app configuration has occurred
/// </summary>
public class AppConfigValidationException : Exception
{

    public AppConfigValidationException( string? message )
        : base( message )
    {
    }

}
