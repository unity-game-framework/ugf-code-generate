using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor
{
    public static class CodeGenerateEditorUtility
    {
        public static PropertyDeclarationSyntax AutoPropertyDeclaration(CSharpCompilation compilation, string name, Type returnType, Accessibility accessibility)
        {
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(new AdhocWorkspace(), LanguageNames.CSharp);
            SyntaxNode propertyReturnType = generator.TypeExpression(compilation.GetTypeByMetadataName(returnType.FullName));
            var property = (PropertyDeclarationSyntax)generator.PropertyDeclaration(name, propertyReturnType, Accessibility.Public);
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
