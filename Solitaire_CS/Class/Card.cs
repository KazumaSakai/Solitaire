using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Solitaire_CS.Properties;
using System.Windows.Forms;

namespace Solitaire
{
    public class Card : IFormPanel, IDebugOutput
    {
        //
        //  Field + Property
        //
        #region ・Field + Property
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
        public int number;
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
                    UpdatePanel();
                    _open = value;
                }
            }
        }
        private bool _open;
        /// <summary>
        /// カードの画像
        /// </summary>
        private Image image
        {
            get
            {
                if(open)
                {
                    switch (mark)
                    {
                        default:
                        case Mark.spade:
                            switch (number)
                            {
                                default:
                                case 1:
                                    return Resources.s01;

                                case 2:
                                    return Resources.s02;

                                case 3:
                                    return Resources.s03;

                                case 4:
                                    return Resources.s04;

                                case 5:
                                    return Resources.s05;

                                case 6:
                                    return Resources.s06;

                                case 7:
                                    return Resources.s07;

                                case 8:
                                    return Resources.s08;

                                case 9:
                                    return Resources.s09;

                                case 10:
                                    return Resources.s10;

                                case 11:
                                    return Resources.s11;

                                case 12:
                                    return Resources.s12;

                                case 13:
                                    return Resources.s13;
                            }

                        case Mark.heart:
                            switch (number)
                            {
                                default:
                                case 1:
                                    return Resources.h01;

                                case 2:
                                    return Resources.h02;

                                case 3:
                                    return Resources.h03;

                                case 4:
                                    return Resources.h04;

                                case 5:
                                    return Resources.h05;

                                case 6:
                                    return Resources.h06;

                                case 7:
                                    return Resources.h07;

                                case 8:
                                    return Resources.h08;

                                case 9:
                                    return Resources.h09;

                                case 10:
                                    return Resources.h10;

                                case 11:
                                    return Resources.h11;

                                case 12:
                                    return Resources.h12;

                                case 13:
                                    return Resources.h13;
                            }

                        case Mark.clover:
                            switch (number)
                            {
                                default:
                                case 1:
                                    return Resources.c01;

                                case 2:
                                    return Resources.c02;

                                case 3:
                                    return Resources.c03;

                                case 4:
                                    return Resources.c04;

                                case 5:
                                    return Resources.c05;

                                case 6:
                                    return Resources.c06;

                                case 7:
                                    return Resources.c07;

                                case 8:
                                    return Resources.c08;

                                case 9:
                                    return Resources.c09;

                                case 10:
                                    return Resources.c10;

                                case 11:
                                    return Resources.c11;

                                case 12:
                                    return Resources.c12;

                                case 13:
                                    return Resources.c13;
                            }

                        case Mark.diamond:
                            switch (number)
                            {
                                default:
                                case 1:
                                    return Resources.d01;

                                case 2:
                                    return Resources.d02;

                                case 3:
                                    return Resources.d03;

                                case 4:
                                    return Resources.d04;

                                case 5:
                                    return Resources.d05;

                                case 6:
                                    return Resources.d06;

                                case 7:
                                    return Resources.d07;

                                case 8:
                                    return Resources.d08;

                                case 9:
                                    return Resources.d09;

                                case 10:
                                    return Resources.d10;

                                case 11:
                                    return Resources.d11;

                                case 12:
                                    return Resources.d12;

                                case 13:
                                    return Resources.d13;
                            }
                    }
                }
                else
                {
                    return Resources.z01;
                }
            }
        }
        #endregion


        //
        //  Method
        //
        #region ・Method
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mark">マーク</param>
        /// <param name="number">数字</param>
        /// <param name="open">表か裏か</param>
        public Card(Mark mark, int number, bool open)
        {
            this.mark = mark;
            this.number = number;
            this.open = open;
        }
        #endregion


        //
        //  IDebugOutput
        //
        #region ・IDebugOutput
        private static string[] markString = { "S", "H", "C", "D" };
        private static string[] numberString = { " 0", " 1", " 2", " 3", " 4", " 5", " 6", " 7", " 8", " 9", "10", " J", " Q", " K" };
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            return sb.Append(markString[(int)mark]).Append(numberString[number]).ToString();
        }
        public void DebugOutput()
        {
            Console.WriteLine(this.ToString());
        }
        #endregion


        //
        //  IFormPanel
        //
        #region ・IFormPanel
        private Panel basePanel;
        public Panel formPanel
        {
            get
            {
                if (basePanel == null)
                {
                    CreatePanel();
                }
                return basePanel;
            }
        }
        private void CreatePanel()
        {
            basePanel = new Panel();
            basePanel.BackgroundImage = image;
            basePanel.BackgroundImageLayout = ImageLayout.Stretch;
            basePanel.Size = new Size(80, 120);
            basePanel.TabIndex = 0;
            basePanel.MouseEnter += new EventHandler(MouseHover);
            basePanel.MouseLeave += new EventHandler(MouseLeave);
        }
        private void UpdatePanel()
        {
            if (basePanel == null) return;
            basePanel.BackgroundImage = image;
        }
        private void MouseHover(object sender, EventArgs e)
        {
        }
        private void MouseLeave(object sender, EventArgs e)
        {
        }
        #endregion
    }
}
