using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;

namespace GesMgmt.Application.DTOs.Gestion.Converters
{
    public class GetGestionDocumentoResponseDtoJsonConverter
    : JsonConverter<GetGestionDocumentoResponseDto>
    {
        public override GetGestionDocumentoResponseDto Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotSupportedException(
                "GetGestionDocumentoResponseDto es un DTO de respuesta y no admite deserialización."
            );
        }

        public override void Write(
            Utf8JsonWriter writer,
            GetGestionDocumentoResponseDto value,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            // =====================================================
            // CAMPOS CABECERA
            // =====================================================
            EscribirCabecera(writer, value);


            // =====================================================
            // ESTRUCTURA SEGÚN CLIENTE
            // =====================================================

            switch (value.nId_ClienteJson)
            {
                case 95:
                    EscribirCliente95(writer, value);
                    break;
                case 59:
                    EscribirCliente59(writer, value);
                    break;
                default:
                    EscribirClienteGenerico(writer, value);
                    break;
            }
            writer.WriteEndObject();
        }


        // =========================================================
        // CABECERA COMÚN
        // =========================================================

        private static void EscribirCabecera(
            Utf8JsonWriter writer,
            GetGestionDocumentoResponseDto value)
        {
            /*
             * EL ORDEN DE ESTAS LÍNEAS
             * ES EXACTAMENTE EL ORDEN DEL JSON.
             */

            writer.WriteNumber(
                "nId_DocxCobrar",
                value.nId_DocxCobrar
            );


            if (value.mejorStatus.HasValue)
            {
                writer.WriteNumber(
                    "mejorStatus",
                    value.mejorStatus.Value
                );
            }


            writer.WriteNumber(
                "nId_Moneda",
                value.nId_Moneda
            );


            if (value.bEstado.HasValue)
            {
                writer.WriteNumber(
                    "bEstado",
                    value.bEstado.Value
                );
            }


            EscribirString(
                writer,
                "nZona",
                value.nZona
            );


            writer.WriteBoolean(
                "bSelected",
                value.bSelected
            );


            if (value.nId_Estrategia.HasValue)
            {
                writer.WriteNumber(
                    "nId_Estrategia",
                    value.nId_Estrategia.Value
                );
            }


            writer.WriteNumber(
                "nId_Cartera",
                value.nId_Cartera
            );
        }


        // =========================================================
        // CLIENTE 95 - CLARO
        // =========================================================

        private static void EscribirCliente95(
            Utf8JsonWriter writer,
            GetGestionDocumentoResponseDto value)
        {
            /*
             * AQUÍ DEFINES EXACTAMENTE
             * EL ORDEN PARA CLARO.
             */

            EscribirString(
                writer,
                "tramo",
                value.tramo
            );


            writer.WriteNumber(
                "nro",
                value.nro
            );


            EscribirString(
                writer,
                "numeroDocumento",
                value.numeroDocumento
            );


            EscribirString(
                writer,
                "estado",
                value.estado
            );


            EscribirString(
                writer,
                "fechaVencimiento",
                value.fechaVencimiento
            );


            EscribirString(
                writer,
                "siglaMoneda",
                value.siglaMoneda
            );


            EscribirDecimal(
                writer,
                "importeTotal",
                value.importeTotal
            );


            EscribirDecimal(
                writer,
                "importeSaldo",
                value.importeSaldo
            );


            writer.WriteNumber(
                "diasAtrazo",
                value.diasAtrazo
            );

            // =====================================================
            // CAMPOS ESPECÍFICOS CLARO
            // =====================================================

            EscribirString(
                writer,
                "servicio",
                value.servicio
            );


            EscribirString(
                writer,
                "comentario",
                value.comentario
            );


            EscribirString(
                writer,
                "codigoCliente",
                value.codigoCliente
            );


            EscribirString(
                writer,
                "estadoDocumento",
                value.estadoDocumento
            );


            EscribirString(
                writer,
                "fechaEstadoDocumento",
                value.fechaEstadoDocumento
            );


            EscribirString(
                writer,
                "estadoPago",
                value.estadoPago
            );


            EscribirString(
                writer,
                "statusDocumento",
                value.statusDocumento
            );


            EscribirString(
                writer,
                "fechaStatusDocumento",
                value.fechaStatusDocumento
            );


            EscribirString(
                writer,
                "gestorCall",
                value.gestorCall
            );


            EscribirString(
                writer,
                "bajaProvabilidad",
                value.bajaProvabilidad
            );
        }


        // =========================================================
        // CLIENTE 59 - MAF
        // =========================================================

        private static void EscribirCliente59(
            Utf8JsonWriter writer,
            GetGestionDocumentoResponseDto value)
        {
            /*
             * AQUÍ DEFINES EXACTAMENTE
             * EL ORDEN PARA MAF.
             *
             * ESTE ORDEN PUEDE SER COMPLETAMENTE
             * DIFERENTE AL CLIENTE 59.
             */

            // =====================================================
            // CAMPOS MAF
            // =====================================================

            EscribirString(
                writer,
                "tramo",
                value.tramo
            );

            writer.WriteNumber(
                "nro",
                value.nro
            );

            EscribirString(
                writer,
                "numeroDocumento",
                value.numeroDocumento
            );

            EscribirString(
                writer,
                "estado",
                value.estado
            );

            EscribirString(
                writer,
                "numeroCuota",
                value.numeroCuota
            );

            EscribirString(
                writer,
                "fechaVencimiento",
                value.fechaVencimiento
            );

            EscribirString(
                writer,
                "siglaMoneda",
                value.siglaMoneda
            );

            EscribirDecimal(
                writer,
                "importeTotal",
                value.importeTotal
            );

            EscribirDecimal(
                writer,
                "importeSaldo",
                value.importeSaldo
            );

            EscribirDecimal(
                writer,
                "deudaVencida",
                value.deudaVencida
            );

            writer.WriteNumber(
                "diasAtrazo",
                value.diasAtrazo
            );

            EscribirString(
                writer,
                "tipoCredito",
                value.tipoCredito
            );

            EscribirString(
                writer,
                "COD_ACC_PREV",
                value.COD_ACC_PREV
            );

            EscribirString(
                writer,
                "COD_ACC_PREJU",
                value.COD_ACC_PREJU
            );

            EscribirString(
                writer,
                "ultimoTramo",
                value.ultimoTramo
            );

            EscribirString(
                writer,
                "ultimoFechaPago",
                value.ultimoFechaPago
            );

            EscribirString(
                writer,
                "categoria",
                value.categoria
            );

            EscribirString(
                writer,
                "numeroReprogramaciones",
                value.numeroReprogramaciones
            );

            EscribirString(
                writer,
                "gWhatsApp",
                value.gWhatsApp
            );

            EscribirString(
                writer,
                "cuotaActual",
                value.cuotaActual
            );

            EscribirString(
                writer,
                "interesActual",
                value.interesActual
            );

            EscribirString(
                writer,
                "placa",
                value.placa
            );

            EscribirString(
                writer,
                "numeroCuenta",
                value.numeroCuenta
            );

            EscribirString(
                writer,
                "MARCA_ESPECIAL",
                value.MARCA_ESPECIAL
            );

            EscribirString(
                writer,
                "plazoReprogramado",
                value.plazoReprogramado
            );

            EscribirString(
                writer,
                "plazoMaximoReprogramado",
                value.plazoMaximoReprogramado
            );

            EscribirString(
                writer,
                "gestorCall",
                value.gestorCall
            );

            EscribirString(
                writer,
                "MARCA_ESPECIAL2",
                value.MARCA_ESPECIAL2
            );

            EscribirString(
                writer,
                "COMENTARIO_REPROG",
                value.COMENTARIO_REPROG
            );

            EscribirString(
                writer,
                "TASA_INTERES",
                value.TASA_INTERES
            );

            EscribirString(
                writer,
                "CAPITAL_ACTUAL",
                value.CAPITAL_ACTUAL
            );
        }


        // =========================================================
        // CLIENTE GENÉRICO
        // =========================================================

        private static void EscribirClienteGenerico(
            Utf8JsonWriter writer,
            GetGestionDocumentoResponseDto value)
        {
            /*
             * ESTE BLOQUE SE UTILIZA PARA
             * CLIENTES QUE TODAVÍA NO TIENEN
             * UNA CONFIGURACIÓN ESPECÍFICA.
             */

            EscribirString(
                writer,
                "tramo",
                value.tramo
            );


            writer.WriteNumber(
                "nro",
                value.nro
            );


            EscribirString(
                writer,
                "numeroDocumento",
                value.numeroDocumento
            );


            EscribirString(
                writer,
                "estado",
                value.estado
            );


            EscribirString(
                writer,
                "fechaVencimiento",
                value.fechaVencimiento
            );


            EscribirString(
                writer,
                "siglaMoneda",
                value.siglaMoneda
            );


            EscribirDecimal(
                writer,
                "importeTotal",
                value.importeTotal
            );


            EscribirDecimal(
                writer,
                "importeSaldo",
                value.importeSaldo
            );


            writer.WriteNumber(
                "diasAtrazo",
                value.diasAtrazo
            );


            EscribirString(
                writer,
                "gestorCall",
                value.gestorCall
            );
        }


        // =========================================================
        // MÉTODOS AUXILIARES
        // =========================================================

        private static void EscribirString(
            Utf8JsonWriter writer,
            string nombrePropiedad,
            string? valor)
        {
            /*
             * SI EL VALOR ES NULL
             * LA PROPIEDAD NO APARECE EN EL JSON.
             */

            if (valor != null)
            {
                writer.WriteString(
                    nombrePropiedad,
                    valor
                );
            }
        }


        private static void EscribirDecimal(
            Utf8JsonWriter writer,
            string nombrePropiedad,
            decimal? valor)
        {
            if (valor.HasValue)
            {
                writer.WriteNumber(
                    nombrePropiedad,
                    valor.Value
                );
            }
        }
    }
}