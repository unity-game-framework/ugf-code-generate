using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UGF.Code.Generate.Editor
{
    /// <summary>
    /// Represents walker that determines whether the specified syntax node contains declaration with an attribute of the specified type symbol.
    /// </summary>
    public class CodeGenerateWalkerCheckAttribute : CSharpSyntaxWalker
    {
        /// <summary>
        /// Gets semantic model.
        /// </summary>
        public SemanticModel SemanticModel { get; }

        /// <summary>
        /// Gets type symbol of the attribute to check.
        /// </summary>
        public ITypeSymbol TypeSymbol { get; }

        /// <summary>
        /// Gets the result of the check.
        /// </summary>
        public bool Result { get; private set; }

        /// <summary>
        /// Creates walker with the specified semantic model and attribute type symbol.
        /// </summary>
        /// <param name="semanticModel">The semantic model used to get attribute type info.</param>
        /// <param name="typeSymbol">The attribute type symbol to check.</param>
        public CodeGenerateWalkerCheckAttribute(SemanticModel semanticModel, ITypeSymbol typeSymbol)
        {
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            TypeSymbol = typeSymbol ?? throw new ArgumentNullException(nameof(typeSymbol));
        }

        /// <summary>
        /// Checks whether the specified attribute syntax has type equals to the target attribute type.
        /// </summary>
        /// <param name="node">The attribute syntax to check.</param>
        protected virtual bool CheckAttribute(AttributeSyntax node)
        {
            return SemanticModel.GetTypeInfo(node).ConvertedType.Equals(TypeSymbol);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            base.VisitAttribute(node);

            if (!Result && CheckAttribute(node))
            {
                Result = true;
            }
        }
    }
}
