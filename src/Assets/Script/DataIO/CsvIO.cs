using UnityEngine;

using System; // `StringSplitOptions`と`IDisposable`に必要
using System.IO; // ファイル入出力
using System.Text; // 文字列制御

/*
// ===== 使い方 =====
using (CsvIO csv = new CsvIO(50, 20))
{
    csv.Write(1, 1, "Hello");
    csv.Write(2, 1, "World");
    csv.Flush(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Test.csv"));
}

using (CsvIO csv = new CsvIO(50, 20))
{
    csv.Load(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Test.csv"));
    print(csv.Read(1, 1));
    print(csv.Read(2, 1));
}
// ===== ======= =====
*/

public class CsvIO : IDisposable // : MonoBehaviour // newキーワードでインスタンス化するために`MonoBehaviour`を継承させない
{
    protected int recordCount;
    protected int fieldCount;
    protected string delimiter; // 区切り文字（このコードでは`,`を使用するため，初期値は`,`にしておく）
    protected string premodifier; // 区切り文字の前置修飾文字
    protected string postmodifier; // 区切り文字の後置修飾文字
    protected string[,] buff; // バッファ用2次元配列

    public CsvIO(int _recordCount, int _fieldCount, 
                string _delimiter = ",", string _premodifier = "", string _postmodifier = " ")
    {
        recordCount = _recordCount;
        fieldCount = _fieldCount;
        delimiter = _delimiter;
        premodifier = _premodifier;
        postmodifier = _postmodifier;
        buff = new string[recordCount, fieldCount];
    }

    public void Load(string path) // CSVからバッファに読み出す
    {
        try
        {
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) // usingブロックから出るとリソース解放
            {
                string readTxt = sr.ReadToEnd();
                //Debug.Log("Read CSV content:\n" + readTxt);

                Array.Clear(buff, 0, buff.Length); // データリストの初期化

                string[] records = readTxt.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // 改行ごとに分割し，`records`の要素に1つずつ代入

                for (int i = 0; i < records.Length; i++) // レコード（行）ごと
                {
                    if (recordCount <= i) break; // レコードの長さが配列の大きさを超えたら`break`

                    string[] fields = records[i].Split(new[] { delimiter }, StringSplitOptions.None); // 区切り文字ごとに分割し，`fields`の要素に1つずつ代入

                    for (int j = 0; j < fields.Length; j++) // フィールド（列）ごと
                    {
                        if (fieldCount <= j) break; // フィールドの長さが配列の大きさを超えたら`break`

                        buff[i, j] = fields[j]; // i行j列の配列要素に，i番目のレコードのj番目のフィールドを代入
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error loading CSV: " + e);
        }
    }

    public string Read(int recordNum, int fieldNum) // バッファの内容を読む
    {
        recordNum--;
        fieldNum--;
        try
        {
            return buff[recordNum, fieldNum]?.Trim();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error reading CSV: " + e);
            return null;
        }
    }

    public void Write(int recordNum, int fieldNum, string str) // バッファに内容を追加/上書きする
    {
        recordNum--;
        fieldNum--;
        try
        {
            buff[recordNum, fieldNum] = str.Trim();
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error writing CSV: " + e);
        }
    }

    public void Clear() // バッファの内容をクリアする
    {
        Array.Clear(buff, 0, buff.Length);
    }

    public void Flush(string path) // バッファからCSVに書き込む
    {
        string flushDelimiter = premodifier + delimiter + postmodifier;
        string[] records = new string[recordCount];

        for (int i = 0; i < recordCount; i++) // 行ごと
        {
            string[] fields = new string[fieldCount];

            for (int j = 0; j < fieldCount; j++) // 列ごと
            {
                fields[j] = buff[i, j] ?? ""; // null合体演算子で`null`の場合には""を代入
            }

            records[i] = string.Join(flushDelimiter, fields); // `Join`を使って1次元配列(fields)を区切り文字で文字列に
        }

        try
        {
            File.WriteAllLines(path, records); // 全ての行（string型配列）を書き込む（エンコードはUTF-8）
            // サンプルコード -> https://learn.microsoft.com/ja-jp/dotnet/standard/io/how-to-write-text-to-a-file#example-write-and-append-text-with-the-file-class
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error flushing CSV: " + e);
        }
    }

    public void Dispose() // usingブロックが使えるようになり，抜けると自動的に実行
    {
        buff = null; // 参照を切る -> 自動的に破棄される
    }
}
