using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoingOutMobile.Models.Restaurant
{
    public class StatusCode
    {

        public string GetStatusDescription(int statusCode)
        {
            return statusCode switch
            {
                // Informativos
                100 => "100 Continue: El cliente debe continuar con su solicitud.",
                101 => "101 Switching Protocols: El servidor está cambiando de protocolo según lo solicitado por el cliente.",
                102 => "102 Processing: El servidor ha recibido la solicitud y la está procesando.",

                // Éxito
                200 => "200 OK: La solicitud ha tenido éxito.",
                201 => "201 Created: La solicitud ha tenido éxito y se ha creado un nuevo recurso.",
                202 => "202 Accepted: La solicitud ha sido aceptada para procesamiento, pero aún no se ha completado.",
                204 => "204 No Content: La solicitud ha tenido éxito, pero no hay contenido para enviar en la respuesta.",

                // Redirección
                301 => "301 Moved Permanently: El recurso solicitado ha sido movido permanentemente a una nueva URL.",
                302 => "302 Found: El recurso solicitado está disponible temporalmente en una nueva URL.",
                304 => "304 Not Modified: El recurso no ha sido modificado desde la última solicitud.",

                // Errores del cliente
                400 => "400 Bad Request: La solicitud no pudo ser entendida o fue malformada.",
                401 => "401 Unauthorized: La solicitud requiere autenticación.",
                403 => "403 Forbidden: El servidor entiende la solicitud, pero se niega a autorizarla.",
                404 => "404 Not Found: El recurso solicitado no pudo ser encontrado.",
                405 => "405 Method Not Allowed: El método de solicitud no está permitido para el recurso solicitado.",

                // Errores del servidor
                500 => "500 Internal Server Error: Se ha producido un error interno en el servidor.",
                501 => "501 Not Implemented: El servidor no reconoce el método de solicitud o carece de la capacidad para cumplirla.",
                503 => "503 Service Unavailable: El servidor no está disponible temporalmente, generalmente debido a mantenimiento o sobrecarga.",

                // Código de estado HTTP 418 (broma de RFC 2324)
                418 => "418 I'm a teapot: Soy una tetera (broma de RFC 2324).",

                // Predeterminado
                _ => $"Código de estado desconocido: {statusCode}"
            };
        }


    }
}
