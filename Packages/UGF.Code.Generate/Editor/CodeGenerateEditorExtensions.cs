using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor
{
    public static class CodeGenerateEditorExtensions
    {
        public static PropertyDeclarationSyntax AutoPropertyDeclaration(this SyntaxGenerator generator, string name, SyntaxNode returnType, Accessibility accessibility)
        {
            var property = (PropertyDeclarationSyntax)generator.PropertyDeclaration(name, returnType, accessibility);
            AccessorListSyntax accessorList = SyntaxFactory.AccessorList();

            for (int i = 0; i < property.AccessorList.Accessors.Count; i++)
            {
                AccessorDeclarationSyntax accessor = property.AccessorList.Accessors[i];

                accessor = accessor.RemoveNode(accessor.Body, SyntaxRemoveOptions.KeepNoTrivia);
                accessor = accessor.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                accessorList = accessorList.AddAccessors(accessor);
            }

            property = property.ReplaceNode(property.AccessorList, accessorList);

            return property;
        }
    }
}
