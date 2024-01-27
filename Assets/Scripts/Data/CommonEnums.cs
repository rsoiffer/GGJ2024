namespace Data
{
    // ReSharper disable InconsistentNaming
    public enum Type
    {
        NORMAL,
        FIGHTING,
        FLYING,
        POISON,
        GROUND,
        ROCK,
        BUG,
        GHOST,
        STEEL,
        QMARKS,
        FIRE,
        WATER,
        GRASS,
        ELECTRIC,
        PSYCHIC,
        ICE,
        DRAGON,
        DARK,
        FAIRY
    }

    public enum Stat
    {
        HP,
        ATK,
        DEF,
        SPEED,
        SPATK,
        SPDEF
    }

    public enum GenderRatio
    {
        FemaleOneEighth,
        Female50Percent,
        AlwaysFemale,
        AlwaysMale,
        Female75Percent,
        Female25Percent,
        Genderless,
        FemaleSevenEighths
    }

    public enum GrowthRate
    {
        Parabolic,
        Medium,
        Fast,
        Slow,
        Fluctuating,
        Erratic
    }
}