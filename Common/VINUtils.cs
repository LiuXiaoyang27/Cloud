using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// 共通方法
    /// </summary>
    public class VINUtils
    {

        /// <summary>
        ///  16进制字符串转字节数组
        ///  格式为 string hexString = "00 01 00 00 00 06 FF 05 00 64 00 00";
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStrToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
            return returnBytes;
        }

        /// <summary>
        /// 字符串 转 16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string StrToHexStr(string data, Encoding encode)
        {
            byte[] b = encode.GetBytes(data);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                if (i == 0)
                {
                    result += "" + Convert.ToString(b[i], 16);
                }
                else
                {
                    result += " " + Convert.ToString(b[i], 16);
                }

            }
            return result;

        }
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] bytes, int count)
        {
            string hexString = "";
            if (bytes != null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i == 0)
                    {
                        hexString += bytes[i].ToString("X2");
                    }
                    else
                    {
                        hexString += " " + bytes[i].ToString("X2");
                    }

                }
            }
            return hexString;
        }
        public static string ByteToHexStr(byte[] bytes)
        {
            string hexString = "";
            int count = bytes.Length;
            if (bytes != null)
            {
                for (int i = 0; i < count; i++)
                {
                    if (i == 0)
                    {
                        hexString += bytes[i].ToString("X2");
                    }
                    else
                    {
                        hexString += " " + bytes[i].ToString("X2");
                    }

                }
            }
            return hexString;
        }

        #region========CRC校验算法========

        /// <summary>
        /// 字节数组CRC校验算法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CCITT_CRC16(string data)
        {
            byte[] byteData = HexStrToByte(data);

            return CCITT_CRC16(byteData);
        }
        /// <summary>
        /// 字节数组CRC校验算法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CCITT_CRC16(byte[] data)
        {
            ushort crc_reg = 0;
            ushort current;
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                current = (ushort)(data[i] << 8);
                for (int j = 0; j < 8; j++)
                {
                    if ((short)(crc_reg ^ current) < 0)
                        crc_reg = (ushort)((crc_reg << 1) ^ 0x1021);
                    else
                        crc_reg <<= 1;
                    current <<= 1;
                }
            }

            byte hi = (byte)((crc_reg & 0xFF00) >> 8);  //高位置
            byte lo = (byte)(crc_reg & 0x00FF);         //低位置

            return new byte[] { hi, lo };
            //return crc_reg;
        }
        #endregion

        #region ========加密/解密通用方法========

        private static byte[] data = new byte[256]
        {
            0x63,0x7C,0x77,0x7B,0xF2,0x6B,0x6F,0xC5,0x30,0x01,0x67,0x2B,0xFE,0xD7,0xAB,0x76,
            0xCA,0x82,0xC9,0x7D,0xFA,0x59,0x47,0xF0,0xAD,0xD4,0xA2,0xAF,0x9C,0xA4,0x72,0xC0,
            0xB7,0xFD,0x93,0x26,0x36,0x3F,0xF7,0xCC,0x34,0xA5,0xE5,0xF1,0x71,0xD8,0x31,0x15,
            0x04,0xC7,0x23,0xC3,0x18,0x96,0x05,0x9A,0x07,0x12,0x80,0xE2,0xEB,0x27,0xB2,0x75,
            0x09,0x83,0x2C,0x1A,0x1B,0x6E,0x5A,0xA0,0x52,0x3B,0xD6,0xB3,0x29,0xE3,0x2F,0x84,
            0x53,0xD1,0x00,0xED,0x20,0xFC,0xB1,0x5B,0x6A,0xCB,0xBE,0x39,0x4A,0x4C,0x58,0xCF,
            0xD0,0xEF,0xAA,0xFB,0x43,0x4D,0x33,0x85,0x45,0xF9,0x02,0x7F,0x50,0x3C,0x9F,0xA8,
            0x51,0xA3,0x40,0x8F,0x92,0x9D,0x38,0xF5,0xBC,0xB6,0xDA,0x21,0x10,0xFF,0xF3,0xD2,
            0xCD,0x0C,0x13,0xEC,0x5F,0x97,0x44,0x17,0xC4,0xA7,0x7E,0x3D,0x64,0x5D,0x19,0x73,
            0x60,0x81,0x4F,0xDC,0x22,0x2A,0x90,0x88,0x46,0xEE,0xB8,0x14,0xDE,0x5E,0x0B,0xDB,
            0xE0,0x32,0x3A,0x0A,0x49,0x06,0x24,0x5C,0xC2,0xD3,0xAC,0x62,0x91,0x95,0xE4,0x79,
            0xE7,0xC8,0x37,0x6D,0x8D,0xD5,0x4E,0xA9,0x6C,0x56,0xF4,0xEA,0x65,0x7A,0xAE,0x08,
            0xBA,0x78,0x25,0x2E,0x1C,0xA6,0xB4,0xC6,0xE8,0xDD,0x74,0x1F,0x4B,0xBD,0x8B,0x8A,
            0x70,0x3E,0xB5,0x66,0x48,0x03,0xF6,0x0E,0x61,0x35,0x57,0xB9,0x86,0xC1,0x1D,0x9E,
            0xE1,0xF8,0x98,0x11,0x69,0xD9,0x8E,0x94,0x9B,0x1E,0x87,0xE9,0xCE,0x55,0x28,0xDF,
            0x8C,0xA1,0x89,0x0D,0xBF,0xE6,0x42,0x68,0x41,0x99,0x2D,0x0F,0xB0,0x54,0xBB,0x16
        };
        private static byte[] M = new byte[256]
        {
                0x52,0x09,0x6A,0xD5,0x30,0x36,0xA5,0x38,0xBF,0x40,0xA3,0x9E,0x81,0xF3,0xD7,0xFB
                ,0x7C,0xE3,0x39,0x82,0x9B,0x2F,0xFF,0x87,0x34,0x8E,0x43,0x44,0xC4,0xDE,0xE9,0xCB
                ,0x54,0x7B,0x94,0x32,0xA6,0xC2,0x23,0x3D,0xEE,0x4C,0x95,0x0B,0x42,0xFA,0xC3,0x4E
                ,0x08,0x2E,0xA1,0x66,0x28,0xD9,0x24,0xB2,0x76,0x5B,0xA2,0x49,0x6D,0x8B,0xD1,0x25
                ,0x72,0xF8,0xF6,0x64,0x86,0x68,0x98,0x16,0xD4,0xA4,0x5C,0xCC,0x5D,0x65,0xB6,0x92
                ,0x6C,0x70,0x48,0x50,0xFD,0xED,0xB9,0xDA,0x5E,0x15,0x46,0x57,0xA7,0x8D,0x9D,0x84
                ,0x90,0xD8,0xAB,0x00,0x8C,0xBC,0xD3,0x0A,0xF7,0xE4,0x58,0x05,0xB8,0xB3,0x45,0x06
                ,0xD0,0x2C,0x1E,0x8F,0xCA,0x3F,0x0F,0x02,0xC1,0xAF,0xBD,0x03,0x01,0x13,0x8A,0x6B
                ,0x3A,0x91,0x11,0x41,0x4F,0x67,0xDC,0xEA,0x97,0xF2,0xCF,0xCE,0xF0,0xB4,0xE6,0x73
                ,0x96,0xAC,0x74,0x22,0xE7,0xAD,0x35,0x85,0xE2,0xF9,0x37,0xE8,0x1C,0x75,0xDF,0x6E
                ,0x47,0xF1,0x1A,0x71,0x1D,0x29,0xC5,0x89,0x6F,0xB7,0x62,0x0E,0xAA,0x18,0xBE,0x1B
                ,0xFC,0x56,0x3E,0x4B,0xC6,0xD2,0x79,0x20,0x9A,0xDB,0xC0,0xFE,0x78,0xCD,0x5A,0xF4
                ,0x1F,0xDD,0xA8,0x33,0x88,0x07,0xC7,0x31,0xB1,0x12,0x10,0x59,0x27,0x80,0xEC,0x5F
                ,0x60,0x51,0x7F,0xA9,0x19,0xB5,0x4A,0x0D,0x2D,0xE5,0x7A,0x9F,0x93,0xC9,0x9C,0xEF
                ,0xA0,0xE0,0x3B,0x4D,0xAE,0x2A,0xF5,0xB0,0xC8,0xEB,0xBB,0x3C,0x83,0x53,0x99,0x61
                ,0x17,0x2B,0x04,0x7E,0xBA,0x77,0xD6,0x26,0xE1,0x69,0x14,0x63,0x55,0x21,0x0C,0x7D
            };


        private static byte[] q = new byte[256]
        {
             0x00 ,0x02 ,0x04 ,0x06 ,0x08 ,0x0A ,0x0C ,0x0E ,0x10 ,0x12 ,0x14 ,0x16 ,0x18 ,0x1A ,0x1C ,0x1E
            ,0x20 ,0x22 ,0x24 ,0x26 ,0x28 ,0x2A ,0x2C ,0x2E ,0x30 ,0x32 ,0x34 ,0x36 ,0x38 ,0x3A ,0x3C ,0x3E
            ,0x40 ,0x42 ,0x44 ,0x46 ,0x48 ,0x4A ,0x4C ,0x4E ,0x50 ,0x52 ,0x54 ,0x56 ,0x58 ,0x5A ,0x5C ,0x5E
            ,0x60 ,0x62 ,0x64 ,0x66 ,0x68 ,0x6A ,0x6C ,0x6E ,0x70 ,0x72 ,0x74 ,0x76 ,0x78 ,0x7A ,0x7C ,0x7E
            ,0x80 ,0x82 ,0x84 ,0x86 ,0x88 ,0x8A ,0x8C ,0x8E ,0x90 ,0x92 ,0x94 ,0x96 ,0x98 ,0x9A ,0x9C ,0x9E
            ,0xA0 ,0xA2 ,0xA4 ,0xA6 ,0xA8 ,0xAA ,0xAC ,0xAE ,0xB0 ,0xB2 ,0xB4 ,0xB6 ,0xB8 ,0xBA ,0xBC ,0xBE
            ,0xC0 ,0xC2 ,0xC4 ,0xC6 ,0xC8 ,0xCA ,0xCC ,0xCE ,0xD0 ,0xD2 ,0xD4 ,0xD6 ,0xD8 ,0xDA ,0xDC ,0xDE
            ,0xE0 ,0xE2 ,0xE4 ,0xE6 ,0xE8 ,0xEA ,0xEC ,0xEE ,0xF0 ,0xF2 ,0xF4 ,0xF6 ,0xF8 ,0xFA ,0xFC ,0xFE
            ,0x1B ,0x19 ,0x1F ,0x1D ,0x13 ,0x11 ,0x17 ,0x15 ,0x0B ,0x09 ,0x0F ,0x0D ,0x03 ,0x01 ,0x07 ,0x05 //??0x0a
	        ,0x3B ,0x39 ,0x3F ,0x3D ,0x33 ,0x31 ,0x37 ,0x35 ,0x2B ,0x29 ,0x2F ,0x2D ,0x23 ,0x21 ,0x27 ,0x25
            ,0x5B ,0x59 ,0x5F ,0x5D ,0x53 ,0x51 ,0x57 ,0x55 ,0x4B ,0x49 ,0x4F ,0x4D ,0x43 ,0x41 ,0x47 ,0x45
            ,0x7B ,0x79 ,0x7F ,0x7D ,0x73 ,0x71 ,0x77 ,0x75 ,0x6B ,0x69 ,0x6F ,0x6D ,0x63 ,0x61 ,0x67 ,0x65
            ,0x9B ,0x99 ,0x9F ,0x9D ,0x93 ,0x91 ,0x97 ,0x95 ,0x8B ,0x89 ,0x8F ,0x8D ,0x83 ,0x81 ,0x87 ,0x85
            ,0xBB ,0xB9 ,0xBF ,0xBD ,0xB3 ,0xB1 ,0xB7 ,0xB5 ,0xAB ,0xA9 ,0xAF ,0xAD ,0xA3 ,0xA1 ,0xA7 ,0xA5
            ,0xDB ,0xD9 ,0xDF ,0xDD ,0xD3 ,0xD1 ,0xD7 ,0xD5 ,0xCB ,0xC9 ,0xCF ,0xCD ,0xC3 ,0xC1 ,0xC7 ,0xC5
            ,0xFB ,0xF9 ,0xFF ,0xFD ,0xF3 ,0xF1 ,0xF7 ,0xF5 ,0xEB ,0xE9 ,0xEF ,0xED ,0xE3 ,0xE1 ,0xE7 ,0xE5
        };



        //以下是关于 AES 算法的函数
        private static void i7317a0(byte[] in_data, int i, byte[] out_data)//????i???? 
        {
            byte[] s = new byte[10] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1b, 0x36 };
            byte[] p = new byte[16];
            int j, ii;

            mem_copy(p, 0, in_data, 0, 16);
            for (j = 0; j < i; j++)
            {
                p[0] = (byte)(data[p[0x0d]] ^ s[j] ^ p[0]);
                p[1] = (byte)(data[p[0x0e]] ^ p[1]);
                p[2] = (byte)(data[p[0x0f]] ^ p[2]);
                p[3] = (byte)(data[p[0x0c]] ^ p[3]);
                for (ii = 0; ii < 12; ii++)
                {
                    p[ii + 4] = (byte)(p[ii] ^ p[ii + 4]);
                }
            }
            mem_copy(out_data, 0, p, 0, 16);
        }

        private static void i7330e0fan(byte[] n, byte[] p)
        {
            int i;
            for (i = 0; i < 16; i += 4)
            {
                p[i + 0] = (byte)(n[i + 0] ^ n[i + 1] ^ n[i + 2] ^ n[i + 3] ^ n[i + 0] ^ q[n[i + 0] ^ n[i + 1]]);
                p[i + 1] = (byte)(n[i + 0] ^ n[i + 1] ^ n[i + 2] ^ n[i + 3] ^ n[i + 1] ^ q[n[i + 1] ^ n[i + 2]]);
                p[i + 2] = (byte)(n[i + 0] ^ n[i + 1] ^ n[i + 2] ^ n[i + 3] ^ n[i + 2] ^ q[n[i + 2] ^ n[i + 3]]);
                p[i + 3] = (byte)(n[i + 0] ^ n[i + 1] ^ n[i + 2] ^ n[i + 3] ^ n[i + 3] ^ q[n[i + 3] ^ n[i + 0]]);
            }

        }
        private static void i7330e0(byte[] n, byte[] o)
        {
            int i;
            for (i = 0; i < 16; i += 4)
            {
                o[i] = (byte)(q[q[n[i]]] ^ q[n[i]] ^ q[q[q[n[i]]]] ^ n[i + 1] ^ q[n[i + 1]] ^ q[q[q[n[i + 1]]]] ^ n[i + 2] ^ q[q[n[i + 2]]] ^ q[q[q[n[i + 2]]]] ^ n[i + 3] ^ q[q[q[n[i + 3]]]]);
                o[i + 1] = (byte)(q[q[q[n[i]]]] ^ n[i] ^ q[n[i + 1]] ^ q[q[n[i + 1]]] ^ q[q[q[n[i + 1]]]] ^ n[i + 2] ^ q[n[i + 2]] ^ q[q[q[n[i + 2]]]] ^ n[i + 3] ^ q[q[n[i + 3]]] ^ q[q[q[n[i + 3]]]]);
                o[i + 2] = (byte)(q[q[n[i]]] ^ n[i] ^ q[q[q[n[i]]]] ^ n[i + 1] ^ q[q[q[n[i + 1]]]] ^ q[n[i + 2]] ^ q[q[n[i + 2]]] ^ q[q[q[n[i + 2]]]] ^ n[i + 3] ^ q[n[i + 3]] ^ q[q[q[n[i + 3]]]]);
                o[i + 3] = (byte)(q[n[i]] ^ n[i] ^ q[q[q[n[i]]]] ^ n[i + 1] ^ q[q[n[i + 1]]] ^ q[q[q[n[i + 1]]]] ^ n[i + 2] ^ q[q[q[n[i + 2]]]] ^ q[n[i + 3]] ^ q[q[n[i + 3]]] ^ q[q[q[n[i + 3]]]]);
            }

        }

        private static void mem_copy(byte[] dst, int dst_offst, byte[] src, int src_offst, int len)
        {
            for (int i = 0; i < len; i++)
            {
                dst[dst_offst++] = src[src_offst++];
            }

        }
        #endregion

        #region ========加密========
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="key_data"></param>
        /// <returns></returns>
        public static byte[] AES_Encrypt(string str_in_data, string str_key_data)
        {
            byte[] in_data = HexStrToByte(str_in_data);
            byte[] key_data = HexStrToByte(str_key_data);
            return AES_Encrypt(in_data, key_data);
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="key_data"></param>
        /// <returns></returns>
        public static byte[] AES_Encrypt(byte[] in_data, byte[] key_data)
        {
            byte[] meter = new byte[16];
            byte[] metertemp = new byte[16];
            byte[] key_datatemp = new byte[200];
            byte[] out_data = new byte[in_data.Length];

            {
                mem_copy(meter, 0, in_data, 0, 16);
                for (int i = 0; i < 16; i++)//732d60
                {
                    meter[i] = (byte)(meter[i] ^ key_data[i]);
                }

                mem_copy(metertemp, 0, meter, 0, 16);//731c60
                meter[0x0] = data[metertemp[0x0]]; meter[0x2] = data[metertemp[0xa]];
                meter[0x4] = data[metertemp[0x4]]; meter[0xa] = data[metertemp[0x2]];
                meter[0x8] = data[metertemp[0x8]]; meter[0x6] = data[metertemp[0xe]];
                meter[0xc] = data[metertemp[0xc]]; meter[0xe] = data[metertemp[0x6]];
                meter[0xd] = data[metertemp[0x1]]; meter[0x7] = data[metertemp[0x3]];
                meter[0x9] = data[metertemp[0xd]]; meter[0xb] = data[metertemp[0x7]];
                meter[0x5] = data[metertemp[0x9]]; meter[0xf] = data[metertemp[0xb]];
                meter[0x1] = data[metertemp[0x5]]; meter[0x3] = data[metertemp[0xf]];


                for (int k = 1; k < 10; k++)
                {
                    mem_copy(metertemp, 0, meter, 0, 16);//??
                    i7330e0fan(metertemp, meter);

                    i7317a0(key_data, k, key_datatemp);//

                    for (int i = 0; i < 16; i++)//732d60
                    {
                        meter[i] = (byte)(meter[i] ^ key_datatemp[i]);
                    }

                    mem_copy(metertemp, 0, meter, 0, 16);//731c60
                    meter[0x0] = data[metertemp[0x0]]; meter[0x2] = data[metertemp[0xa]];
                    meter[0x4] = data[metertemp[0x4]]; meter[0xa] = data[metertemp[0x2]];
                    meter[0x8] = data[metertemp[0x8]]; meter[0x6] = data[metertemp[0xe]];
                    meter[0xc] = data[metertemp[0xc]]; meter[0xe] = data[metertemp[0x6]];
                    meter[0xd] = data[metertemp[0x1]]; meter[0x7] = data[metertemp[0x3]];
                    meter[0x9] = data[metertemp[0xd]]; meter[0xb] = data[metertemp[0x7]];
                    meter[0x5] = data[metertemp[0x9]]; meter[0xf] = data[metertemp[0xb]];
                    meter[0x1] = data[metertemp[0x5]]; meter[0x3] = data[metertemp[0xf]];
                }

                i7317a0(key_data, 10, key_datatemp);
                for (int i = 0; i < 16; i++)//732d60
                {
                    meter[i] = (byte)(meter[i] ^ key_datatemp[i]);
                }
                mem_copy(out_data, 0, meter, 0, 16);
                return out_data;
            }
        }

        #endregion

        #region  ========解密========
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="key_data"></param>
        /// <returns></returns>
        public static byte[] AES_Decrypt(string str_in_data, string str_key_data)
        {
            byte[] in_data = HexStrToByte(str_in_data);
            byte[] key_data = HexStrToByte(str_key_data);
            return AES_Decrypt(in_data, key_data);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="key_data"></param>
        /// <returns></returns>
        public static byte[] AES_Decrypt(byte[] in_data, byte[] key_data)
        {
            byte[] meter = new byte[16];
            byte[] metertemp = new byte[16];
            byte[] duoyutemp = new byte[16];
            byte[] out_data = new byte[in_data.Length];

            mem_copy(meter, 0, in_data, 0, 16);
            i7317a0(key_data, 10, duoyutemp);
            for (int i = 0; i < 16; i++)//732d60
            {
                meter[i] = (byte)(meter[i] ^ duoyutemp[i]);
            }

            for (int k = 9; k > 0; k--)
            {
                mem_copy(metertemp, 0, meter, 0, 16);//731c60
                meter[0x0] = M[metertemp[0x0]]; meter[0x2] = M[metertemp[0xa]];
                meter[0x4] = M[metertemp[0x4]]; meter[0xa] = M[metertemp[0x2]];
                meter[0x8] = M[metertemp[0x8]]; meter[0x6] = M[metertemp[0xe]];
                meter[0xc] = M[metertemp[0xc]]; meter[0xe] = M[metertemp[0x6]];
                meter[0x1] = M[metertemp[0xd]]; meter[0x3] = M[metertemp[0x7]];
                meter[0xd] = M[metertemp[0x9]]; meter[0x7] = M[metertemp[0xb]];
                meter[0x9] = M[metertemp[0x5]]; meter[0xb] = M[metertemp[0xf]];
                meter[0x5] = M[metertemp[0x1]]; meter[0xf] = M[metertemp[0x3]];

                i7317a0(key_data, k, duoyutemp);//7312e0

                for (int i = 0; i < 16; i++)//732d60
                {
                    meter[i] = (byte)(meter[i] ^ duoyutemp[i]);
                }

                mem_copy(metertemp, 0, meter, 0, 16);//7330e0
                i7330e0(metertemp, meter);
            }
            mem_copy(metertemp, 0, meter, 0, 16);//731c60
            meter[0x0] = M[metertemp[0x0]]; meter[0x2] = M[metertemp[0xa]];
            meter[0x4] = M[metertemp[0x4]]; meter[0xa] = M[metertemp[0x2]];
            meter[0x8] = M[metertemp[0x8]]; meter[0x6] = M[metertemp[0xe]];
            meter[0xc] = M[metertemp[0xc]]; meter[0xe] = M[metertemp[0x6]];
            meter[0x1] = M[metertemp[0xd]]; meter[0x3] = M[metertemp[0x7]];
            meter[0xd] = M[metertemp[0x9]]; meter[0x7] = M[metertemp[0xb]];
            meter[0x9] = M[metertemp[0x5]]; meter[0xb] = M[metertemp[0xf]];
            meter[0x5] = M[metertemp[0x1]]; meter[0xf] = M[metertemp[0x3]];

            for (int i = 0; i < 16; i++)//732d60
            {
                meter[i] = (byte)(meter[i] ^ key_data[i]);
            }
            mem_copy(out_data, 0, meter, 0, 16);
            return out_data;
        }
        #endregion

        #region  ========应答数据状态========

        public enum DataStatusEnum
        {
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 校验错误
            /// </summary>
            CheckError,

            PreDownload,
            /// <summary>
            /// 数据长度错误
            /// </summary>
            LengthError,
            /// <summary>
            /// 数据格式错误
            /// </summary>
            FormatError,
            /// <summary>
            /// 数据不完整
            /// </summary>
            MissError,
            /// <summary>
            /// 设备没有激活次数
            /// </summary>
            TimesError
        }

        /// <summary>
        /// 平台应答数据包-状态
        /// </summary>
        /// <param name="status">枚举数据状态</param>
        /// <returns></returns>
        public static string GetDataStatusByte(DataStatusEnum status)
        {
            string result = "";
            switch (status)
            {
                // 0000	成功
                case DataStatusEnum.Success:
                    result = "00 00";
                    break;
                // FF01 校验错误
                case DataStatusEnum.CheckError:
                    result = "FF 01";
                    break;
                // FF02 数据长度错误
                case DataStatusEnum.LengthError:
                    result = "FF 02";
                    break;
                // FF03 数据格式错误
                case DataStatusEnum.FormatError:
                    result = "FF 03";
                    break;
                // FF04 数据不完整
                case DataStatusEnum.MissError:
                    result = "FF 04";
                    break;
                // FF05 设备没有激活次数
                case DataStatusEnum.TimesError:
                    result = "FF 05";
                    break;
                // FF05 设备没有激活次数
                case DataStatusEnum.PreDownload:
                    result = "FF 06";
                    break;
            }

            return result;
        }
        #endregion

        #region  ========应答车辆状态========
        public enum CarStatusEnum
        {
            /// <summary>
            /// 秘钥有效
            /// </summary>
            KeySuccess,
            /// <summary>
            /// 数据待下载
            /// </summary>
            PreDownload,
            /// <summary>
            /// 数据计算中
            /// </summary>
            OnCompute,
            /// <summary>
            /// 数据有误
            /// </summary>
            DataError,
            /// <summary>
            /// 不存在
            /// </summary>
            DataNothing
        }

        /// <summary>
        /// 车辆状态回复-状态
        /// </summary>
        /// <param name="status">枚举车辆状态</param>
        /// <returns></returns>
        public static string GetCarStatusByte(CarStatusEnum status)
        {
            string result = "";
            switch (status)
            {
                // 0000	秘钥有效
                case CarStatusEnum.KeySuccess:
                    result = "00 00";
                    break;
                // 0001 数据待下载
                case CarStatusEnum.PreDownload:
                    result = "00 01";
                    break;
                // 0002 数据计算中
                case CarStatusEnum.OnCompute:
                    result = "00 02";
                    break;
                // FE01 数据有误
                case CarStatusEnum.DataError:
                    result = "FE 01";
                    break;
                // FE02 不存在
                case CarStatusEnum.DataNothing:
                    result = "FE 02";
                    break;
            }

            return result;
        }
        #endregion

        #region ==========写日志===========
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="strContent"></param>
        public static void SaveLog(string strContent)
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                StreamWriter sw = new StreamWriter(Path.Combine(path, string.Format("Server_Log_{0}.txt", DateTime.Now.ToString("yyyyMMdd"))), true);
                strContent.Split('\n').ToList().ForEach(line =>
                {
                    sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + line.Trim());
                });
                sw.Flush();
                sw.Close();
            }
            catch { }
        }

        #endregion

        #region =========获得服务器IP地址==========
        public static string GetServerIP()
        {
            string ipAddress = "";
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPAddress[] addressList = Dns.GetHostAddresses(strHostName); //取得本机IP
            foreach (IPAddress address in addressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = address.ToString();
                }
            }
            return ipAddress;
        }
        #endregion

        #region ========OBD数据=========

        public class OBD
        {
            public int Len { get; set; }

            public string Cmd { get; set; }

            public string Value { get; set; }

            public string Type { get; set; }
        }
        public static List<OBD> GetOBD(byte[] bytes)
        {
            List<OBD> list = new List<OBD>();

            for (int i = 0; i < bytes.Length; i++)
            {
                // 类型
                string type = bytes[i].ToString("X2");  // 01:发动机 02：仪表
                if (!type.Equals("01") && !type.Equals("02"))
                {
                    break;
                }
                // 命令
                string cmd = ByteToHexStr(bytes.Skip(i + 1).Take(3).ToArray());
                // 长度
                int len = int.Parse(bytes[i + 4].ToString("X2"), NumberStyles.HexNumber);
                if (len == 0)
                {
                    break;
                }
                // 数据
                string data = ByteToHexStr(bytes.Skip(i + 5).Take(len).ToArray());
                list.Add(new OBD()
                {
                    Type = type,
                    Cmd = cmd,
                    Len = len,
                    Value = data
                });
                i += 4 + len;
            }
            return list;
        }

        public static void OBDLog(List<OBD> data)
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                StreamWriter sw = new StreamWriter(Path.Combine(path, string.Format("OBD_Log_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"))), true);

                sw.WriteLine("");
                sw.WriteLine("-----  发动机  ------");

                List<OBD> fdj = data.FindAll(a => a.Type.Equals("01"));
                foreach (var item in fdj)
                {
                    sw.WriteLine("发送:" + item.Cmd);
                    sw.WriteLine("接收:" + item.Value);
                }
                sw.WriteLine("");
                sw.WriteLine("-----  IPC  仪表  ------");

                List<OBD> yb = data.FindAll(a => a.Type.Equals("02"));
                foreach (var item in yb)
                {
                    sw.WriteLine("发送:" + item.Cmd);
                    sw.WriteLine("接收:" + item.Value);
                }

                sw.Flush();
                sw.Close();
            }
            catch { }
        }
        #endregion

        #region ========应答数据=========

        /// <summary>
        /// OBD应答信息
        /// </summary>
        /// <param name="data"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetObdPostBack(byte[] data, DataStatusEnum status)
        {

            // 应答
            string msg = "00 08 70 03 " + data[31].ToString("X2") + " " + data[32].ToString("X2") + " " + GetDataStatusByte(status) + "";

            // crc
            byte[] crc = CCITT_CRC16(msg);

            string result = "FF " + msg + " " + ByteToHexStr(crc);

            return result;
        }

        public static string GetAuthPostBack(byte[] data)
        {
            var keyBack = data.Skip(8).Take(16).ToArray();
            var sendMsg = "00 0C 70 01 " + ByteToHexStr(keyBack);
            var backcrc = CCITT_CRC16(sendMsg);

            string result = "FF " + sendMsg + " " + ByteToHexStr(backcrc);

            return result;
        }

        public static string GetStatuPostBack(byte[] data, CarStatusEnum status, string key = "")
        {
            var sendLen = status == CarStatusEnum.KeySuccess ? "00 2E" : "00 1E";

            var dev = ByteToHexStr(data.Skip(5).Take(8).ToArray());
            var car = ByteToHexStr(data.Skip(13).Take(17).ToArray());

            var sendMsg1 = sendLen + " 70 04 " + dev + " " + car + " " + GetCarStatusByte(status) + " " + key;

            var backcrc1 = CCITT_CRC16(sendMsg1);

            string result = "FF " + sendMsg1 + " " + ByteToHexStr(backcrc1);

            return result;

        }
        #endregion

        #region  ========校验数据=========

        /// <summary>
        /// 校验数据长度
        /// </summary>
        /// <param name="result">没有空格</param>
        /// <returns></returns>
        public static bool CheckLen(string result, int dataLen)
        {
            // int dataLen = int.Parse(result.Substring(2, 4), NumberStyles.HexNumber);
            int strLen = (dataLen + 3) * 2;
            if (result.Length != strLen)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 校验CRC
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="len"></param>
        /// <param name="crc"></param>
        /// <returns></returns>
        public static bool CheckCRC(byte[] buffer, int len, string crc)
        {
            //int dataLen = int.Parse(result.Substring(2, 4), NumberStyles.HexNumber);
            var crcData = buffer.Skip(1).Take(len).ToArray();
            var crcResult = VINUtils.ByteToHexStr(VINUtils.CCITT_CRC16(crcData)).Replace(" ", "");

            // 校验crc与后4位比较
            if (!crcResult.Equals(crc))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发动机
        /// </summary>
        public static Dictionary<string, int> OBD01 = new Dictionary<string, int>
        {
            {"22 F1 7C",26},
            {"22 F1 90",20},
            {"22 02 E0",7},
            {"22 02 ED",11},
            {"22 02 EE",13},
            {"22 EF 90",3},
            {"22 02 F9",8},
            {"22 02 FF",21},
            {"22 02 FA",3},
            {"22 F1 A2",9},
            {"22 F1 9E",28}
        };

        /// <summary>
        /// IPC  仪表
        /// </summary>
        public static Dictionary<string, int> OBD02 = new Dictionary<string, int>
        {
            {"22 02 E4",7},
            {"22 02 E5",7},
            {"22 02 E6",7},
            {"22 02 E7",3},
            {"22 02 ED",13},
            {"22 02 EE",13},
            {"22 02 F9",3},
            {"22 02 FF",13},
            {"22 02 FA",8},
            {"22 F1 A2",9},
            {"22 F1 9E",24},
            {"22 F1 7C",26},
            {"22 F1 90",20}
        };

        public static bool CheckOBD(List<OBD> list)
        {
            foreach (KeyValuePair<string, int> kvp in OBD01)
            {
                OBD data = list.Find(a => a.Type.Equals("01") && a.Cmd.Replace(" ", "").Equals(kvp.Key.Replace(" ", "")) && a.Value.Replace(" ", "").Length == kvp.Value * 2 && a.Len == kvp.Value);
                if (data == null)
                {
                    return false;
                }
            }
            foreach (KeyValuePair<string, int> kvp in OBD02)
            {
                OBD data = list.Find(a => a.Type.Equals("02") && a.Cmd.Replace(" ", "").Equals(kvp.Key.Replace(" ", "")) && a.Value.Replace(" ", "").Length == kvp.Value * 2 && a.Len == kvp.Value);
                if (data == null)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}
