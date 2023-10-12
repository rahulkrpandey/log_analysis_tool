using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

public class FunnelAnalyser
{
    private CSVParser cp;
    private Dictionary<string, List<int>> groupedRecords;
    private static FunnelAnalyser fa;
    public static FunnelAnalyser Fa
    {
        get {
            if (fa == null)
            {
                fa = new FunnelAnalyser();
            }

            return fa;
        }

        private set
        {

        }
    }

    private FunnelAnalyser()
    {
        cp = CSVParser.CsParser;
        groupedRecords = new();
    }

    public void ParseCsvFile(string path)
    {
        groupedRecords.Clear();
        cp.ParseCsvFile(path);
    }

    public void GroupByKeys(List<int> keys)
    {
        //if (keys == null || keys.Count == 0)
        //{
        //    List<int> rowIndexes = new();
        //    for (int i = 0; i < cp.RECORDS.Count; i++)
        //    {
        //        rowIndexes.Add(i);
        //    }

        //    groupedRecords["$"] = rowIndexes;
        //    return;
        //}

        //foreach (var key in keys)
        //{
        //    Debug.Log($"GroupBy keys: {GetHeaders()[key]}");
        //    Debug.Log($"Records length: {cp.RECORDS.Count}");
        //}
        int rowIdx = 0;
        foreach (List<string> rows in cp.RECORDS)
        {
            StringBuilder dictionaryKey = new();
            foreach (int key in keys)
            {
                dictionaryKey.Append($"{rows[key]}$$,$$");
            }

            Debug.Log($"Dictionary key: {dictionaryKey}");

            List<int> list;
            string tempDictionaryKey = dictionaryKey.ToString();
            if (groupedRecords.ContainsKey(tempDictionaryKey) == false)
            {
                list = new();
                groupedRecords.Add(tempDictionaryKey, list);
            } else
            {
                list = groupedRecords[tempDictionaryKey];
            }
            
            list.Add(rowIdx++);
        }
    }

    public List<List<int>> AnalyseFunnel(List<int> columns, List<string> vals)
    {
        for (int i = 0; i <= columns.Count - 1; i++)
        {
            Debug.Log($"Funnel Steps: {GetHeaders()[columns[i]]} | {vals[i]}");
        }

        int[] groupsThatReachKthStep = new int[columns.Count+1];

        int currentState = 0;
        List<List<int>> result = new();

        foreach (var record in groupedRecords)
        {
            List<int> rows = record.Value;
            Debug.Log($"Group Key: {record.Key}");

            int maxState = 0;
            foreach (int row in rows)
            {
                Debug.Log($"row: {row}, columns: {columns[currentState]}, currentState: {currentState}, record_val: {cp.RECORDS[row][columns[currentState]]}, vals: {vals[currentState]}");
                Debug.Log($"recordLength = {cp.RECORDS[row][columns[currentState]].Length}, vals length: {vals[currentState].Length}");
                if (currentState < columns.Count && string.Equals(cp.RECORDS[row][columns[currentState]], vals[currentState]))
                {
                    Debug.Log($"current state increased");
                    currentState++;
                }

                maxState = Mathf.Max(maxState, currentState);
                if (currentState == columns.Count)
                {
                    result.Add(rows);
                    currentState = 0;
                    break;
                }
            }

            groupsThatReachKthStep[maxState]++;
            currentState = 0;
        }

        for (int i = columns.Count - 1; i >= 0; i--)
        {
            groupsThatReachKthStep[i] += groupsThatReachKthStep[i + 1];
        }

        result.Add(groupsThatReachKthStep.ToList());

        return result;
    }

    public List<string> GetData(int row)
    {
        return cp.RECORDS[row];
    }

    //public int[] GetColumnNumbers(string[] headers)
    //{
    //    int[] column = new int[headers.Length];
    //    for (int i = 0; i < headers.Length; i++)
    //    {
    //        column[i] = cp.HEADERS_INDEXS[headers[i]];
    //    }
    //    return column;
    //}

    public string[] GetHeaders()
    {
        return cp.HEADERS;
    }

    public Dictionary<string, int> GetHeadersIndexes()
    {
        return cp.GetHeadersIndexes();
    }
}
