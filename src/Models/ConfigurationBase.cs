using System.ComponentModel.DataAnnotations;

namespace IOptionTest.Models;

public partial class ConfigurationBase
{
    public ConfigurationBase()
    {
        Name = GetType().Name;
    }

    public string Name { get; set; } = "";

    public string OverriddenInCode { get; set; } = "";
    public string FromEnvironment { get; set; } = "";

    [Required]
    [RegularExpression("^\\w+Settings\\d$")]
    public string FromAppSettings { get; set; } = "";

    [Required]
    [RegularExpression("^\\w+Settings\\d$")]
    public string FromDevelopmentSettings { get; set; } = "";

    public string FromSharedDevelopmentSettings { get; set; } = "";

}
