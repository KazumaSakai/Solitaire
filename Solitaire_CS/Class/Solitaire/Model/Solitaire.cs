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
        /// 山札のカード
        /// </summary>
        public List<Card> stockCards;
        /// <summary>
        /// 開かれた山札のカード
        /// </summary>
        public List<Card> openedStockCards;
        /// <summary>
        /// 現在右上に積み重ねられているカードの番号
        /// </summary>
        public int[] finishNumber;

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
            stockCards = new List<Card>(tramp.cards.Length);
            openedStockCards = new List<Card>(tramp.cards.Length);
            finishNumber = new int[4];

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

            for (int i = 0; i < finishNumber.Length; i++)
            {
                finishNumber[i] = 0;
            }

            for (int i = num; i < tramp.cards.Length; i++)
            {
                Card card = tramp.cards[i];

                System.Drawing.Point point = new System.Drawing.Point(-2, 0);
                cardPoint[card.index] = point;

                stockCards.Add(card);
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
                    OpenStockCard();
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
        public void OpenStockCard()
        {
            //  スタックしているカードがない場合
            if (stockCards.Count == 0)
            {

                //  開かれているカードもない場合
                if (openedStockCards.Count == 0) return;
                
                foreach (Card card in openedStockCards)
                {
                    card.open = false;
                    cardPoint[card.index] = new System.Drawing.Point(-2, 0);
                    basePanel.UpdateCard(card);
                }
                List<Card> emptyList = stockCards;
                stockCards = openedStockCards;
                openedStockCards = emptyList;
                return;
            }

            Card topStockCard = stockCards[0];
            openedStockCards.Add(topStockCard);

            stockCards.Remove(topStockCard);
            cardPoint[topStockCard.index] = new System.Drawing.Point(-1, 0);
            basePanel.UpdateCard(topStockCard);
            topStockCard.open = true;
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
            if (lineIndex < -1 || lineIndex >= tableCards.Length)
            {
                ClearGrabCard();
                return false;
            }

            if (lineIndex == -1 && (grabCards.Length == 1))
            {
                FinishCard(grabCards[0]);
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
                openedStockCards.Remove(card);
                newLine.Add(card);
                basePanel.UpdateCard(card);
            }

            grabCards = null;
            return true;
        }
        /// <summary>
        /// 掴んでいるカードを戻す
        /// </summary>
        private void ClearGrabCard()
        {
            for (int i = 0; i < grabCards.Length; i++)
            {
                basePanel.UpdateCard(grabCards[i]);
            }
            grabCards = null;
        }
        /// <summary>
        /// カードを完了させることができるか
        /// </summary>
        /// <param name="card">カード</param>
        /// <returns></returns>
        public bool FinishCard(Card card)
        {
            int mark = (int)card.mark;
            if (finishNumber[mark] != card.number - 1)
            {
                basePanel.UpdateCard(card);
                return false;
            }
            finishNumber[mark]++;

            System.Drawing.Point point = cardPoint[card.index];
            if(point.X >= 0)
            {
                tableCards[point.X].Remove(card);
                if (tableCards[point.X].Count > 0) tableCards[point.X].Last().open = true;
            }
            else if(point.X == -1)
            {
                openedStockCards.Remove(card);
            }
            cardPoint[card.index] = new System.Drawing.Point(mark, -1);
            basePanel.UpdateCard(card);
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
