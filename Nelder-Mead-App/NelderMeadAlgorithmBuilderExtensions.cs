using Nelder_Mead_App.Models;
using NelderMeadLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
