using Social.Domain.Abstractions;

namespace Social.Domain.ValueObjects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
public sealed class ETag : ValueObject
{
    public string Value { get; private set; }

    public ETag()
    {
    }

    public ETag(string value)
    {
        Value = value;
    }

    public static ETag Generate()
    {
        return new ETag(Guid.NewGuid().ToString());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
