using confuscsharpnator.Core.Models;
using confuscsharpnator.Core.Passes;
using System.Text;
using System.Text.RegularExpressions;

namespace confuscsharpnator.Core.Tests.Passes
{
    public class StringEncryptionPassTests
    {

        //Happy path: podstawowe szyfrowanie stringów w zmiennych i wywołaniach metod
        [Fact]
        public void Process_ShouldEncryptStringLiterals_AndReplaceWithDecryptCall()
        {


            //ARRANGE
            var pass = new StringEncryptionPass();
            var context = new ObfuscationContext();

            string inputCode = @"
                using System;

                public class AuthService
                {
                    public void Login()
                    {
                        string apiKey = ""SuperSecretKey_2026"";
                        Console.WriteLine(""Autoryzacja udana"");
                    }
                }";



            //ACT
            string resultCode = pass.Process(inputCode, context);



            //ASSERT

            //Nie może być oryginalnych napisów
            Assert.DoesNotContain("SuperSecretKey_2026", resultCode);
            Assert.DoesNotContain("Autoryzacja udana", resultCode);

            //W mijscu napisów ma być wywołanie funckcji dekodującej
            Assert.Contains("_0xDecrypt(\"", resultCode);

            //Struktura klas i zmiennych ma pozostać nienaruszona
            Assert.Contains("string apiKey =", resultCode);
            Assert.Contains("Console.WriteLine", resultCode);

        }




        //Weryfikacja szyfrowania dla polskich znaków UTF-8 i znaków \
        [Fact]
        public void Process_ShouldCorrectlyEncryptAndDecryptUtf8AndEscapeCharacters()
        {

            //ARRANGE
            var pass = new StringEncryptionPass();
            var context = new ObfuscationContext();


            string originalSecret = "Zażółć gęślą jaźń\n\t\"Admin123\"";

            string inputCode = $@"
                class C {{
                    void M() {{
                        string text = ""Zażółć gęślą jaźń\n\t\""Admin123\"""";
                    }}
                }}";


            //ACT
            string resultCode = pass.Process(inputCode, context);

            //ASSERT
            Assert.DoesNotContain("Zażółć", resultCode);

            //Wyciągnij wygenerowany ciąg Base64 z wywołania _0xDecrypt("...")
            var match = Regex.Match(resultCode, @"_0xDecrypt\(""([^""]+)""\)");
            Assert.True(match.Success, "Nie znaleziono wywołania _0xDecrypt w wygenerowanym kodzie.");

            string base64Payload = match.Groups[1].Value;
            string decrypted = SimulateRuntimeDecrypt(base64Payload);

            //Sprawdź, czy odszyfrowany w runtime napis jest w 100% identyczny z oryginałem
            Assert.Equal(originalSecret, decrypted);
        }






        //Puste stringi ("") nie powinny być szyfrowane
        [Fact]
        public void Process_ShouldIgnoreEmptyStringLiterals()
        {
            //ARRANGE
            var pass = new StringEncryptionPass();
            var context = new ObfuscationContext();


            string inputCode = @"
                class C {
                    void M() {
                        string empty = """";
                    }
                }";


            //ACT
            string resultCode = pass.Process(inputCode, context);


            //ASSERT
            Assert.Contains("string empty = \"\";", resultCode);
            Assert.DoesNotContain("_0xDecrypt", resultCode);
        }



        //Inne literały(liczby, char, bool, null) nie mogą zostać ruszone
        [Fact]
        public void Process_ShouldNotEncryptNonStringLiterals()
        {
            //ARRANGE
            var pass = new StringEncryptionPass();
            var context = new ObfuscationContext();

            string inputCode = @"
                class C {
                    void M() {
                        int count = 42;
                        double price = 19.99;
                        char letter = 'A';
                        bool isActive = true;
                        object nothing = null;
                    }
                }";


            //ACT
            string resultCode = pass.Process(inputCode, context);


            //ASSERT
            Assert.DoesNotContain("_0xDecrypt", resultCode);
            Assert.Contains("int count = 42;", resultCode);
            Assert.Contains("double price = 19.99;", resultCode);
            Assert.Contains("char letter = 'A';", resultCode);
            Assert.Contains("bool isActive = true;", resultCode);
            Assert.Contains("object nothing = null;", resultCode);
        }





        //Wiele stringów w jednej linii (np. konkatenacja lub tablica)
        [Fact]
        public void Process_ShouldEncryptMultipleStringsInSingleLine()
        {
            //ARRANGE
            var pass = new StringEncryptionPass();
            var context = new ObfuscationContext();

            string inputCode = @"class C { void M() { string full = ""PartOne"" + ""PartTwo""; } }";


            //ACT
            string resultCode = pass.Process(inputCode, context);


            //ASSERT
            Assert.DoesNotContain("PartOne", resultCode);
            Assert.DoesNotContain("PartTwo", resultCode);

            //Mają pojawić się 2 wywołania Decrypt
            var matches = Regex.Matches(resultCode, @"_0xDecrypt\(""[^""]+""\)");
            Assert.Equal(2, matches.Count);
        }




        //Odporność na pusty ciąg wejściowy, białe znaki oraz null
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Process_ShouldReturnOriginal_WhenInputIsNullOrWhitespace(string? input)
        {
            //ARRANGE
            var pass = new StringEncryptionPass();
            var context = new ObfuscationContext();

            //ACT
            string result = pass.Process(input!, context);

            //ASSERT
            Assert.Equal(input, result);
        }




        //brak context powinien dać ArgumentNullException
        [Fact]
        public void Process_ShouldThrowArgumentNullException_WhenContextIsNull()
        {
            //ARRANGE
            var pass = new StringEncryptionPass();
            string inputCode = @"class C { void M() { string s = ""test""; } }";

            //ACT & ASSERT
            Assert.Throws<ArgumentNullException>(() => pass.Process(inputCode, null!));
        }




        //Metoda pomocnicza symulująca działanie dekodera w runtime - będzie dodana jako osobny pass
        private static string SimulateRuntimeDecrypt(string base64Payload)
        {
            byte[] encryptedBytes = Convert.FromBase64String(base64Payload);
            byte[] decryptedBytes = new byte[encryptedBytes.Length];

            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                byte positionKey = (byte)((i * 7 + 0x42) & 0xFF);
                decryptedBytes[i] = (byte)(encryptedBytes[i] ^ positionKey);
            }

            return Encoding.UTF8.GetString(decryptedBytes);
        }

    }
}
