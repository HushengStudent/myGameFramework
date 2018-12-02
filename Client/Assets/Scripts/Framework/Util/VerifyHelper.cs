/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/05/15 00:01:02
** desc:  Crc32/MD5验证工具;
*********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Framework
{
    public static class VerifyHelper
    {
        private static readonly byte[] _zero = new byte[] { 0, 0, 0, 0 };

        /// <summary>
        /// 计算二进制流的CRC32;
        /// </summary>
        /// <param name="bytes">指定的二进制流;</param>
        /// <returns>计算后的CRC32;</returns>
        public static byte[] GetCrc32(byte[] bytes)
        {
            if (bytes == null)
            {
                return _zero;
            }
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                Crc32 calculator = new Crc32();
                byte[] result = calculator.ComputeHash(memoryStream);
                calculator.Clear();
                return result;
            }
        }

        /// <summary>
        /// 计算指定文件的CRC32;
        /// </summary>
        /// <param name="fileName">指定文件的完全限定名称;</param>
        /// <returns>计算后的CRC32;</returns>
        public static byte[] GetCrc32(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return _zero;
            }
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Crc32 calculator = new Crc32();
                byte[] result = calculator.ComputeHash(fileStream);
                calculator.Clear();
                return result;
            }
        }

        /// <summary>
        /// 计算二进制流的MD5;
        /// </summary>
        /// <param name="bytes">指定的二进制流;</param>
        /// <returns>计算后的MD5;</returns>
        public static string GetMD5(byte[] bytes)
        {
            MD5 alg = new MD5CryptoServiceProvider();
            byte[] data = alg.ComputeHash(bytes);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        #region Crc32

        /// <summary>
        /// CRC32算法;
        /// </summary>
        private class Crc32 : HashAlgorithm
        {
            /// <summary>
            /// 默认多项式;
            /// </summary>
            public const uint _defaultPolynomial = 0xedb88320;

            /// <summary>
            /// 默认种子数;
            /// </summary>
            public const uint _defaultSeed = 0xffffffff;

            private static uint[] _defaultTable = null;
            private readonly uint _seed;
            private readonly uint[] _table;
            private uint _hash;

            /// <summary>
            /// 初始化CRC32类的新实例;
            /// </summary>
            public Crc32()
            {
                _seed = _defaultSeed;
                _table = InitializeTable(_defaultPolynomial);
                _hash = _defaultSeed;
            }

            /// <summary>
            /// 初始化CRC32类的新实例;
            /// </summary>
            /// <param name="polynomial">指定的多项式。</param>
            /// <param name="seed">指定的种子数。</param>
            public Crc32(uint polynomial, uint seed)
            {
                _seed = seed;
                _table = InitializeTable(polynomial);
                _hash = seed;
            }

            /// <summary>
            /// 初始化Crc32类的实现;
            /// </summary>
            public override void Initialize()
            {
                _hash = _seed;
            }

            /// <summary>
            /// 将写入对象的数据路由到哈希算法以计算哈希值;
            /// </summary>
            /// <param name="array">要计算其哈希代码的输入;</param>
            /// <param name="ibStart">字节数组中的偏移量，从该位置开始使用数据;</param>
            /// <param name="cbSize">字节数组中用作数据的字节数;</param>
            protected override void HashCore(byte[] array, int ibStart, int cbSize)
            {
                _hash = CalculateHash(_table, _hash, array, ibStart, cbSize);
            }

            /// <summary>
            /// 在加密流对象处理完最后的数据后完成哈希计算;
            /// </summary>
            /// <returns>计算所得的哈希代码;</returns>
            protected override byte[] HashFinal()
            {
                byte[] hashBuffer = UInt32ToBigEndianBytes(~_hash);
                HashValue = hashBuffer;
                return hashBuffer;
            }

            private static uint[] InitializeTable(uint polynomial)
            {
                if (_defaultTable != null && polynomial == _defaultPolynomial)
                {
                    return _defaultTable;
                }
                uint[] createTable = new uint[256];
                for (int i = 0; i < 256; i++)
                {
                    uint entry = (uint)i;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((entry & 1) == 1)
                        {
                            entry = (entry >> 1) ^ polynomial;
                        }
                        else
                        {
                            entry = entry >> 1;
                        }
                    }

                    createTable[i] = entry;
                }
                if (polynomial == _defaultPolynomial)
                {
                    _defaultTable = createTable;
                }
                return createTable;
            }

            private static uint CalculateHash(uint[] table, uint seed, byte[] bytes, int start, int size)
            {
                uint crc = seed;
                for (int i = start; i < size; i++)
                {
                    unchecked
                    {
                        crc = (crc >> 8) ^ table[bytes[i] ^ crc & 0xff];
                    }
                }
                return crc;
            }

            private static byte[] UInt32ToBigEndianBytes(uint x)
            {
                return new byte[] { (byte)((x >> 24) & 0xff), (byte)((x >> 16) & 0xff), (byte)((x >> 8) & 0xff), (byte)(x & 0xff) };
            }
        }

        #endregion
    }
}
