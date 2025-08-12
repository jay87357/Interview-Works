using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;


namespace VueApp1.Server.Models
{
    public static class CaptchaModel
    {
        public static string GenerateCode(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => chars[random.Next(chars.Length)]).ToArray());
        }

        public static Image<Rgba32> DrawCaptchaImage(string code)
        {
            var fontCollection = new FontCollection();
            fontCollection.AddSystemFonts();

            string[] preferredFonts = { "Arial", "微軟正黑體", "Noto Sans", "DejaVu Sans" };
            var matchedFont = fontCollection.Families.FirstOrDefault(f => preferredFonts.Contains(f.Name));

            var font = matchedFont.CreateFont(32, FontStyle.Bold);

            var textOptions = new TextOptions(font)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Dpi = 72
            };

            var textSize = TextMeasurer.MeasureSize(code, textOptions);
            int padding = 20;
            int width = (int)textSize.Width + padding * 2;
            int height = (int)textSize.Height + padding * 2;

            var image = new Image<Rgba32>(width, height);
            var rand = new Random();

            image.Mutate(ctx =>
            {
                // 背景白色
                ctx.Fill(Color.White);

                // 隨機字色
                var textColor = RandomColor(rand);

                // 畫出文字置中
                var location = new PointF(width / 2f, height / 2f);
                var richTextOptions = new RichTextOptions(font)
                {
                    Origin = location,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                ctx.DrawText(richTextOptions, code, textColor);

                // 干擾線條
                int lineCount = rand.Next(5, 11);
                for (int i = 0; i < lineCount; i++)
                {
                    var p1 = new PointF(rand.Next(width), rand.Next(height));
                    var p2 = new PointF(rand.Next(width), rand.Next(height));
                    var color = RandomColor(rand);
                    ctx.DrawLine(color, 3, p1, p2);
                }
            });


            return image;
        }

        private static Color RandomColor(Random rand)
        {
            return Color.FromRgb((byte)rand.Next(30, 200), (byte)rand.Next(30, 200), (byte)rand.Next(30, 200));
        }
    }
}
