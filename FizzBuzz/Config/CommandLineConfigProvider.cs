using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace FizzBuzz.Config;


public class CommandLineConfigProvider : IConfigProvider
{

    // rounds data
    private const string _roundsArgName = "rounds";
    private static readonly Regex _roundsArgRegex = new( $"^{_roundsArgName}=(?<{_roundsArgName}>[0-9]+)$" );
    private const int _roundsDefault = 100;

    // divisor data
    private const string _divisorsArgName = "divisor";
    private static readonly Regex _divisorArgRegex = new( $"^{_divisorsArgName}=((?<key>[0-9]+)=(?<text>.*))$" );

    private static readonly Dictionary<int, string> _divisorsDefault = new()
    {
        { 3, "Fizz" },
        { 5, "Buzz" }
    };

    private static readonly List<ConfigItemMeta> _configItemDescriptions =
    [
        new()
        {
            Name = _roundsArgName,
            IsRequired = false,
            Description = $"Number of rounds to play. Default: {_roundsDefault}"
        },
        new()
        {
            Name = _divisorsArgName,
            IsRequired = false,
            Description =
                "Multiple 'divisor' arguments specifying an integer divisor and the associated text to output when the round's integer is evenly devisable by this number. Specify in the form: \"divisor={whole number}={text} ... divisor={whole number}={text}\". E.g. \"divisor=3=Fizz divisor=5=Buzz\". Text segment can be enclosed in double-quotes to accomodate whitespace"
        }
    ];

    private readonly string[] _commandLineArgs;


    /// <summary>
    /// Parses command line arguments to obtain application configuration
    /// </summary>
    /// <param name="commandLineArgs">Command line argument array</param>
    public CommandLineConfigProvider( string[] commandLineArgs )
    {
        _commandLineArgs = commandLineArgs ?? [ ];
    }


    /// <inheritdoc />
    public IList<ConfigItemMeta> GetConfigItemDescriptions()
    {
        return _configItemDescriptions;
    }


    /// <summary>
    /// Obtains application configuration
    /// </summary>
    /// <exception cref="AppConfigLoadingException">Loading configuration failed</exception>
    public AppConfig GetConfig()
    {
        // test for unknown args passed in
        if ( _commandLineArgs.Any( a => !a.StartsWith( _divisorsArgName + "=" ) &&
                                        !a.StartsWith( _roundsArgName + "=" ) ) )
            throw new AppConfigLoadingException( "Unknown arguments given" );

        return new AppConfig
        {
            Rounds = ParseRoundCountArgument() ?? _roundsDefault,
            Divisors = ParseDivisorArguments() ?? _divisorsDefault
        };
    }


    /// <summary>
    /// Parses any round parameter found
    /// </summary>
    private int? ParseRoundCountArgument()
    {
        string[] roundsArgs = _commandLineArgs.Where( a => a.StartsWith( _roundsArgName + "=" ) ).ToArray();

        if ( !roundsArgs.Any() )
            return null;

        if ( roundsArgs.Length > 1 )
            throw new AppConfigLoadingException( $"Duplicate {_roundsArgName} argument" );

        Match match = _roundsArgRegex.Match( roundsArgs[ 0 ] );

        if ( !match.Success || !match.Groups.TryGetValue( _roundsArgName, out Group? roundsGroup ) )
            throw new AppConfigLoadingException( $"Argument {_roundsArgName} is malformed" );

        if ( !int.TryParse( roundsGroup.Value, out int rounds ) )
            throw new AppConfigLoadingException( $"Argument {_roundsArgName} is not a number" );

        if ( rounds < 1 )
            throw new AppConfigLoadingException( $"{_roundsArgName} argument must be greater than 0" );

        return rounds;
    }


    /// <summary>
    /// Parses any divisor arguments found
    /// </summary>
    private Dictionary<int, string>? ParseDivisorArguments()
    {
        string[] divisorArgs = _commandLineArgs.Where( a => a.StartsWith( _divisorsArgName + "=" ) ).ToArray();

        if ( !divisorArgs.Any() )
            return null;

        Dictionary<int, string> returnDict = new();

        foreach ( string arg in divisorArgs )
        {
            Match match = _divisorArgRegex.Match( arg );

            if ( !match.Success )
                throw new AppConfigLoadingException( $"Argument {_divisorsArgName} is malformed" );

            if ( !match.Groups.TryGetValue( "key", out Group? keyGroup ) )
                throw new AppConfigLoadingException( $"Malformed {_divisorsArgName} number argument" );

            if ( !int.TryParse( keyGroup.Value, out int key ) )
                throw new AppConfigLoadingException( $"Key argument for {_divisorsArgName} is not a number" );

            if ( key < 1 )
                throw new AppConfigLoadingException( $"{_divisorsArgName} key argument must be greater than 0" );

            if ( !match.Groups.TryGetValue( "text", out Group? termGroup ) )
                throw new AppConfigLoadingException( $"Malformed {_divisorsArgName} text argument" );

            returnDict.Add( key, termGroup.Value );
        }

        return returnDict;
    }

}
