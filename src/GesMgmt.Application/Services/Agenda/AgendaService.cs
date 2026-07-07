using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Agenda;
using GesMgmt.Application.Validators.Agenda;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Entities;
using GesMgmt.Domain.Interfaces;
using static GesMgmt.Application.DTOs.Agenda.AgendaRequestDto;
using static GesMgmt.Application.DTOs.Agenda.AgendaResponseDto;

namespace GesMgmt.Application.Services.Agenda
{
    public class AgendaService : IAgendaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;

        public AgendaService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        #region "Grabar Agenda"
        public async Task<ResultDto<CreateAgendaResponsetDto>> CreateAgendaAsync(CreateAgendaRequestDto agendaCreateDto)
        {
            CreateAgendaRequestValidator validator = new CreateAgendaRequestValidator(_unitOfWork, _validationMessageService, agendaCreateDto);

            // Validaciones
            var validationResult = await validator.Validate();

            if (validationResult.Code != Const.SUCCESS_CODE)
            {
                return validationResult;
            }

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                av_Agenda agenda = new av_Agenda
                {
                    dFechNuevaGestion = agendaCreateDto.dFechNuevaGestion,
                    nid_PersDeudor = agendaCreateDto.nid_PersDeudor,
                    Nombre = agendaCreateDto.Nombre,
                    Cartera = agendaCreateDto.Cartera,
                    nid_Cartera = agendaCreateDto.nid_Cartera,
                    nid_Cliente = agendaCreateDto.nid_Cliente,
                    nid_UsuOpe = agendaCreateDto.nid_UsuOpe,
                    dFecRegistro = DateTime.Now,
                    cUsr_Login = agendaCreateDto.cUsr_Login,
                    nId_TipoOpeCodCliOut = agendaCreateDto.nId_TipoOpeCodCliOut,
                    cRespuestaOpe = agendaCreateDto.cRespuestaOpe,
                    nId_OpeCodCliOut = agendaCreateDto.nId_OpeCodCliOut,
                };
                var agendaCreate = await _unitOfWork.av_Agendas.AddAsync(agenda);
                await _unitOfWork.SaveChangesAsync();

                CreateAgendaResponsetDto responseDto = new CreateAgendaResponsetDto
                {
                    nid_Cliente = agendaCreate.nid_Cliente,
                    nid_Cartera = agendaCreate.nid_Cartera,
                    nid_UsuOpe = agendaCreate.nid_UsuOpe,
                    nid_PersDeudor = agendaCreate.nid_PersDeudor,
                };

                ResultDto<CreateAgendaResponsetDto> response = ResultDto<CreateAgendaResponsetDto>
                                                   .Success(responseDto, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);

                await _unitOfWork.CommitTransactionAsync();

                return response;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResultDto<CreateAgendaResponsetDto>.Failure("500", "Error interno del servidor.", "Ocurrió un error al procesar la solicitud. " + ex.Message, 500);
            }
        }
        #endregion
    }
}