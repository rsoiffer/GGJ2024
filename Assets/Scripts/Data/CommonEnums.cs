namespace Data
{
    // ReSharper disable InconsistentNaming
    public enum Type
    {
        GRASS,
        POISON,
        FIRE,
        FLYING,
        WATER,
        BUG,
        NORMAL,
        ELECTRIC,
        GROUND,
        FAIRY,
        FIGHTING,
        PSYCHIC,
        ROCK,
        STEEL,
        ICE,
        GHOST,
        DRAGON,
        DARK,
        QMARKS
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

    public enum MoveCategory
    {
        Physical,
        Special,
        Status
    }
}