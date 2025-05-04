using System.Collections.Generic;


namespace FizzBuzz.Config;


public interface IConfigProvider
{

    /// <summary>
    /// Provide descriptions of configuration items
    /// </summary>
    IList<ConfigItemMeta> GetConfigItemDescriptions();


    /// <summary>
    /// Get configuration from source
    /// </summary>
    AppConfig GetConfig();

}
