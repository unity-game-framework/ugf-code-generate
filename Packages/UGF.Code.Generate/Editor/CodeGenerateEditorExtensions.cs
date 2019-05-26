using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace UGF.Code.Generate.Editor
{
    /// <summary>
    /// Provides extensions for code generate.
    /// </summary>
    public static partial class CodeGenerateEditorExtensions
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
        /// Tries to get any found type symbol from the specified type metadata name.
        /// <para>
        /// If specified compilation contains more than one matched types, will return first found.
        /// </para>
        /// </summary>
        /// <param name="compilation">The compilation to use.</param>
        /// <param name="fullyQualifiedMetadataName">The name of the metadata.</param>
        /// <param name="typeSymbol">The found type symbol.</param>
        public static bool TryGetAnyTypeByMetadataName(this Compilation compilation, string fullyQualifiedMetadataName, out INamedTypeSymbol typeSymbol)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (fullyQualifiedMetadataName == null) throw new ArgumentNullException(nameof(fullyQualifiedMetadataName));

            typeSymbol = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);

            if (typeSymbol == null)
            {
                typeSymbol = compilation.Assembly.GetTypeByMetadataName(fullyQualifiedMetadataName);

                if (typeSymbol == null)
                {
                    foreach (MetadataReference reference in compilation.References)
                    {
                        if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol assemblySymbol)
                        {
                            typeSymbol = assemblySymbol.GetTypeByMetadataName(fullyQualifiedMetadataName);

                            if (typeSymbol != null)
                            {
                                break;
                            }
                        }
                    }
                }

                return typeSymbol != null;
            }

            return true;
        }

        /// <summary>
        /// Constructs type symbol from the specified type.
        /// <para>
        /// This method will throw exception whether type symbol not found for the specified type.
        /// </para>
        /// </summary>
        /// <param name="compilation">The compilation to use.</param>
        /// <param name="type">The type to construct.</param>
        public static INamedTypeSymbol ConstructTypeSymbol(this Compilation compilation, Type type)
        {
            if (!TryConstructTypeSymbol(compilation, type, out INamedTypeSymbol typeSymbol))
            {
                throw new ArgumentException($"The type symbol for the specified type not found: '{type}'.");
            }

            return typeSymbol;
        }

        /// <summary>
        /// Tries to construct type symbol from the specified type.
        /// <para>
        /// If the specified type is generic, will construct type symbol based on generic definition and arguments.
        /// </para>
        /// </summary>
        /// <param name="compilation">The compilation to use.</param>
        /// <param name="type">The type to construct.</param>
        /// <param name="typeSymbol">The constructed type.</param>
        public static bool TryConstructTypeSymbol(this Compilation compilation, Type type, out INamedTypeSymbol typeSymbol)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!TryGetAnyTypeByMetadataName(compilation, type.FullName, out typeSymbol))
            {
                if (type.IsGenericType && !type.IsGenericTypeDefinition)
                {
                    Type definition = type.GetGenericTypeDefinition();
                    Type[] arguments = type.GenericTypeArguments;

                    return TryConstructGenericTypeSymbol(compilation, definition, arguments, out typeSymbol);
                }
            }

            return typeSymbol != null;
        }

        /// <summary>
        /// Tries to construct type symbol from the generic definition and arguments.
        /// </summary>
        /// <param name="compilation">The compilation to use.</param>
        /// <param name="definition">The definition of the generic type.</param>
        /// <param name="arguments">The arguments of the generic to construct.</param>
        /// <param name="typeSymbol">The constructed type symbol.</param>
        public static bool TryConstructGenericTypeSymbol(this Compilation compilation, Type definition, IReadOnlyList<Type> arguments, out INamedTypeSymbol typeSymbol)
        {
            if (compilation == null) throw new ArgumentNullException(nameof(compilation));
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (!definition.IsGenericTypeDefinition) throw new ArgumentException("The specified type is not a generic type definition.", nameof(definition));
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            if (arguments.Count == 0) throw new ArgumentException("The specified arguments collection is empty.", nameof(arguments));

            if (TryConstructTypeSymbol(compilation, definition, out INamedTypeSymbol definitionTypeSymbol))
            {
                var argumentTypeSymbols = new ITypeSymbol[arguments.Count];

                for (int i = 0; i < arguments.Count; i++)
                {
                    Type argument = arguments[i];

                    if (TryConstructTypeSymbol(compilation, argument, out INamedTypeSymbol argumentTypeSymbol))
                    {
                        argumentTypeSymbols[i] = argumentTypeSymbol;
                    }
                    else
                    {
                        typeSymbol = null;
                        return false;
                    }
                }

                typeSymbol = definitionTypeSymbol.Construct(argumentTypeSymbols);
                return true;
            }

            typeSymbol = null;
            return false;
        }

        /// <summary>
        /// Tries to get generic name syntax from the specified type syntax.
        /// </summary>
        /// <param name="typeSyntax">The type syntax to get from.</param>
        /// <param name="genericNameSyntax">The found generic name syntax.</param>
        public static bool TryGetGenericNameSyntax(this TypeSyntax typeSyntax, out GenericNameSyntax genericNameSyntax)
        {
            if (typeSyntax == null) throw new ArgumentNullException(nameof(typeSyntax));

            if (typeSyntax is NameSyntax nameSyntax)
            {
                if (nameSyntax is QualifiedNameSyntax qualifiedNameSyntax)
                {
                    nameSyntax = qualifiedNameSyntax.Right;
                }

                genericNameSyntax = nameSyntax as GenericNameSyntax;

                return genericNameSyntax != null;
            }

            genericNameSyntax = null;
            return false;
        }
    }
}
