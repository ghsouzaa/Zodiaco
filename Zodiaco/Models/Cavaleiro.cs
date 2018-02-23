using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zodiaco.Models
{
    public class Cavaleiro
    {
        public string Nome { get; set; }
        public double PoderCosmico { get; set; }
        public int Vida { get; set; }

        public Cavaleiro(string nome, double poder)
        {
            Nome = nome;
            PoderCosmico = poder;
            Vida = 5;
        }
    }
}