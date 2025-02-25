using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcNetCoreSessionEmpleados.Data;
using MvcNetCoreSessionEmpleados.Models;

namespace MvcNetCoreSessionEmpleados.Repositories
{
    public class RepositoryEmpleados
    {
        HospitalContext context;
        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }
        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            var consulta = from datos in this.context.Empleados
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task<Empleado> FindEmpleadoAsync
            (int idEmpleado)
        {
            var consulta = from datos in this.context.Empleados
                           where datos.IdEmpleado == idEmpleado
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }
        //TERCERA VERSION
        public async Task<List<Empleado>> GetEmpleadosSessionAsync
            (List<int> ids)
        {
            var consulta = from datos in this.context.Empleados
                           //   ES UNA CONSULTA CON UN IN
                           where ids.Contains(datos.IdEmpleado)
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                return await consulta.ToListAsync();
            }
        }
        //CUARTA VERSION
        public async Task<List<Empleado>> GetEmpleadosNotSessionAsync
            (List<int> ids)
        {
            var consulta = from datos in this.context.Empleados
                           where !ids.Contains(datos.IdEmpleado)
                           select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                return await consulta.ToListAsync();
            }
        }
    }
}
