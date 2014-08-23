using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// Csv reader.
/// NOTE Row numbers start at 1 and match the row numbers in excel
/// </summary>
/// <exception cref='IndexOutOfRangeException'>
/// Is thrown when an attempt is made to access an element of an array with an index that is outside the bounds of the array.
/// </exception>
/// 
/// EXAMPLE
/// var r = new UKCsvReader();
/// r.Load("id;value\na;1");
/// foreach(var row in r.EnumDataRows()) Debug.Log(row.GetInt("id"));
public class UKCsvReader
{
	private const string FACILITY = "CSVREADER";
	
	public class CsvRow {
		public string[] data;
		public int lineNumber;
		
		private UKCsvReader reader;
		
		public CsvRow(UKCsvReader reader, string[] data, int lineNumber)
		{
			this.data = data;
			this.reader = reader;
			this.lineNumber = lineNumber;
		}
		
		public bool HasColumn(string columnName)
		{
			return reader.columnNameIndexMap.ContainsKey(columnName);
		}
		
		public string GetLineInfo()
		{
			return string.Format("csv line {0}: {1}", lineNumber, string.Join(", ", data).Substring(0, 10) + "...");
		}
		
		public string GetFirstNonEmptyString(params string[] columnNames)
		{
			foreach(var columnName in columnNames)
			{
				if (HasColumn(columnName) == false)continue;
				
				string v = GetString(columnName);
				
				if (v != null && v.Length > 0)
				{
					return v;
				}
			}
			
			return "";
		}
		
		public string GetString(string columName)
		{
			return data[reader.ColumnHeader(columName)];
		}
		
		public int GetInt(string columnName)
		{
			try {
				return int.Parse(GetString(columnName));
			}
			catch
			{
				return 0;
			}
		}

		public T GetEnum<T>(string columnName) 
		{
			try {
				return (T)System.Enum.Parse(typeof(T), GetString(columnName));
			}
			catch
			{
				return default(T);
			}
		}

		// whitespaces count as empty space
		public bool IsEmpty(string columnName) 
		{
			try {
				string s = GetString(columnName);
				return s == null || s.Trim().Length == 0;
			}
			catch
			{
				return true;
			}
		}

		public float GetFloat(string columnName)
		{
			try {
				return float.Parse(GetString(columnName));
			}
			catch
			{
				return 0f;
			}
		}

		public bool GetBool(string columnName)
		{
			string s = GetString(columnName).ToLower();
			return s == "1" || s == "true" || s == "yes";
		}
		
		public override string ToString ()
		{
			return string.Join(", ", data);
		}
	}
	
	private Dictionary<string, int> columnNameIndexMap;
	
	private List<string[]> lines;
	
	public UKCsvReader ()
	{
		columnNameIndexMap = new Dictionary<string, int>();
	}
	
	/// <summary>
	/// Parses the text.
	/// </summary>
	/// <param name='text'>
	/// Text.
	/// </param>
	/// <param name='quoteChar'>
	/// Quote char.
	/// </param>
	/// <param name='seperatorChar'>
	/// Seperator char.
	/// </param>
	/// <param name='unquote'>
	/// if true quoted parts gets unquoted otherwise they stay the same
	/// </param>
	/// <param name='partCallback'>
	/// Part callback. String in parameter gets trimmed!
	/// </param>
	private void ParseText(string text, char quoteChar, char seperatorChar, bool unquote, Action<string> partCallback)
	{
		bool inQuote = false;
		StringBuilder sb = new StringBuilder();
		for(int i = 0; i < text.Length; ++i)
		{
			char c = text[i];
			
			if (c == quoteChar)
			{
				if (inQuote)
				{
					// quoted quote?
					if (i+1 < text.Length && text[i+1] == quoteChar)
					{
						// add one quote
						sb.Append(c);
						if (unquote == false)sb.Append(text[i+1]);
						++i;
					}
					else
					{
						// quote end
						inQuote = false;
						if (unquote == false)sb.Append(c);
					}
				}
				else
				{
					inQuote = true;
					if (unquote == false)sb.Append(c);
				}
			}
			else if (inQuote == false && c == seperatorChar)
			{
				partCallback(sb.ToString().Trim());
				sb.Remove(0, sb.Length);
			}
			else
			{
				sb.Append(c);
			}
		}
		
		partCallback(sb.ToString().Trim());
		sb.Remove(0, sb.Length);
	}
	
	public void LoadWithDefaultSeperators(string text)
	{
		Load('\n', ';', '"', text);
	}
	
	public void Load(char lineSeperator, char fieldSeperator, char quoteChar, string text)
	{
		List<string> unsplittedLines = new List<string>();
		
		// collect lines
		ParseText(text, quoteChar, lineSeperator, false, (line) => {
			if (line.Length > 0)unsplittedLines.Add(line);
		});
		
		lines = new List<string[]>();
		
		List<string> fields = new List<string>();
		
		string[] header = null;
		
		// collect fields in lines
		foreach(string unsplittedLine in unsplittedLines)
		{
			fields.Clear();
			
			ParseText(unsplittedLine, quoteChar, fieldSeperator, true, (field) => {
				fields.Add(field);
			});
			
			if (header == null)
			{
				// header line
				header = fields.ToArray();
			}
			
			// header is also added as data line that
			// row numbers match the excel row numbers
			// data line
			lines.Add(fields.ToArray());
		}
		
		// read header
		columnNameIndexMap.Clear();
		for(int i = 0; i < header.Length; ++i)
		{
			columnNameIndexMap[header[i]] = i;
		}
	}
	
	public string GetField(int column, int row)
	{
		// rows start at 1
		int rowIndex = row - 1;
		
		if (rowIndex >= lines.Count)
			throw new IndexOutOfRangeException("invalid row: " + row);
		
		var r = lines[rowIndex];
		
		if (column >= r.Length)
			throw new IndexOutOfRangeException("invalid column: " + column);
		
		return r[column];
	}
	
	public string GetField(string columnName, int row)
	{
		return GetField(ColumnHeader(columnName), row);
	}
	
	/// <summary>
	/// Counts the data rows (without header)
	/// </summary>
	/// <returns>
	/// The data rows.
	/// </returns>
	public int CountDataRows()
	{
		return CountRows(true);
	}
	
	public int CountRows(bool skipHeader)
	{
		return lines.Count - (skipHeader ? 1 : 0);
	}
	
	/// <summary>
	/// Enums the data rows (without header).
	/// </summary>
	/// <returns>
	/// The data rows.
	/// </returns>
	public IEnumerable<CsvRow> EnumDataRows()
	{
		return EnumRows(true);
	}
		
	public IEnumerable<CsvRow> EnumRows(bool skipHeader)
	{
		int lineNumber = 0;
		
		// first line contains the header, so skip it
		foreach(var line in (skipHeader ? lines.Skip(1) : lines))
		{
			yield return new CsvRow(this, line, lineNumber);
			++lineNumber;
		}
	}
	
	public int ColumnHeader(string columnName)
	{
		if (columnNameIndexMap.ContainsKey(columnName) == false)
			throw new IndexOutOfRangeException("invalid column name: " + columnName);
		
		return columnNameIndexMap[columnName];
	}
}

