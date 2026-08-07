using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Cliente;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Cliente.ClienteResponseDto;

namespace GesMgmt.Application.Services.Cliente
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public ClienteService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        public async Task<ResultListDto<IEnumerable<GetClientesActivosResponsetDto>>> GetClientesActivosAsync()
        {
            try
            {
                var q_clientes = await _unitOfWork.av_Clientes.ClientesActivosAsync();
                var data = await (
                                from cliente in q_clientes
                                orderby cliente.cCli_Nombre
                                select new GetClientesActivosResponsetDto
                                {
                                    nId_Cliente = cliente.nId_Cliente,
                                    cCli_Nombre = cliente.cCli_Nombre
                                }).ToListAsync();
                return ResultListDto<IEnumerable<GetClientesActivosResponsetDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetClientesActivosAsync|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetClientesActivosResponsetDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
    }
}