using Xunit;
using confuscsharpnator.Core.Models;
using confuscsharpnator.Core.Passes;

namespace confuscsharpnator.Core.Tests.Passes
{


    // Testy jednostkowe weryfikujące poprawność działania modułu zmiany nazw zmiennych.


    public class VariableRenamingPassTests
    {

        //Happy path

        [Fact]
        public void Process_ShouldRenameLocalVariables_AndPreserveSystemCalls()
        {


            //ARRANGE

            var pass = new VariableRenamingPass();  
            var context = new ObfuscationContext();


            //Przykładowy kod C# przeznaczony do obfuskacji
            string inputCode = @"
                using System;

                public class Sample
                {
                    public void Calculate()
                    {
                        int firstNumber = 5;
                        int secondNumber = 10;
                        int totalSum = firstNumber + secondNumber;
                        Console.WriteLine(totalSum);
                    }
                }";
            



            //ACT
            string resultCode = pass.Process(inputCode, context);



            //ASSERT

            //Sprawdź czy oryginalne nazwy smiennych zniknęły z kodu
            Assert.DoesNotContain("firstNumber", resultCode);
            Assert.DoesNotContain("secondNumber", resultCode);
            Assert.DoesNotContain("totalSum", resultCode);

            //Sprawdź czy zastosowano nowe nazwy zmiennych zgodnie z prefiksem
            Assert.Contains("_0x0001", resultCode);
            Assert.Contains("_0x0002", resultCode);
            Assert.Contains("_0x0003", resultCode);

            //Sprawdź czy nazwy typów i metod systemowych pozostały nienaruszone
            Assert.Contains("Console.WriteLine", resultCode);
            Assert.Contains("int", resultCode);

            //Sprawdź czy w context pojawiły się 3 nowe nazwy
            Assert.Equal(3, context.ProcessedSymbolsCount);
        }
    }
}
