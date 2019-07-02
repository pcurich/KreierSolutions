using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ks.Batch.Util
{
    public static class LogMessages
    {
        public static String BatchReadOk        = "Lectura de configuracion leido correctamente";
        public static String BatchReadError     = "Lectura de configuracion con problemas de lectura debido a : {0}";

        public static String BatchStartOk       = "El Servicio {0} a iniciado correctamente";
        public static String BatchStartError    = "El Servicio {0} no ha podidio iniciar correctamente debido al siguiente error: {1}";

        public static String BatchStopOk        = "El Servicio {0} se ha detenido correctamente";
        public static String BatchStopError     = "El Servicio {0} no se ha detenido correctamente debido al siguiente error: {1}";

        public static String BatchPauseOk       = "El Servicio {0} se ha pausado correctamente";
        public static String BatchPauseError    = "El Servicio {0} no se ha pausado correctamente debido al siguiente error: {1}";

        public static String BatchContinueOk = "El Servicio {0} se ha detenido correctamente";
        public static String BatchContinueError = "El Servicio {0} no se ha detenido correctamente debido al siguiente error: {1}";
    }
}
