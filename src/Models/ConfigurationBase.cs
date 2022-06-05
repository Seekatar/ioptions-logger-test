using System.ComponentModel.DataAnnotations;

namespace IOptionTest;

public partial class ConfigurationBase
{
    public ConfigurationBase()
    {
        Name = this.GetType().Name;
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
