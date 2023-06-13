
using System.Reflection;
using BitCleaner.BreakMe;
using BitCleaner.Calli_to_Call;
using BitCleaner.DotNet_Release;
using BitCleaner.String_Decryption;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

string filePath;
try
{
    filePath = args[0];
}
catch
{
    Console.WriteLine("Enter file path:");
    filePath = Console.ReadLine();
}


var module = ModuleDefMD.Load(filePath, new ModuleCreationOptions(CLRRuntimeReaderKind.Mono));
var asm = Assembly.LoadFile(filePath);

StringDecryptor.Deobfuscate(module, asm); //Decrypt Strings
Unhooker.Deobfuscate(module,asm); //Unhook the hooker
CalliToCall.Deobfuscate(module,asm); //Convert Calli to Call
DebuggerBreakPointEnabler.Deobfuscate(module,asm); //Delete Anti Debugger Breakpoint thing

string outputPath = Path.GetFileNameWithoutExtension(args[0]) + "-bitCleaned" + Path.GetExtension(args[0]);
var writer = new ModuleWriterOptions(module);
writer.Logger = DummyLogger.NoThrowInstance;
module.Write(outputPath,writer);
Console.ReadKey();