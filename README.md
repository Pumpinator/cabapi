# CAB API - Sistema de Clasificación Automática de Basura

## Descripción
API REST para el sistema de gestión y clasificación inteligente de residuos de la Universidad Tecnológica de León.

## Autenticación
La API utiliza JWT (JSON Web Tokens) para autenticación. Incluye el token en el header `Authorization: Bearer {token}`.

## Endpoints

### 🔐 Autenticación (`/api/usuarios`)

#### POST `/api/usuarios/login`
Iniciar sesión
```json
{
  "correo": "admin@utleon.edu.mx",
  "password": "password"
}
```

#### POST `/api/usuarios/register`
Registrar nuevo usuario
```json
{
  "nombre": "Juan Pérez",
  "correo": "juan@utleon.edu.mx",
  "password": "password123"
}
```

#### GET `/api/usuarios/profile`
Obtener perfil del usuario autenticado (🔒 Requiere autenticación)

#### POST `/api/usuarios/change-password`
Cambiar contraseña (🔒 Requiere autenticación)
```json
{
  "passwordActual": "password123",
  "passwordNueva": "newpassword123"
}
```

#### GET `/api/usuarios` (🔒 Solo Administradores)
Obtener todos los usuarios

#### GET `/api/usuarios/{id}` (🔒 Usuario propio o Administrador)
Obtener usuario por ID

#### PUT `/api/usuarios/{id}` (🔒 Usuario propio o Administrador)
Actualizar usuario

#### DELETE `/api/usuarios/{id}` (🔒 Solo Administradores)
Desactivar usuario

---

### 📊 Detecciones (`/api/detecciones`)

#### GET `/api/detecciones` (🔒 Solo Administradores)
Obtener todas las detecciones

#### GET `/api/detecciones/{id}` (🔒 Requiere autenticación)
Obtener detección por ID

#### POST `/api/detecciones` (🔒 Requiere autenticación)
**RF06 - Crear nueva detección desde app móvil**
```json
{
  "tipo": "Organico",
  "clasificadorId": 1
}
```

#### PUT `/api/detecciones/{id}` (🔒 Solo Administradores)
Actualizar detección

#### DELETE `/api/detecciones/{id}` (🔒 Solo Administradores)
Eliminar detección

#### GET `/api/detecciones/estadisticas` (🔒 Requiere autenticación)
**RF07 - Estadísticas generales de reciclaje**
Retorna estadísticas completas incluyendo:
- Total de detecciones
- Detecciones por tipo
- Detecciones por zona
- Detecciones por hora
- Tendencia mensual

#### GET `/api/detecciones/estadisticas/zonas` (🔒 Requiere autenticación)
**RF11 - Gráfico de zonas donde más se tira basura**

#### GET `/api/detecciones/estadisticas/tipos-populares` (🔒 Requiere autenticación)
**RF12 - Gráfico de tipos de basura más populares**

#### GET `/api/detecciones/estadisticas/horarios-recurrentes` (🔒 Requiere autenticación)
**RF13 - Gráfico de horarios más recurrentes**

#### POST `/api/detecciones/{id}/confirmar` (🔒 Requiere autenticación)
**RF18 - Retroalimentación para mejora continua de IA**
```json
{
  "esCorrecta": false,
  "tipoCorregido": "Valorizable"
}
```

#### GET `/api/detecciones/por-zona/{zonaId}` (🔒 Requiere autenticación)
Obtener detecciones por zona

#### GET `/api/detecciones/recientes?limit=10` (🔒 Requiere autenticación)
Obtener detecciones recientes

---

### 🗺️ Clasificadores (`/api/clasificadores`)

#### GET `/api/clasificadores` (🔒 Requiere autenticación)
Obtener todos los clasificadores

#### GET `/api/clasificadores/{id}` (🔒 Requiere autenticación)
Obtener clasificador por ID

#### POST `/api/clasificadores` (🔒 Solo Administradores)
Crear nuevo clasificador
```json
{
  "nombre": "Entrada Principal",
  "latitud": 21.0635822,
  "longitud": -101.5803752,
  "zonaId": 5
}
```

#### PUT `/api/clasificadores/{id}` (🔒 Solo Administradores)
Actualizar clasificador

#### DELETE `/api/clasificadores/{id}` (🔒 Solo Administradores)
Eliminar clasificador

#### POST `/api/clasificadores/mas-cercano` (🔒 Requiere autenticación)
**RF09 - Encontrar clasificador más cercano**
```json
{
  "latitud": 21.0635822,
  "longitud": -101.5803752
}
```

#### GET `/api/clasificadores/por-zona/{zonaId}` (🔒 Requiere autenticación)
Obtener clasificadores por zona

#### GET `/api/clasificadores/estadisticas` (🔒 Requiere autenticación)
Estadísticas de uso de clasificadores

---

### 🏢 Zonas (`/api/zonas`)

#### GET `/api/zonas` (🔒 Requiere autenticación)
Obtener todas las zonas

#### GET `/api/zonas/{id}` (🔒 Requiere autenticación)
Obtener zona por ID

#### POST `/api/zonas` (🔒 Solo Administradores)
Crear nueva zona
```json
{
  "nombre": "Edificio G"
}
```

#### PUT `/api/zonas/{id}` (🔒 Solo Administradores)
Actualizar zona

#### DELETE `/api/zonas/{id}` (🔒 Solo Administradores)
Eliminar zona

#### GET `/api/zonas/estadisticas` (🔒 Requiere autenticación)
Estadísticas por zona

#### GET `/api/zonas/{id}/detecciones` (🔒 Requiere autenticación)
Obtener detecciones de una zona específica

#### GET `/api/zonas/{id}/clasificadores` (🔒 Requiere autenticación)
Obtener clasificadores de una zona específica

---

### 📱 Contenido (`/api/contenido`)

#### GET `/api/contenido/tips?tipo=Organico` (🔒 Requiere autenticación)
**RF08 - Tips y recordatorios para clasificación**

#### GET `/api/contenido/tips/aleatorio` (🔒 Requiere autenticación)
Obtener tip aleatorio

#### GET `/api/contenido/campanas?activas=true`
**RF14 - Catálogo de campañas con el CUPA**

#### GET `/api/contenido/campanas/{id}`
Obtener campaña específica

#### GET `/api/contenido/campanas/activas`
Obtener solo campañas activas

#### POST `/api/contenido/imagenes/upload` (🔒 Requiere autenticación)
**RF06 - Captura y procesamiento de imagen desde app**
```
Content-Type: multipart/form-data
imagen: [archivo de imagen]
clasificadorId: 1 (opcional)
latitud: 21.0635822 (opcional)
longitud: -101.5803752 (opcional)
```

---

### 📈 Reportes (`/api/reportes`) (🔒 Solo Administradores)

#### GET `/api/reportes/impacto-ambiental?mes=7&ano=2025`
**RF19 - Reportes de impacto ambiental**
Genera reporte mensual con:
- Reducción estimada de CO2
- Peso de residuos procesados
- Desglose por tipo y zona
- Comparativo con mes anterior
- Proyección anual

#### GET `/api/reportes/dashboard`
**RF10 - Panel de administración**
Dashboard con métricas en tiempo real

#### GET `/api/reportes/exportar/{tipo}?mes=7&ano=2025`
Exportar reportes en diferentes formatos

#### GET `/api/reportes/estadisticas-avanzadas?fechaInicio=2025-07-01&fechaFin=2025-07-31`
Estadísticas avanzadas con predicciones y recomendaciones

---

## Códigos de Estado HTTP

- `200 OK` - Solicitud exitosa
- `201 Created` - Recurso creado exitosamente
- `204 No Content` - Operación exitosa sin contenido
- `400 Bad Request` - Datos de entrada inválidos
- `401 Unauthorized` - No autenticado
- `403 Forbidden` - Sin permisos
- `404 Not Found` - Recurso no encontrado
- `500 Internal Server Error` - Error del servidor

## Roles de Usuario

- **Usuario**: Acceso básico a funcionalidades de la app móvil
- **Administrador**: Acceso completo a todas las funcionalidades

## Tipos de Residuos Soportados

- `Organico` - Residuos orgánicos biodegradables
- `Valorizable` - Materiales reciclables (plástico, metal, papel)
- `No Valorizable` - Residuos no reciclables

## Ejemplos de Uso

### Flujo completo de la app móvil:

1. **Registro/Login**
```bash
POST /api/usuarios/register
```

2. **Subir imagen para clasificación**
```bash
POST /api/contenido/imagenes/upload
```

3. **Registrar detección**
```bash
POST /api/detecciones
```

4. **Obtener clasificador más cercano**
```bash
POST /api/clasificadores/mas-cercano
```

5. **Ver estadísticas personales**
```bash
GET /api/detecciones/estadisticas
```

6. **Obtener tips**
```bash
GET /api/contenido/tips/aleatorio
```

### Panel de administración web:

1. **Dashboard principal**
```bash
GET /api/reportes/dashboard
```

2. **Estadísticas por zona**
```bash
GET /api/detecciones/estadisticas/zonas
```

3. **Reporte de impacto**
```bash
GET /api/reportes/impacto-ambiental
```

4. **Gestión de usuarios**
```bash
GET /api/usuarios
```

## Notas de Implementación

- **RF15**: La API REST permite comunicación entre IA, app móvil y sistema web
- **RF16**: La base de datos almacena usuarios y clasificaciones históricas
- **RF17**: Se recomienda implementar respaldos automáticos diarios
- **RF18**: Sistema de retroalimentación implementado para mejora continua del modelo de IA

## Configuración

Asegúrate de configurar las siguientes variables en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "tu_cadena_de_conexion"
  },
  "JwtSettings": {
    "SecretKey": "tu_clave_secreta_muy_larga",
    "Issuer": "CAB-API",
    "Audience": "CAB-Client",
    "ExpiryDays": 7
  }
}
```
