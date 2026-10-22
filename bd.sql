-- =====================================================================
-- ESQUEMA CONSOLIDADO POSTGRESQL (bd.sql) - IESTP ARGENTINA
-- Base de datos configurada: postgresql://postgres:gfDnA*xx.B28wdB@db.oxmjxyywxksuwrcgewdg.supabase.co:5432/postgres
-- =====================================================================

DO $$ BEGIN
    CREATE TYPE public.estado_general AS ENUM ('Activo', 'Inactivo');
EXCEPTION WHEN duplicate_object THEN null; END $$;

DO $$ BEGIN
    CREATE TYPE public.estado_bien AS ENUM ('Bueno', 'Regular', 'Malo', 'En reparación', 'De baja');
EXCEPTION WHEN duplicate_object THEN null; END $$;

DO $$ BEGIN
    CREATE TYPE public.estado_asignacion AS ENUM ('Vigente', 'Finalizada', 'Anulada');
EXCEPTION WHEN duplicate_object THEN null; END $$;

CREATE TABLE IF NOT EXISTS public.personas (
    id_persona integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    tipo_documento varchar(10) NOT NULL,
    numero_documento varchar(20) NOT NULL UNIQUE,
    nombres varchar(100) NOT NULL,
    apellidos varchar(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS public.roles (
    id_rol integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre varchar(50) NOT NULL UNIQUE,
    descripcion text,
    estado varchar(15) DEFAULT 'Activo' NOT NULL,
    creado_el timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS public.usuarios (
    id_usuario integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_persona integer NOT NULL UNIQUE REFERENCES public.personas(id_persona) ON DELETE RESTRICT,
    dni_login varchar(20) NOT NULL UNIQUE,
    correo varchar(100) NOT NULL UNIQUE,
    clave_cifrada varchar(255),
    estado varchar(15) DEFAULT 'Activo' NOT NULL,
    ultimo_acceso timestamp without time zone,
    creado_el timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    actualizado_el timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS public.usuarios_roles (
    id_usuario integer NOT NULL REFERENCES public.usuarios(id_usuario) ON DELETE CASCADE,
    id_rol integer NOT NULL REFERENCES public.roles(id_rol) ON DELETE RESTRICT,
    estado varchar(15) DEFAULT 'Activo' NOT NULL,
    fecha_asignacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_usuario, id_rol)
);

CREATE TABLE IF NOT EXISTS public.permisos_modulos (
    id_permiso integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre_modulo varchar(100) NOT NULL UNIQUE,
    nivel_acceso varchar(20) NOT NULL,
    descripcion text
);

CREATE TABLE IF NOT EXISTS public.roles_permisos (
    id_rol integer NOT NULL REFERENCES public.roles(id_rol) ON DELETE CASCADE,
    id_permiso integer NOT NULL REFERENCES public.permisos_modulos(id_permiso) ON DELETE CASCADE,
    PRIMARY KEY (id_rol, id_permiso)
);

CREATE TABLE IF NOT EXISTS public.sesiones_login (
    id_sesion uuid DEFAULT gen_random_uuid() PRIMARY KEY,
    id_usuario integer NOT NULL REFERENCES public.usuarios(id_usuario) ON DELETE CASCADE,
    metodo_autenticacion varchar(20) NOT NULL,
    ip_origen varchar(45),
    fecha_inicio timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    fecha_expiracion timestamp without time zone NOT NULL,
    estado varchar(15) DEFAULT 'Activa' NOT NULL
);

CREATE TABLE IF NOT EXISTS public.especialidad (
    id_especialidad integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre varchar(100) NOT NULL,
    estado public.estado_general DEFAULT 'Activo' NOT NULL
);

CREATE TABLE IF NOT EXISTS public.almacen (
    id_almacen integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_especialidad integer NOT NULL REFERENCES public.especialidad(id_especialidad),
    nombre varchar(100) NOT NULL,
    ubicacion varchar(150),
    responsable varchar(100),
    estado public.estado_general DEFAULT 'Activo' NOT NULL
);

CREATE TABLE IF NOT EXISTS public.ambiente (
    id_ambiente integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_especialidad integer NOT NULL REFERENCES public.especialidad(id_especialidad),
    nombre varchar(100) NOT NULL,
    estado public.estado_general DEFAULT 'Activo' NOT NULL
);

CREATE TABLE IF NOT EXISTS public.bien (
    id_bien integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_almacen integer NOT NULL REFERENCES public.almacen(id_almacen),
    cbi varchar(30) UNIQUE,
    descripcion varchar(150) NOT NULL,
    tipo_bien varchar(50) NOT NULL,
    marca varchar(50),
    modelo varchar(50),
    serie varchar(50) UNIQUE,
    procedencia varchar(50),
    fecha_ingreso date DEFAULT CURRENT_DATE NOT NULL,
    estado public.estado_bien DEFAULT 'Bueno' NOT NULL,
    observacion text
);

CREATE TABLE IF NOT EXISTS public.asignacion (
    id_asignacion integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_bien integer NOT NULL REFERENCES public.bien(id_bien),
    id_ambiente integer NOT NULL REFERENCES public.ambiente(id_ambiente),
    responsable varchar(100) NOT NULL,
    fecha_asignacion date DEFAULT CURRENT_DATE NOT NULL,
    fecha_fin date,
    estado public.estado_asignacion DEFAULT 'Vigente' NOT NULL,
    observacion text
);

CREATE TABLE IF NOT EXISTS public.transferencia (
    id_transferencia integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_bien integer NOT NULL REFERENCES public.bien(id_bien),
    id_ambiente_origen integer NOT NULL REFERENCES public.ambiente(id_ambiente),
    id_ambiente_destino integer NOT NULL REFERENCES public.ambiente(id_ambiente),
    fecha date DEFAULT CURRENT_DATE NOT NULL,
    motivo varchar(150) NOT NULL,
    dni_responsable char(8) NOT NULL,
    observacion text
);

CREATE TABLE IF NOT EXISTS public.inventario_anual (
    id_inventario integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_bien integer NOT NULL REFERENCES public.bien(id_bien),
    anio smallint NOT NULL,
    fecha_verificacion date DEFAULT CURRENT_DATE NOT NULL,
    resultado varchar(50) NOT NULL,
    observacion text
);

CREATE TABLE IF NOT EXISTS public.incidencias (
    id_incidencia integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_bien integer NOT NULL REFERENCES public.bien(id_bien),
    reportador_id integer NOT NULL REFERENCES public.usuarios(id_usuario),
    tecnico_id integer REFERENCES public.usuarios(id_usuario),
    fecha_reporte timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    fecha_cierre timestamp without time zone,
    tipo_falla varchar(100) NOT NULL,
    gravedad varchar(20) DEFAULT 'Media',
    descripcion text NOT NULL,
    solucion_aplicada text,
    estado varchar(30) DEFAULT 'Abierta'
);

CREATE TABLE IF NOT EXISTS public.requerimientos (
    id_requerimiento integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    solicitante_id integer NOT NULL REFERENCES public.usuarios(id_usuario),
    tecnico_id integer REFERENCES public.usuarios(id_usuario),
    id_ambiente integer REFERENCES public.ambiente(id_ambiente),
    tipo_servicio varchar(50) NOT NULL,
    prioridad varchar(20) DEFAULT 'Media' NOT NULL,
    descripcion text NOT NULL,
    fecha_solicitud timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    fecha_atencion timestamp without time zone,
    estado varchar(30) DEFAULT 'Pendiente' NOT NULL,
    observacion_resolucion text
);

CREATE TABLE IF NOT EXISTS public.botica_medicamentos (
    id_medicamento integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre varchar(100) NOT NULL,
    presentacion varchar(50) NOT NULL,
    stock_actual integer DEFAULT 0 NOT NULL,
    fecha_vencimiento date NOT NULL,
    estado varchar(20) DEFAULT 'Disponible'
);

CREATE TABLE IF NOT EXISTS public.atenciones_topico (
    id_atencion_top integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    persona_id integer NOT NULL REFERENCES public.personas(id_persona),
    atendido_por_id integer NOT NULL REFERENCES public.usuarios(id_usuario),
    fecha_hora timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    motivo varchar(250) NOT NULL,
    medicamento_id integer REFERENCES public.botica_medicamentos(id_medicamento),
    es_alergico boolean DEFAULT false NOT NULL,
    detalle_alergia varchar(250),
    seguro_salud varchar(20) DEFAULT 'NINGUNO',
    observacion text
);

CREATE TABLE IF NOT EXISTS public.atenciones_psicologia (
    id_atencion_psico integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    persona_id integer NOT NULL REFERENCES public.personas(id_persona),
    psicologo_id integer NOT NULL REFERENCES public.usuarios(id_usuario),
    fecha_hora timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    motivo_consulta varchar(250) NOT NULL,
    observacion_notas text,
    estado_atencion varchar(30) DEFAULT 'Pendiente'
);
