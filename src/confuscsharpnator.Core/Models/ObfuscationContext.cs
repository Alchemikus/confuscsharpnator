namespace confuscsharpnator.Core.Models
{

    //Stan obfuskacji
    public sealed class ObfuscationContext
    {

        //Słownik old_ane -> new_name
        public Dictionary<string, string> RenamedSymbols { get; } = new();


        //Prefix do generowania nazw zmiannych
        public string RenamingPrefix { get; set; } = "_0x";

        //Oblicz ilość zmienonych nazw
        public int ProcessedSymbolsCount => RenamedSymbols.Count;
    }

}
