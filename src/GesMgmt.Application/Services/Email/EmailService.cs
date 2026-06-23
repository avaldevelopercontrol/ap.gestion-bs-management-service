using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Email;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Direccion.DireccionResponseDto;
using static GesMgmt.Application.DTOs.Email.EmailRequestDto;
using static GesMgmt.Application.DTOs.Email.EmailResponseDto;

namespace GesMgmt.Application.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;

        public EmailService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        public async Task<ResultListDto<IEnumerable<GetEmailsPersDeudorResponseDto>>> GetEmailsByIdDeudorAsync(GetEmailsPersDeudorRequestDto gestionTelefonoDto)
        {
            try
            {
                var q_PerEmail = _unitOfWork.av_PersEmails.GetEmailsByIdDeudorAsync(gestionTelefonoDto.nId_Cliente, gestionTelefonoDto.nId_Persdeudor);
                var q_Empresa = await _unitOfWork.av_Clientes.Query();
                var q_PeEmOpe = await _unitOfWork.av_PersEmailOpes.Query();

                var data = await (
                                    from pe in q_PerEmail

                                    join ope in q_PeEmOpe
                                    on pe.nId_PersEmailOpe equals ope.nId_PersEmailOpe
                                    into opeGroup
                                    from ope in opeGroup.DefaultIfEmpty()

                                    join cli in q_Empresa
                                    on pe.nId_Cliente equals cli.nId_Cliente
                                    into cliGroup
                                    from cli in cliGroup.DefaultIfEmpty()

                                    select new GetEmailsPersDeudorResponseDto
                                    {
                                        nId_PersEmail = pe.nId_PersEmail,
                                        email = pe.cPers_Email,
                                        fechaActivacion = pe.dFecActualizacion,
                                        estado = pe.bEstado == true ? "ACTIVO" : "INACTIVO",
                                        status = ope.cSigla_PersEmailOpe,
                                        fuente = cli.cCli_Siglas,
                                        baseCliente = pe.dFecBaseCliente.ToString() ?? "",
                                        contacto = pe.cEmail_Contacto,
                                        prioridad = pe.nEmail_Prioridad ?? 0,
                                        comentario = pe.cEmail_Coment
                                    })
                                    .Skip((gestionTelefonoDto.PageNumber - 1) * gestionTelefonoDto.PageSize)
                                    .Take(gestionTelefonoDto.PageSize)
                                    .ToListAsync();

                int totalRecords = data.Count();

                var response = ResultListDto<IEnumerable<GetEmailsPersDeudorResponseDto>>.Success(data, "200", "OK", "OK", 200);

                response.TotalRecords = totalRecords;
                response.PageNumber = gestionTelefonoDto.PageNumber;
                response.PageSize = gestionTelefonoDto.PageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalRecords / gestionTelefonoDto.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ResultListDto<IEnumerable<GetEmailsPersDeudorResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultDto<GetPersEmailsResponseDto>> GetEmailsByIdEmailPersAsync(int nId_PersEmail)
        {
            try
            {
                var q_PerEma = _unitOfWork.av_PersEmails.GetEmailsByIdPersEmail(nId_PersEmail);

                var data = await (
                    from pe in q_PerEma

                    select new GetPersEmailsResponseDto
                    {
                        nId_PersEmail = pe.nId_PersEmail,
                        nId_PersDeudor = pe.nId_PersDeudor,
                        cPers_Email = pe.cPers_Email,
                        bEstado = pe.bEstado,
                        cEmail_Coment = pe.cEmail_Coment ?? "",
                        cEmail_Contacto = pe.cEmail_Contacto ?? "",
                        nId_Cliente = pe.nId_Cliente,
                        bBaseCliente = pe.bBaseCliente,
                        dFecRegistro = pe.dFecRegistro,
                        nId_UsuarioAct = pe.nId_UsuarioAct,
                        dFecActualizacion = pe.dFecActualizacion,
                        nEmail_Prioridad = pe.nEmail_Prioridad ?? 0,
                        nId_EstadoEnvioEmail = pe.nId_EstadoEnvioEmail ?? 0,
                        cEstado = pe.cEstado ?? "",
                        dFecEstadoEnvio = pe.dFecEstadoEnvio ?? DateTime.Now,
                        nId_EstadoEnvioEmailGen = pe.nId_EstadoEnvioEmailGen ?? 0,
                        dFecBaseCliente = pe.dFecBaseCliente,
                        nId_PersEmailOpe = pe.nId_PersEmailOpe

                    }
                ).FirstOrDefaultAsync();

                return ResultDto<GetPersEmailsResponseDto>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                return ResultDto<GetPersEmailsResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

    }
}