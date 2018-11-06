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

            for (int i = 0; i < cardPanels.Length; i++)
            {
                cardPanels[i] = tramp.cards[i].formPanel;
                cardPanels[i].Name = i.ToString();
                cardPanels[i].MouseDown += new MouseEventHandler(TakeCard);
                cardPanels[i].MouseUp += new MouseEventHandler(ReleaseCard);
                basePanel.Controls.Add(cardPanels[i]);
            }
            UpdateCards();
        }

        byte dragCardIndex = 0;
        int lastIndex = 0;
        /// <summary>
        /// カードをドラッグし始めた
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TakeCard(object sender, MouseEventArgs e)
        {
            dragCardIndex = byte.Parse((sender as Panel).Name);
            if (!tramp.cards[dragCardIndex].open) return;

            Form1.TickUpdateEvents += new EventHandler(TakingCard);

            lastIndex = ((Cursor.Position.X - basePanel.FindForm().Location.X - 30) / 85);
            tableCards[lastIndex].RemoveAt(tableCards[lastIndex].Count - 1);
        }
        /// <summary>
        /// カードをドラッグ中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TakingCard(object sender, EventArgs e)
        {
            lastIndex = ((Cursor.Position.X - basePanel.FindForm().Location.X - 30) / 85);
        }
        /// <summary>
        /// カードのドラッグを終えた
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReleaseCard(object sender, MouseEventArgs e)
        {
            if (!tramp.cards[dragCardIndex].open) return;

            Form1.TickUpdateEvents -= new EventHandler(TakingCard);

            tableCards[lastIndex].Add(dragCardIndex);
            UpdateCards();
        }

        private void UpdateCards()
        {
            for (int i = 0; i < tableCards.Length; i++)
            {
                for (int j = 0; j < tableCards[i].Count; j++)
                {
                    byte index = tableCards[i][j];
                    bool open = tramp.cards[tableCards[i][j]].open;

                    cardPanels[index].Location = new System.Drawing.Point((12 + (i * 86)), (150 + (j * 30)));
                    cardPanels[index].BringToFront();

                    if(open)
                    {
                        cardPanels[index].BackgroundImage = tramp.cards[index].image;
                    }
                    else
                    {
                        cardPanels[index].BackgroundImage = Solitaire_CS.Properties.Resources.z01;
                    }
                }
            }
        }
        public void SizeChanged(object sender, EventArgs e)
        {
            basePanel.Size = new System.Drawing.Size((sender as Control).Size.Width - 42, (sender as Control).Size.Height - 64);
        }
    }
}
