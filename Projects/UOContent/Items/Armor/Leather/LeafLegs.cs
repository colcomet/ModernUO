using ModernUO.Serialization;

namespace Server.Items
{
    [SerializationGenerator(0)]
    [Flippable(0x2FC9, 0x317F)]
    public partial class LeafLegs : BaseArmor
    {
        [Constructible]
        public LeafLegs() : base(0x2FC9)
        {
        }

        public override double DefaultWeight => 2.0;

        public override int RequiredRaces => Race.AllowElvesOnly;

        public override int BasePhysicalResistance => 2;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 2;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 4;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 40;

        public override int AosStrReq => 20;
        public override int OldStrReq => 20;

        public override int ArmorBase => 13;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Leather;
        public override CraftResource DefaultResource => CraftResource.RegularLeather;

        public override ArmorMeditationAllowance DefMedAllowance => ArmorMeditationAllowance.All;
    }
}
