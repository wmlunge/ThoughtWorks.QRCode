# ThoughtWorks.QRCode.Core

#### 介绍
ThoughtWorks.QRCode的作者不知道是谁，也没有留联系方式（如有侵权请联系我删除）

然后也不支持.net core，于是我通过反编译dll，做了个.net core版本

qrCodeEncoder.QRCodeScale = 4;

qrCodeEncoder.QRCodeVersion = 5;

必须严格这样写，因为.net版本的resource没有完全加载，有兴趣的同学可以帮我实现完全加载

源码地址：https://gitee.com/atalent/ThoughtWorks.QRCode.Core

有.net版本的sln和.net core版本的sln，运行之后查看todo，你就懂了
