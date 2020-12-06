using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ks.Batch.Util
{
    public static class LogMessages
    {
        public static string BatchReadOk        = "Lectura de configuracion leido correctamente";
        public static string BatchReadError     = "Lectura de configuracion con problemas de lectura debido a : {0}";

        public static string BatchStart         = "El servicio {0} ha iniciado";
        public static string BatchStartOk       = "El Servicio {0} ha iniciado correctamente";
        public static string BatchStartError    = "El Servicio {0} no ha podidio iniciar correctamente debido al siguiente error: {1}";

        public static string BatchStopOk        = "El Servicio {0} se ha detenido correctamente";
        public static string BatchStop          = "El servicio {0} se ha detenido";
        public static string BatchStopError     = "El Servicio {0} no se ha detenido correctamente debido al siguiente error: {1}";

        public static string BatchPauseOk       = "El Servicio {0} se ha pausado correctamente";
        public static string BatchPause         = "El servicio {0} se ha pausado";
        public static string BatchPauseError    = "El Servicio {0} no se ha pausado correctamente debido al siguiente error: {1}";

        public static string BatchContinueOk    = "El Servicio {0} se ha pausado correctamente";
        public static string BatchContinue      = "El servicio {0} se ha pausado";
        public static string BatchContinueError = "El Servicio {0} no se ha pausado correctamente debido al siguiente error: {1}";

        public static string BatchWaitFile = "El servicio se encuentra a la espera de su activacion por archivo";
        public static string BatchScheduler = "El servicio encontrado es: {0} con parametria: {1}";
        public static string BatchScheduleRevert = "Periodo a revertir {0}";

        public static string BatchReportDelete = "Borrado de reporte {0} en el periodo {1}";


        public static string DataSize = "Registros encontrados {0}";

        public static string FindInfoReport = "Reporte a buscar : {0}";
        public static string FindInfoReportResult = "Cantidad de datos encontrados a revertir : {0}"; 
        
        

    }
}
