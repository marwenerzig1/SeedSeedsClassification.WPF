using SeedsClassification.Core.Models;

namespace SeedsClassification.Core.Classification
{
    public interface IDistance
    {
        double Calculer(GrainBle a, GrainBle b);
    }
}