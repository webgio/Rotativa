using Rotativa.Options;

namespace Rotativa
{
    public abstract class AsImageResultBase : AsResultBase
    {
        /// <summary>
        /// Set height for cropping
        /// </summary>
        [OptionFlag("-f")]
        public ImageFormat? Format { get; set; }

        /// <summary>
        /// Output image quality (between 0 and 100) (default 94)
        /// </summary>
        [OptionFlag("--quality")]
        public int? Quality { get; set; }

        /// <summary>
        /// Set height for cropping
        /// </summary>
        [OptionFlag("--crop-h")]
        public int? CropHeight { get; set; }

        /// <summary>
        /// Set width for cropping
        /// </summary>
        [OptionFlag("--crop-w")]
        public int? CropWidth { get; set; }

        /// <summary>
        /// Set x coordinate for cropping
        /// </summary>
        [OptionFlag("--crop-x")]
        public int? CropX { get; set; }

        /// <summary>
        /// Set y coordinate for cropping
        /// </summary>
        [OptionFlag("--crop-y")]
        public int? CropY { get; set; }

        /// <summary>
        /// Sets the page width.
        /// </summary>
        /// <remarks>Set screen width, note that this is used only as a guide line.</remarks>
        [OptionFlag("--width")]
        public int? PageWidth { get; set; }

        /// <summary>
        /// Sets the page height in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageWidth"/> has to be also specified.</remarks>
        [OptionFlag("--height")]
        public int? PageHeight { get; set; }

        protected override byte[] WkhtmlConvert(string switches)
        {
            return WkhtmltoimageDriver.Convert(this.WkhtmlPath, switches);
        }

        protected override string GetContentType()
        {
            var imageFormat = this.Format;
            if (!imageFormat.HasValue)
            {
                imageFormat = ImageFormat.jpeg;
            }

            return string.Format("image/{0}", imageFormat);
        }
    }
}