using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHH.Common
{
    public class QHHEmbedBuilder
    {
        private string title;
        private string description;
        private string footer;
        private string image;
        private EmbedStyle style;

        public string Title
        {
            get => this.Title;
            set
            {
                if (value?.Length > 256)
                {
                    throw new ArgumentException(message: $"Title length must be less than or equal to 256.", paramName: nameof(this.Title));
                }

                this.title = value;
            }
        }

        public string Description
        {
            get => this.description;
            set
            {
                if (value?.Length > 2048)
                {
                    throw new ArgumentException(message: $"Description length must be less than or qeual to 2048.", paramName: nameof(this.Description));
                }

                this.description = value;
            }
        }

        public string Footer
        {
            get => this.footer;
            set
            {
                if(value?.Length > 2048)
                {
                    throw new ArgumentException(message: $"Footer length must be less than or equal to 2048.", paramName: nameof(this.Footer));
                }

                this.footer = value;
            }
        }
        public string Image
        {
            get => this.Image;
            set
            {
                if (value?.Length > 256)
                {
                    throw new ArgumentException(message: $"Image URL must be less than or equal to 256.", paramName: nameof(this.Title));
                }

                this.image = value;
            }
        }

        public EmbedStyle Style
        {
            get => this.style;
            set
            {
                this.style = value;
            }
        }

        public QHHEmbedBuilder WithTitle(string title)
        {
            this.Title = title;
            return this;
        }

        public QHHEmbedBuilder WithDescription(string description)
        {
            this.Description = description;
            return this;
        }

        public QHHEmbedBuilder WithFooter(string footer)
        {
            this.Footer = footer;
            return this;
        }

        public QHHEmbedBuilder WithImage(string image)
        {
            this.Image = image;
            return this;
        }

        public QHHEmbedBuilder WithStyle(EmbedStyle style)
        {
            this.Style = style;
            return this;
        }

        public Embed Build()
        {
            EmbedBuilder builder = new EmbedBuilder()
                .WithDescription(this.description)
                .WithFooter(this.footer);

            switch (this.style)
            {
                case EmbedStyle.Success:
                    builder
                        .WithColor(Colors.Success)
                        .WithAuthor(x =>
                        {
                            x
                            .WithIconUrl(Icons.Success)
                            .WithName(this.title);
                        });
                    break;

                case EmbedStyle.Error:
                    builder
                        .WithColor(Colors.Error)
                        .WithAuthor(x =>
                        {
                            x
                            .WithIconUrl(Icons.Error)
                            .WithName(this.title);
                        });
                    break;

                case EmbedStyle.Information:
                    builder
                        .WithColor(Colors.Information)
                        .WithAuthor(x =>
                        {
                            x
                            .WithIconUrl(Icons.Information)
                            .WithName(this.title);
                        });
                    break;

                case EmbedStyle.Image:
                    builder
                        .WithTitle(this.title)
                        .WithImageUrl(this.image)
                        .WithColor(Colors.Quest);
                    break;
            }

            return builder.Build();
        }
    }
}
