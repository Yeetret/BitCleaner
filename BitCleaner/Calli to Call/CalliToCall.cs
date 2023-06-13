using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet.Emit;

namespace BitCleaner.Calli_to_Call
{
    internal class CalliToCall
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
                        if (instr[i].OpCode.Code == Code.Ldtoken
                            && instr[i + 1].OpCode.Code == Code.Call
                            && instr[i + 2].OpCode.Code == Code.Callvirt
                            && instr[i + 3].OpCode.Code == Code.Ldc_I4 //mdtoken
                            && instr[i + 4].OpCode.Code == Code.Call
                            && instr[i + 5].OpCode.Code == Code.Callvirt 
                            && instr[i + 6].IsStloc() 
                            && instr[i + 7].OpCode.Code == Code.Ldloca
                            && instr[i + 8].OpCode.Code == Code.Call
                            && instr[i + 9].OpCode.Code == Code.Calli)
                        {
                            var zeCall = instr[i + 8];
                            if(!IsValid(zeCall))
                                continue;

                            var tok = instr[i + 3].GetLdcI4Value();
                            var resolvedMethod = module.Import(assembly.ManifestModule.ResolveMethod(tok));

                            instr[i].OpCode = OpCodes.Call;
                            instr[i].Operand = resolvedMethod;

                            instr[i + 1].OpCode = OpCodes.Nop;
                            instr[i + 2].OpCode = OpCodes.Nop;
                            instr[i + 3].OpCode = OpCodes.Nop;
                            instr[i + 4].OpCode = OpCodes.Nop;
                            instr[i + 5].OpCode = OpCodes.Nop;
                            instr[i + 6].OpCode = OpCodes.Nop;
                            instr[i + 7].OpCode = OpCodes.Nop;
                            instr[i + 8].OpCode = OpCodes.Nop;
                            instr[i + 9].OpCode = OpCodes.Nop;

                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Calli has been converted to call");
        }
        private static bool IsValid(Instruction instr)
        {
            return instr.OpCode.Code == Code.Call && instr.Operand.ToString().Contains("GetFunctionPointer");
        }
    }
}