using Microsoft.AspNetCore.Mvc;
using MvcNetCoreSessionEmpleados.Extensions;
using MvcNetCoreSessionEmpleados.Models;
using MvcNetCoreSessionEmpleados.Repositories;

namespace MvcNetCoreSessionEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        RepositoryEmpleados repo;
        public EmpleadosController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

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
