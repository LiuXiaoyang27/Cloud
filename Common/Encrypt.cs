using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALG_API
{
    public class Encrypt
    {
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


        //用于算Key的四个16字节的输入 以下是固定值 
        private static byte[] Key_Rand1 = new byte[16]
        { 0x60, 0x05, 0x23, 0xE4, 0xD5, 0x31, 0xF2, 0x3D, 0x4E, 0xAC, 0xD7, 0xD4, 0x50, 0x83, 0x3D, 0x0A };
        private static byte[] Key_Rand2 = new byte[16]
        { 0x73, 0xEC, 0xF7, 0x13, 0x87, 0x15, 0xAE, 0xDD, 0x4F, 0x84, 0xDD, 0xDE, 0x38, 0x54, 0x66, 0x7D };
        private static byte[] Key_Rand3 = new byte[16]
        { 0x92, 0xC6, 0xF5, 0xDB, 0x10, 0x2C, 0x07, 0x0C, 0x10, 0x87, 0xA5, 0x6B, 0x95, 0xCF, 0x23, 0xEA };
        private static byte[] Key_Rand4 = new byte[16]
        { 0xA9, 0xD2, 0x56, 0xE9, 0x34, 0x76, 0x77, 0xB1, 0x50, 0x42, 0x95, 0xA1, 0xCE, 0x84, 0x54, 0x5A };

        //算完两次加密以后异或的数据
        private static byte[] XOR__First_Data = new byte[16]
        { 0x49, 0x79, 0xC2, 0xD6, 0xB1, 0xE7, 0x02, 0x98, 0x0E, 0xF1, 0xE6, 0x9F, 0x17, 0x4E, 0x0D, 0x1A };

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

        private static void AES_Encrype(byte[] in_data, byte[] key_data, byte[] out_data)//????i???? 
        {
            byte[] meter = new byte[16];
            byte[] metertemp = new byte[16];
            byte[] key_datatemp = new byte[200];
            int i;
            {
                mem_copy(meter, 0, in_data, 0, 16);
                for (i = 0; i < 16; i++)//732d60
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

                    for (i = 0; i < 16; i++)//732d60
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
                for (i = 0; i < 16; i++)//732d60
                {
                    meter[i] = (byte)(meter[i] ^ key_datatemp[i]);
                }
                mem_copy(out_data, 0, meter, 0, 16);

            }
        }

        private static void mem_copy(byte[] dst, int dst_offst, byte[] src, int src_offst, int len)
        {
            for (int i = 0; i < len; i++)
            {
                dst[dst_offst++] = src[src_offst++];
            }

        }
        private static byte[] HandlKey(byte[] data)
        {
            byte[] out_key = new byte[64];
            byte[] xor_data = new byte[64];

            mem_copy(xor_data, 0, Key_Rand1, 0, 16);
            mem_copy(xor_data, 16, Key_Rand2, 0, 16);
            mem_copy(xor_data, 32, Key_Rand3, 0, 16);
            mem_copy(xor_data, 48, Key_Rand4, 0, 16);

            for (int i = 0; i < 64; i++)
            {
                out_key[i] = (byte)(data[i] ^ xor_data[i]);
            }
            return out_key;
        }

        //计算Key
        public static byte[] Get_Key(byte[] Device_ID, byte[] Chip_ID, byte[] Key_out, int Version)
        {
            byte[] AES_key = new byte[16];
            byte[] AES_Data = new byte[16];
            byte[] Key_All = new byte[64];
            byte[] XORChipID = new byte[32];
            byte[] XORU3 = new byte[16];
            //V2版对应输出的KEY
            byte[] V2KeyOut = new byte[32];

            mem_copy(XORChipID, 0, Chip_ID, 0, 12);
            mem_copy(XORChipID, 12, Chip_ID, 0, 12);
            mem_copy(XORChipID, 24, Chip_ID, 0, 8);

            //获取AES加密的KEY
            get_first_key(Device_ID, Chip_ID, AES_key);
            //获取第一个组数据
            AES_Encrype(Key_Rand1, AES_key, AES_Data);
            mem_copy(V2KeyOut, 0, AES_Data, 0, 16);
            mem_copy(Key_All, 0, AES_Data, 0, 16);
            mem_copy(Key_out, 24, AES_Data, 0, 8);
            mem_copy(XORU3, 0, AES_Data, 8, 8);

            AES_Encrype(Key_Rand2, AES_key, AES_Data);
            mem_copy(V2KeyOut, 16, AES_Data, 0, 16);
            mem_copy(Key_All, 16, AES_Data, 0, 16);
            mem_copy(Key_out, 32, AES_Data, 0, 8);
            mem_copy(XORU3, 8, AES_Data, 8, 8);

            AES_Encrype(Key_Rand3, AES_key, AES_Data);
            mem_copy(Key_All, 32, AES_Data, 0, 16);
            mem_copy(Key_out, 16, AES_Data, 0, 8);

            AES_Encrype(Key_Rand4, AES_key, AES_Data);
            mem_copy(Key_All, 48, AES_Data, 0, 16);
            mem_copy(Key_out, 0, AES_Data, 0, 16);

            XorData(XORU3, XORChipID, XORU3, 16);
            mem_copy(Key_All, 16, XORU3, 0, 16);
            if (Version == 2)
            {
                XorData(V2KeyOut, XORChipID, Key_out, 32);
                getRandom(Key_out, 32, 8);
            }

            Key_All = HandlKey(Key_All);

            return Key_All;
        }

        public static byte[] Encrypt_Data(int cnt, byte[] in_data, byte[] xor_data, byte[] key_data, int Version)
        {
            byte[] out_buf = new byte[16];
            byte[] buf = new byte[16];
            byte[] AES_Key_Second = new byte[16];
            key_data = HandlKey(key_data);
            mem_copy(AES_Key_Second, 0, key_data, 0, 16);

            //应对版本1的升级，含有U3 U1两块板子，要在代码前64字节加入U3存的KEY
            if (Version == 1)
            {
                if (cnt == 4)
                {
                    mem_copy(xor_data, 0, XOR__First_Data, 0, 16);
                }
                switch (cnt)
                {
                    //0-3 为设定密钥
                    case 0:
                        mem_copy(out_buf, 0, key_data, 48, 16);
                        break;
                    case 1:
                        mem_copy(out_buf, 0, key_data, 32, 8);
                        mem_copy(out_buf, 8, key_data, 16, 8);
                        break;
                    case 2:
                        mem_copy(out_buf, 0, key_data, 24, 8);
                        getRandom(out_buf, 8, 8);
                        break;
                    case 3:
                        getRandom(out_buf, 0, 16);
                        break;
                    //正常数据加密
                    default:
                        AES_Encrype(in_data, AES_Key_Second, buf);
                        XorData(buf, xor_data, out_buf, 16);
                        break;
                }
            }
            //对应的V2版本没有前64自己的KEY
            else if (Version == 2)
            {
                if (cnt == 0)
                {
                    mem_copy(xor_data, 0, XOR__First_Data, 0, 16);
                }
                AES_Encrype(in_data, AES_Key_Second, buf);
                XorData(buf, xor_data, out_buf, 16);
            }

            return out_buf;
        }

        /*
         *通过 设备ID 和 芯片ID 获取一个16位的Key
         *@Device_ID    设备ID    17字节
         *@Chip_ID      芯片ID    12字节
         *@key_out      输出KEY   16字节
         *备注：
         *  Key = Chip_ID :: data
        */
        private static void get_first_key(byte[] Device_ID, byte[] Chip_ID, byte[] key_out)
        {
            byte[] data = new byte[4];
            byte[] data1 = new byte[4];
            byte[] data2 = new byte[4];
            byte[] data3 = new byte[4];
            byte[] data4 = new byte[4];
            byte[] data5 = new byte[4];
            mem_copy(data1, 0, Device_ID, 0, 4);
            mem_copy(data2, 0, Device_ID, 4, 4);
            mem_copy(data3, 0, Device_ID, 8, 4);
            mem_copy(data4, 0, Device_ID, 12, 4);
            data5[0] = Device_ID[16];
            data5[1] = 0xff;
            data5[2] = 0xff;
            data5[3] = 0xff;
            for (int i = 0; i < 4; i++)
            {
                data[i] = (byte)(data1[i] ^ data2[i] ^ data3[i] ^ data4[i] ^ data5[i]);
            }
            mem_copy(key_out, 0, Chip_ID, 0, 12);
            mem_copy(key_out, 12, data, 0, 4);
        }

        //异或数据
        private static void XorData(byte[] in_data, byte[] xor_data, byte[] out_dat, int len)
        {
            for (int i = 0; i < len; i++)
            {
                out_dat[i] = (byte)(in_data[i] ^ xor_data[i]);
            }
        }

        private static void getRandom(byte[] in_data, int offset, int size)
        {
            Random example = new Random();
            for (int i = 0; i < size; i++)
            {
                in_data[offset + i] = (byte)example.Next(0, 255);
            }
        }
    }

}
