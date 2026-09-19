using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Produccion;
using GesMgmt.Application.Logger;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using static GesMgmt.Application.DTOs.Produccion.ProduccionRequestDto;
using static GesMgmt.Application.DTOs.Produccion.ProduccionResponseDto;

namespace GesMgmt.Application.Services.Produccion
{
    public class ProduccionService : IProduccionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;

        public ProduccionService(IUnitOfWork unitOfWork, IValidationMessageService validationMessageService)
        {
            _unitOfWork = unitOfWork;
            _validationMessageService = validationMessageService;
        }

        public async Task<ResultListDto<IEnumerable<GetProvinciasProduccionResponseDto>>>GetProvinciasProduccionAsync()
        {
            try
            {
                var q_ubigeo = await _unitOfWork.av_Ubigeos.Query();

                var provincias = await (
                    from ubigeo in q_ubigeo
                    where ubigeo.nNivel_Id == 2
                       && ubigeo.nId_Pais == 2196
                    select new GetProvinciasProduccionResponseDto
                    {
                        nId_Ubigeo = ubigeo.nId_Ubigeo,
                        cNombre_Ubigeo = ubigeo.cNombre_Ubigeo
                    }
                ).ToListAsync();

                // Equivalente a:
                // SELECT -1, 'Todo Provincia'

                provincias.Add(new GetProvinciasProduccionResponseDto
                {
                    nId_Ubigeo = -1,
                    cNombre_Ubigeo = "Todo Provincia"
                });

                // Equivalente a ORDER BY 2
                var data = provincias
                    .OrderBy(x => x.cNombre_Ubigeo)
                    .ToList();

                return ResultListDto<IEnumerable<GetProvinciasProduccionResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetProvinciasProduccionAsync|DatabaseError: {ex.Message}");

                return ResultListDto<IEnumerable<GetProvinciasProduccionResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListDto<IEnumerable<GetClientesProduccionActivosResponsetDto>>> GetClientesProduccionActivosAsync()
        {
            try
            {
                var q_clientes = await _unitOfWork.av_Clientes.ClientesActivosAsync();
                var q_carteras = await _unitOfWork.av_Carteras.Query();

                var data = await (
                    from cliente in q_clientes
                    join cartera in q_carteras
                        on cliente.nId_Cliente equals cartera.nId_Cliente
                    where cliente.bEstado == true
                       && cartera.bEstado == true
                    group cliente by new
                    {
                        cliente.nId_Cliente,
                        cliente.cCli_Siglas
                    }
                    into g
                    orderby g.Key.cCli_Siglas
                    select new GetClientesProduccionActivosResponsetDto
                    {
                        nId_Cliente = g.Key.nId_Cliente,
                        cCli_Siglas = g.Key.cCli_Siglas
                    }
                ).ToListAsync();

                return ResultListDto<IEnumerable<GetClientesProduccionActivosResponsetDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetClientesProduccionActivosAsync|DatabaseError: {ex.Message}");
                return ResultListDto<IEnumerable<GetClientesProduccionActivosResponsetDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListDto<IEnumerable<GetPerfilesActivosResponsetDto>>>GetPerfilesProduccionAsync()
        {
            try
            {
                var q_produccion = await _unitOfWork.av_ProduccionDias.Query();
                var q_usuarios = await _unitOfWork.av_Usuarios.Query();
                var q_perfiles = await _unitOfWork.av_Perfils.Query();

                var data = await (
                    from pd in q_produccion
                    join us in q_usuarios
                        on pd.nId_UsuOpe equals us.nId_Usuario
                    join per in q_perfiles
                        on us.nid_perfil equals per.nid_perfil
                    group per by new
                    {
                        per.nid_perfil,
                        per.per_Nombre
                    }
                    into g
                    orderby g.Key.per_Nombre
                    select new GetPerfilesActivosResponsetDto
                    {
                        nid_perfil = g.Key.nid_perfil,
                        per_Nombre = g.Key.per_Nombre
                    }
                ).ToListAsync();

                return ResultListDto<IEnumerable<GetPerfilesActivosResponsetDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"GetPerfilesProduccionAsync|DatabaseError: {ex.Message}");

                return ResultListDto<IEnumerable<GetPerfilesActivosResponsetDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }

        public async Task<ResultListDto<IEnumerable<GetProduccionResumenResponseDto>>> GetProduccionResumenAsync(GetProduccionResumenRequestDto request)
        {
            try
            {
                // =========================================================
                // 1. QUERYS BASE
                // =========================================================
                var q_produccion = await _unitOfWork.av_ProduccionDias.Query();
                var q_usuarios = await _unitOfWork.av_Usuarios.Query();
                var q_clientes = await _unitOfWork.av_Clientes.Query();
                var q_contactos = await _unitOfWork.av_OpeCodCliOuts.Query();

                // =========================================================
                // 2. PARAMETROS
                // SQL ORIGINAL:
                //
                // @nid_cliente
                // @nid_perfil
                // @nid_ubigeo
                // @nid_tipoLlamada
                // =========================================================

                var fechaInicio = DateTime.Today.AddHours(7);
                var fechaFin = DateTime.Now;
                var nIdCliente = request.nId_Cliente;
                var nIdPerfil = request.nId_Perfil;
                var nIdUbigeo = request.nId_Ubigeo;
                var nIdTipoLlamada = request.nId_TipoLlamada;

                //// =========================================================
                //// 3. LOG DE PARAMETROS
                //// =========================================================
                //_Logger.LogError($"GetProduccionResumenAsync|" +
                //    $"Cliente={nIdCliente}|" +
                //    $"Perfil={nIdPerfil}|" +
                //    $"Ubigeo={nIdUbigeo}|" +
                //    $"TipoLlamada={nIdTipoLlamada}|" +
                //    $"FechaInicio={fechaInicio:yyyy-MM-dd HH:mm:ss}|" +
                //    $"FechaFin={fechaFin:yyyy-MM-dd HH:mm:ss}");

                // =========================================================
                // 4. SUBQUERY GESTION
                //
                // Equivalente a:
                //
                // Select
                //      op.nId_UsuOpe,
                //      nid_cliente,
                //      nId_PersDeudor,
                //      nId_opeCodOut,
                //      dDocCobOpe_FecIni
                // From av_ProduccionDia
                // Where ...
                // Group By ...
                // =========================================================

                var gestion =
                    from    op in q_produccion
                    where   op.dDocCobOpe_FecIni >= fechaInicio
                            && op.dDocCobOpe_FecIni <= fechaFin
                        // ---------------------------------------------
                        // @nid_cliente > 0
                        // ---------------------------------------------
                        && (nIdCliente <= 0 || op.nid_cliente == nIdCliente)
                        // ---------------------------------------------
                        // @nid_tipoLlamada >= 0
                        // ---------------------------------------------
                        && (nIdTipoLlamada < 0 || (op.cDocxCobOpeInconcert ?? 0) == nIdTipoLlamada)
                    group   op by new
                    {
                        op.nId_UsuOpe,
                        op.nid_cliente,
                        op.nId_PersDeudor,
                        op.nId_opeCodOut,
                        op.dDocCobOpe_FecIni
                    }
                    into g
                    select new
                    {
                        g.Key.nId_UsuOpe,
                        g.Key.nid_cliente,
                        g.Key.nId_PersDeudor,
                        g.Key.nId_opeCodOut,
                        g.Key.dDocCobOpe_FecIni
                    };

                // =========================================================
                // 5. CONTACTOS
                //
                // contacto:
                // ISNULL(nId_OpeCodOut2,0) = 1
                //
                // contactoPromesa:
                // ISNULL(nId_OpeCodOut2,0) = 1
                // AND nId_TipoContacto = 2
                // =========================================================
                var contactos = q_contactos.Where(x => (x.nId_OpeCodOut2 ?? 0) == 1);
                var contactosPromesa = q_contactos.Where(x => (x.nId_OpeCodOut2 ?? 0) == 1 && x.nId_TipoContacto == 2);

                // =========================================================
                // 6. QUERY PRINCIPAL
                // =========================================================
                var query = from ges in gestion
                    // -----------------------------------------------------
                    // INNER JOIN av_Usuario
                    // -----------------------------------------------------
                    join us in q_usuarios
                        on (ges.nId_UsuOpe ?? 0)
                        equals us.nId_Usuario
                    // -----------------------------------------------------
                    // INNER JOIN av_Cliente
                    // -----------------------------------------------------
                    join cl in q_clientes
                        on (ges.nid_cliente ?? 0)
                        equals cl.nId_Cliente
                    // =====================================================
                    // LEFT JOIN CONTACTO
                    //
                    // IMPORTANTE:
                    // usamos ?? 0 para que ambos lados sean INT
                    // y evitar CS1941.
                    // =====================================================
                    join contactoTmp in contactos
                        on new
                        {
                            Ope = ges.nId_opeCodOut ?? 0,
                            Cliente = ges.nid_cliente ?? 0
                        }
                        equals new
                        {
                            Ope = contactoTmp.nId_OpeCodCliOut,
                            Cliente = contactoTmp.nId_Cliente
                        }
                        into contactoJoin
                    from contacto in contactoJoin.DefaultIfEmpty()
                    // =====================================================
                    // LEFT JOIN CONTACTO PROMESA
                    // =====================================================
                    join promesaTmp in contactosPromesa
                        on new
                        {
                            Ope = ges.nId_opeCodOut ?? 0,
                            Cliente = ges.nid_cliente ?? 0
                        }
                        equals new
                        {
                            Ope = promesaTmp.nId_OpeCodCliOut,
                            Cliente = promesaTmp.nId_Cliente
                        }
                        into promesaJoin
                    from contactoPromesa in
                        promesaJoin.DefaultIfEmpty()
                        // =====================================================
                        // WHERE DINAMICO
                        // =====================================================
                    where
                        // ---------------------------------------------
                        // @nid_perfil
                        //
                        // 0 = TODOS
                        // >0 = perfil especifico
                        // ---------------------------------------------
                        (
                            nIdPerfil <= 0 || us.nid_perfil == nIdPerfil
                        )
                        &&
                        // ---------------------------------------------
                        // @nid_ubigeo
                        //
                        // 0  = Todos
                        // -1 = Provincia:
                        //      todo excepto Lima 1379
                        // >0 = Ubigeo especifico
                        // ---------------------------------------------
                        (
                            nIdUbigeo == 0 || (nIdUbigeo == -1 && (us.nId_Ubigeo) != 1379) || (nIdUbigeo > 0 && (us.nId_Ubigeo) == nIdUbigeo)
                        )
                    select new
                    {
                        ges,
                        us,
                        cl,
                        contacto,
                        contactoPromesa
                    };
                // =========================================================
                // 7. EJECUTAR LOS JOINS EN SQL
                // =========================================================

                var resultadoBase = await query
                    .Select(x => new
                    {
                        // Usuario
                        x.us.cUsr_Nombres,
                        x.us.cUsr_ApePat,

                        // Cliente
                        x.cl.cCli_Siglas,

                        // Gestión
                        x.ges.nId_PersDeudor,
                        x.ges.dDocCobOpe_FecIni,

                        // Contacto
                        EsContacto =
                            x.contacto != null &&
                            (x.contacto.nId_OpeCodOut2 ?? 0) == 1,

                        // Promesa
                        EsPromesa =
                            x.contactoPromesa != null &&
                            (x.contactoPromesa.nId_OpeCodOut2 ?? 0) == 1
                    })
                    .ToListAsync();


                // =========================================================
                // 8. AGRUPACION EN MEMORIA
                // =========================================================

                var data = resultadoBase

                    // -----------------------------------------------------
                    // Generamos primer nombre + apellido
                    // -----------------------------------------------------
                    .Select(x =>
                    {
                        var nombres =
                            (x.cUsr_Nombres ?? "").Trim();

                        var primerNombre =
                            nombres.Contains(" ")
                                ? nombres.Substring(
                                    0,
                                    nombres.IndexOf(" ")
                                )
                                : nombres;

                        var nombreCompleto =
                            (
                                primerNombre
                                + " "
                                + (x.cUsr_ApePat ?? "").Trim()
                            )
                            .ToUpper();

                        return new
                        {
                            nombresUsu = nombreCompleto,

                            clienteNom =
                                x.cCli_Siglas ?? "",

                            x.nId_PersDeudor,

                            x.dDocCobOpe_FecIni,

                            x.EsContacto,

                            x.EsPromesa
                        };
                    })


                    // =====================================================
                    // GROUP BY
                    // =====================================================
                    .GroupBy(x => new
                    {
                        x.nombresUsu,
                        x.clienteNom
                    })


                    // =====================================================
                    // AGREGADOS
                    // =====================================================
                    .Select(g =>
                    {
                        var fechaMin =
                            g.Min(x =>
                                x.dDocCobOpe_FecIni
                            );

                        var fechaMax =
                            g.Max(x =>
                                x.dDocCobOpe_FecIni
                            );

                        var minutos =
                            fechaMin.HasValue &&
                            fechaMax.HasValue
                                ? (int)(
                                    fechaMax.Value
                                    -
                                    fechaMin.Value
                                ).TotalMinutes
                                : 0;

                        var contactos =
                            g.Count(x =>
                                x.EsContacto
                            );

                        var totalGestiones =
                            g.Count(x =>
                                x.nId_PersDeudor.HasValue
                            );

                        var contactosPromesa =
                            g.Count(x =>
                                x.EsPromesa
                            );

                        return new GetProduccionResumenResponseDto
                        {
                            nombresUsu = g.Key.nombresUsu,
                            clienteNom = g.Key.clienteNom,
                            minutosGes = minutos,
                            contactGes = contactos,
                            totalesGes = totalGestiones,
                            contactGesProm = contactosPromesa
                        };
                    })


                    // =====================================================
                    // HAVING
                    //
                    // contactGes >= 2
                    // minutosGes > 0
                    // =====================================================
                    .Where(x =>
                        x.contactGes >= 2
                        &&
                        x.minutosGes > 0
                    )


                    // =====================================================
                    // ORDER BY
                    //
                    // contactGes DESC
                    // minutosGes ASC
                    // =====================================================
                    .OrderByDescending(x =>
                        x.contactGes
                    )
                    .ThenBy(x =>
                        x.minutosGes
                    )
                    .ToList();

                // =========================================================
                // 10. RETURN
                // =========================================================
                return ResultListDto<IEnumerable<GetProduccionResumenResponseDto>>.Success(data, Const.SUCCESS_CODE, Const.SUCCESS_MESSAGE, Const.SUCCESS_MESSAGE, Const.OK_REQUEST_CODE);
            }
            catch (Exception ex)
            {
                // =========================================================
                // LOG CON PARAMETROS
                // =========================================================
                _Logger.LogError($"GetProduccionResumenAsync|" + $"Cliente={request.nId_Cliente}|" + $"Perfil={request.nId_Perfil}|" + $"Ubigeo={request.nId_Ubigeo}|" + $"TipoLlamada={request.nId_TipoLlamada}|" + $"Error={ex.Message}");
                return ResultListDto<IEnumerable<GetProduccionResumenResponseDto>>.Failure("500", "Error interno del servidor.", ex.Message, 500);
            }
        }
    }
}