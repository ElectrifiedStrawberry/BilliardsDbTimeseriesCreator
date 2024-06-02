using System;

namespace BilliardsDbTimeseriesCreator
{
	internal static class Program
	{
		private static readonly List<DatabaseEntry> Entries = new List<DatabaseEntry>(7_500_000);

		static void Main(String[] args)
		{
			foreach (String file in args.SkipLast(1))
			{
				using (FileStream f = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					Read(new StreamReader(f), file);
				}
			}
			Entries.Sort(new Comparison<DatabaseEntry>((e1, e2) => e1.UnixTime.CompareTo(e2.UnixTime)));
			using (FileStream fOut = File.Open(args.Last(), FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				using (StreamWriter sw = new StreamWriter(fOut))
				{
					foreach (DatabaseEntry entry in Entries)
					{
						sw.WriteLine($"{entry.UnixTime}\t{entry.Sunk}");
					}
				}
			}
		}

		public static void Read(TextReader reader, String fileName)
		{
			Boolean hasSchemaVersion = true;
			String? line = reader.ReadLine() ?? throw new Exception("No useful data on first line (please headerify the file)");
			Int32 lineNo = 1;
			if (line.StartsWith("StartupTime") || !line.StartsWith("VersionSchema"))
			{
				// Schema version isn't stored in this type of file.
				hasSchemaVersion = false;
			}
			

			line = reader.ReadLine();
			lineNo++;
			while (line != null)
			{
				ReadOnlySpan<String> cols = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
				if (cols.Length == 0)
				{
					// Do nothing on blank lines.
				}
				else if (!hasSchemaVersion)
				{
					if (cols.Length == 19) // pre-2024-05-05 version
					{
						ReadLineNoSunkNoCueBall(cols, 1, 13, 16, 17);
					}
					else if (cols.Length == 20) // 2024-05-09 version
					{
						ReadLineNoSunkNoCueBall(cols, 1, 14, 17, 18);
					}
					else if (cols.Length == 23) // 2024-05-11 version
					{
						ReadLineNoSunkNoCueBall(cols, 1, 14, 17, 18);
					}
					else
					{
						throw new NotImplementedException($"Unexpected column length: {cols.Length} in file {fileName} (line {lineNo})");
					}
				}
				else
				{
					switch (cols[0])
					{
						case "1": throw new NotImplementedException("Schema version 1");
						case "2": throw new NotImplementedException("Schema version 2");
						case "3": // 2024-05-20
							ReadLine(cols, 3, 19, 22);
							break;
						case "4":
							ReadLine(cols, 4, 20, 23);
							break;
						default:
							throw new Exception($"Invalid data (schema version {cols[0]}) in file {fileName} (line {lineNo})");
					}
				}
				line = reader.ReadLine();
				lineNo++;
			}
		}

		public static void ReadLine(ReadOnlySpan<String> columns, Int32 timeIdx, Int32 sunkIdx, Int32 flagsIdx)
		{
			Int32 sunk = Int32.Parse(columns[sunkIdx]);
			if (Int32.Parse(columns[flagsIdx]) == 3) sunk = -1; // Invalid shot
			Int64 time = DateTimeOffset.Parse(columns[timeIdx]).ToUnixTimeSeconds();
			Entries.Add(new DatabaseEntry(time, sunk));
		}

		public static void ReadLineNoSunkNoCueBall(ReadOnlySpan<String> columns, Int32 timeIdx, Int32 sunkIdx, Int32 flagsIdx, Int32 foulTypeIdx)
		{
			Int32 sunk = Int32.Parse(columns[sunkIdx]);
			Int64 time = DateTimeOffset.Parse(columns[timeIdx]).ToUnixTimeSeconds();

			Int32 foul = Int32.Parse(columns[foulTypeIdx]);
			if (foul == 1) sunk--; // Sunk the cue ball
			if (Int32.Parse(columns[flagsIdx]) == 3) sunk = -1; // Invalid shot

			Entries.Add(new DatabaseEntry(time, sunk));
		}
	}
}
