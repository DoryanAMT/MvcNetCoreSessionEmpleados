using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using MvcNetCoreSessionEmpleados.Extensions;
using MvcNetCoreSessionEmpleados.Models;
using MvcNetCoreSessionEmpleados.Repositories;
using System.CodeDom;

namespace MvcNetCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private RepositoryEmpleados repo;
        private IMemoryCache memoryCache;
        public EmpleadosController(RepositoryEmpleados repo,
            IMemoryCache memoryCache)
        {
            this.repo = repo;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> EmpleadosFavoritos
            (int? idEliminar)
        {
            if (idEliminar != null)
            {
                List<Empleado> favoritos = this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                //  BUSCAMOS AL EMPLEADO A ELIMINAR DENTRO DE LA
                //  COLECCION DE FAVORITOS
                Empleado empDelete =
                    favoritos.Find(z => z.IdEmpleado == idEliminar.Value);
                favoritos.Remove(empDelete);
                if (favoritos.Count == 0)
                {
                    this.memoryCache.Remove("FAVORITOS");
                }
                else
                {
                    this.memoryCache.Set("FAVORITOS", favoritos);
                }
            }
            return View();
        }
        //  SEXTA VERSION
        public async Task<IActionResult> SessionEmpleadosV6
            (int? idEmpleado, int? idFavorito)
        {
            if (idFavorito != null)
            {
                //  COMO ESTOY ALMACENANDO EN CACHE DE CLIENTE, VAMOS A 
                //  UTILIZAR LOS OBJETOS EN LUGAR DE LOS IDS
                //  GUARDAREMOS DENTRO DE IMEMORY CACHE TODOS LOS EMPLEADOS
                List<Empleado> empleadosFavoritos;
                if (this.memoryCache.Get("FAVORITOS") == null)
                {
                    //  NO EXISTEN, CREAMOS LA COLECCION DE FAVORITOS
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    //  RECUPERAMOS LOS EMPLEADOS QUE TENEMOS EN LA
                    //  COLECCION DE FAVORITOS DE CACHE
                    empleadosFavoritos =
                        this.memoryCache.Get<List<Empleado>>("FAVORITOS");
                }
                //  BUSCAMOS EL OBJETO EMPLEADO A ALMACENAR
                Empleado empleado = await this.repo.FindEmpleadoAsync(idFavorito.Value);
                empleadosFavoritos.Add(empleado);
                this.memoryCache.Set("FAVORITOS", empleadosFavoritos);
            }

            if (idEmpleado != null)
            {
                //  ALMACENAREMOS LO MINIMO QUE PODAMOS
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    //  NO EXISTE Y CREAMOS LA COLECCION
                    idsEmpleados = new List<int>();
                }
                else
                {
                    //  EXISTE Y RECUERAMOS LA COLECCION
                    idsEmpleados =
                        HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                idsEmpleados.Add(idEmpleado.Value);
                //  REFRESCAMOS LOS DATOS DE SESSION
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count();
            }
            List<Empleado> empleados =
                await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        
        public async Task<IActionResult> EmpleadosAlmacenadosV6
            (int? idEmpleado)
        {
            List<int> idsEmpleados;
            //  DEBEMOS RECUPERAR LOS IDS DE EMPLEADOS QUE TENGAMOS
            //  EN SESSION
            idsEmpleados =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados.IsNullOrEmpty())
            {
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session";
                return View();
            }
            else
            {
                if (idEmpleado != null)
                {
                    idsEmpleados.Remove(idEmpleado.Value);
                    HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                }
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session";
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
            
        }
        //  QUINTA VERSION
        public async Task<IActionResult> SessionEmpleadosV5
            (int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                //  ALMACENAREMOS LO MINIMO QUE PODAMOS
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    //  NO EXISTE Y CREAMOS LA COLECCION
                    idsEmpleados = new List<int>();
                }
                else
                {
                    //  EXISTE Y RECUERAMOS LA COLECCION
                    idsEmpleados =
                        HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                idsEmpleados.Add(idEmpleado.Value);
                //  REFRESCAMOS LOS DATOS DE SESSION
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count();
            }
            List<Empleado> empleados =
                await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosV5()
        {
            //  DEBEMOS RECUPERAR LOS IDS DE EMPLEADOS QUE TENGAMOS
            //  EN SESSION
            List<int> idsEmpleados =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        //  CUARTA VERSION
        public async Task<IActionResult> SessionEmpleadosV4
            (int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                //  ALMACENAREMOS LO MINIMO QUE PODAMOS
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    //  NO EXISTE Y CREAMOS LA COLECCION
                    idsEmpleados = new List<int>();
                }
                else
                {
                    //  EXISTE Y RECUERAMOS LA COLECCION
                    idsEmpleados =
                        HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                idsEmpleados.Add(idEmpleado.Value);
                //  REFRESCAMOS LOS DATOS DE SESSION
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count();
            }
            //COMPROBAMOS SI TENEMOS IDS EN SESSION
            List<int> ids =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (ids == null)
            {
                List<Empleado> empleados =
                    await this.repo.GetEmpleadosAsync();
                return View(empleados);
            }
            else
            {
                List<Empleado> empleados =
                    await this.repo.GetEmpleadosNotSessionAsync(ids);
                return View(empleados);
            }

        }
        public async Task<IActionResult> EmpleadosAlmacenadosV4()
        {
            //  DEBEMOS RECUPERAR LOS IDS DE EMPLEADOS QUE TENGAMOS
            //  EN SESSION
            List<int> idsEmpleados =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }
        //TERCERA VERSION
        public async Task<IActionResult> SessionEmpleadosOK
            (int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                //  ALMACENAREMOS LO MINIMO QUE PODAMOS
                List<int> idsEmpleados;
                if (HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS") == null)
                {
                    //  NO EXISTE Y CREAMOS LA COLECCION
                    idsEmpleados = new List<int>();
                }
                else
                {
                    //  EXISTE Y RECUERAMOS LA COLECCION
                    idsEmpleados =
                        HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
                }
                idsEmpleados.Add(idEmpleado.Value);
                //  REFRESCAMOS LOS DATOS DE SESSION
                HttpContext.Session.SetObject("IDSEMPLEADOS", idsEmpleados);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleados.Count();
            }
            List<Empleado> empleados =
                await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> EmpleadosAlmacenadosOK()
        {
            //  DEBEMOS RECUPERAR LOS IDS DE EMPLEADOS QUE TENGAMOS
            //  EN SESSION
            List<int> idsEmpleados =
                HttpContext.Session.GetObject<List<int>>("IDSEMPLEADOS");
            if (idsEmpleados == null)
            {
                ViewData["MENSAJE"] = "No existen empleados almacenados en Session";
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosSessionAsync(idsEmpleados);
                return View(empleados);
            }
        }

        //SEGUNDA VERSION
        public async Task<IActionResult> SessionEmpleados
            (int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                //  BUSCAMOS AL EMPLEADO
                Empleado empleado = await this.repo.FindEmpleadoAsync(idEmpleado.Value);
                //  PARA PODER ALMACENARLO
                List<Empleado> empleadosList;
                //  DEBEMOS PREGUNTAR SI TENEMOS ALMACENADOS UN CONJUNTO DE EMPLEDOS
                //  ALMACENAMOS EN SESION
                if (HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    //  RECUPERAMOS LOS EMPLEADOS QUE TENGAMOS EN SESSION
                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    //  SI NO EXISTE INSTANCIAMOS LA COLECCION
                    empleadosList = new List<Empleado>();
                }
                //  ALMACENAMOS EL EMPLEADO DENTRO DE NUESTRA COLECCION
                empleadosList.Add(empleado);
                //  GUARDAMOS EN SESSION LA COLECCION CON EL NUEVO EMPLEADO
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                //  ENVIAMOS UN MENSAJE PARA MOSTRAR EL OBJETO LIST<EMPLEADO> DE LA SESSION
                ViewData["MENSAJE"] = "Empleado" + empleado.Apellido +
                    " almacenado correctamente.";
            }
            List<Empleado> empleados =
                await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }
        //PRIMERA VERSION
        public async Task<IActionResult> SessionSalarios
            (int? salario)
        {
            if (salario != null)
            {
                //  NECESITAMOS ALMACENAR EL SALARIO DEL EMPLEADO
                //  Y LA SUMA TOTAL DE SALARIOS QUE TENGAMOS
                int sumaSalarial = 0;
                //  PREGUNTAREMOS SI YA TENEMOS LA SUMA ALMACENADA EN SESSION
                if (HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    //  SI YA EXISTE LA SUMA SALARIAL, RECUPERAMOS
                    //  SU VALOR
                    sumaSalarial = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                //  REALIZAMOS LA SUMA
                sumaSalarial += salario.Value;
                //  ALMACENAMOS EL NUEVO VALOR DE LA SUMA SALARIAL
                //  DENTRO DE SESSION
                HttpContext.Session.SetObject("SUMASALARIAL", sumaSalarial);
                ViewData["MENSAJE"] = "Salario almacenado: " + salario.Value;
            }
            List<Empleado> empleados =
                await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult SumaSalarial()
        {
            return View();
        }
    }
}
