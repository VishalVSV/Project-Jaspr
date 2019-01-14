using Project_Jaspr.Utils;
using Project_Jaspr.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Jaspr
{
    //nasm -fwin32 hello.asm - ASSEMBLING
    //ld -o hello.exe hello.obj -e_main -L "C:\Windows\System32" -lkernel32 --enable-stdcall-fixup "C:\Windows\System32\kernel32.dll" - LINKING

    public static class Compiler
    {
        public static void ExpCompile(AbstractSyntaxTree ast, string outPath)
        {
            string asm = "";
            using (StreamReader sr = new StreamReader("BasicAsm.asm"))
            {
                asm = sr.ReadToEnd();
            }
            //asm += "extern _ExitProcess@4\nglobal main\nsection .text" + "\n\n";

            asm += ast.root.EvaluateAsm();

            asm += "section .data\n\n";
            int i = 0;
            while (i < asm.Length)
            {
                if (asm.Substring(i, 1) == "{")
                {
                    string data = "";
                    if (asm.IndexOf("}", i) != -1)
                    {
                        data = asm.Substring(i+1, asm.IndexOf("}",i) - i -1);
                    }
                    asm = asm.Remove(i, asm.IndexOf("}", i) - i + 1);
                    asm += data + "\n";
                }
                i++;
            }

            File.Create(outPath + ".asm").Close();
            using (StreamWriter sr = new StreamWriter(outPath + ".asm"))
            {
                sr.Write(asm);
            }
            ProcessStartInfo si = new ProcessStartInfo("nasm.exe", "-fwin32 " + outPath + ".asm");
            si.CreateNoWindow = true;
            Process.Start(si).WaitForExit();
            si = new ProcessStartInfo("ld.exe", "-o" + outPath + ".exe " + outPath + ".obj -emain");
            si.CreateNoWindow = true;
            Process.Start(si).WaitForExit();
            //File.Delete(outPath + ".asm");
            //File.Delete(outPath + ".obj");
        }

        public static void Compile(AbstractSyntaxTree ast, string outPath)
        {
            string asm = "";

            asm += @"extern _ExitProcess@4
global main
section .text" + "\n";



            int j = -1;
            for (int i = 0; i < ast.root.branches.Count; i++)
            {
                if (((Function)ast.root.branches[i]).Name == "main")
                {
                    j = i;
                    break;
                }
            }

            foreach (Function function in ast.root.branches)
            {
                asm += function.Name + ":" + "\n";
                foreach (Statement statement in function.branches)
                {
                    asm += statement.Evaluate() + "\n";
                }
            }
            File.Create(outPath + ".asm").Close();

            using (StreamWriter sw = new StreamWriter(outPath + ".asm"))
            {
                sw.Write(asm);
            }

            Process.Start("nasm.exe", "-fwin32 " + outPath + ".asm");
            Process.Start("ld.exe", "-o" + outPath + ".exe " + outPath + ".obj -emain -L \"C:\\Windows\\System32\" -lkernel32 --enable-stdcall-fixup \"C:\\Windows\\System32\\kernel32.dll\"");
        }
    }
}
