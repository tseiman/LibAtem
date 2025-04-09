﻿using System;
using System.Linq;
using System.Security.Cryptography;
using LibAtem.Common;

namespace LibAtem.Util.Media
{
    public enum ColourSpace
    {
        BT601,
        BT709,
    }

    public class AtemFrame
    {
        private readonly byte[] _data;

        public string Name { get; }

        public static AtemFrame FromAtem(VideoModeResolution res, string name, byte[] data)
        {
            return new AtemFrame(name, FrameEncodingUtil.DecodeRLE(res, data));
        }

        public static AtemFrame FromYCbCr(string name, byte[] data)
        {
            return new AtemFrame(name, data);
        }

        public static AtemFrame FromRGBA(string name, byte[] data, ColourSpace colour)
        {
            return new AtemFrame(name, GetRGBAToYcbCrConverter(colour)(data));
        }
        
        private static Func<byte[], byte[]> GetRGBAToYcbCrConverter(ColourSpace colour)
        {
            switch (colour)
            {
                case ColourSpace.BT709:
                    return BT709ColourSpaceConverter.RGBAToYCbCrA10Bit422;
                default:
                    throw new ArgumentOutOfRangeException("colour");
            }
        }

        private AtemFrame(string name, byte[] data)
        {
            Name = name;
            _data = data;
        }

        public byte[] GetYCbCrData()
        {
            return _data;
        }

        public byte[] GetRLEEncodedYCbCr()
        {
            return FrameEncodingUtil.EncodeRLE(_data);
        }

        public byte[] GetRGBA(ColourSpace colour)
        {
            switch (colour)
            {
                case ColourSpace.BT709:
                    return BT709ColourSpaceConverter.ToRGBA8(_data);
                default:
                    throw new ArgumentOutOfRangeException("colour");
            }
        }
        
        public byte[] GetBGRA(ColourSpace colour)
        {
            switch (colour)
            {
                case ColourSpace.BT709:
                    return BT709ColourSpaceConverter.ToBGRA8(_data);
                default:
                    throw new ArgumentOutOfRangeException("colour");
            }
        }

		/*    public byte[] GetHash()
			{
				using (MD5 md5Hash = MD5.Create())
					return md5Hash.ComputeHash(_data);
			}
		*/

		public byte[] GetHash() {
			// Use a fake, deterministic-ish "hash"
			// Just take the UTF-8 bytes of the name and timestamp (or random GUID)
			// the dynamic loading of the original MD5.Create() caused
			// System.InvalidOperationException: Argument_CustomAssemblyLoadContextRequestedNameMismatch
            // This Hash is only needed for identifying frames individually
            // which we fake here a bit ...

			var input = Name + "_" + _data.Length;
			return System.Text.Encoding.UTF8.GetBytes(input).Take(32).ToArray();
		}


	}
}