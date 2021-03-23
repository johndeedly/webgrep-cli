using System;
using System.Collections.Generic;
using System.Linq;
using webgrep;

namespace webgrep_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = args[0];
            string clickBefore = null;

            List<string> param = args.Skip(1).ToList();
            for (int i = 0; i < param.Count; i++)
            {
                if (param[i].StartsWith("--"))
                {
                    var splitted = param[i].Split('=').Select(x => x.Trim('"'));
                    if (splitted.First() == "--click")
                    {
                        clickBefore = splitted.Last();
                    }
                }
            }

            string selector = string.Join(" ", args.Skip(1).SkipWhile(x => x.StartsWith("--")));
            using (FirefoxInstance firefox = new FirefoxInstance())
            {
                if (firefox.NavigateTo(url))
                {
                    if (clickBefore != null)
                    {
                        foreach (var frame in firefox.Frames)
                        {
                            var firstElem = frame.AttachedElements(clickBefore).FirstOrDefault();
                            if (firstElem != null)
                            {
                                firstElem.ClickOn();
                                continue;
                            }
                        }
                        firefox.WaitDomContentLoaded();
                    }
                    
                    foreach (var elem in firefox.AttachedElements(selector).Select(x => x.GetText()))
                    {
                        Console.WriteLine(elem);
                    }
                }
            }
        }
    }
}
