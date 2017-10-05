using System.Drawing;
using System.Drawing.Imaging;
using Evolution.Models;
using System.Collections.Generic;

namespace Evolution.Drawers
{
    public class BoardDrawer
    {
        private Size BoardSize { get; set; }
        private Bitmap BackBitmap { get; set; }
        private Bitmap FrontBitmap { get; set; }

        public BoardDrawer(Size boardSize)
        {
            this.BoardSize = boardSize;
        }

        public void DrawEntities(List<Entity> entities, string statusMessage)
        {
            this.BackBitmap = new Bitmap(this.BoardSize.Width, this.BoardSize.Height, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(this.BackBitmap))
            {
                foreach (var entity in entities)
                {
                    entity.Draw(graphics);
                }

                graphics.DrawString(statusMessage, SystemFonts.CaptionFont, Brushes.LightBlue, 10, 40);
            }
        }

        public void DrawBitmap(Graphics graphics)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    this.FrontBitmap = new Bitmap(this.BackBitmap);
                    graphics.DrawImage(this.FrontBitmap, 0, 0);
                    break;
                }
                catch
                {
                    AutoClosingMessageBox.Show("Error occurred when drawing the world", "Error", 5000);
                }
            }
        }
    }
}
