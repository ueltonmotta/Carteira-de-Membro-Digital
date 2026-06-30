# 05 — Modelo de Dados (Banco de Dados)

## Entidades Principais

### `Usuario`

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int (PK) | Identificador único |
| `Nome` | string | Nome completo do membro |
| `Email` | string | E-mail (login) |
| `Senha` | string | Senha criptografada |
| `CPF` | string | Documento de identificação |
| `Perfil` | enum | `Membro`, `Secretaria`, `Pastor` |
| `Status` | enum | `Pendente`, `Ativo`, `Inativo` |
| `DataValidade` | DateTime | Validade da credencial |

> **Nota:** O campo `Status` é o mecanismo central de controle de acesso. Membros com status `Pendente` não acessam a carteirinha completa.

---

### `Evento`

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int (PK) | Identificador único |
| `Titulo` | string | Nome do evento |
| `DataHora` | DateTime | Data e hora do culto/reunião |
| `Tipo` | enum | `Culto`, `Reuniao`, `SantaCeia` |

---

### `Presenca`

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int (PK) | Identificador único |
| `UsuarioId` | int (FK) | Referência ao membro |
| `EventoId` | int (FK) | Referência ao evento |
| `Status` | enum | `Presente`, `Falta` |

> **Nota:** Esta tabela é o resultado do check-in por GPS. A localização é validada no momento do registro, mas apenas o status é persistido.

---

### `Justificativa`

| Campo | Tipo | Descrição |
|---|---|---|
| `Id` | int (PK) | Identificador único |
| `UsuarioId` | int (FK) | Referência ao membro ausente |
| `EventoId` | int (FK) | Referência ao evento perdido |
| `Motivo` | string | Texto explicativo da ausência |

---

## Relacionamentos

```
Usuario (1) ──────< Presenca >────── (1) Evento
Usuario (1) ──────< Justificativa >── (1) Evento
```

## Lógica do Alerta Pastoral

A consulta LINQ que detecta ausências consecutivas cruza as tabelas `Evento` (filtrando por `Tipo == SantaCeia`), `Presenca` e `Justificativa`:

```csharp
// Pseudo-código da lógica de alerta
var ultimasTresCeias = db.Eventos
    .Where(e => e.Tipo == TipoEvento.SantaCeia)
    .OrderByDescending(e => e.DataHora)
    .Take(3)
    .ToList();

var membrosEmAlerta = db.Usuarios
    .Where(u => u.Perfil == Perfil.Membro && u.Status == Status.Ativo)
    .Where(u => ultimasTresCeias.All(ceia =>
        db.Presencas.Any(p =>
            p.UsuarioId == u.Id &&
            p.EventoId == ceia.Id &&
            p.Status == StatusPresenca.Falta) &&
        !db.Justificativas.Any(j =>
            j.UsuarioId == u.Id &&
            j.EventoId == ceia.Id)))
    .ToList();
```

Membros identificados por esta query exibem o ícone pulsante **"Visita Pastoral"** no dashboard da Secretaria.
