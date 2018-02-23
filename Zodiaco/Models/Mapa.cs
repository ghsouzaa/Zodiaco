using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Zodiaco.Models
{
    public class Mapa
    {
        public char CharMapa { get; set; }
        public double Esforco { get; set; }
        public string Sprite { get; set; }
        public bool PosicaoLuta { get; set; }
        public Dictionary<string, int> Posicao { get; set; }
        public List<Mapa> Filhos { get; set; }

        public Mapa() { }

        public Mapa(char c, int linha, int coluna, List<Casa> casas)
        {
            Posicao = new Dictionary<string, int>();
            Filhos = new List<Mapa>();

            Posicao.Add("Linha", linha);
            Posicao.Add("Coluna", coluna);

            CharMapa = c;

            switch (c)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'J':
                case 'Q':
                case 'K':
                    Sprite = "C.jpg";
                    PosicaoLuta = true;
                    CarregarEsforcoCasa(casas);
                    break;
                default:
                    Sprite = c + ".jpg";
                    PosicaoLuta = false;
                    CarregarEsforco();
                    break;
            }
        }

        private void CarregarFilhos(Mapa[,] matriz)
        {
            int linha = Posicao["Linha"];
            int coluna = Posicao["Coluna"];

            Filhos.Add(matriz[linha, coluna - 1]); //Add filho esquerda
            Filhos.Add(matriz[linha - 1, coluna]); //Add filho cima
            Filhos.Add(matriz[linha, coluna + 1]); //Add filho direita
            Filhos.Add(matriz[linha + 1, coluna]); //Add filho baixo
        }

        private void CarregarEsforco()
        {
            var config = AppDomain.CurrentDomain.BaseDirectory + "Content\\CONFIG.txt";
            using (StreamReader reader = new StreamReader(config))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] op = line.Split('-');
                    if (op[0] == CharMapa.ToString())
                    {
                        Esforco = short.Parse(op[1]);
                        break;
                    }
                }
            };
        }

        private void CarregarEsforcoCasa(List<Casa> casas)
        {
            Esforco = 1;
            if (casas.Count != 0)
                Esforco = casas.FirstOrDefault(x => x.CharCasa == CharMapa).Dificuldade;
        }

        public Mapa[,] CarregarMapa(List<Casa> casas)
        {
            var mapa = AppDomain.CurrentDomain.BaseDirectory + "Content\\MAPA.txt";
            Mapa[,] Matriz = new Mapa[42, 42];
            using (StreamReader reader = new StreamReader(mapa))
            {
                int contadorLinha = 0;
                int contadorColuna = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    char[] caracteres = line.ToCharArray();
                    foreach (var ch in caracteres)
                    {
                        Matriz[contadorLinha, contadorColuna] = new Mapa(ch, contadorLinha, contadorColuna, casas);
                        contadorColuna++;
                    }
                    contadorColuna = 0;
                    contadorLinha++;
                }
            };

            foreach (var item in Matriz)
                if (item.CharMapa != 'M')
                    item.CarregarFilhos(Matriz);

            return Matriz;
        }

        public Mapa BuscarNoMapa(char c, Mapa[,] map)
        {
            Mapa m = new Mapa();
            foreach (var item in map)
            {
                if (item.CharMapa == c)
                {
                    m = item;
                    break;
                }
            }
            return m;
        }
    }
}