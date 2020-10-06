using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor
{
    /// <summary>
    /// Represents the syntax rewrite that adds attribute to the specified target.
    /// <para>
    /// This rewriter visit all types of declaration where attribute can be applied.
    /// </para>
    /// <para>
    /// Rewrite will no format trivia of the added attributes.
    /// </para>
    /// </summary>
    public class CodeGenerateRewriterAddAttributeToNode : CSharpSyntaxRewriter
    {
        /// <summary>
        /// Gets syntax generator used to apply attribute.
        /// </summary>
        public SyntaxGenerator Generator { get; }

        /// <summary>
        /// Gets syntax node of the attribute to apply.
        /// </summary>
        public SyntaxNode Attribute { get; }

        /// <summary>
        /// Gets delegate to validate target declaration to apply attribute.
        /// </summary>
        public CodeGenerateRewriterAddAttributeToNodeValidate Validate { get; }

        /// <summary>
        /// Creates rewriter with the specified generator, attribute and validate handler.
        /// </summary>
        /// <param name="generator">The syntax generator used to apply attribute.</param>
        /// <param name="attribute">The syntax node attribute to apply.</param>
        /// <param name="validate">The delegate handler to validate declaration.</param>
        public CodeGenerateRewriterAddAttributeToNode(SyntaxGenerator generator, SyntaxNode attribute, CodeGenerateRewriterAddAttributeToNodeValidate validate = null)
        {
            Generator = generator ?? throw new ArgumentNullException(nameof(generator));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            Validate = validate ?? (declaration => true);
        }

        /// <summary>
        /// Applies attribute to the specified syntax node that pass validation.
        /// </summary>
        /// <param name="node">The syntax node to apply.</param>
        protected virtual SyntaxNode Apply(SyntaxNode node)
        {
            return Validate(node) ? AddAttribute(node) : node;
        }

        /// <summary>
        /// Adds attribute to the specified declaration.
        /// </summary>
        /// <param name="declaration">The declaration to add attribute.</param>
        protected virtual SyntaxNode AddAttribute(SyntaxNode declaration)
        {
            return Generator.AddAttributes(declaration, Attribute);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return Apply(base.VisitClassDeclaration(node));
        }

        public override SyntaxNode VisitStructDeclaration(StructDeclarationSyntax node)
        {
            return Apply(base.VisitStructDeclaration(node));
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            return Apply(base.VisitInterfaceDeclaration(node));
        }

        public override SyntaxNode VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            return Apply(base.VisitEnumDeclaration(node));
        }

        public override SyntaxNode VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            return Apply(base.VisitDelegateDeclaration(node));
        }

        public override SyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            return Apply(base.VisitEnumMemberDeclaration(node));
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            return Apply(base.VisitFieldDeclaration(node));
        }

        public override SyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            return Apply(base.VisitEventFieldDeclaration(node));
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return Apply(base.VisitMethodDeclaration(node));
        }

        public override SyntaxNode VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            return Apply(base.VisitOperatorDeclaration(node));
        }

        public override SyntaxNode VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            return Apply(base.VisitConversionOperatorDeclaration(node));
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            return Apply(base.VisitConstructorDeclaration(node));
        }

        public override SyntaxNode VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            return Apply(base.VisitDestructorDeclaration(node));
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            return Apply(base.VisitPropertyDeclaration(node));
        }

        public override SyntaxNode VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            return Apply(base.VisitIndexerDeclaration(node));
        }

        public override SyntaxNode VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            return Apply(base.VisitAccessorDeclaration(node));
        }

        public override SyntaxNode VisitTypeParameter(TypeParameterSyntax node)
        {
            return Apply(base.VisitTypeParameter(node));
        }

        public override SyntaxNode VisitParameter(ParameterSyntax node)
        {
            return Apply(base.VisitParameter(node));
        }
    }
}
