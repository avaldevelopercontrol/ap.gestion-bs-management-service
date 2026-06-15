using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Direccion;
using GesMgmt.Application.Validators.Gestion;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Direccion.DireccionResponseDto;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;

namespace GesMgmt.Application.Services.Direccion
{
    public class DireccionService : IDireccionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;

        public DireccionService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        public async Task<ResultDto<GetDireccionAsync>> GetDireccionByIdDireccionAsync(int nId_PersDirecc)
        {
            try
            {
                var q_PerDir = _unitOfWork.av_PersDireccs.GetDireccionByIdDireccion(nId_PersDirecc);
                var q_PerRefUbi = await _unitOfWork.av_PersRefUbis.Query();
                var q_PersDeudor = await _unitOfWork.av_PersDeudors.Query();
                var q_Ubigeo = await _unitOfWork.av_Ubigeos.Query();

                var data = await (
                    from pe in q_PerDir

                    join refUbi in q_PerRefUbi
                        on pe.nId_PersRefUbi equals refUbi.nId_PersRefUbi
                        into refUbiJoin
                    from refUbi in refUbiJoin.DefaultIfEmpty()

                    join deu in q_PersDeudor
                        on pe.nId_PersDeudor equals deu.nId_PersDeudor
                        into avalJoin
                    from aval in avalJoin.DefaultIfEmpty()

                    join ubi in q_Ubigeo
                        on pe.nId_ubigeo equals ubi.nId_Ubigeo
                        into ubiJoin
                    from persubi in ubiJoin.DefaultIfEmpty()

                    select new GetDireccionAsync
                    {
                        nId_PersDirecc = pe.nId_PersDirecc,
                        cNombre_PersRefUbi = refUbi.cNombre_PersRefUbi ?? "",
                        cDirecc_Nomb = pe.cDirecc_Nomb ?? "",
                        estado = pe.bEstado == true ? "OK" : "",
                        nId_PersRefUbi = pe.nId_PersRefUbi ?? 0,
                        cDirecc_Coment = pe.cDirecc_Coment ?? "",
                        bEstado = pe.bEstado ?? false,
                        bOrigen_Base = pe.bOrigen_Base ?? false,
                        nId_PersTitDeudor = pe.nId_PersTitDeudor ?? 0,
                        nombreAval = pe.nId_PersTitDeudor == null
                            ? ""
                            : (pe.cTipoCoDeudor ?? "") == "AVAL"
                                ? (aval != null ? aval.cNomCompleto : "")
                                : "",
                        cTipoCoDeudor = pe.cTipoCoDeudor ?? "",
                        nid_CalifDirecc = pe.nid_CalifDirecc ?? 0,
                        cDescrip_Fija = pe.cDescrip_Fija ?? "",
                        nId_Ubigeo = pe.nId_ubigeo ?? 0,
                        nId_Departamento = persubi.nId_Departamento,
                        nId_Provincia = persubi.nId_Provincia,
                        nId_Distrito = persubi.nId_Distrito
                    }
                ).FirstOrDefaultAsync();

                return ResultDto<GetDireccionAsync>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultDto<GetDireccionAsync>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListaDto<IEnumerable<GetUbigeoDepartamentos>>> GetUbigeoDepartamentosAsync() 
        {
            try
            {
                var q_Resultados = _unitOfWork.av_Ubigeos.GetDepartamentosAsync();
                var data = await (
                                    from s in q_Resultados
                                    orderby s.cNombre_Ubigeo, s.nId_Departamento
                                    select new GetUbigeoDepartamentos
                                    {
                                        nId_Departamento = s.nId_Ubigeo,
                                        cNombre_Departamento = s.cNombre_Ubigeo
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetUbigeoDepartamentos>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetUbigeoDepartamentos>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListaDto<IEnumerable<GetUbigeoProvincias>>> GetUbigeoProvinciasAsync(int nId_Departamento)
        {
            try
            {
                var q_Resultados = _unitOfWork.av_Ubigeos.GetProvinciasAsync(nId_Departamento);
                var data = await (
                                    from s in q_Resultados
                                    orderby s.cNombre_Ubigeo, s.nId_Departamento
                                    select new GetUbigeoProvincias
                                    {
                                        nId_Provincia = s.nId_Ubigeo,
                                        cNombre_Provincia = s.cNombre_Ubigeo
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetUbigeoProvincias>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetUbigeoProvincias>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListaDto<IEnumerable<GetUbigeoDistritos>>> GetUbigeoDistritosAsync(int nId_Departamento, int nId_Provincia)
        {
            try
            {
                var q_Resultados = _unitOfWork.av_Ubigeos.GetDistritosAsync(nId_Departamento, nId_Provincia);
                var data = await (
                                    from s in q_Resultados
                                    orderby s.cNombre_Ubigeo, s.nId_Departamento
                                    select new GetUbigeoDistritos
                                    {
                                        nId_Distrito = s.nId_Ubigeo,
                                        cNombre_Distrito = s.cNombre_Ubigeo
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetUbigeoDistritos>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetUbigeoDistritos>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
    }
}