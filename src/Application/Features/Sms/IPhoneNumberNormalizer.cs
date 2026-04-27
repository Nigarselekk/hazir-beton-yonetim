namespace HazirBeton.Application.Features.Sms;

public interface IPhoneNumberNormalizer
{
    // Returns the normalized E.164-style Turkish number (e.g. "905321234567"),
    // or null if the input cannot be parsed as a valid Turkish mobile number.
    string? Normalize(string? raw);
}
