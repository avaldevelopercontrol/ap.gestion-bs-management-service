namespace GesMgmt.Application.Utils
{
    public class PayUtils
    {
        public static DateTime CalculaFechaFin(
            DateTime fechaInicio,
            string intervaloFrecuenciaCobro,
            int cantidadPorIntervalo,
            int numeroDeCuotas)
        {
            if (string.IsNullOrWhiteSpace(intervaloFrecuenciaCobro))
            {
                throw new ArgumentException("El intervalo de frecuencia es obligatorio.", nameof(intervaloFrecuenciaCobro));
            }

            if (cantidadPorIntervalo <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cantidadPorIntervalo), "La cantidad por intervalo debe ser mayor que cero.");
            }

            if (numeroDeCuotas <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numeroDeCuotas), "El número de cuotas debe ser mayor que cero.");
            }

            // La fecha fin es la fecha de la ultima cuota: se suman (N-1) intervalos.
            int saltos = numeroDeCuotas - 1;
            string frecuencia = intervaloFrecuenciaCobro.Trim().ToUpperInvariant();

            return frecuencia switch
            {
                "SEMANAL" => fechaInicio.AddDays(7 * cantidadPorIntervalo * saltos),
                "MENSUAL" => fechaInicio.AddMonths(1 * cantidadPorIntervalo * saltos),
                "BIMESTRAL" => fechaInicio.AddMonths(2 * cantidadPorIntervalo * saltos),
                "TRIMESTRAL" => fechaInicio.AddMonths(3 * cantidadPorIntervalo * saltos),
                "SEMESTRAL" => fechaInicio.AddMonths(6 * cantidadPorIntervalo * saltos),
                "ANUAL" => fechaInicio.AddYears(1 * cantidadPorIntervalo * saltos),
                _ => throw new ArgumentException(
                    "Frecuencia no valida. Use: SEMANAL, MENSUAL, BIMESTRAL, TRIMESTRAL, SEMESTRAL o ANUAL.",
                    nameof(intervaloFrecuenciaCobro))
            };
        }

        public static List<DateTime> ObtenerFechasCobro(
            DateTime fechaInicio,
            string intervaloFrecuenciaCobro,
            int cantidadPorIntervalo,
            int numeroDeCuotas)
        {
            if (string.IsNullOrWhiteSpace(intervaloFrecuenciaCobro))
            {
                throw new ArgumentException("El intervalo de frecuencia es obligatorio.", nameof(intervaloFrecuenciaCobro));
            }

            if (cantidadPorIntervalo <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(cantidadPorIntervalo), "La cantidad por intervalo debe ser mayor que cero.");
            }

            if (numeroDeCuotas <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numeroDeCuotas), "El número de cuotas debe ser mayor que cero.");
            }

            string frecuencia = intervaloFrecuenciaCobro.Trim().ToUpperInvariant();
            var fechas = new List<DateTime>(numeroDeCuotas);

            for (int i = 0; i < numeroDeCuotas; i++)
            {
                DateTime fecha = frecuencia switch
                {
                    "SEMANAL" => fechaInicio.AddDays(7 * cantidadPorIntervalo * i),
                    "MENSUAL" => fechaInicio.AddMonths(1 * cantidadPorIntervalo * i),
                    "BIMESTRAL" => fechaInicio.AddMonths(2 * cantidadPorIntervalo * i),
                    "TRIMESTRAL" => fechaInicio.AddMonths(3 * cantidadPorIntervalo * i),
                    "SEMESTRAL" => fechaInicio.AddMonths(6 * cantidadPorIntervalo * i),
                    "ANUAL" => fechaInicio.AddYears(1 * cantidadPorIntervalo * i),
                    _ => throw new ArgumentException(
                        "Frecuencia no valida. Use: SEMANAL, MENSUAL, BIMESTRAL, TRIMESTRAL, SEMESTRAL o ANUAL.",
                        nameof(intervaloFrecuenciaCobro))
                };

                fechas.Add(fecha);
            }

            return fechas;
        }
    }
}
