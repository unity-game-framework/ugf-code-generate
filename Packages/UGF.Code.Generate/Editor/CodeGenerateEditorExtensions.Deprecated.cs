using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace UGF.Code.Generate.Editor
{
    public static partial class CodeGenerateEditorExtensions
    {
        /// <summary>
        /// Tries to get type symbol by the specified type metadata name.
        /// <para>
        /// This method also resolve generic types.
        /// </para>
        /// </summary>
        /// <param name="compilation">The compilation to use.</param>
        /// <param name="type">The type to find symbol.</param>
        /// <param name="typeSymbol">The found type symbol.</param>
        [Obsolete("TryGetTypeByMetadataName has been deprecated. Use TryConstructTypeSymbol instead.")]
        public static bool TryGetTypeByMetadataName(this Compilation compilation, Type type, out INamedTypeSymbol typeSymbol)
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
        [Obsolete("TryGetGenericTypeByMetadataName has been deprecated. Use TryConstructTypeSymbol or TryConstructGenericTypeSymbol instead.")]
        public static bool TryGetGenericTypeByMetadataName(this Compilation compilation, string genericDefinition, IEnumerable<string> arguments, out INamedTypeSymbol typeSymbol)
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
