using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Classes
{
	public class ByteAssembler
	{
		public long targetLength;

		public List<byte[]> byteFragments = new List<byte[]>();
		public int currentLength = 0;

		public ByteAssembler(long targetLength)
		{
			this.targetLength = targetLength;
		}

		public void AddByteArray(byte[] array, int usingInt)
		{
			byteFragments.Add(SubArray<byte>(array, 0, usingInt));
			currentLength += usingInt;
		}

		public bool isFull
		{
			get { return targetLength <= currentLength; }
		}

		public byte[] Create()
		{
			if (isFull)
			{
				byte[] returnValue = new byte[0];
				foreach(byte[] array in byteFragments)
				{
					returnValue = Combine(returnValue, array);
				}
				return returnValue;
			}
			else
			{
				return null;
			}
		}

		public void Clear(long targetLength)
		{
			byteFragments = new List<byte[]>();
			currentLength = 0;
			this.targetLength = targetLength;
		}

		private byte[] Combine(params byte[][] arrays)
		{
			byte[] rv = new byte[arrays.Sum(a => a.Length)];
			int offset = 0;
			foreach (byte[] array in arrays)
			{
				System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
				offset += array.Length;
			}
			return rv;
		}

		private T[] SubArray<T>(T[] array, int offset, int length)
		{
			T[] result = new T[length];
			Array.Copy(array, offset, result, 0, length);
			return result;
		}
	}
}
