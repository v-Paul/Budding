using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace VTMC.Utils.Models
{

    /// <summary>
    /// Wosa TrackReader返回数据模型
    /// </summary>
    public class TrackReaderBufferModel
    {
        /// <summary>
        /// 磁道1(data  or  nullptr)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string track1 { get; set; }
        /// <summary>
        /// 磁道1状态(ok, missing, invalid)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string track1status { get; set; }
        /// <summary>
        /// 磁道2(data  or  nullptr)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string track2 { get; set; }
        /// <summary>
        /// 磁道2状态(ok, missing, invalid)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string track2status { get; set; }
        /// <summary>
        /// 磁道3(data  or  nullptr)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string track3 { get; set; }
        /// <summary>
        /// 磁道3状态(ok, missing, invalid)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string track3status { get; set; }
        /// <summary>
        /// chip(data  or  nullptr)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string chip { get; set; }
        /// <summary>
        /// chip(ok, missing, invalid)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string chipstatus { get; set; }


    }
    /// <summary>
    /// Ic 卡返回数据结构
    /// </summary>
    [Serializable]
    public class ICData
    {
        /// <summary>
        /// track2
        /// </summary>
        public string track2 { get; set; }
        /// <summary>
        /// Card Number
        /// </summary>
        public string cardNo { get; set; }
        /// <summary>
        /// expiry Date
        /// </summary>
        public string expiryDate { get; set; }
    }


    /// <summary>
    /// Wosa TrackReader状态返回数据模型
    /// </summary>
    public class TrackReaderBufferStatusModel
    {
        /// <summary>
        /// Extra
        /// </summary>
        public string Extra { get; set; }
        /// <summary>
        /// fwDevice
        /// </summary>
        public string fwDevice { get; set; }
        /// <summary>
        /// fwMedia
        /// </summary>
        public string fwMedia { get; set; }
        /// <summary>
        /// ChipPower
        /// </summary>
        public string ChipPower { get; set; }
        /// <summary>
        /// fwRetainBin
        /// </summary>
        public string fwRetainBin { get; set; }
        /// <summary>
        /// usCards
        /// </summary>
        public string usCards { get; set; }
    }

    public class APDUCommand
    {
        // Fields
        private const int APDU_MIN_LENGTH = 4;
        private byte cla;
        private byte[] data;
        private byte ins;
        private byte le;
        private byte p1;
        private byte p2;

        // Properties
        public byte Class
        {
            get
            {
                return this.cla;
            }
        }


        public byte[] Data
        {
            get
            {
                return this.data;
            }
        }


        public byte Ins
        {
            get
            {
                return this.ins;
            }
        }

        public byte Le
        {
            get
            {
                return this.le;
            }
        }

        public byte P1
        {
            get
            {
                return this.p1;
            }
        }

        public byte P2
        {
            get
            {
                return this.p2;
            }
        }


        // Methods
        public APDUCommand(byte cla, byte ins, byte p1, byte p2, byte[] data, byte le)
        {
            this.cla = cla;
            this.ins = ins;
            this.p1 = p1;
            this.p2 = p2;
            this.data = data;
            this.le = le;
        }

        public byte[] ToArray([Optional, DefaultParameterValue(false)] bool expandExtra)
        {
            byte[] buffer = null;
            if (this.Data == null)
            {
                if (expandExtra)
                {
                    buffer = new byte[4 + ((this.Le != 0) ? 2 : 2)];
                    buffer[4] = 0;
                    buffer[5] = 0;
                    if (this.Le > 0)
                    {
                        buffer[5] = this.Le;
                    }
                }
                else
                {
                    buffer = new byte[4 + ((this.Le != 0) ? 1 : 0)];
                    if (this.Le > 0)
                    {
                        buffer[4] = this.Le;
                    }
                }
            }
            else
            {
                buffer = new byte[5 + this.Data.Length];
                for (int i = 0; i < this.Data.Length; i++)
                {
                    buffer[5 + i] = this.Data[i];
                }
                buffer[4] = (byte)this.Data.Length;
            }
            buffer[0] = this.Class;
            buffer[1] = this.Ins;
            buffer[2] = this.P1;
            buffer[3] = this.P2;
            return buffer;
        }

        public override string ToString()
        {
            string str = null;
            if (this.data != null)
            {
                StringBuilder builder2 = new StringBuilder(this.data.Length * 2);
                for (int i = 0; i < this.data.Length; i++)
                {
                    builder2.AppendFormat("{0:X02}", this.data[i]);
                }
                str = "Data: " + builder2.ToString();
                this.le = (byte)this.data.Length;
            }
            StringBuilder builder = new StringBuilder();
            object[] args = new object[] { this.cla, this.ins, this.p1, this.p2, this.le };
            builder.AppendFormat("Cla: {0:X02} Ins: {1:X02} P1: {2:X02} P2: {3:X02} Le: {4:X02} ", args);
            if (this.data != null)
            {
                builder.Append(str);
            }
            return builder.ToString();
        }

    }

    public class ASN1 : IEnumerable
    {
        // Fields
        public ArrayList elist;
        private byte[] m_aTag;
        private byte[] m_aValue;

        // Methods
        public ASN1(byte tag) : this(tag, null)
        {
        }
        public ASN1(byte[] data)
        {
            int num = 0;
            int count = 0;
            if (data == null)
            {
                throw new ArgumentNullException();
            }
            if ((data[count++] & 0x1f) == 0x1f)
            {
                while ((data[count++] & 0x80) == 0x80)
                {
                }
            }
            this.m_aTag = new byte[count];
            Buffer.BlockCopy(data, 0, this.m_aTag, 0, count);
            num = count;
            int num3 = 0;
            int num4 = data[num++];
            if (num4 > 0x80)
            {
                num3 = num4 - 0x80;
                num4 = 0;
                for (int i = 0; i < num3; i++)
                {
                    num4 *= 0x100;
                    num4 += data[i + num];
                }
            }
            else if (num4 == 0x80)
            {
                throw new NotSupportedException("Undefined length encoding.");
            }
            this.m_aValue = new byte[num4];
            Buffer.BlockCopy(data, num + num3, this.m_aValue, 0, num4);
            if ((this.m_aTag[0] & 0x20) == 0x20)
            {
                int anPos = num + num3;
                this.Decode(data, ref anPos, data.Length);
            }
        }



        public ASN1(byte[] tag, byte[] data)
        {
            this.m_aTag = new byte[tag.Length];
            Buffer.BlockCopy(tag, 0, this.m_aTag, 0, tag.Length);
            this.m_aValue = data;
        }

        public ASN1(byte tag, byte[] data) : this(new byte[] { tag }, data)
        {

        }

        public ASN1 Add(ASN1 asn1)
        {
            if (asn1 != null)
            {
                if (this.elist == null)
                {
                    this.elist = new ArrayList();
                }
                this.elist.Add(asn1);
            }
            return asn1;
        }

        private bool CompareArray(byte[] array1, byte[] array2)
        {
            bool flag = array1.Length == array2.Length;
            if (flag)
            {
                for (int i = 0; i < array1.Length; i++)
                {
                    if (array1[i] != array2[i])
                    {
                        return false;
                    }
                }
            }
            return flag;
        }

        public bool CompareTag(byte[] tag)
        {
            return this.CompareArray(this.m_aTag, tag);
        }

        public bool CompareValue(byte[] value)
        {
            return this.CompareArray(this.m_aValue, value);
        }


        protected void Decode(byte[] asn1, ref int anPos, int anLength)
        {
            int count = 0;
            if ((asn1[anPos + count++] & 0x1f) == 0x1f)
            {
                while ((asn1[anPos + count++] & 0x80) == 0x80)
                {
                }
            }
            byte[] dst = new byte[count];
            Buffer.BlockCopy(asn1, anPos, dst, 0, count);
            while (anPos < (anLength - 1))
            {
                int num2;
                byte[] buffer2;
                this.DecodeTLV(asn1, ref anPos, out dst, out num2, out buffer2);
                if (dst[0] != 0)
                {
                    ASN1 asn = this.Add(new ASN1(dst, buffer2));
                    if ((dst[0] & 0x20) == 0x20)
                    {
                        int num3 = anPos;
                        asn.Decode(asn1, ref num3, num3 + num2);
                    }
                    anPos += num2;
                }
            }
        }



        protected void DecodeTLV(byte[] asn1, ref int pos, out ushort tag, out int length, out byte[] content)
        {
            int index = pos;
            pos = index + 1;
            tag = asn1[index];
            switch (tag)
            {
                case 0x5f:
                case 0x9f:
                case 0xbf:
                    tag = (ushort)(tag << 8);
                    index = pos;
                    pos = index + 1;
                    tag = (ushort)(tag | asn1[index]);
                    break;
            }
            index = pos;
            pos = index + 1;
            length = asn1[index];
            if ((length & 0x80) == 0x80)
            {
                int num3 = length & 0x7f;
                length = 0;
                for (int i = 0; i < num3; i++)
                {
                    index = pos;
                    pos = index + 1;
                    length = (length * 0x100) + asn1[index];
                }
            }
            content = new byte[length];
            Buffer.BlockCopy(asn1, pos, content, 0, length);
        }



        protected void DecodeTLV(byte[] asn1, ref int pos, out byte[] tag, out int length, out byte[] content)
        {
            int count = 0;
            if ((asn1[pos + count++] & 0x1f) == 0x1f)
            {
                while ((asn1[pos + count++] & 0x80) == 0x80)
                {
                }
            }
            tag = new byte[count];
            Buffer.BlockCopy(asn1, pos, tag, 0, count);
            pos += count;
            int index = pos;
            pos = index + 1;
            length = asn1[index];
            if ((length & 0x80) == 0x80)
            {
                int num3 = length & 0x7f;
                length = 0;
                for (int i = 0; i < num3; i++)
                {
                    index = pos;
                    pos = index + 1;
                    length = (length * 0x100) + asn1[index];
                }
            }
            content = new byte[length];
            Buffer.BlockCopy(asn1, pos, content, 0, length);
        }


        public ASN1 Element(int index, byte[] aTag)
        {
            try
            {
                if ((this.elist != null) && (index < this.elist.Count))
                {
                    ASN1 asn = (ASN1)this.elist[index];
                    if (asn.Tag == aTag)
                    {
                        return asn;
                    }
                }
                return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }


        public bool Equals(byte[] asn1)
        {
            return this.CompareArray(this.GetBytes(), asn1);
        }

        public ASN1 Find(byte tag)
        {
            byte[] buffer1 = new byte[] { tag };
            return this.Find(buffer1);
        }

        public ASN1 Find(byte[] tag)
        {
            bool found = false;
            return this.Find(this, tag, ref found);
        }

        protected ASN1 Find(ASN1 asn, byte[] tag, ref bool found)
        {
            ASN1 asn2 = null;
            if (this.CompareArray(asn.Tag, tag))
            {
                found = true;
                return asn;
            }
            if (asn.Count > 0)
            {
                foreach (ASN1 asn4 in asn.elist)
                {
                    asn2 = this.Find(asn4, tag, ref found);
                    if (found)
                    {
                        return asn2;
                    }
                }
            }
            return asn2;
        }


        public virtual byte[] GetBytes()
        {
            byte[] dst = null;
            byte[] buffer2;
            if (this.Count > 0)
            {
                int num2 = 0;
                ArrayList list = new ArrayList();
                foreach (ASN1 asn in this.elist)
                {
                    byte[] bytes = asn.GetBytes();
                    list.Add(bytes);
                    num2 += bytes.Length;
                }
                dst = new byte[num2];
                int dstOffset = 0;
                for (int i = 0; i < this.elist.Count; i++)
                {
                    byte[] src = (byte[])list[i];
                    Buffer.BlockCopy(src, 0, dst, dstOffset, src.Length);
                    dstOffset += src.Length;
                }
            }
            else if (this.m_aValue != null)
            {
                dst = this.m_aValue;
            }
            int num = 0;
            if (dst != null)
            {
                int length = dst.Length;
                if (length > 0x7f)
                {
                    if (length <= 0xff)
                    {
                        buffer2 = new byte[3 + length];
                        Buffer.BlockCopy(dst, 0, buffer2, 3, length);
                        num = 0x81;
                        buffer2[2] = (byte)length;
                    }
                    else if (length <= 0xffff)
                    {
                        buffer2 = new byte[4 + length];
                        Buffer.BlockCopy(dst, 0, buffer2, 4, length);
                        num = 130;
                        buffer2[2] = (byte)(length >> 8);
                        buffer2[3] = (byte)length;
                    }
                    else if (length <= 0xffffff)
                    {
                        buffer2 = new byte[5 + length];
                        Buffer.BlockCopy(dst, 0, buffer2, 5, length);
                        num = 0x83;
                        buffer2[2] = (byte)(length >> 0x10);
                        buffer2[3] = (byte)(length >> 8);
                        buffer2[4] = (byte)length;
                    }
                    else
                    {
                        buffer2 = new byte[6 + length];
                        Buffer.BlockCopy(dst, 0, buffer2, 6, length);
                        num = 0x84;
                        buffer2[2] = (byte)(length >> 0x18);
                        buffer2[3] = (byte)(length >> 0x10);
                        buffer2[4] = (byte)(length >> 8);
                        buffer2[5] = (byte)length;
                    }
                }
                else
                {
                    buffer2 = new byte[(this.m_aTag.Length + 1) + length];
                    Buffer.BlockCopy(dst, 0, buffer2, this.m_aTag.Length + 1, length);
                    num = length;
                }
                if (this.m_aValue == null)
                {
                    this.m_aValue = dst;
                }
            }
            else
            {
                buffer2 = new byte[this.m_aTag.Length + 1];
            }
            Buffer.BlockCopy(this.m_aTag, 0, buffer2, 0, this.m_aTag.Length);
            buffer2[this.m_aTag.Length] = (byte)num;
            return buffer2;
        }


        public IEnumerator GetEnumerator()
        {
            return this.elist.GetEnumerator();
        }

        public void SaveToFile(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            using (FileStream stream = File.OpenWrite(filename))
            {
                byte[] bytes = this.GetBytes();
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Tag: ", new object[0]);
            for (int i = 0; i < this.Tag.Length; i++)
            {
                builder.AppendFormat("{0} ", this.Tag[i].ToString("X2"));
                if (((i + 1) % 0x10) == 0)
                {
                    builder.AppendFormat(Environment.NewLine, new object[0]);
                }
            }
            builder.AppendFormat("Length: {0} {1}", this.Value.Length, Environment.NewLine);
            builder.Append("Value: ");
            builder.Append(Environment.NewLine);
            for (int j = 0; j < this.Value.Length; j++)
            {
                builder.AppendFormat("{0}", this.Value[j].ToString("X2"));
                if (((j + 1) % 0x10) == 0)
                {
                    builder.AppendFormat(Environment.NewLine, new object[0]);
                }
            }
            return builder.ToString();
        }


        public int Count
        {
            get
            {
                if (this.elist == null)
                {
                    return 0;
                }
                return this.elist.Count;
            }
        }

        public ASN1 this[int index]
        {
            get
            {
                try
                {
                    if ((this.elist == null) || (index >= this.elist.Count))
                    {
                        return null;
                    }
                    return (ASN1)this.elist[index];
                }
                catch (ArgumentOutOfRangeException)
                {
                    return null;
                }
            }
        }

        public int Length
        {
            get
            {
                if (this.m_aValue != null)
                {
                    return this.m_aValue.Length;
                }
                return 0;
            }
        }
        public byte[] Tag
        {
            get
            {
                return this.m_aTag;
            }
        }

        public byte[] Value
        {
            get
            {
                if (this.m_aValue == null)
                {
                    this.GetBytes();
                }
                return (byte[])this.m_aValue.Clone();
            }
            set
            {
                if (value != null)
                {
                    this.m_aValue = (byte[])value.Clone();
                }
            }
        }

    }

    public class ApplicationFileLocator
    {
        // Fields
        private byte[] data;

        // Methods
        public ApplicationFileLocator(byte[] data)
        {
            this.data = new byte[4];
            Buffer.BlockCopy(data, 0, this.data, 0, 4);
        }

        public byte FirstRecord
        {
            get
            {
                return this.data[1];
            }
        }

        public byte LastRecord
        {
            get
            {
                return this.data[2];
            }
        }

        public byte OfflineRecords
        {
            get
            {
                return this.data[3];
            }
        }

        public byte SFI
        {
            get
            {
                return (byte)(this.data[0] >> 3);
            }
        }

    }

    public class GeneralFunc
    {
        // Methods
        public static string ByteArrayToHexString(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder(bytes.Length * 2);
            foreach (byte num2 in bytes)
            {
                builder.AppendFormat("{0:X2}", num2);
            }
            return builder.ToString();
        }

        public static string ConvertToHex(string text)
        {
            char[] chArray = text.Replace(" ", "").ToCharArray();
            StringBuilder builder = new StringBuilder();
            foreach (char ch in chArray)
            {
                builder.Append(((short)ch).ToString("x"));
            }
            return builder.ToString().ToUpper();
        }

        public static byte[] HexStringToByteArray(string HexStr)
        {
            if ((HexStr.Length % 2) > 0)
            {
                throw new ArgumentException(string.Format("The binary key cannot have an odd number of digits: {0}", HexStr));
            }
            int length = HexStr.Length;
            byte[] buffer = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                buffer[i / 2] = Convert.ToByte(HexStr.Substring(i, 2), 0x10);
            }
            return buffer;
        }

    }

}
