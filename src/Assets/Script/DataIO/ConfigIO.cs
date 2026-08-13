using System; // `StringSplitOptions`と`IDisposable`に必要
using System.IO; // ファイル入出力
using UnityEngine;

public class ConfigIO : CsvIO
{
    public ConfigIO(
        int _recordCount,
        int _fieldCount = 2,
        string _delimiter = "=",
        string _premodifier = " ",
        string _postmodifier = " "
    )
        : base(_recordCount, _fieldCount, _delimiter, _premodifier, _postmodifier) { }

    public new void Flush(string path)
    {
        string flushDelimiter = premodifier + delimiter + postmodifier;
        string[] records = new string[recordCount];

        for (int i = 0; i < recordCount; i++) // 行ごと
        {
            string line = "";
            line += buff[i, 0];

            if (!line.StartsWith("//") && line.Trim() != "") // コメント行や空行は区切り文字を入れない
            {
                line += flushDelimiter + buff[i, 1];
            }

            records[i] = line; // `Join`を使って1次元配列(fields)を区切り文字で文字列に
        }

        try
        {
            File.WriteAllLines(path, records); // 全ての行（string型配列）を書き込む（エンコードはUTF-8）
            // サンプルコード -> https://learn.microsoft.com/ja-jp/dotnet/standard/io/how-to-write-text-to-a-file#example-write-and-append-text-with-the-file-class
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error flushing Config: " + e);
        }
    }
}
