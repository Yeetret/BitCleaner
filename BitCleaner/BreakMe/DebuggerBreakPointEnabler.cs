using dnlib.DotNet.Emit;
using dnlib.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BitCleaner.BreakMe
{
    internal class DebuggerBreakPointEnabler
    {
        internal static void Deobfuscate(ModuleDefMD module, Assembly assembly)
        {
            foreach (TypeDef types in module.GetTypes().Where(type => type.HasMethods))
            {
                foreach (MethodDef method in types.Methods.Where(m => m.HasBody && m.Body.HasInstructions))
                {
                    
                    var instr = method.Body.Instructions;
                    if(instr.Count < 13)
                        continue;
                    for (int i = 0; i < instr.Count; i++)
                    {
                        if (instr[i].OpCode.Code == Code.Call
                            && instr[i + 1].IsLdloc()
                            && instr[i + 2].OpCode.Code == Code.Call
                            && instr[i + 3].IsStloc() 
                            && instr[i + 4].OpCode.Code == Code.Ldloca_S
                            && instr[i + 5].OpCode.Code == Code.Call //get_TotalMillisecond
                            && instr[i + 6].OpCode.Code == Code.Ldc_R8 //5000
                            && instr[i + 7].OpCode.Code == Code.Ble_Un_S
                            && instr[i + 8].OpCode.Code == Code.Ldc_I4_1
                            && instr[i + 9].OpCode.Code == Code.Ldc_I4_0
                            && instr[i + 10].IsStloc()
                            && instr[i + 11].IsLdloc()
                            && instr[i + 12].OpCode.Code == Code.Div
                            && instr[i + 13].OpCode.Code == Code.Pop
                            //extra nop here no idea why
                            )
                        {
                            var zeCall = instr[i + 5];
                            if (!IsValid(zeCall))
                                continue;
                            var zeR8 = instr[i + 6];
                            if (!IsValidR8(zeR8))
                                continue;

                            instr[i].OpCode = OpCodes.Nop;
                            instr[i + 1].OpCode = OpCodes.Nop;
                            instr[i + 2].OpCode = OpCodes.Nop;
                            instr[i + 3].OpCode = OpCodes.Nop;
                            instr[i + 4].OpCode = OpCodes.Nop;
                            instr[i + 5].OpCode = OpCodes.Nop;
                            instr[i + 6].OpCode = OpCodes.Nop;
                            instr[i + 7].OpCode = OpCodes.Nop;
                            instr[i + 8].OpCode = OpCodes.Nop;
                            instr[i + 9].OpCode = OpCodes.Nop;
                            instr[i + 10].OpCode = OpCodes.Nop;
                            instr[i + 11].OpCode = OpCodes.Nop;
                            instr[i + 12].OpCode = OpCodes.Nop;
                            instr[i + 13].OpCode = OpCodes.Nop;
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("DebuggerBreakPoint has been deleted.");
        }
        private static bool IsValid(Instruction instr)
        {
            return instr.OpCode.Code == Code.Call && instr.Operand.ToString().Contains("get_TotalMillisecond");
        }
        private static bool IsValidR8(Instruction instr)
        {
            return (double)instr.Operand == 5000.0;
        }
    }
}
