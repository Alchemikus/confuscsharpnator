using confuscsharpnator.Core.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace confuscsharpnator.Core.Passes
{

    //wyszukiwanie, podmienianie nazw zmiennych w drzewie AST
    //Dziedziczy po CSharpSyntaxRewriter - modyfikacja węzłów drzewa.

    public sealed class VariableRenamingRewriter: CSharpSyntaxRewriter
    {
        
        private readonly ObfuscationContext _context;

        //Licznik dla unikalnych nazw
        private int _counter = 0;


        public VariableRenamingRewriter(ObfuscationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        // Szukaj nazwy w słowniku lub utórz nowy identyfikator
        private string GetOrCreateNewName(string originalName)
        {

            //Szukaj w słowniku
            if(_context.RenamedSymbols.TryGetValue(originalName, out var existingName))
            {
                return existingName;
            }

            //Nowa nazwa
            _counter++;
            string newName = $"{_context.RenamingPrefix}{_counter:X4}";

            //Dodaj nową pare do słownika
            _context.RenamedSymbols[originalName] = newName;

            return newName;
        }
        


        //Znajdź deklaracje zmiennych i pól
        // Podmień token nazwy, przekaż zmodyfikowany węzeł

        public override SyntaxNode? VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            string oldName = node.Identifier.ValueText;

            if (!string.IsNullOrWhiteSpace(oldName))
            {
                //Stwóz nowy identyfikator
                string newName = GetOrCreateNewName(oldName);

                //Utwórz nowy węzeł 
                var updatedNode = node.WithIdentifier(SyntaxFactory.Identifier(newName));

                return base.VisitVariableDeclarator(updatedNode);
            }

            return base.VisitVariableDeclarator(node);
        }



        //Znajdź każde uzycie nazwy w kodzie
        //Podmień nazwe tylko gdy znajdziesz w słowniku

        public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
        {

            string currentName = node.Identifier.ValueText;


            //Sprawdz czy identyfikator jest w słowniku
            if (_context.RenamedSymbols.TryGetValue(currentName, out var newName))
            {
                //Nowy token nazwy, zachowaj Trivia (odstępy, wcięcia, komentarze otaczające token)
                var newIdentifier = SyntaxFactory.Identifier(
                    node.Identifier.LeadingTrivia,
                    newName,
                    node.Identifier.TrailingTrivia);

            }

            //Identyfikatora nie było w dict - pozostaw bez zmian
            return base.VisitIdentifierName(node);
        }


    }
}







