# ThoughtWorks.QRCode.Core
####ThoughtWorks.QRCode Core版扩展
ThoughtWorks.QRCode很好用，但是该类库不支持.net core<br/>
作者haoersheng没有留联系方式，于是我通过反编译dll，做了个.net core版本<br/>
如有侵权请联系我删除！！！<br/>
<br/>
代码中必须使用<br/>
qrCodeEncoder.QRCodeScale = 4;<br/>
qrCodeEncoder.QRCodeVersion = 5;<br/>
因为.net core版本中的resource没有完全加载<br/>
有兴趣的同学可以帮我实现完全加载<br/>
源码地址：[https://gitee.com/atalent/ThoughtWorks.QRCode.Core](https://gitee.com/atalent/ThoughtWorks.QRCode.Core)<br/>
有.net版本的sln和.net core版本的sln，运行之后查看//TODO:，你就懂了<br/>
<br/>
以下是代码摘要
```
using System;
using System.Drawing;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace ThoughtWorks.QRCode.Demo
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
            try
            {
                var qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = 4;
                qrCodeEncoder.QRCodeVersion = 5;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                return qrCodeEncoder.Encode(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// 定义参数,生成二维码
        /// </summary>
        public static void Create(string text, string path)
        {
            try
            {
                var image = Encode(text);
                if (image == null)
                    return;
                image.Save(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 返回二维码定义的字符串
        /// </summary>
        public static string Decode(Bitmap image)
        {
            try
            {
                var qrCodeBitmapImage = new QRCodeBitmapImage(image);
                var qrCodeDecoder = new QRCodeDecoder();
                return qrCodeDecoder.decode(qrCodeBitmapImage); ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "#";
            }
        }

        /// <summary>
        /// 返回二维码定义的字符串
        /// </summary>
        public static string Decode(string path)
        {
            return Decode(new Bitmap(path));
        }
    }
}

```

```
using System;

namespace ThoughtWorks.QRCode.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "D:/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            var qr = QrCodeUtil.Encode("Hello World!");
            qr.Save(path);
            Console.WriteLine(QrCodeUtil.Decode(qr));

            path = "D:/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
            QrCodeUtil.Create("中国智造，惠及全球！", path);
            Console.WriteLine(QrCodeUtil.Decode(path));

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}

```

