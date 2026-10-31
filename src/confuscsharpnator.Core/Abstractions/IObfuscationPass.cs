using confuscsharpnator.Core.Models;

namespace confuscsharpnator.Core.Abstractions
{
    //Interface for all obfuscation steps (passes)
    //Interfejs dla każdego passu (etapu) obfuskacji
    public interface IObfuscationPass
    {

        //Nazwa kroku
        string Name { get; }


        //Transformacja / obfuskacja przekazanego kodu
        string Process(string sourceCode, ObfuscationContext context);
    }
}
