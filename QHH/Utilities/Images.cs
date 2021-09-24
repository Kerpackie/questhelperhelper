namespace QHH.Utilities
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Discord.WebSocket;

    public class Images
    {
        /// <summary>
        /// Creates the Quest Cape Reward image.
        /// </summary>
        /// <param name="user">User who the image should be targeted at.</param>
        /// <param name="url">URL to use </param>
        /// <returns>An image that gets uploaded to a channel that the command is run in.</returns>
        public async Task<string> CreateQuestCapeImageAsync(SocketGuildUser user, string url = "https://cdn.discordapp.com/attachments/854379120624271380/886730785049161758/3lJqsAX.jpeg")
        {
            var avatar = await FetchImageAsync(user.GetAvatarUrl(size: 2048, format: Discord.ImageFormat.Png) ?? user.GetDefaultAvatarUrl());
            var background = await FetchImageAsync(url);

            background = CropToBanner(background);
            avatar = ClipImageToCircle(avatar);

            var bitmap = avatar as Bitmap;
            bitmap?.MakeTransparent();

            var banner = CopyRegionIntoImage(bitmap, background);
            banner = DrawTextToImage(banner, $"{user.Username}#{user.Discriminator} has achieved their quest cape!", $"Congratulations, {user.Username}!");

            string path = $"{Guid.NewGuid()}.png";
            banner.Save(path);

            return await Task.FromResult(path);
        }

        /// <summary>
        /// Crops the supplied image down to the banner size required.
        /// </summary>
        /// <param name="image">Image to be cropped.</param>
        /// <returns>A cropped image to be used in other functions.</returns>
        private static Bitmap CropToBanner(Image image)
        {
            var originalWidth = image.Width;
            var originalHeight = image.Height;
            var destinationSize = new Size(1100, 450);

            var heightRatio = (float)originalHeight / destinationSize.Height;
            var widthRatio = (float)originalWidth / destinationSize.Width;

            var ratio = Math.Min(heightRatio, widthRatio);

            var heightScale = Convert.ToInt32(destinationSize.Height * ratio);
            var widthScale = Convert.ToInt32(destinationSize.Width * ratio);

            var startX = (originalWidth - widthScale) / 2;
            var startY = (originalHeight - heightScale) / 2;

            var sourceRectangle = new Rectangle(startX, startY, widthScale, heightScale);
            var bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);
            var destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            using var g = Graphics.FromImage(bitmap);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(image, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);

            return bitmap;
        }

        /// <summary>
        /// Clips the avatar image to a circle to be applied to the middle of the image.
        /// </summary>
        /// <param name="image">User or Default Avatar</param>
        /// <returns>A circular image of the users avatar.</returns>
        private Image ClipImageToCircle(Image image)
        {
            Image destination = new Bitmap(image.Width, image.Height, image.PixelFormat);
            var radius = image.Width / 2;
            var x = image.Width / 2;
            var y = image.Height / 2;

            using Graphics g = Graphics.FromImage(destination);
            var r = new Rectangle(x - radius, y - radius, radius * 2, radius * 2);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Brush brush = new SolidBrush(Color.Tomato))
            {
                g.FillRectangle(brush, 0, 0, destination.Width, destination.Height);
            }

            var path = new GraphicsPath();
            path.AddEllipse(r);
            g.SetClip(path);
            g.DrawImage(image, 0, 0);
            return destination;
        }

        /// <summary>
        /// Copy avatar to image
        /// </summary>
        /// <param name="source">Source image</param>
        /// <param name="destination">destination image</param>
        /// <returns>Image with avatar applied.</returns>
        private Image CopyRegionIntoImage(Image source, Image destination)
        {
            using var drD = Graphics.FromImage(destination);

            var x = (destination.Width / 2) - 110;
            var y = (destination.Height / 2) - 155;

            drD.DrawImage(source, x, y, 220, 220);
            return destination;
        }

        /// <summary>
        /// Writes the text onto the image.
        /// </summary>
        /// <param name="image">Image to be written on</param>
        /// <param name="header">Header text</param>
        /// <param name="subheader">Sub Text</param>
        /// <returns>Image with text written on it</returns>
        private Image DrawTextToImage(Image image, string header, string subheader)
        {
            var roboto = new Font("Roboto", 30, FontStyle.Regular);
            var robotoSmall = new Font("Roboto", 23, FontStyle.Regular);

            var brushWhite = new SolidBrush(Color.White);
            var brushGrey = new SolidBrush(ColorTranslator.FromHtml("#B3B3B3"));

            var headerX = image.Width / 2;
            var headerY = (image.Height / 2) + 115;

            var subheaderX = image.Width / 2;
            var subheaderY = (image.Height / 2) + 160;

            var drawFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center,
            };

            using var GrD = Graphics.FromImage(image);
            GrD.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            GrD.DrawString(header, roboto, brushWhite, headerX, headerY, drawFormat);
            GrD.DrawString(subheader, robotoSmall, brushGrey, subheaderX, subheaderY, drawFormat);

            var img = new Bitmap(image);
            return img;
        }

        /// <summary>
        /// Gets the image from the URL 
        /// </summary>
        /// <param name="url">URL To be fetched</param>
        /// <returns>A downloaded image to be used as the basis for the image generator.</returns>
        private async Task<Image> FetchImageAsync(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var backupResponse = await client.GetAsync("https://cdn.discordapp.com/attachments/854379120624271380/886730785049161758/3lJqsAX.jpeg");
                var backupStream = await backupResponse.Content.ReadAsStreamAsync();
                return Image.FromStream(backupStream);
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return Image.FromStream(stream);
        }
    }
}
