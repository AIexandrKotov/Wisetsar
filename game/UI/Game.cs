using KCore;
using KCore.Graphics;
using KCore.Graphics.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisetsar
{
    public class Game : Form
    {
        public Trigger CloseTrigger;
        public Trigger EnterTrigger;

        public Window StageTextWindow;
        public ScrollableText StageText;

        public Window VariantsWindow;
        public ListBox Variants;

        public Window PietyWindow;
        public TextWidget Piety;

        #region Data
        public Player Player;
        #endregion
        public Game(Player player)
        {
            Player = player;

            StageText = new ScrollableText(Stage.Quest[player.CurrentStage].Text);
            StageTextWindow = new Window(StageText, 80, 16, "Текст квеста", top: -3, fillWidth: false, fillHeight: false, alignment: Alignment.CenterWidth | Alignment.CenterHeight);

            Variants = new ListBox();
            VariantsWindow = new Window(Variants, 60, 6, "Выберите действие", top: +7, fillWidth: false, fillHeight: false, alignment: Alignment.CenterWidth | Alignment.CenterHeight);

            Piety = new TextWidget();
            PietyWindow = new Window(Piety, top: 1, height: 1, hasBorder: false, fillHeight: false, alignment: Alignment.CenterWidth | Alignment.DownHeight);

            Root.AddWidget(StageTextWindow);
            Root.AddWidget(StageText);
            Root.AddWidget(VariantsWindow);
            Root.AddWidget(Variants);
            Root.AddWidget(PietyWindow);
            Root.AddWidget(Piety);

            Bind(EnterTrigger = new Trigger(this, form => (form as Game).Action()));
            Bind(CloseTrigger = new Trigger(this, form => form.Close()));
            Update();

            StageTextWindow.Resize();
            StageText.Resize();
            VariantsWindow.Resize();
            PietyWindow.Resize();
        }

        public bool Locked = false;

        public void Update()
        {
            var stage = Stage.Quest[Player.CurrentStage];
            StageText.Text = stage.Text;
            StageText.Resize();
            StageText.Redraw();

            Variants.UpPosition = Variants.Position = 0;
            Variants.Childs.Clear();
            if (stage.Win)
            {
                Locked = true;
                Variants.Childs.Add(new ListBox.ListItem(new TextWidget("%=>Magenta%Вы победили%=>reset%", fillHeight: false)));
            }
            else if (stage.Death)
            {
                Locked = true;
                Variants.Childs.Add(new ListBox.ListItem(new TextWidget($"Открыта смерть %=>Red%{stage.DeathName}%=>reset%", fillHeight: false)));
            }
            else Variants.Childs = stage.Variants
                .Select(x => new ListBox.ListItem(new TextWidget(x.Text, fillHeight: false))).ToList();
            Variants.Resize();
            Variants.Redraw();

            Piety.Text = $"Очки веры: %=>Blue%{Player.Piety}%=>reset%".PadCenter(30);
            Piety.Resize();
            Piety.Redraw();

            ActiveWidget = Variants;
        }

        public void Action()
        {
            if (Locked)
            {
                Close();
                return;
            }
            var next_id = Stage.Quest[Player.CurrentStage].Variants[Variants.Position].SetStage;
            var next = Stage.Quest[next_id];
            if (next.Win || next.Death)
                Locked = true;
            Player.Piety += Stage.Quest[Player.CurrentStage].Variants[Variants.Position].Piety;
            Player.CurrentStage = next_id;
            Update();
        }

        protected override void OnKeyDown(byte key)
        {
            base.OnKeyDown(key);
            if (key == Key.Escape || key == Key.Tab) CloseTrigger.Do();
            if (key == Key.Enter || key == Key.E) EnterTrigger.Do();
        }
    }
}
