using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * This file is based on LegitimacyChecking from the CitizenFX Project - http://citizen.re/
 * 
 * See the included licenses for licensing information on this code
 * 
 * Rewritten in C# by @dr490n/@jaredtb  
 */
namespace Project_127
{
    class EntitlementBlock
    {
		private UInt64 m_rockstarId;
		private byte[] m_machineHash;
		private string m_xml;
		private string m_date;
		private bool m_valid;


		private static byte[] rsa_pub = {
			0x30,0x81,0x89,0x02,0x81,0x81,0x00,0xAB,0xC1,0x95,0x54,
			0x2B,0x9C,0x48,0x52,0x36,0x71,0xC9,0x15,0x2B,0x09,0xB8,
			0x59,0xD0,0x16,0xD5,0xA4,0x2A,0x7A,0x84,0xFC,0x55,0xF6,
			0x5D,0x79,0x2B,0xCD,0x81,0x78,0xA3,0x2B,0x02,0x7D,0x7F,
			0xFC,0x34,0xEE,0x4F,0x18,0x73,0xF5,0xDE,0xC1,0x22,0xC7,
			0xFC,0xC4,0x2B,0xFE,0xAA,0x8D,0xC8,0x05,0xCC,0x40,0x97,
			0xCF,0xEA,0x0A,0x5A,0x42,0xB0,0x24,0xB7,0xE6,0x17,0x6C,
			0x9F,0x1C,0xBE,0x17,0xA7,0x51,0xB8,0xF5,0xDA,0x9B,0xEF,
			0x25,0x1A,0xE0,0xE1,0x1B,0x8E,0x80,0x12,0x5B,0x52,0x3E,
			0x49,0x5B,0xD5,0xF5,0xBB,0x5B,0x0E,0xB0,0x6C,0x7D,0x35,
			0x02,0x22,0x32,0xC9,0xCF,0x80,0xA4,0x94,0x4C,0x12,0x26,
			0x40,0x0B,0xDA,0x81,0xDD,0x6E,0x65,0xD9,0x3D,0xC4,0x44,
			0x6B,0x42,0x17,0x02,0x03,0x01,0x00,0x01,
		};

		private UInt32 BigLong(UInt32 val)
        {
			val = ((val << 8) & 0xFF00FF00) | ((val >> 8) & 0xFF00FF);
			return (val << 16) | (val >> 16);
		}

		private UInt64 BigLongLong(UInt64 val)
        {
			val = ((val << 8) & 0xFF00FF00FF00FF00) | ((val >> 8) & 0x00FF00FF00FF00FF);
			val = ((val << 16) & 0xFFFF0000FFFF0000) | ((val >> 16) & 0x0000FFFF0000FFFF);
			return (val << 32) | (val >> 32);
		}

		public EntitlementBlock(byte[] buffer)
        {
			int offset = 0;

			var length = BigLong(BitConverter.ToUInt32(buffer, offset));
			offset += 4;

			m_rockstarId = BigLongLong(BitConverter.ToUInt64(buffer, offset));
			offset += 8;

			var machineHashSize = BigLong(BitConverter.ToUInt32(buffer, offset));
			offset += 4;

			m_machineHash = new ArraySegment<byte>(buffer, offset, (int)machineHashSize).ToArray();
			offset += (int)machineHashSize;

			var dateSize = BigLong(BitConverter.ToUInt32(buffer, offset));
			offset += 4;

			m_date = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, offset, (int)dateSize).ToArray());
			offset += (int)dateSize;

			var xmlSize = BigLong(BitConverter.ToUInt32(buffer, offset));
			offset += 4;

			m_xml = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, offset, (int)xmlSize).ToArray());
			offset += (int)xmlSize;

			var sigSize = BigLong(BitConverter.ToUInt32(buffer, offset));
			offset += 4;

			var sig = new List<byte>((int)sigSize);
			sig.AddRange(new ArraySegment<byte>(buffer, offset, (int)sigSize));

			//~sig doesn't matter atm

			m_valid = true; // for now

		}

		public UInt64 GetRockstarId()
        {
			return m_rockstarId;
        }

		public byte[] GetMachineHash()
        {
			return m_machineHash;
        }

		public string GetXml()
        {
			return m_xml;
        }

		public string GetExpirationDate()
        {
			return m_date;
        }

		public bool IsValid()
        {
			return m_valid;
        }

	}
}
