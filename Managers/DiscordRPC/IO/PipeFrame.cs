using System;
using System.IO;
using System.Text;
using Valve.Newtonsoft.Json;

namespace iiMenu.Managers.DiscordRPC.IO
{
	public struct PipeFrame : IEquatable<PipeFrame>
	{
		public Opcode Opcode { get; set; }

		public uint Length
		{
			get
			{
				return (uint)Data.Length;
			}
		}

		public byte[] Data { get; set; }

		public string Message
		{
			get
			{
				return GetMessage();
			}
			set
			{
                SetMessage(value);
			}
		}

		public PipeFrame(Opcode opcode, object data)
		{
            Opcode = opcode;
            Data = null;
            SetObject(data);
		}

		public Encoding MessageEncoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		private void SetMessage(string str)
		{
            Data = MessageEncoding.GetBytes(str);
		}

		private string GetMessage()
		{
			return MessageEncoding.GetString(Data);
		}

		public void SetObject(object obj)
		{
			string message = JsonConvert.SerializeObject(obj);
            SetMessage(message);
		}

		public void SetObject(Opcode opcode, object obj)
		{
            Opcode = opcode;
            SetObject(obj);
		}

		public T GetObject<T>()
		{
			return JsonConvert.DeserializeObject<T>(GetMessage());
		}

		public bool ReadStream(Stream stream)
		{
			uint opcode;
			if (!TryReadUInt32(stream, out opcode))
			{
				return false;
			}
			uint num;
			if (!TryReadUInt32(stream, out num))
			{
				return false;
			}
			uint num2 = num;
			bool result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				uint num3 = (uint)Min(2048, num);
				byte[] array = new byte[num3];
				int count;
				while ((count = stream.Read(array, 0, Min(array.Length, num2))) > 0)
				{
					num2 -= num3;
					memoryStream.Write(array, 0, count);
				}
				byte[] array2 = memoryStream.ToArray();
				if (array2.Length != (long)(ulong)num)
				{
					result = false;
				}
				else
				{
                    Opcode = (Opcode)opcode;
                    Data = array2;
					result = true;
				}
			}
			return result;
		}

		private int Min(int a, uint b)
		{
			if (b >= (ulong)(long)a)
			{
				return a;
			}
			return (int)b;
		}

		private bool TryReadUInt32(Stream stream, out uint value)
		{
			byte[] array = new byte[4];
			if (stream.Read(array, 0, array.Length) != 4)
			{
				value = 0U;
				return false;
			}
			value = BitConverter.ToUInt32(array, 0);
			return true;
		}

		public void WriteStream(Stream stream)
		{
			byte[] bytes = BitConverter.GetBytes((uint)Opcode);
			byte[] bytes2 = BitConverter.GetBytes(Length);
			byte[] array = new byte[bytes.Length + bytes2.Length + Data.Length];
			bytes.CopyTo(array, 0);
			bytes2.CopyTo(array, bytes.Length);
            Data.CopyTo(array, bytes.Length + bytes2.Length);
			stream.Write(array, 0, array.Length);
		}

		public bool Equals(PipeFrame other)
		{
			return Opcode == other.Opcode && Length == other.Length && Data == other.Data;
		}

		public static readonly int MAX_SIZE = 16384;
	}
}
