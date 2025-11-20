using Boothaus.Domain;
using Bogus; 

namespace Boothaus.GUI;

internal static class DefaultData
{
    internal static List<Lagerauftrag> Aufträge(Lager lager, IEnumerable<Boot> boote)
    {
        Faker faker = new("de");
        var saisonStartJahr = 2025;
         
        var inVon = new DateTime(saisonStartJahr, 09, 01);
        var inBis = new DateTime(saisonStartJahr, 12, 15);
        var outVon = new DateTime(saisonStartJahr + 1, 1, 10);
        var outBis = new DateTime(saisonStartJahr + 1, 4, 15);


        var result = new List<Lagerauftrag>();



        foreach (var boot in boote)
        {
            if (faker.Random.Bool(0.5f)) 
                continue;

            var von = DateOnly.FromDateTime(faker.Date.Between(inVon, inBis));
            var bis = DateOnly.FromDateTime(faker.Date.Between(outVon, outBis));
            var auftrag = new Lagerauftrag(
                id: Guid.NewGuid(),
                lager: lager,
                boot: boot,
                von: von,
                bis: bis
            );

            result.Add(auftrag); 
        }

        return result;
    }

    internal static List<Boot> Boote()
    {
        var faker = new BootFaker(saisonStartJahr: 2025, seed: 1234);
        return faker.Generate(70);
    }

    internal static Lager Lager()
    {
        double standardMaxBreite = 5.0;
        double standardMaxLänge = 12.0;
        int anzahlReihen = 7;
        int plätzeProReihe = 10;

        var lager = new Lager(standardMaxBreite: standardMaxBreite, standardMaxLänge: standardMaxLänge);

        for (int reiheIndex = 0; reiheIndex < anzahlReihen; reiheIndex++)
        {
            var reihe = new Lagerreihe(reiheIndex);

            for (int platzIndex = 0; platzIndex < plätzeProReihe; platzIndex++)
            {
                var platz = new Lagerplatz
                {
                    Id = Guid.NewGuid(),
                };

                reihe.PlatzHinzufügen(platz);
            }

            lager.ReiheUpsert(reihe);
        }

        return lager;
    }
}



/// <summary>
/// Erzeugt realistisch wirkende Jachten für eine Wintersaison.
/// Maße in Metern, Gewicht in kg. Einlagerung im Herbst,
/// Auslagerung im Folgejahr Winter/Frühling.
/// </summary>
public sealed class BootFaker : Faker<Boot>
{
    private static readonly string[] namen =
    {
        "Seeadler", "Freya", "Wogenlied", "Sturmvogel", "Hoffnung", "Aurora",
        "Nordstern", "Greif", "Seerose", "Blitz", "Isolde", "Walhall", "Wellenkind",
        "Rungholt", "Albatros", "Sigrun", "Hertha", "Möwe", "Falke", "Minna", "Seebrise",
        "Thor", "Loreley", "Frithjof", "Helga", "Wiking", "Hansa", "Nixe", "Undine", "Edda",
        "Odin", "Valkyrie", "Freiheit", "Greta", "Orkan", "Sonnenschein", "Hedwig", "Silbermöwe",
        "Wanderer", "Nordlicht", "Schwan", "Viktoria", "Traumsee", "Gertrud", "Roland", "Lilie", "Baltica",
        "Hagen", "Sehnsucht", "Adlerhorst", "Marie", "Brunhild", "Stella", "Gudrun", "Trave", "Neptun", "Helene",
        "Sylvia", "Eisvogel", "Perle", "Hoffart", "Windspiel", "Ragna", "Störtebeker", "Seeschwalbe", "Freundschaft",
        "Jungfernstieg", "Else", "Woge", "Blankenese", "Glückauf", "Alma", "Fernweh", "Perle", "Regina",
        "Eckernförde", "Luna", "Christa", "Falkenauge", "Seemannsgarn", "Hildegard", "Kapitän", "Elena", "Carina",
        "Marina", "Seestern", "Antje", "Ingrid", "Charlotte", "Gorch Fock", "Hilde", "Liselotte", "Martha", "Rasmus",
        "Suse", "Walfisch", "Brise", "Ebbe", "Flut", "Gisela", "Hildegard", "Kormoran", "Lotte", "Moby",
        "Rita", "Senta", "Tümmler", "Walfänger", "Yvonne", "Zora", "Robinson Crusoe", "Pitcairn", "Weihnachtsinsel",
        "Schellfisch", "England", "Dschibuti", "Helgoland", "Seepferd", "Fidschi", "Sindbad", "Gozo",
        "Bärlauch", "Norderney", "Schlossberg", "Pinguin", "Katharina", "Orient", "Irene"
    };

    private static readonly string[] namenszusatz =
    {
        "", "I", "II", "Jung", "III", "IV", "V", "VI", "VII", "VIII", "IX", "Senior", "Junior",
        "von Hamburg", "von Kiel", "von Lübeck", "von Rostock", "von Stralsund", "von Bremen",
        "von Cuxhaven", "von Flensburg", "von Travemünde", "von Warnemünde",
        "am See", "an der Elbe", "an der Ostsee", "an der Nordsee",
        "des kalten Nords", "des wilden Meeres", "des stillen Wassers",
        "siebenmal um die Welt"
    };

    public BootFaker(int saisonStartJahr, int? seed = null, string locale = "de")
    {
        if (seed is not null) Randomizer.Seed = new Random(seed.Value); 

        double maxLänge = 12.0;

        Locale = locale;

        RuleFor(boot => boot.Id, _ => Guid.NewGuid());

        RuleFor(boot => boot.Name, f =>
        {
            var name = f.PickRandom(namen);
            var zusatz = f.PickRandom(namenszusatz);
            if (!string.IsNullOrEmpty(zusatz) && f.Random.Bool(0.3f))
                name += " " + zusatz;
            return name;
        });

        RuleFor(boot => boot.Rumpflänge, f =>
        {
            var länge = f.Random.Double(5.0, maxLänge);
            return Math.Round(länge, 2);

        });

        RuleFor(boot => boot.Breite, (f, b) =>
        {
            var min = b.Rumpflänge * 0.26;
            var max = b.Rumpflänge * 0.34;
            var w = f.Random.Double(min, max);
            return Math.Round(w, 2);
        });

        
        RuleFor(boot => boot.Kontakt, f => f.Name.LastName());
    }
}
  
public sealed class LagerplatzFaker
{  
    public IEnumerable<Lagerplatz> Generate(int totalCount = 70)
    {
        // alle Plätze erzeugen
        var plaetze = Enumerable.Range(0, totalCount)
            .Select(_ => new Lagerplatz
            {
                Id = Guid.NewGuid()
            });

        return plaetze;
    }
}

public sealed class LagerreiheFaker
{

}

