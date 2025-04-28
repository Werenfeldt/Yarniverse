using Application.Model;

namespace Application.Commands;

public record CreateYarnCommand(
    List<string> ProducerNames,
    List<string> YarnNames,
    List<Range<int>> Gauges,
    List<Range<double>> NeedleSizes);