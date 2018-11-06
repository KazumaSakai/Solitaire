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
        private List<(byte, bool)>[] tableCards;
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
            tableCards = new List<(byte, bool)>[7];
            for (int i = 0; i < tableCards.Length; i++)
            {
                tableCards[i] = new List<(byte, bool)>();
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
                    tableCards[i].Add((num, isCardOpen));
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
                    if(tableCards[i].Count > len)
                    {
                        count++;
                        if (tableCards[i][len].Item2)
                        {
                            sb.Append(tramp.cards[tableCards[i][len].Item1]).Append("　");
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
            basePanel = new Panel();
            basePanel.BackColor = System.Drawing.Color.Transparent;
            basePanel.Location = new System.Drawing.Point(12, 12);
            basePanel.Name = "SolitaireBasePanel";
            basePanel.Size = new System.Drawing.Size(960, 537);
            basePanel.TabIndex = 0;

            cardPanels = new Panel[52];

            for (int i = 0; i < cardPanels.Length; i++)
            {
                cardPanels[i] = new Panel();

                cardPanels[i].BackgroundImage = Solitaire_CS.Properties.Resources.z01;
                cardPanels[i].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                cardPanels[i].Name = "panel2";
                cardPanels[i].Size = new System.Drawing.Size(80, 120);
                cardPanels[i].TabIndex = 0;
                basePanel.Controls.Add(cardPanels[i]);
            }
            UpdateCards();
        }
        private void UpdateCards()
        {
            for (int i = 0; i < tableCards.Length; i++)
            {
                for (int j = 0; j < tableCards[i].Count; j++)
                {
                    byte index = tableCards[i][j].Item1;
                    bool open = tableCards[i][j].Item2;

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
    }
}
