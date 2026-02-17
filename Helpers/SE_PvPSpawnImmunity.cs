namespace PvPBiomeDominions.Helpers
{
    public class SE_PvPSpawnImmunity : StatusEffect
    {
        public static readonly int HASH = 371857150;
        public override void Setup(Character character)
        {
            base.Setup(character);
            m_name = "Spawn Protection";
            m_tooltip = "You are temporarily immune to PvP damage.";
            m_icon = GameManager.getSprite("SoftDeath");
            name = "Spawn Protection";
        }
    }
}