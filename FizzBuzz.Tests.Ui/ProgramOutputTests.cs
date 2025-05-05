using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace FizzBuzz.Tests.Ui;


public class ProgramOutputTests
{

    [TestCaseSource( nameof( GoodCommandTests ) )]
    public void TestDefaultValues( (string[] Args, string ExpectedOutput) testProps )
    {
        using Process process = new();

        process.StartInfo = new ProcessStartInfo( "FizzBuzz.exe", testProps.Args )
        {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        process.Start();

        process.WaitForExit( TimeSpan.FromSeconds( 5 ) );

        Assert.That( process.ExitCode, Is.EqualTo( 0 ) );

        string error = process.StandardError.ReadToEnd();
        Assert.That( error, Is.Empty );

        string output = process.StandardOutput.ReadToEnd();
        Assert.That( output, Is.Not.Empty );
        Assert.That( output, Is.EqualTo( testProps.ExpectedOutput ) );
    }


    private static IEnumerable<(string[] Args, string ExpectedOutput)> GoodCommandTests()
    {
        // test default output
        yield return ( [ ],
                       "1, 2, Fizz, 4, Buzz, Fizz, 7, 8, Fizz, Buzz, 11, Fizz, 13, 14, Fizz Buzz, 16, 17, Fizz, 19, Buzz, Fizz, 22, 23, Fizz, Buzz, 26, Fizz, 28, 29, Fizz Buzz, 31, 32, Fizz, 34, Buzz, Fizz, 37, 38, Fizz, Buzz, 41, Fizz, 43, 44, Fizz Buzz, 46, 47, Fizz, 49, Buzz, Fizz, 52, 53, Fizz, Buzz, 56, Fizz, 58, 59, Fizz Buzz, 61, 62, Fizz, 64, Buzz, Fizz, 67, 68, Fizz, Buzz, 71, Fizz, 73, 74, Fizz Buzz, 76, 77, Fizz, 79, Buzz, Fizz, 82, 83, Fizz, Buzz, 86, Fizz, 88, 89, Fizz Buzz, 91, 92, Fizz, 94, Buzz, Fizz, 97, 98, Fizz, Buzz" +
                       Environment.NewLine );

        // test custom rounds with default divisors
        yield return ( [ "rounds=15" ], "1, 2, Fizz, 4, Buzz, Fizz, 7, 8, Fizz, Buzz, 11, Fizz, 13, 14, Fizz Buzz" + Environment.NewLine );

        // test single custom divisors with default rounds
        yield return ( [ "divisors:4=Test4" ],
                       "1, 2, 3, Test4, 5, 6, 7, Test4, 9, 10, 11, Test4, 13, 14, 15, Test4, 17, 18, 19, Test4, 21, 22, 23, Test4, 25, 26, 27, Test4, 29, 30, 31, Test4, 33, 34, 35, Test4, 37, 38, 39, Test4, 41, 42, 43, Test4, 45, 46, 47, Test4, 49, 50, 51, Test4, 53, 54, 55, Test4, 57, 58, 59, Test4, 61, 62, 63, Test4, 65, 66, 67, Test4, 69, 70, 71, Test4, 73, 74, 75, Test4, 77, 78, 79, Test4, 81, 82, 83, Test4, 85, 86, 87, Test4, 89, 90, 91, Test4, 93, 94, 95, Test4, 97, 98, 99, Test4" +
                       Environment.NewLine );

        // test multiple custom default divisors with default rounds
        yield return ( [ "divisors:4=Test4", "divisors:10=Test10", "divisors:19=Test19" ],
                       "1, 2, 3, Test4, 5, 6, 7, Test4, 9, Test10, 11, Test4, 13, 14, 15, Test4, 17, 18, Test19, Test4 Test10, 21, 22, 23, Test4, 25, 26, 27, Test4, 29, Test10, 31, Test4, 33, 34, 35, Test4, 37, Test19, 39, Test4 Test10, 41, 42, 43, Test4, 45, 46, 47, Test4, 49, Test10, 51, Test4, 53, 54, 55, Test4, Test19, 58, 59, Test4 Test10, 61, 62, 63, Test4, 65, 66, 67, Test4, 69, Test10, 71, Test4, 73, 74, 75, Test4 Test19, 77, 78, 79, Test4 Test10, 81, 82, 83, Test4, 85, 86, 87, Test4, 89, Test10, 91, Test4, 93, 94, Test19, Test4, 97, 98, 99, Test4 Test10" +
                       Environment.NewLine );

        // test matchByContains
        yield return ( [ "rounds=100", "matchByContains=true", "divisors:4=Test4", "divisors:10=Test10", "divisors:19=Test19" ],
                       "1, 2, 3, Test4, 5, 6, 7, Test4, 9, Test10, 11, Test4, 13, Test4, 15, Test4, 17, 18, Test19, Test4 Test10, 21, 22, 23, Test4, 25, 26, 27, Test4, 29, Test10, 31, Test4, 33, Test4, 35, Test4, 37, Test19, 39, Test4 Test10, Test4, Test4, Test4, Test4, Test4, Test4, Test4, Test4, Test4, Test10, 51, Test4, 53, Test4, 55, Test4, Test19, 58, 59, Test4 Test10, 61, 62, 63, Test4, 65, 66, 67, Test4, 69, Test10, 71, Test4, 73, Test4, 75, Test4 Test19, 77, 78, 79, Test4 Test10, 81, 82, 83, Test4, 85, 86, 87, Test4, 89, Test10, 91, Test4, 93, Test4, Test19, Test4, 97, 98, 99, Test4 Test10" +
                       Environment.NewLine );
    }


    [Test]
    public void TestError()
    {
        using Process process = new();

        process.StartInfo = new ProcessStartInfo( "FizzBuzz.exe", [ "notavalidparam=true" ] )
        {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        process.Start();

        process.WaitForExit( TimeSpan.FromSeconds( 5 ) );

        Assert.That( process.ExitCode, Is.EqualTo( 1 ) );

        string error = process.StandardError.ReadToEnd();
        Assert.That( error, Is.Not.Empty );

        string output = process.StandardOutput.ReadToEnd();
        Assert.That( output, Is.Empty );
    }

}
