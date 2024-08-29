using System.Reflection;

namespace TableOfRecords;

public static class TableOfRecordsCreator
{
    public static void WriteTable<T>(ICollection<T>? collection, TextWriter? writer)
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(writer);
        if (collection.Count == 0)
        {
            throw new ArgumentException("Collection cannot be empty.", nameof(collection));
        }

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (properties.Length == 0)
        {
            throw new ArgumentException("Collection cannot be empty.", nameof(collection));
        }

        var headers = properties.Select(x => x.Name).ToArray();
        var columnWidths = headers.Select(headers => headers.Length).ToArray();

        foreach (var item in collection)
        {
            for (var i = 0; i < properties.Length; i++)
            {
                var value = properties[i].GetValue(item)?.ToString() ?? string.Empty;
                if (value.Length > columnWidths[i])
                {
                    columnWidths[i] = value.Length;
                }
            }
        }

        WriteRow(writer, headers, columnWidths);

        foreach (var item in collection)
        {
            var values = properties.Select(p => p.GetValue(item)?.ToString() ?? string.Empty).ToArray();
            WriteRow(writer, values, columnWidths);
        }
    }

    private static void WriteRow(TextWriter writer, string[] columns, int[] columnWidths)
    {
        writer.Write("| ");
        for (int i = 0; i < columns.Length; i++)
        {
            writer.Write(columns[i].PadRight(columnWidths[i]));
            writer.Write(" | ");
        }

        writer.WriteLine();
    }
}
