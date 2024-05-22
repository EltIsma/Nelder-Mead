using NelderMeadLib;
using System.Windows.Documents;

namespace Nelder_Mead_App;

public static class NelderMeadAlgorithmBuilderExtensions
{
    public static NelderMeadAlgorithmBuilder UseFlowDocumentLogger(
        this NelderMeadAlgorithmBuilder builder,
        FlowDocument document)
    {
        builder.SetLogger(new FlowDocumentLogger(document));
        return builder;
    }
}
