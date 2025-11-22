using Boothaus.Domain;
using Boothaus.Services.Contracts;
using Boothaus.Services.Persistence;
using Domain.Services;
using NSubstitute;
using NUnit.Framework;

namespace Boothaus.Tests;


[TestFixture]
public class LagerApplicationServiceTests
{
    private IBootRepository bootRepo;
    private IAuftragRepository auftragRepo;
    private ILagerRepository lagerRepo;
    private ImportExportService importExport;
    private LagerApplicationService sut;

    [SetUp]
    public void SetUp()
    {
        bootRepo = Substitute.For<IBootRepository>();
        auftragRepo = Substitute.For<IAuftragRepository>();
        lagerRepo = Substitute.For<ILagerRepository>();
    }

    [Test]
    public void ErstelleLagerkalender_KeineKonflikte_AlleAufträgeZugewiesen()
    {
        // Arrange
        bootRepo.GetAll().Returns(new List<Boot>()
        {
            new Boot("", "", 7, 5),
            new Boot("", "", 8, 4)
        });

        auftragRepo.GetAll().Returns(new List<Auftrag>
        {
            new Auftrag(),
            new Auftrag()
        });

        // Act
        // Assert
    }

    [Test]
    public void ErstelleLagerkalender_MehrAufträgeAlsPlätze_KeineKonsistenteZuweisung()
    {
        // Arrange
        // Act
        // Assert
    }


    [Test]
    public void ErstelleLagerkalender_AufträgeNichtVerschachtelt_KeineKonsistenteZuweisung()
    {
        // Arrange
        // Act
        // Assert
    }

    [Test]
    public void ErstelleLagerkalender_KeineAufträge_AlleAufträgeZugewiesen()
    {
        // Arrange
        // Act
        // Assert
    }


}
