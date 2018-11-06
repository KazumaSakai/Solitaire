﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    public class Tramp: IDebugOutput
    {
        public Card[] cards;

        public Tramp()
        {
            cards = new Card[52];
            Sort();
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
        }

        /// <summary>
        /// デバッグ出力
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
