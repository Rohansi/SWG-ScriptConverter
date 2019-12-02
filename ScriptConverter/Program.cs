using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptConverter.Ast;
using ScriptConverter.Parser;

namespace ScriptConverter
{
    class Program
    {
        private static Dictionary<string, Document> _astCache = new Dictionary<string, Document>();
        private static string _basePath;

        static void Main(string[] args)
        {
            _basePath = Path.GetFullPath(".").TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;

            var text = File.ReadAllText(@"C:\Users\rohan\Desktop\script\systems\combat\combat_base.script");
            var lexer = new ScriptLexer(text, "combat_base");
            var parser = new ScriptParser(Preprocessor(lexer, "DEBUG"));
            var document = parser.ParseAll();

            using (var output = new StreamWriter("output.java"))
            {
                var printer = new AstPrinter(output, "package", "name", false);
                printer.Visit(document);
            }
            /*
            var scriptPaths =
                Directory.EnumerateFiles(_basePath, "*.script", SearchOption.AllDirectories)
                .Concat(Directory.EnumerateFiles(_basePath, "*.scriptlib", SearchOption.AllDirectories))
                .Select(p => p.Substring(_basePath.Length))
                .Where(p => !p.Contains(".deps"));

            foreach (var path in scriptPaths)
            {
                Console.WriteLine(path);

                var outputPath = Path.GetDirectoryName(path);
                var outputName = Path.GetFileNameWithoutExtension(path);
                var outputFileName = outputName + ".java";
                var isLibrary = Path.GetExtension(path) == ".scriptlib";

                if (outputPath == null)
                    throw new Exception();

                var document = Load(path);

                using (var output = new StreamWriter(Path.Combine(outputPath, outputFileName)))
                {
                    var printer = new AstPrinter(output, outputPath.Replace('\\', '.'), outputName, isLibrary);
                    printer.Visit(document);
                }
            }*/
        }

        static Document Load(string path)
        {
            Document document;
            if (_astCache.TryGetValue(path, out document))
                return document;

            var fullPath = Path.Combine(_basePath, path);
            var source = File.ReadAllText(fullPath);

            var lexer = new ScriptLexer(source, Path.GetFileName(path));
            var parser = new ScriptParser(Preprocessor(lexer, "DEBUG"));

            document = parser.ParseAll();
            //_astCache.Add(path, document);

            return document;
        }

        static IEnumerable<ScriptToken> Preprocessor(IEnumerable<ScriptToken> tokens, params string[] define)
        {
            var defines = new HashSet<string>(define);
            var enumerator = tokens.GetEnumerator();

            var skipping = false;

            while (enumerator.MoveNext())
            {
                var token = enumerator.Current;

                if (skipping && token.Type != ScriptTokenType.PreprocessEndIf)
                    continue;

                switch (token.Type)
                {
                    case ScriptTokenType.PreprocessDefine:
                    {
                        if (!enumerator.MoveNext())
                            throw new Exception("#define has no name");

                        defines.Add(enumerator.Current.Contents);

                        if (!enumerator.MoveNext()) // TODO: this should eat the rest of the line or something
                            throw new Exception("#define has no value");

                        continue;
                    }

                    case ScriptTokenType.PreprocessIfDef:
                    {
                        if (!enumerator.MoveNext())
                            throw new Exception("ifdef has no value");

                        skipping = !defines.Contains(enumerator.Current.Contents);
                        continue;
                    }

                    case ScriptTokenType.PreprocessEndIf:
                    {
                        skipping = false;
                        continue;
                    }
                }

                yield return token;
            }
        }
    }
}
