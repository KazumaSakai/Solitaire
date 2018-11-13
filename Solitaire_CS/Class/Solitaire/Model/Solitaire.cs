using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Solitaire
{
    /// <summary>
    /// Solitaireモデル
    /// </summary>
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
        /// ドラッグ中のカード
        /// </summary>
        private Card[] grabCards;

        /// <summary>
        /// データ
        /// </summary>
        public struct Data
        {
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

            public Data(int cardLength, int tableLength)
            {
                cardPoint = new System.Drawing.Point[cardLength];
                stockCards = new List<Card>(cardLength);
                openedStockCards = new List<Card>(cardLength);
                finishNumber = new int[4];

                tableCards = new List<Card>[tableLength];
                for (int i = 0; i < tableCards.Length; i++)
                {
                    tableCards[i] = new List<Card>();
                }
            }

            public void Initialize(Tramp tramp)
            {
                stockCards.Clear();
                openedStockCards.Clear();
                for (int i = 0; i < finishNumber.Length; i++)
                {
                    finishNumber[i] = 0;
                }

                int num = 0;
                for (int i = 0; i < tableCards.Length; i++)
                {
                    tableCards[i].Clear();
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

                    stockCards.Add(card);
                    card.open = false;
                }
            }
        }
        /// <summary>
        /// ゲームのデータ
        /// </summary>
        public Data data;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Solitaire()
        {            
            tramp = new Tramp();
            tramp.Shuffle();
            data = new Data(52, 7);

            Initialize();
        }
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            tramp.Shuffle();
            Restart();
        }
        /// <summary>
        /// 最初からやり直す
        /// </summary>
        public void Restart()
        {
            data.Initialize(tramp);

            if (basePanel != null)
            {
                basePanel.UpdateAllCard();
            }
        }
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
                if(data.cardPoint[card.index].X == -2)
                {
                    OpenStockCard();
                    return false;
                }
                else
                {
                    return false;
                }
            }

            System.Drawing.Point point = data.cardPoint[card.index];

            if (point.Y == -1) return false;

            if(point.X >= 0)
            {
                List<Card> line = data.tableCards[point.X];
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
            if (data.stockCards.Count == 0)
            {

                //  開かれているカードもない場合
                if (data.openedStockCards.Count == 0) return;
                
                foreach (Card card in data.openedStockCards)
                {
                    card.open = false;
                    data.cardPoint[card.index] = new System.Drawing.Point(-2, 0);
                    basePanel.UpdateCard(card);
                }
                List<Card> emptyList = data.stockCards;
                data.stockCards = data.openedStockCards;
                data.openedStockCards = emptyList;
                return;
            }

            Card topStockCard = data.stockCards[0];
            data.openedStockCards.Add(topStockCard);

            data.stockCards.Remove(topStockCard);
            data.cardPoint[topStockCard.index] = new System.Drawing.Point(-1, 0);
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
            if (lineIndex < -1 || lineIndex >= data.tableCards.Length)
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
            if (data.tableCards[lineIndex].Count > 0)
            {
                Card finalCard = data.tableCards[lineIndex].Last();
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

            System.Drawing.Point cardPoint = this.data.cardPoint[grabCards[0].index];
            List<Card> newLine = data.tableCards[lineIndex];
            if (cardPoint.X >= 0)
            {
                List<Card> line = data.tableCards[cardPoint.X];
                for (int i = 0; i < grabCards.Length; i++)
                {
                    Card card = grabCards[i];

                    System.Drawing.Point point = new System.Drawing.Point(lineIndex, newLine.Count);
                    this.data.cardPoint[card.index] = point;
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
                this.data.cardPoint[card.index] = point;
                data.openedStockCards.Remove(card);
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
            if (data.finishNumber[mark] != card.number - 1)
            {
                basePanel.UpdateCard(card);
                return false;
            }
            data.finishNumber[mark]++;

            System.Drawing.Point point = data.cardPoint[card.index];
            if(point.X >= 0)
            {
                data.tableCards[point.X].Remove(card);
                if (data.tableCards[point.X].Count > 0) data.tableCards[point.X].Last().open = true;
            }
            else if(point.X == -1)
            {
                data.openedStockCards.Remove(card);
            }
            data.cardPoint[card.index] = new System.Drawing.Point(mark, -1);
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
