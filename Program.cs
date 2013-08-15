using System;
using System.Collections.Generic;
using System.Text;

namespace six_bit__ascii_decoder_encoder
{
    class Program
    {
        /// <summary>
        /// Convert unicode c-sharp string to 6bit ASCII byte array
        /// </summary>
        /// <param name="value_string">c-sharp string</param>
        /// <returns>6bit ASCII byte array</returns>
        private static byte[] encode(String value_string)
        {
            // to 6-bit ASCII 
            byte[] t_b_in = Encoding.ASCII.GetBytes(value_string);
            return encode(t_b_in);
        }

        /// <summary>
        /// Convert 8 bit ASCII byte array to 6 bit ASCII byte array
        /// </summary>
        /// <param name="ascii_bytes"></param>
        /// <returns></returns>
        private static byte[] encode(byte[] ascii_bytes)
        {
            // to 6-bit ASCII 
            byte[] t_b_in = ascii_bytes;

            byte[] t_b_out = new byte[Convert.ToInt32(Math.Floor(((double)t_b_in.Length * 6) / 8)) + 1];
            int i2 = 0, i3 = 0;
            byte a, b;
            for (int i1 = 0, cnt1 = t_b_in.Length; i1 < cnt1; i1++)
            {
                if (t_b_in[i1] == (byte)0 || (t_b_in[i1] >= (byte)32 && t_b_in[i1] <= (byte)95))
                { // chars at 32 to 95 
                    a = (byte)(t_b_in[i1] & (0x3F >> 2 * i2));
                    a <<= 2 + 2 * i2;
                    b = (i1 + 1 < t_b_in.Length) ? t_b_in[i1 + 1] : (byte)0x00;
                    b &= 0x3F;
                    b >>= 4 - 2 * i2;
                    t_b_out[i3] = (byte)(a | b);
                    i3++;
                    if (i2 < 2)
                    {
                        i2++;
                    }
                    else
                    {
                        i2 = 0;
                        i1++;
                    }

                }
                else
                {
                    throw new OverflowException("symbol " + t_b_in[i1] + " is not encoding in 6-bit Ascii");
                }
            }
            if ((t_b_out[t_b_out.Length - 1] & 0x3F) != 0)
            { // add 6-bit char [null] to end of array
                Array.Resize(ref t_b_out, t_b_out.Length + 1);
                t_b_out[t_b_out.Length - 1] = (byte)0x00;
            }
            if ((t_b_out.Length & 1) != 0)
            { // alignment to words (2-bytes)
                Array.Resize(ref t_b_out, t_b_out.Length + 1);
                t_b_out[t_b_out.Length - 1] = (byte)0x00;
            }
            return t_b_out;
        }

        /// <summary>
        /// Decoder bytes array 6-bit ASCII to 8-bit ASCII array
        /// </summary>
        /// <param name="bytes_value">8-bit ASCII array</param>
        /// <returns>array 6-bit ASCII</returns>
        private static byte[] decode(byte[] bytes_value)
        {
            byte[] t_a_out = new byte[Convert.ToInt32(Math.Floor(((double)bytes_value.Length * 8) / 6)) + 1];
            byte a = 0, b = 0;
            int i2 = 0, i3 = 0;
            for (int i1 = 0, cnt1 = bytes_value.Length; i1 < cnt1; i1++)
            {
                a = (byte)(bytes_value[i1] & (0xFC << 2 * i2));
                b <<= 6 - 2 * i2;
                a >>= 2 + 2 * i2;
                a = (byte)(a | b);
                t_a_out[i3] = a;
                if ((a & 0x20) == 0x20)
                {

                }
                else
                {
                    if (a != 0x00)
                        t_a_out[i3] |= 0x40;
                }
                if (t_a_out[i1] == (byte)0 || (t_a_out[i1] >= (byte)32 && t_a_out[i1] <= (byte)95))
                {
                }
                else
                {
                    throw new OverflowException("symbol " + t_a_out[i1] + " is not encoding in 6-bit Ascii");
                }
                b = (byte)(bytes_value[i1] & (~(0xFC << 2 * i2)));
                i3++;
                if (i2 < 2)
                {
                    i2++;
                }
                else
                {
                    b = 0;
                    i2 = 0;
                    if ((bytes_value[i1] & 0x20) == 0x20)
                    {
                        t_a_out[i3] = (byte)(bytes_value[i1] & 0x3F);
                    }
                    else
                    {
                        if ((bytes_value[i1] & 0x3F) == 0x00)
                        {
                            t_a_out[i3] = 0x00;
                        }
                        else
                            t_a_out[i3] = (byte)((bytes_value[i1] & 0x3F) | 0x40);
                    }
                    if (t_a_out[i1] == (byte)0 || (t_a_out[i1] >= (byte)32 && t_a_out[i1] <= (byte)95))
                    {

                    }
                    else
                    {
                        throw new OverflowException("symbol " + t_a_out[i1] + " is not encoding in 6-bit Ascii");
                    }
                    i3++;
                }
            }
            int index_null = -1;
            for (int i1 = 0, cnt1 = t_a_out.Length; i1 < cnt1; i1++)
            {
                if (t_a_out[i1] == 0x00)
                {
                    index_null = i1;
                    break;
                }
            }
            if (index_null != -1)
            {
                Array.Resize(ref t_a_out, index_null);
            }
            return t_a_out;
        }
    

        static void Main(string[] args)
        {
            // chars from 32 to 95
            String test_string=" !\"#$%&'()*+,-./0123456789:;<=>?ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_";
            Console.WriteLine("test string  : " + test_string);
            byte[] result_array = encode(test_string);
            string result_string = Encoding.ASCII.GetString(decode(result_array));
            Console.WriteLine("result string: " + result_string);
            Console.ReadKey();
        }
    }
}
