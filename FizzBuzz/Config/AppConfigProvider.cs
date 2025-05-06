using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;


namespace FizzBuzz.Config;


public class AppConfigProvider : IAppConfigProvider
{

    // rounds data
    private const string _roundsArgName = "rounds";
    private const int _roundsDefault = 100;

    // divisor data
    private const string _divisorsArgName = "divisors";

    private static readonly Dictionary<int, string> _divisorsDefault = new()
    {
        { 3, "Fizz" },
        { 5, "Buzz" }
    };

    // matchByDigit data
    private const string _matchByContainsArgName = "matchByContains";
    private const bool _matchByContainsDefault = false;

    private static readonly List<ConfigItemMeta> _configItemDescriptions =
    [
        new()
        {
            Name = _roundsArgName,
            IsRequired = false,
            Usage = $"{_roundsArgName}=<number>",
            Description = $"Number of rounds to play. Default: {_roundsDefault}"
        },
        new()
        {
            Name = _divisorsArgName,
            IsRequired = false,
            Usage = $"{_divisorsArgName}:<number>=<text> [..{_divisorsArgName}:<number>=<text>]",
            Description =
                $"One or more 'divisors' arguments specifying an integer divisor and the associated text to output when the round's integer is evenly divisible by this number. Text segment can be enclosed in double-quotes to accomodate whitespace. Default: {string.Join( " ", _divisorsDefault.Select( d => $"{d.Key}={d.Value}" ) )}"
        },
        new()
        {
            Name = _matchByContainsArgName,
            IsRequired = false,
            Usage = $"{_matchByContainsArgName}=<true|false>",
            Description =
                $"Also trigger a replacement during the round when the round number contains any of the divisor numbers. I.e. \"31\" triggers if there is a divisor \"3\" set. Default {_matchByContainsDefault}"
        },
        new()
        {
            Name = "help",
            IsRequired = false,
            Usage = "help",
            Description = "Shows help message"
        }
    ];

    private readonly AppConfig _appConfig;


    /// <summary>
    /// Parses command line arguments to obtain application configuration
    /// </summary>
    /// <param name="configRoot">Config root</param>
    /// <exception cref="AppConfigValidationException">Configuration invalid</exception>
    public AppConfigProvider( IConfigurationRoot configRoot )
    {
        // reject any non-defined config items passed in
        if ( configRoot.GetChildren()
                       .Any( c =>
                                 !_configItemDescriptions.Exists( meta => meta.Name == c.Key ) && ( c.Path != ( _divisorsArgName + ':' ) )
                       ) )
            throw new AppConfigValidationException( "Unknown argument" );

        // note: not using config binding because we need to fully validate config
        _appConfig = new AppConfig
        {
            Rounds = ParseRoundsConfig( configRoot ) ?? _roundsDefault,
            Divisors = ParseDivisorConfig( configRoot ) ?? _divisorsDefault,
            MatchByContains = ParseMatchByContainsConfig( configRoot ) ?? _matchByContainsDefault
        };
    }


    /// <inheritdoc />
    public IList<ConfigItemMeta> GetConfigItemDescriptions()
    {
        return _configItemDescriptions;
    }


    /// <inheritdoc />
    public AppConfig GetConfig()
    {
        return _appConfig;
    }


    /// <summary>
    /// Parses and validates any round parameter found
    /// </summary>
    /// <param name="configRoot"></param>
    /// <exception cref="AppConfigValidationException">Configuration invalid</exception>
    private static int? ParseRoundsConfig( IConfigurationRoot configRoot )
    {
        IConfigurationSection configSection = configRoot.GetSection( _roundsArgName );

        if ( !configSection.Exists() )
            return null;

        if ( !int.TryParse( configSection.Value, out int rounds ) )
            throw new AppConfigValidationException( $"Argument {_roundsArgName} is not a number" );

        if ( rounds < 1 )
            throw new AppConfigValidationException( $"{_roundsArgName} argument must be greater than 0" );

        return rounds;
    }


    /// <summary>
    /// Parses and validates any divisor arguments found
    /// </summary>
    /// <param name="configRoot"></param>
    /// <exception cref="AppConfigValidationException">Configuration invalid</exception>
    private static Dictionary<int, string>? ParseDivisorConfig( IConfigurationRoot configRoot )
    {
        IConfigurationSection configSection = configRoot.GetSection( _divisorsArgName );

        if ( !configSection.Exists() )
            return null;

        if ( !configSection.GetChildren().Any() )
            throw new AppConfigValidationException( $"{_divisorsArgName} argument malformed" );

        List<IConfigurationSection> configItems = configSection.GetChildren().ToList();

        if ( !configItems.Any() )
            return null;

        Dictionary<int, string> returnDict = new();

        foreach ( IConfigurationSection divisorConfig in configItems )
        {
            if ( !int.TryParse( divisorConfig.Key, out int divisor ) )
                throw new AppConfigValidationException( $"{_divisorsArgName} argument index is not a invalid integer" );

            if ( divisor < 1 )
                throw new AppConfigValidationException( $"{_divisorsArgName} must be greater than 0" );

            if ( string.IsNullOrWhiteSpace( divisorConfig.Value ) )
                throw new AppConfigValidationException( $"{_divisorsArgName} argument replacement text missing" );

            returnDict.Add( divisor, divisorConfig.Value );
        }

        return returnDict;
    }


    /// <summary>
    /// Parses any matchByContains config item if found
    /// </summary>
    /// <param name="configRoot"></param>
    /// <exception cref="AppConfigValidationException">Configuration invalid</exception>
    private static bool? ParseMatchByContainsConfig( IConfigurationRoot configRoot )
    {
        IConfigurationSection configSection = configRoot.GetSection( _matchByContainsArgName );

        if ( !configSection.Exists() )
            return null;

        if ( !bool.TryParse( configSection.Value, out bool val ) )
            throw new AppConfigValidationException( $"Argument {_matchByContainsArgName} is not a boolean value" );

        return val;
    }

}
