using Application.Model;

namespace Application.Commands;

public record CreateYarnCommand(
    List<string> ProducerNames,
    List<string> YarnNames,
    List<StitchRange> Gauges,
    List<NeedleRange> NeedleSizes);