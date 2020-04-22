using System;
using System.Collections.Generic;
using System.Text;

namespace JMangaReader.Models
{
    public enum MenuItemType
    {
        Browse,
        MangaSelectorView,
        About,
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }
    }
}
