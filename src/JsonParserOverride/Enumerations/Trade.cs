using System;

namespace JsonParserOverride.Enumerations
{
    [Flags]
    public enum Trade
    {
        Gutters = 1,
        WindowBeading = 2,
        WindowScreens = 4,
        WindowReglazing = 8,
        Painting = 16
    }
}
