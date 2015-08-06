using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Maria_Radio.Controls
{
    class ImageButtonDesigner : ControlDesigner
    {
        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            ImageButton button = Control as ImageButton;
            if (button != null && button.NormalImage == null)
            {
                Rectangle rect = pe.ClipRectangle;
                rect.Width--;
                rect.Height--;
                pe.Graphics.DrawRectangle(Pens.Gray, rect);
            }
        }
    }
}
