using Microsoft.CodeAnalysis;

namespace UGF.Code.Generate.Editor
{
    /// <summary>
    /// Represents delegate to validate specified declaration to apply an attribute.
    /// </summary>
    /// <param name="declaration">The declaration to validate.</param>
    public delegate bool CodeGenerateRewriterAddAttributeToNodeValidate(SyntaxNode declaration);
}
