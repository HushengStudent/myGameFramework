/********************************************************************************
** auth:  https://github.com/HushengStudent
** date:  2018/02/28 00:50:20
** desc:  Buff
*********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 缓冲类型;
    /// </summary>
    internal enum NetBufferType
    {
        _4K,
        _32K,
    }

    public class NetBuffer32K : NetBuffer
    {
        public NetBuffer32K()
        {
            m_buffer = new byte[1024 * 32];
            BuffSizeType = NetBufferType._32K;
        }
    }

    public class NetBuffer
    {
        public NetBuffer()
        {
            m_buffer = new byte[1024 * 4];
            BuffSizeType = NetBufferType._4K;
        }

        internal NetBufferType BuffSizeType { get; set; }

        /// <summary>
        /// 字节的数组;
        /// </summary>
        protected byte[] m_buffer;

        public Byte[] Bytes
        {
            get { return m_buffer; }
        }

        /// <summary>
        /// 当前使用的数据长度;
        /// </summary>
        public int Length { get; set; }

        private int referenceCounter;

        /// <summary>
        /// 扩大缓冲数据的大小(注意，扩大后，Byte返回的数组的引用将不同);
        /// </summary>
        /// <param name="minSize">扩大的最小尺寸</param>
        public void UpdateCapacity(int minSize = 0)
        {
            int newSize;
            if (minSize == 0)
            {
                newSize = m_buffer.Length * 2;
            }
            else
            {
                newSize = FixSize(minSize);
                LogUtil.LogUtility.Print(string.Format("[NetBuff]UpdateCapacity size={0} newsize={1}", minSize, newSize));
            }
            var newBuffer = new byte[newSize];
            Buffer.BlockCopy(m_buffer, 0, newBuffer, 0, m_buffer.Length);
            m_buffer = newBuffer;
        }

        /// <summary>
        /// 按照4K对齐;
        /// </summary>
        /// <param name="minSize"></param>
        /// <returns></returns>
        private int FixSize(int minSize)
        {
            return (minSize / 4096 + 1) * 4096;
        }

        /// <summary>
        /// 如果对象是通过Pool获得对象,则不用调用该方法;
        /// 如果是参数传入,并且需要使用它的byte数组,则需要先Use,再Release;
        /// </summary>
        public void Use()
        {
            referenceCounter++;
        }

        /// <summary>
        /// 当不再使用缓冲区时,需要手动释放,对象会自动返回对象池里;
        /// </summary>
        public void Release()
        {
            referenceCounter--;
            if (referenceCounter < 0)
            {
                LogUtil.LogUtility.Print("[NetBuff]Release repeat!");
                referenceCounter = 0;
                return;
            }
            if (referenceCounter == 0)
            {
                //ReleaseToPool(this);
            }
        }
    }
}
