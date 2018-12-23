using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Solitaire
{
    public class Card : IFormPanel
    {
        //
        //  Model
        //
        /// <summary>
        /// カードのインデックス
        /// </summary>
        public int index;
        /// <summary>
        /// カードのマーク列挙型
        /// </summary>
        public enum Mark
        {
            spade = 0,
            heart,
            clover,
            diamond
        }
        /// <summary>
        /// カードのマーク
        /// </summary>
        public Mark mark;
        /// <summary>
        /// カードの数字
        /// </summary>
        public int number
        {
            get
            {
                return _number;
            }
            set
            {
                if(value != _number)
                {
                    _number = value;
                    if (basePanel != null)
                    {
                        basePanel.UpdatePanel();
                    }
                }
            }
        }
        private int _number;
        /// <summary>
        /// カードが表か裏か
        /// </summary>
        public bool open
        {
            get
            {
                return _open;
            }
            set
            {
                if(value != _open)
                {
                    _open = value;
                    if (basePanel != null)
                    {
                        basePanel.UpdatePanel();
                    }
                }
            }
        }
        private bool _open;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mark">マーク</param>
        /// <param name="number">数字</param>
        /// <param name="open">表か裏か</param>
        public Card(int index, Mark mark, int number, bool open)
        {
            this.index = index;
            this.mark = mark;
            this.number = number;
            this.open = open;
        }

        private static string[] markString = { "S", "H", "C", "D" };
        private static string[] numberString = { " 0", " 1", " 2", " 3", " 4", " 5", " 6", " 7", " 8", " 9", "10", " J", " Q", " K" };
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append(markString[(int)mark]).Append(numberString[number]).ToString();
        }
        /// <summary>
        /// コンソールにカードの情報を出力する
        /// </summary>
        public void DebugOutput()
        {
            Console.WriteLine(this.ToString());
        }


        //
        //  View + Controller
        //
        private CardPanel basePanel;
        public Panel formPanel
        {
            get
            {
                if (basePanel == null)
                {
                    basePanel = new CardPanel(this);
                }
                return basePanel;
            }
        }
    }
}
