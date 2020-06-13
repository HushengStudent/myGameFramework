/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2019/10/20 20:43:53
** desc:  SpriteAtlas����;
*********************************************************************************/

using UnityEngine;
using UnityEngine.U2D;

namespace FrameworkEditor
{
    public class SpriteAtlasHelper
    {
        /*
        SpriteAtlas spriteAtlas = new SpriteAtlas();
        //ѡ�����ڹ����а���Atlas Asset����ע�⣬ȡ��ѡ�д�ѡ��ᵼ���ڲ���ģʽ�ڼ䲻�����κδ�����ʲ���
        //�ᵼ�´������  ȡ������Ҫ�ڴ����м���SpriteAtlasManager.atlasRequested�¼����ڳ����м�����ͼ����
        //��Ȼ�ᵼ��AB���׸�����ͼƬΪ�հ� ResourceLoadû����
        spriteAtlas.SetIncludeInBuild(false);
        SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
        {
            //��ƫ��
            blockOffset = 1,
            //���ʱ�Ƿ�֧�ְ���ת
            enableRotation = false,
            //�Ƿ���ܴ��
            enableTightPacking = false,
            //�߽����ֵ
            padding = 2
        };
        spriteAtlas.SetPackingSettings(packSetting);

        SpriteAtlasTextureSettings textureSettings = new SpriteAtlasTextureSettings()
        {
            //�Ƿ�����д
            readable = false,
            //Point�������ģʽ ����� - �������ر�ÿ�״�����롣
            //Bilinear˫���Թ��� - ����������ƽ��ֵ��
            //Trilinear�����Թ��� - ��������������ƽ��������mipmap����֮����л�ϡ�
            filterMode = FilterMode.Bilinear,
            //�Ƿ�����mipmap
            generateMipMaps = false,
            //����洢��٤��ռ��С�
            sRGB = true
        };

        spriteAtlas.SetTextureSettings(textureSettings);


        TextureImporterPlatformSettings textureImporterPlatformSettings = new TextureImporterPlatformSettings()
        {
            //������������
            name = "Android",
            //�Ƿ���д
            overridden = true,
            //�ڲ�֧��ETC2���ֻ��ϻ���
            androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality16Bit,
            maxTextureSize = 1024,
            //ѹ����ʽ ��֮ǰ�˼Ҳ�����õ�����ѹ����ʽ
            format = TextureImporterFormat.ASTC_RGBA_4x4,
            //�����ᱻѹ����δѹ����= 0,
            //ժҪ��������ƽ̨// (DXT, ASTC����)ʹ�ñ�׼��ʽ����ѹ����Compressed= 1,
            //ժҪ��������ƽ̨�Ϳ�����(BC7, ASTC4x4����)ʹ�ø������ĸ�ʽ����ѹ����CompressedHQ = 2,
            //ժҪ����ƽ̨�Ϳ�����(2bpp PVRTC, ASTC8x8����)������ʹ�õ������������ܡ���ѹ��/��ʽ����ѹ����CompressedLQ = 3
            textureCompression = TextureImporterCompression.CompressedHQ,
            //��ʹ��ʱѹ��
            crunchedCompression = true,
            //ѹ����������[0-100]
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
