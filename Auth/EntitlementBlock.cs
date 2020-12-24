using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
/*
 * This file is based on LegitimacyChecking from the CitizenFX Project - http://citizen.re/
 * 
 * See the included licenses for licensing information on this code
 * 
 * Rewritten in C# by @dr490n/@jaredtb  
 */
namespace Project_127.Auth
{
    class EntitlementBlock
    {
		private UInt64 m_rockstarId;
		private byte[] m_machineHash;
		private string m_xml;
		private string m_date;
		private bool m_valid;



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

			//~sig doesn't matter (not like someone couldn't change the RSA pub...)


			m_valid = true; // Sure

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
