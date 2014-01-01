using System;
using System.Web;
using BlogEngine.Core.Web;
using BlogEngine.Core.Web.Controls;
using BlogEngine.Core.Web.HttpHandlers;
using BlogEngine.Core.Web.Extensions;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using BlogEngine.Core;
using System.Globalization;

/// <summary>
///ImageGuard 
/// </summary>
/// 
[Extension("Stops other websites in displaying your images on their own website", "1.0", "<a href=\"http://www.zizhujy.com/BlogEngine/BlogEngine/BlogEngine.NET\" title=\"紫竹叽歪\" target=\"_blank\">Jeff</a>")]
public class ImageGuard
{
    static protected ExtensionSettings settings = null;

    public ImageGuard()
    {
        ImageHandler.Serving += new EventHandler<EventArgs>(ImageGuarding);

        InitSettings();
    }

    private void InitSettings()
    {
        ExtensionSettings initialSettings = new ExtensionSettings(this);
        initialSettings.IsScalar = true;

        // define parameters
        initialSettings.AddParameter("guardingType", "Guarding Type: ", 50, false, false, ParameterType.RadioGroup);
        initialSettings.AddParameter("watermarkText", "Watermark Text");
        initialSettings.AddParameter("watermarkImagePath", "Virtual Path Of Watermark Image");
        initialSettings.AddParameter("blockImagePath", "Virtual Path Of Block Image");

        initialSettings.AddValue("guardingType", new string[] { "Watermark", "Block", "Reject" }, "Watermark");
        initialSettings.AddValue("watermarkText", "text");
        initialSettings.AddValue("watermarkImagePath", "");
        initialSettings.AddValue("blockImagePath", "");

        initialSettings.Help = "Notice: There are 3 types of image guarding. Watermark is to add an watermark information(such as your blog url) to your images that is requested by other host; Block is to replace the requested image with a specific image, such as an image only shows Not Allowed information on it; Reject is not to respond to the requested image at all. <br />The Watermark Text and Virtual Path Of Watermark Image is for Watermark Guarding Type; Virtual Path Of Block Image for Block; and if you choose Reject Guarding Type, then there is no need to do other settings.";

        settings = ExtensionManager.InitSettings(this.GetType().Name, initialSettings);
    }

    void ImageGuarding(object sender, EventArgs e)
    {
        HttpContext context = HttpContext.Current;
        if (context.Request.UrlReferrer != null)
        {
            if (string.Compare(context.Request.Url.Host.Replace("www.", ""), context.Request.UrlReferrer.Host.Replace("www.", ""), true, CultureInfo.InvariantCulture) != 0)
            {
                string guardingType = settings.GetSingleValue("guardingType");
                if (guardingType.Equals("Reject"))
                {
                    context.Response.StatusCode = 403;
                    context.Response.End();
                }
                else if (guardingType.Equals("Block"))
                {
                    string blockImagePath = settings.GetSingleValue("blockImagePath");
                    var fi = new FileInfo(context.Server.MapPath(blockImagePath));
                    if (fi.Exists)
                    {
                        //context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        //context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));

                        string extension = Path.GetExtension(fi.Name);

                        context.Response.ContentType = string.Compare(extension.ToUpper(), "JPG") == 0 ? "image/jpeg" : string.Format("image/{0}", extension);

                        context.Response.TransmitFile(fi.FullName);
                        context.Response.End();
                    }
                }
                else if (guardingType.Equals("Watermark"))
                {
                    FileInfo originImageFileInfo = GetOriginalImage();
                    if (originImageFileInfo != null)
                    {
                        TextWatermarker txtWatermarker = null;
                        ImageWatermarker imgWatermarker = null;
                        Image originImage = Bitmap.FromFile(originImageFileInfo.FullName);
                        string watermarkText = settings.GetSingleValue("watermarkText");

                        if (!string.IsNullOrWhiteSpace(watermarkText))
                        {
                            txtWatermarker = new TextWatermarker(originImage, watermarkText);
                            txtWatermarker.AddWatermark();
                            originImage = txtWatermarker.WatermarkedImage;
                        }

                        string watermarkImagePath = settings.GetSingleValue("watermarkImagePath");

                        if (!string.IsNullOrWhiteSpace(watermarkImagePath))
                        {
                            FileInfo fi = new FileInfo(context.Server.MapPath(watermarkImagePath));
                            if (fi.Exists)
                            {
                                Image watermarkImage = Bitmap.FromFile(fi.FullName);

                                imgWatermarker = new ImageWatermarker(originImage, watermarkImage);
                                imgWatermarker.Position = WatermarkPostion.BottomLeft;
                                imgWatermarker.AddWatermark();

                                originImage = imgWatermarker.WatermarkedImage;
                            }
                        }

                        // Serving the watermarked image
                        //context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        //context.Response.Cache.SetExpires(DateTime.Now.AddYears(1));

                        string extension = Path.GetExtension(originImageFileInfo.Name);

                        context.Response.ContentType = string.Compare(extension.ToUpper(), "JPG") == 0 ? "image/jpeg" : string.Format("image/{0}", extension);

                        //originImage.Save(context.Response.OutputStream, ImageHelper.GetImageFormatByExtension(extension));
                        MemoryStream mem = new MemoryStream();
                        originImage.Save(mem, ImageHelper.GetImageFormatByExtension(extension));
                        mem.WriteTo(context.Response.OutputStream);
                        mem.Dispose();
                        originImage.Dispose();

                        if (txtWatermarker != null)
                            txtWatermarker.Dispose();
                        if (imgWatermarker != null)
                            imgWatermarker.Dispose();

                        context.Response.End();
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.End();
                    }
                }
                else
                {
                }
            }
            else
            {
                // string.Compare(context.Request.Url.Host, context.Request.UrlReferrer.Host, true, CultureInfo.InvariantCulture) == 0
                // The request host and the referrer host are the same, serve the image normally

                //FileInfo originImageFileInfo = GetOriginalImage();
                //if (originImageFileInfo != null)
                //{
                //    TextWatermarker txtWatermarker = null;
                //    Image originImage = Bitmap.FromFile(originImageFileInfo.FullName);
                //    string watermarkText = context.Request.Url.Host;

                //    if (!string.IsNullOrWhiteSpace(watermarkText))
                //    {
                //        txtWatermarker = new TextWatermarker(originImage, watermarkText);
                //        txtWatermarker.Position = WatermarkPostion.TopLeft;
                //        txtWatermarker.AddWatermark();
                //        originImage = txtWatermarker.WatermarkedImage;
                //    }

                //    watermarkText = context.Request.UrlReferrer.Host;

                //    if (!string.IsNullOrWhiteSpace(watermarkText))
                //    {
                //        txtWatermarker = new TextWatermarker(originImage, watermarkText);
                //        txtWatermarker.AddWatermark();
                //        originImage = txtWatermarker.WatermarkedImage;                        
                //    }

                //    string extension = Path.GetExtension(originImageFileInfo.Name);

                //    context.Response.ContentType = string.Compare(extension.ToUpper(), "JPG") == 0 ? "image/jpeg" : string.Format("image/{0}", extension);

                //    //originImage.Save(context.Response.OutputStream, ImageHelper.GetImageFormatByExtension(extension));
                //    MemoryStream mem = new MemoryStream();
                //    originImage.Save(mem, ImageHelper.GetImageFormatByExtension(extension));
                //    mem.WriteTo(context.Response.OutputStream);
                //    mem.Dispose();
                //    originImage.Dispose();

                //    if (txtWatermarker != null)
                //        txtWatermarker.Dispose();

                //    context.Response.End();
                //}
                //else
                //{
                //    context.Response.StatusCode = 404;
                //    context.Response.End();
                //}
            }
        }
    }

    public FileInfo GetOriginalImage()
    {
        HttpContext context = HttpContext.Current;

        if (!string.IsNullOrEmpty(context.Request.QueryString["picture"]))
        {
            var fileName = context.Request.QueryString["picture"];
            try
            {
                var folder = string.Format("{0}/files/", Blog.CurrentInstance.StorageLocation);
                var fi = new FileInfo(context.Server.MapPath(folder) + fileName);

                if (fi.Exists &&
                    fi.Directory.FullName.ToUpperInvariant().Contains(string.Format("{0}FILES", Path.DirectorySeparatorChar)))
                {
                    
                    //if (Utils.SetConditionalGetHeaders(fi.CreationTimeUtc))
                    //{
                    //    return null;
                    //}

                    return fi;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return new FileInfo(context.Server.MapPath(string.Format("{0}/files/", Blog.CurrentInstance.StorageLocation)) + "1.gif");
            }
        }
        else
        {
            return null;
        }
    }
}

/// <summary>
/// Watermarker Template class
/// </summary>
public abstract class Watermarker : IDisposable
{
    #region Properties

    private Image originImage;
    public Image OriginImage
    {
        get
        {
            return originImage;
        }
        set
        {
            originImage = value;
        }
    }

    private Image watermarkedImage;
    public Image WatermarkedImage
    {
        get
        {
            return watermarkedImage;
        }
        set
        {
            watermarkedImage = value;
        }
    }

    private SizeF watermarkSize;
    protected SizeF WatermarkSize
    {
        get
        {
            return watermarkSize;
        }
        set
        {
            watermarkSize = value;
        }
    }

    private float x;
    protected float X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
        }
    }

    private float y;
    protected float Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }

    private bool computeX = true;
    public bool ComputeX
    {
        get { return computeX; }
    }

    private bool computeY = true;
    public bool ComputeY
    {
        get { return computeY; }
    }

    private WatermarkHorizontalPostion hPos;
    public WatermarkHorizontalPostion HorizontalPosition
    {
        get
        {
            return hPos;
        }
        set
        {
            computeX = true;
            hPos = value;
        }
    }

    private WatermarkVerticalPostion vPos;
    public WatermarkVerticalPostion VerticalPosition
    {
        get
        {
            return vPos;
        }
        set
        {
            computeY = true;
            vPos = value;
        }
    }

    public WatermarkPostion Position
    {
        get
        {
            return GetPosition(this.HorizontalPosition, this.VerticalPosition);
        }
        set
        {
            this.HorizontalPosition = GetHorizentalPosition(value);
            this.VerticalPosition = GetVerticalPosition(value);
        }
    }

    private int hMarginPixel;
    public int HorizontalMarginPixel
    {
        get
        {
            return hMarginPixel;
        }
        set
        {
            hMarginPixel = value;
        }
    }

    private int vMarginPixel;
    public int VerticalMarginPixel
    {
        get
        {
            return vMarginPixel;
        }
        set
        {
            vMarginPixel = value;
        }
    }

    public double HorizontalMarginPercent
    {
        get
        {
            return this.HorizontalMarginPixel / this.OriginImage.Width;
        }
        set
        {
            double percent = value > 1 ? 1 : (value < 0 ? 0 : value);
            this.HorizontalMarginPixel = (int)(this.OriginImage.Width * percent);
        }
    }

    public double VerticalMarginPercent
    {
        get
        {
            return this.VerticalMarginPixel / this.OriginImage.Height;
        }
        set
        {
            double percent = value > 1 ? 1 : (value < 0 ? 0 : value);
            this.VerticalMarginPixel = (int)(this.originImage.Height * percent);
        }
    }

    private Color shadowColor;
    public Color ShadowColor
    {
        get
        {
            return shadowColor;
        }
        set
        {
            shadowColor = value;
        }
    }

    private Color foreColor;
    public Color ForeColor
    {
        get
        {
            return foreColor;
        }
        set
        {
            foreColor = value;
        }
    }

    public double ShadowOpacity
    {
        get
        {
            return shadowColor.A / 255;
        }
        set
        {
            double percent = value > 1 ? 1 : (value < 0 ? 0 : value);
            int alpha = (int)(255 * percent);

            shadowColor = Color.FromArgb(alpha, shadowColor);
        }
    }

    public virtual double ForegroundOpacity
    {
        get
        {
            return foreColor.A / 255;
        }
        set
        {
            double percent = value > 1 ? 1 : (value < 0 ? 0 : value);
            int alpha = (int)(255 * percent);

            foreColor = Color.FromArgb(alpha, foreColor);
        }
    }

    private int shadowOffsetX;
    public int ShadowOffsetX
    {
        get
        {
            return shadowOffsetX;
        }
        set
        {
            shadowOffsetX = value;
        }
    }

    private int shadowOffsetY;
    public int ShadowOffsetY
    {
        get
        {
            return shadowOffsetY;
        }
        set
        {
            shadowOffsetY = value;
        }
    }

    #endregion

    #region Contructor

    public Watermarker(Image originImage)
    {
        this.OriginImage = originImage;
        // 默认值
        this.ShadowColor = Color.FromArgb(153, 0, 0, 0);
        this.ForeColor = Color.FromArgb(153, 255, 255, 255);
        this.ShadowOffsetX = 1;
        this.ShadowOffsetY = 1;
        this.Position = WatermarkPostion.BottomRight;
        this.HorizontalMarginPixel = 10;
        this.VerticalMarginPixel = 10;
    }

    #endregion

    #region Methods

    public void SetX(float x)
    {
        this.X = x;
        computeX = false;
    }

    public void SetY(float y)
    {
        this.Y = y;
        computeY = false;
    }

    public void SetXY(float x, float y)
    {
        SetX(x);
        SetY(y);
    }

    /// <summary>
    /// Add water mark to original image
    /// </summary>
    public Image AddWatermark()
    {
        ComputeWatermarkSize();
        UpdateXY();
        watermarkedImage = AddWatermarkToOriginImage();

        return watermarkedImage;
    }

    /// <summary>
    /// hook
    /// </summary>
    /// <returns></returns>
    protected virtual SizeF ComputeWatermarkSize()
    {
        this.WatermarkSize = new SizeF(100f, 50f);
        return this.WatermarkSize;
    }

    /// <summary>
    /// hook
    /// </summary>
    protected virtual Image AddWatermarkToOriginImage()
    {
        return originImage;
    }

    /// <summary>
    /// hook
    /// </summary>
    protected virtual void UpdateXY()
    {
        if (this.ComputeX)
        {
            switch (this.HorizontalPosition)
            {
                case WatermarkHorizontalPostion.Left:
                    this.X = this.HorizontalMarginPixel;
                    break;

                case WatermarkHorizontalPostion.Center:
                    this.X = this.OriginImage.Width / 2 - this.WatermarkSize.Width / 2;
                    break;

                case WatermarkHorizontalPostion.Right:
                    this.X = (this.OriginImage.Width - this.HorizontalMarginPixel - this.WatermarkSize.Width);
                    break;

                default:
                    break;
            }
        }

        if (this.ComputeY)
        {
            switch (this.VerticalPosition)
            {
                case WatermarkVerticalPostion.Top:
                    this.Y = this.VerticalMarginPixel;
                    break;

                case WatermarkVerticalPostion.Middle:
                    this.Y = this.OriginImage.Height / 2 - this.WatermarkSize.Height / 2;
                    break;

                case WatermarkVerticalPostion.Bottom:
                    this.Y = (this.OriginImage.Height - this.VerticalMarginPixel - this.WatermarkSize.Height);
                    break;

                default:
                    break;
            }
        }
    }

    private static WatermarkHorizontalPostion GetHorizentalPosition(WatermarkPostion pos)
    {
        // Mask: 000 111
        return (WatermarkHorizontalPostion)((char)pos & (char)0x07);
    }

    private static WatermarkVerticalPostion GetVerticalPosition(WatermarkPostion pos)
    {
        // Mask: 111 000
        return (WatermarkVerticalPostion)((char)pos & (char)0x38);
    }

    private static WatermarkPostion GetPosition(WatermarkHorizontalPostion hPos, WatermarkVerticalPostion vPos)
    {
        return (WatermarkPostion)((char)hPos | (char)vPos);
    }

    #endregion

    #region IDisposable 成员

    public virtual void Dispose()
    {
        this.OriginImage.Dispose();
        this.WatermarkedImage.Dispose();
    }

    #endregion
}

public enum WatermarkPostion
{
    TopLeft = 0x24,         // 100 100
    TopCenter = 0x22,       // 100 010
    TopRight = 0x21,        // 100 001
    MiddleLeft = 0x14,      // 010 100
    MiddleCenter = 0x12,    // 010 010
    MiddleRight = 0x11,     // 010 001    
    BottomLeft = 0x0C,      // 001 100
    BottomCenter = 0x0A,    // 001 010
    BottomRight = 0x09,     // 001 001
}

public enum WatermarkHorizontalPostion
{
    Left = 0x04,            // 000 100
    Center = 0x02,          // 000 010
    Right = 0x01,           // 000 001
}

public enum WatermarkVerticalPostion
{
    Top = 0x20,             // 100 000
    Middle = 0x10,          // 010 000
    Bottom = 0x08,          // 001 000
}

public class ImageWatermarker : Watermarker
{
    #region Properties

    private Image watermarkImage;
    public Image WatermarkImage
    {
        get
        {
            return watermarkImage;
        }
        set
        {
            watermarkImage = value;
        }
    }

    // 自动计算最合适的水印图片大小
    private bool autoSize = true;
    public bool AutoSize
    {
        get { return autoSize; }
        set
        {
            autoSize = value;
        }
    }

    private ImageAttributes imageAttributes = new ImageAttributes();
    public ImageAttributes ImageAttributes
    {
        get
        {
            return imageAttributes;
        }
        set
        {
            imageAttributes = value;
        }
    }

    private List<ColorMap> colorMaps = new List<ColorMap>();
    public List<ColorMap> ColorMaps
    {
        get
        {
            return colorMaps;
        }
        set { colorMaps = value; }
    }

    public float[][] ColorMatrixElements
    {
        get
        {
            if (this.ColorMatrix != null)
            {
                return new float[][] {
                        new float[] {this.ColorMatrix.Matrix00, this.ColorMatrix.Matrix01, this.ColorMatrix.Matrix02, this.ColorMatrix.Matrix03, this.ColorMatrix.Matrix04},
                        new float[] {this.ColorMatrix.Matrix10, this.ColorMatrix.Matrix11, this.ColorMatrix.Matrix12, this.ColorMatrix.Matrix13, this.ColorMatrix.Matrix14},
                        new float[] {this.ColorMatrix.Matrix20, this.ColorMatrix.Matrix21, this.ColorMatrix.Matrix22, this.ColorMatrix.Matrix23, this.ColorMatrix.Matrix24},
                        new float[] {this.ColorMatrix.Matrix30, this.ColorMatrix.Matrix31, this.ColorMatrix.Matrix32, this.ColorMatrix.Matrix33, this.ColorMatrix.Matrix34},
                        new float[] {this.ColorMatrix.Matrix40, this.ColorMatrix.Matrix41, this.ColorMatrix.Matrix42, this.ColorMatrix.Matrix43, this.ColorMatrix.Matrix44}
                    };
            }
            else
            {
                return null;
            }
        }
        set
        {
            this.ColorMatrix.Matrix00 = value[0][0];
            this.ColorMatrix.Matrix01 = value[0][1];
            this.ColorMatrix.Matrix02 = value[0][2];
            this.ColorMatrix.Matrix03 = value[0][3];
            this.ColorMatrix.Matrix04 = value[0][4];
            this.ColorMatrix.Matrix10 = value[1][0];
            this.ColorMatrix.Matrix11 = value[1][1];
            this.ColorMatrix.Matrix12 = value[1][2];
            this.ColorMatrix.Matrix13 = value[1][3];
            this.ColorMatrix.Matrix14 = value[1][4];
            this.ColorMatrix.Matrix20 = value[2][0];
            this.ColorMatrix.Matrix21 = value[2][1];
            this.ColorMatrix.Matrix22 = value[2][2];
            this.ColorMatrix.Matrix23 = value[2][3];
            this.ColorMatrix.Matrix24 = value[2][4];
            this.ColorMatrix.Matrix30 = value[3][0];
            this.ColorMatrix.Matrix31 = value[3][1];
            this.ColorMatrix.Matrix32 = value[3][2];
            this.ColorMatrix.Matrix33 = value[3][3];
            this.ColorMatrix.Matrix34 = value[3][4];
            this.ColorMatrix.Matrix40 = value[4][0];
            this.ColorMatrix.Matrix41 = value[4][1];
            this.ColorMatrix.Matrix42 = value[4][2];
            this.ColorMatrix.Matrix43 = value[4][3];
            this.ColorMatrix.Matrix44 = value[4][4];
        }
    }

    private ColorMatrix colorMatrix;
    public ColorMatrix ColorMatrix
    {
        get
        {
            return colorMatrix;
        }
        set
        {
            colorMatrix = value;
        }
    }

    private bool keepScale;
    public bool KeepScale
    {
        get { return keepScale; }
        set { keepScale = value; }
    }

    public override double ForegroundOpacity
    {
        get
        {
            return base.ForegroundOpacity;
        }
        set
        {
            double percent = value > 1 ? 1 : (value < 0 ? 0 : value);

            base.ForegroundOpacity = percent;
            this.ColorMatrix.Matrix33 = (float)percent;
        }
    }

    #endregion

    #region Constructor

    public ImageWatermarker(Image originImage, Image watermarkImage)
        : base(originImage)
    {
        this.WatermarkImage = watermarkImage;

        // Default values
        this.ColorMaps.Add(new ColorMap());
        this.ColorMaps[0].OldColor = Color.FromArgb(255, 255, 255, 255);
        this.ColorMaps[0].NewColor = Color.FromArgb(0, 0, 0, 0);

        float[][] colorMatrixElements = {
                new float[]{1.0f, 0.0f, 0.0f, 0.0f, 0.0f},
                new float[]{0.0f, 1.0f, 0.0f, 0.0f, 0.0f},
                new float[]{0.0f, 0.0f, 1.0f, 0.0f, 0.0f},
                new float[]{0.0f, 0.0f, 0.0f, 0.3f, 0.0f},
                new float[]{0.0f, 0.0f, 0.0f, 0.0f, 1.0f}   
            };
        this.ColorMatrix = new ColorMatrix(colorMatrixElements);
        this.WatermarkSize = this.WatermarkImage.Size;
        this.KeepScale = true;
    }

    #endregion

    #region Methods

    protected override SizeF ComputeWatermarkSize()
    {
        if (this.AutoSize)
        {
            this.SetWatermarkWidth((int)(this.OriginImage.Width * 0.2));
        }
        else
        {
        }
        return this.WatermarkSize;
    }

    protected override Image AddWatermarkToOriginImage()
    {
        this.WatermarkedImage = this.OriginImage;
        Graphics watermarkGraphic = Graphics.FromImage(this.WatermarkedImage);

        this.ImageAttributes.SetRemapTable(this.ColorMaps.ToArray(), ColorAdjustType.Bitmap);
        this.ImageAttributes.SetColorMatrix(this.ColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        watermarkGraphic.DrawImage(this.WatermarkImage,
            new Rectangle((int)this.X, (int)this.Y, (int)this.WatermarkSize.Width, (int)this.WatermarkSize.Height),
            0, 0, this.WatermarkImage.Width, this.WatermarkImage.Height,
            GraphicsUnit.Pixel, this.ImageAttributes);

        watermarkGraphic.Dispose();
        imageAttributes.Dispose();

        return this.WatermarkedImage;
    }

    public void SetWatermarkWidth(int width)
    {
        float height = this.WatermarkSize.Height;
        if (this.KeepScale)
        {
            height = height * width / this.WatermarkSize.Width;
        }

        this.WatermarkSize = new SizeF(width, height);
    }

    public void SetWatermarkHeight(int height)
    {
        float width = this.WatermarkSize.Width;
        if (this.KeepScale)
        {
            width = width * height / this.WatermarkSize.Height;
        }

        this.WatermarkSize = new SizeF(width, height);
    }

    public void SetWatermarkSize(int width, int height)
    {
        this.WatermarkSize = new SizeF(width, height);
        this.KeepScale = false;
    }

    public override void Dispose()
    {
        base.Dispose();
        this.WatermarkImage.Dispose();
    }

    #endregion
}

public class TextWatermarker : Watermarker
{
    #region Properties

    private string watermarkText;
    public string WatermarkText
    {
        get
        {
            return watermarkText;
        }
        set
        {
            watermarkText = value;
        }
    }

    // 自动计算最合适的水印文本字体大小
    private bool autoSize = true;
    public bool AutoSize
    {
        get
        {
            return autoSize;
        }
        set
        {
            autoSize = value;
        }
    }

    private double fontSizePercent;
    public double FontSizePercent
    {
        get { return fontSizePercent; }
        set
        {
            fontSizePercent = value > 1 ? 1 : (value <= 0.01 ? 0.01 : value);
        }
    }

    private List<Font> fonts = new List<Font>();
    public List<Font> Fonts
    {
        get
        {
            return fonts;
        }
        set
        {
            fonts = value;
        }
    }

    private Font bestFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
    public Font BestFont
    {
        get
        {
            return bestFont;
        }
        set
        {
            bestFont = value;
        }
    }

    private StringFormat StringFormat
    {
        get
        {
            StringFormat format = new StringFormat();
            if (this.ComputeX)
            {
                switch (this.HorizontalPosition)
                {
                    case WatermarkHorizontalPostion.Left:
                        format.Alignment = StringAlignment.Near;
                        break;

                    case WatermarkHorizontalPostion.Center:
                        format.Alignment = StringAlignment.Center;
                        break;

                    case WatermarkHorizontalPostion.Right:
                        format.Alignment = StringAlignment.Far;
                        break;
                }
            }
            else
            {
                format.Alignment = StringAlignment.Near;
            }

            return format;
        }
    }

    #endregion

    #region Constructor

    public TextWatermarker(Image originImage, string watermarkText)
        : base(originImage)
    {
        this.WatermarkText = watermarkText;

        // Default values
        this.Fonts.Add(new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel));
        this.Fonts.Add(new Font("Arial", 16, FontStyle.Regular, GraphicsUnit.Pixel));
        this.Fonts.Add(new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Pixel));
        this.Fonts.Add(new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel));
        this.Fonts.Add(new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel));
        this.Fonts.Add(new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Pixel));
        this.Fonts.Add(new Font("Arial", 6, FontStyle.Regular, GraphicsUnit.Pixel));
        this.Fonts.Add(new Font("Arial", 4, FontStyle.Regular, GraphicsUnit.Pixel));
        this.FontSizePercent = 30.00 / 480.00;
        this.AutoSize = true;
    }

    #endregion

    #region Methods

    public void ChangeFont(string fontFamily, bool bold, bool italic, bool underline, bool strikeThrough)
    {
        FontStyle fontStyle = FontStyle.Regular;

        if (bold) fontStyle = fontStyle | FontStyle.Bold;
        if (italic) fontStyle = fontStyle | FontStyle.Italic;
        if (underline) fontStyle = fontStyle | FontStyle.Underline;
        if (strikeThrough) fontStyle = fontStyle | FontStyle.Strikeout;

        ChangeFont(fontFamily, fontStyle);
    }

    public void ChangeFontFamily(string fontFamily)
    {
        FontFamily ff = new FontFamily(fontFamily);
        ChangeFontFamily(ff);
    }

    public void ChangeFont(string fontFamily, FontStyle fontStyle)
    {
        for (int i = 0; i < this.Fonts.Count; i++)
        {
            this.Fonts[i] = new Font(fontFamily, this.Fonts[i].Size, fontStyle, this.Fonts[i].Unit, this.Fonts[i].GdiCharSet, this.Fonts[i].GdiVerticalFont);
        }
        this.BestFont = new Font(fontFamily, this.BestFont.Size, fontStyle, this.BestFont.Unit, this.BestFont.GdiCharSet, this.BestFont.GdiVerticalFont);
    }

    public void ChangeFont(FontFamily fontFamily, FontStyle fontStyle)
    {
        for (int i = 0; i < this.Fonts.Count; i++)
        {
            this.Fonts[i] = new Font(fontFamily, this.Fonts[i].Size, fontStyle, this.Fonts[i].Unit, this.Fonts[i].GdiCharSet, this.Fonts[i].GdiVerticalFont);
        }
        this.BestFont = new Font(fontFamily, this.BestFont.Size, fontStyle, this.BestFont.Unit, this.BestFont.GdiCharSet, this.BestFont.GdiVerticalFont);
    }

    public void ChangeFontFamily(Font font)
    {
        for (int i = 0; i < this.Fonts.Count; i++)
        {
            this.Fonts[i] = new Font(font.FontFamily, this.Fonts[i].Size, font.Style, this.Fonts[i].Unit, this.Fonts[i].GdiCharSet, this.Fonts[i].GdiVerticalFont);
        }
        this.BestFont = new Font(font.FontFamily, this.BestFont.Size, font.Style, this.BestFont.Unit, this.BestFont.GdiCharSet, this.BestFont.GdiVerticalFont);
    }

    public void ChangeFontFamily(FontFamily fontFamily)
    {
        for (int i = 0; i < this.Fonts.Count; i++)
        {
            this.Fonts[i] = new Font(fontFamily, this.Fonts[i].Size, this.Fonts[i].Style, this.Fonts[i].Unit, this.Fonts[i].GdiCharSet, this.Fonts[i].GdiVerticalFont);
        }
        this.BestFont = new Font(fontFamily, this.BestFont.Size, this.BestFont.Style, this.BestFont.Unit, this.BestFont.GdiCharSet, this.BestFont.GdiVerticalFont);
    }

    protected override SizeF ComputeWatermarkSize()
    {
        FindAvailableMaxSizedFont();
        return this.WatermarkSize;
    }

    protected override void UpdateXY()
    {
        if (this.ComputeX)
        {
            switch (this.HorizontalPosition)
            {
                case WatermarkHorizontalPostion.Left:
                    this.X = this.HorizontalMarginPixel;
                    break;

                case WatermarkHorizontalPostion.Center:
                    this.X = this.OriginImage.Width / 2;
                    break;

                case WatermarkHorizontalPostion.Right:
                    this.X = (this.OriginImage.Width - this.HorizontalMarginPixel);
                    break;

                default:
                    break;
            }
        }

        if (this.ComputeY)
        {
            switch (this.VerticalPosition)
            {
                case WatermarkVerticalPostion.Top:
                    this.Y = this.VerticalMarginPixel;
                    break;

                case WatermarkVerticalPostion.Middle:
                    this.Y = this.OriginImage.Height / 2 - this.WatermarkSize.Height / 2;
                    break;

                case WatermarkVerticalPostion.Bottom:
                    this.Y = (this.OriginImage.Height - this.VerticalMarginPixel - this.WatermarkSize.Height);
                    break;

                default:
                    break;
            }
        }
    }

    protected override Image AddWatermarkToOriginImage()
    {
        this.WatermarkedImage = this.OriginImage;
        if (this.BestFont != null)
        {
            Graphics g = Graphics.FromImage(this.WatermarkedImage);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            SolidBrush semiTransBrushShadow = new SolidBrush(this.ShadowColor);

            StringFormat format = this.StringFormat;
            g.DrawString(this.WatermarkText, this.BestFont, semiTransBrushShadow, new PointF(this.X + this.ShadowOffsetX, this.Y + this.ShadowOffsetY), format);
            SolidBrush semiTransBrush = new SolidBrush(this.ForeColor);
            g.DrawString(this.WatermarkText, this.BestFont, semiTransBrush, new PointF(this.X, this.Y), format);
            g.Dispose();
        }
        return this.WatermarkedImage;
    }

    private Font FindAvailableMaxSizedFont()
    {
        if (this.AutoSize)
        {
            this.BestFont = FindAvailableMaxSizedFont(false);
            if (this.BestFont != null)
            {
                if (this.BestFont.Size >= 16)
                {
                    return this.BestFont;
                }
                else
                {
                    return FindAvailableMaxSizedFont(true);
                }
            }
            else
            {
                return null;
            }
        }
        else
        {
            return FindAvailableMaxSizedFont(true);
        }
    }

    private Font FindAvailableMaxSizedFont(bool byAbsoluteSizes)
    {
        if (byAbsoluteSizes)
        {
            this.Fonts.Sort(delegate(Font font1, Font font2) { return font2.Size.CompareTo(font1.Size); });
            SizeF size = new SizeF();
            Bitmap image = new Bitmap(1, 1);
            Graphics g = Graphics.FromImage(image);

            for (int i = 0; i < this.Fonts.Count; i++)
            {
                //size = MeasureSize(this.WatermarkText, this.Fonts[i]);
                size = g.MeasureString(this.WatermarkText, this.Fonts[i]);
                if ((ushort)(size.Width + this.HorizontalMarginPixel * 2) < (ushort)this.OriginImage.Width && (ushort)(size.Height + this.VerticalMarginPixel * 2) < (ushort)this.OriginImage.Height)
                {
                    g.Dispose();
                    image.Dispose();
                    this.WatermarkSize = size;
                    this.BestFont = this.fonts[i];
                    return this.Fonts[i];
                }
            }

            g.Dispose();
            image.Dispose();

            this.WatermarkSize = new SizeF(0, 0);
            return null;
        }
        else
        {
            int fontSize = (int)(this.OriginImage.Height * this.FontSizePercent);
            fontSize = fontSize >= 1 ? fontSize : 1;
            this.BestFont = new Font(this.BestFont.FontFamily, fontSize, this.BestFont.Style, this.BestFont.Unit);
            this.WatermarkSize = MeasureSize(this.WatermarkText, this.BestFont);
            if ((ushort)(this.WatermarkSize.Width + this.HorizontalMarginPixel * 2) < (ushort)this.OriginImage.Width && (ushort)(this.WatermarkSize.Height + this.VerticalMarginPixel * 2) < (ushort)this.OriginImage.Height)
            {
                return this.BestFont;
            }
            else
            {
                this.WatermarkSize = new SizeF(0, 0);
                return null;
            }
        }
    }

    private SizeF MeasureSize(string text, Font font)
    {
        // Use a test image to measure the text.
        Bitmap image = new Bitmap(1, 1);
        Graphics g = Graphics.FromImage(image);
        SizeF size = g.MeasureString(text, font);
        g.Dispose();
        image.Dispose();

        return size;
    }

    #endregion
}

public static class ImageHelper
{
    public static ImageFormat GetImageFormatByExtension(string extension)
    {
        ImageFormat format = null;
        switch (extension.Replace(".", "").ToLower())
        {
            case "png":
                format = ImageFormat.Png;
                break;

            case "jpg":
            case "jpeg":
                format = ImageFormat.Jpeg;
                break;

            case "gif":
                format = ImageFormat.Gif;
                break;

            case "ico":
                format = ImageFormat.Icon;
                break;

            case "bmp":
                format = ImageFormat.Bmp;
                break;

            case "tiff":
                format = ImageFormat.Tiff;
                break;

            default:
                break;
        }

        return format;
    }
}