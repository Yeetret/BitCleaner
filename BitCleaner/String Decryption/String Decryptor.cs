using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace BitCleaner.String_Decryption
{
    internal class StringDecryptor
    {
        internal static void Deobfuscate(ModuleDefMD module, Assembly assembly)
        {
            foreach (TypeDef types in module.GetTypes().Where(type => type.HasMethods))
            {
                foreach (MethodDef method in types.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    var instr = method.Body.Instructions;
                    for (int i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode.Code == Code.Ldsfld
                            && instr[i + 1].OpCode.Code == Code.Ldsfld
                            && instr[i + 2].OpCode.Code == Code.Ldsfld
                            && instr[i + 3].OpCode.Code == Code.Call)
                        {
                            var suspectedDecrypter = instr[i + 3].Operand as MethodDef;

                            var firstField = instr[i];
                            var secondField = instr[i + 1];
                            var thirdField = instr[i + 2];

                            if (IsValidField(firstField)
                                && IsValidField(secondField)
                                && IsValidField(thirdField)
                                && IsValidDecrypter(suspectedDecrypter))
                            {
                                var firstFieldValid = firstField.Operand as FieldDef;
                                var secondFieldValid = secondField.Operand as FieldDef;
                                var thirdFieldValid = thirdField.Operand as FieldDef;

                                byte[] firstFieldValue = FieldInitializer.GetBytesOfField(assembly, firstFieldValid);
                                byte[] secondFieldValue = FieldInitializer.GetBytesOfField(assembly, secondFieldValid);
                                byte[] thirdFieldValue = FieldInitializer.GetBytesOfField(assembly, thirdFieldValid);

                                string decryptedString = StaticDecryptor.Decrypt(firstFieldValue, secondFieldValue,
                                    thirdFieldValue);

                                instr[i].OpCode = OpCodes.Ldstr;
                                instr[i].Operand = decryptedString;

                                instr.Remove(instr[i + 3]);
                                instr.Remove(secondField);
                                instr.Remove(thirdField);
                            }
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Strings Decrypted.");
        }
        private static bool IsValidField(Instruction suspectedField)
        {

            if (suspectedField.OpCode.Code != Code.Ldsfld)
                return false;
            FieldDef validOpCode = suspectedField.Operand as FieldDef;
            if (!validOpCode.DeclaringType.IsNested)
                return false;
            if (!validOpCode.DeclaringType.DeclaringType.IsGlobalModuleType)
                return false;
            if (!validOpCode.DeclaringType.IsExplicitLayout)
                return false;
            return validOpCode.DeclaringType.Fields.Count == 2;
        }
        private static bool IsValidDecrypter(MethodDef suspectedMethod)
        {
            if (suspectedMethod.DeclaringType.Methods.Count != 1)
                return false;
            if (suspectedMethod.GetParams().Count != 3)
                return false;
            return suspectedMethod.ReturnType == suspectedMethod.Module.CorLibTypes.String;
        }
    }
}
