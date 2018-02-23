using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zodiaco.Models
{
    public class Casa
    {
        public char CharCasa { get; set; }
        public double Dificuldade { get; set; }

        public int ParametroPelaDificuldade(double dificuldade)
        {
            if (dificuldade <= 70)
                return 37;
            else if (dificuldade > 70 && dificuldade <= 90)
                return 35;
            else
                return 51;
        }
    }
}