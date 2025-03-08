using System;
using System.Collections.Generic;

namespace Gameplay.Unit.Character.Player
{
    public class PlayerBlockInput
    {
        private Dictionary<InputType, int> blockedInputs;
        
        public bool IsInputBlocked(InputType input)
        {
            if (blockedInputs == null) return false;
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                if (blockedInputs.ContainsKey(flag) && blockedInputs[flag] > 0)
                    return true;
            }
            return false;
        }
        
        public void BlockInput(InputType input)
        {
            blockedInputs ??= new();
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                blockedInputs.TryAdd(flag, 0);
                blockedInputs[flag]++;
            }
        }
        
        public void UnblockInput(InputType input)
        {
            if(blockedInputs == null) return;
            foreach (InputType flag in Enum.GetValues(typeof(InputType)))
            {
                if (flag == InputType.Nothing || (input & flag) == 0 || 
                    flag == InputType.Everything) continue;

                if (blockedInputs.ContainsKey(flag))
                {
                    blockedInputs[flag]--;

                    if (blockedInputs[flag] <= 0) 
                        blockedInputs.Remove(flag);
                }
            }
        }
        
        public void Initialize()
        {
            
        }
    }
}