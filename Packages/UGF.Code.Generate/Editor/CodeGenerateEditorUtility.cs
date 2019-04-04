using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UGF.Code.Generate.Editor
{
    public static class CodeGenerateEditorUtility
    {
        public static HashSet<string> UsingDirectivesCollectUniqueNames(IEnumerable<UsingDirectiveSyntax> directives)
        {
            if (directives == null) throw new ArgumentNullException(nameof(directives));

            var hashset = new HashSet<string>();

            foreach (UsingDirectiveSyntax directive in directives)
            {
                hashset.Add(directive.Name.GetText().ToString());
            }

            return hashset;
        }
    }
}
