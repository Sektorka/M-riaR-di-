using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Maria_Radio.Controls
{
    [Designer(typeof(ImageButtonDesigner)), ToolboxBitmap(typeof(Button))]
    public class ImageButton : Control
    {
        #region... Variables ...

        enum ButtonState
        {
            Normal,
            Hover,
            Pushed,
            Disabled
        }

        private ButtonState mState = ButtonState.Normal;
        private Dictionary<ButtonState, Image> mImages = new Dictionary<ButtonState, Image>();

        #endregion

        #region... Events	   ...

        #endregion

        #region... Construct ...

        public ImageButton()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
        }
        #endregion

        #region... Property  ...

        [DefaultValue(null)]
        public Image NormalImage
        {
            get
            {
                if (!mImages.ContainsKey(ButtonState.Normal))
                    mImages.Add(ButtonState.Normal, null);
                return mImages[ButtonState.Normal];
            }
            set
            {
                mImages[ButtonState.Normal] = value;
                Invalidate();
            }
        }

        [DefaultValue(null)]
        public Image HoverImage
        {
            get
            {
                if (!mImages.ContainsKey(ButtonState.Hover))
                    mImages.Add(ButtonState.Hover, null);
                return mImages[ButtonState.Hover];
            }
            set
            {
                mImages[ButtonState.Hover] = value;
                Invalidate();
            }
        }

        [DefaultValue(null)]
        public Image PushedImage
        {
            get
            {
                if (!mImages.ContainsKey(ButtonState.Pushed))
                    mImages.Add(ButtonState.Pushed, null);
                return mImages[ButtonState.Pushed];
            }
            set
            {
                mImages[ButtonState.Pushed] = value;
                Invalidate();
            }
        }

        [DefaultValue(null)]
        public Image DisabledImage
        {
            get
            {
                if (!mImages.ContainsKey(ButtonState.Disabled))
                    mImages.Add(ButtonState.Disabled, null);
                return mImages[ButtonState.Disabled];
            }
            set
            {
                mImages[ButtonState.Disabled] = value;
                Invalidate();
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(70, 23);
            }
        }
        #endregion

        #region... Method    ...

        private void DrawImage(Graphics g, Image image)
        {
            Rectangle rect = ClientRectangle;

            g.DrawImage(image, rect);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.Clear(BackColor);
            if (mImages.ContainsKey(mState))
            {
                Image image = mImages[mState];
                if (image != null)
                    DrawImage(g, image);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            mState = ButtonState.Normal;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.Left)
            {
                if (mState != ButtonState.Hover)
                {
                    mState = ButtonState.Hover;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mState = ButtonState.Pushed;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            mState = ButtonState.Normal;
            Invalidate();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (!Enabled)
            {
                mState = ButtonState.Disabled;
                Invalidate();
            }
            else
            {
                mState = ButtonState.Normal;
                Invalidate();
            }
        }

        public void PerformClicked()
        {
            OnClick(EventArgs.Empty);
        }
        #endregion

        #region... Interface ...

        #endregion
    }
}
