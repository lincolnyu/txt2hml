using System;
using System.IO;
using System.Linq;

namespace TxtToHtmlConverterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir;
            if (args.Length==1)
            {
                dir = args[0];
            }
            else if (args.Length == 0)
            {
                dir = Directory.GetCurrentDirectory();
            }
            else
            {
                Console.WriteLine("Usage: txt2htm.exe [<dir>]");
                return;
            }
            ConvertsAllInDir(dir);
            Console.WriteLine("Done.");
        }

        private static void ConvertsAllInDir(string dir)
        {
            var di = new DirectoryInfo(dir);
            foreach (var f in di.GetFiles().Where(x=>x.Extension.ToLower() == ".txt"))
            {
                var n = Path.GetFileNameWithoutExtension(f.FullName);
                var htmlf = Path.Combine(di.FullName, n + ".html");
                using (var tr = new StreamReader(f.Open(FileMode.Open)))
                using (var tw = new StreamWriter(htmlf))
                {
                    Console.Write("Converting {0}...", f.Name);
                    var res = Convert(tr, tw);
                    Console.WriteLine(res ? "OK" : "Failed");
                }
            }
        }

        private static bool Convert(TextReader tr, TextWriter tw)
        {
            var title = tr.ReadLine();
            if (title == null)
            {
                return false;
            }
            CreateHeader(tw, title);
            var trimmedTitle = title.Trim();
            var firstLines = true;
            while (true)
            {
                var line = tr.ReadLine();
                if (line == null)
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    // all trivial lines are eliminated
                    continue;
                }
                var trimmedLine = line.Trim();
                if (firstLines)
                {
                    if (trimmedLine == trimmedTitle)
                    {
                        continue;
                    }
                    else
                    {
                        WriteTitleInBody(tw, title);
                        firstLines = false;
                    }
                }
                else
                {
                    // assuming each line of the text is a paragraph
                    WriteParagraph(tw, line);
                }
            }
            if (firstLines)
            {
                WriteTitleInBody(tw, title);
            }
            WriteFooter(tw);
            return true;
        }

        private static void CreateHeader(TextWriter tw, string title)
        {
            const string HeaderFormat = "<!DOCTYPE html>\r\n<html>"
                + "\r\n<header>\r\n<meta charset=\"UTF-8\">\r\n"
                + "<title>{0}</title>\r\n</header>\r\n"
                + "<body>\r\n<article>\r\n";
            tw.Write(HeaderFormat, title);
        }

        private static void WriteTitleInBody(TextWriter tw, string title)
        {
            tw.Write("<h1>{0}</h1>\r\n", title);
        }

        private static void WriteParagraph(TextWriter tw, string content)
        {
            tw.Write("<p>{0}</p>\r\n", content);
        }

        private static void WriteFooter(TextWriter tw)
        {
            tw.Write("</article>\r\n</body>\r\n</html>\r\n");
        }
    }
}
