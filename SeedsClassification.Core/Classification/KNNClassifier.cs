using System.Collections.Generic;
using System.Linq;
using SeedsClassification.Core.Models;

namespace SeedsClassification.Core.Classification
{
    public class KNNClassifier : IKNNClassifier
    {
        private readonly KDTree _tree;
        private readonly int _k;
        private readonly IDistance _distance;

        public KNNClassifier(KDTree tree, int k, IDistance distance)
        {
            _tree = tree;
            _k = k;
            _distance = distance;
        }

        public TypeBle Classifier(GrainBle grain)
        {
            var voisins = _tree.RechercherKPlusProches(grain, _k, _distance);

            var compteur = new Dictionary<TypeBle, int>();

            foreach (var voisin in voisins)
            {
                if (!compteur.ContainsKey(voisin.ClasseReelle))
                    compteur[voisin.ClasseReelle] = 0;

                compteur[voisin.ClasseReelle]++;
            }

            TypeBle classePredite = default;
            int max = -1;

            foreach (var pair in compteur)
            {
                if (pair.Value > max)
                {
                    max = pair.Value;
                    classePredite = pair.Key;
                }
            }

            return classePredite;
        }
    }
}