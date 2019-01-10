﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace Dome
{
    /// <summary>
    /// 二维码工具
    /// </summary>
    public static class QrCodeUtil
    {
        /// <summary>
        /// 枚举资源
        /// </summary>
        public static void ResourceEnum()
        {
            var qrCodeEncoder = new QRCodeEncoder();
            for (int version = 0; version < 50; version++)
                for (int mode = 0; mode < 4; mode++)
                    for (int error = 0; error < 3; error++)
                        for (int scale = 0; scale < 3; scale++)
                        {
                            qrCodeEncoder.QRCodeVersion = version;
                            qrCodeEncoder.QRCodeEncodeMode = (QRCodeEncoder.ENCODE_MODE)mode;
                            qrCodeEncoder.QRCodeErrorCorrect = (QRCodeEncoder.ERROR_CORRECTION)error;
                            qrCodeEncoder.QRCodeScale = scale;
                            Console.WriteLine("version: {0},mode: {1},error: {2},scale: {3}", version, mode, error, scale);
                            qrCodeEncoder.Encode("Hello");
                        }
        }

        /// <summary>
        /// 存储资源
        /// </summary>
        public static void SaveEnum()
        {
            ThoughtWorks.QRCode.Properties.ResourceMap.Save();
        }

        /// <summary>
        /// 返回二维码图片
        /// </summary>
        public static Bitmap Encode(string text)
        {
            var qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 4;
            qrCodeEncoder.QRCodeVersion = 5;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            return qrCodeEncoder.Encode(text);
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
