using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace confuscsharpnator.Core.Passes
{

    /// <summary>
    /// Klasa reprezentująca pojedynczy pass (etap potoku) odpowiedzialny za szyfrowanie napisów.
    /// 
    /// 
    /// Rewriter szuka w drzewie AST wyłącznie węzłów typu SyntaxKind.StringLiteralExpression, czyli np
    /// - Napisy informacyjne
    /// - Adresy URL i ścieżki
    /// - Zapytania SQL / Formaty
    /// - Klucze i sekrety
    /// 
    /// </summary>


    public sealed class StringEncryptionRewriter: CSharpSyntaxRewriter
    {

        private readonly ObfuscationContext _context;

        public StringEncryptionRewriter(ObfuscationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }




        /// <summary>
        /// Metoda wywoływana automatycznie przez Roslyna dla każdego wyrażenia stałego (literal) w kodzie C#
        /// (np. 123, bool true/false, string "abc").
        /// </summary>
        /// 

        public override SyntaxNode? VisitLiteralExpression(LiteralExpressionSyntax node)
        {

            // Sprawdź, czy konkretny literal w kodzie jest ciągiem znaków (string).
            // Nie biore pod uwage liczb (int, double) czy wartości bool, obsługując tylko SyntaxKind.StringLiteralExpression.
            if (node.IsKind(SyntaxKind.StringLiteralExpression))
            {
                //Usuń nawiasy "" ze stringa
                string rawString = node.Token.ValueText;

                //Pozbądź sie pustych stringów - ""
                if (string.IsNullOrEmpty(rawString))
                {
                    return base.VisitLiteralExpression(node);
                }


                //Konwersja tekstu na tablice bajtów
                byte[] inputBytes = Encoding.UTF8.GetBytes(rawString);

                //Tablica wynikowa
                byte[] encryptedBytes = new byte[inputBytes.Length];


                //Szyfrowanie pozycyjnym XOR-em.
                //Wzór: (i * 7 + 0x42)
                for(int i = 0; i< inputBytes.Length; i++)
                {
                    //Operacja & 0xFF po to, by wynik mieścił się 0-255
                    byte positionKey = (byte)((i * 7 + 0x42) & 0xFF);

                    //Operacja XOR
                    encryptedBytes[i] = (byte)(inputBytes[i] ^ positionKey);
                }


                // Zmiana na ciąg tekstowy Base64
                string base64Payload = Convert.ToBase64String(encryptedBytes);




                //Konstrukcja nowego węzła AST
                //Podmienia literał na wyrażenie z wywołaniem metody (do odszyfrowania w runtime)
                //Np dla "Witaj" wygeneruje _0xDecrypt("BASE64...")

                var invocationNode = SyntaxFactory.ParseExpression($"_0xDecrypt(\"{base64Payload}\")")

                    //Przekopiuj białe znaki, wcięcia i komentarze otaczające oryginalny napis,
                    .WithLeadingTrivia(node.GetLeadingTrivia())
                    .WithTrailingTrivia(node.GetTrailingTrivia());


                //Zwróć zaktualizowane drzewo
                return invocationNode;
            }

            //Gdy węzeł nie był stringiem, idź dalej
            return base.VisitLiteralExpression(node);
        }
    }
}
