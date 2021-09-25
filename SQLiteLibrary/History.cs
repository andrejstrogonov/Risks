using SQLiteLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SQLiteLibrary
{
    [Serializable]
    public enum TypeOfHistoryItem
    {
        Product,
        Resource,
        ResourceOfProduct,
        RiskOfProduct,
        Group,
        FixedCost,
        Settings
    }

    [Serializable]
    public class HistoryItem
    {
        public DateTime DateTime { get; set; }
        public TypeOfHistoryItem TypeOfItem { get; set; }
        public string Title { get; set; }
        public string TitleChange { get; set; }

        public long ID { get; set; }
        public string Property { get; set; }

        public object LastValue { get; set; }
        public object NewValue { get; set; }

        public List<HistoryItem> GroupItems { get; set; }
    }

    [Serializable]
    public class History
    {
        public DateTime LastTimeAccess { get; set; }

        private long Capacity;

        public History()
        {
            Capacity = 100;
            history = new List<HistoryItem>();
        }

        public History(List<HistoryItem> items)
        {
            Capacity = 100;
            history = items;
          
        }

        public List<HistoryItem> HistoryList
        {
            get
            {
                return history;
            }
        }



        public List<HistoryItem> GetHistory(TypeOfHistoryItem typeOfHistoryItem)
        {
            LastTimeAccess = DateTime.Now;

            if (typeOfHistoryItem== TypeOfHistoryItem.Resource || typeOfHistoryItem== TypeOfHistoryItem.FixedCost)
                return history.Where(p => p.TypeOfItem == typeOfHistoryItem).ToList();
            return history.Where(p => p.TypeOfItem != TypeOfHistoryItem.Resource && p.TypeOfItem != TypeOfHistoryItem.FixedCost).ToList();
        }


        private List<HistoryItem> history { get;  set; }

        public void Add(HistoryItem historyItem)
        {
            LastTimeAccess = DateTime.Now;
            history.Add(historyItem);
            while (history.Count() > Capacity)
            {
                history.RemoveAt(0);
            }
        }

        public HistoryItem GetNew()
        {
            LastTimeAccess = DateTime.Now;
            return history[history.Count() - 1];
        }

        public HistoryItem CanselNew()
        {
            LastTimeAccess = DateTime.Now;
            var hi = history[history.Count() - 1];
            history.RemoveAt(history.Count() - 1);
            return hi;
        }
    }

    public class HistorySetuper
    {
        private  string _path;
        public  string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                History = null;
                History = Get();
            }
        }

        public HistorySetuper(string path)
        {
            Path = path;
        }

        public  History History { get; set; }

        private  History CreateNew()
        {
            if (Path is null)
                return null;
            var set = new History();
            var writer = new XmlSerializer(typeof(History));
            var wfile = new StreamWriter(Path);
            writer.Serialize(wfile, set);
            wfile.Close();
            return set;
        }

        public  History Get()
        {
            if (History != null)
            {
                return History;
            }
            if (!File.Exists(Path))
            {
                History = CreateNew();
                History.LastTimeAccess = DateTime.Now;
                return History;
            }
            var Serializer = new XmlSerializer(typeof(History));
            var file = new StreamReader(Path);
            try
            {
                History history = (History)Serializer.Deserialize(file);
                file.Close();
                History = history;
                if(History is null)
                {
                    History= CreateNew();
                    History.LastTimeAccess = DateTime.Now;
                }
                else
                {
                    //UniteSomeElements();
                }
                return History;
            }
            catch (System.InvalidOperationException)
            {
                file.Close();
                History = CreateNew();
                History.LastTimeAccess = DateTime.Now;
                return History;
            }
        }



        public static bool needSave=false;

        /*private  string GetTitle(TypeOfHistoryItem typeOfHistory, long id)
        {
            switch (typeOfHistory)
            {
                case TypeOfHistoryItem.FixedCost:
                    {
                        var fc = FixedCostDAL.GetFixedCost(id);
                        if (!(fc is null))
                            return fc.Name;
                        return null;
                    }
                case TypeOfHistoryItem.Group:
                    {
                        var g = GroupDAL.GetGroup(id);
                        if (g is null)
                            return null;
                        return "Работа с группой "+'\n'+ g.Name;
                    }
                case TypeOfHistoryItem.Product:
                    {
                        var p = ProductDAL.GetProduct(id);
                        if (p is null)
                            return null;
                        return "Работа с товаром " + '\n' + p.Name;
                    }
                case TypeOfHistoryItem.Resource:
                    {
                        var p = ResourceDAL.GetResource(id);
                        if (p is null)
                            return null;
                        return "Работа с ресурсом " + '\n' + p.Name;
                    }
                case TypeOfHistoryItem.ResourceOfProduct:
                    {
                        var p = ResourceOfProductDAL.GetResourceOfProduct(id);
                        if (p is null)
                            return null;
                        var pr = ProductDAL.GetProduct(p.ID_Product);
                        var res = ResourceDAL.GetResource(p.ID_Resource);
                        if(!(pr is null || res is null))
                        {
                            return res.Name + '\n' + "в составе товара" + '\n' + pr.Name;
                        }
                        return null;
                    }
                default:
                    {
                        var p = RiskOfProductDAL.GetRiskOfProduct(id);
                        if (p is null)
                            return null;
                        var pr = ProductDAL.GetProduct(p.ID_Product);
                        if (pr is null)
                            return null;
                        return "Оценка товара" +pr.Name+ '\n' + "при цене" +'\n'+ p.Price;
                    }
            }
        }

        private  void UniteSomeElements()
        {
            if (History is null)
                return;
            List<HistoryItem> newitems = new List<HistoryItem>();
            Dictionary<KeyValuePair<TypeOfHistoryItem, long>, List<HistoryItem>> groups = new Dictionary<KeyValuePair<TypeOfHistoryItem, long>, List<HistoryItem>>();
            foreach(var h in History.HistoryList)
            {
                if (!(GetTitle(h.TypeOfItem, h.ID) is null))
                {
                    if ((h.DateTime.Date < DateTime.Now.Date) && (h.GroupItems is null))
                    {
                        var pair = new KeyValuePair<TypeOfHistoryItem, long>(h.TypeOfItem, h.ID);
                        if (!groups.ContainsKey(pair))
                            groups[pair] = new List<HistoryItem>();
                        groups[pair].Add(h);

                    }
                    else
                    {
                        newitems.Add(h);
                    }
                }
            }
            foreach(var group in groups)
            {
                var title = GetTitle(group.Key.Key, group.Key.Value);
                if (title is null)
                    continue;
                List<HistoryItem> yesterday = new List<HistoryItem>();
                List<HistoryItem> earlier = new List<HistoryItem>();
                foreach(var g in group.Value)
                {
                    if (g.DateTime.AddDays(1).Date == DateTime.Now.Date)
                    {
                        yesterday.Add(g);
                    }
                    else
                    {
                        earlier.Add(g);
                    }
                }
                if (yesterday.Count() > 0)
                {
                    HistoryItem historyGroupItemYes = new HistoryItem();
                    historyGroupItemYes.DateTime = yesterday[0].DateTime;
                    historyGroupItemYes.TypeOfItem = group.Key.Key;
                    historyGroupItemYes.ID = group.Key.Value;
                    historyGroupItemYes.GroupItems = yesterday;
                    historyGroupItemYes.Title = title;
                    historyGroupItemYes.TitleChange = "Ряд изменений";
                    newitems.Add(historyGroupItemYes);
                }
                if (earlier.Count() > 0)
                {
                    HistoryItem historyGroupItemEr = new HistoryItem();
                    historyGroupItemEr.DateTime = earlier[0].DateTime;
                    historyGroupItemEr.TypeOfItem = group.Key.Key;
                    historyGroupItemEr.ID = group.Key.Value;
                    historyGroupItemEr.GroupItems = earlier;
                    historyGroupItemEr.Title = title;
                    historyGroupItemEr.TitleChange = "Ряд изменений";
                    newitems.Add(historyGroupItemEr);
                }
            }
            if (newitems.Count > 0)
            {
                DateTime last = History.LastTimeAccess;
                History = new History(newitems);
                History.LastTimeAccess = last;
            }
        }
        */
        public  void SaveReal()
        {
            if (History is null)
                return;
            //UniteSomeElements();
            var writer = new XmlSerializer(typeof(History));
            var wfile = new StreamWriter(_path);
            writer.Serialize(wfile, History);
            wfile.Close();
        }
    }
}
