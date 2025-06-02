using Application.Helpers;
using Application.Model;
using Application.Services;
using Database;
using Moq;

namespace Test;

public class Tests
{
    private IYarnAlternativeService _yarnAlternativeService;
    private Mock<IMongoDb> _mongoDbMock;
    
    [SetUp]
    public void Setup()
    {
        _mongoDbMock = new Mock<IMongoDb>();
    }

    [Test]
    public async Task Test1()
    {
        //Arrange
        SetUpMongoDb(20, 1);
        
        //Act
        var result = await _yarnAlternativeService.FindSingleYarnSuggestions(20, 4.0);
        var yarn = result.ToList().First();
        
        //Assert
        Assert.That(yarn.Yarn.Name, Is.EqualTo("Lima"));
    }

    private void SetUpMongoDb(int targetGauge, int gaugeTolerance)
    {
        // Act: use actual YarnMath logic to get the expected predicate
        var predicate = YarnMath.SingleYarn(targetGauge, gaugeTolerance);

        // Apply predicate to known yarns to get the expected filtered result
        var expectedYarns = GetYarn().Where(predicate.Compile()).ToList();

        _mongoDbMock.Setup(r => r.GetByPredicateAsync(predicate)).ReturnsAsync(expectedYarns);
        _yarnAlternativeService = new YarnAlternativeService(_mongoDbMock.Object);
    }

    private List<Yarn> GetYarn()
    {
        var filcolana = new Producer("filcolana");
        var sandnes = new Producer("sandnes");
        var drops = new Producer("drop");
        var isager = new Producer("isager");

        return new List<Yarn>()
        {
            new("Arwetta", filcolana, new Gauge(new (28, 30), new (2.5, 3.0))),
            new("Peruvian Highland Wool", filcolana, new Gauge(new(18, 20), new(4.5, 5.5))),
            new("Tilia", filcolana, new Gauge(new(24, 28),new (3.0, 4.0))),
            new("Saga", filcolana, new Gauge(new(26, 28), new(3.0, 3.5))),
            new("Merci", filcolana, new Gauge(new(26, 28), new(2.5, 3.0))),

            new("Tynn Merinoull", sandnes, new Gauge(new(27, 31),new(2.5, 3.0))),
            new("Peer Gynt", sandnes, new Gauge(new(20, 22), new(3.5, 4.0))),
            new("Kos", sandnes, new Gauge(new(16, 18),new (5.0, 5.5))),
            new("Sunday", sandnes, new Gauge(new(28, 32),new (2.5, 3.0))),
            new("Double Sunday", sandnes, new Gauge(new(20, 22),new (3.5, 4.0))),

            new("Flora", drops, new Gauge(new(24, 26),new (3.0, 3.5))),
            new("Lima", drops, new Gauge(new(20, 22), new(4.0, 5.0))),
            new("Karisma", drops, new Gauge(new(21, 23), new(4.0, 4.5))),
            new("Air", drops, new Gauge(new(17, 19),new (5.0, 5.5))),
            new("Baby Merino", drops, new Gauge(new(28, 30), new(2.5, 3.5))),

            new("Alpaca", isager, new Gauge(new(26, 28), new(3.0, 3.5))),
            new("Spinni", isager, new Gauge(new(28, 32), new(2.5, 3.0))),
            new("Tvinni", isager, new Gauge(new(26, 28), new(2.5, 3.5))),
            new("Eco Soft", isager, new Gauge(new(16, 18), new(5.0, 6.0))),
            new("Silk Mohair", isager, new Gauge(new(24, 28), new(3.0, 5.0))),
        };
    }
}