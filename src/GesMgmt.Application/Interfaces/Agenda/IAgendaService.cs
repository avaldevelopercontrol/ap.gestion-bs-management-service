using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Agenda.AgendaRequestDto;
using static GesMgmt.Application.DTOs.Agenda.AgendaResponseDto;

namespace GesMgmt.Application.Interfaces.Agenda
{
    public interface IAgendaService
    {
        Task<ResultDto<CreateAgendaResponsetDto>> CreateAgendaAsync(CreateAgendaRequestDto agendaCreateDto);
    }
}