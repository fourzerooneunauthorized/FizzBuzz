using FizzBuzz.Config;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;


namespace FizzBuzz;


internal class Program
{

    private static void Main( string[] args )
    {
        IAppConfigProvider configProvider;

        try
        {
            configProvider = new AppConfigProvider( new ConfigurationBuilder()
                                                    .AddCommandLine( args )
                                                    .Build() );
        }
        catch ( Exception ex )
        {
            Console.Error.WriteLine( ex.Message );
            Environment.Exit( 1 );

            throw;
        }

        AppConfig config = configProvider.GetConfig();

        // short-circuit if requesting app help
        if ( ( args.Length == 1 ) && ( args.Single() == "help" ) )
        {
            IList<ConfigItemMeta> itemDesc = configProvider.GetConfigItemDescriptions();

            Console.WriteLine( "Solves the FizzBuzz game: https://en.wikipedia.org/wiki/Fizz_buzz" );

            Console.WriteLine(
                $"Usage: FizzBuzz.exe {string.Join( " ", itemDesc.Select( i => i.Name + "={value}" ) )}" );

            Console.WriteLine( "Parameters -" );

            foreach ( ConfigItemMeta item in itemDesc )
                Console.WriteLine( $"{item.Name}: ({( item.IsRequired ? "required" : "optional" )}) {item.Description}" );

            return;
        }

        // check number representing each round if any divisors can evenly divide into it, and build a full game result set
        List<string> roundsOutput = [ ];

        for ( var round = 1; round <= config.Rounds; round++ )
        {
            // note when multiple divisor hits for round, that text is outputted in order of lowest to highest divisor
            string[] divisorMatchedTerms = config.Divisors
                                                 .Where( d => ( ( round % d.Key ) == 0 ) ||
                                                              ( config.MatchByContains && round.ToString().Contains( d.Key.ToString() ) ) )
                                                 .OrderBy( d => d.Key )
                                                 .Select( d => d.Value )
                                                 .ToArray();

            // if no divisor matches then just output the number unmodified
            roundsOutput.Add( divisorMatchedTerms.Any()
                                  ? string.Join( " ", divisorMatchedTerms )
                                  : round.ToString() );
        }

        Console.WriteLine( string.Join( ", ", roundsOutput ) );
    }

}
