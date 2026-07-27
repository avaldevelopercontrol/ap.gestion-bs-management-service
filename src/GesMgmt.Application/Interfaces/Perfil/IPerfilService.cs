using GesMgmt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.Application.Interfaces.Perfil
{
    public interface IPerfilService
    {
        Task<ResultListaDto<IEnumerable<GetPerfilListResponseDto>>> GetPerfilesAsync();
    }
}