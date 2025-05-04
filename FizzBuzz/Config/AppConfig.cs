using System.Collections.Generic;


namespace FizzBuzz.Config;


public class AppConfig
{

    public int Rounds { get; set; }
    public required Dictionary<int, string> Devisors { get; set; }

}
