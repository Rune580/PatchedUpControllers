using System.Reflection.Emit;
using HarmonyLib;

namespace PatchedUpControllers.Utils;

internal static class CodeInstructionUtils
{
    public static int FindIndexOfInstruction(this IEnumerable<CodeInstruction> instructions, int matchIndex, OpCode opCode)
    {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

        int match = 0;
        int codeIndex = -1;

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode == opCode)
            {
                if (match == matchIndex)
                {
                    codeIndex = i;
                    break;
                }
                
                match++;
            }
        }

        if (match > matchIndex || codeIndex < 0)
            throw new Exception("Couldn't find match!");

        return codeIndex;
    }
}