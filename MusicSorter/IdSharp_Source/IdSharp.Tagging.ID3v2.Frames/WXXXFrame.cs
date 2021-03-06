namespace IdSharp.Tagging.ID3v2.Frames
{
    using IdSharp.Tagging.ID3v2;
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal sealed class WXXXFrame : IWXXXFrame, IFrame, INotifyPropertyChanged, ITextEncoding
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WXXXFrame()
        {
            this.m_FrameHeader = new IdSharp.Tagging.ID3v2.FrameHeader();
        }

        private void FirePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler1 = this.PropertyChanged;
            if (handler1 != null)
            {
                handler1(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public byte[] GetBytes(ID3v2TagVersion tagVersion)
        {
            if (string.IsNullOrEmpty(this.Value))
            {
                return new byte[0];
            }
            byte[] buffer1 = new byte[] { this.TextEncoding };
            byte[] buffer2 = Utils.GetStringBytes(tagVersion, this.TextEncoding, this.Description, true);
            byte[] buffer3 = Utils.ISO88591GetBytes(this.Value);
            using (MemoryStream stream1 = new MemoryStream())
            {
                stream1.Write(buffer1, 0, buffer1.Length);
                stream1.Write(buffer2, 0, buffer2.Length);
                stream1.Write(buffer3, 0, buffer3.Length);
                return this.m_FrameHeader.GetBytes(stream1, tagVersion, this.GetFrameID(tagVersion));
            }
        }

        public string GetFrameID(ID3v2TagVersion tagVersion)
        {
            switch (tagVersion)
            {
                case ID3v2TagVersion.ID3v22:
                    return "WXX";

                case ID3v2TagVersion.ID3v23:
                case ID3v2TagVersion.ID3v24:
                    return "WXXX";
            }
            throw new ArgumentException("Unknown tag version");
        }

        public void Read(TagReadingInfo tagReadingInfo, Stream stream)
        {
            this.m_FrameHeader.Read(tagReadingInfo, ref stream);
            if (this.m_FrameHeader.FrameSizeExcludingAdditions > 0)
            {
                this.TextEncoding = (EncodingType) Utils.ReadByte(stream);
                int num1 = this.m_FrameHeader.FrameSizeExcludingAdditions - 1;
                this.Description = Utils.ReadString(this.TextEncoding, stream, ref num1);
                this.Value = Utils.ReadString(EncodingType.ISO88591, stream, num1);
            }
            else
            {
                this.Description = "";
                this.Value = "";
            }
        }


        public string Description
        {
            get
            {
                return this.m_Description;
            }
            set
            {
                this.m_Description = value;
                this.FirePropertyChanged("Description");
            }
        }

        public IFrameHeader FrameHeader
        {
            get
            {
                return this.m_FrameHeader;
            }
        }

        public EncodingType TextEncoding
        {
            get
            {
                return this.m_TextEncoding;
            }
            set
            {
                this.m_TextEncoding = value;
                this.FirePropertyChanged("TextEncoding");
            }
        }

        public string Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
                this.FirePropertyChanged("Value");
            }
        }


        private string m_Description;
        private IdSharp.Tagging.ID3v2.FrameHeader m_FrameHeader;
        private EncodingType m_TextEncoding;
        private string m_Value;
    }
}

