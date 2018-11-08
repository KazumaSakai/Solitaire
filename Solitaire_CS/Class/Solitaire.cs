using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Solitaire
{
    public class Solitaire : IDebugOutput, IFormPanel
    {
        /// <summary>
        /// トランプ
        /// </summary>
        private Tramp tramp;

        /// <summary>
        /// テーブル上のカード
        /// </summary>
        private List<byte>[] tableCards;
        /// <summary>
        /// 上にあるカード
        /// </summary>
        private List<byte> topCards;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Solitaire()
        {
            tramp = new Tramp();
            tramp.Shuffle();
            tableCards = new List<byte>[7];
            for (int i = 0; i < tableCards.Length; i++)
            {
                tableCards[i] = new List<byte>();
            }
            topCards = new List<byte>();
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            tramp.Shuffle();

            byte num = 0;
            for (byte i = 0; i < tableCards.Length; i++)
            {
                for (byte j = 0; j < (i + 3); j++)
                {
                    bool isCardOpen = (i <= j);
                    tableCards[i].Add(num);
                    tramp.cards[num].open = isCardOpen;
                    num++;
                }
            }
        }

        /// <summary>
        /// カードを移動する
        /// </summary>
        /// <param name="targetIndex">移動先の列のインデックス</param>
        /// <param name="moveIndex">移動する列のインデックス</param>
        /// <param name="length">移動する枚数</param>
        public void MoveCard(int targetIndex, int moveIndex, int length)
        {
            byte[] moveCards = new byte[length];
            int startIndex = tableCards[moveIndex].Count - 1;
            length--;
            for (int i = 0; i < moveCards.Length; i++)
            {
                moveCards[length] = tableCards[moveIndex][startIndex];
                tableCards[moveIndex].RemoveAt(startIndex);
                startIndex--;
                length--;
            }
            for (int i = 0; i < moveCards.Length; i++)
            {
                tableCards[targetIndex].Add(moveCards[i]);
                tramp.cards[moveCards[i]].open = true;
            }
            UpdateCards();
        }

        public bool CanMoveCard(int targetIndex, Card card)
        {
            bool isRed = (card.mark == Card.Mark.diamond || card.mark == Card.Mark.heart);
            Card targetCard = tramp.cards[tableCards[targetIndex].Last()];
            bool targetRed = (targetCard.mark == Card.Mark.diamond || targetCard.mark == Card.Mark.heart);
            bool fowardNumber = (targetCard.number - 1 == card.number);
            return (isRed != targetRed) && fowardNumber;
        }

        /// <summary>
        /// デバッグ出力
        /// </summary>
        public void DebugOutput()
        {
            StringBuilder sb = new StringBuilder();
            int len = 0;
            int count = 1;

            while(count > 0)
            {
                count = 0;
                for (int i = 0; i < tableCards.Length; i++)
                {
                    byte index = tableCards[i][len];
                    bool open = tramp.cards[tableCards[i][len]].open;

                    if (tableCards[i].Count > len)
                    {
                        count++;
                        if (open)
                        {
                            sb.Append(tramp.cards[index]).Append("　");
                        }
                        else
                        {
                            sb.Append("___　");
                        }
                    }
                    else
                    {
                        sb.Append("   　");
                    }
                }
                Console.WriteLine(sb.ToString());
                sb.Clear();
                len++;
            }
        }

        //
        //  Form
        //
        private Panel basePanel;

        private Panel dragPanel;

        private Panel dropBasePanel;
        private Panel[] dropPanels;

        private Panel cardsBasePanel;

        /// <summary>
        /// このクラスのフォーム
        /// </summary>
        public Panel formPanel
        {
            get
            {
                if(basePanel == null)
                {
                    CreatePanel();
                }
                return basePanel;
            }
        }

        /// <summary>
        /// パネルを作成した
        /// </summary>
        private void CreatePanel()
        {
            basePanel = CreateEmptyPanel(new System.Drawing.Size(624, 737), "SolitaireBasePanel");

            cardsBasePanel = CreateEmptyPanel(basePanel.Size, "SolitaireCardsBasePanel");
            basePanel.Controls.Add(cardsBasePanel);

            dropBasePanel = CreateEmptyPanel(basePanel.Size, "SolitaireDropPanel");
            basePanel.Controls.Add(dropBasePanel);

            dragPanel = CreateEmptyPanel(new System.Drawing.Size(80, 120), "SolitaireDragPanel");
            basePanel.Controls.Add(dragPanel);

            dropPanels = new Panel[7];
            for (int i = 0; i < dropPanels.Length; i++)
            {
                dropPanels[i] = new Panel();
                dropPanels[i].Location = new System.Drawing.Point(9 + (i * 86), 147);
                dropPanels[i].Size = new System.Drawing.Size(86, basePanel.Height - 153);
                dropPanels[i].BackColor = System.Drawing.Color.Transparent;
                dropBasePanel.Controls.Add(dropPanels[i]);
            }

            for (int i = 0; i < tramp.cards.Length; i++)
            {
                Panel cardPanel = tramp.cards[i].formPanel;
                cardPanel.Name = i.ToString();
                cardPanel.MouseDown += new MouseEventHandler(DragCard);
                cardsBasePanel.Controls.Add(cardPanel);
            }
            UpdateCards();
        }
        private Panel CreateEmptyPanel(System.Drawing.Size size, string name = "SolitaireEmptyPanel")
        {
            Panel newPanel = new DoubleBufferPanel();
            newPanel.BackColor = System.Drawing.Color.Transparent;
            newPanel.Location = new System.Drawing.Point(0, 0);
            newPanel.Name = name;
            newPanel.Size = size;
            newPanel.TabIndex = 0;

            return newPanel;
        }

        /// <summary>
        /// ドラッグしているカード
        /// </summary>
        private Card dragCard;
        /// <summary>
        /// ドラックしているコントロールの中央
        /// </summary>
        private System.Drawing.Point centerPoint;
        /// <summary>
        /// カードをドラッグし始めた
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragCard(object sender, MouseEventArgs e)
        {
            dragCard = FindCard(sender as Panel);
            if (dragCard == null || !dragCard.open) return;

            (int, int) cardPos = FindPos(dragCard);
            dragPanel.Size = new System.Drawing.Size(80, 90 + (30 * (cardPos.Item2)));
            List<byte> line = tableCards[cardPos.Item1];

            int c = 0;
            for (int i = line.Count - cardPos.Item2; i < line.Count; i++)
            {
                dragPanel.Controls.Add(tramp.cards[line[i]].formPanel);
                tramp.cards[line[i]].formPanel.Location = new System.Drawing.Point(0, (30 * c));
                tramp.cards[line[i]].formPanel.BringToFront();
                c++;
            }

            Cursor.Current = Cursors.Hand;
            
            dragPanel.BringToFront();

            centerPoint = new System.Drawing.Point(40, 30);
            dragPanel.Location = dragPanel.Parent.PointToClient(new System.Drawing.Point(Cursor.Position.X - centerPoint.X, Cursor.Position.Y - centerPoint.Y));

            (basePanel.FindForm() as Form1).timer.Tick += new EventHandler(Dragging);
        }
        /// <summary>
        /// カードをドラッグ中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dragging(object sender, EventArgs e)
        {
            if((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                dragPanel.Location = dragPanel.Parent.PointToClient(new System.Drawing.Point(Cursor.Position.X - centerPoint.X, Cursor.Position.Y - centerPoint.Y));
            }
            else
            {
                Drop();
            }
        }
        /// <summary>
        /// カードをドロップした
        /// </summary>
        private void Drop()
        {
            dragPanel.SendToBack();
            dragPanel.Location = new System.Drawing.Point(0, 0);
            dragPanel.Size = new System.Drawing.Size(0, 0);

            Panel newParentPanel = dropBasePanel.GetChildAtPoint(dropBasePanel.PointToClient(Cursor.Position)) as Panel;
            if (newParentPanel != null)
            {
                for (int i = dragPanel.Controls.Count - 1; i >= 0; i--)
                {
                    cardsBasePanel.Controls.Add(dragPanel.Controls[i]);
                }

                (int, int) cardPos = FindPos(dragCard);
                int targeLine = FindLine(newParentPanel);
                if (CanMoveCard(targeLine, dragCard))
                {
                    MoveCard(targeLine, cardPos.Item1, cardPos.Item2);
                }
                else
                {
                    UpdateCards();
                }
            }

            Cursor.Current = Cursors.Default;
            dragCard = null;
            (basePanel.FindForm() as Form1).timer.Tick -= new EventHandler(Dragging);
        }
        /// <summary>
        /// カードをパネルから探す
        /// </summary>
        /// <param name="panel">パネル</param>
        /// <returns>カードクラス</returns>
        private Card FindCard(Panel panel)
        {
            if (panel == null) return null;
            for (int i = 0; i < tramp.cards.Length; i++)
            {
                if(tramp.cards[i].formPanel == panel)
                {
                    return tramp.cards[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 列をパネルから探す
        /// </summary>
        /// <param name="panel">パネル</param>
        /// <returns></returns>
        private int FindLine(Panel panel)
        {
            if (panel == null) return 0;
            for (int i = 0; i < dropPanels.Length; i++)
            {
                if (dropPanels[i] == panel)
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// カードから位置を探す
        /// </summary>
        /// <param name="card">カード</param>
        /// <returns></returns>
        private (int, int) FindPos(Card card)
        {
            for (int i = 0; i < tableCards.Length; i++)
            {
                for (int j = 0; j < tableCards[i].Count; j++)
                {
                    if (tramp.cards[tableCards[i][j]] == card)
                    {
                        return (i, tableCards[i].Count - j);
                    }
                }
            }
            return (0, 0);
        }

        /// <summary>
        /// カードの描写を更新する
        /// </summary>
        private void UpdateCards()
        {
            for (int i = 0; i < tableCards.Length; i++)
            {
                for (int j = 0; j < tableCards[i].Count; j++)
                {
                    byte index = tableCards[i][j];

                    tramp.cards[index].formPanel.Location = new System.Drawing.Point(12 + (i * 84), 150 + (j * 30));
                    tramp.cards[index].formPanel.BringToFront();
                }
            }
            basePanel.Update();
        }
        /// <summary>
        /// フォームのサイズが変更されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SizeChanged(object sender, EventArgs e)
        {
            basePanel.Size = new System.Drawing.Size((sender as Control).Size.Width - 42, (sender as Control).Size.Height - 64);
        }
    }
}
