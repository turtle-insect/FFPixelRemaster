using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public FF6Items ImportantItems { get; set; }

		[JsonPropertyName("normalOwnedItemList")]
		public String NormalItemString { get; set; }

		[JsonIgnore]
		public FF6Items NormalItems { get; set; }

		[JsonPropertyName("ownedCharacterList")]
		public String CharacterString { get; set; }

		[JsonIgnore]
		public FF6Characters Characters { get; set; }

		[JsonExtensionData]
		public IDictionary<String, JsonElement>? ExtensionData { get; set; }

		public void Deserialize()
		{
			ImportantItems = JsonSerializer.Deserialize<FF6Items>(ImportantItemString);
			ImportantItems.Deserialize();
			NormalItems = JsonSerializer.Deserialize<FF6Items>(NormalItemString);
			NormalItems.Deserialize();
			Characters = JsonSerializer.Deserialize<FF6Characters>(CharacterString);
			Characters.Deserialize();
		}

		public void Serialize()
		{
			ImportantItems.Serialize();
			ImportantItemString = JsonSerializer.Serialize(ImportantItems, FFSaveData.Options);
			NormalItems.Serialize();
			NormalItemString = JsonSerializer.Serialize(NormalItems, FFSaveData.Options);
			Characters.Serialize();
			CharacterString = JsonSerializer.Serialize(Characters, FFSaveData.Options);
		}
	}

	internal class FF6Characters
	{
		[JsonPropertyName("target")]
		public List<String> TargetString { get; set; }

		[JsonIgnore]
		public ObservableCollection<FF6Character> Targets { get; set; } = new ObservableCollection<FF6Character>();

		public void Deserialize()
		{
			foreach (var target in TargetString)
			{
				var character = JsonSerializer.Deserialize<FF6Character>(target);
				character?.Deserialize();
				Targets.Add(character);
			}
		}

		public void Serialize()
		{
			TargetString.Clear();
			foreach (var target in Targets)
			{
				target.Serialize();
				TargetString.Add(JsonSerializer.Serialize(target, FFSaveData.Options));
			}
		}
	}

	internal class FF6Character
	{
		[JsonPropertyName("addtionalLevel")]
		public uint Lv { get; set; }

		[JsonPropertyName("name")]
		public String Name { get; set; }

		[JsonPropertyName("currentExp")]
		public uint Exp { get; set; }

		[JsonExtensionData]
		public IDictionary<String, JsonElement>? ExtensionData { get; set; }

		public void Deserialize()
		{

		}

		public void Serialize()
		{

		}
	}

	internal class FF6Items
	{
		[JsonPropertyName("target")]
		public List<String> TargetString { get; set; }

		[JsonIgnore]
		public ObservableCollection<FF6Item> Targets { get; set; } = new ObservableCollection<FF6Item>();

		public void Deserialize()
		{
			foreach (var target in TargetString)
			{
				Targets.Add(JsonSerializer.Deserialize<FF6Item>(target));
			}
		}

		public void Serialize()
		{
			TargetString.Clear();
			foreach (var target in Targets)
			{
				TargetString.Add(JsonSerializer.Serialize(target, FFSaveData.Options));
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
