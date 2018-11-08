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
            Console.WriteLine("{0} : {1} : {2}", targetIndex, moveIndex, length);
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
                tramp.cards[moveCards[i]].open = false;
            }
            UpdateCards();
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
        private Panel[] cardPanels;
        private Panel[] dropPanels;

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
            basePanel = new DoubleBufferPanel();
            basePanel.BackColor = System.Drawing.Color.Transparent;
            basePanel.Location = new System.Drawing.Point(12, 12);
            basePanel.Name = "SolitaireBasePanel";
            basePanel.Size = new System.Drawing.Size(624, 737);
            basePanel.TabIndex = 0;

            cardPanels = new Panel[52];

            dropPanels = new Panel[7];
            for (int i = 0; i < dropPanels.Length; i++)
            {
                dropPanels[i] = new Panel();
                dropPanels[i].Location = new System.Drawing.Point(9 + (i * 86), 147);
                dropPanels[i].Size = new System.Drawing.Size(86, basePanel.Height - 153);
                dropPanels[i].BackColor = System.Drawing.Color.Transparent;
                basePanel.Controls.Add(dropPanels[i]);
            }

            for (int i = 0; i < cardPanels.Length; i++)
            {
                cardPanels[i] = tramp.cards[i].formPanel;
                cardPanels[i].Name = i.ToString();
                cardPanels[i].MouseDown += new MouseEventHandler(DragCard);
                basePanel.Controls.Add(cardPanels[i]);

                tramp.cards[i].open = !tramp.cards[i].open;
            }
            UpdateCards();
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
            if (dragCard == null) return;

            Cursor.Current = Cursors.Hand;

            dragCard.formPanel.FindForm().Controls.Add(dragCard.formPanel);
            dragCard.formPanel.BringToFront();

            centerPoint = new System.Drawing.Point(dragCard.formPanel.Width / 2, dragCard.formPanel.Height / 2);
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
                dragCard.formPanel.Location = dragCard.formPanel.Parent.PointToClient(new System.Drawing.Point(Cursor.Position.X - centerPoint.X, Cursor.Position.Y - centerPoint.Y));
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
            dragCard.formPanel.SendToBack();
            Panel newParentPanel = basePanel.GetChildAtPoint(basePanel.PointToClient(Cursor.Position)) as Panel;
            if (newParentPanel != null)
            {
                newParentPanel.Controls.Add(dragCard.formPanel);
                dragCard.formPanel.Location = new System.Drawing.Point(0, 30);
                (int, int) cardPos = FindPos(dragCard);
                MoveCard(FindLine(newParentPanel), cardPos.Item1, cardPos.Item2);
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
            for (int i = 0; i < cardPanels.Length; i++)
            {
                if(cardPanels[i] == panel)
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

                    dropPanels[i].Controls.Add(cardPanels[index]);
                    cardPanels[index].Location = new System.Drawing.Point(0, (j * 30));
                    cardPanels[index].BringToFront();
                }
            }
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
