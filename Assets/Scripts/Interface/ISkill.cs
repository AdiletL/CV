using Gameplay.Skill;

public interface ISkill
{
    public float cooldown { get; }
    public float countCoolodwn { get; }
    
    public void AddSkill(ISkill skill);
    public void RemoveSkill(ISkill skill);
    public void Active();
    public void StartCooldown();
    public void DisplayUI(SkillUI skillUIController);
    public void Upgrade(SkillUpgrade skillUpgrade);
}
