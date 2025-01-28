using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    public static class Config
    {
        public static readonly (string Header, int Width)[] ColumnsConfig =
        {
        ("Process", 300),
        ("RAM", 150),
        ("StartTime", 230),
        ("Priority", 150),
        ("ThreadsNumber", 170)
    };

        public static readonly Dictionary<string, string> ImagePaths = new()
    {
        { "expand", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "expand.png") },
        { "expand_hover", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "expand_hover.png") },
        { "collapse", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "collapse.png") },
        { "collapse_hover", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "collapse_hover.png") }
    };

        public static readonly (string Text, ProcessPriorityClass Priority)[] PriorityItems =
        {
        ("Реального часу", ProcessPriorityClass.RealTime),
        ("Високий", ProcessPriorityClass.High),
        ("Вище середнього", ProcessPriorityClass.AboveNormal),
        ("Середній", ProcessPriorityClass.Normal),
        ("Нижче середнього", ProcessPriorityClass.BelowNormal),
        ("Низький", ProcessPriorityClass.Idle)
    };
    }
}
