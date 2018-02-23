using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zodiaco.Models
{
    public class Time
    {
        public List<Cavaleiro> cavaleiros { get; set; }

        public Time()
        {
            cavaleiros = new List<Cavaleiro>();
        }

        public Time(List<Cavaleiro> c1)
        {
            AddCavaleiro(c1);
        }

        public void AddCavaleiro(List<Cavaleiro> lsita)
        {
            cavaleiros = new List<Cavaleiro>();
            foreach(var item in lsita)
            {
                cavaleiros.Add(item);
            }
        }
    }
}