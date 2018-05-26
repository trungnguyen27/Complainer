using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ComplainerProd.Models
{
    public class MenuItem
    {
        public Symbol Icon { get; set; }
        public string Name { get; set; }
        public Type PageType { get; set; }

        public static List<MenuItem> GetMainItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Find, Name = "Truy Cập", PageType = typeof(Views.SearchPage) });
            items.Add(new MenuItem() { Icon = Symbol.Home, Name = "Trang Chủ", PageType = typeof(Views.HomePage) });
            items.Add(new MenuItem() { Icon = Symbol.Manage, Name = "Quản Lý", PageType = typeof(Views.ChannelPage) });
            return items;
        }

        public static List<MenuItem> GetOptionsItems()
        {
            var items = new List<MenuItem>();
            items.Add(new MenuItem() { Icon = Symbol.Setting, Name = "Thông tin", PageType = typeof(Views.Settings) });
            return items;
        }
    }
}
