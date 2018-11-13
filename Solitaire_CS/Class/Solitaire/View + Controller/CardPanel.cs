using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Solitaire
{
    public class CardPanel : Panel
    {
        Card model;

        public CardPanel(Card model)
        {
            this.model = model;
            this.BackgroundImage = GetCardImage(model.mark, model.number, model.open);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Size = new Size(80, 120);
            this.TabIndex = 0;
        }

        public void UpdatePanel()
        {
            this.BackgroundImage = GetCardImage(model.mark, model.number, model.open);
        }

        private static Image GetCardImage(Card.Mark mark, int number, bool isOpen)
        {
            if(!isOpen)
            {
                return Resources.z01;
            }

            return Resources.GetCard(mark, number);
        }
    }
}
