using GesMgmt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;
using static GesMgmt.Application.DTOs.Email.EmailRequestDto;
using static GesMgmt.Application.DTOs.Email.EmailResponseDto;

namespace GesMgmt.Application.Interfaces.Email
{
    public interface IEmailService
    {
        Task<ResultListDto<IEnumerable<GetEmailsPersDeudorResponseDto>>> GetEmailsByIdDeudorAsync(GetEmailsPersDeudorRequestDto gestionTelefonoDto);
        Task<ResultDto<GetPersEmailsResponseDto>> GetEmailsByIdEmailPersAsync(int nId_PersEmail);
        Task<ResultDto<CreateEmailResponseDto>> CreateEmailAsync(CreateEmailRequestDto emailCreateDto);
        Task<ResultDto<EditEmailResponseDto>> EditEmailAsync(EditEmailRequestDto emailEditDto);
        Task<ResultListaDto<IEnumerable<GetStatus>>> GetStatusAsync();
    }
}