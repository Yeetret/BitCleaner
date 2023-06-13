using BitCleaner.String_Decryption;
using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BitCleaner.DotNet_Release
{
    internal class Unhooker //The original implementation on the obfuscator is broken so I won't bother fixing anything.
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
                        if (instr[i].OpCode.Code == Code.Ldc_I4
                            && instr[i + 1].OpCode.Code == Code.Ldc_I4
                            && instr[i + 2].OpCode.Code == Code.Call)
                        {
                            var opOne = instr[i + 2].Operand as MethodDef;
                            if(!IsHooker(opOne))
                                continue;

                            var resolvedMethod = module.Import(assembly.ManifestModule.ResolveMethod(instr[i].GetLdcI4Value()));

                            instr[i].OpCode = OpCodes.Call;
                            instr[i].Operand = resolvedMethod;
                            instr.Remove(instr[i + 1]);
                            instr.Remove(instr[i + 2]);
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hooker unhooked.");
        }

        private static bool IsHooker(MethodDef suspectedHooker/* bro the name is funny af idk why */)
        {
            if (suspectedHooker == null)
                return false;
            if (suspectedHooker.DeclaringType.Methods.Count != 2)
                return false;
            return suspectedHooker.DeclaringType.Methods.Any(x => x.IsPinvokeImpl) 
                   && suspectedHooker.Body.Instructions.Any(x => x.OpCode.Code == Code.Call 
                && x.Operand != null 
                && x.Operand.ToString().Contains("GetFunctionPointer"));
        } 
    }
}