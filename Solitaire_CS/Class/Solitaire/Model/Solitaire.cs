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
        /// 開いているカード
        /// </summary>
        public Card openStackCard;
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
                for (int j = 0; j < (i + 1); j++)
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

                System.Drawing.Point point = new System.Drawing.Point(-2, 0);
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
            if (card == null) return false;
            if (!card.open)
            {
                if(cardPoint[card.index].X == -2)
                {
                    OpenStackCard();
                    return false;
                }
                else
                {
                    return false;
                }
            }

            System.Drawing.Point point = cardPoint[card.index];

            if(point.X >= 0)
            {
                List<Card> line = tableCards[point.X];
                for (int i = point.Y + 1; i < line.Count; i++)
                {
                    Card nextCard = line[i];

                    if (!CanStackCard(card, nextCard)) return false;

                    card = nextCard;
                }

                grabCards = new Card[line.Count - point.Y];
                for (int i = 0; i < line.Count - point.Y; i++)
                {
                    grabCards[i] = line[point.Y + i];
                }
            }
            else
            {
                grabCards = new Card[]{ card };
            }

            basePanel.GrabCard(grabCards);

            return true;
        }
        /// <summary>
        /// スタックしてあるカードを開く
        /// </summary>
        /// <param name="card"></param>
        public void OpenStackCard()
        {
            if (stackCards.Count == 0) return;

            Card topStackCard = stackCards[0];


            stackCards.Remove(topStackCard);
            cardPoint[topStackCard.index] = new System.Drawing.Point(-1, 0);
            basePanel.UpdateCard(topStackCard);
            topStackCard.open = true;

            if (openStackCard != null)
            {
                stackCards.Add(openStackCard);
                cardPoint[openStackCard.index] = new System.Drawing.Point(-2, 0);
                basePanel.UpdateCard(openStackCard);
                openStackCard.open = false;
            }

            openStackCard = topStackCard;
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
            //  掴んでいるカードが存在しない
            if (grabCards == null || grabCards.Length == 0) return false;
            
            //  挿入先が存在しない場合
            if (lineIndex < 0 || lineIndex >= tableCards.Length)
            {
                ClearGrabCard();
                return false;
            }

            //  挿入する列にカードがあるか
            if (tableCards[lineIndex].Count > 0)
            {
                Card finalCard = tableCards[lineIndex].Last();
                if (!CanStackCard(finalCard, grabCards[0]))
                {
                    ClearGrabCard();
                    return false;
                }
            }

            //  空の列に挿入する場合、King以外は処理しない
            else if (grabCards[0].number != 13)
            {
                ClearGrabCard();
                return false;
            }


            System.Drawing.Point cardPoint = this.cardPoint[grabCards[0].index];
            List<Card> newLine = tableCards[lineIndex];
            if (cardPoint.X >= 0)
            {
                List<Card> line = tableCards[cardPoint.X];
                for (int i = 0; i < grabCards.Length; i++)
                {
                    Card card = grabCards[i];

                    System.Drawing.Point point = new System.Drawing.Point(lineIndex, newLine.Count);
                    this.cardPoint[card.index] = point;
                    line.Remove(card);
                    newLine.Add(card);

                    basePanel.UpdateCard(card);
                }
                if (line.Count > 0) line.Last().open = true;
            }
            else
            {
                Card card = grabCards[0];
                System.Drawing.Point point = new System.Drawing.Point(lineIndex, newLine.Count);
                this.cardPoint[card.index] = point;
                openStackCard = null;
                newLine.Add(card);
                basePanel.UpdateCard(card);

                OpenStackCard();
            }

            grabCards = null;
            return true;
        }
        private void ClearGrabCard()
        {
            for (int i = 0; i < grabCards.Length; i++)
            {
                basePanel.UpdateCard(grabCards[i]);
            }
            grabCards = null;
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
