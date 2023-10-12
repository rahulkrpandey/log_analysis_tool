using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AnalyserCanvas : MonoBehaviour
{
    [SerializeField] private GameObject FileName;
    [SerializeField] private GameObject ParseButton;
    [SerializeField] private GameObject GroupBy;
    [SerializeField] private GameObject Column;
    [SerializeField] private GameObject ColumnValue;
    [SerializeField] private GameObject FunnelItem;
    [SerializeField] private GameObject FunnelSteps;
    [SerializeField] private GameObject FunnelStepsArea;
    [SerializeField] private GameObject GroupArea;
    [SerializeField] private GameObject AnalyseButton;
    [SerializeField] private GameObject Fields;
    [SerializeField] private GameObject OutputButton;
    [SerializeField] private GameObject Table;
    [SerializeField] private GameObject TableItem;
    [SerializeField] private GameObject OutputContent;


    private FunnelAnalyser fa;
    private Queue<FunnelItemC> funnelList, groupList;
    private Queue<GameObject> tableItemList;
    private List<List<int>> resultRows;

    private void Start()
    {
        funnelList = new();
        tableItemList = new();
        resultRows = new();
        groupList = new();
    }

    private void OnEnable()
    {
        Actions.RemoveItemFromList += OnRemoveItemClicked;
        Actions.ChangeGroupItem += OnGroupChanged;
    }

    private void OnDisable()
    {
        Actions.RemoveItemFromList -= OnRemoveItemClicked;
        Actions.ChangeGroupItem -= OnGroupChanged;
    }

    public void OnParseClicked()
    {
        var obj = ParseButton.GetComponent<Button>();
        if (obj != null)
        {
            obj.interactable = false;
        }
        try
        {
            fa = FunnelAnalyser.Fa;
            string path = Application.streamingAssetsPath + "/TextAssets" + $"/{FileName.GetComponent<TMP_InputField>().text}.csv";
            Debug.Log($"Parse button clicked {path}");
            fa.ParseCsvFile(path);
            PopulateGroupByOptions();
            PopulateColumnOptions();
            obj.interactable = true;
        } catch (Exception e)
        {
            obj.interactable = true;
            Debug.Log(e.Message);
        } finally
        {
            if (obj != null)
            {
                obj.interactable = true;
            }
        }
        
    }

    public void OnAddFunnelItemButtonClicked()
    {
        try
        {
            var dropdown = Column.GetComponent<TMP_Dropdown>();
            string col = dropdown.options[dropdown.value].text;
            string val = ColumnValue.GetComponent<TMP_InputField>().text.Trim();
            Debug.Log($"FunelItem: {val}, length: {val.Length}");
            //Debug.Log($"{col} | {val} ...");
            for (int i = val.Length - 1; i >= 0; i--)
            {
                Debug.Log($"char value: {val[i]}");
            }

            InstantiateFunnelItem(col, val);
        } catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnRemoveItemClicked(GameObject obj)
    {
        int count = funnelList.Count;
        while (count > 0)
        {
            count--;
            var item = funnelList.Dequeue();
            if (item.obj == obj)
            {
                continue;
            }

            funnelList.Enqueue(item);
        }

        Destroy(obj);
    }
    
    public void OnAnalyseClicked()
    {
        //List<int> keys = GroupBy.GetComponentInChildren<GetItems>()?.GetItemKeys();
        //fa.GroupByKeys(keys);

        var button = AnalyseButton.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = false;
        }
        try
        {
            List<int> keys = GroupBy.GetComponent<GroupBy>().GetItems();
            fa.GroupByKeys(keys);

            List<int> columns = new();
            List<string> vals = new();
            foreach (var obj in funnelList)
            {
                columns.Add(obj.key);
                vals.Add(obj.value);
            }

            resultRows = fa.AnalyseFunnel(columns, vals);
            Debug.Log($"Result's length is {resultRows.Count}");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            if (button != null)
            {
                button.interactable = true;
            }
        }
    }

    public void OnOutputButtonClicked()
    {
        var button = OutputButton.GetComponent<Button>();
        button.interactable = false;
        try
        {
            ClearTablelList();
            var headers = fa.GetHeaders();
            var selectedList = Fields.GetComponent<GroupBy>().GetItems();

            //List<List<int>> testResult = new();
            //List<int> x = new()
            //{
            //    1,
            //    2,
            //    3,
            //    4,
            //    5
            //};
            //testResult.Add(x);
            //testResult.Add(x);
            List<int> stats = resultRows[resultRows.Count - 1];
            resultRows.RemoveAt(resultRows.Count - 1);
            Debug.Log($"Result's length is {resultRows.Count} in output click");

            InstantiateStats(stats);

            foreach (var resultRow in resultRows)
            {
                var table = Instantiate(Table, OutputContent.transform);
                var gridLayout = table.GetComponent<GridLayoutGroup>();
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = selectedList.Count;
                InstantiateHeaders(table, selectedList);
                InstantiateResult(table, resultRow, selectedList);
                tableItemList.Enqueue(table);
            }
            
        } catch (Exception e)
        {
            Debug.Log(e.Message);
        } finally
        {
            button.interactable = true;
        }

    }

    private void InstantiateStats(List<int> stats)
    {
        var table = Instantiate(Table, OutputContent.transform);
        var gridLayout = table.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;

        string[] headers =
        {
            "Funnel Step",
            "Groups reached that step",
            "Percentage"
        };

        foreach (var str in headers)
        {
            var obj = Instantiate(TableItem, table.transform);
            var txt = obj.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = str;
            txt.fontSize = 18;
            txt.fontStyle = FontStyles.Bold;
            tableItemList.Enqueue(obj);
        }

        for (int i = 1; i < stats.Count; i++)
        {
            string[] state =
            {
                $"{i}",
                $"{stats[i]} / {stats[0]}",
                $"{100.0 * stats[i] / stats[0]}"
            };

            foreach (var str in state)
            {
                var obj = Instantiate(TableItem, table.transform);
                var txt = obj.GetComponentInChildren<TextMeshProUGUI>();
                txt.text = str;
                tableItemList.Enqueue(obj);
            }
        }
    }

    private void InstantiateFunnelItem(string col, string val)
    {
        try
        {
            var obj = Instantiate(FunnelItem, FunnelStepsArea.transform);
            var txt = obj.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = $"{col} | {val}";
            int key = fa.GetHeadersIndexes()[col];
            funnelList.Enqueue(new FunnelItemC(key, val, obj));

        } catch(Exception e)
        {
            Debug.Log(e.Message);
        }        
    }

    private void InstantiateHeaders(GameObject table, List<int> headers)
    {
        var HEADERS = fa.GetHeaders();
        foreach (var key in headers)
        {
            var header = HEADERS[key];
            var item = Instantiate(TableItem, table.transform);
            var txt = item.GetComponentInChildren<TextMeshProUGUI>();
            txt.text = header;
            txt.fontSize = 18;
            txt.fontStyle = FontStyles.Bold;
            tableItemList.Enqueue(item);
        }
    }

    private void InstantiateResult(GameObject table, List<int> resultRow, List<int> headers)
    {
        foreach (int x in resultRow)
        {
            var data = fa.GetData(x);
            foreach (int y in headers)
            {
                var val = data[y];
                var obj = Instantiate(TableItem, table.transform);
                obj.GetComponentInChildren<TextMeshProUGUI>().text = val;
                tableItemList.Enqueue(obj);
            }
        }
    }

    private void Swap<T> (T a, T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }

    public void PopulateGroupByOptions()
    {
        try
        {
            GroupBy.GetComponent<GroupBy>().PopulateOptions(false, fa.GetHeaders());
            Fields.GetComponent<GroupBy>().PopulateOptions(true, fa.GetHeaders());
        } catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void PopulateColumnOptions()
    {
        try
        {
            Column.GetComponent<Column>().PopulateColumnOptions(fa.GetHeaders());
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void ClearFunnelList()
    {
        while (funnelList.Count > 0)
        {
            var obj = funnelList.Dequeue();
            Destroy(obj.obj);
        }
    }

    private void ClearTablelList()
    {
        //resultRows.Clear();
        while (tableItemList.Count > 0)
        {
            var obj = tableItemList.Dequeue();
            Destroy(obj);
        }
    }

    public void OnGroupChanged(string option, GameObject obj)
    {
        Debug.Log($"Group change function called: {option}");
        if (obj.GetComponent<Toggle>().isOn)
        {
            var item = Instantiate(TableItem, GroupArea.transform);
            item.GetComponentInChildren<TextMeshProUGUI>().text = option;
            groupList.Enqueue(new FunnelItemC(0, option, item));
        } else
        {
            int count = groupList.Count;
            while (count > 0)
            {
                var item = groupList.Dequeue();
                count--;
                if (item.obj.GetComponentInChildren<TextMeshProUGUI>().text == option)
                {
                    Destroy(item.obj);
                    continue;
                }

                groupList.Enqueue(item);
            }
        }
    }

    private class FunnelItemC
    {
        public int key;
        public string value;
        public GameObject obj;

        public FunnelItemC(int key, string value, GameObject obj)
        {
            this.key = key;
            this.value = value.Trim();
            
            this.obj = obj;
        }
    }
}
