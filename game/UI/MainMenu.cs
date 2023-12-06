using KCore;
using KCore.Graphics;
using KCore.Graphics.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisetsar
{
    public class MainMenu : Form
    {
        public Window MenuContainer;
        public ListBox Menu;
        public TextRow WisetsarRow;

        public Trigger EnterTrigger;
        public Trigger CloseTrigger;

        #region Data
        public Player Current;

        public static void AddScore(Player player)
        {
            using (var stream = File.OpenWrite("scores.dat"))
            {
                using (var bw = new BinaryWriter(stream))
                {
                    bw.BaseStream.Position = bw.BaseStream.Length;
                    player.PlayedDate = DateTime.Now;
                    player.Write(bw);
                }
            }
        }
        #endregion

        public MainMenu()
        {
            WisetsarRow = new TextRow(
                Terminal.FixedWindowWidth / 3 * 2,
                "Мудрость царя",
                alignment: Alignment.CenterWidth | Alignment.UpHeight);

            Menu = new ListBox();
            Menu.Childs = new List<ListBox.ListItem>()
            {
                new ListBox.ListItem(new TextWidget("Продолжить игру", fillHeight: false)),
                new ListBox.ListItem(new TextWidget("Новая игра", fillHeight: false)),
                new ListBox.ListItem(new TextWidget("Рекорды", fillHeight: false)),
                new ListBox.ListItem(new TextWidget("Выход", fillHeight: false)),
            };

            MenuContainer = new Window(Menu, 40, 12, "Главное меню", fillWidth: false, fillHeight: false, alignment: Alignment.CenterWidth | Alignment.CenterHeight);

            Root.AddWidget(WisetsarRow);
            Root.AddWidget(MenuContainer);
            Root.AddWidget(Menu);

            WisetsarRow.Resize();
            MenuContainer.Resize();

            ActiveWidget = Menu;

            Bind(EnterTrigger = new Trigger(this, form => (form as MainMenu).Enter((form as MainMenu).Menu.Position)));
            Bind(CloseTrigger = new Trigger(this, form => form.Close()));
        }

        public void Enter(int i)
        {
            switch (i)
            {
                case 0:
                    if (Current != null)
                    {
                        var stage = Stage.Quest[Current.CurrentStage];
                        if (stage.Win || stage.Death) return;
                        this.RealizeAnimation(new Game(Current));
                        if (stage.Win || stage.Death)
                        {
                            AddScore(Current);
                            Current = null;
                        }
                    }
                    return;
                case 1:
                    {
                        Current = new Player() { CurrentStage = Stage.Quest.Values.First().Id };
                        Menu.Position = 0;
                        this.RealizeAnimation(new Game(Current));
                        var stage = Stage.Quest[Current.CurrentStage];
                        if (stage.Win || stage.Death)
                        {
                            AddScore(Current);
                            Current = null;
                        }
                    }
                    return;
                case 2:
                    this.RealizeAnimation(new Scores());
                    return;
                case 3: CloseTrigger.Do(); return;
            }
        }

        protected override void OnKeyDown(byte key)
        {
            base.OnKeyDown(key);
            if (key == Key.Enter || key == Key.E) EnterTrigger.Do();
            if (key == Key.Escape) CloseTrigger.Do();
        }
    }
}
