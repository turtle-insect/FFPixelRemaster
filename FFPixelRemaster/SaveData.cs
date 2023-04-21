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
		const String mPassword = "TKX73OHHK1qMonoICbpVT0hIDGe7SkW0";
		const String mSalt = "71Ba2p0ULBGaE6oJ7TjCqwsls1jBKmRL";

		private String mJson = "";
		private String mFileName = "";
		public bool Open(String filename)
		{
			if (!File.Exists(filename)) return false;

			var buffer = File.ReadAllBytes(filename);
			buffer = Crypt(false, buffer);
			if (buffer.Length == 0) return false;

			using var ds = new DeflateStream(new MemoryStream(buffer), CompressionMode.Decompress);
			using var ms = new MemoryStream();
			try
			{
				ds.CopyTo(ms);
			}
			catch
			{
				return false;
			}

			mJson = Encoding.UTF8.GetString(ms.ToArray());
			mFileName = filename;
			return true;
		}

		public void Save()
		{
			if (String.IsNullOrEmpty(mFileName)) return;
			if (String.IsNullOrEmpty(mJson)) return;

			using var input = new MemoryStream(Encoding.UTF8.GetBytes(mJson));
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

			File.WriteAllBytes(mFileName, buffer);
		}

		public void Import(String filename)
		{
			if (String.IsNullOrEmpty(mFileName)) return;
			if (!File.Exists(filename)) return;

			mJson = File.ReadAllText(filename);
		}

		public void Export(String filename)
		{
			if (String.IsNullOrEmpty(mFileName)) return;
			if (String.IsNullOrEmpty(mJson)) return;

			File.WriteAllText(filename, mJson);
		}

		private Byte[] Crypt(bool isEncryption, Byte[] input)
		{
			Byte[] saltByte = Encoding.UTF8.GetBytes(mSalt);
			using var generator = new Rfc2898DeriveBytes(mPassword, saltByte, 10, HashAlgorithmName.SHA1);

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
