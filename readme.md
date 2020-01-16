# ThoughtWorks.QRCode
#### ThoughtWorks.QRCode标准版
ThoughtWorks.QRCode很好用，但是该类库不支持标准库

于是我通过反编译dll，做了个标准版，并依赖引用了System.Drawing.Common

作者haoersheng没有留联系方式，如有侵权请联系我删除！！！  

代码摘要  

```c#
using System.Drawing;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace CommonUtils
{
    /// <summary>
    /// 二维码工具
    /// </summary>
    public static class QrCodeUtil
    {
        /// <summary>
        /// 返回二维码图片
        /// </summary>
        public static Bitmap Encode(string text)
        {
            var qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 6;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            return qrCodeEncoder.Encode(text);
        }

        /// <summary>
        /// 定义参数,生成二维码
        /// </summary>
        public static void Create(string text, string path)
        => Encode(text).Save(path);

        /// <summary>
        /// 返回二维码定义的字符串
        /// </summary>
        public static string Decode(Bitmap image)
        {
            var qrCodeBitmapImage = new QRCodeBitmapImage(image);
            var qrCodeDecoder = new QRCodeDecoder();
            return qrCodeDecoder.decode(qrCodeBitmapImage); ;
        }

        /// <summary>
        /// 返回二维码定义的字符串
        /// </summary>
        public static string Decode(string path)
        => Decode(new Bitmap(path));
    }
}
```


```c#
            var path = "D:/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            var qr = QrCodeUtil.Encode("Hello World!");
            qr.Save(path);
            Console.WriteLine(QrCodeUtil.Decode(qr));

            path = "D:/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            QrCodeUtil.Create("中国智造，惠及全球！", path);
            Console.WriteLine(QrCodeUtil.Decode(path));

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
```
源码

[https://gitee.com/atalent/ThoughtWorks.QRCode](https://gitee.com/atalent/ThoughtWorks.QRCode)

