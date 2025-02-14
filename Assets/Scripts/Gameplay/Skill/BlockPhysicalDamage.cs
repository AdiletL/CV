using System;

namespace Gameplay.Skill
{
    public class BlockPhysicalDamage : Skill
    {
        public override SkillType SkillType { get; protected set; } = SkillType.blockPhysicalDamage;
        
        public override void Execute(Action exitCallBack = null)
        {
            base.Execute(exitCallBack);
            
        }
    }
}