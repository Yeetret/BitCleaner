using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;

namespace BitCleaner.String_Decryption
{
    internal class FieldInitializer
    {
        /// <summary>
        /// This gets the value of the field which is byte[]
        /// </summary>
        /// <param name="assembly">The assembly to be used.</param>
        /// <param name="field">The field that is going to used.</param>
        /// <returns>The byte array value of the given field</returns>
        internal static byte[]? GetBytesOfField(Assembly assembly, FieldDef field)
        {
            int fieldToken = field.MDToken.ToInt32();
            return assembly.ManifestModule.ResolveField(fieldToken)?.GetValue(null) as byte[];
        }
    }
}
