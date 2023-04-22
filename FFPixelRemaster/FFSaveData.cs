using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FFPixelRemaster
{
	internal abstract class FFSaveData
	{
		public bool Open(String json) => Deserialize(json);
		public String Save() => Serialize();

		protected abstract bool Deserialize(String json);
		protected abstract String Serialize();

		public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions()
		{
			Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};
	}
}
