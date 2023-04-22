using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace FFPixelRemaster
{
	internal class SaveData
	{
		public static String Open(String filename)
		{
			if (!File.Exists(filename)) return "";

			var buffer = File.ReadAllBytes(filename);
			buffer = Crypt(false, buffer);
			if (buffer.Length == 0) return "";

			using var ds = new DeflateStream(new MemoryStream(buffer), CompressionMode.Decompress);
			using var ms = new MemoryStream();
			try
			{
				ds.CopyTo(ms);
			}
			catch
			{
				return "";
			}

			return Encoding.UTF8.GetString(ms.ToArray());
		}

		public static void Save(String filename, String json)
		{
			if (String.IsNullOrEmpty(filename)) return;
			if (String.IsNullOrEmpty(json)) return;

			using var input = new MemoryStream(Encoding.UTF8.GetBytes(json));
			using var ms = new MemoryStream();
			using var ds = new DeflateStream(ms, CompressionMode.Compress);
			try
			{
				input.CopyTo(ds);
				ds.Close();
			}
			catch
			{
				return;
			}

			var buffer = Crypt(true, ms.ToArray());
			if (buffer.Length == 0) return;

			File.WriteAllBytes(filename, buffer);
		}

		private static Byte[] Crypt(bool isEncryption, Byte[] input)
		{
			Byte[] saltByte = Encoding.UTF8.GetBytes("71Ba2p0ULBGaE6oJ7TjCqwsls1jBKmRL");
			using var generator = new Rfc2898DeriveBytes("TKX73OHHK1qMonoICbpVT0hIDGe7SkW0", saltByte, 10, HashAlgorithmName.SHA1);

			var blockCipher = new CbcBlockCipher(new RijndaelEngine(256));
			var cipher = new PaddedBufferedBlockCipher(blockCipher, new ZeroBytePadding());
			var parameters = new ParametersWithIV(new KeyParameter(generator.GetBytes(32)), generator.GetBytes(32));

			Byte[] output = new Byte[cipher.GetOutputSize(input.Length)];

			try
			{
				cipher.Init(isEncryption, parameters);
				int bytesProcessed = cipher.ProcessBytes(input, output, 0);
				cipher.DoFinal(output, bytesProcessed);
			}
			catch
			{
				return new Byte[0];
			}

			return output;
		}
	}
}
