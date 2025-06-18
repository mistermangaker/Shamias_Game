public static class StringFormatter
{
    public static string Personal(SoulGender gender)
    {
        switch (gender)
        {
            case SoulGender.Male:
                return "him";
            case SoulGender.Female:
                return "her";
            default:
            case SoulGender.None:
                return "their";
        }
    }
    public static string Posessive(SoulGender gender)
    {
        switch (gender)
        {
            case SoulGender.Male:
                return "his";
            case SoulGender.Female:
                return "her";
            default:
            case SoulGender.None:
                return "their";
        }
    }
    public static string Pronoun(SoulGender gender)
    {
        switch (gender)
        {
            case SoulGender.Male:
                return "he";
            case SoulGender.Female:
                return "she";
            default:
            case SoulGender.None:
                return "it";
        }         
    }
}
