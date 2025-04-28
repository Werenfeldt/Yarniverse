using System.Linq.Expressions;

namespace Application.Model;

public readonly record struct Range<T>(T Min, T Max) where T : struct
{
    public Range()
        : this(default!, default!)
    {
        if (typeof(T) != typeof(int) && typeof(T) != typeof(double))
        {
            throw new InvalidOperationException("Range<T> only supports int or double.");
        }
    }
}