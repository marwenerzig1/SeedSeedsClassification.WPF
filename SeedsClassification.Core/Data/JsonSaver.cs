using Newtonsoft.Json;
using SeedsClassification.Core.Evaluation;
using System.IO;

namespace SeedsClassification.Core.Data
{
    public class JsonSaver
    {
        public void Sauvegarder(GlobalResult result, string cheminFichier)
        {
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

            File.WriteAllText(cheminFichier, json);
        }
    }
}