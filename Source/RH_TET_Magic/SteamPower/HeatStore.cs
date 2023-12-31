namespace TheEndTimes_Magic
{
    /**
     * Original code for this came from Dubs Central Heating Mod, and was
     * trimmed down to suit this mod's needs.
     */
    public interface HeatStore
    {
        bool LowCap { get; }

        bool ThermostatControlled { get; }

        float GetStoreCapacity { get; }

        float HeaterUsage { get; set; }

        bool DrawOverlay { get; set; }
    }
}
