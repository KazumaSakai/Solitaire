using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Solitaire
{
    /// <summary>
    /// Solitaireの[View + Controller]
    /// </summary>
    public class SolitairePanel : Panel
    {
        /// <summary>
        /// Solitaireモデル
        /// </summary>
        private Solitaire model;

        /// <summary>
        /// カードを表示したりするパネル
        /// </summary>
        private ViewBasePanel cardsBasePanel;
        /// <summary>
        /// ドロップした時用の当たり判定
        /// </summary>
        private DropPanel dropBasePanel;
        /// <summary>
        /// ドラッグするときカードをまとめるパネル
        /// </summary>
        private DragPanel dragPanel;

        /// <summary>
        /// カードをつかむポイント
        /// </summary>
        private System.Drawing.Point grabPoint;
        /// <summary>
        /// ドラッグ中に呼ぶイベント
        /// </summary>
        private EventHandler dragEventHandler;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model">モデル</param>
        public SolitairePanel(Solitaire model)
        {
            this.model = model;

            this.DoubleBuffered = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "SolitairePanel";
            this.Size = new System.Drawing.Size(625, 625);
            this.TabIndex = 0;
            this.dragEventHandler = new EventHandler((object sender, EventArgs e) => DragCard());

            this.Controls.Add(cardsBasePanel = new ViewBasePanel(model));
            this.Controls.Add(dropBasePanel = new DropPanel(model));
            this.Controls.Add(dragPanel = new DragPanel());
        }

        /// <summary>
        /// カードを掴む
        /// </summary>
        /// <param name="dragCards">掴んだカード</param>
        public void GrabCard(Card[] dragCards)
        {
            dragPanel.Size = new System.Drawing.Size(80, 90 + (30 * (dragCards.Length)));
            grabPoint = new System.Drawing.Point(40, 30);
            MoveDragCard();
            dragPanel.BringToFront();

            for (int i = 0; i < dragCards.Length; i++)
            {
                Panel panel = dragCards[i].formPanel;
                dragPanel.Controls.Add(panel);
                panel.Location = new System.Drawing.Point(0, 30 * i);
                panel.BringToFront();
            }

            Cursor.Current = Cursors.Cross;
            (this.FindForm() as MainForm).timer.Tick += dragEventHandler;

        }
        /// <summary>
        /// カードをドラッグしている
        /// </summary>
        private void DragCard()
        {
            if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                MoveDragCard();
            }
            else
            {
                DropCard();
                (this.FindForm() as MainForm).timer.Tick -= dragEventHandler;
            }
        }
        /// <summary>
        /// 掴んだカードを動かした時
        /// </summary>
        private void MoveDragCard()
        {
            dragPanel.Location = dragPanel.Parent.PointToClient(new System.Drawing.Point(Cursor.Position.X - grabPoint.X, Cursor.Position.Y - grabPoint.Y));
        }
        /// <summary>
        /// カードをドロップした時
        /// </summary>
        private void DropCard()
        {
            dragPanel.SendToBack();
            dragPanel.Location = new System.Drawing.Point(0, 0);
            dragPanel.Size = new System.Drawing.Size(0, 0);

            for (int i = dragPanel.Controls.Count - 1; i >= 0; i--)
            {
                cardsBasePanel.Controls.Add(dragPanel.Controls[i]);
            }

            Cursor.Current = Cursors.Default;

            int targeLine = dropBasePanel.FindDropedLine(Cursor.Position);
            model.DropCard(targeLine);
        }
        /// <summary>
        /// カードの位置を更新する
        /// </summary>
        /// <param name="card">対象のカード</param>
        /// <param name="toFront">前に出すか</param>
        public void UpdateCard(Card card, bool toFront = true)
        {
            System.Drawing.Point point = model.data.cardPoint[card.index];

            if(point.Y == -1)
            {
                card.formPanel.Location = new System.Drawing.Point(12 + (84 * (model.data.tableCards.Length - 4 + point.X)), 12);
                if (toFront) card.formPanel.BringToFront();
                return;
            }

            switch (point.X)
            {
                case -2:
                    card.formPanel.Location = new System.Drawing.Point(12, 12);
                    break;

                case -1:
                    card.formPanel.Location = new System.Drawing.Point(96, 12);
                    break;

                default:
                    card.formPanel.Location = new System.Drawing.Point(12 + (84 * point.X), 150 + (30 * point.Y));
                    break;
            }
            if (toFront) card.formPanel.BringToFront();
        }
        /// <summary>
        /// すべてのカードを更新する
        /// </summary>
        public void UpdateAllCard()
        {
            for (int i = 0; i < model.tramp.cards.Length; i++)
            {
                UpdateCard(model.tramp.cards[i]);
            }
        }

        /// <summary>
        /// フォームのサイズが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SizeChangedCallBack(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(Parent.Size.Width - 42, Parent.Size.Height - 64);
        }
        
        /// <summary>
        /// カードを表示したりする基本的なパネル
        /// </summary>
        private class ViewBasePanel : Panel
        {
            /// <summary>
            /// Solitaireモデル
            /// </summary>
            private Solitaire model;
            /// <summary>
            /// コマンドのパネル
            /// </summary>
            private CommandPanel commandPanel;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="model">モデル</param>
            public ViewBasePanel(Solitaire model)
            {
                this.model = model;

                this.DoubleBuffered = true;
                this.BackColor = System.Drawing.Color.Transparent;
                this.Location = new System.Drawing.Point(0, 0);
                this.Name = "SolitaireCardsBasePanel";
                this.Size = new System.Drawing.Size(625, 625);
                this.TabIndex = 0;

                for (int i = 0; i < model.tramp.cards.Length; i++)
                {
                    Card card = model.tramp.cards[i];
                    Panel cardPanel = card.formPanel;
                    cardPanel.Name = i.ToString();
                    cardPanel.MouseDown += new MouseEventHandler((object sender, MouseEventArgs e) => ClickCard(card));
                    this.Controls.Add(cardPanel);
                }

                EmptyCard emptyCard = new EmptyCard(new System.Drawing.Point(12, 12));
                emptyCard.MouseDown += new MouseEventHandler((object sender, MouseEventArgs e) => ClickStackCardEmpty());
                this.Controls.Add(emptyCard);

                for (int i = 0; i < model.data.tableCards.Length; i++)
                {
                    emptyCard = new EmptyCard(new System.Drawing.Point(12 + (84 * i), 150));
                    this.Controls.Add(emptyCard);
                }

                for (int i = 0; i < 4; i++)
                {
                    emptyCard = new EmptyCard(new System.Drawing.Point(12 + (84 * (model.data.tableCards.Length - 1 - i)), 12));
                    this.Controls.Add(emptyCard);
                }

                this.Controls.Add(commandPanel = new CommandPanel(model));
            }

            /// <summary>
            /// カードをクリックした時
            /// </summary>
            /// <param name="card"></param>
            private void ClickCard(Card card)
            {
                if ((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
                {
                    model.GrabCard(card);
                }
                else if ((Control.MouseButtons & MouseButtons.Right) == MouseButtons.Right)
                {
                    model.FinishCard(card);
                }
            }
            /// <summary>
            /// 空になった山札をクリックしたとき
            /// </summary>
            private void ClickStackCardEmpty()
            {
                model.OpenStockCard();
            }

            /// <summary>
            /// カードが何もないという状態を表すパネル
            /// </summary>
            public class EmptyCard : Panel
            {
                public EmptyCard(System.Drawing.Point point)
                {
                    this.BackgroundImage = Resources.empty;
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                    this.Size = new System.Drawing.Size(80, 120);
                    this.Location = point;
                    this.TabIndex = 0;
                }
            }
            /// <summary>
            /// ゲームのリスタートなどのコマンドを追加する
            /// </summary>
            public class CommandPanel : Panel
            {
                /// <summary>
                /// Solitaireモデル
                /// </summary>
                Solitaire model;
                /// <summary>
                /// 各コマンドのパネル
                /// </summary>
                Panel[] command;
                /// <summary>
                /// コマンドの説明
                /// </summary>
                ToolTip toolTip;

                public CommandPanel(Solitaire model)
                {
                    this.model = model;

                    this.BackColor = System.Drawing.Color.White;
                    this.BorderStyle = BorderStyle.FixedSingle;
                    this.Location = new System.Drawing.Point(509, 561);
                    this.Name = "SolitaireCommnadPanel";
                    this.Size = new System.Drawing.Size(100, 25);
                    this.TabIndex = 0;

                    toolTip = new ToolTip();
                    toolTip.InitialDelay = 100;
                    toolTip.ReshowDelay = 100;
                    toolTip.AutoPopDelay = 10000;

                    command = new Panel[4];
                    for (int i = 0; i < command.Length; i++)
                    {
                        Panel panel = new Panel();

                        panel.BackColor = System.Drawing.Color.White;
                        panel.BorderStyle = BorderStyle.FixedSingle;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        panel.Location = new System.Drawing.Point(-1 + (25 * i), -1);
                        panel.Name = "ReloadPanel";
                        panel.Size = new System.Drawing.Size(26, 26);
                        panel.TabIndex = 0;
                        this.Controls.Add(panel);

                        command[i] = panel;
                    }

                    command[0].BackgroundImage = Resources.plus;
                    command[0].MouseDown += new MouseEventHandler((object sender, MouseEventArgs e) => model.Initialize());
                    toolTip.SetToolTip(command[0], "新しく始める");

                    command[1].BackgroundImage = Resources.reload;
                    command[1].MouseDown += new MouseEventHandler((object sender, MouseEventArgs e) => model.Restart());
                    toolTip.SetToolTip(command[1], "最初からやり直す");

                    command[2].BackgroundImage = Resources.soundOn;
                    toolTip.SetToolTip(command[2], "効果音");

                    command[3].BackgroundImage = Resources.hint;
                    toolTip.SetToolTip(command[3], "ヒント");
                }
            }
        }
        /// <summary>
        /// カードをドロップする箇所の当たり判定
        /// </summary>
        private class DropPanel : Panel
        {
            /// <summary>
            /// Solitaireモデル
            /// </summary>
            private Solitaire model;
            /// <summary>
            /// 各行のパネル
            /// </summary>
            private Panel[] linePanels;
            /// <summary>
            /// 完了したカードが並ぶパネル
            /// </summary>
            private Panel finishPanel;

            public DropPanel(Solitaire model)
            {
                this.model = model;

                this.BackColor = System.Drawing.Color.Transparent;
                this.Location = new System.Drawing.Point(0, 0);
                this.Name = "SolitaireDropPanel";
                this.Size = new System.Drawing.Size(620, 737);
                this.TabIndex = 0;
                this.Visible = false;

                linePanels = new Panel[model.data.tableCards.Length];
                for (int i = 0; i < linePanels.Length; i++)
                {
                    Panel panel = new Panel();
                    panel.BackColor = System.Drawing.Color.Transparent;
                    panel.Location = new System.Drawing.Point(9 + (i * 86), 147);
                    panel.Size = new System.Drawing.Size(86, this.Height - 153);
                
                    linePanels[i] = panel;
                    this.Controls.Add(panel);
                }

                finishPanel = new Panel();
                finishPanel.BackColor = System.Drawing.Color.Transparent;
                finishPanel.Location = new System.Drawing.Point(9 + (84 * (model.data.tableCards.Length - 4)), 9);
                System.Drawing.Point endPoint = new System.Drawing.Point(15 + (84 * (model.data.tableCards.Length - 1)), 15);
                finishPanel.Size = new System.Drawing.Size(endPoint.X - finishPanel.Location.X + 80, endPoint.Y - finishPanel.Location.Y + 120);
                this.Controls.Add(finishPanel);
            }

            /// <summary>
            /// 座標からドロップした列を特定する
            /// </summary>
            /// <param name="point">座標</param>
            /// <returns>失敗　-2</returns>
            public int FindDropedLine(System.Drawing.Point point)
            {
                this.Visible = true;

                Control hitControl = this.GetChildAtPoint(this.PointToClient(Cursor.Position));
                if (hitControl == null) return -2;

                if (hitControl == finishPanel)
                {
                    this.Visible = false;
                    return -1;
                }

                for (int i = 0; i < linePanels.Length; i++)
                {
                    if(hitControl == linePanels[i])
                    {
                        linePanels[i].Visible = false;
                        this.Visible = false;
                        return i;
                    }
                }

                this.Visible = false;
                return -2;
            }
        }
        /// <summary>
        /// ドラッグしたカードをひとまとめにしておくパネル
        /// </summary>
        private class DragPanel : Panel
        {
            public DragPanel()
            {
                this.BackColor = System.Drawing.Color.Transparent;
                this.Location = new System.Drawing.Point(0, 0);
                this.Name = "SolitaireDragPanel";
                this.Size = new System.Drawing.Size(0, 0);
                this.TabIndex = 0;
            }
        }
    }

}
