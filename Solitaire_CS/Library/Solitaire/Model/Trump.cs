﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    /// <summary>
    /// トランプ
    /// </summary>
    public class Tramp: IDebugOutput
    {
        /// <summary>
        /// トランプの所有するカード
        /// </summary>
        public Card[] cards;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Tramp()
        {
            cards = new Card[52];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = new Card(i, (Card.Mark)(i / 13), (i % 13) + 1, true);
            }
        }

        /// <summary>
        /// カードの山をソートする（そろえる）
        /// </summary>
        public void Sort()
        {
            for (int i = 0; i < 52; i++)
            {
                cards[i].mark = (Card.Mark)(i / 13);
                cards[i].number = (i % 13) + 1;
            }
        }

        /// <summary>
        /// カードの山をシャッフルする
        /// </summary>
        public void Shuffle(int times = 1000)
        {
            Random r = new Random();
            for (int i = 0; i < times; i++)
            {
                int index = r.Next(1, 51);
                Card changeCard = cards[index];
                cards[index] = cards[0];
                cards[0] = changeCard;
            }

            for(int i = 0; i < cards.Length; i++)
            {
                cards[i].index = i;
            }
        }

        /// <summary>
        /// カードのインデックスを探す
        /// </summary>
        /// <param name="card">探すカード</param>
        /// <returns>インデックス</returns>
        public int FindCardIndex(Card card)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] == card)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// トランプの情報をコンソールに出力する
        /// </summary>
        public void DebugOutput()
        {
            for (int i = 0; i < 52; i++)
            {
                Console.WriteLine(cards[i]);
            }
        }
    }
}
