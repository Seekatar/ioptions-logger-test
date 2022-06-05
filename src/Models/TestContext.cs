namespace IOptionTest.Models;

/// <summary>
/// Sample context for scoped logging
/// </summary>
public class TestContext
{
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public string ClientId { get; set; } = "";
    public int MarketEntityId { get; set; }

    public static Dictionary<string, object> ToDictionary(TestContext me)
    {
        return new Dictionary<string, object>
        {
            { nameof(CorrelationId), me.CorrelationId },
            { nameof(ClientId), me.ClientId },
            { nameof(MarketEntityId), me.MarketEntityId }
        };
    }
}
