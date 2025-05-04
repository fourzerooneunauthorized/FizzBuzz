using System.Collections.Generic;


namespace FizzBuzz.Config;


public class AppConfig
{

    /// <summary>
    /// Number of rounds to play
    /// </summary>
    public int Rounds { get; init; }

    /// <summary>
    /// Divisor settings, where key=divisor whole number;value=text replacement value
    /// </summary>
    public required Dictionary<int, string> Divisors { get; init; }

}
