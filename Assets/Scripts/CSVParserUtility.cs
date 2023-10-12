using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

public class CSVParserUtility
{
    private List<List<string>> data;
    private List<string> result;
    private StringBuilder token;
    private bool quotationEnd;

    private static CSVParserUtility cpu;
    public static CSVParserUtility Cpu
    {
        get
        {
            if (cpu == null)
            {
                cpu = new CSVParserUtility();
            }

            return cpu;
        }

        private set
        {

        }
    }

    public CSVParserUtility()
    {
        data = new List<List<string>>();
        result = new List<string>();
        token = new StringBuilder();
        quotationEnd = true;
    }

    public void ParseCsvFile(StreamReader sr)
    {
        data.Clear();
        result.Clear();
        token.Clear();
        quotationEnd = true;
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            ParseLine(line);
        }
    }

    public List<List<string>> GetData()
    {
        return data;
    }

    private void ParseLine(string line)
    {
            
        int currChar = 0;
        int n = line.Length;
        while (currChar < n)
        {
            if (quotationEnd == true && line[currChar] != '"')
            {
                while (currChar < n && line[currChar] != ',')
                {
                    token.Append(line[currChar++]);
                }

                if (currChar < n)
                {
                    currChar++;
                    if (currChar == n)
                    {
                        result.Add(token.ToString().Trim());
                        token.Clear();
                    }
                }

                result.Add(token.ToString().Trim());
                token.Clear();
            } else
            {
                if (quotationEnd == true)
                {
                    quotationEnd = false;
                    currChar++;
                } else
                {
                    token.Append("/n");
                }
                    
                while (currChar < n)
                {
                    if (line[currChar] == '"' && (currChar + 1 == n || line[currChar + 1] == ','))
                    {
                        currChar++;
                        quotationEnd = true;
                        break;
                    } else 
                    {
                        if (line[currChar] == '"')
                        {
                            currChar++;
                        }
                            
                        token.Append(line[currChar++]);
                    }
                }

                if (quotationEnd == false)
                {
                    return;
                }

                if (currChar < n)
                {
                    currChar++;
                    if (currChar == n)
                    {
                        result.Add(token.ToString().Trim());
                        token.Clear();
                    }
                }

                result.Add(token.ToString().Trim());
                token.Clear();
            }
        }

        if (quotationEnd)
        {
            data.Add(result);
            result = new List<string>();
        }
    }
}

