using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UGF.Code.Generate.Editor
{
    public class CodeGenerateWalkerCheckAttribute : CSharpSyntaxWalker
    {
        public SemanticModel SemanticModel { get; }
        public ITypeSymbol TypeSymbol { get; }
        public bool Result { get; private set; }

        public CodeGenerateWalkerCheckAttribute(SemanticModel semanticModel, ITypeSymbol typeSymbol, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node) : base(depth)
        {
            SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
            TypeSymbol = typeSymbol ?? throw new ArgumentNullException(nameof(typeSymbol));
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            base.VisitAttribute(node);

            if (!Result)
            {
                ITypeSymbol typeSymbol = SemanticModel.GetTypeInfo(node).ConvertedType;

                if (typeSymbol.Equals(TypeSymbol))
                {
                    Result = true;
                }
            }
        }
    }
}
