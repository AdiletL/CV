namespace Gameplay.Skill
{
    public class FireSkill : ISkill
    {
        public float cooldown { get; set; }
        public float countCoolodwn { get; set; }
        
        public void AddSkill(ISkill skill)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveSkill(ISkill skill)
        {
            throw new System.NotImplementedException();
        }

        public void Active()
        {
            throw new System.NotImplementedException();
        }

        public void StartCooldown()
        {
            throw new System.NotImplementedException();
        }

        public void DisplayUI(SkillUI skillUIController)
        {
            throw new System.NotImplementedException();
        }

        public void Upgrade(SkillUpgrade skillUpgrade)
        {
            throw new System.NotImplementedException();
        }
    }
}