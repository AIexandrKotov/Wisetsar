using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KCore;
using SLThree;
using SLThree.Embedding;

namespace Wisetsar
{
    public class Program
    {
        private static string GetResource(string path)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
            {
                using (var tr = new StreamReader(stream))
                {
                    return tr.ReadToEnd();
                }
            }
        }

        private static string GetInitialCode() => GetResource("Wisetsar.data.init.slt");

        private static void Init()
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            ScriptLayout.Execute(GetInitialCode());
            GetResource("Wisetsar.data.quests.slt").RunScript(ExecutionContext.global.pred);
            Stage.Quest = ExecutionContext.global.pred.LocalVariables
                .GetAsDictionary().Values
                .OfType<Stage>()
                .ToDictionary(x => x.Id, x => x);
        }

        public static void Main(string[] args)
        {
            Init();
            Console.Title = "Мудрость царя";
            Terminal.Init(100, 30);
            new MainMenu().Start();
            Terminal.Abort();
        }
    }
}
