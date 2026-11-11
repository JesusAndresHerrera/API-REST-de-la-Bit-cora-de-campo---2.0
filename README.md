# Bitacora de campo - API REST Segura - Tareas 2.2 y 2.4

API REST desarrollada en C# y ASP.NET Core para la gestion de registros de campo (con titulo, descripcion y fotografia en Base64). El sistema cuenta con autenticacion mediante JWT, autorizacion basada en roles (Admin y Usuario), contrasenas hasheadas con BCrypt y despliegue en la nube con HTTPS.

El proyecto sigue los principios de Clean Architecture y esta preparado para ser consumido por la aplicacion movil Android (Tarea 2.3).

## Entregables y documentacion

- [Informe PDF de la tarea 2.2](<Informe Investigacion y Practica con Postman.pdf>).
- [Coleccion de Postman](postman/Bitacora-2.2.postman_collection.json).
- [Resultados de las pruebas](postman/RESULTADOS.md).
- [Diagrama relacional del modelo](docs/modelo/Diagrama_Relacional_Bitacora_Campo.png).

(Nota: Puedes agregar aqui los enlaces a los nuevos PDFs o capturas de la Tarea 2.4 si los tienes).

## Tecnologias Utilizadas

- Backend: .NET 10 (C#) / ASP.NET Core
- Base de datos: SQLite + Entity Framework Core
- Seguridad: JWT (JSON Web Tokens), BCrypt para hasheo de contrasenas, autenticacion por roles.
- Despliegue: AWS EC2 (Windows Server) + IIS + HTTPS.
- Pruebas: Postman (Pruebas de integracion y seguridad).

## Arquitectura del Proyecto

| Proyecto | Responsabilidad | Referencias |
| --- | --- | --- |
| Bitacora.Domain | Entidades (Registro, Fotografia, Usuario) y reglas de validacion (RegistroReglas). | Ninguna. |
| Bitacora.Application | DTOs, servicios de negocio (RegistroService, AuthService) e interfaces de repositorios. | Bitacora.Domain. |
| Bitacora.Infrastructure | Implementacion de repositorios, BitacoraDbContext, configuracion de SQLite y migraciones. | Bitacora.Application y Bitacora.Domain. |
| Bitacora.Api | Controladores HTTP (Registros, Auth), configuracion de JWT, inyeccion de dependencias y manejo global de errores. | Bitacora.Application y Bitacora.Infrastructure. |

Flujo de una peticion:
Cliente (Android/Postman) -> Controlador HTTP -> Servicio (Application) -> Repositorio (Infrastructure) -> EF Core -> SQLite

## Como ejecutar el proyecto localmente

1. Restaurar dependencias:
   ```cmd
   dotnet restore Bitacora.sln

2. Configurar la clave secreta de JWT localmente:
   ```cmd
   dotnet user-secrets set "Jwt:Key" "xxxxxxxx" --project src/Bitacora.Api

   Nota: La clave JWT sera compartira en un archivo de texto en Canvas UTH

3.  Ejecutar la API:
   ```cmd
   dotnet run --project src\Bitacora.Api

## Probar la API

Navega a http://localhost:5106/swagger para probar los endpoints.
