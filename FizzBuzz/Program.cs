using FizzBuzz.Config;
using System;
using System.Collections.Generic;
using System.Linq;


namespace FizzBuzz;


class Program
{

    static void Main( string[] args )
    {
        IConfigProvider configProvider = new CommandLineConfigProvider( args );

        if ( ( args.Length == 1 ) && ( args.Single() == "help" ) )
        {
            var itemDesc = configProvider.GetConfigItemDescriptions();

            Console.WriteLine( "Solves the FizzBuzz game: https://en.wikipedia.org/wiki/Fizz_buzz" );

            Console.WriteLine(
                $"Usage: FizzBuzz.exe {string.Join( " ", itemDesc.Select( i => i.Name + "={value}" ) )}" );

            Console.WriteLine( "Parameters -" );

            foreach ( var item in itemDesc )
                Console.WriteLine( $"{item.Name}: ({( item.IsRequired ? "required" : "optional" )}) {item.Description}" );

            return;
        }

        AppConfig config = configProvider.GetConfig();

        var roundsOutput = new List<string>();

        for ( int round = 1; round < config.Rounds; round++ )
        {
            var devisorMatchedTerms = config.Devisors
                                            .Where( d => ( round % d.Key ) == 0 )
                                            .OrderBy( d => d.Key )
                                            .Select( d => d.Value )
                                            .ToList();

            roundsOutput.Add( devisorMatchedTerms.Any()
                                  ? string.Join( " ", devisorMatchedTerms )
                                  : round.ToString() );
        }

        Console.WriteLine( string.Join( ", ", roundsOutput ) );
    }

}
