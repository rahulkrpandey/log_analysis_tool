using System.Collections.Generic;
using System.IO;
using CSVFile;
using System.Linq;
using UnityEngine;

public class CSVParser
{
    private static CSVParser csParser;
    public static CSVParser CsParser
    {
        get
        {
            if (csParser == null)
            {
                csParser = new CSVParser();
            }

            return csParser;
        }

        private set
        {

        }
    }

    public string[] HEADERS;
    public List<List<string>> RECORDS;
    public Dictionary<string, int> HEADERS_INDEXS;
    private CSVParserUtility cpu;

    private CSVParser ()
    {
        RECORDS = new();
        cpu = CSVParserUtility.Cpu;
        HEADERS_INDEXS = new();
    }

    public void ParseCsvFile(string path)
    {
        using StreamReader sr = new(path);
        cpu.ParseCsvFile(sr);

        HEADERS = cpu.GetData()[0].ToArray();
        RECORDS.Clear();
        HEADERS_INDEXS.Clear();

        bool ignoreHeader = true;
        foreach (var row in cpu.GetData())
        {
            if (ignoreHeader)
            {
                ignoreHeader = false;
                continue;
            }
            RECORDS.Add(row.ToList());
            //Debug.Log("~~~~~~~~~~~~~~~log~~~~~~~~~~~~~~");
            //foreach (var str in row)
            //{
            //    Debug.Log(str);
            //}
        }

        for (int i = 0; i < HEADERS.Length; i++)
        {
            HEADERS_INDEXS[HEADERS[i]] = i;
        }
    }

    public Dictionary<string, int> GetHeadersIndexes()
    {
        return HEADERS_INDEXS;
    }
}
