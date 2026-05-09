using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using SeedsClassification.Core.Models;

namespace SeedsClassification.Core.Data
{
    public class CsvLoader
    {
        public List<GrainBle> Charger(string cheminFichier)
        {
            var liste = new List<GrainBle>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            };

            using (var reader = new StreamReader(cheminFichier))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    string classeStr = csv.GetField<string>(0);

                    TypeBle classe;

                    switch (classeStr)
                    {
                        case "Kama":
                            classe = TypeBle.Kama;
                            break;
                        case "Rosa":
                            classe = TypeBle.Rosa;
                            break;
                        case "Canadian":
                            classe = TypeBle.Canadian;
                            break;
                        default:
                            throw new Exception("Classe inconnue : " + classeStr);
                    }

                    double area = csv.GetField<double>(1);
                    double perimeter = csv.GetField<double>(2);
                    double compactness = csv.GetField<double>(3);
                    double length = csv.GetField<double>(4);
                    double width = csv.GetField<double>(5);
                    double asymmetry = csv.GetField<double>(6);
                    double grooveLength = csv.GetField<double>(7);

                    var grain = new GrainBle(
                        area,
                        perimeter,
                        compactness,
                        length,
                        width,
                        asymmetry,
                        grooveLength,
                        classe
                    );

                    liste.Add(grain);
                }
            }

            return liste;
        }
    }
}