﻿using AgendaWeb.Infra.Data.Interfaces;
using AgendaWeb.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AgendaWeb.Presentation.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //atributo
        private readonly IEventoRepository _eventoRepository;

        //contrutor do atributo
        public HomeController(IEventoRepository eventoRepository)
        {
            _eventoRepository = eventoRepository;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel();

            try
            {
                var dataAtual = DateTime.Now;

                //definindo as datas...
                model.DataInicio = new DateTime(dataAtual.Year, dataAtual.Month, 1);
                model.DataFim = new DateTime(dataAtual.Year, dataAtual.Month, 
                    DateTime.DaysInMonth(dataAtual.Year, dataAtual.Month));

                //obtendo o usuario autenticado na sessão
                var json = HttpContext.Session.GetString("usuario");
                var usuario = JsonConvert.DeserializeObject<UserIdentityModel>(json);

                //consultar os eventos ativo e inativos deste periodo
                var eventosAtivos = _eventoRepository.GetByDatas
                    (model.DataInicio, model.DataFim, 1, usuario.Id);
                var eventosInativos = _eventoRepository.GetByDatas
                    (model.DataInicio, model.DataFim, 0, usuario.Id);

                //calculando as informações da model
                model.TotalPrioridadeAlta = eventosAtivos
                    .Count(e => e.Prioridade == 3);

                model.TotalPrioridadeMedia = eventosAtivos
                    .Count(e => e.Prioridade == 2);

                model.TotalPrioridadeBaixa = eventosAtivos
                    .Count(e => e.Prioridade == 1);

                model.TotalAtivos = eventosAtivos.Count();
                model.TotalInativos = eventosInativos.Count();
            }
            catch(Exception e)
            {
                TempData["MensagemErro"] = e.Message;
            }

            return View(model);
        }
    }
}
