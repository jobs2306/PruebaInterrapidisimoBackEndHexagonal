# BackEnd – Registro de Materias de Estudiantes  
## Prueba Técnica – Interrapidísimo

Este proyecto corresponde al **BackEnd de una prueba técnica** desarrollada para la empresa **Interrapidísimo**.  
La aplicación está orientada a la **gestión y registro de materias de estudiantes que se encuentren en sesión activa**, aplicando buenas prácticas de desarrollo y una **arquitectura hexagonal (Ports & Adapters)**.

---

## Descripción General

El sistema expone una **API REST** que permite:

- Registrar estudiantes en el sistema.
- Inicio de sesión.
- Registrar materias únicamente para estudiantes que se encuentren **en sesión**.
- Aplicar reglas de negocio desacopladas de la infraestructura.
- Facilitar la mantenibilidad, escalabilidad y testabilidad del backend.

El proyecto fue diseñado con un enfoque profesional, priorizando claridad estructural, separación de responsabilidades y facilidad de evolución.

---

## Arquitectura

La solución está construida bajo el enfoque de **Arquitectura Hexagonal**, lo que permite aislar la lógica de negocio de los detalles técnicos.

### Capas principales:

- **Dominio**  
  Contiene las entidades y reglas de negocio centrales del sistema.

- **Aplicación**  
  Implementa los casos de uso y coordina la interacción entre el dominio y los adaptadores.
  Implementa patrón de diseño Mediator para la ejecución de commands y queries.

- **Infraestructura**  
  Incluye los controladores HTTP, persistencia de datos, configuración de Swagger y demás detalles técnicos.

Este enfoque garantiza que el dominio sea independiente de frameworks, bases de datos o tecnologías externas.

---

## Despliegue

El backend se encuentra **desplegado en una VPS Linux**, ejecutándose en un entorno de producción real.
La **VPS** usa **Nginx** para que las peticiones se redirijan al servicio correcto.
El sitio web en el que se despliega usa **HTTPS** para que la comunicación sea segura.

### Documentación Swagger

La documentación interactiva de la API se encuentra disponible en la siguiente URL:

https://apihexregistroestudiantes.joelflow.com/swagger/index.html

Desde Swagger es posible:
- Explorar todos los endpoints disponibles.
- Probar las operaciones de la API.
- Visualizar los modelos de datos expuestos.

---

## Tecnologías Utilizadas

- ASP.NET Core (.NET)
- Arquitectura Hexagonal (Ports & Adapters)
- API REST
- Swagger / OpenAPI
- Despliegue en VPS Linux mediante CI/CD
- Control de versiones con Git

---

## Objetivo de la Prueba Técnica

Esta prueba técnica tiene como objetivo evaluar:

- Capacidad de análisis y modelado del problema.
- Aplicación de buenas prácticas de desarrollo backend.
- Uso correcto de una arquitectura limpia y desacoplada.
- Organización clara del código y facilidad de mantenimiento.

---

## Autor

**Ing. Joel Baena**  
Backend Developer – .NET  

Proyecto desarrollado como prueba técnica para **Interrapidísimo**.
