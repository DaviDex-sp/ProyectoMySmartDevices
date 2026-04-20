using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProyectoMSD.Interfaces;
using ProyectoMSD.Modelos;
using ProyectoMSD.Modelos.DTOs;
using System;

namespace ProyectoMSD.Services
{
    public class PropiedadService : IPropiedadService
    {
        private readonly AppDbContext _context;

        public PropiedadService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PropiedadDto>> GetPropiedadesAsync(bool isAdmin, int? userId)
        {
            IQueryable<Propiedade> propQuery = _context.Propiedades
                .Include(p => p.UsuariosPropiedades)
                    .ThenInclude(up => up.IdUsuarioNavigation)
                .Include(p => p.Espacios)
                    .ThenInclude(e => e.Dispositivos);

            if (!isAdmin)
            {
                if (userId.HasValue)
                {
                    propQuery = propQuery.Where(p => p.UsuariosPropiedades.Any(up => up.IdUsuario == userId.Value));
                }
                else
                {
                    propQuery = propQuery.Where(p => false);
                }
            }

            var propiedades = await propQuery.ToListAsync();

            return propiedades.Select(p => {
                var propietario = p.UsuariosPropiedades.FirstOrDefault()?.IdUsuarioNavigation;
                var totalDispositivos = p.Espacios?.Sum(e => e.Dispositivos?.Count ?? 0) ?? 0;
                
                return new PropiedadDto
                {
                    Id = p.Id,
                    Direccion = p.Direccion ?? string.Empty,
                    Tipo = p.Tipo ?? string.Empty,
                    Pisos = p.Pisos,
                    PropietarioNombre = propietario?.Nombre ?? "Usuario no disponible",
                    PropietarioCorreo = propietario?.Correo ?? string.Empty,
                    PropietarioInicial = (propietario?.Nombre?.Length > 0) ? propietario.Nombre.Substring(0, 1).ToUpper() : "U",
                    TotalEspacios = p.Espacios?.Count ?? 0,
                    TotalDispositivos = totalDispositivos,
                    // TODO: (Arch-1) Aislar simulación domótica de este servicio.
                    ConsumoEnergeticoSimulado = Random.Shared.Next(18, 35) + Random.Shared.NextDouble()
                };
            }).ToList();
        }

        public async Task<int> GetTotalPropiedadesAsync()
        {
            return await _context.Propiedades.CountAsync();
        }
    }
}
