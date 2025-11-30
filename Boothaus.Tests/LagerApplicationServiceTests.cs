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
        bootRepo = new BootRepository();
        auftragRepo = new AuftragRepository();
        lagerRepo = new LagerRepository();
        importExport = Substitute.For<ImportExportService>();
        sut = new LagerApplicationService(bootRepo, auftragRepo, lagerRepo, importExport);
    }

    [Test]
    public void ErstelleLagerkalender_KeineKonflikte_AlleAufträgeZugewiesen()
    {
        // Arrange
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
