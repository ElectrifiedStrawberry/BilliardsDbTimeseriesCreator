using System;

namespace BilliardsDbTimeseriesCreator
{
	public record struct DatabaseEntry(Int64 UnixTime, Int32 Sunk)
	{
	}
}
