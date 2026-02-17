namespace PvPBiomeDominions.Helpers
{
    public class SE_PvPSpawnImmunity : StatusEffect
    {
        public override void Setup(Character character)
        {
            base.Setup(character);
            m_name = ConfigurationFile.pvpSpawnProtection.Value;
            m_tooltip = ConfigurationFile.pvpSpawnProtectionDescription.Value;
            m_icon = GameManager.getSprite("pvp_on");
            name = nameof(SE_PvPSpawnImmunity);
        }
    }
}