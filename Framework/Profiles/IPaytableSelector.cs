using Framework.Domain;

namespace Framework.Profiles;

public interface IPaytableSelector<TTier> where TTier : Tier
{
    Paytable<TTier> SelectPaytable(decimal stake, DateTimeOffset time);
}
