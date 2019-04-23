using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor
{
    /// <summary>
    /// Provides extensions for code generate.
    /// </summary>
    public static class CodeGenerateEditorExtensions
    {
        /// <summary>
        /// Generates auto property with the specified name, type, accessibility, modifiers and initializer.
        /// </summary>
        /// <param name="generator">The syntax generator to use.</param>
        /// <param name="name">The name of the auto property.</param>
        /// <param name="type">The type of the property.</param>
        /// <param name="accessibility">The accessibility of the property.</param>
        /// <param name="modifiers">The modifiers of the property.</param>
        /// <param name="initializer">The initializer of the property.</param>
        public static PropertyDeclarationSyntax AutoPropertyDeclaration(this SyntaxGenerator generator, string name, SyntaxNode type, Accessibility accessibility = Accessibility.NotApplicable, DeclarationModifiers modifiers = default, SyntaxNode initializer = null)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var property = (PropertyDeclarationSyntax)generator.PropertyDeclaration(name, type, accessibility, modifiers);
            AccessorListSyntax accessorList = SyntaxFactory.AccessorList();

            for (int i = 0; i < property.AccessorList.Accessors.Count; i++)
            {
                AccessorDeclarationSyntax accessor = property.AccessorList.Accessors[i];

                accessor = accessor.RemoveNode(accessor.Body, SyntaxRemoveOptions.KeepNoTrivia);
                accessor = accessor.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

                accessorList = accessorList.AddAccessors(accessor);
            }

            property = property.ReplaceNode(property.AccessorList, accessorList);

            if (initializer != null)
            {
                property = property.WithInitializer(SyntaxFactory.EqualsValueClause((ExpressionSyntax)initializer));
                property = property.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            }

            return property;
        }

        /// <summary>
        /// Tries to get type symbol by the specified type metadata name.
        /// <para>
        /// This method also resolve generic types.
        /// </para>
        /// </summary>
        /// <param name="compilation">The compilation to use.</param>
        /// <param name="type">The type to find symbol.</param>
        /// <param name="typeSymbol">The found type symbol.</param>
        public static bool TryGetTypeByMetadataName(this CSharpCompilation compilation, Type type, out INamedTypeSymbol typeSymbol)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (type == null) throw new ArgumentNullException(nameof(type));

            typeSymbol = compilation.GetTypeByMetadataName(type.FullName);

            if (typeSymbol == null)
            {
                if (type.IsGenericType)
                {
                    Type genericDefinition = type.GetGenericTypeDefinition();
                    Type[] genericArguments = type.GetGenericArguments();

                    return TryGetGenericTypeByMetadataName(compilation, genericDefinition.FullName, genericArguments.Select(x => x.FullName), out typeSymbol);
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to get type symbol from the specified generic type definition and type arguments metadata.
        /// </summary>
        /// <param name="compilation">The compilation to use.</param>
        /// <param name="genericDefinition">The type metadata name of the generic definition.</param>
        /// <param name="arguments">The types metadata names of the generic arguments.</param>
        /// <param name="typeSymbol">The found type symbol.</param>
        public static bool TryGetGenericTypeByMetadataName(this CSharpCompilation compilation, string genericDefinition, IEnumerable<string> arguments, out INamedTypeSymbol typeSymbol)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (genericDefinition == null) throw new ArgumentNullException(nameof(genericDefinition));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));

            INamedTypeSymbol genericDefinitionTypeSymbol = compilation.GetTypeByMetadataName(genericDefinition);

            if (genericDefinitionTypeSymbol != null)
            {
                var argumentTypeSymbols = new ITypeSymbol[arguments.Count()];
                int index = 0;

                foreach (string argument in arguments)
                {
                    INamedTypeSymbol argumentTypeSymbol = compilation.GetTypeByMetadataName(argument);

                    if (argumentTypeSymbol != null)
                    {
                        argumentTypeSymbols[index++] = argumentTypeSymbol;
                    }
                    else
                    {
                        typeSymbol = null;
                        return false;
                    }
                }

                typeSymbol = genericDefinitionTypeSymbol.Construct(argumentTypeSymbols);
                return true;
            }

            typeSymbol = null;
            return false;
        }
    }
}
