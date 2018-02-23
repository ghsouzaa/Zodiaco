using System;
using System.Collections.Generic;
using System.Linq;

namespace Zodiaco.Models
{
    public class Agente
    {
        List<Time> usadosBatalha = new List<Time>();
        public Time time;
        // Objetivo é o número da casa a qual o char tem como alvo atual
        public char objetivoAtual = '1';
        // O objetivoMapa guarda o mapa e retorna a posição da casa solicitada
        public Mapa objetivoMapa;

        public char[] objetivos = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'J', 'Q', 'K', 'E' };

        public List<Mapa> caminhoExcluido = new List<Mapa>();

        public List<Mapa> BuscarCaminho(Mapa[,] map, Time t)
        {
            time = t;
            List<Mapa> caminhoEscolhido = new List<Mapa>();
            foreach (var item in map)
            {
                if (item.CharMapa == 'S')
                {
                    caminhoEscolhido = Caminhar(item, map, caminhoEscolhido);
                    break;
                }
            }

            return caminhoEscolhido;
        }

        public List<Mapa> Caminhar(Mapa atual, Mapa[,] map, List<Mapa> caminho)
        {
            try
            {
                // verifica se a posição eh uma casa de luta
                if (atual.PosicaoLuta)
                    atual.Esforco = Lutar(atual); //roda o Lutar p/ calcular o resultado da batalha

                //verifica se um dos "filhos" (casas proximas, next step) é o seu objetivo
                var objetivo = atual.Filhos.FirstOrDefault(x => objetivos.Contains(x.CharMapa));
                if (objetivo != null)
                {
                    caminho.Add(atual);
                    if (objetivoAtual != 'E') //'E' é o ultimo objetivo, entra aki se não for o objetivo final
                    {
                        var indexObjetivo = Array.IndexOf(objetivos, objetivo.CharMapa);
                        objetivoAtual = objetivos[indexObjetivo + 1];
                    }

                    if (objetivo.CharMapa == 'E') // adiciona o ultimo passo, que é o objetivo final (casa do Gran Mestre)
                    {
                        caminho.Add(objetivo);
                        return caminho;
                    }

                    objetivo.Filhos.Remove(atual); 

                    return Caminhar(objetivo, map, caminho);
                }
                objetivoMapa = new Mapa().BuscarNoMapa(objetivoAtual, map);

                // busca a proxima casa (objetivo), a posição da casa 
                //objetivoMapa = new Mapa().BuscarNoMapa(objetivoAtual, map);

                //retorna os caminhos
                var filhosSemMontanhas = atual.Filhos.Where(x => x.CharMapa != 'M').ToList();
                // filtra os caminhos que já foram percorridos
                var filhosSemRepetir = filhosSemMontanhas.Except(caminho).ToList();

                if (filhosSemRepetir.Count > 0)
                    filhosSemRepetir = filhosSemRepetir.Except(caminhoExcluido).ToList(); //remove os caminhos que deram erro

                if (filhosSemRepetir.Count == 0)// quando não tem mais para onde ir (da rollback)
                {
                    caminhoExcluido.Add(atual);
                    caminho.Remove(atual); // faz rollback e volta recursivamente no caminho anterior
                    return Caminhar(caminho.Last(), map, caminho);
                }
                else
                { //tem caminhos para percorrer
                    caminho.Add(atual); //da o passo no caminho selecionado e ve o que que da!

                    var menorEsforco = filhosSemRepetir.OrderBy(x => x.Esforco).First().Esforco; // calcula o menor esforço dos possíveis proximos passos
                    var todosMenor = filhosSemRepetir.Where(x => x.Esforco == menorEsforco).ToList(); // seleciona/filtra os proximos passos com essa mesma qntd de esforço

                    //se tiver mais de 1 'proximo passo' com o menor esforço
                    if (todosMenor.Count > 1)
                    {
                        var filhoEscolhido = EscolherMelhorFilho(todosMenor); // escolhe o filho mais próximo do objetivo (casa)
                        return Caminhar(filhoEscolhido, map, caminho);
                    }
                    else
                    {
                        var filhoEscolhido = todosMenor.OrderBy(x => x.Esforco).Last(); //escolhe o primeiro mais fácil (caminho)
                        filhoEscolhido.Filhos.Remove(atual);
                        return Caminhar(filhoEscolhido, map, caminho);
                    }
                }                
            }
            catch (Exception e)
            {
                return caminho;
            }
        }

        public double Lutar(Mapa casa)
        {
            var poderCasa = casa.Esforco;

            var poderEquipe = SelecionarEquipe(poderCasa);

            return poderCasa / poderEquipe;
        }
        /// <summary>
        /// Seleciona a equipe ideal de acordo com o Poder da Casa (adversário)
        /// </summary>
        /// <param name="poderCasa">Valor (ponto flutuante) do poder da casa</param>
        /// <returns></returns>
        public double SelecionarEquipe(double poderCasa)
        {
            Cavaleiro primeiro, segundo, terceiro;
            List<Cavaleiro> c1 = new List<Cavaleiro>();

            var parametro = new Casa().ParametroPelaDificuldade(poderCasa);
            var dificuldade = 0.0;

            var cavaleirosComVida = time.cavaleiros.Where(x => x.Vida > 1).OrderBy(x => x.PoderCosmico);
            foreach (var cav in cavaleirosComVida)
            {
                var menorParametro = poderCasa / cav.PoderCosmico;
                if (menorParametro <= parametro)
                {
                    cav.Vida--;
                    c1.Add(cav);
                    usadosBatalha.Add(new Time(c1));
                    return cav.PoderCosmico;
                }
            }

            primeiro = time.cavaleiros.Where(x => x.Vida > 1).OrderBy(x => x.PoderCosmico).ThenBy(x => x.Vida).First();
            primeiro.Vida--;

            dificuldade = (poderCasa / primeiro.PoderCosmico);
            if (dificuldade < parametro)
            {
                c1.Add(primeiro);
                usadosBatalha.Add(new Time(c1));
                return primeiro.PoderCosmico;
            }


            segundo = time.cavaleiros.Where(x => (x.Vida > 1) && (x.Nome != primeiro.Nome) ).OrderBy(x => x.PoderCosmico).ThenBy(x => x.Vida).FirstOrDefault();
            if (segundo == null)
            {
                c1.Add(primeiro);
                usadosBatalha.Add(new Time(c1));
                return primeiro.PoderCosmico;
            }                

            segundo.Vida--;

            dificuldade = (poderCasa / (primeiro.PoderCosmico + segundo.PoderCosmico));
            if (dificuldade < parametro)
            {
                c1.Add(primeiro);
                c1.Add(segundo);
                usadosBatalha.Add(new Time(c1));
                return primeiro.PoderCosmico + segundo.PoderCosmico;
            }


            terceiro = time.cavaleiros.Where(x => (x.Vida > 1) && ((x.Nome != primeiro.Nome) && (x.Nome != segundo.Nome))).OrderBy(x => x.PoderCosmico).ThenBy(x => x.Vida).FirstOrDefault();

            if (segundo == null)
            {
                c1.Add(primeiro);
                c1.Add(segundo);
                usadosBatalha.Add(new Time(c1));
                return primeiro.PoderCosmico + segundo.PoderCosmico;
            }

            terceiro.Vida--;

            dificuldade = (poderCasa / (primeiro.PoderCosmico + segundo.PoderCosmico + terceiro.PoderCosmico));
            if (dificuldade < parametro)
            {
                c1.Add(primeiro);
                c1.Add(segundo);
                c1.Add(terceiro);
                usadosBatalha.Add(new Time(c1));
                return primeiro.PoderCosmico + segundo.PoderCosmico + terceiro.PoderCosmico;
            }

            throw new Exception();
        }

        public Mapa EscolherMelhorFilho(List<Mapa> filhos)
        {
            try
            {
                Dictionary<Mapa, double> Posicao = new Dictionary<Mapa, double>();
                foreach (var filho in filhos)
                {
                    var esforco = CalcularDistanciaFilho(filho, objetivoMapa); //calcula matematicamente a coordenada na matrix que está mais próxima do objetivo
                    Posicao.Add(filho, esforco);
                }
                var melhor = Posicao.OrderBy(x => x.Value).First().Key;
                return melhor;
            }
            catch (Exception)
            {
                return filhos.First();
            }
        }

        public double CalcularDistanciaFilho(Mapa filho, Mapa objetivo)
        {
            float[] i = { objetivo.Posicao["Linha"], objetivo.Posicao["Coluna"] };
            float[] n = { filho.Posicao["Linha"], filho.Posicao["Coluna"] };

            var distancia = Math.Sqrt(i.Zip(n, (x, y) => (x - y) * (x - y)).Sum());

            return distancia;
        }
        
    }
}