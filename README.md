# API de Envío de Correos

Esta aplicación es una **API REST** desarrollada en **.NET 8.0** para el envío de correos electrónicos a uno o más destinatarios. 

## Descripción

La API permite el envío de correos electrónicos con contenido dinámico y soporte para múltiples destinatarios. El mensaje se puede personalizar a través del cuerpo del correo y los destinatarios. Además, la configuración de la aplicación se puede ajustar para adaptarse a tus necesidades, como el puerto de despliegue y la configuración de la base de datos.

## Características

- **Envío de correos electrónicos a 1 o más destinatarios**. (ver el Postman agregado para la plantilla del body)
- **Configuración flexible** para el Remitente a travez de los endpoints.
- **Base de datos SQL Server** para la configuración del servidor SMTP.
- **Desarrollado en .NET 8.0**.

## Requisitos

- **.NET 8.0**: Asegúrate de tener instalada la versión correcta de .NET en tu máquina. Puedes descargarla desde [aquí](https://dotnet.microsoft.com/download/dotnet).
- **SQL Server**: La aplicación usa una base de datos SQL Server para almacenar la configuración del servidor SMTP.

## Configuración

### Base de Datos

La aplicación utiliza una base de datos SQL Server (v20.2) para almacenar la configuración relacionada con el servidor SMTP (como el host, puerto, usuario y contraseña). Asegúrate de tener una instancia de SQL Server disponible y realizar la configuración adecuada en la base de datos.

La descripcion de la tabla es :

CREATE TABLE [SmtpSettings] (
    [Id] int NOT NULL IDENTITY,
    [Host] nvarchar(max) NOT NULL,
    [Port] int NOT NULL,
    [Username] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [UseSSL] bit NOT NULL,
    [FromEmail] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SmtpSettings] PRIMARY KEY ([Id])
);


### AppSettings.json


el appsettings  contiene la configuracion inicial de la aplicacion, donde se debe indicar la ubicacion de la base de datos, y el puerto donde despliega la aplicacion:

{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "email_db": "Server=localhost\\SQLEXPRESS;Database=email_sender_db;TrustServerCertificate=True;Trusted_Connection=True;"
  },
  "Urls": "http://localhost:8081"     //Puerto de la app es lo unico que se configura. Manteniendo el http:servidor:puerto . **SOLO MODIFICAR EL CAMPO PUERTO** 
}


### Funcionamiento del API

#### SMTP CONFIG
El Api cuenta con rutas para realizar las operaciones de CRUD para el  **EMAIL SENDER** que son los datos de configuracion del SMTP. 
se puede crear email sender, modificar, listar y eliminar por identificador unico. Importante destacar que **LA API SELECCIONA EL PRIMER REGISTRO DE LA TABLA SMTPSETTINGS PARA SELECCIONAR EL REMITENTE**

Los datos de los Json estan especificados en el servicio de **SWAGGER** configurado.

#### API enviar correo.

El Api para enviar correos recibe una peticion al end point envio correo:[{{server}}/api/email/sendMail] el cuerpo de esta peticion es :


{
  "to": "correo@correo.com", // debe indicar solo correos validos, y si es una lista deberia ser indicada en formato : "Correo1@correo.com;correo2@correo.com" **(separados por ";")**
  "subject": "Test API Email Sender", // el Asunto del correo
  "body": "Hello World" // el cuerpo del correo, el mensaje que se envia. Puede venir en formato HTML
}

Si almenos 1 de los correos no es valido, no se envia a ningun remitente. 


Ademas puedes configurar un smtp a travez de los endpoints de settings:

**Crear un smtp** HTTP POST [CrearEmailSender]({{server}}/api/email/settings/create) 
{
    "host": "smtp.correo.com",
    "port": 465, //si es ssl usar el puerto seguro del proveedor de correo.
    "username": "correo@correo.com",
    "password": "clave_de_usuario_o_aplicacion",
    "useSSL": true, //esto depende del puerto, si es SSL dejar en true.
    "fromEmail": "correo@correo.com"
}

PD: siempre que se cree un nuevo SMTP settigns se usara el ultimo registrado.

**Listar los SMTP configurados** HTTP GET [listadeEmailSenders]({{server}}/api/email/settings/list]) 

Se registra una lista de SMTP para llevar un historial de los smtp configurados. ya que el servicio de envio de correos tomara el ultimo configurado. (el campo Password se muestra cifrado)
[
    {
        "id": 1,
        "host": "smtp.correo.com",
        "port": 465,
        "username": "correo@correo.com",
        "password": "clave_cifrada",
        "useSSL": true,
        "fromEmail": "correo@correo.com",
        "createdAt": "YYYY-MM-DD HH:MM:SS"
    }
]


**Eliminar un SMTP** HTTP: DELETE  [EliminarEmailSender]({{server}}/api/email/settings/delete/{id}) 

<div style="margin-top: 50px; text-align: center; font-size: 12px;">
    <hr>
    <p><strong>Desarrollado por Andrés Alejo</strong></p>
    <p>Marzo de 2025</p>
    <hr>
    <p>[Visita mi GitHub](https://github.com/aalejog09)</p>
    <p>© 2025. Todos los derechos reservados.</p>
</div>