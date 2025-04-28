using System.Linq.Expressions;
using Application.Model;

namespace Application.Helpers;

public record IntRange(int Lower, int Upper);
public record DoubleRange(double Lower, double Upper);

public static class RangeHelper
{
    public static Expression<Func<Yarn, bool>> StitchInRange(int value) => yarn => yarn.Gauge.Stitch.Min <= value && yarn.Gauge.Stitch.Max >= value;
    
    public static Expression<Func<Yarn, bool>> NeedlesInRange(double value) => yarn => yarn.Gauge.NeedleSize.Min <= value && yarn.Gauge.NeedleSize.Max >= value;
    
    public static bool InRange((int,int) value, int target) => value.Item1 <= target && target <= value.Item2; 
    
    public static (int Lower, int Upper) MapRange(IntRange value) => (value.Lower, value.Upper);
    
    public static (double Lower, double Upper) MapRange(DoubleRange value) => (value.Lower, value.Upper);
    
    public static List<Range<double>> MapRange(List<(double,double)> value) => value.ConvertAll(x => new Range<double>(x.Item1, x.Item2));
    
    public static List<Range<int>> MapRange(List<(int,int)> value) => value.ConvertAll(x => new Range<int>(x.Item1, x.Item2));
}
