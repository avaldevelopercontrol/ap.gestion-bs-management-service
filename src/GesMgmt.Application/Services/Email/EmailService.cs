using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Email;
using GesMgmt.Application.Logger;
using GesMgmt.Application.Validators.Email;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Email.EmailRequestDto;
using static GesMgmt.Application.DTOs.Email.EmailResponseDto;

namespace GesMgmt.Application.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

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
                _Logger.LogError($"GetEmailsByIdDeudor|DatabaseError: {ex.Message}");
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
                _Logger.LogError($"GetEmailsByIdEmailPers|DatabaseError: {ex.Message}");
                return ResultDto<GetPersEmailsResponseDto>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        #region "Grabar Email"
        public async Task<ResultDto<CreateEmailResponseDto>> CreateEmailAsync(CreateEmailRequestDto emailCreateDto)
        {
            CreateEmailRequestValidator validator = new CreateEmailRequestValidator(_unitOfWork, _validationMessageService, emailCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_PersEmail persemail = new av_PersEmail
                {
                    nId_PersDeudor = emailCreateDto.nId_PersDeudor,
                    cPers_Email = emailCreateDto.cPers_Email,
                    bEstado = emailCreateDto.bEstado,
                    cEmail_Coment = emailCreateDto.cEmail_Coment,
                    cEmail_Contacto = emailCreateDto.cEmail_Contacto,
                    nId_Cliente = emailCreateDto.nId_Cliente,
                    bBaseCliente = emailCreateDto.bBaseCliente,
                    nId_UsuarioAct = emailCreateDto.nId_UsuarioAct,
                    dFecRegistro = DateTime.Now,
                    dFecActualizacion = DateTime.Now,
                    nEmail_Prioridad = emailCreateDto.nEmail_Prioridad,
                    nId_PersEmailOpe = emailCreateDto.nId_PersEmailOpe
                };
                var emailCreate = await _unitOfWork.av_PersEmails.AddAsync(persemail);
                await _unitOfWork.SaveChangesAsync();

                CreateEmailResponseDto responseDto = new CreateEmailResponseDto
                {
                    nId_PersEmail = emailCreate.nId_PersEmail,
                    nId_PersDeudor = emailCreate.nId_PersDeudor,
                    cPers_Email = emailCreate.cPers_Email
                };

                ResultDto<CreateEmailResponseDto> response = ResultDto<CreateEmailResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"CreateEmail|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateEmailResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud. " + ex.Message, 500);
            }
        }
        #endregion

        #region "Editar Email"
        public async Task<ResultDto<EditEmailResponseDto>> EditEmailAsync(EditEmailRequestDto emailEditDto)
        {
            EditEmailRequestValidator validator = new EditEmailRequestValidator(_unitOfWork, _validationMessageService, emailEditDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_PersEmail persemail = new av_PersEmail
                {
                    nId_PersEmail = emailEditDto.nId_PersEmail,
                    nId_PersDeudor = emailEditDto.nId_PersDeudor,
                    cPers_Email = emailEditDto.cPers_Email,
                    bEstado = emailEditDto.bEstado,
                    cEmail_Coment = emailEditDto.cEmail_Coment,
                    cEmail_Contacto = emailEditDto.cEmail_Contacto,
                    nId_Cliente = emailEditDto.nId_Cliente,
                    bBaseCliente = emailEditDto.bBaseCliente,
                    nId_UsuarioAct = emailEditDto.nId_UsuarioAct,
                    dFecRegistro = DateTime.Now,
                    dFecActualizacion = DateTime.Now,
                    nEmail_Prioridad = emailEditDto.nEmail_Prioridad,
                    nId_PersEmailOpe = emailEditDto.nId_PersEmailOpe
                };
                var emailEdit = await _unitOfWork.av_PersEmails.UpdateAsync(persemail);
                await _unitOfWork.SaveChangesAsync();

                EditEmailResponseDto responseDto = new EditEmailResponseDto
                {
                    nId_PersEmail = emailEdit.nId_PersEmail,
                    nId_PersDeudor = emailEdit.nId_PersDeudor,
                    cPers_Email = emailEdit.cPers_Email
                };

                ResultDto<EditEmailResponseDto> response = ResultDto<EditEmailResponseDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"EditEmail|DatabaseError: {ex.Message}");
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<EditEmailResponseDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud. " + ex.Message, 500);
            }
        }
        #endregion

        #region "Listado de Status"
        public async Task<ResultListaDto<IEnumerable<GetStatus>>> GetStatusAsync()
        {
            try
            {
                var q_Status = await _unitOfWork.av_PersEmailOpes.Query();
                var data = await (
                                    from s in q_Status
                                    orderby s.cNombre_PersEmailOpe
                                    select new GetStatus
                                    {
                                        nId_PersTelefOpe = s.nId_PersEmailOpe,
                                        cNombre_PersTelefOpe = s.cNombre_PersEmailOpe
                                    }
                    ).ToListAsync();

                return ResultListaDto<IEnumerable<GetStatus>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetStatus|DatabaseError: {ex.Message}");
                return ResultListaDto<IEnumerable<GetStatus>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
        #endregion
    }
}