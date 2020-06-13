/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/10/20 20:43:53
** desc:  SpriteAtlas工具;
*********************************************************************************/

using UnityEngine;
using UnityEngine.U2D;

namespace FrameworkEditor
{
    public class SpriteAtlasHelper
    {
        /*
        SpriteAtlas spriteAtlas = new SpriteAtlas();
        //选中以在构建中包含Atlas Asset。请注意，取消选中此选项会导致在播放模式期间不呈现任何打包的资产。
        //会导致打包冗余  取消后需要在代码中监听SpriteAtlasManager.atlasRequested事件来在程序中加载贴图集。
        //不然会导致AB包首个加载图片为空白 ResourceLoad没问题
        spriteAtlas.SetIncludeInBuild(false);
        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
        {
            //包偏移
            blockOffset = 1,
            //打包时是否支持包旋转
            enableRotation = false,
            //是否紧密打包
            enableTightPacking = false,
            //边界填充值
            padding = 2
        };
        spriteAtlas.SetPackingSettings(packSetting);

        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
        {
            //是否开启读写
            readable = false,
            //Point分离过滤模式 点过滤 - 纹理像素变得块状近距离。
            //Bilinear双线性过滤 - 纹理样本的平均值。
            //Trilinear三线性过滤 - 对纹理样本进行平均，并在mipmap级别之间进行混合。
            filterMode = FilterMode.Bilinear,
            //是否启用mipmap
            generateMipMaps = false,
            //纹理存储在伽马空间中。
            sRGB = true
        };

        spriteAtlas.SetTextureSettings(textureSettings);


        TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings()
        {
            //设置类型名字
            name = "Android",
            //是否重写
            overridden = true,
            //在不支持ETC2的手机上回退
            androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality16Bit,
            maxTextureSize = 1024,
            //压缩格式 用之前人家测试最好的手游压缩格式
            format = TextureImporterFormat.ASTC_RGBA_4x4,
            //纹理不会被压缩。未压缩的= 0,
            //摘要纹理将根据平台// (DXT, ASTC，…)使用标准格式进行压缩。Compressed= 1,
            //摘要纹理将根据平台和可用性(BC7, ASTC4x4，…)使用高质量的格式进行压缩。CompressedHQ = 2,
            //摘要根据平台和可用性(2bpp PVRTC, ASTC8x8，…)，纹理将使用低质量但高性能、高压缩/格式进行压缩。CompressedLQ = 3
            textureCompression = TextureImporterCompression.CompressedHQ,
            //可使用时压缩
            crunchedCompression = true,
            //压缩纹理质量[0-100]
            compressionQuality = 50,
        };
        spriteAtlas.SetPlatformSettings(textureImporterPlatformSettings);
        spriteAtlas.Add(Selection.objects);

        AssetDatabase.CreateAsset(spriteAtlas, atlasPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        */
    }
}
