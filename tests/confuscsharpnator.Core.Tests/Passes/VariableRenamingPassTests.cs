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





        //Test sprawdzający czy zmieni wszystkie nazwy zmiennych w jednej linii

        [Fact]
        public void Process_ShouldHandleMultipleVariablesInSingleLine()
        {

            //ARRANGE
            var pass = new VariableRenamingPass();
            var context = new ObfuscationContext();

            string input = "class C { void M() { int a = 1, b = 2; int sum = a + b; } }";


            //ACT
            string result = pass.Process(input, context);


            //ASSERT
            Assert.DoesNotContain("a = 1", result);
            Assert.DoesNotContain("int a", result);
            Assert.DoesNotContain("b = 2", result);
            Assert.DoesNotContain("int b", result);
            Assert.DoesNotContain("int sum", result);

            Assert.Contains("_0x0001", result);
            Assert.Contains("_0x0002", result);
            Assert.Contains("_0x0003", result);
        }




        //Sprawdzenie działania przy interpolacji stringów
        [Fact]
        public void Process_ShouldHandleStringInterpolation() 
        { 
            //ARRANGE
            var pass = new VariableRenamingPass();
            var context = new ObfuscationContext();

            string input = @"
                class C { 
                    void M() { 
                        int value = 42; 
                        string msg = $""Number is {value}""; 
                    } 
                }";


            //ACT
            string result = pass.Process(input, context);


            //ASSERT
            Assert.DoesNotContain("value = 42", result);
            Assert.DoesNotContain("int value", result);
            Assert.DoesNotContain("{value}", result);


            Assert.Contains("{_0x0001}", result);
            Assert.Contains("int _0x0001", result);
        }



        //Sprawdzenie w znaku "_" (discard symbol)
        [Fact]
        public void Process_ShouldNotRenameDiscardSymbol()
        {
            //ARRANGE
            var pass = new VariableRenamingPass();
            var context = new ObfuscationContext();

            string input = @"
                class C { 
                    void M() {
                        _ = System.Math.Abs(-5); 
                    } 
                }";


            //ACT
            string result = pass.Process(input, context);


            //ASSERT

            //Sprawdź czy symbol discard "_" otrzymał nową nazwę
            Assert.DoesNotContain("_0x0001", result);
        }



        //Sprawdzenie w przypadku braku deklaracji zmiennych
        [Fact]
        public void Process_ShouldNotRenameWhenNoVariableDeclarations()
        {
            //ARRANGE
            var pass = new VariableRenamingPass();
            var context = new ObfuscationContext();

            string input = @"
                class C { 
                    void M() {
                        Console.WriteLine(""Hello World"");
                    } 
                }";


            //ACT
            string result = pass.Process(input, context);


            //ASSERT

            //Sprawdź czy pojwaiy się nowe nazwy
            Assert.DoesNotContain("_0x0001", result);
        }


    }



}
