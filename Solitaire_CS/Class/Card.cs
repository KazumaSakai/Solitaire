using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    public struct Card
    {
        private static string[] markString = { "S", "H", "C", "D" };
        private static string[] numberString = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        public enum Mark
        {
            spade = 0,
            heart,
            clover,
            diamond
        }
        public Mark mark;

        public int number;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append(markString[(int)mark]).Append(numberString[number]).ToString();
        }
    }
}
