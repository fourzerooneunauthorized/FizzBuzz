namespace FizzBuzz.Config;


public class ConfigItemMeta
{

    /// <summary>
    /// Config item name
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Non-empty value for config item is required
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Config item description
    /// </summary>
    public required string Description { get; init; }

}
