using System.Security.Claims;
using System.Text.Json;

namespace Web.Core.Utile
{
    public static class JwtClaimReader
    {
        public static ClaimsIdentity BuildIdentityFromJwt(string jwt)
        {
            var claims = ParseClaimsFromJwt(jwt);
            return new ClaimsIdentity(claims, authenticationType: "Bearer");
        }

        // 
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3) return Array.Empty<Claim>();

            var payloadJson = Base64UrlDecodeToString(parts[1]);
            using var doc = JsonDocument.Parse(payloadJson);
            var root = doc.RootElement;

            var claims = new List<Claim>();
            foreach (var p in root.EnumerateObject())
            {
                switch (p.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        claims.Add(new Claim(p.Name, p.Value.GetString()!));
                        break;
                    case JsonValueKind.Number:
                        claims.Add(new Claim(p.Name, p.Value.GetRawText()));
                        break;
                    case JsonValueKind.Array:
                        foreach (var v in p.Value.EnumerateArray())
                            claims.Add(new Claim(p.Name, v.ToString()));
                        break;
                    default:
                        claims.Add(new Claim(p.Name, p.Value.GetRawText()));
                        break;
                }
            }
            return claims;
        }

        public static string Base64UrlDecodeToString(string input)
        {
            string s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4) { case 2: s += "=="; break; case 3: s += "="; break; }
            var bytes = Convert.FromBase64String(s);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
