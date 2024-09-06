using System.Reflection;

namespace Cola.Utils.Enums;

/// <summary>
///  枚举类
/// </summary>
/// <param name="id"></param>
/// <param name="description"></param>
public abstract class Enumeration(int id, string description) : IComparable
{
    private string Description { get; set; } = description;

    private int Id { get; set; } = id;

    public override string ToString() => Description;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
            .Select(f => f.GetValue(null))
            .Cast<T>();

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
}