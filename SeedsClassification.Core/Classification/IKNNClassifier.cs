using SeedsClassification.Core.Models;

namespace SeedsClassification.Core.Classification
{
    public interface IKNNClassifier
    {
        TypeBle Classifier(GrainBle grain);
    }
}