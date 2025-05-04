using FizzBuzz.Config;
using NUnit.Framework;
using System.Collections.Generic;


namespace FizzBuzz.Tests;


public class CommandLineConfigProviderTests
{

    [TestCaseSource( nameof( GoodCommandLineArgsSource ) )]
    public void TestGetConfig( string[] args )
    {
        var configProvider = new CommandLineConfigProvider( args );
        Assert.DoesNotThrow( () => configProvider.GetConfig() );

        AppConfig config = configProvider.GetConfig();
        Assert.That( config, Is.Not.Null);
        Assert.That( config.Rounds, Is.GreaterThan( 0 ) );
        Assert.That( config.Devisors, Is.Not.Null);
        Assert.That( config.Devisors, Is.Not.Empty);
    }


    static IEnumerable<string[]> GoodCommandLineArgsSource()
    {
        yield return [ ];
        yield return [ "rounds=100" ];
        yield return [ "devisor=3=Fizz" ];
        yield return [ "rounds=100", "devisor=3=Fizz" ];
        yield return [ "rounds=100", "devisor=5=Buzz" ];
        yield return [ "rounds=100", "devisor=3=Fizz", "devisor=5=Buzz" ];
        yield return [ "rounds=1000", "devisor=3=Fizz", "devisor=567=Buzz Test 123" ];
        yield return [ "rounds=100", "devisor=3=Fizz", "devisor=5=69CEA7E7-5B19-4C09-807C-D971A05928D7" ];
        yield return [ "rounds=100", "devisor=3=Fizz", "devisor=5=Buzz=Zzub" ];
        yield return [ "rounds=100", "devisor=3=Fizz", "devisor=5=" ];
    }


    [TestCaseSource( nameof( BadCommandLineArgsSource ) )]
    public void TestRejectedConfig( string[] args )
    {
        var configProvider = new Config.CommandLineConfigProvider( args );
        Assert.Throws<AppConfigLoadingException>( () => configProvider.GetConfig() );
    }


    static IEnumerable<string[]> BadCommandLineArgsSource()
    {
        yield return [ "rounds=0", "devisor=3=Fizz", "notasupportedarg=1" ]; // unknown arg given
        yield return [ "rounds=0", "devisor=3=Fizz" ]; // rounds < 1
        yield return [ "rounds=not a number", "devisor=3=Fizz" ]; // rounds is not a number
        yield return [ "rounds=100", "rounds=1", "devisor=3=Fizz" ]; // duplicate rounds arg
        yield return [ "devisor=not a number=Fizz" ]; // devisor arg key not a number
        yield return [ "devisor=0=Fizz" ]; // devisor arg key < 1
    }

}
