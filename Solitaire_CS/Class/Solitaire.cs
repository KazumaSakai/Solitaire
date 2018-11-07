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

        private Card dragCard;
        private System.Drawing.Point deltaPoint;
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

            deltaPoint = new System.Drawing.Point(dragCard.formPanel.Width / 2, dragCard.formPanel.Height / 2);
            (basePanel.FindForm() as Form1).timer.Tick += new EventHandler(Dragging);
        }
        private void Dragging(object sender, EventArgs e)
        {
            if((Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left)
            {
                dragCard.formPanel.Location = dragCard.formPanel.Parent.PointToClient(new System.Drawing.Point(Cursor.Position.X - deltaPoint.X, Cursor.Position.Y - deltaPoint.Y));
            }
            else
            {
                dragCard.formPanel.SendToBack();
                Panel newParentPanel = basePanel.GetChildAtPoint(basePanel.PointToClient(Cursor.Position), GetChildAtPointSkip.None) as Panel;
                if(newParentPanel != null)
                {
                    newParentPanel.Controls.Add(dragCard.formPanel);
                    dragCard.formPanel.Location = new System.Drawing.Point(0, 30);
                }

                Cursor.Current = Cursors.Default;
                dragCard = null;
                (basePanel.FindForm() as Form1).timer.Tick -= new EventHandler(Dragging);
            }
        }
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

        private void UpdateCards()
        {
            for (int i = 0; i < tableCards.Length; i++)
            {
                for (int j = 0; j < tableCards[i].Count; j++)
                {
                    byte index = tableCards[i][j];

                    cardPanels[index].Location = new System.Drawing.Point((12 + (i * 86)), (150 + (j * 30)));
                    cardPanels[index].BringToFront();
                }
            }
        }
        public void SizeChanged(object sender, EventArgs e)
        {
            basePanel.Size = new System.Drawing.Size((sender as Control).Size.Width - 42, (sender as Control).Size.Height - 64);
        }
    }
}
