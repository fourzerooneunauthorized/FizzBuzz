using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace FizzBuzz.Config;


public class CommandLineConfigProvider : IConfigProvider
{

    const string _roundsArgName = "rounds";
    private static readonly Regex _roundsArgRegex = new( $"^{_roundsArgName}=(?<{_roundsArgName}>[0-9]+)$" );
    const int _roundsDefault = 100;

    const string _devisorsArgName = "devisor";
    private static readonly Regex _devisorArgRegex = new( $"^{_devisorsArgName}=((?<key>[0-9]+)=(?<text>.*))$" );

    private static readonly Dictionary<int, string> _devisorsDefault = new()
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
            Name = _devisorsArgName,
            IsRequired = false,
            Description =
                "Multiple 'devisor' arguments specifying an integer devisor and the associated text to output when the round's integer is evenly devisable by this number. Specify in the form: \"devisor={whole number}={text} ... devisor={whole number}={text}\". E.g. \"devisor=3=Fizz devisor=5=Buzz\". Text segment can be enclosed in double-quotes to accomodate whitespace"
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
        if ( _commandLineArgs.Any( a => !a.StartsWith( _devisorsArgName + "=" ) &&
                                        !a.StartsWith( _roundsArgName + "=" ) ) )
            throw new AppConfigLoadingException( "Unknown arguments given" );

        return new AppConfig()
        {
            Rounds = ParseRoundCountArgument( _commandLineArgs ) ?? _roundsDefault,
            Devisors = ParseDevisorArguments( _commandLineArgs ) ?? _devisorsDefault
        };
    }


    private int? ParseRoundCountArgument( string[] args )
    {
        string[] roundsArgs = args.Where( a => a.StartsWith( _roundsArgName + "=" ) ).ToArray();

        if ( !roundsArgs.Any() )
            return null;

        if ( roundsArgs.Length > 1 )
            throw new AppConfigLoadingException( $"Duplicate {_roundsArgName} argument" );

        Match match = _roundsArgRegex.Match( roundsArgs[ 0 ] );

        if ( !match.Success || !match.Groups.TryGetValue( _roundsArgName, out var roundsGroup ) )
            throw new AppConfigLoadingException( $"Argument {_roundsArgName} is malformed" );

        if ( !int.TryParse( roundsGroup.Value, out var rounds ) )
            throw new AppConfigLoadingException( $"Argument {_roundsArgName} is not a number" );

        if ( rounds < 1 )
            throw new AppConfigLoadingException( $"{_roundsArgName} argument must be greater than 0" );

        return rounds;
    }


    private Dictionary<int, string>? ParseDevisorArguments( string[] args )
    {
        string[] devisorArgs = args.Where( a => a.StartsWith( _devisorsArgName + "=" ) ).ToArray();

        if ( !devisorArgs.Any() )
            return null;

        Dictionary<int, string> returnDict = new();

        foreach ( string arg in devisorArgs )
        {
            Match match = _devisorArgRegex.Match( arg );

            if ( !match.Success )
                throw new AppConfigLoadingException( $"Argument {_devisorsArgName} is malformed" );

            if ( !match.Groups.TryGetValue( "key", out var keyGroup ) )
                throw new AppConfigLoadingException( $"Malformed {_devisorsArgName} number argument" );

            if ( !int.TryParse( keyGroup.Value, out var key ) )
                throw new AppConfigLoadingException( $"Key argument for {_devisorsArgName} is not a number" );

            if ( key < 1 )
                throw new AppConfigLoadingException( $"{_devisorsArgName} key argument must be greater than 0" );

            if ( !match.Groups.TryGetValue( "text", out var termGroup ) )
                throw new AppConfigLoadingException( $"Malformed {_devisorsArgName} text argument" );

            returnDict.Add( key, termGroup.Value );
        }

        return returnDict;
    }

}
