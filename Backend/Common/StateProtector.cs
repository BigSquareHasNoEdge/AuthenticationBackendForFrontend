using Microsoft.AspNetCore.DataProtection;

namespace Backend.Common;

public class StateProtector(IDataProtectionProvider provider)
{
    const string PURPOSE = "encrypt state parameter";
    private readonly IDataProtector protector = provider.CreateProtector(PURPOSE);

    public string Protect(string plain) => protector.Protect(plain);
    public string? Unprotect(string cypher)
    {
        try
        {
            return protector.Unprotect(cypher);
        }
        catch { return null; }
    } 
}
