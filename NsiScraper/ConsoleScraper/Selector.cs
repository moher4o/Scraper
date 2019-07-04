using System.Collections.Generic;

namespace ConsoleScraper
{
    public class Selector
    {
        public Selector(string _Id, string _Type, List<Selector> _ParentSelectors, string _Element, string _Regex, string _Multiple, string _Delay)
        {
            this.Id = _Id;
            this.Type = _Type;
            this.ParentSelectors = _ParentSelectors;
            this.Element = _Element;
            this.Multiple = _Multiple == "true" ? true : false;
            this.Delay = _Delay;
            this.Regex = _Regex;
        }

        public string Id { get; set; }

        public string Type { get; set; }

        public List<Selector> ParentSelectors { get; set; }

        public string Element { get; set; }

        public string Regex { get; set; }

        public bool Multiple { get; set; }

        public string Delay { get; set; }
    }
}
