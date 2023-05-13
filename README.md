# Oh my dog!
 Proyecto "Oh my dog!" correspondiente a la materia Ingenieria de software 2 - UNLP.

| <image src="https://www.info.unlp.edu.ar/wp-content/uploads/2020/01/cropped-cabeceraweb.jpg" alt="Logo - Facultad de informática UNLP" width="300px"> | <image src="https://github.com/thiago-laurence/Oh-my-dog/assets/42177696/0674a9f4-d396-4c36-b289-db5dd85e8ca0" alt="Logo - Oh my dog!" width="100px"> |
| ------ | -------- |
---

#### Tecnologias utilizadas:
| <image src="https://upload.wikimedia.org/wikipedia/commons/e/ee/.NET_Core_Logo.svg" alt="C#" width="50px"> | <image src="https://cdn.cdnlogo.com/logos/c/68/c-sharp-800x800.png" alt="C#" width="50px"> | <image src="https://upload.wikimedia.org/wikipedia/commons/9/99/Unofficial_JavaScript_logo_2.svg" alt="JavaScript" width="50px"> |
| ------ | ------ | ------ |

## IDE y Servidor
***
> Microsoft SQL Server Management Studio Version 19.0

> Visual Studio 2022

## Instalacion dependencias con Nuget
***
> Microsoft.EntityFrameworkCore.SqlServer
> Microsoft.EntityFrameworkCore.Tools

## Comando para conexion con la BD
***
- Herramientas--> Administrador de paquetes Nuget --> Consola

> Scaffold-DbContext "server=localhost; database=ohmydogdb; integrated security=true; Encrypt=False;" Microsoft.EntityFrameworkCore.SqlServer -OutPutDir Models

--> appsettings.json
'''
"ConnectionStrings": {
    "conexion": "server=localhost; database=ohmydogdb; integrated security=true; Encrypt=False;"
  }
'''

--> OhmydogdbContex.cs
'''
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) { 
		//Comentar lo de esta linea            
        }
    }
'''

--> Program.cs
Antes de var app = builder.Build(); Agregar lo siguiente
'''
builder.Services.AddDbContext<OhmydogdbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));
'''



