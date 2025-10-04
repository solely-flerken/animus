using System.Collections.Generic;
using UnityEngine;

namespace Packages.Animus.Unity.Runtime.Agent
{
    [CreateAssetMenu(fileName = "InstructionRegistry", menuName = "NPC/Instructions/InstructionRegistry")]
    public class InstructionRegistry : ScriptableObject
    {
        [SerializeField] private List<NpcInstruction> instructions;

        private Dictionary<string, NpcInstruction> _instructionMap;

        public void Initialize()
        {
            _instructionMap = new Dictionary<string, NpcInstruction>();

            foreach (var instruction in instructions)
            {
                if (instruction == null || string.IsNullOrEmpty(instruction.instructionKey))
                {
                    continue;
                }

                if (!_instructionMap.TryAdd(instruction.instructionKey, instruction))
                {
                    Debug.LogWarning($"Duplicate instruction key found in registry: '{instruction.instructionKey}'");
                }
            }
        }

        public NpcInstruction GetInstruction(string key)
        {
            _instructionMap.TryGetValue(key, out var instruction);
            return instruction;
        }
    }
}