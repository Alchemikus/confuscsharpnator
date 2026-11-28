using confuscsharpnator.Core.Models;
using confuscsharpnator.Core.Passes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace confuscsharpnator.Core.Tests.Passes
{
    public class ArithmeticObfuscationPassTests
    {


        //context = null 
        [Fact]
        public void Process_NullContext_ThrowsArgumentNullException()
        {
            var pass = new ArithmeticObfuscationPass(seed: 42);

            Assert.Throws<ArgumentNullException>(() => pass.Process("class C { int x = 1; }", null!));
        }



        //Pusty source
        [Theory]
        [InlineData("")]
        [InlineData("   \r\n\t  ")]
        public void Process_EmptyOrWhitespaceSource_ReturnsUnchanged(string source)
        {
            var pass = new ArithmeticObfuscationPass(seed: 42);
            var context = new ObfuscationContext();

            string result = pass.Process(source, context);

            Assert.Equal(source, result);
        }



        //Ten sam seed daje ten sam output
        [Fact]
        public void Process_SameSeed_ProducesDeterministicOutput()
        {

            //ARRANGE
            const string source = @"
                public class Sample
                {
                    public int Calc(int a, int b) => (a + b) - (a * 4) + 1234;
                }";


            //ACT
            var pass1 = new ArithmeticObfuscationPass(seed: 100);
            var pass2 = new ArithmeticObfuscationPass(seed: 100);

            string result1 = pass1.Process(source, new ObfuscationContext());
            string result2 = pass2.Process(source, new ObfuscationContext());


            //ASSERT
            Assert.Equal(result1, result2);
        }






        //Inny seed daje inne warianty MBA
        [Fact]
        public void Process_DifferentSeeds_ProducePolymorphicMbaVariants()
        {
            
            //ARRANGE
            const string source = @"
                public class Sample
                {
                    public int Calc(int a, int b) => (a + b) - (b - a);
                }";


            //ACT

            //Wersje z różnym seed
            var outputs = Enumerable.Range(1, 10)
                .Select(seed => new ArithmeticObfuscationPass(seed).Process(source, new ObfuscationContext()))
                .Distinct()
                .ToList();



            //ASSERT
            Assert.True(outputs.Count > 1, "Oczekiwano różnych wariantów formuł MBA dla innych seed.");
        }



        //Sprawdzenie czy literały numeryczne są maskowane
        [Fact]
        public void Process_NumericLiterals_AreHiddenInGeneratedCode()
        {

            //ARRANGE
            const string source = @"
                public class ConstantsHolder
                {
                    public int GetInt() => 1337;
                    public long GetLong() => 9876543210L;
                    public double GetDouble() => 3.14159265;
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 42);
            string obfuscated = pass.Process(source, new ObfuscationContext());


            //ASSERT

            //Brak wartości numerycznych
            Assert.DoesNotContain("1337", obfuscated);
            Assert.DoesNotContain("9876543210L", obfuscated);
            Assert.DoesNotContain("3.14159265", obfuscated);


            //Po kompilacji w pamięci metody muszą zwracać wartości pierwotne
            Assert.Equal(1337, CompileAndInvoke(obfuscated, "ConstantsHolder", "GetInt"));
            Assert.Equal(9876543210L, CompileAndInvoke(obfuscated, "ConstantsHolder", "GetLong"));
            Assert.Equal(3.14159265, CompileAndInvoke(obfuscated, "ConstantsHolder", "GetDouble"));
        }





        //Każdy wariant MBA daje ten sam rezultat
        //Kilka wartości seed wymusza wylosowanie każdego z wariantów MBA
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(15)]
        [InlineData(42)]
        public void Process_IntegerAddAndSubtract_AllMbaVariantsReturnExactResults(int seed)
        {
            //ARRANGE
            const string source = @"
                public class MathOps
                {
                    public int Add(int a, int b) => a + b;
                    public int Sub(int a, int b) => a - b;
                    public long AddLong(long a, long b) => a + b;
                    public long SubLong(long a, long b) => a - b;
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed);
            string obfuscated = pass.Process(source, new ObfuscationContext());
            Assembly asm = CompileToAssembly(obfuscated);


            //ASSERT

            //Spardź liczby dodatnie, ujemne, zero, wartości brzegowe typów
            (int a, int b)[] intCases =
            {
                (0, 0),
                (15, 27),
                (-100, 45),
                (-50, -75),
                (int.MaxValue, 0),
                (int.MinValue, 0),
                (int.MaxValue, -1),
                (int.MinValue, 1),
                (int.MaxValue, 10) 
            };

            foreach (var (a, b) in intCases)
            {
                Assert.Equal(unchecked(a + b), Invoke(asm, "MathOps", "Add", a, b));
                Assert.Equal(unchecked(a - b), Invoke(asm, "MathOps", "Sub", a, b));
            }

            Assert.Equal(long.MaxValue - 123L, Invoke(asm, "MathOps", "SubLong", long.MaxValue, 123L));
            Assert.Equal(long.MinValue + 123L, Invoke(asm, "MathOps", "AddLong", long.MinValue, 123L));
        }



        //Sprawdzenie operacji mnożenia, potęgowania
        [Fact]
        public void Process_IntegerMultiplication_HandlesPowersOfTwoAndRegularNumbers()
        {

            //ARRANGE
            const string source = @"
                public class MulOps
                {
                    public int MulByPowerOfTwo(int x) => x * 8;
                    public int MulByRegularNumber(int x) => x * 3;
                    public int MulByZero(int x) => x * 0;
                    public int MulByOne(int x) => x * 1;
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 12);
            string obfuscated = pass.Process(source, new ObfuscationContext());


            //ASSERT

            //Potęga dwójki (8 = 2^3) ma zostać zamieniona na przesunięcie bitowe
            Assert.Contains("<< 3", obfuscated);

            Assembly asm = CompileToAssembly(obfuscated);

            foreach (int x in new[] { -15, -1, 0, 1, 7, 1024 })
            {
                Assert.Equal(x * 8, Invoke(asm, "MulOps", "MulByPowerOfTwo", x));
                Assert.Equal(x * 3, Invoke(asm, "MulOps", "MulByRegularNumber", x));
                Assert.Equal(0, Invoke(asm, "MulOps", "MulByZero", x));
                Assert.Equal(x, Invoke(asm, "MulOps", "MulByOne", x));
            }
        }




        //Spradzenie maskowania operatora ''=='' na XOR
        [Fact]
        public void Process_IntegerEqualityComparisons_TransformsToXorZeroCheckAndPreservesLogic()
        {
            //ARRANGE
            const string source = @"
                public class ComparisonOps
                {
                    public bool AreEqual(int a, int b) => a == b;
                    public bool AreNotEqual(int a, int b) => a != b;
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 99);
            string obfuscated = pass.Process(source, new ObfuscationContext());


            //ASSERT
            Assert.Contains("^", obfuscated);
            Assert.Contains("== 0", obfuscated);
            Assert.Contains("!= 0", obfuscated);

            Assembly asm = CompileToAssembly(obfuscated);

            Assert.Equal(true, Invoke(asm, "ComparisonOps", "AreEqual", -42, -42));
            Assert.Equal(false, Invoke(asm, "ComparisonOps", "AreEqual", -42, 42));
            Assert.Equal(true, Invoke(asm, "ComparisonOps", "AreNotEqual", 10, 20));
            Assert.Equal(false, Invoke(asm, "ComparisonOps", "AreNotEqual", 0, 0));
        }



        // Weryfikacja, czy dodanie nawiasów chroni przed błędnym priorytetem operatorów bitowych (^, &, |)
        // względem mnożenia, dodawania i porównań
        [Fact]
        public void Process_DeeplyNestedExpressions_PreservesOperatorPrecedence()
        {

            //ARRANGE
            const string source = @"
                public class TestClass
                {
                    public int ComplexFormula(int a, int b, int c)
                    {
                        return ((a + b) * 4 - (c + 10)) + (a - b) * 2;
                    }

                    public bool ComplexCondition(int a, int b)
                    {
                        return (a + 5) == (b - 3);
                    }
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 55);
            string obfuscated = pass.Process(source, new ObfuscationContext());
            Assembly asm = CompileToAssembly(obfuscated);


            //ASSERT
            int expectedFormula = ((7 + 3) * 4 - (2 + 10)) + (7 - 3) * 2;
            Assert.Equal(expectedFormula, Invoke(asm, "TestClass", "ComplexFormula", 7, 3, 2));
            Assert.Equal(true, Invoke(asm, "TestClass", "ComplexCondition", 10, 18));
            Assert.Equal(false, Invoke(asm, "TestClass", "ComplexCondition", 10, 19));
        }





        //Sprawdzenie maskowania liczb double
        [Fact]
        public void Process_DoubleArithmeticAndLiterals_PreservesExactBitPrecision()
        {

            //ARRANGE
            const string source = @"
                public class DoubleOps
                {
                    public double Compute(double a, double b)
                    {
                        double sum = a + b;
                        double diff = a - b;
                        double prod = a * b;
                        double quot = a / b;
                        double constant = 0.1234567890123456;
                        return (sum + diff) - (prod / 2.0) + quot + constant;
                    }
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 77);
            string obfuscated = pass.Process(source, new ObfuscationContext());


            //ASSERT

            //Double musi być odtwarzane bezstratnie
            Assert.Contains("System.BitConverter.Int64BitsToDouble", obfuscated);

            //Porównanie wyniku po obfuskacji z wynikiem oryginalnego kodu
            Assembly originalAsm = CompileToAssembly(source);
            Assembly obfuscatedAsm = CompileToAssembly(obfuscated);

            double a = 12.3456789012345;
            double b = -0.000987654321;

            double expected = (double)Invoke(originalAsm, "DoubleOps", "Compute", a, b)!;
            double actual = (double)Invoke(obfuscatedAsm, "DoubleOps", "Compute", a, b)!;

            //Wartości muszą być odtworzone bezstratnie
            Assert.Equal(expected, actual);
        }




        //Sprawdzenie stałych
        [Fact]
        public void Process_CompileTimeConstantContexts_AreNotBrokenAndCompileCleanly()
        {
            //ARRANGE
            const string source = @"
                using System;

                public enum StatusCode
                {
                    Ok = 1 + 1,
                    Error = 10 + 5
                }   

                public class ConstantContextSample
                {
                    private const int ClassConstInt = 10 + 20;
                    private const double ClassConstDouble = 3.14;

                    [Obsolete(""Wersja 1"", false)]
                    public int Evaluate(int input = 5 + 5)
                    {
                        const int LocalConstInt = 2 * 3;
                        const double LocalConstDouble = 1.5;

                        int bonus = input switch
                        {
                            10 => 100,
                            _ => 0
                        };

                        switch (input)
                        {
                            case 20:
                                bonus += 200;
                                break;
                        }

                        return input + ClassConstInt + LocalConstInt + bonus + (int)(ClassConstDouble + LocalConstDouble);
                    }
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 33);
            string obfuscated = pass.Process(source, new ObfuscationContext());



            //ASSERT

            Assembly asm = CompileToAssembly(obfuscated);

            //Dla input = 10: 10 + 30 + 6 + 100 + (int)(4.64) = 150
            Assert.Equal(150, Invoke(asm, "ConstantContextSample", "Evaluate", 10));
        }



        //Wzory MBA duplikują operandy — jeśli w wyrażeniu jest i++ lub wywołanie metody,
        //Rewriter nie może zastosować MBA, aby nie wykonać operacji dwukrotnie.
        [Fact]
        public void Process_SideEffectsInOperands_AreEvaluatedOnce()
        {
            //ARRANGE
            const string source = @"
                public class SideEffectSample
                {
                    private int _counter = 0;

                    private int IncrementAndGet()
                    {
                        _counter++;
                        return 10;
                    }

                    public int Run(int x)
                    {
                        int a = x++ + 5;               // ++ po lewej stronie
                        int b = 5 + ++x;               // ++ po prawej stronie
                        int c = IncrementAndGet() + 20; // wywołanie metody modyfikującej stan
                        int d = (x = 100) + 1;         // przypisanie wewnątrz wyrażenia

                        return a + b + c + d + (_counter * 1000) + x;
                    }
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 88);
            string obfuscated = pass.Process(source, new ObfuscationContext());
            Assembly asm = CompileToAssembly(obfuscated);


            //ASSERT
            int expected = 6 + 8 + 30 + 101 + 1000 + 100; // 1245
            Assert.Equal(expected, Invoke(asm, "SideEffectSample", "Run", 1));
        }



        // Weryfikacja, że:
        // 1. Konkatenacja stringów ("a" + "b") nie jest mylona z dodawaniem liczb
        // 2. Mniejsze typy (byte, short) nie powodują błędów 
        // 3. Typy float i decimal przechodzą 
        [Fact]
        public void Process_NonTargetTypesAndStringConcatenation_CompileAndBehaveCorrectly()
        {
            //ARRANGE
            const string source = @"
                public class MixedTypesSample
                {
                    public string FormatMessage(int value)
                    {
                        return ""Wynik: "" + (value + 10) + "" pkt"";
                    }

                    public int ComputeOtherPrimitives()
                    {
                        byte b1 = 10, b2 = 20;
                        byte bSum = (byte)(b1 + b2);

                        short s1 = 100, s2 = 40;
                        short sDiff = (short)(s1 - s2);

                        float f = 2.5f + 1.5f;
                        decimal dec = 10.5m + 4.5m;

                        return bSum + sDiff + (int)f + (int)dec;
                    }
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 11);
            string obfuscated = pass.Process(source, new ObfuscationContext());
            Assembly asm = CompileToAssembly(obfuscated);


            //ASSERT
            Assert.Equal("Wynik: 15 pkt", Invoke(asm, "MixedTypesSample", "FormatMessage", 5));
            Assert.Equal(30 + 60 + 4 + 15, Invoke(asm, "MixedTypesSample", "ComputeOtherPrimitives"));
        }



        //Sprawdzenie czy zachowano komentarze
        [Fact]
        public void Process_PreservesLeadingAndTrailingTriviaComments()
        {
            //ARRANGE
            const string source = @"
                public class CommentSample
                {
                    public int Calc(int a, int b)
                    {
                        // Komentarz przed obliczeniem
                        int res = a + b; /* komentarz za obliczeniem */
                        return res;
                    }
                }";


            //ACT
            var pass = new ArithmeticObfuscationPass(seed: 42);
            string obfuscated = pass.Process(source, new ObfuscationContext());


            //ASSERT
            Assert.Contains("// Komentarz przed obliczeniem", obfuscated);
            Assert.Contains("/* komentarz za obliczeniem */", obfuscated);
        }


        /// <summary>
        /// Kompiluje podany kod źródłowy C# w pamięci RAM.
        /// W przypadku błędów kompilacji rzuca wyjątek z listą błędów.
        /// </summary>

        private static Assembly CompileToAssembly(string sourceCode)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            //Aby kompilator rozpoznał typy wbudowane
            MetadataReference[] references =
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(typeof(BitConverter).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ObsoleteAttribute).Assembly.Location)
            };


            var compilation = CSharpCompilation.Create(
                assemblyName: $"ObfuscatedTestAssembly_{Guid.NewGuid():N}",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                string errors = string.Join(
                    Environment.NewLine,
                    emitResult.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .Select(d => $"{d.Id}: {d.GetMessage()} (line {d.Location.GetLineSpan().StartLinePosition.Line + 1})"));

                Assert.Fail($"Wygenerowany kod nie skompilował się:" +
                    $"{Environment.NewLine}{errors}" +
                    $"{Environment.NewLine}{Environment.NewLine} Kod:{Environment.NewLine}{sourceCode}");
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }




        /// <summary>
        /// Kompiluje kod w pamięci, tworzy instancję klasy i wywołuje wskazaną metodę.
        /// </summary>
        private static object? CompileAndInvoke(string sourceCode, string typeName, string methodName, params object?[] args)
        {
            Assembly assembly = CompileToAssembly(sourceCode);
            return Invoke(assembly, typeName, methodName, args);
        }




        /// <summary>
        /// Tworzy instancję klasy z załadowanego Assembly i wywołuje na niej metodę.
        /// </summary>
        private static object? Invoke(Assembly assembly, string typeName, string methodName, params object?[] args)
        {
            Type? type = assembly.GetType(typeName);
            Assert.NotNull(type);

            object? instance = Activator.CreateInstance(type);
            Assert.NotNull(instance);

            MethodInfo? method = type.GetMethod(methodName);
            Assert.NotNull(method);

            return method.Invoke(instance, args);
        }
    }
}
