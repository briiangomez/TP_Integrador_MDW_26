CREATE EXTENSION IF NOT EXISTS "pgcrypto";

CREATE TABLE audiencias (
    id VARCHAR(100) PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    grupo_origen VARCHAR(50) NOT NULL,
    status VARCHAR(20) NOT NULL,
    creado_en TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE rol_audiencias (
    rol VARCHAR(50) NOT NULL,
    audiencia_id VARCHAR(100) NOT NULL REFERENCES audiencias(id) ON DELETE CASCADE,
    creado_en TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    PRIMARY KEY (rol, audiencia_id)
);

CREATE TABLE clientes_servicio (
    client_id VARCHAR(100) PRIMARY KEY,
    client_secret_hash VARCHAR(255) NOT NULL,
    nombre VARCHAR(150) NOT NULL,
    rol VARCHAR(50) NOT NULL,
    status VARCHAR(20) NOT NULL,
    creado_en TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE usuarios (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(100) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    rol VARCHAR(50) NOT NULL,
    status VARCHAR(20) NOT NULL,
    creado_en TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_rol_audiencias_lookup ON rol_audiencias (rol, audiencia_id);
CREATE INDEX idx_usuarios_lookup ON usuarios (username, status);
CREATE INDEX idx_clientes_servicio_lookup ON clientes_servicio (client_id, status);