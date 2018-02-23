using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using Zodiaco.Models;
using Zodiaco.Models.ViewModel;

namespace Zodiaco.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IndexViewModel index = new IndexViewModel();
            index.Mapa = new Mapa().CarregarMapa(new List<Casa>());
            return View(index);
        }

        public JsonResult CalcularJson(string c1, string c2, string c3, string c4, string c5, string c6, string c7, string c8, string c9, string c10, string c11, string c12, string Seya, string Shiryu, string Hyoga, string Shun, string Ikki)
        {
            List<Casa> casas = new List<Casa>();
            casas.Add(new Casa() { CharCasa = '1', Dificuldade = double.Parse(c1, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '2', Dificuldade = double.Parse(c2, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '3', Dificuldade = double.Parse(c3, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '4', Dificuldade = double.Parse(c4, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '5', Dificuldade = double.Parse(c5, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '6', Dificuldade = double.Parse(c6, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '7', Dificuldade = double.Parse(c7, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '8', Dificuldade = double.Parse(c8, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = '9', Dificuldade = double.Parse(c9, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = 'J', Dificuldade = double.Parse(c10, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = 'Q', Dificuldade = double.Parse(c11, CultureInfo.InvariantCulture) });
            casas.Add(new Casa() { CharCasa = 'K', Dificuldade = double.Parse(c12, CultureInfo.InvariantCulture) });

            Time time = new Time();
            time.cavaleiros.Add(new Cavaleiro("Seya", double.Parse(Seya, CultureInfo.InvariantCulture)));
            time.cavaleiros.Add(new Cavaleiro("Shiryu", double.Parse(Shiryu, CultureInfo.InvariantCulture)));
            time.cavaleiros.Add(new Cavaleiro("Hyoga", double.Parse(Hyoga, CultureInfo.InvariantCulture)));
            time.cavaleiros.Add(new Cavaleiro("Shun", double.Parse(Shun, CultureInfo.InvariantCulture)));
            time.cavaleiros.Add(new Cavaleiro("Ikki", double.Parse(Ikki, CultureInfo.InvariantCulture)));


            var mapa = new Mapa().CarregarMapa(casas);
            List<Mapa> caminho = new Agente().BuscarCaminho(mapa, time);

            List<Caminho> lista = new List<Caminho>();
            foreach(var item in caminho)
            {
                lista.Add(new Caminho() { idMatriz = item.Posicao["Linha"].ToString() + "-" + item.Posicao["Coluna"].ToString(), esforco = item.Esforco });
            }

            return Json(lista, JsonRequestBehavior.AllowGet);
        }
    }
}