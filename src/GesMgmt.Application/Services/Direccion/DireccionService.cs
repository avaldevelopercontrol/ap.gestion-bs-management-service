using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Direccion;
using GesMgmt.Application.Validators.Direccion;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Direccion.DireccionRequestDto;
using static GesMgmt.Application.DTOs.Direccion.DireccionResponseDto;

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

        public async Task<ResultDto<CreateDireccionResponseDto>> CreateDireccionAsync(CreateDireccionRequestDto direccionCreateDto)
        {
            CreateDireccionRequestValidator validator = new CreateDireccionRequestValidator(_unitOfWork, _validationMessageService, direccionCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_PersDirecc persDirecc = new av_PersDirecc
                {
                    nId_PersDeudor = direccionCreateDto.nId_PersDeudor ?? 0,
                    cDirecc_Nomb = direccionCreateDto.cDirecc_Nomb,
                    nId_ubigeo = direccionCreateDto.nId_Distrito != 0
                                ? direccionCreateDto.nId_Distrito
                                : direccionCreateDto.nId_Provincia != 0
                                ? direccionCreateDto.nId_Provincia
                                : direccionCreateDto.nId_Departamento != 0
                                ? direccionCreateDto.nId_Departamento
                                : 0,
                    nId_PersRefUbi = direccionCreateDto.nId_PersRefUbi,
                    cDirecc_Coment = direccionCreateDto.cDirecc_Coment,
                    bEstado = direccionCreateDto.bEstado,
                    bOrigen_Base = direccionCreateDto.bOrigen_Base,
                    cTipoCoDeudor = direccionCreateDto.cTipoCoDeudor,
                    dFec_Actualizacion = DateTime.Now,
                    nId_Cliente = direccionCreateDto.nId_Cliente,
                    nid_CalifDirecc = direccionCreateDto.nid_CalifDirecc,
                    nid_usuarioUpd = direccionCreateDto.nid_usuarioUpd,
                };
                var direccionCreate = await _unitOfWork.av_PersDireccs.AddAsync(persDirecc);
                await _unitOfWork.SaveChangesAsync();

                CreateDireccionResponseDto responseDto = new CreateDireccionResponseDto
                {
                    nId_PersDeudor = direccionCreate.nId_PersDeudor,
                    nId_PersDirecc = direccionCreate.nId_PersDirecc,
                    nId_Ubigeo = direccionCreate.nId_ubigeo
                };

                ResultDto<CreateDireccionResponseDto> response = ResultDto<CreateDireccionResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateDireccionResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud.", 500);
            }
        }

        public async Task<ResultDto<EditDireccionResponseDto>> EditDireccionAsync(EditDireccionRequestDto direccionEditDto)
        {
            EditDireccionRequestValidator validator = new EditDireccionRequestValidator(_unitOfWork, _validationMessageService, direccionEditDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            // Iniciar Transacción y ejecutar actualización (común para ambos casos)
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_PersDirecc persDirecc = new av_PersDirecc
                {
                    nId_PersDeudor = direccionEditDto.nId_PersDeudor ?? 0,
                    cDirecc_Nomb = direccionEditDto.cDirecc_Nomb,
                    nId_ubigeo = direccionEditDto.nId_Distrito != 0
                                ? direccionEditDto.nId_Distrito
                                : direccionEditDto.nId_Provincia != 0
                                ? direccionEditDto.nId_Provincia
                                : direccionEditDto.nId_Departamento != 0
                                ? direccionEditDto.nId_Departamento
                                : 0,
                    nId_PersRefUbi = direccionEditDto.nId_PersRefUbi,
                    cDirecc_Coment = direccionEditDto.cDirecc_Coment,
                    bEstado = direccionEditDto.bEstado,
                    bOrigen_Base = direccionEditDto.bOrigen_Base,
                    cTipoCoDeudor = direccionEditDto.cTipoCoDeudor,
                    dFec_Actualizacion = DateTime.Now,
                    nId_Cliente = direccionEditDto.nId_Cliente,
                    nid_CalifDirecc = direccionEditDto.nid_CalifDirecc,
                    nid_usuarioUpd = direccionEditDto.nid_usuarioUpd,
                };
                var direccionCreate = await _unitOfWork.av_PersDireccs.UpdateAsync(persDirecc);
                await _unitOfWork.SaveChangesAsync();

                EditDireccionResponseDto responseDto = new EditDireccionResponseDto
                {
                    nId_PersDeudor = direccionCreate.nId_PersDeudor,
                    nId_PersDirecc = direccionCreate.nId_PersDirecc,
                    nId_Ubigeo = direccionCreate.nId_ubigeo
                };

                ResultDto<EditDireccionResponseDto> response = ResultDto<EditDireccionResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ResultListaDto<IEnumerable<GetDireccionDepartamentos>>> GetDireccionDepartamentosAsync() 
        {
            try
            {
                var q_Resultados = _unitOfWork.av_Ubigeos.GetDepartamentosAsync();
                var data = await (
                                    from s in q_Resultados
                                    orderby s.cNombre_Ubigeo, s.nId_Departamento
                                    select new GetDireccionDepartamentos
                                    {
                                        nId_Departamento = s.nId_Ubigeo,
                                        cNombre_Departamento = s.cNombre_Ubigeo
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetDireccionDepartamentos>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetDireccionDepartamentos>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListaDto<IEnumerable<GetDireccionProvincias>>> GetDireccionProvinciasAsync(int nId_Departamento)
        {
            try
            {
                var q_Resultados = _unitOfWork.av_Ubigeos.GetProvinciasAsync(nId_Departamento);
                var data = await (
                                    from s in q_Resultados
                                    orderby s.cNombre_Ubigeo, s.nId_Departamento
                                    select new GetDireccionProvincias
                                    {
                                        nId_Provincia = s.nId_Ubigeo,
                                        cNombre_Provincia = s.cNombre_Ubigeo
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetDireccionProvincias>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetDireccionProvincias>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListaDto<IEnumerable<GetDireccionDistritos>>> GetDireccionDistritosAsync(int nId_Departamento, int nId_Provincia)
        {
            try
            {
                var q_Resultados = _unitOfWork.av_Ubigeos.GetDistritosAsync(nId_Departamento, nId_Provincia);
                var data = await (
                                    from s in q_Resultados
                                    orderby s.cNombre_Ubigeo, s.nId_Departamento
                                    select new GetDireccionDistritos
                                    {
                                        nId_Distrito = s.nId_Ubigeo,
                                        cNombre_Distrito = s.cNombre_Ubigeo
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetDireccionDistritos>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetDireccionDistritos>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListaDto<IEnumerable<GetDireccionUbicaciones>>> GetDireccionUbicacionesAsync()
        {
            try
            {
                var q_Resultados = _unitOfWork.av_PersRefUbis.GetUbicacionesTelefono();
                var data = await (
                                    from s in q_Resultados
                                    select new GetDireccionUbicaciones
                                    {
                                        nId_PersRefUbi = s.nId_PersRefUbi,
                                        cNombre_PersRefUbi = s.cNombre_PersRefUbi,
                                        cSigla_PersRefUbi = s.cSigla_PersRefUbi,
                                        bEstado = s.bEstado,
                                        nGestionMovil = s.nGestionMovil ?? 0
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetDireccionUbicaciones>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultListaDto<IEnumerable<GetDireccionUbicaciones>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
    }
}