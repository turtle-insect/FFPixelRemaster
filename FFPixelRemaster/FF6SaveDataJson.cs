using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FFPixelRemaster
{
	internal class FF6SaveData : FFSaveData
	{
		[JsonPropertyName("userData")]
		public String UserDataString { get; set; }

		[JsonIgnore]
		public FF6UserData UserData { get; set; }

		[JsonInclude, JsonIgnore]
		public System.Windows.Media.ImageSource Thumbnail
		{
			get
			{
				var buffer = Convert.FromBase64String(ExtensionData?["pictureData"].GetString());
				var decoder = new PngBitmapDecoder(new System.IO.MemoryStream(buffer),
					BitmapCreateOptions.PreservePixelFormat,
					BitmapCacheOption.Default);
				return decoder.Frames[0];
			}
		}

		[JsonExtensionData]
		public IDictionary<String, JsonElement>? ExtensionData { get; set; }

		protected override bool Deserialize(string json)
		{
			var obj = JsonSerializer.Deserialize<FF6SaveData>(json);
			if (obj != null)
			{
				ExtensionData = obj.ExtensionData;
				UserData = JsonSerializer.Deserialize<FF6UserData>(obj.UserDataString);
				UserData.Deserialize();
				return true;
			}

			return false;
		}

		protected override string Serialize()
		{
			UserData.Serialize();
			UserDataString = JsonSerializer.Serialize(UserData, FFSaveData.Options);
			return JsonSerializer.Serialize(this, FFSaveData.Options);
		}
	}

	internal class FF6UserData
	{
		[JsonPropertyName("owendGil")]
		public uint Gil { get; set; }

		[JsonPropertyName("importantOwendItemList")]
		public String ImportantItemString { get; set; }

		[JsonIgnore]
		public FF6Items ImportantItem { get; set; }

		[JsonPropertyName("normalOwnedItemList")]
		public String NormalItemString { get; set; }

		[JsonIgnore]
		public FF6Items NormalItem { get; set; }

		[JsonExtensionData]
		public IDictionary<String, JsonElement>? ExtensionData { get; set; }

		public void Deserialize()
		{
			ImportantItem = JsonSerializer.Deserialize<FF6Items>(ImportantItemString);
			ImportantItem.Deserialize();
			NormalItem = JsonSerializer.Deserialize<FF6Items>(NormalItemString);
			NormalItem.Deserialize();
		}

		public void Serialize()
		{
			ImportantItem.Serialize();
			ImportantItemString = JsonSerializer.Serialize(ImportantItem, FFSaveData.Options);
			NormalItem.Serialize();
			NormalItemString = JsonSerializer.Serialize(NormalItem, FFSaveData.Options);
		}
	}

	internal class FF6Items
	{
		[JsonPropertyName("target")]
		public List<String> TargetString { get; set; }

		[JsonIgnore]
		public List<FF6Item> Items { get; set; } = new List<FF6Item>();

		public void Deserialize()
		{
			foreach (var target in TargetString)
			{
				Items.Add(JsonSerializer.Deserialize<FF6Item>(target));
			}
		}

		public void Serialize()
		{
			TargetString.Clear();
			foreach (var item in Items)
			{
				TargetString.Add(JsonSerializer.Serialize(item, FFSaveData.Options));
			}
		}
	}

	internal class FF6Item
	{
		[JsonPropertyName("contentId")]
		public uint ID { get; set; }

		[JsonPropertyName("count")]
		public uint Count { get; set; }
	}
}
