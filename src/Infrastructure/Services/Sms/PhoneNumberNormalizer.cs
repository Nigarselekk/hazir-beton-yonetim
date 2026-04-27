using HazirBeton.Application.Features.Sms;

namespace HazirBeton.Infrastructure.Services.Sms;

public class PhoneNumberNormalizer : IPhoneNumberNormalizer
{
    public string? Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        Span<char> digits = stackalloc char[raw.Length];
        var len = 0;
        foreach (var ch in raw)
        {
            if (ch >= '0' && ch <= '9')
                digits[len++] = ch;
        }

        if (len == 0)
            return null;

        var stripped = new string(digits[..len]);

        // Drop a country code 90 prefix so we work with the 10-digit subscriber number.
        if (stripped.Length == 12 && stripped.StartsWith("90"))
            stripped = stripped[2..];
        // Some inputs like "0090..." may appear; collapse to subscriber digits.
        else if (stripped.Length == 13 && stripped.StartsWith("0090"))
            stripped = stripped[4..];
        // A leading domestic 0 is common in Turkish formatting.
        else if (stripped.Length == 11 && stripped.StartsWith("0"))
            stripped = stripped[1..];

        // Subscriber number must be 10 digits and start with "5" (Turkish mobile).
        if (stripped.Length != 10 || stripped[0] != '5')
            return null;

        return "90" + stripped;
    }
}
