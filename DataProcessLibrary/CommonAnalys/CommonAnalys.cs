using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataProcessLibrary.Analys;
using System.Collections.ObjectModel;
using System.IO;
using ProjectManagerLibrary.WorkWithProjects;


namespace DataProcessLibrary.CommonAnalys
{
    public class CommonAnalys
    {
        public HierarhyElem_Analys HierarhiParent { get; set; }

        public CommonAnalys(HierarhyElem_Analys hierarhyElem)
        {
            HierarhiParent = hierarhyElem;
        }

        public CommonDecidionGraphic GetCommonDecidionGraphic()
        {
            return HierarhiParent.Analys() as CommonDecidionGraphic;
        }
    }
}
