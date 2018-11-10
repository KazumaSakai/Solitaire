using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Solitaire
{
    public class Solitaire : IFormPanel
    {
        //
        //  Model
        //
        /// <summary>
        /// トランプ
        /// </summary>
        public Tramp tramp;
        /// <summary>
        /// カードの位置
        /// </summary>
        public System.Drawing.Point[] cardPoint;

        /// <summary>
        /// テーブル上のカード
        /// </summary>
        public List<Card>[] tableCards;
        /// <summary>
        /// 上にあるカード
        /// </summary>
        public List<Card> stackCards;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Solitaire()
        {            
            tramp = new Tramp();
            tramp.Shuffle();
            tableCards = new List<Card>[7];
            cardPoint = new System.Drawing.Point[52];
            for (int i = 0; i < tableCards.Length; i++)
            {
                tableCards[i] = new List<Card>();
            }
            stackCards = new List<Card>();
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            tramp.Shuffle();


            int num = 0;
            for (int i = 0; i < tableCards.Length; i++)
            {
                for (int j = 0; j < (i + 3); j++)
                {
                    bool isCardOpen = (i <= j);

                    Card card = tramp.cards[num];

                    System.Drawing.Point point = new System.Drawing.Point(i, j);
                    cardPoint[card.index] = point;
                    tableCards[i].Add(card);
                    card.open = isCardOpen;
                
                    num++;
                }
            }

            for (int i = num; i < tramp.cards.Length; i++)
            {
                Card card = tramp.cards[i];

                System.Drawing.Point point = new System.Drawing.Point(-1, 0);
                cardPoint[card.index] = point;

                stackCards.Add(card);
                card.open = false;
            }
        }

        /// <summary>
        /// ドラッグ中のカード
        /// </summary>
        Card[] grabCards;
        /// <summary>
        /// カードをつかむ
        /// </summary>
        /// <param name="card">掴むカード</param>
        /// <returns>つかめたかの結果</returns>
        public bool GrabCard(Card card)
        {
            if (card == null || !card.open) return false;

            System.Drawing.Point point = cardPoint[card.index];

            List<Card> line = tableCards[point.X];
            for (int i = point.Y + 1; i < line.Count; i++)
            {
                Card nextCard = line[i];

                if(!CanStackCard(card, nextCard)) return false;
            
                card = nextCard;
            }

            grabCards = new Card[line.Count - point.Y];
            for (int i = 0; i < line.Count - point.Y; i++)
            {
                grabCards[i] = line[point.Y + i];
            }

            basePanel.GrabCard(grabCards);

            Console.WriteLine("GrabCard : {0}", card);

            return true;
        }
        /// <summary>
        /// カードを重ねられるか
        /// </summary>
        /// <param name="fowardCard">前</param>
        /// <param name="nextCard">後</param>
        /// <returns></returns>
        public bool CanStackCard(Card fowardCard, Card nextCard)
        {
            bool isRed = (fowardCard.mark == Card.Mark.diamond || fowardCard.mark == Card.Mark.heart);
            bool nextIsRed = (nextCard.mark == Card.Mark.diamond || nextCard.mark == Card.Mark.heart);
            bool isFowardNumber = (nextCard.number == fowardCard.number - 1);

            return (isRed != nextIsRed && isFowardNumber);
        }
        /// <summary>
        /// カードをドロップする
        /// </summary>
        /// <param name="lineIndex">ドロップする列</param>
        /// <returns></returns>
        public bool DropCard(int lineIndex)
        {
            if (grabCards == null) return false;
        
            if (lineIndex < 0 || lineIndex >= tableCards.Length)
            {
                for (int i = 0; i < grabCards.Length; i++)
                {
                    basePanel.UpdateCard(grabCards[i]);
                }
                grabCards = null;
                return false;
            }

            if(tableCards[lineIndex].Count > 0)
            {
                Card finalCard = tableCards[lineIndex].Last();
                if (!CanStackCard(finalCard, grabCards[0]))
                {
                    for (int i = 0; i < grabCards.Length; i++)
                    {
                        basePanel.UpdateCard(grabCards[i]);
                    }
                    grabCards = null;
                    return false;
                }
            }

            List<Card> line = tableCards[cardPoint[grabCards[0].index].X];
            List<Card> newLine = tableCards[lineIndex];
            for (int i = 0; i < grabCards.Length; i++)
            {
                Card card = grabCards[i];

                System.Drawing.Point point = new System.Drawing.Point(lineIndex, newLine.Count);
                cardPoint[card.index] = point;
                line.Remove(card);
                newLine.Add(card);
            
                basePanel.UpdateCard(card);
            }
            if (line.Count > 0) line.Last().open = true;

            grabCards = null;
            return true;
        }

        //
        //  View + Controller
        //
        private SolitairePanel basePanel;
        /// <summary>
        /// このクラスのフォーム
        /// </summary>
        public Panel formPanel
        {
            get
            {
                if(basePanel == null)
                {
                    basePanel = new SolitairePanel(this);
                    basePanel.UpdateAllCard();
                }
                return basePanel;
            }
        }
    }
}
