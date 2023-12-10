using KCore;
using KCore.Graphics;
using KCore.Graphics.Containers;
using KCore.Graphics.Widgets;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Wisetsar
{
    public class Scores : Form
    {
        public const string ScoresPath = "scores.dat";

        public Trigger CloseTrigger;
        public Trigger DeleteTrigger;
        public TriggerRedrawer EndsRedrawer;

        public Player[] ScoreList;

        public ListBox List;

        public void DeleteAt(int index)
        {
            ScoreList = ScoreList.Where((x, i) => i != index).ToArray();
            using (var stream = File.Create(ScoresPath))
            {
                using (var bw = new BinaryWriter(stream))
                {
                    foreach (var x in ScoreList.Reverse())
                        x.Write(bw);
                }
            }
            if (ScoreList.Length > 0)
            {
                if (List.Position > 0) List.Position -= 1;
                if (List.UpPosition > 0) List.UpPosition -= 1;
            }
            UpdateListChilds();
            List.Resize();
            List.Redraw();
            EndsRedrawer.Do();
        }

        public void UpdateListChilds()
        {
            List.Childs = ScoreList.Select(x => new ListBox.ListItem(new TextWidget(GetString(x), alignment: Alignment.LeftWidth | Alignment.UpHeight, fillHeight: false))).ToList();
        }

        public static string GetString(Player player)
        {
            var sb = new StringBuilder();
            var stage = Stage.Quest[player.CurrentStage];

            sb.Append(player.PlayedDate.ToString("dd.MM.yyyy HH:mm "));
            if (stage.Win) sb.Append("%=>Magenta%Победа%=>reset% ");
            else if (stage.Death) sb.Append($"%=>Red%{stage.DeathName}%=>reset% ");
            sb.Append($"%=>Blue%{player.Piety}%=>reset%");

            return sb.ToString();
        }

        public Scores()
        {
            if (File.Exists(ScoresPath))
            {
                using (var stream = File.OpenRead(ScoresPath))
                {
                    using (var br = new BinaryReader(stream))
                    {
                        ScoreList = Player.ReadPlayers(br, br.BaseStream.Length / 24);
                        Array.Reverse(ScoreList);
                    }
                }
            }
            else ScoreList = new Player[0];

            Bind(CloseTrigger = new Trigger(this, form => form.Close()));
            Bind(DeleteTrigger = new Trigger(this, form => (form as Scores).DeleteAt((form as Scores).List.Position)));
            Bind(EndsRedrawer = new TriggerRedrawer(this, form => (form as Scores).RedrawEnds()));

            List = new ListBox(selectLocation: ListBox.Location.Left);
            List.Container = new DynamicContainer()
            {
                GetLeft = () => 1,
                GetTop = () => 1,
                GetWidth = () => Terminal.FixedWindowWidth / 2,
                GetHeight = () => Terminal.FixedWindowHeight - 2
            };
            List.SelectingPadding = (1, 0);
            UpdateListChilds();

            Root.AddWidget(List);

            List.Resize();

            ActiveWidget = List;
        }

        public void RedrawEnds()
        {
            var (left, top) = (Terminal.FixedWindowWidth / 2 + 10, 1);
            Terminal.Set(left, top);
            $"Открытые концовки: ".PrintSuperText(null, TextAlignment.Center);
            var deaths = Stage.Quest.Values.Where(x => x.Death || x.Win).ToDictionary(x => x.Id, x => x.Win ? "Победа" : x.DeathName).ToArray();
            for (var i = 0; i < deaths.Length; i++)
            {
                Terminal.Set(left, top + 1 + i);
                var text = deaths[i].Value;
                var wrap = ScoreList.Any(x => x.CurrentStage == deaths[i].Key)
                    ? (text == "Победа" ? $"%=>Magenta%{text}%=>reset%" : $"%=>Red%{text}%=>reset%")
                    : $"%=>Gray%{new string('?', text.Length)}%=>reset%";
                wrap.PrintSuperText(null, TextAlignment.Center);
            }
        }

        protected override void OnAllRedraw()
        {
            Terminal.Set(Terminal.FixedWindowWidth / 2 + 10, Terminal.FixedWindowHeight - 3);
            $"%=>Gray back% Delete %=>reset back% Удалить рекорд".PrintSuperText(null, TextAlignment.Center);
        }

        protected override void OnKeyDown(byte key)
        {
            base.OnKeyDown(key);
            if (key == Key.Escape || key == Key.Tab || key == Key.Backspace) CloseTrigger.Do();
            if (key == Key.Delete) DeleteTrigger.Do();
        }
    }
}
