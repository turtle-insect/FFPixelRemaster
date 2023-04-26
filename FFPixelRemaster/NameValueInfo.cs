using System;
using System.Collections.Generic;

namespace FFPixelRemaster
{
	internal class NameValueInfo : IComparable
	{
		public uint Value { get; set; }
		public String Genre { get; set; } = "";
		public String Name
		{
			get
			{
				if (Names.Count == 0) return "";

				var index = Properties.Settings.Default.lang;
				if (index >= Names.Count) return "";

				var value = Names[index];
				if(String.IsNullOrEmpty(value)) value = Names[0];
				return value;
			}
		}
		private List<String> Names { get; set; } = new List<String>();

		public int CompareTo(object? obj)
		{
			var dist = obj as NameValueInfo;
			if (dist == null) return 0;

			if (Value < dist.Value) return -1;
			else if (Value > dist.Value) return 1;
			else return 0;
		}

		public bool Line(String[] oneLine)
		{
			if (oneLine[0].Length > 1 && oneLine[0][1] == 'x') Value = Convert.ToUInt32(oneLine[0], 16);
			else Value = Convert.ToUInt32(oneLine[0]);
			Genre = oneLine[1];
			for (int i = 2; i < oneLine.Length; i++)
			{
				Names.Add(oneLine[i]);
			}
			return true;
		}
	}
}
